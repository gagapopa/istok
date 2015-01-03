using System;
using System.Collections.Generic;
using System.Text;
using gppg;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Контекст компиляции.
    /// Здесь хранится состояние компиляции, 
    /// информация о используемых переменных,
    /// список временных переменных
    /// и стэк операций для организации переходов.
    /// </summary>
    public class CompileContext
    {
        /// <summary>
        /// Формат имён для временных переменных
        /// </summary>
        const String TempVariableFormat = COTES.ISTOK.Calc.CalcContext.TempVariablePrefix + "temp{0}";

        /// <summary>
        /// Контекст расчёта.
        /// Нужен для доступа к функциям и передачи сообщений
        /// </summary>
        public ICalcContext CalcContext { get; protected set; }

        /// <summary>
        /// Компилируемая ревизия.
        /// Вызовы для функций и условных параметры будут компилироваться 
        /// исходя из данной ревизии
        /// </summary>
        public RevisionInfo Revision { get; protected set; }

        /// <summary>
        /// Таблица символов
        /// </summary>
        public SymbolTable SymbolTable { get; protected set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="context">Базовый контекст</param>
        public CompileContext(ICalcContext context,RevisionInfo revision)
        {
            //LabelList = new List<Label>();

            this.Revision = revision;
            this.CalcContext = context;
            this.SymbolTable = new SymbolTable(context);
        }

        /// <summary>
        /// Передаваемые параметры (требуются при компиляции условных параметров)
        /// </summary>
        public KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>[] ArgumentsKey { get; set; }

        /// <summary>
        /// Список созданных временных переменных
        /// </summary>
        Dictionary<String, Variable> tempSymbol = new Dictionary<String, Variable>();

        /// <summary>
        /// Создать новую временную переменную, если требуется
        /// </summary>
        /// <param name="address">Исходный адрес</param>
        /// <returns>Елси address не равен null, возвращается address, иначе новая временная переменная</returns>
        public Address GetTempVariable(Address address)
        {
            //newVariable = false;
            if (address == null)
            {
                lock (tempSymbol)
                {
                    Variable var = null;

                    // поиск освободившейся временной переменной
                    foreach (var item in tempSymbol.Keys)
                        if (!tempSymbol[item].IsAlive)
                        {
                            tempSymbol[item].IsAlive = true;
                            var = tempSymbol[item];
                            break;
                        }
                    // создание новой временной переменной
                    if (var == null)
                    {
                        String name = String.Format(TempVariableFormat, tempSymbol.Count);
                        tempSymbol[name] = var = new Variable(name, SymbolValue.Nothing, false, true);
                        //newVariable = true;
                        SymbolTable.DeclareSymbol(var, false, true);
                    }
                    address = new Address(var.Name);
                }
            }
            return address;
        }

        public IEnumerable<Instruction> TempVariablesDeclaration()
        {
            List<Instruction> declarationList = new List<Instruction>();
            lock (tempSymbol)
            {
                foreach (var item in tempSymbol.Keys)
                {
                    declarationList.Add(new Instruction(null, Instruction.OperationCode.VarDecl, new Address(item)));
                }
            }
            return declarationList;
        }


        /// <summary>
        /// Отметить временные переменные как не используемые
        /// </summary>
        /// <param name="adress">Адрес, который может хранить временную переменную</param>
        public void KillTempVariable(Address adress)
        {
            Variable tempVar;
            if (adress != null && adress.Type == Address.AddressType.Symbol
                && tempSymbol.TryGetValue(adress.SymbolName, out tempVar)
                && tempVar.IsTemp)
                tempVar.IsAlive = false;
        }

        /// <summary>
        /// Отметить временные переменные как не используемые
        /// </summary>
        /// <param name="addresses">Адреса, которые могут хранить временные переменные</param>
        /// <param name="address">
        /// Адрес, который будет использоваться дальше, 
        /// и, если он хранит временную переменную, освобождать её не стоит
        /// </param>
        public void KillTempVariable(IEnumerable<Address> addresses, Address address)
        {
            if (addresses != null)
                foreach (Address item in addresses)
                {
                    Variable tempVar;
                    if (item != address && item.Type == Address.AddressType.Symbol
                        && tempSymbol.TryGetValue(item.SymbolName, out tempVar)
                        && tempVar.IsTemp)
                        tempVar.IsAlive = false;
                }
        }

        #region Стек операций (требуется для break и continue)
        /// <summary>
        /// Информация о компилируемой операции
        /// </summary>
        class OperationState
        {
            /// <summary>
            /// Код операции из дерева разбора
            /// </summary>
            public CalcTree.Operator OperationCode { get; set; }

            /// <summary>
            /// Размер таблицы символов
            /// </summary>
            public int SymbolTableCount { get; set; }

            private List<Instruction> breakList;

            /// <summary>
            /// Коллекция инструкций break, для данной операции
            /// </summary>
            public IEnumerable<Instruction> BreakList
            {
                get
                {
                    return breakList.AsReadOnly();
                }
            }

            private List<Instruction> continueList;

            /// <summary>
            /// Коллекция инструкций continue, для данной операции
            /// </summary>
            public IEnumerable<Instruction> ContinueList
            {
                get
                {
                    return continueList.AsReadOnly();
                }
            }

            public OperationState()
            {
                breakList = new List<Instruction>();

                continueList = new List<Instruction>();
            }

            /// <summary>
            /// Добавить инструкцию break в список.
            /// Инстукция добавляется только для тех операций, 
            /// к которым применим break.
            /// (Например, циклы и switch)
            /// </summary>
            /// <param name="instr">Добавляемая инструкция</param>
            /// <returns>
            /// Если опереция соответствующего типа и инструкция добавлена, возвращает true.
            /// В противном случае - false.
            /// </returns>
            public bool AddBreakInstruction(Instruction instr)
            {
                if (OperationCode == CalcTree.Operator.WhileStatement)
                {
                    breakList.Add(instr);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Добавить инструкцию continue в список.
            /// Инстукция добавляется только для тех операций, 
            /// к которым применим continue.
            /// (Например, циклы)
            /// </summary>
            /// <param name="instr">Добавляемая инструкция</param>
            /// <returns>
            /// Если опереция соответствующего типа и инструкция добавлена, возвращает true.
            /// В противном случае - false.
            /// </returns>
            public bool AddContinueInstruction(Instruction instr)
            {
                if (OperationCode == CalcTree.Operator.WhileStatement)
                {
                    continueList.Add(instr);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Стек операций
        /// </summary>
        Stack<OperationState> operationStack = new Stack<OperationState>();

        /// <summary>
        /// Добавить в стек операций новую операцию
        /// </summary>
        /// <param name="operation">Добавляемая операция</param>
        public void PushOperationState(CalcTree.Operator operation)
        {
            OperationState state = new OperationState()
            {
                OperationCode = operation,
                SymbolTableCount = SymbolTable.GetDeep()
            };

            operationStack.Push(state);
        }

        /// <summary>
        /// Убрать из стека операций верхний элемент
        /// </summary>
        public void PopOperationState()
        {
            operationStack.Pop();
        }

        /// <summary>
        /// Добавить инструкцию break в список 
        /// для ближайшего подходящего элемента в стеке операций.
        /// </summary>
        /// <remarks>
        /// Операция переберает стек, начиная с верхнего элемнта
        /// пока не найдет подходящий.
        /// Если подходящей операции, для которой применим break, 
        /// будет возвращено false.
        /// </remarks>
        /// <param name="instr">Добавляемая инструкция с break</param>
        /// <param name="symbolCountDiff">Насколько таблица символов увеличелась с момента операции, к которой добавлена инструкция break</param>
        /// <returns>
        /// Если найдена подходящая операция и к ней добавлена инструкция, вернуть true.
        /// Если такая инструкция не найдена - вернуть false.
        /// </returns>
        public bool AddBreakInstruction(Instruction instr, out int symbolCountDiff)
        {
            foreach (var item in operationStack)
            {
                if (item.AddBreakInstruction(instr))
                {
                    symbolCountDiff = SymbolTable.GetDeep() - item.SymbolTableCount;
                    return true;
                }
            }
            symbolCountDiff = 0;
            return false;
        }

        /// <summary>
        /// Добавить инструкцию continue в список 
        /// для ближайшего подходящего элемента в стеке операций.
        /// </summary>
        /// <remarks>
        /// Операция переберает стек, начиная с верхнего элемнта
        /// пока не найдет подходящий.
        /// Если подходящей операции, для которой применим continue, 
        /// будет возвращено false.
        /// </remarks>
        /// <param name="instr">Добавляемая инструкция с continue</param>
        /// <param name="symbolCountDiff">Насколько таблица символов увеличелась с момента операции, к которой добавлена инструкция continue</param>
        /// <returns>
        /// Если найдена подходящая операция и к ней добавлена инструкция, вернуть true.
        /// Если такая инструкция не найдена - вернуть false.
        /// </returns>
        public bool AddContinueInstruction(Instruction instr, out int symbolCountDiff)
        {
            foreach (var item in operationStack)
            {
                if (item.AddContinueInstruction(instr))
                {
                    symbolCountDiff = SymbolTable.GetDeep() - item.SymbolTableCount;
                    return true;
                }
            }
            symbolCountDiff = 0;
            return false;
        }

        /// <summary>
        /// Получить список инструкция break для операции с вершины стека операций
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Instruction> GetBreakInstructions()
        {
            return operationStack.Peek().BreakList;
        }

        /// <summary>
        /// Получить список инструкция continue для операции с вершины стека операций
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Instruction> GetContinueInstructions()
        {
            return operationStack.Peek().ContinueList;
        } 
        #endregion
    }
}

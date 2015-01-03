using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using gppg;

namespace COTES.ISTOK.Calc
{
    public class SymbolTable
    {
        class StackScope
        {
            public StackScope()
            {
                Scope = new Dictionary<String, Symbol>();
            }

            public Dictionary<String, Symbol> Scope { get; protected set; }
        }

        class StackFrame : StackScope
        {
            public StackFrame(ICallContext context, StackScope prevScope)
            {
                this.Context = context;

                if (prevScope != null)
                {
                    this.Scope = prevScope.Scope;
                    ScopeReflected = true;
                }
                else
                {
                    ScopeReflected = false;
                }
            }

            public ICallContext Context { get; protected set; }

            public bool ScopeReflected { get; private set; }

            public override bool Equals(object obj)
            {
                StackFrame frame = obj as StackFrame;

                if (frame != null)
                {
                    return Context.Equals(frame.Context);
                }

                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return Context.GetHashCode();
            }
        }

        ICalcContext calcContext;

        /// <summary>
        /// Переменные и функции объявленные глобально
        /// </summary>
        protected Dictionary<String, Symbol> globalScope;

        Dictionary<String, RevisedStorage<Function>> functionTable;

        /// <summary>
        /// Таблица символов текущего контекста
        /// </summary>
        Stack<StackScope> calcStack;

        public SymbolTable(ICalcContext calcContext)
        {
            this.calcContext = calcContext;
            globalScope = new Dictionary<String, Symbol>();
            calcStack = new Stack<StackScope>();
            functionTable = new Dictionary<String, RevisedStorage<Function>>();
        }

        public ICallContext CallContext
        {
            get
            {
                StackFrame frame = GetStackFrame();

                if (frame != null)
                    return frame.Context;

                return null;
            }
        }

        private StackFrame GetStackFrame()
        {
            StackFrame frame;

            foreach (var item in calcStack)
            {
                if ((frame = item as StackFrame) != null)
                    return frame;
            }

            return null;
        }

        private StackScope GetBottomFrame()
        {
            StackFrame searchContext = GetStackFrame();
            StackScope stackScope = searchContext;

            foreach (StackScope curScope in calcStack)
            {
                if (curScope == searchContext)
                    break;
                stackScope = curScope;
            }
            return stackScope;
        }

        /// <summary>
        /// Добавить в таблицу символов дополнительный фрэйм
        /// </summary>
        public void PushSymbolScope()
        {
            calcStack.Push(new StackScope());
        }

        public void PushSymbolScope(ICallContext context, bool isolated)
        {
            StackScope prevScope = isolated || calcStack.Count == 0 ? null : calcStack.Peek();

            calcStack.Push(new StackFrame(context, prevScope));
        }

        public void PushSymbolScope(ICallContext context)
        {
            PushSymbolScope(context, false);
        }

        /// <summary>
        /// Очистить в таблице символов верхний фрэйм
        /// </summary>
        public void PopSymbolScope()
        {
            calcStack.Pop();
        }

        public void PopSymbolScope(ICallContext context)
        {
            StackFrame frame = new StackFrame(context, null);

            if (calcStack.Contains(frame))
                while (!calcStack.Pop().Equals(frame)) ;
        }

        public int GetDeep() {
            return calcStack.Count;
        }

        public int GetDeep(ICallContext context)
        {
            StackFrame frame = new StackFrame(context, null);
            int i = 0;

            if (calcStack.Contains(frame))
            {
                foreach (var item in calcStack)
                {
                    if (item.Equals(context))
                        break;
                    ++i;
                }
            }
            else i = -1;

            return i;
        }

        #region Работа с символами
        /// <summary>
        /// Получить символ из таблицы символов
        /// </summary>
        /// <param name="name">Имя искомого символа</param>
        /// <returns>Символ из таблицы символов или null, если символа с таким именем не найдено</returns>
        public Symbol GetSymbol(String name)
        {
            return GetSymbol(name, false);
        }

        /// <summary>
        /// Получить символ из таблицы символов
        /// </summary>
        /// <param name="name">Имя искомого символа</param>
        /// <param name="skipTopFrame">Пропустить при поиске символа верхний фрэйм</param>
        /// <returns>Символ из таблицы символов или null, если символа с таким именем не найдено</returns>
        public Symbol GetSymbol(String name, bool skipTopFrame)
        {
            Symbol symbol = null;

            foreach (var item in calcStack)
            {
                if (skipTopFrame)
                {
                    skipTopFrame = false;
                }
                else if (item.Scope.TryGetValue(name, out symbol))
                {
                    break;
                }
                if (item is StackFrame) break;
            }

            if (symbol == null)
                globalScope.TryGetValue(name, out symbol);

            return symbol;
        }

        /// <summary>
        /// Получить символ из верхнего фрэйма таблицы символов
        /// </summary>
        /// <param name="name">Имя искомого символа</param>
        /// <returns>Символ из таблицы символов или null, если символа с таким именем не найдено</returns>
        public Symbol GetSupSymbol(String name)
        {
            Symbol symbol;
            StackScope scope = calcStack.Peek();
            scope.Scope.TryGetValue(name, out symbol);
            return symbol;
        }

        /// <summary>
        /// Получить символ из нижней секции текущего стэкового фрэйма таблицы символов
        /// </summary>
        /// <param name="name">Имя искомого символа</param>
        /// <returns>Символ из таблицы символов или null, если символа с таким именем не найдено</returns>
        public Symbol GetStackFrameSymbol(String name)
        {
            Symbol symbol;
            StackFrame frame = GetStackFrame();

            if (frame == null || !frame.Scope.TryGetValue(name, out symbol))
                symbol = null;

            return symbol;
        }

        /// <summary>
        /// Добавить символ в таблицу символов
        /// </summary>
        /// <param name="symbol">Добавляемый символ</param>
        /// <param name="isGlobal">Добавить в глобальной область или в верхний фрэйм таблицы символов</param>
        /// <returns>Добавленый символ, null, в случае дублирвания символа</returns>
        public Symbol DeclareSymbol(Symbol symbol, bool isGlobal, bool isBottom)
        {
            Dictionary<String, Symbol> scope;
            StackScope stackScope;
            String name = symbol.Name;

            if (isGlobal || calcStack.Count == 0)
                scope = globalScope;
            else if (isBottom)
            {
                stackScope = GetBottomFrame();

                scope = stackScope.Scope;
            }
            else scope = calcStack.Peek().Scope;

            if (scope.ContainsKey(name))
            {
                calcContext.AddMessage(new CalcMessage(MessageCategory.Error, "Дублированное объявление символа '{0}'", name));
                return null;
            }
            else scope.Add(name, symbol);

            return symbol;
        }

        /// <summary>
        /// Добавить символ в таблицу символов в локальную область
        /// </summary>
        /// <param name="symbol">Добавляемый символ</param>
        /// <returns>Добавленый символ, null, в случае дублирвания символа</returns>
        public Symbol DeclareSymbol(Symbol symbol)
        {
            return DeclareSymbol(symbol, false, false);
        }

        /// <summary>
        /// Добавить символы в таблицу символов
        /// </summary>
        /// <param name="symbolList">Добавляемые символы</param>
        /// <param name="isGlobal">Добавить в глобальной область или в верхний фрэйм таблицы символов</param>
        public void DeclareSymbol(IEnumerable<Symbol> symbolList, bool isGlobal)
        {
            foreach (var symbol in symbolList)
            {
                DeclareSymbol(symbol, isGlobal, false);
            }
        }

        /// <summary>
        /// Добавить символы в таблицу символов в локальную область
        /// </summary>
        /// <param name="symbolList">Добавляемые символы</param>
        public void DeclareSymbol(IEnumerable<Symbol> symbolList)
        {
            DeclareSymbol(symbolList, false);
        }
        #endregion

        #region Работа с функциями
        public void DeclareFunction(Function function)
        {
            RevisedStorage<Function> functionStorage;

            if (!functionTable.TryGetValue(function.Name, out functionStorage))
                functionTable[function.Name] = functionStorage = new RevisedStorage<Function>();

            functionStorage.Set(function.Revision, function);
        }

        public void DeclareFunction(IEnumerable<Symbol> functionList)
        {
            foreach (var symbol in functionList)
            {
                DeclareFunction(symbol as Function);
            }
        }

        /// <summary>
        /// Получить функцию из таблицы символов
        /// </summary>
        /// <param name="name">Имя функции</param>
        /// <returns>Функция или null, если функция в таблице символов не найдена</returns>
        public Function GetFunction(RevisionInfo revision, String name)
        {
            RevisedStorage<Function> functionStorage;
            Function function = null;

            if (functionTable.TryGetValue(name, out functionStorage))
            {
                function = functionStorage.Get(revision);
            }

            if (function == null)
                function = GetLocalFunction(name);

            return function;
        }

        public Function GetFunction(DateTime time, string name)
        {
            RevisionInfo revision = calcContext.GetRevision(time);

            return GetFunction(revision, name);
        }

        private Function GetLocalFunction(string name)
        {
            Symbol symbol = null;
            Function func = null;

            foreach (var frame in calcStack)
            {
                if (frame.Scope.TryGetValue(name, out symbol)
                    && (func = symbol as Function) != null)
                    break;
                if (frame is StackFrame)
                    break;
            }

            if (func == null)
            {
                globalScope.TryGetValue(name, out symbol);
                func = symbol as Function;
            }

            return func;
        }
        #endregion

        /// <summary>
        /// Получить список всех глобальных функций
        /// </summary>
        /// <returns></returns>
        public List<FunctionInfo> GetAllFunction(RevisionInfo revision)
        {
            Function func;

            List<FunctionInfo> functionList = new List<FunctionInfo>();
            foreach (Symbol symbol in globalScope.Values)
            {
                if ((func = symbol as Function) != null) functionList.Add(new FunctionInfo(func));
            }

            foreach (var funcName in functionTable.Keys)
            {
                RevisedStorage<Function> storage = functionTable[funcName];

                func = storage.Get(revision);

                if (func != null)
                {
                    functionList.Add(new FunctionInfo(func));
                }
            }
            return functionList;
        }

        public String SymbolTableReport()
        {
            StringBuilder reportBuilder = new StringBuilder();
            int contextNum = 0;

            foreach (var item in calcStack)
            {
                bool printScopeContent = true;
                StackFrame frame = item as StackFrame;


                if (frame != null)
                {
                    printScopeContent = !frame.ScopeReflected;
                    reportBuilder.AppendFormat("\nstack frame of {0} ({1}) context[", ++contextNum, frame.Context);
                    if (!printScopeContent)
                        reportBuilder.Append("stack frame reflect previous scope]");
                }
                else
                    reportBuilder.Append("\nscope[");

                if (printScopeContent)
                {
                    foreach (var symbolName in item.Scope.Keys)
                    {
                        reportBuilder.Append("\n");
                        reportBuilder.Append(item.Scope[symbolName]);
                    }
                    reportBuilder.Append("\n]"); 
                }
            }
            return reportBuilder.ToString();
        }

        public IEnumerable<ICallContext> GetAllContext()
        {
            return from f in calcStack where f is StackFrame select (f as StackFrame).Context;
        }

        public IEnumerable<Symbol> GetAllGlobalSymbol()
        {
            return globalScope.Values.Where(s => s is Variable).ToArray();
        }
    }
}

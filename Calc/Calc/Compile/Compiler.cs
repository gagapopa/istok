using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Компилятор
    /// </summary>
    public class Compiler : ICompiler
    {
        /// <summary>
        /// Имя функции агрегации по умолчанию
        /// </summary>
        const String DefaultAggregationFunctionName = "first";

        /// <summary>
        /// Парсер. Строит по коду дерево разбора
        /// </summary>
        TepParser parser;

        public Compiler()
        {
            parser = new TepParser();
            parser.initAliasses();
        }

        /// <summary>
        /// Объект синхронизации для обращения к парсеру.
        /// Парсер несколько болезнено относиться к многопоточности.
        /// </summary>
        static Object parserSyncObject = new Object();

        public Instruction[] Compile(ICalcContext context,
                                     RevisionInfo revision,
                                     String formulaText,
                                     KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>[] arguments)
        {
            bool fail = false;
            CompileContext compileContext;
            CalcTree tree;
            List<Message> messages;
            tree = Parse(formulaText, out messages);

            if (messages != null)
            {
                CalcMessage calcMessage;
                foreach (Message parseMessage in messages)
                {
                    if ((calcMessage = parseMessage as CalcMessage) != null)
                        calcMessage.Position.Merge(context.GetIdentifier());
                    if ((int)parseMessage.Category >= (int)MessageCategory.Error)
                        fail = true;
                }
                context.AddMessage(messages);
            }
            if (fail)
                return null;
            if (tree != null)
            {
                Instruction retInstruction;
                List<Instruction> code;
                Address paramValue = null;

                compileContext = new CompileContext(context, revision);

                var symbols = context.SymbolTable.GetAllGlobalSymbol();

                foreach (var item in symbols)
                {
                    compileContext.SymbolTable.DeclareSymbol(item, true, false);
                }

                compileContext.SymbolTable.PopSymbolScope(null);
                tree.MustHaveReturn = true;

                // добавляем в контекс информацию о аргументах условных параметров и функций
                compileContext.ArgumentsKey = arguments;

                // передача аргументов при компиляции
                if(arguments!=null)
                    foreach (var argsLayer in arguments)
                    {
                        foreach (var item in argsLayer.Value)
                        {
                            compileContext.SymbolTable.DeclareSymbol(new Variable(item.Name));
                        }
                    }

                CodePart codePart = Compile(compileContext, tree, null, true);

                if (codePart.Fail)
                {
                    return null;
                }

                code = new List<Instruction>();

                code.AddRange(codePart.Code);
                if (code.Count > 0 && code[0].Operation == Instruction.OperationCode.EnterLevel)
                    code.InsertRange(1, compileContext.TempVariablesDeclaration());
                paramValue = codePart.Result;

                Location locat;
                if (code.Count > 0) locat = code[code.Count - 1].Location;
                else locat = null;
                code.Add(retInstruction = new Instruction(locat, Instruction.OperationCode.Return));

                return code.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Производит разбор в формуле параметра на дерево разбора
        /// </summary>
        /// <param name="text">Разбираемая формула</param>
        /// <param name="parseMess">Список сообщений, полученных во время разбора</param>
        /// <returns>Дерево разбора</returns>
        /// <remarks>Выполняется в одном потоке, что может замедлить процесс компиляции для многопоточного расчета</remarks>
        private CalcTree Parse(String text, out List<Message> parseMess)
        {
            if (String.IsNullOrEmpty(text))
            {
                parseMess = null;
                return null;
            }

            lock (parserSyncObject)
            {
                CalcTree tree = null;
                Stream codeStream = null;
                bool ret = false;
                bool stop = false;

                codeStream = new MemoryStream(Encoding.UTF8.GetBytes(text));

                if (parser.scanner == null)
                    parser.scanner = new Scanner(codeStream, "utf-8");
                else
                    ((Scanner)parser.scanner).SetSource(codeStream, Encoding.UTF8.CodePage);
                ((Scanner)parser.scanner).parseMessages.Clear();
                while (!stop)
                {
                    try
                    {
                        ret = parser.Parse();
                        stop = codeStream.Length <= (((Scanner)parser.scanner).buffer.ReadPos + 1);
                    }
                    catch (Exception exc)
                    {
                        parser.scanner.yyerror(exc.Message);
                    }
                }
                tree = parser.RetValue;
                parseMess = ((Scanner)parser.scanner).parseMessages;
                return tree;
            }
        }

        /// <summary>
        /// Общее, ничего не говорящие сообщение об ошибке
        /// </summary>
        private const String CompilationErrorMessage = "Ошибка компиляции";

        /// <summary>
        /// Рекурсивная функция компиляции текущего узла дерева разбора в трехадресный код
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="result">
        /// Адрес, который следует использовать как результат для компилируемого узла. 
        /// Применяется для упрощения операций присваивания
        /// </param>
        /// <returns>Скомпилированный код для данного узла</returns>
        private CodePart Compile(CompileContext context, CalcTree currentNode, Address result, bool resolveParameter)
        {
            Function func;
            Variable var;
            BranchesInfo info = null;

            CodePart retPart = new CodePart();

            retPart.Result = result;

            context.PushOperationState(currentNode.CalcOperator);

            switch (currentNode.CalcOperator)
            {
                case CalcTree.Operator.DoubleValue:
                    retPart.Result = new Address(new DoubleValue(currentNode.DoubleValue));
                    break;
                case CalcTree.Operator.StringValue:
                    retPart.Result = new Address(new StringValue(currentNode.StringValue));
                    break;
                case CalcTree.Operator.Variable:
                    retPart.Result = new Address(currentNode.StringValue);
                    break;
                case CalcTree.Operator.VariableDeclaration:
                    info = CompileBranches(context, currentNode, 0, 1, false, false, CompilationErrorMessage);
                    if (info != null)
                    {
                        context.SymbolTable.DeclareSymbol(var = new Variable(currentNode.StringValue));
                        if (info.Parts.Length == 1)
                        {
                            retPart.Code.AddRange(info.Parts[0].Code);
                            //context.SymbolTable.DeclareSymbol(var = new Variable(currentNode.StringValue));
                            retPart.Code.Add(new Instruction(currentNode.Location, Instruction.OperationCode.VarDecl, retPart.Result = new Address(var.Name), info.Parts[0].Result));
                        }
                        else
                        {
                            //context.SymbolTable.DeclareSymbol(var = new Variable(currentNode.StringValue));
                            retPart.Code.Add(new Instruction(currentNode.Location, Instruction.OperationCode.VarDecl, retPart.Result = new Address(var.Name)));
                        }
                    }
                    else
                    {
                        retPart.Fail = true;
                    }
                    break;
                case CalcTree.Operator.Parameter:
                    info = CompileParameter(context, currentNode, retPart);
                    break;
                case CalcTree.Operator.Call:
                    String name = currentNode.StringValue;
                    if ((func = context.CalcContext.SymbolTable.GetFunction(context.Revision, name)) != null)
                    {
                        retPart.Add(CompileFunctionCall(context, currentNode, func));
                    }
                    else
                    {
                        retPart.Fail = true;
                        CalcMessage msg = new CalcMessage(MessageCategory.Error, currentNode.Location, "функция '{0}' не определена", name);
                        context.CalcContext.AddMessage(msg);
                    }
                    break;
                case CalcTree.Operator.RetStatement:
                    info = CompileBranches(context, currentNode, 0, 1, false, false, CompilationErrorMessage);
                    if (info != null && info.Parts.Length > 0)
                    {
                        retPart.Code.AddRange(info.Parts[0].Code);
                        retPart.Code.Add(new Instruction(currentNode.Location, Instruction.OperationCode.Move, new Address(CalcContext.ReturnVariableName), info.Parts[0].Result));
                    }
                    retPart.Code.Add(new Instruction(currentNode.Location, Instruction.OperationCode.Return));
                    break;
                case CalcTree.Operator.Addition:
                    BinaryOperation(context, currentNode, retPart, Instruction.OperationCode.Addition, (v1, v2) => v1.Addition(context.CalcContext, v2));
                    break;
                case CalcTree.Operator.Subtraction:
                    BinaryOperation(context, currentNode, retPart, Instruction.OperationCode.Subtraction, (v1, v2) => v1.Subtraction(context.CalcContext, v2));
                    break;
                case CalcTree.Operator.Power:
                    const String powerFunction = "pow";
                    if ((func = context.CalcContext.SymbolTable.GetFunction(context.Revision, powerFunction)) != null)
                    {
                        retPart.Add(CompileFunctionCall(context, currentNode, func));
                    }
                    else
                    {
                        retPart.Fail = true;
                        CalcMessage msg = new CalcMessage(MessageCategory.Error, currentNode.Location, "функция '{0}' не определена", powerFunction);
                        context.CalcContext.AddMessage(msg);
                    }
                    break;
                case CalcTree.Operator.Multiplication:
                    BinaryOperation(context, currentNode, retPart, Instruction.OperationCode.Multiplication, (v1, v2) => v1.Multiplication(context.CalcContext, v2));
                    break;
                case CalcTree.Operator.Division:
                    BinaryOperation(context, currentNode, retPart, Instruction.OperationCode.Division, (v1, v2) => v1.Division(context.CalcContext, v2));
                    break;
                case CalcTree.Operator.Modulo:
                    BinaryOperation(context, currentNode, retPart, Instruction.OperationCode.Modulo, (v1, v2) => v1.Modulo(context.CalcContext, v2));
                    break;
                case CalcTree.Operator.Equal:
                    BinaryOperation(context, currentNode, retPart, Instruction.OperationCode.Equal, (v1, v2) => v1.Equal(context.CalcContext, v2));
                    break;
                case CalcTree.Operator.NotEqual:
                    BinaryOperation(context, currentNode, retPart, Instruction.OperationCode.NotEqual, (v1, v2) => v1.NotEqual(context.CalcContext, v2));
                    break;
                case CalcTree.Operator.Greater:
                    BinaryOperation(context, currentNode, retPart, Instruction.OperationCode.Greater, (v1, v2) => v1.Greater(context.CalcContext, v2));
                    break;
                case CalcTree.Operator.Less:
                    BinaryOperation(context, currentNode, retPart, Instruction.OperationCode.Less, (v1, v2) => v1.Less(context.CalcContext, v2));
                    break;
                case CalcTree.Operator.GreaterOrEqual:
                    BinaryOperation(context, currentNode, retPart, Instruction.OperationCode.GreaterOrEqual, (v1, v2) => v1.GreaterOrEqual(context.CalcContext, v2));
                    break;
                case CalcTree.Operator.LessOrEqual:
                    BinaryOperation(context, currentNode, retPart, Instruction.OperationCode.LessOrEqual, (v1, v2) => v1.LessOrEqual(context.CalcContext, v2));
                    break;
                case CalcTree.Operator.LogicalNot:
                    UnaryOperation(context, currentNode, retPart, Instruction.OperationCode.LogicalNot, v => v.IsZero(context.CalcContext) ? SymbolValue.TrueValue : SymbolValue.FalseValue);
                    break;
                case CalcTree.Operator.LogicalAnd:
                    BinaryLogicalOperation(context, currentNode, retPart, Instruction.OperationCode.JumpZ, v => v.IsZero(context.CalcContext));
                    break;
                case CalcTree.Operator.LogicalOr:
                    BinaryLogicalOperation(context, currentNode, retPart, Instruction.OperationCode.JumpNZ, v => !v.IsZero(context.CalcContext));
                    break;
                case CalcTree.Operator.UnaryPlus:
                    info = CompileBranches(context, currentNode, 1, false, false, CompilationErrorMessage);
                    if (info != null)
                    {
                        retPart = info.Parts[0];
                    }
                    break;
                case CalcTree.Operator.UnaryMinus:
                    UnaryOperation(context, currentNode, retPart, Instruction.OperationCode.UnaryMinus, v => v.UnaryMinus(context.CalcContext));
                    break;
                case CalcTree.Operator.IncrementPrefix:
                    UnaryAssignment(context, currentNode, retPart, Instruction.OperationCode.IncrementPrefix);
                    break;
                case CalcTree.Operator.IncrementSuffix:
                    UnaryAssignment(context, currentNode, retPart, Instruction.OperationCode.IncrementSuffix);
                    break;
                case CalcTree.Operator.DecrementPrefix:
                    UnaryAssignment(context, currentNode, retPart, Instruction.OperationCode.DecrementPrefix);
                    break;
                case CalcTree.Operator.DecrementSuffix:
                    UnaryAssignment(context, currentNode, retPart, Instruction.OperationCode.DecrementSuffix);
                    break;
                case CalcTree.Operator.Assignment:
                    CompileAssignment(context, currentNode, retPart);
                    break;
                case CalcTree.Operator.AdditionAssignment:
                    BinaryAssignment(context, currentNode, retPart, Instruction.OperationCode.Addition);
                    break;
                case CalcTree.Operator.SubtractionAssignment:
                    BinaryAssignment(context, currentNode, retPart, Instruction.OperationCode.Subtraction);
                    break;
                case CalcTree.Operator.MultiplicationAssignment:
                    BinaryAssignment(context, currentNode, retPart, Instruction.OperationCode.Multiplication);
                    break;
                case CalcTree.Operator.DivisionAssignment:
                    BinaryAssignment(context, currentNode, retPart, Instruction.OperationCode.Division);
                    break;
                case CalcTree.Operator.ModuloAssignment:
                    BinaryAssignment(context, currentNode, retPart, Instruction.OperationCode.Modulo);
                    break;
                case CalcTree.Operator.ArrayValue:
                    info = CompileArrayValue(context, currentNode, retPart);
                    break;
                case CalcTree.Operator.ArrayAccessor:
                    info = CompileBranches(context, currentNode, 2, false, false, CompilationErrorMessage);
                    if (info != null)
                    {
                        retPart.Code.AddRange(info.Parts[0].Code);
                        retPart.Code.AddRange(info.Parts[1].Code);
                        retPart.Result = new Address(info.Parts[0].Result, info.Parts[1].Result);
                    }
                    break;
                case CalcTree.Operator.Comma:
                case CalcTree.Operator.CompositeStatement:
                    if (currentNode.Branches.Count > 0)
                    {
                        Instruction exitInstruction;
                        if (currentNode.MustHaveReturn) { currentNode.Branches[currentNode.Branches.Count - 1].MustHaveReturn = true; currentNode.MustHaveReturn = false; }
                        retPart.Code.Add(new Instruction(currentNode.Location, Instruction.OperationCode.EnterLevel));
                        context.SymbolTable.PushSymbolScope();
                        foreach (CalcTree branchTree in currentNode.Branches)
                        {
                            CodePart codePart = Compile(context, branchTree, null, true);
                            //retPart.Code.AddRange(codePart.Code);
                            retPart.Add(codePart);
                            context.KillTempVariable(retPart.Result);// = codePart.Result);
                        }
                        context.SymbolTable.PopSymbolScope();
                        retPart.Code.Add(exitInstruction = new Instruction(currentNode.Location, Instruction.OperationCode.LeaveLevel));
                    }
                    break;
                case CalcTree.Operator.IfStatement:
                    CompileIf(context, currentNode, retPart);
                    break;
                case CalcTree.Operator.WhileStatement:
                    CompileWhile(context, currentNode, retPart);
                    break;
                case CalcTree.Operator.EmptyStatement:
                    retPart.Code.Add(new Instruction(currentNode.Location, Instruction.OperationCode.NOP));
                    break;
                case CalcTree.Operator.BreakStatement:
                    CompileBreakContinue(context, currentNode, retPart, true);
                    break;
                case CalcTree.Operator.ContinueStatement:
                    CompileBreakContinue(context, currentNode, retPart, false);
                    break;
                default:
                    break;
            }

            if (info != null)
            {
                KillTempVariables(context, retPart, info.Parts);
            }

            if (retPart.Result != null && retPart.Result.Type == Address.AddressType.Parameter
                && (currentNode.MustHaveReturn || resolveParameter))
            {
                CodePart parameterPart = retPart;
                retPart = CalcDefaultAggregationParameter(context, currentNode.Location, parameterPart);
                KillTempVariables(context, retPart, new CodePart[] { parameterPart });
            }
            if (currentNode.MustHaveReturn && retPart.Result != null)
            {
                if (retPart.Result.Type == Address.AddressType.Symbol || retPart.Result.Type == Address.AddressType.Value)
                    retPart.Code.Add(new Instruction(currentNode.Location, Instruction.OperationCode.Move, new Address(CalcContext.ReturnVariableName), retPart.Result));
            }

            context.PopOperationState();
            return retPart;
        }

        /// <summary>
        /// Скомпилировать все дочернии узлы текущего узла дерева разбора
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="minargs">Минимальное количество требуемых аргументов</param>
        /// <param name="maxargs">Максимальное количество требуемых аргументов</param>
        /// <param name="isAssign">Первый аргумент должен быть переменной или элементом массива</param>
        /// <param name="forwardResult">Передавать ли адресс первого операнда второму. Что бы туда сразу сохранялся результат второго операнда</param>
        /// <param name="onErrorMessage">Текст сообщения, если количество аргументов не соответствует требуемому</param>
        /// <param name="onErrorMessageParams">Параметры для текста сообщения</param>
        /// <returns></returns>
        private BranchesInfo CompileBranches(CompileContext context, CalcTree currentNode, int minargs, int maxargs, bool isAssign, bool forwardResult, String onErrorMessage, params Object[] onErrorMessageParams)
        {
            bool fail = false;
            bool isPar, isVal;
            BranchesInfo parts = new BranchesInfo();

            parts.Parts = new CodePart[currentNode.Branches.Count];
            parts.IsAllValues = true;

            Address result = null;

            for (int i = 0; i < parts.Parts.Length; i++)
            {
                parts.Parts[i] = Compile(context, currentNode.Branches[i], result, false);

                if (forwardResult && i == 0)
                    result = parts.Parts[i].Result;
                else
                    result = null;

                // при ошибке на нижнем уровне вернуть null
                if (parts.Parts[i].Result == null)
                {
                    fail = true;
                }
                else
                {
                    isPar = parts.Parts[i].Parameter != null;
                    isVal = parts.Parts[i].Result.Type == Address.AddressType.Value;

                    CheckAddress(context, currentNode.Branches[i].Location, parts.Parts[i].Result, isAssign && i == 0, isAssign && i == 0);

                    parts.IsAllValues = parts.IsAllValues && isVal;

                    if (isPar)
                    {
                        CodePart parameterPart = parts.Parts[i];
                        parts.Parts[i] = CalcDefaultAggregationParameter(context, currentNode.Location, parameterPart);
                        KillTempVariables(context, parts.Parts[i], new CodePart[] { parameterPart });
                    }
                }
            }

            if (fail)
                return null;

            if (parts.Parts.Length < minargs || parts.Parts.Length > maxargs)
                context.CalcContext.AddMessage(new CalcMessage(MessageCategory.Error, currentNode.Location, onErrorMessage, onErrorMessageParams));

            return parts;
        }

        /// <summary>
        /// Скомпилировать все дочернии узлы текущего узла дерева разбора 
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="argc">Требуемое количество аргументов</param>
        /// <param name="isAssign">Первый аргумент должен быть переменной или элементом массива</param>
        /// <param name="forwardResult">Передавать ли адресс первого операнда второму. Что бы туда сразу сохранялся результат второго операнда</param>
        /// <param name="onErrorMessage">Текст сообщения, если количество аргументов не соответствует требуемому</param>
        /// <param name="onErrorMessageParams">Параметры для текста сообщения</param>
        /// <returns></returns>
        private BranchesInfo CompileBranches(CompileContext context, CalcTree currentNode, int argc, bool isAssign,bool forwardResult, String onErrorMessage, params Object[] onErrorMessageParams)
        {
            return CompileBranches(context, currentNode, argc, argc, isAssign, forwardResult, onErrorMessage, onErrorMessageParams);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calcArgumentInfo"></param>
        /// <returns></returns>
        private object ArgumentsToString(CalcArgumentInfo[] calcArgumentInfo)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("(");
            for (int i = 0; i < calcArgumentInfo.Length; i++)
            {
                if (i > 0) builder.Append(", ");
                builder.Append(calcArgumentInfo[i].Name);
                if (!String.IsNullOrEmpty(calcArgumentInfo[i].DefaultValue))
                    builder.AppendFormat("={0}", calcArgumentInfo[i].DefaultValue);
            }
            builder.Append(")");

            return builder.ToString();
        }

        /// <summary>
        /// Проверить адрес на соответсвие требованиям
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="location">Положение в тексте, для сообщений об ошибке</param>
        /// <param name="address">Проверяемый адрес</param>
        /// <param name="isAssign">Адрем должен быть переменной или элементом массива</param>
        /// <param name="implicitDeclare">Разрешено неявное объявление переменной</param>
        private void CheckAddress(CompileContext context, Location location, Address address, bool isAssign, bool implicitDeclare)
        {
            Variable var;
            if (address.Type == Address.AddressType.Symbol)
            {
                var = context.SymbolTable.GetSymbol(address.SymbolName) as Variable;
                if (var == null)
                {
                    if (implicitDeclare)
                        var = context.SymbolTable.DeclareSymbol(new Variable(address.SymbolName), false, true) as Variable;
                    else
                        context.CalcContext.AddMessage(new CalcMessage(MessageCategory.Error, location, "Использование необъявленной переменной '{0}'", address.SymbolName));
                }
                else if (isAssign)
                {
                    if (var is Parameter && (var as Parameter).ParameterAccessor == ParameterAccessor.In)
                        context.CalcContext.AddMessage(new CalcMessage(MessageCategory.Error, location, String.Format("Значение входного параметра '{0}' нельзя изменять", var.Name)));
                    else if (var.IsConst)
                        context.CalcContext.AddMessage(new CalcMessage(MessageCategory.Error, location, String.Format("Значение константы '{0}' не может быть изменено", var.Name)));
                    else if (var.IsTemp)
                        context.CalcContext.AddMessage(new CalcMessage(MessageCategory.Error, location, "Операция присваивания должна производиться над переменными"));
                }
            }
            else if (address.Type == Address.AddressType.ArrayElement)
            {
                CheckAddress(context, location, address.ArrayAddress, isAssign, false);
            }
            else if (isAssign)
                context.CalcContext.AddMessage(new CalcMessage(MessageCategory.Error, location, "Операция присваивания должна производиться над переменными"));
        }

        /// <summary>
        /// Освободить временные переменные, которые больше не нужны.
        /// Освобожденные переменные будут повторно использоваться в будущем.
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="retPart">Текущая часть кода, если её результатом является временная переменная, освобождать её не надо</param>
        /// <param name="parts">Части кода, результаты которых можно освобождать</param>
        private void KillTempVariables(CompileContext context, CodePart retPart, IEnumerable<CodePart> parts)
        {
            foreach (var item in parts)
            {
                if (retPart == null || retPart.Result == null || !retPart.Result.Equals(item.Result))
                    context.KillTempVariable(item.Result);
                if (item.ParameterArguments != null)
                {
                    foreach (var address in item.ParameterArguments)
                    {
                        if (retPart == null || retPart.Result == null || !retPart.Result.Equals(address))
                            context.KillTempVariable(address);
                    }
                }
            }
        }

        /// <summary>
        /// Скомпилировать объявление массива
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="retPart">Результирующая часть кода</param>
        /// <returns></returns>
        private BranchesInfo CompileArrayValue(CompileContext context, CalcTree currentNode, CodePart retPart)
        {
            int arrayLength = currentNode.Branches.Count;
            BranchesInfo info = CompileBranches(context, currentNode, arrayLength, false, false, CompilationErrorMessage);
            if (info != null)
            {
                if (info.IsAllValues)
                {
                    List<SymbolValue> arrayValues = new List<SymbolValue>();
                    foreach (CodePart codePart in info.Parts)
                        if (codePart.Result.Type == Address.AddressType.Value)
                            arrayValues.Add(codePart.Result.Value);
                    retPart.Result = new Address(new ArrayValue(context.CalcContext, arrayValues.ToArray()));
                }
                else
                {
                    retPart.Code.Add(new Instruction(
                        currentNode.Location,
                        Instruction.OperationCode.Move,
                        retPart.Result = context.GetTempVariable(retPart.Result),
                        new Address(new ArrayValue(context.CalcContext, arrayLength))));

                    Address counter = context.GetTempVariable(null), tempVariable = context.GetTempVariable(null);
                    retPart.Code.Add(new Instruction(currentNode.Location, Instruction.OperationCode.Move, counter, new Address(new DoubleValue(0))));

                    foreach (CodePart codePart in info.Parts)
                    {
                        retPart.Code.AddRange(codePart.Code);
                        retPart.Code.Add(new Instruction(
                            currentNode.Location,
                            Instruction.OperationCode.Move,
                            new Address(retPart.Result, counter),
                            codePart.Result));
                        retPart.Code.Add(new Instruction(
                            currentNode.Location,
                            Instruction.OperationCode.IncrementPrefix,
                            counter,
                            counter));
                    }
                }
            }
            return info;
        }

        /// <summary>
        /// Скомпилировать присваивание
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="retPart">Результирующая часть кода</param>
        private void CompileAssignment(CompileContext context, CalcTree currentNode, CodePart retPart)
        {
            BranchesInfo branches = CompileBranches(context, currentNode, 2, true, true, CompilationErrorMessage);

            CodePart assignPart = branches.Parts[0];

            retPart.Add(assignPart);

            CodePart expressionPart= branches.Parts[1];

            retPart.Add(expressionPart);

            KillTempVariables(context, null, new CodePart[] { assignPart, expressionPart });
            // добавляем инструкцию move, если требуется
            if (assignPart.Result != retPart.Result)
            {
                retPart.Code.Add(new Instruction(
                     currentNode.Location,
                     Instruction.OperationCode.Move,
                     assignPart.Result,
                     retPart.Result));
                retPart.Result = assignPart.Result;
            }
        }

        /// <summary>
        /// Скомпилировать операцию унарного присваивания (инкремент, дикремент)
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="retPart">Результирующая часть кода</param>
        /// <param name="operation">Код инструкии</param>
        private void UnaryAssignment(CompileContext context, CalcTree currentNode, CodePart retPart, Instruction.OperationCode operation)
        {
            BranchesInfo info = CompileBranches(context, currentNode, 1, true, true, CompilationErrorMessage);
            if (info != null)
            {
                retPart.Code.AddRange(info.Parts[0].Code);

                KillTempVariables(context, null, info.Parts);
                retPart.Code.Add(new Instruction(
                    currentNode.Location,
                    operation,
                    retPart.Result = context.GetTempVariable(retPart.Result),//, out newVariable),
                    info.Parts[0].Result));
            }
        }

        /// <summary>
        /// Скомпилирвать унарную операцию
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="retPart">Результирующая часть кода</param>
        /// <param name="operation">Код инструкии</param>
        /// <param name="onValueFunc">Функция для упращения, если выражение является значением</param>
        private void UnaryOperation(CompileContext context, CalcTree currentNode, CodePart retPart, Instruction.OperationCode operation, Func<SymbolValue, SymbolValue> onValueFunc)
        {
            BranchesInfo info = CompileBranches(context, currentNode, 1, false, false, CompilationErrorMessage);
            //if (info != null)
            if (info==null)
            {
                retPart.Fail = true;
            }
            else
            {
                if (info.IsAllValues)
                {
                    retPart.Result = new Address(onValueFunc(info.Parts[0].Result.Value));
                }
                else
                {
                    retPart.Code.AddRange(info.Parts[0].Code);
                    KillTempVariables(context, null, info.Parts);
                    retPart.Code.Add(new Instruction(
                        currentNode.Location,
                        operation,
                        retPart.Result = context.GetTempVariable(retPart.Result),//, out newVariable),
                        info.Parts[0].Result));
                }
            }
        }

        /// <summary>
        /// Скомпилировать бинарное операцию с присваиванием (x op= y)
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="retPart">Результирующая часть кода</param>
        /// <param name="operation">Код инструкии (без присваивания)</param>
        private void BinaryAssignment(CompileContext context, CalcTree currentNode, CodePart retPart, Instruction.OperationCode operation)
        {
            BranchesInfo info = CompileBranches(context, currentNode, 2, true,false, CompilationErrorMessage);
            if (info != null)
            {
                retPart.Code.AddRange(info.Parts[0].Code);
                retPart.Code.AddRange(info.Parts[1].Code);
                KillTempVariables(context, null, info.Parts);
                retPart.Code.Add(new Instruction(
                    currentNode.Location,
                    operation,
                    retPart.Result = info.Parts[0].Result,
                    retPart.Result,
                    info.Parts[1].Result));
            }
        }

        /// <summary>
        /// Скомпилирвать бинарную операцию
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="retPart">Результирующая часть кода</param>
        /// <param name="operation">Код инструкии</param>
        /// <param name="onValueFunc">Функция для упращения, если оба аргумента является значениями</param>
        private void BinaryOperation(CompileContext context, CalcTree currentNode, CodePart retPart, Instruction.OperationCode operation, Func<SymbolValue, SymbolValue, SymbolValue> onValueFunc)
        {
            BranchesInfo info = CompileBranches(context, currentNode, 2, false, false, CompilationErrorMessage);
            //if (info != null)
            if (info == null)
            {
                retPart.Fail = true;
            }
            else
            {
                if (info.IsAllValues)
                {
                    retPart.Result = new Address(onValueFunc(info.Parts[0].Result.Value, info.Parts[1].Result.Value));//info.Parts[0].Result.Value.Addition(context.CalcContext, info.Parts[1].Result.Value));
                }
                else
                {
                    retPart.Code.AddRange(info.Parts[0].Code);
                    retPart.Code.AddRange(info.Parts[1].Code);

                    KillTempVariables(context, null, info.Parts);
                    retPart.Code.Add(new Instruction(
                        currentNode.Location,
                        operation,
                        retPart.Result = context.GetTempVariable(retPart.Result),// out newVariable),
                        info.Parts[0].Result,
                        info.Parts[1].Result));
                }
            }
        }

        /// <summary>
        /// Скомпилировать бинарную логическую операция (&& и ||)
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="retPart">Результирующая часть кода</param>
        /// <param name="jumpOperation">Код инструкции перехода</param>
        /// <param name="skipNextExprFunc">Функция, если один из аргументов значения, для определения следует ли пропустить другой аргумент</param>
        private void BinaryLogicalOperation(CompileContext context, CalcTree currentNode, CodePart retPart, Instruction.OperationCode jumpOperation, Func<SymbolValue, bool> skipNextExprFunc)
        {
            SymbolValue val;
            Address resultAddress = retPart.Result;
            Address tempAddress;

            retPart.Result = context.GetTempVariable(resultAddress);
            tempAddress = retPart.Result;

            // компилируем левую часть
            CodePart leftPart = Compile(context, currentNode.Branches[0], retPart.Result, true);
            KillTempVariables(context, retPart, new CodePart[] { leftPart });

            // компилируем правую часть
            CodePart rightPart = Compile(context, currentNode.Branches[1], retPart.Result, true);
            KillTempVariables(context, retPart, new CodePart[] { rightPart });

            if (leftPart.Result != null && leftPart.Result.Type == Address.AddressType.Value)
            {
                val = leftPart.Result.Value;
                if (val != null && skipNextExprFunc(val))
                    retPart.Result = leftPart.Result;
                else // если первая часть заведомо истина, то результат равен второму операнду
                {
                    if (rightPart.Result.Type != Address.AddressType.Value)
                        //retPart.Code.AddRange(rightPart.Code);
                        retPart.Add(rightPart);
                    retPart.Result = rightPart.Result;
                }
            }
            else if (rightPart.Result != null && rightPart.Result.Type == Address.AddressType.Value)
            {
                val = rightPart.Result.Value;
                if (val != null && skipNextExprFunc(val))
                    retPart.Result = rightPart.Result;
                else // если первая часть заведомо истина, то результат равен второму операнду
                {
                    //retPart.Code.AddRange(leftPart.Code);
                    retPart.Add(leftPart);
                    retPart.Result = leftPart.Result;
                }
            }
            else
            {
                // считаем левую часть
                //retPart.Code.AddRange(leftPart.Code);
                retPart.Add(leftPart);
                retPart.Result = tempAddress;
                if (retPart.Result != leftPart.Result)
                    retPart.Code.Add(new Instruction(currentNode.Location, Instruction.OperationCode.Move, retPart.Result, leftPart.Result));

                // если результат операции уже понятен, переход в конец
                Instruction gotoInstruction = new Instruction(currentNode.Location, jumpOperation, leftPart.Result, new Address(Address.AddressType.Label));

                retPart.Code.Add(gotoInstruction);

                // считаем правую часть
                //retPart.Code.AddRange(rightPart.Code);
                retPart.Add(rightPart);
                retPart.Result = tempAddress;
                if (retPart.Result != rightPart.Result)
                    retPart.Code.Add(new Instruction(currentNode.Location, Instruction.OperationCode.Move, retPart.Result, rightPart.Result));

                gotoInstruction.B.ReferenceIndex = retPart.Code.Count - retPart.Code.IndexOf(gotoInstruction);
            }

            if (retPart.Result != tempAddress && tempAddress != resultAddress)
                context.KillTempVariable(tempAddress);
        }

        /// <summary>
        /// Скомпилировать параметр. 
        /// Для условного параметра подобрать все передаваемые аргументы
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="retPart">Результирующая часть кода</param>
        /// <returns></returns>
        private BranchesInfo CompileParameter(CompileContext context, CalcTree currentNode, CodePart retPart)
        {
            BranchesInfo info = null;
            NodeState nodeState;

            if ((nodeState = context.CalcContext.GetParameter(context.Revision, currentNode.StringValue)) != null)
            {
                retPart.Result = new Address(Address.AddressType.Parameter) { SymbolName = currentNode.StringValue };
                retPart.Parameter = nodeState;
                // подготовить аргументы для параметризированных узлов
                if ((nodeState.NodeInfo as IParameterInfo).Optimization != null)
                {
                    OptimizationState optimState = context.CalcContext.GetOptimization((nodeState.NodeInfo as IParameterInfo).Optimization, context.Revision);

                    int requiredStart = 0;
                    int requiredEnd = optimState.ArgumentsKey.Length - 1;

                    // определяем количество аргументов, которые могут быть пропущенны справа
                    for (; requiredEnd >= 0 && optimState.ArgumentsKey[requiredEnd].Key.Calculable; --requiredEnd) ;

                    // определяем количество аргументов, которые могут быть пропущенны слева
                    if (context.ArgumentsKey != null)
                    {
                        for (int i = 0;
                             i < context.ArgumentsKey.Length &&
                             i <= requiredEnd; i++)
                        {
                            if (context.ArgumentsKey[i].Key.Equals(optimState.ArgumentsKey[i].Key))
                                ++requiredStart;
                            else break;
                        }
                    }

                    // получаем минимальное количество аргументов
                    int minargs = 0;

                    for (int i = requiredStart; i <= requiredEnd; i++)
                    {
                        minargs += optimState.ArgumentsKey[i].Value.Length;
                    }

                    info = CompileBranches(context, currentNode, minargs, optimState.Arguments.Length, false, false, "Параметр ${0}$ должен вызываться с аргументами {1}", (nodeState.NodeInfo as IParameterInfo).Code, ArgumentsToString(optimState.Arguments));

                    //if (info != null)
                    if (info == null)
                    {
                        retPart.Fail = true;
                    }
                    else
                    {
                        List<Address> addressesList = new List<Address>();

                        for (int i = 0;
                             context.ArgumentsKey != null
                             && i < context.ArgumentsKey.Length
                             && i < optimState.ArgumentsKey.Length
                             && context.ArgumentsKey[i].Key.Equals(optimState.ArgumentsKey[i].Key)
                             && addressesList.Count + info.Parts.Length < optimState.Arguments.Length;
                             i++)
                        {
                            // не верное количество явных аргументов
                            if (addressesList.Count + context.ArgumentsKey[i].Value.Length + info.Parts.Length > optimState.Arguments.Length)
                                context.CalcContext.AddMessage(new CalcMessage(MessageCategory.Error, currentNode.Location, "Не верное количество передаваемых аргументов условного параметраs"));

                            for (int k = 0;
                                k < context.ArgumentsKey[i].Value.Length &&
                                addressesList.Count < optimState.Arguments.Length - info.Parts.Length; k++)
                            {
                                addressesList.Add(new Address(context.ArgumentsKey[i].Value[k].Name, true));
                            }
                        }

                        foreach (var codePart in info.Parts)
                        {
                            retPart.Code.AddRange(codePart.Code);
                            addressesList.Add(codePart.Result);
                        }
                        retPart.ParameterArguments = addressesList.ToArray();
                    }
                }
            }
            else
            {
                CalcMessage msg = new CalcMessage(MessageCategory.CriticalError, currentNode.Location, "Параметр ${0}$ не найден", currentNode.StringValue);
                msg.ObjectExpression = currentNode.StringValue;
                context.CalcContext.AddMessage(msg);
                retPart.Fail = true;
            }
            return info;
        }

        /// <summary>
        /// Получить код, который требуется для вызова функции 
        /// или для расчета в случае макросов
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="locat">Положение вызова функции в коде</param>
        /// <param name="func"></param>
        /// <param name="args"></param>
        /// <returns>сгенерированный код</returns>
        private CodePart CompileFunctionCall(CompileContext context, Location locat, Function func, params Tuple<Parameter, Address>[] args)
        {
            Address retVar, val;
            CodePart codePart = new CodePart();

            codePart.Result = context.GetTempVariable(codePart.Result);
            codePart.Code.Add(new Instruction(locat, Instruction.OperationCode.EnterLevel));
            for (int i = 0; i < args.Length; i++)
            {
                val = null;
                if (args[i] != null)
                {
                    if (args[i].Item2 != null
                        && args[i].Item2.Type == Address.AddressType.Symbol)
                    {
                        val = new Address(args[i].Item2.SymbolName, true);
                    }
                    else
                    {
                        val = args[i].Item2;
                    }
                }
                else if (args[i].Item1.Value != null)
                    val = new Address(args[i].Item1.Value);
                if (val != null)
                    codePart.Code.Add(new Instruction(locat, Instruction.OperationCode.VarDecl, new Address(args[i].Item1.Name), val));
            }
            codePart.Code.Add(new Instruction(locat, Instruction.OperationCode.VarDecl, retVar = new Address(CalcContext.ReturnVariableName), new Address(SymbolValue.Nothing)));
            codePart.Code.Add(new Instruction(locat, Instruction.OperationCode.Call, new Address(func.Name)));
            codePart.Code.Add(new Instruction(locat, Instruction.OperationCode.Move, codePart.Result, retVar));
            codePart.Code.Add(new Instruction(locat, Instruction.OperationCode.LeaveLevel));
            return codePart;
        }

        /// <summary>
        /// Скомпилировать вызов функции
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="func">Вызываемая функция</param>
        /// <returns></returns>
        private CodePart CompileFunctionCall(CompileContext context, CalcTree currentNode, Function func)
        {
            ParameterFunction parameterFunction;
            MacrosFunction macroFunction;
            CodePart retCode = new CodePart();

            // Проверка количества передаваемых аргументов
            if (func.Parameters.Length < currentNode.Branches.Count)
            {
                CalcMessage msg = new CalcMessage(MessageCategory.Error, currentNode.Location, "Слишком много аргументов для функции '{0}'", func.Name);
                context.CalcContext.AddMessage(msg);
                return null;
            }
            else
            {
                bool error = false;
                for (int i = currentNode.Branches.Count; !error && i < func.Parameters.Length; i++)
                {
                    error = error || String.IsNullOrEmpty(func.Parameters[i].DefaultValue);
                }
                if (error)
                {
                    CalcMessage msg = new CalcMessage(MessageCategory.Error, currentNode.Location, "Слишком мало аргументов у функции '{0}'", func.Name);
                    context.CalcContext.AddMessage(msg);
                    return null;
                }
            }
            // подготовка аргументов для передачи функции
            int parameterCount = func.Parameters.Length;
            List<Tuple<Parameter, Address>> args = new List<Tuple<Parameter, Address>>();

            List<CodePart> cleanupList = new List<CodePart>();

            for (int i = 0; i < func.Parameters.Length; i++)
            {
                Address defaultValue;
                double val;

                if (String.IsNullOrEmpty(func.Parameters[i].DefaultValue))
                    defaultValue = null;
                else if (double.TryParse(func.Parameters[i].DefaultValue, out val))
                    defaultValue = new Address(SymbolValue.CreateValue(val));
                else
                    defaultValue = new Address(SymbolValue.CreateValue(func.Parameters[i].DefaultValue));

                args.Add(Tuple.Create(new Parameter(func.Parameters[i]), defaultValue));
            }

            for (int i = 0; i < currentNode.Branches.Count; i++)
            {
                CodePart codePart = Compile(context, currentNode.Branches[i], null, false);
                //retCode.Code.AddRange(codePart.Code);
                retCode.Add(codePart);
                retCode.Result = null;

                cleanupList.Add(codePart);
                if (codePart.Result != null && codePart.Result.Type == Address.AddressType.Parameter)
                {
                    // добавляем в функцию агрегации аргументы условных параметров
                    if ((parameterFunction = func as ParameterFunction) != null && i < parameterFunction.ParameterCount)
                    {
                        //args[i].Item2 = new Address(Address.AddressType.Parameter)
                        //{
                        //    SymbolName = (codePart.Parameter.NodeInfo as IParameterInfo).Code
                        //};
                        args[i] = Tuple.Create(
                            args[i].Item1,
                            new Address(Address.AddressType.Parameter)
                            {
                                SymbolName = (codePart.Parameter.NodeInfo as IParameterInfo).Code
                            });

                        IOptimizationInfo optimization = (codePart.Parameter.NodeInfo as IParameterInfo).Optimization;

                        if (optimization != null)
                        {
                            OptimizationState optimizationState = context.CalcContext.GetOptimization(optimization, context.Revision);
                            Parameter par;
                            Address value;

                            for (int j = 0; j < optimizationState.Arguments.Length && j < codePart.ParameterArguments.Length; j++)
                            {
                                par = new Parameter(
                                    parameterFunction.GetParameterName(i, optimizationState.Arguments[j].Name),
                                    SymbolValue.ValueFromString(optimizationState.Arguments[j].DefaultValue),
                                    optimizationState.Arguments[j].ParameterAccessor);

                                value = codePart.ParameterArguments[j];
                                args.Add(Tuple.Create(par, value));
                            }
                        }
                    }
                    else
                    {
                        retCode.Add(CalcDefaultAggregationParameter(context, currentNode.Location, codePart));
                        //args[i].Item2 = retCode.Result;
                        args[i] = Tuple.Create(args[i].Item1, retCode.Result);
                    }
                }
                else
                {
                    //args[i].Item2 = codePart.Result;
                    args[i] = Tuple.Create(args[i].Item1, codePart.Result);
                }
            }
            if ((macroFunction = func as MacrosFunction) != null)
            {
                retCode.Add(macroFunction.CallCode(context, currentNode.Location, args.ToArray()));
            }
            else
            {
                retCode.Add(CompileFunctionCall(context, currentNode.Location, func, args.ToArray()));
            }
            KillTempVariables(context, retCode, cleanupList);
            return retCode;
        }

        /// <summary>
        /// Скомпилровать функцию агрегации параметра по умолчанию
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="location">Положение в тексте, для сообщений об ошибке</param>
        /// <param name="parameterCodePart">Часть кода с параметром</param>
        /// <returns></returns>
        private CodePart CalcDefaultAggregationParameter(CompileContext context, Location location, CodePart parameterCodePart)
        {
            ParameterFunction func;

            if ((func = context.CalcContext.SymbolTable.GetFunction(context.Revision, DefaultAggregationFunctionName) as ParameterFunction) != null)
            {
                OptimizationState optimState = context.CalcContext.GetOptimization((parameterCodePart.Parameter.NodeInfo as IParameterInfo).Optimization, context.Revision);

                int argumentsLength = optimState == null ? 0 : optimState.Arguments.Length;
                List<Tuple<Parameter, Address>> args = new List<Tuple<Parameter, Address>>();

                for (int j = 0; j < func.Parameters.Length; j++)
                {
                    Address defaultValue;
                    double val;

                    if (func.Parameters[j].Name.Equals(ParameterFunction.ParameterDefaultArgumentName))
                        defaultValue = parameterCodePart.Result;
                    else
                    {
                        if (String.IsNullOrEmpty(func.Parameters[j].DefaultValue))
                            defaultValue = null;
                        else if (double.TryParse(func.Parameters[j].DefaultValue, out val))
                            defaultValue = new Address(SymbolValue.CreateValue(val));
                        else
                            defaultValue = new Address(SymbolValue.CreateValue(func.Parameters[j].DefaultValue));
                    }

                    args.Add(Tuple.Create(new Parameter(func.Parameters[j]), defaultValue));
                }
                // передача дополнительных аргументов для условных параметров
                OptimizationState optimizationState = context.CalcContext.GetOptimization((parameterCodePart.Parameter.NodeInfo as IParameterInfo).Optimization, context.Revision);
                if (parameterCodePart.ParameterArguments != null)
                    for (int j = 0; j < argumentsLength && j < parameterCodePart.ParameterArguments.Length; j++)
                    {
                        Parameter par = new Parameter(func.GetParameterName(0, optimizationState.Arguments[j].Name), ParameterAccessor.In);
                        Address adr = parameterCodePart.ParameterArguments[j];
                        args.Add(Tuple.Create(par, adr));
                    }

                CodePart retPart = new CodePart();
                retPart.Code.AddRange(parameterCodePart.Code);
                retPart.Add(CompileFunctionCall(context, location, func, args.ToArray()));

                return retPart;
            }
            return null;
        }

        /// <summary>
        /// Скомпилировать оператор if
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="retPart">Результирующая часть кода</param>
        private void CompileIf(CompileContext context, CalcTree currentNode, CodePart retPart)
        {
            if (currentNode.Branches.Count > 1)
            {
                Instruction elseInstruction = null;
                Instruction gotoElseInstruction = null, gotoEndInstruction = null;

                if (currentNode.MustHaveReturn)
                {
                    currentNode.Branches[1].MustHaveReturn = true;
                    if (currentNode.Branches.Count > 2) currentNode.Branches[2].MustHaveReturn = true;
                    currentNode.MustHaveReturn = false;
                }

                CodePart conditionCode = Compile(context, currentNode.Branches[0], null, true);
                bool alwaysTrue, alwaysFalse;

                //retPart.Code.AddRange(conditionCode.Code);
                retPart.Add(conditionCode);
                retPart.Result = null;
                retPart.Parameter = null;
                retPart.ParameterArguments = null;

                alwaysFalse = conditionCode.Result != null
                    && conditionCode.Result.Type == Address.AddressType.Value
                    && conditionCode.Result.Value.IsZero(context.CalcContext);

                alwaysTrue = conditionCode.Result != null
                    && conditionCode.Result.Type == Address.AddressType.Value
                    && !conditionCode.Result.Value.IsZero(context.CalcContext);

                if (!alwaysTrue && !alwaysFalse)
                {
                    // переход к else или в конец
                    gotoElseInstruction = new Instruction(currentNode.Location, Instruction.OperationCode.JumpZ, conditionCode.Result, new Address(Address.AddressType.Label));
                    retPart.Code.Add(gotoElseInstruction);
                }
                context.KillTempVariable(conditionCode.Result);

                if (alwaysTrue || !alwaysFalse)
                {
                    CodePart thenCode = Compile(context, currentNode.Branches[1], null, true);
                    retPart.Code.AddRange(thenCode.Code);
                    context.KillTempVariable(thenCode.Result);
                }

                if (currentNode.Branches.Count > 2)
                {
                    if (alwaysFalse || !alwaysTrue)
                    {
                        CodePart elseCode = Compile(context, currentNode.Branches[2], null, true);

                        if (elseCode.Code != null && elseCode.Code.Count > 0)
                        {
                            if (!alwaysFalse)
                            {
                                // переход в конец
                                gotoEndInstruction = new Instruction(currentNode.Location, Instruction.OperationCode.Jump, new Address(Address.AddressType.Label));
                                retPart.Code.Add(gotoEndInstruction);
                            }

                            retPart.Code.AddRange(elseCode.Code);
                            if (!alwaysFalse)
                            {
                                elseInstruction = elseCode.Code[0];
                            }
                        }
                        context.KillTempVariable(elseCode.Result);
                    }
                }

                // выставляем переходы
                if (gotoElseInstruction != null)
                {
                    int elsePosition = elseInstruction == null ? retPart.Code.Count : retPart.Code.IndexOf(elseInstruction);
                    gotoElseInstruction.B.ReferenceIndex = elsePosition - retPart.Code.IndexOf(gotoElseInstruction);
                }
                if (gotoEndInstruction != null)
                    gotoEndInstruction.A.ReferenceIndex = retPart.Code.Count - retPart.Code.IndexOf(gotoEndInstruction);
            }
        }

        /// <summary>
        /// Скомпилировать оператор while
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="retPart">Результирующая часть кода</param>
        private void CompileWhile(CompileContext context, CalcTree currentNode, CodePart retPart)
        {
            if (currentNode.Branches.Count > 1)
            {
                Address endAddress, loopAddress;
                Instruction endInstruction, loopInstruction;
                Instruction endFromInstruction, loopFromInstruction;

                if (currentNode.MustHaveReturn) { currentNode.Branches[1].MustHaveReturn = true; currentNode.MustHaveReturn = false; }

                CodePart conditionCode = Compile(context, currentNode.Branches[0], null, true);

                loopInstruction = new Instruction(currentNode.Location, Instruction.OperationCode.LoopEnter);

                retPart.Code.Add(loopInstruction);
                retPart.Code.AddRange(conditionCode.Code);

                // ссылка на выход
                endAddress = new Address(Address.AddressType.Label);
                endFromInstruction = new Instruction(currentNode.Location, Instruction.OperationCode.JumpZ, conditionCode.Result, endAddress);
                retPart.Code.Add(endFromInstruction);
                retPart.Code.Add(new Instruction(currentNode.Location, Instruction.OperationCode.LoopPass));

                context.KillTempVariable(conditionCode.Result);

                CodePart loopCode = Compile(context, currentNode.Branches[1], null, true);

                retPart.Code.AddRange(loopCode.Code);
                context.KillTempVariable(loopCode.Result);

                // ссылка на вход в цикл
                loopAddress = new Address(Address.AddressType.Label);
                loopFromInstruction = new Instruction(currentNode.Location, Instruction.OperationCode.Jump, loopAddress);
                retPart.Code.Add(loopFromInstruction);

                endInstruction = new Instruction(currentNode.Location, Instruction.OperationCode.LoopLeave);
                retPart.Code.Add(endInstruction);

                // вынесем объявления переменных, сделанных вне структурных операторов, за цикл 
                List<Instruction> pushAtBegin = new List<Instruction>();
                int level = 0;
                foreach (Instruction item in retPart.Code)
                {
                    if (item.Operation == Instruction.OperationCode.EnterLevel)
                        break;

                    else if (item.Operation == Instruction.OperationCode.VarDecl && level == 0)
                        pushAtBegin.Add(item);
                }
                foreach (var item in pushAtBegin)
                {
                    retPart.Code.Remove(item);
                    retPart.Code.Insert(0, item);
                }
                // выставляем переходы
                // на выход из цикла
                endAddress.ReferenceIndex = retPart.Code.IndexOf(endInstruction) - retPart.Code.IndexOf(endFromInstruction);
                // в т.ч. по break
                foreach (var instruction in context.GetBreakInstructions())
                {
                    instruction.A.ReferenceIndex = retPart.Code.IndexOf(endInstruction) - retPart.Code.IndexOf(instruction);
                }
                // повтор цикла
                // переходим не на сам loopInstruction, а на следующий за ним
                loopAddress.ReferenceIndex = retPart.Code.IndexOf(loopInstruction) - retPart.Code.IndexOf(loopFromInstruction) + 1;
                // в т.ч. по continue
                foreach (var instruction in context.GetContinueInstructions())
                {
                    instruction.A.ReferenceIndex = retPart.Code.IndexOf(loopInstruction) - retPart.Code.IndexOf(instruction) + 1;
                }
            }
        }

        /// <summary>
        /// Скомпилировать оператор break или оператор continue
        /// </summary>
        /// <param name="context">Контекст компиляции</param>
        /// <param name="currentNode">Текущий узел дерева разбора</param>
        /// <param name="retPart">Результирующая часть кода</param>
        /// <param name="isBreak">
        /// Если требуется компилировать оператор break, то true.
        /// Если continue - то false.
        /// </param>
        private void CompileBreakContinue(CompileContext context, CalcTree currentNode, CodePart retPart, bool isBreak)
        {
            bool ret;
            int symbolTableDiff;
            Instruction instruction = new Instruction(currentNode.Location, Instruction.OperationCode.Jump, new Address(0));

            if (isBreak)
                ret = context.AddBreakInstruction(instruction, out symbolTableDiff);
            else
                ret = context.AddContinueInstruction(instruction, out symbolTableDiff);

            if (ret)
            {
                for (int i = 0; i < symbolTableDiff; i++)
                {
                    retPart.Code.Add(new Instruction(currentNode.Location, Instruction.OperationCode.LeaveLevel));
                }
                retPart.Code.Add(instruction);
            }
            else context.CalcContext.AddMessage(new CalcMessage(MessageCategory.Error, currentNode.Location, "Использование оператора {0} вне цикла", isBreak ? "break" : "continue"));
        }
    }
}

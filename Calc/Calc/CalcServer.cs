using System;
using System.Linq;
using System.Collections.Generic;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Объект управления расчетами
    /// Производит как автоматический расчет, так и расчеты, вызываемые в ручную
    /// </summary>
    public class CalcServer
    {
        //CalcStateStorage calcCache;
        ICompiler compiler;
        Interpreter interpreter;

        ICalcSupplier calcSupplier;

        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Путь к внешним подгружаемым библиотекам функций
        /// </summary>
        public String FunctionsPath { get; set; }

        /// <summary>
        /// Стандартные функции, используемые в расчете
        /// (Функции агрегации, математические и "встроенные")
        /// </summary>
        Function[] methods;

        public CalcServer(ICalcSupplier calcSupplier)
        {
            this.calcSupplier = calcSupplier;

            //calcCache = new CalcStateStorage();
            compiler = new Compiler();
            interpreter = new Interpreter();

            // установить путь к подгружаемым функциям
            String rootDirectory = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            this.FunctionsPath = System.IO.Path.Combine(rootDirectory, "lib");
 
            InitilizeStandardFunction(compiler);
        }

        /// <summary>
        /// Заполняет поле methods стандартными функциями
        /// </summary>
        private void InitilizeStandardFunction(ICompiler compiler)
        {
            Type mathType = typeof(Math), contextType = typeof(CallContext);
            Type[] oneDouble = new Type[] { typeof(double) }, twoDouble = new Type[] { typeof(double), typeof(double) };
            String agrGroup = "Агрегация", mathGroup = "Математические", contextGroup = "Текущая дискретность", arrayGroup = "Работа с массивами";

            methods = new Function[]{
                new ParameterFunction(calcSupplier,     "first",        CalcAggregation.First,      agrGroup,   "Получить первое значение параметра за период"),
                new ParameterFunction(calcSupplier,     "last",         CalcAggregation.Last,       agrGroup,   "Получить последнее значение параметра за период"),
                new ParameterFunction(calcSupplier,     "minp",         CalcAggregation.Minimum,    agrGroup,   "Получить минимальное значение параметра за период"),
                new ParameterFunction(calcSupplier,     "maxp",         CalcAggregation.Maximum,    agrGroup,   "Получить максимальное значение параметра за период"),
                new ParameterFunction(calcSupplier,     "avg",          CalcAggregation.Average,    agrGroup,   "Получить среднее значение параметра за период"),
                new ParameterFunction(calcSupplier,     "sum",          CalcAggregation.Sum,        agrGroup,   "Получить сумму значений параметра за период"),
                new ParameterFunction(calcSupplier,     "weight",       CalcAggregation.Weighted,   agrGroup,   "Получить взвешенное значение параметра за период"),
                new ParameterFunction(calcSupplier,     "count",        CalcAggregation.Count,      agrGroup,   "Получить количество значений параметра за период"),
                new ParameterFunction(calcSupplier,     "exist",        CalcAggregation.Exist,      agrGroup,   "Существуют ли значения параметра за период"),
                new SetParameterFunction(calcSupplier,  "set_param",                                agrGroup,   "Изменить значение параметра"),
                                                                                        
                new StandardFunction("abs",         mathType.GetMethod("Abs", oneDouble),       mathGroup,  "Возвращает абсолютное значение заданного числа."), 
                new StandardFunction("acos",        mathType.GetMethod("Acos", oneDouble),      mathGroup,  "Возвращает угол, косинус которого равен указанному числу."),
                new StandardFunction("asin",        mathType.GetMethod("Asin", oneDouble),      mathGroup,  "Возвращает угол, синус которого равен указанному числу."),
                new StandardFunction("atan",        mathType.GetMethod("Atan", oneDouble),      mathGroup,  "Возвращает угол, тангенс которого равен указанному числу."),
                new StandardFunction("atan2",       mathType.GetMethod("Atan2", twoDouble),     mathGroup,  "Возвращает угол, тангенс которого равен отношению двух указанных чисел."),
                new StandardFunction("ceiling",     mathType.GetMethod("Ceiling", oneDouble),   mathGroup,  "Возвращает наименьшее целое число, которое больше или равно заданному числу."),
                new StandardFunction("cos",         mathType.GetMethod("Cos", oneDouble),       mathGroup,  "Возвращает косинус указанного угла."),
                new StandardFunction("cosh",        mathType.GetMethod("Cosh", oneDouble),      mathGroup,  "Возвращает гиперболический косинус указанного угла."),
                new StandardFunction("exp",         mathType.GetMethod("Exp", oneDouble),       mathGroup,  "Возвращает e, возведенное в указанную степень."),
                new StandardFunction("floor",       mathType.GetMethod("Floor", oneDouble),     mathGroup,  "Возвращает наибольшее целое число, которое меньше или равно указанному числу."),
                new StandardFunction("log",         mathType.GetMethod("Log", oneDouble),       mathGroup,  "Возвращает логарифм указанного числа."),
                new StandardFunction("logb",        mathType.GetMethod("Log", twoDouble),       mathGroup,  "Возвращает логарифм указанного числа в системе счисления с указанным основанием."),
                new StandardFunction("log10",       mathType.GetMethod("Log10", oneDouble),     mathGroup,  "Возвращает логарифм с основанием 10 указанного числа."),
                new StandardFunction("max",         mathType.GetMethod("Max", twoDouble),       mathGroup,  "Возвращает большее из двух указанных чисел."),
                new StandardFunction("min",         mathType.GetMethod("Min", twoDouble),       mathGroup,  "Возвращает меньшее из двух чисел."),
                new StandardFunction("pow",         mathType.GetMethod("Pow", twoDouble),       mathGroup,  "Возвращает указанное число, возведенное в указанную степень."),
                new StandardFunction("round",       mathType.GetMethod("Round", oneDouble),     mathGroup,  "Округляет значение до ближайшего целого или указанного количества десятичных знаков."),
                new StandardFunction("sign",        mathType.GetMethod("Sign", oneDouble),      mathGroup,  "Возвращает значение, определяющее знак числа."),
                new StandardFunction("sin",         mathType.GetMethod("Sin", oneDouble),       mathGroup,  "Возвращает синус указанного угла."),
                new StandardFunction("sinh",        mathType.GetMethod("Sinh", oneDouble),      mathGroup,  "Возвращает гиперболический синус указанного угла."),
                new StandardFunction("sqrt",        mathType.GetMethod("Sqrt", oneDouble),      mathGroup,  "Возвращает квадратный корень из указанного числа."),
                new StandardFunction("tan",         mathType.GetMethod("Tan", oneDouble),       mathGroup,  "Возвращает тангенс указанного угла."),
                new StandardFunction("tanh",        mathType.GetMethod("Tanh", oneDouble),      mathGroup,  "Возвращает гиперболический тангенс указанного угла."),
                new StandardFunction("truncate",    mathType.GetMethod("Truncate", oneDouble),  mathGroup,  "Вычисляет целую часть числа."),

                new ContextFunction("GetInterval",     contextGroup,   "Возвращает текущий интервал в секундах"),
                new ContextFunction("GetStartYear",    contextGroup,   "Возвращает год, начала интервала расчета"),
                new ContextFunction("GetStartMonth",   contextGroup,   "Возвращает месяц, начала интервала расчета"),
                new ContextFunction("GetStartDay",     contextGroup,   "Возвращает день, начала интервала расчета"),
                new ContextFunction("GetEndYear",      contextGroup,   "Возвращает год, конца интервала расчета"),
                new ContextFunction("GetEndMonth",     contextGroup,   "Возвращает месяц, конца интервала расчета"),
                new ContextFunction("GetEndDay",       contextGroup,   "Возвращает день, конца интервала расчета"),
                new ContextFunction("GetDaysInMonth",  contextGroup,   "Возвращает количество дней в месеце"),

                new MacrosFunction("array", new CalcArgumentInfo[]{new CalcArgumentInfo("N", "", ParameterAccessor.In)}, arrayGroup, "", 
                    new Instruction[]{
                        new Instruction(null, Instruction.OperationCode.ArrayDecl, new Address(CalcContext.ReturnVariableName), new Address("N"))
                    }),
                new MacrosFunction("length", new CalcArgumentInfo[]{new CalcArgumentInfo("arr", "", ParameterAccessor.In)}, arrayGroup, "", 
                    new Instruction[]{
                        new Instruction(null, Instruction.OperationCode.ArrayLength, new Address(CalcContext.ReturnVariableName), new Address("arr"))
                    })
            };
        }

        /// <summary>
        /// Значение по умолчанию для максимального количество расчитываемых значений одного параметра за рах
        /// </summary>
        public const int DefaultMaxCalcCount = 500;

        /// <summary>
        /// Максимальное количество расчитываемых значений одного параметра за рах
        /// </summary>
        public int MaxCalcCount
        {
            get
            {
                try
                {
                    return (int)GlobalSettings.Instance.ParametersConstraction;
                }
                catch
                {
                    return DefaultMaxCalcCount;
                }
            }
        }

        public int MaxLoopCount { get { return (int)GlobalSettings.Instance.LoopConstraction; } }

        /// <summary>
        /// Произвести расчет парамтеров
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="pars">Расчитываемые параметры</param>
        /// <param name="startTime">Начальное расчитываемое время</param>
        /// <param name="endTime">Конечное расчитываемое время</param>
        /// <param name="isRoundRobin">Расчет вызван из циклического расчета?</param>
        public void Calc(OperationState state, ICalcNode[] pars, DateTime startTime, DateTime endTime, bool isRoundRobin, bool recalcAll)
        {
#if DEBUG
            //calcCache.Clear();
#endif
            ShadowCalc(state, pars, startTime, endTime, isRoundRobin, recalcAll);
        }

        /// <summary>
        /// Произвести расчет парамтеров
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="pars">Расчитываемые параметры</param>
        /// <param name="startTime">Начальное расчитываемое время</param>
        /// <param name="endTime">Конечное расчитываемое время</param>
        /// <param name="isRoundRobin">Расчет вызван из циклического расчета?</param>
        /// <param name="recalcAll"></param>
        /// <returns>Контекст расчета содержащий все данные полученные в ходе расчета</returns>
        private void ShadowCalc(OperationState state, ICalcNode[] pars, DateTime startTime, DateTime endTime, bool isRoundRobin, bool recalcAll)
        {
            if (endTime <= startTime)
            {
                throw new ArgumentException("Начало периода расчета должно быть меньше конца периода расчета", "startTime, endTime");
            }

            using (CalcContext calcContext = CreateBaseContext(state))
            {
                calcContext.RecalcAll = recalcAll;
                calcContext.IsRoundRobin = isRoundRobin;

                List<CalcState> toCalcNodes = new List<CalcState>();
                System.Text.StringBuilder reportBuilder = new System.Text.StringBuilder();
                //state.StateString = "Компиляция параметров";
                state.AllowStartAsyncResult = true;
#if DEBUG
                DateTime startSave = DateTime.Now;
#endif
                //calcContext.OperationState = state;
                calcContext.AddParametersToCalc(pars);
                calcContext.SetTime(startTime, endTime);

                interpreter.Exec(calcContext);
                state.StateString = "Сохранение значений";
#if DEBUG
                TimeSpan saveValueTime = DateTime.Now.Subtract(startSave);
                state.AddMessage(new CalcMessage(MessageCategory.Message, "Время расчета {0:HH:mm:ss}", new DateTime(saveValueTime.Ticks)));
                log.Debug("Время расчета {0:HH:mm:ss}", new DateTime(saveValueTime.Ticks));
                startSave = DateTime.Now;
                int count = 0;
#endif
                Package[] savingValues = calcContext.ValuesKeeper.GetAllCalculatedValues();
                try
                {
                    //calcSupplier.DeleteNeedless(calcContext.ValuesKeeper.GetAllValues());
                    List<Message> messages;
                    calcSupplier.SaveParameterNodeValue(calcContext, savingValues, out messages);
                    state.AddMessage(messages);
                }
                catch (System.Threading.ThreadInterruptedException) { throw; }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception exc)
                {
                    AddMessageByException(state, exc);
                }
#if DEBUG
                saveValueTime = DateTime.Now.Subtract(startSave);
                foreach (Package item in savingValues)
                    count += item.Count;

                state.AddMessage(new CalcMessage(MessageCategory.Message, "{0} значений сохранено за {1:HH:mm:ss}", count, new DateTime(saveValueTime.Ticks)));
                log.Debug("{0} значений сохранено за {1:HH:mm:ss}", count, new DateTime(saveValueTime.Ticks));
#endif
            }
        }

        /// <summary>
        /// Добавить сообщение по ошибке
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="exc">Исключение</param>
        private static void AddMessageByException(OperationState state, Exception exc)
        {
            String mess = exc.Message;
#if DEBUG
            String stackTrace, errorReport = String.Empty; ;
            if (exc.Data.Contains("StackTrace"))
                stackTrace = exc.Data["StackTrace"].ToString();
            else stackTrace = exc.StackTrace;
            if (exc.Data.Contains("error"))
                errorReport = "\n" + exc.Data["error"].ToString();
            else errorReport = String.Empty;
            mess += String.Format("\n{0}{1}", stackTrace, errorReport);
#endif
            mess = mess.Replace("{", "{{").Replace("}", "}}");
            state.AddMessage(new CalcMessage(MessageCategory.CriticalError, mess));
        }

        /// <summary>
        /// Создать верхний контекст для расчета и добавить в него все глобальные символы
        /// </summary>
        /// <returns></returns>
        private CalcContext CreateBaseContext(OperationState state)
        {
            CalcContext context;
            context = new CalcContext(state, new CalcStateStorage(), calcSupplier, compiler);
            context.MaxLoopCount = MaxLoopCount;

            IEnumerable<ConstsInfo> consts = GetCalcConsts();
            foreach (ConstsInfo item in consts)
                context.SymbolTable.DeclareSymbol(new Variable(item.Name, SymbolValue.ValueFromString(item.Value), true, false), true, false);

            // standard functions
            context.SymbolTable.DeclareSymbol(methods, true);

            // custom functions
            foreach (var item in calcSupplier.GetCustomFunction())
                context.SymbolTable.DeclareFunction(new CustomFunction(item));

            // регистрация внешних функций
            foreach (var item in calcSupplier.GetExternalFunctions(context))
                context.SymbolTable.DeclareFunction(new ExternalFunction(item));

            // регистрация библиотечных функций
            List<Symbol> libraryFunctions;
            context.FunctionManager = RemoteFunctionManager.Create(FunctionsPath);
            libraryFunctions = context.FunctionManager.LoadFunction();

            context.SymbolTable.DeclareFunction(libraryFunctions);

            context.ValuesKeeper = new ValuesKeeper(calcSupplier);
            return context;
        }

        #region Поиск ссылок и зависимостей, изменение кода параметра
        /// <summary>
        /// Заменяет код параметра или название функции в формулах
        /// </summary>
        /// <param name="old_code">Старый код параметра (название функции)</param>
        /// <param name="new_code">Новый код параметра (название функции)</param>
        /// <param name="nodes">Параметры, которые подвержены изменению формулы</param>
        public void ChangeParameterCode(OperationState state, /*Guid userGuid,*/ string old_code, string new_code,
           String[] nodes)
        {
            Tokens token;
            Scanner scanner;
            String formula;
            int pos;

            state.StateString = "Изменение формул";
            double step = AsyncOperation.MaxProgressValue / 2 / nodes.Length;

            for (int i = 0; i < nodes.Length; i++)
            {
                formula = nodes[i];

                if (!String.IsNullOrEmpty(formula))
                {
                    scanner = new Scanner();
                    scanner.SetSource(formula, 0);

                    while (Tokens.EOF != (token = (Tokens)scanner.yylex()))
                    {
                        if (token == Tokens.lxmParamIdentifier &&
                            scanner.yylval.ltrIdent == old_code)
                        {
                            pos = scanner.buffer.ReadPos - old_code.Length - 1;// 1 for $
                            formula = formula.Remove(pos,
                                old_code.Length);
                            formula = formula.Insert(pos, new_code);
                            scanner.SetSource(formula, pos + new_code.Length + 1);
                        }
                        else
                            if (token == Tokens.lxmIdentifier &&
                                scanner.yylval.ltrIdent == old_code)
                            {
                                Scanner tmp_scanner = new Scanner();
                                Tokens tmp_token;

                                tmp_scanner.SetSource(formula, scanner.buffer.ReadPos);
                                tmp_token = (Tokens)tmp_scanner.yylex();
                                if (tmp_token == Tokens.lxmLeftParenth)
                                {
                                    pos = scanner.buffer.ReadPos - old_code.Length;
                                    formula = formula.Remove(pos,
                                        old_code.Length);
                                    formula = formula.Insert(pos, new_code);
                                    scanner.SetSource(formula, pos + new_code.Length + 1);
                                }
                            }
                    }
                    nodes[i] = formula;
                }
                state.Progress += step;
            }
        }

        /// <summary>
        /// Проверяет, имеет ли параметр зависимости
        /// </summary>
        /// <param name="node">параметр</param>
        /// <returns>True, если параметр имеет зависимости</returns>
        public bool HasDependence(RevisionInfo revision, IParameterInfo node)
        {
            OperationState state;
            if (node.Calculable) return false;
            state = new OperationState();
            GetDependence(state, revision, node, true);
            return state.GetNextAsyncResult(true) != null;
        }

        /// <summary>
        /// Поиск элементов, необходимых для расчета данного параметра
        /// </summary>
        /// <param name="node">параметр</param>
        /// <param name="first_only">вернуть только первый параметр</param>
        /// <remarks>Через state возвращаеются INodeInfo параметров, необходимых для расчета,
        /// и String если параметра по коду не найдено</remarks>
        public IEnumerable<Tuple<IParameterInfo, IParameterInfo[]>> GetDependence(OperationState state, RevisionInfo revision, IParameterInfo node)
        {
            IParameterInfo parameterNode = node;

            return GetDependence(state, revision, parameterNode, false);
        }
        // мб возвращать список кодов параметров или ид
        private IEnumerable<Tuple<IParameterInfo, IParameterInfo[]>> GetDependence(OperationState state, RevisionInfo revision, IParameterInfo parameterNode, bool first_only)
        {
            List<IParameterInfo> parameters, retParameter = new List<IParameterInfo>();
            List<IParameterInfo> reparseParameters = new List<IParameterInfo>();
            IParameterInfo param;
            Scanner scanner = new Scanner();
            String formula = parameterNode.Formula;
            Tokens token;

            state.AllowStartAsyncResult = true;
            reparseParameters.Add(parameterNode);
            List<IParameterInfo> retDependences = new List<IParameterInfo>();

            using (var context = new CalcContext(state, null, calcSupplier, null))
            {
                while (reparseParameters.Count > 0)
                {
                    parameters = reparseParameters;
                    reparseParameters = new List<IParameterInfo>();
                    foreach (IParameterInfo p in parameters)
                    {
                        if (p.Calculable)
                        {
                            retDependences.Clear();
                            formula = p.Formula;
                            scanner.SetSource(formula, 0);
                            while ((token = (Tokens)scanner.yylex()) != Tokens.EOF)
                            {
                                if (token == Tokens.lxmParamIdentifier)
                                {
                                    param = calcSupplier.GetParameterNode(context, revision, scanner.yylval.ltrIdent);
                                    if (param == null)
                                        retDependences.Add(calcSupplier.GetEmptyParameterNode(revision, scanner.yylval.ltrIdent));
                                    else
                                    {
                                        if (!retDependences.Contains(param))
                                            retDependences.Add(param);
                                        if (first_only)
                                            break;
                                        if (!retParameter.Contains(param))
                                        {
                                            retParameter.Add(param);
                                            if (param.Calculable)
                                                reparseParameters.Add(param);
                                        }
                                    }
                                }
                            }
                            if (retDependences.Count > 0)
                            {
                                yield return Tuple.Create(p, retDependences.ToArray());
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Проверяет, имеет ли параметр ссылки
        /// </summary>
        /// <param name="node">параметр</param>
        /// <returns>True, если параметр имеет ссылки</returns>
        public bool HasReference(OperationState state, String code)
        {
            GetReference(state, code, true);
            return state.GetNextAsyncResult(true) != null;
        }

        /// <summary>
        /// Поиск элементов, которым необходим для расчета данный параметр
        /// </summary>
        /// <param name="node">параметр</param>
        /// <returns>массив параметров, зависимых от данного</returns>
        public void GetReference(OperationState state,  /*Guid userGUID,*/ String code)
        {
            GetReference(state, code, false);
        }

        /// <summary>
        /// Поиск элементов, которым необходим для расчета данный параметр
        /// </summary>
        /// <param name="node">параметр</param>
        /// <param name="first_only">вернуть только первый параметр</param>
        /// <returns>массив параметров, зависимых от данного</returns>
        private void GetReference(OperationState state, String node_code, bool first_only)
        {
            IEnumerable<ICalcNode> parameters = null;
            Scanner scanner = new Scanner();
            Tokens token;

            using (var context = new CalcContext(state, null, calcSupplier, null))
            {
                parameters = calcSupplier.GetParameterNodes(context);
                foreach (ICalcNode calcNode in parameters)
                {
                    foreach (var revision in calcNode.Revisions)
                    {
                        IParameterInfo p = calcNode.GetParameter(revision);
                        if (p == null)
                            continue;
                        try
                        {
                            if (p.Calculable)
                            {
                                scanner.SetSource(p.Formula, 0);
                                while ((token = (Tokens)scanner.yylex()) != Tokens.EOF)
                                    if (token == Tokens.lxmParamIdentifier
                                        && scanner.yylval.ltrIdent.Equals(node_code))
                                    {
                                        state.AddAsyncResult(p);
                                        if (first_only) return;
                                        break;
                                    }
                                    else
                                        if (token == Tokens.lxmIdentifier &&
                                            scanner.yylval.ltrIdent.Equals(node_code))
                                        {
                                            Scanner tmp_scanner = new Scanner();
                                            Tokens tmp_token;

                                            tmp_scanner.SetSource(p.Formula, scanner.buffer.ReadPos);
                                            tmp_token = (Tokens)tmp_scanner.yylex();
                                            if (tmp_token == Tokens.lxmLeftParenth)
                                            {
                                                state.AddAsyncResult(p);
                                                if (first_only) return;
                                                break;
                                            }
                                        }
                            }
                        }
                        catch (Exception) { }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Запросить список всех функций используемых в расчете
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <returns></returns>
        public FunctionInfo[] GetCalcFunctions(OperationState state, RevisionInfo revision)
        {
            using (CalcContext context = CreateBaseContext(state))
            {
                return context.SymbolTable.GetAllFunction(revision).ToArray();
            }
        }

        /// <summary>
        /// Получить стандартные константы
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ConstsInfo> GetCalcConsts()
        {
            List<ConstsInfo> consts = new List<ConstsInfo>();

            consts.Add(new ConstsInfo(false, "PI", "число пи", Math.PI.ToString()));
            consts.Add(new ConstsInfo(false, "E", "число e", Math.E.ToString()));
            consts.AddRange(calcSupplier.GetConsts());

            return consts.ToArray();
        }

        /// <summary>
        /// Проверить формулу на ошибки
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="formulaText">Текст формулы</param>
        /// <param name="parameters">Аргументы функции/условного параметра</param>
        public void CheckFormula(OperationState state, RevisionInfo revision, string formulaText, KeyValuePair<int, CalcArgumentInfo[]>[] parameters)
        {
            using (CalcContext baseContext = CreateBaseContext(state))
            {
                try
                {
                    KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>[] args = null;

                    if (parameters != null)
                    {
                        args = new KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>[parameters.Length];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            ICalcNode calcNode = calcSupplier.GetParameterNode(baseContext, parameters[i].Key);

                            IOptimizationInfo optimizationInfo = calcNode.GetOptimization(revision);
                            if (optimizationInfo == null)
                                throw new ArgumentException();
                            args[i] = new KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>(optimizationInfo, parameters[i].Value);
                        }
                    }

                    compiler.Compile(baseContext, revision, formulaText, args);//, out references, out neededs);
                }
                catch (Exception exc)
                {
                    AddMessageByException(state, exc);
                }
            }
        }

        public ICalcContext CreateCalcContext(OperationState state)
        {
            return new CalcContext(state, null, calcSupplier, null);
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using gppg;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Контекст для проведения оптимизационных расчетов
    /// </summary>
    class OptimizationContext : CallContext
    {
        public OptimizationContext(OptimizationState state, DateTime startTime, DateTime endTime)
            : base(startTime, endTime)
        {
            this.Node = state;

            argumentsValues = new Dictionary<String, List<SymbolValue>>();

            argumentsCallQueue = new Queue<Tuple<String, CallContext>>();
            ddArgumentsValueQueue = new Queue<ArgumentsValues>();
            expressionArgumentsValueQueue = new Queue<ArgumentsValues>();
            parameterQueue = new Queue<IParameterInfo>();

            CurrentNode = new CalcPosition()
            {
                NodeID = state.NodeInfo.CalcNode.NodeID,
                Intime = CalcPosition.IntimeIdentification.Runtime
            };
        }

        public ArgumentsValues Arguments { get; set; }

        public OptimizationState Node { get; protected set; }

        public override DateTime GetStartTime(int tau)
        {
            return Node.NodeInfo.Interval.GetTime(startTime, -tau);
        }

        public override Interval GetInterval(int tau, int tautill)
        {
            return Node.NodeInfo.Interval.Multiply(tautill - tau + 1);
        }

        DateTime startTime, endTime;

        /// <summary>
        /// Возможные значения для каждого аргумента
        /// </summary>
        Dictionary<String, List<SymbolValue>> argumentsValues;

        /// <summary>
        /// Значение выражения при оптимальных (на данный момент) значениях аргументов
        /// </summary>
        SymbolValue optimalExpressionValue;

        /// <summary>
        /// Оптимальные (на данный момент) значения аргументов
        /// </summary>
        ArgumentsValues optimalArguments;

        /// <summary>
        /// Значения аргументов переданные явно
        /// </summary>
        ArgumentsValues explicitArguments;

        /// <summary>
        /// Значения аргументов базовой(-ых) оптимизации(-ий)
        /// </summary>
        ArgumentsValues baseArguments;

        IOptimizationInfo toppestOptimizationToCalc;

        /// <summary>
        /// Текущий расчитываемый аргумент
        /// </summary>
        String currentArgumentName;

        /// <summary>
        /// Текущее значение аргументов для которых производятся расчёты
        /// </summary>
        ArgumentsValues currentArgument;

        /// <summary>
        /// Очередь для расчёта значений аргументов
        /// (имя аргумента, контекст для запуска)
        /// </summary>
        Queue<Tuple<String, CallContext>> argumentsCallQueue;

        /// <summary>
        /// Очередь аргументов для расчёта области определения
        /// </summary>
        Queue<ArgumentsValues> ddArgumentsValueQueue;

        /// <summary>
        /// Очередь аргументов для расчёта выражения
        /// </summary>
        Queue<ArgumentsValues> expressionArgumentsValueQueue;

        /// <summary>
        /// Очередь для расчёта дочерних параметров
        /// </summary>
        Queue<IParameterInfo> parameterQueue;

        internal enum ContextState
        {
            /// <summary>
            /// Переход к следующему интервалу
            /// </summary>
            NextTime,
            /// <summary>
            /// Получение (запрос) значений базовой оптимизации
            /// </summary>
            GetBaseArguments,
            /// <summary>
            /// 
            /// </summary>
            PrepareCallArgumentsContext,
            /// <summary>
            /// Расчёт возможных значений аргументов
            /// </summary>
            NextArgument,
            /// <summary>
            /// Принять результаты по ококончанию расчёта значений
            /// </summary>
            ReturnArgumentValue,
            ///// <summary>
            ///// Перейти к расчёту следующего набора аргументов
            ///// </summary>
            //NextArgumentSet,
            CallDD,
            CallExpression,
            /// <summary>
            /// Принять результаты по окончанию расчёта области определения
            /// </summary>
            ReturnDDValue,
            /// <summary>
            /// Принять результаты по окончанию расчёта критерия
            /// </summary>
            ReturnExpressionValue,
            /// <summary>
            /// Расчёт дочерних параметров
            /// </summary>
            CallChildParameters,
            /// <summary>
            /// Выйти из оптимизации
            /// </summary>
            ExitOptimization,
        }

        ContextState currentContextState;

        public override string GetStatusString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("Расчёт оптимизации {0}.", Node.NodeInfo);
            switch (currentContextState)
            {
                case ContextState.ReturnArgumentValue:
                    builder.AppendFormat(" Расчёт допустимых значений параметра '{0}'.", currentArgumentName);
                    break;
                case ContextState.CallDD:
                case ContextState.ReturnDDValue:
                    builder.AppendFormat(" Расчёт области определения при ({0}).", currentArgument);
                    break;
                case ContextState.CallExpression:
                case ContextState.ReturnExpressionValue:
                    builder.AppendFormat(" Расчёт выражения при ({0}).", currentArgument);
                    break;
            }

            return builder.ToString();
        }

        internal bool PrepareArgs(ICalcContext calcContext)
        {
            if (toppestOptimizationToCalc == null)
            {
                explicitArguments = GetArguments(calcContext, Node.NodeInfo as IOptimizationInfo, out toppestOptimizationToCalc);

                if (toppestOptimizationToCalc == null)
                {
                    // !Выход из оптимиации
                    calcContext.AddMessage(new CalcMessage(MessageCategory.Error, "Ошибка оптимизации. Все аргументы переданы явно"));

                    calcContext.Return();
                    return false;
                }
            }
            return true;
        }

        public override void Prepare(ICalcContext calcContext)
        {
            if (!PrepareArgs(calcContext))
                return;

            // не могу ничего придумать, так что будет конечный автомат
            while (calcContext.SymbolTable.CallContext == this)
            {
                switch (currentContextState)
                {
                    case ContextState.NextTime:
                        currentContextState = NextTime(calcContext);
                        break;
                    case ContextState.GetBaseArguments:
                        currentContextState = GetOptimalBaseArguments(calcContext);
                        break;
                    case ContextState.PrepareCallArgumentsContext:
                        currentContextState = PrepareArgumentContexts(calcContext);
                        break;
                    case ContextState.NextArgument:
                        currentContextState = NextArgument(calcContext);
                        break;
                    case ContextState.ReturnArgumentValue:
                        currentContextState = ReturnArgument(calcContext);
                        break;
                    case ContextState.CallDD:
                        currentContextState = CallDD(calcContext);
                        break;
                    case ContextState.ReturnDDValue:
                        currentContextState = ReturnDDValue(calcContext);
                        break;
                    case ContextState.CallExpression:
                        currentContextState = CallExpression(calcContext);
                        break;
                    case ContextState.ReturnExpressionValue:
                        currentContextState = ReturnExpressionValue(calcContext);
                        break;
                    case ContextState.CallChildParameters:
                        currentContextState = NextChildParameter(calcContext);
                        break;
                    case ContextState.ExitOptimization:
                        calcContext.Return();
                        break;
                    default:
                        currentContextState = ContextState.NextTime;
                        break;
                }
            }
        }

        internal ContextState CallExpression(ICalcContext calcContext)
        {
            if (expressionArgumentsValueQueue.Count > 0)
            {
                currentArgument = expressionArgumentsValueQueue.Dequeue();

                if (Node.ExpressionBody == null)
                {
                    return CallChildParameters(currentArgument);
                }
                else
                {
                    Variable retVariable = calcContext.SymbolTable.GetSymbol(CalcContext.ReturnVariableName) as Variable;
                    if (retVariable == null)
                        calcContext.SymbolTable.DeclareSymbol(retVariable = new Variable(CalcContext.ReturnVariableName));
                    retVariable.Value = SymbolValue.Nothing;

                    // передача значений аргументов
                    foreach (var argumentName in currentArgument)
                    {
                        Variable var = calcContext.SymbolTable.GetSymbol(argumentName) as Variable;
                        if (var == null)
                            calcContext.SymbolTable.DeclareSymbol(var = new Variable(argumentName));

                        var.Value = SymbolValue.CreateValue(currentArgument[argumentName]);
                    }

                    calcContext.Call(new CommonContext(
                        new CalcPosition()
                        {
                            NodeID = Node.NodeInfo.CalcNode.NodeID,
                            CurrentPart = CalcPosition.NodePart.Expression
                        },
                        String.Format("Расчёт выражения '{0}' при '{1}'", Node.NodeInfo, currentArgument),
                        startTime,
                        Node.NodeInfo.Interval,
                        Node.ExpressionBody));

                    return ContextState.ReturnExpressionValue;
                }
            }
            else
            {
                // сохраняем результат оптимизации
                calcContext.ValuesKeeper.SetOptimal(Node.NodeInfo.CalcNode, explicitArguments, startTime, optimalArguments);

                // расчёт всех параметров при оптимальных значениях аргументов
                if (!(Node.NodeInfo as IOptimizationInfo).CalcAllChildParameters
                    && optimalArguments != ArgumentsValues.BadArguments)
                {
                    return CallChildParameters(optimalArguments);
                }
                else return ContextState.NextTime;
            }
        }

        private ContextState CallChildParameters(ArgumentsValues args)
        {
            foreach (var item in (Node.NodeInfo as IOptimizationInfo).ChildParameters)
            {
                parameterQueue.Enqueue(item);
            }
            currentArgument = args;
            return ContextState.CallChildParameters;
        }

        internal ContextState CallDD(ICalcContext calcContext)
        {
            // если области определения нет, переносим все значения аргументы для расчёта оптимизационного выражения
            if (Node.DDBody == null)
            {
                expressionArgumentsValueQueue = ddArgumentsValueQueue;
                ddArgumentsValueQueue = new Queue<ArgumentsValues>();
                return ContextState.CallExpression;
            }

            if (ddArgumentsValueQueue.Count > 0)
            {
                currentArgument = ddArgumentsValueQueue.Dequeue();

                Variable retVariable = calcContext.SymbolTable.GetSymbol(CalcContext.ReturnVariableName) as Variable;
                if (retVariable == null)
                    calcContext.SymbolTable.DeclareSymbol(retVariable = new Variable(CalcContext.ReturnVariableName));
                retVariable.Value = SymbolValue.Nothing;

                // передача значений аргументов
                foreach (var argumentName in currentArgument)
                {
                    Variable var = calcContext.SymbolTable.GetSymbol(argumentName) as Variable;
                    if (var == null)
                        calcContext.SymbolTable.DeclareSymbol(var = new Variable(argumentName));

                    var.Value = SymbolValue.CreateValue(currentArgument[argumentName]);
                }

                // вызов расчёта области определения для данного аргумента
                calcContext.Call(new CommonContext(
                    new CalcPosition()
                    {
                        NodeID = Node.NodeInfo.CalcNode.NodeID,
                        CurrentPart = CalcPosition.NodePart.DefinitionDomain
                    },
                    String.Format("Расчёт области определения '{0}' при '{1}'", Node.NodeInfo, currentArgument),
                    startTime,
                    Node.NodeInfo.Interval,
                    Node.DDBody));
                return ContextState.ReturnDDValue;
            }
            else
                return ContextState.CallExpression;
        }

        /// <summary>
        /// Перейти к расчёту следующего времени. 
        /// При первом запуске время расчёта выставляется на первый запрашиваемый интервал.
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <returns>
        /// ContextState.GetBaseArguments 
        /// или ContextState.ExitOptimization, если время расчёта превысило CalcEndTime 
        /// </returns>
        internal ContextState NextTime(ICalcContext calcContext)
        {
            if (startTime == DateTime.MinValue)
                startTime = CalcStartTime;
            else
                startTime = endTime;
            endTime = Node.NodeInfo.Interval.GetNextTime(startTime);

            // !Выход из оптимиации
            // если перебрали всё время, выход
            if (startTime >= CalcEndTime)
                return ContextState.ExitOptimization;

            // сбрасываем данные за предыдущий интервал
            optimalExpressionValue = SymbolValue.Nothing;
            optimalArguments = ArgumentsValues.BadArguments; //null;

            return ContextState.GetBaseArguments;
        }

        /// <summary>
        /// Получить оптимальные аргументы для базовых оптимизаций, 
        /// чьи аргументы не были переданны явно.
        /// По необходимости, вызвать расчёт базовой оптимизации.
        /// Так же запрашивается значение оптимальных аргументов для текущей оптимизации 
        /// и, если они есть, текущие время расчёта пропускается.
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <returns>
        /// ContextState.CallArgumentValue - переход к расчёту возможных значений аргументов;
        /// <br />
        /// ContextState.GetBaseArguments - была вызванна базовая оптимизация, 
        /// по завершению её расчёта вернуть к получению значений аргументов базовой оптимизации;
        /// <br />
        /// ContextState.NextTime - за данное время расчёта оптимальные аргументы уже есть.
        /// </returns>
        internal ContextState GetOptimalBaseArguments(ICalcContext calcContext)
        {
            IOptimizationInfo optimization = Node.NodeInfo as IOptimizationInfo;
            Stack<IOptimizationInfo> optimizationStack = new Stack<IOptimizationInfo>();

            while (optimization != null)
            {
                optimizationStack.Push(optimization);
                optimization = optimization.Optimization;
                if (optimization == toppestOptimizationToCalc)
                    optimization = null;
            }

            ArgumentsValues prevArgs = explicitArguments;
            ArgumentsValues args = explicitArguments;

            while (optimizationStack.Count > 0)
            {
                optimization = optimizationStack.Pop();

                // Если аргументы не переданны явно, запросить оптимальные аргументы
                if (prevArgs == null || !prevArgs.Include((from a in optimization.Arguments select a.Name).ToArray()))
                {
                    args = calcContext.ValuesKeeper.GetOptimal(optimization.CalcNode, prevArgs, startTime);
                }
                if (optimization == Node.NodeInfo)
                {
                    // если оптимизация уже посчитана, переход к следующему времени
                    if (args != null)
                        return ContextState.NextTime;
                }
                else
                {
                    // вызвать базовую оптимизацию, если нужно
                    if (args == null)
                    {
                        calcContext.Call(startTime, endTime/*CalcEndTime*/, new CalcNodeKey(optimization.CalcNode, prevArgs));
                        return ContextState.GetBaseArguments;
                    }
                    // !Переход к следующему времени
                    // базовая оптимизация закончилась неудачей, перейти к следующему времени
                    else if (args.Count == 0)
                        return ContextState.NextTime;
                    else
                    {
                        prevArgs = args;
                    }
                }
            }
            baseArguments = prevArgs;
            return ContextState.PrepareCallArgumentsContext;
        }

        /// <summary>
        /// Подготовить значения аргументов (для аргументов нерасчитываемого типа)
        /// и подготовить контексты для расчёта аргументов расчитываемого типа.
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <returns>
        /// ContextState.CallArgumentValue - переход к расчёту аргументов;
        /// <br />
        /// ContextState.ExitOptimization - ошибка расчёта оптимизации.
        /// </returns>
        internal ContextState PrepareArgumentContexts(ICalcContext calcContext)
        {
            IOptimizationInfo optimization = Node.NodeInfo as IOptimizationInfo;

            int manualCount = (from a in optimization.Arguments where a.Mode == OptimizationArgumentMode.Manual select a).Count();

            if (0 < manualCount && manualCount < optimization.Arguments.Length)
            {
                Node.Failed = true;
                calcContext.AddMessage(new CalcMessage(MessageCategory.Error, "У оптимизации '{0}' часть аргументов вводяться вручную, часть расчитываемая.", Node.NodeInfo.Name));
                return ContextState.ExitOptimization;
            }

            foreach (var item in optimization.Arguments)
            {
                switch (item.Mode)
                {
                    case OptimizationArgumentMode.Default:
                        break;
                    case OptimizationArgumentMode.Manual:
                        // если один аргумент ручной, все остальные должны быть ручными, иначе ошибка
                        ArgumentsValues[] manualArgs = calcContext.GetManualArgValues(optimization, baseArguments, startTime);
                        foreach (var args in manualArgs)
                        {
                            ddArgumentsValueQueue.Enqueue(args);
                        }
                        return ContextState.CallDD;
                        break;
                    case OptimizationArgumentMode.Interval:
                        List<SymbolValue> valueList = new List<SymbolValue>(); ;
                        for (double intervalItem = item.IntervalFrom; intervalItem < item.IntervalTo; intervalItem += item.IntervalStep)
                        {
                            valueList.Add(SymbolValue.CreateValue(intervalItem));
                        }
                        argumentsValues[item.Name] = valueList;
                        break;
                    case OptimizationArgumentMode.Expression:
                        Instruction[] body = Node.GetArgumentBody(item.Name);

                        // !Выход из оптимиации
                        // сообщить об ошибке и перейти к следующему времени
                        if (body == null)
                        {
                            calcContext.AddMessage(new CalcMessage(MessageCategory.Error, ""));
                            return ContextState.ExitOptimization;
                        }

                        CallContext callContext = new CommonContext(
                            new CalcPosition()
                            {
                                NodeID = Node.NodeInfo.CalcNode.NodeID,
                                CurrentPart = CalcPosition.NodePart.ArgumentExpression,
                                AdditionNote = item.Name
                            },
                            String.Format("Расчёт допустиых значения аргумента {1} '{0}'", Node.NodeInfo, item.Name),
                            startTime,
                            Node.NodeInfo.Interval,
                            body);
                        argumentsCallQueue.Enqueue(Tuple.Create(item.Name, callContext));
                        break;
                    case OptimizationArgumentMode.ColumnNum:
                        break;
                    default:
                        break;
                }
            }
            return ContextState.NextArgument;
        }

        /// <summary>
        /// Переход к расчёту допустимых значений следующего аргумента.
        /// Если значения для всех аргументов расчитанны (или нет ни одного расчитываемого аргумента), 
        /// перейти к составлению списка всех доступных значений аргументов.
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <returns>
        /// ContextState.ReturnArgumentValue - запущен расчёт аргумента, принять его результаты;
        /// <br />
        /// ContextState.NextArgumentSet - перейти к расчёту оптимизации для каждого возможного значения аргументов.
        /// </returns>
        internal ContextState NextArgument(ICalcContext calcContext)
        {
            if (argumentsCallQueue.Count > 0)
            {
                var call = argumentsCallQueue.Dequeue();

                currentArgumentName = call.Item1;
                calcContext.Call(call.Item2);

                // сбросить значение переменной @ret
                Variable retVar = calcContext.SymbolTable.GetSymbol(CalcContext.ReturnVariableName) as Variable;
                if (retVar == null)
                    calcContext.SymbolTable.DeclareSymbol(retVar = new Variable(CalcContext.ReturnVariableName));
                retVar.Value = SymbolValue.Nothing;

                return ContextState.ReturnArgumentValue;
            }
            else
            {
                CollectArgumentsValues();
                return ContextState.CallDD;
            }
        }

        /// <summary>
        /// Составить список всех возможных значений аргументов
        /// </summary>
        private void CollectArgumentsValues()
        {
            ArgumentsValues args;

            List<ArgumentsValues> argumentsList = new List<ArgumentsValues>();
            List<ArgumentsValues> tempList = new List<ArgumentsValues>();

            argumentsList.Add(baseArguments);

            foreach (var name in argumentsValues.Keys)
            {
                foreach (var argument in argumentsList)
                {
                    foreach (var item in argumentsValues[name])
                    {
                        args = new ArgumentsValues(argument);
                        args[name] = (double)item;
                        tempList.Add(args);
                    }
                }
                argumentsList = tempList;
                tempList = new List<ArgumentsValues>();
            }

            foreach (var item in argumentsList)
            {
                ddArgumentsValueQueue.Enqueue(item);
            }
        }

        /// <summary>
        /// Принять результаты расчёта допустимых значений аргумента.
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <returns>
        /// ContextState.NextArgument - расчёт значений следующего аргумента;
        /// <br />
        /// ContextState.NextTime - переход к следующему времени.
        /// </returns>
        internal ContextState ReturnArgument(ICalcContext calcContext)
        {
            Variable retVariable = calcContext.SymbolTable.GetSymbol(CalcContext.ReturnVariableName) as Variable;

            List<SymbolValue> valueList = new List<SymbolValue>();

            if (retVariable.Value == SymbolValue.Nothing)
            {
                calcContext.AddMessage(new CalcMessage(MessageCategory.Error, "Ошибка оптимизации '{0}' за '{1:yyyy-MM-dd}'. У аргумента '{2}' нет допустимых значенний", Node.NodeInfo.Name, startTime, currentArgumentName));
                calcContext.ValuesKeeper.SetOptimal(Node.NodeInfo.CalcNode, explicitArguments, startTime, ArgumentsValues.BadArguments);
                return ContextState.NextTime;
            }
            else if (retVariable.Value is ArrayValue)
                for (int i = 0; i < retVariable.Value.ArrayLength(calcContext); i++)
                {
                    valueList.Add(retVariable.Value.ArrayAccessor(calcContext, i));
                }
            else
                valueList.Add(retVariable.Value);

            argumentsValues[currentArgumentName] = valueList;

            return ContextState.NextArgument;
        }

        /// <summary>
        /// Принять результаты расчёта области определения и, 
        /// в зависимости, от них перейти к расчёту критерия оптимизации
        /// или к расчёту следующего аргумента.
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <returns>
        /// ContextState.ReturnExpressionValue - переход к расчёту критерия оптимизации;
        /// <br />
        /// ContextState.NextArgumentSet - переход к расчёту следующего набора аргументов
        /// </returns>
        internal ContextState ReturnDDValue(ICalcContext calcContext)
        {
            Variable retVariable = calcContext.SymbolTable.GetSymbol(CalcContext.ReturnVariableName) as Variable;

            // если область определения возвращает TRUE, добавляем значения аргументов в одобренные
            if (retVariable.Value != SymbolValue.Nothing && !retVariable.Value.IsZero(calcContext))
            {
                expressionArgumentsValueQueue.Enqueue(currentArgument);
            }

            return ContextState.CallDD;
        }

        /// <summary>
        /// Принять результаты критерия оптимизации.
        /// При IOptimizationInfo.CalcAllChildParameters == true, 
        /// перейти к расчёту дочерних параметров.
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <returns>
        /// ContextState.CallChildParameters - перейти к расчёту дочерних параметров;
        /// <br />
        /// ContextState.NextArgumentSet - перейти к расчёту следующих аргументов;
        /// </returns>
        internal ContextState ReturnExpressionValue(ICalcContext calcContext)
        {
            Variable retVariable = calcContext.SymbolTable.GetSymbol(CalcContext.ReturnVariableName) as Variable;

            // TODO добавить юниттест
            calcContext.ValuesKeeper.AddCalculatedValue(Node.NodeInfo.CalcNode, currentArgument, StartTime, retVariable.Value);

            if (retVariable.Value != SymbolValue.Nothing)
            {
                IOptimizationInfo optimizationInfo = Node.NodeInfo as IOptimizationInfo;

                if (optimalExpressionValue == SymbolValue.Nothing
                   || ((Node.NodeInfo as IOptimizationInfo).Maximalize && retVariable.Value.Greater(calcContext, optimalExpressionValue) == SymbolValue.TrueValue)
                    || (!(Node.NodeInfo as IOptimizationInfo).Maximalize && retVariable.Value.Less(calcContext, optimalExpressionValue) == SymbolValue.TrueValue))
                {
                    optimalExpressionValue = retVariable.Value;
                    optimalArguments = currentArgument;
                }
                if (optimizationInfo.CalcAllChildParameters)
                {
                    return CallChildParameters(currentArgument);
                }
            }

            return ContextState.CallExpression;
        }

        /// <summary>
        /// Перейти к расчёту следующего дочернего параметра.
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <returns>
        /// ContextState.CallChildParameters - продолжить расчёт дочерних параметров;
        /// <br />
        /// ContextState.NextArgumentSet - перейти к расчёту следующего набора аргументов, когда все дочернии параметры закончились;
        /// <br />
        /// ContextState.NextTime - перейти к следующему времени расчёта, когда все дочернии параметры закончились.
        /// </returns>
        internal ContextState NextChildParameter(ICalcContext calcContext)
        {
            if (parameterQueue.Count > 0)
            {
                var currentParameter = parameterQueue.Dequeue();

                calcContext.Call(StartTime, EndTime, new CalcNodeKey(currentParameter.CalcNode, currentArgument));

                return ContextState.CallChildParameters;
            }
            else if ((Node.NodeInfo as IOptimizationInfo).CalcAllChildParameters || Node.ExpressionBody == null)
                return ContextState.CallExpression;
            else
                return ContextState.NextTime;
        }

        public override void Return(ICalcContext calcContext)
        {
            base.Return(calcContext);
        }

        private ArgumentsValues GetArguments(ICalcContext calcContext, IOptimizationInfo optmimzationInfo, out IOptimizationInfo skipedFrom)
        {
            ArgumentsValues arguments;

            if (optmimzationInfo.Optimization != null)
            {
                arguments = GetArguments(calcContext, optmimzationInfo.Optimization, out skipedFrom);
            }
            else
            {
                arguments = new ArgumentsValues();
                skipedFrom = null;
            }

            foreach (var argument in optmimzationInfo.Arguments)
            {
                Variable var = calcContext.SymbolTable.GetSymbol(argument.Name) as Variable;

                if (var == null)
                {
                    skipedFrom = optmimzationInfo;
                    break;
                }
                arguments[argument.Name] = (double)var.Value;
            }

            if (arguments != null && arguments.Count == 0)
                return null;

            return arguments;
        }

        public override bool TheSame(ICallContext callContext)
        {
            OptimizationContext optimizationContext;
            if ((optimizationContext = callContext as OptimizationContext) != null)
            {
                // для разных ревизий всегда false
                if (!optimizationContext.Node.Revision.Equals(Node.Revision))
                    return false;

                bool ret = optimizationContext.Node.NodeInfo.Equals(Node.NodeInfo)
                    && ((Arguments == null && (optimizationContext.Arguments == null || optimizationContext.Arguments.Count == 0))
                    || (Arguments != null && Arguments.Equals(optimizationContext.Arguments))
                    || (optimizationContext.Arguments != null && optimizationContext.Arguments.Include(Arguments)));
                return ret;
            }
            return base.TheSame(callContext);
        }

        public override string ToString()
        {
            return String.Format("Оптимизация {0}", Node.NodeInfo);
        }
    }
}

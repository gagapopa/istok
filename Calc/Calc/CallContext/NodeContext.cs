using System;
using System.Collections.Generic;
using System.Text;
using gppg;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Контекст для вычисления значений параметра
    /// </summary>
    class NodeContext : CallContext
    {
        /// <summary>
        /// Информация о вычисляемом параметре
        /// </summary>
        public NodeState Node { get; protected set; }

        ICalcSupplier calcSupplier;

        public override string GetStatusString()
        {
            return String.Format("Расчёт параметра {0}", Node.NodeInfo);
        }

        /// <summary>
        /// Узел оптимизации
        /// </summary>
        public OptimizationState OptimizationState { get; set; }

        public ArgumentsValues Arguments { get; set; }

        /// <summary>
        /// текущие расчитываемое время
        /// </summary>
        protected DateTime startTime, endTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">Вызывающий контекст</param>
        /// <param name="node">Информация о вычисляемом параметре</param>
        /// <param name="startTime">Началное вычисляемое время</param>
        /// <param name="endTime">Конечное вычисляемое время</param>
        public NodeContext(ICalcSupplier calcSupplier, NodeState node, DateTime startTime, DateTime endTime)
            : base(startTime, endTime)
        {
            Node = node;
            this.calcSupplier = calcSupplier;
            Fail = Node.NodeInfo.Interval == Interval.Zero;
            CurrentNode = new CalcPosition()
            {
                NodeID = node.NodeInfo.CalcNode.NodeID,
                Intime = CalcPosition.IntimeIdentification.Runtime,
                CurrentPart = CalcPosition.NodePart.Formula
            };
            body = Node.Body;
            this.CalcStartTime = Node.NodeInfo.Interval.NearestEarlierTime(startTime);
            this.CalcEndTime = Node.NodeInfo.Interval.NearestLaterTime(endTime);
        }

        public override void Prepare(ICalcContext calcContext)
        {
            base.Prepare(calcContext);
            //Fail = Fail || Node.NodeInfo.Interval == Interval.Zero;

            IOptimizationInfo implicitFrom = null;

            ArgumentsValues arguments = null;

            if (Node.NodeInfo.Optimization != null)
                arguments = GetArguments(calcContext, Node.NodeInfo.Optimization, out implicitFrom);

            SymbolValue value = SymbolValue.Nothing;

            DateTime start = startTime, end = endTime;

            while (value != null)
            {
                if (DateTime.MinValue.Equals(start))
                {
                    // запросить сырые данные из БД
                    GetRawValues(calcContext, Node.NodeInfo, arguments);

                    start = CalcStartTime = Node.NodeInfo.Interval.NearestEarlierTime(/*Node.NodeInfo.StartTime,*/ CalcStartTime);
                
                    if (Fail)
                    {
                        calcContext.ValuesKeeper.SetFailNode(Node.NodeInfo, calcContext.StartTime, calcContext.EndTime);
                        calcContext.Return();
                        return;
                    }
                }
                else
                    start = end;
                end = Node.NodeInfo.Interval.GetNextTime(start);

                // выход из расчёта параметра
                //if (start >= CalcEndTime 
                //    || end >= CalcEndTime)
                if (end > CalcEndTime
                    || end > calcContext.EndTime
                    || start == end)
                {
                    this.startTime = start;
                    this.endTime = end;
                    calcContext.Return();
                    return;
                }

                value = calcContext.ValuesKeeper.GetRawValue(Node.NodeInfo.CalcNode, arguments, start);

                // если есть неявная передача аргументов
                if (value == null && implicitFrom != null)
                {
                    Stack<IOptimizationInfo> baseOptimizationStack = new Stack<IOptimizationInfo>();

                    IOptimizationInfo optimizationInfo = Node.NodeInfo.Optimization;

                    while (!implicitFrom.Equals((baseOptimizationStack.Count > 0 ? baseOptimizationStack.Peek() : null)))
                    {
                        baseOptimizationStack.Push(optimizationInfo);
                        optimizationInfo = optimizationInfo.Optimization;
                    }

                    ArgumentsValues baseArguments = arguments;
                    ArgumentsValues currentArguments = arguments;

                    // брать оптимальные аргументы из кэша расчёта
                    while (baseOptimizationStack.Count > 0 && (currentArguments != null || currentArguments == baseArguments))
                    {
                        optimizationInfo = baseOptimizationStack.Pop();
                        baseArguments = currentArguments;
                        currentArguments = calcContext.ValuesKeeper.GetOptimal(optimizationInfo.CalcNode, baseArguments, start);

                        // сообщить об отсутствии оптимального решения
                        if (currentArguments == ArgumentsValues.BadArguments)
                        {
                            value = SymbolValue.Nothing;
                            // тут по идее нужно писать базовые аргументы...
                            // и ValuesKeeper должен ориентироваться на базовые аргументы
                            calcContext.ValuesKeeper.AddCalculatedValue(Node.NodeInfo.CalcNode, baseArguments, start, value);
                            break; 
                        }

                        // вызвать расчёт оптимизации, если в кэше нет достаточно аргументов
                        if (currentArguments == null)
                        {
                            calcContext.Call(start, CalcEndTime, new CalcNodeKey(optimizationInfo.CalcNode, baseArguments));
                            NeedPrepare = true;
                            return;
                        }
                    }
                    // переходим к следующему времени 
                    if (value != null)
                        continue;

                    value = calcContext.ValuesKeeper.GetRawValue(Node.NodeInfo.CalcNode, currentArguments, start);

                    if (value != null)
                        calcContext.ValuesKeeper.AddRawValue(Node.NodeInfo.CalcNode, arguments, start, value);
                    else
                    {
                        // вызвать расчёт параметра с полученными аргументами
                        calcContext.Call(start, end, new CalcNodeKey(Node.NodeInfo.CalcNode, currentArguments));
                        NeedPrepare = true;
                        return;
                    }
                }
                // Вызов расчёта зависимостей за данное время
                else if (value == null
                    && CallNeededNodes(calcContext, arguments, start, end))
                {
                    NeedPrepare = true;
                    return;
                }
                this.startTime = start;
                this.endTime = end;
            }
        }

        /// <summary>
        /// Вызвать по необходимости зависимые параметры.
        /// </summary>
        /// <remarks>
        /// Вызываемый параметр должен иметь теже аргументы, что и вызывающий
        /// </remarks>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <param name="currentArguments">Аргументы переданные параметру</param>
        /// <param name="start">Начальное время запрашиваемого расчёта</param>
        /// <param name="end">Конечное время запрашиваемого расчёта</param>
        /// <returns>
        /// Если был вызван расчёт зависимого параметра, возвращает true.
        /// В противном случае false.
        /// </returns>
        private bool CallNeededNodes(
            ICalcContext calcContext, 
            ArgumentsValues currentArguments,
            DateTime start,
            DateTime end)
        {
            if (Node.NodeInfo.Needed != null)
            {
                foreach (var needCode in Node.NodeInfo.Needed)
                {
                    var nodeInfo = calcContext.GetParameterNode(Node.Revision, needCode);

                    // Зависимость не найдена, 
                    // сообщить об ошибке, отметить вызывающий параметр как не расчитываемый
                    if (nodeInfo == null)
                    {
                        calcContext.AddMessage(new CalcMessage(
                            MessageCategory.Error,
                            "Параметра {0} не может быть расчитан из-за не удовлетворенных зависимостей. Параметр ${1}$ Не найден.", 
                            Node.NodeInfo, needCode));
                        calcContext.ValuesKeeper.SetFailNode(Node.NodeInfo, start, end);
                        return true;
                    }
                    // Если оптимизация у вызывающего и вызываемого парамтра различна,
                    // сообщить об ошибке, отметить вызывающий параметр как не расчитываемый
                    else if ((nodeInfo.Optimization != null && Node.NodeInfo.Optimization != null && !nodeInfo.Optimization.Equals(Node.NodeInfo.Optimization))
                        || (nodeInfo.Optimization == null && Node.NodeInfo.Optimization != null)
                        || (nodeInfo.Optimization != null && Node.NodeInfo.Optimization == null))
                    {
                        calcContext.AddMessage(new CalcMessage(
                            MessageCategory.Error,
                            "Для параметра {0} зависимость от {1} не может быть удовлетворена",
                            Node.NodeInfo,
                            nodeInfo));
                        calcContext.ValuesKeeper.SetFailNode(Node.NodeInfo, start, end);
                        return true;
                    }

                    bool call = false;
                    var node = nodeInfo.CalcNode;

                    for (
                        DateTime time = nodeInfo.Interval.NearestEarlierTime(/*nodeInfo.StartTime,*/ start);
                        !call && time < end;
                        time = nodeInfo.Interval.GetNextTime(time))
                    {
                        call = !calcContext.ValuesKeeper.IsCalculated(node, currentArguments, time);
                    }

                    // вызвать расчёт, если не хватает значений
                    if (call)
                    {
                        calcContext.Call(start, end, new CalcNodeKey(nodeInfo.CalcNode, currentArguments));
                        return true;
                    }
                }
            }

            return false;
        }

        private void GetRawValues(ICalcContext calcContext, ICalcNodeInfo calcNodeInfo, ArgumentsValues arguments)
        {
            List<Message> messages;
            bool serverNotAccessible;

            var values = calcSupplier.GetParameterNodeRawValues(calcContext, calcNodeInfo.CalcNode, arguments, CalcStartTime, CalcEndTime, out messages, out serverNotAccessible);

            if (messages != null)
                calcContext.AddMessage(messages);

            if (values != null)
                foreach (var valueItem in values)
                {
                    if (!calcContext.ForceCalc(Node.NodeInfo) || valueItem is CorrectedParamValueItem)
                    {
                        calcContext.ValuesKeeper.AddRawValue(calcNodeInfo.CalcNode, valueItem.Arguments, valueItem.Time, SymbolValue.CreateValue(valueItem.Value));
                    }
                }
        }

        private ArgumentsValues GetArguments(ICalcContext calcContext, IOptimizationInfo optmimzationInfo, out IOptimizationInfo skipedFrom)
        {
            ArgumentsValues arguments;

            if (optmimzationInfo.Optimization != null)
            {
                arguments = GetArguments(calcContext, optmimzationInfo.Optimization, out skipedFrom);
                if (skipedFrom != null)
                    return arguments;
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

            if (arguments.Count == 0)
                return null;

            return arguments;
        }

        /// <summary>
        /// Очищает таблицу символов, добавляет сообщение базовому контексту и сдвигает текущие расчитываемое время
        /// Если сдвинутое время выходит за диапазон ParameterEndTime, выполнение передается базовому контексту
        /// В противном случае переходит на начало кода в текущем контексте
        /// </summary>
        /// <returns>сам себя или базовый контекст, если вычисление параметра завершено</returns>
        public override void Return(ICalcContext calcContext)
        {
            bool failed = Fail;
            Variable var = null;
            SymbolValue nodeValue;
            // очищается стэк, и сообщения об ошибках записываются в базовый контекст
            base.Return(calcContext);

            if (failed || (var = calcContext.SymbolTable.GetSymbol(COTES.ISTOK.Calc.CalcContext.ReturnVariableName) as Variable) != null)
            {
                IOptimizationInfo implicitOptimization = null;
                ArgumentsValues args = null;
                if (Blocked)
                {
                    nodeValue = SymbolValue.BlockedValue;
                }
                else if (failed || var.Value == null)
                    nodeValue = SymbolValue.Nothing;
                else nodeValue = var.Value;

                if (Node.NodeInfo.Optimization != null)
                    args = GetArguments(calcContext, Node.NodeInfo.Optimization, out implicitOptimization);

                if (DateTime.MinValue == startTime)
                {
                    calcContext.ValuesKeeper.SetFailNode(Node.NodeInfo, CalcStartTime, CalcEndTime);
                }
                else
                {
                    // сохранить значения в кэш оптимизационного расчета
                    calcContext.ValuesKeeper.AddCalculatedValue(Node.NodeInfo.CalcNode, args, startTime, nodeValue);
                }
            }
        }

        public override DateTime GetStartTime(int tau)
        {
            return Node.NodeInfo.Interval.GetTime(startTime, -tau);
        }

        public override Interval GetInterval(int tau, int tautill)
        {
            return Node.NodeInfo.Interval.Multiply(tautill - tau + 1);
        }

        public override bool TheSame(ICallContext callContext)
        {
            NodeContext nodeContext;
            if ((nodeContext = callContext as NodeContext) != null)
            {
                // для разных ревизий всегда false
                if (!nodeContext.Node.Revision.Equals(Node.Revision))
                    return false;

                bool ret = nodeContext.Node.NodeInfo.Equals(Node.NodeInfo)
                    && ((Arguments == null && (nodeContext.Arguments == null || nodeContext.Arguments.Count == 0))
                    || (Arguments != null && Arguments.Equals(nodeContext.Arguments)));
                return ret;
            }
            return base.TheSame(callContext);
        }

        public override string ToString()
        {
            return String.Format("Расчёт параметра {0}", Node.NodeInfo);
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using gppg;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Класс хранит информацию о расчете, такую как таблица символов и стек вызова
    /// </summary>
    class CalcContext : IDisposable, ICalcContext
    {
        public const int StartIndexPosition = -1;
        public const String TempVariablePrefix = "@";
        public const String ReturnVariableName = TempVariablePrefix + "ret";

        public RemoteFunctionManager FunctionManager { get; set; }

        /// <summary>
        /// Объект компилятора, для компиляции параметров во время расчёта
        /// </summary>
        public ICompiler Compiler { get; protected set; }

        public OperationState OperationState { get; protected set; }

        public SymbolTable SymbolTable { get; protected set; }

        HashSet<ICalcNode> calculedNodes = new HashSet<ICalcNode>();

        HashSet<ICalcNode> scheduledNodes = new HashSet<ICalcNode>();

        HashSet<ICalcNode> allNodes = new HashSet<ICalcNode>();

        public void AddParametersToCalc(IEnumerable<ICalcNode> calcNode)
        {
            //allNodes.AddRange(calcNode);
            //scheduledNodes.AddRange(calcNode);
            foreach (var item in calcNode)
            {
                allNodes.Add(item);
                scheduledNodes.Add(item);
            }
        }


        public bool IsParameterToCalc(ICalcNode calcNode)
        {
            return allNodes.Contains(calcNode);
        }

        public bool NextNode()
        {
            if (SymbolTable.CallContext != null)
                throw new InvalidOperationException("NextNode должен вызываться только когда предыдущий параметр расчитан");
            
            if (scheduledNodes.Count > 0)
            {
                //ICalcNode calcNode = scheduledNodes[0];
                ICalcNode calcNode = scheduledNodes.First();

                Call(StartTime, EndTime, new CalcNodeKey(calcNode, null));
                return true;
            }

            return false;
        }

        public bool Call(DateTime startTime, DateTime endTime, params CalcNodeKey[] calcNodes)
        {
            bool ret = true;
            int calledNodes = 0;

            foreach (var calcNodeTuple in calcNodes)
            {
                ICalcNode calcNode = calcNodeTuple.Node;

                // Если параметр запланирован на расчёт и запрашиваемое время пересекается со временем заания,
                // то расширить время расчёта и убрать параметр из задания
                if (scheduledNodes.Contains(calcNode)
                    && (StartTime <= startTime && startTime <= EndTime
                    || StartTime <= endTime && endTime <= EndTime))
                {
                    startTime = startTime < StartTime ? startTime : StartTime;
                    endTime = endTime > EndTime ? endTime : EndTime;

                    scheduledNodes.Remove(calcNode);
                }

                var contexts = CreateCallContext(calcNode, calcNodeTuple.Arguments, startTime, endTime);

                if (contexts == null || contexts.Count() == 0)
                    ret = false;

                foreach (var callContext in contexts)
                {
                    if (Call(callContext, true))
                    {
                        // передача аргументов оптимизации
                        if (calcNodeTuple.Arguments != null)
                            foreach (var name in calcNodeTuple.Arguments)
                            {
                                SymbolTable.DeclareSymbol(new Variable(name, calcNodeTuple.Arguments[name]));
                            }
                        calledNodes++;
                    }
                    else ret = false;
                }
            }
            return ret;
        }

        private bool Call(ICallContext callContext, bool isolated)
        {
            foreach (var context in SymbolTable.GetAllContext())
            {
                if (callContext.TheSame(context))
                {
                    if (callContext.ToString().Equals(context.ToString()))
                        AddMessage(new CalcMessage(MessageCategory.Error, "Ошибка в ходе расчёта. Попытка вызывать {0} дважды", callContext));
                    else
                        AddMessage(new CalcMessage(MessageCategory.Error, "Ошибка в ходе расчёта. Нельзя вызывать {0} после {1}", callContext, context));

                    return false;
                }
            }
            SymbolTable.PushSymbolScope(callContext, isolated);

            ObtainStatus();
            return true;
        }

        public bool Call(ICallContext callContext)
        {
            return Call(callContext, false);
        }

        #region CreateCallContext()
        /// <summary>
        /// Создать контекст выполнения для функции
        /// </summary>
        /// <param name="function">Расчитываемая функция</param>
        /// <param name="startTime">Начальное время</param>
        /// <param name="endTime">Конечное время</param>
        /// <returns></returns>
        internal ICallContext CreateCallContext(CustomFunction function, DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Создать контексты выполнения для расчёта параметра за указанное время.
        /// Если за запрашиваемый интервал присутствуют несколько ревизий, то будет создано по контексту на каждую ревизию
        /// </summary>
        /// <param name="calcNode"></param>
        /// <param name="startTime">Начальное время</param>
        /// <param name="endTime">Конечное время</param>
        /// <returns></returns>
        internal IEnumerable<ICallContext> CreateCallContext(ICalcNode calcNode, ArgumentsValues args, DateTime startTime, DateTime endTime)
        {
            var revisions = calcSupplier.GetRevisions(startTime, endTime).ToArray();

            if (revisions.Length > 1)
            {
                var contexts = new List<ICallContext>();

                DateTime start, end;

                for (int i = 0; i < revisions.Length; i++)
                {
                    RevisionInfo revision = revisions[i];
                    ICalcNodeInfo nodeInfo = calcNode.Revisions.Get(revision);
                    RevisionInfo nextRevision = null;

                    //if (nodeInfo.Calculable)
                    //{
                        if (i + 1 < revisions.Length)
                            nextRevision = revisions[i + 1];

                        // корректируем начальное время
                        if (startTime < revision.Time)
                            start = revision.Time;
                        else
                            start = startTime;

                        start = nodeInfo.Interval.NearestEarlierTime(/*nodeInfo.StartTime,*/ start);

                        // корректируем конечное время
                        if (nextRevision != null && endTime > nextRevision.Time)
                            end = nextRevision.Time;
                        else
                            end = endTime;

                        end = nodeInfo.Interval.NearestEarlierTime(/*nodeInfo.StartTime,*/ end);

                        if (start < end)
                        {
                            // создаем контекст
                            ICallContext callContext = CreateCallContext(GetState(calcNode, revision), args, start, end, revision);

                            if (callContext != null)
                                contexts.Add(callContext);
                        }
                    //}
                }

                return contexts;
            }
            else
            {
                RevisionInfo revision = revisions[0];
                ICallContext callContext = null;

                //if (calcNode.Revisions.Get(revision).Calculable)
                //{
                    ICalcState state = GetState(calcNode, revision);
                    callContext = CreateCallContext(state, args, startTime, endTime, revision);
                //}

                if (callContext != null)
                    return new ICallContext[] { callContext };
                else
                    return new ICallContext[] { };

            }
        }

        /// <summary>
        /// Создать контексты выполнения для расчёта параметра за указанное время и гарантированно для одной ревизии.
        /// </summary>
        /// <param name="state">Состояние параметра за указанную ревизию</param>
        /// <param name="startTime">Начальное время</param>
        /// <param name="endTime">Конечное время</param>
        /// <returns></returns>
        private ICallContext CreateCallContext(ICalcState state, ArgumentsValues args, DateTime startTime, DateTime endTime, RevisionInfo revision)
        {
            //if (!state.NodeInfo.Calculable)
            //{
            //    return null;
            //}
            if (state != null && !CompiledYet(state.NodeInfo.CalcNode, revision))
            {
                EnterCompiling(state);
                state.Compile(Compiler, this);
                LeaveCompiling();

                SetCompiled(state.NodeInfo.CalcNode, revision);
            }

            if (state.Failed)
            {
                // заполняем Nothing'ами
                ValuesKeeper.SetFailNode(state.NodeInfo, startTime, endTime);
            }
            else
            {
                // создаем контекст
                if (state is NodeState)
                {
                    var context = new NodeContext(calcSupplier, state as NodeState, startTime, endTime) { Arguments = args };
                    return context;
                }
                else if (state is OptimizationState)
                {
                    var context = new OptimizationContext(state as OptimizationState, startTime, endTime) { Arguments = args };
                    return context;
                }
            }

            return null;
        }
        #endregion

        private void ObtainStatus()
        {
            ICallContext callContext = SymbolTable.CallContext;

            if (callContext != null)
            {
                OperationState.StateString = callContext.GetStatusString();
            }
            else
            {
                OperationState.StateString = String.Empty;
            }

            OperationState.Progress = 100.0 * calculedNodes.Count / allNodes.Count;
        }

        public void Return()
        {
            ICalcState calcState;
            ICallContext callContext = SymbolTable.CallContext;
            SymbolTable.PopSymbolScope(callContext);

            if ((callContext is NodeContext && (calcState = (callContext as NodeContext).Node).Failed)
                || (callContext is OptimizationContext && (calcState = (callContext as OptimizationContext).Node).Failed))
                ValuesKeeper.SetFailNode(calcState.NodeInfo, callContext.CalcStartTime, callContext.CalcEndTime);


            SendNodeBack(callContext);

            ObtainStatus();
        }

        private void SendNodeBack(ICallContext callContext)
        {
            ICalcNode calcNode = null;

            if (callContext is NodeContext)
                calcNode = (callContext as NodeContext).Node.NodeInfo.CalcNode;
            else if (callContext is OptimizationContext)
                calcNode = (callContext as OptimizationContext).Node.NodeInfo.CalcNode;

            if (calcNode != null && allNodes.Contains(calcNode) && !calculedNodes.Contains(calcNode))
            {
                NodeBackState state = NodeBackState.Unknown;
                TimeValueStructure lastTimeValue = new TimeValueStructure();

                IEnumerable<DateTime> times = GetAllTimes(StartTime, EndTime, calcNode);

                foreach (DateTime time in times)
                {
                    SymbolValue value = ValuesKeeper.GetRawValue(calcNode, null, time);

                    //if (value == null)
                    //    return;
                    if (value == SymbolValue.BlockedValue)
                    {
                        state |= NodeBackState.Blocked;
                    }
                    else if (value == null || value == SymbolValue.Nothing)
                    {
                        state |= NodeBackState.Failed;
                    }
                    else
                    {
                        state |= NodeBackState.Success;
                        lastTimeValue.Time = time;
                        lastTimeValue.Value = (double)value.GetValue();
                    }
                    //if (state == NodeBackState.Middle)
                    //{
                    //    break;
                    //}
                }
                NodeBack back = new NodeBack(calcNode.NodeID, state) { TimeValue = lastTimeValue };
                OperationState.AddAsyncResult(back);
                calculedNodes.Add(calcNode);
            }
        }

        private IEnumerable<DateTime> GetAllTimes(DateTime beginTime, DateTime endTime, ICalcNode calcNode)
        {
            // возможно, лучше идти от конца в начало
            DateTime time = beginTime;

            ICalcNodeInfo currentInfo = calcNode.Revisions.Get(time);
            ICalcNodeInfo nextInfo;

            time = currentInfo.Interval.NearestEarlierTime(/*currentInfo.StartTime,*/ beginTime);

            yield return time;

            if (currentInfo.Interval == Interval.Zero)
            {
                yield break;
            }

            while (true)
            {
                time = currentInfo.Interval.GetNextTime(time);

                nextInfo = calcNode.Revisions.Get(time);

                if (nextInfo != currentInfo)
                {
                    currentInfo = nextInfo;
                    time = currentInfo.Interval.NearestEarlierTime(/*currentInfo.StartTime,*/ time);
                }
                if (time >= endTime)
                    yield break;

                yield return time;
            }
        }

        public IParameterInfo GetParameterNode(RevisionInfo revision, string parameterCode)
        {
            return calcSupplier.GetParameterNode(this, revision, parameterCode);
        }

        public bool ForceCalc(ICalcNodeInfo calcNodeInfo)
        {
            // Если вжата кнопка "пересчитать всё" или
            // расчитывается параметр внутри оптимизаионного расчёта
            return calcNodeInfo.Calculable
                && (RecalcAll
                || (calcNodeInfo.Optimization != null
                || (!IsRoundRobin && IsParameterToCalc(calcNodeInfo.CalcNode))));
        }

        public ICalcState GetState(ICalcNode calcNode, RevisionInfo revision)
        {
            ICalcState state = StateStorage.GetState(calcNode, revision);

            return state;
        }

        private ICalcState compilingState;

        private void EnterCompiling(ICalcState state)
        {
            compilingState = state;
        }

        private void LeaveCompiling()
        {
            compilingState = null;
        }

        Dictionary<ICalcNode, List<RevisionInfo>> compiledNodes;

        /// <summary>
        /// Компилировался ли параметр в ходе текущего расчета
        /// </summary>
        /// <param name="nodeInfo">Информация о параметре</param>
        /// <returns></returns>
        private bool CompiledYet(ICalcNode calcNode, RevisionInfo revision)
        {
            List<RevisionInfo> compiledRevisionList;

            return compiledNodes.TryGetValue(calcNode, out compiledRevisionList)
                && compiledRevisionList.Contains(revision);
        }

        /// <summary>
        /// Отметить параметр как скомпилированный
        /// </summary>
        /// <param name="nodeInfo"></param>
        private void SetCompiled(ICalcNode calcNode, RevisionInfo revision)
        {
            List<RevisionInfo> compiledRevisionList;

            if (!compiledNodes.TryGetValue(calcNode, out compiledRevisionList))
                compiledNodes[calcNode] = compiledRevisionList = new List<RevisionInfo>();

            compiledRevisionList.Add(revision);
        }

        public bool RecalcAll { get; set; }

        public int MaxLoopCount { get; set; }

        public ICalcStateStorage StateStorage { get; protected set; }

        private ICalcSupplier calcSupplier;

        /// <summary>
        /// Сообщения, возникшие при расчете
        /// </summary>
        public List<Message> Messages { get; protected set; }

        /// <summary>
        /// Хранилище значений параметров
        /// </summary>
        public IValuesKeeper ValuesKeeper { get; set; }

        public CalcContext(OperationState state, ICalcStateStorage calcCache, ICalcSupplier calcSupplier, ICompiler compiler)
        {
            this.OperationState = state;
            this.Compiler = compiler;
            this.StateStorage = calcCache;
            this.calcSupplier = calcSupplier;
            compiledNodes = new Dictionary<ICalcNode, List<RevisionInfo>>();
            SymbolTable = new SymbolTable(this);
            Messages = new List<Message>();
        }

        public RevisionInfo GetRevision(DateTime time)
        {
            return calcSupplier.GetRevision(time);
        }

        const MessageCategory stopCategory = MessageCategory.Error;

        /// <summary>
        /// Добавить сообщение
        /// </summary>
        /// <param name="messageAB">Добавляемое сообщение</param>
        public void AddMessage(Message messageAB)
        {
            if (Messages.Find(m => m.Text.Equals(messageAB.Text)) == null)
                Messages.Add(messageAB);

            ICallContext callContext = SymbolTable.CallContext;

            CalcMessage calcMessage;
            if ((calcMessage = messageAB as CalcMessage) == null)
            {
                calcMessage = new CalcMessage(messageAB.Category, GetIdentifier(), messageAB.Text);
            }
            //if ((calcMessage = messageAB as CalcMessage) != null)
            else
            {
                calcMessage.Position.Merge(GetIdentifier());
            }

            // выставить флаг ошибки
            if (calcMessage.Category >= stopCategory)
            {
                if (compilingState != null)
                    compilingState.Failed = true;
                else if (callContext != null)
                    callContext.Fail = true;
            }

            OperationState.AddMessage(calcMessage);
        }

        public void AddMessage(IEnumerable<Message> messages)
        {
            foreach (Message message in messages)
                AddMessage(message);
        }

        /// <summary>
        /// Получить подробный отчет о состоянии расчета
        /// </summary>
        /// <returns></returns>
        public String ContextStateReport()
        {
            StringBuilder reportBuilder = new StringBuilder();

            reportBuilder.Append("\nsymbol stack:");

            reportBuilder.Append(SymbolTable.SymbolTableReport());

            reportBuilder.Append("\ncall stack:");

            foreach (ICallContext callContext in SymbolTable.GetAllContext())
            {
                reportBuilder.Append(callContext.ContextReport());
            }

            return reportBuilder.ToString();
        }

        public ArgumentsValues[] GetManualArgValues(IOptimizationInfo optimizationInfo, ArgumentsValues arguments, DateTime startTime)
        {
            List<ArgumentsValues> argumentsList = new List<ArgumentsValues>();

            return calcSupplier.GetManualArgValue(this, optimizationInfo, arguments, startTime);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (FunctionManager != null)
                RemoteFunctionManager.Dispose(FunctionManager);
        }

        #endregion

        public CalcPosition GetIdentifier()
        {
            if (compilingState != null)
            {
                return compilingState.CalcPosition;
            }
            if (SymbolTable.CallContext != null)
            {
                //return SymbolTable.CallContext.CurrentNode;

                var pos = new CalcPosition();
                pos.Merge(SymbolTable.CallContext.CurrentNode);
                pos.Location = SymbolTable.CallContext.CurrentLocation;

                return pos;
            }
            return new CalcPosition();
        }

        public DateTime StartTime { get; protected set; }

        public DateTime EndTime { get; protected set; }

        public void SetTime(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public bool IsRoundRobin { get; set; }
    }
}

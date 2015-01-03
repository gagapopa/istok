using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ASC.Audit;
using COTES.ISTOK.Assignment.Audit;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.Assignment
{
    public class AutoCalcTask
    {
        public int ID { get; set; }

        public String Name { get; set; }

        public DateTime StartTime { get; set; }

        public Schedule Schedule { get; set; }

        public UnitNode[] Nodes { get; set; }
    }

    class CalcServerAdapter
    {
        public CalcServer CalcServer { get; set; }

        public IAuditServer Audit { get; set; }

        public AssignmentCalcSupplier CalcSupplier { get; set; }

        public IUnitManager Units { get; set; }

        public ISecurityManager Security { get; set; }

        public Scheduler Scheduler { get; set; }

        public IEnumerable<AutoCalcTask> AutoTasks { get; set; }

        //OperationState calcState;

        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        public CalcServerAdapter()
        {
            //calcState = new OperationState(Security.InternalSession);
        }

        public void ChangeParameterCode(OperationState state, string oldCode, string newCode, string[] formulaArray)
        {
            CalcServer.ChangeParameterCode(state, oldCode, newCode, formulaArray);
        }

        public IEnumerable<CalcParameterNode> ChangeParameterCode(OperationState state, String oldCode, String newCode)
        {
            OperationState subState = state.Sub();

            CalcServer.GetReference(subState, oldCode);

            CalcParameterNode node;
            List<CalcParameterNode> unitNodeList = new List<CalcParameterNode>();
            List<String> formulaList = new List<string>();
            String[] formulaArray;

            IParameterInfo nodeInfo;
            foreach (var item in subState.AsyncResult)
            {
                if ((nodeInfo = item as IParameterInfo) != null
                    && (node = CalcSupplier.GetParameterNode(nodeInfo) as CalcParameterNode) != null)
                {
                    node = node.Clone() as CalcParameterNode;
                    unitNodeList.Add(node);
                    formulaList.Add(node.Formula);
                }
            }

            formulaArray = formulaList.ToArray();
            CalcServer.ChangeParameterCode(state.Sub(), oldCode, newCode, formulaArray);

            for (int i = 0; i < formulaArray.Length; i++)
            {
                unitNodeList[i].Formula = formulaArray[i];
            }

            return unitNodeList;
        }

        public ParameterNodeDependence[] GetDependence(OperationState state, RevisionInfo revision, CalcParameterNode node)
        {
            using (var context = CalcServer.CreateCalcContext(state))
            {
                ICalcNode calcNode = CalcSupplier.GetNodeInfo(node);
                IParameterInfo nodeInfo = calcNode.GetParameter(revision);

                var tupleList = CalcServer.GetDependence(context.OperationState, revision, nodeInfo);

                List<ParameterNode> parameterNodeList = new List<ParameterNode>();
                List<ParameterNodeDependence> dependences = new List<ParameterNodeDependence>();

                foreach (var tuple in tupleList)
                {
                    parameterNodeList.Clear();
                    foreach (IParameterInfo paramInfo in tuple.Item2)
                    {
                        parameterNodeList.Add(CalcSupplier.GetParameterNode(paramInfo));
                    }

                    ParameterNode keyNode = CalcSupplier.GetParameterNode(tuple.Item1);
                    dependences.Add(new ParameterNodeDependence() { ParameterId = keyNode.Idnum, Dependences = parameterNodeList.ToArray() });
                }
                return dependences.ToArray();
            }
        }

        public bool HasReference(OperationState state, string code)
        {
            return CalcServer.HasReference(state, code);
        }

        public ParameterNodeReference[] GetReference(OperationState state, string code)
        {
            IParameterInfo nodeInfo;
            List<ParameterNodeReference> retNodes = new List<ParameterNodeReference>();

            var subState = state.Sub();

            CalcServer.GetReference(subState, code);

            foreach (var item in subState.AsyncResult)
            {
                if ((nodeInfo = item as IParameterInfo) != null)
                    retNodes.Add(new ParameterNodeReference()
                    {
                        ParameterNode = CalcSupplier.GetParameterNode(nodeInfo),
                        Revision = nodeInfo.Revision
                    });
            }

            return retNodes.ToArray();
        }

        public void Calc(OperationState state, IEnumerable<UnitNode> nodes, DateTime beginTime, DateTime startTime, bool isRoundRobin, bool recalcAll)
        {
            List<ICalcNode> parameterList = new List<ICalcNode>();

            var auditCalcStart = new AuditCalcStart()
            {
                CalcStart = beginTime,
                CalcEnd = startTime,
                CalcRecalc = recalcAll
            };

            //using (var context = CalcServer.CreateCalcContext(state))
            //{
            foreach (var parameterNode in nodes)
            {
                if (parameterNode != null)
                    parameterList.Add(CalcSupplier.GetNodeInfo(parameterNode));

                var auditCalcNode = new AuditCalcNode()
                {
                    UnitNodeID = parameterNode.Idnum,
                    UnitNodeFullPath = parameterNode.FullName
                };
                auditCalcStart.AuditCalcNodes.Add(auditCalcNode);
            }

            CalcServer.Calc(state, parameterList.ToArray(), beginTime, startTime, isRoundRobin, recalcAll);
            //CalcServer.Calc(context);
            //}

            var auditEntry = new AuditEntry(Security.GetUserInfo(state.UserGUID));
            auditEntry.AuditCalcStarts.Add(auditCalcStart);

            Audit.WriteAuditEntry(auditEntry);
        }

        public FunctionInfo[] GetCalcFunctions(OperationState state, RevisionInfo revision)
        {
            return CalcServer.GetCalcFunctions(state, revision);
        }

        public void CheckFormula(OperationState state, RevisionInfo revision, string formulaText, KeyValuePair<int, CalcArgumentInfo[]>[] arguments)
        {
            CalcServer.CheckFormula(state, revision, formulaText, arguments);
        }

        public IEnumerable<ConstsInfo> GetCalcConsts()
        {
            return CalcServer.GetCalcConsts();
        }

        Schedule roundRobinSchedule;
        Schedule RoundRobinSchedule
        {
            get
            {
                if (roundRobinSchedule == null)
                {
                    roundRobinSchedule = new Schedule()
                    {
                        Name = "RoundRobinCalc",
                        Rule = new ScheduleReg(String.Format("0 0 1 00:00 1 {0} 86400", GlobalSettings.Instance.CalculationInterval))
                    };
                }
                return roundRobinSchedule;
            }
        }

        RoundRobinInfo roundRobinInfo;
        List<Message> lastMessages;
        bool inProccess;

        public Object RoundRobinCalc(OperationState state, params Object[] parameters)
        {
            Schedule schedule = parameters[0] as Schedule;

            roundRobinInfo.LastStartTime = DateTime.Now;

            lock (this)
            {
                if (inProccess)
                {
                    log.Warn("Вызов автоматического расчёта, пока не закончен предыдущий.");
                    return null;
                }
                inProccess = true;
            }

            try
            {
                using (var context = CalcServer.CreateCalcContext(state))
                {
                    List<ICalcNode> roundRobinParams = new List<ICalcNode>();
                    IParameterInfo parameterInfo;
                    DateTime minValue = DateTime.Now.AddMilliseconds(-GlobalSettings.Instance.CalculationInterval);
                    foreach (var calcNode in CalcSupplier.GetParameterNodes(context))
                    {
                        if ((parameterInfo = calcNode.GetParameter(RevisionInfo.Default)) != null
                               && parameterInfo.RoundRobinCalc)
                        {
                            DateTime lastTime = CalcSupplier.GetLastTimeValue(context, parameterInfo);
                            if (lastTime != DateTime.MinValue
                                && lastTime < minValue)
                            {
                                minValue = lastTime;
                            }
                            roundRobinParams.Add(calcNode);
                        }
                    }

                    CalcServer.Calc(state, roundRobinParams.ToArray(), minValue, DateTime.Now, true, false);

                    lastMessages = new List<Message>(state.messages);
                }
            }
            finally
            {
                roundRobinInfo.LastStopTime = DateTime.Now;
                roundRobinInfo.LastCalcTimeSpan = roundRobinInfo.LastStopTime.Subtract(roundRobinInfo.LastStartTime);
                lock (this)
                {
                    inProccess = false;
                }

            }
            return null;
        }

        public bool IsStarted { get; private set; }

        public void StartRoundRobin()
        {
            IsStarted = true;

            Scheduler.RegisterSchedule(RoundRobinSchedule, RoundRobinCalc);
        }

        public void StopRoundRobin()
        {
            IsStarted = false;
            Scheduler.UnregisterSchedule(RoundRobinSchedule, RoundRobinCalc);
        }

        public List<Message> LastRoundRobinMessages
        {
            get { return lastMessages; }
        }

        public RoundRobinInfo GetRoundRobinInfo()
        {
            return roundRobinInfo;
        }
    }
}

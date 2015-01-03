using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Класс поддержки расчетов на глобале
    /// </summary>
    class AssignmentCalcSupplier : ICalcSupplier
    {
        public RevisionManager RevisionManager { get; set; }

        public IUnitManager UnitManager { get; set; }

        public ValueReceiver ValueReceiver { get; set; }

        MyDBdata dbwork;

        OperationState calcState;

        public AssignmentCalcSupplier(MyDBdata dbwork, SecurityManager securityManager)
        {
            this.dbwork = dbwork.Clone();
            calcState = new OperationState(securityManager.InternalSession);
            InterpolateCount = 4;
        }

        /// <summary>
        /// Сохранить константы
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="consts">Изменяемые константы</param>
        public void SaveCalcConsts(OperationState state, IEnumerable<ConstsInfo> consts)
        {
            const String insertConstsQuery = "INSERT INTO [calc_consts] ([name], [description], [value]) VALUES (@name, @description, @value)";
            const String updateConstsQuery = "UPDATE [calc_consts] SET [name] = @name,[description] = @description,[value] = @value WHERE [id] = @id";

            DB_Parameter nameParameter, descriptionParameter, valueParameter, idParameter;
            DB_Parameters pars = new DB_Parameters();

            pars.Add(nameParameter = new DB_Parameter("name", DbType.String));
            pars.Add(descriptionParameter = new DB_Parameter("description", DbType.String));
            pars.Add(valueParameter = new DB_Parameter("value", DbType.String));
            pars.Add(idParameter = new DB_Parameter("id", DbType.String));

            foreach (var constant in consts)
            {
                if (constant.Editable)
                {
                    nameParameter.ParamValue = constant.Name;
                    descriptionParameter.ParamValue = constant.Description;
                    valueParameter.ParamValue = constant.Value;
                    idParameter.ParamValue = constant.ID;

                    if (dbwork.ExecSQL(updateConstsQuery, pars) == 0)
                        dbwork.ExecSQL(insertConstsQuery, pars);
                }
            }
        }

        /// <summary>
        /// Удалить константы
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="consts">Удаляемые константы</param>
        public void RemoveCalcConsts(OperationState state, IEnumerable<ConstsInfo> consts)
        {
            const String deleteConstsQuery = "DELETE FROM [calc_consts] WHERE [id] = @id";

            DB_Parameter idParameter;
            DB_Parameters pars = new DB_Parameters();

            pars.Add(idParameter = new DB_Parameter("id", DbType.String));

            foreach (var constant in consts)
                if (constant.Editable)
                {
                    idParameter.ParamValue = constant.ID;

                    dbwork.ExecSQL(deleteConstsQuery, pars);
                }
        }

        /// <summary>
        /// Сохранить пользовательские функции
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="functionInfo">Сохраняемые функции</param>
        public void SaveCustomFunctions(OperationState state, IEnumerable<CustomFunctionInfo> functionInfo)
        {
            const String insertConstsQuery = "INSERT INTO [calc_function] ([name], [description], [args], [text], [group_name]) VALUES (@name, @description, @args, @text, @group_name)";
            const String updateConstsQuery = "UPDATE [calc_function] SET [name] = @name, [description] = @description, [args] = @args, [text] = @text, [group_name] = @group_name WHERE[id] = @id";

            DB_Parameter nameParameter, descriptionParameter, argsParameter, textParameter, groupParameter, idParameter;
            DB_Parameters pars = new DB_Parameters();

            pars.Add(nameParameter = new DB_Parameter("name", DbType.String));
            pars.Add(descriptionParameter = new DB_Parameter("description", DbType.String));
            pars.Add(argsParameter = new DB_Parameter("args", DbType.String));
            pars.Add(textParameter = new DB_Parameter("text", DbType.String));
            pars.Add(groupParameter = new DB_Parameter("group_name", DbType.String));
            pars.Add(idParameter = new DB_Parameter("id", DbType.String));

            foreach (var constant in functionInfo)
            {
                nameParameter.ParamValue = constant.Name;
                descriptionParameter.ParamValue = constant.Comment;
                textParameter.ParamValue = constant.Text;
                groupParameter.ParamValue = constant.GroupName;
                idParameter.ParamValue = constant.ID;

                if (dbwork.ExecSQL(updateConstsQuery, pars) == 0)
                    dbwork.ExecSQL(insertConstsQuery, pars);
            }
        }

        /// <summary>
        /// Удалить пользовательские функции
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="functionInfo">Удаляемые функции</param>
        public void RemoveCustomFunctions(OperationState state, IEnumerable<CustomFunctionInfo> functionInfo)
        {
            const String deleteConstsQuery = "DELETE FROM [calc_function] WHERE [id] = @id";

            DB_Parameter idParameter;
            DB_Parameters pars = new DB_Parameters();

            pars.Add(idParameter = new DB_Parameter("id", DbType.String));

            foreach (var constant in functionInfo)
            {
                idParameter.ParamValue = constant.ID;

                dbwork.ExecSQL(deleteConstsQuery, pars);
            }
        }

        //Dictionary<int, CalcNode> calcNodeCache = new Dictionary<int, CalcNode>();

        /// <summary>
        /// Получить объект ICalcNodeInfo для параметра
        /// </summary>
        /// <param name="unitNode">Узел</param>
        /// <returns></returns>
        public ICalcNode GetNodeInfo(UnitNode unitNode)
        {
            //CalcNode calcNode;

            //if (!calcNodeCache.TryGetValue(unitNode.Idnum, out calcNode))
            //    calcNodeCache[unitNode.Idnum] = calcNode = new CalcNode(this, UnitManager, calcState, unitNode.Idnum);

            //return calcNode;

            return new CalcNode(this, UnitManager, calcState, unitNode.Idnum);
        }

        /// <summary>
        /// Получить параметр по INodeInfo
        /// </summary>
        /// <param name="depNodeInfo"></param>
        /// <returns></returns>
        public ParameterNode GetParameterNode(IParameterInfo depNodeInfo)
        {
            CalcNode calcNode = depNodeInfo.CalcNode as CalcNode;

            if (calcNode != null)
                return calcNode.Node as ParameterNode;

            return null;
        }

        public IEnumerable<ICalcNode> GetChilds<T>(UnitNode baseUnitNode)
            where T : UnitNode
        {
            List<ICalcNode> paramsList = new List<ICalcNode>();
            List<UnitNode> checkNodeList = new List<UnitNode>();
            UnitNode unitNode;
            T paramNode;

            checkNodeList.Add(baseUnitNode);
            while (checkNodeList.Count > 0)
            {
                UnitNode[] nodes = checkNodeList.ToArray();
                checkNodeList.Clear();
                foreach (UnitNode currentNode in nodes)
                {
                    foreach (int childNodeId in currentNode.NodesIds)
                    {
                        if ((unitNode = UnitManager.ValidateUnitNode(calcState, childNodeId, Privileges.Read)) != null)
                        {
                            if ((paramNode = unitNode as T) != null)
                                paramsList.Add(GetNodeInfo(paramNode));
                            if (unitNode.HasChild && unitNode.Typ != (int)UnitTypeId.OptimizeCalc) // исключаем параметры из вложенных оптимизаций
                                checkNodeList.Add(unitNode);
                        }
                    }
                }
            }
            return paramsList.ToArray();
        }

        IOptimizationInfo GetBaseOptimization(OperationState state, CalcNode calcNode, RevisionInfo revision)
        {
            OptimizationGateNode retNode;
            UnitNode currentNode = calcNode.Node;

            retNode = UnitManager.CheckUnitNode<OptimizationGateNode>(state, currentNode.ParentId, Privileges.Read);


            if (retNode != null)
            {
                ICalcNode retCalcNode = GetNodeInfo(retNode);
                return retCalcNode.GetOptimization(revision);
            }
            return null;
        }

        Interval GetInterval(OperationState state, CalcNode calcNode, RevisionInfo revision)
        {
            LoadParameterNode loadParameter;
            ParameterGateNode gateNode;
            UnitNode curNode;

            curNode = calcNode.Node;

            if ((loadParameter = curNode as LoadParameterNode) != null)
                return loadParameter.Interval;

            gateNode = UnitManager.CheckParentNode<ParameterGateNode>(state.Sub(), curNode.ParentId, Privileges.Read);

            if (gateNode!=null)
            {
                return gateNode.Interval;
            }

            return Interval.Zero;
        }
        
        #region ICalcSupplier Members

        public RevisionInfo GetRevision(DateTime time)
        {
            return RevisionManager.GetRevision(time);
        }

        public IEnumerable<RevisionInfo> GetRevisions(DateTime startTime, DateTime endTime)
        {
            return RevisionManager.GetRevisions(startTime, endTime);
        }

        public IParameterInfo GetParameterNode(ICalcContext context, RevisionInfo revision, string parameterCode)
        {
            ParameterNode paramNode = UnitManager.GetParameter(calcState, parameterCode);
            if (paramNode == null)
            {
                return null;
            }
            return GetNodeInfo(paramNode).Revisions.Get(revision) as IParameterInfo;
        }

        public ICalcNode GetParameterNode(ICalcContext context, int nodeID)
        {
            UnitNode unitNode = UnitManager.ValidateUnitNode(calcState.Sub(), nodeID, Privileges.Read);
            return GetNodeInfo(unitNode);
        }

        public IParameterInfo GetEmptyParameterNode(RevisionInfo revision, String parameterCode)
        {
            ParameterNode node = new ParameterNode();
            node.Idnum = -1;
            node.Typ = (int)UnitTypeId.Parameter;
            node.Code = parameterCode;
            node.Text = String.Format("Параметр ${0}$", parameterCode);

            CalcNode calcNode = new CalcNode(node);
            return calcNode.GetParameter(RevisionInfo.Default);
        }

        public IEnumerable<ICalcNode> GetParameterNodes(ICalcContext context)
        {
            List<ICalcNode> ret = new List<ICalcNode>();
            foreach (var item in UnitManager.GetParameters(calcState.Sub()))
                if (item != null)
                    ret.Add(GetNodeInfo(item));

            return ret;
        }

        public DateTime GetLastTimeValue(ICalcContext context, IParameterInfo parameterInfo)
        {
            return ValueReceiver.GetLastTimeValueParameter(calcState.Sub(), parameterInfo.CalcNode.NodeID);
        }

        public ParamValueItem GetParameterNodeValue(ICalcContext context,
                                                    IEnumerable<Tuple<ICalcNode, ArgumentsValues>> nodeArgs,
                                                    CalcAggregation agreg,
                                                    DateTime startTime,
                                                    Interval interval,
                                                    out List<Message> messages,
                                                    out bool serverNotAccessible)
        {
            serverNotAccessible = false;
            //CalcNode nodeInfo;
            messages = null;

            var nodes = (from t in nodeArgs select Tuple.Create((t.Item1 as CalcNode).Node as ParameterNode, t.Item2)).ToArray();

            //if ((nodeInfo = node as CalcNode) != null)
            //{
            MessageByException messageByException;

            OperationState state = calcState.Sub();
            //ParameterNode parameterNode = nodeInfo.Node as ParameterNode;

            ValueReceiver.AsyncGetValues(
                state,
                0f,
                new ParameterValuesRequest[]
                    {  
                        new ParameterValuesRequest()
                        {
                            //Parameters = new Tuple<ParameterNode, ArgumentsValues>[]
                            //{ 
                            //    Tuple.Create(parameterNode, args) 
                            //},
                            Parameters = nodes,
                            StartTime = startTime,
                            EndTime = interval.GetNextTime(startTime),
                            AggregationInterval = interval,
                            Aggregation = agreg,
                        }
                    },
                true);//,
            //true/*false*/);

            messages = state.messages;

            Package[] packs;

            if (state.AsyncResult != null)
                packs = (from x in state.AsyncResult where x is Package select x as Package).ToArray();
            else
                packs = null;

            // Установить Флаг доступности блочного
            foreach (Message message in messages)
                if ((messageByException = message as MessageByException) != null
                    && messageByException.Exception is ServerNotAccessibleException)
                {
                    serverNotAccessible = true;
                    break;
                }

            if (packs != null && packs.Length > 0 && packs[0].Values.Count > 0)
                //return packs[0].Values[0];
                return (from v in packs[0].Values where v.Time == startTime select v).FirstOrDefault();
            //}
            return null;
        }

        public List<ParamValueItem> GetParameterNodeRawValues(ICalcContext context,
                                                              ICalcNode parameterInfo,
                                                              ArgumentsValues args,
                                                              DateTime startTime,
                                                              DateTime endTime,
                                                              out List<Message> messages,
                                                              out bool serverNotAccessible)
        {
            serverNotAccessible = false;
            CalcNode nodeInfo;
            messages = null;

            if ((nodeInfo = parameterInfo as CalcNode) != null)
            {
                MessageByException messageByException;
                List<ParamValueItem> valueList = new List<ParamValueItem>();
                ParameterNode parameterNode = nodeInfo.Node as ParameterNode;

                OperationState state = calcState.Sub();

                ValueReceiver.AsyncGetValues(
                    state,
                    0f,
                    new ParameterValuesRequest[]
                    {
                        new ParameterValuesRequest()
                        {
                            Parameters = new Tuple<ParameterNode,ArgumentsValues>[]
                            {
                                Tuple.Create(parameterNode, args)
                            },
                            StartTime = startTime, 
                            EndTime = endTime, 
                        }
                    },
                    true,
                    false);

                messages = state.messages;

                Package[] packs;

                if (state.AsyncResult != null)
                    packs = (from x in state.AsyncResult where x is Package select x as Package).ToArray();
                else
                    packs = null;

                // Установить Флаг доступности блочного
                foreach (Message message in messages)
                    if ((messageByException = message as MessageByException) != null
                        && messageByException.Exception is ServerNotAccessibleException)
                    {
                        serverNotAccessible = true;
                        break;
                    }

                if (packs != null)
                    foreach (Package package in packs)
                        foreach (ParamValueItem valueItem in package.Values)
                            if (!valueItem.ChangeTime.Equals(DateTime.MinValue))
                                valueList.Add(valueItem);

                return valueList;
            }
            return null;
        }

        public IEnumerable<ConstsInfo> GetConsts()
        {
            const String selectConstsQuery = "SELECT [id],[name],[description],[value] FROM [calc_consts]";
            try
            {
                List<ConstsInfo> consts = new List<ConstsInfo>();
                ConstsInfo constant;
                DataSet dataSet = dbwork.ExecSQL_toDataset(selectConstsQuery, null);

                foreach (DataTable table in dataSet.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        constant = new ConstsInfo(Convert.ToInt32(row["id"]),
                            row["name"].ToString(),
                            row["description"].ToString(),
                            row["value"].ToString());
                        consts.Add(constant);
                    }
                }
                return consts.ToArray();
            }
            catch { }
            return new ConstsInfo[0];
        }

        public void SaveParameterNodeValue(ICalcContext context, Package[] savingValues, out List<Message> messages)
        {
            OperationState state = context.OperationState.Sub();

            ValueReceiver.SaveValues(state, AsyncOperation.MaxProgressValue, savingValues, true);

            messages = new List<Message>(state.messages);
        }

        public int InterpolateCount { get; set; }

        public IEnumerable<IExternalFunctionInfo> GetExternalFunctions(ICalcContext context)
        {
            NormFuncNode[] normfuncs = UnitManager.GetNormFuncs(context.OperationState.Sub());
            List<IExternalFunctionInfo> funcs = new List<IExternalFunctionInfo>();
            string txt;

            foreach (var item in normfuncs)
            {
                UnitNode unitNode = UnitManager.CheckUnitNode<UnitNode>(context.OperationState.Sub(), item.ParentId, Privileges.Read);

                if (unitNode != null)
                    txt = unitNode.Text;
                else
                    txt = "";
                foreach (var revision in item.GetRevisions())
                {
                    funcs.Add(new CTableFunctionInfo(item, revision, txt));
                }
            }

            for (int i = 1; i < InterpolateCount; i++)
                funcs.Add(new InterpolateFunctionInfo(String.Format("interpolate{0}", i), i));

            return funcs.ToArray();
        }

        public IEnumerable<CustomFunctionInfo> GetCustomFunction()
        {
            const String selectConstsQuery = "SELECT [id], [name], [description], [args], [text], [group_name] FROM [calc_function]";
            try
            {
                List<CustomFunctionInfo> functionList = new List<CustomFunctionInfo>();
                CustomFunctionInfo function;
                DataSet dataSet = dbwork.ExecSQL_toDataset(selectConstsQuery, null);

                foreach (DataTable table in dataSet.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        function = new CustomFunctionInfo(Convert.ToInt32(row["id"]),
                            row["name"].ToString(),
                            row["description"].ToString(),
                            row["group_name"].ToString(),
                            row["text"].ToString());
                        functionList.Add(function);
                    }
                }
                return functionList.ToArray();
            }
            catch { }
            return new CustomFunctionInfo[0];
        }

        public ArgumentsValues[] GetManualArgValue(ICalcContext context, IOptimizationInfo optimizationInfo, ArgumentsValues baseArgs, DateTime startTime)
        {
            CalcNode calcNode;
            ParameterNode parameter;
            List<ParameterNode> parameterNodeList = new List<ParameterNode>();

            foreach (ICalcNodeInfo parameterInfo in optimizationInfo.ChildParameters)
                if (!parameterInfo.Calculable
                    && (calcNode = parameterInfo.CalcNode as CalcNode) != null
                    && (parameter = calcNode.Node as ParameterNode) != null)
                {
                    parameterNodeList.Add(parameter);
                }

            return ValueReceiver.GetManualArgs(context.OperationState.Sub(), parameterNodeList, startTime);
        }
        #endregion

        /// <summary>
        /// Время, после последнего обращения к узлу, которое хранится ссылка на узел
        /// </summary>
        static readonly TimeSpan maxIdle = TimeSpan.FromSeconds(5);

        class CalcNode : ICalcNode
        {
            int unitNodeID;

            UnitNode unitNode;

            AssignmentCalcSupplier calcSupplier;

            IUnitManager unitManager;

            OperationState state;

            ///// <summary>
            ///// Время последнего обращения к узлу
            ///// </summary>
            //DateTime lastParameterNodeActive;

            public CalcNode(AssignmentCalcSupplier calcSupplier, IUnitManager unitManager, OperationState state, int unitNodeID)
            {
                this.calcSupplier = calcSupplier;
                this.unitManager = unitManager;
                this.unitNodeID = unitNodeID;
                this.state = state;
            }

            public CalcNode(UnitNode unitNode)
            {
                this.unitNode = unitNode;
            }

            public UnitNode Node
            {
                get
                {
                    if (unitManager != null &&
                        //(unitNode == null ||
                        //DateTime.Now.Subtract(lastParameterNodeActive) > maxIdle))
                        unitNode == null)
                    {
                        unitNode = unitManager.ValidateUnitNode(state.Sub(), unitNodeID, Privileges.Read);
                        //lastParameterNodeActive = DateTime.Now;
                        if (unitNode == null)
                            throw new Exception("Параметр был удален во время расчета");
                    }

                    return unitNode;
                }
            }

            #region ICalcNode Members

            public int NodeID
            {
                get { return Node.Idnum; }
            }

            public string Name
            {
                get { return Node.Text; }
            }

            public RevisedStorage<ICalcNodeInfo> Revisions
            {
                get
                {
                    RevisedStorage<ICalcNodeInfo> storage = new RevisedStorage<ICalcNodeInfo>();
                    foreach (var revision in Node.GetRevisions())
                    {
                        ICalcNodeInfo nodeInfo;
                        if (Node is ParameterNode)
                            nodeInfo = new ParameterInfo(calcSupplier, state, this, revision);
                        else if (Node is OptimizationGateNode)
                            nodeInfo = new OptimizationInfo(calcSupplier, state, this, revision);
                        else
                            throw new Exception("Не верный тип параметра");

                        storage.Set(revision, nodeInfo);
                    }
                    return storage;
                }
            }

            #endregion

            public override bool Equals(object obj)
            {
                CalcNode calcNode = obj as CalcNode;

                if (calcNode != null)
                {
                    return NodeID.Equals(calcNode.NodeID);
                }
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return NodeID.GetHashCode();
            }
        }

        /// <summary>
        /// Реализация INodeInfo для станционного сервера
        /// </summary>
        class ParameterInfo : IParameterInfo
        {
            AssignmentCalcSupplier calcSupplier;

            CalcNode calcNode;
            RevisionInfo revision;

            OperationState state;

            ParameterNode ParameterNode
            {
                get
                {
                    return calcNode.Node as ParameterNode;
                }
            }

            public ParameterInfo(AssignmentCalcSupplier calcSupplier, OperationState state, CalcNode calcNode, RevisionInfo revision)
            {
                this.calcSupplier = calcSupplier;
                this.state = state;
                this.calcNode = calcNode;
                this.revision = revision;
            }

            #region INodeInfo Members

            public ICalcNode CalcNode
            {
                get { return calcNode; }
            }

            public RevisionInfo Revision
            {
                get { return revision; }
            }

            public string Name
            {
                get { return ParameterNode.Text; }
            }

            public string Code
            {
                get { return ParameterNode.Code; }
            }

            public string Formula
            {
                get
                {
                    CalcParameterNode calcNode;
                    if ((calcNode = ParameterNode as CalcParameterNode) != null) return calcNode.Formula;
                    return null;
                }
            }

            IOptimizationInfo optimization;
            public IOptimizationInfo Optimization
            {
                get
                {
                    if (optimization == null)
                    {
                        optimization = calcSupplier.GetBaseOptimization(state, calcNode, revision);
                    }
                    return optimization;
                }
            }

            public IEnumerable<String> Needed
            {
                get
                {
                    CalcParameterNode calcNode;
                    if ((calcNode = ParameterNode as CalcParameterNode) != null
                        && !String.IsNullOrEmpty(calcNode.NeededParametersCodes))
                        return calcNode.NeededParametersCodes.Split(';');
                    return null;
                }
            }

            Interval interval;
            public Interval Interval
            {
                get
                {
                    if (interval == null)
                    {
                        interval = calcSupplier.GetInterval(state, calcNode, revision);
                    }
                    return interval;
                }
            }

            public bool Calculable
            {
                get
                {
                    return ParameterNode is CalcParameterNode
                            && !String.IsNullOrEmpty(Formula)
                            && Interval != Interval.Zero;
                }
            }

            public bool RoundRobinCalc
            {
                get
                {
                    CalcParameterNode calcNode;
                    if ((calcNode = ParameterNode as CalcParameterNode) != null) return calcNode.RoundRobinCalc;
                    return false;
                }
            }
            #endregion

            public override int GetHashCode()
            {
                return ParameterNode.Idnum.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                ParameterInfo info = obj as ParameterInfo;

                if (info != null)
                    return ParameterNode.Idnum.Equals(info.ParameterNode.Idnum);

                return base.Equals(obj);
            }

            public override string ToString()
            {
                String text;

                if (!String.IsNullOrEmpty(ParameterNode.Code))
                    text = String.Format("${1}$ '{0}'", ParameterNode.Text, ParameterNode.Code);
                else
                    text = ParameterNode.Text;

#if DEBUG
                text = String.Format("[ID={0}]", ParameterNode.Idnum) + text;
#endif

                return text;
            }
        }

        /// <summary>
        /// Реализация IOptimizationInfo для станционного сервера
        /// </summary>
        class OptimizationInfo : IOptimizationInfo
        {
            AssignmentCalcSupplier calcSupplier;

            RevisionInfo revision;
            CalcNode calcNode;

            OperationState state;

            public OptimizationInfo(AssignmentCalcSupplier calcSupplier, OperationState state, CalcNode calcNode, RevisionInfo revision)
            {
                this.calcSupplier = calcSupplier;
                this.state = state;
                this.calcNode = calcNode;
                this.revision = revision;
            }

            public OptimizationGateNode OptimizationNode
            {
                get
                {
                    return calcNode.Node as OptimizationGateNode;
                }
            }

            #region IOptimizationInfo Members

            public ICalcNode CalcNode
            {
                get { return calcNode; }
            }

            public RevisionInfo Revision
            {
                get { return revision; }
            }

            public string Name
            {
                get { return CalcNode.Name; }
            }

            public string Expression
            {
                get { return OptimizationNode.Expression; }
            }

            public string DefinationDomain
            {
                get { return OptimizationNode.DefinationDomain; }
            }

            public bool Maximalize
            {
                get { return OptimizationNode.Maximalize; }
            }

            public Interval Interval
            {
                get { return OptimizationNode.Interval; }
            }

            public IOptimizationArgument[] Arguments
            {
                get
                {
                    return OptimizationNode.GetArgsValues(Revision).ToArray();
                }
            }

            public bool Calculable
            {
                get
                {
                    return !String.IsNullOrEmpty(Expression);
                }
            }

            IOptimizationInfo optimization;
            public IOptimizationInfo Optimization
            {
                get
                {
                    if (optimization == null)
                    {
                        optimization = calcSupplier.GetBaseOptimization(state, calcNode, revision);
                    }
                    return optimization;
                }
            }

            public IEnumerable<String> Needed
            {
                get { throw new NotImplementedException(); }
            }


            public bool CalcAllChildParameters
            {
                get { return OptimizationNode.CalcAllChildParameters; }
            }

            public IEnumerable<IParameterInfo> ChildParameters
            {
                get
                {
                    var childs = calcSupplier.GetChilds<ParameterNode>(calcNode.Node);
                    return from c in childs select c.GetParameter(revision);
                }
            }

            public IEnumerable<IOptimizationInfo> ChildOptimization
            {
                get
                {
                    var childs = calcSupplier.GetChilds<OptimizationGateNode>(calcNode.Node);
                    return from c in childs select c.GetOptimization(revision);
                }
            }

            #endregion

            public override int GetHashCode()
            {
                return OptimizationNode.Idnum.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                OptimizationInfo info = obj as OptimizationInfo;

                if (info != null)
                    return OptimizationNode.Idnum.Equals(info.OptimizationNode.Idnum);

                return base.Equals(obj);
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();

#if DEBUG
                builder.AppendFormat("[ID={0}]", OptimizationNode.Idnum);
#endif

                builder.AppendFormat("'{0}'", OptimizationNode.Text);
                builder.Append("(");
                int length = builder.Length;
                foreach (var item in OptimizationNode.GetArgsValues(Revision))
                {
                    if (builder.Length > length) builder.Append(", ");
                    builder.Append(item.Name);
                }
                builder.Append(")");
                if (Calculable)
                {
                    builder.AppendFormat(" -> {0}", OptimizationNode.Maximalize ? "max" : "min");
                }

                return builder.ToString();
            }
        }
    }
}

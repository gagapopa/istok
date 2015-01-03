using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Assignment
{
    class ExportImportManager
    {
        public IUnitManager UnitManager { get; set; }

        public ISecurityManager SecurityManager { get; set; }

        public ValueReceiver ValueReceiver { get; set; }

        public RevisionManager RevisionManager { get; set; }

        public IUserManager UserManager { get; set; }
        //COTES.ISTOK.Calc.CalcServer calcServer;

        public ExportImportManager()
            //IUnitManager unitManager,
            //IUserManager userManager,
            //ISecurityManager securityManager,
            //ValueReceiver valueReceiver,
            //RevisionManager revisionManager,
            //COTES.ISTOK.Calc.CalcServer calcServer)
        {
            //this.unitManager = unitManager;
            //this.securityManager = securityManager;
            //this.valueReceiver = valueReceiver;
            //this.revisionManager = revisionManager;
            //this.calcServer = calcServer;

            //_service = new ExportImportService(unitManager, userManager, valueReceiver);
        }

        ExportImportService __service;
        ExportImportService _service
        {
            get
            {
                if (__service==null)
                {
                    __service = new ExportImportService(UnitManager, UserManager, ValueReceiver);
                }
                return __service;
            }
        }

        public void Export(OperationState state, int[] nodeIds, DateTime beginValuesTime, DateTime endValuesTime, ExportFormat exportFormat)
        {
            Exporter exporter = GetExporter(_service, exportFormat);

            Dictionary<int, TreeWrapp<UnitNode>> unitNodeDictionary = new Dictionary<int, TreeWrapp<UnitNode>>();
            List<ParameterNode> parameterNodeList = new List<ParameterNode>();
            UnitNode unitNode;
            ParameterNode parameterNode;

            foreach (var id in nodeIds)
            {
                //unitNode = UnitManager.GetUnitNode(state, id);
                unitNode = UnitManager.ValidateUnitNode(state, id, Privileges.Read);

                if ((parameterNode = unitNode as ParameterNode) != null)
                    parameterNodeList.Add(parameterNode);

                //unitNodeDictionary[id] = new TreeWrapp<UnitNode>(UnitManager.GetUnitNode(state, id));
                unitNodeDictionary[id] = new TreeWrapp<UnitNode>(unitNode);
            }

            // формируем дерево
            foreach (int nodeID in unitNodeDictionary.Keys)
            {
                TreeWrapp<UnitNode> treeNode = unitNodeDictionary[nodeID];
                TreeWrapp<UnitNode> childNode;

                foreach (int childID in treeNode.Item.NodesIds)
                {
                    if (unitNodeDictionary.TryGetValue(childID, out childNode))
                    {
                        treeNode.AddChild(childNode);
                    }
                }
            }

            // находим корневые узлы
            var rootNodes = from node in unitNodeDictionary.Values
                            where !unitNodeDictionary.ContainsKey(node.Item.ParentId)
                            select node;

            byte[] buff = exporter.Serialize(state, rootNodes.ToArray(), unitNodeDictionary.Count, parameterNodeList.ToArray(), beginValuesTime, endValuesTime);
            state.AddAsyncResult(buff);
        }

        public void Import(OperationState state, byte[] buffer, ExportFormat exportFormat)
        {
            Exporter exporter = GetExporter(_service, exportFormat);

            state.StateString = "Открытие файла импорта";

            ImportDataContainer container = exporter.Deserialize(state, buffer);

            state.AddAsyncResult(container);
        }

        Exporter GetExporter(IExportService service, ExportFormat exportFormat)
        {
            switch (exportFormat)
            {
                case ExportFormat.XML:
                    return new XmlExporter(service, false);
                case ExportFormat.ZippedXML:
                    return new XmlExporter(service, true);
                case ExportFormat.Excel:
                    return new ExcelExporter();
                case ExportFormat.WordX:
                    return new WordExporter(UnitManager);
                default:
                    throw new InvalidOperationException();
            }
        }

        class ExportImportService:IExportService
        {
            IUnitManager unitManager;

            IUserManager userManager;

            ValueReceiver valueReceiver;

            public ExportImportService(IUnitManager unitManager, IUserManager userManager, ValueReceiver valueReceiver)
            {
                this.unitManager = unitManager;
                this.userManager = userManager;
                this.valueReceiver = valueReceiver;

            }

            #region IExportService Members

            public void OwnerToString(int owner, out string ownerClass, out string ownerValue)
            {
                if (owner < 0)
                {
                    ownerClass = "user";
                    ownerValue = userManager.GetUser(-owner).Text;
                }
                else if (owner > 0)
                {
                    ownerClass = "group";
                    ownerValue = userManager.GetGroup(owner).Text;
                }
                else
                {
                    ownerClass = "public";
                    ownerValue = null;
                }

            }

            public int OwnerFromString( string ownerClass, string ownerValue)
            {
                switch (ownerClass.ToLower())
                {
                    case "user":
                        UserNode user = userManager.GetUser(ownerValue);

                        if (user != null)
                            return -user.Idnum;

                        goto default;
                    case "group":
                        GroupNode group = userManager.GetGroup(ownerValue);

                        if (group != null)
                            return group.Idnum;

                        goto default;
                    default:
                    case "public":
                        return 0;
                }
            }

            public string GetParameterCode(OperationState state, int parameterID)
            {
                //ParameterNode parameterNode = unitManager.GetUnitNode(state, parameterID) as ParameterNode;
                ParameterNode parameterNode = unitManager.CheckUnitNode<ParameterNode>(state, parameterID, Privileges.Read);

                if (parameterNode != null)
                    return parameterNode.Code;

                return null;
            }

            public UnitNode NewInstance(OperationState state, int unitTypeId)
            {
                return unitManager.NewInstance(state, unitTypeId);
            }

            public IEnumerable<Package> GetValues(OperationState state, ParameterNode parameterNode, DateTime startTime, DateTime endTime)
            {
                bool multiParse = true;

                DateTime currentStartTime = startTime;

                while (multiParse && currentStartTime < endTime)
                {
                    OperationState innerState = state.Sub();

                    valueReceiver.AsyncGetParameterValues(
                        innerState,
                        0f,
                        new ParameterValuesRequest()
                        {
                            Parameters = new Tuple<ParameterNode, ArgumentsValues>[] { Tuple.Create(parameterNode, (ArgumentsValues)null) },
                            StartTime = currentStartTime,
                            EndTime = endTime
                        },
                        false,
                        ref multiParse);

                    foreach (var item in innerState.AsyncResult)
                    {
                        Package pack = item as Package;

                        if (pack != null)
                        {
                            currentStartTime = pack.DateTo;
                            yield return pack;
                        }
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Сохранение импортированных узлов
        /// </summary>
        /// <param name="state">Состояние операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="importRootNode">
        /// Базовый узел, в который будет импортироваться данные. 
        /// Чтобы данные импортировались в корень - должен быть null.
        /// </param>
        /// <param name="importContainer">Обертка добавляемого дерева</param>
        public void ApplyImport(OperationState state, Guid userGUID, UnitNode importRootNode, ImportDataContainer importContainer)
        {
            SecurityManager.ValidateAdminAccess(userGUID);

            List<RevisionInfo> revisionList=new List<RevisionInfo>();
            GetRevisions(importContainer.Nodes, revisionList);
            Dictionary<int, int> changeRevisionTable = new Dictionary<int, int>();

            // Применяем информацию о ревизиях.
            // Связываем существующие ревизии с импортируемыми данными
            // и добавляем недостающие ревизии
            foreach (var revision in revisionList)
            {
                var systemRevision = RevisionManager.GetRevision(revision.Time);

                // если время не совпадает, добавляем новую ревизию
                if (!systemRevision.Time.Equals(revision.Time))
                {
                    systemRevision = new RevisionInfo(revision);
                    systemRevision.ID = 0;
                    RevisionManager.Update(systemRevision);
                    systemRevision = RevisionManager.GetRevision(revision.Time);
                }
                // если в системе есть ревизия с подходящим временем, принять её
                if (systemRevision.Time.Equals(revision.Time) && systemRevision.ID != revision.ID)
                    changeRevisionTable[revision.ID] = systemRevision.ID;
            }
            ChangeRevision(importContainer.Nodes, changeRevisionTable);

            // импортируем структуру
            UnitManager.ApplyImport(state, importRootNode, importContainer);

            // импортируем значения
            if (importContainer.Values != null)
                foreach (var item in importContainer.Values.Keys)
                {
                    ParameterNode parameterNode = UnitManager.GetParameter(state, item);

                    if (parameterNode != null)
                    {
                        Package[] packages = importContainer.Values[item];

                        foreach (var pack in importContainer.Values[item])
                        {
                            pack.Id = parameterNode.Idnum;
                        }
                        ValueReceiver.SaveValues(state, packages);
                    }
                }
        }

        private void ChangeRevision(TreeWrapp<UnitNode>[] treeWrapp, Dictionary<int, int> changeRevisionTable)
        {
            int newRevisionID;
            foreach (var unitNode in treeWrapp)
            {
                foreach (var revision in unitNode.Item.GetRevisions())
                {
                    if (changeRevisionTable.TryGetValue(revision.ID, out newRevisionID))
                    {
                        revision.ID = newRevisionID;
                    }
                }
                if (unitNode.ChildNodes != null)
                    ChangeRevision(unitNode.ChildNodes, changeRevisionTable);
            }
        }

        private void ChangeRevision(TreeWrapp<UnitNode>[] treeWrapp, int oldRevisionID, int newRevisionID)
        {
            foreach (var unitNode in treeWrapp)
            {
                foreach (var revision in unitNode.Item.GetRevisions())
                {
                    if (revision.ID == oldRevisionID)
                        revision.ID = newRevisionID;
                }
                if (unitNode.ChildNodes != null)
                    ChangeRevision(unitNode.ChildNodes, oldRevisionID, newRevisionID);
            }
        }

        private void GetRevisions(TreeWrapp<UnitNode>[] treeWrapp, List<RevisionInfo> revisionList)
        {
            foreach (var unitNode in treeWrapp)
            {
                foreach (var revision in unitNode.Item.GetRevisions())
                {
                    if (revision != RevisionInfo.Default
                        && !revisionList.Contains(revision))
                    {
                        revisionList.Add(revision);
                    }
                }
                if (unitNode.ChildNodes != null)
                    GetRevisions(unitNode.ChildNodes, revisionList);
            }
        }
    }
}

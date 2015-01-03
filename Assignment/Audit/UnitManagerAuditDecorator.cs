using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ASC.Audit;
using NLog;

namespace COTES.ISTOK.Assignment.Audit
{
    class UnitManagerAuditDecorator : IUnitManager
    {
        Logger log = LogManager.GetCurrentClassLogger();

        IAuditServer audit;

        IUnitManager unitManager;

        public IUnitTypeManager UnitTypes { get; set; }
        public ISecurityManager Security { get; set; }

        public UnitManagerAuditDecorator(IAuditServer audit, IUnitManager unitManager)
        {
            this.audit = audit;
            this.unitManager = unitManager;
        }

        private AuditEntry CreateAuditEntry(OperationState state)
        {
            var user = Security.GetUserInfo(state.UserGUID);

            var auditEntry = new AuditEntry(user);

            return auditEntry;
        }

        //private void WriteAuditEntry(AuditEntry auditEntry)
        //{
        //    if (!auditEntry.IsEmpty)
        //    {
        //        Audit.WriteAuditEntry(auditEntry);
        //    }
        //}

        private void AuditNodeChange(OperationState state, AuditEntry auditEntry, UnitNode oldNode, UnitNode newNode)
        {
            int idnum = (newNode ?? oldNode).Idnum;

            if (oldNode == null 
                || newNode == null 
                || !oldNode.FullName.Equals(newNode.FullName) 
                || !oldNode.Typ.Equals(newNode.Typ))
            {
                auditEntry.AuditUnits.Add(new AuditUnit()
                {
                    UnitNodeID = idnum,
                    FullPathOld = (oldNode == null ? null : oldNode.FullName),
                    FullPathNew = (newNode == null ? null : newNode.FullName),
                    TypeNameOld = (oldNode == null ? null : UnitTypes.GetUnitType(state, oldNode.Typ).Text),
                    TypeNameNew = (newNode == null ? null : UnitTypes.GetUnitType(state, newNode.Typ).Text)
                });
            }

            AuditPropsChange(auditEntry, oldNode, newNode);

            AuditLobsChange(auditEntry, oldNode, newNode);
        }

        private void AuditPropsChange(AuditEntry auditEntry, UnitNode oldNode, UnitNode newNode)
        {
            if (newNode != null)
            {
                IEnumerable<String> properties = newNode.Attributes.Keys;

                if (oldNode != null)
                {
                    properties = properties.Union(oldNode.Attributes.Keys);
                }

                foreach (var propertyName in properties)
                {
                    RevisedStorage<String> newStorage;
                    RevisedStorage<String> oldStorage = null;

                    if (oldNode != null)
                        oldNode.Attributes.TryGetValue(propertyName, out oldStorage);

                    newNode.Attributes.TryGetValue(propertyName, out newStorage);

                    IEnumerable<RevisionInfo> revisions;

                    if (newStorage == null)
                        revisions = oldStorage;
                    else if (oldStorage == null)
                        revisions = newStorage;
                    else
                        revisions = oldStorage.Union(newStorage);

                    foreach (var revision in revisions)
                    {
                        String oldValue = null;
                        String newValue = null;

                        if (oldStorage != null && oldStorage.Contains(revision))
                            oldValue = oldStorage.Get(revision);
                        if (newStorage != null && newStorage.Contains(revision))
                            newValue = newStorage.Get(revision);

                        if (!String.Equals(oldValue, newValue))
                        {
                            auditEntry.AuditProps.Add(new AuditProp()
                            {
                                UnitNodeID = newNode.Idnum,
                                UnitNodeFullPath = newNode.FullName,
                                PropName = propertyName,
                                RevisionID = (revision == RevisionInfo.Default ? null : (int?)revision.ID),
                                RevisionTime = (revision == RevisionInfo.Default ? null : (DateTime?)revision.Time),
                                RevisionBrief = (revision == RevisionInfo.Default ? null : revision.Brief),
                                ValueOld = oldValue,
                                ValueNew = newValue
                            });
                        }
                    }
                }
            }
        }

        private void AuditLobsChange(AuditEntry auditEntry, UnitNode oldNode, UnitNode newNode)
        {
            if (newNode != null)
            {
                IEnumerable<String> properties = newNode.Binaries.Keys;

                if (oldNode != null)
                {
                    properties = properties.Union(oldNode.Binaries.Keys);
                }

                foreach (var propertyName in properties)
                {
                    RevisedStorage<byte[]> newStorage;
                    RevisedStorage<byte[]> oldStorage = null;

                    if (oldNode != null)
                        oldNode.Binaries.TryGetValue(propertyName, out oldStorage);

                    newNode.Binaries.TryGetValue(propertyName, out newStorage);

                    IEnumerable<RevisionInfo> revisions;

                    if (newStorage == null)
                        revisions = oldStorage;
                    else if (oldStorage == null)
                        revisions = newStorage;
                    else
                        revisions = oldStorage.Union(newStorage);

                    foreach (var revision in revisions)
                    {
                        byte[] oldValue = null;
                        byte[] newValue = null;

                        if (oldStorage != null && oldStorage.Contains(revision))
                            oldValue = oldStorage.Get(revision);
                        if (newStorage != null && newStorage.Contains(revision))
                            newValue = newStorage.Get(revision);

                        if (!BinariesAreEquals(oldValue, newValue))
                        {
                            auditEntry.AuditLobs.Add(new AuditLob()
                            {
                                UnitNodeID = newNode.Idnum,
                                UnitNodeFullPath = newNode.FullName,
                                PropName = propertyName,
                                RevisionID = (revision == RevisionInfo.Default ? null : (int?)revision.ID),
                                RevisionTime = (revision == RevisionInfo.Default ? null : (DateTime?)revision.Time),
                                RevisionBrief = (revision == RevisionInfo.Default ? null : revision.Brief),
                                ValueOld = oldValue,
                                ValueNew = newValue
                            });
                        }
                    }
                }
            }
        }

        private bool BinariesAreEquals(byte[] oldValue, byte[] newValue)
        {
            if (oldValue == null && newValue == null)
            {
                return true;
            }
            if (oldValue == null || newValue == null)
            {
                return false;
            }
            if (oldValue.Length != newValue.Length)
            {
                return false;
            }
            for (int i = 0; i < oldValue.Length; i++)
            {
                if (!byte.Equals(oldValue[i], newValue[i]))
                {
                    return false;
                }
            }
            return true;
        }

        #region Audited IUnitManager Members

        public IEnumerable<UnitNode> AddUnitNode(OperationState state, IEnumerable<UnitNode> nodes, int parent)
        {
            var addedNodes = unitManager.AddUnitNode(state, nodes, parent);

            // создаем запись аудита
            var auditEntry = CreateAuditEntry(state);

            foreach (var item in addedNodes)
            {
                // Формируем список изменений для аудита
                AuditNodeChange(state, auditEntry, null, item);
            }

            log.Trace("Аудит добавление узла в структуру {0}.", auditEntry);

            // запись аудита
            audit.WriteAuditEntry(auditEntry);

            return addedNodes;
        }

        public UnitNode UpdateUnitNode(OperationState state, UnitNode updnode)
        {
            // сохраняем старое состояние
            var oldUnitNode = unitManager.ValidateUnitNode<UnitNode>(state, updnode.Idnum, Privileges.Read);

            // вызываем декорируемый метод
            var newUnitNode = unitManager.UpdateUnitNode(state, updnode);

            // создаем запись аудита
            var auditEntry = CreateAuditEntry(state);

            // сравниваем старое и новое состояние
            AuditNodeChange(state, auditEntry, oldUnitNode, newUnitNode);

            log.Trace("Аудит редактирование узла в структуре {0}.", auditEntry);

            // запись аудита
            audit.WriteAuditEntry(auditEntry);

            return newUnitNode;
        }

        public void RemoveUnitNode(OperationState state, int[] remnodes)
        {
            //// сохраняем старое состояние
            //List<UnitNode> nodesToRemove = new List<UnitNode>();

            //foreach (var item in remnodes)
            //{
            //    nodesToRemove.Add(unitManager.ValidateUnitNode<UnitNode>(state, item, Privileges.Read));
            //}

            // вызываем декорируемый метод
            unitManager.RemoveUnitNode(state, remnodes);

            // создаем запись аудита
            var auditEntry = CreateAuditEntry(state);

            //foreach (var item in nodesToRemove)
            //{
            //    // Формируем список изменений для аудита
            //    AuditNodeChange(state, auditEntry, item, null); 
            //}
            foreach (var item in state.AsyncResult)
            {
                var node = item as UnitNode;
                if (node!=null)
                {
                    // Формируем список изменений для аудита
                    AuditNodeChange(state, auditEntry, node, null); 
                }
            }

            log.Trace("Аудит удаление узла из структуры {0}.", auditEntry);

            // запись аудита
            audit.WriteAuditEntry(auditEntry);
        }

        public void CopyUnitNode(OperationState state, int[] unitIDs, bool recursive)
        {
            unitManager.CopyUnitNode(state, unitIDs, recursive);

            // создаем запись аудита
            var auditEntry = CreateAuditEntry(state);

            foreach (var obj in state.AsyncResult)
            {
                var node = obj as UnitNode;
                if (node!=null)
                {
                    // Формируем список изменений для аудита
                    AuditNodeChange(state, auditEntry, null, node); 
                }
            }

            log.Trace("Аудит копирования узла {0}.", auditEntry);

            // запись аудита
            audit.WriteAuditEntry(auditEntry);
        }

        public void MoveUnitNode(OperationState state, int newParentId, int unitNodeId, int index)
        {
            // сохраняем старое состояние
            var oldNode = unitManager.ValidateUnitNode<UnitNode>(state, unitNodeId, Privileges.Read).Clone() as UnitNode;

            // вызываем декорируемый метод
            unitManager.MoveUnitNode(state, newParentId, unitNodeId, index);

            // получаем новое состояние
            var newNode = unitManager.ValidateUnitNode<UnitNode>(state, unitNodeId, Privileges.Read).Clone() as UnitNode;

            // создаем запись аудита
            var auditEntry = CreateAuditEntry(state);

            // сравниваем старое и новое состояние
            AuditNodeChange(state, auditEntry, oldNode, newNode);

            log.Trace("Аудит перемещения узла по структуре {0}.", auditEntry);

            // запись аудита
            audit.WriteAuditEntry(auditEntry);
        }


        public void ApplyImport(OperationState state, UnitNode importRootNode, ImportDataContainer importContainer)
        {
            unitManager.ApplyImport(state, importRootNode, importContainer);

            // создаем запись аудита
            var auditEntry = CreateAuditEntry(state);

            foreach (var obj in state.AsyncResult)
            {
                var node = obj as TreeWrapp<UnitNode>;
                if (node != null)
                {
                    // Формируем список изменений для аудита
                    AuditNodeChange(state, auditEntry, node.OldItem, node.Item);
                }
            }

            log.Trace("Аудит импорта в структуру {0}.", auditEntry);

            // запись аудита
            audit.WriteAuditEntry(auditEntry);
        }


        #endregion

        #region Rest IUnitManager Members

        public void LoadUnits(OperationState state)
        {
            unitManager.LoadUnits(state);
        }

        public UnitNode NewInstance(OperationState state, int type)
        {
            return unitManager.NewInstance(state, type);
        }


        public UnitNode Find(OperationState state, UnitNode parentNode, int unitNodeID)
        {
            return unitManager.Find(state, parentNode, unitNodeID);
        }

        public UnitNode GetUnitNode(OperationState state, int unitId, int[] filterTypes, Privileges privileges)
        {
            return unitManager.GetUnitNode(state, unitId, filterTypes, privileges);
        }

        public UnitNode[] GetUnitNodes(OperationState state, int id, ParameterFilter filter, RevisionInfo revision)
        {
            return unitManager.GetUnitNodes(state, id, filter, revision);
        }

        public UnitNode[] GetUnitNodes(OperationState state, int id, int[] filterTypes, Privileges privileges)
        {
            return unitManager.GetUnitNodes(state, id, filterTypes, privileges);
        }

        public UnitNode[] GetUnitNodes(OperationState state, int[] ids)
        {
            return unitManager.GetUnitNodes(state, ids);
        }

        public UnitNode[] GetAllUnitNodes(OperationState state, int id, int[] filterTypes, Privileges privileges)
        {
            return unitManager.GetAllUnitNodes(state, id, filterTypes, privileges);
        }

        public UnitNode[] GetAllUnitNodes(OperationState state, int id, int[] filterTypes, int minLevel, int maxLevel, Privileges privileges)
        {
            return unitManager.GetAllUnitNodes(state, id, filterTypes, minLevel, maxLevel, privileges);
        }

        public UnitNode GetParentNode(OperationState state, UnitNode unitNode, int unitTypeId)
        {
            return unitManager.GetParentNode(state, unitNode, unitTypeId);
        }

        public UnitNode[] GetChildNodes(OperationState state, UnitNode unitNode, int unitTypeId)
        {
            return unitManager.GetChildNodes(state, unitNode, unitTypeId);
        }

        public TreeWrapp<UnitNode>[] GetUnitNodeTree(OperationState state, int[] unitIds, int[] filterTypes, Privileges privileges)
        {
            return unitManager.GetUnitNodeTree(state, unitIds, filterTypes, privileges);
        }

        public NormFuncNode[] GetNormFuncs(OperationState state)
        {
            return unitManager.GetNormFuncs(state);
        }

        public ParameterNode GetParameter(OperationState state, string code)
        {
            return unitManager.GetParameter(state, code);
        }

        public IEnumerable<ParameterNode> GetParameters(OperationState state)
        {
            return unitManager.GetParameters(state);
        }

        public DataSet GetBlockData(OperationState state, int id_block)
        {
            return unitManager.GetBlockData(state, id_block);
        }

        public DataSet GetBlockData(OperationState state, int id_block, int id_chanell)
        {
            return unitManager.GetBlockData(state, id_block, id_chanell);
        }

        public byte[] SerializeDataSet(OperationState state, DataSet ds)
        {
            return unitManager.SerializeDataSet(state, ds);
        }

        public string GetFullName(UnitNode unitNode)
        {
            return unitManager.GetFullName(unitNode);
        }

        public string[] GetRequiredArguments(OperationState state, UnitNode unitNode)
        {
            return unitManager.GetRequiredArguments(state, unitNode);
        }

        public Dictionary<int, int> GetStatistic(OperationState state, UnitNode unitNode)
        {
            return unitManager.GetStatistic(state, unitNode);
        }

        public IntervalDescription[] GetStandardsIntervals(OperationState state)
        {
            return unitManager.GetStandardsIntervals(state);
        }

        public void RemoveStandardIntervals(OperationState state, IEnumerable<IntervalDescription> intervalsToRemove)
        {
            unitManager.RemoveStandardIntervals(state, intervalsToRemove);
        }

        public void SaveStandardIntervals(OperationState state, IEnumerable<IntervalDescription> modifiedIntervals)
        {
            unitManager.SaveStandardIntervals(state, modifiedIntervals);
        }

        public ParameterNode[] GetParametersForSchedule(OperationState state, string uid, int schedule_id)
        {
            return unitManager.GetParametersForSchedule(state, uid, schedule_id);
        }

        public T ValidateParentNode<T>(OperationState state, int unitNodeID, Privileges privileges) where T : UnitNode
        {
            return unitManager.ValidateParentNode<T>(state, unitNodeID, privileges);
        }

        public T CheckParentNode<T>(OperationState operationState, int unitNodeID, Privileges privileges) where T : UnitNode
        {
            return unitManager.CheckParentNode<T>(operationState, unitNodeID, privileges);
        }

        public UnitNode ValidateUnitNode(OperationState state, int unitNodeID, Privileges privileges)
        {
            return unitManager.ValidateUnitNode(state, unitNodeID, privileges);
        }

        public T ValidateUnitNode<T>(OperationState state, int unitNodeID, Privileges privileges) where T : UnitNode
        {
            return unitManager.ValidateUnitNode<T>(state, unitNodeID, privileges);
        }

        public T CheckUnitNode<T>(OperationState state, int unitNodeID, Privileges privileges) where T : UnitNode
        {
            return unitManager.CheckUnitNode<T>(state, unitNodeID, privileges);
        }

        public event EventHandler<UnitNodeEventArgs> UnitNodeChanged
        {
            add { unitManager.UnitNodeChanged += value; }
            remove { unitManager.UnitNodeChanged -= value; }
        }

        public event EventHandler<UnitNodeEventArgs> UnitNodeLoaded
        {
            add { unitManager.UnitNodeLoaded += value; }
            remove { unitManager.UnitNodeLoaded -= value; }
        }

        #endregion
    }
}

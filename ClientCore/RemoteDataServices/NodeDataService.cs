using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace COTES.ISTOK.ClientCore
{
    [KnownType(typeof(UTypeNode))]
    public class NodeDataService : AsyncGlobalWorker
    {
        /// <summary>
        /// Кэшированные ЮнитНоды
        /// </summary>
        Dictionary<int, UnitNode> dicUnitNodes = new Dictionary<int, UnitNode>();

        internal NodeDataService(Session session)
            : base(session)
        {
            //
        }

        #region Types
        public ulong BeginGetUnitTypes()
        {
            //return qManager.BeginGetUnitTypes(session.Uid);
            throw new NotImplementedException();
        }
        public UTypeNode[] GetUnitTypes()
        {
            string opid = "GetUnitTypes" + Guid.NewGuid().ToString();
            try
            {
                var res= AllocQManager(opid).GetUnitTypes(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UTypeNode[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void RemoveUnitType(int[] unitTypeNodesIDs)
        {
            string opid = "RemoveUnitType" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).RemoveUnitType(session.Uid, unitTypeNodesIDs);
                session.CommitDataChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public void AddUnitType(UTypeNode uTypeNode)
        {
            string opid = "AddUnitType" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).AddUnitType(session.Uid, uTypeNode);
                session.CommitDataChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void UpdateUnitType(UTypeNode uTypeNode)
        {
            string opid = "UpdateUnitType" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).UpdateUnitType(session.Uid, uTypeNode);
                session.CommitDataChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        #endregion

        #region Nodes
        //public ulong BeginGetAllUnitNodes( int parentId, int[] filterTypes)
        //{
        //    //return qManager.BeginGetAllUnitNodes(session.Uid, parentId, filterTypes);
        //    throw new NotImplementedException();
        //}
        public UnitNode[] GetAllUnitNodes(int parentId, int[] filterTypes)
        {
            string opid = "GetAllUnitNodes" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetAllUnitNodes(session.Uid, parentId, filterTypes);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UnitNode[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public UnitNode[] GetAllUnitNodes(int parentId, int[] typeFilter, int minLevel, int maxLevel)
        {
            string opid = "GetAllUnitNodes" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetAllUnitNodesMinMax(session.Uid, parentId, typeFilter, minLevel, maxLevel);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UnitNode[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public ulong BeginGetUnitNodesFiltered(int parent, int[] filterTypes)
        {
            //return qManager.BeginGetUnitNodesFiltered(session.Uid, parent, filterTypes);
            throw new NotImplementedException();
        }
        public UnitNode[] GetUnitNodesFiltered(int parent, int[] filterTypes)
        {
            string opid = "GetUnitNodesFiltered" + Guid.NewGuid().ToString();
            try
            {
                var man = AllocQManager(opid);
                man.Test(null);
                var res = man.GetUnitNodesFiltered(session.Uid, parent, filterTypes);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UnitNode[]>(ex);
            }
            //catch (Exception) { throw; }
            finally
            {
                FreeQManager(opid);
            }
        }

        public UnitNode[] GetUnitNodesFiltered(int parent, ParameterFilter filterTypes)
        {
            string opid = "GetUnitNodesFiltered" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetUnitNodes2(session.Uid, parent, filterTypes, session.CurrentRevision);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UnitNode[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public UnitNode GetUnitNode(int node)
        {
            string opid = "GetUnitNode" + Guid.NewGuid().ToString();
            try
            {
                if (node <= 0)
                {
                    return null;
                }
                var unode = GetCachedUnitNode(node);
                if (unode == null)
                {
                    var res = AllocQManager(opid).GetUnitNode(session.Uid, node);
                    CommitChanges(res.Changes);
                    UpdateCachedNodes(new UnitNode[] { res.Result });
                    return res.Result;
                }
                else
                    return unode;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UnitNode>(ex);
            }
            catch (Exception) { throw; }
            finally
            {
                FreeQManager(opid);
            }
        }

        public UnitNode GetUnitNode(string code)
        {
            string opid = "GetUnitNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetUnitNodeByCode(session.Uid, code);
                CommitChanges(res.Changes);
                UpdateCachedNodes(new UnitNode[] { res.Result });
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UnitNode>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public UnitNode GetParent(UnitNode unitNode, int typeID)
        {
            string opid = "GetParent" + Guid.NewGuid().ToString();
            try
            {
                var node = GetCachedParentUnitNode(unitNode.Idnum, typeID);
                if (node == null)
                {
                    var res = AllocQManager(opid).GetParentUnitNode(session.Uid, unitNode.Idnum, typeID);
                    CommitChanges(res.Changes);
                    node = res.Result;
                    if (node != null) UpdateCachedNode(node);
                }
                return node;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UnitNode>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public int AddUnitNode(int type, int parentId)
        {
            string opid = "AddUnitNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).AddUnitNode(session.Uid, type, parentId);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<int>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void AddUnitNodes(UnitNode[] unitNodes, UnitNode parentNode)
        {
            string opid = "AddUnitNodes" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).AddUnitNodes(session.Uid, unitNodes, parentNode.Idnum);
                CommitChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public UnitNode UpdateUnitNode(UnitNode unitNode)
        {
            string opid = "UpdateUnitNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).UpdateUnitNode(session.Uid, unitNode);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UnitNode>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void LockNode(UnitNode unitNode)
        {
            string opid = "LockNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).LockNode(session.Uid, unitNode.Idnum);
                CommitChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void ReleaseAll(UnitNode unitNode)
        {
            string opid = "ReleaseAll" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).ReleaseAll(session.Uid, unitNode.Idnum);
                CommitChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public void ReleaseNode(UnitNode unitNode)
        {
            string opid = "ReleaseNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).ReleaseNode(session.Uid, unitNode.Idnum);
                CommitChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public TreeWrapp<UnitNode>[] GetUnitNodeTree(int[] unitNodeIds, int[] filterTypes, Privileges privileges)
        {
            string opid = "GetUnitNodeTree" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetUnitNodeTree(session.Uid, unitNodeIds, filterTypes, privileges);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<TreeWrapp<UnitNode>[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public ParameterNodeDependence[] GetDependence(RevisionInfo revisionInfo, int parameterId)
        {
            string opid = "GetDependence" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetDependence(session.Uid, revisionInfo, parameterId);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ParameterNodeDependence[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public ParameterNodeReference[] GetReference(string code)
        {
            string opid = "GetReference" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetReference(session.Uid, code);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ParameterNodeReference[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void ChangeParameterCode(UnitNode oldNode, UnitNode newNode, IEnumerable<ParameterNodeReference> nodes)
        {
            string opid = "ChangeParameterCode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).ChangeParameterCode(session.Uid, oldNode.Code, newNode.Code, nodes.ToArray());
                CommitChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public UnitNode[] GetUnitNodes(int[] unitNodeIds)
        {
            string opid = "GetUnitNodes" + Guid.NewGuid().ToString();
            try
            {
                var result = new UnitNode[unitNodeIds.Length];
                var dicPositions = new Dictionary<int, int>();
                var lstIds = new List<int>();

                //фильтрация недостающих в кэше нодов, с учетом позиции в массиве запрашиваемых id
                UnitNode node;
                for (int i = 0; i < unitNodeIds.Length; i++)
                {
                    node = GetCachedUnitNode(unitNodeIds[i]);
                    if (node == null)
                        dicPositions[unitNodeIds[i]] = i;
                    else
                        result[i] = node;
                }

                if (dicPositions.Count > 0)
                {
                    var res = AllocQManager(opid).GetUnitNodes(session.Uid, dicPositions.Keys.ToArray());
                    CommitChanges(res.Changes);
                    UpdateCachedNodes(res.Result);
                    foreach (var inode in res.Result)
                        result[dicPositions[inode.Idnum]] = inode;
                }

                //хотфикс: возвращаемые нулы (если у юзера нет прав на нод)
                //генерят ошибки на верхних уровнях, поэтому убираем их в ущерб
                //сохранения позиции относительно запрашиваемого массива идентификаторов
                if (result.Contains(null))
                {
                    result = (from elem in result
                             where elem != null
                             select elem).ToArray();
                }

                return result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UnitNode[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void DeleteUnitNode(int[] unitNodeIds)
        {
            string opid = "DeleteUnitNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).RemoveUnitNodes(session.Uid, unitNodeIds);
                CommitChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public ulong BeginDeleteUnitNode(Guid userGuid, int[] unitNodeIds)
        {
            string opid = "BeginDeleteUnitNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).BeginRemoveUnitNodes(userGuid, unitNodeIds);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ulong>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void CopyUnitNode(int[] unitNodeIds, bool recursive)
        {
            string opid = "CopyUnitNode" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).CopyUnitNode(session.Uid, unitNodeIds, recursive);
                CommitChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void MoveUnitNode(UnitNode parent, UnitNode node, UnitNode neighbor, bool addAfter)
        {
            string opid = "MoveUnitNode" + Guid.NewGuid().ToString();
            try
            {
                var oldParent = GetCachedUnitNode(node.ParentId);
                var res = AllocQManager(opid).MoveUnitNode(session.Uid,
                                                       parent == null ? 0 : parent.Idnum,
                                                       node.Idnum,
                                                       neighbor == null ? 0 : neighbor.Idnum,
                                                       addAfter);
                if (oldParent != null)
                {
                    DropChildrenCache(oldParent);
                }
                if (parent != oldParent)
                {
                    DropChildrenCache(parent);
                }
                CommitChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        #endregion

        #region Access
        /// <summary>
        /// Проверить права текущего подключенного пользователя
        /// </summary>
        /// <param name="node">Узел</param>
        /// <param name="privileges">Права</param>
        /// <returns>true, если права пользователя удовлетворяют запрошенным</returns>
        public bool CheckAccess(UserNode user, UnitNode node, Privileges privileges)
        {
            int type = node.Typ;
            int owner = node.Owner;

            return user.IsAdmin || (user.CheckPrivileges(type, privileges) && user.CheckGroupPrivilegies(owner, privileges));
        }
        #endregion

        public Interval GetParameterInterval(int parameterId)
        {
            string opid = "GetParameterInterval" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetParameterInterval(session.Uid, parameterId);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<Interval>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public double GetLoadProgress()
        {
            string opid = "GetLoadProgress" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetLoadProgress(session.Uid);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<double>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public string GetLoadStatusString()
        {
            string opid = "GetLoadStatusString" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetLoadStatusString(session.Uid);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<string>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public bool HasReference(string code)
        {
            string opid = "HasReference" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).HasReference(session.Uid, code);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<bool>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public Dictionary<int, int> GetStatistic(int nodeId)
        {
            string opid = "GetStatistic" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetStatistic(session.Uid, nodeId);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<Dictionary<int, int>>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public ulong BeginExport(int[] nodeIds, DateTime beginTime, DateTime endTime, ExportFormat exportFormat)
        {
            string opid = "BeginExport" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).BeginExport(session.Uid, nodeIds, beginTime, endTime, exportFormat);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ulong>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        internal ImportDataContainer Import(byte[] buf, ExportFormat exportFormat)
        {
            string opid = "Import" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).BeginImport(session.Uid, buf, exportFormat);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ImportDataContainer>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public ulong BeginApplyImport(UnitNode rootNode, ImportDataContainer importContainer)
        {
            string opid = "BeginApplyImport" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).BeginApplyImport(session.Uid, rootNode == null ? 0 : rootNode.Idnum, importContainer);
                CommitChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ulong>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        #region Internal Methods
        private void DropChildrenCache(UnitNode parentNode)
        {
            if (parentNode != null)
            {
                lock (dicUnitNodes)
                {
                    foreach (var nodeId in parentNode.NodesIds)
                    {
                        dicUnitNodes.Remove(nodeId);
                    } 
                }
            }
        }
        private UnitNode GetCachedParentUnitNode(int nodeId, int typeId)
        {
            var ptr = GetCachedUnitNode(nodeId);
            if (ptr != null)
                if (ptr.ParentId != 0)
                {
                    ptr = GetCachedUnitNode(ptr.ParentId);
                }
                else
                    ptr = null;
            while (ptr != null)
            {
                if (ptr.Typ == typeId) return ptr;
                ptr = GetCachedUnitNode(ptr.ParentId);
            }
            return null;
        }
        private UnitNode GetCachedUnitNode(int nodeId)
        {
            lock (dicUnitNodes)
            {
                if (dicUnitNodes.ContainsKey(nodeId))
                    return dicUnitNodes[nodeId];
            }
            return null;
        }
        private void UpdateCachedNode(UnitNode node)
        {
            if (node == null) throw new ArgumentNullException("node");
            lock (dicUnitNodes)
            {
                dicUnitNodes[node.Idnum] = node;
            }
        }
        private void UpdateCachedNodes(UnitNode[] nodes)
        {
            if (nodes == null) throw new ArgumentNullException("nodes");

            foreach (var node in nodes)
                if (node != null) UpdateCachedNode(node);
        }
        //TODO: дубляж кода с сессией, надо что-то одно оставить
        private void CommitChanges(IEnumerable<ServiceDataChange> changes)
        {
            if (changes != null && changes.Count() > 0)
            {
                // it shall be called in separate thread, basically
                ProccessChanges(changes);
            }
            session.CommitDataChanges(changes);
        }
        private void ProccessChanges(IEnumerable<ServiceDataChange> changes)
        {
            foreach (var item in changes)
            {
                ServiceUnitNodeChange unitNodeChange;
                //ServiceUnitTypeChange unitTypeChange;

                if ((unitNodeChange = item as ServiceUnitNodeChange) != null)
                {
                    foreach (var unitNodeID in unitNodeChange.NodeIDs)
                        lock (dicUnitNodes)
                            dicUnitNodes.Remove(unitNodeID);
                }
            }
        }
        #endregion
    }
}

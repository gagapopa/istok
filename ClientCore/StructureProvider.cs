using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;
using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.Extension;

namespace COTES.ISTOK.ClientCore
{
    public class StructureProvider : AsyncRDSWorker, IUnitProviderSupplier, IDisposable
    {
        /// <summary>
        /// Фильтрация определенных типов узлов
        /// </summary>
        public int[] FilteredTypes { get; set; }

        public RevisionInfo CurrentRevision
        {
            get { return Session.CurrentRevision; }
            set { Session.CurrentRevision = value; }
        }

        public event EventHandler CurrentRevisionChanged
        {
            add { Session.CurrentRevisionChanged += value; }
            remove { Session.CurrentRevisionChanged -= value; }
        }

        protected FilterParams filter;
        /// <summary>
        /// Фильтрация групп параметров (создавалось для меню тонкого клиента)
        /// </summary>
        public FilterParams FilterParams
        {
            get { return filter; }
            set
            {
                filter = value;
                switch (filter)
                {
                    case FilterParams.All:
                        FilteredTypes = new int[] { };
                        break;
                    case FilterParams.TepParameters:
                        FilteredTypes = new int[] { (int)UnitTypeId.ManualGate, (int)UnitTypeId.ManualParameter, (int)UnitTypeId.TEP, (int)UnitTypeId.TEPTemplate };
                        break;
                    case FilterParams.Schemas:
                        FilteredTypes = new int[] { (int)UnitTypeId.Schema };
                        break;
                    case FilterParams.Reports:
                        FilteredTypes = new int[] { (int)UnitTypeId.Report, (int)UnitTypeId.ExcelReport };
                        break;
                    case FilterParams.Graphs:
                        FilteredTypes = new int[] { (int)UnitTypeId.Graph };
                        break;
                    case FilterParams.NormFunctions:
                        FilteredTypes = new int[] { (int)UnitTypeId.NormFunc };
                        break;
                    default:
                        break;
                }
            }
        }

        public UnitNode CurrentNode { get; set; }
        public UnitProvider CurrentUnitProvider { get; set; }

        public Dictionary<string, object> Data { get; private set; }

        List<UnitNode> lockedParameters = new List<UnitNode>();

        internal StructureProvider(Session session)
            : base(session)
        {
            //CurrentRevision = RevisionInfo.Default;
            Data = new Dictionary<string, object>();
            Session.UnitNodeChanged += session_UnitNodeChanged;
        }

        void session_UnitNodeChanged(object sender, UnitNodeEventArgs e)
        {
            UnitProvider provider;

            int unitID = e.UnitNode == null ? e.UnitNodeID : e.UnitNode.Idnum;

            if (unitProviderDictionary.TryGetValue(unitID, out provider))
            {
                //TODO: удалить вкладки
                unitProviderDictionary.Remove(unitID);
                provider.Dispose();
            }
        }

        #region Working with Structure(tree)
        /// <summary>
        /// Проверить права текущего подключенного пользователя
        /// </summary>
        /// <param name="node">Узел</param>
        /// <param name="privileges">Права</param>
        /// <returns>true, если права пользователя удовлетворяют запрошенным</returns>
        public bool CheckAccess(UnitNode node, Privileges privileges)
        {
            int owner = node.Owner;

            return Session.User.IsAdmin || (Session.User.CheckPrivileges(node.Typ, privileges) && Session.User.CheckGroupPrivilegies(owner, privileges));
        }

        //public ulong BeginGetUnitNode(int node)
        //{
        //    return RDS.NodeDataService.BeginGetUnitNode( node);
        //}
        public UnitNode GetUnitNode(int node)
        {
            return RDS.NodeDataService.GetUnitNode(node);
        }

        //public ulong BeginGetUnitNodesFiltered(int parent)
        //{
        //    return RDS.NodeDataService.BeginGetUnitNodesFiltered( parent, FilteredTypes);
        //}
        public UnitNode[] GetUnitNodesFiltered(int parent)
        {
            return RDS.NodeDataService.GetUnitNodesFiltered(parent, FilteredTypes);
        }

        public UnitNode[] GetUnitNodesFiltered(int parent, ParameterFilter parameterFilter)
        {
            return RDS.NodeDataService.GetUnitNodesFiltered(parent, parameterFilter);
        }

        public UnitNode GetUnitNode(string code)
        {
            return RDS.NodeDataService.GetUnitNode(code);
        }

        public UnitNode[] GetUnitNodes(int[] unitNodeIds)
        {
            return RDS.NodeDataService.GetUnitNodes(unitNodeIds);
        }

        public UnitNode UpdateUnitNode(UnitNode unitNode)
        {
            return RDS.NodeDataService.UpdateUnitNode(unitNode);
        }

        public int AddUnitNode(int type, int parentId)
        {
            return RDS.NodeDataService.AddUnitNode(type, parentId);
        }
        public void AddUnitNodes(UnitNode[] unitNodes, UnitNode parentNode)
        {
            RDS.NodeDataService.AddUnitNodes(unitNodes, parentNode);
        }
        public void DeleteUnitNode(int[] unitNodeIds)
        {
            RDS.NodeDataService.DeleteUnitNode(unitNodeIds);
        }
        public AsyncOperationWatcher<UAsyncResult> BeginDeleteUnitNode(int[] unitNodeIds)
        {
            var op = RDS.NodeDataService.BeginDeleteUnitNode(Session.Uid, unitNodeIds);

            return new AsyncOperationWatcher<UAsyncResult>(op, RDS);
        }

        public void ReleaseAll(UnitNode unitNode)
        {
            RDS.NodeDataService.ReleaseAll(unitNode);
        }
        public void ReleaseNode()
        {
            foreach (var item in lockedParameters)
                RDS.NodeDataService.ReleaseNode(item);
        }

        public TreeWrapp<UnitNode>[] GetUnitNodeTree(int[] unitNodeIds, int[] filterTypes, Privileges privileges)
        {
            return RDS.NodeDataService.GetUnitNodeTree(unitNodeIds, filterTypes, privileges);
        }

        public ParameterNodeDependence[] GetDependence(RevisionInfo revisionInfo, int parameterId)
        {
            return RDS.NodeDataService.GetDependence(revisionInfo, parameterId);
        }
        public ParameterNodeReference[] GetReference(string code)
        {
            return RDS.NodeDataService.GetReference(code);
        }

        public Interval GetParameterInterval(int parameterId)
        {
            if (parameterId<0)
            {
                return Interval.Zero;
            }
            return RDS.NodeDataService.GetParameterInterval(parameterId);
        }

        public bool HasReference(string code)
        {
            return RDS.NodeDataService.HasReference(code);
        }

        public void MoveUnitNode(UnitNode parent, UnitNode node, UnitNode neighbor, bool addAfter)
        {
            RDS.NodeDataService.MoveUnitNode(parent, node, neighbor, addAfter);
        }
        public void CopyUnitNode(int[] unitNodeIds, bool recursive)
        {
            RDS.NodeDataService.CopyUnitNode(unitNodeIds, recursive);
        }
        #endregion

        #region Async operations
        #endregion

        #region UnitProvider
        Dictionary<int, UnitProvider> unitProviderDictionary = new Dictionary<int, UnitProvider>();

        public virtual UnitProvider GetUnitProvider(UnitNode unitNode)
        {
            UnitProvider unitProvider;

            if (!unitProviderDictionary.TryGetValue(unitNode.Idnum, out unitProvider)
                //хз нужна эта строка или нет. суть - в кэше юнитпровайдер может хранить нод, ссылка которого
                //не будет совпадать с передаваемым unitNode (не произойдет сохранения в таких UnitEditForm'ах,
                //которые делают UnitNode.Equals(unitprovider.UnitNode) (если такие есть или будут))
                //|| (unitProvider != null && unitProvider.UnitNode != unitNode)
                )
            {
                unitProviderDictionary[unitNode.Idnum] = unitProvider = CreateUnitProvider(unitNode);
            }
            return unitProvider;
        }

        protected virtual UnitProvider CreateUnitProvider(UnitNode unitNode)
        {
            switch (unitNode.Typ)
            {
                case (int)UnitTypeId.Unknown:
                    return new UnitProvider(this, unitNode);
                case (int)UnitTypeId.Station:
                    return new UnitProvider(this, unitNode);
                case (int)UnitTypeId.Block:
                    return new UnitProvider(this, unitNode);
                case (int)UnitTypeId.Channel:
                    return new UnitProvider(this, unitNode);
                case (int)UnitTypeId.Parameter:
                    return new UnitProvider(this, unitNode);
                case (int)UnitTypeId.TEP:
                    return new FormulaUnitProvider(this, unitNode as CalcParameterNode);
                case (int)UnitTypeId.Graph:
                    return new GraphUnitProvider(this, unitNode as GraphNode);
                case (int)UnitTypeId.Histogram:
                    return new GraphUnitProvider(this, unitNode as HistogramNode);
                case (int)UnitTypeId.Schema:
                    return new SchemaUnitProvider(this, unitNode as SchemaNode);
                //case UnitTypeId.MonitorTable:
                //    return new MonitorTableUnitProvider(unitNode as MonitorTableNode, uniForm, rds);
                //case UnitTypeId.ManualGate:
                //    return new ParameterGateUnitProvider(unitNode, uniForm, rds);
                case (int)UnitTypeId.ManualParameter:
                    return new UnitProvider(this, unitNode);
                case (int)UnitTypeId.Report:
                    return new ReportUnitProvider(this, unitNode as ReportNode);
                case (int)UnitTypeId.Folder:
                    return new MulticontrolUnitProvider(this, unitNode);
                case (int)UnitTypeId.NormFunc:
                    return new NormFuncUnitProvider(this, unitNode as NormFuncNode);
                case (int)UnitTypeId.ExcelReport:
                    return new ExcelReportUnitProvider(this, unitNode as ExcelReportNode);
                case (int)UnitTypeId.ManualGate:
                case (int)UnitTypeId.TEPTemplate:
                    //UnitNode tempNode = unitNode;
                    //OptimizationGateNode optimizationNode = null;
                    //while (tempNode != null && (optimizationNode = tempNode as OptimizationGateNode) == null)
                    //    tempNode = RDS.NodeDataService.GetUnitNode( tempNode.ParentId);
                    var optimizationNode = RDS.NodeDataService.GetParent(unitNode, (int)UnitTypeId.OptimizeCalc) as OptimizationGateNode;
                    if (optimizationNode != null)
                        return new CParameterGateProvider(this, unitNode as ParameterGateNode);
                    return new ParameterGateUnitProvider(this, unitNode);
                //case UnitTypeId.Boiler:
                //    return new MonitorTableUnitProvider(unitNode as MonitorTableNode, uniForm, rds);
                case (int)UnitTypeId.OptimizeCalc:
                    return new OptimizationUnitProvider(this, unitNode as OptimizationGateNode);
                default:
                    if (unitNode is ExtensionUnitNode)
                        return new ExtensionUnitProvider(this, unitNode as ExtensionUnitNode);

                    return new UnitProvider(this, unitNode);
            }

            throw new NotSupportedException();
        }

        #region IUnitProviderSupplier members
        public IUnitNodeProvider GetProvider(UnitNode unitNode)
        {
            return GetUnitProvider(unitNode);
        }
        #endregion
        #endregion

        #region Values
        public void DeleteLoadValues(int unitNodeId, DateTime dateTime)
        {
            RDS.ValuesDataService.DeleteLoadValues(unitNodeId, dateTime);
        }
        public void DeleteValues(int parameterId, DateTime[] dateTime)
        {
            RDS.ValuesDataService.DeleteValues(parameterId, dateTime);
        }
        public void SaveValues(Package[] package)
        {
            RDS.ValuesDataService.SaveValues(package);
        }
        public AsyncOperationWatcher<UAsyncResult> BeginGetValues(int parameterId, ArgumentsValues arguments, DateTime startTime, DateTime endTime)
        {
            var op = RDS.ValuesDataService.BeginGetValues(parameterId, arguments, startTime, endTime);
            var res = new AsyncOperationWatcher<UAsyncResult>(op, RDS);
            return res;
        }
        #endregion

        public UnitNode[] GetAllUnitNodes(int rootNodeId, int[] filter)
        {
            return RDS.NodeDataService.GetAllUnitNodes(rootNodeId, filter);
        }

        public Dictionary<int, int> GetStatistic(int nodeId)
        {
            return RDS.NodeDataService.GetStatistic(nodeId);
        }

        public UnitNode GetParent(UnitNode unitNode, int unitTypeId)
        {
            return RDS.NodeDataService.GetParent(unitNode, unitTypeId);
        }

        #region Reports
        public ReportSourceInfo[] GetReportSources()
        {
            return RDS.ReportDataService.GetReportSources();
        }
        public ReportSourceSettings GetReportSourceSettings(Guid reportSourceID)
        {
            return RDS.ReportDataService.GetReportSourceSettings(reportSourceID);
        }
        public FastReportWrap GenerateReportData(ReportSourceSettings[] reportSourceSettings, ReportParameter[] reportParameter)
        {
            return RDS.ReportDataService.GenerateReportData(reportSourceSettings, reportParameter);
        }
        public FastReportWrap GenerateEmptyReportData(ReportSourceSettings[] reportSourceSettings)
        {
            return RDS.ReportDataService.GenerateEmptyReportData(reportSourceSettings);
        }
        public PreferedReportInfo[] GetPreferedReports(DateTime dateTime1, DateTime dateTime2)
        {
            return RDS.ReportDataService.GetPreferedReports(dateTime1, dateTime2);
        }
        public byte[] GetPreferedReportBody(PreferedReportInfo reportInfo)
        {
            return RDS.ReportDataService.GetPreferedReportBody(reportInfo);
        }
        public void DeletePreferedReport(PreferedReportInfo reportInfo)
        {
            RDS.ReportDataService.DeletePreferedReport(reportInfo);
        }
        #endregion

        #region Extensions
        public ExtensionDataInfo[] GetExtensionTableInfo(int unitNodeId)
        {
            return RDS.ExtensionDataService.GetExtensionTableInfo(unitNodeId);
        }
        public ExtensionDataInfo[] GetExtensionTableInfo(string extensionName)
        {
            return RDS.ExtensionDataService.GetExtensionTableInfo(extensionName);
        }

        public ExtensionData GetExtensionExtendedTable(int unitNodeId, string tabKeyword, DateTime dateFrom, DateTime dateTo)
        {
            return RDS.ExtensionDataService.GetExtensionExtendedTable(unitNodeId, tabKeyword, dateFrom, dateTo);
        }        
        public ExtensionData GetExtensionExtendedTable(int unitNodeId, string tabKeyword)
        {
            return RDS.ExtensionDataService.GetExtensionExtendedTable(unitNodeId, tabKeyword);
        }
        public ExtensionData GetExtensionExtendedTable(string extensionName, string tabKeyword)
        {
            return RDS.ExtensionDataService.GetExtensionExtendedTable(extensionName, tabKeyword);
        }
        #endregion

        #region Locks
        public void LockNode(UnitNode node)
        {
            RDS.NodeDataService.LockNode(node);
        }

        public void ReleaseNode(UnitNode node)
        {
            RDS.NodeDataService.ReleaseNode(node);
        }
        public void LockValues(UnitNode node, DateTime startTime, DateTime endTime)
        {
            RDS.ValuesDataService.LockValues(node, startTime, endTime);
        }

        public void ReleaseValues(UnitNode node, DateTime startTime, DateTime endTime)
        {
            RDS.ValuesDataService.ReleaseValues(node, startTime, endTime);
        }
        #endregion

        public AsyncOperationWatcher<UAsyncResult> BeginExport(int[] nodeIds, DateTime beginTime, DateTime endTime, ExportFormat exportFormat)
        {
            var op = RDS.NodeDataService.BeginExport(nodeIds, beginTime, endTime, exportFormat);

            return new AsyncOperationWatcher<UAsyncResult>(op, RDS);
        }

        public ImportDataContainer Import(byte[] buf, ExportFormat exportFormat)
        {
            return RDS.NodeDataService.Import(buf, exportFormat);
        }

        public AsyncOperationWatcher BeginApplyImport(UnitNode rootNode, ImportDataContainer importContainer)
        {
            var op = RDS.NodeDataService.BeginApplyImport(rootNode, importContainer);

            return new AsyncOperationWatcher<UAsyncResult>(op, RDS);
        }

        System.ComponentModel.ISite serviceContainer;
        public System.ComponentModel.ISite GetServiceContainer()
        {
            if (serviceContainer == null)
            {
                FormOrientedServiceContainer formOrientedServiceContainer;
                formOrientedServiceContainer = new FormOrientedServiceContainer(Session.GetServiceContainer());
                formOrientedServiceContainer.AddService(this);
                serviceContainer = formOrientedServiceContainer;
            }
            return serviceContainer;
        }

        public void ChangeParameterCode(UnitNode oldNode, UnitNode newNode, IEnumerable<ParameterNodeReference> changeCodeNodes)
        {
            RDS.NodeDataService.ChangeParameterCode(oldNode, newNode, changeCodeNodes);

        }

        private Dictionary<RevisionInfo, FunctionInfo[]> functionByRevision = new Dictionary<RevisionInfo, FunctionInfo[]>();

        public FunctionInfo[] GetFunctions(RevisionInfo revision)
        {
            FunctionInfo[] functionArray;

            revision = GetRealRevision(revision);

            if (!functionByRevision.TryGetValue(revision, out functionArray))
            {
                functionArray = RDS.CalcDataService.GetCalcFunction(revision);
                functionByRevision[revision] = functionArray;
            }
            return functionArray;
        }

        public RevisionInfo GetRealRevision(RevisionInfo revision)
        {
            if (RevisionInfo.Current.Equals(revision))
                return CurrentRevision;

            //if (RevisionInfo.Head.Equals(revision))
            //    return unitNode.GetHeadRevision();
            return revision;
        }
   
        #region IDisposable Members

        public void Dispose()
        {
            Session.UnitNodeChanged -= session_UnitNodeChanged;

            foreach (var item in unitProviderDictionary.Values)
            {
                item.Dispose();
            }
        }

        #endregion

        public AsyncOperationWatcher SendSprav(int nodeId)
        {
            ulong op = RDS.BlockDataService.SendSprav(nodeId);
            return new AsyncOperationWatcher(op, RDS);
        }

        private DateTime lastDateTime;
        public DateTime LastDateTime
        {
            get
            {
                if (lastDateTime == DateTime.MinValue)
                    lastDateTime = DateTime.Now;
                return lastDateTime;
            }
            set
            {
                lastDateTime = value;
            }
        }
    }

    /// <summary>
    /// Странный класс, который нужен для редактирования нодов в ПропертиГридах
    /// </summary>
    public class FakeStructureProvider : StructureProvider
    {
        public FakeStructureProvider(Session session)
            :base(session)
        {
        }

        public override UnitProvider GetUnitProvider(UnitNode unitNode)
        {
            return CreateUnitProvider(unitNode);
        }

        protected override ClientCore.UnitProviders.UnitProvider CreateUnitProvider(UnitNode unitNode)
        {
            //UnitProvider up = base.CreateUnitProvider(unitNode);
            UnitProvider up = new UnitProvider(this, unitNode);
            up.EditMode = true;
            return up;
        }
    }

    public enum FilterParams
    {
        All,
        ManualParameters,
        TepParameters,
        Schemas,
        Reports,
        Graphs,
        NormFunctions
    }
}

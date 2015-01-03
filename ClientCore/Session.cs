using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;
using COTES.ISTOK.DiagnosticsInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace COTES.ISTOK.ClientCore
{
    public class Session
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        RemoteDataService rds;

        public string HostPort { get; set; }
        //public int Port { get; set; }

        public UserNode User { get; internal set; }
        public Guid Uid { get; private set; }
        public UTypeNode[] Types { get; private set; }
        public RevisionInfo[] Revisions { get; private set; }

        private RevisionInfo currentRevision;
        public RevisionInfo CurrentRevision
        {
            get { return currentRevision; }
            set
            {
                currentRevision = value;
                OnCurrentRevisionChanged();
            }
        }

        public ConnectionState ConnectionState { get; private set; }

        public StructureProvider StructureProvider { get; private set; }

        internal RemoteDataService RemoteDataService { get { return rds; } }

        public event EventHandler CurrentRevisionChanged;

        protected virtual void OnCurrentRevisionChanged()
        {
            if (CurrentRevisionChanged != null)
            {
                CurrentRevisionChanged(this, EventArgs.Empty);
            }
        }

        public Session()
        {
            SetLogUser();
            //SetupGlobalQueryManager();
        }

        #region Events
        public event EventHandler<UnitNodeEventArgs> UnitNodeChanged;

        private void OnUnitNodeChanged(int unitNodeId)
        {
            if (UnitNodeChanged != null)
                UnitNodeChanged(this, new UnitNodeEventArgs(unitNodeId));
        }

        public event EventHandler<UnitTypeEventArgs> UnitTypeChanged;

        private void OnUnitTypeChanged(UnitTypeId typeID)
        {
            if (UnitTypeChanged != null)
                UnitTypeChanged(this, new UnitTypeEventArgs(typeID));
        }

        #endregion

        #region Connection
        ChannelFactory<IGlobalQueryManager> factory;
        private IGlobalQueryManager SetupGlobalQueryManager()
        {
            IGlobalQueryManager qm;

            if (factory != null) factory.Abort();
            if (!string.IsNullOrEmpty(HostPort))
            {
                EndpointAddress address = new EndpointAddress(string.Format("net.tcp://{0}/GlobalQueryManager", HostPort));
                factory = new ChannelFactory<IGlobalQueryManager>("NetTcpBinding_GlobalQueryManager", address);
            }
            else
                factory = new ChannelFactory<IGlobalQueryManager>("NetTcpBinding_GlobalQueryManager");
            factory.Open();

            try
            {
                qm = factory.CreateChannel();
                if (!qm.Test(null))
                {
                    //
                }
            }
            catch (Exception exc)
            {
                log.ErrorException("Не удалось создать соединение с сервером", exc);
                // Окно переподключения вызывается только для исключения UserNotConnectedException.
                // По этому, если у нас какие то проблемы со связью, подменим переданное исключение.
                throw new UserNotConnectedException("", exc);
                //throw;
            }
            //rds = new RemoteDataService(this);
            //StructureProvider = new StructureProvider(this);

            return qm;
        }

        public void Connect(string userName, string password)
        {
            rds = new RemoteDataService(this);
            StructureProvider = new StructureProvider(this);

            //if (!rds.TestConnection()) SetupGlobalQueryManager();
            Uid = rds.Connect(userName, password);
            if (Uid == Guid.Empty) throw new UserNotConnectedException();
            ConnectionState = ConnectionState.Connected;
            User = rds.UserDataService.GetUser();
            SetLogUser(User);
            GetUnitTypes();

            RefreshRevisions();
        }

        private void RefreshRevisions()
        {
            Revisions = rds.RevisionDataService.GetRevisions();
            //CurrentRevision = RevisionInfo.Default;
            CurrentRevision = (from r in Revisions
                               where r.Time < DateTime.Now
                               orderby r.Time descending
                               select r).First();
        }

        public void Disconnect()
        {
            //if (!rds.TestConnection()) SetupGlobalQueryManager();
            SetLogUser();
            if (rds != null)
                rds.Disconnect();
            ConnectionState = ConnectionState.Disconnected;
            qManagerCounter = 0;
        }

        private void SetLogUser(UserNode user = null)
        {
            const String nlogUserProperty = "istok-user";
            const String nooneUserName = "NOBODY";

            String userName = user == null ? nooneUserName : user.Text;

            NLog.GlobalDiagnosticsContext.Set(nlogUserProperty, userName);
        }
        #endregion

        #region Types
        public UTypeNode[] GetUnitTypes()
        {
            return Types = rds.NodeDataService.GetUnitTypes();
        }
        public void RemoveUnitType(int[] unitTypeNodesIDs)
        {
            rds.NodeDataService.RemoveUnitType(unitTypeNodesIDs);
        }
        public void AddUnitType(UTypeNode uTypeNode)
        {
            rds.NodeDataService.AddUnitType(uTypeNode);
        }
        public void UpdateUnitType(UTypeNode uTypeNode)
        {
            rds.NodeDataService.UpdateUnitType(uTypeNode);
        }
        #endregion

        #region Schedules
        public Schedule[] GetSchedules()
        {
            return rds.ScheduleDataService.GetParamsSchedules();
        }
        public Schedule GetSchedule(int id)
        {
            return rds.ScheduleDataService.GetParamsSchedule(id);
        }
        public Schedule GetSchedule(string name)
        {
            return rds.ScheduleDataService.GetParamsSchedule(name);
        }
        public void AddSchedule(Schedule schedule)
        {
            rds.ScheduleDataService.AddParamsSchedule(schedule);
        }
        public void UpdateSchedule(Schedule schedule)
        {
            rds.ScheduleDataService.UpdateParamsSchedule(schedule);
        }
        public void DeleteSchedule(int id)
        {
            rds.ScheduleDataService.DeleteSchedule(id);
        }
        #endregion

        #region StructureProvider
        public StructureProvider GetStructureProvider()
        {
            var s = new StructureProvider(this);
            //s.CurrentRevision = CurrentRevision;
            return s;
        }
        #endregion

        #region GetProperties
        public ItemProperty[] GetModuleLibraryProperties(int blockId, string libName)
        {
            return rds.BlockDataService.GetModuleLibraryProperties(blockId, libName);
        }
        public ModuleInfo[] GetModuleLibNames(int blockId)
        {
            return rds.BlockDataService.GetModuleLibNames(blockId);
        }
        #endregion

        #region GetChannelParameters
        public ParameterItem[] GetChannelParameters(int channelNodeId)
        {
            return rds.BlockDataService.GetChannelParameters(channelNodeId);
        }
        #endregion

        public event Action<Exception> ErrorOcuired;

        protected void OnError(Exception exc)
        {
            if (ErrorOcuired!=null)
            {
                ErrorOcuired(exc);
            }
        }

        public System.ComponentModel.ISite GetServiceContainer()
        {
            var sc = new ISTOKServiceContainer(rds);
            sc.ErrorOcuired += OnError;
            return sc;
        }

        #region Revisions
        //public void RemoveRevisions(int[] revisionsIds)
        //{
        //    rds.RevisionDataService.RemoveRevisions(revisionsIds);
        //}

        //public void UpdateRevisions(RevisionInfo[] revisions)
        //{
        //    rds.RevisionDataService.UpdateRevisions(revisions);
        //}

        public void UpdateRevisions(int[] removeArray, RevisionInfo[] revisionInfo)
        {
            rds.RevisionDataService.UpdateRevisions(removeArray, revisionInfo);
            RefreshRevisions();
        }
        #endregion

        #region Users and Groups
        public GroupNode[] GetGroupNodes()
        {
            return rds.UserDataService.GetGroupNodes();
        }
        public UserNode[] GetUserNodes()
        {
            return rds.UserDataService.GetUserNodes();
        }
        public UserNode[] GetUserNodes(int[] userNodeIds)
        {
            return rds.UserDataService.GetUserNodes(userNodeIds);
        }
        public UserNode AddUserNode(UserNode userNode)
        {
            return rds.UserDataService.AddUserNode(userNode);
        }
        public UserNode UpdateUserNode(UserNode userNode)
        {
            return rds.UserDataService.UpdateUserNode(userNode);
        }
        public void RemoveUserNode(UserNode userNode)
        {
            rds.UserDataService.RemoveUserNode(userNode);
        }
        public GroupNode AddGroupNode(GroupNode groupNode)
        {
            return rds.UserDataService.AddGroupNode(groupNode);
        }
        public GroupNode UpdateGroupNode(GroupNode groupNode)
        {
            return rds.UserDataService.UpdateGroupNode(groupNode);
        }
        public void RemoveGroupNode(int[] groupNodeIds)
        {
            rds.UserDataService.RemoveGroupNode(groupNodeIds);
        }
        public void NewAdmin(UserNode userNode)
        {
            rds.UserDataService.NewAdmin(userNode);
        }
        #endregion

        #region Calc
        public ConstsInfo[] GetConsts()
        {
            return rds.CalcDataService.GetConsts();
        }
        public CustomFunctionInfo[] GetCustomFunctions()
        {
            return rds.CalcDataService.GetCustomFunctions();
        }
        public void SaveConsts(ConstsInfo[] constsInfo)
        {
            rds.CalcDataService.SaveConsts(constsInfo);
        }
        public void RemoveConsts(ConstsInfo[] constsInfo)
        {
            rds.CalcDataService.RemoveConsts(constsInfo);
        }
        public void SaveCustomFunctions(CustomFunctionInfo[] customFunctionInfo)
        {
            rds.CalcDataService.SaveCustomFunctions(customFunctionInfo);
        }
        public bool GetRoundRobinAutoStart()
        {
            return rds.CalcDataService.GetRoundRobinAutoStart();
        }
        public bool IsRoundRobinStarted()
        {
            return rds.CalcDataService.IsRoundRobinStarted();
        }
        public int GetLastRoundRobinMessagesCount()
        {
            return rds.CalcDataService.GetLastRoundRobinMessagesCount();
        }
        public Message[] GetLastRoundRobinMessages(int start, int count)
        {
            return rds.CalcDataService.GetLastRoundRobinMessages(start, count);
        }
        public void SetRoundRobinAutoStart(bool p)
        {
            rds.CalcDataService.SetRoundRobinAutoStart(p);
        }
        public RoundRobinInfo GetRoundRobinInfo()
        {
            return rds.CalcDataService.GetRoundRobinInfo();
        }
        public void StartRoundRobinCalc()
        {
            rds.CalcDataService.StartRoundRobinCalc();
        }
        public void StopRoundRobinCalc()
        {
            rds.CalcDataService.StopRoundRobinCalc();
        }
        #endregion

        #region Diagnostics
        public Diagnostics GetDiagnosticsObject()
        {
            return rds.DiagnosticsDataService.GetDiagnosticsObject();
        }
        #endregion

        #region Intervals
        public IntervalDescription[] GetStandardIntervals()
        {
            return rds.IntervalDataService.GetStandardsIntervals();
        }
        public void RemoveStandardIntervals(IntervalDescription[] intervalsToRemove)
        {
            rds.IntervalDataService.RemoveStandardIntervals(intervalsToRemove);
        }
        public void SaveStandardIntervals(IntervalDescription[] modifiedIntervals)
        {
            rds.IntervalDataService.SaveStandardIntervals(modifiedIntervals);
        }
        #endregion

        #region Audit
        public ASC.Audit.AuditEntry[] GetAudit(ASC.Audit.AuditRequestContainer request)
        {
            return rds.AuditDataService.GetAudit(request);
        }
        #endregion

        #region Structure loading
        public double GetLoadProgress()
        {
            return rds.NodeDataService.GetLoadProgress();
        }
        public string GetLoadStatusString()
        {
            return rds.NodeDataService.GetLoadStatusString();
        }
        #endregion

        #region QueryManager
        IGlobalQueryManager qManager = null;
        int qManagerCounter = 0;
        object sync = new object();
        
        //Dictionary<string, int> dicAllocs = new Dictionary<string, int>();
        List<string> lstAllocs = new List<string>();
        object listsync = new object();
        IGlobalQueryManager qManagerList = null;
        internal IGlobalQueryManager AllocQManager(string opid)
        {
            if(string.IsNullOrEmpty(opid))
                throw new Exception("Cannot alloc manager with this id");

            lock (listsync)
            {
                if (lstAllocs.Contains(opid))
                    throw new Exception("Cannot alloc manager with same id");
                lstAllocs.Add(opid);
                if (qManagerList == null)
                    qManagerList = SetupGlobalQueryManager();
            }
            return qManagerList;
        }
        internal void FreeQManager(string opid)
        {
            if (string.IsNullOrEmpty(opid))
                throw new Exception("Cannot alloc manager with this id");

            lock (listsync)
            {
                if (!lstAllocs.Contains(opid))
                {
#if DEBUG
                    System.Diagnostics.Trace.TraceWarning("Manager with this id is not allocated");
                    //throw new Exception("Manager with this id is not allocated");
#endif
                    return;                    
                }
                lstAllocs.Remove(opid);
                if (lstAllocs.Count == 0)
                {
                    if (factory != null)
                    {
                        try
                        {
                            factory.Close();
                        }
                        catch (Exception)
                        {
                            factory.Abort();
                        }
                    }
                    qManagerList = null;
                }
            }
        }
        //internal IGlobalQueryManager AllocQManager()
        //{
        //    IGlobalQueryManager res;

        //    lock (sync)
        //    {
        //        var cnt = Interlocked.Increment(ref qManagerCounter);
        //        if (cnt == 1) qManager = SetupGlobalQueryManager();
        //        try
        //        {
        //            qManager.Test(null);
        //        }
        //        catch (Exception) { throw; }
        //        res = qManager;

        //        //if (qManagerCounter == 0)
        //        //    qManager = SetupGlobalQueryManager();

        //        //try
        //        //{
        //        //    qManager.Test(null);
        //        //}
        //        //catch (Exception) { throw; }
        //        //res = qManager;
        //        //qManagerCounter++;
        //    }

        //    return res;
        //}
        
        //internal void FreeQManager()
        //{
        //    lock (sync)
        //    {
        //        var cnt = Interlocked.Decrement(ref qManagerCounter);
        //        if (cnt == 0)
        //        {
        //            if (factory != null)
        //            {
        //                try
        //                {
        //                    factory.Close();
        //                }
        //                catch (Exception)
        //                {
        //                    factory.Abort();
        //                }
        //            }
        //        }
        //        else
        //            if (cnt < 0)
        //                Interlocked.Exchange(ref qManagerCounter, 0);
        //        //if (qManagerCounter == 0)
        //        //{
        //        //    //WTF?
        //        //}
        //        //if (qManagerCounter > 0) qManagerCounter--;
        //        //if (qManagerCounter == 0)
        //        //{
        //        //    if (factory != null)
        //        //    {
        //        //        try
        //        //        {
        //        //            factory.Close();
        //        //        }
        //        //        catch (Exception)
        //        //        {
        //        //            factory.Abort();
        //        //        }
        //        //    }
        //        //}
        //    }
        //}
        #endregion

        public AsyncOperationWatcher<UAsyncResult> BeginCalc(UnitNode[] unitNode, DateTime timeStart, DateTime timeEnd, bool recalcAll)
        {
            var op = rds.CalcDataService.BeginCalc(unitNode, timeStart, timeEnd, recalcAll);
            return new AsyncOperationWatcher<UAsyncResult>(op, rds);
        }

        //Queue<ServiceDataChange> serviceDataChangeQueue = new Queue<ServiceDataChange>();

        public void CommitDataChanges(IEnumerable<ServiceDataChange> changes)
        {
            if (changes != null && changes.Count() > 0)
            {
                // it shall be called in separate thread, basically
                ProccessChanges(changes);
            }
        }

        private void ProccessChanges(IEnumerable<ServiceDataChange> changes)
        {
            foreach (var item in changes)
            {
                ServiceUnitNodeChange unitNodeChange;
                ServiceUnitTypeChange unitTypeChange;

                if ((unitNodeChange = item as ServiceUnitNodeChange) != null)
                {
                    foreach (var unitNodeID in unitNodeChange.NodeIDs)
                    {
                        OnUnitNodeChanged(unitNodeID);
                    }
                }
                else if ((unitTypeChange = item as ServiceUnitTypeChange) != null)
                {
                    foreach (var typeID in unitTypeChange.TypeIDs)
                    {
                        OnUnitTypeChanged((UnitTypeId)typeID);
                    }
                }
            }
        }
    }

    public enum ConnectionState
    {
        Disconnected,
        Connected
    }
}

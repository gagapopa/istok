using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace COTES.ISTOK.ClientCore
{
    public class RemoteDataService : AsyncGlobalWorker
    {
        public NodeDataService NodeDataService { get; private set; }
        public RevisionDataService RevisionDataService { get; private set; }
        public ScheduleDataService ScheduleDataService { get; private set; }
        public ValuesDataService ValuesDataService { get; private set; }
        public ExtensionDataService ExtensionDataService { get; private set; }
        public OnlineDataService OnlineDataService { get; private set; }
        public BlockDataService BlockDataService { get; private set; }
        public ReportDataService ReportDataService { get; private set; }
        public UserDataService UserDataService { get; private set; }
        public CalcDataService CalcDataService { get; private set; }
        public DiagnosticsDataService DiagnosticsDataService { get; private set; }
        public IntervalDataService IntervalDataService { get; private set; }
        public AuditDataService AuditDataService { get; private set; }

        public RemoteDataService(Session session)
            : base(session)
        {
            NodeDataService = new NodeDataService(session);
            RevisionDataService = new RevisionDataService(session);
            ScheduleDataService = new ScheduleDataService(session);
            ValuesDataService = new ValuesDataService(session);
            ExtensionDataService = new ExtensionDataService(session);
            OnlineDataService = new OnlineDataService(session);
            BlockDataService = new BlockDataService(session);
            ReportDataService = new ReportDataService(session);
            UserDataService = new UserDataService(session);
            CalcDataService = new CalcDataService(session);
            DiagnosticsDataService = new DiagnosticsDataService(session);
            IntervalDataService = new IntervalDataService(session);
            AuditDataService = new AuditDataService(session);
        }

        //TimeSpan aliveTimeout;
        Timer aliveTimer;

        private void AliveEnable(TimeSpan aliveTimeout)
        {
            if (aliveTimer == null)
            {
                aliveTimer = new Timer(AliveMethod, null, aliveTimeout, aliveTimeout);
            }
            else
            {
                aliveTimer.Change(aliveTimeout, aliveTimeout);
            }
        }

        private void AliveDisable()
        {
            aliveTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void AliveMethod(Object state)
        {
            string opid = "AliveMethod" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).Alive(session.Uid);
                session.CommitDataChanges(res.Changes);
            }
            catch
            {

            }
            finally
            {
                FreeQManager(opid);
            }
        }

        #region Connection
        public Guid Connect(string userName, string password)
        {
            string opid = "Connect" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).Connect(userName, password);

                // switch on alive requests
                TimeSpan aliveTimeout = TimeSpan.FromMilliseconds(res.AliveTimeout.TotalMilliseconds * 0.7);
                AliveEnable(aliveTimeout);

                return res.UserGuid;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<Guid>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public void Disconnect()
        {
            string opid = "Disconnect" + Guid.NewGuid().ToString();
            try
            {
                // switch off alive requests
                AliveDisable();

                AllocQManager(opid).Disconnect(session.Uid);
            }
            catch (Exception) // (FaultException ex)
            {
                //ExceptionMethod<object>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        #endregion

        internal bool TestConnection()
        {
            string opid = "TestConnection" + Guid.NewGuid().ToString();
            try
            {
                var r = TestConnection<object>.Test(AllocQManager(opid), null);
                return r;
            }
            finally
            {
                FreeQManager(opid);
            }
        }
    }
}

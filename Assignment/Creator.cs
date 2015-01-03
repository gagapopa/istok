using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.ServiceModel;
using System.Threading;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Assignment.Audit;
using COTES.ISTOK.Assignment.Extension;
using COTES.ISTOK.Calc;
using COTES.ISTOK.DiagnosticsInfo;
using NLog;

namespace COTES.ISTOK.Assignment
{
    public class Creator
    {
        Logger log = LogManager.GetCurrentClassLogger();

        private IChannel tcpChannel = null;
        private Thread waitingDBthread = null;
        private Thread backupThread = null;
        private Thread worker = null;

        //private GlobalDiagnostics globalDiagnostics = null;

        MyDBdata dbwork = null;

        //AuditServer auditServer = new AuditServer();

        //IUnitTypeManager unitTypeManager = null;
        //IUnitManager unitManager = null;
        //ScheduleManager scheduleManager = null;
        //BlockProxy blockProxy = null;
        //IUserManager userManager = null;
        //SecurityManager securityManager = null;
        //LockManager lockManager = null;

        ///// <summary>
        ///// Объект регистрации обновлений параметров
        ///// </summary>
        //ParameterRegistrator registrator;
        //ValueReceiver valueReceiver = null;
        //AssignmentCalcSupplier calcSupplier = null;
        //CalcServer cserv = null;
        //ReportUtility reportUtility = null;
        //ExportImportManager exportImportManager = null;
        //ExtensionManager extensionManager;
        //Scheduler scheduler = null;
        //RevisionManager revisionManager;

        public Creator()
        {
            Init();
        }

        void Init()
        {
            CommonData.applicationName = "Станционный сервер. ИСТОК";

            RemotingConfiguration.CustomErrorsEnabled(false);
            RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception exc = e.ExceptionObject as Exception;

                log.FatalException("Сбой при работе общестанционного сервера", exc);
            }
            catch { }
        }

        #region Start\Stop

        public void Start()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(StartMethod));
            }
            catch (Exception ex)
            {
                log.FatalException("", ex);
            }
        }

        public void Stop()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(StopMethod));
            }
            catch (Exception ex)
            {
                log.FatalException("", ex);
            }
        }

        private void StartMethod(object state)
        {
            try
            {
                bool reg = CommonData.CheckRegister(GNSI.LicenseFile);
                if (!reg)
                    throw new Exception("Служба не зарегистрированна");
                GlobalDiagnosticsContext.Set("server-name", "global");

                StartRemotingServer();
                InitWCFServer();
                StartWCFServer();

                LoadMethod();

                backupThread = new Thread(new ThreadStart(BackupDB));
                backupThread.Start();

                log.Info("Global started.");
            }
            catch (Exception ex)
            {
                log.FatalException("", ex);
                StopMethod(state);
                OnStopped();
            }
        }

        private void StopMethod(object state)
        {
            try
            {
                if (calcServer != null) calcServer.StopRoundRobin();
                if (worker != null) worker.Abort();
                if (scheduler != null) scheduler.Stop();
                if (backupThread != null) backupThread.Abort();
                if (waitingDBthread != null) waitingDBthread.Abort();
                if (valueReceiver != null) valueReceiver.Stop();
                StopWCFServer();
                StopRemotingServer();

                log.Info("Global stoped");
            }
            catch (Exception ex)
            {
                log.FatalException("", ex);
            }
        }

        public event EventHandler Stopped;

        private void OnStopped()
        {
            if (Stopped != null)
                Stopped(this, EventArgs.Empty);
        }

        #endregion

        private void BackupDB()
        {
            List<BackUpSettings> lstBU = new List<BackUpSettings>();
            BackUpSettings buDiff;
            BackUpSettings buFull;
            DateTime dtime;
            bool needBackup;

            try
            {
                buDiff = GlobalSettings.Instance.BackUpDiff;
                buFull = GlobalSettings.Instance.BackUpFull;

                if (buDiff != null && buDiff.InUse) lstBU.Add(buDiff);
                if (buFull != null && buFull.InUse) lstBU.Add(buFull);
                while (lstBU.Count > 0)
                {
                    dtime = DateTime.Now;
                    foreach (BackUpSettings item in lstBU)
                    {
                        needBackup = false;
                        if (item.Time.Hour == dtime.Hour &&
                            item.Time.Minute == dtime.Minute)
                            needBackup = true;
                        if (needBackup)
                        {
                            if (((TimeSpan)dtime.Subtract(item.LastBackup)).TotalHours < 1)
                                needBackup = false;
                        }
                        if (needBackup)
                        {
                            switch (item.Period)
                            {
                                case BackUpPeriod.Week:
                                    if (item.DayOfWeek != dtime.DayOfWeek)
                                        needBackup = false;
                                    break;
                                case BackUpPeriod.Month:
                                    int day = item.Day;
                                    day = DateTime.DaysInMonth(dtime.Year, dtime.Month);
                                    if (item.Day < day)
                                        day = item.Day;
                                    if (dtime.Day != day)
                                        needBackup = false;
                                    break;
                            }
                        }

                        if (needBackup)
                        {
                            MyDBdata dbwork = null;

                            try
                            {
                                if (dbwork != null)
                                    dbwork = dbwork.Clone();

                                dbwork.ExecSQL(item.GetBackupQuery(), null, 0);
                                item.LastBackup = dtime;
                            }
                            catch (ThreadAbortException ex)
                            {
                                throw ex;
                            }
                            catch (Exception ex)
                            {
                                log.ErrorException("Backup error", ex);
                            }
                            finally
                            {
                                if (dbwork != null)
                                    dbwork.Dispose();
                            }
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                log.FatalException("", ex);
            }
        }

        private ServiceHost serviceHost = null;
        private ServiceHost loggerHost = null;

        private void InitWCFServer()
        {
            var uri = new Uri(String.Format("net.tcp://localhost:{0}/GlobalQueryManager", GlobalSettings.Instance.Port));
            serviceHost = new ServiceHost(GlobalQueryManager.globSvcManager, uri);

            var logUri = new Uri(String.Format("net.tcp://localhost:{0}/LogReceiverServer", GlobalSettings.Instance.Port));
            loggerHost = new ServiceHost(typeof(LogReceiverServer), logUri);
        }
        private void StartWCFServer()
        {
            serviceHost.Open();
            loggerHost.Open();
        }
        private void StopWCFServer()
        {
            serviceHost.Close();
            loggerHost.Close();
        }

        private void ValueGenerator(object state)
        {
            List<Package> lstPack = new List<Package>();
            Random rand = new Random();
            try
            {
                Package p = new Package(6829, new ParamValueItem[] { new ParamValueItem(DateTime.Now, Quality.Good, Math.Round(rand.NextDouble() * 300, 3)) });
                lstPack.Add(p);
                p = new Package(6830, new ParamValueItem[] { new ParamValueItem(DateTime.Now, Quality.Good,  Math.Round(rand.NextDouble() * 500 - 500, 3)) });
                lstPack.Add(p);
                p = new Package(6831, new ParamValueItem[] { new ParamValueItem(DateTime.Now, Quality.Good,  Math.Round(rand.NextDouble() * 30 + 15, 3)) });
                lstPack.Add(p);
                p = new Package(6834, new ParamValueItem[] { new ParamValueItem(DateTime.Now, Quality.Good, Math.Round(rand.NextDouble() * 100 + 300, 3)) });
                lstPack.Add(p);
                valueReceiver.SaveValues(new OperationState(securityManager.InternalSession), lstPack.ToArray());
            }
            catch (Exception)
            {

            }
        }

        IUserManager userManager;
        ISecurityManager securityManager;
        ILockManager lockManager;
        IUnitManager unitManager;
        IUnitTypeManager unitTypeManager;
        ScheduleManager scheduleManager;
        BlockProxy blockProxy;
        ValueReceiver valueReceiver;
        ParameterRegistrator registrator;
        ExtensionManager extensionManager;
        AssignmentCalcSupplier calcSupplier;
        CalcServerAdapter calcServer;
        ReportUtility reportUtility;
        ExportImportManager exportImportManager;
        Diagnostics globalDiagnostics;
        RevisionManager revisionManager;
        IAuditServer auditServer;
        Scheduler scheduler;
        CalcServer cserv;

        private void StartRemotingServer()
        {
            if (tcpChannel != null)
                ChannelServices.RegisterChannel(tcpChannel, false);

            // стартуем все удаленные сервисы

            // подключаемся в серверу БД
            string db_type = GlobalSettings.Instance.DataBase.Type;
            dbwork = new MyDBdata(ServerTypeFormatter.Format(db_type), 3);

            dbwork.SetParam(
                            GlobalSettings.Instance.DataBase.Host,
                            GlobalSettings.Instance.DataBase.Name,
                            GlobalSettings.Instance.DataBase.User,
                            CommonData.SecureStringToString(
                             CommonData.DecryptText(
                                 CommonData.Base64ToString(
                                     GlobalSettings.Instance.DataBase.Password)))
                            );
            {
                // create all objects
                var auditServer = new AuditServer();
                var securityManager = new SecurityManager();

                var realUnitTypeManager = new UnitTypeManager(dbwork);
                var realUnitManager = new UnitManager(dbwork);
                var scheduleManager = new ScheduleManager(dbwork);
                var blockProxy = new BlockProxy(CommonData.MaxBlockCount);
                var realUserManager = new UserManager(dbwork, securityManager);
                var lockManager = new LockManager();

                var registrator = new ParameterRegistrator();
                var valueReceiver = new ValueReceiver(dbwork);
                var calcSupplier = new AssignmentCalcSupplier(dbwork, securityManager);
                var cserv = new CalcServer(calcSupplier);
                var exportImportManager = new ExportImportManager();
                var extensionManager = new ExtensionManager();
                var scheduler = new Scheduler();
                var revisionManager = new RevisionManager(dbwork);
                var calcAdapter = new CalcServerAdapter();

                // create decorates
                var unitManagerDecorator = new UnitManagerAuditDecorator(auditServer, realUnitManager);
                var typeManagerDecorator = new UnitTypeManagerAuditDecorator(auditServer, realUnitTypeManager);
                var userManagerDecorator = new UserManagerAuditDecorator(auditServer, realUserManager);

                // Assign Objects
                this.auditServer = auditServer;
                this.userManager = userManagerDecorator;
                this.securityManager = securityManager;
                this.lockManager = lockManager;
                this.unitManager = unitManagerDecorator;
                this.unitTypeManager = typeManagerDecorator;
                this.scheduleManager = scheduleManager;
                this.blockProxy = blockProxy;
                this.valueReceiver = valueReceiver;
                this.registrator = registrator;
                this.extensionManager = extensionManager;
                this.calcSupplier = calcSupplier;
                this.calcServer = calcAdapter;
                //this.reportUtility=reportUtility;
                this.exportImportManager = exportImportManager;
                //this.globalDiagnostics=globalDiagnostics;
                this.revisionManager = revisionManager;
                this.scheduler = scheduler;
                this.cserv = cserv;

                // resolve dependences
                //auditServer
                //securityManager
                securityManager.UnitManager = this.unitManager;
                securityManager.UserManager = this.userManager;
                //realUnitTypeManager 
                realUnitTypeManager.SecurityManager = this.securityManager;
                realUnitTypeManager.ExtensionManager = this.extensionManager;
                //realUnitManager 
                realUnitManager.SecurityManager = this.securityManager;
                realUnitManager.RevisionManager = this.revisionManager;
                realUnitManager.UnitTypeManager = this.unitTypeManager;
                realUnitManager.ExtensionManager = this.extensionManager;
                realUnitManager.BlockProxy = this.blockProxy;
                realUnitManager.LockManager = this.lockManager;
                //scheduleManager 
                scheduleManager.BlockProxy = this.blockProxy;
                //blockProxy
                blockProxy.Manager = this.unitManager;
                blockProxy.ValueReceiver = this.valueReceiver;
                //realUserManager 
                //lockManager 
                lockManager.SecurityManager = this.securityManager;
                lockManager.UnitManager = this.unitManager;
                //registrator 
                //valueReceiver 
                valueReceiver.Audit = this.auditServer;
                valueReceiver.BlockProxy = this.blockProxy;
                valueReceiver.SecurityManager = this.securityManager;
                valueReceiver.UnitManager = this.unitManager;
                //calcSupplier 
                calcSupplier.UnitManager = this.unitManager;
                calcSupplier.RevisionManager = this.revisionManager;
                calcSupplier.ValueReceiver = this.valueReceiver;
                //cserv 
                //exportImportManager 
                exportImportManager.SecurityManager = this.securityManager;
                exportImportManager.RevisionManager = this.revisionManager;
                exportImportManager.UnitManager = this.unitManager;
                exportImportManager.UserManager = this.userManager;
                exportImportManager.ValueReceiver = this.valueReceiver;
                //extensionManager 
                extensionManager.UnitManager = this.unitManager;
                extensionManager.UnitTypeManager = this.unitTypeManager;
                extensionManager.ValueReceiver = this.valueReceiver;
                //scheduler 
                scheduler.BlockProxy = this.blockProxy;
                scheduler.ScheduleManager = this.scheduleManager;
                scheduler.UnitManager = this.unitManager;
                scheduler.Security = this.securityManager;
                //revisionManager 
                //calcAdapter 
                calcAdapter.Audit = this.auditServer;
                calcAdapter.CalcServer = this.cserv;
                calcAdapter.CalcSupplier = this.calcSupplier;
                calcAdapter.Security = this.securityManager;
                calcAdapter.Units = this.unitManager;
                calcAdapter.Scheduler = this.scheduler;

                //unitManagerDecorator 
                unitManagerDecorator.Security = this.securityManager;
                unitManagerDecorator.UnitTypes = this.unitTypeManager;
                //typeManagerDecorator 
                typeManagerDecorator.Security = this.securityManager;
                //userManagerDecorator 
                userManagerDecorator.Security = this.securityManager;

                // create also objects
                this.reportUtility = new ReportUtility(dbwork, unitManager, unitTypeManager, extensionManager, valueReceiver, securityManager, userManager);

                //if (globalDiagnostics == null)
                //{
                GlobalDiag gnode = new GlobalDiag();
                gnode.Text = System.Net.Dns.GetHostName() + " (globalsvc)";
                gnode.DbHost = GlobalSettings.Instance.DataBase.Host;
                gnode.DbName = GlobalSettings.Instance.DataBase.Name;
                gnode.DbUser = GlobalSettings.Instance.DataBase.User;
                gnode.Port = GlobalSettings.Instance.Port;

                //globalDiagnostics = new GlobalDiagnostics(gnode,
                //    registrator,
                //    blockProxy,
                //    securityManager);
                globalDiagnostics = new Diagnostics();
                //globalDiagnostics.AddInfoGetter(securityManager);
                               
                //RemotingServices.Marshal(globalDiagnostics, "GlobalDiagnostics.rem");
                //}
                //if (blockProxy == null)
                //    blockProxy = new BlockProxy(CommonData.MaxBlockCount);

                //if (revisionManager == null)
                //    revisionManager = new RevisionManager(dbwork);

                //if (unitTypeManager==null)
                //{
                //    unitTypeManager = new UnitTypeManager(dbwork);
                //}

                //UnitManager realUnitManager = null;
                //if (unitManager == null)
                //{
                //    realUnitManager = new UnitManager(dbwork, revisionManager, blockProxy, unitTypeManager);
                //    unitManager = new UnitManagerAuditDecorator(realUnitManager);
                //}
                //blockProxy.Manager = unitManager;

                //if (securityManager == null)
                //{
                //    securityManager = new SecurityManager();
                //    securityManager.UnitManager = unitManager;
                //}
                //(unitManager as UnitManagerAuditDecorator).Audit = auditServer;
                //(unitManager as UnitManagerAuditDecorator).Security = securityManager;
                //(unitManager as UnitManagerAuditDecorator).UnitTypes = unitTypeManager;

                //scheduleManager = new ScheduleManager(dbwork, blockProxy);

                //if (userManager == null)
                //{
                //    userManager = new UserManager(dbwork, securityManager);
                //    securityManager.UserManager = userManager;
                //}

                //if (lockManager == null)
                //{
                //    lockManager = new LockManager(unitManager, securityManager);
                //}
                //realUnitManager.SetSecurity(securityManager, lockManager);
                //unitTypeManager.SecurityManager = securityManager;

                //if (registrator == null)
                //    registrator = new ParameterRegistrator();

                //if (valueReceiver == null)
                //{
                //    valueReceiver = new ValueReceiver(dbwork, auditServer, securityManager, unitManager,
                //        //registrator, 
                //        blockProxy);
                if (this.registrator != null)
                {
                    this.registrator.GetLastValue += new GetLastValueDelegate(id =>
                    {
                        ParamValueItemWithID param = new ParamValueItemWithID(valueReceiver.GetLastValueParameter(new OperationState(), id));
                        param.ParameterID = id;
                        return param;
                    });
                    valueReceiver.ValuesChanged += new EventHandler<ValueReceiver.ValueChangedEventArgs>((s, e) =>
                    {
                        foreach (UnitNode node in e.Nodes)
                        {
                            //Обновление значения для мнемосхем {
                            ParamValueItemWithID param = new ParamValueItemWithID(e.GetLastValue(node));
                            param.ParameterID = node.Idnum;
                            //param.Time = receiveItem.Time;
                            //param.ChangeTime = dtnow;
                            //param.Quality = receiveItem.Quality;
                            //param.Value = receiveItem.Value;
                            registrator.UpdateValue(param);
                            // } 
                        }
                    });
                }
                //}
                //blockProxy.ValueReceiver = valueReceiver;

                //if (extensionManager == null)
                //    extensionManager = new ExtensionManager(unitManager, valueReceiver);
                //realUnitManager.ExtensionManager = extensionManager;
                //valueReceiver.ValuesChanged += new EventHandler<ValueReceiver.ValueChangedEventArgs>(
                //    (s, e) => extensionManager.ValuesChanged(e.Nodes)
                //    );

                //unitTypeManager.ExtensionManager = extensionManager;

                //if (calcSupplier == null)
                //{
                //    calcSupplier = new AssignmentCalcSupplier(unitManager, revisionManager, valueReceiver);
                //    calcSupplier.DBWork = dbwork;
                //}

                //if (cserv == null)
                //    cserv = new CalcServer(calcSupplier);

                //if (reportUtility == null)
                //    reportUtility = new ReportUtility(dbwork, unitManager, extensionManager, valueReceiver, securityManager, userManager);

                //if (exportImportManager == null)
                //    exportImportManager = new ExportImportManager(unitManager,userManager, securityManager, valueReceiver, revisionManager, cserv);

                //if (scheduler == null)
                //{
                //    scheduler = new Scheduler(unitManager, scheduleManager, blockProxy);                
                //}

               

                //var calcAdapter = new CalcServerAdapter()
                //{
                //    CalcServer = cserv,
                //    CalcSupplier = calcSupplier,
                //    Audit = auditServer,
                //    Units = realUnitManager,
                //    Security = securityManager
                //};
            }

            if (GlobalQueryManager.globSvcManager == null)
            {
                GlobalQueryManager.globSvcManager = new GlobalQueryManager(
                        userManager,
                        securityManager,
                        lockManager,
                        unitManager,
                        unitTypeManager,
                        scheduleManager,
                        blockProxy,
                        valueReceiver,
                        registrator,
                        extensionManager,
                        calcSupplier,
                        calcServer,
                        reportUtility,
                        exportImportManager,
                        globalDiagnostics,
                        revisionManager,
                        auditServer);
            }
#if EMA
                extensionManager.Start();
#endif
        }

        private void LoadMethod()
        {
            var state = new OperationState(securityManager.InternalSession);

            GlobalQueryManager.globSvcManager.LoadState = state;

            TimeSpan atempDelay = TimeSpan.FromSeconds(18);
            DateTime now = DateTime.Now;

            log.Info("Начата загрузка списков");

            int attemptCount = 10;
            while (attemptCount > 0)
            {
                try
                {
                    state.Progress = 0.0;

                    unitTypeManager.LoadTypes(state);

                    scheduleManager.LoadSchedules(state);
                    state.Progress += 10;

                    unitManager.LoadUnits(state);

                    attemptCount = 0;
                }
                /**
                 * TODO данный цикл должен быть внутри DAL'a и работать при любой потери связи с СКЛ
                 */
                catch (Exception exc)
                {
                    if (--attemptCount > 0)
                    {
                        log.FatalException("", exc);
                        log.Info("Попытка переподключения");
                        Thread.Sleep(atempDelay);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            log.Info("Загрузка списов завершена, {0}", (now - DateTime.Now).ToString());
            valueReceiver.Start();
            this.scheduler.Start();
            if (GlobalSettings.Instance.AllowRoundRobinAutostart)
            {
                this.calcServer.StartRoundRobin();
            }
            //this.cserv.Start();

            state.Progress = AsyncOperation.MaxProgressValue;
        }

        private void StopRemotingServer()
        {
            // останавливаем remoting сервер
            System.Runtime.Remoting.Channels.IChannel[] chnls = System.Runtime.Remoting.Channels.ChannelServices.RegisteredChannels;
            if (tcpChannel != null) ChannelServices.UnregisterChannel(tcpChannel);

            if (GlobalQueryManager.globSvcManager != null) GlobalQueryManager.globSvcManager.Clear();
            if (dbwork != null)
            {
                dbwork.Dispose();
                dbwork = null;
            }
        }
    }
}

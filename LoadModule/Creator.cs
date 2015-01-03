using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.ServiceModel;
using COTES.ISTOK.Block.Redundancy;
using NLog;

namespace COTES.ISTOK.Block
{
    public class Creator
    {
        private DALManager dalManager = null;
        private IChannel tcpChannel = null;
        private ValueBuffer valBuffer = null;
        private ValueReceiver valReceiver = null;
        private ChannelManager chManager = null;
        private ParameterRegistrator parRegistrator = null;
        private QueryManager queryManager = null;
        private RedundancyServer redundancyServer = null;
        private BlockDiagnostics blockDiagnostics = null;
        private DataBaseMaintance maintance = null;
        private string uid = "";
        private System.Threading.Timer restartTimer;

        Logger log = LogManager.GetCurrentClassLogger();
        private ServiceHost serviceHost;
        private ServiceHost diagnosticServiceHost;

        public Creator()
        {
            RemotingConfiguration.CustomErrorsEnabled(false);
            RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
        }

        public void Start()
        {
            uid = BlockSettings.Instance.ServerName;

            SetupNSI();
            dalManager = SetupDAL();
            NSI.dalManager = dalManager;
            GlobalDiagnosticsContext.Set("server-name", BlockSettings.Instance.ServerName);

            valBuffer = new ValueBuffer();
            valReceiver = new ValueReceiver(dalManager, valBuffer);
            chManager = new ChannelManager(dalManager, valBuffer);
            redundancyServer = new RedundancyServer(uid);

            parRegistrator = new ParameterRegistrator();
            valBuffer.UpdateValue += (p, v) => parRegistrator.UpdateValue(new ParamValueItemWithID(v, p.Idnum));
            parRegistrator.GetLastValue += id => new ParamValueItemWithID(valReceiver.GetLastValue(id), id);

            maintance = new DataBaseMaintance(chManager, dalManager, valBuffer);

            NSI.redundancyServer = redundancyServer;
            NSI.valReceiver = valReceiver;                                      
            NSI.chanManager = chManager;
            NSI.parRegistrator = parRegistrator;


            blockDiagnostics = new BlockDiagnostics(chManager, valBuffer, CommonData.applicationName, redundancyServer);

            queryManager = new QueryManager(blockDiagnostics);
            queryManager.Host = Dns.GetHostName();
            queryManager.Port = (int)BlockSettings.Instance.Port;

            InitWCF();

            NSI.conInspector =
              new ConnectionInspector(BlockSettings.Instance.GlobalServerHost,
                                      BlockSettings.Instance.GlobalServerPort.ToString());

            NSI.conInspector.UID = uid;
            NSI.conInspector.RouterIp = BlockSettings.Instance.RouterIp;
            NSI.conInspector.RouterPort = (int)BlockSettings.Instance.Port;
            NSI.conInspector.BlockServer = queryManager;

            redundancyServer.ServerIsMaster += (object sender, EventArgs e) =>
            {
                NSI.conInspector.BlockServerIsMaster();
                maintance.StartMaintenance();
                StartManager();
            };
            redundancyServer.ServerIsSlave += (object sender, EventArgs e) =>
            {
                NSI.conInspector.BlockServerIsSlave();
                maintance.StopMaintenance();
                StopManager();
            };
            TimeSpan restartSpan = TimeSpan.FromHours(20);
            restartTimer = new System.Threading.Timer(RestartManager, null, restartSpan, restartSpan);
        }

        void StartManager()
        {
            try
            {
                chManager.StartManager();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }
        }
        void StopManager()
        {
            try
            {
                if (chManager != null)
                    chManager.StopManager();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }
        }

        void RestartManager(object state)
        {
            StopManager();
            if (redundancyServer.GetInfo().IsMaster)
                redundancyServer.OnServerIs();
        }

        private void InitWCF()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            serviceHost = new ServiceHost(queryManager, new Uri(String.Format("net.tcp://localhost:{0}/BlockQueryManager", BlockSettings.Instance.Port)));
            serviceHost.Open();

            if (diagnosticServiceHost != null)
            {
                diagnosticServiceHost.Close();
            }
            diagnosticServiceHost = new ServiceHost(blockDiagnostics, new Uri(String.Format("net.tcp://localhost:{0}/BlockDiagnostics", BlockSettings.Instance.Port)));
            diagnosticServiceHost.Open();
        }

        private void StopWCF()
        {
            serviceHost.Close();
            serviceHost = null;
            diagnosticServiceHost.Close();
            diagnosticServiceHost = null;
        }

        public void Stop()
        {
            if (redundancyServer != null) redundancyServer.Dispose();
            StopWCF();

            if (chManager != null) chManager.StopManager();

            NSI.valReceiver = null;
            NSI.chanManager = null;
            NSI.parRegistrator = null;

            queryManager.Dispose();

            parRegistrator = null;
            chManager = null;
            valReceiver = null;
            valBuffer = null;
            dalManager = null;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }


        private void SetupNSI()
        {
            // путь к загрузочным модулям
            const String path = "Modules";
            NSI.LoadersPath = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), path);
        }

        private DALManager SetupDAL()
        {
            DALManager dalManager;
            var props = new Dictionary<string, string>();
            string db_type = BlockSettings.Instance.DataBase.Type;
            ServerType serverType = ServerTypeFormatter.Format(db_type);

            dalManager = CreateDAL(serverType);

            props.Add("DB_host", BlockSettings.Instance.DataBase.Host);
            props.Add("DB_name", BlockSettings.Instance.DataBase.Name);
            props.Add("DB_user", BlockSettings.Instance.DataBase.User);
            props.Add("DB_pass",
            CommonData.SecureStringToString(
               CommonData.DecryptText(
                   CommonData.Base64ToString(
                       BlockSettings.Instance.DataBase.Password))));
            try { props.Add("ReplicationUser", BlockSettings.Instance.ReplicationUser); }
            catch { }
            try { props.Add("ReplicationPassword", BlockSettings.Instance.ReplicationPassword); }
            catch { }
            dalManager.ConnectionProperties = props;
            //try { dalManager.Connect(); }
            //catch (Exception exc)
            //{
            //    MessageLog.Message(MessageLevel.Error, "Ошибка подключения к БД: {0}", exc.Message);
            //}
            dalManager.CheckConnection();

            return dalManager;
        }

        //private void LoadChannels()
        //{
        //    try
        //    {
        //        chManager.LoadChannels();
        //    }
        //    catch (Exception exc)
        //    {
        //        MessageLog.Message(MessageLevel.Error, "Ошибка запуска каналов: {0}", exc.Message);
        //    }
        //    try
        //    {
        //        chManager.StartManager();
        //    }
        //    catch (Exception exc)
        //    {
        //        MessageLog.Message(MessageLevel.Error, "Ошибка запуска каналов: {0}", exc.Message);
        //    }
        //}

        //private static DALManager CreateDAL(ServerType serverType)
        //{
        //    return CreateDAL(serverType);
        //}

        private static DALManager CreateDAL(ServerType serverType)
        {
            DALManager dalManager;
            switch (serverType)
            {
                case ServerType.MSSQL:
                    dalManager = new MSSqlDAL();
                    break;
                default:
                    throw new NotSupportedException(ServerTypeFormatter.Format(serverType));
            }
            return dalManager;
        }

        public static DataTable GetDataSource(ServerType serverType)
        {
            DataTable servTable = null;
            DALManager dalManager = CreateDAL(serverType);

            if (dalManager != null)
            {
                servTable = dalManager.GetDataSources();
            }
            return servTable;
        }

        public static String[] GetBases(ServerType serverType, String host, String login, String password)
        {
            String[] bases = null;
            DALManager dalManager = CreateDAL(serverType);

            if (dalManager != null)
            {
                dalManager.ConnectionProperties.Add("DB_host", host);
                dalManager.ConnectionProperties.Add("DB_user", login);
                dalManager.ConnectionProperties.Add("DB_pass", password);

                bases = dalManager.GetBases();
            }
            return bases;
        }

        public static bool TestConnection(ServerType serverType, string host, string baseName, string login, string password)
        {
            DALManager dalManager = CreateDAL(serverType);

            if (dalManager != null)
            {
                dalManager.ConnectionProperties.Add("DB_host", host);
                dalManager.ConnectionProperties.Add("DB_name", baseName);
                dalManager.ConnectionProperties.Add("DB_user", login);
                dalManager.ConnectionProperties.Add("DB_pass", password);

                dalManager.Connect();
                dalManager.Disconnect();
                return true;
            }
            return false;
        }
    }
}

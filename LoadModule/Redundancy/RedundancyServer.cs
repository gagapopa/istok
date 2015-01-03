using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Net.Sockets;
using COTES.ISTOK.Block.MirroringManager;
using System.Data.SqlClient;
using System.Security;
using System.Data;
using NLog;
//using SimpleLogger;

namespace COTES.ISTOK.Block.Redundancy
{
    /// <summary>
    /// Сервер дублирования
    /// </summary>
    public class RedundancyServer : IRedundancy, COTES.ISTOK.DiagnosticsInfo.ISummaryInfo, ITestConnection<Object>, IDisposable
    {
        const int childServerCount = 1;

        PublicationInfo replicationInfo = null;
        SubscriptionInfo subscriptionInfo = null;
        
        volatile List<CloneServer> lstServers;
        volatile ServerInfo serverInfo;
        volatile bool needSwitchToSlave = false;
        //volatile ILogger log = null;
        Logger log = LogManager.GetCurrentClassLogger();

        volatile CloneServer parentServer;
        volatile CloneServer[] childServers = new CloneServer[childServerCount];

        //volatile bool isOnline = false;
        volatile bool isSuspended = false;

        uint lastId;

        public string UID { get; private set; }
        
        public event EventHandler ServerIsMaster = null;
        public event EventHandler ServerIsSlave = null; 

        public RedundancyServer(string uid)
        {
            lstServers = new List<CloneServer>();
            UID = uid;
            //this.log = log;
            lastId = 0;
            Init();
        }

        //private void WriteLog(MessageLevel category, string message, params object[] pars)
        //{
        //    Console.WriteLine(category.ToString() + "-" + message);
        //    if (log == null) return;

        //    log.Message(category, message, pars);
        //}

        #region Диагностика
        const string caption = "Дублирование";

        public DataSet GetSummaryInfo()
        {
            DataSet ds = new DataSet();
            DataTable table;
            //DataRow row;

            table = new DataTable(caption);
            table.Columns.Add("Url");
            table.Columns.Add("Состояние");
            table.Columns.Add("UID");
            table.Columns.Add("Главный", typeof(bool));
            table.Columns.Add("Приоритет");
            table.Columns.Add("Версия");

            foreach (var item in GetServers())
            {
                table.Rows.Add(item.URL,
                    item.State.ToString(),
                    item.UID,
                    item.IsMaster,
                    item.Priority,
                    item.Version);
            }
            ds.Tables.Add(table);
            return ds;
        }
        public string GetSummaryCaption()
        {
            return caption;
        }
        #endregion

        private void Init()
        {
            try
            {
                byte priority = BlockSettings.Instance.ServerPriority;

                string url = string.Format("{0}://{1}:{2}/{3}",
                    "tcp",
                    System.Net.Dns.GetHostName(),
                    BlockSettings.Instance.Port,
                    CommonData.QueryManagerURI);
                List<string> lstIps = new List<string>();
                System.Net.IPHostEntry IPHost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var item in IPHost.AddressList)
                    if (item.AddressFamily == AddressFamily.InterNetwork)
                        lstIps.Add(item.ToString());
                serverInfo = new ServerInfo(UID, lstIps.ToArray(), BlockSettings.Instance.Port, priority);
                serverInfo.URL = url;
                serverInfo.ConnectionString =
                    CommonData.StringToBase64(
                        CommonData.EncryptText(NSI.dalManager.GetConnectionString()));
                LoadServers();
                PrintNeighbours();

                serverInfo.State = ServerState.Waiting;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Делает БД сервера активной (зеркало, репликация)
        /// </summary>
        private void SetDBMaster()
        {
            //SwitchMirroring();
            serverInfo.DBVersion++;
#if REPLICATION
            SetDBNothing();
            CreatePublication();
#endif
        }
        /// <summary>
        /// Делает БД сервера обновляемой с другого сервера (зеркало, репликация)
        /// </summary>
        private void SetDBSlave()
        {
#if REPLICATION
            //InitMirroring();
            SetDBNothing();
            CreateSubscription();
#endif
        }
        /// <summary>
        /// Очищает БД сервера от всяких синхронизационных штучек (зеркало, репликация)
        /// </summary>
        private void SetDBNothing()
        {
#if REPLICATION
            //ClearMirroring();
            DeleteSubscription();
            DeletePublication();
#endif
        }

        private void InitMirroring()
        {
            try
            {
                //SecureString ss = CommonData.DecryptText(parentServer.RedundancyServer.GetConnectionString());

                serverInfo.MirrorConnectionString = parentServer.ServerInfo.ConnectionString;
                NSI.dalManager.CreateMirroring(serverInfo.MirrorConnectionString);
            }
            catch (ThreadAbortException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                log.ErrorException("InitMirroring error:" + ex.Message, ex);
            }
        }
        private void SwitchMirroring()
        {
            try
            {
                //if (parentServer != null && parentServer.ServerInfo != null)
                //    NSI.dalManager.SwitchMirroring(parentServer.ServerInfo.ConnectionString);
                //else
                //    NSI.dalManager.SwitchMirroring(null);
                NSI.dalManager.SwitchMirroring(serverInfo.MirrorConnectionString);
            }
            catch (ThreadAbortException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                log.ErrorException("SwitchMirroring error:" + ex.Message, ex);
            }
        }

        public void Stop()
        {
            try
            {
                foreach (var item in lstServers)
                {
                    if (item.Thread != null) item.Thread.Abort();
                }
                if (ServerIsSlave != null) ServerIsSlave(this, new EventArgs());
                SaveServers();
                BaseSettings.Instance["Settings/ServerPriority"] = serverInfo.Priority;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }
        }

        //временно паблик. да и вообще метод временный
        public void PrintNeighbours()
        {
            Console.WriteLine("Parent: " + (parentServer == null ? "null" : parentServer.ServerInfo.URL));
            for (int i = 0; i < childServerCount; i++)
                Console.WriteLine("Child[" + i.ToString() + "]: " + (childServers[i] == null ? "null" : childServers[i].ServerInfo.URL));
        }

        private void AddServer(ServerInfo server)
        {
            try
            {
                CloneServer cloneServer;
                Thread thread;

                var p = from elem in lstServers
                        where elem.ServerInfo.URL == server.URL
                        select elem;
                if (p.Count() > 0)
                {
                    cloneServer = p.First();
                    if (cloneServer.Thread == null || !cloneServer.IsAlive)
                        lstServers.Remove(cloneServer);
                    else
                        return;
                }

                if (server.URL != serverInfo.URL)
                {
                    thread = new Thread(new ParameterizedThreadStart(ThreadMethod));
                    thread.Name = "RedundancyThread (" + server.URL + ")";
                    server.State = ServerState.Online;
                }
                else
                {
                    thread = new Thread(new ParameterizedThreadStart(MainThreadMethod));
                    thread.Name = "RedundancyThread Main";
                }
                cloneServer = new CloneServer(server, thread);
                thread.Start(cloneServer);
                lstServers.Add(cloneServer);
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }
        }
        private void LoadServers()
        {
            try
            {
                ServerInfo[] arrServers = null;

                //AddServer(serverInfo);
                FileInfo finfo = new FileInfo(BlockSettings.Instance.DefaultConfigFile);//OldClientSettings.FileName);
                string fname = finfo.FullName.Substring(0, finfo.FullName.Length - finfo.Extension.Length);
                fname += "-rd.xml";
                try
                {
                    if (File.Exists(fname))
                    {
                        using (FileStream stream = new FileStream(fname, FileMode.Open))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ServerInfo[]));
                            arrServers = serializer.Deserialize(stream) as ServerInfo[];
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.ErrorException(ex.Message, ex);
                }

                if (arrServers != null)
                {
                    foreach (var item in arrServers)
                        if (item.URL == serverInfo.URL)
                        {
                            serverInfo.MirrorConnectionString = item.MirrorConnectionString;
                        }
                        else
                            AddServer(item);
                }
                AddServer(serverInfo);
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }
        }
        private void SaveServers()
        {
            try
            {
                FileInfo finfo = new FileInfo(BlockSettings.Instance.DefaultConfigFile);
                string fname = finfo.FullName.Substring(0, finfo.FullName.Length - finfo.Extension.Length);
                fname += "-rd.xml";
                using (FileStream stream = new FileStream(fname, FileMode.Create))
                {
                    List<ServerInfo> lstServerInfo = new List<ServerInfo>();
                    XmlSerializer serializer = new XmlSerializer(typeof(ServerInfo[]));

                    foreach (var item in lstServers)
                        lstServerInfo.Add(item.ServerInfo);

                    serializer.Serialize(stream, lstServerInfo.ToArray());
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }
        }
        private void UpdateServerList(ServerInfo[] arrServers)
        {
            try
            {
                bool found;

                // DOTO: fixme
                foreach (var item in arrServers)
                {
                    found = false;
                    if (!found) AddServer(item);
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }
        }

        private CloneServer FindParent()
        {
            try
            {
                IOrderedEnumerable<CloneServer> par;
                par = from elem in lstServers
                      where (elem.ServerInfo.Priority > serverInfo.Priority ||
                            (elem.ServerInfo.Priority == serverInfo.Priority &&
                            (elem.ServerInfo.URL.CompareTo(serverInfo.URL) > 0 ||
                            elem.ServerInfo.State == ServerState.Online))) &&
                            elem.ServerInfo.State != ServerState.Offline &&
                            !elem.ServerInfo.Equals(serverInfo)
                      orderby elem.ServerInfo.Priority
                      orderby elem.ServerInfo.URL descending
                      select elem;

                if (par.Count() > 0) return par.ElementAt(0);

            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }

            return null;
        }
        private CloneServer[] FindChildren()
        {
            CloneServer[] childServers = new CloneServer[childServerCount];

            for (int i = 0; i < childServerCount; i++) childServers[i] = null;

            try
            {
                CloneServer[] arr;
                arr = (from elem in lstServers
                       where (elem.ServerInfo.Priority < serverInfo.Priority ||
                             (elem.ServerInfo.Priority == serverInfo.Priority &&
                             elem.ServerInfo.URL.CompareTo(serverInfo.URL) < 0)) &&
                             elem.ServerInfo.State == ServerState.Online &&
                             !elem.ServerInfo.Equals(serverInfo)
                       orderby elem.ServerInfo.Priority descending
                       orderby elem.ServerInfo.URL
                       select elem).ToArray();


                for (int i = 0; i < childServerCount; i++)
                {
                    if (i < arr.Length)
                        childServers[i] = arr[i];
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }

            return childServers;
        }
        private CloneServer FindMaster()
        {
            try
            {
                var server = from elem in lstServers
                              where elem.ServerInfo.State == ServerState.Online
                              orderby elem.ServerInfo.Priority descending
                              select elem;
                if (server.Count() > 0) return server.ElementAt(0);
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }

            return null;
        }

        #region Работа с командами
        /// <summary>
        /// Добавление команды для передачи "клонам"
        /// </summary>
        /// <param name="type">Тип команды</param>
        /// <param name="data">Данные команды</param>
        public void AddCommand(RedundancyCommandType type, object data)
        {
            try
            {
                RedundancyCommand command =
                    new RedundancyCommand(++lastId, type, data, DateTime.Now);

                //foreach
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Выполнение команды
        /// </summary>
        /// <param name="command">Команда</param>
        public void ExecCommand(RedundancyCommand command)
        {
            //
        }
        #endregion

        #region Работа потоков
        /// <summary>
        /// Метод потока связи с другими участниками дублирования
        /// </summary>
        /// <param name="param">CloneServer object</param>
        private void ThreadMethod(object param)
        {
            CloneServer cserver = null;
            string name = "?";
            
            try
            {
                IRedundancy qmanager = null;
                ServerInfo sInfo;
                bool isOffline = true;

                cserver = (CloneServer)param;
                cserver.IsAlive = true;
                Console.Write("Thread started");
                sInfo = cserver.ServerInfo;
                name = sInfo.URL;
                Console.WriteLine(" (" + name + ")");
                
                while (true)
                {
                    try
                    {
                        if (qmanager == null)
                        {
                            isOffline = true;
                            sInfo.State = ServerState.Offline;
                            qmanager = (IRedundancy)Activator.GetObject(typeof(IRedundancy), sInfo.URL);
                            string url;
                            foreach (var item in sInfo.NetworkInterfaces)
                            {
                                try
                                {
                                    url = string.Format("{0}://{1}:{2}/{3}",
                                        "tcp",
                                        item,
                                        sInfo.Port.ToString(),
                                        CommonData.QueryManagerURI);
                                    qmanager = (IRedundancy)Activator.GetObject(typeof(IRedundancy), url);
                                    if (!TestConnection<Object>.Test(qmanager, null)) qmanager = null;
                                    else break;
                        }
                                catch (SocketException) { qmanager = null; }
                            }
                        }
                        if (qmanager != null)
                        {
                            sInfo = qmanager.GetInfo();
                            cserver.ServerInfo = sInfo;
                            if (serverInfo.IsMaster && sInfo.IsMaster)
                            {
                                if (sInfo.URL.CompareTo(serverInfo.URL) > 0)
                                    needSwitchToSlave = true;
                            }
                            if (isOffline)
                            {
                                isOffline = false;
                            }
                        }
                    }
                    catch (SocketException)
                    {
                        if (parentServer != null && parentServer.Equals(cserver))
                            serverInfo.State = ServerState.Waiting;
                        qmanager = null;
                    }
                    catch (RemotingException)
                    {
                        if (parentServer != null && parentServer.Equals(cserver))
                            serverInfo.State = ServerState.Waiting;
                        qmanager = null;
                    }

                    Thread.Sleep(500);
                }
            }
            catch (ThreadAbortException)
            {
                //
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }
            if (cserver != null) cserver.IsAlive = false;
            Console.WriteLine("Thread aborted (" + name + ")");
        }
        /// <summary>
        /// Основной метод потока
        /// </summary>
        private void MainThreadMethod(object param)
        {
            CloneServer cserver = null;

            try
            {
                CloneServer prevParent = null;
                CloneServer syncTarget = null;

                cserver = (CloneServer)param;
                cserver.IsAlive = true;
                Console.WriteLine("Main thread started");
                Console.WriteLine("Waiting connections...");
                Thread.Sleep(5000);
                Console.WriteLine("Main thread at work");

                while (true)
                {
                    try
                    {
                        if (!serverInfo.IsMaster && needSwitchToSlave)
                            needSwitchToSlave = false;
                        else
                            if (needSwitchToSlave)
                            {
                                serverInfo.State = ServerState.Waiting;
                                needSwitchToSlave = false;
                            }

                        if ((serverInfo.State != ServerState.Online && !serverInfo.IsMaster) ||
                            prevParent != null ||
                            parentServer != null)
                        {
                            parentServer = FindParent();
                            childServers = FindChildren();
                        }

                        if (parentServer != null && prevParent != null &&
                            prevParent.ServerInfo.State == ServerState.Online)
                        {
                            parentServer = prevParent;
                        }

                        if ((parentServer != null && parentServer != prevParent) ||
                            (parentServer == null && prevParent != null &&
                            prevParent.ServerInfo.State == ServerState.Offline))
                        {
                            serverInfo.State = ServerState.Synchronizing;
                            syncTarget = parentServer;
                            prevParent = parentServer;
                        }

                        switch (serverInfo.State)
                        {
                            case ServerState.Offline:
                            case ServerState.Waiting:
                                syncTarget = null;
                                if (parentServer != null)
                                {
                                    syncTarget = parentServer;
                                }
                                else
                                {
                                    foreach (var item in childServers)
                                    {
                                        if (item != null)
                                        {
                                            syncTarget = item;
                                            if (item.ServerInfo.Priority == serverInfo.Priority)
                                            {
                                                if (item.ServerInfo.URL.CompareTo(item.ServerInfo.URL) > 0)
                                                {
                                                    parentServer = item;
                                                    break;
                                                }
                                            }
                                            else
                                                break;
                                        }
                                    }
                                }

                                if (!isSuspended)
                                {
                                serverInfo.State = ServerState.Synchronizing;
                                goto case ServerState.Synchronizing;
                                }
                                break;
                            case ServerState.Synchronizing:
                                bool ok = false;

                                if (syncTarget != null)
                                {
                                    if (syncTarget.ServerInfo.State == ServerState.Online)
                                    {
                                        ServerInfo[] arrServers;

                                        Console.WriteLine("%Synchronization with '" + syncTarget.ServerInfo.URL);
                                        arrServers = syncTarget.RedundancyServer.AttachServer(serverInfo, TransferDirection.All);
                                        UID = syncTarget.RedundancyServer.UID;

                                        foreach (var item in arrServers) AddServer(item);
                                            ok = true;
                                    }
                                    //else
                                    //    Console.WriteLine("Waiting sync server");
                                }
                                else
                                {
                                    //serverInfo.State = ServerState.Online;
                                    //Console.WriteLine("*IS MAIN*");
                                    ok = true;
                                }
                                if (ok)
                                {
                                    //CreatePublication();

                                    if (parentServer == null)
                                    {
                                        //if (subscriptionInfo != null)
                                        //{
                                        //    NSI.dalManager.DeleteReplicationSubscription(subscriptionInfo);
                                        //    subscriptionInfo = null;
                                        //}
                                        SetDBMaster();
                                        serverInfo.IsMaster = true;
                                        if (ServerIsMaster != null) ServerIsMaster(this, new EventArgs());
                                        prevParent = null;
                                        Console.WriteLine("*IS MAIN*");
                                    }
                                    else
                                    {
                                        //parentServer.RedundancyServer;
                                        serverInfo.IsMaster = false;
                                        serverInfo.DBVersion = parentServer.ServerInfo.DBVersion;
                                        SetDBSlave();
                                        if (ServerIsSlave != null) ServerIsSlave(this, new EventArgs());
                                        Console.WriteLine("*online*");
                                    }
                                    serverInfo.State = ServerState.Online;
                                    //isOnline = true;
                                }
                                
                                if (serverInfo.State == ServerState.Synchronizing)
                                    //скорее всего, что-то не так, возвращаемся в исходное состояние
                                    serverInfo.State = ServerState.Waiting;
                                break;
                            case ServerState.Online:
                                //Console.WriteLine("*ONLINE*");
                                if (parentServer != null &&
                                    parentServer.ServerInfo.DBVersion != serverInfo.DBVersion)
                                {
                                    serverInfo.State = ServerState.Synchronizing;
                                    Console.WriteLine("-resynchronizing-");
                                }
                                break;
                            //case ServerState.Offline:
                            //    serverInfo.State = ServerState.Waiting;
                            //    break;
                        }
                    }
                    catch(SocketException ex)
                    {
                        log.ErrorException("SocketException:" + ex.Message, ex);
                    }
                    catch (RemotingException ex)
                    { 
                        log.ErrorException("RemotingException:" + ex.Message, ex);
                    }
                    Thread.Sleep(5000);
                }
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("Main thread aborted");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Main thread exception: " + ex.Message);
            }
            if (cserver != null) cserver.IsAlive = false;
            Console.WriteLine("Main thread stopped");
        }
        #endregion

        #region Синхронизация
        private void CreatePublication()
        {
            Console.WriteLine("Creating publication");
            try
            {
                NSI.dalManager.ResetIdentities();
#if DEBUG
                Console.WriteLine("publ name: " + serverInfo.UID);
#endif
                replicationInfo = NSI.dalManager.CreateReplicationPublication(serverInfo.UID);
            }
            catch (ThreadAbortException) { throw; }
            catch (SqlException exc)
            {
                LogSqlException(exc);
            }
            catch (Exception ex)
            {
                //WriteLog(MessageLevel.Error, ex.Message);
                log.ErrorException(ex.Message, ex);
            }
            if (replicationInfo == null)
                Console.WriteLine("!Publication with errors!");
            else
                Console.WriteLine("Publication created");
        }

        private void LogSqlException(SqlException exc)
        {
             foreach (SqlError item in exc.Errors)
                {
                 log.Error(item.ToString());
                }
        }

        //private void WriteLog(MessageLevel messageLevel, Exception ex)
        //{
        //    if (ex.InnerException != null)
        //        WriteLog(messageLevel, ex.InnerException);
        //    SqlException sqlException;
        //    if((sqlException=ex as SqlException)!=null)
        //        foreach (SqlError item in sqlException.Errors)
        //        {
        //            WriteLog(messageLevel, item.ToString());
        //        }
        //    WriteLog(messageLevel, ex.Message);
        //}
        private void DeletePublication()
        {
            Console.WriteLine("Deleting publication");
            try
            {
                //if (replicationInfo != null)
                {
                    NSI.dalManager.DeleteReplicationPublication(serverInfo.UID);
                    replicationInfo = null;
                }
            }
            catch (ThreadAbortException ex) { throw ex; }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }
            if (replicationInfo != null)
                Console.WriteLine("!Publication is NOT deleted");
            else
                Console.WriteLine("Publication deleted");
        }
        private void CreateSubscription()
        {
            Console.WriteLine("Creating subscription");
            try
            {
                if (parentServer == null)
                    throw new Exception("Главный сервер не найден.");
                PublicationInfo repl = parentServer.RedundancyServer.GetPublicationInfo();
                if (repl != null)
                    subscriptionInfo = NSI.dalManager.CreateReplicationSubscription(repl,
                        CommonData.DecryptText(
                        CommonData.Base64ToString(parentServer.ServerInfo.ConnectionString)));
#if DEBUG
                else
                    Console.WriteLine("repl==null");
#endif
            }
            catch (ThreadAbortException ex) { throw ex; }
            catch (Exception ex)
            {
                //string txt = ex.Message;
                //if (ex.InnerException != null)
                //    txt += " " + ex.InnerException.Message;
                //WriteLog(MessageLevel.Error, txt);
                log.ErrorException(ex.Message, ex);
                subscriptionInfo = null;
            }
            if (subscriptionInfo == null)
                Console.WriteLine("!Subscription is NOT created");
            else
                Console.WriteLine("Subscription created");
        }
        private void DeleteSubscription()
        {
            Console.WriteLine("Deleting subscription");
            try
            {
                //if (subscriptionInfo != null)
                //{
                //    NSI.dalManager.DeleteReplicationSubscription(serverInfo.UID);
                //    subscriptionInfo = null;
                //}

                NSI.dalManager.DeleteReplicationSubscription(null);
                subscriptionInfo = null;
            }
            catch (ThreadAbortException ex) { throw ex; }
            catch (Exception ex)
            {
                log.ErrorException("DeleteSubscription error: " + ex.Message, ex);
            }
            if (subscriptionInfo != null)
                Console.WriteLine("!Subscription is NOT deleted");
            else
                Console.WriteLine("Subscription deleted (or not found)");
        }
        private bool SynchronizeWith(CloneServer target)
        {
            bool res = false;

            try
            {
                Thread.Sleep(2000);
                res = true;
                //PublicationInfo replInfo;

                //replInfo = target.RedundancyServer.GetReplicationPublication();

                //if (replInfo != null)
                //{
                //    subscriptionInfo = NSI.dalManager.CreateReplicationSubscription(replInfo);
                //    if (subscriptionInfo != null)
                //        res = true;
                //}
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
            }

            return res;
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Stop();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion

        #region IRedundancy Members
        /// <summary>
        /// Возвращает массив участников дублирования
        /// </summary>
        /// <returns>Массив участников</returns>
        public ServerInfo[] GetServers()
        {
            //try
            //{
                Console.WriteLine("?Server list requested?");
                
                var arr = from elem in lstServers
                          select elem.ServerInfo;

                return arr.ToArray();
            //}
            //catch (Exception ex)
            //{
            //    WriteLog(MessageCategory.Error, ex.Message);
            //}
        }

        /// <summary>
        /// Запрашивает состояние участника дублирования
        /// </summary>
        /// <returns></returns>
        public ServerState GetState()
        {
            return serverInfo.State;
        }
        /// <summary>
        /// Запрашивает информацию об участнике дублирования
        /// </summary>
        /// <returns></returns>
        public ServerInfo GetInfo()
        {
            return serverInfo;
        }

        /// <summary>
        /// Присоединяет сервер к списку участников дублирования
        /// </summary>
        /// <param name="serverInfo">Информация о подключаемом сервере</param>
        /// <param name="direction">Направление передачи информации</param>
        /// <returns>Массив участников</returns>
        public ServerInfo[] AttachServer(ServerInfo serverInfo, TransferDirection direction)
        {
            //try
            //{
                Console.WriteLine("!Server '" + serverInfo.URL + "' attaching!");
                AddServer(serverInfo);
                if (direction == TransferDirection.All || direction == TransferDirection.Parent)
                {
                    //parentServer = FindParent();
                    if (parentServer != null && parentServer.ServerInfo.URL != serverInfo.URL
                        /*&& parentServer.RedundancyServer != null*/)
                        UpdateServerList(parentServer.RedundancyServer.AttachServer(serverInfo, TransferDirection.Parent));
                }

                if (direction == TransferDirection.All || direction == TransferDirection.Child)
                {
                    foreach (var item in FindChildren())
                    {
                        if (item != null && item.ServerInfo.URL != serverInfo.URL
                            /*&& item.RedundancyServer != null*/)
                            UpdateServerList(item.RedundancyServer.AttachServer(serverInfo, TransferDirection.Child));
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    WriteLog(MessageCategory.Error, ex.Message);
            //}

            return GetServers();
        }

        /// <summary>
        /// Добавлеяет команду в список выполнения
        /// </summary>
        /// <param name="commands">Команда</param>
        public void AddCommands(RedundancyCommand[] commands)
        {
            throw new NotImplementedException();
        }

        //public PublicationInfo GetReplicationPublication()
        //{
        //    return replicationInfo;
        //}

        public string GetConnectionString()
        {
            return CommonData.EncryptText(NSI.dalManager.GetConnectionString());
        }

        public PublicationInfo GetPublicationInfo()
        {
            return replicationInfo;
        }

        #endregion

        internal void OnServerIs()
        {
            if (this.serverInfo.IsMaster)
            {
                if (ServerIsMaster != null)
                    ServerIsMaster(this, EventArgs.Empty);
            }
            else
            {
                if (ServerIsSlave != null)
                    ServerIsSlave(this, EventArgs.Empty);
            }
        }
        internal void Suspend()
        {
            isSuspended = true;
            if (serverInfo != null)
                serverInfo.State = ServerState.Waiting;
            SetDBNothing();
    }
        internal void Continue()
        {
            isSuspended = false;
        }

        #region ITestConnection Members

        public bool Test(Object obj)
        {
            return true;
        }

        #endregion
    }

    /// <summary>
    /// Направление передачи данных
    /// </summary>
    public enum TransferDirection
    {
        /// <summary>
        /// Родителю
        /// </summary>
        Parent,
        /// <summary>
        /// Ребенку
        /// </summary>
        Child,
        /// <summary>
        /// Всем
        /// </summary>
        All,
        /// <summary>
        /// Никому
        /// </summary>
        None
    }

    /// <summary>
    /// Связь информации об участнике дублирования и его потока
    /// </summary>
    internal class CloneServer
    {
        IRedundancy redun = null;

        public IRedundancy RedundancyServer
        {
            get
            {
                if (redun == null)
                    redun = (IRedundancy)Activator.GetObject(typeof(IRedundancy), ServerInfo.URL);
                return redun;
            }
        }
        public ServerInfo ServerInfo { get; internal set; }
        public Thread Thread { get; private set; }
        public bool IsAlive { get; set; }

        public CloneServer(ServerInfo serverInfo, Thread thread)
        {
            ServerInfo = serverInfo;
            Thread = thread;
            IsAlive = false;
        }
    }
}

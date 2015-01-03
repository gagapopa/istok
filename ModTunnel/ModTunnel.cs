//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.Remoting;
//using System.Runtime.Remoting.Channels;
//using System.Runtime.Remoting.Channels.Tcp;
//using System.Runtime.Remoting.Lifetime;
//using System.Text;
//using System.Threading;
//using COTES.ISTOK;
//using COTES.ISTOK.Modules;
//using SimpleLogger;

//namespace COTES.ISTOK.Modules.Tunnel
//{
//    public class ModTunnel
//    {
//        private TcpChannel tcpChannel;
//        private RemoteServer remoteServer;
//        private ObjRef objRemoteServer;
//        private List<MarshalByRefObject> objList;
//        private List<String> modules;
//        private int count;
//        private int port;
//        private bool started;
//        private TimeSpan timeOut;
//        private Timer zombieKillerTimer;
//        private Object syncObject = new Object();

//        public ILogger MessageLog { get; set; }

//        public ModTunnel()
//        {
//            started = false;
//            objList = new List<MarshalByRefObject>();
//            modules = new List<string>();
//            timeOut = TimeSpan.Zero;
//            zombieKillerTimer = new Timer(new TimerCallback(zombieKillerCallBack), null, TimeSpan.FromMilliseconds(-1), timeOut);
//        }
//        public void Start()
//        {
//            Start(port);
//        }
//        public void Start(int Port)
//        {
//            port = Port;
//            try
//            {
//                RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
//                LifetimeServices.LeaseTime = timeOut;
//                LifetimeServices.RenewOnCallTime = timeOut;
//            }
//            catch { }
//            try
//            {
//                if (tcpChannel == null)
//                {
//                    BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
//                    BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();
//                    IDictionary props = new Hashtable();

//                    serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

//                    props["port"] = port;
//                    props.Add("typeFilterLevel", System.Runtime.Serialization.Formatters.TypeFilterLevel.Full);

//                    tcpChannel = new TcpChannel(props, clientProv, serverProv);
//                    ChannelServices.RegisterChannel(tcpChannel, false);

//                    //tcpChannel = new TcpChannel(port);
//                    //ChannelServices.RegisterChannel(tcpChannel, false);
//                }
//                if (remoteServer == null)
//                {
//                    remoteServer = new RemoteServer(this);
//                }

//                objRemoteServer = RemotingServices.Marshal(remoteServer,
//                    Consts.remoteServerURI);

//                zombieKillerTimer.Change(timeOut, timeOut);
//                started = true;
//                OnStartStateChanged(EventArgs.Empty);
//            }
//            catch (Exception exc)
//            {
//                MessageLog.Message(MessageLevel.Error, "Ошибка запуска туннеля: {0}", exc.Message);
//                started = false;
//            }
//        }

//        public bool IsStarted { get { return started; } }
//        public List<MarshalByRefObject> SharedObjects { get { lock (this) { return objList; } } }
//        public List<String> SharedModules { get { lock (this) { return modules; } } }
//        public TimeSpan TimeOut
//        {
//            get { return timeOut; }
//            set
//            {
//                timeOut = value;
//                try
//                {
//                    LifetimeServices.LeaseTime = timeOut;
//                    LifetimeServices.RenewOnCallTime = timeOut;
//                }
//                catch { }
//                zombieKillerTimer.Change(timeOut, timeOut);
//            }
//        }

//        public String GetFreeURI(String URIBase)
//        {
//            //int count;
//            lock (syncObject)//count)
//            {
//                return URIBase + count++ + ".rem";
//            }
//        }
//        public void RegisterObject(MarshalByRefObject Instance, String URI, String ModuleName)
//        {
//            try
//            {
//                lock (this)
//                {
//                    ObjRef objRef = RemotingServices.Marshal(Instance, URI);
//                    objList.Add(Instance);
//                    modules.Add(ModuleName);
//                }
//            }
//            catch (RemotingException exc)
//            {
//                //Program.WriteLog("Error", exc.Message);
//                //Consts.WriteLog(MessageCategory.Error, "Tunnel - " + exc.Message);
//                MessageLog.Message(MessageLevel.Error, "Tunnel - " + exc.Message);
//            }
//            TunnelEventArgs ev = new TunnelEventArgs(ModuleName, URI, port);
//            this.OnObjectRegistered(ev);
//        }
//        public void UnRegisterObject(String URI)
//        {
//            lock (this)
//            {
//                foreach (MarshalByRefObject objRef in objList)
//                {
//                    String localURI = RemotingServices.GetObjectUri(objRef);
//                    localURI = localURI.Substring(localURI.IndexOf("/", 2) + 1);
//                    if (localURI == URI)
//                    {
//                        //modules.Remove(modules[objList.IndexOf(objRef)]);
//                        modules.RemoveAt(objList.IndexOf(objRef));
//                        RemotingServices.Disconnect(objRef);
//                        objList.Remove(objRef);
//                        ((IHandler)objRef).cmd_Close();
//                        break;
//                    }
//                }
//            }
//            TunnelEventArgs ev = new TunnelEventArgs(URI, port);
//            this.OnObjectUnRegistered(ev);
//        }
//        public void Stop()
//        {
//            if (started)
//            {
//                RemotingServices.Disconnect(remoteServer);
//                lock (this)
//                {
//                    foreach (MarshalByRefObject objRef in objList)
//                    {
//                        RemotingServices.Disconnect(objRef);
//                        ((IHandler)objRef).cmd_Close();

//                    }
//                    objList.Clear();
//                    modules.Clear();
//                    started = false;
//                    zombieKillerTimer.Change(-1, 0);
//                    //RemotingServices.Disconnect(remoteServer);
//                }
//                OnStartStateChanged(EventArgs.Empty);
//            }
//        }
//        public MarshalByRefObject GetObjectByURI(String URI)
//        {
//            MarshalByRefObject ret = null;
//            foreach (MarshalByRefObject objRef in objList)
//            {
//                String localURI = RemotingServices.GetObjectUri(objRef);
//                localURI = localURI.Substring(localURI.IndexOf("/", 2) + 1);
//                if (localURI == URI)
//                {
//                    ret = objRef; break;
//                }
//            }
//            return ret;
//        }

//        protected void zombieKillerCallBack(Object state)
//        {
//            int i;
//            //List<MarshalByRefObject> toDeleteObject = new List<MarshalByRefObject>();
//            lock (this)
//            {
//                for (i = 0; i < objList.Count; i++)
//                {
//                    ILease lease = (ILease)objList[i].GetLifetimeService();
//                    if (lease!=null && lease.CurrentState == LeaseState.Expired)
//                    {
//                        //toDeleteObject.Add(objList[i]);
//                        //modules.Remove(RemotingServices.GetObjectUri(objList[i]));
//                        modules.RemoveAt(i);
//                        RemotingServices.Disconnect(objList[i]);
//                        ((IHandler)objList[i]).cmd_Close();
//                        String localURI = RemotingServices.GetObjectUri(objList[i]);
//                        objList.Remove(objList[i]);
//                        TunnelEventArgs ev = new TunnelEventArgs(localURI, 0);
//                        this.OnObjectUnRegistered(ev);
//                        --i;
//                    }
//                }
//                //foreach (MarshalByRefObject obj in toDeleteObject) objList.Remove(obj);
//            }
//            //foreach (MarshalByRefObject obj in toDeleteObject)
//            //{
//            //    String localURI = RemotingServices.GetObjectUri(obj);
//            //    TunnelEventArgs ev = new TunnelEventArgs(null, 0);
//            //    this.OnObjectUnRegistered(ev);
//            //}
//        }

//        protected void OnStartStateChanged(EventArgs e)
//        {
//            if (StartStateChanged != null) StartStateChanged(this, e);
//        }
//        protected void OnObjectRegistered(TunnelEventArgs e)
//        {
//            try
//            {
//                if (ObjectRegistered != null) ObjectRegistered(this, e);
//            }
//            catch { }
//        }
//        protected void OnObjectUnRegistered(TunnelEventArgs e)
//        {
//            try
//            {
//                if (ObjectUnRegistered != null) ObjectUnRegistered(this, e);
//            }
//            catch { }
//        }

//        public event EventHandler StartStateChanged;
//        public event EventHandler<TunnelEventArgs> ObjectRegistered;
//        public event EventHandler<TunnelEventArgs> ObjectUnRegistered;
//    }
//    public class TunnelEventArgs : EventArgs
//    {
//        private String m_uri;
//        private String m_module;
//        private int m_port;
//        public TunnelEventArgs( String uri, int port)
//        {
//            m_uri = uri;
//            m_port = port;
//        }
//        public TunnelEventArgs(String moduleName, String uri, int port)
//        {
//            m_uri = uri;
//            m_port = port;
//            m_module = moduleName;
//        }
//        public String URI { get { return m_uri; } }
//        public int Port { get { return m_port; } }
//        public String ModuleName { get { return m_module; } }
//    }
//}

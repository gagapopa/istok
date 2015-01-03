//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;
//using System.IO;
//using System.IO.Compression;
//using COTES.ISTOK.Modules;
//using System.Runtime.Remoting.Lifetime;
//using System.Threading;

//namespace COTES.ISTOK.Modules.Tunnel
//{
//    public class RemoteServer : MarshalByRefObject, IRemoteServer
//    {
//        private ModTunnel modTunnel;
//        private const String URIBase = "ModuleHandler";

//        public RemoteServer(ModTunnel Tunnel)
//        {
//            modTunnel = Tunnel;
//        }

//        public override object InitializeLifetimeService()
//        {
//            ILease lease = (ILease)base.InitializeLifetimeService();
//            if (LeaseState.Initial == lease.CurrentState)
//            {
//                lease.InitialLeaseTime = TimeSpan.Zero;
//            }
//            return lease;
//        }

//        #region IRemoteServer Members
//        //private FalseLog falseLog;
//        //public void SetFalseLog(FalseLog falseLog)
//        //{
//        //    this.falseLog = falseLog;
//        //}

//        //public String GetHandlerURI(String ModuleName)
//        //{
//        //    return GetHandlerURI(ModuleName);
//        //}

//        public String GetHandlerURI(String ModuleName)//, FalseLog falseLog)
//        {
//            if (ModuleName == null) throw new ArgumentNullException();

//            MarshalByRefObject handler = null;
//            Assembly module = Assembly.LoadFrom(ModuleName);
//            String URI;

//            foreach (Type type in module.GetTypes())
//            {
//                if (type.GetInterface("IHandler") != null) { handler = (MarshalByRefObject)Activator.CreateInstance(type); break; }
//            }
//            if (handler == null)
//                foreach (AssemblyName assemblyName in module.GetReferencedAssemblies())
//                {
//                    try
//                    {
//                        Assembly assembly = Assembly.Load(assemblyName);
//                        foreach (Type type in assembly.GetTypes())
//                        {
//                            if (type.GetInterface("IHandler") != null) { handler = (MarshalByRefObject)Activator.CreateInstance(type); break; }
//                        }
//                    }
//                    catch (Exception) { }
//                }
//            if (handler == null) handler = new Handler();
//            URI = modTunnel.GetFreeURI(URIBase);
//            modTunnel.RegisterObject(handler, URI, ModuleName);
//            return URI;
//        }

//        public void UnRegister(string URI)
//        {
//            modTunnel.UnRegisterObject(URI);
//        }

//        public void SendFile(byte[] stream, string filename)
//        {
//            FileStream fs;

//            if (stream == null)
//                throw new ArgumentNullException("null:stream");
//            if (string.IsNullOrEmpty(filename)) 
//                throw new ArgumentNullException("null:filename");

//            fs = new FileStream(filename, FileMode.Create);

//            fs.Write(stream, 0, stream.Length);
//            fs.Flush();
//            fs.Close();
//        }

//        public void Restart()
//        {
//            Restart(0);
//        }
//        public void Restart(int sleep)
//        {
//            modTunnel.Stop();
//            Thread.Sleep(sleep);
//            modTunnel.Start();
//        }

//        public void Stop()
//        {
//            modTunnel.Stop();
//        }

//        public bool IsStarted
//        {
//            get { return modTunnel.IsStarted; }
//        }
//        #endregion


//        private static int ReadAllBytesFromStream(Stream stream, byte[] buffer)
//        {
//            // Use this method is used to read all bytes from a stream.
//            int offset = 0;
//            int totalCount = 0;
//            while (true)
//            {
//                int bytesRead = stream.Read(buffer, offset, 100);
//                if (bytesRead == 0)
//                {
//                    break;
//                }
//                offset += bytesRead;
//                totalCount += bytesRead;
//            }
//            return totalCount;
//        }
//    }
//}

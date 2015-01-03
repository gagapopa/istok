using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.ServiceModel;
using System.Text;
using System.Threading;
using COTES.ISTOK;
using NLog;

namespace COTES.ISTOK.Block
{
    /// <summary>
    /// класс поддерживающие соединение с глобалом
    /// </summary>
    class ConnectionInspector
    {
        private Thread work_thread = null;

        public IGlobal GlobalServer { get; protected set; }
        public string UID { get; set; }
        public IBlockQueryManager BlockServer { set; protected get; }
        public string RouterIp { get; set; }
        public int RouterPort { get; set; }

        private string host;
        private string port;

        Logger log = LogManager.GetCurrentClassLogger();

        public ConnectionInspector(string globalHost,
                                   string globalPort)
        {
            host = globalHost;
            port = globalPort;
        }

        private void QueryConnectionCycle(object param)
        {
            GlobalServer = null;
            int sleepDelay = 5000;
            const int mulNorm = 1, mulLarge = 5;
            int mul = mulNorm;
            bool wasConnected = true;

            try
            {
                string url = param as string;

                if (string.IsNullOrEmpty(url)) throw new ArgumentException("param is Empty");
                while (true)
                {
                    if (GlobalServer != null)
                    {
                        if (!ISTOK.TestConnection<String>.Test(GlobalServer, UID, false))
                        {
                            GlobalServer = null;
                            Console.WriteLine(">>> GlobalConnector Block disconnected");
                        }
                    }
                    if (GlobalServer == null)
                    {
                        try
                        {
                            //GlobalServer = (IGlobal)Activator.GetObject(typeof(IGlobal), url);
                            Dictionary<string, string> dicAttrs = new Dictionary<string, string>();
                            dicAttrs["url"] = string.Format(@"net.tcp://{0}:{1}/{2}", RouterIp, RouterPort, CommonData.QueryManagerURI);
                            //String externalIP = GetExternalIP();
                            //if (!String.IsNullOrEmpty(externalIP))
                            //    dicAttrs["external_ip"] = externalIP;
                            dicAttrs["port"] = RouterPort.ToString();
                            dicAttrs["rem"] = "BlockQueryManager";// CommonData.QueryManagerURI;

                            const String urlFormat = "net.tcp://{0}:{1}/{2}";

                            ////ClientSettings.FileName = NSI.ConfigFile;
                            String globalUrl = String.Format(urlFormat,
                                                       BlockSettings.Instance.GlobalServerHost,
                                                       BlockSettings.Instance.GlobalServerPort,
                                                       "GlobalQueryManager");

                            EndpointAddress endpoint = new EndpointAddress(globalUrl);
                            var binding = new NetTcpBinding();
                            binding.MaxBufferSize = int.MaxValue;
                            binding.MaxBufferPoolSize = int.MaxValue;
                            binding.MaxReceivedMessageSize = int.MaxValue;
                            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
                            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
                            binding.Security.Mode = SecurityMode.None;

                            //var factory = new ChannelFactory<IGlobal>("NetTcpBinding_GlobalQueryManager", endpoint);
                            var factory = new ChannelFactory<IGlobal>(binding, endpoint);
                            factory.Open();
                            GlobalServer = factory.CreateChannel();

                            if (!GlobalServer.AttachBlock(UID, dicAttrs))
                            {
                                mul = mulNorm;
                                GlobalServer = null;
                                Console.WriteLine(">>> GlobalConnector Block attaching fail (\nURL: {0}\n BackURL: {1}\nPort: {2}\nRem: {3})", url, dicAttrs["url"], dicAttrs["port"], dicAttrs["rem"]);
                            }
                            else
                            {
                                mul = mulLarge;
                                wasConnected = true;
                                Console.WriteLine(">>> GlobalConnector Block attached successfully");
                            }
                        }
                        catch (SocketException exc)
                        {
                            Console.WriteLine(">>> GlobalConnector Block attaching fail: {0}", exc.Message);
                            GlobalServer = null;
                        }
                        catch (EndpointNotFoundException)
                        {
                            if (wasConnected)
                                Console.WriteLine(">>> GlobalConnector Block attaching fail: (Server not found)");
                            wasConnected = false;
                            GlobalServer = null;
                        }
                        catch (RemotingException exc)
                        {
                            Console.WriteLine(">>> GlobalConnector Block attaching fail: {0}", exc.Message);
                            GlobalServer = null;
                        }
                        catch (IOException exc)
                        {
                            Console.WriteLine(">>> GlobalConnector Block attaching fail: {0}", exc.Message);
                            GlobalServer = null;
                        }
                        catch (ArgumentException ex)
                        {
                            GlobalServer = null;
                            log.ErrorException("GlobalConnector AttachBlock ArgumentException", ex);
                        }
                        catch (Exception exc)
                        {
                            GlobalServer = null;
                            log.ErrorException("GlobalConnector AttachBlock", exc);
                        }
                    }

                    if (GlobalServer != null)
                        Thread.Sleep(sleepDelay * mul);
                    else
                        Thread.Sleep(sleepDelay);
                }
            }
            catch (ThreadAbortException) { log.Warn("GlobalConnector is stopped"); }
            catch (Exception ex)
            {
                log.ErrorException("GlobalConnector error", ex);
            }
        }

        private String _externalIP;
        private DateTime _externalIPAge = DateTime.MinValue;

        private string GetExternalIP()
        {
            if (DateTime.Now.Subtract(_externalIPAge) > TimeSpan.FromHours(2))
            {
                try
                {
                    //var proc = new System.Diagnostics.Process();
                    //var startInfo = new System.Diagnostics.ProcessStartInfo("tracert.exe", "-d -h 2 google.com");
                    //startInfo.RedirectStandardOutput = true;

                    //startInfo.UseShellExecute = false;

                    //startInfo.CreateNoWindow = true;

                    //proc.StartInfo = startInfo;

                    //proc.Start();

                    //proc.WaitForExit();

                    //var ipReg = new System.Text.RegularExpressions.Regex(@"\d+\.\d+\.\d+\.\d+");
                    //String output = proc.StandardOutput.ReadToEnd();
                    //var matches = ipReg.Matches(output);

                    //if (matches.Count > 2 && matches[2].Success)
                    //    _externalIP = matches[2].Value;
                    //else _externalIP = null;
                    string whatIsMyIp = "http://automation.whatismyip.com/n09230945.asp";
                    string getIpRegex = @"\d*\.\d*\.\d*\.\d*";
                    System.Net.WebClient wc = new System.Net.WebClient();
                    UTF8Encoding utf8 = new UTF8Encoding();
                    string requestHtml = "";
                    try
                    {
                        requestHtml = utf8.GetString(wc.DownloadData(whatIsMyIp));
                    }
                    catch (System.Net.WebException we)
                    {
                        // do something with exception
                        Console.Write(we.ToString());
                    }
                    System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(getIpRegex);
                    System.Text.RegularExpressions.Match m = r.Match(requestHtml);
                    //System.Net.IPAddress externalIp = null;
                    if (m.Success)
                    {
                        //externalIp = System.Net.IPAddress.Parse(m.Value);
                        _externalIP = m.Value;
                        Console.WriteLine("External IP retrieved: {0}", _externalIP);
                    }
                    else _externalIP = null;
                    _externalIPAge = DateTime.Now;
                }
                catch (Exception exc)
                {
                    log.ErrorException(exc.Message, exc);
                }
            }
            return _externalIP;
        }

        public void BlockServerIsMaster()
        {
            try
            {
                if (work_thread != null)
                {
                    if (work_thread.ThreadState != ThreadState.Suspended)
                        work_thread.Abort();
                    work_thread = null;
                }

                string url = "tcp://" + host + ":" + port + "/GlobalQueryManager.rem";

                WellKnownClientTypeEntry wncte = System.Runtime.Remoting.RemotingConfiguration.IsWellKnownClientType(typeof(IGlobal));
                if (wncte == null)
                {
                    System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownClientType(typeof(IGlobal), url);
                }

                work_thread = new Thread(this.QueryConnectionCycle);
                work_thread.Start(url);
            }
            catch (Exception ex)
            {
                log.ErrorException("ServerChanged error", ex);
            }
        }

        public void BlockServerIsSlave()
        {
            try
            {
                if (work_thread != null)
                {
                    if (work_thread.ThreadState != ThreadState.Suspended)
                        work_thread.Abort();
                    work_thread = null;
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("ServerChanged error", ex);

            }
        }

        //private void WriteToLog(string message)
        //{
        //    Console.WriteLine(message);
        //    if (log != null)
        //        log.Message(MessageLevel.Error,
        //                    message);
        //}

        //private void WriteToLog(Exception exp)
        //{
        //    Console.WriteLine(exp.ToString());
        //    if (log != null)
        //        log.Message(MessageLevel.Error,
        //                    exp.ToString());
        //}
    }
}

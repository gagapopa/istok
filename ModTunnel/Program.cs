using System;
//using System.Collections.Generic;
//using System.Windows.Forms;
//using System.Collections;
//using System.ServiceProcess;
//using System.Reflection;
//using System.Diagnostics;
//using System.Configuration.Install;
//using System.Resources;
//using System.Globalization;
//using System.IO;
//using System.Text;
//using COTES.ISTOK.Modules;
//using COTES.ISTOK;
//using SimpleLogger;

namespace COTES.ISTOK.Modules.Tunnel
{
    static class Program
    {
//        private static ModTunnel modTunnel = null;
//        private static ModTunnelForm tunnelForm = null;

//        static NotifyIcon notifyIcon;

        [STAThread]
        static void Main(String[] args)
        {
//            ResourceManager resourceManager = Properties.Resources.ResourceManager;
//            Application.EnableVisualStyles();
//            Application.SetCompatibleTextRenderingDefault(false);
//            String text;
//            bool repair = false, autostart = false; ;
//            String PIDString = String.Empty;

//            foreach (String argument in args)
//            {
//                if (repair)
//                {
//                    PIDString = argument;
//                    break;
//                }
//                if (argument.ToLower().Equals("-restore")) repair = true;
//            }
//            if (repair)
//            {
//                int PID = -1;
//                Process proc = null;
//                if (String.IsNullOrEmpty(PIDString))
//                    throw new ArgumentException("Не указан PID туннеля");
//                try
//                {
//                    PID = int.Parse(PIDString);
//                }
//                catch (FormatException) { throw new ArgumentException(String.Format("Значение '{0}' не является допустимым PID", PIDString)); }
//                try
//                {
//                    proc = Process.GetProcessById(PID);
//                    proc.WaitForExit();
//                }
//                catch { }
//                autostart = true;
//            }

//            setCulture(resourceManager);

//            //modTunnel = new ModTunnel();
//            tunnelForm = new ModTunnelForm();
//            tunnelForm.Hide();
//            //tunnelForm.Tunnel = modTunnel;

//            text = resourceManager.GetString("ModTunnelText");
//            notifyIcon = new NotifyIcon();
//            notifyIcon.Icon = Properties.Resources.well01;
//            notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);
//            notifyIcon.Visible = true;
//            notifyIcon.Text = text;// "ModTunnel";
//            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
//            text = resourceManager.GetString("MainFormText");
//            contextMenuStrip.Items.Add(text, null, new EventHandler(notifyIcon_DoubleClick));

//            contextMenuStrip.Items.Add(new ToolStripSeparator());
//            text = resourceManager.GetString("ExitText");
//            contextMenuStrip.Items.Add(text, null, new EventHandler(Exit_Click));
//            notifyIcon.ContextMenuStrip = contextMenuStrip;
//            try
//            {
//                //MessageLog = GreateLog();
//                //modTunnel.MessageLog = MessageLog;

//                //MessageCategory level = (MessageCategory)Properties.Settings.Default.MessageLevel;
//                //modTunnel.MessageLog.CurrentMessageLevel = level;
//                //Consts.CurrentMessageLevel =
//            }
//            catch { }

//            tunnelForm.Show();
//            for (int i = 0; i < args.Length && !autostart; i++)
//            {
//                if (args[i].ToLower().Equals("-start") || args[i].Equals("/start")) autostart = true;
//            }
//            if (autostart) tunnelForm.StartTunnel();

//            try
//            {
//                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
//                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
//                Application.Run();
//            }
//            catch (Exception exc)
//            {
//                MessageBox.Show(exc.Message, "Fatal Error");
//                //Consts.WriteLog(MessageCategory.Error, "Tunnel - " + exc.Message);
//                modTunnel.MessageLog.Message(MessageLevel.Error, "Tunnel - " + exc.Message);
//            }
//            finally
//            {
//                notifyIcon.Visible = false;
//                //if (logFile != null) logFile.Close();
//            }

//        }

//        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
//        {
//            Application_ThreadException(sender, new System.Threading.ThreadExceptionEventArgs(e.ExceptionObject as Exception));
//        }

//        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
//        {
//            String format = "Сбой в работе туннеля: {0}";
//#if DEBUG
//            format += String.Format(", \nСбой произошел при выполнении метода {0} \nStackTrace: {1}", e.Exception.TargetSite, e.Exception.StackTrace);
//#endif
//            //MessageLog.Write(MessageCategory.CriticalError, format, e.Exception.Message);
//            Process currentProc = Process.GetCurrentProcess();

//            Process.Start(Application.ExecutablePath, String.Format("-restore {0}", currentProc.Id));
//            try
//            {
//                if (modTunnel.IsStarted)
//                    modTunnel.Stop();
//                foreach (System.Runtime.Remoting.Channels.IChannel chan in System.Runtime.Remoting.Channels.ChannelServices.RegisteredChannels)
//                    System.Runtime.Remoting.Channels.ChannelServices.UnregisterChannel(chan);
//            }
//            catch { }
//            notifyIcon.Visible = false;
//            Application.Exit();
//        }

//        //private static Log GreateLog()
//        //{
//        //    Log log;
//        //    String fileName = AppDomain.CurrentDomain.BaseDirectory + "ModTunnel.log";
//        //    FileStream logFile = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

//        //    log = new StreamLog(logFile);
//        //    return log;
//        //}

//        private static void setCulture(ResourceManager resourceManager)
//        {
//            CultureInfo currentCulture;
//            try
//            {
//                currentCulture = new CultureInfo(Properties.Settings.Default.CurrentCulture);
//            }
//            catch (NullReferenceException)
//            {
//                currentCulture = CultureInfo.InstalledUICulture;
//            }
//            catch (ArgumentException)
//            {
//                currentCulture = CultureInfo.InvariantCulture;
//            }
//            if (resourceManager.GetResourceSet(currentCulture, true, false) == null)
//            {
//                currentCulture = CultureInfo.InvariantCulture;
//            }
//            if (currentCulture.IsNeutralCulture)
//            {
//                foreach (CultureInfo culture in
//                    CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures))
//                {
//                    if (culture.Parent.Equals(currentCulture)) { currentCulture = culture; break; }
//                }
//            }
//            System.Threading.Thread.CurrentThread.CurrentUICulture =
//                System.Threading.Thread.CurrentThread.CurrentCulture =
//                currentCulture;
//        }

//        private static void notifyIcon_DoubleClick(object sender, EventArgs e)
//        {
//            if (tunnelForm != null)
//            {
//                tunnelForm.Show();
//                if (tunnelForm.WindowState == FormWindowState.Minimized)
//                    tunnelForm.WindowState = FormWindowState.Normal;
//                tunnelForm.BringToFront();
//            }
//        }

//        private static void Exit_Click(object sender, EventArgs e)
//        {
//            modTunnel.Stop();
//            Application.Exit();
//        }
//        public static CultureInfo CurrentCulture
//        {
//            get
//            {
//                return CultureInfo.GetCultureInfo(Properties.Settings.Default.CurrentCulture);
//            }
//            set
//            {
//                Properties.Settings.Default.CurrentCulture = value.LCID;
//                Properties.Settings.Default.Save();

//            }
        }
    }
}
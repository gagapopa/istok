using System;
using System.Collections;
using System.ServiceProcess;
using System.Text;
using System.Configuration.Install;
using System.Diagnostics;
using COTES.ISTOK;
using COTES.ISTOK.Assignment;

namespace COTES.ISTOK.Assignment.Service
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                GlobalSettings.Instance.Load(GlobalSettings.DefaultConfigPath);//GlobalSettings.Instance.DefaultConfigFile);
                ServiceBase.Run(new GlobalService());
            }
            else if (args.Length == 2 && args[0].ToLower() == "-config")
            {
                GlobalSettings.Instance.Load(args[1]);
                ServiceBase.Run(new GlobalService());
            }
            else if (args.Length == 1)
            {
                if (args[0].ToLower() == "-help")
                    ShowHelp();
                else if (args[0].ToLower() == "-console")
                {
                    GlobalSettings.Instance.Load(GlobalSettings.DefaultConfigPath);//GlobalSettings.Instance.DefaultConfigFile);
                    ConsoleRun();
                }
                else if (args[0].ToLower() == "-install")
                    InstallService();
                else if (args[0].ToLower() == "-remove")
                    RemoveService();
                else if (args[0].ToLower() == "-start")
                    StartService();
                else if (args[0].ToLower() == "-stop")
                    StopService();
                else if (args[0].ToLower() == "-restart")
                    RestartService();
                else
                    ShowHelp();
            }
            else if (args.Length == 3 &&
                     args[0].ToLower() == "-console" &&
                     args[1].ToLower() == "-config")
            {
                GlobalSettings.Instance.Load(args[2]);
                ConsoleRun();
            }
            else
                ShowHelp();
            Console.ReadLine();
        }

        private static void ConsoleRun()
        {
            Creator creator = new Creator();

            Console.Clear();

            bool reg=false;
            try
            {
                reg = CommonData.CheckRegister(COTES.ISTOK.Assignment.GNSI.LicenseFile);
            }
            catch (Exception exc)
            {
                Console.Write(exc.Message);
            }
            if (!reg) Console.WriteLine("\nNot registered. Dummy mode.");
            Console.WriteLine("Product version is {0}", VersionInfo.BuildVersion.VersionString/*CommonData.Version*/);
            Console.Write("Global server starting...");

            creator.Stopped += new EventHandler((s, e) => Environment.Exit(0));
            creator.Start();

            Console.WriteLine("OK\nPress any ENTER to exit.");

            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

            Console.WriteLine("Shutting down...");

            creator.Stop();
        }

        private static void ShowHelp()
        {
            Console.WriteLine("GlobSvc.exe [key]");
            Console.WriteLine("  where key is:");
            Console.WriteLine("  -help    - show this text");
            Console.WriteLine("  -install - install service into system");
            Console.WriteLine("  -remove  - uninstall service from system");
            Console.WriteLine("  -start   - run service manually");
            Console.WriteLine("  -stop    - stop service manually");
            Console.WriteLine("  -console - run server in console");
        }

        private static void InstallService()
        {
            using (GlobalServiceInstaller pi = new GlobalServiceInstaller())
            {
                IDictionary savedState = new Hashtable();
                try
                {
                    pi.Context = new InstallContext();
                    pi.Context.Parameters.Add("assemblypath", Process.GetCurrentProcess().MainModule.FileName);
                    foreach (Installer i in pi.Installers)
                        i.Context = pi.Context;
                    pi.Install(savedState);
                    pi.Commit(savedState);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    pi.Rollback(savedState);
                }
            }
        }

        private static void RemoveService()
        {
            using (GlobalServiceInstaller pi = new GlobalServiceInstaller())
            {
                try
                {
                    pi.Context = new InstallContext();
                    pi.Context.Parameters.Add("assemblypath", Process.GetCurrentProcess().MainModule.FileName);
                    foreach (Installer i in pi.Installers)
                        i.Context = pi.Context;
                    pi.Uninstall(null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void StartService()
        {
            foreach (ServiceController sc in ServiceController.GetServices())
            {
                if (sc.ServiceName == CommonData.GlobalServiceName)
                {
                    if ((sc.Status.Equals(ServiceControllerStatus.Stopped))
                        || (sc.Status.Equals(ServiceControllerStatus.StopPending)))
                    {
                        sc.Start();
                        Console.WriteLine("Служба запущена.");
                    }
                    return;
                }
            }
            Console.WriteLine("Служба не установлена. Воспользуйтесь ключом -install");
        }

        private static void StopService()
        {
            foreach (ServiceController sc in ServiceController.GetServices())
            {
                if (sc.ServiceName == CommonData.GlobalServiceName)
                {
                    if ((!sc.Status.Equals(ServiceControllerStatus.Stopped))
                      && (!sc.Status.Equals(ServiceControllerStatus.StopPending)))
                    {
                        sc.Stop();
                    }
                    Console.WriteLine("Служба остановлена.");
                    return;
                }
            }
        }

        private static void RestartService()
        {
            ServiceController svc = new ServiceController(CommonData.GlobalServiceName);
            if (svc != null)
            {
                svc.Stop();
                svc.WaitForStatus(ServiceControllerStatus.Stopped);
                svc.Start();
            }
        }
    }
}

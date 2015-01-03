using System;
using System.Collections;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;
using System.IO;

namespace COTES.ISTOK.Block.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            CommonData.applicationName = CommonData.BlockServiceName;

            try
            {
                if (args.Length == 0)
                {
                    BlockSettings.Instance.Load(BlockSettings.DefaultConfigPath);//BlockSettings.Instance.DefaultConfigFile);
                    ServiceBase.Run(new BlockService());
                }
                else if (args.Length == 2 && args[0].ToLower() == "-config")
                {
                    BlockSettings.Instance.Load(args[1]);
                    ServiceBase.Run(new BlockService());
                }
                else if (args.Length == 1)
                {
                    if (args[0].ToLower() == "-install")
                        InstallService();
                    else if (args[0].ToLower() == "-remove")
                        RemoveService();
                    else if (args[0].ToLower() == "-start")
                        StartService();
                    else if (args[0].ToLower() == "-stop")
                        StopService();
                    else if (args[0].ToLower() == "-console")
                    {
                        BlockSettings.Instance.Load(BlockSettings.DefaultConfigPath);//BlockSettings.Instance.DefaultConfigFile);
                        ConsoleRun();
                    }
                    else
                        ShowHelp();
                }
                else if (args.Length == 3 && 
                         args[0].ToLower() == "-console" && 
                         args[1].ToLower() == "-config")
                {
                    BlockSettings.Instance.Load(args[2]);
                    ConsoleRun();
                }
                else
                    ShowHelp();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static void ConsoleRun()
        {
            Console.WriteLine("Product version is {0}", VersionInfo.BuildVersion.VersionString/*CommonData.Version*/);
            Console.WriteLine("Server starting with " + "...");

            Creator creator = new Creator();
            creator.Start();
            Console.WriteLine("OK\nPress ENTER to exit.");
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.P) NSI.redundancyServer.PrintNeighbours();
            }
            Console.WriteLine("Shutting down...");
            creator.Stop();
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Program keys:");
            Console.WriteLine("  -install - install service into system");
            Console.WriteLine("  -remove  - uninstall service from system");
            Console.WriteLine("  -start   - run service manually");
            Console.WriteLine("  -stop    - stop service manually");
            Console.WriteLine("  -config file - run service with configuration");
            Console.WriteLine("  -console [-config file] - run service in console [with configuration]");
        }

        private static void InstallService()
        {
            // установить
            using (BlockServiceInstaller pi = new BlockServiceInstaller())
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
            // удалить
            using (BlockServiceInstaller pi = new BlockServiceInstaller())
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
                if (sc.ServiceName == CommonData.BlockServiceName)
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
                if (sc.ServiceName == CommonData.BlockServiceName)
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
    }
}

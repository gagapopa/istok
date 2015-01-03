using COTES.ISTOK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace COTES.ISTOKDataUpdate
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            if (args==null||args.Length<1)
            {
                StartGUI();
            }
            else
            {
                switch (args[0])
                {
                    case "-gui":
                        StartGUI();
                        break;
                    case "-block":
                        UpdateBlock();
                        break;
                    case "-global":
                        UpdateGlobal();
                        break;
                    case "-client":
                        UpdateClient();
                        break;
                    default:
                        StartGUI();
                        break;
                } 
            }
        }

        private static void StartGUI()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DataUpdateForm());
        }

        private static void UpdateBlock()
        {
            Update(CurrentService.ServiceType.Block);
        }

        private static void UpdateGlobal()
        {
            Update(CurrentService.ServiceType.Global);
        }

        private static void UpdateClient()
        {
            Update(CurrentService.ServiceType.Client);
        }

        private static void Update(CurrentService.ServiceType serviceType)
        {
            try
            {
                CurrentService service = new CurrentService();

                //service.RegisterMessageWriter(WriteMessage);

                service.CurrentServiceType = serviceType;
                service.RegisterMessageWriter(WriteMessage);

                switch (serviceType)
                {
                    //case CurrentService.ServiceType.Client:
                    //    ClientSettings.Instance.Load(ClientSettings.DefaultConfigPath);
                    //    service.Settings = ClientSettings.Instance;
                    //    break;
                    case CurrentService.ServiceType.Block:
                        BlockSettings.Instance.Load(BlockSettings.DefaultConfigPath);
                        service.Settings = BlockSettings.Instance;
                        break;
                    case CurrentService.ServiceType.Global:
                        GlobalSettings.Instance.Load(GlobalSettings.DefaultConfigPath);
                        service.Settings = GlobalSettings.Instance;
                        break;
                    default:
                        return; ;
                }

                UpdateProccess updater = new UpdateProccess(service);

                updater.Proccess();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        public static void WriteMessage(MessageCategory category, String text)
        {
            Console.WriteLine(text);
        }
    }
}

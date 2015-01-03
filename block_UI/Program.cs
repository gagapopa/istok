using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ServiceProcess;
using COTES.ISTOK;
using Microsoft.Win32;

namespace COTES.ISTOK.Block.UI
{
    static class Program
    {
        public static String ServicePath { get; set; }
        public static ServiceController BlockSvc { get; set; }

        private static string config = "";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            try
            {
                if (args.Length == 2 && args[0] == "-config")
                {
                    BlockSettings.Instance.Load(args[1]);
                }
                else
                {
                    BlockSettings.Instance.Load(BlockSettings.DefaultConfigPath);// BlockSettings.Instance.DefaultConfigFile);
                }
            }
            catch
            { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //string[] args = Environment.GetCommandLineArgs();

            //if (args.Length > 1)
            //{
            //    config = args[1];
            //    //ClientSettings.FileName = config; ???
            //}

            UpdateService();
            Application.Run(new SettingForm());
        }

        public static void UpdateService()
        {
            try
            {
                ServiceController[] serArray = ServiceController.GetServices();
                BlockSvc = null;

                foreach (ServiceController service in serArray)
                    if (service.ServiceName == CommonData.BlockServiceName)
                    {
                        BlockSvc = service;
                        break;
                    }
                if (BlockSvc != null) ServicePath = GetImagePath(BlockSvc.ServiceName);
                else return;

                //if (string.IsNullOrEmpty(config))
                    //ClientSettings.FileName = ServicePath + "\\config.xml";

            }
            catch (Exception ex)
            {
                MessageBox.Show("BlockService.DoWork: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GetImagePath(string serviceName)
        {
            string registryPath = @"SYSTEM\CurrentControlSet\Services\" + serviceName;
            RegistryKey keyHKLM = Registry.LocalMachine;
            RegistryKey key;
            string value;
            int i;

            key = keyHKLM.OpenSubKey(registryPath);

            value = key.GetValue("ImagePath").ToString();
            key.Close();
            value = Environment.ExpandEnvironmentVariables(value);

            i = value.LastIndexOf('\\');

            if (i != -1)
                value = value.Substring(1, i - 1);

            return value;
        }
    }
}

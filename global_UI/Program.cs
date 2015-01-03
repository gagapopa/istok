using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace COTES.ISTOK.Assignment.UI
{
    static class Program
    {
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
                    GlobalSettings.Instance.Load(args[1]);
                }
                else
                {
                    GlobalSettings.Instance.Load(GlobalSettings.DefaultConfigPath);//GlobalSettings.Instance.DefaultConfigFile);
                }
            }
            catch
            { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SettingForm());
        }
    }
}
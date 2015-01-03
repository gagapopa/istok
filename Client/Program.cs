using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Client.Extension;

namespace COTES.ISTOK.Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new MainForm();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            ExtensionManager = new ClientExtensionManager();
            //ReportSourceManager = new ReportSourceManager();
            Application.Run(MainForm);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exc = e.ExceptionObject as Exception;
            String mess;

            if (exc == null)
            {
                mess = (e.ExceptionObject ?? String.Empty).ToString();

                NLog.LogManager.GetCurrentClassLogger().Fatal("Сбой при работе клиента. {0}", mess);

                MessageBox.Show(mess, "Сбой в работе приложения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            NLog.LogManager.GetCurrentClassLogger().FatalException("Сбой при работе клиента", exc);
            mess = exc.Message;
#if DEBUG
            mess += "\nStackTrace: " + exc.StackTrace;
#endif
            MessageBox.Show(mess, "Сбой в работе приложения", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        internal static MainForm MainForm { get; private set; }

        internal static ClientExtensionManager ExtensionManager { get; private set; }

        //internal static ReportSourceManager ReportSourceManager { get; private set; }

        internal static string DragDropPrefix = "ISTOK";
        internal static string DragDropParameter = "PARAM";
        internal static string DragDropFunction = "FUNC";

        internal static object CreateDragDropData(UnitNode node)
        {
            string res, type;

            if (node is NormFuncNode) type = Program.DragDropFunction;
            else type = Program.DragDropParameter;
            res = string.Format("{0}{1}{2}{3}{4}",
                Program.DragDropPrefix,
                type,
                node.Code,
                '\a',
                node.Text);
            return res;
        }
    }
}

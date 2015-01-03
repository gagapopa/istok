using System;
using System.ServiceProcess;
using System.IO;
using NLog;
//using SimpleLogger;

namespace COTES.ISTOK.Block.Service
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class BlockService : ServiceBase
    {
        private Creator creator = null;
        Logger log = LogManager.GetCurrentClassLogger();

        public BlockService()
        {
            InitializeComponent();

            CommonData.applicationName = "Сервер сбора данных. ИСТОК";

            this.ServiceName = CommonData.applicationName;

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            creator = new Creator();
            //try
            //{
            //    LoggerContainer container = new LoggerContainer();
            //    creator.MessageLog = container;
            //    LoggerManager.AcceptConfiguration(BlockSettings.Instance.Logs);
            //    container.Content = LoggerManager.DefaultLogger;
            //}
            //catch { }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception exc = e.ExceptionObject as Exception;
                AppDomain domain = sender as AppDomain;
                //this.creator.MessageLog.Message(MessageLevel.Critical, exc);
                NLog.LogManager.GetCurrentClassLogger().FatalException("Сбой в работе одного из потоков", exc);
                AppDomain.Unload(domain);
            }
            catch { }
        }

        protected override void OnStart(string[] args)
        {
            string mesaga = "";
            try
            {
                creator.Start();
                mesaga = String.Format("Служба {0} запущена", CommonData.applicationName);

                log.Info("Служба {0} запущена", CommonData.applicationName);
            }
            catch (Exception ex)
            {
                log.ErrorException("Ошибка запуска службы", ex);
                throw;
            }
        }

        protected override void OnStop()
        {
            try
            {
                creator.Stop();
                log.Info("Остановлена служба {0}", CommonData.applicationName);
            }
            catch (Exception ex)
            {
                log.ErrorException(String.Format("Ошибка остановки службы {0}", CommonData.applicationName), ex);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnContinue()
        {
            base.OnContinue();
        }
    }
}

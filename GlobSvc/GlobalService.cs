using System;
using System.ServiceProcess;
using System.Threading;
using COTES.ISTOK;
using COTES.ISTOK.Assignment;
using System.Diagnostics;
using NLog;

namespace COTES.ISTOK.Assignment.Service
{
    public partial class GlobalService : ServiceBase
    {
        Logger log = LogManager.GetCurrentClassLogger();

        private Thread worker = null;
        Creator creator;

        public GlobalService()
        {
            creator = new Creator();
            }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception exc = e.ExceptionObject as Exception;
                log.FatalException("Сбой при работе общестанционного сервера", exc);
            }
            catch { }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                creator.Start();
            }
            catch (Exception ex) 
            {
                log.FatalException("", ex);
            }
        }
        protected override void OnStop()
        {
            try
            {
                if (worker != null) worker.Abort();
                creator.Stop();
            }
            catch (Exception ex)
            {
                log.FatalException("", ex);
            }
        }

        private void DoImportantWork()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(500);
                }
            }
            catch { }
        }

        protected override void OnPause()
        {
            base.OnPause();
            try
            {
                creator.Stop();
            }
            catch (Exception ex)
            {
                log.FatalException("", ex);
            }
        }
        protected override void OnContinue()
        {
            base.OnContinue();
            try
            {
                creator.Start();
            }
            catch (Exception ex)
            {
                log.FatalException("", ex);
            }
        }
    }
}

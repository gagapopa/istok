using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    public partial class BaseAsyncWorkForm : Form
    {
        //private object sync_obj = new object();
        private List<AsyncOperationWatcher> watchers = new List<AsyncOperationWatcher>();
        //private AsyncOperationWatcher watcher = null;

        private bool closeOnFinishWatcher;

        protected StructureProvider strucProvider;

        /// <summary>
        /// свойство сигнализируеющие, что окно закрывается программно
        /// и не следует вести диалог с пользователем о закрытии.
        /// </summary>
        private bool soft_close_form = false;
        private bool form_closing = false;
        /// <summary>
        /// Свойство, говорящие о том, что окно будет закрыто когда асинхронная операция будет завершена
        /// </summary>
        public bool CloseOnFinishWatcher
        {
            get { return closeOnFinishWatcher; }
            set
            {
                closeOnFinishWatcher = value;
                
                lock (watchers)
                {
                    form_closing = AllWatchersComplete();
                }

                if (form_closing)
                {
                    if (InvokeRequired) BeginInvoke((Action)Close);
                    else Close();
                }
            }
        }
        public BaseAsyncWorkForm()
        {
            InitializeComponent();
            ShowStatusStripAsyncView();
        }

        public BaseAsyncWorkForm(StructureProvider strucProvider):this()
        {
            this.strucProvider = strucProvider;
            //InitializeComponent();
            //ShowStatusStripAsyncView();
        }

        private bool statusHiden;

        [Browsable(true)]
        [Category("Behavior")]
        //[DefaultValue(true)]
        public bool StatusHiden
        {
            get { return statusHiden; }
            set
            {
                statusHiden = value;
                statusStripAsyncView.Visible = !statusHiden; }
        }

        /// <summary>
        /// Задает флаг закрытия формы для free-low-lock-custom-синхронизации
        /// </summary>
        /// <param name="value">состояние</param>
        public void SetFormClosingFlag(bool value)
        {
            form_closing = value;
        }

        public IEnumerable<AsyncOperationWatcher> Watchers
        {
            get { return watchers; }
        }

        protected virtual void ShowStatus(string status, double progress)
        {
            try
            {
                if (this.IsHandleCreated && !this.Disposing && !this.IsDisposed)
                {
                    var dlg =
                        new Action<string, double>((string s, double p) =>
                            {
                                this.toolStripStatusLabel.Text = s;
                                if (p > 100) p = 100;
                                this.toolStripProgressBar.Value = (int)p;
                            });
                    if (InvokeRequired)
                        this.Invoke(dlg, status, progress);
                    else
                        dlg(status, progress);
                }
            }
            catch (ObjectDisposedException) { }
        }

//        protected void ShowError(Exception exp)
//        {
//            bool hand = false;
//            ShowError(exp, ref hand);
//        }
//        protected void ShowError(Exception exp, ref bool handlered)
//        {
//            if (!handlered)
//            {
//                if (exp is UserNotConnectedException)
//                {
//                    MessageBox.Show("Пользователь был отключен от системы, необходимо переподключится", "Переподключение к серверу");
//                    Program.MainForm.Reconnect();
//                }
//                else
//                {

//                    String mess = exp.Message;
//#if DEBUG
//                    mess += String.Format("\n{0}", exp.StackTrace);
//#endif
//                    MessageBox.Show(mess,
//                                       "Error",
//                                       MessageBoxButtons.OK,
//                                       MessageBoxIcon.Error);
//                }
//                handlered = true;
//            }
//        }

        public void ShowError(Exception ex, ref bool handlered)
        {
            if (!handlered)
            {
                ShowError(ex);
                handlered = true;
            }
        }

        public virtual void ShowError(Exception ex)
        {
            //ShowError(ex);
            Program.MainForm.ShowError(ex);
        }

        protected void WaitWatcher(AsyncOperationWatcher watcher)
        {
            while (watcher != null && watcher.State!= AsyncOperationWatcher.WatcherState.OutOfWork)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(50);
            }
        }

        protected virtual void BaseAsyncWorkForm_FormClosing(object sender, 
                                                             FormClosingEventArgs e)
        {
            //form_closing = true;

            //if (soft_close_form /*&& closeOnFinishWatcher*/) return;

            lock (watchers)
            {
                if (AllWatchersComplete()) return;

                if (soft_close_form)
                {
                    AbortWatchers(watchers);
                }
                else
                {
                    DialogResult exit = MessageBox.Show("Ожидается завершение запроса. Прервать и выйти?",
                                            "Выйти?",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Question);

                    if (exit == DialogResult.Yes)
                        AbortWatchers(watchers);
                    else
                    {
                        form_closing = false;
                        e.Cancel = true;
                    } 
                }
            }
        }

        protected void toolStripSplitButtonAbort_ButtonClick(object sender, EventArgs e)
        {
            List<AsyncOperationWatcher> aborted_watchers;
            lock (watchers)
            {
                if (AllWatchersComplete()) return;
                
                aborted_watchers = watchers;
                watchers = new List<AsyncOperationWatcher>();
            }

            DialogResult abort = MessageBox.Show("При прерывании некоторые данные могут быть утеряны. Все равно прервать?",
                                                 "Вы уверены, что желаете прервать операцию?",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Stop);

            if (abort == DialogResult.Yes)
                AbortWatchers(aborted_watchers);
            ShowStatusStripAsyncView();
        }

        private void AbortWatchers(List<AsyncOperationWatcher> watchers)
        {
            soft_close_form = true;
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
                watchers.ForEach(x =>
                    {
                        try
                        {
                            x.AbortWatcher();
                        }
                        catch { }
                    });
            });
        }

        protected bool AllWatchersComplete()
        {
            lock (watchers)
            {
                return watchers.Count == 0 ||
                           watchers.All(x =>
                               x.State == AsyncOperationWatcher.WatcherState.OutOfWork); 
            }
        }

        public void RunWatcher(AsyncOperationWatcher watcher_added)
        { RunWatcher(watcher_added, true); }

        internal void RunWatcher(AsyncOperationWatcher watcher_added, bool retrieveMessages)
        {
            if (form_closing)
                throw new Exception("Can't run operation, because the form is closing at this moment");

            lock (watchers)
            {
                if (AllWatchersComplete())
                {
                    RemoveNotWorkWatchers();
                    AddHandlers(watcher_added);
                }
                watchers.Add(watcher_added);
                if (retrieveMessages) watcher_added.AddMessageReceivedHandler(m => Program.MainForm.MessagesForm.ShowMessages(this, m));
                watcher_added.Run();
            }
        }

        protected virtual void AddHandlers(AsyncOperationWatcher watcher_added)
        {
            watcher_added.AddStartHandler(() =>
                {
                    try
                    {
                        this.ShowStatus("", 0.0);
                        ShowStatusStripAsyncView();
                    }
                    catch (ObjectDisposedException) { }
                });
            watcher_added.AddStatusChangeHandle(this.ShowStatus);
            watcher_added.AddErrorHandler(this.ShowError);
            watcher_added.AddFinishHandler(() =>
                {
                    try
                    {
                        RegisterNextWorkWatcher();
                        ShowStatusStripAsyncView();
                        RemoveNotWorkWatchers();
                        watchers.Remove(watcher_added);
                    }
                    catch (ObjectDisposedException) { }
                });
            watcher_added.AddWatcherDownHandler((Exception exp, ref bool handlered) =>
                {
                    try
                    {
                        ShowError(exp, ref handlered);
                        handlered = true;
                        RegisterNextWorkWatcher();
                        ShowStatusStripAsyncView();
                        RemoveNotWorkWatchers();
                        watchers.Remove(watcher_added);
                    }
                    catch (ObjectDisposedException) { }
                });
        }

        private void RegisterNextWorkWatcher()
        {
            lock (watchers)
            {
                while (!AllWatchersComplete())
                {
                    var next = watchers.FirstOrDefault(x => 
                        x.State == AsyncOperationWatcher.WatcherState.Work ||
                        x.State == AsyncOperationWatcher.WatcherState.NotStart);
                    if (next == null) break;
                    AddHandlers(next);
                    if (next.State == AsyncOperationWatcher.WatcherState.Work) break;
                }
                if (CloseOnFinishWatcher && AllWatchersComplete())
                {
                    if (InvokeRequired) BeginInvoke((Action)Close);
                    else Close();
                }
            }
        }

        private void RemoveNotWorkWatchers()
        {
            lock (watchers)
            {
                watchers.RemoveAll(x => x.State == AsyncOperationWatcher.WatcherState.OutOfWork);
            }
        }

        /// <summary>
        /// Показать/скрыть содержимое панели состояния
        /// </summary>
        protected virtual void ShowStatusStripAsyncView()
        {
            if (InvokeRequired) Invoke((Action)ShowStatusStripAsyncView);
            else
            {
                lock (watchers)
                {
                    bool visible = !AllWatchersComplete();
                    foreach (ToolStripItem item in statusStripAsyncView.Items)
                        item.Visible = visible;
                }
            }
        }

        private void BaseAsyncWorkForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            form_closing = false;
            ClientSettings.Instance.SaveFormState(this);
            strucProvider.ReleaseNode();
        }

        private void BaseAsyncWorkForm_Load(object sender, EventArgs e)
        {
            ClientSettings.Instance.LoadFormState(this);
        }

        internal void AbortAllAsyncOperations()
        {
            lock (watchers)
            {
                try
                {
                    //watchers.ForEach(x => x.AbortOperation());
                    AbortWatchers(watchers);
                }
                catch { }
            }
        }

        //ISite serviceContainer;
        //public virtual ISite ServiceContainer
        //{
        //    get
        //    {
        //        if (serviceContainer == null)
        //        {
        //            FormOrientedServiceContainer formOrientedServiceContainer;
        //            formOrientedServiceContainer = new FormOrientedServiceContainer(strucProvider.Session.GetServiceContainer());
        //            formOrientedServiceContainer.AddService(this);
        //            serviceContainer = formOrientedServiceContainer;
        //        }
        //        return serviceContainer;
        //    }
        //}
    }
}

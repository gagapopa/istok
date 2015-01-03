using System;
using System.Threading;

namespace COTES.ISTOK.ClientCore
{
    /// <summary>
    /// Смотритель за асинхронными операциями.
    /// Запускает поток в котором через заданные 
    /// промежутки времени опрашивает состоянии асинхронной операции.
    /// </summary>
    public class AsyncOperationWatcher
    {
        public enum WatcherState : byte
        {
            NotStart,
            Work,
            OutOfWork
        }

        private Thread watch_thread = null;
        protected IAsyncOperationManager manager = null;
        private TimeSpan query_timeout = new TimeSpan(0, 0, 10);
        private bool has_error = false;
        private volatile bool work = true;
        private TimeSpan exit_wait_time = new TimeSpan(0, 0, 1);

        //protected Guid userGuid;

        public TimeSpan QueryPeriod
        {
            set
            {
                if (value.TotalSeconds < 0)
                    throw new ArgumentException("Query timeout must be more then 0");

                query_timeout = value;
            }
            get { return query_timeout; }
        }
        public WatcherState State { get; protected set; }
        public ulong OperationID { private set; get; }

        public delegate void StartDelegate();
        public delegate void MessageReceivedDelegate(Message[] messages);
        public delegate void ErrorNotificationDelegate(Exception exp, ref bool handlered);
        public delegate void StatusChangeDelegate(string status, double progress);
        public delegate void FinishNotificationDelegate();

        private event StartDelegate StartEvent;
        private event MessageReceivedDelegate MessageReceivedEvent;
        private event FinishNotificationDelegate FinishEvent;
        private event ErrorNotificationDelegate ErrorEvent;
        private event StatusChangeDelegate StatusChangeEvent;
        private event FinishNotificationDelegate SuccessFinishEvent;
        private event ErrorNotificationDelegate WatcherDownEvent;

        public AsyncOperationWatcher(//Guid 
                                     ulong operation_id,
                                     IAsyncOperationManager manager)
        {
            //this.userGuid = userGuid;
            OperationID = operation_id;
            watch_thread = new Thread(this.QueryCicle);
            watch_thread.Priority = ThreadPriority.Lowest;
            this.manager = manager;
            QueryPeriod = TimeSpan.FromMilliseconds(100);
            State = WatcherState.NotStart;
        }

        public AsyncOperationWatcher(//Guid  
                                     ulong operation_id,
                                     IAsyncOperationManager manager,
                                     TimeSpan query_period)
            : this(operation_id, manager)
        {
            QueryPeriod = query_period;
        }

        public void AddStartHandler(StartDelegate handler)
        { if (CanAddHandler(handler))StartEvent += handler; }

        public void AddMessageReceivedHandler(MessageReceivedDelegate handler)
        { if (CanAddHandler(handler))MessageReceivedEvent += handler; }

        public void AddFinishHandler(FinishNotificationDelegate handler)
        {
            if (handler != null
                && (watch_thread.ThreadState == ThreadState.Stopped
                || watch_thread.ThreadState == ThreadState.Aborted))
                handler();
            if (CanAddHandler(handler)) FinishEvent += handler;
        }

        public void AddErrorHandler(ErrorNotificationDelegate handler)
        { if (CanAddHandler(handler))ErrorEvent += handler; }

        public void AddStatusChangeHandle(StatusChangeDelegate handler)
        { if (CanAddHandler(handler))StatusChangeEvent += handler; }

        public void AddWatcherDownHandler(ErrorNotificationDelegate handle)
        { if (CanAddHandler(handle))WatcherDownEvent += handle; }

        public void AddSuccessFinishHandler(FinishNotificationDelegate handler)
        { if (CanAddHandler(handler))SuccessFinishEvent += handler; }

        protected bool CanAddHandler(object handler)
        { return !(watch_thread.IsAlive && handler == null); }

        public virtual void Run()
        {
            try
            {
                watch_thread.Start();
            }
            catch (Exception ex)
            {
                State = WatcherState.OutOfWork;
                WatcherDown(ex);
            }
        }

        public virtual void Wait()
        {
            while (State == WatcherState.Work)
                Thread.Sleep(50);
        }

        private void QueryCicle()
        {
            double last_progress = 0.0;
            double current_progress = 0.0;

            string last_status = "";
            string current_status = "";

            //object recived_value = null;
            UAsyncResult r;

            State = WatcherState.Work;

            try
            {
                if (StartEvent != null)
                    StartEvent.Invoke();

                while (work)
                {
                    if ((current_status = manager.GetStatus(OperationID)) != last_status ||
                        (current_progress = manager.GetProgress(OperationID)) != last_progress)
                    {
                        if (StatusChangeEvent != null)
                            StatusChangeEvent(current_status, current_progress);
                        last_status = current_status;
                        last_progress = current_progress;
                    }

                    //CheckErrorAndInformIfNeed();

                    if (!manager.IsInterrupted(OperationID))
                    {
                        ValueReceive(false);

                        if (MessageReceivedEvent != null) // запрос сообщений
                        {
                            r = manager.GetMessages(OperationID);
                            if (r != null && r.Messages != null)
                                MessageReceivedEvent(r.Messages);
                        }
                    }
                    else
                        InformAboutAbort();
                    
                    if (manager.IsComplete(OperationID))
                    {
                        //CheckErrorAndInformIfNeed();

                        ValueReceive(true);

                        if (MessageReceivedEvent != null) // собрать оставшиеся сообщения
                            while ((r = manager.GetMessages(OperationID)) != null)
                                if (r.Messages != null)
                                    MessageReceivedEvent.Invoke(r.Messages);

                        work = false;
                        try
                        {
                            manager.End(OperationID);
                        }
                        catch (Exception ex)
                        {
                            bool handlered = false;
                            has_error = true;
                            if (ErrorEvent != null)
                                ErrorEvent.Invoke(ex, ref handlered);
                        }

                        if (!has_error &&
                            SuccessFinishEvent != null)
                            SuccessFinishEvent.Invoke();
                    }

                    Thread.Sleep(QueryPeriod);
                }
            }
            catch (ThreadInterruptedException) { }
            catch (ThreadAbortException) { }
            catch (Exception exp)
            {
                WatcherDown(exp);
            }
            finally
            {
                State = WatcherState.OutOfWork;
                InformAboutFinish();
            }
        }

        protected virtual void ValueReceive(bool all)
        { }

        //private void CheckErrorAndInformIfNeed()
        //{
        //    try
        //    {
        //        Exception exp = null;
        //        if ((exp = manager.GetError(OperationID)) != null)
        //        {
        //            bool handlered = false;
        //            has_error = true;
        //            if (ErrorEvent != null)
        //                ErrorEvent.Invoke(exp, ref handlered);
        //        }
        //    }
        //    catch
        //    {
        //        //
        //    }
        //}

        private void InformAboutFinish()
        {
            try
            {
                if (FinishEvent != null)
                    FinishEvent();
            }
            catch
            {
                // 
            }
        }

        private void WatcherDown(Exception ex)
        {
            try
            {
                bool handlered = false;
                if (WatcherDownEvent != null)
                    WatcherDownEvent(ex, ref handlered);
            }
            catch
            {
                //
            }
        }

        public void AbortOperation()
        {
            ThreadPool.QueueUserWorkItem(s =>
            {
                StopWatchThread();

                manager.Abort(OperationID);

                State = WatcherState.OutOfWork;
                InformAboutAbort();
            });
        }

        private void StopWatchThread()
        {
            this.work = false;
            watch_thread.Interrupt();
            watch_thread.Join(exit_wait_time);

            if (watch_thread.ThreadState != ThreadState.Stopped)
                //watch_thread.Interrupt();
                watch_thread.Abort();
        }

        public void AbortWatcher()
        {
            StopWatchThread();
            State = WatcherState.OutOfWork;
        }

        private void InformAboutAbort()
        {
            try
            {
                if (StatusChangeEvent != null)
                    StatusChangeEvent("Операция прервана...", 0.0);
            }
            catch
            {
                //
            }
        }
    }

    public class AsyncOperationWatcher<RetType> : AsyncOperationWatcher
    {
        public delegate void ValueRecivedDelegate<T>(T value);
        private event ValueRecivedDelegate<RetType> ValueRecivedEvent;

        public AsyncOperationWatcher(ulong operation_id,
                                    IAsyncOperationManager manager)
            : base(operation_id, manager)
        { }

        public AsyncOperationWatcher(ulong operation_id,
                                     IAsyncOperationManager manager,
                                     TimeSpan query_period)
            : base(operation_id, manager, query_period)
        { }

        public void AddValueRecivedHandler(ValueRecivedDelegate<RetType> handler)
        { if (CanAddHandler(handler))ValueRecivedEvent += handler; }

        protected override void ValueReceive(bool all)
        {
            RetType recived_value;

            if (ValueRecivedEvent != null)
            {
                do
                {
                    object c = manager.GetResult(OperationID);
                    recived_value = (RetType)c;

                    if (recived_value != null)
                        ValueRecivedEvent.Invoke(recived_value);
                } while (all && recived_value != null);
            }
        }
    }
}

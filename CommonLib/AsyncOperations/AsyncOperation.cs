using System;
using System.Collections.Generic;
using System.Threading;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using NLog;

namespace COTES.ISTOK
{
    /// <summary>
    /// Делегат для выполнения асинхронной операции
    /// </summary>
    /// <param name="operationState">объект, в котором будут храниться текущий прогресс выполнения операции и проч.</param>
    /// <param name="parameters">Аргуметы, необходимые для выполнения операции</param>
    /// <returns>Возвращаемый результат операции</returns>
    public delegate Object AsyncDelegate(OperationState asyncContext, params Object[] parameters);

    /// <summary>
    /// Класс, отвечающий за выполнение асихронных операции
    /// </summary>
    public class AsyncOperation : IDisposable, /*COTES.ISTOK.IAsyncOperationManager,*/ COTES.ISTOK.DiagnosticsInfo.ISummaryInfo
    {
        /// <summary>
        /// Максимальная отметка прогресса
        /// </summary>
        public const double MaxProgressValue = 100f;

        /// <summary>
        /// Хранилище контекстов асинхронных операций
        /// </summary>
        private Dictionary<ulong, AsyncContext> operationDictionary = new Dictionary<ulong, AsyncContext>();

        /// <summary>
        /// Счетчик используемый для генерации идентификаторов операций
        /// </summary>
        private ulong counter = 0;

        Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Старший байт выдаваемых идентификаторов операций
        /// </summary>
        public byte IDPrefix { get; protected set; }

        public AsyncOperation() { }

        public AsyncOperation(byte prefix)
            : this()
        {
            IDPrefix = prefix;
        }

        #region Диагностика
        const string caption = "Выполняемые операции";

        public DataSet GetSummaryInfo()
        {
            DataSet ds = new DataSet();
            DataTable table = new DataTable(caption);

            table.Columns.Add("Состояние");
            table.Columns.Add("Прогресс");
            table.Columns.Add("Завершена", typeof(bool));
            table.Columns.Add("Прервана", typeof(bool));
            lock (operationDictionary)
            {
                foreach (var item in operationDictionary.Values)
                {
                    table.Rows.Add(item.State.StateString,
                        item.State.Progress.ToString(),
                        item.State.IsCompleted,
                        item.State.IsInterrupted);
                }
            }
            ds.Tables.Add(table);

            return ds;
        }
        public string GetSummaryCaption()
        {
            return caption;
        }
        #endregion

        /// <summary>
        /// Начать выполнение операции асинхронно
        /// </summary>
        /// <param name="operation">Операции</param>
        /// <param name="parameters">Параметры, требуемые для выполнения операции</param>
        /// <returns>Уникальный идентификатор операции</returns>
        public ulong BeginAsyncOperation(Guid userGUID, AsyncDelegate operation, params Object[] parameters)
        {
            return BeginAsyncOperation(userGUID, false, operation, parameters);
        }

        /// <summary>
        /// Начать выполнение операции асинхронно
        /// </summary>
        /// <param name="autoEnd">Автоматическое завершение операции по окончанию</param>
        /// <param name="operation">Операции</param>
        /// <param name="parameters">Параметры, требуемые для выполнения операции</param>
        /// <returns>Уникальный идентификатор операции</returns>
        public ulong BeginAsyncOperation(Guid userGUID, bool autoEnd, AsyncDelegate operation, params Object[] parameters)
        {
            CheckOnDisposed();

            ulong operationID;
            AsyncContext context;
            Thread asyncThread = new Thread(new ParameterizedThreadStart(runOperationInThread));

            lock (operationDictionary)
            {
                operationID = generateID();
                context = new AsyncContext(operationID, userGUID, asyncThread, autoEnd, operation, parameters);
                operationDictionary.Add(operationID, context);
            }
            asyncThread.Start(context);
            return operationID;
        }

        /// <summary>
        /// Маска для номера операции
        /// </summary>
        const ulong counterMask = 0x00FFFFFFFFFFFFFF;

        /// <summary>
        /// Количество бит в номере операции
        /// </summary>
        const int counterBitNum = 7 * 8;

        /// <summary>
        /// Сгенерировать уникальный идентификатор операции
        /// </summary>
        /// <returns>Уникальный идентификатор операции</returns>
        private ulong generateID()
        {
            ulong id, initialCount = counter, idBase = (ulong)IDPrefix << counterBitNum;
            while ((operationDictionary.ContainsKey(id = idBase + counter++) || id == 0)
                && (counter = counter & counterMask) != initialCount) ;
            if (counter == initialCount)
            {
                log.Error("Исчерпан лимит количества асинхронных операций");
                throw new Exception("Исчерпано количество асинхронных операций"); 
            }

            return id;
        }

        /// <summary>
        /// Поточная функция (которая и вызывает операцию)
        /// </summary>
        /// <param name="state"></param>
        private void runOperationInThread(Object state)
        {
            AsyncContext asyncContext = state as AsyncContext;
            if (asyncContext != null)
            {
                try
                {
                    AsyncDelegate oper = asyncContext.Operation;
                    Object retValue = oper(asyncContext.State, asyncContext.Parameters);
                    asyncContext.Complete(retValue);
                }
                catch (ThreadAbortException)
                {
                    asyncContext.State.IsInterrupted = true;
                }
                catch (ThreadInterruptedException)
                {
                    asyncContext.State.IsInterrupted = true;
                }
                catch (Exception exc)
                {
                    log.ErrorException("Ошибка в асинхронной операции.", exc);

                    exc = NonSerializedException.WrapException(exc);
                    TerminateByException(asyncContext, exc);
                    asyncContext.State.Error = exc;
                    asyncContext.State.IsInterrupted = true;
                }
                finally
                {
                    //asyncContext.State.DataCount = asyncContext.State.AsyncResult.Count;
                    asyncContext.State.IsCompleted = true;

                }
                if (asyncContext.AutoEnd)
                {
                    EndAsyncOperation(asyncContext.OperationID);
                }
            }
        }

        private void TerminateByException(AsyncContext asyncContext, Exception exc)
        {
            asyncContext.State.Error = exc;
            //if (!State.Error.Data.Contains("StackTrace"))
            //{
            //    State.Error.Data.Add("StackTrace", exc.StackTrace);
            //}
        }

        /// <summary>
        /// Ожидание завершения асинхронной операции (синхронизация)
        /// </summary>
        /// <param name="operationID">Идентификатор асинхронной операции</param>
        /// <exception cref="System.ArgumentException">Операция с указанным идентификатором не создавалась или уже удалена</exception>
        public void WaitEndOperation(ulong operationID)
        {
            WaitEndOperation(operationID, Timeout.Infinite);
        }

        /// <summary>
        /// Ожидание завершения асинхронной операции (синхронизация)
        /// </summary>
        /// <param name="operationID">Идентификатор асинхронной операции</param>
        /// <param name="timeout">Время ожидание завершения операции</param>
        /// <returns>true, если операция завершилась,
        /// false, если истекло время ожидания</returns>
        /// <exception cref="System.ArgumentException">Операция с указанным идентификатором не создавалась или уже удалена</exception>
        public bool WaitEndOperation(ulong operationID, int timeout)
        {
            var context = GetAsyncContent(operationID);

            if (context.operationThread != null
                && context.State != null
                && !context.State.IsCompleted)
            {
                return context.operationThread.Join(timeout);
        }
            return true;
        }

        /// <summary>
        /// Метод ожидает завершение выполнения операции 
        /// и удаляет операцию из списка выполняемых операций
        /// </summary>
        /// <param name="operationID">Уникальный идентификатор операции</param>
        /// <exception cref="System.Exception">Если операция завершилась ошибкой</exception>
        /// <exception cref="System.ArgumentException">Операции с таким идентификатором не существует</exception>
        public void EndAsyncOperation(ulong operationID)
        {
            AsyncContext context = GetAsyncContent(operationID);
            //context.WaitOperation();
            WaitEndOperation(operationID);
            lock (operationDictionary)
            {
                operationDictionary.Remove(operationID); 
            }
            if (context.State.Error != null)
                throw context.State.Error;
        }

        /// <summary>
        /// Получить результаты или часть возвращаемых асинхронной операцией данных
        /// </summary>
        /// <param name="operationID">Идентификатор асинхронной операции</param>
        /// <param name="next">true, если запрашивается следующая пачка данных<br />false, если необходимо повторить передачу последних передаваемых данных</param>
        /// <returns>null, если данные ещё не готовы или уже все переданны<br />
        /// В противном случае очередную порцию возвращаемых результатов операции</returns>
        /// <exception cref="System.ArgumentException">Операция с указанным идентификатором не создавалась или уже удалена</exception>
        public Object GetAsyncOperationData(ulong operationID, bool next)
        {
            AsyncContext context = GetAsyncContent(operationID);
            if (context.State.IsCompleted || context.State.AllowStartAsyncResult)
            {
                return context.State.GetNextAsyncResult(next);
            }
            else 
                return null;
        }

        /// <summary>
        /// Получить сообщения сформированные в процессе выполнения операции
        /// </summary>
        /// <param name="asyncResult">Идентификатор асинхронной операции</param>
        /// <param name="getNext">true, если запрашивается следующая пачка сообщений<br />false, если необходимо повторить передачу последних передаваемых сообщений</param>
        /// <returns>null, если сообщений нет или уже все были переданны ранее<br />
        /// В противном случае очередную порцию возвращаемых результатов операции</returns>
        /// <exception cref="System.ArgumentException">Операция с указанным идентификатором не создавалась или уже удалена</exception>
        public Message[] GetAsyncOperationMessages(ulong operationID, bool next)
        {
            return GetAsyncContent(operationID).State.GetNextAsyncMessage(next);
        }

        /// <summary>
        /// Время ожидания завершения потока при прерывании операции
        /// </summary>
        private TimeSpan interuptDelay = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Прервать выполнение асинхронной операции
        /// </summary>
        /// <param name="operationID">Уникальный идентификатор операции</param>
        /// <exception cref="System.ArgumentException">Операции с таким идентификатором не существует</exception>
        public void InteruptAsyncOperation(ulong operationID)
        {
            //const int wait_time = 300; //ms

            AsyncContext context = GetAsyncContent(operationID);
            context.State.IsInterrupted = true;
            if (context.operationThread != null)
            {
                context.operationThread.Interrupt();
                context.operationThread.Join(interuptDelay);
            }
            //Thread.Sleep(wait_time);
            if (!context.State.IsCompleted)
            {
                context.operationThread.Abort();
            }
            lock (operationDictionary)
            {
                operationDictionary.Remove(operationID); 
            }
        }

        /// <summary>
        /// Получить текущие состояние выполнения асинхронной операции
        /// </summary>
        /// <param name="operationID">Уникальный идентификатор операции</param>
        /// <returns>Состояние</returns>
        /// <exception cref="System.ArgumentException">Операции с таким идентификатором не существует</exception>
        public OperationInfo GetAsyncOperationState(ulong operationID)
        {
            return GetAsyncContent(operationID).State.GetInfo();
        }

        private AsyncContext GetAsyncContent(ulong operationID)
        {
            CheckOnDisposed();

            AsyncContext result;
            lock (operationDictionary)
            {
                if (!operationDictionary.TryGetValue(operationID, out result))
                {
                    log.Error("Обращение к несуществующей асинхронной операции.");
                    throw new ArgumentException("Нет такой транзакции");
                } 
            }

            return result;
        }

        private void CheckOnDisposed()
        {
            if (disposed)
            {
                log.Error("Рассогласованное состояние приложения");
                throw new ObjectDisposedException("Менеджер асинхронных операций был освобожден.");
            }
        }

        //#region IAsyncOperationManager Members

        //bool COTES.ISTOK.IAsyncOperationManager.IsComplete(ulong id)
        //{
        //    return GetAsyncOperationState(id).IsCompleted;
        //}

        //bool COTES.ISTOK.IAsyncOperationManager.IsInterrupted(ulong id)
        //{
        //    return GetAsyncOperationState(id).IsInterrupted;
        //}

        //object COTES.ISTOK.IAsyncOperationManager.GetResult(ulong id)
        //{
        //    return GetAsyncOperationData(id, true);
        //}

        //Message[] COTES.ISTOK.IAsyncOperationManager.GetMessages(ulong id)
        //{
        //    return GetAsyncOperationMessages(id, true);
        //}

        //Exception COTES.ISTOK.IAsyncOperationManager.GetError(ulong id)
        //{
        //    return null;
        //}

        //double COTES.ISTOK.IAsyncOperationManager.GetProgress(ulong id)
        //{
        //    return GetAsyncOperationState(id).Progress;
        //}

        //string COTES.ISTOK.IAsyncOperationManager.GetStatus(ulong id)
        //{
        //    return GetAsyncOperationState(id).StateString;
        //}

        //void COTES.ISTOK.IAsyncOperationManager.End(ulong id)
        //{
        //    EndAsyncOperation(id);
        //}

        //bool COTES.ISTOK.IAsyncOperationManager.Abort(ulong id)
        //{
        //    InteruptAsyncOperation(id);
        //    return true;
        //}

        //#endregion

        #region IDisposable Members

        bool disposed = false;
        public void Dispose()
        {
            disposed = true;
            lock (operationDictionary)
            {
                foreach (ulong operationID in operationDictionary.Keys)
                {
                    AsyncContext context = operationDictionary[operationID];
                    context.operationThread.Abort();
                }
                operationDictionary = null; 
            }
        }

        #endregion
    }

    [global::System.Serializable]
    public class NonSerializedException : Exception, System.Runtime.Serialization.ISerializable
    {
        public String OriginalExceptionName { get; private set; }


        //public NonSerializedException() { }
        //public NonSerializedException(string message) : base(message) { }
        //public NonSerializedException(string message, Exception inner) : base(message, inner) { }
        protected NonSerializedException(Exception exc)
            : base(exc.Message, WrapException(exc.InnerException))
        {
            this.HelpLink = exc.HelpLink;
            this.Source = exc.Source;
            this.stackTrace = exc.StackTrace;
            OriginalExceptionName = exc.GetType().FullName;
        }

        protected NonSerializedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            stackTrace = info.GetString("serializedstackTrace");
            OriginalExceptionName = info.GetString("OriginalExceptionName");
        }

        #region ISerializable Members

        void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("serializedstackTrace", stackTrace);
            info.AddValue("OriginalExceptionName", OriginalExceptionName);
        }

        #endregion

        private string stackTrace;

        public override string StackTrace
        {
            get
            {
                return stackTrace;
            }
        }

#if DEBUG
        public override string Message
        {
            get
            {
                return String.Format("Несереализуемое исключение {0}.\nСообщение: {1}\nSource: {2}\nStackTrace: {3}", OriginalExceptionName, base.Message, Source, StackTrace);
            }
        }
#endif

        public static Exception WrapException(Exception exc)
        {
            if (exc != null && !exc.GetType().IsSerializable)
            {
                exc = new NonSerializedException(exc);
            }
            return exc;
        }
    }
}

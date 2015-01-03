using System;
using System.Collections.Generic;

namespace COTES.ISTOK
{
    /// <summary>
    /// Состояние выполнения операции
    /// </summary>
    public class OperationState
    {
        public Guid UserGUID { get; private set; }

        /// <summary>
        /// Завершилась ли операция
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Состояние операции, определенное пользователем
        /// </summary>
        public string StateString { get; set; }

        /// <summary>
        /// Текущий прогресс выполнения операции
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// Прервана ли операция
        /// </summary>
        public bool IsInterrupted { get; set; }

        /// <summary>
        /// Ошибка с которой завершилась операция.
        /// </summary>
        public Exception Error { get; set; }
        /// <summary>
        /// Сообщения полученные в ходе выполнения операции
        /// </summary>
        public List<Message> messages { get; protected set; }

        /// <summary>
        /// Результаты операции, накопленные в ходе её выполнения
        /// </summary>
        public List<Object> AsyncResult { get; protected set; }

        /// <summary>
        /// Позволить забирать асинхронные результаты до завершения операции
        /// </summary>
        public bool AllowStartAsyncResult { get; set; }

        /// <summary>
        /// Индекс последнего переданного объекта из AsyncResult по запросу
        /// </summary>
        private int lastReturnedResult = -1;

        /// <summary>
        /// Ограничение размера сообщений передаваемый за один раз
        /// </summary>
        private int messageReceiveByteLimit = 64 * 1024;

        /// <summary>
        /// Индекс и длина последней пердаваемой пачки сообщений по запросу
        /// </summary>
        private int lastReturnedMessageStart, lastReturnedMessageLength;

        public OperationState(Guid userGUID)
        {
            UserGUID = userGUID;
            messages = new List<Message>();
            AsyncResult = new List<Object>();
            AllowStartAsyncResult = false;
        }

        public OperationState()
            : this(Guid.Empty)
        {

        }

        /// <summary>
        /// Получить информацию
        /// </summary>
        /// <returns>Информация</returns>
        public OperationInfo GetInfo()
        {
            return new OperationInfo()
            {
                IsCompleted = IsCompleted,
                StateString = StateString,
                Progress = Progress,
                IsInterrupted = IsInterrupted,
                //Error = Error,
                DataCount = AsyncResult.Count
            };
        }

        public OperationState Sub()
        {
            return new OperationState(UserGUID) { };
        }

        /// <summary>
        /// Добавить объект для асинхронного возвращения результатов
        /// </summary>
        /// <param name="result">Фрагмент возвращаемого результата</param>
        public void AddAsyncResult(Object result)
        {
            lock (AsyncResult)
            {
                if (result != null)
                {
                    AsyncResult.Add(result);
                }
            }
        }

        /// <summary>
        /// Добавить новое сообщение в список сообщений
        /// </summary>
        /// <param name="exc">Сообщение</param>
        public void AddMessage(Message message)
        {
            lock (messages)
            {
                messages.Add(message);
            }
        }

        /// <summary>
        /// Добавить несколько новых сообщений в список сообщений
        /// </summary>
        /// <param name="messageArray">Массив сообщений</param>
        public void AddMessage(IEnumerable<Message> messageArray)
        {
            lock (messages)
            {
                messages.AddRange(messageArray);
            }
        }

        /// <summary>
        /// Получить следующий фрагмент асинхронно-возвращаемых результатов
        /// </summary>
        /// <param name="next">Запросить следующую порцию результатов или повторить последний запрос</param>
        /// <returns>Очередная порция результатов операции</returns>
        public Object GetNextAsyncResult(bool next)
        {
            int index = lastReturnedResult;
            lock (AsyncResult)
            {
                if (next) ++index;
                if (index < 0) index = 0;
                if (AsyncResult != null && AsyncResult.Count > index)
                {
                    return AsyncResult[lastReturnedResult = index];
                }
            }
            return null;
        }

        public IEnumerable<Object> GetAsyncResult()
        {
            if (AsyncResult != null)
                return AsyncResult.ToArray();
            return null;
        }

        public IEnumerable<T> GetAsyncResult<T>()
        {
            foreach (var item in AsyncResult)
                if (item is T) yield return (T)item;
                else
                    if (item is T[])
                        foreach (var it in (T[])item) yield return it;
        }

        /// <summary>
        /// Получить следующую порцию сообщений от операции
        /// </summary>
        /// <param name="next">Запросить следующую порцию сообщений или повторить последний запрос</param>
        /// <returns>Очередная порция сообщений</returns>
        public Message[] GetNextAsyncMessage(bool next)
        {
            lock (messages)
            {
                if (next)
                {
                    int bytes = 0;
                    for (lastReturnedMessageStart += lastReturnedMessageLength, lastReturnedMessageLength = 0;
                        bytes < messageReceiveByteLimit && lastReturnedMessageStart + lastReturnedMessageLength < messages.Count;
                        lastReturnedMessageLength++)
                    {
                        Message mess = messages[lastReturnedMessageStart + lastReturnedMessageLength];
                        bytes += mess.Text.Length * sizeof(char);
                    }
                    if (bytes > messageReceiveByteLimit)
                    {
                        if (lastReturnedMessageLength > 1)
                            --lastReturnedMessageLength;
                        else
                        {
                            Message mess = messages[lastReturnedMessageStart + lastReturnedMessageLength - 1];
                            mess.Text = mess.Text.Substring(0, messageReceiveByteLimit / sizeof(char));
                        }
                        //break;
                    }
                }
                Message[] ret = null;
                if (lastReturnedMessageLength > 0)
                {
                    ret = new Message[lastReturnedMessageLength];
                    messages.CopyTo(lastReturnedMessageStart, ret, 0, lastReturnedMessageLength);
                }
                return ret;
            }
        }
    }
}

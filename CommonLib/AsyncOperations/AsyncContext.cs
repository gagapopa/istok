using System;
using System.Threading;

namespace COTES.ISTOK
{
    /// <summary>
    /// Контекст асинхронных опреаций
    /// </summary>
    class AsyncContext
    {
        /// <summary>
        /// Поток, в котором выполняется асинхронная операция
        /// </summary>
        public Thread operationThread { get; private set; }

        /// <summary>
        /// Создать контекст для новой асинхронной операции
        /// </summary>
        /// <param name="thread">Поток операции</param>
        /// <param name="autoEnd">Автоматическое завершение операции по окончанию</param>
        /// <param name="asyncOperation">Операция</param>
        /// <param name="parameters">Параметры, необходимые для выполнения операции</param>
        public AsyncContext(ulong operationID, Guid userGUID, Thread thread, bool autoEnd, AsyncDelegate asyncOperation, Object[] parameters)
        {
            OperationID = operationID;
            operationThread = thread;
            AutoEnd = autoEnd;
            Parameters = parameters;
            Operation = asyncOperation;
            State = new OperationState(userGUID);
        }

        /// <summary>
        /// Идентификатор операции
        /// </summary>
        public ulong OperationID { get; protected set; }

        /// <summary>
        /// Операция
        /// </summary>
        public AsyncDelegate Operation { get; protected set; }

        /// <summary>
        /// Параметры, необходимые для выполнения операции
        /// </summary>
        public Object[] Parameters { get; protected set; }

        /// <summary>
        /// Автоматическое завершение операции по окончанию
        /// </summary>
        public bool AutoEnd { get; protected set; }

        /// <summary>
        /// Состояние выполнения операции
        /// </summary>
        public OperationState State { get; protected set; }

        public Guid UserGUID { get { return State.UserGUID; } }

        /// <summary>
        /// Завершить выполнение операции
        /// </summary>
        /// <param name="result">Результат выполнения операции</param>
        public void Complete(Object result)
        {
            if (result != null)
            {
                State.AddAsyncResult(result);
            }
        }
    }
}

using System;
using System.Runtime.Serialization;
namespace COTES.ISTOK
{
    /// <summary>
    /// Состояние выполнения операции
    /// </summary>
    [Serializable]
    [DataContract]
    public class OperationInfo
    {
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
        /// Количество возвращаемых, в ходе выполнения операции, порций результатов
        /// </summary>
        public int DataCount { get; set; }

        public OperationInfo() { }

        public OperationInfo(OperationInfo opInfo)
        {
            this.IsInterrupted = opInfo.IsInterrupted;
            this.IsCompleted = opInfo.IsCompleted;
            this.Progress = opInfo.Progress;
            this.StateString = opInfo.StateString;
            this.DataCount = opInfo.DataCount;
            //this.Error = opInfo.Error;
        }
    }
}

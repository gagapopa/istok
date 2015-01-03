using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Block.Redundancy
{
    /// <summary>
    /// Предоставляет обмен данными между участниками дублирования
    /// </summary>
    [Serializable]
    public class RedundancyCommand
    {
        public uint Id { get; protected set; }
        public RedundancyCommandType CommandType { get; protected set; }
        public DateTime TimeStamp { get; protected set; }
        public object Data { get; protected set; }

        public RedundancyCommand(uint id, RedundancyCommandType type, object data, DateTime timestamp)
        {
            Id = id;
            CommandType = type;
            Data = data;
            TimeStamp = timestamp;
        }
    }

    /// <summary>
    /// Перечисление типов команд
    /// </summary>
    public enum RedundancyCommandType
    {
        /// <summary>
        /// Сохранение пакета в БД
        /// </summary>
        FlushPackage,
        /// <summary>
        /// Передача всей справочной информации
        /// </summary>
        ReceiveDictionaryAll,
        /// <summary>
        /// Передача справочной информации одного канала
        /// </summary>
        ReceiveDictionaryChannel
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ParameterReceiverExtension
{
    /// <summary>
    /// Свойства параметра
    /// </summary>
    [Serializable]
    public class Parameter
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public int Id { get; set; }

        public int ExternalID { get; set; }

        /// <summary>
        /// Уникальное строковое обозначение
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Код качества
        /// </summary>
        public int Quality { get; set; }

        /// <summary>
        /// Время
        /// </summary>
        public DateTime Time { get; set; }
    }
}

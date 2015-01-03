using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK
{
    /// <summary>
    /// Метод сбора данных
    /// </summary>
    [System.ComponentModel.TypeConverter(typeof(DataLoadMethodTypeConverter))]
    public enum DataLoadMethod
    {
        /// <summary>
        /// Сбор текущих данных
        /// </summary>
        Current,
        /// <summary>
        /// Получение данных по подписке
        /// </summary>
        Subscribe,
        /// <summary>
        /// Сбор архивных данных с общим трендом
        /// </summary>
        Archive,
        /// <summary>
        /// Сбор архивных данных отдельно для каждого собираемого параметра
        /// </summary>
        ArchiveByParameter
    }
}

using System;
using System.Collections.Generic;
using System.Data;

namespace COTES.ISTOK.Extension
{
    /// <summary>
    /// Тип передаваемых данных.
    /// Говорит каким образом следует отображать данные.
    /// </summary>
    public enum ExtensionDataType
    {
        /// <summary>
        /// Передается график.
        /// Первая колонка является абциссой, 
        /// остальные ординаты разных линий
        /// </summary>
        Graph,

        /// <summary>
        /// Гистограмма
        /// </summary>
        Histogram,

        /// <summary>
        /// Таблица
        /// </summary>
        Table
    }
}
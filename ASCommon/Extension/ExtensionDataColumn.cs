using System;
using System.Collections.Generic;
using System.Data;

namespace COTES.ISTOK.Extension
{
    /// <summary>
    /// Информация о колонке в данных из расширения
    /// </summary>
    [Serializable]
    public class ExtensionDataColumn
    {
        /// <summary>
        /// Наименование колонки
        /// </summary>
        public String Name { get;protected set; }

        /// <summary>
        /// Загаловок колонки
        /// </summary>
        public String Caption { get; set; }

        /// <summary>
        /// Единица измерения значений
        /// </summary>
        public String Unit { get;  set; }

        /// <summary>
        /// Тип данных значений
        /// </summary>
        public Type ValueType { get; protected set; }

        public ExtensionDataColumn(String name,Type valueType)
        {
            this.Name = name;
            this.ValueType = valueType;
        }
    }
}
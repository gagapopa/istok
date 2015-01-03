using System;
using System.Collections.Generic;
using System.Data;

namespace COTES.ISTOK.Extension
{
    /// <summary>
    /// Информация о тренде для данных из расширения.
    /// </summary>
    /// <remarks>
    /// Здесь, обысно, идет речь о линиях на графике. 
    /// Информация какая колонка является x а какая y.
    /// </remarks>
    [Serializable]
    public class ExtensionDataTrend
    {
        /// <summary>
        /// Имя тренда
        /// </summary>
        public String Name { get; protected set; }

        /// <summary>
        /// Заголовок тренда
        /// </summary>
        public String Caption { get; set; }

        /// <summary>
        /// Краткое описание
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Фильтр для DataTable, если только часть данных относяться к данному трейду
        /// </summary>
        public String FilterString { get; protected set; }

        /// <summary>
        /// Колонки из DataTable, имеющие отношение к данному трейду.
        /// </summary>
        /// <remarks>
        /// Предпологается, что первая колонка это x, а вторая y.
        /// </remarks>
        public ExtensionDataColumn[] Columns { get; protected set; }

        public ExtensionDataTrend(String name, String filterString,ExtensionDataColumn[] columns)
        {
            this.Name = name;
            this.Caption = name;
            this.FilterString = filterString;
            this.Columns = columns;
        }

        public ExtensionDataTrend(ExtensionDataTrend info)
            : this(info.Name, info.FilterString, info.Columns)
        {
            this.Caption = info.Caption;
            this.Description = info.Description;
            this.FilterString = info.FilterString;
        }
    }
}
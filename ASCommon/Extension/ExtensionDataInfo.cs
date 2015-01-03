using System;
using System.Collections.Generic;
using System.Data;

namespace COTES.ISTOK.Extension
{
    /// <summary>
    /// Метаданные для ExtensionData.
    /// </summary>
    [Serializable]
    public class ExtensionDataInfo
    {
        /// <summary>
        /// Информация о расширении, у которого запрашиваются данные
        /// </summary>
        public ExtensionInfo ExtensionInfo { get; set; }

        /// <summary>
        /// Служебное название данных
        /// </summary>
        public String Name { get; protected set; }

        /// <summary>
        /// Загаловок данных
        /// </summary>
        public String Caption { get; set; }

        /// <summary>
        /// Возвращает, являются ли данные не зависящими от узлов
        /// </summary>
        public bool IsCommon { get; set; }

        /// <summary>
        /// Тип данных для отображения
        /// </summary>
        public ExtensionDataType Type { get; protected set; }

        /// <summary>
        /// Для гистограммы, является ли она горизонтальной или вертикальной
        /// </summary>
        public bool Horizontal { get; set; }

        /// <summary>
        /// Получить список трендов для данных
        /// </summary>
        public List<ExtensionDataTrend> Trends { get; protected set; }

        private List<ExtensionDataColumn> commonColumns;

        /// <summary>
        /// Колонки общие для всех трэндов, или не относящиеся ни к одному из них
        /// </summary>
        public List<ExtensionDataColumn> CommonColumns
        {
            get
            {
                if (commonColumns == null)
                    commonColumns = new List<ExtensionDataColumn>();
                return commonColumns;
            }
        }

        /// <summary>
        /// Все колонки таблицы данных
        /// </summary>
        public ExtensionDataColumn[] Columns
        {
            get
            {
                if (Trends == null)
                    return null;

                List<ExtensionDataColumn> columnsList = new List<ExtensionDataColumn>();
                if (commonColumns!=null)
                    columnsList.AddRange(commonColumns);

                foreach (var item in Trends)
                {
                    if (item.Columns != null)
                        foreach (var column in item.Columns)
                        {
                            if (!columnsList.Contains(column))
                                columnsList.Add(column);
                        }
                }
                return columnsList.ToArray();
            }
        }

        /// <summary>
        /// Получить информацию о колонки из таблицы по её названию
        /// </summary>
        /// <param name="lineName"></param>
        /// <returns></returns>
        public ExtensionDataTrend this[String lineName] { get { return Trends.Find(c => c.Name.Equals(lineName)); } }

        public ExtensionDataInfo(String name, ExtensionDataType type)
        {
            this.Name = name;
            this.Type = type;
            Trends = new List<ExtensionDataTrend>();
            this.Caption = name;
        }

        public ExtensionDataInfo(String name, ExtensionDataType type, String caption)
            : this(name, type)
        {
            this.Caption = caption;
        }

        public ExtensionDataInfo(ExtensionDataInfo info)
            : this(info.Name, info.Type, info.Caption)
        {
            if (info.commonColumns != null)
                commonColumns = new List<ExtensionDataColumn>(info.commonColumns);

            foreach (var item in info.Trends)
            {
                Trends.Add(new ExtensionDataTrend(item));
            }
        }
    }
}
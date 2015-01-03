using System;
using System.Collections.Generic;
using System.Data;

namespace COTES.ISTOK.Extension
{
    /// <summary>
    /// Данные из расширения
    /// </summary>
    /// <remarks>
    /// По сути являются обёрткой DataTable 
    /// с дополнительной информацией о данных и их отображении
    /// </remarks>
    [Serializable]
    public class ExtensionData
    {
        /// <summary>
        /// Дополнительная метаинформация о струтуре передаваемых данных
        /// </summary>
        public ExtensionDataInfo Info { get; protected set; }

        /// <summary>
        /// Передаваемые данные в виде DataTable
        /// </summary>
        public DataTable Table { get; protected set; }

        public DataView this[String columnName]
        {
            get
            {
                ExtensionDataTrend columnInfo = Info[columnName];
                if (Table != null && columnInfo != null)
                {
                    return new DataView(Table, columnInfo.FilterString, String.Empty, DataViewRowState.CurrentRows);
                }
                return null;
            }
        }

        public ExtensionData(ExtensionDataInfo info)
        {
            this.Info = info;
            Table = CreateTable(info);
        }

        public ExtensionData(ExtensionDataInfo info, DataTable sourceTable)
        {
            this.Info = info;
            Table = sourceTable;
        }

        /// <summary>
        /// Создать DataTable по метаданным из ExtendedTableInfo
        /// </summary>
        /// <param name="info">Метаданные</param>
        /// <returns></returns>
        private DataTable CreateTable(ExtensionDataInfo info)
        {
            DataTable table = new DataTable();

            foreach (var item in info.Trends)
            {
                foreach (var column in item.Columns)
                {
                    DataColumn dataColumn = table.Columns.Add(column.Name, column.ValueType);
                    dataColumn.Caption = column.Caption;
                }
            }

            return table;
        }
    }
}
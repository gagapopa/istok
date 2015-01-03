using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Информация о источнике данных отчёта
    /// </summary>
    [Serializable]
    public sealed class ReportSourceInfo
    {
        /// <summary>
        /// ИД источника данных
        /// </summary>
        public Guid ReportSourceId { get; private set; }

        /// <summary>
        /// Название источника данных
        /// </summary>
        public String Caption { get; private set; }

        /// <summary>
        /// Источник данных может быть выбран вручную
        /// </summary>
        public bool Visible { get; private set; }

        public ReportSourceInfo(Guid sourceId, bool visible, String caption)
        {
            this.ReportSourceId = sourceId;
            this.Visible = visible;
            this.Caption = caption;
        }
    }
}

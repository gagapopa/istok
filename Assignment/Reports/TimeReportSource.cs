using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Источник данных для отчёта.
    /// Предоставляет параметры отчёта для определения отчётного периода
    /// </summary>
    class TimeReportSource : IReportSource
    {
        ReportSourceInfo info;
        internal static readonly Guid TimeReportSourceID = new Guid("{B09BCB02-E5DA-4f65-BCB2-819FA8A1AEA4}");

        public TimeReportSource()
        {
            info = new ReportSourceInfo(TimeReportSourceID, false, "Отчетный период");
        }

        #region IReportSource Members

        public ReportSourceInfo Info
        {
            get { return info; }
        }

        public Guid[] References
        {
            get { return null; }
        }

        public void SetReference(IReportSource source)
        { }

        public ReportSourceSettings CreateSettingsObject()
        {
            return new TimeReportSourceSetting(info);
        }

        public void GenerateData(OperationState state, DataSet dataSet, ReportSourceSettings settings, params ReportParameter[] reportParameters)
        {
        }

        public void GenerateEmptyData(OperationState state, DataSet dataSet, ReportSourceSettings settings)
        {
        }
        #endregion
    }
}

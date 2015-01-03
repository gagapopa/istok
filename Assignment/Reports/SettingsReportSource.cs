using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using System.Data;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Источник данных для передачи 
    /// </summary>
    class SettingsReportSource : IReportSource
    {
        ReportSourceInfo info;
        static readonly Guid SettingsReportSourceID = new Guid("{314AA70A-8ED4-4a11-A6EB-0EA53E39CC85}");

        public SettingsReportSource()
        {
            info = new ReportSourceInfo(SettingsReportSourceID, true, "Настройки системы");
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
        {
            throw new NotImplementedException();
        }

        public ReportSourceSettings CreateSettingsObject()
        {
            return new SimpleReportSourceSettings(info);
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

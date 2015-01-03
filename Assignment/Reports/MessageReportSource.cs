using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using System.Data;
using COTES.ISTOK.Assignment.Extension;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Источник данных отчёта сообщений
    /// </summary>
    class MessageReportSource : IReportSource
    {
        ExtensionManager extensionManager;

        TimeReportSource timeReportSource;

        ReportSourceInfo info;
        static readonly Guid SettingsReportSourceID = new Guid("{B1B9748F-EC4A-4398-8083-B2AF8F5F8D6B}");

        public MessageReportSource(ExtensionManager extensionManager)
        {
            info = new ReportSourceInfo(SettingsReportSourceID, true, "Сообщений");
            //this.logger = logger;
            this.extensionManager = extensionManager;
        }

        #region IReportSource Members

        public ReportSourceInfo Info
        {
            get { return info; }
        }

        public Guid[] References
        {
            get { return new Guid[] { TimeReportSource.TimeReportSourceID }; }
        }

        public void SetReference(IReportSource source)
        {
            TimeReportSource valueReportSource=source as TimeReportSource;

            if (valueReportSource!=null)
            {
                this.timeReportSource = valueReportSource;
            }
        }

        public ReportSourceSettings CreateSettingsObject()
        {
            SimpleReportSourceSettings settings= new SimpleReportSourceSettings(info);
            settings.SetReferences(References);
            return settings;
        }

        public void GenerateData(OperationState state, DataSet dataSet, ReportSourceSettings settings, params ReportParameter[] reportParameters)
        {
            TimeReportSourceSetting timeReportSettings = settings.GetReference(timeReportSource.Info.ReportSourceId) as TimeReportSourceSetting;

            DateTime startTime, endTime;

            timeReportSettings.GetReportInterval(reportParameters, out startTime, out endTime);

            DataTable table = extensionManager.GetMessages(MessageCategory.Warning, startTime, endTime);
            if (table != null)
            {
                table.TableName = info.Caption;
                dataSet.Tables.Add(table);
            }
        }

        public void GenerateEmptyData(OperationState state, DataSet dataSet, ReportSourceSettings settings)
        {            
        }

        #endregion
    }
}

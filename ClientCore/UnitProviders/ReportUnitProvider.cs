using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using System.IO;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    /// <summary>
    /// Провайдер для формирования отчётов
    /// </summary>
    public class ReportUnitProvider : UnitProvider
    {
        public ReportUnitProvider(StructureProvider strucProvider, ReportNode reportNode)
            : base(strucProvider, reportNode)
        {
            ObtainProperties(reportNode);
        }

        private void ObtainProperties(ReportNode reportNode)
        {
            Guid[] guids = reportNode.GetReportSourcesGuids();
            List<ReportParameter> reportParameters = new List<ReportParameter>();

            if (guids != null)
                foreach (var sourceGuid in guids)
                {
                    ReportSourceSettings settings = reportNode.GetReportSetting(sourceGuid);
                    if (settings != null && settings.Enabled)
                    {
                        ReportParameter[] properties = settings.GetReportParameters();
                        if (properties != null)
                            reportParameters.AddRange(properties);
                    }
                }
            if (PropertiesContainer == null)
                PropertiesContainer = new ReportParametersContainer();
            PropertiesContainer.SetProperties(reportParameters);
            SaveInSystem = true;
        }

        public override void CommitSaving()
        {
            //throw new NotImplementedException("CommitSaving");
            base.CommitSaving();
            ReportNode reportNode = UnitNode as ReportNode;
            if (reportNode != null)
                ObtainProperties(reportNode);
        }

        /// <summary>
        /// Контейнер со с параметрами отчёта
        /// </summary>
        public ReportParametersContainer PropertiesContainer { get; private set; }

        /// <summary>
        /// Сформировать отчёт
        /// </summary>
        public void GenerateReport()
        {
            //throw new NotImplementedException("GenerateReport");
            byte[] bs = null;
            bs = strucProvider.RDS.ReportDataService.GenerateFastReport(UnitNode as ReportNode, SaveInSystem, PropertiesContainer.Parameters.ToArray());
            OnReportGenerated(bs);
            //AsyncOperationWatcher<byte[]> watcher = rds.BeginGenerateReport(UnitNode as ReportNode, SaveInSystem, PropertiesContainer.Parameters.ToArray());
            //watcher.AddValueRecivedHandler(b => bs = b);
            //watcher.AddSuccessFinishHandler(() => OnReportGenerated(bs));
            //UniForm.RunWatcher(watcher);
        }

        /// <summary>
        /// Событие возникающие при окончании формирования отчёта
        /// </summary>
        public event EventHandler<ReportEventArgs> ReportGenerated;

        /// <summary>
        /// Вызвать уведомление о формировании отчёта
        /// </summary>
        /// <param name="reportBody">тело отчёта, полученное с сервера</param>
        private void OnReportGenerated(byte[] reportBody)
        {
            //throw new NotImplementedException("OnReportGenerated");
            FastReport.Report frx = new FastReport.Report();

            if (reportBody != null && reportBody.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(reportBody))
                {
                    ms.Position = 0;
                    frx.LoadPrepared(ms);
                }
            }
            if (ReportGenerated != null)
                ReportGenerated(this, new ReportEventArgs(frx));
        }

        #region Настройки компонента
        /// <summary>
        /// Записать ли сформированный отчёт на сервере
        /// </summary>
        public bool SaveInSystem { get; set; }
        #endregion
    }

    /// <summary>
    /// Аргументы события, передающие FastReport-отчёт 
    /// </summary>
    public class ReportEventArgs : EventArgs
    {
        /// <summary>
        /// Отчёт
        /// </summary>
        public FastReport.Report Report { get; private set; }

        public ReportEventArgs(FastReport.Report report)
        {
            this.Report = report;
        }
    }
}

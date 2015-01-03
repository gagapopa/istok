using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastReport;
using COTES.ISTOK.ASC;
using System.Data;
using System.IO;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Обертка отчёта FastReport для передачи по сети совместно с телом отчёта данные для него
    /// </summary>
    [Serializable]
    public class FastReportWrap
    {
        /// <summary>
        /// Имя отчёта
        /// </summary>
        public String ReportName { get; set; }

        /// <summary>
        /// Сереализованное тело отчёта 
        /// </summary>
        public byte[] ReportBody { get; set; }

        /// <summary>
        /// Имена источников данных для текущего отчёта
        /// </summary>
        public String[] DataSources
        {
            get
            {
                List<String> dataSourcesList = new List<string>();
                foreach (var item in reportSourceData.Values)
                {
                    foreach (DataTable table in item.Tables)
                    {
                        dataSourcesList.Add(table.TableName);
                    }
                }
                return dataSourcesList.ToArray();
            }
        }

        /// <summary>
        /// Тело отчёта
        /// </summary>
        [NonSerialized]
        Report report;

        /// <summary>
        /// Установить или получить параметры отчёта
        /// </summary>
        public ReportParameter[] ReportParameters { get; set; }

        Dictionary<Guid, DataSet> reportSourceData = new Dictionary<Guid, DataSet>();

        //public Report Frx
        //{
        //    get
        //    {
        //        if (report == null)
        //        {
        //            report = new Report();
        //            report.FileName = ReportName;
        //            report.AutoFillDataSet = true;

        //            if (ReportBody != null && ReportBody.Length > 0)
        //            {
        //                using (MemoryStream ms = new MemoryStream(ReportBody))
        //                {
        //                    report.Load(ms);
        //                }
        //            }
        //            SetData();
        //        }
        //        return report;
        //    }
        //}

        /// <summary>
        /// Обновить источники данных для отчёта.
        /// </summary>
        /// <param name="frxReport">Тело отчёта, если null то будет созданно новый отчёт из ReportBody</param>
        /// <returns></returns>
        public Report UpdateReport(Report frxReport)
        {
            report = frxReport;
            if (report == null)
            {
                report = new Report();
                report.FileName = ReportName;
                //report.AutoFillDataSet = true;

                if (ReportBody != null && ReportBody.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(ReportBody))
                    {
                        report.Load(ms);
                    }
                }
                //SetData();
            }
            SetData();
            return report;
        }

        //public FastReportWrap(String name)
        //{
        //    this.reportName = name;
        //    //this.reportBody = body;
        //}

        /// <summary>
        /// Зарегистрировать исходные данные отчёта
        /// </summary>
        /// <param name="guid">ИД источника данных</param>
        /// <param name="dataSet">Данные источника данных</param>
        public void RegisterData(Guid guid, DataSet dataSet)
        {
            reportSourceData[guid] = dataSet;

            SetData();
        }

        /// <summary>
        /// Обновить исходные данные у тела отчета
        /// </summary>
        private void SetData()
        {
            if (report != null)
            {
                // передаём отчёту значения параметров
                if (ReportParameters != null)
                {
                    foreach (var reportProperty in ReportParameters)
                    {
                        Object value;
                        if (reportProperty.RequiredString && !String.IsNullOrEmpty(reportProperty.StringValue))
                        {
                            value = reportProperty.StringValue;
                        }
                        else
                        {
                            value = reportProperty.GetValue();
                        }

                        report.SetParameterValue(reportProperty.Name, value);
                    }
                }
                // передаем отчёту данные
                foreach (var item in reportSourceData.Keys)
                {
                    report.RegisterData(reportSourceData[item], item.ToString());
                }
            }
        }

        /// <summary>
        /// Сохранить тело отчёта
        /// </summary>
        /// <returns></returns>
        public byte[] Save()
        {
            if (report != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    report.Save(ms);
                    return ms.ToArray();
                }
            }
            return null;
        }
    }
}

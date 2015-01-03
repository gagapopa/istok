using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Информация о готовом отчёте
    /// </summary>
    [Serializable]
    public class PreferedReportInfo
    {
        /// <summary>
        /// ИД в системе
        /// </summary>
        public int Idnum { get; private set; }

        /// <summary>
        /// ИД пользователя, сформировавшего отчёт
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// ИД узла отчёта
        /// </summary>
        public int ReportId { get; private set; }

        /// <summary>
        /// Дата формирования отчёта
        /// </summary>
        public DateTime CreationDate { get; private set; }

        /// <summary>
        /// Параметры отчёта при формировании
        /// </summary>
        public ReportParameter[] ReportParameters { get; private set; }

        public PreferedReportInfo(int idnum, int userId, int reportId, DateTime creationDate, ReportParameter[] parameters)
        {
            Idnum = idnum;
            UserId = userId;
            ReportId = reportId;
            CreationDate = creationDate;
            ReportParameters = parameters;
        }
    }
}

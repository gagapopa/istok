using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Настройки источника данных для запроса значений
    /// </summary>
    [DataContract]
    public class TimeReportSourceSetting : ReportSourceSettings
    {
        public TimeReportSourceSetting(ReportSourceInfo info)
            : base(info)
        {
            ReportInterval = ReportTimeInterval.Custom;
        }

        public TimeReportSourceSetting(TimeReportSourceSetting x)
            : this(x.Info)
        {
            this.ReportInterval = x.ReportInterval;
            //this.IsParameterOnColumn = x.IsParameterOnColumn;
        }

        /// <summary>
        /// Тип отчётного периода
        /// </summary>
        [DataMember]
        [DisplayName("Интервал")]
        public ReportTimeInterval ReportInterval { get; set; }

        ///// <summary>
        ///// true, если для каждого параметра своя колонка и 
        ///// false, если ИД параметра размещены в отдельной колонке
        ///// </summary>
        //public bool IsParameterOnColumn { get; set; }

        //[NonSerialized]
        //private int[] parameterIds;

        ///// <summary>
        ///// ИД запрашиваемых параметров.
        ///// Нигде не хранятся, кто-то должен будет лично их заполнять
        ///// </summary>
        //public int[] ParameterIds
        //{
        //    get { return parameterIds; }
        //    set { parameterIds = value; }
        //}

        public override byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(Enabled);
                    //bw.Write(IsParameterOnColumn);
                    bw.Write((int)ReportInterval);
                }
                return ms.ToArray();
            }
        }

        public override void FromBytes(byte[] bytes)
        {
            if (bytes == null)
                return;

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    Enabled = br.ReadBoolean();
                    //IsParameterOnColumn = br.ReadBoolean();
                    ReportInterval = (ReportTimeInterval)br.ReadInt32();
                }
            }
        }

        public override object Clone()
        {
            return new TimeReportSourceSetting(this);
        }

        public override ReportParameter[] GetReportParameters()
        {
            ReportParameter[] pars = GetParameters(ReportInterval);
            if (pars != null)
                foreach (var item in pars)
                {
                    item.BaseSettings = this;
                }
            return pars;
        }

        #region Параметры отчета
        private const String intervalCategory = "Отчётный период";

        private const String timeFromParameterName = "timeinterval_from";
        private const String timeFromParameterDisplayName = "1. С";
        private const String timeFromParameterDescription = "Начальное время периода";
        private const String timeToParameterName = "timeinterval_to";
        private const String timeToParameterDisplayName = "2. По";
        private const String timeToParameterDescription = "Конечное время периода";

        private const String timeDayParameterName = "timeinterval_day";
        private const String timeDayParameterDisplayName = "Сутки";
        private const String timeDayParameterDescription = "Отчётные сутки";

        private const String timeMonthParameterName = "timeinterval_month";
        private const String timeMonthParameterDisplayName = "Месяц";
        private const String timeMonthParameterDescription = "Отчётный месяц";

        private const String timeQuarterParameterName = "timeinterval_quarter";
        private const String timeQuarterParameterDisplayName = "Квартал";
        private const String timeQuarterParameterDescription = "Отчётный квартал";

        private const String timeYearParameterName = "timeinterval_year";
        private const String timeYearParameterDisplayName = "Год";
        private const String timeYearParameterDescription = "Отчётный год";

        /// <summary>
        /// Получить параметры для указанного отчётного периода
        /// </summary>
        /// <param name="reportInterval">Тип отчётного периода</param>
        /// <returns></returns>
        public ReportParameter[] GetParameters(ReportTimeInterval reportInterval)
        {
            switch (reportInterval)
            {
                case ReportTimeInterval.Custom:
                    return new ReportParameter[] { 
                        new ReportParameter(timeFromParameterName, 
                                            timeFromParameterDisplayName, 
                                            timeFromParameterDescription, 
                                            intervalCategory,
                                            typeof(DateTime).FullName,
                                            DateTime.Now),
                        new ReportParameter(timeToParameterName, 
                                            timeToParameterDisplayName, 
                                            timeToParameterDescription,
                                            intervalCategory,
                                            typeof(DateTime).FullName,
                                            DateTime.Now),
                    };
                case ReportTimeInterval.Dayly:
                    return new ReportParameter[] { 
                        new ReportParameter(timeDayParameterName, 
                                            timeDayParameterDisplayName, 
                                            timeDayParameterDescription, 
                                            intervalCategory, 
                                            typeof(DateTime).FullName,
                                            DateTime.Now)
                    };
                case ReportTimeInterval.Mohtly:
                    return new ReportParameter[] { 
                        new ReportParameter(timeMonthParameterName, 
                                            timeMonthParameterDisplayName, 
                                            timeMonthParameterDescription, 
                                            intervalCategory, 
                                            typeof(DateTime).FullName, 
                                            typeof(MonthDateTimeTypeConverter).FullName, 
                                            DateTime.Now)
                    };
                case ReportTimeInterval.Quarterly:
                    return new ReportParameter[] { 
                        new ReportParameter(timeQuarterParameterName, 
                                            timeQuarterParameterDisplayName, 
                                            timeQuarterParameterDescription, 
                                            intervalCategory, 
                                            typeof(DateTime).FullName,
                                            typeof(QuarterDateTimeTimeConverter).FullName, 
                                            DateTime.Now)
                    };
                case ReportTimeInterval.Yearly:
                    return new ReportParameter[] { 
                        new ReportParameter(timeYearParameterName, 
                                            timeYearParameterDisplayName, 
                                            timeYearParameterDescription, 
                                            intervalCategory, 
                                            typeof(DateTime).FullName, 
                                            typeof(YearDateTimeConverter).FullName, 
                                            DateTime.Now)
                    };
                default:
                    return null;
            }
        }

        /// <summary>
        /// Получить отчётный период из параметров отчёта
        /// </summary>
        /// <param name="parameters">Параметры отчёта</param>
        /// <param name="TimeFrom">Начальное время</param>
        /// <param name="TimeTo">Конечное время</param>
        public void GetReportInterval(ReportParameter[] parameters, out DateTime TimeFrom, out DateTime TimeTo)
        {
            DateTime tempDateTime;

            // значения по умолчанию
            TimeFrom = DateTime.MaxValue;
            TimeTo = DateTime.Now;

            foreach (var item in parameters)
            {
                switch (item.Name)
                {
                    case timeFromParameterName:
                        TimeFrom = (DateTime)item.GetValue();
                        break;
                    case timeToParameterName:
                        TimeTo = (DateTime)item.GetValue();
                        break;
                    case timeDayParameterName:
                        tempDateTime = (DateTime)item.GetValue();
                        TimeFrom = new DateTime(tempDateTime.Year, tempDateTime.Month, tempDateTime.Day);
                        TimeTo = Interval.Day.GetNextTime(TimeFrom);
                        return;
                    case timeMonthParameterName:
                        tempDateTime = (DateTime)item.GetValue();
                        tempDateTime = (DateTime)item.GetValue();
                        TimeFrom = new DateTime(tempDateTime.Year, tempDateTime.Month, 1);
                        TimeTo = Interval.Month.GetNextTime(TimeFrom);
                        return;
                    case timeQuarterParameterName:
                        tempDateTime = (DateTime)item.GetValue();
                        Interval qurto = Interval.Quarter;
                        TimeFrom = qurto.NearestEarlierTime(/*Interval.MinDateTime,*/ tempDateTime);
                        TimeTo = qurto.GetNextTime(TimeFrom);
                        return;
                    case timeYearParameterName:
                        tempDateTime = (DateTime)item.GetValue();
                        tempDateTime = (DateTime)item.GetValue();
                        TimeFrom = new DateTime(tempDateTime.Year, 1, 1);
                        TimeTo = Interval.Year.GetNextTime(TimeFrom);
                        return;
                }
            }
        }

        #endregion
    }
}

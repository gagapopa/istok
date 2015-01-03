using System;
using System.ComponentModel;
using System.Data;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class LoadParameterNode : ParameterNode
    {
        const String scheduleAttributeName = @"schedule";
        const String intervalAttributeName = "interval";
        const String aggregIntervalAttributeName = "aggreg_interval";
        const String aggregationAttributeName = "aggregation";
        const String startTimeAttributeName = "start_time";
        const String loadCodeAttributeName = "code";

        [Browsable(false)]
        public bool HasAperture
        {
            get
            {
                if (Attributes.ContainsKey(CommonData.ApertureProperty))
                {
                    if (string.IsNullOrEmpty(Attributes[CommonData.ApertureProperty].ToString()))
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
        }

#if DEBUG
        [Browsable(true)]
        [DisplayName("Расписание")]
        [Description("Расписание выгрузки параметров")]
        [Category("Передача значений")]
        [TypeConverter(typeof(ScheduleTypeConverter))]
        public int Schedule
        {
            get
            {
                int result;

                if (int.TryParse(GetAttribute(scheduleAttributeName), out result))
                    return result;

                return 0;
            }
            set
            {
                SetAttribute(scheduleAttributeName, value.ToString());
            }
        }
        [DisplayName("Интервал агрегации"), Description("Переодичность параметров передаваемая на сервер приложения.")]
        //[ReadOnly(false)]
        [Category("Передача значений")]
        [TypeConverter(typeof(IntervalTypeConverter))]
        public Interval AggregationInterval
        {
            get
            {
                try
                {
                    //return new Interval((double)doubleconv.ConvertFromInvariantString(GetAttribute(aggregIntervalAttributeName)));
                    return Interval.FromString(GetAttribute(aggregIntervalAttributeName));
                }
                catch { return Interval.Zero; }
            }
            set
            {
                //SetAttribute(aggregIntervalAttributeName, value.ToDouble().ToString());
                SetAttribute(aggregIntervalAttributeName, value.ToString());
            }
        }

        [DisplayName("Агрегация"), Description("Алгоритм агрегации параметров.")]
        [Category("Передача значений")]
        [TypeConverter(typeof(AggregationTypeConverter))]
        public CalcAggregation Aggregation
        {
            get
            {
                int res;

                if (int.TryParse(GetAttribute(aggregationAttributeName), out res))
                    return (CalcAggregation)res;

                return CalcAggregation.Nothing;
            }
            set
            {
                SetAttribute(aggregationAttributeName, ((int)value).ToString());
            }
        } 
#endif

        [DisplayName("Аппертура")]
        [Description("Аппертура")]
        [CategoryOrder(CategoryGroup.Values)]
        public double Aperture
        {
            get
            {
                try
                {
                    String stringValue = GetAttribute(CommonData.ApertureProperty);

                    if (!String.IsNullOrEmpty(stringValue))
                        return (double)doubleconv.ConvertFromInvariantString(stringValue);
                }
                catch (FormatException) { }
                return 0;
            }
            set
            {
                SetAttribute(CommonData.ApertureProperty, doubleconv.ConvertToInvariantString(value));
            }
        }

        /// <summary>
        /// интервал в секундах
        /// </summary>
        [TypeConverter(typeof(IntervalTypeConverter))]
        [Description("Интервал")]
        [DisplayName("Интервал")]
        [CategoryOrder(CategoryGroup.Values)]
        [Browsable(true)]
        public Interval Interval
        {
            get
            {
                try
                {
                    //if (typ == UnitTypeId.ManualParameter && parentNode != null && parentNode is ParameterGateNode) return ((ParameterGateNode)parentNode).Interval;
                    //return new Interval((double)doubleconv.ConvertFromInvariantString(GetAttribute(intervalAttributeName)));
                    return Interval.FromString(GetAttribute(intervalAttributeName));
                }
                catch { return Interval.Zero; }
            }
            set
            {
                //SetAttribute(intervalAttributeName, value.ToDouble().ToString());
                SetAttribute(intervalAttributeName, value.ToString());
            }
        }

        /// <summary>
        /// дата начала расчетов
        /// </summary>
        [DisplayName("Время запуска")]
        [Description("Дата и время начала сбора данных")]
        [CategoryOrder(CategoryGroup.Load)]
        [Browsable(true)]
        public DateTime StartTime
        {
            get
            {
                DateTime res;
                if (DateTime.TryParse(GetAttribute(startTimeAttributeName), out res))
                    return res;
                return DateTime.MinValue;
            }
            set
            {
                SetAttribute(startTimeAttributeName, value.ToString());
            }
        }

        /// <summary>
        /// Код используется модулем сбора для идентификации параметра во внешней системе
        /// </summary>
        [DisplayName("Код собираемого параметра")]
        [Description("Код используется модулем сбора для идентификации параметра во внешней системе")]
        [CategoryOrder(CategoryGroup.Load)]
        public String LoadCode
        {
            get { return GetAttribute(loadCodeAttributeName); }
            set { SetAttribute(loadCodeAttributeName, value); }
        }

        public LoadParameterNode()
            : base() { }
        public LoadParameterNode(int id)
            : base(id) { }
        public LoadParameterNode(DataRow row)
            : base(row)
        { }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

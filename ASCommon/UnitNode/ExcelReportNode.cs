using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class ExcelReportNode : ParametrizedUnitNode
    {
        const String reportBodyAttributeName = "report";
        const String intervalAttributeName = "interval";
        const String aggregationAttributeName = "aggregation";

        protected bool bodyModified = false;
        protected byte[] bodyBuffer = null;
        // интервал в секундах
        [DisplayName("Интервал")]
        [TypeConverter(typeof(IntervalTypeConverter))]
        public Interval Interval
        {
            get
            {
                Interval inter;
                try
                {
                    //inter = new Interval(Convert.ToDouble(GetAttribute(intervalAttributeName), System.Globalization.NumberFormatInfo.InvariantInfo));
                    inter = Interval.FromString(GetAttribute(intervalAttributeName));
                }
                catch { inter = Interval.Zero; }
                return inter;
            }
            set
            {
                SetAttribute(intervalAttributeName, value.ToString());// doubleconv.ConvertToInvariantString(value.ToDouble()));
            }
        }
        // интервал в секундах
        [DisplayName("Агрегация"), Description("Алгоритм агрегации параметров в отчете.")]
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

        [DisplayName("ИД узла оптимизации"),
        Category("Отчет оптимизации"),
        Description("ИД узла оптимизации для которого формируется отчёт и чьи аргументы будут перебираться")]
        public int OptimizationID
        {
            get
            {
                int s;

                if (int.TryParse(GetAttribute(optimizationIDAttributeName), out s))
                    return s;

                return 0;
            }
            set
            {
                SetAttribute(optimizationIDAttributeName, value.ToString());
            }
        }
        const String optimizationIDAttributeName = "optimization_id";

        [DisplayName("Копируемый диапазон"),
        Category("Отчет оптимизации"),
        Description("")]
        public String OptimizationExcelRange
        {
            get { return GetAttribute(optimizationExcelRangeAttributeName); }
            set { SetAttribute(optimizationExcelRangeAttributeName, value); }
        }
        const String optimizationExcelRangeAttributeName = "optimization_excel_range";

        [DisplayName("Направление копирования"),
        Category("Отчет оптимизации"),
        Description("0-вниз, 1-вправо")]
        public bool OptimizationCopyDirection
        {
            get
            {
                bool res;

                if (bool.TryParse(GetAttribute(optimizationCopyDirectionAttributeName), out res))
                    return res;

                return false;
            }
            set
            {
                SetAttribute(optimizationCopyDirectionAttributeName, value.ToString());
            }
        }
        const String optimizationCopyDirectionAttributeName = "optimization_copy_direction";

        [Browsable(false)]
        public byte[] ExcelReportBody
        {
            get
            {
                bodyBuffer = GetBinaries(reportBodyAttributeName);
                return bodyBuffer;
            }
            set
            {
                bodyBuffer = value;
                SetBinaries(reportBodyAttributeName, bodyBuffer);
            }
        }

        [Browsable(false)]
        public string FileName
        {
            get
            {
                return GetAttribute("report_cod");
            }
        }

        public ExcelReportNode() : base() { }
        public ExcelReportNode(DataRow row)
            : base(row)
        {
            if (Typ != (int)UnitTypeId.ExcelReport) throw new Exception("Неверный тип узла");
        }

        public override ChildParamNode CreateNewChildParamNode()
        {
            return new ChildParamNode();
        }
    }
}

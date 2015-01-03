using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using COTES.ISTOK;

namespace COTES.ISTOK.ASC.TypeConverters
{
    class AggregationTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            List<CalcAggregation> lstValues = new List<CalcAggregation>();

            foreach (CalcAggregation item in Enum.GetValues(typeof(CalcAggregation)))
                lstValues.Add(item);

            return new StandardValuesCollection(lstValues);
        }

        const String AverageLocal = "Среднее";
        const String FirstLocal = "Первое";
        const String LastLocal = "Последнее";
        const String MaximumLocal = "Максимум";
        const String MinimumLocal = "Минимум";
        const String NothingLocal = "Нет";
        const String SumLocal = "Сумма";
        const String CountLocal = "Количество значений";
        const String ExistLocal = "Существует";
        const String WeightedLocal = "Взвешенное значение";

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            switch ((string)value)
            {
                case AverageLocal:
                    return CalcAggregation.Average;
                case FirstLocal:
                    return CalcAggregation.First;
                case LastLocal:
                    return CalcAggregation.Last;
                case MaximumLocal:
                    return CalcAggregation.Maximum;
                case MinimumLocal:
                    return CalcAggregation.Minimum;
                case NothingLocal:
                    return CalcAggregation.Nothing;
                case SumLocal:
                    return CalcAggregation.Sum;
                case CountLocal:
                    return CalcAggregation.Count;
                case ExistLocal:
                    return CalcAggregation.Exist;
                //case WeightedLocal:
                //    return CalcAggregation.Weighted;
                default:
                    return CalcAggregation.Nothing;
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            switch ((CalcAggregation)value)
            {
                case CalcAggregation.Average:
                    return AverageLocal;
                case CalcAggregation.First:
                    return FirstLocal;
                case CalcAggregation.Last:
                    return LastLocal;
                case CalcAggregation.Maximum:
                    return MaximumLocal;
                case CalcAggregation.Minimum:
                    return MinimumLocal;
                case CalcAggregation.Nothing:
                    return NothingLocal;
                case CalcAggregation.Sum:
                    return SumLocal;
                case CalcAggregation.Count:
                    return CountLocal;
                case CalcAggregation.Exist:
                    return ExistLocal;
                //case CalcAggregation.Weighted:
                //    return WeightedLocal;
                default:
                    return NothingLocal;
            }
        }
    }
}

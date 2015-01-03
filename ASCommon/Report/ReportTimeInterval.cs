using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Интервал отчётного периода отчёта
    /// </summary>
    [TypeConverter(typeof(ReportTimeIntervalConverter))]
    public enum ReportTimeInterval
    {
        /// <summary>
        /// Выбор
        /// </summary>
        Custom, 
        
        /// <summary>
        /// Сутки
        /// </summary>
        Dayly, 
        
        /// <summary>
        /// Месяц
        /// </summary>
        Mohtly, 
        
        /// <summary>
        /// Квартал
        /// </summary>
        Quarterly, 
        
        /// <summary>
        /// Год
        /// </summary>
        Yearly
    }

    /// <summary>
    /// TypeConverter для ReportTimeInterval
    /// </summary>
    public class ReportTimeIntervalConverter : EnumConverter
    {
        public ReportTimeIntervalConverter()
            : base(typeof(ReportTimeInterval))
        {

        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return "Выбор";
            if (destinationType == typeof(String))
                switch ((ReportTimeInterval)value)
                {
                    case ReportTimeInterval.Custom:
                        return "Выбор";
                    case ReportTimeInterval.Dayly:
                        return "Сутки";
                    case ReportTimeInterval.Mohtly:
                        return "Месяц";
                    case ReportTimeInterval.Quarterly:
                        return "Квартал";
                    case ReportTimeInterval.Yearly:
                        return "Год";
                    default:
                        return value.ToString();
                }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                switch (value.ToString().ToLower())
                {
                    default:
                    case "выбор":
                        return ReportTimeInterval.Custom;
                    case "сутки":
                        return ReportTimeInterval.Dayly;
                    case "месяц":
                        return ReportTimeInterval.Mohtly;
                    case "квартал":
                        return ReportTimeInterval.Quarterly;
                    case "год":
                        return ReportTimeInterval.Yearly;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    /// <summary>
    /// TypeConverter для выбора месяца
    /// </summary>
    public class MonthDateTimeTypeConverter : DateTimeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String))
            {
                return ((DateTime)value).ToString("MMMM yyyy", culture.DateTimeFormat);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    /// TypeConverter для выбора квартала
    /// </summary>
    public class QuarterDateTimeTimeConverter:DateTimeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String))
            {
                DateTime time = (DateTime)value;
                String ret = "";
                int nom = (time.Month - 1) / 3 + 1;
                if (nom == 1)
                    ret = "I";
                else if (nom == 2)
                    ret = "II";
                else if (nom == 3)
                    ret = "III";
                else if (nom == 4)
                    ret = "IV";

                return String.Format("{0} квартал {1}", ret, time.Year);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is String)
            {
                String[] parts = value.ToString().Split();
                int temp;
                int quart = 0, year = 0;

                foreach (var item in parts)
                {
                    if (int.TryParse(item, out temp))
                    {
                        if (2000 <= temp && temp <= 9999)
                            year = temp;
                    }
                    else switch (item.ToLower())
                        {
                        case "i":
                                quart = 1;
                                break;
                        case "ii":
                                quart = 2;
                                break;
                        case "iii":
                                quart = 3;
                                break;
                        case "iv":
                                quart = 4;
                                break;
                        }
                    if (quart > 0 && year > 0)
                        return new DateTime(year, (quart - 1) * 3 + 1, 1);
                }

            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    /// <summary>
    /// TypeConverter для выбора года
    /// </summary>
    public class YearDateTimeConverter : DateTimeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String))
            {
                return ((DateTime)value).ToString("yyyy", culture.DateTimeFormat);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

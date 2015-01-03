using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace COTES.ISTOK.ASC.TypeConverters
{
    /// <summary>
    /// TypeConverter для double, который конвертирует double.NaN в пустую строку
    /// </summary>
    public class DoubleTypeConverter : DoubleConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType.Equals(typeof(String))&&double.IsNaN((double)value)) return String.Empty;
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null || String.IsNullOrEmpty(value.ToString())) return double.NaN;
            return base.ConvertFrom(context, culture, value);
        }
    }
}

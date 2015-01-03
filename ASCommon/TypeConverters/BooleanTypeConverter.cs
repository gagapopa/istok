using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace COTES.ISTOK.ASC.TypeConverters
{
    public class BooleanTypeConverter : BooleanConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context,
          CultureInfo culture,
          object value,
          Type destType)
        {
            if (value == null) return value;
            return (bool)value ? "Да" : "Нет";
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            return (string)value == "Да";
        }
    }
}

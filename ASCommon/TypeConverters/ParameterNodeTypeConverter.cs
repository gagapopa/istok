using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    class ParameterNodeTypeConverter:TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is UnitNode[])
            {
                return String.Format("Элементов: {0}", (value as UnitNode[]).Length);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

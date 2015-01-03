using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    public class DataLoadMethodTypeConverter : EnumConverter
    {
        const String CurrentRuLocal = "Текущие";
        const String SubscribeRuLocal = "Подписка";
        const String ArchiveRuLocal = "Архивные";
        const String ArchiveByParameterRuLocal = "Архивные по параметрам";

        public DataLoadMethodTypeConverter()
            : base(typeof(DataLoadMethod))
        {

        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is String && culture == CultureInfo.InvariantCulture)
            {
                return (DataLoadMethod)int.Parse(value.ToString());
            }
            else if (value is String)
            {
                switch (value.ToString())
                {
                    case CurrentRuLocal:
                        return DataLoadMethod.Current;
                    case SubscribeRuLocal:
                        return DataLoadMethod.Subscribe;
                    case ArchiveRuLocal:
                        return DataLoadMethod.Archive;
                    case ArchiveByParameterRuLocal:
                        return DataLoadMethod.ArchiveByParameter;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is DataLoadMethod)
            {
                if (destinationType == typeof(String) && culture == CultureInfo.InvariantCulture)
                {
                    return ((int)value).ToString();
                }
                else if (destinationType == typeof(String))
                {
                    switch ((DataLoadMethod)value)
                    {
                        case DataLoadMethod.Current:
                            return CurrentRuLocal;
                        case DataLoadMethod.Subscribe:
                            return SubscribeRuLocal;
                        case DataLoadMethod.Archive:
                            return ArchiveRuLocal;
                        case DataLoadMethod.ArchiveByParameter:
                            return ArchiveByParameterRuLocal;
                    }
                } 
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

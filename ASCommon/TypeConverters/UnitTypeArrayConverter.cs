using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    class UnitTypeArrayConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String))
            {
                var types = value as int[];
                var typeRetrieval = context.GetService(typeof(IUnitTypeRetrieval)) as IUnitTypeRetrieval;
                if (typeRetrieval != null)
                {
                    var localDict = typeRetrieval.GetUnitTypeLocalization(culture);

                    StringBuilder builder = new StringBuilder();
                    if (types != null)
                    {
                        foreach (var typeID in types)
                        {
                            if (builder.Length > 0)
                            {
                                builder.Append(";");
                            }
                            builder.Append(localDict[typeID]);
                        }
                    }

                    return builder.ToString();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

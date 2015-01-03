using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    class StructureReportTableConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is String)
            {
                value = ConvertFrom(context, culture, value);
            }

            if (context != null && value != null && destinationType == typeof(String))
            {
                var table = context.Instance as StructureReportTable;
                var tableId = (int)value;

                if (tableId < 0)
                    return "<нет>";
                return table.SourceSettings.Tables[tableId].TableName;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(String);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is String)
            {
                var table = context.Instance as StructureReportTable;

                if (table.SourceSettings != null)
                {
                    var targetTable = (from t in table.SourceSettings.Tables where t.TableName == value.ToString() select t).FirstOrDefault();

                    if (targetTable != null)
                        return new List<StructureReportTable>(table.SourceSettings.Tables).IndexOf(targetTable);
                }
                return -1;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var table = context.Instance as StructureReportTable;

            List<int> ids = new List<int>();
            
            ids.Add(-1);

            for (int i = 0; table.SourceSettings != null && i < table.SourceSettings.Tables.Length; i++)
            {
                if (table.SourceSettings.Tables[i]!=table)
                {
                    ids.Add(i);
                }
            }

            return new StandardValuesCollection(ids);
        }
    }
}

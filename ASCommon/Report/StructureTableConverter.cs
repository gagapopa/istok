using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    class StructureTableConverter : ExpandableObjectConverter
    {
        //public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        //{
        //    return true;
        //    //return base.GetCreateInstanceSupported(context);
        //}

        //public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
        //{
        //    return base.CreateInstance(context, propertyValues);
        //}
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            var tables = value as StructureReportTable[];

            if (tables != null)
            {
                var reportNode = context.Instance as ReportNode;
                StructureReportSourceSettings structureSettigns = null;

                foreach (var guid in reportNode.GetReportSourcesGuids())
                {
                    var settings = reportNode.GetReportSetting(guid);
                    if (settings is StructureReportSourceSettings)
                        structureSettigns = settings as StructureReportSourceSettings;
                }
                foreach (var item in tables)
                {

                    item.SourceSettings = structureSettigns;
                }
            }
            return String.Format("Таблиц: {0}", tables == null ? 0 : tables.Length);
            //return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

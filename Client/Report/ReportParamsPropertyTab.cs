using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms.Design;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Client
{
    class ReportParamsPropertyTab : PropertyTab
    {
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(ReportParametersContainer));

        public override string TabName
        {
            get { return "Report Time"; }
        }

        public override System.Drawing.Bitmap Bitmap
        {
            get
            {
                return Properties.Resources._1downarrow;
            }
        }

        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
        {
            return GetProperties(null, component, attributes);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
        {
            ReportNode report = component as ReportNode;

            if (report != null)
            {
                ReportParametersContainer container = report.ParameterContainer;

                List<ReportParameter> parameterList = new List<ReportParameter>();

                foreach (var guid in report.GetReportSourcesGuids())
                {
                    var settings = report.GetReportSetting(guid);
                    if (settings != null && settings.Enabled)
                    {
                        var pars=settings.GetReportParameters();
                        if (pars != null)
                        {
                            parameterList.AddRange(pars);
                        }
                    }
                }
                container.SetProperties(parameterList);
                return converter.GetProperties(context, container, attributes);
            }

            return null;
        }
    }
}

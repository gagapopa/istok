using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms.Design;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    class ReportSettingPropertyTab : PropertyTab
    {
        TypeConverter designTimeSourceTypeConverter = new DesignTimeReportSourceTypeConverter();

        public override string TabName
        {
            get { return "Design Time"; }
        }

        public override System.Drawing.Bitmap Bitmap
        {
            get
            {
                return Properties.Resources._1uparrow;
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
                List<PropertyDescriptor> propertyList = new List<PropertyDescriptor>();

                foreach (var guid in report.GetReportSourcesGuids())
                {
                    var settings = report.GetReportSetting(guid);
                    if (settings != null && settings.Enabled)
                    {
                        var propCollection = designTimeSourceTypeConverter.GetProperties(context, settings, attributes);
                        if (propCollection != null)
                        {
                            foreach (PropertyDescriptor property in propCollection)
                            {
                                propertyList.Add(property);
                            }
                        }
                    }
                }
                return new PropertyDescriptorCollection(propertyList.ToArray());
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(component);
                return converter.GetProperties(context, component, attributes);
            }
        }

        public override bool CanExtend(object extendee)
        {
            return base.CanExtend(extendee);
        }
    }

    class DesignTimeReportSourceTypeConverter : TypeConverter
    {

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var settings = value as ReportSourceSettings;

            if (settings != null)
            {
                List<PropertyDescriptor> properties = GetDesignTimeProperties(context, attributes, settings);
                return new PropertyDescriptorCollection(properties.ToArray());
            }

            return null;
        }

        private List<PropertyDescriptor> GetDesignTimeProperties(ITypeDescriptorContext context, Attribute[] attributes, ReportSourceSettings settings)
        {
            var converter = TypeDescriptor.GetConverter(settings);
            List<PropertyDescriptor> properties = new List<PropertyDescriptor>();

            var propertyCollection = converter.GetProperties(context, settings, attributes);
            if (propertyCollection != null)
            {
                foreach (PropertyDescriptor property in propertyCollection)
                {
                    properties.Add(new DesignTimeProprtyDescriptor(null, settings.Info, property));
                }
            }
            return properties;
        }

        class DesignTimeProprtyDescriptor : SimplePropertyDescriptor
        {
            public StructureProvider Provider { get; private set; }

            public ReportSourceInfo Source { get; private set; }

            public PropertyDescriptor Descriptor { get; private set; }

            public DesignTimeProprtyDescriptor(StructureProvider provider, ReportSourceInfo source, PropertyDescriptor descriptor)
                : base(descriptor.ComponentType, descriptor.Name, descriptor.PropertyType)
            {
                this.Descriptor = descriptor;
                this.Provider = provider;
                this.Source = source;

                AttributeArray = descriptor.Attributes.OfType<Attribute>().ToArray();
            }

            public override string DisplayName
            {
                get
                {
                    return Descriptor.DisplayName;
                }
            }

            public override string Category
            {
                get
                {
                    return Source.Caption;
                }
            }

            public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
            {
                return base.GetChildProperties(instance, filter);
            }

            public override object GetValue(object component)
            {
                var report = component as ReportNode;

                if (report != null)
                {
                    var settings = report.GetReportSetting(Source.ReportSourceId);

                    if (settings != null)
                    {
                        return Descriptor.GetValue(settings);
                    }
                }

                return null;
            }

            public override void SetValue(object component, object value)
            {
                var report = component as ReportNode;

                if (report != null)
                {
                    var settings = report.GetReportSetting(Source.ReportSourceId);

                    if (settings == null)
                    {
                        settings = Provider.GetReportSourceSettings(Source.ReportSourceId);
                        report.SetReportSetting(Source.ReportSourceId, settings);
                    }

                    Descriptor.SetValue(settings, value);
                }
            }
        }
    }
}

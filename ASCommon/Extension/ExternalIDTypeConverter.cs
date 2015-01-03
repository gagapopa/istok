using System;
using System.Collections.Generic;
using System.ComponentModel;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Extension
{
    public class ExternalIDTypeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            ExtensionUnitNode unitNode = context.Instance as ExtensionUnitNode;
            IExternalsSupplier supplier = context.GetService(typeof(IExternalsSupplier)) as IExternalsSupplier;

            if (supplier != null && unitNode != null)
                return supplier.ExternalIDSupported(unitNode);

            return base.GetStandardValuesSupported(context);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            ExtensionUnitNode unitNode = context.Instance as ExtensionUnitNode;
            IExternalsSupplier supplier = context.GetService(typeof(IExternalsSupplier)) as IExternalsSupplier;
         
            if (supplier != null && unitNode != null)
            {
                List<int> extensionName = new List<int>();
                EntityStruct[] list = supplier.GetExternalIDList(unitNode);
                foreach (var item in list)
                    extensionName.Add(item.ID);

                if (supplier.ExternalIDCanAdd(unitNode))
                    extensionName.Add(0);
                return new StandardValuesCollection(extensionName);
            }
            return base.GetStandardValues(context);
        }

        const String addItemString = "<добавить>";
        const String nothingItemString = "<нет>";
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(String))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is String&&!String.IsNullOrEmpty((String)value))
            {
                String text = (String)value;
                if (text.IndexOf(' ') > 0)
                    return int.Parse(text.Substring(0, text.IndexOf(' ')));
                if (addItemString.Equals(text))
                    return 0;
            }
            return -1;
            //return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(String))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (context!=null)
            {
                ExtensionUnitNode unitNode = context.Instance as ExtensionUnitNode;
                IExternalsSupplier supplier = context.GetService(typeof(IExternalsSupplier)) as IExternalsSupplier;

                if (destinationType == typeof(String) && supplier != null && unitNode != null)
                {
                    if (Object.Equals(value, 0))
                        return addItemString;
                    if ((int)value < 0)
                        return nothingItemString;
                    EntityStruct[] list = supplier.GetExternalIDList(unitNode);
                    if (list != null)
                        foreach (var item in list)
                            if (item.ID.Equals(value))
                                return String.Format("{0} {1}", item.ID, item.Name);

                    return value.ToString();
                } 
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

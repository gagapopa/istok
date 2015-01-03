using System;
using System.Collections.Generic;
using System.ComponentModel;
using COTES.ISTOK.ASC.TypeConverters;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Extension
{
    public class ExtensionUnitNodeTypeConverter : UnitNodeTypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection result = base.GetProperties(context, value, attributes);
            IExternalsSupplier supplier = context.GetService(typeof(IExternalsSupplier)) as IExternalsSupplier;
            ExtensionUnitNode unitNode = value as ExtensionUnitNode;

            if (unitNode != null && supplier != null)
            {
                List<PropertyDescriptor> resultList = new List<PropertyDescriptor>();
                IUnitProviderSupplier unitProviderSupplier = context.GetService(typeof(IUnitProviderSupplier)) as IUnitProviderSupplier;
                IUnitNodeProvider unitProvider = null;

                if (unitProviderSupplier != null)
                    unitProvider = unitProviderSupplier.GetProvider(unitNode);

                foreach (PropertyDescriptor item in result)
                {
                    if (item.Name.Equals("ExternalID"))
                    {
                        if (supplier.ExternalIDSupported(unitNode))
                            resultList.Add(new ExternalIDPropertyDescriptor(context, unitProvider, item));
                    }
                    else if (!item.Name.Equals("ExternalCode") || supplier.ExternalCodeSupported(unitNode))
                    {
                        resultList.Add(item);
                    }
                }

                ItemProperty[] props = supplier.GetExternalProperties(unitNode);

                if (props != null)
                    foreach (var item in props)
                        resultList.Add(new ExternalPropertyDescriptor(unitNode, item));
                return new PropertyDescriptorCollection(resultList.ToArray());
            }
            return result;
        }

        /// <summary>
        /// Дескриптор свойства, полученного из расширения
        /// </summary>
        class ExternalPropertyDescriptor : SimplePropertyDescriptor
        {
            ExtensionUnitNode unitNode;
            ItemProperty propertyInfo;

            public ExternalPropertyDescriptor(ExtensionUnitNode unitNode, ItemProperty propertyInfo) :
                base(unitNode.GetType(), propertyInfo.Name, typeof(String))
            {
                this.unitNode = unitNode;
                this.propertyInfo = propertyInfo;
            }

            public override string DisplayName
            {
                get
                {
                    if (String.IsNullOrEmpty(propertyInfo.DisplayName))
                        return Name;
                    return propertyInfo.DisplayName;
                }
            }

            public override string Description
            {
                get
                {
                    return propertyInfo.Description;
                }
            }

            public override string Category
            {
                get
                {
                    return propertyInfo.Category;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public override object GetValue(object component)
            {
                return propertyInfo.DefaultValue;
            }

            public override void SetValue(object component, object value)
            {
            }
        }

        /// <summary>
        /// Дескриптор свойства "ИД внешнего узла".
        /// При выборе нового ИД, меняет свойство "Код внешнего узла"
        /// </summary>
        class ExternalIDPropertyDescriptor : UnitNodePropertyDescriptor
        {
            ITypeDescriptorContext context;

            public ExternalIDPropertyDescriptor(
                ITypeDescriptorContext context,
                //COTES.ISTOK.ASC.UnitNode unitNode,
                COTES.ISTOK.ASC.IUnitNodeProvider unitProvider,
                PropertyDescriptor baseDescriptor)
                : base(unitProvider, baseDescriptor)
            {
                this.context = context;
            }

            public override void SetValue(object component, object value)
            {
                ExtensionUnitNode extensionUnitNode = component as ExtensionUnitNode;

                if (extensionUnitNode != null)
                {
                    int unitNodeId = (int)value;

                    //extensionUnitNode.ExternalID = unitNodeId;
                    if (unitNodeId > 0)
                    {
                        String text = (new ExternalIDTypeConverter()).ConvertToString(context, value);
                        text = text.Substring(text.IndexOf(' ') + 1);
                        extensionUnitNode.ExternalCode = text;
                        context.OnComponentChanging();
                    }
                }

                base.SetValue(component, value);
            }
        }
    }
}
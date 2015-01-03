using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Client
{
    class ImportUnitNodeTypeConverter : ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (context != null)
            {
                ImportUnitNode unitNode = value as ImportUnitNode;

                List<PropertyDescriptor> propertyList = new List<PropertyDescriptor>();

                PropertyDescriptorCollection baseCollection = base.GetProperties(context, value, attributes);

                foreach (PropertyDescriptor item in baseCollection)
                {
                    if (item.Name.Equals("ImportValues")
                        || (item.Name.Equals("ImportCode") && unitNode != null && unitNode.Node == null))
                        propertyList.Add(item);
                }

                if (unitNode!=null&&unitNode.Node!=null)
                {
                    Type nodeType = unitNode.Node.GetType();

                    Object[] attrs = nodeType.GetCustomAttributes(typeof(TypeConverterAttribute), true);
                    TypeConverterAttribute converterAttribute = null;

                    if (attrs != null && attrs.Length > 0)
                        converterAttribute = attrs[0] as TypeConverterAttribute;

                    if (converterAttribute != null)
                    {
                        Type converterType = Type.GetType(converterAttribute.ConverterTypeName);
                        if (converterType != null)
                        {
                            TypeConverter converter = Activator.CreateInstance(converterType) as TypeConverter;

                            if (converter != null)
                            {
                                PropertyDescriptorCollection collection = converter.GetProperties(context, unitNode.Node, attributes);

                                if (collection != null)
                                {
                                    foreach (PropertyDescriptor item in collection)
                                    {
                                        propertyList.Add(new ImportPropertyDescriptor(item));
                                    }
                                }
                            }
                        }
                    } 
                }
                return new PropertyDescriptorCollection(propertyList.ToArray());
            }

            return base.GetProperties(context, value, attributes);
        }

        class ImportPropertyDescriptor : SimplePropertyDescriptor
        {
            PropertyDescriptor property;

            public ImportPropertyDescriptor(PropertyDescriptor property)
                : base(property.ComponentType, property.Name, property.PropertyType)
            {
                this.property = property;
            }

            public override AttributeCollection Attributes
            {
                get
                {
                    return property.Attributes;
                }
            }

            public override string DisplayName
            {
                get
                {
                    return property.DisplayName;
                }
            }

            public override string Description
            {
                get
                {
                    return property.Description;
                }
            }

            public override string Category
            {
                get
                {
                    return property.Category;
                }
            }

            public override TypeConverter Converter
            {
                get
                {
                    return property.Converter;
                }
            }

            public override object GetValue(object component)
            {
                ImportUnitNode importNode = component as ImportUnitNode;
                return property.GetValue(importNode.Node);
            }

            public override void SetValue(object component, object value)
            {
                ImportUnitNode importNode = component as ImportUnitNode;
                property.SetValue(importNode.Node, value);
            }
        }
    }
}

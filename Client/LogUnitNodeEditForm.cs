using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    partial class LogUnitNodeEditForm : Form
    {
        LogUnitNode logUnitNode;

        public LogUnitNodeEditForm(StructureProvider strucProvider)
        {
            logUnitNode = new LogUnitNode(strucProvider.CurrentNode);
            InitializeComponent();
            propertyGrid1.Site = strucProvider.Session.GetServiceContainer();
            propertyGrid1.SelectedObject = logUnitNode;
            propertyGrid1.CollapseAllGridItems();

            Text = String.Format("Изменения узла {0}", strucProvider.CurrentNode.Text);
        }
    }

    /// <summary>
    /// Обёртка UnitNode'а, для отображение истории ревизий
    /// </summary>
    [TypeConverter(typeof(LogUnitNodeTypeConverter))]
    class LogUnitNode
    {
        public UnitNode UnitNode { get; protected set; }

        public LogUnitNode(UnitNode unitNode)
        {
            this.UnitNode = unitNode;
        }
    }

    /// <summary>
    /// TypeConverter для LogUnitNode.
    /// Для свойств UnitNode'а, отмеченных атрибутом RevisionUnitNodeAttribute, добавляет по строчке для каждой ревизии.
    /// </summary>
    class LogUnitNodeTypeConverter : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            LogUnitNode logUnitNode = value as LogUnitNode;
            Type unitNodeType = logUnitNode.UnitNode.GetType();
            var typeConverterAttribute = unitNodeType.GetCustomAttributes(typeof(TypeConverterAttribute), true).FirstOrDefault(a => a is TypeConverterAttribute) as TypeConverterAttribute;

            var converterType=Type.GetType(typeConverterAttribute.ConverterTypeName);

            TypeConverter baseConverter = converterType.GetConstructor(new Type[] { }).Invoke(new Object[] { }) as TypeConverter;

            var properties = baseConverter.GetProperties(context, logUnitNode.UnitNode, attributes);

            List<PropertyDescriptor> descriptorList = new List<PropertyDescriptor>();

            foreach (PropertyDescriptor item in properties)
            {
                var revAttr = GetRevisionAttribute(item.Attributes);

                if (revAttr != null)
                {
                    IEnumerable<RevisionInfo> revisions;

                    if (revAttr.IsBinary && logUnitNode.UnitNode.Binaries.ContainsKey(revAttr.PropertyName))
                        revisions = logUnitNode.UnitNode.Binaries[revAttr.PropertyName];
                    else if (!revAttr.IsBinary && logUnitNode.UnitNode.Attributes.ContainsKey(revAttr.PropertyName))
                        revisions = logUnitNode.UnitNode.Attributes[revAttr.PropertyName];
                    else revisions = null;

                    if (revisions != null)
                    {
                        foreach (var revision in revisions)
                        {
                            descriptorList.Add(new LogPropertyDescriptor(item, revAttr.PropertyName, revAttr.IsBinary, revision));
                        }
                    }
                }
                else descriptorList.Add(new LogPropertyDescriptor(item, null, false, RevisionInfo.Default));
            }

            //logUnitNode.UnitNode.Revisions
            return new PropertyDescriptorCollection(descriptorList.ToArray());
            //return base.GetProperties(context, value, attributes);
        }

        private RevisionUnitNodeAttribute GetRevisionAttribute(AttributeCollection attributeCollection)
        {
            RevisionUnitNodeAttribute attr;
            
            foreach (Attribute item in attributeCollection)
            {
                if ((attr = item as RevisionUnitNodeAttribute) != null)
                    return attr;
            }
            return null;
        }

        class LogPropertyDescriptor:SimplePropertyDescriptor
        {
            PropertyDescriptor propertyDescriptor;
            String attributeName;
            bool isBinary;
            RevisionInfo revision;

            public LogPropertyDescriptor(PropertyDescriptor propertyDescriptor, String attributeName, bool isBinary, RevisionInfo revision)
                : base(propertyDescriptor.ComponentType, GetName(propertyDescriptor,  attributeName,  revision), propertyDescriptor.PropertyType)
            {
                this.propertyDescriptor = propertyDescriptor;
                this.attributeName = attributeName;
                this.isBinary = isBinary;
                this.revision = revision;
            }

            private static string GetName(PropertyDescriptor propertyDescriptor, string attributeName, RevisionInfo revision)
            {
                if (String.IsNullOrEmpty(attributeName))
                {
                    return propertyDescriptor.Name;
                }
                return String.Format("{0}$${1}", attributeName, revision.ID);
            }

            public override void AddValueChanged(object component, EventHandler handler)
            {
                LogUnitNode logUnitNode = component as LogUnitNode;

                propertyDescriptor.AddValueChanged(logUnitNode.UnitNode, handler);
            }

            //protected override Attribute[] AttributeArray
            //{
            //    get
            //    {
            //        return propertyDescriptor.AttributeArray;
            //    }
            //    set
            //    {
            //        propertyDescriptor.AttributeArray = value;
            //    }
            //}

            public override AttributeCollection Attributes
            {
                get
                {
                    return propertyDescriptor.Attributes;
                }
            }

            public override Type ComponentType
            {
                get
                {
                    return propertyDescriptor.ComponentType;
                }
            }

            public override TypeConverter Converter
            {
                get
                {
                    return propertyDescriptor.Converter;
                }
            }

            protected override AttributeCollection CreateAttributeCollection()
            {
                return base.CreateAttributeCollection();
            }

            public override string Description
            {
                get
                {
                    return propertyDescriptor.Description;
                }
            }

            public override bool DesignTimeOnly
            {
                get
                {
                    return propertyDescriptor.DesignTimeOnly;
                }
            }

            public override string DisplayName
            {
                get
                {
                    return propertyDescriptor.DisplayName;
                }
            }

            //protected override void FillAttributes(System.Collections.IList attributeList)
            //{
            //    propertyDescriptor.FillAttributes(attributeList);
            //}

            public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
            {
                return propertyDescriptor.GetChildProperties(instance, filter);
            }

            public override object GetEditor(Type editorBaseType)
            {
                return propertyDescriptor.GetEditor(editorBaseType);
            }

            //protected override object GetInvocationTarget(Type type, object instance)
            //{
            //    return propertyDescriptor.GetInvocationTarget(type, instance);
            //}

            public override bool IsBrowsable
            {
                get
                {
                    return propertyDescriptor.IsBrowsable;
                }
            }

            public override bool IsLocalizable
            {
                get
                {
                    return propertyDescriptor.IsLocalizable;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return propertyDescriptor.IsReadOnly;
                }
            }

            //protected override int NameHashCode
            //{
            //    get
            //    {
            //        return propertyDescriptor.NameHashCode;
            //    }
            //}

            protected override void OnValueChanged(object component, EventArgs e)
            {
                LogUnitNode logUnitNode = component as LogUnitNode;
                
                base.OnValueChanged(logUnitNode.UnitNode, e);
            }

            public override Type PropertyType
            {
                get
                {
                    return propertyDescriptor.PropertyType;
                }
            }

            public override void RemoveValueChanged(object component, EventHandler handler)
            {
                LogUnitNode logUnitNode = component as LogUnitNode;

                propertyDescriptor.RemoveValueChanged(logUnitNode.UnitNode, handler);
            }

            public override bool CanResetValue(object component)
            {
                LogUnitNode logUnitNode = component as LogUnitNode;

                return propertyDescriptor.CanResetValue(logUnitNode.UnitNode);
            }

            public override void ResetValue(object component)
            {
                LogUnitNode logUnitNode = component as LogUnitNode;

                propertyDescriptor.ResetValue(logUnitNode.UnitNode);
            }

            public override bool ShouldSerializeValue(object component)
            {
                LogUnitNode logUnitNode = component as LogUnitNode;
                
                return propertyDescriptor.ShouldSerializeValue(logUnitNode.UnitNode);
            }

            public override bool SupportsChangeEvents
            {
                get
                {
                    return propertyDescriptor.SupportsChangeEvents;
                }
            }

            //public override string Name
            //{
            //    get
            //    {
            //        return base.Name;
            //    }
            //}

            public override string Category
            {
                get
                {
                    if (RevisionInfo.Default.Equals(revision))
                        return String.Format("(0) {0}", revision);
                    return revision.ToString();
                }
            }

            public override object GetValue(object component)
            {
                LogUnitNode logUnitNode = component as LogUnitNode;
                
                if (!String.IsNullOrEmpty(attributeName))
                {
                    if (isBinary)
                        return logUnitNode.UnitNode.GetBinaries(attributeName, revision);
                    else
                    {
                        String stringValue = logUnitNode.UnitNode.GetAttribute(attributeName, revision);
                        return Convert.ChangeType(stringValue, PropertyType);
                    }
                }
                //throw new NotImplementedException();
                return propertyDescriptor.GetValue(logUnitNode.UnitNode);
            }

            public override void SetValue(object component, object value)
            {
                LogUnitNode logUnitNode = component as LogUnitNode;

                if (!String.IsNullOrEmpty(attributeName))
                {
                    if (isBinary)
                        logUnitNode.UnitNode.SetBinaries(attributeName, revision, value as byte[]);
                    else
                    {
                        String stringValue = Convert.ToString(value);
                        logUnitNode.UnitNode.SetAttribute(attributeName, revision, stringValue); 
                    }
                }
                else
                {
                    //throw new NotImplementedException();
                    propertyDescriptor.SetValue(logUnitNode.UnitNode, value);
                }
            }
        }
    }
}

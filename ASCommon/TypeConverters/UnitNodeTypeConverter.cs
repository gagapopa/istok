using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace COTES.ISTOK.ASC.TypeConverters
{
    /// <summary>
    /// Конвертер всего UnitNode
    /// Отмечает все свойства только для чтения, в зависимости от значения UnitNode.ReadOnly
    /// </summary>
    public class UnitNodeTypeConverter : ExpandableObjectConverter
    {
        private string getCategoryString(CategoryGroup categoryGroup)
        {
            switch (categoryGroup)
            {
                case CategoryGroup.Debug:
                    return "DEBUG";
                case CategoryGroup.General:
                    return "Основное";
                case CategoryGroup.Calc:
                    return "Расчет";
                case CategoryGroup.Values:
                    return "Значения";
                case CategoryGroup.Appearance:
                    return "Внешний вид";
                case CategoryGroup.Load:
                    return "Сбор";
                case CategoryGroup.Misc:
                default:
                    return "Прочие";
            }
        }

        protected void SetCategories(List<PropertyDescriptor> descriptorCollection)
        {
            var groups = (from d in descriptorCollection.OfType<PropertyDescriptorWithCategory>()
                          orderby d.Group, d.SubCategoryOrder, d.SubCategoryString
                          select new { Group = d.Group, SubCategory = d.SubCategoryString }).Distinct().ToList();

            int incr = groups.Contains(new { Group = CategoryGroup.Debug, SubCategory = (String)null }) ? 0 : 1;

            foreach (var descriptor in descriptorCollection.OfType<PropertyDescriptorWithCategory>())
            {
                descriptor.CategoryNum = incr + groups.IndexOf(new { Group = descriptor.Group, SubCategory = descriptor.SubCategoryString });
                descriptor.CategoryString = getCategoryString(descriptor.Group);
            }
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            UnitNode unitNode;

            unitNode = value as UnitNode;
            if (unitNode != null)
            {
                PropertyDescriptorCollection baseCollection = base.GetProperties(context, value, attributes);
                List<PropertyDescriptor> descriptorCollection = new List<PropertyDescriptor>();
                IUnitProviderSupplier unitProviderSupplier = null;
                if (context != null)
                    unitProviderSupplier = context.GetService(typeof(IUnitProviderSupplier)) as IUnitProviderSupplier;
                IUnitNodeProvider unitProvider = null;

                if (unitProviderSupplier != null)
                    unitProvider = unitProviderSupplier.GetProvider(unitNode);

                foreach (PropertyDescriptor descriptor in baseCollection)
                {
                    descriptorCollection.Add(new UnitNodePropertyDescriptor(unitProvider, descriptor));
                    //descriptorCollection.Add(descriptor);
                }

                if (context != null)
                {
                    IAttributeRetrieval attributeRetrieval = context.GetService(typeof(IAttributeRetrieval)) as IAttributeRetrieval;
                    ItemProperty[] attrs = null;

                    if (attributeRetrieval != null)
                        attrs = attributeRetrieval.GetProperties(unitNode);

                    if (attrs != null)
                        foreach (var item in attrs)
                        {
                            descriptorCollection.Add(new UnitNodeAttributesPropertyDescriptor(unitNode.GetType(), unitProvider, attributeRetrieval, item));
                        }
                }

                SetCategories(descriptorCollection);

                return new PropertyDescriptorCollection(descriptorCollection.ToArray()/*, unitNode.ReadOnly*/);
            }

            return base.GetProperties(context, value, attributes);
        }

        protected abstract class PropertyDescriptorWithCategory : SimplePropertyDescriptor
        {
            public int CategoryNum { get; set; }

            public abstract CategoryGroup Group { get; }

            public String CategoryString { get; set; }

            public virtual int SubCategoryOrder { get { return 0; } }

            public virtual String SubCategoryString { get { return null; } }

            public override string Category
            {
                get
                {
                    String category = CategoryString;

                    if (CategoryNum >= 0)
                    {
                        category = String.Format("{0}. {1}", CategoryNum, category);
                    }

                    if (!String.IsNullOrEmpty(SubCategoryString))
                    {
                        category = String.Format("{0}. {1}", category, SubCategoryString);
                    }
                    return category;
                }
            }

            public PropertyDescriptorWithCategory(Type componentType, string name, Type propertyType)
                : base(componentType, name, propertyType)
            {
                CategoryNum = -1;
            }

            public PropertyDescriptorWithCategory(Type componentType, string name, Type propertyType, Attribute[] attributes)
                : base(componentType, name, propertyType, attributes)
            {
                CategoryNum = -1;
            }
        }

        /// <summary>
        /// Свойства добавленные пользователем
        /// </summary>
        class UnitNodeAttributesPropertyDescriptor : PropertyDescriptorWithCategory
        {
            //UnitNode unitNode;
            IUnitNodeProvider unitProvider;
            IAttributeRetrieval attributeRetrieval;
            ItemProperty attribute;

            public UnitNodeAttributesPropertyDescriptor(Type componentType,
                                                        IUnitNodeProvider unitProvider,
                                                        IAttributeRetrieval attributeRetrieval,
                                                        ItemProperty attribute) :
                base(componentType, attribute.Name, typeof(String))
            {
                //this.unitNode = unitNode;
                this.unitProvider = unitProvider;
                this.attributeRetrieval = attributeRetrieval;
                this.attribute = attribute;
            }
            public override string DisplayName
            {
                get
                {
                    if (String.IsNullOrEmpty(attribute.DisplayName))
                        return Name;
                    return attribute.DisplayName;
                }
            }

            public override string Description
            {
                get
                {
                    return attribute.Description;
                }
            }

            public override CategoryGroup Group
            {
                get { return CategoryGroup.Misc; }
            }

            public override string SubCategoryString
            {
                get
                {
                    return attribute.Category;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    if (unitProvider != null)
                        return unitProvider.ReadOnly;
                    return false; //unitNode.ReadOnly;
                }
            }
            public override object GetValue(object component)
            {
                UnitNode unitNode = component as UnitNode;
                if (unitNode != null && unitNode.Attributes.ContainsKey(Name))
                    return unitNode.Attributes[Name];
                return String.Empty;
            }

            public override void SetValue(object component, object value)
            {
                UnitNode unitNode = component as UnitNode;
                if (unitNode != null)
                    unitNode.SetAttribute(Name, value == null ? null : value.ToString());
            }
        }

        /// <summary>
        /// Обертка для стандартного PropertyDescriptor.
        /// </summary>
        protected class UnitNodePropertyDescriptor : PropertyDescriptorWithCategory
        {
            //UnitNode unitNode;
            protected PropertyDescriptor basePropertyDescriptor;
            IUnitNodeProvider unitProvider;

            public UnitNodePropertyDescriptor(IUnitNodeProvider nodeProvider, PropertyDescriptor baseDescriptor)
                : base(baseDescriptor.ComponentType, baseDescriptor.Name, baseDescriptor.PropertyType)
            {
                //this.unitNode = node;
                this.unitProvider = nodeProvider;
                this.basePropertyDescriptor = baseDescriptor;
            }

            RevisionUnitNodeAttribute RevisionAttribute
            {
                get
                {
                    foreach (Attribute attribute in Attributes)
                    {
                        if (attribute is RevisionUnitNodeAttribute)
                            return attribute as RevisionUnitNodeAttribute;
                    }
                    //return (from a in Attributes where a is RevisionUnitNodeAttribute select a).FirstOrDefault() as RevisionUnitNodeAttribute;
                    return null;
                }
            }

            public override void AddValueChanged(object component, EventHandler handler)
            {
                basePropertyDescriptor.AddValueChanged(component, handler);
            }

            //protected override Attribute[] AttributeArray
            //{
            //    get
            //    {
            //        return basePropertyDescriptor.AttributeArray;
            //    }
            //    set
            //    {
            //        basePropertyDescriptor.AttributeArray = value;
            //    }
            //}

            public override AttributeCollection Attributes
            {
                get
                {
                    return basePropertyDescriptor.Attributes;
                }
            }

            public override bool CanResetValue(object component)
            {
                return basePropertyDescriptor.CanResetValue(component);
            }

            public override CategoryGroup Group
            {
                get
                {
                    var group = Attributes.OfType<CategoryOrderAttribute>().FirstOrDefault();
                    if (group != null)
                    {
                        return group.Group;
                    }
                    return CategoryGroup.Misc;
                }
            }

            public override string SubCategoryString
            {
                get
                {
                    if (Group == CategoryGroup.Misc)
                    {
                        return basePropertyDescriptor.Category;
                    }

                    return base.SubCategoryString;
                }
            }

            public override Type ComponentType
            {
                get
                {
                    return basePropertyDescriptor.ComponentType;
                }
            }

            public override TypeConverter Converter
            {
                get
                {
                    return basePropertyDescriptor.Converter;
                }
            }

            //protected override AttributeCollection CreateAttributeCollection()
            //{
            //    return basePropertyDescriptor.CreateAttributeCollection();
            //}

            public override string Description
            {
                get
                {
                    return basePropertyDescriptor.Description;
                }
            }

            public override bool DesignTimeOnly
            {
                get
                {
                    return basePropertyDescriptor.DesignTimeOnly;
                }
            }

            public override string DisplayName
            {
                get
                {
                    return basePropertyDescriptor.DisplayName;
                }
            }

            public override bool Equals(object obj)
            {
                return basePropertyDescriptor.Equals(obj);
            }

            //protected override void FillAttributes(System.Collections.IList attributeList)
            //{
            //    basePropertyDescriptor.FillAttributes(attributeList);
            //}

            public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
            {
                return basePropertyDescriptor.GetChildProperties(instance, filter);
            }

            public override object GetEditor(Type editorBaseType)
            {
                return basePropertyDescriptor.GetEditor(editorBaseType);
            }

            public override int GetHashCode()
            {
                return basePropertyDescriptor.GetHashCode();
            }

            //protected override object GetInvocationTarget(Type type, object instance)
            //{
            //    return basePropertyDescriptor.GetInvocationTarget(type, instance);
            //}

            public override bool IsBrowsable
            {
                get
                {
                    return basePropertyDescriptor.IsBrowsable;
                }
            }

            public override bool IsLocalizable
            {
                get
                {
                    return basePropertyDescriptor.IsLocalizable;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    //if (unitNode != null && unitNode.ReadOnly)
                    //    return true;
                    if (unitProvider != null)
                    {
                        //// Если свойство не верефицируемое и редактируется не дефолтная ревизия
                        //if (RevisionAttribute == null
                        //    && !unitProvider.CurrentRevision.Equals(RevisionInfo.Current)
                        //    && !unitProvider.CurrentRevision.Equals(RevisionInfo.Head))
                        //    return true;
                        return basePropertyDescriptor.IsReadOnly || unitProvider.ReadOnly;
                    }
                    return basePropertyDescriptor.IsReadOnly;
                    //return false;
                }
            }

            public override string Name
            {
                get
                {
                    return basePropertyDescriptor.Name;
                }
            }

            //protected override int NameHashCode
            //{
            //    get
            //    {
            //        return basePropertyDescriptor.NameHashCode;
            //    }
            //}

            //protected override void OnValueChanged(object component, EventArgs e)
            //{
            //    basePropertyDescriptor.OnValueChanged(component, e);
            //}

            public override Type PropertyType
            {
                get
                {
                    return basePropertyDescriptor.PropertyType;
                }
            }

            public override void RemoveValueChanged(object component, EventHandler handler)
            {
                basePropertyDescriptor.RemoveValueChanged(component, handler);
            }

            public override void ResetValue(object component)
            {
                basePropertyDescriptor.ResetValue(component);
            }

            public override object GetValue(object component)
            {
                if (unitProvider != null
                    //&& !unitProvider.CurrentRevision.Equals(RevisionInfo.Head)
                    && RevisionAttribute != null)
                {
                    UnitNode unitNode = component as UnitNode;
                    if (unitNode != null)
                    {
                        RevisionInfo revision = unitProvider.GetRealRevision(unitProvider.CurrentRevision);
                        if (RevisionAttribute.IsBinary)
                        {
                            return unitNode.GetBinaries(RevisionAttribute.PropertyName, revision);
                        }
                        else
                        {
                            String stringValue = unitNode.GetAttribute(RevisionAttribute.PropertyName, revision);
                            return Convert.ChangeType(stringValue, PropertyType, System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                }

                return basePropertyDescriptor.GetValue(component);
            }

            public override void SetValue(object component, object value)
            {
                if (unitProvider != null
                    //&& !unitProvider.RealCurrentRevision.Equals(RevisionInfo.Default)
                    && RevisionAttribute != null)
                {
                    UnitNode unitNode = component as UnitNode;
                    if (unitNode != null)
                    {
                        RevisionInfo revision = unitProvider.GetRealRevision(unitProvider.CurrentRevision);
                        if (RevisionAttribute.IsBinary)
                        {
                            unitNode.SetBinaries(RevisionAttribute.PropertyName, revision, (byte[])value);
                        }
                        else
                        {
                            String stringValue = Convert.ChangeType(value, typeof(String), System.Globalization.CultureInfo.InvariantCulture).ToString();
                            unitNode.SetAttribute(RevisionAttribute.PropertyName, revision, stringValue);
                        }
                    }
                }
                else
                {
                    basePropertyDescriptor.SetValue(component, value);
                }
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;// basePropertyDescriptor.ShouldSerializeValue(component);
            }

            public override bool SupportsChangeEvents
            {
                get
                {
                    return basePropertyDescriptor.SupportsChangeEvents;
                }
            }

            public override string ToString()
            {
                return basePropertyDescriptor.ToString();
            }
        }
    }
}

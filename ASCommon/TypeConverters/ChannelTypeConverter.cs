using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using COTES.ISTOK;

namespace COTES.ISTOK.ASC.TypeConverters
{
    public class ChannelTypeConverter : UnitNodeTypeConverter//ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (context != null)
            {
                IChannelRetrieval channelRetrieval = context.GetService(typeof(IChannelRetrieval)) as IChannelRetrieval;
                IUnitProviderSupplier unitProviderSupplier = context.GetService(typeof(IUnitProviderSupplier)) as IUnitProviderSupplier;
                IUnitNodeProvider unitProvider = null;

                if (channelRetrieval != null)
                {
                    //ChannelNode channel = context.Instance as ChannelNode;
                    ChannelNode channel = value as ChannelNode;
                    if (channel != null)
                    {
                        BlockNode blockNode = channelRetrieval.GetBlockNode(channel);
                        if (blockNode != null)
                        {
                            IEnumerable<ItemProperty> properties = channelRetrieval.GetProperties(blockNode, channel.Libname);
                            PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
                            List<PropertyDescriptor> descriptors = new List<PropertyDescriptor>();

                            if (unitProviderSupplier != null)
                                unitProvider = unitProviderSupplier.GetProvider(channel);

                            List<String> sortNames = new List<string>();
                            collection = collection.Sort();
                            if (collection != null)
                                foreach (PropertyDescriptor descriptor in collection)
                                {
                                    descriptors.Add(descriptor);
                                    sortNames.Add(descriptor.Name);
                                }

                            if (properties != null)
                                foreach (ItemProperty ItemProperty in properties)
                                {
                                    descriptors.Add(new ItemPropertyDescriptor(unitProvider, value.GetType(), channelRetrieval, ItemProperty));
                                    sortNames.Add(ItemProperty.Name);
                                }

                            SetCategories(descriptors);

                            collection = new PropertyDescriptorCollection(descriptors.ToArray());

                            return collection.Sort(sortNames.ToArray());
                        }
                    }
                }
            }

            return base.GetProperties(context, value, attributes);
        }

        class ItemPropertyDescriptor : PropertyDescriptorWithCategory
        {
            //ChannelNode channelNode;
            IUnitNodeProvider unitProvider;
            IChannelRetrieval channelRetrievar;
            public ItemProperty ItemProperty { get; private set; }

            public ItemPropertyDescriptor(IUnitNodeProvider unitProvider, Type componentType, IChannelRetrieval channelRetriever, ItemProperty ItemProperty) :
                base(componentType, ItemProperty.Name, typeof(String))
            {
                //this.channelNode = channelNode;
                this.unitProvider = unitProvider;
                this.channelRetrievar = channelRetriever;
                this.ItemProperty = ItemProperty;
            }
            public override string DisplayName
            {
                get
                {
                    if (String.IsNullOrEmpty(ItemProperty.DisplayName))
                        return Name;
                    return ItemProperty.DisplayName;
                }
            }

            public override string Description
            {
                get
                {
                    return ItemProperty.Description;
                }
            }

            public override CategoryGroup Group
            {
                get { return CategoryGroup.Load; }
            }

            public override int SubCategoryOrder
            {
                get
                {
                    return SubCategoryString == "Общие" ? 0 : 1;
                }
            }

            public override string SubCategoryString
            {
                get
                {
                    return ItemProperty.Category;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return unitProvider.ReadOnly;
                }
            }

            public override object GetValue(object component)
            {
                ChannelNode channelNode = component as ChannelNode;
                if (channelNode != null && channelNode.Attributes.ContainsKey(Name))
                {
                    return TypeDescriptor.GetConverter(ItemProperty.ValueType).ConvertFromInvariantString(channelNode.GetAttribute(Name));
                }
                else if (!String.IsNullOrEmpty(ItemProperty.DefaultValue))
                {
                    var innerConverter = TypeDescriptor.GetConverter(PropertyType);
                    return innerConverter.ConvertFromInvariantString(ItemProperty.DefaultValue);
                }
                return String.Empty;
            }

            public override void SetValue(object component, object value)
            {
                ChannelNode channelNode = component as ChannelNode;
                if (channelNode != null)
                {
                    //String val = null;
                    //if (value != null) val = value.ToString();
                    //channelNode.SetAttribute(Name, value == null ? null : value.ToString());
                    channelNode.SetAttribute(Name, TypeDescriptor.GetConverter(ItemProperty.ValueType).ConvertToInvariantString(value));
                }
            }

            public override Type PropertyType
            {
                get
                {
                    if (ItemProperty.ValueType == null)
                    {
                        return base.PropertyType;
                    }
                    return ItemProperty.ValueType;// base.ComponentType;
                }
            }

            public override AttributeCollection Attributes
            {
                get
                {
                    //var attributes = base.Attributes;

                    //var hasTypeConverter = (from a in attributes.AsQueryable() where a.GetType().IsSubclassOf(typeof(TypeConverterAttribute)) select a).Count() > 0;

                    //if (!hasTypeConverter)
                    //{

                    //}

                    List<Attribute> attributeList = new List<Attribute>();

                    foreach (Attribute item in base.Attributes)
                    {
                        attributeList.Add(item);
                    }

                    attributeList.Add(new TypeConverterAttribute(typeof(ItemPropertyTypeConverter)));

                    return new AttributeCollection(attributeList.ToArray());
                }
            }
        }

        class ItemPropertyTypeConverter : TypeConverter
        {
            ItemPropertyDescriptor _descriptor;

            ItemPropertyDescriptor GetDescriptor(ITypeDescriptorContext context)
            {
                if (_descriptor == null && context != null)
                {
                    _descriptor = context.PropertyDescriptor as ItemPropertyDescriptor;
                }

                return _descriptor;
            }


            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                ItemPropertyDescriptor descriptor = GetDescriptor(context);
                if (descriptor != null)
                {
                    return descriptor.ItemProperty.HasStandardValues;
                }
                else
                {
                    return base.GetStandardValuesSupported(context);
                }
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                ItemPropertyDescriptor descriptor = GetDescriptor(context);
                if (descriptor != null)
                {
                    return descriptor.ItemProperty.StandardValuesAreExtinct;
                }
                else
                {
                    return base.GetStandardValuesExclusive(context);
                }
            }

            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                ItemPropertyDescriptor descriptor = GetDescriptor(context);
                if (descriptor != null)
                {
                    var innerConverter = TypeDescriptor.GetConverter(descriptor.PropertyType);
                    var values = from s in descriptor.ItemProperty.StandardValues select innerConverter.ConvertFromInvariantString(s);
                    return new StandardValuesCollection(values.ToArray()); //descriptor.ItemProperty.StandardValues);
                }
                else
                {
                    return base.GetStandardValues(context);
                }
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                ItemPropertyDescriptor descriptor = GetDescriptor(context);
                if (descriptor != null)
                {
                    var innerConverter = TypeDescriptor.GetConverter(descriptor.PropertyType);
                    return innerConverter.CanConvertFrom(context, sourceType);
                }
                else
                {
                    return base.CanConvertFrom(context, sourceType);
                }
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                ItemPropertyDescriptor descriptor = GetDescriptor(context);
                if (descriptor != null)
                {
                    var innerConverter = TypeDescriptor.GetConverter(descriptor.PropertyType);
                    return innerConverter.ConvertFrom(context, culture, value);
                }
                else
                {
                    return base.ConvertFrom(context, culture, value);
                }
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                ItemPropertyDescriptor descriptor = GetDescriptor(context);
                if (descriptor != null)
                {
                    var innerConverter = TypeDescriptor.GetConverter(descriptor.PropertyType);
                    return innerConverter.CanConvertTo(context, destinationType);
                }
                else
                {
                    return base.CanConvertTo(context, destinationType);
                } 
            }
           
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                ItemPropertyDescriptor descriptor = GetDescriptor(context);
                if (descriptor != null)
                {
                    var innerConverter = TypeDescriptor.GetConverter(descriptor.PropertyType);
                    try
                    {
                        return innerConverter.ConvertTo(context, culture, value, destinationType);
                    }
                    catch (FormatException)
                    {
                        return null;
                    }
                }
                else
                {
                    return base.ConvertTo(context, culture, value, destinationType);
                }
            }
        }
    }
}

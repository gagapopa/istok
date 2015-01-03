using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace COTES.ISTOK.WebClient.Models
{
    public static class PropertyFormatter
    {
        public static IRemoteDataService rds;

        public static IEnumerable<CategoryProperties> GetProperties(COTES.ISTOK.ClientCore.Session session, object component)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(component);
            MyTypeDescriptorContext context = new MyTypeDescriptorContext(session.GetServiceContainer());
            var tmp = tc.GetProperties(context, component);

            List<CategoryProperties> lstCats = new List<CategoryProperties>();
            Dictionary<string, List<Property>> dic = new Dictionary<string, List<Property>>();

            foreach (PropertyDescriptor item in tmp)
            {
                if (item.IsBrowsable)
                {
                    if (!dic.ContainsKey(item.Category)) dic[item.Category] = new List<Property>();
                    dic[item.Category].Add(new Property() { Descriptor = item, Component = component });
                }
            }

            foreach (var item in dic.Keys)
            {
                List<Property> lstProps = dic[item];
                lstCats.Add(new CategoryProperties()
                {
                    Category = item,
                    Properties = lstProps.ToArray()
                });
            }

            return lstCats.ToArray();
        }
        public static IEnumerable<CategoryProperties> GetNodeProperties(UnitProvider unitProvider)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(unitProvider.UnitNode);
            FormOrientedServiceContainer fcont = new FormOrientedServiceContainer(unitProvider.StructureProvider.Session.GetServiceContainer());
            fcont.AddService(unitProvider.StructureProvider);
            MyTypeDescriptorContext context = new MyTypeDescriptorContext(fcont);
            var tmp = tc.GetProperties(context, unitProvider.UnitNode);

            //PropertyDescriptorCollection tmp = TypeDescriptor.GetProperties(component);
            List<CategoryProperties> lstCats = new List<CategoryProperties>();
            Dictionary<string, List<Property>> dic = new Dictionary<string, List<Property>>();
            
            foreach (PropertyDescriptor item in tmp)
            {
                if (item.IsBrowsable)
                {
                    if (!dic.ContainsKey(item.Category)) dic[item.Category] = new List<Property>();
                    dic[item.Category].Add(new Property() { Descriptor = item, Component = unitProvider.UnitNode });
                }
            }

            foreach (var item in dic.Keys)
            {
                List<Property> lstProps = dic[item];
                lstCats.Add(new CategoryProperties()
                {
                    Category = item,
                    Properties = lstProps.ToArray()
                });
            }

            return lstCats.ToArray();
        }

        public static string GetPropertyValue(UnitProvider unitProvider, Property property)
        {
            FormOrientedServiceContainer fcont = new FormOrientedServiceContainer(unitProvider.StructureProvider.Session.GetServiceContainer());
            fcont.AddService(unitProvider.StructureProvider);
            MyTypeDescriptorContext context = new MyTypeDescriptorContext(fcont, property.Descriptor);
            var tmp = property.Descriptor.Converter.ConvertTo(context, System.Globalization.CultureInfo.CurrentCulture, property.Descriptor.GetValue(property.Component), typeof(string)) as string;
            return tmp;
        }
    }

    public class CategoryProperties
    {
        public string Category { get; set; }
        public IEnumerable<Property> Properties { get; set; }
    }
    public class Property
    {
        public PropertyDescriptor Descriptor { get; set; }
        public object Component { get; set; }
    }
}
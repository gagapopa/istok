using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace WebClient
{
    /// <summary>
    /// Описатель свойств объекта.
    /// Описывает свойства объекта для отображения
    /// в проперти гриде.
    /// </summary>
    [Serializable]
    public class ObjectPropertyDescriptor
    {
        private UnitObjectDescriptor descriptor;

        /// <summary>
        /// Категория свойства.
        /// </summary>
        public string Category { get; private set; }
        /// <summary>
        /// Отображаемое имя свойства.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Пояснение к свойству.
        /// </summary>
        public string Tooltip { get; private set; }

        public Type ValueType { get; private set; }
        /// <summary>
        /// Значение свойства.
        /// </summary>
        public object Value { get; private set; }

        private bool readOnly;

        public bool ReadOnly
        {
            get { return readOnly && descriptor.ReadOnly; }
            set { readOnly = value; }
        }

        public TypeConverter TypeConverter { get; private set; }

        public bool ValidateValue(Object value) { return true; }

        /// <summary>
        /// Конструктор. Формирует свойства на основании
        /// данных полученных через рефлексию.
        /// </summary>
        /// <param name="obj">
        ///     Экземпляр объекта, которому принадлежат свойства.
        /// </param>
        /// <param name="property">
        ///     Описание и сервисная информация свойства.
        /// </param>
        /// <param name="service_container">
        ///     Контайнер сервисов, для конвертирования значений.
        /// </param>
        public ObjectPropertyDescriptor(UnitObjectDescriptor descriptor,
                                        object obj, 
                                        PropertyInfo property)
                                        //WebClientServiceContainer service_container)
        {
            this.descriptor = descriptor;

            var attribyte_list = property.GetCustomAttributes(true);
            object temp = GetAttribute(attribyte_list, typeof(DisplayNameAttribute));
            if (temp != null)
                Name = (temp as DisplayNameAttribute).DisplayName;
            else
                Name = property.Name;

            temp = GetAttribute(attribyte_list, typeof(CategoryAttribute));
            if (temp != null)
                Category = (temp as CategoryAttribute).Category;
            else
                Category = @"";

            temp = GetAttribute(attribyte_list, typeof(DescriptionAttribute));
            if (temp != null)
                Tooltip = (temp as DescriptionAttribute).Description;
            else
                Tooltip = @"";

            try
            {
                ValueType = property.PropertyType;
                Value = property.GetValue(obj, null);

                temp = GetAttribute(attribyte_list, typeof(TypeConverterAttribute));
                if (temp != null)
                {
                    Type converter_type = Type.GetType((temp as TypeConverterAttribute).ConverterTypeName);
                    TypeConverter converter = Activator.CreateInstance(converter_type) as TypeConverter;
                    //Value = converter.ConvertTo(service_container,
                    //                            CultureInfo.CurrentCulture,
                    //                            Value,
                    //                            typeof(String));
                    TypeConverter = converter;
                }
            }
            catch
            { Value = @""; }
        }

        private static object GetAttribute(object[] attributes, Type attribute_type)
        {
            return attributes.FirstOrDefault((object x) => 
                        Type.Equals(x.GetType(), attribute_type));
        }
    }
}

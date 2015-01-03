using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.ComponentModel;

namespace WebClient
{
    /// <summary>
    /// Описатель объекта.
    /// Содержит описатели свойств объекта.
    /// </summary>
    public class UnitObjectDescriptor : IEnumerable<string> 
    {
        /// <summary>
        /// Описатели свойств объекта.
        /// </summary>
        private Dictionary<string, List<ObjectPropertyDescriptor>> properties =
            new Dictionary<string, List<ObjectPropertyDescriptor>>();
        /// <summary>
        /// Заголовок объекта.
        /// </summary>
        public string Caption { get; private set; }

        public bool ReadOnly { get; set; }

        /// <summary>
        /// Конструктор.
        /// Через рефлексию исследует объект и строит его описание.
        /// </summary>
        /// <param name="obj">
        ///     Объект.
        /// </param>
        /// <param name="container">
        ///     Сервис контейнет для конвертирования значений свойств объекта.
        /// </param>
        public UnitObjectDescriptor(object obj)//, 
                                    //WebClientServiceContainer container)
        {
            Caption = obj.ToString();

            PropertyInfo[] properties_info = obj.GetType().GetProperties();

            ObjectPropertyDescriptor temp = null;
            foreach (var it in properties_info)
                if (IsBrowsable(it))
                {
                    temp = new ObjectPropertyDescriptor(this,
                                                        obj,
                                                        it);//,
                                                        //container);

                    if (!properties.ContainsKey(temp.Category))
                        properties.Add(temp.Category, new List<ObjectPropertyDescriptor>());

                    properties[temp.Category].Add(temp);
                }
        }

        /// <summary>
        /// Индексатор свойств.
        /// </summary>
        /// <param name="key">
        ///     Имя свойств.
        /// </param>
        /// <returns>
        ///     Описатель свойства.
        /// </returns>
        public List<ObjectPropertyDescriptor> this[string key]
        {
            get
            {
                return properties.ContainsKey(key) ?
                    properties[key] : null;
            }
        }

        #region IEnumerable<string>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return properties.Keys.GetEnumerator();
        }
        #endregion

        private bool IsBrowsable(PropertyInfo property)
        {
            object[] attributes = property.GetCustomAttributes(true);

            object browsable_attribute = attributes.FirstOrDefault((object x) => x is BrowsableAttribute);
            if (browsable_attribute == null)
                return true;

            return (browsable_attribute as BrowsableAttribute).Browsable;
        }
    }
}

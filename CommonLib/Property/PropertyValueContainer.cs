using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    /// <summary>
    /// Контэйнер для хранений свойств-значений
    /// </summary>
    /// <typeparam name="T">Тип данных значений свойств</typeparam>
    [Serializable]
    class PropertyValueContainer<T> : IPropertyable<T>
    {
        Dictionary<ItemProperty, T> valueDictionary = new Dictionary<ItemProperty, T>();

        public bool Contains(ItemProperty property)
        {
            return valueDictionary.ContainsKey(property);
        }

        public bool Contains(String propertyName)
        {
            return Contains(new ItemProperty() { Name = propertyName });
        }

        public IEnumerable<ItemProperty> Properties
        {
            get { return valueDictionary.Keys.ToArray(); }
        }

        public T this[ItemProperty property]
        {
            get { return GetPropertyValue(property); }
            set { SetPropertyValue(property, value); }
        }

        public T GetPropertyValue(ItemProperty property)
        {
            T value;
            if (valueDictionary.TryGetValue(property, out value))
            {
                return value;
            }
            else if (!String.IsNullOrEmpty(property.DefaultValue))
            {
                return (T)System.ComponentModel.TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(property.DefaultValue);
            }
            return default(T);
        }

        public T GetPropertyValue(string propertyName)
        {
            return GetPropertyValue(new ItemProperty() { Name = propertyName });
        }

        public void SetPropertyValue(ItemProperty property, T value)
        {
            valueDictionary[property] = value;
        }

        public void SetPropertyValue(string propertyName, T value)
        {
            SetPropertyValue(new ItemProperty() { Name = propertyName }, value);
        }
    }
}

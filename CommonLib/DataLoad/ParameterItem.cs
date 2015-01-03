using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    /// <summary>
    /// Описание собираемого параметра
    /// </summary>
    [Serializable]
    public class ParameterItem : IPropertyable<String>
    {
        /// <summary>
        /// ИД параметра в базе
        /// </summary>
        public int Idnum { get; set; }

        /// <summary>
        /// Имя параметра
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Время последнего полученного значения параметра
        /// </summary>
        public DateTime LastTime { get; set; }

        /// <summary>
        /// Число дней хранения значения в архиве
        /// </summary>
        public int Store_days { get; set; }

        #region IPropertyable<string> Members

        PropertyValueContainer<String> propertyContainer = new PropertyValueContainer<String>();

        public bool Contains(ItemProperty property)
        {
            return propertyContainer.Contains(property);
        }

        public bool Contains(String propertyName)
        {
            return propertyContainer.Contains(propertyName);
        }

        public IEnumerable<ItemProperty> Properties
        {
            get
            {
                return propertyContainer.Properties;
            }
        }

        public string this[ItemProperty property]
        {
            get
            {
                return propertyContainer[property];
            }
            set
            {
                propertyContainer[property] = value;
            }
        }

        public string GetPropertyValue(ItemProperty property)
        {
            return propertyContainer.GetPropertyValue(property);
        }

        public string GetPropertyValue(string propertyName)
        {
            return propertyContainer.GetPropertyValue(propertyName);
        }

        public void SetPropertyValue(ItemProperty property, string value)
        {
            propertyContainer.SetPropertyValue(property, value);
        }

        public void SetPropertyValue(string propertyName, string value)
        {
            propertyContainer.SetPropertyValue(propertyName, value);
        }
        #endregion

        public override bool Equals(object obj)
        {
            ParameterItem param = obj as ParameterItem;
            if (param!=null)
            {
                return param.Idnum.Equals(Idnum);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Idnum.GetHashCode();
        }
    }
}
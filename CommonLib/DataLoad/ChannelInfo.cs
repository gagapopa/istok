using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    /// <summary>
    /// Описание канала сбора
    /// </summary>
    [Serializable]
    public class ChannelInfo : IPropertyable<String>
    {
        /// <summary>
        /// ИД в базе
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Имя канала
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Информация по модулю сбора
        /// </summary>
        public ModuleInfo Module { get; set; }

        /// <summary>
        /// Собираемые параметры
        /// </summary>
        public IEnumerable<ParameterItem> Parameters { get; set; }

        /// <summary>
        /// Канал активен
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Данные канала должны сохранятся в базе
        /// </summary>
        public bool Storable { get; set; }

        /// <summary>
        /// Канал приостановлен
        /// </summary>
        public bool Suspended { get; set; }

        /// <summary>
        /// Наяальное время сбора параметра
        /// </summary>
        public DateTime StartTime { get; set; }

        #region IPropertyable<string> Members

        private PropertyValueContainer<String> propertyValueContainer = new PropertyValueContainer<String>();

        public bool Contains(ItemProperty property)
        {
            return propertyValueContainer.Contains(property);
        }

        public bool Contains(String propertyName)
        {
            return propertyValueContainer.Contains(propertyName);
        }
        public IEnumerable<ItemProperty> Properties
        {
            get
            {
                return propertyValueContainer.Properties;
            }
        }

        public string this[ItemProperty property]
        {
            get
            {
                return propertyValueContainer[property];
            }
            set
            {
                propertyValueContainer[property] = value;
            }
        }

        public string GetPropertyValue(ItemProperty property)
        {
            return propertyValueContainer.GetPropertyValue(property);
        }

        public string GetPropertyValue(string propertyName)
        {
            return propertyValueContainer.GetPropertyValue(propertyName);
        }

        public void SetPropertyValue(ItemProperty property, string value)
        {
            propertyValueContainer.SetPropertyValue(property, value);
        }

        public void SetPropertyValue(string propertyName, string value)
        {
            propertyValueContainer.SetPropertyValue(propertyName, value);
        }

        #endregion

        public override bool Equals(object obj)
        {
            ChannelInfo info = obj as ChannelInfo;

            if (info != null)
            {
                return info.Id.Equals(Id);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("'{0}' [{1}, {2}]", Name, Id, Module.Name);
            //return base.ToString();
        }
    }
}
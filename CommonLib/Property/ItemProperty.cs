using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK
{
    /// <summary>
    /// Свойство
    /// </summary>
    [Serializable]
    [DataContract]
    public class ItemProperty
    {
        /// <summary>
        /// Имя свойства
        /// </summary>
        [DataMember]
        public String Name { get; set; }

        /// <summary>
        /// Отображаемое имя свойства, для отображения на пользовательском интерфейсе
        /// </summary>
        [DataMember]
        public String DisplayName { get; set; }

        /// <summary>
        /// Описание свойства, для отображения на пользовательском интерфейсе
        /// </summary>
        [DataMember]
        public String Description { get; set; }

        /// <summary>
        /// Категория свойства, для отображения на пользовательском интерфейсе
        /// </summary>
        [DataMember]
        public String Category { get; set; }

        private Type valueType;

        /// <summary>
        /// Тип данных свойства
        /// </summary>
        public Type ValueType
        {
            get
            {
                if (valueType == null
                    && !String.IsNullOrEmpty(ValueTypeName))
                {
                    valueType = Type.GetType(ValueTypeName);
                }
                return valueType;
            }
            set
            {
                valueType = value;
                if (valueType != null)
                {
                    ValueTypeName = valueType.FullName;
                }
                else
                {
                    ValueTypeName = null;
                }
            }
        }

        [DataMember]
        public String ValueTypeName { get; private set; }

        /// <summary>
        /// Есть для данного свойства стандартные значения
        /// </summary>
        [DataMember]
        public bool HasStandardValues { get; set; }

        /// <summary>
        /// Являются ли стандартные свойства исключительными
        /// </summary>
        [DataMember]
        public bool StandardValuesAreExtinct { get; set; }

        /// <summary>
        /// Список стандартных значений
        /// </summary>
        [DataMember]
        public String[] StandardValues { get; set; }

        public object PropertyValue { get; set; }

        /// <summary>
        /// Значение по умолчанию для данного свойства
        /// </summary>
        [DataMember]
        public String DefaultValue { get; set; }

        public override bool Equals(object obj)
        {
            ItemProperty itemProperty = obj as ItemProperty;

            if (itemProperty != null)
            {
                return String.Equals(Name, itemProperty.Name);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (Name ?? String.Empty).GetHashCode();
        }
    }
}
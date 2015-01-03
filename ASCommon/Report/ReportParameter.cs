using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Параметры отчета.
    /// Значения параметров настраивается в PropertyGrid'е при формирвоании и при редактировании отчёта.
    /// А так же значения параметров передаются в сам отчёт.
    /// </summary>
    [Serializable]
    public class ReportParameter
    {
        public ReportParameter(String name, String displayName, String description, String category, String propertyType, String typeConverter, Object defaultValue)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.Description = description;
            this.Category = category;
            this.PropertyType = propertyType;
            this.TypeConverter = typeConverter;
            SetValue(defaultValue);
        }

        public ReportParameter(String name, String displayName, String description, String category, String propertyType, Object defaultValue)
            : this(name, displayName, description, category, propertyType, null, defaultValue)
        { }

        [NonSerialized]
        private ReportSourceSettings baseSettings;

        public ReportSourceSettings BaseSettings
        {
            get { return baseSettings; }
            set { baseSettings = value; }
        }

        /// <summary>
        /// Имя параметра для отчётта
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Тип параметра отчёта
        /// </summary>
        public String PropertyType { get; private set; }

        /// <summary>
        /// Отображаемое имя парамтера для PropertyGrid'а
        /// </summary>
        public String DisplayName { get; private set; }

        /// <summary>
        /// Описание парамтера для PropertyGrid'а
        /// </summary>
        public String Description { get; private set; }

        /// <summary>
        /// Категория парамтера для PropertyGrid'а
        /// </summary>
        public String Category { get; private set; }

        public String TypeConverter { get; private set; }

        public bool RequiredString { get; set; }

        public String StringValue { get; set; }

        Object parameterValue;

        /// <summary>
        /// Получить значения параметра для отчётов
        /// </summary>
        public Object GetValue()
        {
            return parameterValue;
        }

        /// <summary>
        /// Установить значения параметра. Используется для отображения параметров сформированных отчётов.
        /// </summary>
        public void SetValue(Object value)
        {
            Type type = Type.GetType(PropertyType);

            //if (PropertyType.IsInstanceOfType(value))
            if (type.IsInstanceOfType(value))
                parameterValue = value;
        }
    }
}

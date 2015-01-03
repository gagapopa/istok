using System;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Класс представления информации о аргументе условного параметра или функции
    /// </summary>
    [Serializable]
    public class CalcArgumentInfo
    {
        /// <summary>
        /// Имя параметра
        /// </summary>
        public String Name { get; protected set; }

        /// <summary>
        /// Описание или комментарии параметра
        /// </summary>
        public String Description { get; protected set; }

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        public String DefaultValue { get; protected set; }

        /// <summary>
        /// Является ли параметр входным или выходным
        /// </summary>
        public ParameterAccessor ParameterAccessor { get; protected set; }

        public CalcArgumentInfo(String name, String description, String defaultValue, ParameterAccessor accessor)
        {
            this.Name = name;
            this.Description = description;
            this.DefaultValue = defaultValue;
            this.ParameterAccessor = accessor;
        }

        public CalcArgumentInfo(String name, String description, ParameterAccessor accessor)
            : this(name, description, null, accessor)
        { }
    }
}

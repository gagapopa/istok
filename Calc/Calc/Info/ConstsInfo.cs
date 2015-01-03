using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Информация о константе
    /// </summary>
    [Serializable]
    public class ConstsInfo
    {
        /// <summary>
        /// Идентификатор константы
        /// </summary>
        public int ID { get; protected set; }

        /// <summary>
        /// Наимнование константы
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// Можно ли редактировать константу в клиенте
        /// </summary>
        public bool Editable { get; private set; }

        public ConstsInfo(int id, String name, String description, String value)
        {
            this.ID = id;
            this.Editable = true;
            this.Name = name;
            this.Description = description;
            this.Value = value;
        }

        public ConstsInfo(String name, String description, String value)
            : this(0, name, description, value) { }

        public ConstsInfo(bool editable, String name, String description, String value)
            : this(0, name, description, value)
        {
            this.Editable = editable;
        }
    }
}

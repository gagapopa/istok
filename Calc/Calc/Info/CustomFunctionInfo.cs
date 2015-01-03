using System;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Информация о пользовательской функции
    /// </summary>
    [Serializable]
    public class CustomFunctionInfo : FunctionInfo
    {
        public CustomFunctionInfo(int id, String name, String description, String groupName, String text)
            : base(name, description, groupName)
        {
            this.ID = id;
            this.Name = name;
            this.Comment = description;
            this.GroupName = groupName;
            this.Text = text;
        }

        public int ID { get; protected set; }

        /// <summary>
        /// Текст функции
        /// </summary>
        public String Text { get; set; }
    }
}

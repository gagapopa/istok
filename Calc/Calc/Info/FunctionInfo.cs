using System;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Информация о функции используемой в расчете
    /// </summary>
    [Serializable]
    public class FunctionInfo
    {
        /// <summary>
        /// Наименование функции
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Параметры функции
        /// </summary>
        public CalcArgumentInfo[] Arguments { get; set; }

        /// <summary>
        /// Группа функции (для группировки функций в редакторе формул)
        /// </summary>
        public String GroupName { get; set; }

        /// <summary>
        /// Примечания к функции
        /// </summary>
        public String Comment { get; set; }

        public FunctionInfo(String name, String description, String groupName)
        {
            this.Name = name;
            this.Comment = description;
            this.GroupName = groupName;
        }

        internal FunctionInfo(Function func)
        {
            Name = func.Name;
            GroupName = func.GroupName;
            Comment = func.Comment;
            Arguments = func.Parameters;
        }
    }
}

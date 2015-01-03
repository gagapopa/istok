using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    /// <summary>
    /// Информация о ревизии
    /// </summary>
    [Serializable]
    public class RevisionInfo
    {
        /// <summary>
        /// Ревизия по умолчанию. Начинает действовать с DateTime.MinValue
        /// </summary>
        public static readonly RevisionInfo Default = new RevisionInfo { ID = -1 };

        /// <summary>
        /// Старшая ревизия. Для каждого элемента может быть разной.
        /// </summary>
        public static readonly RevisionInfo Head = new RevisionInfo { ID = -2 };

        /// <summary>
        /// Текущая ревизия, должна использоваться только на стороне клиента
        /// </summary>
        public static readonly RevisionInfo Current = new RevisionInfo { ID = -3 };

        /// <summary>
        /// ИД ревизии
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Время начала действия ревизии
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Краткое описание ревизии
        /// </summary>
        public String Brief { get; set; }

        /// <summary>
        /// Описание ревизии
        /// </summary>
        public String Comment { get; set; }

        public RevisionInfo()
        {

        }

        public RevisionInfo(RevisionInfo revision)
            : this()
        {
            ID = revision.ID;
            Time = revision.Time;
            Brief = revision.Brief;
            Comment = revision.Comment;
        }

        public override bool Equals(object obj)
        {
            RevisionInfo revision = obj as RevisionInfo;

            if (revision != null)
                return ID.Equals(revision.ID);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            if(this.Equals(RevisionInfo.Default))
                return "[НАЧАЛЬНАЯ]";
            if (this.Equals(RevisionInfo.Head))
                return "[ТЕКУЩАЯ]";
            if(this.Equals(RevisionInfo.Current))
                return "[РАБОЧИЯ]";
            return String.Format("{2}({0:yyyy-MM-dd}) {1}", Time, Brief, ID == 0 ? "[НОВАЯ]" : "");
        }
    }
}

using System;
using System.Data;
using System.Runtime.Serialization;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Представление для группы
    /// </summary>
    [Serializable]
    [DataContract]
    public class GroupNode : ServerNode
    {
        protected String description = null;

        /// <summary>
        /// Получить, установить описание группы
        /// </summary>
        [DataMember]
        public String Description
        {
            get { return description; }
            set { description = value; }
        }

        public GroupNode() : base() { }
        public GroupNode(DataRow row)
        {
            idnum = (int)row["idnum"];
            Text = row["name"].ToString();
            description = row["description"].ToString();
        }

        public override object[] ToCells()
        {
            return new object[] { idnum, Text, description };
        }
        public override object Clone()
        {
            GroupNode res = new GroupNode();
            res.idnum = Idnum;
            res.Text = Text;

            res.description = description;
            return res;
        }

        public override bool Equals(object obj)
        {
            GroupNode node = obj as GroupNode;
            
            if (node == null) return false;
            if (!base.Equals(obj)) return false;
            return Description == node.Description;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

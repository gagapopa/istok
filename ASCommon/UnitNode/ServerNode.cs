using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    [DataContract]
    [KnownType(typeof(UserNode))]
    [KnownType(typeof(GroupNode))]
    [KnownType(typeof(UnitNode))]
    [KnownType(typeof(UTypeNode))]
    public abstract class ServerNode : ICloneable, IComparable<ServerNode>
    {
        [DataMember]
        protected int idnum;
#if DEBUG
        [Browsable(true)]
        [CategoryOrder(CategoryGroup.Debug)]
        [DisplayName("Тип узла")]
        public String UnitNodeType { get { return GetType().ToString(); } }
#endif

#if DEBUG
        [Browsable(true)]
        [DisplayName("ИД в базе")]
        [CategoryOrder(CategoryGroup.Debug)]
        [ReadOnly(true)]        
#else
        [Browsable(false)] 
#endif
        public int Idnum { get { return idnum; } set { idnum = value; } }

        [Description("")]
        [DisplayName("Название")]
        [CategoryOrder(CategoryGroup.General)]
        [DataMember]
        public string Text { get; set; }

        public ServerNode()
        {}
        
        public ServerNode(int _idnum)
            : this()
        { this.idnum = _idnum; }
        
        public ServerNode(int _idnum, string _text)
            : this(_idnum)
        { this.Text = _text; }

        public ServerNode(DataRow row)
        {
            idnum = (int)row["idnum"];
            Text = row["name"].ToString();
        }

        public override string ToString()
        {
            return this.Text;
        }

        public virtual object[] ToCells()
        {
            return new object[] { Idnum, Text };
        }

        public virtual void AcceptChanges()
        {
            // ???
        }

        public override bool Equals(object obj)
        {
            ServerNode node = obj as ServerNode;

            if (node == null) return false;
            if (this == node) return true;
            if (!Text.Equals(node.Text)) return false;
            if (Idnum != node.Idnum) return false;
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 17;

                result += idnum * 31;
                result += Text != null ? Text.GetHashCode() : 0;

                return base.GetHashCode();
            }
        }

        #region ICloneable Members

        public virtual object Clone()
        {
            //ServerNode res = new ServerNode(this.Idnum, this.Text);
            //res.editor = Editor;
            //res.modified = Modified;
            //return res;
            throw new Exception("Метод не реализован.");
        }

        #endregion
        #region IComparable<ServerNode> Members

        public int CompareTo(ServerNode other)
        {
            return String.Compare(Text, other.Text);
        }

        #endregion
        }
}

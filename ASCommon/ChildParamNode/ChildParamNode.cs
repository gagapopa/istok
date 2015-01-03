using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using COTES.ISTOK.ASC.TypeConverters;
using System.Collections;
using System.Runtime.Serialization;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    [TypeConverter(typeof(UnitNodeTypeConverter))]
    [KnownType(typeof(GraphParamNode))]
    [KnownType(typeof(HistogramParamNode))]
    [KnownType(typeof(SchemaParamNode))]
    [KnownType(typeof(MonitorTableParamNode))]
    [DataContract]
    public class ChildParamNode
    {
        protected static DoubleConverter doubleconv = new DoubleConverter();
        protected static DateTimeConverter dateconv = new DateTimeConverter();

        public virtual void SetParameter(ParameterNode paramnode)
        {
            if (paramnode == null) return;
            parameterId = paramnode.Idnum;
            parameterFullName = paramnode.FullName;
            Text = paramnode.ToString();
        }
        [DataMember]
        protected UnitNode parentNode = null;
        [Browsable(false)]
        public UnitNode Parent { get { return parentNode; } }
        public virtual void SetParent(UnitNode value)
        {
            parentNode = value;
        }
        public virtual void SetIdnum(int value)
        {
            idnum = value;
        }

        [DataMember]
        protected int idnum = 0;
        [Browsable(false)]
        public int Idnum { get { return idnum; } }
        [DataMember]
        protected int parameterId = 0;
        [Browsable(false)]
        public int ParameterId { get { return parameterId; } }
        [Browsable(true)]
        [Category("Свойства параметра"), DisplayName("Название"), Description("Название параметра.")]
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        protected string parameterFullName = "";
        [Category("Свойства параметра"), DisplayName("Путь"), Description("Полный путь параметра в структуре оборудования.")]
        public string ParameterFullName { get { return parameterFullName; } }
        [Browsable(false)]
        public int SortIndex
        {
            get
            {
                int res = 0;
                if (Attributes.ContainsKey("sortindex")) int.TryParse(Attributes["sortindex"], out res);
                return res;
            }
            set
            {
                Attributes["sortindex"] = value.ToString();
            }
        }

        [Browsable(false)]
        [DataMember]
        public Dictionary<string, string> Attributes { get; set; }

        public ChildParamNode()
        {
            Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        public ChildParamNode(DataRow row)
            : this()
        {
            LoadData(row);
        }
        public ChildParamNode(ChildParamNode param)
        {
            idnum = param.Idnum;
            parameterFullName = param.ParameterFullName;
            parameterId = param.ParameterId;
            Text = param.Text;
            Attributes = new Dictionary<string, string>(param.Attributes, StringComparer.OrdinalIgnoreCase);
        }

        public virtual ChildParamNode Clone()
        {
            ChildParamNode res = new ChildParamNode(this);
            return res;
        }
        public virtual void SetFullName(string fullname)
        {
            parameterFullName = fullname;
        }

        public virtual void LoadData(DataRow row)
        {
            //try
            //{
            idnum = (int)row["idnum"];
            Text = row["name"].ToString();
            parameterId = (int)row["paramId"];
            parameterFullName = parameterId.ToString();
            //}
            //catch (Exception ex)
            //{
            //    CommonData.Error("ChildParamNode.LoadData: " + ex.Message + "(text=" + Text + ")");
            //}
        }

        public override bool Equals(object obj)
        {
            ChildParamNode node = obj as ChildParamNode;

            if (node == null) return false;
            if (Idnum != node.Idnum) return false;
            if (ParameterId != node.ParameterId) return false;
            if (Text != node.Text) return false;
            if (!CheckDictionaryEquals(Attributes, node.Attributes)) return false;
            return true;
        }

        public override int GetHashCode()
        {
            int result = 0;
            unchecked
            {
                result += idnum * 3 + 17;
                result += parameterId * 3 + 17;
                result += Text.GetHashCode() * 3 + 17;
                foreach (var it in Attributes)
                    result += it.Key.GetHashCode() * 3 + it.Value.GetHashCode() * 3 + 17;
            }
            return base.GetHashCode() + result;
        }

        private bool CheckDictionaryEquals(IDictionary a, IDictionary b)
        {
            if (a != null && b != null)
            {
                if (a != b)
                {
                    if (a.Count != b.Count) return false;
                    foreach (var item in a.Keys)
                        if (!b.Contains(item) || !b[item].Equals(a[item])) return false;
                }
            }
            else if (a != b) return false;
            return true;
        }
    }
}

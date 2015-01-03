using System;
using System.ComponentModel;
using System.Data;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class MonitorTableParamNode : ChildParamNode
    {
        [Browsable(true)]
        [Category("Свойства параметра"), DisplayName("Количество знаков"), Description("Количество знаков после запятой.")]
        public int? DecNumber
        {
            get
            {
                try
                {
                    if (!Attributes.ContainsKey("decNumber") || Attributes["decNumber"] == null) return null;
                    return Convert.ToInt32(Attributes["decNumber"]);
                }
                catch { return null; }
            }
            set
            {
                if (!value.Equals(DecNumber))
                {
                    if (value == null)
                    { if (Attributes.ContainsKey("decNumber")) Attributes.Remove("decNumber"); }
                    else { Attributes["decNumber"] = value.ToString(); }
                }
            }
        }

        public MonitorTableParamNode() { }
        public MonitorTableParamNode(DataRow row)
            : base(row)
        {
        }
        public MonitorTableParamNode(MonitorTableParamNode param)
            : base(param)
        {
            //
        }

        public override ChildParamNode Clone()
        {
            return new MonitorTableParamNode(this);
        }
    }
}

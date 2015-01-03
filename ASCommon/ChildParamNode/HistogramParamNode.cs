using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class HistogramParamNode : ChildParamNode
    {
        [Browsable(true)]
        [Category("Свойства параметра"), DisplayName("Цвет"), Description("Цвет линии на графике.")]
        public Color LineColor
        {
            get
            {
                try
                {
                    if (!Attributes.ContainsKey("color")) return Color.Empty;
                    return Color.FromArgb(Convert.ToInt32(Attributes["color"]));
                }
                catch { return Color.Empty; }
            }
            set
            {
                if (!value.Equals(LineColor)) Attributes["color"] = value.ToArgb().ToString();
            }
        }

        [TypeConverter(typeof(AggregationTypeConverter))]
        [Category("Свойства параметра"), DisplayName("Агреграция"), Description("Тип агрегации, используемый параметром")]
        public CalcAggregation Aggregation
        {
            get
            {
                CalcAggregation res = CalcAggregation.Last;
                int ires;
                if (!Attributes.ContainsKey("aggregation") || !int.TryParse(Attributes["aggregation"], out ires))
                    res = CalcAggregation.Last;
                else
                    res = (CalcAggregation)ires;
                return res;
            }
            set { Attributes["aggregation"] = ((int)value).ToString(); }
        }

        public HistogramParamNode() { }
        public HistogramParamNode(DataRow row)
            : base(row)
        {

        }
        public HistogramParamNode(HistogramParamNode param)
            : base(param)
        {
            //
        }

        public override ChildParamNode Clone()
        {
            return new HistogramParamNode(this);
        }
    }
}

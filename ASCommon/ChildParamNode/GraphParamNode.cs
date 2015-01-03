using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    //[TypeConverter(typeof(MyTypeConverter))]
    public class GraphParamNode : ChildParamNode
    {
        private static Random rdm = new Random(unchecked((int)DateTime.Now.Ticks));
        public static Color GetNewLineColor()
        {
            return Color.FromArgb(rdm.Next(10, 255), rdm.Next(10, 255), rdm.Next(10, 255));
        }

        //[Browsable(false)]
        //public GraphicsPath ZedSymbol
        //{
        //    get { return new GraphicsPath();/*GetSymbol(LineSymbol);*/ }
        //}
        //[Browsable(false)]
        //public GraphicsPath GridSymbol
        //{
        //    get { return new GraphicsPath();/*GetSymbol(LineSymbol, 5f);*/ }
        //}

        [Browsable(true)]
        //[Editable(true)]
        [Category("Внешний вид"), DisplayName("Цвет"), Description("Цвет линии на графике.")]
        public Color LineColor
        {
            get
            {
                try { return Color.FromArgb(Convert.ToInt32(Attributes["color"])); }
                catch { return Color.Empty; }
            }
            set
            {
                if (!value.Equals(LineColor))
                {
                    Attributes["color"] = value.ToArgb().ToString();
                    //if (parentNode != null) parentNode.DoItemChanged(this);
                }
            }
        }

        [Browsable(true)]
        //[Editable(true)]
        [Category("Внешний вид"), DisplayName("Маркер"), Description("Вид маркера.")]
        public LineSymbolType LineSymbol
        {
            get
            {
                try { return (LineSymbolType)Convert.ToInt32(Attributes["symbol"]); }
                catch { return LineSymbolType.None; }
            }
            set
            {
                if (!value.Equals(LineSymbol))
                {
                    Attributes["symbol"] = value.GetHashCode().ToString();
                    //if (parentNode != null) parentNode.DoItemChanged(this);
                }
            }
        }

        [Browsable(true)]
        //[Editable(true)]
        [Category("Внешний вид"), DisplayName("Стиль"), Description("Стиль линии на графике.")]
        public LineStyle LineStyle
        {
            get
            {
                try
                {
                    if (!Attributes.ContainsKey("style")) return LineStyle.Сплошная;
                    return (LineStyle)Convert.ToInt32(Attributes["style"]);
                }
                catch { return LineStyle.Сплошная; }
            }
            set
            {
                if (!value.Equals(LineStyle))
                {
                    Attributes["style"] = value.GetHashCode().ToString();
                    //if (parentNode != null) parentNode.DoItemChanged(this);
                }
            }
        }

        [Browsable(true)]
        //[Editable(true)]
        [Category("Внешний вид"), DisplayName("Толщина"), Description("Толщина линии на графике.")]
        public double LineWidth
        {
            get
            {
                try { return (double)doubleconv.ConvertFromInvariantString(Attributes["width"].ToString()); }
                catch { return 1f; }
            }
            set
            {
                if (!value.Equals(LineWidth))
                {
                    Attributes["width"] = doubleconv.ConvertToInvariantString(value);
                    //if (parentNode != null) parentNode.DoItemChanged(this);
                }
            }
        }

        [Browsable(true)]
        //[Editable(true)]
        [Category("Свойства параметра"), DisplayName("Минимум"), Description("Нижнее значение на оси Y.")]
        public double? MinValue
        {
            get
            {
                try
                {
                    if (Attributes["minValue"] == null) return null;
                    return (double)doubleconv.ConvertFromInvariantString(Attributes["minValue"].ToString());
                }
                catch { return null; }
            }
            set
            {
                if (!value.Equals(MinValue))
                {
                    if (value == null) Attributes["minValue"] = null;
                    else { Attributes["minValue"] = doubleconv.ConvertToInvariantString(value); }
                    //if (parentNode != null) parentNode.DoItemChanged(this);
                }
            }
        }

        [Browsable(true)]
        //[Editable(true)]
        [Category("Свойства параметра"), DisplayName("Максимум"), Description("Верхнее значение на оси Y.")]
        public double? MaxValue
        {
            get
            {
                try
                {
                    if (Attributes["maxValue"] == null) return null;
                    return (double)doubleconv.ConvertFromInvariantString(Attributes["maxValue"].ToString());
                }
                catch { return null; }
            }
            set
            {
                if (!value.Equals(MaxValue))
                {
                    if (value == null) Attributes["maxValue"] = null;
                    else { Attributes["maxValue"] = doubleconv.ConvertToInvariantString(value); }
                    //if (parentNode != null) parentNode.DoItemChanged(this);
                }
            }
        }

        [Browsable(true)]
        //[Editable(true)]
        [Category("Внешний вид"), DisplayName("Количество знаков"), Description("Количество отображаемых знаков после запятой в значениях параметра.")]
        public int? DecNumber
        {
            get
            {
                try
                {
                    if (Attributes["decNumber"] == null) return null;
                    return Convert.ToInt32(Attributes["decNumber"]);
                }
                catch { return null; }
            }
            set
            {
                if (!value.Equals(DecNumber))
                {
                    if (value == null) Attributes["decNumber"] = null;
                    else { Attributes["decNumber"] = value.ToString(); }
                    //if (parentNode != null) parentNode.DoItemChanged(this);
                }
            }
        }

        public GraphParamNode() { }
        public GraphParamNode(DataRow row)
            : base(row)
        {

        }
        public GraphParamNode(GraphParamNode param)
            : base(param)
        {
            //
        }

        public override void SetParameter(ParameterNode paramnode)
        {
            if (paramnode == null) return;
            parameterId = paramnode.Idnum;
            parameterFullName = paramnode.FullName;
            Text = paramnode.ToString();

            Attributes["width"] = "1";
            if (paramnode.Attributes.ContainsKey("symbol"))
                Attributes["symbol"] = paramnode.GetAttribute("symbol");
            else
                Attributes["symbol"] = "1";
            Color color = Color.Empty;
            if (paramnode.Attributes.ContainsKey("LineColor"))
            {
                string tmp = paramnode.Attributes["LineColor"].ToString();

                try
                {
                    color = Color.FromArgb(int.Parse(tmp));
                }
                catch (FormatException) { }
            }
            if (color == Color.Empty || color.ToArgb() == 0) color = GetNewLineColor();
            Attributes["color"] = color.ToArgb().ToString();
            //if (paramnode.Attributes.Contains("LineColor"))
            //    Attributes["color"] = paramnode.Attributes["LineColor"];
            //else
            //    Attributes["color"] = GetNewLineColor().ToArgb().ToString();
            if (!paramnode.MinValue.Equals(DateTime.MinValue))
                Attributes["minValue"] = doubleconv.ConvertToInvariantString(paramnode.MinValue);
            if (!paramnode.MaxValue.Equals(DateTime.MinValue))
                Attributes["maxValue"] = doubleconv.ConvertToInvariantString(paramnode.MaxValue);
        }

        public override ChildParamNode Clone()
        {
            return new GraphParamNode(this);
        }
    }
}

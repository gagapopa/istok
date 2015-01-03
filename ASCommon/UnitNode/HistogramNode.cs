using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using COTES.ISTOK.ASC.TypeConverters;
using System.Drawing;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class HistogramNode : GraphNode
    {
        [DisplayName("Горизонтальная")]
        [Description("Показывает, будет ли гистограмма отображена в горизонтальном виде.")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool Horizontal
        {
            get
            {
                if (GetAttribute("horizontal") == "1")
                    return true;
                return false;
            }
            set
            {
                SetAttribute("horizontal", value ? "1" : "0");
            }
        }

        [Browsable(true)]
        [/*Category("Свойства параметра"),*/ DisplayName("Цвет максимума"), Description("Цвет максимального столбца гистограммы.")]
        public Color MaxColor
        {
            get
            {
                try
                {
                    Color val = Color.FromArgb(Convert.ToInt32(GetAttribute("maxcolor")));
                    if (val.A == Color.Empty.A &&
                        val.B == Color.Empty.B &&
                        val.G == Color.Empty.G &&
                        val.R == Color.Empty.R)
                        val = Color.Empty;
                    return val;
                }
                catch { return Color.Empty; }
            }
            set
            {
                if (!value.Equals(MaxColor)) SetAttribute("maxcolor", value.ToArgb().ToString());
            }
        }
        [Browsable(true)]
        [/*Category("Свойства параметра"),*/ DisplayName("Цвет минимума"), Description("Цвет минимального столбца гистограммы.")]
        public Color MinColor
        {
            get
            {
                try
                {
                    Color val = Color.FromArgb(Convert.ToInt32(GetAttribute("mincolor")));
                    if (val.A == Color.Empty.A &&
                        val.B == Color.Empty.B &&
                        val.G == Color.Empty.G &&
                        val.R == Color.Empty.R)
                        val = Color.Empty;
                    return val;
                }
                catch { return Color.Empty; }
            }
            set
            {
                if (!value.Equals(MinColor))
                {
                    Color val = value;
                    if (val.A == Color.Empty.A &&
                        val.B == Color.Empty.B &&
                        val.G == Color.Empty.G &&
                        val.R == Color.Empty.R)
                        val = Color.Empty;
                    SetAttribute("mincolor", val.ToArgb().ToString());
                }
            }
        }
        //[DisplayName("Сортировка")]
        //[Description("Переключение режимов сортировки гистограммы")]
        //public int Sorting { get; set; }

        public HistogramNode()
            : base()
        {
            Typ = (int)UnitTypeId.Histogram;
        }

        public HistogramNode(DataRow row)
            : base(row)
        {
            //
        }

        public override ChildParamNode CreateNewChildParamNode()
        {
            return new HistogramParamNode();
        }
    }
}

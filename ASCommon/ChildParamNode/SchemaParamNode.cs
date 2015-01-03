using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class SchemaParamNode : ChildParamNode
    {
        [Category("Внешний вид"), DisplayName("X"), Description("Отступ слева на мнемосхеме (пиксели).")]
        public int Left
        {
            get
            {
                int res = 0;
                if (Attributes.ContainsKey("left")) int.TryParse(Attributes["left"], out res);
                return res;
            }
            set
            {
                Attributes["left"] = value.ToString();
            }
        }
        [Category("Внешний вид"), DisplayName("Y"), Description("Отступ сверху на мнемосхеме (пиксели).")]
        public int Top
        {
            get
            {
                int res = 0;
                if (Attributes.ContainsKey("top")) int.TryParse(Attributes["top"], out res);
                return res;
            }
            set
            {
                Attributes["top"] = value.ToString();
            }
        }
        [Category("Внешний вид"), DisplayName("Ширина"), Description("Ширина области отображения параметра (пиксели).")]
        public int Width
        {
            get
            {
                int res = 0;
                if (Attributes.ContainsKey("width")) int.TryParse(Attributes["width"], out res);
                if (res == 0)
                {
                    res = 54;
                    Attributes["width"] = res.ToString();
                }
                return res;
            }
            set
            {
                Attributes["width"] = value.ToString();
            }
        }
        [Category("Внешний вид"), DisplayName("Высота"), Description("Высота области отображения параметра (пиксели).")]
        public int Height
        {
            get
            {
                int res = 0;
                if (Attributes.ContainsKey("height")) int.TryParse(Attributes["height"], out res);
                if (res == 0)
                {
                    res = 20;
                    Attributes["height"] = res.ToString();
                }
                return res;
            }
            set
            {
                Attributes["height"] = value.ToString();
            }
        }
        [Category("Внешний вид"), DisplayName("Цвет"), Description("Цвет фона области отображения.")]
        public Color NominalColor
        {
            get
            {
                int res = Color.LightGreen.ToArgb();
                if (Attributes.ContainsKey("NominalColor")) int.TryParse(Attributes["NominalColor"], out res);
                return Color.FromArgb(res);
            }
            set
            {
                Attributes["NominalColor"] = value.ToArgb().ToString();
            }
        }
        [Category("Внешний вид"), DisplayName("Цвет н. пред. гр."), Description("Цвет нижней предупредительной границы.")]
        public Color MinWarningColor
        {
            get
            {
                int res = Color.LightGreen.ToArgb();
                if (Attributes.ContainsKey("minWarningColor")) int.TryParse(Attributes["minWarningColor"], out res);
                return Color.FromArgb(res);
            }
            set
            {
                Attributes["minWarningColor"] = value.ToArgb().ToString();
            }
        }
        [Category("Внешний вид"), DisplayName("Цвет н. авар. гр."), Description("Цвет нижней аварийной границы.")]
        public Color MinAlertColor
        {
            get
            {
                int res = Color.LightGreen.ToArgb();
                if (Attributes.ContainsKey("minAlertColor")) int.TryParse(Attributes["minAlertColor"], out res);
                return Color.FromArgb(res);
            }
            set
            {
                Attributes["minAlertColor"] = value.ToArgb().ToString();
            }
        }
        [Category("Внешний вид"), DisplayName("Цвет в. пред. гр."), Description("Цвет верхней предупредительной границы.")]
        public Color MaxWarningColor
        {
            get
            {
                int res = Color.LightGreen.ToArgb();
                if (Attributes.ContainsKey("maxWarningColor")) int.TryParse(Attributes["maxWarningColor"], out res);
                return Color.FromArgb(res);
            }
            set
            {
                Attributes["maxWarningColor"] = value.ToArgb().ToString();
            }
        }
        [Category("Внешний вид"), DisplayName("Цвет в. авар. гр."), Description("Цвет верхней аварийной границы.")]
        public Color MaxAlertColor
        {
            get
            {
                int res = Color.LightGreen.ToArgb();
                if (Attributes.ContainsKey("maxAlertColor")) int.TryParse(Attributes["maxAlertColor"], out res);
                return Color.FromArgb(res);
            }
            set
            {
                Attributes["maxAlertColor"] = value.ToArgb().ToString();
            }
        }
        [Category("Свойства параметра"), DisplayName("Знач. н. пред. гр."), Description("Значение нижней предупредительной границы.")]
        public double? MinWarningValue
        {
            get
            {
                try
                {
                    string res;
                    if (Attributes.ContainsKey("minWarningValue")
                        && !string.IsNullOrEmpty(res = Attributes["minWarningValue"]))
                        return (double)doubleconv.ConvertFromInvariantString(res);
                }
                catch (FormatException) { }
                return null;
            }
            set
            {
                Attributes["minWarningValue"] = doubleconv.ConvertToInvariantString(value);
            }
        }
        [Category("Свойства параметра"), DisplayName("Знач. н. авар. гр."), Description("Значение нижней аварийной границы.")]
        public double? MinAlertValue
        {
            get
            {
                try
                {
                    string res;
                    if (Attributes.ContainsKey("minAlertValue")
                        && !string.IsNullOrEmpty(res = Attributes["minAlertValue"]))
                        return (double)doubleconv.ConvertFromInvariantString(res);
                }
                catch (FormatException) { }
                return null;
            }
            set
            {
                Attributes["minAlertValue"] = doubleconv.ConvertToInvariantString(value);
            }
        }
        [Category("Свойства параметра"), DisplayName("Знач. в. пред. гр."), Description("Значение верхней предупредительной границы.")]
        public double? MaxWarningValue
        {
            get
            {
                try
                {
                    string res;
                    if (Attributes.ContainsKey("maxWarningValue")
                        && !string.IsNullOrEmpty(res = Attributes["maxWarningValue"]))
                        return (double)doubleconv.ConvertFromInvariantString(res);
                }
                catch (FormatException) { }
                return null;
            }
            set
            {
                Attributes["maxWarningValue"] = doubleconv.ConvertToInvariantString(value);
            }
        }
        [Category("Свойства параметра"), DisplayName("Знач. в. авар. гр."), Description("Значение верхней аварийной границы.")]
        public double? MaxAlertValue
        {
            get
            {
                try
                {
                    string res;
                    if (Attributes.ContainsKey("maxAlertValue")
                        && !string.IsNullOrEmpty(res = Attributes["maxAlertValue"]))
                        return (double)doubleconv.ConvertFromInvariantString(res);
                }
                catch (FormatException) { }
                return null;
            }
            set
            {
                Attributes["maxAlertValue"] = doubleconv.ConvertToInvariantString(value);
            }
        }
        [Category("Свойства параметра"), DisplayName("Гистерезис"), Description("Зона нечувствительности значения параметра (%).")]
        public double? Hysteresis
        {
            get
            {
                try
                {
                    string res;
                    if (Attributes.ContainsKey("hysteresis")
                        && !string.IsNullOrEmpty(res = Attributes["hysteresis"]))
                        return (double)doubleconv.ConvertFromInvariantString(res);
                }
                catch (FormatException) { }
                return null;
            }
            set
            {
                Attributes["hysteresis"] = doubleconv.ConvertToInvariantString(value);
            }
        }

        [Category("Внешний вид"), DisplayName("Шрифт")]
        public Font TextFont
        {
            get
            {
                String font;
                if (Attributes.TryGetValue("font", out font))
                    return new FontConverter().ConvertFromInvariantString(font) as Font;

                return null;
            }
            set
            {
                String fontString = new FontConverter().ConvertToInvariantString(value);
                if (value != null)
                    fontString = new FontConverter().ConvertToInvariantString(value);
                else
                    fontString = null;

                Attributes["font"] = fontString;
            }
        }

        [Browsable(true)]
        //[Editable(true)]
        [Category("Внешний вид"), DisplayName("Количество знаков"), Description("Количество знаков после запятой.")]
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
                    if (value == null)
                    { if (Attributes.ContainsKey("decNumber")) Attributes.Remove("decNumber"); }
                    else { Attributes["decNumber"] = value.ToString(); }
                    //if (parentNode != null) parentNode.DoItemChanged(this);
                }
            }
        }

        public SchemaParamNode() { }
        public SchemaParamNode(DataRow row)
            : base(row)
        {
        }
        public SchemaParamNode(SchemaParamNode param)
            : base(param)
        {
        }

        public override ChildParamNode Clone()
        {
            return new SchemaParamNode(this);
        }
    }
}

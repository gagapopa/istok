using System;
using System.ComponentModel;
using System.Data;
using COTES.ISTOK.ASC.TypeConverters;
using System.Drawing;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class ParameterNode : UnitNode, IComparable<ParameterNode>
    {
        const String formulaAttributeName = "formula_cod";
        const String storeDaysAttributeName = "store_days";
        const String roundCountAttributeName = "round_count";
        const String unitAttributeName = "unit";
        const String lineColorAttributeName = "linecolor";
        const String minValueAttributeName = "minValue";
        const String maxValueAttributeName = "maxValue";

        [Description("Уникальный символьный код, используемый в формулах")]
        [DisplayName("Код параметра")]
        [CategoryOrder(CategoryGroup.Calc)]
        [Browsable(true)]
        public override string Code
        {
            get { return GetAttribute(formulaAttributeName); }
            set { SetAttribute(formulaAttributeName, value); }
        }

        [DisplayName("Дней хранения")]
        [Description("Минимальный срок хранения значения параметра в базе")]
        [CategoryOrder(CategoryGroup.Values)]
        public int StoreDays
        {
            get
            {
                int s;

                if (int.TryParse(GetAttribute(storeDaysAttributeName), out s))
                    return s;

                return 0;
            }
            set
            {
                SetAttribute(storeDaysAttributeName, value.ToString());
            }
        }

#if EMA
        public const String ExtensionSourceAttributeName = "astdk_receive";

        /// <summary>
        /// Атрибут параметра, отмечающий его как отправляемый
        /// </summary>
        [DisplayName("Передавать АСТДК")]
        [Description("")]
        [Category("АСТДК")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool ValueReceive
        {
            get
            {
                bool res;

                if (bool.TryParse(GetAttribute(ExtensionSourceAttributeName), out res))
                    return res;

                return false;
            }
            set { SetAttribute(ExtensionSourceAttributeName, value.ToString()); }
        } 
#endif

        /// <summary>
        /// Единица измерения параметра
        /// </summary>
        [DisplayName("Единица измерения"),
        Description(""),
        CategoryOrder(CategoryGroup.General)]
        [RevisionUnitNode(unitAttributeName)]
        public String Unit
        {
            get { return GetAttribute(unitAttributeName); }
            set { SetAttribute(unitAttributeName, value); }
        }

        [DisplayName("Знаков после запятой"),
        Description("Точность округления при отображении значений значений параметра."),
        CategoryOrder(CategoryGroup.Appearance)]
        public int? RoundCount
        {
            get
            {
                int s;

                if (int.TryParse(GetAttribute(roundCountAttributeName), out s))
                    return s;

                return null;
            }
            set
            {
                if (value == null)
                    SetAttribute(roundCountAttributeName, null);
                else
                    SetAttribute(roundCountAttributeName, value.ToString());
            }
        }

        public override string GetNodeText()
        {
            if (!String.IsNullOrEmpty(Unit))
                return String.Format("{0}, {1}", base.GetNodeText(), Unit);
            return base.GetNodeText();
        }
        /// <summary>
        /// цвет параметра на графике
        /// </summary>
        [DisplayName("Цвет")]
        [Description("Цвет параметра на графике")]
        [CategoryOrder(CategoryGroup.Appearance)]
        public Color LineColor
        {
            get
            {
                int res;

                int.TryParse(GetAttribute(lineColorAttributeName), out res);

                return Color.FromArgb(res);
            }
            set { SetAttribute(lineColorAttributeName, value.ToArgb().ToString()); }
        }

        /// <summary>
        /// Нижнее значение на оси Y на графике
        /// </summary>
        [DisplayName("Минимальное")]
        [Description("Минимум значения на графике")]
        [CategoryOrder(CategoryGroup.Values)]
        [TypeConverter(typeof(DoubleTypeConverter))]
        public double MinValue
        {
            get
            {
                String stringValue = GetAttribute(minValueAttributeName);
                
                if (!string.IsNullOrEmpty(stringValue))
                    return (double)doubleconv.ConvertFromInvariantString(stringValue);
                
                return double.NaN;
            }
            set { SetAttribute(minValueAttributeName, doubleconv.ConvertToInvariantString(value)); }
        }

        /// <summary>
        /// Верхнее значение на оси Y на графике
        /// </summary>
        [DisplayName("Максимальное")]
        [Description("Максимум значения на графике")]
        [CategoryOrder(CategoryGroup.Values)]
        [TypeConverter(typeof(DoubleTypeConverter))]
        public double MaxValue
        {
            get
            {
                String stringValue = GetAttribute(maxValueAttributeName);

                if (!string.IsNullOrEmpty(stringValue))
                    return (double)doubleconv.ConvertFromInvariantString(stringValue);

                return double.NaN;
            }
            set { SetAttribute(maxValueAttributeName, doubleconv.ConvertToInvariantString(value)); }
        }

        public ParameterNode()
            : base() { }
        public ParameterNode(int id)
            : base() { idnum = id; }
        public ParameterNode(DataRow row)
            : base(row)
        {
        }

        #region IComparable<ParameterNode> Members

        public int CompareTo(ParameterNode other)
        {
            if (idnum == -1) return Code.CompareTo(other.Code);
            else return idnum.CompareTo(other.idnum);
        }

        #endregion
    }

    [Serializable]
    public class ParameterNodeDependence
    {
        public int ParameterId { get; set; }
        public ParameterNode[] Dependences { get; set; }
    }

    public class ParameterNodeReference
    {
        public ParameterNode ParameterNode { get; set; }

        public RevisionInfo Revision { get; set; }
    }
}

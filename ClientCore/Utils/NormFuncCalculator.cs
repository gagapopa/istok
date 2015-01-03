using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ClientCore.Utils
{
    [TypeConverter(typeof(NormFuncCalcTypeConverter))]
    public class NormFuncCalculator
    {
        [Browsable(false)]
        public MultiDimensionalTable MDTable { get; private set; }

        [Browsable(false)]
        public DimensionValue[] DimensionValues { get; private set; }

        [DisplayName("Значение")]
        [Category("Результат")]
        [Description("")]
        [ReadOnly(true)]
        [TypeConverter(typeof(RoundDoubleConverter))]
        public double? Result
        {
            get
            {
                try
                {
                    List<double> lstCoords = new List<double>();
                    for (int i = 0; i < DimensionValues.Length; i++)
                        lstCoords.Add(DimensionValues[i].Value);
                    return MDTable.GetValue(lstCoords.ToArray());
                }
                catch
                {
                    return null;
                }
            }
        }

        public NormFuncCalculator(MultiDimensionalTable mdt)
        {
            List<DimensionValue> lstValues = new List<DimensionValue>();

            MDTable = mdt;
            if (MDTable != null)
                foreach (var item in MDTable.DimensionInfo)
                    lstValues.Add(new DimensionValue(item.Name, item.Measure));
            DimensionValues = lstValues.ToArray();
        }
    }

    class RoundDoubleConverter : DoubleConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is double && destinationType == typeof(String))
            {
                double d = (double)value;
                return d.ToString(DoubleControlSettings.DoubleFormat(3));
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class NormFuncCalcTypeConverter : ExpandableObjectConverter
    {
        public override System.ComponentModel.PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection result = new PropertyDescriptorCollection(null);
            NormFuncCalculator obj = (NormFuncCalculator)value;
            string category;

            category = "Измерения";
            for (int i = obj.DimensionValues.Length - 1; i >= 0; i--)
            {
                result.Add(new DimensionValuePropertyDescriptor(obj.DimensionValues[i], obj.DimensionValues[i].Name, category));
            }
            PropertyDescriptorCollection props = base.GetProperties(context, value, attributes);
            foreach (PropertyDescriptor item in props)
                result.Add(item);

            return result;
        }

        protected class DimensionValuePropertyDescriptor : SimplePropertyDescriptor
        {
            DimensionValue dimensionInfo;
            string category;

            public DimensionValuePropertyDescriptor(DimensionValue dimensionInfo, string propName, string category)
                : base(typeof(DimensionPropertyCollection), propName, typeof(double))
            {
                this.dimensionInfo = dimensionInfo;
                this.category = category;
            }

            public override string Category
            {
                get { return category; }
            }
            public override string DisplayName
            {
                get
                {
                    string unit = "";

                    if (!string.IsNullOrEmpty(dimensionInfo.Measure))
                        unit = ", " + dimensionInfo.Measure;
                    return dimensionInfo.Name + unit;
                }
            }
            public override string Description
            {
                get { return "Описание"; }
            }
            public override object GetValue(object component)
            {
                return dimensionInfo.Value;
            }
            public override void SetValue(object component, object value)
            {
                dimensionInfo.Value = (double)value;
            }
        }
    }
}

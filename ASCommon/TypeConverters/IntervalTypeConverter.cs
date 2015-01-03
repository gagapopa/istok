using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Resources;
using COTES.ISTOK;

namespace COTES.ISTOK.ASC.TypeConverters
{
    public interface IIntervalSupplier
    {
        IEnumerable<Interval> GetStandardIntervals();
        String GetIntervalHeader(Interval interval);
        Interval GetIntervalByHeader(String intervalHeader);
    }

    public class IntervalTypeConverter : TypeConverter
    {
        public IIntervalSupplier Supplier { get; set; }
        private IIntervalSupplier getSupplier(ITypeDescriptorContext context)
        {
            if (Supplier == null && context != null)
            {
                Supplier = context.GetService(typeof(IIntervalSupplier)) as IIntervalSupplier;
            }
            return Supplier;
        }

        private ResourceManager resourceManager = new ResourceManager(typeof(IntervalTypeConverter));
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //if (context!=null)
            //{

            if (getSupplier(context) != null)
            {
                TypeConverter.StandardValuesCollection coll = new StandardValuesCollection(getSupplier(context).GetStandardIntervals().ToArray());
                return coll;
            }

            //}
            return base.GetStandardValues(context);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType.Equals(typeof(String));
        }

        public override object ConvertTo(
            ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            object value,
            Type destinationType)
        {
            if (value == null) return null;
            //if (context != null)
            //{
            //IIntervalSupplier intervalSupplier = context.GetService(typeof(IIntervalSupplier)) as IIntervalSupplier;
            if (destinationType.Equals(typeof(String)) && value is Interval && getSupplier(context) != null)
            {
                //String ret = null;
                Interval val = (Interval)value;

                String ret= getSupplier(context).GetIntervalHeader(val);
                //ret = String.Format("StandardValue{0}Header", val.ToDouble().ToString(System.Globalization.NumberFormatInfo.InvariantInfo).Replace('-', '_').Replace('.', '_'));
                //ret = resourceManager.GetString(ret, culture);
                //if (ret == null) ret = value.ToString();
                return ret;
            }
            //}
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.Equals(typeof(String));
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture, object value)
        {
            if (value is String)
            {
                //if (context != null)
                //{
                //IIntervalSupplier intervalSupplier = context.GetService(typeof(IIntervalSupplier)) as IIntervalSupplier;
                if (getSupplier(context) != null)
                {
                    return getSupplier(context).GetIntervalByHeader(value.ToString());
                }
                //if (value == null) return null;
                //String str = value.ToString();
                //String[] values = resourceManager.GetString("StandardValues", culture).Split(',');
                ////Interval dblValue;
                //int i;

                //try
                //{
                //    return new Interval(double.Parse(str, System.Globalization.NumberFormatInfo.InvariantInfo));
                //}
                //catch (FormatException) { }
                //for (i = 0; i < values.Length; i++)
                //    try
                //    {
                //        if (str.Equals(resourceManager.GetString(String.Format("StandardValue{0}Header", values[i].Replace('-', '_').Replace('.', '_').Replace(" ", "")), culture), StringComparison.InvariantCultureIgnoreCase))
                //            return new Interval(double.Parse(values[i], System.Globalization.NumberFormatInfo.InvariantInfo));
                //    }
                //    catch (FormatException) { } 
                //}
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return base.IsValid(context, value);
        }
    }
}

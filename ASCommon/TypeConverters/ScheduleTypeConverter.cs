using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using COTES.ISTOK;

namespace COTES.ISTOK.ASC.TypeConverters
{
    public class ScheduleTypeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IScheduleRetrivial scheduleRetrieval = 
                context.GetService(typeof(IScheduleRetrivial)) as IScheduleRetrivial;

            if (scheduleRetrieval != null)
            {
                List<int> lst = new List<int>();
                foreach (var item in scheduleRetrieval.GetSchedules())
                    lst.Add(item.Id);
                return new StandardValuesCollection(lst.ToArray());
            }

            return base.GetStandardValues(context);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType.Equals(typeof(string)) || destinationType.Equals(typeof(int));
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType.Equals(typeof(String))/* && value.GetType().Equals(typeof(int))*/)
            {
                if (value is int)
                {
                    int scheduleID = (int)value;
                    if (context != null)
                    {
                        IScheduleRetrivial scheduleRetrivial =
                            context.GetService(typeof(IScheduleRetrivial)) as IScheduleRetrivial;
                        if (scheduleRetrivial != null)
                        {
                            Schedule shed = scheduleRetrivial.GetSchedule(scheduleID);
                            if (shed == null) return String.Empty;
                            return shed.Name;
                        }
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.Equals(typeof(string)) || sourceType.Equals(typeof(int));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (context != null)
            {
                IScheduleRetrivial scheduleRetrivial =
                    context.GetService(typeof(IScheduleRetrivial)) as IScheduleRetrivial;
                if (scheduleRetrivial != null)
                {
                    Schedule shed = null;
                    if (value is string)
                        shed = scheduleRetrivial.GetSchedule((string)value);
                    else if (value is int)
                        shed = scheduleRetrivial.GetSchedule((int)value);
                    if (shed == null) return 0;
                    return shed.Id;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace COTES.ISTOK.ASC.TypeConverters
{
    public class OwnerTypeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return context != null && context.GetService(typeof(IOwnerRetrieval))!=null;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                IOwnerRetrieval ownerRetrieval = context.GetService(typeof(IOwnerRetrieval)) as IOwnerRetrieval;
                if (ownerRetrieval != null)
                {
                    List<int> ownerList = new List<int>();
                    ownerList.Add(0);
                    ownerList.Add(ownerRetrieval.GetCurrentUser());
                    var groups = ownerRetrieval.GetGroups();
                    if (groups != null)
                    {
                        ownerList.AddRange(groups);
                    }
                    return new StandardValuesCollection(ownerList);
                }
            }
            return base.GetStandardValues(context);
        }

        Dictionary<int, String> localizationData = new Dictionary<int, String>();
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType.Equals(typeof(String)) && value.GetType().Equals(typeof(int)))
            {
                int ownerID = (int)value;
                if (context != null)
                {
                    IOwnerRetrieval ownerRertrieval = context.GetService(typeof(IOwnerRetrieval)) as IOwnerRetrieval;
                    if (ownerRertrieval != null)
                    {
                        if (!localizationData.ContainsKey(ownerID)) localizationData[ownerID] = ownerRertrieval.GetOwnerLocalization(ownerID, culture);
                        return localizationData[ownerID];
                    }
                }
                String ret;
                localizationData.TryGetValue(ownerID, out ret);
                return ret;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.Equals(typeof(String));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value != null && value.GetType().Equals(typeof(String)))
            {
                if (localizationData.ContainsValue(value.ToString()))
                    foreach (int ownerID in localizationData.Keys)
                        if (localizationData[ownerID].Equals(value)) return ownerID;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            if (value == null || !value.GetType().Equals(typeof(int))) return false;

            IOwnerRetrieval ownerRetrieval = context.GetService(typeof(IOwnerRetrieval)) as IOwnerRetrieval;
            if (ownerRetrieval != null)
            {
                List<int> list = new List<int>();
                list.Add(0);
                list.AddRange(ownerRetrieval.GetGroups());
                list.AddRange(ownerRetrieval.GetUsers());
                if (list.Contains((int)value)) return true;
                return false;
            }
            return base.IsValid(context, value);
        }
    }
}

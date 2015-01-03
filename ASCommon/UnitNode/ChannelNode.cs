using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    [TypeConverter(typeof(ChannelTypeConverter))]
    public class ChannelNode : UnitNode
    {
        const String libNameAttributeName = "libname";
        const String startTimeAttributeName = "start_time";
        const String activeAttributeName = "active";
        const String storeValuesAttributeName = "store_db";

        [DisplayName("Библиотека")]
        [CategoryOrder(CategoryGroup.Load)]
        [TypeConverter(typeof(ModuleLibNameTypeConverter))]
        public string Libname
        {
            get { return GetAttribute(libNameAttributeName); }
            set { SetAttribute(libNameAttributeName, value); }
        }

        [DisplayName("Время начала")]
        [CategoryOrder(CategoryGroup.Load)]
        public DateTime StartTime
        {
            get
            {
                String stringValue = GetAttribute(startTimeAttributeName);

                if (!String.IsNullOrEmpty(stringValue))
                    try
                    {
                        return (DateTime)dateconv.ConvertFromInvariantString(stringValue);
                    }
                    catch { }

                return DateTime.MinValue;
            }
            set
            {
                SetAttribute(startTimeAttributeName, dateconv.ConvertToInvariantString(value));
            }
        }

        [DisplayName("Активен")]
        [CategoryOrder(CategoryGroup.Load)]
        //[ReadOnly(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool Active
        {
            get
            {
                //bool res;

                //if (bool.TryParse(GetAttribute(activeAttributeName), out res))
                //    return res;
                if (GetAttribute(activeAttributeName) == "1")
                    return true;


                return false;
            }
            set { SetAttribute(activeAttributeName, value ? "1" : "0"); }
        }

        [DisplayName("Запись в базу")]
        [CategoryOrder(CategoryGroup.Load)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool Storable
        {
            get
            {
                //bool res;

                //if (bool.TryParse(GetAttribute(storeValuesAttributeName), out res))
                //    return res;

                if (GetAttribute(storeValuesAttributeName) == "1")
                    return true;

                return false;
            }
            set { SetAttribute(storeValuesAttributeName, value ? "1" : "0"); }
        }

        public ChannelNode()
            : base()
        {
            Active = true;
            Storable = true;
        }

        public ChannelNode(DataRow row)
            : base(row)
        {
        }
    }
}

using System;
using System.ComponentModel;
using System.Data;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.Extension
{
    [Serializable]
    [TypeConverter(typeof(ExtensionUnitNodeTypeConverter))]
    public class ExtensionUnitNode : UnitNode
    {
        const String externalIDPropertyID = "external_id";
        [Category("Основное"),
        DisplayName("ИД узла во внешней системе"),
        Description("При использовании расширений, ИД внешней системы, соответствующий данному узлу")]
        [TypeConverter(typeof(ExternalIDTypeConverter))]
        public int ExternalID
        {
            get
            {
                int res;

                if (int.TryParse(GetAttribute(externalIDPropertyID), out res))
                    return res;

                return 0;
            }
            set
            {
                SetAttribute(externalIDPropertyID, value.ToString());
            }
        }

        const String ExternalCodePropertyName = "external_code";
        [Category("Основное"),
        DisplayName("Код внешнего узла"),
        Description("")]
        public String ExternalCode
        {
            get { return GetAttribute(ExternalCodePropertyName); }
            set { SetAttribute(ExternalCodePropertyName, value); }
        }

        public ExtensionUnitNode()
            : base()
        {

        }

        public ExtensionUnitNode(DataRow row)
            : base(row)
        {
            ExternalID = -1;
        }
    }
}

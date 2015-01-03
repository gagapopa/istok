using System;
using System.Xml.Serialization;

namespace COTES.ISTOK
{
    [Serializable]
    public class MaintenanceSettings
    {
        public MaintenanceSettings()
        {
            Enabled = false;
            Schedule = String.Empty;
            ValueDeleteCount = DEFAULT_DELETE_COUNT;
            KeepValuesDays = DEFAULT_KEEP_VALUES_DAYS;
        }

        [XmlAttribute]
        public bool Enabled { get; set; }
        [XmlAttribute]
        public string Schedule { get; set; }

        [XmlIgnore]
        public const uint DEFAULT_DELETE_COUNT = 1000000;

        [XmlAttribute]
        public uint ValueDeleteCount { get; set; }

        [XmlIgnore]
        public const uint DEFAULT_KEEP_VALUES_DAYS = 90;

        [XmlAttribute]
        public uint KeepValuesDays { get; set; }
    }
}

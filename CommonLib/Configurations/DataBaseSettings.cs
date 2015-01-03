using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace COTES.ISTOK
{
    [Serializable]
    public class DataBaseSettings
    {
        [XmlAttribute]
        public string Type { get; set; }
        [XmlAttribute]
        public string Host { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string User { get; set; }
        [XmlAttribute]
        public string Password { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace COTES.ISTOK
{
    [Serializable]
    public class VPNSettings
    {
        [XmlAttribute]
        public bool Use { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Server { get; set; }
        [XmlAttribute]
        public string UserName { get; set; }
        [XmlAttribute]
        public string Password { get; set; }
    }
}

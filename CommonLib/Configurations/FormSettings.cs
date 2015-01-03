using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace COTES.ISTOK
{
    [Serializable]
    public class FormSettings
    {
        [XmlAttribute]
        public int X { get; set; }
        [XmlAttribute]
        public int Y { get; set; }
        [XmlAttribute]
        public int Width { get; set; }
        [XmlAttribute]
        public int Height { get; set; }
        [XmlAttribute]
        public FormWindowState State { get; set; }
    }
}

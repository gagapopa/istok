using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK
{
    [DataContract]
    public class UAsyncResult
    {
        [DataMember]
        public Package[] Packages { get; set; }
        [DataMember]
        public Message[] Messages { get; set; }
        [DataMember]
        public byte[] Bytes { get; set; }
        [DataMember]
        public COTES.ISTOK.Calc.NodeBack[] CalcNodeBack { get; set; }

        [DataMember]
        public PackedPackage[] PackedPackage { get; set; }
    }
}

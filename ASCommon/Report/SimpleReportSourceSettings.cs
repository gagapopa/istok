using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace COTES.ISTOK.ASC
{
    [DataContract]
    public class SimpleReportSourceSettings : ReportSourceSettings
    {
        public SimpleReportSourceSettings(ReportSourceInfo info)
            : base(info)
        {
        }

        public SimpleReportSourceSettings(SimpleReportSourceSettings x)
            : this(x.Info)
        {
            Enabled = x.Enabled;
        }

        [DataMember]
        private Guid[] references;

        public void SetReferences(Guid[] references)
        {
            this.references = references;
        }

        public override Guid[] References
        {
            get
            {
                return references;
            }
        }

        public override byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(Enabled);
                }
                return ms.ToArray();
            }
        }

        public override void FromBytes(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    Enabled = br.ReadBoolean();
                }
            }
        }

        public override object Clone()
        {
            return new SimpleReportSourceSettings(this);
        }
    }
}

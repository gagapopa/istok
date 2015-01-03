using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Настройки источника данных, запрашивающего значения для явного указанных параметров
    /// </summary>
    [DataContract]
    public class ParameterReportSourceSetting : ReportSourceSettings
    {
        public ParameterReportSourceSetting(ReportSourceInfo info)
            : base(info)
        {
        }

        public ParameterReportSourceSetting(ParameterReportSourceSetting x)
            : this(x.Info)
        {
            this.ValueSourceSetting = x.ValueSourceSetting;
            this.ParameterIds = x.ParameterIds;
        }

        [DataMember]
        [Browsable(false)]
        public Guid ValueSourceSetting { get; set; }

        /// <summary>
        /// ИД запрашиваемых параметров
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public int[] ParameterIds { get; set; }

        private UnitNode[] parameterNodes;

        [Editor(typeof(ParameterNodeUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(ParameterNodeTypeConverter))]
        [DisplayName("Параметры")]
        public UnitNode[] ParameterNodes
        {
            get
            {
                if (parameterNodes == null && ParameterIds != null)
                {
                    parameterNodes = (from i in ParameterIds 
                                      select new ParameterNode(i) { Typ = (int)UnitTypeId.Parameter }).ToArray();
                }
                return parameterNodes;
            }
            set
            {
                parameterNodes = value;
                if (parameterNodes != null)
                {
                    ParameterIds = (from u in parameterNodes
                                    where u is ParameterNode
                                    select u.Idnum).ToArray();
                }
            }
        }

        public override byte[] ToBytes()
        {
            if (ParameterIds == null)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(Enabled);
                    foreach (int parameterID in ParameterIds)
                    {
                        bw.Write(parameterID);
                    }
                }
                return ms.ToArray();
            }
        }

        public override void FromBytes(byte[] bytes)
        {
            if (bytes == null)
                return;

            List<int> idList = new List<int>();
            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        Enabled = br.ReadBoolean();
                        while (true)
                        {
                            idList.Add(br.ReadInt32()); 
                        }
                    }
                }
            }
            catch (EndOfStreamException)
            {
                ParameterIds = idList.ToArray();
            }
        }

        public override object Clone()
        {
            return new ParameterReportSourceSetting(this);
        }

        public override Guid[] References
        {
            get
            {
                if (ValueSourceSetting!=null)
                    return new Guid[] { ValueSourceSetting };

                return base.References;
            }
        }
    }
}

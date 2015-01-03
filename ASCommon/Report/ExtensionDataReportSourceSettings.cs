using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using COTES.ISTOK.Extension;
using System.Runtime.Serialization;

namespace COTES.ISTOK.ASC
{
    [DataContract]
    public class ExtensionDataReportSourceSettings : ReportSourceSettings
    {
        public ExtensionDataReportSourceSettings(ReportSourceInfo info)
            : base(info)
        {
        }

        public ExtensionDataReportSourceSettings(ExtensionDataReportSourceSettings x)
            : this(x.Info)
        {
            Enabled = x.Enabled;
            StructTableName = x.StructTableName;
            
            if (x.extensionTableNames != null)
            {
                extensionTableNames = new Dictionary<string, List<string>>();
                foreach (var item in x.extensionTableNames.Keys)
                {
                    extensionTableNames[item] = new List<String>(x.extensionTableNames[item]);
                }
            }
        }

        [DataMember]
        public String StructTableName { get; set; }

        [DataMember]
        Dictionary<String, List<String>> extensionTableNames;

        public IEnumerable<KeyValuePair<String, String>> ExtensionTableNames
        {
            get
            {
                if (extensionTableNames == null)
                    yield break;

                foreach (var extensionName in extensionTableNames.Keys)
                {
                    foreach (var item in extensionTableNames[extensionName])
                    {
                        yield return new KeyValuePair<String, String>(extensionName, item);
                    }
                }
            }
        }

        public bool Checked(ExtensionDataInfo info)
        {
            List<String> extensionNameList;

            return extensionTableNames != null
                && extensionTableNames.TryGetValue(info.ExtensionInfo.Caption, out extensionNameList)
                && extensionNameList.Contains(info.Name);
        }

        public void CheckTabInfo(ExtensionDataInfo info, bool check)
        {
            if (check)
                CheckTabInfo(info);
            else
                UncheckTabInfo(info);
        }

        private void CheckTabInfo(ExtensionDataInfo info)
        {
            List<String> extensionNameList;

            if (extensionTableNames == null)
                extensionTableNames = new Dictionary<String, List<String>>();

            if (!extensionTableNames.TryGetValue(info.ExtensionInfo.Caption,out extensionNameList))
                extensionTableNames[info.ExtensionInfo.Caption] = extensionNameList = new List<String>();

            if (!extensionNameList.Contains(info.Name))
                extensionNameList.Add(info.Name);
        }

        private void UncheckTabInfo(ExtensionDataInfo info)
        {
            List<String> extensionNameList;

            if (extensionTableNames != null && extensionTableNames.TryGetValue(info.ExtensionInfo.Caption, out extensionNameList))
            {
                extensionNameList.Remove(info.Name);
            }
        }

        [DataMember]
        public Guid ValueSourceSetting { get; set; }

        [DataMember]
        public Guid StructureSourceSetting { get; set; }

        public override Guid[] References
        {
            get
            {
                return new Guid[] { ValueSourceSetting, StructureSourceSetting };
            }
        }

        public override ReportParameter[] GetReportParameters()
        {
            return base.GetReportParameters();
        }

        public override byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    int extensionCount = extensionTableNames == null ? 0 : extensionTableNames.Count;

                    bw.Write(Enabled);

                    bw.Write(StructTableName);

                    bw.Write(extensionCount);
                    if(extensionCount>0)
                    foreach (var item in extensionTableNames.Keys)
                    {
                        int tableCount = extensionTableNames[item].Count;

                        bw.Write(item);
                        bw.Write(tableCount);
                        for (int i = 0; i < tableCount; i++)
                        {
                            bw.Write(extensionTableNames[item][i]);
                        }
                    }
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
                    int extensionCount, tableCount;
                    String extensionName;
                    String tableName;
                    List<String> tablesList;

                    Enabled = br.ReadBoolean();

                    StructTableName = br.ReadString();

                    extensionCount = br.ReadInt32();

                    if (extensionCount > 0)
                        extensionTableNames = new Dictionary<String, List<String>>();
                    for (int i = 0; i < extensionCount; i++)
                    {
                        extensionName = br.ReadString();
                        tableCount = br.ReadInt32();
                        tablesList = new List<String>();

                        for (int j = 0; j < tableCount; j++)
                        {
                            tableName = br.ReadString();
                            tablesList.Add(tableName);
                        }
                        extensionTableNames[extensionName] = tablesList;
                    }
                }
            }
        }

        public override object Clone()
        {
            return new ExtensionDataReportSourceSettings(this);
        }
    }
}

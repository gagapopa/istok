using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class MonitorTableNode : ParametrizedUnitNode
    {
        protected DataTable table = null;
        [Browsable(false)]
        public DataTable Table
        {
            get
            {
                try
                {
                    if (table != null) return table;
                    byte[] b = null;
                    b = GetBinaries("table");
                    if (b != null)
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream ms = new MemoryStream(b);
                        table = (DataTable)bf.Deserialize(ms);
                    }
                }
                catch
                {
                    table = null;
                }

                return table;
            }
            set
            {
                try
                {
                    if (value != null)
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream ms = new MemoryStream();
                        value.RemotingFormat = SerializationFormat.Binary;
                        bf.Serialize(ms, value);
                        SetBinaries("table", ms.ToArray());
                    }
                    else
                        Binaries["table"] = null;
                    table = value;
                }
                catch
                {
                    table = null;
                    Binaries["table"] = null;
                }
            }
        }

        [Category(""), DisplayName("Обновление"), Description("Интервал обновления таблицы (в секундах).")]
        public int UpdateInterval
        {
            get
            {
                int res;

                if (int.TryParse(GetAttribute("UpdateInterval"), out res))
                    return res;

                return 1;
            }
            set
            {
                SetAttribute("UpdateInterval", value.ToString());
            }
        }

        //[Category(""), DisplayName("Актуальность"), Description("Время, спустя которое значение перестает считаться актуальным (в секундах).")]
        //public int Relevance
        //{
        //    get
        //    {
        //        int res = 1;
        //        if (Attributes.ContainsKey("relevance")) int.TryParse(Attributes["relevance"], out res);
        //        //res /= 60;
        //        return res;
        //    }
        //    set
        //    {
        //        Attributes["relevance"] = value.ToString();
        //    }
        //}

        public MonitorTableNode() : base() { Typ = (int)UnitTypeId.MonitorTable; }
        public MonitorTableNode(DataRow row)
            : base(row)
        {
            if (Typ != (int)UnitTypeId.MonitorTable) throw new Exception("Неверный тип узла");
        }

        public override ChildParamNode CreateNewChildParamNode()
        {
            return new MonitorTableParamNode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class ImportDataContainer
    {
        public TreeWrapp<UnitNode>[] Nodes { get; set; }

        //public Package[] Values { get; set; }

        public Dictionary<String, Package[]> Values { get; set; }

        public ImportDataContainer(IEnumerable<TreeWrapp<UnitNode>> nodes, Dictionary<String, Package[]> values)
        {
            if (nodes != null)
                this.Nodes = nodes.ToArray();

            if (values != null)
                this.Values = values;
        }
    }
}

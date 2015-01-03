using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    [DataContract]
    public class GraphNode : ParametrizedUnitNode
    {
        const String updateIntervalAttributeName = "UpdateInterval";

        public GraphNode() : base() { Typ = (int)UnitTypeId.Graph; UpdateInterval = 5; }
        public GraphNode(DataRow row)
            : base(row)
        {
        }

        public override ChildParamNode CreateNewChildParamNode()
        {
            return new GraphParamNode();
        }

        [CategoryOrder(CategoryGroup.Appearance), DisplayName("Обновление"), Description("Интервал обновления графика (в секундах).")]
        public int UpdateInterval
        {
            get
            {
                int res = 1;

                if (int.TryParse(GetAttribute(updateIntervalAttributeName), out res))
                    return res;

                return 1;
            }
            set
            {
                SetAttribute(updateIntervalAttributeName, value.ToString());
            }
        }
    }
}

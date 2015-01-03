using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Сообщение, уведомляющие об изменениях UnitNode'а(ов)
    /// </summary>
    [DataContract]
    public class ServiceUnitNodeChange : ServiceDataChange
    {
        public ServiceUnitNodeChange(params int[] ids)
        {
            if (ids != null)
                NodeIDs = ids;
            else
                NodeIDs = new int[] { };
        }

        [DataMember]
        public int[] NodeIDs { get; private set; }

        public override bool Union(ServiceDataChange message)
        {
            ServiceUnitNodeChange unitNodeMessage;

            if ((unitNodeMessage = message as ServiceUnitNodeChange) != null)
            {
                NodeIDs = NodeIDs.Concat(unitNodeMessage.NodeIDs).ToArray();
                return true;
            }
            return false;
        }

        public override object Clone()
        {
            return new ServiceUnitNodeChange(NodeIDs);
        }
    }
}

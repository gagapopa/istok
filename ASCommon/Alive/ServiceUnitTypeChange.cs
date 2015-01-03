using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Сообщение, уведомляющие об изменениях UnitType'а(ов)
    /// </summary>
    [DataContract]
    public class ServiceUnitTypeChange : ServiceDataChange
    {
        public ServiceUnitTypeChange(params int[] ids)
        {
            if (ids != null)
                TypeIDs = ids;
            else
                TypeIDs = new int[] { };
        }

        [DataMember]
        public int[] TypeIDs { get; private set; }

        public override bool Union(ServiceDataChange message)
        {
            ServiceUnitTypeChange unitNodeMessage;

            if ((unitNodeMessage = message as ServiceUnitTypeChange) != null)
            {
                TypeIDs = TypeIDs.Concat(unitNodeMessage.TypeIDs).ToArray();
                return true;
            }
            return false;
        }

        public override object Clone()
        {
            return new ServiceUnitTypeChange(TypeIDs);
        }
    }
}

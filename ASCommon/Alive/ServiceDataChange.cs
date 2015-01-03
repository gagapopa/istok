using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Сообщение, передающиеся от глобала к клиенту. Уведомляющие о недавних изменениях.
    /// </summary>
    [DataContract]
    [KnownType(typeof(ServiceUnitNodeChange))]
    [KnownType(typeof(ServiceUnitTypeChange))]
    public class ServiceDataChange : ICloneable
    {
        /// <summary>
        /// Объеденить два сообщение
        /// </summary>
        /// <param name="message">Второе сообщение</param>
        /// <returns>Если сообщения объеденины, вернуть true. В противном случае false.</returns>
        public virtual bool Union(ServiceDataChange message)
        {
            return false;
        }

        #region ICloneable Members

        public virtual object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

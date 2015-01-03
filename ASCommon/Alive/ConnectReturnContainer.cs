using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Информация, передаваемая клиенту при подключении
    /// </summary>
    [DataContract]
    public class ConnectReturnContainer
    {
        /// <summary>
        /// ИД новой сессии
        /// </summary>
        [DataMember]
        public Guid UserGuid { get; set; }

        /// <summary>
        /// Время после последней активности, через которое сессия будет считаться просроченной
        /// </summary>
        [DataMember]
        public TimeSpan AliveTimeout { get; set; }
    }
}

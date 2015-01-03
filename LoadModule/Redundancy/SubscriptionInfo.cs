using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Block.Redundancy
{
    /// <summary>
    /// Информация о подписке
    /// </summary>
    [Serializable]
    public class SubscriptionInfo
    {
        public string PublisherName { get; set; }
        public string PublicationName { get; set; }
        public string PublicationDbName { get; set; }
        public string SubscriberName { get; set; }
        public string SubscriptionDbName { get; set; }
    }
}

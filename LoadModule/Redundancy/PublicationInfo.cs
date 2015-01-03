using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Block.Redundancy
{
    /// <summary>
    /// Информация о публикации и ее издателе
    /// </summary>
    [Serializable]
    public class PublicationInfo
    {
        public string PublisherName { get; set; }
        public string PublicationName { get; set; }
        public string PublicationDbName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;

namespace COTES.ISTOK
{
    /// <summary>
    /// Представления значения параметра с информацией о ИД параметра
    /// </summary>
    [Obsolete("Надо переходить на пару Package/ParamValueItem с хранением ИД параметра в пачке")]
    [Serializable]
    [DataContract]
    public class ParamValueItemWithID : ParamValueItem
    {
        /// <summary>
        /// ИД параметра
        /// </summary>
        [DataMember]
        public int ParameterID { get; set; }

        /// <summary>
        /// ИД канала сбора
        /// </summary>
        [DataMember]
        public int ChannelID { get; set; }

        public ParamValueItemWithID()
            : base()
        {
            this.ParameterID = 0;
        }

        public ParamValueItemWithID(DateTime time,int channelID, int parameterID, Quality quality, double value)
            : base(time, quality, value)
        {
            this.ParameterID = parameterID;
            this.ChannelID = channelID;
        }

        public ParamValueItemWithID(ParamValueItem baseValue)
            : base(baseValue)
        {
            ParamValueItemWithID paramItem = baseValue as ParamValueItemWithID;
            if (paramItem != null)
            {
                this.ParameterID = paramItem.ParameterID;
                this.ChannelID = paramItem.ChannelID;
            }
        }

        public ParamValueItemWithID(ParamValueItem valueItem, int parameterID)
            : base(valueItem)
        {
            this.ParameterID = parameterID;
        }

        public override object Clone()
        {
            return new ParamValueItemWithID(this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + ParameterID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ParamValueItemWithID paramItem = obj as ParamValueItemWithID;

            if (paramItem != null)
                return base.Equals(obj) && ParameterID.Equals(paramItem.ParameterID);
            return false;
        }
    }
}

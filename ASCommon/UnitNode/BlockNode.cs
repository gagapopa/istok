using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    [DataContract]
    public class BlockNode : UnitNode, IEquatable<BlockNode>
    {
        [DisplayName("Компьютер")]
        [ReadOnly(true)]
        [Browsable(false)]
        public string Host { get { return GetAttribute("Host"); } }
        [DisplayName("Порт")]
        [ReadOnly(true)]
        [Browsable(false)]
        public string Port { get { return GetAttribute("Port"); } }
        
        [DisplayName("Идентификатор"), Description("Идентификатор сервера сбора данных.")]
        [CategoryOrder(CategoryGroup.Load)]
        [TypeConverter(typeof(BlockUIDTypeConverter))]
        public string BlockUID
        {
            get
            {
                    return GetAttribute(CommonData.BlockUIDProperty);
            }
            set
            {
                SetAttribute(CommonData.BlockUIDProperty, value);
            }
        }

        public BlockNode()
            : base()
        {
        }

        public BlockNode(DataRow row)
            : base(row)
        {
            //modified = false;
        }

        #region IEquatable<BlockNode> Members

        public bool Equals(BlockNode other)
        {
            if (other != null && Idnum.Equals(other.Idnum)) return true;
            return false;
        }

        #endregion
    }
}

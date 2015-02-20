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
    /// Представление значения параметра
    /// </summary>
    [Serializable]
    [DataContract]
    [KnownType(typeof(CorrectedParamValueItem))]
    [KnownType(typeof(ParamValueItemWithID))]
    public class ParamValueItem : ICloneable, IComparable<ParamValueItem>
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual ArgumentsValues Arguments { get; set; }
        /// <summary>
        /// дата время до тысячных измерения
        /// </summary>
        public virtual DateTime Time { get; set; }
        /// <summary>
        /// качество значения
        /// </summary>
        public virtual Quality Quality { get; set; }

        private double val;
        /// <summary>
        /// значение параметра
        /// </summary>
        public virtual double Value { get { return val; } set { val = value; } }
        /// <summary>
        /// время изменения параметра
        /// </summary>
        public virtual DateTime ChangeTime { get; set; }

        public ParamValueItem()
            : this(null, DateTime.MinValue, Quality.Good, double.NaN)
        { }

        public ParamValueItem(ArgumentsValues arguments, DateTime time, Quality quality, double value)
        {
            Arguments = arguments;
            Time = time;
            Quality = quality;
            this.val = value;
            ChangeTime = DateTime.MinValue;
        }

        public ParamValueItem(DateTime time, Quality quality, double value)
            : this(null, time, quality, value)
        { }

        public ParamValueItem(ParamValueItem baseValue)
            : this()
        {
            if (baseValue != null)
            {
                this.Arguments = baseValue.Arguments;
                this.Time = baseValue.Time;
                this.ChangeTime = baseValue.ChangeTime;
                this.Quality = baseValue.Quality;
                this.Value = baseValue.Value;
            }
        }

        public override int GetHashCode()
        {
            return (Arguments == null ? 0 : Arguments.GetHashCode()) +
                   Time.GetHashCode() +
                   Quality.GetHashCode() +
                   Value.GetHashCode() +
                   ChangeTime.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ParamValueItem other = obj as ParamValueItem;
            if (other != null)
                return ((Arguments==null && other.Arguments==null) 
                       || (Arguments!=null&&Arguments.Equals(other.Arguments))) &&
                       Time.Equals(other.Time) &&
                       Quality.Equals(other.Quality) &&
                       Value.Equals(other.Value) &&
                       ChangeTime.Equals(other.ChangeTime);
            return false;
        }

        #region IComparable<ParamValueItem> Members

        public int CompareTo(ParamValueItem other)
        {
            return this.Time.CompareTo(other.Time);
        }

        #endregion

        #region ICloneable Members

        public virtual object Clone()
        {
            return new ParamValueItem(this);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.IO;
using System.IO.Compression;

namespace COTES.ISTOK
{
    /// <summary>
    /// Значение параметра с корректировкой значения, времени и/или аргумента
    /// </summary>
    [Serializable]
    public class CorrectedParamValueItem : ParamValueItem
    {
        ParamValueItem originalValue;

        public CorrectedParamValueItem(ParamValueItem originalValue, double correction, DateTime correctedTime):base(originalValue)
        {
            this.originalValue = originalValue;
            base.Value = correction;
            base.Time = correctedTime;
        }

        public CorrectedParamValueItem(ParamValueItem originalValue, double correction)
            : this(originalValue, correction, DateTime.MinValue)
        {

        }

        public CorrectedParamValueItem(ParamValueItem originalValue)
            : this(originalValue, double.NaN, DateTime.MinValue)
        {

        }

        public CorrectedParamValueItem(ParamValueItem originalValue, ParamValueItem correctedValue)
            : base(correctedValue)
        {
            this.originalValue = originalValue;
        }

        public override DateTime Time
        {
            get
            {
                if (base.Time == DateTime.MinValue)
                    return originalValue.Time;
                return base.Time;
            }
            set
            {
                base.Time = value;
            }
        }

        public override double Value
        {
            get
            {
                if (double.IsNaN(base.Value))
                    return originalValue.Value;
                return base.Value;
            }
            set
            {
                base.Value = value;
            }
        }

        /// <summary>
        /// Оригинальное значение параметра
        /// </summary>
        public ParamValueItem OriginalValueItem
        {
            get { return new ParamValueItem(originalValue); }
        }

        /// <summary>
        /// Скорректированное время параметра
        /// </summary>
        public ParamValueItem CorrectedValueItem
        {
            get
            {
                return new ParamValueItem()
                {
                    Arguments = base.Arguments,
                    Time = base.Time,
                    Quality = base.Quality,
                    Value = base.Value,
                    ChangeTime = base.ChangeTime
                };
            }
            //set
            //{
            //    throw new NotImplementedException();
            //}
        }

        public override object Clone()
        {
            return new CorrectedParamValueItem(OriginalValueItem, CorrectedValueItem);
        }
    }
}

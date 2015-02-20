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
    /// Пачка значений
    /// </summary>
    [Serializable]
    [DataContract]
    public class Package : ICloneable
    {
        /// <summary>
        /// ИД параметра
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Время начала пачки
        /// </summary>
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// Время конца пачки
        /// </summary>
        public DateTime DateTo { get; set; }

        /// <summary>
        /// Значения
        /// </summary>
        public List<ParamValueItem> Values { get; set; }

        /// <summary>
        /// Количество параметров в пачке
        /// </summary>
        public int Count { get { return Values.Count; } }

        public Package()
        {
            Values = new List<ParamValueItem>();
        }

        public Package(int parametID, IEnumerable<ParamValueItem> values):this()
        {
            this.Id = parametID;
            Add(values);
        }

        /// <summary>
        /// Добавить новое значение в пачку
        /// </summary>
        /// <param name="param"></param>
        public void Add(ParamValueItem param)
        {
            Values.Add(param);
            UpdateDates(param.Time);
        }

        public void Add(IEnumerable<ParamValueItem> vals)
        {
            foreach (var item in vals)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Обновить значение свойства DateFrom
        /// </summary>
        public void Normailze()
        {
            Values.Sort(ParamCompare);

            if (Count > 0)
            {
                DateFrom = Values[0].Time;
                UpdateDates(Values[Values.Count - 1].Time);
            }
        }

        public void UpdateDates(DateTime time)
        {
            if (time < DateFrom)
                DateFrom = time;
            if (time > DateTo)
                DateTo = time;
        }

        public PackedPackage Pack()
        {
            throw new NotImplementedException();
        }

        private int ParamCompare(ParamValueItem a, ParamValueItem b)
        {
            return DateTime.Compare(a.Time, b.Time);
        }

        #region ICloneable Members

        public virtual object Clone()
        {
            Package res = new Package();
            List<ParamValueItem> clonedList;

            res.Id = Id;
            res.DateFrom = DateFrom;
            res.DateTo = DateTo;
            res.Values = new List<ParamValueItem>();//Values);
            clonedList = new List<ParamValueItem>(Values);
            foreach (ParamValueItem param in clonedList)
                res.Values.Add((ParamValueItem)param.Clone());

            return res;
        }

        #endregion
    }
}

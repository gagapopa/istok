using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Modules
{
    public class ArchiveRequestTransaction
    {
        Dictionary<ParameterItem, Tuple<DateTime, DateTime>> transactionData = new Dictionary<ParameterItem, Tuple<DateTime, DateTime>>();

        public IEnumerable<ParameterItem> GetParameters()
        {
            return transactionData.Keys;
        }

        public void SetInterval(ParameterItem parameter, DateTime startTime, DateTime endTime)
        {
            transactionData[parameter] = Tuple.Create(startTime, endTime);
        }

        public DateTime GetStartTime(ParameterItem parameter)
        {
            return transactionData[parameter].Item1;
        }

        public DateTime GetEndTime(ParameterItem parameter)
        {
            return transactionData[parameter].Item2;
        }
    }
}

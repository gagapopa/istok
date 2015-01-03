using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    class TimeRange
    {
        public TimeRange(DateTime startTime, DateTime endTime)
        {
            this.Start = startTime;
            this.End = endTime;
        }
        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}

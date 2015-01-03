using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    class CommonContext : CallContext
    {
        private DateTime startTime;

        private Interval interval;

        String statusString;

        public CommonContext(CalcPosition node, String statusString, DateTime startTime, Interval interval, Instruction[] body)
            : base(startTime, interval.GetNextTime(startTime))
        {
            this.CurrentNode = node;
            this.statusString = statusString;
            this.startTime = startTime;
            this.interval = interval;
            this.body = body;
        }

        public override DateTime GetStartTime(int tau)
        {
            return interval.GetTime(CalcStartTime, -tau);
        }

        public override Interval GetInterval(int tau, int tautill)
        {
            return interval.Multiply(tautill - tau + 1);
        }

        public override void Prepare(ICalcContext calcContext)
        {
            base.Prepare(calcContext);
        }

        public override void Return(ICalcContext calcContext)
        {
            base.Return(calcContext);
            calcContext.Return();
        }

        public override string GetStatusString()
        {
            return statusString;
        }
    }
}

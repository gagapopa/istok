using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.Tests.Calc
{
    class TestCallContext : CallContext
    {
        public TestCallContext(DateTime startTime, DateTime endTime)
            : base(startTime, endTime)
        {
            CurrentNode = new CalcPosition();
        }

        public void SetBody(IEnumerable<Instruction> code)
        {
            this.body = code.ToArray();
        }

        public override DateTime GetStartTime(int tau)
        {
            throw new NotImplementedException();
        }

        public override Interval GetInterval(int tau, int tautill)
        {
            throw new NotImplementedException();
        }
    }
}

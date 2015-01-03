using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.Tests.Calc
{
    class TestOptimizationArgument : IOptimizationArgument
    {
        #region IOptimizationArgument Members

        public string Name
        {
            get;
            set;
        }

        public string Expression
        {
            get;
            set;
        }

        public OptimizationArgumentMode Mode
        {
            get;
            set;
        }

        public double IntervalFrom
        {
            get;
            set;
        }

        public double IntervalTo
        {
            get;
            set;
        }

        public double IntervalStep
        {
            get;
            set;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.Tests.Calc
{
    class TestParameterinfo : IParameterInfo
    {
        public TestParameterinfo(ICalcNode calcNode, RevisionInfo revision)
        {
            this.CalcNode = calcNode;
            this.Revision = revision;
            this.CalcNode.Revisions.Set(revision, this);
            Interval = Interval.Zero;
        }

        public TestParameterinfo(ICalcNode calcNode)
            : this(calcNode, RevisionInfo.Default)
        {

        }

        #region IParameterInfo Members

        public string Code { get; set; }

        public string Formula { get; set; }

        public bool RoundRobinCalc { get; set; }

        #endregion

        #region ICalcNodeInfo Members

        public ICalcNode CalcNode
        {
            get;
            protected set;
        }

        public RevisionInfo Revision { get; set; }

        public string Name
        {
            get { return CalcNode.Name; }
        }

        public Interval Interval { get; set; }

        public DateTime StartTime { get; set; }

        public bool Calculable { get; set; }

        private IOptimizationInfo optimization;

        public IOptimizationInfo Optimization
        {
            get { return optimization; }
            set
            {
                optimization = value;
                TestOptimizationInfo testOptimization = value as TestOptimizationInfo;
                if (testOptimization != null)
                {
                    testOptimization.SetChildParameter(this);
                }
            }
        }

        public IEnumerable<String> Needed { get; set; }
        #endregion

        public override bool Equals(object obj)
        {
            TestParameterinfo info = obj as TestParameterinfo;
            if (info != null)
            {
                return CalcNode.Equals(info.CalcNode) && Revision.Equals(info.Revision);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return CalcNode.GetHashCode() + Revision.GetHashCode();
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(Code))
                return base.ToString();

            return String.Format("${0}$ '{1}'", Code, CalcNode.Name);
        }
    }
}

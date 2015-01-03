using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.Tests.Calc
{
    class TestOptimizationInfo : IOptimizationInfo
    {
        public TestOptimizationInfo(ICalcNode calcNode, RevisionInfo revision)
        {
            this.CalcNode = calcNode;
            this.Revision = revision;
            this.CalcNode.Revisions.Set(revision, this);
        }

        public TestOptimizationInfo(ICalcNode calcNode)
            : this(calcNode, RevisionInfo.Default)
        {

        }
        #region IOptimizationInfo Members

        public IOptimizationArgument[] Arguments { get; set; }

        public string Expression { get; set; }

        public string DefinationDomain { get; set; }

        public bool Maximalize { get; set; }

        public bool CalcAllChildParameters { get; set; }

        public List<IParameterInfo> parameterList = new List<IParameterInfo>();

        public IEnumerable<IParameterInfo> ChildParameters { get { return parameterList.AsReadOnly(); } }

        public IEnumerable<IOptimizationInfo> ChildOptimization { get; set; }

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

        public IOptimizationInfo Optimization { get; set; }

        public IEnumerable<String> Needed
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        public override bool Equals(object obj)
        {
            TestOptimizationInfo info = obj as TestOptimizationInfo;
            if (info != null)
            {
                return CalcNode.Equals(info.CalcNode) && Revision.Equals(info.Revision);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //return base.GetHashCode();
            return CalcNode.GetHashCode() + Revision.GetHashCode();
        }

        public void SetChildParameter(IParameterInfo parameterInfo)
        {
            parameterList.Add(parameterInfo);
        }
    }
}

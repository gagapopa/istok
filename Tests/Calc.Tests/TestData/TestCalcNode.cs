using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.Tests.Calc
{
    class TestCalcNode : ICalcNode
    {
        RevisedStorage<ICalcNodeInfo> storage = new RevisedStorage<ICalcNodeInfo>();

        #region ICalcNode Members

        public int NodeID { get; set; }

        public string Name { get; set; }

        public RevisedStorage<ICalcNodeInfo> Revisions
        {
            get { return storage; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            TestCalcNode calcNode = obj as TestCalcNode;

            if (calcNode != null)
            {
                return NodeID.Equals(calcNode.NodeID);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return NodeID.GetHashCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    [TestFixture]
    public class CalcStateStorageTests
    {
        [Test]
        public void GetState_IParameterInfo_CreateNodeState()
        {
            var calcSupplierMock = new Moq.Mock<ICalcSupplier>();
            CalcStateStorage stateStorage = new CalcStateStorage();

            TestCalcNode calcNode = new TestCalcNode() { NodeID = 4, Name = "parameter" };
            TestParameterinfo parameterInfo = new TestParameterinfo(calcNode)
            {
                Interval =Interval.Hour,
                Code = "par1",
                Formula = "45+65"
            };

            NodeState state = stateStorage.GetState(calcNode, RevisionInfo.Default) as NodeState;

            Assert.IsNotNull(state);
        }

        [Test]
        public void GetState_IOptimizationInfo_CreateOptimizationState()
        {
            var calcSupplierMock = new Moq.Mock<ICalcSupplier>();
            CalcStateStorage stateStorage = new CalcStateStorage();

            TestCalcNode calcNode = new TestCalcNode() { NodeID = 4, Name = "parameter" };
            TestOptimizationInfo parameterInfo = new TestOptimizationInfo(calcNode)
            {
                Interval = Interval.Hour,
                Arguments = new IOptimizationArgument[0],
                Expression = "45+65"
            };

            OptimizationState state = stateStorage.GetState(calcNode, RevisionInfo.Default) as OptimizationState;

            Assert.IsNotNull(state);
        }

        [Test]
        public void GetState_DifferentRevision_ReternDifferentState()
        {
            var calcSupplierMock = new Moq.Mock<ICalcSupplier>();
            CalcStateStorage stateStorage = new CalcStateStorage();

            RevisionInfo revision2 = new RevisionInfo { ID = 2, Time = new DateTime(2012, 01, 01) };
            RevisionInfo revision56 = new RevisionInfo { ID = 56, Time = new DateTime(2012, 06, 01) };
            RevisionInfo revision1 = new RevisionInfo { ID = 1, Time = new DateTime(2011, 09, 15) };

            TestCalcNode calcNode = new TestCalcNode() { NodeID = 4, Name = "parameter" };
            TestParameterinfo parameterInfo1 = new TestParameterinfo(calcNode, revision1)
            {
                Interval = Interval.Hour,
                Code = "par1",
                Formula = "45+75"
            };
            TestParameterinfo parameterInfo2 = new TestParameterinfo(calcNode, revision2)
            {
                Interval = Interval.Hour,
                Code = "par1",
                Formula = "45+85"
            };
            TestParameterinfo parameterInfo56 = new TestParameterinfo(calcNode, revision56)
            {
                Interval = Interval.Hour,
                Code = "par1",
                Formula = "45+95"
            };

            NodeState state1 = stateStorage.GetState(calcNode, revision1) as NodeState;
            NodeState state2 = stateStorage.GetState(calcNode, revision2) as NodeState;
            NodeState state56 = stateStorage.GetState(calcNode, revision56) as NodeState;

            Assert.AreNotSame(state1, state2);
            Assert.AreNotEqual(state1, state2);
            Assert.AreNotSame(state1, state56);
            Assert.AreNotEqual(state1, state56);
            Assert.AreNotSame(state2, state56);
            Assert.AreNotEqual(state2, state56);
        }

        [Test]
        public void GetState__CreateOptimizationState()
        {
            var calcSupplierMock = new Moq.Mock<ICalcSupplier>();
            CalcStateStorage stateStorage = new CalcStateStorage();

            RevisionInfo revision2 = new RevisionInfo { ID = 2, Time = new DateTime(2012, 01, 01) };
            RevisionInfo revision56 = new RevisionInfo { ID = 56, Time = new DateTime(2012, 06, 01) };
            RevisionInfo revision1 = new RevisionInfo { ID = 1, Time = new DateTime(2011, 09, 15) };

            TestCalcNode calcNode = new TestCalcNode() { NodeID = 4, Name = "parameter" };
            TestParameterinfo parameterInfo1 = new TestParameterinfo(calcNode, revision1)
            {
                Interval = Interval.Hour,
                Code = "par1",
                Formula = "45+75"
            };
            TestParameterinfo parameterInfo2 = new TestParameterinfo(calcNode, revision2)
            {
                Interval = Interval.Hour,
                Code = "par1",
                Formula = "45+85"
            };
            TestParameterinfo parameterInfo56 = new TestParameterinfo(calcNode, revision56)
            {
                Interval = Interval.Hour,
                Code = "par1",
                Formula = "45+95"
            };

            NodeState state1_1 = stateStorage.GetState(calcNode, revision1) as NodeState;
            NodeState state2 = stateStorage.GetState(calcNode, revision2) as NodeState;
            NodeState state56 = stateStorage.GetState(calcNode, revision56) as NodeState;
            NodeState state1_2 = stateStorage.GetState(calcNode, revision1) as NodeState;

            Assert.AreSame(state1_1, state1_2);
        }

        [Test]
        public void GetState_ParameterHasOneRevision_ReturnDifferentStateOnDifferentRevision()
        {
            RevisionInfo firstRevision = new RevisionInfo() { ID = 1, Time = new DateTime(2012, 01, 01) };
            RevisionInfo secondRevision = new RevisionInfo() { ID = 3, Time = new DateTime(2013, 01, 02) };

            var calcSupplierMock = new Moq.Mock<ICalcSupplier>();

            CalcStateStorage stateStorage = new CalcStateStorage();

            TestCalcNode calcNode = new TestCalcNode() { NodeID = 4, Name = "parameter" };
            TestParameterinfo parameterInfo = new TestParameterinfo(calcNode)
            {
                Interval = Interval.Hour,
                Code = "par1",
                Formula = "45+65"
            };

            NodeState defaultState = stateStorage.GetState(calcNode, RevisionInfo.Default) as NodeState;
            NodeState firstState = stateStorage.GetState(calcNode, firstRevision) as NodeState;
            NodeState secondState = stateStorage.GetState(calcNode, secondRevision) as NodeState;

            Assert.IsNotNull(defaultState);
            Assert.IsNotNull(firstState);
            Assert.IsNotNull(secondState);

            Assert.AreNotEqual(defaultState, firstState);
            Assert.AreNotEqual(defaultState, secondState);
            Assert.AreNotEqual(firstState, secondState);
        }
    }
}

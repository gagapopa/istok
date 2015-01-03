using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    [TestFixture]
    public class CallContextTests
    {
        [Test]
        public void ctor__SetNeedPrepareToTrue()
        {
            TestCallContext callContext = new TestCallContext(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            Assert.AreEqual(true, callContext.NeedPrepare);
        }

        [Test]
        public void Prepare__SetFlagsToDefault()
        {
            var calcContext = new Moq.Mock<ICalcContext>();

            SymbolTable symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            TestCallContext callContext = new TestCallContext(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));
            callContext.SetBody(new Instruction[]
            {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.LeaveLevel)
            });

            callContext.Fail = true;

            callContext.Prepare(calcContext.Object);

            Assert.AreEqual(new DateTime(2012, 01, 01), callContext.CalcStartTime);
            Assert.AreEqual(new DateTime(2012, 02, 01), callContext.CalcEndTime);

            Assert.AreEqual(false, callContext.Fail);
            Assert.AreEqual(false, callContext.NeedPrepare);
        }

        [Test]
        public void Prepare__InitRetVariable()
        {
            var calcContext = new Moq.Mock<ICalcContext>();

            SymbolTable symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            TestCallContext callContext = new TestCallContext(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            callContext.Fail = true;

            callContext.Prepare(calcContext.Object);

            Variable retVar = symbolTable.GetSymbol("@ret") as Variable;
            Assert.IsNotNull(retVar);
            Assert.AreSame(SymbolValue.Nothing, retVar.Value);

            retVar.Value = SymbolValue.CreateValue(15);

            callContext.Prepare(calcContext.Object);

            retVar = symbolTable.GetSymbol("@ret") as Variable;
            Assert.IsNotNull(retVar);
            Assert.AreSame(SymbolValue.Nothing, retVar.Value);
        }

        [Test]
        public void Return__SetNeedPrepateToTrue()
        {
            var calcContext = new Moq.Mock<ICalcContext>();

            SymbolTable symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            TestCallContext callContext = new TestCallContext(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            callContext.Fail = true;

            callContext.Prepare(calcContext.Object);

            callContext.Return(calcContext.Object);

            Assert.AreEqual(true, callContext.NeedPrepare);
        }

        [Test]
        public void NextInstruction_OutOfBody_ReturnFalse()
        {
            var calcContext = new Moq.Mock<ICalcContext>();

            SymbolTable symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            TestCallContext callContext = new TestCallContext(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            var code = new Instruction[] { 
                new Instruction(null, Instruction.OperationCode.NOP),
                new Instruction(null, Instruction.OperationCode.NOP),
                new Instruction(null, Instruction.OperationCode.NOP),
                new Instruction(null, Instruction.OperationCode.NOP),
                new Instruction(null, Instruction.OperationCode.NOP),
                new Instruction(null, Instruction.OperationCode.NOP)
            };

            callContext.SetBody(code);

            callContext.Prepare(calcContext.Object);

            bool ret;
            for (int i = 0; i < 6; i++)
            {
                ret = callContext.NextInstruction(calcContext.Object);
                Assert.AreEqual(true, ret, String.Format("Код перестал выполняться на {0} операции", i));
                Assert.AreSame(code[i], callContext.CurrentInstruction);
            }
            ret = callContext.NextInstruction(calcContext.Object);
            Assert.AreEqual(false, ret);
        }

        [Test]
        public void NextInstruction_IsFail_ReturnFalse()
        {
            var calcContext = new Moq.Mock<ICalcContext>();

            SymbolTable symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            TestCallContext callContext = new TestCallContext(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            callContext.SetBody(new Instruction[] { 
                new Instruction(null, Instruction.OperationCode.NOP),
                new Instruction(null, Instruction.OperationCode.NOP),
                new Instruction(null, Instruction.OperationCode.NOP),
                new Instruction(null, Instruction.OperationCode.NOP),
                new Instruction(null, Instruction.OperationCode.NOP),
                new Instruction(null, Instruction.OperationCode.NOP)
            });

            callContext.Prepare(calcContext.Object);

            bool ret;

            ret = callContext.NextInstruction(calcContext.Object);
            Assert.AreEqual(true, ret);

            callContext.Fail = true;

            ret = callContext.NextInstruction(calcContext.Object);
            Assert.AreEqual(false, ret);
        }

        //[Test]
        //public void Fail_isTrue_SetNeedPrepareTo()
        //{
        //    var calcContext = new Moq.Mock<ICalcContext>();

        //    SymbolTable symbolTable = new SymbolTable(calcContext.Object);
        //    calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

        //    TestCallContext callContext = new TestCallContext(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

        //    callContext.Prepare(calcContext.Object);

        //    Assert.IsFalse(callContext.Fail);
        //    Assert.IsFalse(callContext.NeedPrepare);

        //    callContext.Fail = true;

        //    Assert.IsTrue(callContext.Fail);
        //    Assert.IsTrue(callContext.NeedPrepare);
        //}
    
    }
}

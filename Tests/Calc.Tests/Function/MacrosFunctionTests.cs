using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    [TestFixture]
    public class MacrosFunctionTests
    {
        [Test]
        public void CallCode_ReplaceArgumentsAddresses()
        {
            MacrosFunction function = new MacrosFunction(
                "max",
                new CalcArgumentInfo[]
                {
                    new CalcArgumentInfo("a", "", ParameterAccessor.In),
                    new CalcArgumentInfo("a","", ParameterAccessor.In)
                },
                "",
                "",
                new Instruction[] 
                { 
                    new Instruction(null, Instruction.OperationCode.Less, new Address("@ret"), new Address("a"), new Address("b")),
                    new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@ret"), new Address(2)),
                    new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("b")),
                    new Instruction(null, Instruction.OperationCode.Jump, new Address(2)),
                    new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("a"))
                });

            CompileContext compileContext = new CompileContext(null, RevisionInfo.Default);

            //bool newVariable;
            Address aArgument = compileContext.GetTempVariable(null);//, out newVariable);
            Address bArgument = compileContext.GetTempVariable(null);//, out newVariable);

            CodePart codePart = function.CallCode(compileContext, new Location(23, 5, 23, 30),
                new Tuple<Parameter, Address>[]
                {
                    Tuple.Create(new Parameter("a", ParameterAccessor.In), aArgument),
                    Tuple.Create(new Parameter("b", ParameterAccessor.In), bArgument),
                });

            var expectedBody = new Instruction[]
            {
                new Instruction(null, Instruction.OperationCode.Less, new Address("@temp2"), new Address("@temp0"), new Address("@temp1")),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp2"), new Address(2)),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp2"), new Address("@temp1")),
                new Instruction(null, Instruction.OperationCode.Jump, new Address(2)),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp2"), new Address("@temp0"))
            };

            CodeAssert.AreEqual(expectedBody, codePart.Code.ToArray());
            Assert.AreEqual(Address.AddressType.Symbol, codePart.Result.Type);
            Assert.AreEqual("@temp2", codePart.Result.SymbolName);
        }
    }
}

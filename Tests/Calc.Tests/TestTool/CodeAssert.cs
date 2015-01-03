using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    /// <summary>
    /// Класс, содержит статические методы для сравнения трехадресного кода
    /// </summary>
    public static class CodeAssert
    {
        /// <summary>
        /// Сравнить куски трехадресного кода
        /// </summary>
        /// <param name="expectedCode"></param>
        /// <param name="code"></param>
        public static void AreEqual(Instruction[] expectedCode, Instruction[] code)
        {
            Assert.IsNotNull(code);
            int commonLength = Math.Max(expectedCode.Length, code.Length);

            for (int i = 0; i < commonLength; i++)
            {
                Instruction expectedInstruction = expectedCode[i];
                Instruction instruction = code[i];

                String message = String.Format("Expected: {0}: {1}, \n  But was:  {0}: {2}", i, expectedInstruction, instruction);

                Assert.AreEqual(expectedInstruction.Operation, instruction.Operation, message);
                AssertAddress(expectedInstruction.A, instruction.A, message);
                AssertAddress(expectedInstruction.B, instruction.B, message);
                AssertAddress(expectedInstruction.C, instruction.C, message);
            }
            Assert.AreEqual(expectedCode.Length, code.Length);
        }

        /// <summary>
        /// Сравнить адреса для трехадресного кода
        /// </summary>
        /// <param name="expectedAddress"></param>
        /// <param name="returnedAddress"></param>
        /// <param name="message"></param>
        private static void AssertAddress(Address expectedAddress, Address returnedAddress, String message)
        {
            if (expectedAddress == null)
            {
                Assert.IsNull(returnedAddress, message);
            }
            else
            {
                Assert.IsNotNull(returnedAddress, message);
                Assert.AreEqual(expectedAddress.Type, returnedAddress.Type, message);
                switch (expectedAddress.Type)
                {
                    case Address.AddressType.Value:
                        Assert.AreEqual(expectedAddress.Value.GetType(), returnedAddress.Value.GetType(), message);
                        Assert.AreEqual(expectedAddress.Value.GetValue(), returnedAddress.Value.GetValue(), message);
                        break;
                    case Address.AddressType.Symbol:
                    case Address.AddressType.Parameter:
                        Assert.AreEqual(expectedAddress.SymbolName, returnedAddress.SymbolName, message);
                        break;
                    case Address.AddressType.Label:
                        Assert.AreEqual(expectedAddress.ReferenceIndex, returnedAddress.ReferenceIndex, message);
                        break;
                    case Address.AddressType.ArrayElement:
                        AssertAddress(expectedAddress.ArrayAddress, returnedAddress.ArrayAddress, message);
                        AssertAddress(expectedAddress.ArrayIndex, returnedAddress.ArrayIndex, message);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

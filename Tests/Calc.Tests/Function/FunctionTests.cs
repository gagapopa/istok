using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    [TestFixture]
    public class FunctionTests
    {
        [Test]
        public void LoadArguments__()
        {
            TestFunction testFunction = new TestFunction("func1", RevisionInfo.Default, new CalcArgumentInfo[] 
            {
                new CalcArgumentInfo("a", "", ParameterAccessor.In),
                new CalcArgumentInfo("b", "", ParameterAccessor.In),
                new CalcArgumentInfo("c", "", "0", ParameterAccessor.In),
            });

            var calcContext = new Moq.Mock<ICalcContext>();

            SymbolTable symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("a", SymbolValue.CreateValue(15)));
            symbolTable.DeclareSymbol(new Variable("b", SymbolValue.CreateValue(20)));

            var res=testFunction.TestLoadArguments(calcContext.Object);

            Assert.IsNotNull(res);
            Assert.AreEqual(3, res.Length);
            Assert.AreEqual(15.0, res[0]);
            Assert.AreEqual(20.0, res[1]);
            Assert.AreEqual(0.0, res[2]);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void LoadArguments__ReportError()
        {
            TestFunction testFunction = new TestFunction("func1", RevisionInfo.Default, new CalcArgumentInfo[] 
            {
                new CalcArgumentInfo("a", "", ParameterAccessor.In),
                new CalcArgumentInfo("b", "", ParameterAccessor.In),
                new CalcArgumentInfo("c", "",  ParameterAccessor.In),
            });

            var calcContext = new Moq.Mock<ICalcContext>();

            SymbolTable symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("a", SymbolValue.CreateValue(15)));
            symbolTable.DeclareSymbol(new Variable("b", SymbolValue.CreateValue(20)));

            var res = testFunction.TestLoadArguments(calcContext.Object);

            calcContext.Verify(c =>
                c.AddMessage(Moq.It.Is<CalcMessage>(m =>
                    m.Category == MessageCategory.Error && m.Text == "Функции func1(a, b, c) не передан обязательный аргумент c")));

            Assert.IsNull(res);
        }
    }
}

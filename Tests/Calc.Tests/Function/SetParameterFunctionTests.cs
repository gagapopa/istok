using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    [TestFixture]
    class SetParameterFunctionTests
    {
        [Test]
        public void Subroutine__SetParameterValue()
        {
            ICalcNode calcNode = new TestCalcNode() { NodeID = 1 };

            IParameterInfo parameter = new TestParameterinfo(calcNode) { Code = "par1", Interval = Interval.Day };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            // настраиваем запрос ревизий
            calcSupplier.Setup(s =>
                s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>()))
                .Returns(new RevisionInfo[] { RevisionInfo.Default });

            // настраиваем запрос параметра по ревизии
            calcSupplier.Setup(s =>
                s.GetParameterNode(Moq.It.IsAny<ICalcContext>(), Moq.It.IsAny<RevisionInfo>(), "par1"))
                .Returns(parameter);

            var calcContext = new Moq.Mock<ICalcContext>();
            var callContext = new Moq.Mock<ICallContext>();
            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            callContext.Setup(s =>
                s.StartTime)
                .Returns(new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc));

            callContext.Setup(s =>
                s.EndTime)
                .Returns(new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc));

            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@value", SymbolValue.CreateValue(23)));

            SetParameterFunction function = new SetParameterFunction(calcSupplier.Object, "set_param", "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());

            valueKeeper.Verify(k => k.AddCalculatedValue(calcNode, null, new DateTime(2012, 01, 01), Moq.It.Is<DoubleValue>(v => v.GetValue().Equals(23.0))));
        }
        
        [Test]
        public void Subroutine_DeffirentInterval_ReportError()
        {
            ICalcNode calcNode = new TestCalcNode() { NodeID = 1 };

            IParameterInfo parameter = new TestParameterinfo(calcNode) { Code = "par1", Interval = Interval.Day };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            // настраиваем запрос ревизий
            calcSupplier.Setup(s =>
                s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>()))
                .Returns(new RevisionInfo[] { RevisionInfo.Default });

            // настраиваем запрос параметра по ревизии
            calcSupplier.Setup(s =>
                s.GetParameterNode(Moq.It.IsAny<ICalcContext>(), Moq.It.IsAny<RevisionInfo>(), "par1"))
                .Returns(parameter);

            var calcContext = new Moq.Mock<ICalcContext>();
            var callContext = new Moq.Mock<ICallContext>();
            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            callContext.Setup(s =>
                s.StartTime)
                .Returns(new DateTime(2012, 01, 01));

            callContext.Setup(s =>
                s.EndTime)
                .Returns(new DateTime(2012, 02, 01));

            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@value", SymbolValue.CreateValue(23)));

            SetParameterFunction function = new SetParameterFunction(calcSupplier.Object, "set_param", "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c => c.AddMessage(Moq.It.Is<CalcMessage>(m =>
                m.Category == MessageCategory.Error && m.Text == "Попыка сохранить значение параметра $par1$ при неверной дискретности")));

            valueKeeper.Verify(k => k.AddCalculatedValue(Moq.It.IsAny<ICalcNode>(), Moq.It.IsAny<ArgumentsValues>(), Moq.It.IsAny<DateTime>(), Moq.It.IsAny<SymbolValue>()), Moq.Times.Never());
        }
    }
}

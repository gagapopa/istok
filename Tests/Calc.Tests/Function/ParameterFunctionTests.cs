using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    [TestFixture]
    public class ParameterFunctionTests
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ctor_AgregationNothing_ThrowException()
        {
            new ParameterFunction(null, "get", CalcAggregation.Nothing, "Агрегация", "Получить расчитанное значение");
        }

        [Test]
        public void Subroutine_DifferentCalcNodeOnSameCode_ReportError()
        {
            RevisionInfo revision1 = new RevisionInfo() { ID = 1, Time = new DateTime(2011, 01, 01) };
            RevisionInfo revision2 = new RevisionInfo() { ID = 4, Time = new DateTime(2012, 01, 15) };

            ICalcNode calcNode1 = new TestCalcNode() { NodeID = 1 };
            ICalcNode calcNode2 = new TestCalcNode() { NodeID = 2 };

            IParameterInfo parameter1 = new TestParameterinfo(calcNode1, revision1) { Code = "par1" };
            IParameterInfo parameter2 = new TestParameterinfo(calcNode2, revision2) { Code = "par1" };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            // настраиваем запрос ревизий
            calcSupplier.Setup(
                s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>()))
                .Returns(new RevisionInfo[] { revision1, revision2 });
            calcSupplier.Setup(
                s => s.GetRevision(Moq.It.Is<DateTime>(t => t < revision2.Time)))
                .Returns(revision1);
            calcSupplier.Setup(
                s => s.GetRevision(Moq.It.Is<DateTime>(t => t >= revision2.Time)))
                .Returns(revision2);

            // настраиваем запрос параметра по ревизии
            calcSupplier.Setup(s =>
                s.GetParameterNode(Moq.It.IsAny<ICalcContext>(), Moq.It.Is<RevisionInfo>(r => r.Equals(revision1)), "par1"))
                .Returns(parameter1);

            calcSupplier.Setup(s =>
                s.GetParameterNode(Moq.It.IsAny<ICalcContext>(), Moq.It.Is<RevisionInfo>(r => r.Equals(revision2)), "par1"))
                .Returns(parameter2);

            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            valueKeeper.Setup(k =>
                k.GetValue(
                Moq.It.IsAny<CalcAggregation>(), 
                Moq.It.IsAny<DateTime>(), 
                Moq.It.IsAny<Interval>(), 
                Moq.It.IsAny<CalcNodeKey[]>()))
                .Returns((SymbolValue)null);

            var calcContext = new Moq.Mock<ICalcContext>();
            var callContext = new Moq.Mock<ICallContext>();

            callContext.Setup(s =>
                s.GetStartTime(Moq.It.IsAny<int>()))
                .Returns(new DateTime(2012, 01, 01));

            callContext.Setup(s =>
                s.GetInterval(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns(Interval.Month);
            
            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@tau", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@tautill", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@ret", SymbolValue.Nothing));

            ParameterFunction function = new ParameterFunction(calcSupplier.Object, "sum", CalcAggregation.Sum, "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c => 
                c.AddMessage(Moq.It.Is<CalcMessage>(m => 
                    m.Category == MessageCategory.Error && m.Text.Equals("Параметр $par1$ за запрашиваемый период сменил код"))));
        }

        [Test]
        public void Subroutine_DifferentOptimization_ReportError()
        {
            RevisionInfo revision1 = new RevisionInfo() { ID = 1, Time = new DateTime(2011, 01, 01) };
            RevisionInfo revision2 = new RevisionInfo() { ID = 4, Time = new DateTime(2012, 01, 15) };

            ICalcNode calcNode = new TestCalcNode() { NodeID = 1 };
            ICalcNode optimizationCalcNode = new TestCalcNode() { NodeID = 2 };

            IOptimizationInfo optimizatinInfo1 = new TestOptimizationInfo(optimizationCalcNode, revision1)
            {
                Expression = "par1;",
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument(){Name="a"}
                }
            };
            IOptimizationInfo optimizatinInfo2 = new TestOptimizationInfo(optimizationCalcNode, revision2)
            {
                Expression = "-par1;",
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument(){Name="a"}
                }
            };

            IParameterInfo parameter1 = new TestParameterinfo(calcNode, revision1) { Code = "par1", Optimization = optimizatinInfo1 };
            IParameterInfo parameter2 = new TestParameterinfo(calcNode, revision2) { Code = "par1", Optimization = optimizatinInfo2 };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            // настраиваем запрос ревизий
            calcSupplier.Setup(s =>
                s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>()))
                .Returns(new RevisionInfo[] { revision1, revision2 });

            // настраиваем запрос параметра по ревизии
            calcSupplier.Setup(s =>
                s.GetParameterNode(Moq.It.IsAny<ICalcContext>(), Moq.It.Is<RevisionInfo>(r => r.Equals(revision1)), "par1"))
                .Returns(parameter1);

            calcSupplier.Setup(s =>
                s.GetParameterNode(Moq.It.IsAny<ICalcContext>(), Moq.It.Is<RevisionInfo>(r => r.Equals(revision2)), "par1"))
                .Returns(parameter2);

            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            valueKeeper.Setup(k =>
                k.GetValue(
                Moq.It.IsAny<CalcAggregation>(),
                Moq.It.IsAny<DateTime>(),
                Moq.It.IsAny<Interval>(),
                Moq.It.IsAny<CalcNodeKey[]>()))
                .Returns((SymbolValue)null);

            var calcContext = new Moq.Mock<ICalcContext>();
            var callContext = new Moq.Mock<ICallContext>();

            callContext.Setup(s =>
                s.GetStartTime(Moq.It.IsAny<int>()))
                .Returns(new DateTime(2012, 01, 01));

            callContext.Setup(s =>
                s.GetInterval(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns(Interval.Month);

            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@tau", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@tautill", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@ret", SymbolValue.Nothing));

            ParameterFunction function = new ParameterFunction(calcSupplier.Object, "sum", CalcAggregation.Sum, "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c =>
                c.AddMessage(Moq.It.Is<CalcMessage>(m =>
                    m.Category == MessageCategory.Error && m.Text.Equals("У параметра $par1$ за запрашиваемый период изменились настройки оптимизации"))));
        }

        [Test]
        public void Subroutine_ValueKeeperHasValue_ReturnValue()
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
                s.GetStartTime(Moq.It.IsAny<int>()))
                .Returns(new DateTime(2012, 01, 01));

            callContext.Setup(s =>
                s.GetInterval(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns(Interval.Month);

            valueKeeper.Setup(k =>
                k.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 01), Interval.Month, new CalcNodeKey(calcNode, null)))
                .Returns(SymbolValue.CreateValue(42));

            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@tau", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@tautill", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@ret", SymbolValue.Nothing));

            ParameterFunction function = new ParameterFunction(calcSupplier.Object, "sum", CalcAggregation.Sum, "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());

            Variable retVar = symbolTabl.GetSymbol("@ret") as Variable;

            Assert.IsNotNull(retVar);
            Assert.AreEqual(42, retVar.Value.GetValue());
        }

        [Test]
        public void Subroutine_CalcSupplierHasValue_ReturnValue()
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

            List<Message> messages = null;
            bool serverNA = false;
            calcSupplier.Setup(s =>
                s.GetParameterNodeValue(
                    Moq.It.IsAny<ICalcContext>(),
                    Moq.It.Is<IEnumerable<Tuple<ICalcNode, ArgumentsValues>>>(c =>
                        c.Count() == 1
                        && c.First().Item1.Equals(calcNode)
                        && c.First().Item2 == null),
                    CalcAggregation.Sum,
                    Moq.It.Is<DateTime>(dt => dt.Equals(new DateTime(2012, 01, 01))),
                    Interval.Month,
                    out messages,
                    out serverNA))
                .Returns(new ParamValueItem() { Time = new DateTime(2012, 01, 01), Quality = Quality.Good, Value = 42 });

            var calcContext = new Moq.Mock<ICalcContext>();
            var callContext = new Moq.Mock<ICallContext>();
            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            callContext.Setup(s =>
                s.GetStartTime(Moq.It.IsAny<int>()))
                .Returns(new DateTime(2012, 01, 01));

            callContext.Setup(s =>
                s.GetInterval(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns(Interval.Month);

            valueKeeper.Setup(k =>
                k.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 01), Interval.Month, new CalcNodeKey(calcNode, null)))
                .Returns((SymbolValue)null);

            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@tau", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@tautill", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@ret", SymbolValue.Nothing));

            ParameterFunction function = new ParameterFunction(calcSupplier.Object, "sum", CalcAggregation.Sum, "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());

            Variable retVar = symbolTabl.GetSymbol("@ret") as Variable;

            Assert.IsNotNull(retVar);
            Assert.AreEqual(42, retVar.Value.GetValue());
            // проверяем, добавлено ли в кэш полученное из БД значение 
            valueKeeper.Verify(k => 
                k.AddValue(
                CalcAggregation.Sum,
                new DateTime(2012, 01, 01), 
                Interval.Month,
                Moq.It.Is<DoubleValue>(v=>v.GetValue().Equals(42.0)),
                Moq.It.Is<CalcNodeKey[]>(a => a.Length == 1 && a[0].Node.Equals(calcNode) && a[0].Arguments == null)));
        }

        [Test]
        public void Subroutine_ForceCalcNCalcSupplierHasValue_DontReturnValue()
        {
            ICalcNode calcNode = new TestCalcNode() { NodeID = 1 };

            IParameterInfo parameter = new TestParameterinfo(calcNode)
            {
                Calculable = true,
                Code = "par1",
                Interval = Interval.Day
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            // настраиваем запрос ревизий
            calcSupplier.Setup(s =>
                s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>()))
                .Returns(new RevisionInfo[] { RevisionInfo.Default });

            // настраиваем запрос параметра по ревизии
            calcSupplier.Setup(s =>
                s.GetParameterNode(Moq.It.IsAny<ICalcContext>(), Moq.It.IsAny<RevisionInfo>(), "par1"))
                .Returns(parameter);

            List<Message> messages = null;
            bool serverNA = false;
            calcSupplier.Setup(s =>
                s.GetParameterNodeValue(
                    Moq.It.IsAny<ICalcContext>(),
                    //Moq.It.Is<ICalcNode>(c => c.Equals(calcNode)),
                    Moq.It.Is<IEnumerable<Tuple<ICalcNode, ArgumentsValues>>>(c =>
                        c.Count() == 1
                        && c.First().Item1.Equals(calcNode)
                        && c.First().Item2 == null), 
                    CalcAggregation.Sum,
                    //null,
                    Moq.It.Is<DateTime>(dt => dt.Equals(new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc))),
                    Interval.Month,
                    out messages,
                    out serverNA))
                .Returns(new ParamValueItem() { Time = new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Quality = Quality.Good, Value = 42 });

            var calcContext = new Moq.Mock<ICalcContext>();
            var callContext = new Moq.Mock<ICallContext>();
            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            callContext.Setup(s =>
                s.GetStartTime(Moq.It.IsAny<int>()))
                .Returns(new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc));

            callContext.Setup(s =>
                s.GetInterval(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns(Interval.Month);

            valueKeeper.Setup(k =>
                k.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Month, new CalcNodeKey(calcNode, null)))
                .Returns((SymbolValue)null);

            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);
            calcContext.Setup(c => c.ForceCalc(parameter)).Returns(true);
            calcContext.Setup(c => c.Call(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), Moq.It.IsAny<CalcNodeKey[]>()))
                .Returns(true)
                .Callback((DateTime st, DateTime et, CalcNodeKey[] nodes) =>
                    symbolTabl.PushSymbolScope(new CommonContext(null, null, st, Interval.Zero, null)));

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@tau", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@tautill", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@ret", SymbolValue.Nothing));

            ParameterFunction function = new ParameterFunction(calcSupplier.Object, "sum", CalcAggregation.Sum, "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
            callContext.Verify(c => c.JumpShift(Moq.It.IsAny<ICalcContext>(), -1));
            calcContext.Verify(c => c.Call(new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc), new CalcNodeKey(calcNode, null)));
        }

        [Test]
        public void Subroutine_HasntExplicitNodeArgsNotCalculableOptimization_ReportError()
        {
            ICalcNode calcNode = new TestCalcNode() { NodeID = 1 };
            ICalcNode optimizationCalcNode = new TestCalcNode() { NodeID = 2 };

            IOptimizationInfo optimizatinInfo = new TestOptimizationInfo(optimizationCalcNode)
            {
                Calculable = false,
                Expression = "par1;",
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]{
                    new TestOptimizationArgument(){Name="a", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1},
                    new TestOptimizationArgument(){Name="b", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1},
                    new TestOptimizationArgument(){Name="c", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1}
                }
            };

            IParameterInfo parameter = new TestParameterinfo(calcNode)
            {
                Code = "par1",
                Interval = Interval.Day,
                Optimization = optimizatinInfo
            };

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
                s.GetStartTime(Moq.It.IsAny<int>()))
                .Returns(new DateTime(2012, 01, 01));

            callContext.Setup(s =>
                s.GetInterval(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns(Interval.Month);

            // в кэше нет значений
            valueKeeper.Setup(k =>
                k.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 01), Interval.Month, new CalcNodeKey(calcNode, null)))
                .Returns((SymbolValue)null);

            // в БД нет значений
            List<Message> messages = null;
            bool serverNA = false;
            calcSupplier.Setup(s =>
                s.GetParameterNodeValue(
                    Moq.It.IsAny<ICalcContext>(), 
                    //calcNode, 
                     Moq.It.Is<IEnumerable<Tuple<ICalcNode, ArgumentsValues>>>(c =>
                        c.Count() == 1
                        && c.First().Item1.Equals(calcNode)
                        && c.First().Item2 == null),
                    CalcAggregation.Sum, 
                    //null, 
                    new DateTime(2012, 01, 01), 
                    Interval.Month,
                    out messages, 
                    out serverNA))
                .Returns((ParamValueItem)null);

            // зато в кэше есть оптимальные аргументы
            valueKeeper.Setup(k =>
                k.GetOptimal(optimizationCalcNode, null, new DateTime(2012, 01, 01)))
                .Returns(new ArgumentsValues(new { a = 8, b = 6, c = 7 }));

            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@tau", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@tautill", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@ret", SymbolValue.Nothing));

            ParameterFunction function = new ParameterFunction(calcSupplier.Object, "sum", CalcAggregation.Sum, "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c => 
                c.AddMessage(
                Moq.It.Is<CalcMessage>(m => m.Category == MessageCategory.Error && m.Text.Equals("Параметр $par1$ должен вызываться с аргументами (a, b, c)"))));

            calcContext.Verify(c =>
                c.Call(
                Moq.It.IsAny<DateTime>(),
                Moq.It.IsAny<DateTime>(),
                Moq.It.IsAny<CalcNodeKey[]>()),
                Moq.Times.Never());
        }

        [Test]
        public void Subroutine_HasPartlyExplicitNodeArgs_ReportError()
        {
            ICalcNode calcNode = new TestCalcNode() { NodeID = 1 };
            ICalcNode optimizationCalcNode = new TestCalcNode() { NodeID = 2 };

            IOptimizationInfo optimizatinInfo = new TestOptimizationInfo(optimizationCalcNode)
            {
                Calculable = true,
                Expression = "par1;",
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]{
                    new TestOptimizationArgument(){Name="a", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1},
                    new TestOptimizationArgument(){Name="b", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1},
                    new TestOptimizationArgument(){Name="c", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1}
                }
            };

            IParameterInfo parameter = new TestParameterinfo(calcNode)
            {
                Code = "par1",
                Interval = Interval.Day,
                Optimization = optimizatinInfo
            };

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
                s.GetStartTime(Moq.It.IsAny<int>()))
                .Returns(new DateTime(2012, 01, 01));

            callContext.Setup(s =>
                s.GetInterval(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns(Interval.Month);

            // в кэше нет значений
            valueKeeper.Setup(k =>
                k.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 01), Interval.Month, new CalcNodeKey(calcNode, null)))
                .Returns((SymbolValue)null);

            // в БД нет значений
            List<Message> messages = null;
            bool serverNA = false;
            calcSupplier.Setup(s =>
                s.GetParameterNodeValue(
                    Moq.It.IsAny<ICalcContext>(),
                    //calcNode, 
                     Moq.It.Is<IEnumerable<Tuple<ICalcNode, ArgumentsValues>>>(c =>
                        c.Count() == 1
                        && c.First().Item1.Equals(calcNode)
                        && c.First().Item2 == null),
                    CalcAggregation.Sum, 
                    //null,
                    new DateTime(2012, 01, 01), 
                    Interval.Month,
                    out messages, 
                    out serverNA))
                .Returns((ParamValueItem)null);

            // зато в кэше есть оптимальные аргументы
            valueKeeper.Setup(k =>
                k.GetOptimal(optimizationCalcNode, null, new DateTime(2012, 01, 01)))
                .Returns(new ArgumentsValues(new { a = 8, b = 6, c = 7 }));

            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@tau", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@tautill", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@ret", SymbolValue.Nothing));
            symbolTabl.DeclareSymbol(new Variable("@1@a", SymbolValue.CreateValue(32)));

            ParameterFunction function = new ParameterFunction(calcSupplier.Object, "sum", CalcAggregation.Sum, "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c =>
                c.AddMessage(
                Moq.It.Is<CalcMessage>(m => m.Category == MessageCategory.Error && m.Text.Equals("Параметр $par1$ должен вызываться с аргументами (a, b, c)"))));

            calcContext.Verify(c =>
                c.Call(
                Moq.It.IsAny<DateTime>(),
                Moq.It.IsAny<DateTime>(),
                Moq.It.IsAny<CalcNodeKey[]>()),
                Moq.Times.Never());
        }

        [Test]
        public void Subroutine_MultilevelOptimizationExplicitAfterImplicitArgs_ReportError()
        {
            ICalcNode calcNode = new TestCalcNode() { NodeID = 1 };
            ICalcNode optimizationCalcNode = new TestCalcNode() { NodeID = 2 };
            ICalcNode baseOptimizationCalcNode = new TestCalcNode() { NodeID = 4 };

            IOptimizationInfo baseOptimizationInfo = new TestOptimizationInfo(baseOptimizationCalcNode)
            {
                Calculable = true,
                Expression = "1/par1;",
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument() { Name="N" }
                }
            };

            IOptimizationInfo optimizatinInfo = new TestOptimizationInfo(optimizationCalcNode)
            {
                Calculable = true,
                Expression = "par1;",
                Interval = Interval.Day,
                Optimization = baseOptimizationInfo,
                Arguments = new IOptimizationArgument[]{
                    new TestOptimizationArgument(){Name="a", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1},
                    new TestOptimizationArgument(){Name="b", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1},
                    new TestOptimizationArgument(){Name="c", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1}
                }
            };

            IParameterInfo parameter = new TestParameterinfo(calcNode)
            {
                Code = "par1",
                Interval = Interval.Day,
                Optimization = optimizatinInfo
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            // настраиваем запрос ревизий
            calcSupplier.Setup(
                s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>()))
                .Returns(new RevisionInfo[] { RevisionInfo.Default });
            calcSupplier.Setup(
                s => s.GetRevision(Moq.It.IsAny<DateTime>()))
                .Returns(RevisionInfo.Default);

            // настраиваем запрос параметра по ревизии
            calcSupplier.Setup(s =>
                s.GetParameterNode(Moq.It.IsAny<ICalcContext>(), Moq.It.IsAny<RevisionInfo>(), "par1"))
                .Returns(parameter);

            var calcContext = new Moq.Mock<ICalcContext>();
            var callContext = new Moq.Mock<ICallContext>();
            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            callContext.Setup(s =>
                s.GetStartTime(Moq.It.IsAny<int>()))
                .Returns(new DateTime(2012, 01, 01));

            callContext.Setup(s =>
                s.GetInterval(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns(Interval.Month);

            // в кэше нет значений
            valueKeeper.Setup(k =>
                k.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 01), Interval.Month, new CalcNodeKey(calcNode, null)))
                .Returns((SymbolValue)null);

            // в БД нет значений
            List<Message> messages = null;
            bool serverNA = false;
            calcSupplier.Setup(s =>
                s.GetParameterNodeValue(
                    Moq.It.IsAny<ICalcContext>(),
                    //calcNode, 
                     Moq.It.Is<IEnumerable<Tuple<ICalcNode, ArgumentsValues>>>(c =>
                        c.Count() == 1
                        && c.First().Item1.Equals(calcNode)
                        && c.First().Item2 == null),
                    CalcAggregation.Sum, 
                    //null, 
                    new DateTime(2012, 01, 01), 
                    Interval.Month,
                    out messages, 
                    out serverNA))
                .Returns((ParamValueItem)null);

            // зато в кэше есть оптимальные аргументы
            valueKeeper.Setup(k =>
                k.GetOptimal(baseOptimizationCalcNode, null, new DateTime(2012, 01, 01)))
                .Returns(new ArgumentsValues(new { N = 9 }));

            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@tau", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@tautill", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@ret", SymbolValue.Nothing));
            symbolTabl.DeclareSymbol(new Variable("@1@a", SymbolValue.CreateValue(32)));
            symbolTabl.DeclareSymbol(new Variable("@1@b", SymbolValue.CreateValue(4)));
            symbolTabl.DeclareSymbol(new Variable("@1@c", SymbolValue.CreateValue(7)));

            ParameterFunction function = new ParameterFunction(calcSupplier.Object, "sum", CalcAggregation.Sum, "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c =>
                c.AddMessage(
                Moq.It.Is<CalcMessage>(m => m.Category == MessageCategory.Error && m.Text.Equals("Параметр $par1$ должен вызываться с аргументами (N, a, b, c)"))));

            calcContext.Verify(c =>
                c.Call(
                Moq.It.IsAny<DateTime>(),
                Moq.It.IsAny<DateTime>(),
                Moq.It.IsAny<CalcNodeKey[]>()),
                Moq.Times.Never());
        }

        [Test]
        public void Subroutine_NotCalculableParameterHasntValue_ReportError()
        {
            ICalcNode calcNode = new TestCalcNode() { NodeID = 1 };

            IParameterInfo parameter = new TestParameterinfo(calcNode)
            {
                Code = "par1",
                Interval = Interval.Day
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            // настраиваем запрос ревизий
            calcSupplier.Setup(s =>
                s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>()))
                .Returns(new RevisionInfo[] { RevisionInfo.Default });
            calcSupplier.Setup(
                s => s.GetRevision(Moq.It.IsAny<DateTime>()))
                .Returns(RevisionInfo.Default);

            // настраиваем запрос параметра по ревизии
            calcSupplier.Setup(s =>
                s.GetParameterNode(Moq.It.IsAny<ICalcContext>(), Moq.It.IsAny<RevisionInfo>(), "par1"))
                .Returns(parameter);

            var calcContext = new Moq.Mock<ICalcContext>();
            var callContext = new Moq.Mock<ICallContext>();
            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            callContext.Setup(s =>
                s.GetStartTime(Moq.It.IsAny<int>()))
                .Returns(new DateTime(2012, 01, 01));

            callContext.Setup(s =>
                s.GetInterval(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns(Interval.Month);

            valueKeeper.Setup(k =>
                k.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 01), Interval.Month, new CalcNodeKey(calcNode, null)))
                .Returns((SymbolValue)null);

            List<Message> messages = null;
            bool serverNA = false;
            calcSupplier.Setup(s =>
                s.GetParameterNodeValue(
                    Moq.It.IsAny<ICalcContext>(),
                    //calcNode, 
                     Moq.It.Is<IEnumerable<Tuple<ICalcNode, ArgumentsValues>>>(c =>
                        c.Count() == 1
                        && c.First().Item1.Equals(calcNode)
                        && c.First().Item2 == null),
                    CalcAggregation.Sum, 
                    //null, 
                    new DateTime(2012, 01, 01), 
                    Interval.Month,
                    out messages, 
                    out serverNA))
                .Returns((ParamValueItem)null);

            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);
            //calcContext.Setup(c => c.Call(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), Moq.It.IsAny<CalcNodeKey[]>())).Returns(true);
            calcContext.Setup(c => c.Call(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), Moq.It.IsAny<CalcNodeKey[]>()))
                .Returns(true)
                .Callback((DateTime st, DateTime et, CalcNodeKey[] nodes) =>
                    symbolTabl.PushSymbolScope(new CommonContext(null, null, st, Interval.Zero, null)));

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@tau", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@tautill", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@ret", SymbolValue.Nothing));

            ParameterFunction function = new ParameterFunction(calcSupplier.Object, "sum", CalcAggregation.Sum, "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c => c.AddMessage(Moq.It.Is<CalcMessage>(m =>
                m.Category == MessageCategory.Error && m.Text == "Агрегация sum($par1$) не может быть расчитана за '01.01.2012 0:00:00'")));

            callContext.Verify(c => c.JumpShift(Moq.It.IsAny<ICalcContext>(), -1), Moq.Times.Never());
            calcContext.Verify(c => c.Call(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01), new CalcNodeKey(calcNode, null)), Moq.Times.Never());
        }

        [Test]
        public void Subroutine_CalculableParameterHasNothingValue_ReportError()
        {
            ICalcNode calcNode = new TestCalcNode() { NodeID = 1 };

            IParameterInfo parameter = new TestParameterinfo(calcNode)
            {
                Calculable = true,
                Code = "par1",
                Interval = Interval.Day
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            // настраиваем запрос ревизий
            calcSupplier.Setup(s =>
                s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>()))
                .Returns(new RevisionInfo[] { RevisionInfo.Default });
            calcSupplier.Setup(s =>
                s.GetRevision(Moq.It.IsAny<DateTime>()))
                .Returns(RevisionInfo.Default);

            // настраиваем запрос параметра по ревизии
            calcSupplier.Setup(s =>
                s.GetParameterNode(Moq.It.IsAny<ICalcContext>(), Moq.It.IsAny<RevisionInfo>(), "par1"))
                .Returns(parameter);

            var calcContext = new Moq.Mock<ICalcContext>();
            var callContext = new Moq.Mock<ICallContext>();
            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            callContext.Setup(s =>
                s.GetStartTime(Moq.It.IsAny<int>()))
                .Returns(new DateTime(2012, 01, 01));

            callContext.Setup(s =>
                s.GetInterval(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns(Interval.Month);

            valueKeeper.Setup(k =>
                k.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 01), Interval.Month, new CalcNodeKey(calcNode, null)))
                .Returns(SymbolValue.Nothing);

            List<Message> messages = null;
            bool serverNA = false;
            calcSupplier.Setup(s =>
                s.GetParameterNodeValue(
                    Moq.It.IsAny<ICalcContext>(),
                    //calcNode, 
                     Moq.It.Is<IEnumerable<Tuple<ICalcNode, ArgumentsValues>>>(c =>
                        c.Count() == 1
                        && c.First().Item1.Equals(calcNode)
                        && c.First().Item2 == null),
                    CalcAggregation.Sum, 
                    //null, 
                    new DateTime(2012, 01, 01), 
                    Interval.Month,
                    out messages, 
                    out serverNA))
                .Returns((ParamValueItem)null);

            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);
            //calcContext.Setup(c => c.Call(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), Moq.It.IsAny<CalcNodeKey[]>())).Returns(true);
            calcContext.Setup(c => c.Call(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), Moq.It.IsAny<CalcNodeKey[]>())).Returns(true)
                .Callback((DateTime st, DateTime et, CalcNodeKey[] nodes) =>
                    symbolTabl.PushSymbolScope(new CommonContext(null, null, st, Interval.Zero, null)));

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@tau", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@tautill", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@ret", SymbolValue.Nothing));

            ParameterFunction function = new ParameterFunction(calcSupplier.Object, "sum", CalcAggregation.Sum, "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c => c.AddMessage(Moq.It.Is<CalcMessage>(m =>
                m.Category == MessageCategory.Error && m.Text == "Агрегация sum($par1$) не может быть расчитана за '01.01.2012 0:00:00'")));

            callContext.Verify(c => c.JumpShift(Moq.It.IsAny<ICalcContext>(), -1), Moq.Times.Never());
            calcContext.Verify(c => c.Call(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01), new CalcNodeKey(calcNode, null)), Moq.Times.Never());
        }

        [Test]
        public void Subroutine__CallParameterCalc()
        {
            ICalcNode calcNode = new TestCalcNode() { NodeID = 1 };

            IParameterInfo parameter = new TestParameterinfo(calcNode)
            {
                Calculable = true,
                Code = "par1",
                Interval = Interval.Day
            };

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
                s.GetStartTime(Moq.It.IsAny<int>()))
                .Returns(new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc));

            callContext.Setup(s =>
                s.GetInterval(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns(Interval.Month);

            valueKeeper.Setup(k =>
                k.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Month, new CalcNodeKey(calcNode, null)))
                .Returns((SymbolValue)null);

            List<Message> messages = null;
            bool serverNA = false;
            calcSupplier.Setup(s =>
                s.GetParameterNodeValue(
                    Moq.It.IsAny<ICalcContext>(),
                    //calcNode, 
                     Moq.It.Is<IEnumerable<Tuple<ICalcNode, ArgumentsValues>>>(c =>
                        c.Count() == 1
                        && c.First().Item1.Equals(calcNode)
                        && c.First().Item2 == null),
                    CalcAggregation.Sum, 
                    //null, 
                    new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), 
                    Interval.Month,
                    out messages, 
                    out serverNA))
                .Returns((ParamValueItem)null);

            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);
            //calcContext.Setup(c => c.Call(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), Moq.It.IsAny<CalcNodeKey[]>())).Returns(true);
            calcContext.Setup(c => c.Call(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), Moq.It.IsAny<CalcNodeKey[]>())).Returns(true)
                .Callback((DateTime st, DateTime et, CalcNodeKey[] nodes) =>
                    symbolTabl.PushSymbolScope(new CommonContext(null, null, st, Interval.Zero, null)));

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@tau", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@tautill", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@ret", SymbolValue.Nothing));

            ParameterFunction function = new ParameterFunction(calcSupplier.Object, "sum", CalcAggregation.Sum, "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
            callContext.Verify(c => c.JumpShift(Moq.It.IsAny<ICalcContext>(), -1));
            calcContext.Verify(c => c.Call(new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc), new CalcNodeKey(calcNode, null)));
        }

        [Test]
        public void Subroutime_WeightedAggregation_CallBothParametersIfNeeds()
        {
            ICalcNode calcNode1 = new TestCalcNode() { NodeID = 1 };
            ICalcNode calcNode2 = new TestCalcNode() { NodeID = 2 };

            IParameterInfo parameter1 = new TestParameterinfo(calcNode1)
            {
                Calculable = true,
                Code = "par1",
                Interval = Interval.Day
            };
            IParameterInfo parameter2 = new TestParameterinfo(calcNode2)
            {
                Calculable = true,
                Code = "par2",
                Interval = Interval.Day
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            // настраиваем запрос ревизий
            calcSupplier.Setup(s =>
                s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>()))
                .Returns(new RevisionInfo[] { RevisionInfo.Default });

            // настраиваем запрос параметра по ревизии
            calcSupplier.Setup(s =>
                s.GetParameterNode(Moq.It.IsAny<ICalcContext>(), Moq.It.IsAny<RevisionInfo>(), "par1"))
                .Returns(parameter1);
            calcSupplier.Setup(s =>
                s.GetParameterNode(Moq.It.IsAny<ICalcContext>(), Moq.It.IsAny<RevisionInfo>(), "par2"))
                .Returns(parameter2);

            var calcContext = new Moq.Mock<ICalcContext>();
            var callContext = new Moq.Mock<ICallContext>();
            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            callContext.Setup(s =>
                s.GetStartTime(Moq.It.IsAny<int>()))
                .Returns(new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc));

            callContext.Setup(s =>
                s.GetInterval(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns(Interval.Month);

            valueKeeper.Setup(k =>
                k.GetValue(CalcAggregation.Weighted, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Month, new CalcNodeKey(calcNode1, null), new CalcNodeKey(calcNode2, null)))
                .Returns((SymbolValue)null);

            List<Message> messages = null;
            bool serverNA = false;
            calcSupplier.Setup(s =>
                s.GetParameterNodeValue(
                    Moq.It.IsAny<ICalcContext>(),
                    //calcNode1, 
                     Moq.It.Is<IEnumerable<Tuple<ICalcNode, ArgumentsValues>>>(c =>
                        c.Count() == 1
                        && c.First().Item1.Equals(calcNode1)
                        && c.First().Item2 == null),
                    CalcAggregation.Sum, 
                    //null, 
                    new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), 
                    Interval.Month,
                    out messages, 
                    out serverNA))
                .Returns((ParamValueItem)null);

            var symbolTabl = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.SymbolTable).Returns(symbolTabl);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);
            //calcContext.Setup(c => c.Call(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), Moq.It.IsAny<CalcNodeKey[]>())).Returns(true);
            calcContext.Setup(c => c.Call(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), Moq.It.IsAny<CalcNodeKey[]>())).Returns(true)
                .Callback((DateTime st, DateTime et, CalcNodeKey[] nodes) =>
                    symbolTabl.PushSymbolScope(new CommonContext(null, null, st, Interval.Zero, null)));

            symbolTabl.PushSymbolScope();
            symbolTabl.PushSymbolScope(callContext.Object);
            symbolTabl.DeclareSymbol(new Variable("@node1", SymbolValue.CreateValue("par1")));
            symbolTabl.DeclareSymbol(new Variable("@node2", SymbolValue.CreateValue("par2")));
            symbolTabl.DeclareSymbol(new Variable("@tau", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@tautill", SymbolValue.CreateValue(0)));
            symbolTabl.DeclareSymbol(new Variable("@ret", SymbolValue.Nothing));

            ParameterFunction function = new ParameterFunction(calcSupplier.Object, "weight", CalcAggregation.Weighted, "", "");

            function.Subroutine(calcContext.Object);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
            callContext.Verify(c => c.JumpShift(Moq.It.IsAny<ICalcContext>(), -1));
            calcContext.Verify(c => c.Call(new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc),
                                           new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc),
                                           new CalcNodeKey(calcNode1, null),
                                           new CalcNodeKey(calcNode2, null)));
        }
    }
}

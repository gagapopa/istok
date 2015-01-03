using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    [TestFixture]
    public class NodeContextTests
    {
        [Test]
        public void Prepare_ValueKeeperHasValue_SkipInterval()
        {
            ICalcNode calcNode = new TestCalcNode() { Name = "mamburu", NodeID = 32 };
            IParameterInfo parameterInfo = new TestParameterinfo(calcNode)
            {
                Calculable = true,
                Code = "par1",
                Interval = Interval.Day,
                Formula = "32*53;"
            };

            NodeState state = new NodeState(parameterInfo, parameterInfo.Revision);
            state.Body = new Instruction[]
            {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.LeaveLevel)
            };

            var valueKeeper = new Moq.Mock<IValuesKeeper>();
            valueKeeper.Setup(k =>
                k.GetRawValue(calcNode, null, new DateTime(2012, 01, 02)))
                .Returns(SymbolValue.CreateValue(53));

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);
            calcContext.Setup(c => c.StartTime).Returns(new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc));
            calcContext.Setup(c => c.EndTime).Returns(new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("@ret"));

            NodeContext context = new NodeContext(
                calcSupplier.Object, 
                state,
                new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            context.Prepare(calcContext.Object);
            Assert.AreEqual(new DateTime(2012, 01, 01), context.StartTime);
            Assert.AreEqual(new DateTime(2012, 01, 02), context.EndTime);

            context.Return(calcContext.Object);
            context.Prepare(calcContext.Object);

            Assert.AreEqual(new DateTime(2012, 01, 03), context.StartTime);
            Assert.AreEqual(new DateTime(2012, 01, 04), context.EndTime);
        }

        [Test]
        public void Prepare_CalcSupplierHasValueNotRecalcAll_SkipInterval()
        {
            ICalcNode calcNode = new TestCalcNode() { Name = "mamburu", NodeID = 32 };
            IParameterInfo parameterInfo = new TestParameterinfo(calcNode)
            {
                Calculable = true,
                Code = "par1",
                Interval = Interval.Day,
                Formula = "32*53;"
            };

            NodeState state = new NodeState(parameterInfo, parameterInfo.Revision);
            state.Body = new Instruction[]
            {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.LeaveLevel)
            };

            List<Message> messages;
            bool sna;
            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(s =>
                s.GetParameterNodeRawValues(
                    Moq.It.IsAny<ICalcContext>(),
                    calcNode,
                    Moq.It.IsAny<ArgumentsValues>(),
                    new DateTime(2012, 01, 01),
                    new DateTime(2012, 02, 01),
                    out messages, out sna))
                .Returns(new List<ParamValueItem>(new ParamValueItem[] { 
                    new ParamValueItem(new DateTime(2012,01,01), Quality.Good, 42),
                    new CorrectedParamValueItem(new ParamValueItem(new DateTime(2012,01,02), Quality.Good, 34), 43)
                }));

            var valueKeeper = new ValuesKeeper(calcSupplier.Object);

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper);
            calcContext.Setup(c => c.RecalcAll).Returns(false);
            calcContext.Setup(c => c.StartTime).Returns(new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc));
            calcContext.Setup(c => c.EndTime).Returns(new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("@ret"));

            NodeContext context = new NodeContext(
                calcSupplier.Object, 
                state,
                new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            context.Prepare(calcContext.Object);

            Assert.AreEqual(new DateTime(2012, 01, 03), context.StartTime);
            Assert.AreEqual(new DateTime(2012, 01, 04), context.EndTime);
        }

        [Test]
        public void Prepare_CalcSupplierHasValueForceCalc_SkipIntervalIfValueCorrected()
        {
            ICalcNode calcNode = new TestCalcNode() { Name = "mamburu", NodeID = 32 };
            IParameterInfo parameterInfo = new TestParameterinfo(calcNode)
            {
                Calculable = true,
                Code = "par1",
                Interval = Interval.Day,
                Formula = "32*53;"
            };

            NodeState state = new NodeState(parameterInfo, parameterInfo.Revision);
            state.Body = new Instruction[]
            {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.LeaveLevel)
            };

            List<Message> messages;
            bool sna;
            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(s =>
                s.GetParameterNodeRawValues(
                    Moq.It.IsAny<ICalcContext>(),
                    calcNode,
                    Moq.It.IsAny<ArgumentsValues>(),
                    new DateTime(2012, 01, 01),
                    new DateTime(2012, 02, 01),
                    out messages, out sna))
                .Returns(new List<ParamValueItem>(new ParamValueItem[] { 
                    new ParamValueItem(new DateTime(2012,01,01), Quality.Good, 42),
                    new CorrectedParamValueItem(new ParamValueItem(new DateTime(2012,01,02), Quality.Good, 34), 43)
                }));

            var valueKeeper = new ValuesKeeper(calcSupplier.Object);

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper);
            //calcContext.Setup(c => c.RecalcAll).Returns(true);
            calcContext.Setup(c => c.ForceCalc(parameterInfo)).Returns(true);
            calcContext.Setup(c => c.StartTime).Returns(new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc));
            calcContext.Setup(c => c.EndTime).Returns(new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("@ret"));

            NodeContext context = new NodeContext(
                calcSupplier.Object, 
                state,
                new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            context.Prepare(calcContext.Object);
            Assert.AreEqual(new DateTime(2012, 01, 01), context.StartTime);
            Assert.AreEqual(new DateTime(2012, 01, 02), context.EndTime);

            context.Return(calcContext.Object);
            context.Prepare(calcContext.Object);

            Assert.AreEqual(new DateTime(2012, 01, 03), context.StartTime);
            Assert.AreEqual(new DateTime(2012, 01, 04), context.EndTime);
        }

        [Test]
        public void Prepare_CalcEndTimeReached_CallCalcContextReturn()
        {
            ICalcNode calcNode = new TestCalcNode() { Name = "mamburu", NodeID = 32 };
            IParameterInfo parameterInfo = new TestParameterinfo(calcNode)
            {
                Calculable = true,
                Code = "par1",
                Interval = Interval.Day,
                Formula = "32*53;"
            };

            NodeState state = new NodeState(parameterInfo, parameterInfo.Revision);
            state.Body = new Instruction[]
            {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.LeaveLevel)
            };

            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);
            calcContext.Setup(c => c.StartTime).Returns(new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc));
            calcContext.Setup(c => c.EndTime).Returns(new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("@ret"));

            NodeContext context = new NodeContext(
                calcSupplier.Object,
                state,
                new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            context.Prepare(calcContext.Object);
            Assert.AreEqual(new DateTime(2012, 01, 01), context.StartTime);
            Assert.AreEqual(new DateTime(2012, 01, 02), context.EndTime);

            while (context.StartTime < new DateTime(2012, 02, 01))
            {
                context.Return(calcContext.Object);
                context.Prepare(calcContext.Object);
            }

            calcContext.Verify(c => c.Return(), Moq.Times.Once());
        }

        [Test]
        public void Prepare_ImplicitArgsNValueKeeperHasOptimal_UseOptimal()
        {
            // расчитываемые параметры
            ICalcNode optimizationCalcNode = new TestCalcNode() { Name = "optim", NodeID = 32 };
            ICalcNode parameterCalcNode = new TestCalcNode() { Name = "parameter", NodeID = 3 };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimizationCalcNode)
            {
                Calculable = true,
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument(){ Name = "a" },
                    new TestOptimizationArgument(){ Name = "b" }
                }
            };
            IParameterInfo parameterInfo = new TestParameterinfo(parameterCalcNode)
            {
                Calculable = true,
                Code = "par1",
                Interval = Interval.Day,
                Optimization = optimizationInfo,
                Formula = "100 - 35;"
            };
            NodeState state = new NodeState(parameterInfo, parameterInfo.Revision);
            state.Body = new Instruction[]
            {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.LeaveLevel)
            };

            // настройка вспомогательных объектов
            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            var valueKeeper = new Moq.Mock<IValuesKeeper>();
            valueKeeper.Setup(k => k.GetOptimal(optimizationCalcNode, null, new DateTime(2012, 01, 01)))
                .Returns(new ArgumentsValues(new { a = 4, b = 8 }));
            valueKeeper.Setup(k => k.GetOptimal(optimizationCalcNode, null, new DateTime(2012, 01, 02)))
                .Returns(new ArgumentsValues(new { a = 6, b = 7 }));

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);
            calcContext.Setup(c => c.StartTime).Returns(new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc));
            calcContext.Setup(c => c.EndTime).Returns(new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("@ret"));

            // тестируемый объект
            NodeContext context = new NodeContext(
                calcSupplier.Object,
                state,
                new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            context.Prepare(calcContext.Object);

            // время не должно измениться, т.к. вызывается зависимость
            Assert.AreEqual(DateTime.MinValue, context.StartTime);
            Assert.AreEqual(DateTime.MinValue, context.EndTime);
            context.Return(calcContext.Object);

            // как бэ параметр посчитался и кэше появилось значение
            valueKeeper.Setup(k => k.GetRawValue(parameterCalcNode, new ArgumentsValues(new { a = 4, b = 8 }), new DateTime(2012, 01, 01)))
                .Returns(SymbolValue.CreateValue(42));

            context.Prepare(calcContext.Object);

            // время не должно измениться, т.к. вызывается зависимость
            Assert.AreEqual(new DateTime(2012, 01, 01), context.StartTime);
            Assert.AreEqual(new DateTime(2012, 01, 02), context.EndTime);

            // проверяем запрос на расчёт параметра с оптимальными аргументами
            calcContext.Verify(c =>
                c.Call(
                new DateTime(2012, 01, 01),
                new DateTime(2012, 01, 02),
                new CalcNodeKey(parameterCalcNode, new ArgumentsValues(new { a = 4, b = 8 }))));
            calcContext.Verify(c =>
                c.Call(
                new DateTime(2012, 01, 02),
                new DateTime(2012, 01, 03),
                new CalcNodeKey(parameterCalcNode, new ArgumentsValues(new { a = 6, b = 7 }))));
        }

        [Test]
        public void Prepare_ImplicitArgs_CallOptimization()
        {
            // расчитываемые параметры
            ICalcNode optimizationCalcNode = new TestCalcNode() { Name = "optim", NodeID = 32 };
            ICalcNode parameterCalcNode = new TestCalcNode() { Name = "parameter", NodeID = 3 };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimizationCalcNode)
            {
                Calculable = true,
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument(){ Name = "a" },
                    new TestOptimizationArgument(){ Name = "b" }
                }
            };
            IParameterInfo parameterInfo = new TestParameterinfo(parameterCalcNode)
            {
                Calculable = true,
                Code = "par1",
                Interval = Interval.Day,
                Optimization = optimizationInfo,
                Formula = "100 - 35;"
            };
            NodeState state = new NodeState(parameterInfo, parameterInfo.Revision);
            state.Body = new Instruction[]
            {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.LeaveLevel)
            };

            // настройка вспомогательных объектов
            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);
            calcContext.Setup(c => c.StartTime).Returns(new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc));
            calcContext.Setup(c => c.EndTime).Returns(new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("@ret"));

            // тестируемый объект
            NodeContext context = new NodeContext(
                calcSupplier.Object, 
                state, 
                new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc), 
                new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            context.Prepare(calcContext.Object);

            // время не должно измениться, т.к. вызывается зависимость
            Assert.AreEqual(DateTime.MinValue, context.StartTime);
            Assert.AreEqual(DateTime.MinValue, context.EndTime);

            // проверяем запрос на расчёт параметра с оптимальными аргументами
            calcContext.Verify(c =>
                c.Call(
                new DateTime(2012, 01, 01),
                new DateTime(2012, 02, 01),
                new CalcNodeKey(optimizationCalcNode, null)));
        }

        [Test]
        public void Return__AddValueToValueKeeper()
        {
            ICalcNode calcNode = new TestCalcNode() { Name = "mamburu", NodeID = 32 };
            IParameterInfo parameterInfo = new TestParameterinfo(calcNode)
            {
                Calculable = true,
                Code = "par1",
                Interval = Interval.Day,
                Formula = "32*53;"
            };

            NodeState state = new NodeState(parameterInfo, parameterInfo.Revision);
            state.Body = new Instruction[]
            {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.LeaveLevel)
            };

            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            Variable retVariable = new Variable("@ret");
            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(retVariable);

            NodeContext context = new NodeContext(
                calcSupplier.Object,
                state,
                new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            context.Prepare(calcContext.Object);
            Assert.AreEqual(new DateTime(2012, 01, 01), context.StartTime);
            Assert.AreEqual(new DateTime(2012, 01, 02), context.EndTime);

            retVariable.Value = SymbolValue.CreateValue(42);
            context.Return(calcContext.Object);

            valueKeeper.Verify(k =>
                k.AddCalculatedValue(
                calcNode,
                null,
                new DateTime(2012, 01, 01),
                Moq.It.Is<DoubleValue>(v => v.GetValue().Equals(42.0))));
        }

        [Test]
        public void Prepare_OptimizationFaild_AddBadWithBaseArguments()
        {
            ICalcNode optimization1 = new TestCalcNode()
            {
                Name = "opt1",
                NodeID = 34
            };
            ICalcNode optimization2 = new TestCalcNode()
            {
                Name = "opt2",
                NodeID = 35
            };

            IOptimizationInfo optimization1Info = new TestOptimizationInfo(optimization1)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "a"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "b"
                    }
                }
            };
            IOptimizationInfo optimization2Info = new TestOptimizationInfo(optimization2)
            {
                Interval = Interval.Day,
                Optimization = optimization1Info,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "c"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "d"
                    }
                }
            };

            ICalcNode calcNode = new TestCalcNode() { Name = "mamburu", NodeID = 32 };
            IParameterInfo parameterInfo = new TestParameterinfo(calcNode)
            {
                Calculable = true,
                Code = "par1",
                Optimization = optimization2Info,
                Interval = Interval.Day,
                Formula = "32*53;"
            };

            NodeState state = new NodeState(parameterInfo, parameterInfo.Revision);
            state.Body = new Instruction[]
            {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.LeaveLevel)
            };

            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);
            calcContext.Setup(c => c.StartTime).Returns(new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc));
            calcContext.Setup(c => c.EndTime).Returns(new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            // оптимизация optimization2 закончилась не удачая для аргументов a=10, b=12
            valueKeeper.Setup(
                k => k.GetOptimal(
                    optimization2,
                    new ArgumentsValues(new { a = 10, b = 12 }),
                    new DateTime(2012, 01, 01)))
                .Returns(ArgumentsValues.BadArguments);

            // подготавливаем стэк для вызова параметра с аргументами a=10, b=12
            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("a", SymbolValue.CreateValue(10)));
            symbolTable.DeclareSymbol(new Variable("b", SymbolValue.CreateValue(12)));

            NodeContext context = new NodeContext(
                calcSupplier.Object, 
                state,
                new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2012, 01, 02, 0, 0, 0, DateTimeKind.Utc));

            context.Prepare(calcContext.Object);

            valueKeeper.Verify(
                k => k.AddCalculatedValue(
                    calcNode,
                    new ArgumentsValues(new { a = 10, b = 12 }),
                    new DateTime(2012, 01, 01),
                    SymbolValue.Nothing));
        }

        [Test]
        public void CurrentNode_ReturnNodeInfo()
        {
            ICalcNode calcNode = new TestCalcNode() { NodeID = 1 };

            IParameterInfo parameterInfo = new TestParameterinfo(calcNode) { Calculable = true, Interval = Interval.Day };

            NodeState state = new NodeState(parameterInfo, parameterInfo.Revision);

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            var context = new NodeContext(calcSupplier.Object, state, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            Assert.IsNotNull(context.CurrentNode);

            Assert.AreEqual(1, context.CurrentNode.NodeID);
            Assert.AreEqual(CalcPosition.NodePart.Formula, context.CurrentNode.CurrentPart);
            Assert.AreEqual(CalcPosition.IntimeIdentification.Runtime, context.CurrentNode.Intime);
            Assert.IsNullOrEmpty(context.CurrentNode.AdditionNote);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Проверяемое поведение:
        /// <br />
        /// Метод должен возвращать true если совпадают сам параметр
        /// и переданные аргументы
        /// </remarks>
        [Test]
        public void TheSame__ReturnTrueIfNodeAndArgumentsAreSame()
        {
            ICalcNode optimization = new TestCalcNode() { NodeID = 34 };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimization)
            {
                Interval = Interval.Day,
                Calculable = true,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "a"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "b"
                    }
                }
            };

            ICalcNode calcNode1 = new TestCalcNode() { NodeID = 1 };
            ICalcNode calcNode2 = new TestCalcNode() { NodeID = 2 };
            ICalcNode calcNode3 = new TestCalcNode() { NodeID = 3 };

            IParameterInfo parameter1Info = new TestParameterinfo(calcNode1)
            {
                Interval = Interval.Day,
                Calculable = true,
                Formula = "a+b;",
                Optimization = optimizationInfo
            };
            IParameterInfo parameter2Info = new TestParameterinfo(calcNode2)
            {
                Interval = Interval.Day,
                Calculable = true,
                Formula = "a*b;",
                Optimization = optimizationInfo
            };
            IParameterInfo parameter3Info = new TestParameterinfo(calcNode3)
            {
                Interval = Interval.Day,
                Calculable = true,
                Formula = "a/b;",
                Optimization = optimizationInfo
            };
            var state1 = new NodeState(parameter1Info, parameter1Info.Revision);
            var state2 = new NodeState(parameter2Info, parameter2Info.Revision);
            var state3 = new NodeState(parameter3Info, parameter3Info.Revision);

            var context1 = new NodeContext(null, state1, new DateTime(2012, 01, 01), new DateTime(2012, 01, 02));
            var context2_1 = new NodeContext(null, state2, new DateTime(2012, 01, 01), new DateTime(2012, 01, 02));
            context2_1.Arguments = new ArgumentsValues(new { a = 10, b = 12 });
            var context2_2 = new NodeContext(null, state2, new DateTime(2012, 01, 01), new DateTime(2012, 01, 02));
            context2_2.Arguments = new ArgumentsValues(new { a = 10, b = 12 });
            var context2_3 = new NodeContext(null, state2, new DateTime(2012, 01, 01), new DateTime(2012, 01, 02));
            context2_3.Arguments = new ArgumentsValues(new { a = 12, b = 12 });
            var context3_1 = new NodeContext(null, state3, new DateTime(2012, 01, 01), new DateTime(2012, 01, 02));
            context3_1.Arguments = new ArgumentsValues(new { a = 10, b = 12 });
            var context3_2 = new NodeContext(null, state3, new DateTime(2012, 01, 02), new DateTime(2012, 01, 03));
            context3_2.Arguments = new ArgumentsValues(new { a = 12, b = 12 });
            var context3_3 = new NodeContext(null, state3, new DateTime(2012, 01, 05), new DateTime(2012, 01, 06));
            context3_3.Arguments = new ArgumentsValues(new { a = 10, b = 12 });

            bool ret;

            ret = context2_1.TheSame(context1);
            Assert.IsFalse(ret);
            ret = context2_1.TheSame(context2_2);
            Assert.IsTrue(ret);
            ret = context2_1.TheSame(context2_3);
            Assert.IsFalse(ret);


            ret = context3_1.TheSame(context1);
            Assert.IsFalse(ret);
            ret = context3_1.TheSame(context3_2);
            Assert.IsFalse(ret);
            ret = context3_1.TheSame(context3_3);
            Assert.IsTrue(ret);
        }

        [Test]
        public void TheSame_DifferentRevision_ReturnFale()
        {
            RevisionInfo revision1 = new RevisionInfo() { ID = 1, Time = new DateTime(2012, 02, 01) };

            ICalcNode optimization = new TestCalcNode() { NodeID = 34 };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimization)
            {
                Interval = Interval.Day,
                Calculable = true,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "a"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "b"
                    }
                }
            };

            ICalcNode calcNode1 = new TestCalcNode() { NodeID = 1 };

            IParameterInfo parameter1Info = new TestParameterinfo(calcNode1)
            {
                Interval = Interval.Day,
                Calculable = true,
                Formula = "a+b;",
                Optimization = optimizationInfo
            };

            // нодстэйты созданные для разных ревизий
            var state1_0 = new NodeState(parameter1Info, RevisionInfo.Default);
            var state1_1 = new NodeState(parameter1Info, revision1);

            // контексты на основании этих нодстэйтов
            var context1_0 = new NodeContext(null, state1_0, new DateTime(2012, 01, 01), new DateTime(2012, 01, 02));
            context1_0.Arguments = new ArgumentsValues(new { a = 10, b = 12 });
            var context1_1 = new NodeContext(null, state1_1, new DateTime(2012, 02, 01), new DateTime(2012, 02, 02));
            context1_1.Arguments = new ArgumentsValues(new { a = 10, b = 12 });

            bool ret;

            // контексты взаимно безопасны
            ret = context1_0.TheSame(context1_1);
            Assert.IsFalse(ret);
            ret = context1_1.TheSame(context1_0);
            Assert.IsFalse(ret);
        }

        [Test]
        public void Prepare_NodeHasNeededs_CallNeededNode()
        {
            ICalcNode calcNode1 = new TestCalcNode() { NodeID = 1 };
            ICalcNode calcNode2 = new TestCalcNode() { NodeID = 2 };
            ICalcNode calcNode3 = new TestCalcNode() { NodeID = 3 };

            ICalcNode optim1 = new TestCalcNode() { NodeID = 34 };

            IOptimizationInfo optimInfo1 = new TestOptimizationInfo(optim1)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "a"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "b"
                    }
                }
            };
            IParameterInfo nodeInfo2 = new TestParameterinfo(calcNode2)
            {
                Interval = Interval.Day,
                Optimization = optimInfo1,
                Calculable = true,
                Code = "par2",
                Formula = "a+b",
            };
            IParameterInfo nodeInfo3 = new TestParameterinfo(calcNode3)
            {
                Interval = Interval.Day,
                Optimization = optimInfo1,
                Calculable = true,
                Code = "par3",
                Formula = "a*b",
            };
            IParameterInfo nodeInfo1 = new TestParameterinfo(calcNode1)
            {
                Interval = Interval.Day,
                Optimization = optimInfo1,
                Calculable = true,
                Code = "par1",
                Formula = "a/b",
                Needed = new String[] { nodeInfo2.Code, nodeInfo3.Code }
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.StartTime).Returns(new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc));
            calcContext.Setup(c => c.EndTime).Returns(new DateTime(2012, 02, 01, 0, 0, 0, DateTimeKind.Utc));

            var symblTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symblTable);

            var valuesKeeper = new Moq.Mock<IValuesKeeper>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valuesKeeper.Object);

            // запрос параметров по коду
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), nodeInfo1.Code)).Returns(nodeInfo1);
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), nodeInfo2.Code)).Returns(nodeInfo2);
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), nodeInfo3.Code)).Returns(nodeInfo3);

            // настройка вызова расчёта праметра
            calcContext.Setup(
                c => c.Call(
                    Moq.It.IsAny<DateTime>(),
                    Moq.It.IsAny<DateTime>(),
                    Moq.It.IsAny<CalcNodeKey[]>()))
                .Returns((DateTime start, DateTime end, CalcNodeKey[] keys) =>
                {
                    IParameterInfo parameterInfo = keys[0].Node.GetParameter(RevisionInfo.Default);
                    symblTable.PushSymbolScope(
                        new NodeContext(
                            calcSupplier.Object,
                            new NodeState(parameterInfo, parameterInfo.Revision), 
                            start,
                            end), 
                        true);
                    return true;
                });

            bool isCalculated_cn2 = false, isCalculated_cn3 = false;

            valuesKeeper.Setup(
                v => v.IsCalculated(
                    calcNode2,
                    new ArgumentsValues(new { a = 10, b = 12 }),
                    new DateTime(2012, 01, 01)))
                .Returns(() => isCalculated_cn2);
            valuesKeeper.Setup(
                v => v.IsCalculated(
                    calcNode3,
                    new ArgumentsValues(new { a = 10, b = 12 }),
                    new DateTime(2012, 01, 01)))
                .Returns(() => isCalculated_cn3);

            NodeState state1 = new NodeState(nodeInfo1, nodeInfo1.Revision);
            state1.Body = new Instruction[]
            {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.LeaveLevel)
            };

            var nodeContext = new NodeContext(
                calcSupplier.Object, 
                state1, 
                new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2012, 01, 02, 0, 0, 0, DateTimeKind.Utc));

            // подготавливаем стек
            symblTable.PushSymbolScope();
            symblTable.DeclareSymbol(new Variable("a", SymbolValue.CreateValue(10)));
            symblTable.DeclareSymbol(new Variable("b", SymbolValue.CreateValue(12)));
            symblTable.PushSymbolScope(nodeContext);

            // вызывает расчёт параметра nodeInfo2
            nodeContext.Prepare(calcContext.Object);

            var context = symblTable.CallContext;
            Assert.IsNotNull(context);
            Assert.AreNotSame(nodeContext, context);
            Assert.IsInstanceOf<NodeContext>(context);
            Assert.AreSame(nodeInfo2, ((NodeContext)context).Node.NodeInfo);

            // отмечаем параметр nodeInfo2 как посчитанный
            isCalculated_cn2 = true;
            // удаляем вызванный контекст
            symblTable.PopSymbolScope();
            // текущим контекстом должен быть nodeContext
            Assert.AreSame(nodeContext, symblTable.CallContext);

            // вызывает расчёт параметра nodeInfo3
            nodeContext.Prepare(calcContext.Object);

            context = symblTable.CallContext;
            Assert.IsNotNull(context);
            Assert.AreNotSame(nodeContext, context);
            Assert.IsInstanceOf<NodeContext>(context);
            Assert.AreSame(nodeInfo3, ((NodeContext)context).Node.NodeInfo);

            // отмечаем параметр nodeInfo3 как посчитанный
            isCalculated_cn3 = true;
            // удаляем вызванный контекст
            symblTable.PopSymbolScope();
            // текущим контекстом должен быть nodeContext
            Assert.AreSame(nodeContext, symblTable.CallContext);

            // преступаем к расчёту самого параметра
            nodeContext.Prepare(calcContext.Object);

            context = symblTable.CallContext;
            Assert.IsNotNull(context);
            Assert.AreSame(nodeContext, context);

            Assert.AreEqual(new DateTime(2012, 01, 01), context.StartTime);
        }
    }
}

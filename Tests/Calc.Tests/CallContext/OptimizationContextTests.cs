using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    [TestFixture]
    public class OptimizationContextTests
    {
        TestCalcNodeRepository repository;

        [SetUp]
        public void PrepareRepository()
        {
            // опишем оптимизации
            ICalcNode optimization1 = new TestCalcNode()
            {
                NodeID = 33,
                Name = "opt_abc_interval"
            };
            ICalcNode optimization2 = new TestCalcNode()
            {
                NodeID = 37,
                Name = "opt_de_interval"
            };
            ICalcNode optimization3 = new TestCalcNode() {
                NodeID = 34,
                Name = "opt_hij_expression"
            };
            ICalcNode optimization4 = new TestCalcNode() {
                NodeID = 35,
                Name = "opt_kl_child_hij"
            };
            ICalcNode optimization5 = new TestCalcNode() {
                NodeID = 36,
                Name = "opt_mn_child_kl"
            };
            ICalcNode optimization6 = new TestCalcNode()
            {
                NodeID = 37,
                Name = "opt_opq_manual_interval"
            };
            ICalcNode optimization7 = new TestCalcNode()
            {
                NodeID = 37,
                Name = "opt_rs_manual"
            };

            IOptimizationInfo optimization1Info = new TestOptimizationInfo(optimization1)
            {
                Calculable = true,
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[] 
                {
                    new TestOptimizationArgument()
                    {
                        Name = "a",
                        Mode = OptimizationArgumentMode.Interval,
                        IntervalFrom = 10,
                        IntervalTo = 30,
                        IntervalStep = 2
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "b",
                        Mode = OptimizationArgumentMode.Interval,
                        IntervalFrom = 10,
                        IntervalTo = 30,
                        IntervalStep = 2
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "c",
                        Mode = OptimizationArgumentMode.Interval,
                        IntervalFrom = 10,
                        IntervalTo = 30,
                        IntervalStep = 2
                    }
                },
                DefinationDomain = "a * b - c * c > 0;",
                Expression = "c - a * b;",
            };

            IOptimizationInfo optimization2Info = new TestOptimizationInfo(optimization2)
            {
                Calculable = true,
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[] 
                {
                    new TestOptimizationArgument() 
                    { 
                        Name = "d",
                        Mode = OptimizationArgumentMode.Interval,
                        IntervalFrom = 10, 
                        IntervalTo = 30,
                        IntervalStep = 2 
                    },
                    new TestOptimizationArgument() {
                        Name = "e",
                        Mode = OptimizationArgumentMode.Interval,
                        IntervalFrom = 10, 
                        IntervalTo = 30, 
                        IntervalStep = 2 
                    },
                },
                Expression = "d/e;",
            };

            IOptimizationInfo optimization3Info = new TestOptimizationInfo(optimization3)
            {
                Interval = Interval.Day,
                Calculable = true,
                Arguments = new IOptimizationArgument[] 
                { 
                    new TestOptimizationArgument() 
                    { 
                        Name = "h", 
                        Mode = OptimizationArgumentMode.Expression, 
                        Expression = "{10, 13, 16, 17, 20};"
                    },
                    new TestOptimizationArgument() 
                    { 
                        Name = "i", 
                        Mode = OptimizationArgumentMode.Interval, 
                        IntervalFrom = 10, 
                        IntervalTo = 30, 
                        IntervalStep = 2 
                    },
                    new TestOptimizationArgument() 
                    { 
                        Name = "j", 
                        Mode = OptimizationArgumentMode.Expression, 
                        Expression = "{15, 20, 33, 37, 40};" 
                    }
                },
                Expression = "h*i*j;"
            };
            IOptimizationInfo optimization4Info = new TestOptimizationInfo(optimization4)
            {
                Interval = Interval.Day,
                Optimization = optimization3Info,
                Arguments = new IOptimizationArgument[] 
                {
                    new TestOptimizationArgument() { Name = "k" },
                    new TestOptimizationArgument() { Name = "l" },
                }
            };
            IOptimizationInfo optimization5Info = new TestOptimizationInfo(optimization5)
            {
                Interval = Interval.Day,
                Optimization = optimization4Info,
                Arguments = new IOptimizationArgument[] 
                {
                    new TestOptimizationArgument() { Name = "m" },
                    new TestOptimizationArgument() { Name = "n" },
                }
            };
            IOptimizationInfo optimization6Info = new TestOptimizationInfo(optimization6)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument() 
                    { 
                        Name = "o",
                        Mode = OptimizationArgumentMode.Manual
                    },
                    new TestOptimizationArgument() 
                    { 
                        Name = "p",
                        Mode = OptimizationArgumentMode.Interval,
                        IntervalFrom = 10,
                        IntervalTo = 20,
                        IntervalStep = 3
                    },
                    new TestOptimizationArgument() 
                    { 
                        Name = "q",
                        Mode = OptimizationArgumentMode.Manual
                    },
                }
            };
            IOptimizationInfo optimization7Info = new TestOptimizationInfo(optimization7)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument() 
                    { 
                        Name = "r",
                        Mode = OptimizationArgumentMode.Manual
                    },
                    new TestOptimizationArgument() 
                    { 
                        Name = "s",
                        Mode = OptimizationArgumentMode.Manual
                    },
                },
                Expression = "r*s;"
            };

            // опишем параметры
            ICalcNode parameter1 = new TestCalcNode() { NodeID = 1, Name = "par1_child_abc" };
            ICalcNode parameter2 = new TestCalcNode() { NodeID = 2, Name = "par2_child_abc" };
            ICalcNode parameter3 = new TestCalcNode() { NodeID = 3, Name = "par3_child_abc" };
            ICalcNode parameter4 = new TestCalcNode() { NodeID = 4, Name = "par4_child_abc" };
            ICalcNode parameter5 = new TestCalcNode() { NodeID = 5, Name = "par5_child_abc" };

            IParameterInfo parameter1Info = new TestParameterinfo(parameter1)
            {
                Optimization = optimization1Info,
                Interval = Interval.Day,
                Code = "par1",
                Calculable = true,
                Formula = "a + b;"
            };
            IParameterInfo parameter2Info = new TestParameterinfo(parameter2)
            {
                Optimization = optimization1Info,
                Interval = Interval.Day,
                Code = "par2",
                Calculable = true,
                Formula = "a + b;"
            };
            IParameterInfo parameter3Info = new TestParameterinfo(parameter3)
            {
                Optimization = optimization1Info,
                Interval = Interval.Day,
                Code = "par3",
                Calculable = true,
                Formula = "a + b;"
            };
            IParameterInfo parameter4Info = new TestParameterinfo(parameter4)
            {
                Optimization = optimization1Info,
                Interval = Interval.Day,
                Code = "par4",
                Calculable = true,
                Formula = "a + b;"
            };
            IParameterInfo parameter5Info = new TestParameterinfo(parameter5)
            {
                Optimization = optimization1Info,
                Interval = Interval.Day,
                Code = "par5",
                Calculable = true,
                Formula = "a + b;"
            };

            repository = new TestCalcNodeRepository();

            repository.Add(optimization1Info);
            repository.Add(optimization2Info);
            repository.Add(optimization3Info);
            repository.Add(optimization4Info);
            repository.Add(optimization5Info);
            repository.Add(optimization6Info);
            repository.Add(optimization7Info);

            repository.Add(parameter1Info);
            repository.Add(parameter2Info);
            repository.Add(parameter3Info);
            repository.Add(parameter4Info);
            repository.Add(parameter5Info);
        }

        [Test]
        public void Prepare_AllArgumetnsImplicit_ReportError()
        {
            IOptimizationInfo optimizationInfo = (from o in repository.Optimizations where o.Name == "opt_abc_interval" select o).First();

            OptimizationState optimizationState = new OptimizationState(null, optimizationInfo, optimizationInfo.Revision);

            var calcContext = new Moq.Mock<ICalcContext>();

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            symbolTable.DeclareSymbol(new Variable("a", SymbolValue.CreateValue(5)));
            symbolTable.DeclareSymbol(new Variable("b", SymbolValue.CreateValue(10)));
            symbolTable.DeclareSymbol(new Variable("c", SymbolValue.CreateValue(15)));

            OptimizationContext context = new OptimizationContext(optimizationState, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            context.Prepare(calcContext.Object);

            calcContext.Verify(c =>
                c.AddMessage(Moq.It.Is<CalcMessage>(m => m.Category == MessageCategory.Error && m.Text == "Ошибка оптимизации. Все аргументы переданы явно")));
        }

        [Test]
        public void GetOptimalBaseArguments__SetNextTimeNSkipCalculated()
        {
            IOptimizationInfo optimizationInfo = (from o in repository.Optimizations where o.Name == "opt_abc_interval" select o).First();

            OptimizationState state = new OptimizationState(null, optimizationInfo, optimizationInfo.Revision);

            var valueKeeper = new Moq.Mock<IValuesKeeper>();
            valueKeeper.Setup(
                k => k.GetOptimal(optimizationInfo.CalcNode, null, new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc)))
                .Returns(new ArgumentsValues(new { a = 1, b = 4, c = 8 }));

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            OptimizationContext context = new OptimizationContext(state, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            // переход к следующему интервалу
            var contextState = context.NextTime(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.GetBaseArguments, contextState);

            contextState = context.GetOptimalBaseArguments(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.PrepareCallArgumentsContext, contextState);

            Assert.AreEqual(new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), context.StartTime);
            Assert.AreEqual(new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc), context.EndTime);

            // пропуск уже расчитанного интервала
            contextState = context.NextTime(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.GetBaseArguments, contextState);

            // собственно пропуск
            contextState = context.GetOptimalBaseArguments(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.NextTime, contextState);

            contextState = context.NextTime(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.GetBaseArguments, contextState);

            contextState = context.GetOptimalBaseArguments(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.PrepareCallArgumentsContext, contextState);

            Assert.AreEqual(new DateTime(2012, 01, 03, 00, 00, 00, DateTimeKind.Utc), context.StartTime);
            Assert.AreEqual(new DateTime(2012, 01, 04, 00, 00, 00, DateTimeKind.Utc), context.EndTime);

            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void NextTime__CalcContextReturn()
        {
            IOptimizationInfo optimizationInfo = (from o in repository.Optimizations where o.Name == "opt_abc_interval" select o).First();

            OptimizationState optmimizationState = new OptimizationState(null, optimizationInfo, optimizationInfo.Revision);

            var calcContext = new Moq.Mock<ICalcContext>();

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            var context = new OptimizationContext(optmimizationState, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            OptimizationContext.ContextState contextState;

            for (int i = 1; i <= 31; i++)
            {
                contextState = context.NextTime(calcContext.Object);

                DateTime startTime = new DateTime(2012, 01, i, 00, 00, 00, DateTimeKind.Utc);
                DateTime endTime = startTime.AddDays(1);

                Assert.AreEqual(OptimizationContext.ContextState.GetBaseArguments, contextState);
                Assert.AreEqual(startTime, context.StartTime);
                Assert.AreEqual(endTime, context.EndTime);
            }
            contextState = context.NextTime(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.ExitOptimization, contextState);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void GetOptimalBaseArguments__CallBaseOptimization()
        {
            // оперерируеммые данные
            IOptimizationInfo optimization1Info = (from o in repository.Optimizations where o.Name == "opt_hij_expression" select o).First();
            IOptimizationInfo optimization2Info = (from o in repository.Optimizations where o.Name == "opt_kl_child_hij" select o).First();
            IOptimizationInfo optimization3Info = (from o in repository.Optimizations where o.Name == "opt_mn_child_kl" select o).First();

            OptimizationState optimization3state = new OptimizationState(null, optimization3Info, optimization3Info.Revision);

            var valuesKeeper = new Moq.Mock<IValuesKeeper>();
            valuesKeeper.Setup(
                v => v.GetOptimal(optimization1Info.CalcNode, null, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc)))
                .Returns(new ArgumentsValues(new { h = 5, i = 10, j = 15 }));

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valuesKeeper.Object);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            // тестируемый класс
            OptimizationContext context = new OptimizationContext(optimization3state, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            var state = context.NextTime(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.GetBaseArguments, state);

            state = context.GetOptimalBaseArguments(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.GetBaseArguments, state);

            calcContext.Verify(c =>
                c.Call(
                new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc),
                new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc),
                new CalcNodeKey(optimization2Info.CalcNode, new ArgumentsValues(new { h = 5, i = 10, j = 15 }))));

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void GetOptimalBaseArguments_ExplicitArgs_CallBaseOptimization()
        {
            // оперерируеммые данные
            IOptimizationInfo optimization1Info = (from o in repository.Optimizations where o.Name == "opt_hij_expression" select o).First();
            IOptimizationInfo optimization2Info = (from o in repository.Optimizations where o.Name == "opt_kl_child_hij" select o).First();
            IOptimizationInfo optimization3Info = (from o in repository.Optimizations where o.Name == "opt_mn_child_kl" select o).First();

            OptimizationState optimization3state = new OptimizationState(null, optimization3Info, optimization3Info.Revision);

            var valuesKeeper = new Moq.Mock<IValuesKeeper>();

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valuesKeeper.Object);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            // явная передача аргументов для верхней оптимизации
            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("h", SymbolValue.CreateValue(5)));
            symbolTable.DeclareSymbol(new Variable("i", SymbolValue.CreateValue(10)));
            symbolTable.DeclareSymbol(new Variable("j", SymbolValue.CreateValue(15)));
            symbolTable.PushSymbolScope();

            // тестируемый класс
            OptimizationContext context = new OptimizationContext(optimization3state, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            var state = context.NextTime(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.GetBaseArguments, state);

            state = context.GetOptimalBaseArguments(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.GetBaseArguments, state);

            // GetOptimal() для верхней оптимизации не должен вызываться
            valuesKeeper.Verify(
                v => v.GetOptimal(
                    optimization1Info.CalcNode,
                    null,
                    new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc)),
                Moq.Times.Never());

            // расчёт второй оптимизации с переданными аргументам
            calcContext.Verify(
                c => c.Call(
                    new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc),
                    new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc),
                    new CalcNodeKey(optimization2Info.CalcNode, new ArgumentsValues(new { h = 5, i = 10, j = 15 }))));

            calcContext.Verify(
                c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), 
                Moq.Times.Never());
        }

        [Test]
        public void PrepareArgumentContexts_CallNextArgument_CallCommonContext()
        {
            // оперерируеммые данные
            IOptimizationInfo optimization1Info = (from o in repository.Optimizations where o.Name == "opt_hij_expression" select o).First();

            OptimizationState state = new OptimizationState(null, optimization1Info, optimization1Info.Revision);

            var calcContext = new Moq.Mock<ICalcContext>();

            SymbolTable symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            var compiler = new Moq.Mock<ICompiler>();
            compiler.Setup(c =>
                c.Compile(
                Moq.It.IsAny<ICalcContext>(),
                Moq.It.IsAny<RevisionInfo>(),
                "{10, 13, 16, 17, 20};",
                null))
                .Returns(new Instruction[] 
                { 
                    new Instruction(null, Instruction.OperationCode.EnterLevel),
                    new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address(new ArrayValue(calcContext.Object,
                        SymbolValue.CreateValue(10), SymbolValue.CreateValue(13), SymbolValue.CreateValue(16), SymbolValue.CreateValue(17), SymbolValue.CreateValue(20)))),
                    new Instruction(null, Instruction.OperationCode.LeaveLevel),
                    new Instruction(null, Instruction.OperationCode.Return)
                });
            compiler.Setup(c =>
                c.Compile(
                Moq.It.IsAny<ICalcContext>(),
                Moq.It.IsAny<RevisionInfo>(),
                "{15, 20, 33, 37, 40};",
                null))
                .Returns(new Instruction[] 
                { 
                    new Instruction(null, Instruction.OperationCode.EnterLevel),
                    new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address(new ArrayValue(calcContext.Object,
                        SymbolValue.CreateValue(15), SymbolValue.CreateValue(20), SymbolValue.CreateValue(33), SymbolValue.CreateValue(37), SymbolValue.CreateValue(40)))),
                    new Instruction(null, Instruction.OperationCode.LeaveLevel),
                    new Instruction(null, Instruction.OperationCode.Return)
                });

            state.Compile(compiler.Object, calcContext.Object);

            var context = new OptimizationContext(state, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            var contextState = context.NextTime(calcContext.Object);

            contextState = context.PrepareArgumentContexts(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.NextArgument, contextState);

            // расчёт возможных значений аргумента 'h'
            contextState = context.NextArgument(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.ReturnArgumentValue, contextState);

            calcContext.Verify(c => c.Call(Moq.It.IsAny<CommonContext>()));

            // расчёт возможных значений аргумента 'j'
            contextState = context.NextArgument(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.ReturnArgumentValue, contextState);

            calcContext.Verify(c => c.Call(Moq.It.IsAny<CommonContext>()));

            // идём дальше
            contextState = context.NextArgument(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.CallDD, contextState);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void PrepareArgumentContexts_ManualArgumentsAndAnother_ReportErrorSetFailFlagExitOptimization()
        {        
            // оперерируеммые данные
            IOptimizationInfo optimization1Info = (from o in repository.Optimizations where o.Name == "opt_opq_manual_interval" select o).First();

            OptimizationState state = new OptimizationState(null, optimization1Info, optimization1Info.Revision);

            var calcContext = new Moq.Mock<ICalcContext>();

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            var compiler = new Moq.Mock<ICompiler>();

            state.Compile(compiler.Object, calcContext.Object);

            var context = new OptimizationContext(state, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            var contextState = context.NextTime(calcContext.Object);

            contextState = context.PrepareArgumentContexts(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.ExitOptimization, contextState);

            Assert.AreEqual(true, state.Failed);

            calcContext.Verify(c => 
                c.AddMessage(Moq.It.Is<CalcMessage>(m =>
                m.Category == MessageCategory.Error && m.Text == "У оптимизации 'opt_opq_manual_interval' часть аргументов вводяться вручную, часть расчитываемая.")));
        }

        [Test]
        public void PrepareArgumentContexts_ManualArguments_()
        {
            // оперерируеммые данные
            IOptimizationInfo optimization1Info = (from o in repository.Optimizations where o.Name == "opt_rs_manual" select o).First();

            OptimizationState state = new OptimizationState(null, optimization1Info, optimization1Info.Revision);

            ArgumentsValues[] argsArray = new ArgumentsValues[] 
                { 
                    new ArgumentsValues(new { r = 10, s = 23 }),
                    new ArgumentsValues(new { r = 15, s = 13 }),
                    new ArgumentsValues(new { r = 63, s = 31 }),
                    new ArgumentsValues(new { r = 34, s = 36 }),
                    new ArgumentsValues(new { r = 21, s = 93 })
                };

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.GetManualArgValues(optimization1Info, null, new DateTime(2012, 01, 01)))
                .Returns(argsArray);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            var compiler = new Moq.Mock<ICompiler>();

            state.Compile(compiler.Object, calcContext.Object);

            var context = new OptimizationContext(state, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            var contextState = context.NextTime(calcContext.Object);

            contextState = context.PrepareArgumentContexts(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.CallDD, contextState);

            Variable rVariable;
            Variable sVariable;

            // переход к расчёту оптимизационного выражения
            contextState = context.CallDD(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.CallExpression, contextState);

            // для каждой пары аргументов
            foreach (var args in argsArray)
            {
                contextState = context.CallExpression(calcContext.Object);

                Assert.AreEqual(OptimizationContext.ContextState.ReturnExpressionValue, contextState);

                rVariable = symbolTable.GetSymbol("r") as Variable;
                sVariable = symbolTable.GetSymbol("s") as Variable;

                Assert.IsNotNull(rVariable);
                Assert.IsNotNull(sVariable);

                Assert.AreEqual((double)args["r"], rVariable.Value.GetValue());
                Assert.AreEqual((double)args["s"], sVariable.Value.GetValue()); 
            }

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void ReturnCallArgument__()
        {             
            // оперерируеммые данные
            IOptimizationInfo optimization1Info = (from o in repository.Optimizations where o.Name == "opt_hij_expression" select o).First();

            OptimizationState state = new OptimizationState(null, optimization1Info, optimization1Info.Revision);

            var valuesKeeper = new Moq.Mock<IValuesKeeper>();

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valuesKeeper.Object);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            var compiler = new Moq.Mock<ICompiler>();
            compiler.Setup(c =>
                c.Compile(
                Moq.It.IsAny<ICalcContext>(),
                Moq.It.IsAny<RevisionInfo>(),
                "{10, 13, 16, 17, 20};",
                null))
                .Returns(new Instruction[] 
                { 
                    new Instruction(null, Instruction.OperationCode.EnterLevel),
                    new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address(new ArrayValue(calcContext.Object,
                        SymbolValue.CreateValue(10), SymbolValue.CreateValue(13), SymbolValue.CreateValue(16), SymbolValue.CreateValue(17), SymbolValue.CreateValue(20)))),
                    new Instruction(null, Instruction.OperationCode.LeaveLevel),
                    new Instruction(null, Instruction.OperationCode.Return)
                });
            compiler.Setup(c =>
                c.Compile(
                Moq.It.IsAny<ICalcContext>(),
                Moq.It.IsAny<RevisionInfo>(),
                "{15, 20, 33, 37, 40};",
                null))
                .Returns(new Instruction[] 
                { 
                    new Instruction(null, Instruction.OperationCode.EnterLevel),
                    new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address(new ArrayValue(calcContext.Object,
                        SymbolValue.CreateValue(15), SymbolValue.CreateValue(20), SymbolValue.CreateValue(33), SymbolValue.CreateValue(37), SymbolValue.CreateValue(40)))),
                    new Instruction(null, Instruction.OperationCode.LeaveLevel),
                    new Instruction(null, Instruction.OperationCode.Return)
                });

            state.Compile(compiler.Object, calcContext.Object);

            var context = new OptimizationContext(state, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            var contextState = context.NextTime(calcContext.Object);

            contextState = context.PrepareArgumentContexts(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.NextArgument, contextState);

            // расчёт возможных значений аргумента 'h'
            contextState = context.NextArgument(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.ReturnArgumentValue, contextState);

            calcContext.Verify(c => c.Call(Moq.It.IsAny<CommonContext>()));

            Variable retVar = symbolTable.GetSymbol("@ret") as Variable;
            Assert.IsNotNull(retVar);
            retVar.Value = new ArrayValue(calcContext.Object,
                SymbolValue.CreateValue(10), SymbolValue.CreateValue(13), SymbolValue.CreateValue(16), SymbolValue.CreateValue(17), SymbolValue.CreateValue(20));

            contextState = context.ReturnArgument(calcContext.Object);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());

            // расчёт возможных значений аргумента 'j'
            contextState = context.NextArgument(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.ReturnArgumentValue, contextState);

            calcContext.Verify(c => c.Call(Moq.It.IsAny<CommonContext>()));

            // возвращаем Nothing
            Variable var = symbolTable.GetSymbol("@ret") as Variable;
            var.Value = SymbolValue.Nothing;

            contextState = context.ReturnArgument(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.NextTime, contextState);

            valuesKeeper.Verify(k => k.SetOptimal(optimization1Info.CalcNode, null, new DateTime(2012, 01, 01), ArgumentsValues.BadArguments));

            // ожидаем сообщение о неудачной оптимизвации
            calcContext.Verify(c => c.AddMessage(Moq.It.Is<CalcMessage>(m =>
                m.Category == MessageCategory.Error && m.Text == "Ошибка оптимизации 'opt_hij_expression' за '2012-01-01'. У аргумента 'j' нет допустимых значенний")));
        }

        [Test]
        public void NextArgumentSet__CallDDAndExpression()
        {
            IOptimizationInfo optimization1Info = (from o in repository.Optimizations where o.Name == "opt_abc_interval" select o).First();
            IOptimizationInfo optimization2Info = (from o in repository.Optimizations where o.Name == "opt_de_interval" select o).First();

            OptimizationState optimization1State = new OptimizationState(null, optimization1Info, optimization1Info.Revision);

            OptimizationState optimization2State = new OptimizationState(null, optimization2Info, optimization2Info.Revision);

            var compiler = new Moq.Mock<ICompiler>();

            var valuesKeeper = new ValuesKeeper(null);

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valuesKeeper);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            // компиляция
            optimization1State.Compile(compiler.Object, calcContext.Object);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());

            optimization2State.Compile(compiler.Object, calcContext.Object);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());

            // контекст расчёта первой оптимизации
            var context = new OptimizationContext(optimization1State, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            var contextState = context.NextTime(calcContext.Object);

            contextState = context.PrepareArgumentContexts(calcContext.Object);

            contextState = context.NextArgument(calcContext.Object);

            Variable retVariable;
            Variable aVariable;
            Variable bVariable;
            Variable cVariable;

            List<ArgumentsValues> aproovedArgs = new List<ArgumentsValues>();


            // вызов проверки области определения для каждого аргумента
            for (double valA = 10; valA < 30; valA += 2)
            {
                for (double valB = 10; valB < 30; valB += 2)
                {
                    for (double valC = 10; valC < 30; valC += 2)
                    {
                        Assert.AreEqual(OptimizationContext.ContextState.CallDD, contextState);

                        // Если результат расчёта области определения положителен
                        contextState = context.CallDD(calcContext.Object);

                        Assert.AreEqual(OptimizationContext.ContextState.ReturnDDValue, contextState);

                        // проверяем, верно ли переданны аргументы
                        retVariable = symbolTable.GetSymbol("@ret") as Variable;
                        aVariable = symbolTable.GetSymbol("a") as Variable;
                        bVariable = symbolTable.GetSymbol("b") as Variable;
                        cVariable = symbolTable.GetSymbol("c") as Variable;

                        Assert.IsNotNull(retVariable);
                        Assert.IsNotNull(aVariable);
                        Assert.IsNotNull(bVariable);
                        Assert.IsNotNull(cVariable);

                        Assert.AreEqual(SymbolValue.Nothing, retVariable.Value);
                        Assert.AreEqual(valA, aVariable.Value.GetValue());
                        Assert.AreEqual(valB, bVariable.Value.GetValue());
                        Assert.AreEqual(valC, cVariable.Value.GetValue());

                        // результат расчёта области определения
                        if (valA * valB - valC * valC > 0)
                        {
                            aproovedArgs.Add(new ArgumentsValues(new { a = valA, b = valB, c = valC }));
                            retVariable.Value = SymbolValue.TrueValue;
                        }
                        else
                            retVariable.Value = SymbolValue.FalseValue; 


                        contextState = context.ReturnDDValue(calcContext.Object);
                    }
                }
            }

            // переход к расчёту оптимизационного выражения
            Assert.AreEqual(OptimizationContext.ContextState.CallDD, contextState);
            contextState = context.CallDD(calcContext.Object);

            // к расчёту оптимизации передаются только те аргументы, область определения для которых положительна
            foreach (var args in aproovedArgs)
            {
                Assert.AreEqual(OptimizationContext.ContextState.CallExpression, contextState);

                contextState = context.CallExpression(calcContext.Object);

                Assert.AreEqual(OptimizationContext.ContextState.ReturnExpressionValue, contextState);

                retVariable = symbolTable.GetSymbol("@ret") as Variable;
                aVariable = symbolTable.GetSymbol("a") as Variable;
                bVariable = symbolTable.GetSymbol("b") as Variable;
                cVariable = symbolTable.GetSymbol("c") as Variable;

                Assert.IsNotNull(retVariable);
                Assert.IsNotNull(aVariable);
                Assert.IsNotNull(bVariable);
                Assert.IsNotNull(cVariable);

                Assert.AreEqual(SymbolValue.Nothing, retVariable.Value);
                Assert.AreEqual((double)args["a"], aVariable.Value.GetValue());
                Assert.AreEqual((double)args["b"], bVariable.Value.GetValue());
                Assert.AreEqual((double)args["c"], cVariable.Value.GetValue());

                contextState = context.ReturnExpressionValue(calcContext.Object);
            }

            Assert.AreEqual(OptimizationContext.ContextState.CallExpression, contextState);
            contextState = context.CallExpression(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.NextTime, contextState);

            // контекст для расчёта второй оптимизации
            context = new OptimizationContext(optimization2State, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            contextState = context.NextTime(calcContext.Object);

            contextState = context.PrepareArgumentContexts(calcContext.Object);

            contextState = context.NextArgument(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.CallDD, contextState);

            // для области определения не заданна формула, переход к расчёту оптимизационного выражения
            contextState = context.CallDD(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.CallExpression, contextState);

            contextState = context.CallExpression(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.ReturnExpressionValue, contextState);

            // проверяем, верно ли переданны аргументы
            retVariable = symbolTable.GetSymbol("@ret") as Variable;
            Variable dVariable = symbolTable.GetSymbol("d") as Variable;
            Variable eVariable = symbolTable.GetSymbol("e") as Variable;

            Assert.IsNotNull(retVariable);
            Assert.IsNotNull(dVariable);
            Assert.IsNotNull(eVariable);

            Assert.AreEqual(SymbolValue.Nothing, retVariable.Value);
            Assert.AreEqual(10.0, dVariable.Value.GetValue());
            Assert.AreEqual(10.0, eVariable.Value.GetValue());

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void CallExpression_ExpressionIsEmpty_CallAllChildParams()
        {
            IOptimizationInfo optimization1Info = (from o in repository.Optimizations where o.Name == "opt_abc_interval" select o).First();
            (optimization1Info as TestOptimizationInfo).Expression = null;

            OptimizationState optimization1State = new OptimizationState(null, optimization1Info, optimization1Info.Revision);

            var compiler = new Moq.Mock<ICompiler>();

            var calcContext = new Moq.Mock<ICalcContext>();

            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            // компиляция
            optimization1State.Compile(compiler.Object, calcContext.Object);

            // во время компиляции не должно быть ошибок
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());

            // контекст расчёта первой оптимизации
            var context = new OptimizationContext(optimization1State, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            var contextState = context.NextTime(calcContext.Object);

            contextState = context.PrepareArgumentContexts(calcContext.Object);

            contextState = context.NextArgument(calcContext.Object);
            Variable retVariable;


            List<ArgumentsValues> argumentsList = new List<ArgumentsValues>();

            for (int i = 0; i < 1000; i++)
            {
                // Если результат расчёта области определения положителен
                contextState = context.CallDD(calcContext.Object);

                Assert.AreEqual(OptimizationContext.ContextState.ReturnDDValue, contextState);

                if (i % 12 == 0)
                {
                    retVariable = symbolTable.GetSymbol("@ret") as Variable;
                    retVariable.Value = SymbolValue.TrueValue;
                    Variable aVar = symbolTable.GetSymbol("a") as Variable;
                    Variable bVar = symbolTable.GetSymbol("b") as Variable;
                    Variable cVar = symbolTable.GetSymbol("c") as Variable;

                    foreach (var variable in new Variable[] { aVar, bVar, cVar })
                    {
                        Assert.IsNotNull(variable);
                        Assert.IsInstanceOf<double>(variable.Value.GetValue());
                    }

                    argumentsList.Add(new ArgumentsValues(new
                    {
                        a = aVar.Value.GetValue(),
                        b = bVar.Value.GetValue(),
                        c = cVar.Value.GetValue()
                    }));
                }

                contextState = context.ReturnDDValue(calcContext.Object);
            }

            contextState = context.CallDD(calcContext.Object);

            // переход к расчёту оптимизационного выражения
            //contextState = context.CallDD(calcContext.Object);
            foreach (var args in argumentsList)
            {
                Assert.AreEqual(OptimizationContext.ContextState.CallExpression, contextState);

                contextState = context.CallExpression(calcContext.Object);

                // переходим к расчёту дочерних параметров
                Assert.AreEqual(OptimizationContext.ContextState.CallChildParameters, contextState);

                foreach (var parameter in repository.Parameters.Where(p => p.Name.Contains("_child_abc")))// new ICalcNode[] { parameter1, parameter2, parameter3, parameter4, parameter5 })
                {
                    contextState = context.NextChildParameter(calcContext.Object);

                    // переходим к расчёту дочерних параметров
                    Assert.AreEqual(OptimizationContext.ContextState.CallChildParameters, contextState);

                    calcContext.Verify(c =>
                        c.Call(
                        new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc),
                        new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc),
                        new CalcNodeKey(parameter.CalcNode, new ArgumentsValues(args))));
                }
                contextState = context.NextChildParameter(calcContext.Object); 
            }

            Assert.AreEqual(OptimizationContext.ContextState.CallExpression, contextState);

            contextState = context.CallExpression(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.NextTime, contextState);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void ReturnExpressionValue_CallAllChildParameters_CallChildParameters()
        {
            IOptimizationInfo optimization1Info = (from o in repository.Optimizations where o.Name == "opt_abc_interval" select o).First();
            (optimization1Info as TestOptimizationInfo).CalcAllChildParameters = true;
            IOptimizationInfo optimization2Info = (from o in repository.Optimizations where o.Name == "opt_de_interval" select o).First();

            OptimizationState optimization1State = new OptimizationState(null, optimization1Info, optimization1Info.Revision);

            var compiler = new Moq.Mock<ICompiler>();

            var calcContext = new Moq.Mock<ICalcContext>();

            var valueKeeper=new Moq.Mock<IValuesKeeper>();

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            // компиляция
            optimization1State.Compile(compiler.Object, calcContext.Object);

            // во время компиляции не должно быть ошибок
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());

            // контекст расчёта первой оптимизации
            var context = new OptimizationContext(optimization1State, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            var contextState = context.NextTime(calcContext.Object);

            contextState = context.PrepareArgumentContexts(calcContext.Object);

            contextState = context.NextArgument(calcContext.Object);
            Variable retVariable;

            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(OptimizationContext.ContextState.CallDD, contextState);

                // Если результат расчёта области определения положителен
                contextState = context.CallDD(calcContext.Object);

                Assert.AreEqual(OptimizationContext.ContextState.ReturnDDValue, contextState);

                retVariable = symbolTable.GetSymbol("@ret") as Variable;
                if (i == 0)
                    retVariable.Value = SymbolValue.TrueValue;
                else
                    retVariable.Value = SymbolValue.FalseValue;

                contextState = context.ReturnDDValue(calcContext.Object);
            }

            // переход к расчёту оптимизационного выражения
            contextState = context.CallDD(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.CallExpression, contextState);

            contextState = context.CallExpression(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.ReturnExpressionValue, contextState);

            retVariable = symbolTable.GetSymbol("@ret") as Variable;
            retVariable.Value = SymbolValue.CreateValue(5);

            contextState = context.ReturnExpressionValue(calcContext.Object);

            // переходим к расчёту дочерних параметров
            Assert.AreEqual(OptimizationContext.ContextState.CallChildParameters, contextState);

            foreach (var parameter in repository.Parameters.Where(p => p.Name.Contains("_child_abc")))// new ICalcNode[] { parameter1, parameter2, parameter3, parameter4, parameter5 })
            {
                contextState = context.NextChildParameter(calcContext.Object);

                // переходим к расчёту дочерних параметров
                Assert.AreEqual(OptimizationContext.ContextState.CallChildParameters, contextState);

                calcContext.Verify(c =>
                    c.Call(
                    new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc),
                    new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc),
                    new CalcNodeKey(parameter.CalcNode, new ArgumentsValues(new { a = 10, b = 10, c = 10 }))));
            }
            contextState = context.NextChildParameter(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.CallExpression, contextState);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void ReturnExpressionValue_CallAllChildParametersAndExpressionResultIsNothing_NotCallChildParameters()
        {
            IOptimizationInfo optimization1Info = (from o in repository.Optimizations where o.Name == "opt_abc_interval" select o).First();
            (optimization1Info as TestOptimizationInfo).CalcAllChildParameters = true;
            IOptimizationInfo optimization2Info = (from o in repository.Optimizations where o.Name == "opt_de_interval" select o).First();

            OptimizationState optimization1State = new OptimizationState(null, optimization1Info, optimization1Info.Revision);

            var compiler = new Moq.Mock<ICompiler>();

            var calcContext = new Moq.Mock<ICalcContext>();

            var valueKeeper = new Moq.Mock<IValuesKeeper>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            // компиляция
            optimization1State.Compile(compiler.Object, calcContext.Object);

            // во время компиляции не должно быть ошибок
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());

            // контекст расчёта первой оптимизации
            var context = new OptimizationContext(optimization1State, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            var contextState = context.NextTime(calcContext.Object);

            contextState = context.PrepareArgumentContexts(calcContext.Object);

            contextState = context.NextArgument(calcContext.Object);
            Variable retVariable;

            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(OptimizationContext.ContextState.CallDD, contextState);

                // Если результат расчёта области определения положителен
                contextState = context.CallDD(calcContext.Object);

                Assert.AreEqual(OptimizationContext.ContextState.ReturnDDValue, contextState);

                retVariable = symbolTable.GetSymbol("@ret") as Variable;

                if (i == 0)
                    retVariable.Value = SymbolValue.TrueValue;
                else
                    retVariable.Value = SymbolValue.FalseValue;


                contextState = context.ReturnDDValue(calcContext.Object); 
            }

            // переход к расчёту оптимизационного выражения
            contextState = context.CallDD(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.CallExpression, contextState);

            contextState = context.CallExpression(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.ReturnExpressionValue, contextState);

            retVariable = symbolTable.GetSymbol("@ret") as Variable;
            retVariable.Value = SymbolValue.Nothing;

            contextState = context.ReturnExpressionValue(calcContext.Object);

            // переходим к расчёту дочерних параметров
            Assert.AreEqual(OptimizationContext.ContextState.CallExpression, contextState);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void NextArgumentSet_NotCallAllChildParameters_CallChildParametersOnEnd()
        {
            IOptimizationInfo optimization1Info = (from o in repository.Optimizations where o.Name == "opt_abc_interval" select o).First();
            (optimization1Info as TestOptimizationInfo).CalcAllChildParameters = false;

            OptimizationState optimization1State = new OptimizationState(null, optimization1Info, optimization1Info.Revision);

            var compiler = new Moq.Mock<ICompiler>();

            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            // компиляция
            optimization1State.Compile(compiler.Object, calcContext.Object);

            // во время компиляции не должно быть ошибок
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());

            // контекст расчёта первой оптимизации
            var context = new OptimizationContext(optimization1State, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            var contextState = context.NextTime(calcContext.Object);

            contextState = context.PrepareArgumentContexts(calcContext.Object);

            contextState = context.NextArgument(calcContext.Object);

            ArgumentsValues optimalArguments = new ArgumentsValues(new { a = 14, b = 12, c = 16 });
            Variable retVariable;

            // перебор всех аргументов
            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(OptimizationContext.ContextState.CallDD, contextState);

                contextState = context.CallDD(calcContext.Object);

                Assert.AreEqual(OptimizationContext.ContextState.ReturnDDValue, contextState);

                retVariable = symbolTable.GetSymbol("@ret") as Variable;
                retVariable.Value = SymbolValue.TrueValue;

                contextState = context.ReturnDDValue(calcContext.Object);
            }

            // переход к расчёту оптимизационного выражения
            contextState = context.CallDD(calcContext.Object);

            // перебор всех аргументов и возвращение результата оптимизации
            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(OptimizationContext.ContextState.CallExpression, contextState);

                contextState = context.CallExpression(calcContext.Object);
                
                Assert.AreEqual(OptimizationContext.ContextState.ReturnExpressionValue, contextState);

                retVariable = symbolTable.GetSymbol("@ret") as Variable;
                retVariable.Value = SymbolValue.CreateValue(Math.Abs(i - 300) + 100);

                Variable aVariable = symbolTable.GetSymbol("a") as Variable;
                Variable bVariable = symbolTable.GetSymbol("b") as Variable;
                Variable cVariable = symbolTable.GetSymbol("c") as Variable;

                Assert.IsNotNull(aVariable);
                Assert.IsNotNull(bVariable);
                Assert.IsNotNull(cVariable);

                // оптимальное значение
                if (aVariable.Value.GetValue().Equals(optimalArguments["a"])
                    && bVariable.Value.GetValue().Equals(optimalArguments["b"])
                    && cVariable.Value.GetValue().Equals(optimalArguments["c"]))
                    retVariable.Value = SymbolValue.CreateValue(10);

                contextState = context.ReturnExpressionValue(calcContext.Object);
            }
            // переходим к расчёту дочерних параметров
            contextState = context.CallExpression(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.CallChildParameters, contextState);

            foreach (var parameter in repository.Parameters.Where(p => p.Name.Contains("_child_abc")))// new ICalcNode[] { parameter1, parameter2, parameter3, parameter4, parameter5 })
            {
                contextState = context.NextChildParameter(calcContext.Object);

                // переходим к расчёту дочерних параметров
                Assert.AreEqual(OptimizationContext.ContextState.CallChildParameters, contextState);

                calcContext.Verify(c =>
                    c.Call(
                    new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc),
                    new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc),
                    new CalcNodeKey(parameter.CalcNode, optimalArguments)));
            }
            contextState = context.NextChildParameter(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.NextTime, contextState);

            // оптимальные значения были сохранены
            valueKeeper.Verify(v => v.SetOptimal(optimization1Info.CalcNode, null, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), optimalArguments));

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void NextArgumentSet_NotCallAllChildParametersAndOptimizationFaild_NotCallChildParametersOnEnd()
        {
            IOptimizationInfo optimization1Info = (from o in repository.Optimizations where o.Name == "opt_abc_interval" select o).First();
            (optimization1Info as TestOptimizationInfo).CalcAllChildParameters = false;

            OptimizationState optimization1State = new OptimizationState(null, optimization1Info, optimization1Info.Revision);

            var compiler = new Moq.Mock<ICompiler>();

            var valueKeeper = new Moq.Mock<IValuesKeeper>();

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            // компиляция
            optimization1State.Compile(compiler.Object, calcContext.Object);

            // во время компиляции не должно быть ошибок
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());

            // контекст расчёта первой оптимизации
            var context = new OptimizationContext(optimization1State, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            var contextState = context.NextTime(calcContext.Object);

            contextState = context.PrepareArgumentContexts(calcContext.Object);

            contextState = context.NextArgument(calcContext.Object);

            Variable retVariable;

            // перебор всех аргументов и выставления области определения
            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(OptimizationContext.ContextState.CallDD, contextState);

                contextState = context.CallDD(calcContext.Object);

                Assert.AreEqual(OptimizationContext.ContextState.ReturnDDValue, contextState);

                retVariable = symbolTable.GetSymbol("@ret") as Variable;
                retVariable.Value = SymbolValue.TrueValue;

                contextState = context.ReturnDDValue(calcContext.Object);
            }

            // переход к расчёту оптимизационного выражения
            contextState = context.CallDD(calcContext.Object);

            // перебор всех аргументов и возвращение результата оптимизации
            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(OptimizationContext.ContextState.CallExpression, contextState);

                contextState = context.CallExpression(calcContext.Object);

                Assert.AreEqual(OptimizationContext.ContextState.ReturnExpressionValue, contextState);

                // расчёт критерия оптимизации при всех аргументах заканчивается неудачей
                retVariable = symbolTable.GetSymbol("@ret") as Variable;
                //retVariable.Value = SymbolValue.CreateValue(Math.Abs(i - 300) + 100);
                retVariable.Value = SymbolValue.Nothing;

                Variable aVariable = symbolTable.GetSymbol("a") as Variable;
                Variable bVariable = symbolTable.GetSymbol("b") as Variable;
                Variable cVariable = symbolTable.GetSymbol("c") as Variable;

                Assert.IsNotNull(aVariable);
                Assert.IsNotNull(bVariable);
                Assert.IsNotNull(cVariable);

                contextState = context.ReturnExpressionValue(calcContext.Object);
            }
            // переходим к расчёту дочерних параметров
            contextState = context.CallExpression(calcContext.Object);

            Assert.AreEqual(OptimizationContext.ContextState.NextTime, contextState);

            // оптимальные значения были сохранены
            valueKeeper.Verify(v => v.SetOptimal(optimization1Info.CalcNode, null, new DateTime(2012, 01, 01), ArgumentsValues.BadArguments));

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void CurrentNode_ReturnCorrectInfo()
        {
            IOptimizationInfo optimization1Info = (from o in repository.Optimizations where o.Name == "opt_hij_expression" select o).First();
            //(optimization1Info as TestOptimizationInfo).Expression = "h*i*j";

            OptimizationState optimization1State = new OptimizationState(null, optimization1Info, optimization1Info.Revision);

            var compiler = new Moq.Mock<ICompiler>();

            var calcContext = new Moq.Mock<ICalcContext>();

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            ICallContext currentContext=null;

            calcContext.Setup(c => c.Call(Moq.It.IsAny<ICallContext>()))
                .Callback((ICallContext callContext) => currentContext = callContext)
                .Returns(true);

            // компиляция
            optimization1State.Compile(compiler.Object, calcContext.Object);

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());

            // контекст расчёта первой оптимизации
            var context = new OptimizationContext(optimization1State, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            var passed = context.PrepareArgs(calcContext.Object);

            Assert.IsTrue(passed);

            var contextState = context.NextTime(calcContext.Object);

            contextState = context.PrepareArgumentContexts(calcContext.Object);

            contextState = context.NextArgument(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.ReturnArgumentValue, contextState);

            // проверка состояние: расчёт аргумента h
            Assert.IsNotNull(currentContext);
            CalcPosition calcPosition = currentContext.CurrentNode;
            Assert.IsNotNull(calcPosition);
            Assert.AreEqual(34, calcPosition.NodeID);
            Assert.AreEqual(CalcPosition.NodePart.ArgumentExpression, calcPosition.CurrentPart);
            Assert.AreEqual("h", calcPosition.AdditionNote);

            Variable retVariable = symbolTable.GetSymbol("@ret") as Variable;
            retVariable.Value = new ArrayValue(calcContext.Object, new SymbolValue[] {
                SymbolValue.CreateValue(10), 
                SymbolValue.CreateValue(13), 
                SymbolValue.CreateValue(16), 
                SymbolValue.CreateValue(17),
                SymbolValue.CreateValue(20)
            });
            currentContext = null;

            contextState = context.ReturnArgument(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.NextArgument, contextState);

            contextState = context.NextArgument(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.ReturnArgumentValue, contextState);

            // проверка состояние: расчёт аргумента j
            Assert.IsNotNull(currentContext);
            calcPosition = currentContext.CurrentNode;
            Assert.IsNotNull(calcPosition);
            Assert.AreEqual(34, calcPosition.NodeID);
            Assert.AreEqual(CalcPosition.NodePart.ArgumentExpression, calcPosition.CurrentPart);
            Assert.AreEqual("j", calcPosition.AdditionNote);
            currentContext = null;
  
            retVariable = symbolTable.GetSymbol("@ret") as Variable;
            retVariable.Value = new ArrayValue(calcContext.Object, new SymbolValue[] {
                SymbolValue.CreateValue(15), 
                SymbolValue.CreateValue(20), 
                SymbolValue.CreateValue(33), 
                SymbolValue.CreateValue(37),
                SymbolValue.CreateValue(40)
            });

            contextState = context.ReturnArgument(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.NextArgument, contextState);
            contextState = context.NextArgument(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.CallDD, contextState);

            // При отсутствии формулы для расчёта области определения, перейти к расчёту оптимизационного выражения
            contextState = context.CallDD(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.CallExpression, contextState);

            // начало расчёта оптимизационного выражения
            contextState = context.CallExpression(calcContext.Object);
            Assert.AreEqual(OptimizationContext.ContextState.ReturnExpressionValue, contextState);

            // проверка состояние: расчёт критерия
            Assert.IsNotNull(currentContext);
            calcPosition = currentContext.CurrentNode;
            Assert.IsNotNull(calcPosition);
            Assert.AreEqual(34, calcPosition.NodeID);
            Assert.AreEqual(CalcPosition.NodePart.Expression, calcPosition.CurrentPart);
            Assert.IsNullOrEmpty(calcPosition.AdditionNote);
            currentContext = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Проверяемое поведение:
        /// <br />
        /// Метод должен возвращать true, если тот же самый параметр и аргументы более общие или такие же
        /// </remarks>
        [Test]
        public void TheSame__TrueIfNodeIsSameAndArgumentsAreBaseOrSame()
        {
            ICalcNode optimization1 = new TestCalcNode() { NodeID = 34 };
            ICalcNode optimization2 = new TestCalcNode() { NodeID = 35 };
            ICalcNode optimization3 = new TestCalcNode() { NodeID = 36 };

            IOptimizationInfo optimization1Info = new TestOptimizationInfo(optimization1)
            {
                Interval = Interval.Day,
                Calculable = true,
                Expression = "par1;",
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
                Calculable = true,
                Expression = "par1;",
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
            IOptimizationInfo optimization3Info = new TestOptimizationInfo(optimization3)
            {
                Interval = Interval.Day,
                Calculable = true,
                Expression = "par1;",
                Optimization = optimization1Info,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "e"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "f"
                    }
                }
            };
            var state2 = new OptimizationState(null, optimization2Info, optimization2Info.Revision);
            var state3 = new OptimizationState(null, optimization3Info, optimization3Info.Revision);

            var context1 = new OptimizationContext(state3, new DateTime(2012, 01, 01), new DateTime(2012, 01, 01));
            context1.Arguments = new ArgumentsValues(new { a = 10, b = 12, d = 14, e = 16 });

            // те же аргументы
            var context2 = new OptimizationContext(state3, new DateTime(2012, 01, 01), new DateTime(2012, 01, 01));
            context2.Arguments = new ArgumentsValues(new { a = 10, b = 12, d = 14, e = 16 });

            // более общие аргументы
            var context3 = new OptimizationContext(state3, new DateTime(2012, 01, 01), new DateTime(2012, 01, 01));
            context3.Arguments = new ArgumentsValues(new { a = 10, b = 12, });

            // другие аргументы
            var context4 = new OptimizationContext(state3, new DateTime(2012, 01, 01), new DateTime(2012, 01, 01));
            context4.Arguments = new ArgumentsValues(new { a = 12, b = 12, d = 14, e = 16 });

            // другие базовые аргументы
            var context5 = new OptimizationContext(state3, new DateTime(2012, 01, 01), new DateTime(2012, 01, 01));
            context5.Arguments = new ArgumentsValues(new { a = 12, b = 12, });
           
            // другие базовые аргументы
            var context6 = new OptimizationContext(state2, new DateTime(2012, 01, 01), new DateTime(2012, 01, 01));
            context5.Arguments = new ArgumentsValues(new { a = 12, b = 12, });


            bool ret;

            // context1 и context2 взаимно равны
            ret = context1.TheSame(context2);
            Assert.IsTrue(ret);
            ret = context2.TheSame(context1);
            Assert.IsTrue(ret);

            // context1 и context3 частный с частным и общим аргументом
            ret = context1.TheSame(context3);
            Assert.IsFalse(ret);
            ret = context3.TheSame(context1);
            Assert.IsTrue(ret);


            // context1 и context4 взаимно разные
            ret = context1.TheSame(context4);
            Assert.IsFalse(ret);
            ret = context4.TheSame(context1);
            Assert.IsFalse(ret);

            // context1 и context5 взаимно разные
            ret = context1.TheSame(context5);
            Assert.IsFalse(ret);
            ret = context5.TheSame(context1);
            Assert.IsFalse(ret);

            // context5 и context6 взаимно разные (разные узлы, одинаковые аргументы)
            ret = context5.TheSame(context6);
            Assert.IsFalse(ret);
            ret = context6.TheSame(context5);
            Assert.IsFalse(ret);
        }

        [Test]
        public void TheSame_DifferentRevision_ReturnFale()
        {
            RevisionInfo revision1 = new RevisionInfo()
            {
                ID = 1,
                Time = new DateTime(2012, 02, 01)
            };

            ICalcNode optimization1 = new TestCalcNode() { NodeID = 34 };
            ICalcNode optimization2 = new TestCalcNode() { NodeID = 35 };
            ICalcNode optimization3 = new TestCalcNode() { NodeID = 36 };

            IOptimizationInfo optimization1Info = new TestOptimizationInfo(optimization1)
            {
                Interval = Interval.Day,
                Calculable = true,
                Expression = "par1;",
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
                Calculable = true,
                Expression = "par1;",
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
            IOptimizationInfo optimization3Info = new TestOptimizationInfo(optimization3)
            {
                Interval = Interval.Day,
                Calculable = true,
                Expression = "par1;",
                Optimization = optimization1Info,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "e"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "f"
                    }
                }
            };

            // состояния узлов созданные для разных ревизий
            var state3_0 = new OptimizationState(null, optimization3Info, RevisionInfo.Default);
            var state3_1 = new OptimizationState(null, optimization3Info, revision1);

            var context1_0 = new OptimizationContext(state3_0, new DateTime(2012, 01, 01), new DateTime(2012, 01, 01));
            context1_0.Arguments = new ArgumentsValues(new { a = 10, b = 12, d = 14, e = 16 });

            var context1_1 = new OptimizationContext(state3_1, new DateTime(2012, 02, 01), new DateTime(2012, 02, 02));
            context1_1.Arguments = new ArgumentsValues(new { a = 10, b = 12, d = 14, e = 16 });

            bool ret;

            ret = context1_0.TheSame(context1_1);
            Assert.IsFalse(ret);
            ret = context1_1.TheSame(context1_0);
            Assert.IsFalse(ret);
        }
    }
}

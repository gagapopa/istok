using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    [TestFixture]
    public class CalcContextTests
    {
        ICompiler compiler;

        [SetUp]
        public void Init()
        {
            var compilerMoq = new Moq.Mock<ICompiler>();

            compilerMoq.Setup(c =>
               c.Compile(
               Moq.It.IsAny<ICalcContext>(),
               Moq.It.IsAny<RevisionInfo>(),
               Moq.It.IsAny<String>(),
               Moq.It.IsAny<KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>[]>()))
               .Returns(new Instruction[]
                {
                    new Instruction(null, Instruction.OperationCode.EnterLevel),
                    new Instruction(null, Instruction.OperationCode.NOP),
                    new Instruction(null, Instruction.OperationCode.LeaveLevel),
                    new Instruction(null, Instruction.OperationCode.Return)
                });

            compiler = compilerMoq.Object;
        }

        [Test]
        public void GetState__CalcStateCompileOnlyOnce()
        {
            // Список ревизий
            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            // компилируемый параметр
            IParameterInfo paramInfo = new TestParameterinfo(new TestCalcNode() { NodeID = 1 }) { Code = "par1", Formula = "45+65;", Interval = Interval.Hour };

            // состояние этого параметра
            var calcState = new Moq.Mock<ICalcState>();
            calcState.Setup(s => s.NodeInfo).Returns(paramInfo);
            calcState.Setup(s => s.Revision).Returns(RevisionInfo.Default);

            // хранилище состояний параметра
            var calcStateStorage = new Moq.Mock<ICalcStateStorage>();
            calcStateStorage.Setup(c =>
                c.GetState(Moq.It.IsAny<ICalcNode>(), Moq.It.IsAny<RevisionInfo>()))
                .Returns(calcState.Object);

            CalcContext calcContext = new CalcContext(new OperationState(), calcStateStorage.Object, calcSupplier.Object, compiler);

            ICalcState state1 = calcContext.GetState(paramInfo.CalcNode, RevisionInfo.Default);
            ICalcState state2 = calcContext.GetState(paramInfo.CalcNode, RevisionInfo.Default);

            var callContext1 = calcContext.CreateCallContext(paramInfo.CalcNode, null, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));
            var callContext2 = calcContext.CreateCallContext(paramInfo.CalcNode, null, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            Assert.AreSame(calcState.Object, state1);
            Assert.AreSame(calcState.Object, state2);

            Assert.AreNotSame(callContext1, callContext2);

            calcState.Verify(s => s.Compile(Moq.It.IsAny<ICompiler>(), Moq.It.IsAny<ICalcContext>()), Moq.Times.Once());
        }

        [Test]
        public void CreateCallContext_ManyRevisionInInterval_CreateMetaCallContext()
        {
            // настройка списка ревизий
            RevisionInfo revision1 = new RevisionInfo() { ID = 1, Time = new DateTime(2012, 01, 7) };
            RevisionInfo revision2 = new RevisionInfo() { ID = 56, Time = new DateTime(2012, 01, 15) };

            var revisions = new RevisionInfo[] { RevisionInfo.Default, revision1, revision2 };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(revisions);

            CalcStateStorage storage = new CalcStateStorage();

            // Расчитываемые параметры
            ICalcNode calcNode = new TestCalcNode() { NodeID = 34, Name = "parameter" };
            new TestParameterinfo(calcNode) { Code = "par1", Formula = "34+56;", Interval = Interval.Hour };
            // сам параметр не отличается в этой ревизии, но компиляция и расчёт для каждой ревизии должны быть изолированны
            new TestParameterinfo(calcNode, revision2) { Code = "par1", Formula = "744+56;", Interval = Interval.Hour };


            // проверяемый объект
            var calcContext = new CalcContext(new OperationState(), storage, calcSupplier.Object, compiler);

            // проверяемое действие
            var contexts = calcContext.CreateCallContext(calcNode, null, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            Assert.IsNotNull(contexts);
            Assert.AreEqual(3, contexts.Count());

            DateTime[] startTime = new DateTime[] { new DateTime(2012, 01, 01), new DateTime(2012, 01, 07), new DateTime(2012, 01, 15) };
            DateTime[] endTime = new DateTime[] { new DateTime(2012, 01, 07), new DateTime(2012, 01, 15), new DateTime(2012, 02, 01) };

            int i = 0;
            foreach (var subContext in contexts)
            {
                Assert.AreSame(typeof(NodeContext), subContext.GetType());

                Assert.AreEqual(startTime[i], subContext.CalcStartTime);
                Assert.AreEqual(endTime[i], subContext.CalcEndTime);

                ++i;
            }
        }

        [Test]
        public void CreateCallContext_OnlyOneRevisionInInterval_CreateNeededCallContext()
        {
            // список ревизий, состоящий из одного элемента
            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            CalcStateStorage storage = new CalcStateStorage();
            // расчитываемые параметры
            IParameterInfo paramInfo = new TestParameterinfo(new TestCalcNode() { NodeID = 1 }) { Code = "par1", Formula = "45+65;", Interval = Interval.Hour };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(new TestCalcNode() { NodeID = 100 })
            {
                Expression = "a/b;",
                Interval = Interval.Hour,
                Arguments = new IOptimizationArgument[] { }
            };

            // тестируемый объект
            var calcContext = new CalcContext(new OperationState(), storage, calcSupplier.Object, compiler);

            var parameterContexts = calcContext.CreateCallContext(paramInfo.CalcNode, null, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            var optimizationContexts = calcContext.CreateCallContext(optimizationInfo.CalcNode, null, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            // проверка контекста параметра
            Assert.IsNotNull(parameterContexts);
            Assert.AreEqual(1, parameterContexts.Count());
            ICallContext parameterContext = parameterContexts.First();
            Assert.AreEqual(new DateTime(2012, 01, 01), parameterContext.CalcStartTime);
            Assert.AreEqual(new DateTime(2012, 02, 01), parameterContext.CalcEndTime);
            Assert.AreSame(typeof(NodeContext), parameterContext.GetType());

            // проверка контекста оптимизации
            Assert.IsNotNull(optimizationContexts);
            Assert.AreEqual(1, optimizationContexts.Count());
            ICallContext optimizationContext = optimizationContexts.First();
            Assert.AreEqual(new DateTime(2012, 01, 01), optimizationContext.CalcStartTime);
            Assert.AreEqual(new DateTime(2012, 02, 01), optimizationContext.CalcEndTime);
            Assert.AreSame(typeof(OptimizationContext), optimizationContext.GetType());
        }

        [Test]
        public void Call__CreateCallContext()
        {
            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            CalcStateStorage storage = new CalcStateStorage();

            CalcContext calcContext = new CalcContext(new OperationState(), storage, calcSupplier.Object, compiler);

            var paramInfo = new TestParameterinfo(new TestCalcNode() { NodeID = 2 }) { Code = "par2", Formula = "36+56;" };

            calcContext.SymbolTable.PushSymbolScope();

            calcContext.SymbolTable.DeclareSymbol(new Variable("a"));
            calcContext.Call(new DateTime(2012, 01, 01), new DateTime(2012, 01, 02), new CalcNodeKey(paramInfo.CalcNode, null));

            NodeContext nodeContext = calcContext.SymbolTable.CallContext as NodeContext;

            Assert.IsNotNull(nodeContext);
            Assert.AreSame(paramInfo, nodeContext.Node.NodeInfo);

            // тест на изолираванность
            Assert.IsNull(calcContext.SymbolTable.GetSymbol("a"), "Тест на изолированность созданного контекста");
        }

        [Test]
        public void Call_OptimizationParameter_AddArgumentsToSymbolTable()
        {
            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            CalcStateStorage storage = new CalcStateStorage();
            
            CalcContext calcContext = new CalcContext(new OperationState(), storage, calcSupplier.Object, compiler);

            var optimizationInfo = new TestOptimizationInfo(new TestCalcNode() { NodeID = 34 }) { 
                Interval=Interval.Day,
                Arguments= new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name="a"
                    },
                    new TestOptimizationArgument()
                    {
                        Name="b"
                    },
                    new TestOptimizationArgument()
                    {
                        Name="c"
                    }
                }
            };

            var paramInfo = new TestParameterinfo(new TestCalcNode() { NodeID = 2 })
            {
                Interval = Interval.Day,
                Optimization = optimizationInfo,
                Code = "par2",
                Formula = "36+56;"
            };

            calcContext.SymbolTable.PushSymbolScope();

            calcContext.Call(
                new DateTime(2012, 01, 01), 
                new DateTime(2012, 01, 02), 
                new CalcNodeKey(
                    paramInfo.CalcNode, 
                    new ArgumentsValues(new { a = 15, b = 21, c = 34 })));

            NodeContext nodeContext = calcContext.SymbolTable.CallContext as NodeContext;

            Assert.IsNotNull(nodeContext);
            Assert.AreSame(paramInfo, nodeContext.Node.NodeInfo);

            Variable var;
            // аргумент a
            var = calcContext.SymbolTable.GetSymbol("a") as Variable;
            Assert.IsNotNull(var);
            Assert.AreEqual(15.0, var.Value.GetValue());
            // аргумент b
            var = calcContext.SymbolTable.GetSymbol("b") as Variable;
            Assert.IsNotNull(var);
            Assert.AreEqual(21.0, var.Value.GetValue());
            // аргумент c
            var = calcContext.SymbolTable.GetSymbol("c") as Variable;
            Assert.IsNotNull(var);
            Assert.AreEqual(34.0, var.Value.GetValue());
        }

        [Test]
        public void Call_ParameterInList_StretchTime()
        {
            // Список ревизий
            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            // инициализируемый тестируемый объект
            CalcContext calcContext = new CalcContext(new OperationState(), new CalcStateStorage(), calcSupplier.Object, compiler);

            // расчитываемые параметры
            ICalcNodeInfo paramInfo;

            var nodes = new ICalcNodeInfo[] { 
                new TestParameterinfo(new TestCalcNode(){NodeID=1}){Code="par1", Formula="34;"},
                paramInfo = new TestParameterinfo(new TestCalcNode(){NodeID=2}){Code="par2", Formula="36+56;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=3}){Code="par3"},
                new TestParameterinfo(new TestCalcNode(){NodeID=4}){Code="par4", Formula="a=453; a+34;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=5}){Code="par5", Formula="a=453; a+34;"},
            };

            calcContext.AddParametersToCalc(from n in nodes select n.CalcNode);

            calcContext.SetTime(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            calcContext.Call(new DateTime(2012, 01, 01), new DateTime(2012, 01, 02), new CalcNodeKey(paramInfo.CalcNode, null));

            NodeContext callContext = calcContext.SymbolTable.CallContext as NodeContext;

            Assert.IsNotNull(callContext);
            Assert.AreSame(paramInfo, callContext.Node.NodeInfo);
            Assert.AreEqual(new DateTime(2012, 01, 01), callContext.CalcStartTime);
            Assert.AreEqual(new DateTime(2012, 02, 01), callContext.CalcEndTime);
        }

        [Test]
        public void Call_ParameterCalcStateIsFail_FillByNothing()
        {
            ICalcNode calcNode = new TestCalcNode() { NodeID = 45, Name = "abc" };

            IParameterInfo parameter = new TestParameterinfo(calcNode)
            {
                Calculable = true,
                Interval = Interval.Day,
                Formula = "a + b;"
            };
            
            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            var shadowCalcStateStorage = new CalcStateStorage();
            var calcStateStorage = new Moq.Mock<ICalcStateStorage>();
            var parameterState = new NodeState(parameter, parameter.Revision);

            calcStateStorage.Setup(s => s.GetState(calcNode, RevisionInfo.Default)).Returns(parameterState);
            calcStateStorage.Setup(s =>
                s.GetState(Moq.It.Is<ICalcNode>(n => n != calcNode), Moq.It.IsAny<RevisionInfo>()))
                .Returns((ICalcNode n, RevisionInfo r) => shadowCalcStateStorage.GetState(n, r));


            var calcContext = new CalcContext(new OperationState(), calcStateStorage.Object, calcSupplier.Object, compiler);
            calcContext.ValuesKeeper = new ValuesKeeper(calcSupplier.Object);

            parameterState.Compile(compiler, calcContext);
            parameterState.Failed = true;

            bool ret = calcContext.Call(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01), new CalcNodeKey(calcNode, null));

            Assert.IsFalse(ret);

            for (int i = 1; i <= 31; i++)
            {
                Assert.AreSame(SymbolValue.Nothing, calcContext.ValuesKeeper.GetRawValue(calcNode, null, new DateTime(2012, 01, i))); 
            }
        }

        [Test]
        public void Call_OptimizationCalcStateIsFail_FillByNothing()
        {
            ICalcNode optimizationNode = new TestCalcNode() { NodeID = 32, Name = "opt" };
            ICalcNode parameter1Node = new TestCalcNode() { NodeID = 45, Name = "param1" };
            ICalcNode parameter2Node = new TestCalcNode() { NodeID = 45, Name = "param1" };
            ICalcNode parameter3Node = new TestCalcNode() { NodeID = 45, Name = "param1" };
            ICalcNode parameter4Node = new TestCalcNode() { NodeID = 45, Name = "param1" };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimizationNode)
            {
                Interval=Interval.Day,
                Arguments=new IOptimizationArgument[]
                {
                    new TestOptimizationArgument() 
                    {
                        Name = "a",
                        Mode = OptimizationArgumentMode.Interval,
                        IntervalFrom = 10,
                        IntervalTo = 20,
                        IntervalStep = 2
                    },
                    new TestOptimizationArgument() 
                    {
                        Name = "b",
                        Mode = OptimizationArgumentMode.Interval,
                        IntervalFrom = 10,
                        IntervalTo = 20,
                        IntervalStep = 2
                    },
                    new TestOptimizationArgument() 
                    {
                        Name = "c",
                        Mode = OptimizationArgumentMode.Interval,
                        IntervalFrom = 10,
                        IntervalTo = 20,
                        IntervalStep = 2
                    }
                }
            };
            IParameterInfo parameter1Info = new TestParameterinfo(parameter1Node)
            {
                Calculable = true,
                Interval = Interval.Day,
                Formula = "a + b;",
                Optimization = optimizationInfo
            };
            IParameterInfo parameter2Info = new TestParameterinfo(parameter2Node)
            {
                Calculable = true,
                Interval = Interval.Day,
                Formula = "a + b;",
                Optimization = optimizationInfo
            };
            IParameterInfo parameter3Info = new TestParameterinfo(parameter3Node)
            {
                Calculable = true,
                Interval = Interval.Day,
                Formula = "a + b;",
                Optimization = optimizationInfo
            };
            IParameterInfo parameter4Info = new TestParameterinfo(parameter4Node)
            {
                Calculable = true,
                Interval = Interval.Day,
                Formula = "a + b;",
                Optimization = optimizationInfo
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            var shadowCalcStateStorage = new CalcStateStorage();
            var calcStateStorage = new Moq.Mock<ICalcStateStorage>();
            var optimizationState = new OptimizationState(calcStateStorage.Object, optimizationInfo, optimizationInfo.Revision);

            calcStateStorage.Setup(s => s.GetState(optimizationNode, RevisionInfo.Default)).Returns(optimizationState);
            calcStateStorage.Setup(s =>
                s.GetState(Moq.It.Is<ICalcNode>(n => n != optimizationNode), Moq.It.IsAny<RevisionInfo>()))
                .Returns((ICalcNode n, RevisionInfo r) => shadowCalcStateStorage.GetState(n, r));

            var calcContext = new CalcContext(new OperationState(), calcStateStorage.Object, calcSupplier.Object, compiler);
            calcContext.ValuesKeeper = new ValuesKeeper(calcSupplier.Object);

            optimizationState.Compile(compiler, calcContext);
            optimizationState.Failed = true;

            bool ret = calcContext.Call(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01), new CalcNodeKey(optimizationNode, null));

            Assert.IsFalse(ret);

            for (int i = 1; i <= 31; i++)
            {
                Assert.AreSame(ArgumentsValues.BadArguments, calcContext.ValuesKeeper.GetOptimal(optimizationNode, null, new DateTime(2012, 01, i)));
                Assert.AreSame(SymbolValue.Nothing, calcContext.ValuesKeeper.GetRawValue(parameter1Node, null, new DateTime(2012, 01, i)));
                Assert.AreSame(SymbolValue.Nothing, calcContext.ValuesKeeper.GetRawValue(parameter2Node, null, new DateTime(2012, 01, i)));
                Assert.AreSame(SymbolValue.Nothing, calcContext.ValuesKeeper.GetRawValue(parameter3Node, null, new DateTime(2012, 01, i)));
                Assert.AreSame(SymbolValue.Nothing, calcContext.ValuesKeeper.GetRawValue(parameter4Node, null, new DateTime(2012, 01, i)));
            }
        }

        [Test]
        public void Return_ParameterCalcStateIsFail_FillByNothing()
        {
            ICalcNode calcNode = new TestCalcNode() { NodeID = 45, Name = "abc" };

            IParameterInfo parameter = new TestParameterinfo(calcNode)
            {
                Calculable = true,
                Interval = Interval.Day,
                Formula = "a + b;"
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            var shadowCalcStateStorage = new CalcStateStorage();
            var calcStateStorage = new Moq.Mock<ICalcStateStorage>();
            var parameterState = new NodeState(parameter, parameter.Revision);

            calcStateStorage.Setup(s => s.GetState(calcNode, RevisionInfo.Default)).Returns(parameterState);
            calcStateStorage.Setup(s =>
                s.GetState(Moq.It.Is<ICalcNode>(n => n != calcNode), Moq.It.IsAny<RevisionInfo>()))
                .Returns((ICalcNode n, RevisionInfo r) => shadowCalcStateStorage.GetState(n, r));


            var calcContext = new CalcContext(new OperationState(), calcStateStorage.Object, calcSupplier.Object, compiler);
            calcContext.ValuesKeeper = new ValuesKeeper(calcSupplier.Object);

            bool ret = calcContext.Call(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01), new CalcNodeKey(calcNode, null));

            Assert.IsTrue(ret);

            parameterState.Failed = true;

            calcContext.Return();

            for (int i = 1; i <= 31; i++)
            {
                Assert.AreSame(SymbolValue.Nothing, calcContext.ValuesKeeper.GetRawValue(calcNode, null, new DateTime(2012, 01, i)));
            }
        }

        [Test]
        public void Return_OptimizationCalcStateIsFail_FillByNothing()
        {
            ICalcNode optimizationNode = new TestCalcNode() { NodeID = 32, Name = "opt" };
            ICalcNode parameter1Node = new TestCalcNode() { NodeID = 45, Name = "param1" };
            ICalcNode parameter2Node = new TestCalcNode() { NodeID = 45, Name = "param1" };
            ICalcNode parameter3Node = new TestCalcNode() { NodeID = 45, Name = "param1" };
            ICalcNode parameter4Node = new TestCalcNode() { NodeID = 45, Name = "param1" };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimizationNode)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument() 
                    {
                        Name = "a",
                        Mode = OptimizationArgumentMode.Interval,
                        IntervalFrom = 10,
                        IntervalTo = 20,
                        IntervalStep = 2
                    },
                    new TestOptimizationArgument() 
                    {
                        Name = "b",
                        Mode = OptimizationArgumentMode.Interval,
                        IntervalFrom = 10,
                        IntervalTo = 20,
                        IntervalStep = 2
                    },
                    new TestOptimizationArgument() 
                    {
                        Name = "c",
                        Mode = OptimizationArgumentMode.Interval,
                        IntervalFrom = 10,
                        IntervalTo = 20,
                        IntervalStep = 2
                    }
                }
            };
            IParameterInfo parameter1Info = new TestParameterinfo(parameter1Node)
            {
                Calculable = true,
                Interval = Interval.Day,
                Formula = "a + b;",
                Optimization = optimizationInfo
            };
            IParameterInfo parameter2Info = new TestParameterinfo(parameter2Node)
            {
                Calculable = true,
                Interval = Interval.Day,
                Formula = "a + b;",
                Optimization = optimizationInfo
            };
            IParameterInfo parameter3Info = new TestParameterinfo(parameter3Node)
            {
                Calculable = true,
                Interval = Interval.Day,
                Formula = "a + b;",
                Optimization = optimizationInfo
            };
            IParameterInfo parameter4Info = new TestParameterinfo(parameter4Node)
            {
                Calculable = true,
                Interval = Interval.Day,
                Formula = "a + b;",
                Optimization = optimizationInfo
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            var shadowCalcStateStorage = new CalcStateStorage();
            var calcStateStorage = new Moq.Mock<ICalcStateStorage>();
            var optimizationState = new OptimizationState(calcStateStorage.Object, optimizationInfo, optimizationInfo.Revision);

            calcStateStorage.Setup(s => s.GetState(optimizationNode, RevisionInfo.Default)).Returns(optimizationState);
            calcStateStorage.Setup(s =>
                s.GetState(Moq.It.Is<ICalcNode>(n => n != optimizationNode), Moq.It.IsAny<RevisionInfo>()))
                .Returns((ICalcNode n, RevisionInfo r) => shadowCalcStateStorage.GetState(n, r));

            var calcContext = new CalcContext(new OperationState(), calcStateStorage.Object, calcSupplier.Object, compiler);
            calcContext.ValuesKeeper = new ValuesKeeper(calcSupplier.Object);

            bool ret = calcContext.Call(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01), new CalcNodeKey(optimizationNode, null));

            Assert.IsTrue(ret);

            optimizationState.Failed = true;

            calcContext.Return();

            for (int i = 1; i <= 31; i++)
            {
                Assert.AreSame(ArgumentsValues.BadArguments, calcContext.ValuesKeeper.GetOptimal(optimizationNode, null, new DateTime(2012, 01, i)));
                Assert.AreSame(SymbolValue.Nothing, calcContext.ValuesKeeper.GetRawValue(parameter1Node, null, new DateTime(2012, 01, i)));
                Assert.AreSame(SymbolValue.Nothing, calcContext.ValuesKeeper.GetRawValue(parameter2Node, null, new DateTime(2012, 01, i)));
                Assert.AreSame(SymbolValue.Nothing, calcContext.ValuesKeeper.GetRawValue(parameter3Node, null, new DateTime(2012, 01, i)));
                Assert.AreSame(SymbolValue.Nothing, calcContext.ValuesKeeper.GetRawValue(parameter4Node, null, new DateTime(2012, 01, i)));
            }
        }

        [Test]
        public void NextNode__SelectNextNode()
        {
            // Список ревизий
            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            var valuesKeeper = new Moq.Mock<IValuesKeeper>();

            // инициализируемый тестируемый объект
            CalcContext calcContext = new CalcContext(new OperationState(), new CalcStateStorage(), calcSupplier.Object, compiler);
            calcContext.ValuesKeeper = valuesKeeper.Object;

            // расчитываемые параметры
            var nodes = new ICalcNodeInfo[] { 
                new TestParameterinfo(new TestCalcNode(){NodeID=1}){Code="par1", Formula="34;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=2}){Code="par2", Formula="36+56;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=3}){Code="par3"},
                new TestParameterinfo(new TestCalcNode(){NodeID=4}){Code="par4", Formula="a=453; a+34;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=5}){Code="par5", Formula="a=453; a+34;"},
            };

            calcContext.AddParametersToCalc(from n in nodes select n.CalcNode);

            List<ICalcNodeInfo> calcNodeList = new List<ICalcNodeInfo>();

            while (calcContext.NextNode())
            {
                NodeContext nodeContext = calcContext.SymbolTable.CallContext as NodeContext;

                Assert.IsNotNull(nodeContext);

                calcNodeList.Add(nodeContext.Node.NodeInfo);
                // как бы уже посчитали параметр
                calcContext.Return();
            }

            Assert.AreEqual(nodes.Length, calcNodeList.Count);
            for (int i = 0; i < nodes.Length; i++)
            {
                Assert.AreSame(nodes[i], calcNodeList[i]);
            }
        }

        [Test]
        public void NextNode__SkipCalledNodes()
        {
            // Список ревизий
            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            var valuesKeeper = new Moq.Mock<IValuesKeeper>();

            // инициализируемый тестируемый объект
            CalcContext calcContext = new CalcContext(new OperationState(), new CalcStateStorage(), calcSupplier.Object, compiler);
            calcContext.ValuesKeeper = valuesKeeper.Object;

            // расчитываемые параметры
            ICalcNodeInfo paramInfo;

            var nodes = new ICalcNodeInfo[] { 
                new TestParameterinfo(new TestCalcNode(){NodeID=1}){Code="par1", Formula="34;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=2}){Code="par2", Formula="36+56;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=3}){Code="par3"},
                paramInfo = new TestParameterinfo(new TestCalcNode(){NodeID=4}){Code="par4", Formula="a=453; a+34;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=5}){Code="par5", Formula="a=453; a+34;"},
            };

            calcContext.AddParametersToCalc(from n in nodes select n.CalcNode);
            calcContext.SetTime(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            List<ICalcNodeInfo> calcNodeList = new List<ICalcNodeInfo>();

            bool called = false;

            while (calcContext.NextNode())
            {
                NodeContext nodeContext = calcContext.SymbolTable.CallContext as NodeContext;

                Assert.IsNotNull(nodeContext);
                calcNodeList.Add(nodeContext.Node.NodeInfo);

                if (!called)
                {
                    // считаем параметр по зависимости
                    calcContext.Call(new DateTime(2012, 01, 02), new DateTime(2012, 01, 03), new CalcNodeKey(paramInfo.CalcNode, null));
                    called = true;
                    calcContext.Return();
                }
                calcContext.Return();
            }

            var expectedNodes = (from n in nodes where !n.Equals(paramInfo) select n).ToArray();

            Assert.AreEqual(expectedNodes.Length, calcNodeList.Count);
            for (int i = 0; i < expectedNodes.Length; i++)
            {
                Assert.AreSame(expectedNodes[i], calcNodeList[i]);
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NextNode_NotEmptyCallStack_ThrowException()
        {
            // Список ревизий
            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            // инициализируемый тестируемый объект
            CalcContext calcContext = new CalcContext(new OperationState(), new CalcStateStorage(), calcSupplier.Object, compiler);

            // расчитываемые параметры
            var nodes = new ICalcNodeInfo[] { 
                new TestParameterinfo(new TestCalcNode(){NodeID=1}){Code="par1", Formula="34;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=2}){Code="par2", Formula="36+56;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=3}){Code="par3"},
                new TestParameterinfo(new TestCalcNode(){NodeID=4}){Code="par4", Formula="a=453; a+34;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=5}){Code="par5", Formula="a=453; a+34;"},
            };

            calcContext.AddParametersToCalc(from n in nodes select n.CalcNode);

            List<ICalcNodeInfo> calcNodeList = new List<ICalcNodeInfo>();

            calcContext.NextNode();
            // вызываем NextNode без вызова Return
            calcContext.NextNode();
        }

        [Test]
        public void Call__ChangeOperattionStateText()
        {
            OperationState state = new OperationState();

            // Список ревизий
            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            var valuesKeeper = new Moq.Mock<IValuesKeeper>();

            // инициализируемый тестируемый объект
            CalcContext calcContext = new CalcContext(state, new CalcStateStorage(), calcSupplier.Object, compiler);
            calcContext.ValuesKeeper = valuesKeeper.Object;

            var nodes = new ICalcNodeInfo[] { 
                new TestParameterinfo(new TestCalcNode(){NodeID=1}){Code="par1", Formula="34;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=2}){Code="par2", Formula="36+56;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=3}){Code="par3"},
                new TestParameterinfo(new TestCalcNode(){NodeID=4}){Code="par4", Formula="a=453; a+34;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=5}){Code="par5", Formula="a=453; a+34;"},
            };

            calcContext.AddParametersToCalc(from n in nodes select n.CalcNode);

            IParameterInfo parameterInfo;

            while (calcContext.NextNode())
            {
                NodeContext nodeContext = calcContext.SymbolTable.CallContext as NodeContext;

                Assert.IsNotNull(nodeContext);

                parameterInfo = nodeContext.Node.NodeInfo as IParameterInfo;

                Assert.AreEqual(String.Format("Расчёт параметра ${1}$ '{0}'", parameterInfo.Name, parameterInfo.Code), state.StateString);
                // как бы уже посчитали параметр
                calcContext.Return();
            }
        }

        [Test]
        public void Return__ChangeOperationStateText()
        {
            OperationState state = new OperationState();

            // Список ревизий
            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            // инициализируемый тестируемый объект
            CalcContext calcContext = new CalcContext(state, new CalcStateStorage(), calcSupplier.Object, compiler);

            var nodes = new ICalcNodeInfo[] { 
                new TestParameterinfo(new TestCalcNode(){NodeID=1}){Code="par1", Formula="34;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=2}){Code="par2", Formula="36+56;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=3}){Code="par3"},
                new TestParameterinfo(new TestCalcNode(){NodeID=4}){Code="par4", Formula="a=453; a+34;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=5}){Code="par5", Formula="a=453; a+34;"},
            };

            IParameterInfo parameterInfo;

            foreach (var item in nodes)
            {
                calcContext.Call(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01), new CalcNodeKey(item.CalcNode, null));
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                NodeContext nodeContext = calcContext.SymbolTable.CallContext as NodeContext;

                Assert.IsNotNull(nodeContext);

                parameterInfo = nodeContext.Node.NodeInfo as IParameterInfo;

                Assert.AreEqual(String.Format("Расчёт параметра ${1}$ '{0}'", parameterInfo.Name, parameterInfo.Code), state.StateString);
                // как бы уже посчитали параметр
                calcContext.Return();
            }
            Assert.IsNull(calcContext.SymbolTable.CallContext);
        }

        [Test]
        public void AddMessage__OperationStateAddMessage()
        {
            OperationState state = new OperationState();

            // Список ревизий
            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            var valuesKeeper = new Moq.Mock<IValuesKeeper>();

            // инициализируемый тестируемый объект
            CalcContext calcContext = new CalcContext(state, new CalcStateStorage(), calcSupplier.Object, compiler);
            calcContext.ValuesKeeper = valuesKeeper.Object;

            var nodes = new ICalcNodeInfo[] { 
                new TestParameterinfo(new TestCalcNode(){NodeID=1}){Code="par1", Formula="34;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=2}){Code="par2", Formula="36+56;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=3}){Code="par3"},
                new TestParameterinfo(new TestCalcNode(){NodeID=4}){Code="par4", Formula="a=453; a+34;"},
                new TestParameterinfo(new TestCalcNode(){NodeID=5}){Code="par5", Formula="a=453; a+34;"},
            };

            calcContext.AddParametersToCalc(from n in nodes select n.CalcNode);

            Message message;
            List<Message> messageList = new List<Message>();

            IParameterInfo parameterInfo;

            message = new CalcMessage(MessageCategory.Warning, "Проверка передачи сообщений");
            calcContext.AddMessage(message);
            messageList.Add(message);

            while (calcContext.NextNode())
            {
                NodeContext nodeContext = calcContext.SymbolTable.CallContext as NodeContext;

                Assert.IsNotNull(nodeContext);
                parameterInfo = nodeContext.Node.NodeInfo as IParameterInfo;

                message = new CalcMessage(MessageCategory.Warning, "Расчёт параметра ${0}$", parameterInfo.Code);
                calcContext.AddMessage(message);
                messageList.Add(message);
                // как бы уже посчитали параметр
                calcContext.Return();
            }

            message = new CalcMessage(MessageCategory.Warning, "Окончание расчёта");
            calcContext.AddMessage(message);
            messageList.Add(message);

            Assert.AreEqual(messageList.Count, state.messages.Count);
            for (int i = 0; i < messageList.Count; i++)
            {
                Assert.AreEqual(messageList[i].Text, state.messages[i].Text);
            }
        }

        [Test]
        public void GetIdentifier__GetCurrenCalcNodeOrCompilationNode()
        {
            var callContext1 = new Moq.Mock<ICallContext>();
            var callContext2 = new Moq.Mock<ICallContext>();
            var callContext3 = new Moq.Mock<ICallContext>();

            callContext1.Setup(c => c.CurrentNode).Returns(new CalcPosition() { NodeID = 14, CurrentPart = CalcPosition.NodePart.Formula });
            callContext2.Setup(c => c.CurrentNode).Returns(new CalcPosition() { NodeID = 24, CurrentPart = CalcPosition.NodePart.Formula });
            callContext3.Setup(c => c.CurrentNode).Returns(new CalcPosition() { NodeID = 44, CurrentPart = CalcPosition.NodePart.Formula });

            var calcSupllier = new Moq.Mock<ICalcSupplier>();

            var calcContext = new CalcContext(new OperationState(), null, calcSupllier.Object, compiler);

            CalcPosition pos;

            calcContext.Call(callContext1.Object);
            pos = calcContext.GetIdentifier();
            Assert.IsNotNull(pos);
            Assert.AreEqual(14, pos.NodeID);

            calcContext.Call(callContext2.Object);
            pos = calcContext.GetIdentifier();
            Assert.IsNotNull(pos);
            Assert.AreEqual(24, pos.NodeID);

            calcContext.Call(callContext3.Object);
            pos = calcContext.GetIdentifier();
            Assert.IsNotNull(pos);
            Assert.AreEqual(44, pos.NodeID);

            calcContext.Return();
            pos = calcContext.GetIdentifier();
            Assert.IsNotNull(pos);
            Assert.AreEqual(24, pos.NodeID);

            calcContext.Return();
            pos = calcContext.GetIdentifier();
            Assert.IsNotNull(pos);
            Assert.AreEqual(14, pos.NodeID);
            
            calcContext.Return();
        }

        [Test]
        public void Return__AddOperationStateResults()
        {
            OperationState state = new OperationState();

            // Список ревизий
            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(s => s.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>())).Returns(new RevisionInfo[] { RevisionInfo.Default });

            // инициализируемый тестируемый объект
            CalcContext calcContext = new CalcContext(state, new CalcStateStorage(), calcSupplier.Object, compiler);
            calcContext.ValuesKeeper = new ValuesKeeper(calcSupplier.Object);
            calcContext.SetTime(new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc), new DateTime(2012, 01, 03, 0, 0, 0, DateTimeKind.Utc));

            var nodes = new ICalcNodeInfo[] { 
                new TestParameterinfo(new TestCalcNode(){NodeID=1})
                {
                    Interval=Interval.Day, 
                    Code="par1", 
                    Formula="34;"
                },
                new TestParameterinfo(new TestCalcNode(){NodeID=2})
                {
                    Interval=Interval.Day, 
                    Code="par2", 
                    Formula="36+56;"
                },
                new TestParameterinfo(new TestCalcNode(){NodeID=3})
                {
                    Interval=Interval.Day, 
                    Code="par3"
                },
                new TestParameterinfo(new TestCalcNode(){NodeID=4})
                {
                    Interval=Interval.Day,
                    Code="par4", 
                    Formula="a=453; a+34;"
                },
            };

            ICalcNodeInfo param5Info = new TestParameterinfo(new TestCalcNode() { NodeID = 5 })
            {
                Interval = Interval.Day,
                Code = "par5",
                Formula = "a=453; a+34;"
            };
            calcContext.AddParametersToCalc(from n in nodes select n.CalcNode);

            IParameterInfo parameterInfo;

            Dictionary<int, SymbolValue[]> calculatedValues = new Dictionary<int, SymbolValue[]>();
            calculatedValues[1] = new SymbolValue[] { SymbolValue.CreateValue(15), SymbolValue.CreateValue(54) };
            calculatedValues[2] = new SymbolValue[] { SymbolValue.Nothing, SymbolValue.CreateValue(32) };
            calculatedValues[3] = new SymbolValue[] { SymbolValue.CreateValue(68), SymbolValue.Nothing };
            calculatedValues[4] = new SymbolValue[] { SymbolValue.Nothing, SymbolValue.Nothing };
            calculatedValues[5] = new SymbolValue[] { SymbolValue.CreateValue(79), SymbolValue.CreateValue(50) };

            Dictionary<int, NodeBackState> nodeBackStates = new Dictionary<int, NodeBackState>();
            nodeBackStates[1] = NodeBackState.Success;
            nodeBackStates[2] = NodeBackState.Middle;
            nodeBackStates[3] = NodeBackState.Middle;
            nodeBackStates[4] = NodeBackState.Failed;
            nodeBackStates[5] = NodeBackState.Success;

            Dictionary<int, TimeValueStructure> lastValues = new Dictionary<int, TimeValueStructure>();
            lastValues[1] = new TimeValueStructure(new DateTime(2012, 01, 02), 54);
            lastValues[2] = new TimeValueStructure(new DateTime(2012, 01, 02), 32);
            lastValues[3] = new TimeValueStructure(new DateTime(2012, 01, 01), 68);
            lastValues[4] = new TimeValueStructure();
            lastValues[5] = new TimeValueStructure(new DateTime(2012, 01, 02), 50);


            while (calcContext.NextNode())
            {
                NodeContext nodeContext = calcContext.SymbolTable.CallContext as NodeContext;
                Assert.IsNotNull(nodeContext);
                parameterInfo = nodeContext.Node.NodeInfo as IParameterInfo;

                calcContext.ValuesKeeper.AddCalculatedValue(parameterInfo.CalcNode, null, new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc), calculatedValues[parameterInfo.CalcNode.NodeID][0]);
                calcContext.ValuesKeeper.AddCalculatedValue(parameterInfo.CalcNode, null, new DateTime(2012, 01, 02, 0, 0, 0, DateTimeKind.Utc), calculatedValues[parameterInfo.CalcNode.NodeID][1]);

                // как бы уже посчитали параметр
                calcContext.Return();

                // проверяем результаты асинхронной операции
                Assert.Greater(state.AsyncResult.Count, 0);
                Assert.IsInstanceOf<NodeBack>(state.AsyncResult[state.AsyncResult.Count - 1]);
                NodeBack nodeBack = state.AsyncResult[state.AsyncResult.Count - 1] as NodeBack;

                Assert.AreEqual(parameterInfo.CalcNode.NodeID, nodeBack.ParameterID);
                Assert.AreEqual(nodeBackStates[parameterInfo.CalcNode.NodeID], nodeBack.State);
                Assert.AreEqual(lastValues[parameterInfo.CalcNode.NodeID].Time, nodeBack.TimeValue.Time);
                Assert.AreEqual(lastValues[parameterInfo.CalcNode.NodeID].Value, nodeBack.TimeValue.Value);
            }

            // то же для параметра не в списке на расчёт
            calcContext.Call(new DateTime(2012, 01, 01), new DateTime(2012, 01, 03), new CalcNodeKey(param5Info.CalcNode, null));

            calcContext.ValuesKeeper.AddCalculatedValue(param5Info.CalcNode, null, new DateTime(2012, 01, 01, 0, 0, 0, DateTimeKind.Utc), calculatedValues[param5Info.CalcNode.NodeID][0]);
            calcContext.ValuesKeeper.AddCalculatedValue(param5Info.CalcNode, null, new DateTime(2012, 01, 02, 0, 0, 0, DateTimeKind.Utc), calculatedValues[param5Info.CalcNode.NodeID][1]);
      
            calcContext.Return();

            // результаты асинхронной операции не должны были измениться
            Assert.AreEqual(4, state.AsyncResult.Count);
        }

        /// <summary>
        /// Тест. Защита от рекурсии
        /// </summary>
        /// <remarks>
        /// Проверяемое поведение:
        /// <br />
        /// При попытки вызвать расчёт контекста, возвращающего true на методе TheSame()
        /// сообщить об ошибке и вернуть false
        /// </remarks>
        [Test]
        public void Call_RecursiveCall_ReportError()
        {
            var calcContext = new CalcContext(new OperationState(), null, null, null);

            var callContext1 = new Moq.Mock<ICallContext>();
            var callContext2 = new Moq.Mock<ICallContext>();
            var callContext3 = new Moq.Mock<ICallContext>();
            var callContext4 = new Moq.Mock<ICallContext>();
            var callContext5 = new Moq.Mock<ICallContext>();

            // имена контекстов для сообщений
            callContext1.Setup(c => c.ToString()).Returns("callContext1");
            callContext2.Setup(c => c.ToString()).Returns("callContext2");
            callContext3.Setup(c => c.ToString()).Returns("callContext3");
            callContext4.Setup(c => c.ToString()).Returns("callContext4");
            callContext5.Setup(c => c.ToString()).Returns("callContext5");

            // для сообщений
            callContext1.Setup(c => c.CurrentNode).Returns(new CalcPosition());
            callContext2.Setup(c => c.CurrentNode).Returns(new CalcPosition());
            callContext3.Setup(c => c.CurrentNode).Returns(new CalcPosition());
            callContext4.Setup(c => c.CurrentNode).Returns(new CalcPosition());
            callContext5.Setup(c => c.CurrentNode).Returns(new CalcPosition());

            // TheSame возвращает true для самого себя
            callContext1.Setup(c => c.TheSame(callContext1.Object)).Returns(true);
            callContext2.Setup(c => c.TheSame(callContext2.Object)).Returns(true);
            callContext3.Setup(c => c.TheSame(callContext3.Object)).Returns(true);
            callContext4.Setup(c => c.TheSame(callContext4.Object)).Returns(true);
            callContext5.Setup(c => c.TheSame(callContext5.Object)).Returns(true);

            // callContext1 не может вызываться после callContext3,
            // но наоборот проблеммы нет
            callContext1.Setup(c => c.TheSame(callContext3.Object)).Returns(true);

            // callContext5 нельзя вызывать после callContext2
            callContext5.Setup(c => c.TheSame(callContext2.Object)).Returns(true);

            bool ret;
            ret = calcContext.Call(callContext1.Object);
            Assert.IsTrue(ret);
            ret = calcContext.Call(callContext2.Object);
            Assert.IsTrue(ret);
            ret = calcContext.Call(callContext3.Object);
            Assert.IsTrue(ret);
            ret = calcContext.Call(callContext4.Object);
            Assert.IsTrue(ret);
            // Попытка вызвать callContext5 после callContext2 вызывает ошибку
            ret = calcContext.Call(callContext5.Object);
            Assert.IsFalse(ret);

            // в стэк контекст не добавился
            Assert.AreSame(callContext4.Object, calcContext.SymbolTable.CallContext);

            // Проверка сообщения
            Assert.AreEqual(1, calcContext.Messages.Count);
            Assert.IsInstanceOf<CalcMessage>(calcContext.Messages[0]);
            CalcMessage message = calcContext.Messages[0] as CalcMessage;
            Assert.AreEqual(MessageCategory.Error, message.Category);
            //Assert.AreEqual("Ошибка в ходе расчёта. Нельзя вызывать callContext5 после callContext2", message.Text);
            Assert.AreEqual("Ошибка в ходе расчёта. Попытка вызывать Castle.Proxies.ICallContextProxy дважды", message.Text);

            // Попытка второй раз вызвать callContext3
            ret = calcContext.Call(callContext3.Object);
            Assert.IsFalse(ret);

            // в стэк контекст не добавился
            Assert.AreSame(callContext4.Object, calcContext.SymbolTable.CallContext);
        }

        // перенести в CalcContext
        //[Test]
        //public void Prepare_ForceCalcIsTrue_GetRawValueSaveOnlyCorrectedValues()
        //{
        //    ICalcNode calcNode = new TestCalcNode() { Name = "mamburu", NodeID = 32 };
        //    IParameterInfo parameterInfo = new TestParameterinfo(calcNode)
        //    {
        //        Calculable = true,
        //        Code = "par1",
        //        Interval = Interval.Day,
        //        Formula = "32*53;"
        //    };

        //    NodeState state = new NodeState(parameterInfo);

        //    var valueKeeper = new Moq.Mock<IValuesKeeper>();
        //    //valueKeeper.Setup(k =>
        //    //    k.GetRawValue(calcNode, null, new DateTime(2012, 01, 02)))
        //    //    .Returns(SymbolValue.CreateValue(53));

        //    var calcSupplier = new Moq.Mock<ICalcSupplier>();

        //    List<ParamValueItem> parmList = new List<ParamValueItem>();
        //    for (int i = 1; i < 32; i++)
        //    {
        //        if (i % 6 == 2)
        //            parmList.Add(new CorrectedParamValueItem(new ParamValueItem(new DateTime(2012, 01, i), Quality.Good, i * 5 - 100), i * 7 - 100));
        //        else
        //            parmList.Add(new ParamValueItem(new DateTime(2012, 01, i), Quality.Good, i * 5 - 100));
        //    }
        //    List<Message> msg = null;
        //    bool outSrv = false;
        //    calcSupplier.Setup(s =>
        //        s.GetParameterNodeRawValues(calcNode, null, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01), out msg, out outSrv))
        //        .Returns(parmList);

        //    var calcContext = new Moq.Mock<ICalcContext>();
        //    calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);
        //    calcContext.Setup(c => c.IsParameterToCalc(calcNode)).Returns(true);

        //    var symbolTable = new SymbolTable(calcContext.Object);
        //    calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

        //    symbolTable.PushSymbolScope();
        //    symbolTable.DeclareSymbol(new Variable("@ret"));

        //    NodeContext context = new NodeContext(calcSupplier.Object, state, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

        //    Assert.IsTrue(state.ForceCalc(calcContext.Object));

        //    context.Prepare(calcContext.Object);

        //    for (int i = 1; i < 32; i++)
        //    {
        //        Moq.Times times;
        //        if (i % 6 == 2)
        //            times = Moq.Times.Once();
        //        else
        //            times = Moq.Times.Never();

        //        valueKeeper.Verify(k => k.AddRawValue(calcNode, null, new DateTime(2012, 01, i), Moq.It.IsAny<DoubleValue>()), times);
        //    }
        //}

        //[Test]
        //public void Prepare_ForceCalcIsFalse_GetRawValue()
        //{
        //    ICalcNode calcNode = new TestCalcNode() { Name = "mamburu", NodeID = 32 };
        //    IParameterInfo parameterInfo = new TestParameterinfo(calcNode)
        //    {
        //        Calculable = true,
        //        Code = "par1",
        //        Interval = Interval.Day,
        //        Formula = "32*53;"
        //    };

        //    NodeState state = new NodeState(parameterInfo);

        //    var valueKeeper = new Moq.Mock<IValuesKeeper>();
        //    //valueKeeper.Setup(k =>
        //    //    k.GetRawValue(calcNode, null, new DateTime(2012, 01, 02)))
        //    //    .Returns(SymbolValue.CreateValue(53));

        //    var calcSupplier = new Moq.Mock<ICalcSupplier>();

        //    List<ParamValueItem> parmList = new List<ParamValueItem>();
        //    for (int i = 1; i < 32; i++)
        //    {
        //        if (i % 6 == 2)
        //            parmList.Add(new CorrectedParamValueItem(new ParamValueItem(new DateTime(2012, 01, i), Quality.Good, i * 5 - 100), i * 7 - 100));
        //        else
        //            parmList.Add(new ParamValueItem(new DateTime(2012, 01, i), Quality.Good, i * 5 - 100));
        //    }
        //    List<Message> msg = null;
        //    bool outSrv = false;
        //    calcSupplier.Setup(s =>
        //        s.GetParameterNodeRawValues(calcNode, null, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01), out msg, out outSrv))
        //        .Returns(parmList);

        //    var calcContext = new Moq.Mock<ICalcContext>();
        //    calcContext.Setup(c => c.ValuesKeeper).Returns(valueKeeper.Object);
        //    calcContext.Setup(c => c.IsParameterToCalc(calcNode)).Returns(false);

        //    var symbolTable = new SymbolTable(calcContext.Object);
        //    calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

        //    symbolTable.PushSymbolScope();
        //    symbolTable.DeclareSymbol(new Variable("@ret"));

        //    NodeContext context = new NodeContext(calcSupplier.Object, state, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

        //    Assert.IsFalse(state.ForceCalc(calcContext.Object));

        //    context.Prepare(calcContext.Object);

        //    for (int i = 1; i < 32; i++)
        //    {
        //        //Moq.Times times;
        //        //if (i % 6 == 2)
        //        //    times = Moq.Times.Once();
        //        //else
        //        //    times = Moq.Times.Never();

        //        valueKeeper.Verify(k => k.AddRawValue(calcNode, null, new DateTime(2012, 01, i), Moq.It.IsAny<DoubleValue>()));
        //    }
        //}
    }
}
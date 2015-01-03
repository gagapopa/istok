using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    [TestFixture]
    public class InterpreterTests
    {
        [Test]
        public void Exec__CallPrepareAndReturn()
        {
            bool passed = false;
            bool contextPreparedLefts = false;
            int instructionCount = 1;

            var calcContext = new Moq.Mock<ICalcContext>();

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            var callContext = new Moq.Mock<ICallContext>();
            callContext.Setup(c => c.NeedPrepare).Returns(() => !contextPreparedLefts);
            callContext.Setup(c => c.Prepare(Moq.It.IsAny<ICalcContext>())).Callback(() => contextPreparedLefts = true);
            callContext.Setup(c => c.Return(Moq.It.IsAny<ICalcContext>())).Callback(() => symbolTable.PopSymbolScope(callContext.Object));
            callContext.Setup(c => c.NextInstruction(Moq.It.IsAny<ICalcContext>())).Returns(() => instructionCount-- > 0);
            callContext.Setup(c => c.CurrentInstruction).Returns(new Instruction(null, Instruction.OperationCode.Return));
            callContext.Setup(c => c.Fail).Returns(false);

            symbolTable.PushSymbolScope();

            calcContext.Setup(c => c.NextNode()).Returns(() =>
            {
                if (passed)
                    return false;

                passed = true;
                symbolTable.PushSymbolScope(callContext.Object, false);
                return true;
            });

            // должно запуститься, вызвать Prepare, NextInstruction и Return
            Interpreter interpreter = new Interpreter();

            interpreter.Exec(calcContext.Object);

            Assert.AreEqual(true, passed);
            Assert.AreEqual(true, contextPreparedLefts);

            callContext.Verify(c => c.Prepare(calcContext.Object), Moq.Times.Once());
            callContext.Verify(c => c.NextInstruction(calcContext.Object), Moq.Times.Once());
            callContext.Verify(c => c.Return(calcContext.Object), Moq.Times.Once());

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Exec_ContextFailIsTrue_CallReturn()
        {
            bool passed = false;
            bool contextPreparedLefts = false;
            int instructionCount = 10;
            bool fail = false;

            var calcContext = new Moq.Mock<ICalcContext>();

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            var callContext = new Moq.Mock<ICallContext>();
            callContext.Setup(c => c.NeedPrepare).Returns(() => !contextPreparedLefts);
            callContext.Setup(c => c.Prepare(Moq.It.IsAny<ICalcContext>())).Callback(() => 
            {
                if (fail)
                    symbolTable.PopSymbolScope(callContext.Object);
                else
                    contextPreparedLefts = true;
            });
            callContext.Setup(c => c.Return(Moq.It.IsAny<ICalcContext>())).Callback(() => symbolTable.PopSymbolScope(callContext.Object));
            callContext.Setup(c => c.NextInstruction(Moq.It.IsAny<ICalcContext>())).Returns(() => instructionCount-- > 0);
            callContext.Setup(c => c.CurrentInstruction).Returns(new Instruction(null, Instruction.OperationCode.NOP)).Callback(() => fail = true);
            callContext.Setup(c => c.Fail).Returns(() => fail);

            symbolTable.PushSymbolScope();

            calcContext.Setup(c => c.NextNode()).Returns(() =>
            {
                if (passed)
                    return false;

                passed = true;
                symbolTable.PushSymbolScope(callContext.Object, false);
                return true;
            });

            // должно запуститься, вызвать Prepare, NextInstruction и Return
            Interpreter interpreter = new Interpreter();

            interpreter.Exec(calcContext.Object);

            Assert.AreEqual(true, passed);
            Assert.AreEqual(true, contextPreparedLefts);

            callContext.Verify(c => c.Prepare(calcContext.Object), Moq.Times.Exactly(2));
            callContext.Verify(c => c.NextInstruction(calcContext.Object), Moq.Times.Once());
            callContext.Verify(c => c.Return(calcContext.Object), Moq.Times.Once());

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Exec__ExecuteCode()
        {
            bool passed = false;

            // a = 15;
            // b = a * 0.45;
            // c = b / a + a % 4;
            // a *= c / b;
            var body = new Instruction[] 
            { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("a"), new Address(SymbolValue.CreateValue(15))),
                new Instruction(null, Instruction.OperationCode.Multiplication, new Address("b"), new Address("a"), new Address(SymbolValue.CreateValue(.45))),
                new Instruction(null, Instruction.OperationCode.Division, new Address("@temp0"), new Address("b"), new Address("a")),
                new Instruction(null, Instruction.OperationCode.Modulo, new Address("@temp1"), new Address("a"), new Address(SymbolValue.CreateValue(4))),
                new Instruction(null, Instruction.OperationCode.Addition, new Address("c"), new Address("@temp0"), new Address("@temp1")),
                new Instruction(null, Instruction.OperationCode.Division, new Address("@temp0"), new Address("c"), new Address("b")),
                new Instruction(null, Instruction.OperationCode.Multiplication, new Address("a"), new Address("a"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return)
            };

            var callContext = new CommonContext(null, null, new DateTime(2012, 01, 01), Interval.Day, body);

            var calcContext = new Moq.Mock<ICalcContext>();

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            calcContext.Setup(c => c.NextNode()).Returns(() =>
            {
                if (passed)
                    return false;

                passed = true;
                symbolTable.PushSymbolScope(callContext, false);
                return true;
            });
            calcContext.Setup(c => c.Return()).Callback(() => symbolTable.PopSymbolScope(callContext));

            // подготовка переменных, над которыми будут производитсяь расчёты
            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("a"));
            symbolTable.DeclareSymbol(new Variable("b"));
            symbolTable.DeclareSymbol(new Variable("c"));

            Interpreter interpreter = new Interpreter();

            interpreter.Exec(calcContext.Object);

            Assert.AreEqual(true, passed);

            Variable v;
            // проверяем значение a
            v = symbolTable.GetSymbol("a") as Variable;

            Assert.IsNotNull(v);
            Assert.IsInstanceOf<double>(v.Value.GetValue());
            Assert.AreEqual(7.666667, (double)v.Value.GetValue(), 1e-4);

            // проверяем значение b
            v = symbolTable.GetSymbol("b") as Variable;

            Assert.IsNotNull(v);
            Assert.IsInstanceOf<double>(v.Value.GetValue());
            Assert.AreEqual(6.75, v.Value.GetValue());

            // проверяем значение c
            v = symbolTable.GetSymbol("c") as Variable;

            Assert.IsNotNull(v);
            Assert.IsInstanceOf<double>(v.Value.GetValue());
            Assert.AreEqual(3.45, v.Value.GetValue());

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Exec_FunctionCall_GetCurrentFunctionRevision()
        {
            bool passed = false;

            RevisionInfo revision1 = new RevisionInfo() { ID = 1, Time = new DateTime(2011, 01, 31) };
            RevisionInfo revision2 = new RevisionInfo() { ID = 2, Time = new DateTime(2012, 01, 01) };
            RevisionInfo revision3 = new RevisionInfo() { ID = 3, Time = new DateTime(2012, 02, 01) };
            RevisedStorage<RevisionInfo> revisionStorage=new RevisedStorage<RevisionInfo>();
            revisionStorage.Set(revision1, revision1);
            revisionStorage.Set(revision2, revision2);
            revisionStorage.Set(revision3, revision3);

            var args = new CalcArgumentInfo[] 
            { 
                new CalcArgumentInfo("a", "", ParameterAccessor.In),
                new CalcArgumentInfo("b", "", ParameterAccessor.In),
                new CalcArgumentInfo("c", "", ParameterAccessor.In)
            };

            Function function0 = new TestFunction("func1", RevisionInfo.Default, args, (double[] vals) => vals[0] + vals[1] + vals[2]);
            Function function1 = new TestFunction("func1", revision1, args, (double[] vals) => vals[0] * vals[1] + vals[2]);
            Function function2 = new TestFunction("func1", revision2, args, (double[] vals) => vals[0] + vals[1] * vals[2]);
            Function function3 = new TestFunction("func1", revision3, args, (double[] vals) => vals[0] * vals[1] * vals[2]);

            // func1(a, b, c)
            var body = new Instruction[] 
            { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("a"), new Address(SymbolValue.CreateValue(15))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("b"), new Address(SymbolValue.CreateValue(5))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("c"), new Address(SymbolValue.CreateValue(25))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.CreateValue(25))),
                new Instruction(null, Instruction.OperationCode.Call, new Address("func1")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp0"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return)
            };

            var callContext = new CommonContext(null, null, new DateTime(2012, 01, 01), Interval.Day, body);            

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.GetRevision(Moq.It.IsAny<DateTime>()))
                .Returns((DateTime t) => revisionStorage.Get(t));

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            calcContext.Setup(c => c.NextNode()).Returns(() =>
            {
                if (passed)
                    return false;

                passed = true;
                symbolTable.PushSymbolScope(callContext, false);
                return true;
            });
            calcContext.Setup(c => c.Return()).Callback(() => symbolTable.PopSymbolScope(callContext));

            // подготовка переменных, над которыми будут производитсяь расчёты
            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("@ret"));
            // объявление функций
            symbolTable.DeclareFunction(function0);
            symbolTable.DeclareFunction(function1);
            symbolTable.DeclareFunction(function2);
            symbolTable.DeclareFunction(function3);
            //symbolTable.DeclareFunction(new TestFunction("func1", RevisionInfo.Default, new CalcArgumentInfo[]{

            Interpreter interpreter = new Interpreter();

            interpreter.Exec(calcContext.Object);

            Assert.AreEqual(true, passed);

            Variable v;
            // проверяем значение a
            v = symbolTable.GetSymbol("@ret") as Variable;
            
            // func1#0(15, 5, 25) = 45
            // func1#1(15, 5, 25) = 100
            // func1#2(15, 5, 25) = 140
            // func1#3(15, 5, 25) = 1875
            Assert.IsNotNull(v);
            Assert.IsInstanceOf<double>(v.Value.GetValue());
            Assert.AreEqual(140.0, v.Value.GetValue());
            //Assert.Fail();

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Exec_Jump_()
        {
            bool passed = false;

            //compiled code {
            //k = 14;
            //if (k < 10)
            //{
            //    c = k * 14 + 3;
            //}
            //else
            //{
            //    c = k * 15 + 6;
            //}
            //return c - k;
            //compiled code }
            var body = new Instruction[] 
            { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("k"), new Address(SymbolValue.CreateValue(14))),
                new Instruction(null, Instruction.OperationCode.Less, new Address("@temp0"), new Address("k"), new Address(SymbolValue.CreateValue(10))),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp0"), new Address(6)),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.Multiplication, new Address("@temp0"), new Address("k"), new Address(SymbolValue.CreateValue(14))),
                new Instruction(null, Instruction.OperationCode.Addition, new Address("c"), new Address("@temp0"), new Address(SymbolValue.CreateValue(3))),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Jump, new Address(5)),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.Multiplication, new Address("@temp0"), new Address("k"), new Address(SymbolValue.CreateValue(15))),
                new Instruction(null, Instruction.OperationCode.Addition, new Address("c"), new Address("@temp0"), new Address(SymbolValue.CreateValue(6))),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Subtraction, new Address("@temp0"), new Address("c"), new Address("k")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.Return),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return),
            };

            var callContext = new CommonContext(null, null, new DateTime(2012, 01, 01), Interval.Day, body);

            var calcContext = new Moq.Mock<ICalcContext>();

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            calcContext.Setup(c => c.NextNode()).Returns(() =>
            {
                if (passed)
                    return false;

                passed = true;
                symbolTable.PushSymbolScope(callContext, false);
                return true;
            });
            calcContext.Setup(c => c.Return()).Callback(() => symbolTable.PopSymbolScope(callContext));

            // подготовка переменных, над которыми будут производитсяь расчёты
            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("@ret"));

            Interpreter interpreter = new Interpreter();

            interpreter.Exec(calcContext.Object);

            Assert.AreEqual(true, passed);
           
            Variable v;
            // проверяем значение a
            v = symbolTable.GetSymbol("@ret") as Variable;

            Assert.IsNotNull(v);
            Assert.IsInstanceOf<double>(v.Value.GetValue());
            Assert.AreEqual(202.0, v.Value.GetValue());

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Exec__Loop_ReportError()
        {
            bool passed = false;

            //compiled code {
            //var i = 0;
            //arr = array(10);
            //while (i < 100)
            //{
            //    arr[i % 10] += i;
            //    ++i;
            //}
            //compiled code }
            var body = new Instruction[] 
            { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("i"), new Address(SymbolValue.CreateValue(0))),
                new Instruction(null, Instruction.OperationCode.ArrayDecl, new Address("@temp0"), new Address(SymbolValue.CreateValue(10))),
                new Instruction(null, Instruction.OperationCode.Move, new Address("arr"), new Address("@temp0")),
                // условие цикла
                new Instruction(null, Instruction.OperationCode.LoopEnter),
                new Instruction(null, Instruction.OperationCode.Less, new Address("@temp0"), new Address("i"), new Address(SymbolValue.CreateValue(100))),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp0"), new Address(8)),//27)),
                // тело цикла
                new Instruction(null, Instruction.OperationCode.LoopPass),
                new Instruction(null, Instruction.OperationCode.EnterLevel),

                new Instruction(null, Instruction.OperationCode.Modulo, new Address("@temp0"), new Address("i"), new Address(SymbolValue.CreateValue(10))),
                new Instruction(null, Instruction.OperationCode.Addition, new Address(new Address("arr"), new Address("@temp0")),new Address(new Address("arr"), new Address("@temp0")), new Address("i")),
                new Instruction(null, Instruction.OperationCode.IncrementPrefix, new Address("@temp0"), new Address("i")),

                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Jump, new Address(-8)), //-27)),
                new Instruction(null, Instruction.OperationCode.LoopLeave),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return)
            };

            var callContext = new CommonContext(null, null, new DateTime(2012, 01, 01), Interval.Day, body);

            var calcContext = new Moq.Mock<ICalcContext>();

            var symbolTable = new SymbolTable(calcContext.Object);

            calcContext.Setup(c => c.MaxLoopCount).Returns(91);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            // выставляем флаг у контекста для выхода из расчёта по ошибке
            calcContext.Setup(c => 
                c.AddMessage(Moq.It.Is<CalcMessage>(m => m.Category == MessageCategory.Error)))
                .Callback(() => symbolTable.CallContext.Fail = true);

            calcContext.Setup(c => c.NextNode()).Returns(() =>
            {
                if (passed)
                    return false;

                passed = true;
                symbolTable.PushSymbolScope(callContext, false);
                return true;
            });
            calcContext.Setup(c => c.Return()).Callback(() => symbolTable.PopSymbolScope(callContext));

            // подготовка переменных, над которыми будут производитсяь расчёты
            symbolTable.PushSymbolScope();
            symbolTable.DeclareSymbol(new Variable("@ret"));
            symbolTable.DeclareSymbol(new Variable("arr"));

            Interpreter interpreter = new Interpreter();

            interpreter.Exec(calcContext.Object);

            Assert.AreEqual(true, passed);

            Variable v;
            // проверяем значение a
            v = symbolTable.GetSymbol("arr") as Variable;

            Assert.IsNotNull(v);
            Assert.IsInstanceOf<ArrayValue>(v.Value);

            Object[] array = v.Value.GetValue() as Object[];
            Assert.IsNotNull(array);
            Assert.AreEqual(10, array.Length);
            Assert.AreEqual(450.0, array[0]);
            Assert.AreEqual(369.0, array[1]);
            Assert.AreEqual(378.0, array[2]);
            Assert.AreEqual(387.0, array[3]);
            Assert.AreEqual(396.0, array[4]);
            Assert.AreEqual(405.0, array[5]);
            Assert.AreEqual(414.0, array[6]);
            Assert.AreEqual(423.0, array[7]);
            Assert.AreEqual(432.0, array[8]);
            Assert.AreEqual(441.0, array[9]);

            calcContext.Verify(c =>
                c.AddMessage(Moq.It.Is<CalcMessage>(m =>
                    m.Category == MessageCategory.Error 
                    && m.Text == "Цикл был выполнен более максимольного допустимого количества раз (91) и был прерван.")));
        }

        [Test]
        public void Exec__CallDependencePrepare()
        {
            bool passed = false;
            bool contextPreparedLefts = false;
            int instructionIndex = -1;
            bool dependenceCalled = false;

            var primaryBody = new Instruction[]
            {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("a")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return)
            };
  
            var bodyByDependence = new Instruction[]
            {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("a"), new Address(SymbolValue.CreateValue(15))),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return)
            };

            CommonContext contextByDependence = new CommonContext(null, null, new DateTime(2012, 01, 01), Interval.Day, bodyByDependence);

            var primaryContext = new Moq.Mock<ICallContext>();

            var calcContext = new Moq.Mock<ICalcContext>();

            var symbolTable = new SymbolTable(calcContext.Object);

            // setups
            // calcContext
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            calcContext.Setup(c => c.NextNode()).Returns(() =>
            {
                if (passed)
                    return false;

                passed = true;
                symbolTable.PushSymbolScope(primaryContext.Object, false);
                return true;
            });
            calcContext.Setup(c => c.Call(contextByDependence))
                .Callback((ICallContext callContext) => symbolTable.PushSymbolScope(callContext, false));
            calcContext.Setup(c => c.Return()).Callback(() => symbolTable.PopSymbolScope(symbolTable.CallContext));
            // primaryCallContext
            primaryContext.Setup(c => c.NeedPrepare).Returns(() => !contextPreparedLefts);
            primaryContext.Setup(c => c.Prepare(Moq.It.IsAny<ICalcContext>()))
                .Callback((ICalcContext context) =>
                {
                    if (!dependenceCalled)
                    {
                        context.Call(contextByDependence);
                        dependenceCalled = true;
                    }
                    else
                    {
                        contextPreparedLefts = true; 
                    }
                });
            primaryContext.Setup(c => c.Return(Moq.It.IsAny<ICalcContext>())).Callback(() => symbolTable.PopSymbolScope(primaryContext.Object));
            primaryContext.Setup(c => c.NextInstruction(Moq.It.IsAny<ICalcContext>())).Returns(() => ++instructionIndex < primaryBody.Length);
            primaryContext.Setup(c => c.CurrentInstruction).Returns(() => primaryBody[instructionIndex]); //new Instruction(null, Instruction.OperationCode.Return));
            primaryContext.Setup(c => c.Fail).Returns(false);

            // test matter
            Interpreter interpreter = new Interpreter();

            symbolTable.DeclareSymbol(new Variable("@ret"));
            symbolTable.DeclareSymbol(new Variable("a", SymbolValue.CreateValue(.5)));

            interpreter.Exec(calcContext.Object);

            Assert.AreEqual(true, passed);
            Assert.AreEqual(true, contextPreparedLefts);
            Assert.AreEqual(true, dependenceCalled);

            Variable v;
            // проверяем значение a
            v = symbolTable.GetSymbol("@ret") as Variable;

            Assert.IsNotNull(v);
            Assert.IsInstanceOf<double>(v.Value.GetValue());
            Assert.AreEqual(15.0, v.Value.GetValue());

            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }
    }
}

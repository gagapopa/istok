using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    [TestFixture]
    public class SymbolTableTests
    {
        [Test]
        public void DeclareSymbol_IsBottom_()
        {
            var callContext = new Moq.Mock<ICallContext>();

            SymbolTable table = new SymbolTable(null);

            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("x"));
            table.DeclareSymbol(new Variable("y"));
            table.DeclareSymbol(new Variable("z"));
            table.PushSymbolScope();
            table.PushSymbolScope(callContext.Object);
            table.DeclareSymbol(new Variable("x"));
            table.DeclareSymbol(new Variable("y"));
            table.DeclareSymbol(new Variable("z"));
            table.PushSymbolScope(); // объявленная переменная должна оказаться здесь
            table.DeclareSymbol(new Variable("x"));
            table.DeclareSymbol(new Variable("y"));
            table.DeclareSymbol(new Variable("z"));
            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("x"));
            table.DeclareSymbol(new Variable("y"));
            table.DeclareSymbol(new Variable("z"));
            table.PushSymbolScope();

            Variable expectedVariable = new Variable("foo");

            table.DeclareSymbol(expectedVariable, false, true);

            table.PopSymbolScope();
            table.PopSymbolScope();

            Assert.AreSame(expectedVariable, table.GetSupSymbol("foo"));
        }

        [Test]
        public void DeclareSymbol_DublicateDeclare_AddMessage()
        {
            var calcContext = new Moq.Mock<ICalcContext>();

            SymbolTable table = new SymbolTable(calcContext.Object);

            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));

            calcContext.Verify(c => c.AddMessage(Moq.It.Is<CalcMessage>(m => m.Category == MessageCategory.Error && m.Text.Equals("Дублированное объявление символа 'foo'"))), Moq.Times.Exactly(1));
            calcContext.Verify(c => c.AddMessage(Moq.It.Is<CalcMessage>(m => m.Category == MessageCategory.Error && m.Text.Equals("Дублированное объявление символа 'bar'"))), Moq.Times.Exactly(1));
        }

        [Test]
        public void GetSymbol_SameNameVariables_ReturnTop()
        {
            SymbolTable table = new SymbolTable(null);

            Variable topVar = new Variable("x");
            Variable bottomVar = new Variable("x");

            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(bottomVar);
            table.DeclareSymbol(new Variable("y"));
            table.PushSymbolScope();
            table.DeclareSymbol(topVar);

            Assert.AreSame(topVar, table.GetSymbol("x"));
            Assert.AreNotSame(bottomVar, table.GetSymbol("x"));
        }

        [Test]
        public void GetSymbol_VariableUnderStackFrame_ReturnNull()
        {
            var calcContext = new Moq.Mock<ICalcContext>();
            var callContext = new Moq.Mock<ICallContext>();

            SymbolTable table = new SymbolTable(calcContext.Object);

            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("x"));
            table.PushSymbolScope(callContext.Object);
            table.DeclareSymbol(new Variable("y"));

            Assert.IsNull(table.GetSymbol("foo"));
            Assert.IsNull(table.GetSymbol("bar"));
            Assert.IsNotNull(table.GetSymbol("x"));
            Assert.IsNotNull(table.GetSymbol("y"));
        }

        [Test]
        public void GetSymbol_VariableInGlobalScope_ReturnIfNotFindedInStack()
        {
            var callContext = new Moq.Mock<ICallContext>();

            SymbolTable table = new SymbolTable(null);

            Variable globalFoo = new Variable("foo");

            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("x"));
            table.PushSymbolScope(callContext.Object);
            table.DeclareSymbol(new Variable("y"));
            table.DeclareSymbol(globalFoo, true, false);

            Assert.AreSame(globalFoo, table.GetSymbol("foo"));
            Assert.IsNull(table.GetSymbol("bar"));
            Assert.IsNotNull(table.GetSymbol("y"));
        }

        [Test]
        public void GetSumbol_SkipTopFrame_ReturnBelowVariable()
        {
            var callContext = new Moq.Mock<ICallContext>();

            SymbolTable table = new SymbolTable(null);

            Variable topFoo = new Variable("foo");
            Variable nextFoo = new Variable("foo");

            table.DeclareSymbol(new Variable("y"));
            table.DeclareSymbol(new Variable("bar"));
            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("x"));
            table.PushSymbolScope(callContext.Object);
            table.DeclareSymbol(nextFoo);
            table.PushSymbolScope();
            table.DeclareSymbol(topFoo);

            Assert.AreSame(nextFoo, table.GetSymbol("foo", true));
            Assert.AreSame(topFoo, table.GetSymbol("foo", false));
        }

        [Test]
        public void GetSupSymbol()
        {
            SymbolTable table = new SymbolTable(null);

            Variable topVar = new Variable("x");
            Variable bottomVar = new Variable("x");

            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(bottomVar);
            table.DeclareSymbol(new Variable("y"));
            table.PushSymbolScope();
            table.DeclareSymbol(topVar);

            Assert.AreSame(topVar, table.GetSupSymbol("x"));
            Assert.IsNull(table.GetSupSymbol("y"));
        }

        [Test]
        public void GetStackFrameSymbol()
        {
            var callContext = new Moq.Mock<ICallContext>();

            SymbolTable table = new SymbolTable(null);

            Variable stackFrameVariable = new Variable("foo");
            Variable topVariable = new Variable("foo");

            // Переменные на дне стека
            table.DeclareSymbol(new Variable("y"));
            table.DeclareSymbol(new Variable("z"));
            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("x"));
            // стэк-фрэйм 
            table.PushSymbolScope();
            table.PushSymbolScope(callContext.Object);
            table.DeclareSymbol(stackFrameVariable);
            table.DeclareSymbol(new Variable("x"));
            //Переменные за стэк-фреймом
            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(topVariable);

            Assert.AreEqual(stackFrameVariable, table.GetStackFrameSymbol("foo"));
            Assert.AreNotEqual(topVariable, table.GetStackFrameSymbol("foo"));
        }

        [Test]
        public void PopSymbolScope__RemoveAllScopeUntilArg()
        {
            var callContext1 = new Moq.Mock<ICallContext>();
            var callContext2 = new Moq.Mock<ICallContext>();

            SymbolTable table = new SymbolTable(null);

            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));
            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));
            table.PushSymbolScope();
            table.PushSymbolScope(callContext1.Object);
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));
            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));
            table.PushSymbolScope();
            table.PushSymbolScope(callContext2.Object);
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));

            table.PopSymbolScope(callContext1.Object);

            Assert.IsNull(table.CallContext);
            Assert.AreEqual(3, table.GetDeep());
        }

        [Test]
        public void PopSymbolScope_TableNonContainsArg_NothingDo()
        {
            var callContext1 = new Moq.Mock<ICallContext>();
            var callContext2 = new Moq.Mock<ICallContext>();

            SymbolTable table = new SymbolTable(null);

            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));
            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));
            table.PushSymbolScope();
            table.PushSymbolScope(callContext1.Object);
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));
            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));
            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));

            table.PopSymbolScope(callContext2.Object);

            Assert.AreSame(callContext1.Object, table.CallContext);
            Assert.AreEqual(6, table.GetDeep());
        }

        [Test]
        public void PushSymbolScope_DifferentIsolated_UseOrNotUseTopFrameAsStackFrame()
        { 
            var callContext1 = new Moq.Mock<ICallContext>();
            var callContext2 = new Moq.Mock<ICallContext>();

            var calcContext = new Moq.Mock<ICalcContext>();

            SymbolTable table = new SymbolTable(calcContext.Object);

            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));
            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));

            table.PushSymbolScope(callContext1.Object, true);
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));
            // при изолированном Push'е сообщений не будет
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());

            table.PushSymbolScope();
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));
            table.PushSymbolScope(callContext2.Object);
            table.DeclareSymbol(new Variable("foo"));
            table.DeclareSymbol(new Variable("bar"));
            table.DeclareSymbol(new Variable("y"));

            // при не изолированном Push'е должны быть сообщения об ошибке
            calcContext.Verify(c => c.AddMessage(Moq.It.Is<CalcMessage>(m =>
                m.Category == MessageCategory.Error && m.Text == "Дублированное объявление символа 'foo'")));
            calcContext.Verify(c => c.AddMessage(Moq.It.Is<CalcMessage>(m =>
                m.Category == MessageCategory.Error && m.Text == "Дублированное объявление символа 'bar'")));
            calcContext.Verify(c => c.AddMessage(Moq.It.Is<CalcMessage>(m =>
                m.Category == MessageCategory.Error && m.Text == "Дублированное объявление символа 'y'")));
        }

        [Test]
        public void GetFunction__()
        {
            SymbolTable table = new SymbolTable(null);

            RevisionInfo revision2 = new RevisionInfo { ID = 2, Time = new DateTime(2012, 01, 01) };
            RevisionInfo revision56 = new RevisionInfo { ID = 56, Time = new DateTime(2012, 06, 01) };
            RevisionInfo revision1 = new RevisionInfo { ID = 1, Time = new DateTime(2011, 09, 15) };

            TestFunction function2 = new TestFunction("func", revision2, new CalcArgumentInfo[] 
            {
                new CalcArgumentInfo("a", "", ParameterAccessor.In) 
            });
            TestFunction function56 = new TestFunction("func", revision56, new CalcArgumentInfo[] 
            { 
                new CalcArgumentInfo("a", "", ParameterAccessor.In),
                new CalcArgumentInfo("b", "", ParameterAccessor.In),
                new CalcArgumentInfo("c", "", ParameterAccessor.In),
            });
            TestFunction function1 = new TestFunction("func", revision1, new CalcArgumentInfo[]             
            { 
                new CalcArgumentInfo("a", "", ParameterAccessor.In),
                new CalcArgumentInfo("d", "", ParameterAccessor.In),
                new CalcArgumentInfo("b", "", ParameterAccessor.In),
            });


            table.DeclareFunction(function2);
            table.DeclareFunction(function56);
            table.DeclareFunction(function1);

            Assert.AreSame(function1, table.GetFunction(revision1, "func"));
            Assert.AreSame(function2, table.GetFunction(revision2, "func"));
            Assert.AreSame(function56, table.GetFunction(revision56, "func"));

        }
    }
}

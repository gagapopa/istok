using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    [TestFixture]
    public class CompilerTests
    {
        public static double TestFunction(double a, double b, double c)
        {
            return a * (b + c) / (b - c);
        }

        [Test]
        public void Compile_CompoundStatementAndVariableDeclaration()
        {
            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });
            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            Compiler compiler = new Compiler();

            String formula = @"
k=15;
a=5;
{
var a=34;
k+=a/4-a*4+k%a;
}
{
k+=a*(k+6);
}
return k;
";

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formula, null);

            var expectedCode = new Instruction[] { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                // объявление временных переменных
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp1")),

                new Instruction(null, Instruction.OperationCode.Move, new Address("k"), new Address(SymbolValue.CreateValue(15))),
                new Instruction(null, Instruction.OperationCode.Move, new Address("a"), new Address(SymbolValue.CreateValue(5))),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("a"), new Address(SymbolValue.CreateValue(34))),
                new Instruction(null, Instruction.OperationCode.Division, new Address("@temp0"), new Address("a"), new Address(SymbolValue.CreateValue(4))),
                new Instruction(null, Instruction.OperationCode.Multiplication, new Address("@temp1"), new Address("a"), new Address(SymbolValue.CreateValue(4))),
                new Instruction(null, Instruction.OperationCode.Subtraction, new Address("@temp0"), new Address("@temp0"), new Address("@temp1")),
                new Instruction(null, Instruction.OperationCode.Modulo, new Address("@temp1"), new Address("k"), new Address("a")),
                new Instruction(null, Instruction.OperationCode.Addition, new Address("@temp0"), new Address("@temp0"), new Address("@temp1")),
                new Instruction(null, Instruction.OperationCode.Addition, new Address("k"), new Address("k"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.Addition, new Address("@temp0"), new Address("k"), new Address(SymbolValue.CreateValue(6))),
                new Instruction(null, Instruction.OperationCode.Multiplication, new Address("@temp0"), new Address("a"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.Addition, new Address("k"), new Address("k"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("k")),
                new Instruction(null, Instruction.OperationCode.Return),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return),
            };

            CodeAssert.AreEqual(expectedCode, code);

            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Compile_EvaluateConstExpression()
        {
            var calcContext = new Moq.Mock<ICalcContext>();
            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            String formula = @"
v=4;
v*(36+89*5)-47*85+132/3;
";

            Compiler compiler = new Compiler();

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formula, null);

            var expectedCode = new Instruction[] { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                // объявление временных переменных
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("v"), new Address(SymbolValue.CreateValue(4))),
                new Instruction(null, Instruction.OperationCode.Multiplication, new Address("@temp0"), new Address("v"), new Address(SymbolValue.CreateValue(36+89*5))),
                new Instruction(null, Instruction.OperationCode.Subtraction, new Address("@temp0"), new Address("@temp0"), new Address(SymbolValue.CreateValue(47*85))),
                new Instruction(null, Instruction.OperationCode.Addition, new Address("@temp0"), new Address("@temp0"), new Address(SymbolValue.CreateValue(132/3))),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return),
            };

            CodeAssert.AreEqual(expectedCode, code);
    
            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Compile_IfElse()
        {
            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            Compiler compiler = new Compiler();

            String formulaText = @"k=14;
if(k<10)
{
    c=k*14 + 3;
}
else
{
    c=k*15 + 6;
}

return c - k;";

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formulaText, null);

            var expectedCode = new Instruction[] { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                // объявление временных переменных
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")),
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

            CodeAssert.AreEqual(expectedCode, code);

            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Compile_Loop()
        {
            Function arrayFunction = new MacrosFunction("array", new CalcArgumentInfo[] { new CalcArgumentInfo("N", "", ParameterAccessor.In) }, "", "",
                new Instruction[]{
                    new Instruction(null, Instruction.OperationCode.ArrayDecl, new Address(CalcContext.ReturnVariableName), new Address("N"))
                });

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            symbolTable.DeclareFunction(arrayFunction);

            Compiler compiler = new Compiler();

            String formula = @"
var i=0;
arr=array(10);
while(i<100)
{
if(i==31) { arr[i%10]=31; continue; }
arr[i%10]+=i;
++i;
if(arr[i%10]<0){ arr[i%10]=0; break; }
}
";

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formula, null);

            var expectedCode = new Instruction[] { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                // объявление временных переменных
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")),
                
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("i"), new Address(SymbolValue.CreateValue(0))),
                new Instruction(null, Instruction.OperationCode.ArrayDecl, new Address("@temp0"), new Address(SymbolValue.CreateValue(10))),
                new Instruction(null, Instruction.OperationCode.Move, new Address("arr"), new Address("@temp0")),
                // условие цикла
                new Instruction(null, Instruction.OperationCode.LoopEnter),
                new Instruction(null, Instruction.OperationCode.Less, new Address("@temp0"), new Address("i"), new Address(SymbolValue.CreateValue(100))),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp0"), new Address(27)),
                // тело цикла
                new Instruction(null, Instruction.OperationCode.LoopPass),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                // первый if
                new Instruction(null, Instruction.OperationCode.Equal, new Address("@temp0"), new Address("i"), new Address(SymbolValue.CreateValue(31))),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp0"), new Address(8)),
                // then {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.Modulo, new Address("@temp0"), new Address("i"), new Address(SymbolValue.CreateValue(10))),
                new Instruction(null, Instruction.OperationCode.Move, new Address(new Address("arr"), new Address("@temp0")), new Address(SymbolValue.CreateValue(31))), 
                // continue
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Jump, new Address(-11)),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                // then }
                new Instruction(null, Instruction.OperationCode.Modulo, new Address("@temp0"), new Address("i"), new Address(SymbolValue.CreateValue(10))),
                new Instruction(null, Instruction.OperationCode.Addition, new Address(new Address("arr"), new Address("@temp0")), new Address(new Address("arr"), new Address("@temp0")), new Address("i")),
                new Instruction(null, Instruction.OperationCode.IncrementPrefix, new Address("@temp0"), new Address("i")),
                // начинается if
                new Instruction(null, Instruction.OperationCode.Modulo, new Address("@temp0"), new Address("i"), new Address(SymbolValue.CreateValue(10))),
                new Instruction(null, Instruction.OperationCode.Less, new Address("@temp0"), new Address(new Address("arr"), new Address("@temp0")), new Address(SymbolValue.CreateValue(0))),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp0"), new Address(8)),
                // then {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.Modulo, new Address("@temp0"), new Address("i"), new Address(SymbolValue.CreateValue(10))),
                new Instruction(null, Instruction.OperationCode.Move, new Address(new Address("arr"), new Address("@temp0")), new Address(SymbolValue.CreateValue(0))), 
                // break
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Jump, new Address(4)),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                // then }

                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Jump, new Address(-27)),
                new Instruction(null, Instruction.OperationCode.LoopLeave),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return),
            };

            CodeAssert.AreEqual(expectedCode, code);

            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Compile_VariablesDeclare()
        {
            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });
            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            Compiler compiler = new Compiler();

            String doubleDeclarationFormula = @"
k=13;
var k=5;
";
            String useOfAnasignmentVariableFormula = "k=a+4;";

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, doubleDeclarationFormula, null);

            calcContext.Verify(c =>
                c.AddMessage(Moq.It.Is<CalcMessage>(m => m.Category == MessageCategory.Error && m.Text == "Дублированное объявление символа 'k'")));

            code = compiler.Compile(calcContext.Object, RevisionInfo.Default, useOfAnasignmentVariableFormula, null);

            calcContext.Verify(c =>
                c.AddMessage(Moq.It.Is<CalcMessage>(m => m.Category == MessageCategory.Error && m.Text == "Использование необъявленной переменной 'a'")));
        }

        [Test]
        public void Compile_LogicalANDOR()
        {
            var calcContext = new Moq.Mock<ICalcContext>();
            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            Compiler compiler = new Compiler();

            String formula = @"
a=32;
b=23;
c=24;
// обычные И и ИЛИ
if(a<b && a<c) return a;
if(b>a || b>c) return b;

// всегда ложное выражение
if(32>89 && b>6) return 42;

// всегда истенное выражение
if(b>a || 6>2) a+=b;

// выражения сокращаются до одной части
if(32<89 && b>6) return 42;
if(b>a || 6<2) return 34;

// сложно выражение
if(a>b && (b<c || c<a) || ((a/b > b/c) && (b/a < c/b)))
    return a+c;
";

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formula, null);

            var expectedCode = new Instruction[] { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                // объявление временных переменных
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp1")),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp2")),

                new Instruction(null, Instruction.OperationCode.Move, new Address("a"), new Address(SymbolValue.CreateValue(32))),
                new Instruction(null, Instruction.OperationCode.Move, new Address("b"), new Address(SymbolValue.CreateValue(23))),
                new Instruction(null, Instruction.OperationCode.Move, new Address("c"), new Address(SymbolValue.CreateValue(24))),
                // первый if a<b && a<c
                new Instruction(null, Instruction.OperationCode.Less, new Address("@temp0"), new Address("a"), new Address("b")),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp0"), new Address(2)),
                new Instruction(null, Instruction.OperationCode.Less, new Address("@temp0"), new Address("a"), new Address("c")),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp0"), new Address(3)),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("a")),
                new Instruction(null, Instruction.OperationCode.Return),
                // второй if b>a || b>c
                new Instruction(null, Instruction.OperationCode.Greater, new Address("@temp0"), new Address("b"), new Address("a")),
                new Instruction(null, Instruction.OperationCode.JumpNZ, new Address("@temp0"), new Address(2)),
                new Instruction(null, Instruction.OperationCode.Greater, new Address("@temp0"), new Address("b"), new Address("c")),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp0"), new Address(3)),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("b")),
                new Instruction(null, Instruction.OperationCode.Return),
                // третьего if'а нет  32>89 && b>6
                // четвертый if всегда истенен b>a || 6>2
                new Instruction(null, Instruction.OperationCode.Addition, new Address("a"), new Address("a"), new Address("b")),
                // пятый и шестой if'ы упрощенны 32<89 && b>6
                new Instruction(null, Instruction.OperationCode.Greater, new Address("@temp0"), new Address("b"), new Address(SymbolValue.CreateValue(6))),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp0"), new Address(3)),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address(SymbolValue.CreateValue(42))),
                new Instruction(null, Instruction.OperationCode.Return),
                // b>a || 6<2
                new Instruction(null, Instruction.OperationCode.Greater, new Address("@temp0"), new Address("b"), new Address("a")),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp0"), new Address(3)),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address(SymbolValue.CreateValue(34))),
                new Instruction(null, Instruction.OperationCode.Return),
                // седьмой (сложный) if a>b && (b<c || c<a) || ((a/b > b/c) && (b/a < c/b))

                new Instruction(null, Instruction.OperationCode.Greater, new Address("@temp0"), new Address("a"), new Address("b")),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp0"), new Address(4)),
                new Instruction(null, Instruction.OperationCode.Less, new Address("@temp0"), new Address("b"), new Address("c")),
                new Instruction(null, Instruction.OperationCode.JumpNZ, new Address("@temp0"), new Address(2)), // +2 
                new Instruction(null, Instruction.OperationCode.Less, new Address("@temp0"), new Address("c"), new Address("a")),
                new Instruction(null, Instruction.OperationCode.JumpNZ, new Address("@temp0"), new Address(8)),
                new Instruction(null, Instruction.OperationCode.Division, new Address("@temp1"), new Address("a"), new Address("b")),
                new Instruction(null, Instruction.OperationCode.Division, new Address("@temp2"), new Address("b"), new Address("c")),
                new Instruction(null, Instruction.OperationCode.Greater, new Address("@temp0"), new Address("@temp1"), new Address("@temp2")),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp0"), new Address(4)), // +4
                
                new Instruction(null, Instruction.OperationCode.Division, new Address("@temp1"), new Address("b"), new Address("a")),
                new Instruction(null, Instruction.OperationCode.Division, new Address("@temp2"), new Address("c"), new Address("b")),
                new Instruction(null, Instruction.OperationCode.Less, new Address("@temp0"), new Address("@temp1"), new Address("@temp2")),
                new Instruction(null, Instruction.OperationCode.JumpZ, new Address("@temp0"), new Address(4)),
                new Instruction(null, Instruction.OperationCode.Addition, new Address("@temp0"), new Address("a"), new Address("c")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.Return),

                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return)
            };

            CodeAssert.AreEqual(expectedCode, code);

            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Compile_FunctionCall()
        {
            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            Compiler compiler = new Compiler();

            Function testFunction = new StandardFunction("func", typeof(CompilerTests).GetMethod("TestFunction", new Type[] { typeof(double), typeof(double), typeof(double) }), "", "");

            symbolTable.DeclareFunction(testFunction);

            String formulaText = "func(43, 54.5, 453.8);";

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formulaText, null);

            var expectedCode = new Instruction[] { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")/*, new Address(SymbolValue.Nothing)*/),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("a"), new Address(SymbolValue.CreateValue(43))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("b"), new Address(SymbolValue.CreateValue(54.5))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("c"), new Address(SymbolValue.CreateValue(453.8))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.Nothing)),
                new Instruction(null, Instruction.OperationCode.Call, new Address("func")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp0"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return),
            };

            CodeAssert.AreEqual(expectedCode, code);

            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Compile_FunctionCallDifferentRevision()
        {
            RevisionInfo secondRevision = new RevisionInfo() { ID = 1, Time = new DateTime(2012, 02, 01) };

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);

            Function testDefaultFunction = new TestFunction("func", RevisionInfo.Default, new CalcArgumentInfo[]{
                new CalcArgumentInfo("a", "", ParameterAccessor.In),
                new CalcArgumentInfo("b", "", ParameterAccessor.In),
                new CalcArgumentInfo("c", "", ParameterAccessor.In)
            });

            Function testSecondFunction = new TestFunction("func", secondRevision, new CalcArgumentInfo[]{
                new CalcArgumentInfo("c", "", ParameterAccessor.In),
                new CalcArgumentInfo("b", "", ParameterAccessor.In),
                new CalcArgumentInfo("a", "", ParameterAccessor.In)
            });

            symbolTable.DeclareFunction(testDefaultFunction);
            symbolTable.DeclareFunction(testSecondFunction);

            Compiler compiler = new Compiler();

            String formulaText = "func(43, 54.5, 453.8);";

            var codeDefault = compiler.Compile(calcContext.Object, RevisionInfo.Default, formulaText, null);

            var expectedCode = new Instruction[] {         
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")/*, new Address(SymbolValue.Nothing)*/),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("a"), new Address(SymbolValue.CreateValue(43))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("b"), new Address(SymbolValue.CreateValue(54.5))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("c"), new Address(SymbolValue.CreateValue(453.8))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.Nothing)),
                new Instruction(null, Instruction.OperationCode.Call, new Address("func")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp0"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return),
            };

            CodeAssert.AreEqual(expectedCode, codeDefault);

            var codeSecond = compiler.Compile(calcContext.Object, secondRevision, formulaText, null);

            expectedCode = new Instruction[] {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")/*, new Address(SymbolValue.Nothing)*/),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("c"), new Address(SymbolValue.CreateValue(43))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("b"), new Address(SymbolValue.CreateValue(54.5))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("a"), new Address(SymbolValue.CreateValue(453.8))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.Nothing)),
                new Instruction(null, Instruction.OperationCode.Call, new Address("func")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp0"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return),
            };

            CodeAssert.AreEqual(expectedCode, codeSecond);

            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Compile_ParameterNotExist_ReportError()
        {
            ICalcNode optimization = new TestCalcNode() { NodeID = 1 };
            ICalcNode parameter = new TestCalcNode() { NodeID = 4 };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimization)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[] 
                { 
                    new TestOptimizationArgument() { Name = "a" },
                    new TestOptimizationArgument() { Name = "b" },
                    new TestOptimizationArgument() { Name = "c" },
                }
            };
            IParameterInfo parameterInfo = new TestParameterinfo(parameter)
            {
                Code = "par1",
                Interval = Interval.Day,
                Optimization = optimizationInfo
            };

            ICalcState optimizationState = new OptimizationState(null, optimizationInfo, optimizationInfo.Revision);
            ICalcState parameterState = new NodeState(parameterInfo, parameterInfo.Revision);

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), "par1")).Returns(parameterInfo);
            calcContext.Setup(c => c.GetState(optimization, Moq.It.IsAny<RevisionInfo>())).Returns(optimizationState);
            calcContext.Setup(c => c.GetState(parameter, Moq.It.IsAny<RevisionInfo>())).Returns(parameterState);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            symbolTable.DeclareFunction(new ParameterFunction(null, "first", CalcAggregation.First, "", ""));

            String formula = @"$par1$(4, 5, $par2$);";

            Compiler compiler = new Compiler();

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formula, null);

            calcContext.Verify(c => 
                c.AddMessage(Moq.It.Is<CalcMessage>(m => 
                    m.Category == MessageCategory.CriticalError
                    && m.Text.Equals("Параметр $par2$ не найден"))));
        }

        [Test]
        public void Compile_ParameterAggregationExplicit()
        {
            // используемые в формуле параметры
            ICalcNode param1 = new TestCalcNode() { NodeID = 1, Name = "par1" };
            ICalcNode param2 = new TestCalcNode() { NodeID = 2, Name = "par2" };

            IParameterInfo param1Info = new TestParameterinfo(param1)
            {
                Code = "par1",
                Interval = Interval.Hour
            };
            IParameterInfo param2Info = new TestParameterinfo(param2)
            {
                Code = "par2",
                Interval = Interval.Hour
            };

            CalcState param1State = new NodeState(param1Info, param1Info.Revision);
            CalcState param2State = new NodeState(param2Info, param2Info.Revision);

            // контекст расчёта
            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), "par1")).Returns(param1Info);
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), "par2")).Returns(param2Info);
            calcContext.Setup(c => c.GetState(param1, Moq.It.IsAny<RevisionInfo>())).Returns(param1State);
            calcContext.Setup(c => c.GetState(param2, Moq.It.IsAny<RevisionInfo>())).Returns(param2State);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            // функции агрегации
            symbolTable.DeclareFunction(new ParameterFunction(null, "sum", CalcAggregation.Sum, "", ""));
            symbolTable.DeclareFunction(new ParameterFunction(null, "first", CalcAggregation.First, "", ""));

            String formula = "sum($par1$) + sum($par2$);";

            Compiler compiler = new Compiler();

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formula, null);

            var expectedCode = new Instruction[] { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                // объявление временных переменных
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp1")),
                // sum($par1$)
                //new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")/*, new Address(SymbolValue.Nothing)*/),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@node"), new Address( Address.AddressType.Parameter){ SymbolName = "par1" }),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tau"), new Address(SymbolValue.CreateValue(0))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tautill"), new Address(SymbolValue.CreateValue("@tau"))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.Nothing)),
                new Instruction(null, Instruction.OperationCode.Call, new Address("sum")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp0"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                // sum($par2$)
                //new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp1")/*, new Address(SymbolValue.Nothing)*/),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@node"), new Address( Address.AddressType.Parameter){ SymbolName = "par2" }),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tau"), new Address(SymbolValue.CreateValue(0))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tautill"), new Address(SymbolValue.CreateValue("@tau"))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.Nothing)),
                new Instruction(null, Instruction.OperationCode.Call, new Address("sum")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp1"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Addition, new Address("@temp0"), new Address("@temp0"), new Address("@temp1")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return),
            };

            CodeAssert.AreEqual(expectedCode, code);

            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Compile_ParameterAggregationImplicit()
        {
            // используемые в формуле параметры
            ICalcNode param1 = new TestCalcNode() { NodeID = 1, Name = "par1" };
            ICalcNode param2 = new TestCalcNode() { NodeID = 2, Name = "par2" };

            IParameterInfo param1Info = new TestParameterinfo(param1)
            {
                Code = "par1",
                Interval = Interval.Hour
            };
            IParameterInfo param2Info = new TestParameterinfo(param2)
            {
                Code = "par2",
                Interval = Interval.Hour
            };

            CalcState param1State = new NodeState(param1Info, param1Info.Revision);
            CalcState param2State = new NodeState(param2Info, param2Info.Revision);

            // контекст расчёта
            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), "par1")).Returns(param1Info);
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), "par2")).Returns(param2Info);
            calcContext.Setup(c => c.GetState(param1, Moq.It.IsAny<RevisionInfo>())).Returns(param1State);
            calcContext.Setup(c => c.GetState(param2, Moq.It.IsAny<RevisionInfo>())).Returns(param2State);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            // функции агрегации
            symbolTable.DeclareFunction(new ParameterFunction(null, "sum", CalcAggregation.Sum, "", ""));
            symbolTable.DeclareFunction(new ParameterFunction(null, "first", CalcAggregation.First, "", ""));

            String formula = "p=$par1$; p + $par2$;";

            Compiler compiler = new Compiler();

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formula, null);

            var expectedCode = new Instruction[] { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                // sum($par1$)
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@node"), new Address( Address.AddressType.Parameter){ SymbolName = "par1" }),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tau"), new Address(SymbolValue.CreateValue(0))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tautill"), new Address(SymbolValue.CreateValue("@tau"))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.Nothing)),
                new Instruction(null, Instruction.OperationCode.Call, new Address("first")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp0"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("p"), new Address("@temp0")),
                // sum($par2$)
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@node"), new Address( Address.AddressType.Parameter){ SymbolName = "par2" }),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tau"), new Address(SymbolValue.CreateValue(0))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tautill"), new Address(SymbolValue.CreateValue("@tau"))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.Nothing)),
                new Instruction(null, Instruction.OperationCode.Call, new Address("first")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp0"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Addition, new Address("@temp0"), new Address("p"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return),
            };

            CodeAssert.AreEqual(expectedCode, code);

            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Compile_ParameterWeightAggregation()
        {
            ICalcNode optimization1 = new TestCalcNode() { NodeID = 1 };
            ICalcNode optimization2 = new TestCalcNode() { NodeID = 3 };

            ICalcNode parameter1 = new TestCalcNode() { NodeID = 4 };
            ICalcNode parameter2 = new TestCalcNode() { NodeID = 7 };

            IOptimizationInfo optimization1Info = new TestOptimizationInfo(optimization1)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[] 
                {
                    new TestOptimizationArgument() { Name = "a" },
                    new TestOptimizationArgument() { Name = "b" },
                    new TestOptimizationArgument() { Name = "c" }
                }
            };
            IOptimizationInfo optimization2Info = new TestOptimizationInfo(optimization2)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[] 
                {
                    new TestOptimizationArgument() { Name = "d" },
                    new TestOptimizationArgument() { Name = "e" }
                }
            };

            IParameterInfo parameter1Info = new TestParameterinfo(parameter1)
            {
                Code = "par1",
                Interval = Interval.Hour,
                Optimization = optimization1Info
            };
            IParameterInfo parameter2Info = new TestParameterinfo(parameter2)
            {
                Code = "par2",
                Interval = Interval.Hour,
                Optimization = optimization2Info
            };

            ICalcState optimization1State = new OptimizationState(null, optimization1Info, optimization1Info.Revision);
            ICalcState optimization2State = new OptimizationState(null, optimization2Info, optimization2Info.Revision);

            ICalcState parameter1State = new NodeState(parameter1Info, parameter1Info.Revision);
            ICalcState parameter2State = new NodeState(parameter2Info, parameter2Info.Revision);

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), "par1")).Returns(parameter1Info);
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), "par2")).Returns(parameter2Info);
            calcContext.Setup(c => c.GetState(parameter1, Moq.It.IsAny<RevisionInfo>())).Returns(parameter1State);
            calcContext.Setup(c => c.GetState(parameter2, Moq.It.IsAny<RevisionInfo>())).Returns(parameter2State);
            calcContext.Setup(c => c.GetState(optimization1, Moq.It.IsAny<RevisionInfo>())).Returns(optimization1State);
            calcContext.Setup(c => c.GetState(optimization2, Moq.It.IsAny<RevisionInfo>())).Returns(optimization2State);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            // функции агрегации
            symbolTable.DeclareFunction(new ParameterFunction(null, "weight", CalcAggregation.Weighted, "", ""));
            symbolTable.DeclareFunction(new ParameterFunction(null, "sum", CalcAggregation.Sum, "", ""));
            symbolTable.DeclareFunction(new ParameterFunction(null, "first", CalcAggregation.First, "", ""));

            String formula = "weight($par1$(1, 10, 100), $par2$(2, 20));";

            Compiler compiler = new Compiler();

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formula, null);

            var expectedCode = new Instruction[] {           
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@node1"), new Address( Address.AddressType.Parameter){ SymbolName = "par1" }),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@node2"), new Address( Address.AddressType.Parameter){ SymbolName = "par2" }),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tau"), new Address(SymbolValue.CreateValue(0))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tautill"), new Address(SymbolValue.CreateValue("@tau"))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@1@a"), new Address(SymbolValue.CreateValue(1))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@1@b"), new Address(SymbolValue.CreateValue(10))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@1@c"), new Address(SymbolValue.CreateValue(100))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@2@d"), new Address(SymbolValue.CreateValue(2))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@2@e"), new Address(SymbolValue.CreateValue(20))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.Nothing)),
                new Instruction(null, Instruction.OperationCode.Call, new Address("weight")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp0"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return), };

            CodeAssert.AreEqual(expectedCode, code);
        }

        [Test]
        public void Compile_ParameterCallOptimization()
        {
            ICalcNode optimizationNode = new TestCalcNode() { NodeID = 1 };
            ICalcNode parameterNode = new TestCalcNode() { NodeID = 4 };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimizationNode)
            {
                Calculable = true,
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[] { 
                    new TestOptimizationArgument() { Name = "a" },
                    new TestOptimizationArgument() { Name = "b" },
                    new TestOptimizationArgument() { Name = "c" },
                }
            };
            IParameterInfo parameterInfo = new TestParameterinfo(parameterNode)
            {
                Code = "par1",
                Optimization = optimizationInfo,
                Interval = Interval.Day
            };
            ICalcState optimizationState = new OptimizationState(null, optimizationInfo, optimizationInfo.Revision);
            ICalcState parameterState = new NodeState(parameterInfo, parameterInfo.Revision);

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), "par1")).Returns(parameterInfo);
            calcContext.Setup(c => c.GetState(parameterNode, Moq.It.IsAny<RevisionInfo>())).Returns(parameterState);
            calcContext.Setup(c => c.GetState(optimizationNode, Moq.It.IsAny<RevisionInfo>())).Returns(optimizationState);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            // функции агрегации
            symbolTable.DeclareFunction(new ParameterFunction(null, "sum", CalcAggregation.Sum, "", ""));
            symbolTable.DeclareFunction(new ParameterFunction(null, "first", CalcAggregation.First, "", ""));

            String formula = "$par1$;";

            Compiler compiler = new Compiler();

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formula, null);

            var expectedCode = new Instruction[] { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")/*, new Address(SymbolValue.Nothing)*/),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@node"), new Address( Address.AddressType.Parameter){ SymbolName = "par1" }),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tau"), new Address(SymbolValue.CreateValue(0))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tautill"), new Address(SymbolValue.CreateValue("@tau"))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.Nothing)),
                new Instruction(null, Instruction.OperationCode.Call, new Address("first")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp0"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return),
            };

            CodeAssert.AreEqual(expectedCode, code);

            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Compile_ParameterWithImplicitArguments()
        {
            ICalcNode optimizationNode = new TestCalcNode() { NodeID = 1 };
            ICalcNode parameterNode = new TestCalcNode() { NodeID = 4 };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimizationNode)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[] { 
                    new TestOptimizationArgument() { Name = "a" },
                    new TestOptimizationArgument() { Name = "b" },
                    new TestOptimizationArgument() { Name = "c" },
                }
            };
            IParameterInfo parameterInfo = new TestParameterinfo(parameterNode)
            {
                Code = "par1",
                Optimization = optimizationInfo,
                Interval = Interval.Day
            };
            ICalcState optimizationState = new OptimizationState(null, optimizationInfo, optimizationInfo.Revision);
            ICalcState parameterState = new NodeState(parameterInfo, parameterInfo.Revision);

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), "par1")).Returns(parameterInfo);
            calcContext.Setup(c => c.GetState(parameterNode, Moq.It.IsAny<RevisionInfo>())).Returns(parameterState);
            calcContext.Setup(c => c.GetState(optimizationNode, Moq.It.IsAny<RevisionInfo>())).Returns(optimizationState);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            // функции агрегации
            symbolTable.DeclareFunction(new ParameterFunction(null, "sum", CalcAggregation.Sum, "", ""));
            symbolTable.DeclareFunction(new ParameterFunction(null, "first", CalcAggregation.First, "", ""));

            String formula = "$par1$;";
            var args = new KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>[] { 
                new KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>(optimizationInfo, new CalcArgumentInfo[] { 
                    new CalcArgumentInfo("a", "", ParameterAccessor.In),
                    new CalcArgumentInfo("b", "", ParameterAccessor.In),
                    new CalcArgumentInfo("c", "", ParameterAccessor.In),
                })
            };

            Compiler compiler = new Compiler();

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formula, args);

            var expectedCode = new Instruction[] { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")/*, new Address(SymbolValue.Nothing)*/),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@node"), new Address( Address.AddressType.Parameter){ SymbolName = "par1" }),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tau"), new Address(SymbolValue.CreateValue(0))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tautill"), new Address(SymbolValue.CreateValue("@tau"))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@1@a"), new Address("a", true)),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@1@b"), new Address("b", true)),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@1@c"), new Address("c", true)),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.Nothing)),
                new Instruction(null, Instruction.OperationCode.Call, new Address("first")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp0"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return),
            };

            CodeAssert.AreEqual(expectedCode, code);

            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Compile_ParameterWithExplicitArguments()
        {
            // используемые в формуле параметры
            ICalcNode param1 = new TestCalcNode() { NodeID = 1, Name = "par1" };
            ICalcNode param2 = new TestCalcNode() { NodeID = 2, Name = "par2" };
            ICalcNode optim = new TestCalcNode() { NodeID = 101, Name = "optim" };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optim)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[] {
                    new TestOptimizationArgument() { Name = "a" },
                    new TestOptimizationArgument() { Name = "b" },
                    new TestOptimizationArgument() { Name = "c" }
                }
            };
            IParameterInfo param1Info = new TestParameterinfo(param1)
            {
                Code = "par1",
                Optimization = optimizationInfo,
                Interval = Interval.Hour
            };
            IParameterInfo param2Info = new TestParameterinfo(param2)
            {
                Code = "par2",
                Interval = Interval.Hour
            };

            CalcState param1State = new NodeState(param1Info, param1Info.Revision);
            CalcState param2State = new NodeState(param2Info, param2Info.Revision);
            CalcState optimizationState = new OptimizationState(null, optimizationInfo, optimizationInfo.Revision);

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), "par1")).Returns(param1Info);
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), "par2")).Returns(param2Info);
            calcContext.Setup(c => c.GetState(param1, Moq.It.IsAny<RevisionInfo>())).Returns(param1State);
            calcContext.Setup(c => c.GetState(param2, Moq.It.IsAny<RevisionInfo>())).Returns(param2State);
            calcContext.Setup(c => c.GetState(optim, Moq.It.IsAny<RevisionInfo>())).Returns(optimizationState);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            // функции агрегации
            symbolTable.DeclareFunction(new ParameterFunction(null, "sum", CalcAggregation.Sum, "", ""));
            symbolTable.DeclareFunction(new ParameterFunction(null, "first", CalcAggregation.First, "", ""));

            String formula = "sum($par1$(1, 10, $par2$));";

            Compiler compiler = new Compiler();

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formula, null);

            var expectedCode = new Instruction[] {
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                // $par2$
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@node"), new Address( Address.AddressType.Parameter){ SymbolName = "par2" }),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tau"), new Address(SymbolValue.CreateValue(0))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tautill"), new Address(SymbolValue.CreateValue("@tau"))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.Nothing)),
                new Instruction(null, Instruction.OperationCode.Call, new Address("first")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp0"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                // sum($par1$)
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@node"), new Address( Address.AddressType.Parameter){ SymbolName = "par1" }),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tau"), new Address(SymbolValue.CreateValue(0))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tautill"), new Address(SymbolValue.CreateValue("@tau"))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@1@a"), new Address(SymbolValue.CreateValue(1))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@1@b"), new Address(SymbolValue.CreateValue(10))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@1@c"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.Nothing)),
                new Instruction(null, Instruction.OperationCode.Call, new Address("sum")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp0"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return),
            };

            CodeAssert.AreEqual(expectedCode, code);

            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }

        [Test]
        public void Compile_ParameterCallNotCalculableOptimization_ReportError()
        {
            ICalcNode optimization = new TestCalcNode() { NodeID = 1, Name = "optimization" };
            ICalcNode parameter = new TestCalcNode() { NodeID = 43, Name = "paramter" };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimization)
            {
                Interval = Interval.Day,
                Calculable = false,
                Arguments = new IOptimizationArgument[]{
                    new TestOptimizationArgument() { Name = "a" },
                    new TestOptimizationArgument() { Name = "b" },
                    new TestOptimizationArgument() { Name = "c" }
                }
            };
            IParameterInfo parameterInfo = new TestParameterinfo(parameter)
            {
                Code = "par1",
                Interval = Interval.Day,
                Optimization = optimizationInfo
            };

            ICalcState optimizationState = new OptimizationState(null, optimizationInfo, optimizationInfo.Revision);
            ICalcState parameterState = new NodeState(parameterInfo, parameterInfo.Revision);

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });
            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), "par1")).Returns(parameterInfo);
            calcContext.Setup(c => c.GetState(parameter, Moq.It.IsAny<RevisionInfo>())).Returns(parameterState);
            calcContext.Setup(c => c.GetState(optimization, Moq.It.IsAny<RevisionInfo>())).Returns(optimizationState);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            // функции агрегации
            symbolTable.DeclareFunction(new ParameterFunction(null, "sum", CalcAggregation.Sum, "", ""));
            symbolTable.DeclareFunction(new ParameterFunction(null, "first", CalcAggregation.First, "", ""));

            String formula = "$par1$;";

            Compiler compiler = new Compiler();

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formula, null);

            calcContext.Verify(c =>
                c.AddMessage(Moq.It.Is<CalcMessage>(m => m.Category == MessageCategory.Error
                    && m.Text.Equals("Параметр $par1$ должен вызываться с аргументами (a, b, c)"))));
        }

        [Test]
        public void Compile_ParameterCallOptimizationWithImplicitArguments()
        {
            ICalcNode baseOptimization = new TestCalcNode() { NodeID = 3, Name = "base optimization" };
            ICalcNode optimization = new TestCalcNode() { NodeID = 1, Name = "optimization" };
            ICalcNode parameter = new TestCalcNode() { NodeID = 43, Name = "paramter" };

            IOptimizationInfo baseOptimizationInfo = new TestOptimizationInfo(baseOptimization)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]{
                    new TestOptimizationArgument() { Name = "a" },
                    new TestOptimizationArgument() { Name = "b" },
                    new TestOptimizationArgument() { Name = "c" }
                }
            };
            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimization)
            {
                Calculable = true,
                Interval = Interval.Day,
                Optimization = baseOptimizationInfo,
                Arguments = new IOptimizationArgument[]{
                    new TestOptimizationArgument() { Name = "d" },
                    new TestOptimizationArgument() { Name = "e" }
                }
            };
            IParameterInfo parameterInfo = new TestParameterinfo(parameter)
            {
                Code = "par1",
                Interval = Interval.Day,
                Optimization = optimizationInfo
            };

            ICalcState baseOptimizationState = new OptimizationState(null, baseOptimizationInfo, baseOptimizationInfo.Revision);
            ICalcState optimizationState = new OptimizationState(null, optimizationInfo, optimizationInfo.Revision);
            ICalcState parameterState = new NodeState(parameterInfo, parameterInfo.Revision);

            var calcContext = new Moq.Mock<ICalcContext>();
            calcContext.Setup(c =>
                c.AddMessage(Moq.It.IsAny<IEnumerable<Message>>()))
                .Callback((IEnumerable<Message> m) =>
                {
                    foreach (var item in m)
                    {
                        calcContext.Object.AddMessage(item);
                    }
                });

            calcContext.Setup(c => c.GetParameterNode(Moq.It.IsAny<RevisionInfo>(), "par1")).Returns(parameterInfo);
            calcContext.Setup(c => c.GetState(parameter, Moq.It.IsAny<RevisionInfo>())).Returns(parameterState);
            calcContext.Setup(c => c.GetState(baseOptimization, Moq.It.IsAny<RevisionInfo>())).Returns(baseOptimizationState);
            calcContext.Setup(c => c.GetState(optimization, Moq.It.IsAny<RevisionInfo>())).Returns(optimizationState);

            var symbolTable = new SymbolTable(calcContext.Object);
            calcContext.Setup(c => c.SymbolTable).Returns(symbolTable);
            // функции агрегации
            symbolTable.DeclareFunction(new ParameterFunction(null, "sum", CalcAggregation.Sum, "", ""));
            symbolTable.DeclareFunction(new ParameterFunction(null, "first", CalcAggregation.First, "", ""));

            String formula = "$par1$;";

            var args = new KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>[] { 
                new KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>(baseOptimizationInfo, new CalcArgumentInfo[] {
                    new CalcArgumentInfo("a", "", ParameterAccessor.In),
                    new CalcArgumentInfo("b", "", ParameterAccessor.In),
                    new CalcArgumentInfo("c", "", ParameterAccessor.In)
                })
            };

            Compiler compiler = new Compiler();

            var code = compiler.Compile(calcContext.Object, RevisionInfo.Default, formula, args);

            var expectedCode = new Instruction[] { 
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@temp0")/*, new Address(SymbolValue.Nothing)*/),
                new Instruction(null, Instruction.OperationCode.EnterLevel),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@node"), new Address( Address.AddressType.Parameter){ SymbolName = "par1" }),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tau"), new Address(SymbolValue.CreateValue(0))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@tautill"), new Address(SymbolValue.CreateValue("@tau"))),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@1@a"), new Address("a", true)),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@1@b"), new Address("b", true)),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@1@c"), new Address("c", true)),
                new Instruction(null, Instruction.OperationCode.VarDecl, new Address("@ret"), new Address(SymbolValue.Nothing)),
                new Instruction(null, Instruction.OperationCode.Call, new Address("first")),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@temp0"), new Address("@ret")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Move, new Address("@ret"), new Address("@temp0")),
                new Instruction(null, Instruction.OperationCode.LeaveLevel),
                new Instruction(null, Instruction.OperationCode.Return),
            };

            CodeAssert.AreEqual(expectedCode, code);

            // не должно быть никаких сообщений
            calcContext.Verify(c => c.AddMessage(Moq.It.IsAny<CalcMessage>()), Moq.Times.Never());
        }
    }
}

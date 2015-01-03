using System;
using System.Collections.Generic;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Дерево разбора
    /// </summary>
    public class CalcTree
    {
        /// <summary>
        /// Тип узла в дереве разбора
        /// </summary>
        public enum Operator
        {
            DoubleValue,
            StringValue,
            ArrayValue,
            Variable,
            VariableDeclaration,
            Parameter,
            Exist,
            Call,

            Addition,
            Subtraction,
            Power,
            Multiplication,
            Division,
            Modulo,
            UnaryPlus,
            UnaryMinus,
            IncrementPrefix,
            IncrementSuffix,
            DecrementPrefix,
            DecrementSuffix,
            Assignment,
            AdditionAssignment,
            SubtractionAssignment,
            MultiplicationAssignment,
            DivisionAssignment,
            ModuloAssignment,
            Equal,
            NotEqual,
            Greater,
            Less,
            GreaterOrEqual,
            LessOrEqual,
            ArrayAccessor,
            LogicalNot,
            LogicalAnd,
            LogicalOr,
            CompositeStatement,
            Comma,
            IfStatement,
            WhileStatement,
            RetStatement,
            EmptyStatement,
            BreakStatement,
            ContinueStatement
        }

        /// <summary>
        /// Тип текущего узла
        /// </summary>
        public Operator CalcOperator { get; protected set; }

        /// <summary>
        /// Ветку у текущего узла
        /// </summary>
        public List<CalcTree> Branches { get; protected set; }

        /// <summary>
        /// Положение в коде
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Должен ли текущей узел вернуть значение
        /// Применяется при компиляции
        /// </summary>
        public bool MustHaveReturn { get; set; }

        /// <summary>
        /// Вещественное значение узла
        /// </summary>
        public double DoubleValue { get; set; }

        /// <summary>
        /// Строковое значение узла
        /// </summary>
        public String StringValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">Тип текущего узла</param>
        public CalcTree(Operator op)
        {
            CalcOperator = op;
            Branches = new List<CalcTree>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">Тип текущего узла</param>
        /// <param name="val">Вещественное значение узла</param>
        public CalcTree(Operator op, double val)
            : this(op)
        {
            DoubleValue = val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">Тип текущего узла</param>
        /// <param name="val">Строковое значение узла</param>
        public CalcTree(Operator op, String val)
            : this(op)
        {
            StringValue = val;
        }
    }
}

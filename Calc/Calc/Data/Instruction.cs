using System;
using System.Collections.Generic;
using System.Text;
using gppg;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Трехадресная команда
    /// </summary>
    public class Instruction
    {
        /// <summary>
        /// Операция в трехадресной команде
        /// </summary>
        public enum OperationCode
        {
            /// <summary>
            /// Вызов функции
            /// </summary>
            Call,

            /// <summary>
            /// Выход из подпрограммы
            /// </summary>
            Return,

            /// <summary>
            /// Сложение
            /// </summary>
            Addition,

            /// <summary>
            /// Вычитание
            /// </summary>
            Subtraction,

            /// <summary>
            /// Умножение
            /// </summary>
            Multiplication,

            /// <summary>
            /// Деление
            /// </summary>
            Division,

            /// <summary>
            /// Остаток
            /// </summary>
            Modulo,

            /// <summary>
            /// Унарный плюс
            /// </summary>
            UnaryPlus,

            /// <summary>
            /// Унарный минус
            /// </summary>
            UnaryMinus,

            /// <summary>
            /// Преинкремент 
            /// </summary>
            IncrementPrefix,

            /// <summary>
            /// Постинкремент
            /// </summary>
            IncrementSuffix,

            /// <summary>
            /// Предекремент
            /// </summary>
            DecrementPrefix,

            /// <summary>
            /// Постдекремент
            /// </summary>
            DecrementSuffix,

            /// <summary>
            /// Переместить
            /// </summary>
            Move,

            /// <summary>
            /// Равенство
            /// </summary>
            Equal,

            /// <summary>
            /// Неравенство
            /// </summary>
            NotEqual,

            /// <summary>
            /// Больше
            /// </summary>
            Greater,

            /// <summary>
            /// Меньше
            /// </summary>
            Less,

            /// <summary>
            /// Бодьше или равно
            /// </summary>
            GreaterOrEqual,

            /// <summary>
            /// Меньше или равно
            /// </summary>
            LessOrEqual,

            /// <summary>
            /// Отрицание
            /// </summary>
            LogicalNot,

            /// <summary>
            /// Безусловный переход
            /// </summary>
            Jump,

            /// <summary>
            /// Переход если нуль
            /// </summary>
            JumpZ,

            /// <summary>
            /// Переход если не нуль
            /// </summary>
            JumpNZ,

            /// <summary>
            /// Нет Операции
            /// </summary>
            NOP,

            /// <summary>
            /// Добавить уровень в таблицу символов
            /// </summary>
            EnterLevel,

            /// <summary>
            /// Убрать уровень из таблицы символов
            /// </summary>
            LeaveLevel,

            /// <summary>
            /// Объявить переменную
            /// </summary>
            VarDecl,

            /// <summary>
            /// Создать массив
            /// </summary>
            /// <remarks>
            /// A - переменная, куда будет записан массив
            /// B - длина массива
            /// </remarks>
            ArrayDecl,

            /// <summary>
            /// Длина массива
            /// </summary>
            /// <remarks>
            /// A - переменная куда будет помещена длина массива
            /// B - массив
            /// </remarks>
            ArrayLength,
            
            LoopEnter, 
            LoopLeave,
            LoopPass
        }

        /// <summary>
        /// Создание операции
        /// </summary>
        /// <param name="oper">Тип операции</param>
        /// <param name="a">Операнд 1</param>
        /// <param name="b">Операнд 2</param>
        /// <param name="c">Операнд 3</param>
        public Instruction(Location loc, OperationCode oper, Address a, Address b, Address c)
        {
            Location = loc;
            Operation = oper;
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        /// Создание операции
        /// </summary>
        /// <param name="oper">Тип операции</param>
        /// <param name="a">Операнд 1</param>
        /// <param name="b">Операнд 2</param>
        public Instruction(Location loc, OperationCode oper, Address a, Address b) : this(loc, oper, a, b, null) { }

        /// <summary>
        /// Создание операции
        /// </summary>
        /// <param name="oper">Тип операции</param>
        /// <param name="a">Операнд 1</param>
        public Instruction(Location loc, OperationCode oper, Address a) : this(loc, oper, a, null, null) { }

        /// <summary>
        /// Создание операции
        /// </summary>
        /// <param name="oper">Тип операции</param>
        public Instruction(Location loc, OperationCode oper) : this(loc, oper, null, null, null) { }

        /// <summary>
        /// Позиция инструкции в коде формулы
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Операция
        /// </summary>
        public OperationCode Operation { get; protected set; }

        /// <summary>
        /// Первый операнд
        /// </summary>
        public Address A { get; protected set; }

        /// <summary>
        /// Второй операнд
        /// </summary>
        public Address B { get; protected set; }

        /// <summary>
        /// Третий операнд
        /// </summary>
        public Address C { get; protected set; }

        private static readonly int OperationMaxLength = getMaxLength();

        private static int getMaxLength()
        {
            Type enumType = typeof(OperationCode);
            int maxLength = 0, length;

            foreach (System.Reflection.FieldInfo field in enumType.GetFields())
            {
                if (field.IsStatic)
                {
                    length = field.GetValue(null).ToString().Length;
                    if (length > maxLength) maxLength = length;
                }
            }
            return maxLength;
        }

        public override string ToString()
        {
            StringBuilder code = new StringBuilder();
            code.Append(Operation);
            code.Append(' ', OperationMaxLength - Operation.ToString().Length);
            if (A != null || B != null || C != null) code.Append("(");
            code.Append(A);
            if (B != null || C != null) code.Append(", ");
            code.Append(B);
            if (C != null) code.Append(", ");
            code.Append(C);
            if (A != null || B != null || C != null) code.Append(")");
            return code.ToString();
        }
    }
}

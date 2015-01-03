using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Класс для хранения значений
    /// </summary>
    [Serializable]
    public class SymbolValue
    {
        /// <summary>
        /// Пустое значение
        /// </summary>
        public static readonly SymbolValue Nothing = new SymbolValue();

        /// <summary>
        /// Значение блокирующие расчет и перерасчет параметров
        /// </summary>
        public static readonly SymbolValue BlockedValue = new SymbolValue();

        public static readonly SymbolValue TrueValue = new DoubleValue(1.0);
        public static readonly SymbolValue FalseValue = new DoubleValue(0.0);

        const String NotImpementedMessage = "Операция {0} не поддерживается типом {1}";
        const String NaNMessage = "Получено не допустимое значение ({1} {0} {2})";

        private SymbolValue OperationFailValue(ICalcContext context, SymbolValue x, string p)
        {
            if (this == SymbolValue.BlockedValue || x == SymbolValue.BlockedValue)
                return SymbolValue.BlockedValue;
            context.AddMessage(new CalcMessage(MessageCategory.Warning, NaNMessage, p, this, x));
            return Nothing;
        }

        /// <summary>
        /// Операция сложения
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="x">Второй аргумент</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue Addition(ICalcContext context, SymbolValue x)
        {
            return OperationFailValue(context, x, "+");
        }
        
        /// <summary>
        /// Операция вычитания
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="x">Второй аргумент</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue Subtraction(ICalcContext context, SymbolValue x)
        {
            return OperationFailValue(context, x, "-");
        }

        /// <summary>
        /// Операция умножения
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="x">Второй аргумент</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue Multiplication(ICalcContext context, SymbolValue x)
        {
            return OperationFailValue(context, x, "*");
        }

        /// <summary>
        /// Операция деления
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="x">Второй аргумент</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue Division(ICalcContext context, SymbolValue x)
        {
            return OperationFailValue(context, x, "/");
        }

        /// <summary>
        /// Операция остатка
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="x">Второй аргумент</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue Modulo(ICalcContext context, SymbolValue x)
        {
            return OperationFailValue(context, x, "%");
        }

        /// <summary>
        /// Операция унарного плюса
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue UnaryPlus(ICalcContext context)
        {
            if (this == SymbolValue.BlockedValue)
                return SymbolValue.BlockedValue;
            context.AddMessage(new CalcMessage(MessageCategory.Warning, NaNMessage, "+", String.Empty, this));
            return Nothing;
        }

        /// <summary>
        /// Операция минуса
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue UnaryMinus(ICalcContext context)
        {
            if (this == SymbolValue.BlockedValue)
                return SymbolValue.BlockedValue;
            context.AddMessage(new CalcMessage(MessageCategory.Warning, NaNMessage, "-", String.Empty, this));
            return Nothing;
        }

        /// <summary>
        /// Операция преинкримента
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue IncrementPrefix(ICalcContext context)
        {
            if (this == SymbolValue.BlockedValue)
                return SymbolValue.BlockedValue;
            context.AddMessage(new CalcMessage(MessageCategory.Warning, NaNMessage, "++", String.Empty, this));
            return Nothing;
        }

        /// <summary>
        /// Операция постинкримента
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue IncrementSuffix(ICalcContext context)
        {
            if (this == SymbolValue.BlockedValue)
                return SymbolValue.BlockedValue;
            context.AddMessage(new CalcMessage(MessageCategory.Warning, NaNMessage, "++", this, String.Empty));
            return Nothing;
        }

        /// <summary>
        /// Операция предекримента
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue DecrementPrefix(ICalcContext context)
        {
            if (this == SymbolValue.BlockedValue)
                return SymbolValue.BlockedValue;
            context.AddMessage(new CalcMessage(MessageCategory.Warning, NaNMessage, "--", String.Empty, this));
            return Nothing;
        }

        /// <summary>
        /// Операция постдекремента
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue DecrementSuffix(ICalcContext context)
        {
            if (this == SymbolValue.BlockedValue)
                return SymbolValue.BlockedValue;
            context.AddMessage(new CalcMessage(MessageCategory.Warning, NaNMessage, "--", this, String.Empty));
            return Nothing;
        }

        /// <summary>
        /// Операция равенства
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="x">Второй аргумент</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue Equal(ICalcContext context, SymbolValue x)
        {
            return OperationFailValue(context, x, "==");
        }

        /// <summary>
        /// Операция неравенства
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="x">Второй аргумент</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue NotEqual(ICalcContext context, SymbolValue x)
        {
            return OperationFailValue(context, x, "!=");
        }

        /// <summary>
        /// Операция больше
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="x">Второй аргумент</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue Greater(ICalcContext context, SymbolValue x)
        {
            return OperationFailValue(context, x, ">");
        }

        /// <summary>
        /// Операция меньше
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="x">Второй аргумент</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue Less(ICalcContext context, SymbolValue x)
        {
            return OperationFailValue(context, x, "<");
        }

        /// <summary>
        /// Операция больше или равно
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="x">Второй аргумент</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue GreaterOrEqual(ICalcContext context, SymbolValue x)
        {
            return OperationFailValue(context, x, ">=");
        }

        /// <summary>
        /// Операция меньше или равно
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="x">Второй аргумент</param>
        /// <returns>Результат операции</returns>
        public virtual SymbolValue LessOrEqual(ICalcContext context, SymbolValue x)
        {
            return OperationFailValue(context, x, "<=");
        }

        /// <summary>
        /// Получить длину массива
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <returns>Длина массива</returns>
        public virtual int ArrayLength(ICalcContext context) { return 1; }

        /// <summary>
        /// Операция доступа к массиву
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="index">индекс элемента</param>
        /// <returns>Элемент массива</returns>
        public virtual SymbolValue ArrayAccessor(ICalcContext context, int index)
        {
            if (this == SymbolValue.BlockedValue)
                return SymbolValue.BlockedValue;
            context.AddMessage(new CalcMessage(MessageCategory.Error, NotImpementedMessage, "индекса", GetType().Name));
            return Nothing;
        }

        /// <summary>
        /// Изменить значение элемента масива
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="index">индекс элемента</param>
        /// <param name="value">Новое значение элемента</param>
        /// <returns>Элемент массива</returns>
        public virtual SymbolValue ArrayAccessor(ICalcContext context, int index, SymbolValue value)
        {
            if (this == SymbolValue.BlockedValue)
                return SymbolValue.BlockedValue;
            context.AddMessage(new CalcMessage(MessageCategory.Error, NotImpementedMessage, "индекса", GetType().Name));
            return Nothing;
        }

        /// <summary>
        /// Проверить значение на равенство нулю
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <returns>true, если значение является нулевым</returns>
        public virtual bool IsZero(ICalcContext context) { return true; }

        protected SymbolValue() { }

        /// <summary>
        /// Получит реальное значение
        /// </summary>
        /// <returns></returns>
        public virtual Object GetValue() { return null; }

        /// <summary>
        /// Создать конкретную реализацию SymbolValue в зависимости от типа аргумента
        /// </summary>
        /// <param name="value">Значение</param>
        /// <returns></returns>
        public static SymbolValue CreateValue(Object value)
        {
            double obj;
            if (value == null) return SymbolValue.Nothing;
            if (value is double) obj = (double)value;
            else if (value is int) obj = (double)(int)value;
            else return new StringValue(value.ToString());
            if (double.IsNaN(obj))
                return SymbolValue.Nothing;
            return new DoubleValue(obj);
        }

        public override string ToString()
        {
            if (BlockedValue == this) return "Blocked";
            return "Nothing";
        }

        /// <summary>
        /// Создать значение из строки
        /// </summary>
        /// <param name="value">Значение</param>
        /// <returns></returns>
        public static SymbolValue ValueFromString(string value)
        {
            double val;
            if (String.IsNullOrEmpty(value)) return SymbolValue.Nothing;
            if (double.TryParse(value, out val)) return new DoubleValue(val);
            return new StringValue(value);
        }

        /// <summary>
        /// Передать значение по значению
        /// </summary>
        /// <returns></returns>
        public virtual SymbolValue CopyValue(ICalcContext context)
        {
            return this;
        }

        public static explicit operator double(SymbolValue value)
        {
            object objValue = value.GetValue();
            if (objValue != null)
                try
                {
                    return Convert.ToDouble(objValue);
                }
                catch { }
            return double.NaN;
        }
    }
}

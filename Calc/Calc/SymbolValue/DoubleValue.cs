using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Класс для хранения вещественных значений
    /// </summary>
    [Serializable]
    class DoubleValue : SymbolValue
    {
        private double value;

        public DoubleValue() : base() { value = 0; }
        public DoubleValue(double val) : base() { value = val; }

        public override object GetValue()
        {
            return value;
        }

        public override SymbolValue Addition(ICalcContext context, SymbolValue x)
        {
            DoubleValue doublX;

            if ((doublX = x as DoubleValue) != null) return new DoubleValue(value + doublX.value);
            return base.Addition(context, x);
        }

        public override SymbolValue Subtraction(ICalcContext context, SymbolValue x)
        {
            DoubleValue doublX;

            if ((doublX = x as DoubleValue) != null) return new DoubleValue(value - doublX.value);
            return base.Subtraction(context, x);
        }
        public override SymbolValue Multiplication(ICalcContext context, SymbolValue x)
        {
            DoubleValue doublX;

            if ((doublX = x as DoubleValue) != null) return new DoubleValue(value * doublX.value);
            return base.Multiplication(context, x); ;
        }
        public override SymbolValue Division(ICalcContext context, SymbolValue x)
        {
            DoubleValue doublX;

            if ((doublX = x as DoubleValue) != null) return new DoubleValue(value / doublX.value);
            return base.Division(context, x);
        }
        public override SymbolValue Modulo(ICalcContext context, SymbolValue x)
        {
            DoubleValue doublX;

            if ((doublX = x as DoubleValue) != null) return new DoubleValue(value % doublX.value);
            return base.Modulo(context, x);
        }
        public override SymbolValue UnaryPlus(ICalcContext context)
        {
            return new DoubleValue(value);
        }
        public override SymbolValue UnaryMinus(ICalcContext context)
        {
            return new DoubleValue(-value);
        }
        public override SymbolValue IncrementPrefix(ICalcContext context)
        {
            return new DoubleValue(++value);
        }
        public override SymbolValue IncrementSuffix(ICalcContext context)
        {
            return new DoubleValue(value++);
        }
        public override SymbolValue DecrementPrefix(ICalcContext context)
        {
            return new DoubleValue(--value);
        }
        public override SymbolValue DecrementSuffix(ICalcContext context)
        {
            return new DoubleValue(value--);
        }

        public override SymbolValue Equal(ICalcContext context, SymbolValue x)
        {
            DoubleValue doublX;

            if ((doublX = x as DoubleValue) != null)
            {
                //return value == doublX.value ? SymbolValue.TrueValue : SymbolValue.FalseValue;
                if (Math.Abs(value - doublX.value) < 1e-5)
                    return SymbolValue.TrueValue;
                else
                    return SymbolValue.FalseValue;
            }
            return base.Equal(context, x);
        }
        public override SymbolValue NotEqual(ICalcContext context, SymbolValue x)
        {
            DoubleValue doublX;

            if ((doublX = x as DoubleValue) != null) return value != doublX.value ? SymbolValue.TrueValue : SymbolValue.FalseValue;
            return base.NotEqual(context, x);
        }
        public override SymbolValue Greater(ICalcContext context, SymbolValue x)
        {
            DoubleValue doublX;

            if ((doublX = x as DoubleValue) != null) return value > doublX.value ? SymbolValue.TrueValue : SymbolValue.FalseValue;
            return base.Greater(context, x);
        }
        public override SymbolValue Less(ICalcContext context, SymbolValue x)
        {
            DoubleValue doublX;

            if ((doublX = x as DoubleValue) != null) return value < doublX.value ? SymbolValue.TrueValue : SymbolValue.FalseValue;
            return base.Less(context, x);
        }
        public override SymbolValue GreaterOrEqual(ICalcContext context, SymbolValue x)
        {
            DoubleValue doublX;

            if ((doublX = x as DoubleValue) != null) return value >= doublX.value ? SymbolValue.TrueValue : SymbolValue.FalseValue;
            return base.Greater(context, x);
        }
        public override SymbolValue LessOrEqual(ICalcContext context, SymbolValue x)
        {
            DoubleValue doublX;

            if ((doublX = x as DoubleValue) != null) return value <= doublX.value ? SymbolValue.TrueValue : SymbolValue.FalseValue;
            return base.LessOrEqual(context, x);
        }

        public override bool IsZero(ICalcContext context)
        {
            return value == 0;
        }

        public override SymbolValue CopyValue(ICalcContext context)
        {
            return new DoubleValue(value);
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}

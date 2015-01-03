using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Класс для хранения строковых значений
    /// </summary>
    [Serializable]
    class StringValue : SymbolValue
    {
        private String value;

        public StringValue() : base() { value = null; }
        public StringValue(String str) : base() { value = str.ToString(); }

        public override object GetValue()
        {
            return value;
        }

        public override int ArrayLength(ICalcContext context)
        {
            return String.IsNullOrEmpty(value) ? 0 : value.Length;
        }

        public override SymbolValue ArrayAccessor(ICalcContext context, int index)
        {
            return new StringValue(value[index].ToString());
        }

        public override bool IsZero(ICalcContext context)
        {
            return String.IsNullOrEmpty(value);
        }

        public override SymbolValue CopyValue(ICalcContext context)
        {
            return new StringValue(value);
        }

        public override string ToString()
        {
            return value;
        }
    }
}

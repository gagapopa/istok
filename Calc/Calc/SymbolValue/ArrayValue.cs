using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Calc
{
    [Serializable]
    class ArrayValue : SymbolValue
    {
        SymbolValue[] valueArray;

        public ArrayValue(ICalcContext context, int length)
            : base()
        {
            if (length < 0)
                context.AddMessage(new CalcMessage(MessageCategory.Error, "Попытка обявить массив с длиной {0}", length));
            else
                valueArray = new SymbolValue[length];

            for (int i = 0; i < length; i++)
                valueArray[i] = SymbolValue.CreateValue(0);
        }

        public ArrayValue(ICalcContext context, params SymbolValue[] values)
            : this(context, values.Length)
        {
            for (int i = 0; i < values.Length; i++)
                valueArray[i] = values[i];
        }

        public override int ArrayLength(ICalcContext context)
        {
            return valueArray == null ? 0 : valueArray.Length;
        }

        public override SymbolValue ArrayAccessor(ICalcContext context, int index)
        {
            if (index < 0 || index >= valueArray.Length)
            {
                context.AddMessage(new CalcMessage(MessageCategory.Error, "Индекс выходит за область массива", index));
                return Nothing;
            }
            return valueArray[index];
        }

        public override SymbolValue ArrayAccessor(ICalcContext context, int index, SymbolValue value)
        {
            if (index < 0 || index >= valueArray.Length)
            {
                context.AddMessage(new CalcMessage(MessageCategory.Error, "Индекс выходит за область массива", index));
                return Nothing;
            }
            return valueArray[index] = value.CopyValue(context);
        }

        public override object GetValue()
        {
            Object[] arr = new Object[valueArray.Length];

            for (int i = 0; i < valueArray.Length; i++)
                if (valueArray[i] == null)
                    arr[i] = null;
                else
                    arr[i] = valueArray[i].GetValue();

            return arr;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            int builderLength = builder.Length;
            foreach (SymbolValue value in valueArray)
            {
                if (builder.Length > builderLength)
                    builder.Append(",");
                builder.Append(value);
            }
            builder.Append("}");
            return builder.ToString();
        }

        public override SymbolValue CopyValue(ICalcContext context)
        {
            return new ArrayValue(context, valueArray);
        }
    }
}

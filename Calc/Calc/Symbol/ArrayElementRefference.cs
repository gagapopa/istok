using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Ссылка на ячейку массива
    /// </summary>
    class ArrayElementRefference : IValueRefference
    {
        ICalcContext context;

        ArrayValue array;

        int index;

        public ArrayElementRefference(ICalcContext context, ArrayValue arrayValue, int index)
        {
            this.context = context;
            this.array = arrayValue;
            this.index = index;
        }

        #region IValueHolder Members

        public SymbolValue Value
        {
            get
            {
                return array.ArrayAccessor(context, index);
            }
            set
            {
                array.ArrayAccessor(context, index, value);
            }
        }

        #endregion
    }
}

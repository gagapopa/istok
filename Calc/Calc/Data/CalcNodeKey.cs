using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Условный параметр.
    /// Включает в себя данные о параметре и значениях аргументов
    /// </summary>
    public class CalcNodeKey
    {
        /// <summary>
        /// Параметр расчёта
        /// </summary>
        public ICalcNode Node { get; set; }

        /// <summary>
        /// Значения аргументов
        /// </summary>
        public ArgumentsValues Arguments { get; set; }

        public CalcNodeKey()
        {

        }

        public CalcNodeKey(ICalcNode node, ArgumentsValues args)
            : this()
        {
            this.Node = node;
            this.Arguments = args;
        }

        public override bool Equals(object obj)
        {
            CalcNodeKey another = obj as CalcNodeKey;

            if (another != null)
            {
                return Node.Equals(another.Node)
                    && ((Arguments == null && another.Arguments == null)
                    || (Arguments != null && Arguments.Equals(another.Arguments)));
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Node.GetHashCode() +
                (Arguments == null ? 0 : Arguments.GetHashCode());
        }
    }
}

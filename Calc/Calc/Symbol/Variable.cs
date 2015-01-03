using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Переменная
    /// </summary>
    [Serializable]
    public class Variable : Symbol, IValueRefference
    {
        /// <summary>
        /// Переменная константа
        /// </summary>
        public bool IsConst { get; protected set; }

        /// <summary>
        /// Переменная является временной
        /// </summary>
        public bool IsTemp { get; protected set; }

        /// <summary>
        /// Значение переменной
        /// </summary>
        public SymbolValue Value { get; set; }

        /// <summary>
        /// Время жизне переменной не истекло
        /// </summary>
        public bool IsAlive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Имя переменной</param>
        /// <param name="value">Начальное значение переменной</param>
        /// <param name="isConst">Константа</param>
        /// <param name="isTemp">Переменная является временной</param>
        public Variable(String name, SymbolValue value, bool isConst, bool isTemp)
            : base(name)
        {
            IsConst = isConst;
            IsTemp = isTemp;
            IsAlive = true;
            Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Имя переменной</param>
        /// <param name="value">Значение переменной</param>
        public Variable(String name, SymbolValue value) : this(name, value, false, false) { }

        public Variable(String name, double value) : this(name, SymbolValue.CreateValue(value), false, false) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Имя переменной</param>
        public Variable(String name) : this(name, SymbolValue.Nothing, false, false) { }

        /// <summary>
        /// Конструктор копий
        /// </summary>
        /// <param name="x"></param>
        public Variable(Variable x)
            : this(x.Name, x.Value)
        {
            IsConst = x.IsConst;
            IsTemp = x.IsTemp;
            IsAlive = true;
        }

        public override object Clone()
        {
            return new Variable(this);
        }

        public override string ToString()
        {
            return String.Format("(var {0} = {1})", Name, Value);
        }
    }
}

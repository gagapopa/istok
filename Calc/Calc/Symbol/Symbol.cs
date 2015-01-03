using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Именнованный символ
    /// </summary>
    [Serializable]
    public class Symbol : ICloneable
    {
        /// <summary>
        /// Имя
        /// </summary>
        public String Name { get; protected set; }

        /// <summary>
        /// Создание символа
        /// </summary>
        /// <param name="name">Имя</param>
        public Symbol(String name) { Name = name; }

        /// <summary>
        /// Конструктор копий
        /// </summary>
        /// <param name="x"></param>
        public Symbol(Symbol x) : this(x.Name) { }

        public override string ToString()
        {
            return Name;
        }

        #region ICloneable Members

        public virtual object Clone()
        {
            return new Symbol(this);
        }

        #endregion
    }
}

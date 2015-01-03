using System;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Cсылка на значение
    /// </summary>
    public interface IValueRefference
    {
        /// <summary>
        /// Значение
        /// </summary>
        SymbolValue Value { get; set; }
    }
}

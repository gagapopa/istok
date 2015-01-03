using System;
using System.Collections.Generic;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Компилятор
    /// </summary>
    public interface ICompiler
    {
        /// <summary>
        /// Скомпилировать текст при данных аргументах в трехадресный код.
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        /// <param name="revision">Компилируемая ревизия</param>
        /// <param name="formulaText">Компилируемый текст</param>
        /// <param name="arguments">Переданные аргументы</param>
        /// <returns>Скомпилируемый код</returns>
        Instruction[] Compile(
            ICalcContext context,
            RevisionInfo revision,
            string formulaText,
            KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>[] arguments);
    }
}

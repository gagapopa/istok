using System;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Состояние параметра расчёта
    /// </summary>
    public interface ICalcState
    {
        /// <summary>
        /// Скомпилировать параметр
        /// </summary>
        /// <param name="compiler">Контекст комплияции</param>
        /// <param name="context">Контекст расчёта</param>
        void Compile(ICompiler compiler, ICalcContext context);

        /// <summary>
        /// Информация параметра
        /// </summary>
        ICalcNodeInfo NodeInfo { get; }

        /// <summary>
        /// Ревизия
        /// </summary>
        RevisionInfo Revision { get; }

        /// <summary>
        /// Позиция для сообщений компиляции
        /// </summary>
        CalcPosition CalcPosition { get; }

        /// <summary>
        /// Если компиляция закончилась неудачей, возвращает true
        /// </summary>
        bool Failed { get; set; }
    }
}

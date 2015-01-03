using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Базовый класс для информации о расчете
    /// </summary>
    public interface ICalcNodeInfo
    {
        /// <summary>
        /// Сам параметр
        /// </summary>
        ICalcNode CalcNode { get; }

        /// <summary>
        /// Ревизия
        /// </summary>
        RevisionInfo Revision { get; }

        /// <summary>
        /// Наимнование параметра
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Дискретность параметра
        /// </summary>
        Interval Interval { get; }

        ///// <summary>
        ///// Начальнное время параметра
        ///// </summary>
        //DateTime StartTime { get; }

        /// <summary>
        /// Является ли параметр расчитываемым
        /// </summary>
        bool Calculable { get; }

        /// <summary>
        /// Информация для проведения оптимизационных расчетов
        /// </summary>
        IOptimizationInfo Optimization { get; }

        /// <summary>
        /// Список кодов параметров, расчёт которых должен вызываться до данного узла
        /// </summary>
        IEnumerable<String> Needed { get; }
    }
}


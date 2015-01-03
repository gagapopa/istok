using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Информация о оптимизационных расчетах
    /// </summary>
    public interface IOptimizationInfo : ICalcNodeInfo
    {
        /// <summary>
        /// Аргументы для условной оптимизации
        /// </summary>
        IOptimizationArgument[] Arguments { get; }

        /// <summary>
        /// Оптимизируемое выражение
        /// </summary>
        String Expression { get; }

        /// <summary>
        /// Выражение указывающуе попадают ли аргументы в область определения 
        /// </summary>
        String DefinationDomain { get; }

        /// <summary>
        /// Требуется ли поиск максимума
        /// </summary>
        bool Maximalize { get; }

        /// <summary>
        /// Расчитывать все вложенные параметры для всех аргументов
        /// </summary>
        bool CalcAllChildParameters { get; }

        /// <summary>
        /// Все параметры учавствующие в оптимизации за исключением параметров вложенных оптимизаций
        /// </summary>
        IEnumerable<IParameterInfo> ChildParameters { get; }

        /// <summary>
        /// Все вложенные оптимизации
        /// </summary>
        IEnumerable<IOptimizationInfo> ChildOptimization { get; }
    }
}


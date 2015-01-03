using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Расширение для ICalcNode, добавляет дополнительные методы
    /// </summary>
    public static class CalcNodeExtension
    {
        /// <summary>
        /// Получить данные параметра за конкретную ревизию
        /// </summary>
        /// <param name="calcNode"></param>
        /// <param name="revision">Ревизия</param>
        /// <returns></returns>
        public static IParameterInfo GetParameter(this ICalcNode calcNode, RevisionInfo revision)
        {
            return calcNode.Revisions.Get(revision) as IParameterInfo;
        }

        /// <summary>
        /// Получить данные оптимизации за конкретную ревизию
        /// </summary>
        /// <param name="calcNode"></param>
        /// <param name="revision">Ревизия</param>
        /// <returns></returns>
        public static IOptimizationInfo GetOptimization(this ICalcNode calcNode, RevisionInfo revision)
        {
            return calcNode.Revisions.Get(revision) as IOptimizationInfo;
        }
    }
}


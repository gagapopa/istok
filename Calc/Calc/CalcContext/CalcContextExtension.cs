using System;
using System.Linq;
using System.Collections.Generic;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Методы расширения для ICalcContext, скрывающие некоторые касты
    /// </summary>
    public static class CalcContextExtension
    {
        /// <summary>
        /// Получить состояние параметра расчёта
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <param name="revision">Ревизия</param>
        /// <param name="parameterCode">Код параметра</param>
        /// <returns></returns>
        public static NodeState GetParameter(this ICalcContext calcContext, RevisionInfo revision, String parameterCode)
        {
            IParameterInfo parameterInfo = calcContext.GetParameterNode(revision, parameterCode);

            if (parameterInfo == null)
                return null;

            return calcContext.GetState(parameterInfo.CalcNode, revision) as NodeState;
        }

        /// <summary>
        /// Получить состояние оптимизации
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <param name="optimizationInfo">Информация о оптимизации</param>
        /// <returns></returns>
        public static OptimizationState GetOptimization(this ICalcContext calcContext, IOptimizationInfo optimizationInfo, RevisionInfo revision)
        {
            if (optimizationInfo == null)
                return null;

            return calcContext.GetState(optimizationInfo.CalcNode, revision) as OptimizationState;
        }
    }
}

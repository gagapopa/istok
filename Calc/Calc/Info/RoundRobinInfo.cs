using System;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Информация о циклическом расчете
    /// </summary>
    [Serializable]
    public struct RoundRobinInfo
    {
        /// <summary>
        /// Время последнего запуска циклического расчета
        /// </summary>
        public DateTime LastStartTime { get; set; }
        /// <summary>
        /// Время следующего запуска циклического расчета
        /// </summary>
        public DateTime NextStartTime { get; set; }
        /// <summary>
        /// Ремя последнего окончания расчета
        /// </summary>
        public DateTime LastStopTime { get; set; }
        /// <summary>
        /// Продолжительность последнего расчета
        /// </summary>
        public TimeSpan LastCalcTimeSpan { get; set; }
    }
}

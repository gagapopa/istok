using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Информация о параметрах используемых в расчете
    /// </summary>
    public interface IParameterInfo : ICalcNodeInfo
    {
        /// <summary>
        /// Код параметра
        /// </summary>
        String Code { get; }

        /// <summary>
        /// Формула для расчета параметра
        /// </summary>
        String Formula { get; }

        /// <summary>
        /// Участвует ли параметр в циклическом расчете
        /// </summary>
        bool RoundRobinCalc { get; }
    }
}


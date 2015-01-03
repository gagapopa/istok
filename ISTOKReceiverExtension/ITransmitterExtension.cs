using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK;

namespace COTES.ISTOK
{
    /// <summary>
    /// Интерфейс для передачи значений ISTOKExtension
    /// </summary>
    public interface ITransmitterExtension
    {
        /// <summary>
        /// Получить значения отдаваемых параметров
        /// </summary>
        /// <param name="start">Начальное время</param>
        /// <param name="finish">Конечное время</param>
        /// <returns>Значения</returns>
        COTES.ISTOK.ParameterReceiverExtension.Parameter[] GetExtensionParameters(DateTime start, DateTime finish);

        /// <summary>
        /// Запросить код параметра по его идентификатору
        /// </summary>
        /// <param name="parameterID">Идентификатор параметра</param>
        /// <returns>Код параметра</returns>
        string GetParameterCode(int parameterID);
    }
}

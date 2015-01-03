using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ParameterReceiverExtension
{
    /// <summary>
    /// Интерфейс для взаимодействия с сервером данных
    /// </summary>
    public interface IParameterReceiver
    {
        /// <summary>
        /// Событие, возникающее при поступлении данных в систему
        /// </summary>
        event EventHandler DataReady;
        /// <summary>
        /// Получение массива параметров из системы
        /// </summary>
        /// <param name="start">Начальное время</param>
        /// <param name="finish">Конечное время</param>
        /// <returns>Массив параметров</returns>
        Parameter[] GetParameters(DateTime start, DateTime finish);
    }
}

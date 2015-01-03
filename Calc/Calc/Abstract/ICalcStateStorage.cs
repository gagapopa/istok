using System;
namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Хранилище ICalcState для параметров расчёта и для элемнтов оптимизации
    /// </summary>
    public interface ICalcStateStorage
    {
        /// <summary>
        /// Получить ICalcState для конкретного узла расчёта
        /// </summary>
        /// <param name="calcNode">Информация о узле расчёта</param>
        /// <param name="revision">Текущая ревизия</param>
        /// <returns></returns>
        ICalcState GetState(ICalcNode calcNode, RevisionInfo revision); 
    }
}

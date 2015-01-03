using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Контроллер, а может модель для просмотра значений параметров
    /// </summary>
    interface IValuesController
    {
        /// <summary>
        /// Заголовок окна
        /// </summary>
        String Title { get; }
        //DateTime StartTime { get; }
        Interval Interval { get; }
        bool VariableConsts { get; }

        ParameterNode Parameter { get; }

        #region работа со значениями
        /// <summary>
        /// Начальное время значений для текущих отображаемых значений
        /// </summary>
        DateTime CurrentStartTime { get; }

        /// <summary>
        /// Конечное время значений для текущих отображаемых значений
        /// </summary>
        DateTime CurrentEndTime { get; }

        /// <summary>
        /// Запросить значения с сервера
        /// </summary>
        /// <param name="startTime">Начальное запрашивоемое время</param>
        /// <param name="endTime">Конечное запрашиваемое время</param>
        void RetrieveValue(DateTime startTime, DateTime endTime);

        /// <summary>
        /// Текущие отображаемые значения
        /// </summary>
        List<ParamValueItem> Values { get; }

        /// <summary>
        /// Очистить текущие изменения значений
        /// </summary>
        void ClearValues();

        /// <summary>
        /// Изменилось ли данное значение
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Modified(ParamValueItem value);

        bool HasChanges { get; }

        /// <summary>
        /// Проверить евляется ли текущие значение скорректированным
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Corrected(ParamValueItem value);

        /// <summary>
        /// Событие вызываемое когда изменились отображаемые значения
        /// </summary>
        event EventHandler ValuesRetrieved;
        #endregion

        #region Блокировки
        /// <summary>
        /// Можно ли брать значения на изменение значений
        /// </summary>
        bool Lockeable { get; }

        /// <summary>
        /// Блокировки беруться автоматом
        /// </summary>
        bool LockAlways { get; }

        /// <summary>
        /// Взять текущие значения на изменения
        /// </summary>
        void LockValues();

        /// <summary>
        /// Освободить текущие значения
        /// </summary>
        void ReleaseValues();

        /// <summary>
        /// Взяты ли значения на изменение
        /// </summary>
        bool Locked { get; }

        /// <summary>
        /// Время значений, которые блокируются другими пользователями и не могут быть изменены
        /// </summary>
        List<DateTime> LockedTimes { get; }

        /// <summary>
        /// Событие, уведомляющие о том, что блокировка изменилась
        /// </summary>
        event EventHandler LockChanged;
        #endregion

        #region Редактирование значений
        /// <summary>
        /// Можно ли редактировать значения
        /// </summary>
        bool Editable { get; }

        /// <summary>
        /// Можно ли редатировать время знаечний
        /// </summary>
        bool TimeEditable { get; }
     
        /// <summary>
        /// Установить измененное значение
        /// </summary>
        /// <param name="value"></param>
        /// <param name="originalValue"></param>
        void SetValue(ParamValueItem value, ParamValueItem originalValue);
    
        /// <summary>
        /// Удалить значение
        /// </summary>
        /// <param name="value"></param>
        void DeleteValue(ParamValueItem value);
    
        /// <summary>
        /// Можно ли корректировать значения
        /// </summary>
        bool Correctable { get; }

        bool HasOriginalValue { get; }

        /// <summary>
        /// Скорректировать значение
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ParamValueItem Correct(ParamValueItem value);
    
        /// <summary>
        /// Удалить корректировку
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ParamValueItem Decorrect(ParamValueItem value);
   
        /// <summary>
        /// Сохранить изменение
        /// </summary>
        void Save();
        #endregion

        DateTime MinDateTimeValue { get; }
    }
}

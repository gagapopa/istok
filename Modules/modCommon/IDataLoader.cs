using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Modules
{
    /// <summary>
    /// Модуль сбора данных
    /// </summary>
    public interface IDataLoader
    {
        /// <summary>
        /// Инициализация настроек сбора.
        /// </summary>
        /// <remarks>
        /// Метод вызывается один раз для передачи настроек канала 
        /// и запрашиваемых параметров.
        /// </remarks>
        /// <param name="channelInfo">
        /// Настройки канала сбора
        /// </param>
        void Init(ChannelInfo channelInfo);

        /// <summary>
        /// Запросить имеющиеся у источника данных параметры.
        /// </summary>
        /// <returns>
        /// Возвращает множество параметров с настройками достаточнцми 
        /// для дальнейшего запроса данных этих параметров.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Источник данных не поддерживает данную функцию
        /// </exception>
        ParameterItem[] GetParameters();

        /// <summary>
        /// Объект, который принимает новые собранные значения.
        /// </summary>
        IDataListener DataListener { get; set; }

        /// <summary>
        /// Метод сбора в котором работает модуль.
        /// Должен быть выставлен после метода Init()
        /// </summary>
        DataLoadMethod LoadMethod { get; }

        /// <summary>
        /// Зарегистрировать подписку у источника данных.
        /// </summary>
        /// <remarks>
        /// Метод должен быть реализован тольк для LoadMethod == DataLoadMethod.Subscribe.
        /// Предпологается, что модуль сбора подписывается на изменения данных у источника данных.
        /// Когда модуль получает новые данные от источника, он должен передать их объекту DataListener.
        /// </remarks>
        /// <exception cref="NotSupportedException">
        /// Метод сбора DataLoadMethod.Subscribe не поддерживается данным модулем сбора
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Метод сбора DataLoadMethod.Subscribe поддерживается, но LoadMethod != DataLoadMethod.Subscribe
        /// </exception>
        void RegisterSubscribe();

        /// <summary>
        /// Отменить подписку у источника данных.
        /// </summary>
        /// <remarks>
        /// Метод должен быть реализован тольк для LoadMethod == DataLoadMethod.Subscribe
        /// </remarks>
        /// <exception cref="NotSupportedException">
        /// Метод сбора DataLoadMethod.Subscribe не поддерживается данным модулем сбора
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Метод сбора DataLoadMethod.Subscribe поддерживается, но LoadMethod != DataLoadMethod.Subscribe
        /// </exception>
        void UnregisterSubscribe();

        /// <summary>
        /// Запрос текущих данных.
        /// </summary>
        /// <remarks>
        /// Метод должен быть реализован тольк для LoadMethod == DataLoadMethod.Current.
        /// Модуль сбора должен запросить у источника данных текущие данные
        /// и передать полученый результат объекту DataListener.
        /// </remarks>
        /// <exception cref="NotSupportedException">
        /// Метод сбора DataLoadMethod.Current не поддерживается данным модулем сбора
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Метод сбора DataLoadMethod.Current поддерживается, но LoadMethod != DataLoadMethod.Current
        /// </exception>
        void GetCurrent();

        /// <summary>
        /// Запросить у источника данных архивные данные
        /// </summary>
        /// <remarks>
        /// Метод должен быть реализован тольк для LoadMethod == DataLoadMethod.Archive.
        /// Модуль сбора должен запросить у источника данных архивные данные за указанный интервал времени
        /// и передать полученый результат объекту DataListener.
        /// </remarks>
        /// <param name="startTime">Начало запрашиваемого интервала времени</param>
        /// <param name="endTime">Конец запрашиваемого интервала времени</param>
        /// <exception cref="NotSupportedException">
        /// Метод сбора DataLoadMethod.Archive не поддерживается данным модулем сбора
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Метод сбора DataLoadMethod.Archive поддерживается, но LoadMethod != DataLoadMethod.Archive
        /// </exception>
        void GetArchive(DateTime startTime, DateTime endTime);

        /// <summary>
        /// Зарегистрировать запрашиваемый интервал времени для указанного параметра.
        /// </summary>
        /// <remarks>
        /// Метод должен быть реализован тольк для LoadMethod == DataLoadMethod.ArchiveByParameter.
        /// При помощи вызовов данного метода для каждого параметра настраивается запрос к источнику данных, производимый методом GetArchive().
        /// GetArchive() должен запрашивать только те параметры и только за тот интервал времени, который настроен выставлен данным методом.
        /// </remarks>
        /// <param name="parameter">Параметр</param>
        /// <param name="startTime">Начало запрашиваемого интервала времени</param>
        /// <param name="endTime">Конец запрашиваемого интервала времени</param>
        /// <exception cref="NotSupportedException">
        /// Метод сбора DataLoadMethod.ArchiveByParameter не поддерживается данным модулем сбора
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Метод сбора DataLoadMethod.ArchiveByParameter поддерживается, но LoadMethod != DataLoadMethod.ArchiveByParameter
        /// </exception>
        void SetArchiveParameterTime(ParameterItem parameter, DateTime startTime, DateTime endTime);

        /// <summary>
        /// Запросить у источника данных архивные данные.
        /// </summary>
        /// <remarks>
        /// Метод должен быть реализован тольк для LoadMethod == DataLoadMethod.ArchiveByParameter.
        /// Должны запрашитваться только те параметры, запрашиваемый интервал времени для которых выставлен предыдущими вызовами SetArchiveParameterTime()
        /// </remarks>
        /// <exception cref="NotSupportedException">
        /// Метод сбора DataLoadMethod.ArchiveByParameter не поддерживается данным модулем сбора
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Метод сбора DataLoadMethod.ArchiveByParameter поддерживается, но LoadMethod != DataLoadMethod.ArchiveByParameter
        /// </exception>
        void GetArchive();
    }
}

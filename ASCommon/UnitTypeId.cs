using COTES.ISTOK.ASC.TypeConverters;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Тип узла.
    /// Содержит некоторые предопределенные типы.
    /// </summary>
    [DataContract]
    public enum UnitTypeId
    {
        /// <summary>
        /// Не известный тип. Муссорный элемент
        /// </summary>
        [EnumMember]
        Unknown = 0, 

        /// <summary>
        /// Станция
        /// </summary>
        [EnumMember]
        Station = 1, 

        /// <summary>
        /// Сервер сбора
        /// </summary>
        [EnumMember]
        Block = 2, 

        /// <summary>
        /// Канал сбора
        /// </summary>
        [EnumMember]
        Channel = 3,

        /// <summary>
        /// Собираемый параметр
        /// </summary>
        [EnumMember]
        Parameter = 4, 

        /// <summary>
        /// Расчитываемый параметр
        /// </summary>
        [EnumMember]
        TEP = 5/*EEP*/, 

        /// <summary>
        /// График
        /// </summary>
        [EnumMember]
        Graph = 6,

        /// <summary>
        /// Мнемосхема
        /// </summary>
        [EnumMember]
        Schema = 7,

        /// <summary>
        /// Ручной ввод
        /// </summary>
        [EnumMember]
        ManualGate = 8, 

        /// <summary>
        /// Параметр ручного ввода
        /// </summary>
        [EnumMember]
        ManualParameter = 9, 

        /// <summary>
        /// Отчёт
        /// </summary>
        [EnumMember]
        Report = 10, 

        /// <summary>
        /// Папка
        /// </summary>
        [EnumMember]
        Folder = 12, 

        /// <summary>
        /// Нормативный график
        /// </summary>
        [EnumMember]
        NormFunc = 14,

        /// <summary>
        /// Отчёт-Excel
        /// </summary>
        [EnumMember]
        ExcelReport = 15, 

        /// <summary>
        /// Расчёт
        /// </summary>
        [EnumMember]
        TEPTemplate = 16, 

        /// <summary>
        /// Гистограмма
        /// </summary>
        [EnumMember]
        Histogram = 18,

        /// <summary>
        /// Таблица мониторинга
        /// </summary>
        [EnumMember]
        MonitorTable = 19/*, Boiler = 20*/, 

        /// <summary>
        /// Оптимизация
        /// </summary>
        [EnumMember]
        OptimizeCalc = 21
    }
}

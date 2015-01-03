using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace EMA.ASTDK.Data
{
    /// <summary>
    /// Интерфейс для взаимодействия с ПО "Расчетные модули АСТДК" ОАО "ЭМАльянс"
    /// </summary>
    public interface IMonitoringData
    {
        /// <summary>
        /// Получение конструктивных данных по объекту диагностирования "Барабан"
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="code">Код</param>
        /// <returns>Таблица БД, где:
        /// Code - Код;
        /// Steel - Марка стали;
        /// DiameterInside - Внутренний диаметра;
        /// Thickness - Толщина стенки;
        /// SensorPressFirst - Код датчика давления;
        /// SensorPressSecond - Код датчика давления;
        /// SensorTempM_TopLft - Код датчика температуры металла вверху слева; 
        /// SensorTempM_TopCntr - Код датчика температуры металла вверху в центре;
        /// SensorTempM_TopRght - Код датчика температуры металла вверху справа;
        /// SensorTempM_BtmLft - Код датчика температуры металла внизу слева; 
        /// SensorTempM_BtmCntr - Код датчика температуры металла внизу в центре;
        /// SensorTempM_BtmRght - Код датчика температуры металла внизу справа;
        /// NozzleCount - Количество водоопускных стояков;
        /// NozzleCodes - Коды водоопускных стояков (через ";");
        /// PlanResource - Плановый ресурс; 
        /// PID - Путь к PI диаграмме;
        /// Description - Описание;
        ///</returns>  
        DataTable GetDrum(int boiler_id, string code);

        /// <summary>
        /// Получение конструктивных данных по объекту диагностирования "Штуцер барабана"
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="code">Код</param>
        /// <returns>Таблица БД, где:
        /// Code - Код;
        /// DrumCode - Код барабана;
        /// Steel - Марка стали;
        /// SteelWeldedPipe - марка стали привариваемой трубы;
        /// DiameterOutside - Наружный диаметр;
        /// Thickness - Толщина стенки;
        /// SensorTemp - Код датчика температуры среды;
        /// PID - Путь к PI диаграмме;
        /// Description - Описание ;
        /// </returns>
        DataTable GetDrumNozzle(int boiler_id, string code);

        /// <summary>
        /// Получение конструктивных данных по объекту диагностирования "Прямая труба"
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="code">Код</param>
        /// <returns>Таблица БД, где:
        /// Code - Код;
        /// Steel - Марка стали;
        /// DiameterOutside - Наружный диаметр;
        /// Thickness - Толщина стенки;
        /// SensorTempM - Код датчика температуры металла
        /// PlanResource - Плановый ресурс; 
        /// PID - Путь к PI диаграмме;
        /// Description - Описание;
        /// </returns>
        DataTable GetStraightPipe(int boiler_id, string code);

        /// <summary>
        /// Получение конструктивных данных по объекту диагностирования "Гиб трубы"
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="code">Код</param>
        /// <returns>Таблица БД, где:
        /// Code - Код;
        /// Steel - Марка стали;
        /// DiameterOutside - Наружный диаметр;
        /// DiameterOutMin - Минимальный диаметр в сечении;
        /// DiameterOutMax - Максимальный диаметр в сечении;
        /// Thickness - Толщина стенки;
        /// Radius - Радиус по нейтральной оси;
        /// SensorTempM - Код датчика температуры металла;
        /// PlanResource - Плановый ресурс; 
        /// PID - Путь к PI диаграмме;
        /// Description - Описание;
        /// </returns>
        DataTable GetBend(int boiler_id, string code);

        /// <summary>
        /// Получение значений эквивалентных напряжений для объекта диагностирования 
        /// за указанный интервал времени
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="code">Код объекта</param>
        /// <param name="start">Начало интервала времени</param>
        /// <param name="finish">Конец интервала времени</param>
        /// При вызове функции необходимо параметры времени передавать как именованные, 
        /// при этом если задается:
        /// Только start - выбираются все значения от заданного момента времени и до текущего
        /// Только finish - выбираются все значения от начального и до заданного момента времени
        /// start и finish - выбираются все значения за заданный интервал времени
        /// Не заданы параметры времени - выбираются все существующие значения 
        /// <returns>Таблица БД, где:
        /// Code - Код объекта;
        /// TimeStress - Дата и время;
        /// HoopEquivalentStress - Окружное напряжение;
        /// AxialEquivalentStress - Осевое напряжение;
        /// </returns>        
        DataTable GetEquivalentStress(int boiler_id, string code,
            DateTime? start, DateTime? finish);

        /// <summary>
        /// Получение исчерпания ресурса в результате воздействия 
        /// ползучести для объекта диагностирования
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="code">Код объекта</param>
        /// <param name="start">Начало интервала времени</param>
        /// <param name="finish">Конец интервала времени</param>
        /// При вызове функции необходимо параметры времени передавать как именованные, 
        /// при этом если задается:
        /// Только start - выбираются все значения от заданного момента времени и до текущего
        /// Только finish - выбираются все значения от начального и до заданного момента времени
        /// start и finish - выбираются все значения за заданный интервал времени
        /// Не заданы параметры времени - выбираются все существующие значения 
        /// <returns>Таблица БД, где:
        /// TimeCreep - Дата и время;
        /// LifeCreep - Значение ресурса;
        /// Temperature - Значение температуры;
        /// </returns>               
        DataTable GetLifeCreep(int boiler_id, string code,
            DateTime? start, DateTime? finish);

        /// <summary>
        /// Получение суммарного (текущего) значения исчерпания ресурса 
        /// в результате воздействия ползучести для объекта диагностирования
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="code">Код объекта</param>
        /// <param name="timeCreep">Дата и время (выходной параметр)</param>
        /// <param name="lifeCreep">Текущее значение ресурса (выходной параметр)</param>
        void GetLifeCreepCurrent(int boiler_id, string code,
            out DateTime timeCreep, out double lifeCreep);

        /// <summary>
        /// Получение исчерпания ресурса в результате воздействия высокочастотных 
        /// циклов для объекта диагностирования "Барабан"
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="code">Код объекта</param>
        /// <param name="start">Начало интервала времени</param>
        /// <param name="finish">Конец интервала времени</param>
        /// При вызове функции необходимо параметры времени передавать как именованные, 
        /// при этом если задается:
        /// Только start - выбираются все значения от заданного момента времени и до последнего
        /// Только finish - выбираются все значения от начального и до заданного момента времени
        /// start и finish - выбираются все значения за заданный интервал времени
        /// Не заданы параметры времени - выбираются все существующие значения 
        /// <returns>Таблица БД, где:
        /// Code - Код объекта;
        /// TimeHFC - Дата и время;
        /// LifeHFC - Значение ресурса; 
        /// DeltaTemperature - Значение изменения температуры;
        /// </returns>              
        DataTable GetLifeHFC(int boiler_id, string code,
            DateTime? start, DateTime? finish);

        /// <summary>
        /// Получение суммарного (текущего) значения исчерпания ресурса в результате 
        /// воздействия высокочастотных циклов для объекта диагностирования "Барабан" 
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="code">Код объекта</param>
        /// <param name="timeHFC">Дата и время (выходной параметр)</param>
        /// <param name="lifeHFC">Текущее значение ресурса (выходной параметр)</param>
        void GetLifeHFCCurrent(int boiler_id, string code, out DateTime timeHFC,
            out double lifeHFC);

        /// <summary>
        /// Получение исчерпания ресурса в результате воздействия малоцикловой 
        /// усталости для объекта диагностирования 
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="code">Код объекта</param>
        /// <param name="start">Начало интервала времени</param>
        /// <param name="finish">Конец интервала времени</param>
        /// При вызове функции необходимо параметры времени передавать как именованные, 
        /// при этом если задается:
        /// Только start - выбираются все значения от заданного момента времени и до текущего
        /// Только finish - выбираются все значения от начального и до заданного момента времени
        /// start и finish - выбираются все значения за заданный интервал времени
        /// Не заданы параметры времени - выбираются все существующие значения 
        /// <returns>Таблица БД, где:
        /// Code - Код объекта;
        /// TimeLFC - Дата и время;
        /// LifeLFC - Значение ресурса;
        /// StressAmplitude - Значение амплитуды МЦУ;
        /// </returns>                      
        DataTable GetLifeLFC(int boiler_id, string code,
           DateTime? start, DateTime? finish);

        /// <summary>
        /// Получение суммарного (текущего) значения исчерпания ресурса в результате 
        /// воздействия малоцикловой усталости для объекта диагностирования
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="code">Код объекта</param>
        /// <param name="timeLFC">Дата и время (выходной параметр)</param>
        /// <param name="lifeLFC">Значение ресурса (выходной параметр)</param>
        void GetLifeLFCCurrent(int boiler_id, string code,
            out DateTime timeLFC, out double lifeLFC);

        /// <summary> 
        /// Получение исчерпания ресурса (по каждому виду ресурса) относительно
        /// циклов "пуск-останов" котла для объекта диагностирования 
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="code">Код объекта</param>
        /// <param name="start">Начало интервала времени</param>
        /// <param name="finish">Конец интервала времени</param>
        /// при этом если задается:
        /// только start - выбираются все значения от заданного момента времени и до текущего
        /// только stop - выбираются все значения от начального и до заданного момента времени
        /// start и stop - выбираются все значения за заданный интервал времени
        /// не заданы параметры времени - выбираются все существующие значения 
        /// <returns>Таблица БД, где:
        /// Code - Код объекта
        /// TimeStart - Дата и время пуска котла
        /// TimeStop - Дата и время останова котла
        /// LifeTotal - Сумма значений ресурсов
        /// LifeBasicCycle - Значение ресурса за основной цикл (пуск-останов)
        /// LifeLFC - Суммарный ресурс малочастотных циклов за период пуск-останов
        /// LifeHFC_or_Creep - Суммарный ресурс высокочаст. циклов или ползучести 
        /// за период пуск-останов (в зависимости от типа объекта)
        /// StressAmplitude - Значение амплитуды основного цикла 
        /// </returns>                     
        DataTable GetLifeStartStop(int boiler_id, string code,
            DateTime? start, DateTime? finish);

        /// <summary>
        /// Получение суммарного (текущего) исчерпания ресурса (по каждому виду ресурса) 
        /// относительно циклов "пуск-останов" котла для объекта диагностирования 
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="code">Код объекта</param>
        /// <param name="start">Дата и время пуска котла (выходной параметр)</param>
        /// <param name="stop">Дата и время останова котла (выходной параметр)</param> 
        /// <param name="lifeTotal">Значение ресурса за основной цикл (выходной параметр)</param>
        /// <param name="lifeBasicCycle">Суммарное значение ресурса (выходной параметр)</param>
        /// <param name="lifeLFC">Суммарный ресурс малочастотных циклов за 
        /// период пуск-останов (выходной параметр)</param>
        /// <param name="lifeHFC_or_Creep">Суммарный ресурс высокочастотных циклов или ползучести
        ///  за период пуск-останов (в зависимости от типа объекта) (выходной параметр)</param>   
        void GetLifeStartStopCurrent(int boiler_id, string code, out DateTime start,
            out DateTime stop, out double lifeTotal, out double lifeBasicCycle,
            out double lifeLFC, out double lifeHFC_or_Creep);

        /// <summary>
        /// Получение информации о количестве пусков котла из различных состояний
        /// </summary>
        /// <param name="boiler_id">Уникальный идентификатор котла</param>
        /// <param name="totalFact">Всего кол-во пусков фактическое (выходной параметр)</param>
        /// <param name="coldStateFact">Кол-во пусков из холодного состояния фактическое (выходной параметр)</param>
        /// <param name="notCoolStateFact">Кол-во пусков из неостывшего состояния фактическое (выходной параметр)</param>
        /// <param name="hotStateFact">Кол-во пусков из горячего состояния фактическое (выходной параметр)</param>
        /// <param name="hydrotesting">Кол-во гидроопресовок (выходной параметр)</param>
        /// <param name="coldStatePermit">Кол-во пусков из холодного состояния разрешенное (выходной параметр)</param>
        /// <param name="notCoolHotStatePermit">Кол-во пусков из неостывшего состояния разрешенное (выходной параметр)</param>
        void GetStartCount(int boiler_id, out int totalFact, out int coldStateFact,
            out int notCoolStateFact, out int hotStateFact, out int hydrotesting,
            out int coldStatePermit, out int notCoolHotStatePermit);

        /// <summary>
        /// Получение журнала сообщений
        /// </summary>
        /// <param name="type">Тип сообщения</param>
        /// <param name="start">Начало интервала времени появления сообщений</param>
        /// <param name="finish">Конец интервала времени появления сообщений</param>
        /// при этом если задается:
        /// только start - выбираются все сообщения от заданного момента времени и до текущего
        /// только stop - выбираются все сообщения от начального и до заданного момента времени
        /// start и stop - выбираются все сообщения за заданный интервал времени
        /// не заданы параметры времени - выбираются все существующие сообщения 
        /// <returns>Таблица БД, где:
        /// Type - Тип сообщения;
        /// TimeMessage - Время сообщения;
        /// Source - Источник сообщения;
        /// Event - Событие, приведшее к появлению сообшения;
        /// Text - Текст сообщения;   
        /// </returns>                             
        DataTable GetMessageLog(String type, DateTime? start, DateTime? finish);

        /// <summary>
        /// Получение коэффициента - модуль нормальной упругости
        /// </summary>
        /// <returns>Таблица БД, где:
        /// Steel - Марка стали;
        /// Temperature - Значение температуры;
        /// ModulusElasticity - Значение коэффициента; 
        /// </returns>                                            
        DataTable GetModulusElasticity();

        /// <summary>
        /// Получение коэффициента - линейное расширение
        /// </summary>
        /// <returns>Таблица БД, где:
        /// Steel - Марка стали;
        /// Temperature - Значение температуры;
        /// LinearExpansion - Значение коэффициента;
        /// </returns>                             
        DataTable GetLinearExpansion();

        /// <summary>
        /// Получение коэффициента теплопроводности
        /// </summary>
        /// <returns>Таблица БД, где:
        /// Steel - Марка стали;
        /// Temperature - Значение температуры;
        /// ThermalConduct - Значение коэффициента;   
        /// </returns>                            
        DataTable GetThermalConduct();

        /// <summary>
        /// Получение коэффициента температуропроводности
        /// </summary>
        /// <returns>Таблица БД, где:
        /// Steel - Марка стали;
        /// Temperature - Значение температуры;
        /// ThermalConduct - Значение коэффициента;   
        /// </returns>                            
        DataTable GetTemperatureConduct();

        /// <summary>
        /// Получение значений допускаемых напряжений
        /// </summary>
        /// <returns>Таблица БД, где:
        /// Steel - Марка стали;
        /// Temperature - Значение температуры;
        /// CalculatedLife - Расчетный ресурс;
        /// Stress - Значение допускаемого напряжения;   
        /// </returns>                            
        DataTable GetPermissibleStress();

        /// <summary>
        /// Получение температуры ползучести для каждой марки стали
        /// </summary>
        /// <returns>Таблица БД, где:
        /// Steel - Марка стали;
        /// CalculatedLife - Расчетный ресурс;
        /// Temperature - Значение температуры;
        /// </returns>                            
        DataTable GetCreepTemperature();

        /// <summary>
        /// Получение коэффициента - показатель степени
        /// </summary>
        /// <returns>Таблица БД, где:
        /// Steel - Марка стали;
        /// Temperature - Значение температуры;
        /// Exponent - значение коэффициента;
        /// </returns>                            
        DataTable GetExponent();

        /// <summary>
        /// Получение коэффициента - снижение влияния овальности
        /// </summary>
        /// <returns>Таблица БД, где:
        /// Steel - Марка стали;
        /// Temperature - Значение температуры;
        /// Reducing - Значение коэффициента;  
        /// </returns>                             
        DataTable GetReducing();

        /// <summary>
        /// Получение постоянных расчетных коэффициентов
        /// </summary>
        /// <returns>Таблица БД, где:
        /// Name - Наименование константы;
        /// Symbol - Символьное обозначение константы; 
        /// Value - Значение константы;
        /// Description - Описание константы;     
        /// </returns>                             
        DataTable GetConstant();

        /// <summary>
        /// Получение списка электростанций с указанием их внутреннего идентификатора
        /// </summary>
        /// <returns>Таблица БД, где:
        /// PlantId - Идентификационный номер;
        /// PlantName - Название;
        /// </returns>                      
        DataTable GetListPlant();

        /// <summary>
        /// Добавление информации об электростанции в расчетные модули.
        /// Выполняется для синхронизации идентификаторов электростанции (Расчетные модули-Система мониторинга) 
        /// </summary>
        /// <param name="plantKotesId">Уникальный идентификатор электростанции в системе мониторинга</param>
        /// <param name="plantName">Название</param>
        /// <param name="plantEMAllianceId">Уникальный идентификатор электростанции в расчетных модулях 
        /// (Если это новая станция, то необходимо передавать 0, а после выполнения ф-ции данный 
        /// параметр будет иметь значение уникального идентификатора станции в расчетных модулях)
        /// </param>
        void AddInfoPlant(int plantKotesId, string plantName, ref int plantEMAllianceId);

        /// <summary>
        /// Получение списка котлов для заданной электростанции с указанием их внутренних идентификаторов
        /// </summary>
        /// <param name="plantEMAllianceId">ИН станции в расчетных модулях</param>
        /// <returns>Таблица БД, где:
        /// BoilerId - Уникальный идентификатор;
        /// BoilerName - Название (обычно его станционный номер);
        ///</returns>
        DataTable GetListBoiler(int plantEMAllianceId);

        /// <summary>
        /// Добавление информации о котле в расчетные модули.
        /// Выполняется для синхронизации идентификаторов кота (Расчетные модули-Система мониторинга) 
        /// </summary>
        /// <param name="plantEMAllianceId">Уникальный идентификатор электростанции в расчетных модулях</param>
        /// <param name="boilerKotesId">Уникальный идентификатор котла в системе мониторинга</param>
        /// <param name="boilerName">Название котла</param>
        /// <param name="boilerEMAllianceId">Уникальный идентификатор котла в в расчетных модулях
        /// (Если это новый котел, то необходимо передавать 0, а после выполнения ф-ции данный 
        /// параметр будет иметь значение уникального идентификатора котла в расчетных модулях)</param>
        void AddInfoBoiler(int plantEMAllianceId, int boilerKotesId, string boilerName, ref int boilerEMAllianceId);
    }
}

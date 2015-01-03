using System;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Контест выполнения. (Расчитываемый параметр, функции и т.д.)
    /// </summary>
    public interface ICallContext
    {
        /// <summary>
        /// Получить текущую выполняемую инструкцию
        /// </summary>
        Instruction CurrentInstruction { get; }

        /// <summary>
        /// Получить положение текущей операции 
        /// (для сообщения об ошибке и для дебага, если он когда ни будь будет)
        /// </summary>
        Location CurrentLocation { get; }

        /// <summary>
        /// Информация о текущем расчитываемом элементе 
        /// (для сообщений об ошибке)
        /// </summary>
        CalcPosition CurrentNode { get; }

        /// <summary>
        /// Получить начальное время всего расчитываемого интервала
        /// </summary>
        DateTime CalcStartTime { get; }

        /// <summary>
        /// Получить конечное время всего расчитываемого интервала
        /// </summary>
        DateTime CalcEndTime { get; }

        /// <summary>
        /// Получить начальное время текущего расчитываемого интервала
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// Получить конечное время текущего расчитываемого интервала
        /// </summary>
        DateTime EndTime { get; }

        /// <summary>
        /// Получить начальное время текущего расчитываемого интервала с задержкой
        /// </summary>
        /// <param name="tau">Задержка в количестве интервалов</param>
        /// <returns></returns>
        DateTime GetStartTime(int tau);

        /// <summary>
        /// Получить проодолжительность текущего расчитываемого интервала
        /// </summary>
        /// <param name="tau">Задержка в количестве интервалов</param>
        /// <param name="tautill">Задержка в количестве интервалов до конца интервала</param>
        /// <returns></returns>
        Interval GetInterval(int tau, int tautill);

        /// <summary>
        /// Сигнализировать, что расчёт вошёл в цикл.
        /// Для борьбы с бесконечными циклами.
        /// </summary>
        void LoopEnter();

        /// <summary>
        /// Сигнализировать, что расчёт вышел из цикла.
        /// Для борьбы с бесконечными циклами.
        /// </summary>
        void LoopLeave();

        /// <summary>
        /// Увеличить счётчик текущего цикла.
        /// Для борьбы с бесконечными циклами.
        /// </summary>
        /// <returns>Текущие показания счётчика текущего цикла</returns>
        int LoopPass();

        /// <summary>
        /// Получить флаг, требуется ли вызвать метод Prepare
        /// </summary>
        bool NeedPrepare { get; }

        /// <summary>
        /// Подготовить внутренние состояния контекста 
        /// и, по необходимости, вызвать зависимости или выйти из расчёта текущего узла.
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        void Prepare(ICalcContext calcContext);

        /// <summary>
        /// Прекратить выполнение текущего узла.
        /// Сделать действия необходимые при завершении (для параметра сохранить значения в кэш).
        /// И по необходимости выйти из расчёта узла, или выставить флаг NeedPrepare
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        void Return(ICalcContext calcContext);

        /// <summary>
        /// Выставить флаг неудачи расчёта
        /// </summary>
        bool Fail { get; set; }

        /// <summary>
        /// Флаг блокировки изменения значения 
        /// (если не доступен блочный)
        /// </summary>
        bool Blocked { get; set; }

        /// <summary>
        /// Перейти к следующей инструкции
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <returns></returns>
        bool NextInstruction(ICalcContext calcContext);

        /// <summary>
        /// Перейти к указанной инструкции
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <param name="index">Номер инструкции</param>
        /// <returns></returns>
        bool Jump(ICalcContext calcContext, int index);

        /// <summary>
        /// Перейти к указанной инструкции по относительной адресации
        /// </summary>
        /// <param name="calcContext">Контекст расчёта</param>
        /// <param name="delta">Смещение</param>
        /// <returns></returns>
        bool JumpShift(ICalcContext calcContext, int delta);

        /// <summary>
        /// Получить состояние расчёта
        /// </summary>
        /// <returns></returns>
        string GetStatusString();

        /// <summary>
        /// Получить состояние расчёта. 
        /// Состояние таблицы символов,
        /// стэка расчёта (с листингом исполняемого кода)
        /// </summary>
        /// <returns></returns>
        string ContextReport();

        /// <summary>
        /// Проверить контекст на наличие рекурсии.
        /// </summary>
        /// <param name="context">Контекст, расположеный где-то ниже в стеке вызова</param>
        /// <returns>
        /// Если текущий контекст не может быть вызван после данного, вернуть true.
        /// <br />
        /// Если вызов текущего контекста не конфликтует с данным, верунть false.
        /// </returns>
        bool TheSame(ICallContext context);
    }
}

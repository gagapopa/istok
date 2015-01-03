using System;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Функция вызываемая во время расчета, внешняя по отношению к модулю расчета
    /// </summary>
    public interface IExternalFunctionInfo
    {
        /// <summary>
        /// Имя функции
        /// </summary>
        String Name { get; }

        RevisionInfo Revision { get; }

        /// <summary>
        /// Описание функции
        /// </summary>
        String Description { get; }

        /// <summary>
        /// Группа функции (для группировки функций в редакторе формул)
        /// </summary>
        String GroupName { get; }

        /// <summary>
        /// Аргументы функции
        /// </summary>
        CalcArgumentInfo[] Arguments { get; }

        /// <summary>
        /// Вызвать функцию
        /// </summary>
        /// <param name="args">аргументы</param>
        /// <returns>Результат функции</returns>
        Object Call(Object[] args);
    }
}

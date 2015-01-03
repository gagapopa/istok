using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Метод определения возможных значений аргумента оптимизации
    /// </summary>
    public enum OptimizationArgumentMode
    {
        /// <summary>
        /// 
        /// </summary>
        Default, 
        
        /// <summary>
        /// Значения вводятся через ручной ввод
        /// </summary>
        Manual, 
        
        /// <summary>
        /// Заданный интервал
        /// </summary>
        Interval, 
        
        /// <summary>
        /// Значения расчитываются в выражении
        /// </summary>
        Expression, 
        
        /// <summary>
        /// При ручном вводе значений, номер набора значений
        /// </summary>
        ColumnNum
    }

    /// <summary>
    /// Информация о аргументе оптимизации
    /// </summary>
    public interface IOptimizationArgument
    {
        /// <summary>
        /// Имя аргумента
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// Выражение для расчета возможных значений аргумента
        /// </summary>
        String Expression { get; set; }

        /// <summary>
        /// Метод получения возможных значений аргумента
        /// </summary>
        OptimizationArgumentMode Mode { get; set; }

        /// <summary>
        /// Начальное значение интервала
        /// </summary>
        double IntervalFrom { get; set; }

        /// <summary>
        /// Конечное значение интервала
        /// </summary>
        double IntervalTo { get; set; }

        /// <summary>
        /// Шаг интервала
        /// </summary>
        double IntervalStep { get; set; }
    }
}


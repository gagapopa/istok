using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Источник данных для отчёта
    /// </summary>
    public interface IReportSource
    {
        ReportSourceInfo Info { get; }

        /// <summary>
        /// Зависимости источника данных по отношению к другим источникам.
        /// </summary>
        /// <remarks>
        /// Данное свойство является запросом на зависимость.
        /// Оно будет запрошено один раз при инициализации системы.
        /// Затем, по возможности, требуемые зависимости будут удовлетворены через вызов метода SetReference().
        /// </remarks>
        Guid[] References { get; }

        /// <summary>
        /// Удовлетворить зависимость источника данных
        /// </summary>
        /// <param name="source">Источник данных, от которого зависит дданный.</param>
        /// <remarks>
        /// Данные методу будут передоваться согластно свойству References.
        /// </remarks>
        void SetReference(IReportSource source);

        /// <summary>
        /// Настройки источника данных, которые можно изменить в окне редактирования отчёта
        /// </summary>
        ReportSourceSettings CreateSettingsObject();

        /// <summary>
        /// Сформировать данные для отображения, согласно значениям свойств
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="settings">Настройки источника данных отчёта</param>
        /// <param name="reportParameters">Свойства формируемого отчёта</param>
        void GenerateData(OperationState state, DataSet dataSet, ReportSourceSettings settings, params ReportParameter[] reportParameters);

        /// <summary>
        /// Сформировать пустые (тестовые) данные для отображения.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="dataSet"></param>
        /// <param name="settings">Настройки источника данных отчёта</param>
        void GenerateEmptyData(OperationState state, DataSet dataSet, ReportSourceSettings settings);
    }
}

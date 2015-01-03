using System;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Extension;
using System.Threading.Tasks;

namespace COTES.ISTOK.Client.Extension
{
    /// <summary>
    /// Обратная связь расширения с интерфейсным модлем
    /// </summary>
    public interface IClientState
    {
        /// <summary>
        /// Текущая активная униформа
        /// </summary>
        IUniForm ActiveUniForm { get; }

        /// <summary>
        /// Событие, возникающие при смене текущей унифомы
        /// </summary>
        event EventHandler ActiveUniFormChanged;

        /// <summary>
        /// Отобразить окно в MDI
        /// </summary>
        /// <param name="form"></param>
        void ShowMdi(Form form);

        /// <summary>
        /// Получить тип по GUID типа
        /// </summary>
        /// <param name="typeGUID">GUID типа</param>
        /// <returns></returns>
        UTypeNode GetExtensionType(Guid typeGUID);

        /// <summary>
        /// Запросить ближайший родительский узел определенного типа
        /// </summary>
        /// <param name="unitNode">Начальный узел</param>
        /// <param name="unitTypeId">Тип искомого узла</param>
        /// <returns></returns>
        UnitNode GetParent(UnitNode unitNode, int unitTypeId);

        /// <summary>
        /// Запросить список таблиц для указанного узла
        /// </summary>
        /// <param name="extensionUnitNode"></param>
        /// <returns></returns>
        ExtensionDataInfo[] GetExtensionExtendedTableInfo(ExtensionUnitNode extensionUnitNode);

        /// <summary>
        /// Запросить список таблиц не связанных с каким-нибудь узлом
        /// </summary>
        /// <param name="extensionName">Название расширения</param>
        /// <returns></returns>
        ExtensionDataInfo[] GetExtensionExtendedTableInfo(String extensionName);

        /// <summary>
        /// Запросить данные для таблицы
        /// </summary>
        /// <param name="unitNode">Узел</param>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="dateFrom">Начальное запрашиваемое время</param>
        /// <param name="dateTo">Конечное запрашиваемое время</param>
        /// <returns></returns>
        ExtensionData GetExtensionExtendedTable(ExtensionUnitNode unitNode, String tableName, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// Запросить данные для таблицы
        /// </summary>
        /// <param name="unitNode">Узел</param>
        /// <param name="tableInfo">Метаданные для таблицы</param>
        /// <param name="dateFrom">Начальное запрашиваемое время</param>
        /// <param name="dateTo">Конечное запрашиваемое время</param>
        /// <returns></returns>
        ExtensionData GetExtensionExtendedTable(ExtensionUnitNode unitNode, ExtensionDataInfo tableInfo, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// Запросить данные для таблицы
        /// </summary>
        /// <param name="unitNode">Узел</param>
        /// <param name="tableName">Имя таблицы</param>
        /// <returns></returns>
        ExtensionData GetExtensionExtendedTable(ExtensionUnitNode unitNode, String tableName);

        /// <summary>
        /// Запросить данные для таблицы
        /// </summary>
        /// <param name="extensionName">Название расширения</param>
        /// <param name="tableInfo">Метаданные для таблицы</param>
        /// <returns></returns>
        ExtensionData GetExtensionExtendedTable(String extensionName, ExtensionDataInfo tableInfo);
    }
}

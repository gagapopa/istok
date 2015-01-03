using System;
using System.Data;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Extension
{
    /// <summary>
    /// Интерфейс для расширений станционного сервера
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// Обратная связь для запросов к серверу
        /// </summary>
        IExtensionSupplier Supplier { get; set; }
        
        /// <summary>
        /// Заголовок расширения
        /// </summary>
        string Caption { get; }

        #region Работа со структурой
        /// <summary>
        /// Типы Узлов, предоставляемый расширением
        /// </summary>
        UTypeNode[] ProvidedTypes { get; }
        
        /// <summary>
        /// Создать новый узел, предоставляемый расширением
        /// </summary>
        /// <param name="type">Тип нового узла</param>
        /// <returns></returns>
        ExtensionUnitNode NewUnitNode(UTypeNode type);
        
        /// <summary>
        /// Создать узел, предоставляемый расширением, по данным полученным из БД
        /// </summary>
        /// <param name="type">Тип узла</param>
        /// <param name="row">Строка из БД</param>
        /// <returns></returns>
        ExtensionUnitNode NewUnitNode(UTypeNode type, DataRow row);

        /// <summary>
        /// Получить данные при сохранении узла в системе
        /// </summary>
        /// <param name="unitNode">Узел, предоставляемый расширением</param>
        void NotifyChange(ExtensionUnitNode unitNode); 
        #endregion

        #region Дополнительные свойства и связь узлов с внешними ИД
        /// <summary>
        /// Получить дополнительные свойства узла
        /// </summary>
        /// <param name="unitNode">Узел, предоставляемый расширением</param>
        /// <returns></returns>
        ItemProperty[] GetProperties(ExtensionUnitNode unitNode);

        /// <summary>
        /// Поддерживается ли для данного узла выбор ИД из расширения
        /// </summary>
        /// <param name="typeNode"></param>
        /// <returns></returns>
        bool ListSupported(UTypeNode typeNode);

        /// <summary>
        /// Отображается ли для данного узла код из внешней системы
        /// </summary>
        /// <param name="typeNode"></param>
        /// <returns></returns>
        bool CodeSupported(UTypeNode typeNode);

        /// <summary>
        /// Поддерживается ли добавление нового элемента в список ИД внешнего расширения
        /// </summary>
        /// <param name="typeNode"></param>
        /// <returns></returns>
        bool ListCanAdd(UTypeNode typeNode);
        
        /// <summary>
        /// Запросить список ИД из расширения
        /// </summary>
        /// <param name="unitNode">Узел, предоставляемый расширением</param>
        /// <param name="type"></param>
        /// <returns></returns>
        EntityStruct[] GetList(ExtensionUnitNode unitNode, UTypeNode type); 
        #endregion

        #region Данные из расширения
        /// <summary>
        /// Получить информацию о всех данных из расширения.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Данный метод вызывается при редактировании отчётов.
        /// </remarks>
        ExtensionDataInfo[] GetAllDataInfo();

        /// <summary>
        /// Получить информацию о данных из расширения, независящие от конкретного узла.
        /// </summary>
        /// <returns></returns>
        ExtensionDataInfo[] GetDataInfo();
        
        /// <summary>
        /// Получить информацию о данных из расширения, зависящие от конкретного узла.
        /// </summary>
        /// <param name="unitNode">Узел, предоставляемый расширением</param>
        /// <returns></returns>
        /// <remarks>
        /// Данный метод будет вызываться при отображении данных в интерфейсном модуле
        /// для создания вкладок с данными.
        /// </remarks>
        ExtensionDataInfo[] GetDataInfo(ExtensionUnitNode unitNode);
        
        /// <summary>
        /// Запросить данные из расширения
        /// </summary>
        /// <param name="unitNode">Узел, предоставляемый расширением, если требуется</param>
        /// <param name="extensionDataInfo">Название данных, полученно методами GetDataInfo() или GetAllDataInfo()</param>
        /// <param name="beginTime">Начальное запрашиваемое время</param>
        /// <param name="endTime">Конечное запрашиваемое время</param>
        /// <returns></returns>
        ExtensionData GetData(ExtensionUnitNode unitNode, ExtensionDataInfo extensionDataInfo, DateTime beginTime, DateTime endTime); 
        #endregion

        /// <summary>
        /// Запросить сообщения из расширения.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        DataTable GetMessages(MessageCategory filter, DateTime startTime, DateTime endTime);
    }
}

using System;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Assignment
{
    interface IUnitTypeManager
    {
        #region Загрузка типов
        /// <summary>
        /// Загрузить доступные в системе типы.
        /// В том числе запрашиваются типы из расширений методом ExtensionManager.GetExtensionUnitType().
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        void LoadTypes(OperationState state);
        #endregion

        #region Редактирование типов
        /// <summary>
        /// Добавить новый тип оборудования
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitTypeNode">Тип оборудования для добавления</param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UTypeNode AddUnitType(OperationState state, UTypeNode unitTypeNode);

        /// <summary>
        /// Сохранить изменения типа оборудования
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitTypeNode">Тип оборудования для сохранения</param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UTypeNode UpdateUnitType(OperationState state, UTypeNode unitTypeNode);

        /// <summary>
        /// Удалить тип оборудования
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitTypeNodeID">Идентификатор типа оборудования для удаления</param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        void RemoveUnitType(OperationState state, int unitTypeNodeID);
        #endregion

        #region Запрос типов
        /// <summary>
        /// Получить экземпляр типа по идентификатору
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="type">ИД типа</param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        UTypeNode GetUnitType(OperationState state, int type);

        /// <summary>
        /// Запрос типов оборудования
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        UTypeNode[] GetUnitTypes(OperationState state);
        #endregion

        /// <summary>
        /// Получить ИД типа в системе по его идентифекатору из расширения
        /// </summary>
        /// <param name="parentGUID">ИД типа из расширения</param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        int GetExtensionType(Guid parentGUID);
        
        /// <summary>
        /// Событие возникает при изменении типа
        /// </summary>
        event EventHandler<UnitTypeEventArgs> UnitTypeChanged;
    }
}

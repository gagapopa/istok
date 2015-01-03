using System;
using System.Collections.Generic;
using System.Data;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Assignment
{
    interface IUnitManager
    {
        #region Загрузка структуры
        /// <summary>
        /// Загрузить структуру
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        void LoadUnits(OperationState state);

        /// <summary>
        /// Создать новый экземпляр элемента структуры по указанному типу
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="type">Тип для нового элемента</param>
        /// <returns>
        /// Пустой иницированный элемент требуемого типа.
        /// </returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        UnitNode NewInstance(OperationState state, int type);
        #endregion

        #region Редактирование узлов
        /// <summary>
        /// Добавить узлы в указанный элемент
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="nodes">Добавляемые элементы.</param>
        /// <param name="parent">ИД родительского элемента, 0, ксли корень.</param>
        /// <returns>
        /// Добавленные элементы
        /// </returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не может редактировать элемент parent</exception>
        IEnumerable<UnitNode> AddUnitNode(OperationState state, IEnumerable<UnitNode> nodes, int parent);

        /// <summary>
        /// Сохранить изменения узла
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="updnode">Отредактированный узел.</param>
        /// <returns>Новую версию узла после сохранения.</returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UnitNode UpdateUnitNode(OperationState state, UnitNode updnode);
        
        /// <summary>
        /// Удалить указанные узлы из структуры
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="remnodes">ИД удаляемых узлов</param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        void RemoveUnitNode(OperationState state, int[] remnodes);

        /// <summary>
        /// Копировать узел или ветку узлов
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitIDs">ИД копируемых узлов или корневых элементов ветки</param>
        /// <param name="recursive">true, если копируется ветки и false - отдельные элементы</param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        void CopyUnitNode(OperationState state, int[] unitIDs, bool recursive);

        /// <summary>
        /// Переместить узел в другой родительский узел
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="newParentId">ИД нового родителя</param>
        /// <param name="unitNodeId">ИД переносимого узла</param>
        /// <param name="index">
        /// Индекс в новом родительском узле.
        /// Если index &lt; 0, добавить в конец.
        /// </param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        void MoveUnitNode(OperationState state, int newParentId, int unitNodeId, int index);
        #endregion

        #region Импорт
        /// <summary>
        /// Сохранение импортированных узлов
        /// </summary>
        /// <param name="state"></param>
        /// <param name="importRootNode">
        /// Базовый узел, в который будут импортироваться данные. 
        /// Для импорта в корень - должен быть null.
        /// </param>
        /// <param name="importContainer">Обертка добавляемого дерева</param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        void ApplyImport(OperationState state, UnitNode importRootNode, ImportDataContainer importContainer); 
        #endregion

        #region Запрос структуры
        /// <summary>
        /// Найти узел по ИД среди дочерних узлов
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="parentNode">Узел в ветке которого ведётся поиск</param>
        /// <param name="unitNodeID">ИД искомого параметра</param>
        /// <returns>Найденный узел или null, если ничего не найдено</returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UnitNode Find(OperationState state, UnitNode parentNode, int unitNodeID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitId"></param>
        /// <param name="filterTypes"></param>
        /// <param name="privileges"></param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UnitNode GetUnitNode(OperationState state, int unitId, int[] filterTypes, Privileges privileges);

        /// <summary>
        /// Ищет дочерние ноды по фильтру.
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="id"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UnitNode[] GetUnitNodes(OperationState state, int id, ParameterFilter filter, RevisionInfo revision);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="id"></param>
        /// <param name="filterTypes"></param>
        /// <param name="privileges"></param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UnitNode[] GetUnitNodes(OperationState state, int id, int[] filterTypes, Privileges privileges);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UnitNode[] GetUnitNodes(OperationState state, int[] ids);

        /// <summary>
        /// Найти все дочерние узлы с применением фильтра по типам
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="id">ИД корневого узла</param>
        /// <param name="filterTypes">Фильтр типов</param>
        /// <param name="privileges">Фильтр по правам доступа</param>
        /// <returns>Дочерние элементы удовлетворяющие критерию</returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UnitNode[] GetAllUnitNodes(OperationState state, int id, int[] filterTypes, Privileges privileges);

        /// <summary>
        /// Найти все дочерние узлы с применением фильтра по типам
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="id">ИД корневого узла</param>
        /// <param name="filterTypes">Фильтр типов</param>
        /// <param name="minLevel"></param>
        /// <param name="maxLevel"></param>
        /// <param name="privileges">Фильтр по правам доступа</param>
        /// <returns>Плоский список всех дочерних элементов уодвлетворяющих критерий.</returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UnitNode[] GetAllUnitNodes(OperationState state, int id, int[] filterTypes, int minLevel, int maxLevel, Privileges privileges);

        /// <summary>
        /// Найти ближайшего родителя узла соответсвующего типа
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitNode">Узел с которого начинается поиск</param>
        /// <param name="unitTypeId">Искомый тип</param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UnitNode GetParentNode(OperationState state, UnitNode unitNode, int unitTypeId);

        /// <summary>
        /// Получить плоский список всех дочерних элементов с указанным типом
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitNode"></param>
        /// <param name="unitTypeId">Искомый тип</param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UnitNode[] GetChildNodes(OperationState state, UnitNode unitNode, int unitTypeId);
        #endregion

        /// <summary>
        /// Получить структуру за один запрос
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitIds">ИД корневых узлов</param>
        /// <param name="filterTypes">Фильтр типов</param>
        /// <param name="privileges">Фильтр по правам доступа</param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        TreeWrapp<UnitNode>[] GetUnitNodeTree(OperationState state, int[] unitIds, int[] filterTypes, Privileges privileges);

        #region Запрос специфичных элементов
        /// <summary>
        /// Получить все нормативные графики
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <returns></returns>
        NormFuncNode[] GetNormFuncs(OperationState state);

        /// <summary>
        /// Получить параметр по коду
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="code"></param>
        /// <returns></returns>
        ParameterNode GetParameter(OperationState state, string code);

        /// <summary>
        /// Получить все параметры
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <returns></returns>
        IEnumerable<ParameterNode> GetParameters(OperationState state);

        #endregion

        #region Сбрасывание справочников
        DataSet GetBlockData(OperationState state, int id_block);
        DataSet GetBlockData(OperationState state, int id_block, int id_chanell);
        byte[] SerializeDataSet(OperationState state, DataSet ds); 
        #endregion

        string GetFullName(UnitNode unitNode);

        /// <summary>
        /// Получить список аргументов для оптимизации указанного узла
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitNode"></param>
        /// <returns></returns>
        string[] GetRequiredArguments(OperationState state, UnitNode unitNode);
        
        Dictionary<int, int> GetStatistic(OperationState state, UnitNode unitNode);

        #region Расписание
        ParameterNode[] GetParametersForSchedule(OperationState state, string uid, int schedule_id); 
        #endregion

        #region Получить и проверить узел
        /// <summary>
        /// Переберает элементы по иерархии вверх в поисках элемента с подходящим типом.
        /// Поиск начинается с переданного в unitNodeID элемента. 
        /// Если он подходящего типа, то будет возвращён именно он.
        /// Так же проводится проверка на наличие у пользователя 
        /// запрашиваемых прав по отношению к найденному узлу.
        /// </summary>
        /// <typeparam name="T">
        /// Требуемый тип. 
        /// Должен наследоавться от UnitNode.
        /// </typeparam>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitNodeID">ИД искомого узла</param>
        /// <param name="privileges">Проверяемые права доступа</param>
        /// <returns>Элемент искомого типа.</returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает запрашиваемыми правами доступа.</exception>
        /// <exception cref="ISTOKException">Достигнут конец </exception>
        /// <exception cref="Exception">Узел не найден или не подходящего типа для каждого промежуточного типа</exception>
        T ValidateParentNode<T>(OperationState state, int unitNodeID, Privileges privileges) where T : UnitNode;

        T CheckParentNode<T>(OperationState operationState, int unitNodeID, Privileges privileges) where T : UnitNode;

        UnitNode ValidateUnitNode(OperationState state, int unitNodeID, Privileges privileges);

        /// <summary>
        /// Метод производит проверки на:
        /// <list type="bullet">
        /// <item>Наличие узла в справочнике;</item>
        /// <item>Узел соответсвует указанному типу;</item>
        /// <item>У пользователя есть требуемые права доступа по отношению к данному узлу.</item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">
        /// Требуемый тип. 
        /// Должен наследоавться от UnitNode.
        /// </typeparam>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitNodeID">ИД искомого узла</param>
        /// <param name="privileges">Проверяемые права доступа</param>
        /// <returns>Проверяемый узел</returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает запрашиваемыми правами доступа.</exception>
        /// <exception cref="Exception">Узел не найден или не подходящего типа</exception>
        T ValidateUnitNode<T>(OperationState state, int unitNodeID, Privileges privileges) where T : UnitNode;

        /// <summary>
        /// Метод производит проверки на:
        /// <list type="bullet">
        /// <item>Наличие узла в справочнике;</item>
        /// <item>Узел соответсвует указанному типу;</item>
        /// <item>У пользователя есть требуемые права доступа по отношению к данному узлу.</item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">
        /// Требуемый тип. 
        /// Должен наследоавться от UnitNode.
        /// </typeparam>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitNodeID">ИД искомого узла</param>
        /// <param name="privileges">Проверяемые права доступа</param>
        /// <returns>
        /// Проверяемый узел. 
        /// Если какая ни будь проверка увенчалась неудачой, вернет значение по умолчанию для T, 
        /// а в состояние асинхронной операции будет добавленно соответствующие сообщение.
        /// </returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        T CheckUnitNode<T>(OperationState state, int unitNodeID, Privileges privileges) where T : UnitNode; 
        #endregion

        #region События
        /// <summary>
        /// Событие возникает при изменении элемента структуры
        /// </summary>
        event EventHandler<UnitNodeEventArgs> UnitNodeChanged;

        /// <summary>
        /// Событие возникае при загрузки элемента структуры.
        /// При старте, а так же после сохранения, когда может произойти перезагрузка элемента
        /// </summary>
        event EventHandler<UnitNodeEventArgs> UnitNodeLoaded; 
        #endregion

        IntervalDescription[] GetStandardsIntervals(OperationState state);

        void RemoveStandardIntervals(OperationState state, IEnumerable<IntervalDescription> intervalsToRemove);

        void SaveStandardIntervals(OperationState state, IEnumerable<IntervalDescription> modifiedIntervals);
    }
}

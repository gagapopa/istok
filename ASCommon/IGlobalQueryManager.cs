using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using COTES.ISTOK;
using COTES.ISTOK.Calc;
using COTES.ISTOK.Extension;
using COTES.ISTOK.DiagnosticsInfo;

namespace COTES.ISTOK.ASC
{
	[ServiceKnownType("GetFiagTypes",typeof(HelperForDiagnosticType))]	
    [ServiceContract]
    public interface IGlobalQueryManager: ITestConnection<Object>
    {
        #region Работа с блочными

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<string[]> GetBlockUIDs(Guid userGUID);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ModuleInfo[]> GetModuleLibNames(Guid userGUID, int blockID);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ItemProperty[]> GetModuleLibraryProperties(Guid userGUID, int blockId, String libName);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ParameterItem[]> BeginGetChannelParameters(Guid userGUID, int channelNodeId);
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ParameterItem[]> EndGetChannelParameters(Guid userGUID, ulong operationId);

        #endregion

        #region Диагностика
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<COTES.ISTOK.DiagnosticsInfo.Diagnostics> GetDiagnosticsObject(Guid userGUID);
        #endregion

        #region Schema parts
        /// <summary> Регистрирует на сервере приложения клиента, 
        /// для обновления значений мнемосхем
        /// </summary>
        /// <param name="authority">Параметры авторизации</param>
        /// <param name="parameters">Массив технологических параметров, 
        /// требующихся клиенту</param>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<int> RegisterClient(Guid userGuid, ParamValueItemWithID[] parameters);

        /// <summary>
        /// Разрегистрация массива параметров
        /// </summary>
        /// <param name="taID">Номер транзакции</param>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer UnRegisterClient(Guid userGuid, int taID);

        /// <summary>
        /// Получение значений параметров
        /// </summary>
        /// <param name="taID">Номер транзакции</param>
        /// <returns>Массив параметров со значениями</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ParamValueItemWithID[]> GetValuesFromBank(Guid userGuid, int taID);
        #endregion

        /// <summary>
        /// Создаёт новую сессию для пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns>Идентификатор сессии</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        [FaultContract(typeof(NoOneUserException))]
        ConnectReturnContainer Connect(String userName, String password);

        /// <summary>
        /// Удаляет информацию о сессии клиента
        /// </summary>
        /// <param name="sessionGuid">Идентификатор сессии</param>
        [OperationContract]
        void Disconnect(Guid sessionGuid);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UserNode> GetUser(Guid userGUID);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer Alive(Guid userGUID);

        #region Блокировки
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        [FaultContract(typeof(LockExceptionFault))]
        ReturnContainer LockNode(Guid userGUID, int nodeID);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer ReleaseNode(Guid userGUID, int nodeID);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        [FaultContract(typeof(LockExceptionFault))]
        ReturnContainer LockValues(Guid userGUID, int nodeID, DateTime startTime, DateTime endTime);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer ReleaseValues(Guid userGUID, int nodeID, DateTime startTime, DateTime endTime);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer ReleaseAll(Guid userGUID, int nodeID);
        #endregion

        /// <summary>
        /// Получить имя пользователя по UID
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="userID">Идентификатор пользователя</param>
        /// <returns>Имя пользователя</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<string> GetUserLogin(Guid userGUID, int userID);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<double> GetLoadProgress(Guid userGUID);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<string> GetLoadStatusString(Guid userGUID);

        #region Интеграция логов

        #endregion

        #region Работа с типами
        /// <summary>
        /// Запросить типы оборудования
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UTypeNode[]> GetUnitTypes(Guid userGUID);

        /// <summary>
        /// Изменить тип оборудования
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitTypeNode">Тип оборудования для сохранения</param>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer UpdateUnitType(Guid userGUID, UTypeNode unitTypeNode);

        /// <summary>
        /// Добавить тип оборудования
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitTypeNode">Тип оборудования для добавления</param>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer AddUnitType(Guid userGUID, UTypeNode unitTypeNode);

        /// <summary>
        /// Удалить тип оборудования
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitTypeNodesIDs">Идентификатор удаляемых типов оборудования</param>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer RemoveUnitType(Guid userGUID, int[] unitTypeNodesIDs);
        #endregion

        #region Работа с пользователями
        /// <summary>
        /// Запросить пользователей системы
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UserNode[]> GetUserNodes(Guid userGUID);

        /// <summary>
        /// Запросить пользователей системы
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="ids">Массив ИДов</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UserNode[]> GetUserNodesByIds(Guid userGUID, int[] ids);

        /// <summary>
        /// Изменить пользователя системы
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="userNode">Редактируемый пользователь</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UserNode> UpdateUserNode(Guid userGUID, UserNode userNode);

        /// <summary>
        /// Добавить нового пользователя
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="userNode">Добавляемый пользователь</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UserNode> AddUserNode(Guid userGUID, UserNode userNode);

        /// <summary>
        /// Удалить пользователей
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="userNodeIDs">Динтификаторы удаляемых пользователей</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer RemoveUserNode(Guid userGUID, int[] userNodeIDs);

        /// <summary>
        /// Создать первого пользователя
        /// </summary>
        /// <param name="userNode">Узел добавляемого пользователя</param>
        [OperationContract]
        void NewAdmin(UserNode userNode);
        #endregion

        #region Работа с группами пользователей
        /// <summary>
        /// Запросить группы пользователей
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<GroupNode[]> GetGroupNodes(Guid userGUID);

        /// <summary>
        /// Добавить новую группу пользователей
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="groupNode">Добавляемая группа</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<GroupNode> AddGroupNode(Guid userGUID, GroupNode groupNode);

        /// <summary>
        /// Изменить группу пользователей
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="groupNode">Редактируемая группа</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<GroupNode> UpdateGroupNode(Guid userGUID, GroupNode groupNode);

        /// <summary>
        /// Удалить группы пользователей
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="groupNodeIDs">Идентификаторы удаляемых групп</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer RemoveGroupNode(Guid userGUID, int[] groupNodeIDs);
        #endregion

        #region Работа с ревизиями
        /// <summary>
        /// Запросить список ревизий
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<RevisionInfo[]> GetRevisions(Guid userGUID);

        ///// <summary>
        ///// Удалить ревизии
        ///// </summary>
        ///// <param name="userGUID">Идентификатор сессии пользователя</param>
        ///// <param name="revisionIDs">ИД удаляемых ревизий</param>
        //[OperationContract]
        //[FaultContract(typeof(UserNotConnectedException))]
        //ReturnContainer RemoveRevisions(Guid userGUID, int[] revisionIDs);

        ///// <summary>
        ///// Изменить или добавить ревизии
        ///// </summary>
        ///// <param name="userGUID">Идентификатор сессии пользователя</param>
        ///// <param name="revisions">Изменяемые ревизии</param>
        //[OperationContract]
        //[FaultContract(typeof(UserNotConnectedException))]
        //ReturnContainer UpdateRevisions(Guid userGUID, RevisionInfo[] revisions);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer UpdateRevisions(Guid userGUID, int[] revisionIDs, RevisionInfo[] revisions);

        #endregion

        #region Работа с юнит нодами

        /// <summary>
        /// Запускает асинхронную операцию
        /// получения списка идов дочерних узлов для нода.
        /// </summary>
        /// <param name="userGUID">
        ///     юзверь
        /// </param>
        /// <param name="parent">
        ///     Ид родительского узла.
        /// </param>
        /// <param name="filterTypes">
        ///     Фильтр типов в виде массива.
        /// </param>
        /// <returns>
        ///     Идентификатор асинхронной операции.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UnitNode[]> GetUnitNodesFiltered(Guid userGUID, int parent, int[] filterTypes);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UnitNode[]> GetUnitNodes(Guid userGUID, int[] ids);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UnitNode[]> GetAllUnitNodes(Guid userGUID, int parent, int[] filterTypes);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UnitNode[]> GetAllUnitNodesMinMax(Guid userGUID, int parent, int[] filterTypes, int minLevel, int maxLevel);

        /// <summary>
        /// Запускает асинхронную операцию
        /// получения списка дочерних узлов для нода, совпадающих по фильтру.
        /// </summary>
        /// <param name="userGUID">
        ///     юзверь
        /// </param>
        /// <param name="parent">
        ///     Ид родительского узла.
        /// </param>
        /// <param name="filter">
        ///     Фильтр параметров.
        /// </param>
        /// <returns>
        ///     Идентификатор асинхронной операции.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UnitNode[]> GetUnitNodes2(Guid userGUID, int parent, ParameterFilter filter, RevisionInfo revision);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UnitNode> GetUnitNodeFiltered(Guid userGUID, int unitId, int[] filterTypes);

        /// <summary>
        /// Запускает асинхронную операцию для получения
        /// унит нода.
        /// </summary>
        /// <param name="userGUID">
        ///     юзер
        /// </param>
        /// <param name="node">
        ///     Ид получаемого нода.
        /// </param>
        /// <returns>
        ///     Идентификатор асинхронной операции.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UnitNode> GetUnitNode(Guid userGUID, int node);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<TreeWrapp<UnitNode>[]> GetUnitNodeTree(Guid userGUID, int[] unitNodeIDs, int[] filterTypes, Privileges privileges);

        /// <summary>
        /// Запускает операцию по удалению массива нодов.
        /// </summary>
        /// <param name="userGUID">
        ///     юзер.
        /// </param>
        /// <param name="node">
        ///     Айдишки нодов.
        /// </param>
        /// <returns>
        ///     Айдишка асинхронной операции.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer RemoveUnitNodes(Guid userGUID, int[] nodes);
        
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        [FaultContract(typeof(LockExceptionFault))]
        ReturnContainer<ulong> BeginRemoveUnitNodes(Guid userGUID, int[] nodes);
        /// <summary>
        /// Запускает операцию на удаление нода.
        /// </summary>
        /// <param name="userGUID">
        ///     юзер.
        /// </param>
        /// <param name="node">
        ///     Айдишка нода.
        /// </param>
        /// <returns>
        ///     Айдишка асинхронной операции.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer RemoveUnitNode(Guid userGUID, int node);

        /// <summary>
        /// Добавление нода.
        /// </summary>
        /// <param name="userGuid">
        ///     Юзер.
        /// </param>
        /// <param name="unitType">
        ///     Тип нового нода.
        /// </param>
        /// <param name="parent">
        ///     Айдишка родительского нода.
        /// </param>
        /// <returns>ИД нового элемента</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<int> AddUnitNode(Guid userGuid, int unitType, int parent);

        /// <summary>
        /// Добавление нескольких нодов.
        /// </summary>
        /// <param name="userGuid">Юзер.</param>
        /// <param name="nodes">Ноды</param>
        /// <param name="parent">Айдишка родительского нода.</param>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer AddUnitNodes(Guid userGuid, UnitNode[] nodes, int parent);

        /// <summary>
        /// Запускает операцию на обновление нода.
        /// </summary>
        /// <param name="userGUID">
        ///     Юзер.
        /// </param>
        /// <param name="node">
        ///     Обновляемый нод
        /// </param>
        /// <returns>
        ///     Айдишка операции.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UnitNode> UpdateUnitNode(Guid userGUID, UnitNode node);

        /// <summary>
        /// Переместить узел в структуре
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="parentId">
        /// Идентификатор нового родительского узла.
        /// <br/>Если 0, перенос в корень;
        /// <br/>Для перемещения внутри родительского узла, требуется указать идентификатор старого родителя.
        /// </param>
        /// <param name="unitNodeId">Идентификатор перетаскиваемого узла</param>
        /// <param name="neighborId">
        /// ИД узла, рядом с котором разместить перетаскиваемый узел. 
        /// <br/>Если 0, то добавить в конец.
        /// </param>
        /// <param name="addAfter">
        /// Если destNodeID не нулевой, 
        /// то указывает разместить узел после или перед указаным.
        /// </param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer MoveUnitNode(Guid userGUID, int parentId, int unitNodeId, int neighborId, bool addAfter);

        /// <summary>
        /// Копироватьт элементы или ветки
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitIDs">Ид копируемых узлов</param>
        /// <param name="recursive">true, если копируется ветки и false - отдельные элементы</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer CopyUnitNode(Guid userGUID, int[] unitIDs, bool recursive);

        /// <summary>
        /// Экспорт узлов в файл
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="nodeIds">Экспортируемы узлы</param>
        /// <param name="beginValuesTime">Начальное время экспортируемых значений</param>
        /// <param name="endValuesTime">Конечное время экспортируемых значений</param>
        /// <param name="exportFormat">Формат файла</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ulong> BeginExport(Guid userGUID, int[] nodeIds, DateTime beginValuesTime, DateTime endValuesTime, ExportFormat exportFormat);

        /// <summary>
        /// Импортировать из файла узлы и передать их клиенту
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="buffer">Содержимое файла</param>
        /// <param name="exportFormat">Формат файла</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ImportDataContainer> BeginImport(Guid userGUID, byte[] buffer, ExportFormat exportFormat);

        /// <summary>
        /// Сохранить импортируемые узлы
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="rootNodeID">
        /// ИД узла, куда будут добавлены импортируемые данные.
        /// Для импорта в корень - 0.
        /// </param>
        /// <param name="importContainer">Обертка импортируемых узлов</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ulong> BeginApplyImport(Guid userGUID, int rootNodeID, ImportDataContainer importContainer);

        /// <summary>
        /// Изменить код в использующих его формулах
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="oldCode">Старый код</param>
        /// <param name="newCode">Новый код</param>
        /// <param name="unitNodesIds">Ид узлов в которых необходимо изменить код</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer ChangeParameterCode(Guid userGUID, string oldCode, string newCode, ParameterNodeReference[] unitNodesIds);
        #endregion

        #region Работа с расписаниями

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<Schedule[]> GetParamsSchedules(Guid userGUID);
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<Schedule> GetParamsSchedule(Guid userGUID, int id);
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<Schedule> GetParamsScheduleByName(Guid userGUID, string name);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer AddParamsSchedule(Guid userGUID, Schedule added);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer UpdateParamsSchedule(Guid userGUID, Schedule updated);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer DeleteParamsSchedule(Guid userGUID, int id);

        #endregion

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UnitNode> GetUnitNodeByCode(Guid userGuid, string code);

        /// <summary>
        /// Запросить первый родительский узел с заданным типом
        /// </summary>
        /// <param name="userGUID"></param>
        /// <param name="unitNodeID"></param>
        /// <param name="typeID"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UnitNode> GetParentUnitNode(Guid userGUID, int unitNodeID, int typeID);

        #region Работа с отчетами
        /// <summary>
        /// Получить доступные источники данных
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ReportSourceInfo[]> GetReportSources(Guid userGUID);

        /// <summary>
        /// Получить настройки по умолчанию для указанного источника данных
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="reportSourceID">ИД источника данных</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ReportSourceSettings> GetReportSourceSettings(Guid userGUID, Guid reportSourceID);

        /// <summary>
        /// Подготовить данные по указанным источникам данных
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="settings">Настройки запрашивыемых источников данных</param>
        /// <param name="reportParameters">Параметры отчёта</param>
        /// <returns>
        /// Идентификатор асинхронной операции.
        /// Данные приходят в виде словаря GUID -- DataSet
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<FastReportWrap> GenerateReportData(Guid userGUID, ReportSourceSettings[] settings, ReportParameter[] reportParameters);

        /// <summary>
        /// Подготовить пустые данные для дизайнера отчета
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="settings">Настройки запрашивыемых источников данных</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<FastReportWrap> GenerateEmptyReportData(Guid userGUID, ReportSourceSettings[] settings);

        /// <summary>
        /// Сформировать отчёт
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="reportID">ИД узла отчёта</param>
        /// <param name="saveInSystem">Флаг. сохранять ли сформированный отчёт в системе</param>
        /// <param name="reportParameters">Параметры отчёта</param>
        /// <returns>
        /// Идентификатор асинхронной операции.
        /// Тело отчёта приходит в виде byte[].
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<byte[]> GenerateReport(Guid userGUID, int reportID, bool saveInSystem, ReportParameter[] reportParameters);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<byte[]> GenerateExcelReport(Guid userGUID, int parameterId, DateTime dateFrom, DateTime dateTo, bool saveInSystem);

        /// <summary>
        /// Запросить список готов отчётов, сформированных за запрашиваемый интервал времени.
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="dateFrom">Начальное время запрашиваемого интервала</param>
        /// <param name="dateTo">Конечное время запрашиваемого интервала</param>
        /// <returns>
        /// Идентификатор асинхронной операции.
        /// PreferedReportInfo[].
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<PreferedReportInfo[]> GetPreferedReports(Guid userGUID, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// Запросить тело готового отчета
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="reportInfo">Информация о готовом отчёте</param>
        /// <returns>
        /// Идентификатор асинхронной операции.
        /// Тело отчёта в виде byte[].
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<byte[]> GetPreferedReportBody(Guid userGUID, PreferedReportInfo reportInfo);

        /// <summary>
        /// Удалить готовый отчёт из системы
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="reportInfo">Информация о готовом отчёте</param>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer DeletePreferedReport(Guid userGUID, PreferedReportInfo reportInfo);
        #endregion

        #region Слежение за состоянием асинхронных операций
        /// <summary>
        /// Методы управления операциями,
        /// пока еще сырые, в процессе написания 
        /// еще подправятся.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<OperationInfo> GetOperationState(Guid userGUID,
                                              ulong operation_id);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UAsyncResult> GetOperationResult(Guid userGUID,
                                        ulong operation_id);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<UAsyncResult> GetOperationMessages(Guid userGUID, ulong operationID);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer WaitAsyncOperation(Guid userGUID, ulong operationID);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        [FaultContract(typeof(LockExceptionFault))]
        ReturnContainer EndAsyncOperation(Guid userGUID, ulong operationID);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer AbortOperation(Guid userGUID,
                                  ulong operation_id);

        #endregion

        #region Отправка данных на блочные

        //[OperationContract]
        //ulong SendDataToAllBlockServers(Guid userGUID);

        [OperationContract]
        ReturnContainer<ulong> SendDataToBlockServer(Guid userGUID, int blockId);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer DeleteLoadValues(Guid userGUID, int unitNodeID, DateTime timeFrom);

        #endregion

        #region Значения параметров
        //[OperationContract]
        //[FaultContract(typeof(UserNotConnectedException))]
        //ReturnContainer<ulong> BeginGetArgumentedValues(Guid userGUID, int[] ids, DateTime beginTime, DateTime endTime);

        /// <summary>
        /// Запросить значения параметров
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="ids">Идентификаторы парамтров</param>
        /// <param name="beginTime">Начальное время запрашиваемого периода</param>
        /// <param name="endTime">Конечное время запрашиваемого периода</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ulong> BeginGetValues(Guid userGUID, int[] ids, DateTime beginTime, DateTime endTime, bool useBlockValues);

        /// <summary>
        /// Запросить значения параметра
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="parameterID">Идентификатор параметра</param>
        /// <param name="beginTime">Начальное время запрашиваемого периода</param>
        /// <param name="endTime">Конечное время запрашиваемого периода</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ulong> BeginGetValues2(Guid userGUID, int parameterID, DateTime beginTime, DateTime endTime, bool useBlockValues);

        /// <summary>
        /// Запросить значения параметра
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="parameterID">Идентификатор параметра</param>
        /// <param name="beginTime">Начальное время запрашиваемого периода</param>
        /// <param name="endTime">Конечное время запрашиваемого периода</param>
        /// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        /// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ulong> BeginGetValues3(Guid userGUID, int parameterID, DateTime beginTime, DateTime endTime,
           Interval interval, CalcAggregation aggregation, bool useBlockValues);

        /// <summary>
        /// Запросить значения параметра
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="parametersID">Идентификаторы параметров</param>
        /// <param name="beginTime">Начальное время запрашиваемого периода</param>
        /// <param name="endTime">Конечное время запрашиваемого периода</param>
        /// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        /// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        /// <param name="useBlockValues">Обращаться ли к серверу сбора данных для запроса значений или же использовать локальные значения</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ulong> BeginGetValues4(Guid userGUID, int[] parametersID, DateTime beginTime, DateTime endTime,
           Interval interval, CalcAggregation aggregation, bool useBlockValues);

        [OperationContract]
        ReturnContainer<ulong> BeginGetValues5(Guid userGUID,
                             int parameterID,
                             ArgumentsValues arguments,
                             DateTime beginTime,
                             DateTime endTime,
                             Interval interval,
                             CalcAggregation aggregation);

        /// <summary>
        /// Получить отсортированные аргументы условных параметров оптимизационного расчёта в порядки убывания оптимальности
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitNode">ИД узла оптимизации или узла вложенного в эту оптимизацию</param>
        /// <param name="time">Время оптимизационного расчета</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ulong> BeginGetSortedArgs(Guid userGUID, int unitNode, DateTime time);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ulong> BeginGetOptimizationArguments(Guid userGUID, int optimizationNodeID, DateTime time);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ulong> BeginGetOptimizationArgsForReport(Guid userGUID, int optimizationNodeID, DateTime time);

        /// <summary>
        /// Сохранить значения парамтров
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="packages">Сохраняемые параметры</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer SaveValues(Guid userGUID, Package[] packages);

        //ulong BeginChangeArguments(Guid userGUID, int optimizationNodeID, DateTime time, KeyValuePair<ArgumentsValues, ArgumentsValues>[] changedArgs);

        /// <summary>
        /// Удалить значений всех условных параметров внутри оптимизации для указанных значениях аргументов
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="optimizationNodeID">ИД узла оптимизации</param>
        /// <param name="valueArguments">Значения аргументов удаляемых значений</param>
        /// <param name="time">Времена удаляемых значений</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer DeleteValuesOptimization(Guid userGUID, int optimizationNodeID, ArgumentsValues[] valueArguments, DateTime time);

        /// <summary>
        /// Удалить значения параметров
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="parameterID">Идентификатор параметра</param>
        /// <param name="deletingDateTimes">Времена удаляемых значений</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer DeleteValues(Guid userGUID, int parameterID, DateTime[] deletingDateTimes);
        #endregion

        #region Расчет
        /// <summary>
        /// Запросить зависимости параметра
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="parameterID">Идентификатор параметра, чьи зависимочти требуется найти</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ParameterNodeDependence[]> GetDependence(Guid userGUID, RevisionInfo revision, int parameterID);

        /// <summary>
        /// Используется ли в формулах данный код
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="code">Код</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<bool> HasReference(Guid userGuid, String code);

        /// <summary>
        /// Запросить ссылки на узел
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="code">Код параметра</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ParameterNodeReference[]> GetReference(Guid userGUID, String code);

        /// <summary>
        /// Начать расчет параметров
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="parametersIDs">Идентификаторы расчитываемых параметров</param>
        /// <param name="beginTime">Начальное время</param>
        /// <param name="startTime">Конечное время</param>
        /// <param name="recalcAll">Пересичтать все</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ulong> BeginCalc(Guid userGUID, int[] parametersIDs, DateTime beginTime, DateTime startTime, bool recalcAll);

        /// <summary>
        /// Запросить список функций, используемых в расчете
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="revision">Ревизия для которой запрашивается список функций</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<FunctionInfo[]> GetCalcFunction(Guid userGUID, RevisionInfo revision);

        /// <summary>
        /// Проверить формулу на ошибки
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="formulaText">Текст формулы</param>
        /// <param name="arguments">Аргумены для уловных параметров и функций</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<Message[]> CheckFormula(Guid userGUID, RevisionInfo revision, string formulaText, KeyValuePair<int, CalcArgumentInfo[]>[] arguments);

        /// <summary>
        /// Запросить список констант
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ConstsInfo[]> GetConsts(Guid userGUID);

        /// <summary>
        /// Сохранить изменения констант
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="constsInfo">Список измененных констант</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer SaveConsts(Guid userGUID, IEnumerable<ConstsInfo> constsInfo);

        /// <summary>
        /// Удалить константы, используемые в расчете
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="constsInfo">Коллекция удаляемых констант</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer RemoveConsts(Guid userGUID, IEnumerable<ConstsInfo> constsInfo);

        /// <summary>
        /// Запросить список пользовательских функций
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<CustomFunctionInfo[]> GetCustomFunctions(Guid userGUID);

        /// <summary>
        /// Сохранить пользовательские функции
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="functionInfo">Сохраняемые функции</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer SaveCustomFunctions(Guid userGUID, IEnumerable<CustomFunctionInfo> functionInfo);

        /// <summary>
        /// Удалить пользовательские функции
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="functionInfo">Удаляемые функции</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer RemoveCustomFunctions(Guid userGUID, IEnumerable<CustomFunctionInfo> functionInfo);

        #region Циклический расчет
        /// <summary>
        /// Проверить запущен ли циклический расчет
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<bool> IsRoundRobinStarted(Guid userGUID);

        /// <summary>
        /// Запустить циклический расчет
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer StartRoundRobinCalc(Guid userGUID);

        /// <summary>
        /// Остановить циклический расчет
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer StopRoundRobinCalc(Guid userGUID);

        /// <summary>
        /// Получить количество сообщений последнего циклического расчета
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <returns>Количество сообщений</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<int> GetLastRoundRobinMessagesCount(Guid userGUID);

        /// <summary>
        /// Получить сообщения последнего циклического расчета
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="start">Начальное сообщение</param>
        /// <param name="count">Количество сообщений</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<Message[]> GetLastRoundRobinMessages(Guid userGUID, int start, int count);

        /// <summary>
        /// Получить настройки циклического расчета
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<bool> GetRoundRobinAutoStart(Guid userGUID);

        /// <summary>
        /// Установить настройку автозапуска
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="autoStart">true - автозапуск разрешен, false - запрещен</param>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer SetRoundRobinAutoStart(Guid userGUID, bool autoStart);

        /// <summary>
        /// Получить информацию о циклическом расчете
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<RoundRobinInfo> GetRoundRobinInfo(Guid userGUID);
        #endregion
        #endregion

        [OperationContract]
        ReturnContainer<ulong> BeginGetLogs(DateTime from, DateTime to);

        #region Работа с расширениями
        /// <summary>
        /// Поддерживает ли параметр ИД внешнего объекта
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitNodeID">ИД узла</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<bool> ExternalIDSupported(Guid userGUID, int unitNodeID);

        /// <summary>
        /// Поддерживает ли параметр код внешнего объекта
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitNodeID">ИД узла</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<bool> ExternalCodeSupported(Guid userGUID, int unitNodeID);

        /// <summary>
        /// Можно ли из системы добавить новый ИД внешнего объекта
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitNodeID">ИД узла</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<bool> ExternalIDCanAdd(Guid userGUID, int unitNodeID);

        /// <summary>
        /// Получить список внешних объектов
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitNodeID">ИД узла</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<EntityStruct[]> ExternalIDList(Guid userGUID, int unitNodeID);

        /// <summary>
        /// Получить свойства только для чтения из внешних объектов
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitNodeID">ИД узла</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ItemProperty[]> GetExternalProperties(Guid userGUID, int unitNodeID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userGUID"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ExtensionDataInfo[]> GetExtensionTableInfo(Guid userGUID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userGUID"></param>
        /// <param name="unitNodeID"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ExtensionDataInfo[]> GetExtensionTableInfoById(Guid userGUID, int unitNodeID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userGUID"></param>
        /// <param name="extensionCaption"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ExtensionDataInfo[]> GetExtensionTableInfoByCaption(Guid userGUID, String extensionCaption);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userGUID"></param>
        /// <param name="unitNodeID"></param>
        /// <param name="tabKeyword"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        //[OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ExtensionData> GetExtensionExtendedTable(Guid userGUID, int unitNodeID, String tabKeyword, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userGUID"></param>
        /// <param name="extensionCaption"></param>
        /// <param name="tabKeyword"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        //[OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<ExtensionData> GetExtensionExtendedTable2(Guid userGUID, String extensionCaption, String tabKeyword, DateTime dateFrom, DateTime dateTo);
        #endregion

        //DateTime GetParameterStartTime(Guid userGUID, int parameterID);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<Interval> GetParameterInterval(Guid userGUID, int parameterID);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<bool> IsExtensionType(Guid userGUID, int nodeType);

        /// <summary>
        /// Запросить статистику узла.
        /// Получает количество дочерних элементов сгруппированных по типам
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitNodeID">ИД верхнего узла. Сам узел в статистике не участвует</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<Dictionary<int, int>> GetStatistic(Guid userGUID, int unitNodeID);


        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<Audit.AuditEntry[]> GetAudit(Guid userGUID, Audit.AuditRequestContainer request);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer<IntervalDescription[]> GetStandardsIntervals(Guid userGUID);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer RemoveStandardIntervals(Guid userGUID, IntervalDescription[] intervalsToRemove);

        [OperationContract]
        [FaultContract(typeof(UserNotConnectedException))]
        ReturnContainer SaveStandardIntervals(Guid userGUID, IntervalDescription[] modifiedIntervals);
    }

    [Serializable]
    [DataContract]
    public class IntervalDescription
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public bool IsStandard { get; set; }

        [DataMember]
        public String Header { get; set; }

        [DataMember]
        public Interval interval { get; set; }
    }
}

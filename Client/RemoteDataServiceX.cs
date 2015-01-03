using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Security.Authentication;
using System.Text;
using COTES.ISTOK;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;
using COTES.ISTOK.Extension;

namespace COTES.ISTOK.Client
{
    //[System.Serializable]
    //class RemoteDataServiceX : IAsyncOperationManager
    //{
    //    Guid userGUID;
    //    IChannel tcpChannel = null;
    //    IGlobalQueryManager queryManager = null;
    //    internal AsyncOperation localAsyncOperation = new AsyncOperation();

    //    /// <summary>
    //    /// Кэширование юнитов.
    //    /// </summary>
    //    private CacheObjects<int, UnitNode> unitsCache =
    //                                new CacheObjects<int, UnitNode>();

    //    /// <summary>
    //    /// Кеш расписаний выгрузки параметров.
    //    /// </summary>
    //    private CacheObjects<int, Schedule> scheduleCache =
    //                                new CacheObjects<int, Schedule>();

    //    /// <summary>
    //    /// Реализация синглетона.
    //    /// </summary>
    //    private static RemoteDataService instance = new RemoteDataService();

    //    private RemoteDataServiceX()
    //    { }

    //    /// <summary>
    //    /// Сбрасывает кэшированные данные
    //    /// </summary>
    //    public void Reset()
    //    {
    //        unitsCache = new CacheObjects<int, UnitNode>();
    //        scheduleCache = new CacheObjects<int, Schedule>();
    //    }

    //    public static RemoteDataService Instance
    //    {
    //        get { return instance; }
    //    }

    //    /// <summary>
    //    /// Текущий пользователь, которым осуществленно подключение
    //    /// </summary>
    //    public UserNode User { get; protected set; }

    //    /// <summary>
    //    /// Событие возникает при потери связи с сервером
    //    /// </summary>
    //    public event EventHandler ServerNotAccessible;

    //    /// <summary>
    //    /// Событие возникает когда пользователь не подключен к системе, например,
    //    /// когда его отключили при редактировании
    //    /// </summary>
    //    public event EventHandler UserDisconnected;

    //    /// <summary>
    //    /// Вызвать событие ServerNotAccessible
    //    /// </summary>
    //    private void OnServerNotAccessible()
    //    {
    //        System.Threading.ThreadPool.QueueUserWorkItem((Object state) =>
    //        {
    //            if (ServerNotAccessible != null)
    //                ServerNotAccessible(this, EventArgs.Empty);
    //        });
    //    }

    //    /// <summary>
    //    /// Вызвать событие UserDisconnected
    //    /// </summary>
    //    private void OnUserDisconnected()
    //    {
    //        System.Threading.ThreadPool.QueueUserWorkItem((Object state) =>
    //        {
    //            if (UserDisconnected != null)
    //                UserDisconnected(this, EventArgs.Empty);
    //        });
    //    }

    //    System.Threading.Timer aliveTimer;

    //    private void AliveMethod(Object state)
    //    {
    //        try
    //        {
    //            AliveMessageContainer container;
    //            container = RemoteCall<AliveMessageContainer>(() => queryManager.Alive(userGUID));
    //            if (container != null)
    //            {
    //                RiseAliveEvents(container);
    //            }
    //        }
    //        catch
    //        {
    //            if (aliveTimer != null)
    //            {
    //                aliveTimer.Dispose();
    //                aliveTimer = null;
    //            }
    //        }
    //    }

    //    private void RiseAliveEvents(AliveMessageContainer container)
    //    {
    //        foreach (AliveMessage message in container.Messages)
    //        {
    //            if (message is AliveUnitNodeMessage)
    //            {
    //                AliveUnitNodeMessage unitNodeMessage = message as AliveUnitNodeMessage;
    //                foreach (int nodeID in unitNodeMessage.NodeIDs)
    //                {
    //                    OnUnitNodeChanged(nodeID);
    //                }
    //            }
    //            else if (message is AliveUnitTypeMessage)
    //            {
    //                AliveUnitTypeMessage unitTypeMessage = message as AliveUnitTypeMessage;
    //                foreach (var typeID in unitTypeMessage.TypeIDs)
    //                {
    //                    OnTypesChanged(typeID);
    //                }
    //            }
    //        }
    //    }

    //    public event EventHandler<UnitNodeEventArgs> UnitNodeChanged;

    //    private void OnUnitNodeChanged(int unitNodeID)
    //    {
    //        if (UnitNodeChanged != null)
    //            UnitNodeChanged(this, new UnitNodeEventArgs(unitNodeID));
    //    }

    //    /// <summary>
    //    /// Подключение к серверу приложения
    //    /// </summary>
    //    /// <param name="url">Сетевой адрес сервера</param>
    //    /// <param name="userName">Имя пользователя</param>
    //    /// <param name="password">Пароль пользователя</param>
    //    public void Connect(string url, string userName, string password)
    //    {
    //        typesArray = null;
    //        try
    //        {
    //            BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
    //            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
    //            serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
    //            queryManager = null;
    //            if (tcpChannel == null)
    //            {
    //                IDictionary ht = new Hashtable(StringComparer.OrdinalIgnoreCase);
    //                ht.Add("name", string.Empty);
    //                ht.Add("port", "0");
    //                ht.Add("typeFilterLevel", System.Runtime.Serialization.Formatters.TypeFilterLevel.Full);
    //                //ht["machineName"] = ClientSettings.Instance.ClientInterface;
    //                tcpChannel = new TcpChannel(ht, clientProvider, serverProvider);
    //                ChannelServices.RegisterChannel(tcpChannel, false);
    //            }

    //            // Регистрация типа клиента
    //            //bool res = false;
    //            //WellKnownClientTypeEntry[] types = RemotingConfiguration.GetRegisteredWellKnownClientTypes();
    //            //foreach (WellKnownClientTypeEntry typ in types)
    //            //{
    //            //    res = typ.ObjectType.Equals(typeof(IGlobalQueryManager));
    //            //    if (res) break;
    //            //}
    //            //if (!res)
    //            //{
    //            //    System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownClientType(typeof(IGlobalQueryManager), url);
    //            //}

    //            queryManager = (IGlobalQueryManager)Activator.GetObject(typeof(IGlobalQueryManager), url);
    //            if (queryManager == null) throw new Exception("Не удалось подключиться к серверу приложения.");
    //            try
    //            {
    //                Guid guid = queryManager.Connect(userName, password);

    //                if (guid.Equals(Guid.Empty)) throw new Exception("Введенные имя пользователя и пароль не верны");
    //                //qManager.AddClient(Program.Client);
    //                userGUID = guid;
    //            }
    //            catch (System.Runtime.Remoting.RemotingException ex)
    //            {
    //                Exception exc = new Exception("Не удалось подключиться к серверу приложения.", ex);
    //                exc.Data["StackTrace"] = exc.StackTrace;
    //                throw exc;
    //                //throw new Exception("Не удалось подключиться к серверу приложения.", ex);
    //            }
    //            //получение имя пользователя
    //            User = queryManager.GetUser(userGUID);
    //            lockDictionary.Clear();
    //            aliveTimer = new System.Threading.Timer(AliveMethod, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    //        }
    //        catch (NoOneUserException) { throw; }
    //        catch
    //        {
    //            queryManager = null;
    //            throw;
    //        }
    //    }

    //    /// <summary>
    //    /// Отключится от сервера приложения
    //    /// </summary>
    //    public void Disconnect()
    //    {
    //        try
    //        {
    //            if (aliveTimer != null)
    //            {
    //                aliveTimer.Dispose();
    //                aliveTimer = null;
    //            }
    //            if (queryManager != null)
    //                queryManager.Disconnect(userGUID);
    //        }
    //        catch (System.Net.Sockets.SocketException) { }
    //    }

    //    Dictionary<int, List<BaseAsyncWorkForm>> lockDictionary = new Dictionary<int, List<BaseAsyncWorkForm>>();

    //    #region Блокировки
    //    public void LockNode(BaseAsyncWorkForm uniForm, UnitNode node)
    //    {
    //        lock (lockDictionary)
    //        {
    //            List<BaseAsyncWorkForm> lockList;
    //            if (node.Idnum > 0)
    //            {
    //                if (!lockDictionary.TryGetValue(node.Idnum, out lockList))
    //                {
    //                    RemoteCall(() => queryManager.LockNode(userGUID, node.Idnum));
    //                    lockDictionary[node.Idnum] = lockList = new List<BaseAsyncWorkForm>();
    //                }
    //                if (!lockList.Contains(uniForm))
    //                    lockList.Add(uniForm);
    //            }
    //        }
    //    }

    //    public void ReleaseNode(BaseAsyncWorkForm uniForm, UnitNode node)
    //    {
    //        ReleaseNode(uniForm, node.Idnum);
    //    }

    //    private void ReleaseNode(BaseAsyncWorkForm uniForm, int nodeID)
    //    {
    //        lock (lockDictionary)
    //        {
    //            List<BaseAsyncWorkForm> lockList;
    //            if (lockDictionary.TryGetValue(nodeID, out lockList) && lockList.Contains(uniForm))
    //            {
    //                lockList.Remove(uniForm);
    //            }
    //            if (lockList != null && lockList.Count == 0)
    //            {
    //                RemoteCall(() => queryManager.ReleaseNode(userGUID, nodeID));
    //                lockDictionary.Remove(nodeID);
    //            }
    //        }
    //    }

    //    public void ReleaseNode(BaseAsyncWorkForm uniForm)
    //    {
    //        lock (lockDictionary)
    //        {
    //            int[] ids = new List<int>(lockDictionary.Keys).ToArray();
    //            foreach (int nodeID in ids)
    //                ReleaseNode(uniForm, nodeID);
    //        }
    //    }

    //    public void LockValues(UnitNode unitNode, DateTime startTime, DateTime endTime)
    //    {
    //        RemoteCall(() => queryManager.LockValues(userGUID, unitNode.Idnum, startTime, endTime));
    //    }

    //    public void ReleaseValues(UnitNode unitNode, DateTime startTime, DateTime endTime)
    //    {
    //        RemoteCall(() => queryManager.ReleaseValues(userGUID, unitNode.Idnum, startTime, endTime));
    //    }

    //    public void ReleaseAll(BaseAsyncWorkForm uniForm, UnitNode unitNode)
    //    {
    //        ReleaseNode(uniForm, unitNode.Idnum);
    //        RemoteCall(() => queryManager.ReleaseAll(userGUID, unitNode.Idnum));
    //    } 
    //    #endregion

    //    /// <summary>
    //    /// Получить текущую загрузку справочников
    //    /// </summary>
    //    /// <returns>значение в процентах, указывающие текущие состояние загрузки справочников</returns>
    //    public double GetLoadProgress()
    //    {
    //        return RemoteCall<double>(() => queryManager.GetLoadProgress(userGUID), true);
    //    }

    //    /// <summary>
    //    /// Получить текущие состояние загрузки справочников
    //    /// </summary>
    //    /// <returns>Строка состояния загрузки справочников</returns>
    //    public String GetLoadStatusString()
    //    {
    //        return RemoteCall<String>(() => queryManager.GetLoadStatusString(userGUID), true);
    //    }

    //    #region Обёртка для сетевых вызовов
    //    /// <summary>
    //    /// Метод стандартного поведения при обращении к удаленной службе
    //    /// </summary>
    //    /// <param name="call"></param>
    //    private void RemoteCall(Action call)
    //    {
    //        try
    //        {
    //            call();
    //        }
    //        catch (System.Net.Sockets.SocketException) { OnServerNotAccessible(); }
    //        catch (RemotingException) { OnServerNotAccessible(); }
    //        catch (UserNotConnectedException) { OnUserDisconnected(); }
    //    }

    //    /// <summary>
    //    /// Метод стандартного поведения при обращении к удаленной службе с возвращаемым результатом
    //    /// </summary>
    //    /// <typeparam name="T">Результирующий тип удаленного вызова</typeparam>
    //    /// <param name="call">Удаленный вызов</param>
    //    /// <returns>
    //    /// Результат вызова или значение по умолчанию для указанного типа T, 
    //    /// если служба не доступна (UserNotConnectedException, SocketException или RemotingException).
    //    /// </returns>
    //    private T RemoteCall<T>(Func<T> call)
    //    {
    //        return RemoteCall<T>(call, false);
    //    }

    //    /// <summary>
    //    /// Метод стандартного поведения при обращении к удаленной службе
    //    /// </summary>
    //    /// <typeparam name="T">Результирующий тип удаленного вызова</typeparam>
    //    /// <param name="call">Удаленный вызов</param>
    //    /// <param name="throwException">Указывает, следует ли посылать дальше исключение, если служба не доступна</param>
    //    /// <returns>
    //    /// Результат вызова или значение по умолчанию для указанного типа T, 
    //    /// если служба не доступна (UserNotConnectedException, SocketException или RemotingException).
    //    /// </returns>
    //    private T RemoteCall<T>(Func<T> call, bool throwException)
    //    {
    //        try
    //        {
    //            return call();
    //        }
    //        catch (System.Net.Sockets.SocketException)
    //        {
    //            OnServerNotAccessible();
    //            if (throwException)
    //                throw;
    //        }
    //        catch (RemotingException)
    //        {
    //            OnServerNotAccessible();
    //            if (throwException)
    //                throw;
    //        }
    //        catch (UserNotConnectedException)
    //        {
    //            OnUserDisconnected();
    //            if (throwException)
    //                throw;
    //        }
    //        return default(T);
    //    }

    //    /// <summary>
    //    /// Метод стандартного поведения при запуске асинхронной операции 
    //    /// на удаленной службы.
    //    /// </summary>
    //    /// <param name="call">Вызов, запускающий асинхронную операцию на удаленной службе.</param>
    //    /// <returns>
    //    /// Вотчер, для отслеживания состояния асинхронной операции.
    //    /// Пустой вотчер, если удаленная служба не доступна (UserNotConnectedException, SocketException или RemotingException).
    //    /// </returns>
    //    private AsyncOperationWatcher RemoteCallAsync(Func<ulong> call)
    //    {
    //        try
    //        {
    //            return new AsyncOperationWatcher(call(), this);
    //        }
    //        catch (System.Net.Sockets.SocketException) { OnServerNotAccessible(); }
    //        catch (RemotingException) { OnServerNotAccessible(); }
    //        catch (UserNotConnectedException) { OnUserDisconnected(); }
    //        return new AsyncOperationWatcher(0, this);
    //    }

    //    /// <summary>
    //    /// Метод стандартного поведения при запуске асинхронной операции 
    //    /// на удаленной службы.
    //    /// </summary>
    //    /// <typeparam name="T">Результирующий тип асинхронной операции.</typeparam>
    //    /// <param name="call">Вызов, запускающий асинхронную операцию на удаленной службе.</param>
    //    /// <returns>
    //    /// Вотчер, для отслеживания состояния асинхронной операции.
    //    /// Пустой вотчер, если удаленная служба не доступна (UserNotConnectedException, SocketException или RemotingException).
    //    /// </returns>
    //    private AsyncOperationWatcher<T> RemoteCallAsync<T>(Func<ulong> call)
    //    {
    //        try
    //        {
    //            return new AsyncOperationWatcher<T>(call(), this);
    //        }
    //        catch (System.Net.Sockets.SocketException) { OnServerNotAccessible(); }
    //        catch (UserNotConnectedException) { OnUserDisconnected(); }
    //        return new AsyncOperationWatcher<T>(0, this);
    //    }

    //    /// <summary>
    //    /// Метод стандартного поведения при запуске асинхронной операции 
    //    /// на удаленной службы, и синхронизации вызывающего потока и операцией .
    //    /// </summary>
    //    /// <typeparam name="T">Результирующий тип асинхронной операции.</typeparam>
    //    /// <param name="call">Вызов, запускающий асинхронную операцию на удаленной службе.</param>
    //    /// <returns>
    //    /// Результат вызова или значение по умолчанию для указанного типа IEnumerable&lt;T&gt;, 
    //    /// если служба не доступна (UserNotConnectedException, SocketException или RemotingException).
    //    /// </returns>
    //    private IEnumerable<T> RemoteCallSync<T>(Func<ulong> call)
    //    {
    //        try
    //        {
    //            ulong operationID = call();
    //            Object obj;
    //            List<T> valList = new List<T>();

    //            queryManager.WaitAsyncOperation(userGUID, operationID);

    //            while ((obj = queryManager.GetOperationResult(userGUID, operationID)) != null)
    //            {
    //                if (obj is T)
    //                    valList.Add((T)obj);
    //                else if (obj is IEnumerable<T>)
    //                    valList.AddRange(obj as IEnumerable<T>);
    //                //else break;
    //            }
    //            queryManager.EndAsyncOperation(userGUID, operationID);

    //            return valList;
    //        }
    //        catch (System.Net.Sockets.SocketException) { OnServerNotAccessible(); }
    //        catch (RemotingException) { OnServerNotAccessible(); }
    //        catch (UserNotConnectedException) { OnUserDisconnected(); }
    //        return default(IEnumerable<T>);
    //    } 
    //    #endregion

    //    #region UnitType
    //    private UTypeNode[] typesArray;

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public UTypeNode[] Types
    //    {
    //        get
    //        {
    //            if (typesArray == null)
    //                typesArray = GetUnitTypes();
    //            return typesArray;
    //        }
    //    }

    //    private void OnTypesChanged(UnitTypeId typeID)
    //    {
    //        typesArray = null;

    //        if (TypesChanged!=null)
    //            TypesChanged(this, new UnitTypeEventArgs(typeID));
    //    }

    //    private void OnTypesChanged()
    //    {
    //        OnTypesChanged(UnitTypeId.Unknown);
    //    }

    //    public event EventHandler<UnitTypeEventArgs> TypesChanged;

    //    /// <summary>
    //    /// Запросить типы оборудования
    //    /// </summary>
    //    /// <returns>Вотчер</returns>
    //    public AsyncOperationWatcher<UTypeNode[]> BeginGetUnitTypes()
    //    {
    //        return RemoteCallAsync<UTypeNode[]>(() => queryManager.BeginGetUnitTypes(userGUID));
    //    }

    //    private UTypeNode UnknownType
    //    {
    //        get
    //        {
    //            UTypeNode ret = new UTypeNode();
    //            ret.Idnum = (int)UnitTypeId.Unknown;
    //            ret.Text = "Неизветсно";
    //            ret.ChildFilterAll = true;
    //            //ret.Icon=Properties.Resources.unittype_default.
    //            using (MemoryStream ms = new MemoryStream())
    //            {
    //                Properties.Resources.unittype_default.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
    //                ret.Icon = ms.ToArray();
    //            }
    //            return ret;
    //        }
    //    }

    //    /// <summary>
    //    /// Запростит типы оборудования синхронно
    //    /// </summary>
    //    /// <returns>Загруженные типы оборудования</returns>
    //    public UTypeNode[] GetUnitTypes()
    //    {
    //        IEnumerable<UTypeNode> ret = RemoteCallSync<UTypeNode>(() => queryManager.BeginGetUnitTypes(userGUID));

    //        IEnumerable<UTypeNode> retEn = new UTypeNode[] { UnknownType };

    //        if (ret != null)
    //            retEn = retEn.Concat(ret);

    //        return retEn.ToArray();
    //    }

    //    /// <summary>
    //    /// Изменить тип оборудования
    //    /// </summary>
    //    /// <param name="unitTypeNode">Тип оборудования для сохранения</param>
    //    public void UpdateUnitType(UTypeNode unitTypeNode)
    //    {
    //        RemoteCall(() => queryManager.UpdateUnitType(userGUID, unitTypeNode));

    //        OnTypesChanged();
    //    }

    //    /// <summary>
    //    /// Добавить новый тип оборудования
    //    /// </summary>
    //    /// <param name="unitTypeNode">Тип оборудования для добавления</param>
    //    public void AddUnitType(UTypeNode unitTypeNode)
    //    {
    //        RemoteCall(() => queryManager.AddUnitType(userGUID, unitTypeNode));

    //        OnTypesChanged();
    //    }

    //    /// <summary>
    //    /// Удалить тип оборудования
    //    /// </summary>
    //    /// <param name="unitTypeNodes">Удаляемые тип оборудования</param>
    //    public void RemoveUnitType(UTypeNode[] unitTypeNodes)
    //    {
    //        List<int> removingIDs = new List<int>();
    //        foreach (UTypeNode typeNode in unitTypeNodes)
    //            removingIDs.Add(typeNode.Idnum);

    //        RemoteCall(() => queryManager.RemoveUnitType(userGUID, removingIDs.ToArray()));

    //        OnTypesChanged();
    //    }
    //    #endregion

    //    #region Диагностика
    //    public COTES.ISTOK.DiagnosticsInfo.Diagnostics GetDiagnosticsObject()
    //    {
    //        return RemoteCall<COTES.ISTOK.DiagnosticsInfo.Diagnostics>(() => queryManager.GetDiagnosticsObject(userGUID));
    //    }
    //    #endregion

    //    #region Работа с пользователями
    //    /// <summary>
    //    /// Запросить пользователей системы
    //    /// </summary>
    //    /// <returns>Вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginGetUserNodes()
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetUserNodes(userGUID));
    //    }

    //    /// <summary>
    //    /// Запросить пользователей системы
    //    /// </summary>
    //    /// <returns>Вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginGetUserNodes(int[] ids)
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetUserNodes(userGUID, ids));
    //    }

    //    /// <summary>
    //    /// Изменить пользователя системы
    //    /// </summary>
    //    /// <param name="userNode">Пользователь для правки</param>
    //    /// <returns>Вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginUpdateUserNode(UserNode userNode)
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginUpdateUserNode(userGUID, userNode));
    //    }

    //    /// <summary>
    //    /// Добавить нового пользователя
    //    /// </summary>
    //    /// <param name="userNode">Добавляемый пользователь</param>
    //    /// <returns>Вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginAddUserNode(UserNode userNode)
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginAddUserNode(userGUID, userNode));
    //    }

    //    /// <summary>
    //    /// Удалить пользователей из системы
    //    /// </summary>
    //    /// <param name="userNodes">Идентификаторы удаляемых пользователей</param>
    //    /// <returns>Вотчер</returns>
    //    public AsyncOperationWatcher BeginRemoveUserNode(UserNode[] userNodes)
    //    {
    //        return RemoteCallAsync<Object>(() =>
    //        {
    //            List<int> userNodeIDList = new List<int>();

    //            foreach (UserNode userNode in userNodes)
    //                userNodeIDList.Add(userNode.Idnum);
    //            return queryManager.BeginRemoveUserNode(userGUID, userNodeIDList.ToArray());
    //        });
    //    }

    //    /// <summary>
    //    /// Создать новго администратора системы, 
    //    /// когда в системе не присутсвует ни одного другого пользователя
    //    /// </summary>
    //    /// <param name="userNode">Пользователь создаваемого администратора</param>
    //    public void NewAdmin(UserNode userNode)
    //    {
    //        RemoteCall(() => queryManager.NewAdmin(userNode));
    //    }

    //    /// <summary>
    //    /// Проверить права текущего подключенного пользователя
    //    /// </summary>
    //    /// <param name="node">Узел</param>
    //    /// <param name="privileges">Права</param>
    //    /// <returns>true, если права пользователя удовлетворяют запрошенным</returns>
    //    public bool CheckAccess(UnitNode node, Privileges privileges)
    //    {
    //        UnitTypeId type = node.Typ;
    //        int owner = node.Owner;

    //        return User.IsAdmin || (User.CheckPrivileges(type, privileges) && User.CheckGroupPrivilegies(owner, privileges));
    //    }

    //    /// <summary>
    //    /// Получить имя пользователя по его UID
    //    /// </summary>
    //    /// <param name="ownerID">Идентификатор пользователя</param>
    //    /// <returns>Имя пользователя</returns>
    //    public String GetUserLogin(int ownerID)
    //    {
    //        return RemoteCall<String>(() => queryManager.GetUserLogin(userGUID, ownerID));
    //    }
    //    #endregion

    //    #region Работа с группами пользователей
    //    /// <summary>
    //    /// Запросить список групп системы синхронно
    //    /// </summary>
    //    /// <returns>Массив всех групп пользователей системы</returns>
    //    public GroupNode[] GetGroupNodes()
    //    {
    //        IEnumerable<GroupNode> groups = RemoteCallSync<GroupNode>(() => queryManager.BeginGetGroupNodes(userGUID));

    //        if (groups != null)
    //            return groups.ToArray();

    //        return null;
    //    }

    //    /// <summary>
    //    /// Запросить список пользователей системы
    //    /// </summary>
    //    /// <returns>Вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginGetGroupNodes()
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetGroupNodes(userGUID));
    //    }

    //    /// <summary>
    //    /// Добавить новую группу пользователей
    //    /// </summary>
    //    /// <param name="groupNode">Добавляемая группа</param>
    //    /// <returns>Вотчер</returns>
    //    public AsyncOperationWatcher BeginAddGroupNode(GroupNode groupNode)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginAddGroupNode(userGUID, groupNode));
    //    }

    //    /// <summary>
    //    /// Изменить группу пользователей
    //    /// </summary>
    //    /// <param name="groupNode">Редактируемая группа</param>
    //    /// <returns>Вотчер</returns>
    //    public AsyncOperationWatcher BeginUpdateGroupNode(GroupNode groupNode)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginUpdateGroupNode(userGUID, groupNode));
    //    }

    //    /// <summary>
    //    /// Удалить группы пользователя
    //    /// </summary>
    //    /// <param name="groupNodes">Массив идентификаторов удаляемых групп</param>
    //    /// <returns>Вотчер</returns>
    //    public AsyncOperationWatcher BeginRemoveGroupNode(GroupNode[] groupNodes)
    //    {
    //        List<int> groupNodeIDList = new List<int>();

    //        foreach (GroupNode userNode in groupNodes)
    //            groupNodeIDList.Add(userNode.Idnum);

    //        return RemoteCallAsync(() => queryManager.BeginRemoveGroupNode(userGUID, groupNodeIDList.ToArray()));
    //    }
    //    #endregion

    //    #region Revisions

    //    private RevisionInfo currentRevision;

    //    /// <summary>
    //    /// Текущая рабочая ревизия клиента
    //    /// </summary>
    //    public RevisionInfo CurrentRevision
    //    {
    //        get { return currentRevision; }
    //        set
    //        {
    //            if (currentRevision != value)
    //            {
    //                currentRevision = value;
    //                OnCurrentRevisionChanged();
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Событие возникающие при смене текущей рабочей ревизии CurrentRevision
    //    /// </summary>
    //    public event EventHandler CurrentRevisionChanged;

    //    private void OnCurrentRevisionChanged()
    //    {
    //        if (CurrentRevisionChanged != null)
    //            CurrentRevisionChanged(this, EventArgs.Empty);
    //    }

    //    /// <summary>
    //    /// Получить старшую ревизии в системе.
    //    /// Выбирается действующая ревизия время которой ближе всего к ностаящему моменту
    //    /// </summary>
    //    /// <returns></returns>
    //    public RevisionInfo GetHeadRevision()
    //    {
    //        // выбрать ближайшую по времени ревизию
    //        return (from r in GetRevisions() where r.Time <= DateTime.Now orderby r.Time descending select r).First();
    //        // выбрать ревизию с максимальным временем
    //        //return (from r in GetRevisions() orderby r.Time descending select r).First();
    //    }

    //    public AsyncOperationWatcher<IEnumerable<RevisionInfo>> BeginGetRevisions()
    //    {
    //        return RemoteCallAsync<IEnumerable<RevisionInfo>>(() => queryManager.BeginGetRevisions(userGUID));
    //    }

    //    public IEnumerable<RevisionInfo> GetRevisions()
    //    {
    //        List<RevisionInfo> list = new List<RevisionInfo>();

    //        list.AddRange(RemoteCallSync<RevisionInfo>(() => queryManager.BeginGetRevisions(userGUID)));

    //        return list;
    //    }

    //    public void RemoveRevisions(List<RevisionInfo> removeList)
    //    {
    //        RemoteCall(() => queryManager.RemoveRevisions(userGUID, removeList.ConvertAll(r => r.ID).ToArray()));
    //    }

    //    public void UpdateRevisions(List<RevisionInfo> updateList)
    //    {
    //        RemoteCall(() => queryManager.UpdateRevisions(userGUID, updateList.ToArray()));
    //    }
    //    #endregion

    //    #region Работа с юнит нодами

    //    //public AsyncOperationWatcher<Object> QueryAllUnitNodes(int id_parent, UnitTypeId[] filterTypes)
    //    //{
    //    //    var watcher = RemoteCallAsync<Object>(() => queryManager.BeginGetAllUnitNodes(userGUID,
    //    //                                                                     id_parent,
    //    //                                                                     filterTypes));

    //    //    watcher.AddValueRecivedHandler((object x) =>
    //    //    {
    //    //        UnitNode[] nodes = x as UnitNode[];

    //    //        foreach (var it in nodes)
    //    //        {
    //    //            AddUnitToCache(it);
    //    //        }
    //    //    });

    //    //    return watcher;
    //    //}

    //    public UnitNode[] GetAllUnitNodes(UnitNode unitNode, UnitTypeId[] filterTypes)
        //    {
    //        IEnumerable<UnitNode> nodes = RemoteCallSync<UnitNode>(() => queryManager.BeginGetAllUnitNodes(userGUID,
    //                                                     unitNode == null ? 0 : unitNode.Idnum,
    //                                                     filterTypes));
    //        if (nodes != null)
    //        {
    //            foreach (var it in nodes)
    //                AddUnitToCache(it);

    //            return nodes.ToArray();
        //        }
    //        return null;
    //    }

    //    public UnitNode[] GetAllUnitNodes(UnitNode unitNode, UnitTypeId[] filterTypes, int minLevel, int maxLevel)
    //    {
    //        IEnumerable<UnitNode> nodes = RemoteCallSync<UnitNode>(() => queryManager.BeginGetAllUnitNodes(userGUID,
    //                                                     unitNode == null ? 0 : unitNode.Idnum,
    //                                                     filterTypes,
    //                                                     minLevel,
    //                                                     maxLevel));
    //        if (nodes != null)
    //        {
    //            foreach (var it in nodes)
    //                AddUnitToCache(it);

    //            return nodes.ToArray();
    //        }
    //        return null;
    //    }

    //    /// <summary>
    //    /// Запрос на получение списка идов
    //    /// дочерних узлов нода.
    //    /// </summary>
    //    /// <param name="id_parent">
    //    ///     Ид родительского нода.
    //    /// </param>
    //    /// <param name="filterTypes">
    //    ///     Фильтр типов, представленный в виде массива. Выдаваться будут
    //    ///     только элементы с типом из массива.
    //    /// </param> 
    //    /// <returns>
    //    ///     Смотритель за асинхронной операцией,
    //    ///     запросом.
    //    /// </returns>
    //    public AsyncOperationWatcher<Object> QueryUnitNodes(int id_parent, UnitTypeId[] filterTypes)
    //    {
    //        var watcher =
    //            RemoteCallAsync<Object>(() => queryManager.BeginGetUnitNodes(userGUID,
    //                                                                         id_parent,
    //                                                                         filterTypes));

    //        watcher.AddValueRecivedHandler((object x) =>
    //            {
    //                UnitNode[] nodes = x as UnitNode[];

    //                foreach (var it in nodes)
    //                {
    //                    AddUnitToCache(it);
    //                }
    //            });

    //        return watcher;
    //    }
    //    /// <summary>
    //    /// Запрос на получение списка идов
    //    /// дочерних узлов нода.
    //    /// </summary>
    //    /// <param name="id_parent">
    //    ///     Ид родительского нода.
    //    /// </param>
    //    /// <param name="filterTypes">
    //    ///     Фильтр типов, представленный в виде массива. Выдаваться будут
    //    ///     только элементы с типом из массива.
    //    /// </param> 
    //    /// <returns>
    //    ///     Смотритель за асинхронной операцией,
    //    ///     запросом.
    //    /// </returns>
    //    public AsyncOperationWatcher<Object> QueryUnitNodes(int id_parent, ParameterFilter filter)
    //    {
    //        var watcher =
    //             RemoteCallAsync<Object>(() => queryManager.BeginGetUnitNodes(userGUID,
    //                                                                         id_parent,
    //                                                                         filter));

    //        watcher.AddValueRecivedHandler((object x) =>
    //        {
    //            UnitNode[] nodes = x as UnitNode[];

    //            foreach (var it in nodes)
    //            {
    //                AddUnitToCache(it);
    //            }
    //        });

    //        return watcher;
    //    }
    //    /// <summary>
    //    /// Запрос на получение списка идов
    //    /// дочерних узлов нода.
    //    /// </summary>
    //    /// <param name="id_parent">
    //    ///     Ид родительского нода.
    //    /// </param>
    //    /// <returns>
    //    ///     Смотритель за асинхронной операцией,
    //    ///     запросом.
    //    /// </returns>
    //    public AsyncOperationWatcher<Object> QueryUnitNodes(int id_parent)
    //    {
    //        return QueryUnitNodes(id_parent, new UnitTypeId[] { });
    //    }

    //    /// <summary>
    //    /// Запрос на получение узлов
    //    /// </summary>
    //    /// <param name="ids">
    //    ///     Массив ИДов.
    //    /// </param>
    //    /// <returns>
    //    ///     Смотритель за асинхронной операцией,
    //    ///     запросом.
    //    /// </returns>
    //    public AsyncOperationWatcher<Object> QueryUnitNodes(int[] ids)
    //    {
    //        AsyncOperationWatcher<Object> watcher =
    //            RemoteCallAsync<Object>(() => queryManager.BeginGetUnitNodes(userGUID,
    //                                                                         ids));

    //        watcher.AddValueRecivedHandler((object x) =>
    //        {
    //            UnitNode[] nodes = x as UnitNode[];

    //            foreach (var it in nodes) AddUnitToCache(it);
    //        });

    //        return watcher;
    //    }

    //    /// <summary>
    //    /// Функция получения нода по иду.
    //    /// Поиск нода осуществляется в кеше,
    //    /// если нод не найден, то возвращается нул.
    //    /// </summary>
    //    /// <param name="id">
    //    ///     Ид узла который ищем.
    //    /// </param>
    //    /// <returns>
    //    ///     Унит нод если узел в кеше или нул, если чего нет.
    //    /// </returns>
    //    public UnitNode GetUnitNode(int id)
    //    {
    //        return RemoteCall<UnitNode>(() => queryManager.GetUnitNode(userGUID, id));
    //    }

    //    /// <summary>
    //    /// Функция получения нода по коду.
    //    /// Поиск нода осуществляется в кеше,
    //    /// если нод не найден, то возвращается нул.
    //    /// </summary>
    //    /// <param name="id">
    //    ///     Код узла, который ищем.
    //    /// </param>
    //    /// <returns>
    //    ///     Унит нод, если узел в кеше или нул, если чего нет.
    //    /// </returns>
    //    public UnitNode GetUnitNode(string code)
    //    {
    //        return RemoteCall<UnitNode>(() => queryManager.GetUnitNode(userGUID, code));
    //    }

    //    ///// <summary>
    //    ///// Получить интервал параметра
    //    ///// </summary>
    //    ///// <remarks>
    //    ///// Интервал параметра берется и первого родительского элемента в структуре, обладающего интервалом.
    //    ///// Это Ручной ввод, Расчет и оптимизация
    //    ///// </remarks>
    //    ///// <param name="Parameter">Параметр</param>
    //    ///// <returns>Полученный интервал</returns>
    //    //public DateTime GetParameterStartTime(ParameterNode Parameter)
    //    //{
    //    //    return RemoteCall<DateTime>(() => queryManager.GetParameterStartTime(userGUID, Parameter.Idnum));
    //    //    //UnitNode unitNode = Parameter;
    //    //    //ParameterGateNode gateNode;

    //    //    //while (unitNode != null)
    //    //    //{
    //    //    //    if ((gateNode = unitNode as ParameterGateNode) != null)
    //    //    //        return gateNode.StartTime;

    //    //    //    unitNode = GetUnitNode(unitNode.ParentId);
    //    //    //}
    //    //    //return DateTime.MinValue;
    //    //}

    //    /// <summary>
    //    /// Получить интервал параметра
    //    /// </summary>
    //    /// <remarks>
    //    /// Интервал параметра берется и первого родительского элемента в структуре, обладающего интервалом.
    //    /// Это Ручной ввод, Расчет и оптимизация
    //    /// </remarks>
    //    /// <param name="Parameter">Параметр</param>
    //    /// <returns>Полученный интервал</returns>
    //    public Interval GetParameterInterval(ParameterNode Parameter)
    //    {
    //        return RemoteCall<Interval>(() => queryManager.GetParameterInterval(userGUID, Parameter.Idnum));
    //        //UnitNode unitNode = Parameter;
    //        //ParameterGateNode gateNode;

    //        //while (unitNode != null)
    //        //{
    //        //    if ((gateNode = unitNode as ParameterGateNode) != null)
    //        //        return gateNode.Interval;

    //        //    unitNode = GetUnitNode(unitNode.ParentId);
    //        //}
    //        //return Interval.Zero;
    //    }

    //    #region Значения параметров
    //    public AsyncOperationWatcher<Object> BeginGetArgumentedValues(List<ParameterNode> nodes, DateTime beginTime, DateTime endTime)
    //    {
    //        int[] ids = nodes.ConvertAll(n => n.Idnum).ToArray();
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetArgumentedValues(userGUID, ids, beginTime, endTime));
    //    }
    //    /// <summary>
    //    /// Запрос на получение значений параметров.
    //    /// </summary>
    //    /// <param name="ids">Массив идентификаторов параметров</param>
    //    /// <param name="beginTime">Начальное время интервала</param>
    //    /// <param name="endTime">Конечное время интервала</param>
    //    /// <returns>
    //    /// Смотритель за асинхронной операцией.
    //    /// </returns>
    //    public AsyncOperationWatcher<Package> BeginGetValues(int[] ids, DateTime beginTime, DateTime endTime, bool useBlockValues)
    //    {
    //        return RemoteCallAsync<Package>(() => queryManager.BeginGetValues(userGUID, ids, beginTime, endTime, useBlockValues));
    //    }
    //    /// <summary>
    //    /// Запрос на получение значений параметров.
    //    /// </summary>
    //    /// <param name="id"></param>
    //    /// <param name="beginTime">Начальное время интервала</param>
    //    /// <param name="endTime">Конечное время интервала</param>
    //    /// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
    //    /// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
    //    /// <returns>Смотритель за асинхронной операцией.</returns>
    //    public AsyncOperationWatcher<Package> BeginGetValues(int id, DateTime beginTime, DateTime endTime,
    //        Interval interval, CalcAggregation aggregation, bool useBlockValues)
    //    {
    //        return RemoteCallAsync<Package>(() => queryManager.BeginGetValues(userGUID, id, beginTime, endTime, interval, aggregation, useBlockValues));
    //    }
    //    /// <summary>
    //    /// Запрос на получение значений параметров.
    //    /// </summary>
    //    /// <param name="ids">Массив идентификаторов параметров</param>
    //    /// <param name="beginTime">Начальное время интервала</param>
    //    /// <param name="endTime">Конечное время интервала</param>
    //    /// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
    //    /// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
    //    /// <returns>Смотритель за асинхронной операцией.</returns>
    //    public AsyncOperationWatcher<Package> BeginGetValues(int[] ids, DateTime beginTime, DateTime endTime,
    //        Interval interval, CalcAggregation aggregation, bool useBlockValues)
    //    {
    //        return RemoteCallAsync<Package>(() => queryManager.BeginGetValues(userGUID, ids, beginTime, endTime, interval, aggregation, useBlockValues));
    //    }

    //    /// <summary>
    //    /// Запрос на получение значений параметра.
    //    /// </summary>
    //    /// <param name="Parameter">Запрашиваемый параметр</param>
    //    /// <param name="beginTime">Начальное время интервала</param>
    //    /// <param name="endTime">Конечное время интервала</param>
    //    /// <returns>Вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginGetValues(ParameterNode Parameter, DateTime beginTime, DateTime endTime, bool useBlockValues)
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetValues(userGUID, Parameter.Idnum, beginTime, endTime, useBlockValues));
    //    }

    //    public AsyncOperationWatcher<Package> BeginGetValues(int parameterID,
    //                                                ArgumentsValues arguments,
    //                                                DateTime beginTime,
    //                                                DateTime endTime,
    //                                                Interval interval,
    //                                                CalcAggregation aggregation)
    //    {
    //        return RemoteCallAsync<Package>(() => queryManager.BeginGetValues(userGUID, parameterID, arguments, beginTime, endTime, interval, aggregation));
    //    }

    //    public AsyncOperationWatcher<Package> BeginGetValues(int parameterID, ArgumentsValues arguments, DateTime beginTime, DateTime endTime)
    //    {
    //        return BeginGetValues(parameterID, arguments, beginTime, endTime, Interval.Zero, CalcAggregation.Nothing);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="optimizationNode"></param>
    //    /// <param name="time"></param>
    //    /// <returns>List&lt;OAK&gr;</returns>
    //    public AsyncOperationWatcher<Object> BeginGetSortedArgs(UnitNode unitNode, DateTime time)
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetSortedArgs(userGUID, unitNode.Idnum, time));
    //    }

    //    public AsyncOperationWatcher<Object> BeginGetOptimizationArguments(OptimizationGateNode optimizationNode, DateTime time)
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetOptimizationArguments(userGUID, optimizationNode.Idnum, time));
    //    }

    //    public AsyncOperationWatcher<Object> BeginGetOptimizationArgsForReport(OptimizationGateNode optimizationNode, DateTime time)
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetOptimizationArgsForReport(userGUID, optimizationNode.Idnum, time));
    //    }

    //    /// <summary>
    //    /// Сохранить значения
    //    /// </summary>
    //    /// <param name="packages">Пакеты со значениями</param>
    //    /// <returns>Вотчер</returns>
    //    public AsyncOperationWatcher BeginSaveValues(Package[] packages)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginSaveValues(userGUID, packages));
    //    }

    //    //public AsyncOperationWatcher BeginChangeArguments(OptimizationGateNode optimizationNode, DateTime time, KeyValuePair<ArgumentsValues, ArgumentsValues>[] changedArgs)
    //    //{
    //    //    return RemoteCallAsync(() => queryManager.BeginChangeArguments(userGUID, optimizationNode.Idnum, time, changedArgs));
    //    //}

    //    public AsyncOperationWatcher BeginDeleteValues(OptimizationGateNode optimizationNode, ArgumentsValues[] valueArguments, DateTime time)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginDeleteValues(userGUID, optimizationNode.Idnum, valueArguments, time));
    //    }

    //    /// <summary>
    //    /// Удалить значения
    //    /// </summary>
    //    /// <param name="Parameter">Параметр</param>
    //    /// <param name="deletingDateTimes">Времена удаляемых значений</param>
    //    /// <returns>Вотчер</returns>
    //    public AsyncOperationWatcher BeginDeleteValues(ParameterNode Parameter, DateTime[] deletingDateTimes)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginDeleteValues(userGUID, Parameter.Idnum, deletingDateTimes));
    //    }

    //    #endregion
    //    // Реализация интерфейся менеджера асинхронных операций.

    //    /// <summary>
    //    /// Запрос на получение унит нода от глобала.
    //    /// По получению унит нода, он автоматически 
    //    /// будет добавлен в кеш.
    //    /// </summary>
    //    /// <param name="node_id">
    //    ///     Ид запрашиваемого узла.
    //    /// </param>
    //    /// <returns>
    //    ///     Смотритель за асинхронной операцией,
    //    ///     запросом.
    //    /// </returns>
    //    public AsyncOperationWatcher<Object> QueryUnitNode(int node_id)
    //    {
    //        AsyncOperationWatcher<Object> watcher =
    //             RemoteCallAsync<Object>(() => queryManager.BeginGetUnitNode(userGUID,
    //                                                                         node_id));

    //        watcher.AddValueRecivedHandler((object x) =>
    //            {
    //                UnitNode node = x as UnitNode;
    //                AddUnitToCache(node);
    //            });

    //        return watcher;
    //    }

    //    /// <summary>
    //    /// Запрос на удаление унит нода.
    //    /// Унит нод при этом удаляется из кеша тоже.
    //    /// </summary>
    //    /// <param name="id_obj">
    //    ///     Ид удаляемого объекта.
    //    /// </param>
    //    /// <returns>
    //    ///     Смотритель за асинхронной операцией,
    //    ///     запросом.
    //    /// </returns>
    //    public AsyncOperationWatcher QueryDeleteUnitNode(int[] id_objs)
    //    {
    //        foreach (var item in id_objs)
    //            if (unitsCache.ContainsKey(item))
    //                unitsCache.Remove(item);

    //        return RemoteCallAsync(() => queryManager.BeginRemoveUnitNode(userGUID,
    //                                                                            id_objs));
    //    }
    //    public AsyncOperationWatcher QueryDeleteUnitNode(int id_obj)
    //    {
    //        return QueryDeleteUnitNode(new int[] { id_obj });
    //    }

    //    /// <summary>
    //    /// Запрос на добавление унит нода.
    //    /// Унит нод не добавляется в кеш, так как 
    //    /// на текущий момент не имеет валидной айдишки.
    //    /// </summary>
    //    /// <param name="unitType">
    //    ///     Тип нового нода.
    //    /// </param>
    //    /// <param name="parent_id">
    //    ///     Ид родительского нода.
    //    /// </param>
    //    /// <returns>
    //    ///     Смотритель за асинхронной операцией,
    //    ///     запросом.
    //    /// </returns>
    //    public int QueryAddUnitNode(UnitTypeId unitType, UnitNode parentNode)
    //    {
    //        return RemoteCall<int>(() =>
    //            queryManager.AddUnitNode(userGUID,
    //                                     unitType,
    //                                     parentNode == null ? 0 : parentNode.Idnum));
    //    }

    //    public void QueryAddUnitNode(UnitNode[] nodes, UnitNode parentNode)
    //    {
    //        RemoteCall(() => queryManager.AddUnitNode(userGUID, nodes, parentNode == null ? 0 : parentNode.Idnum));
    //    }

    //    /// <summary>
    //    /// Запрос на обновление унит нода.
    //    /// В кеше нод тоже обновиться.
    //    /// </summary>
    //    /// <param name="node">
    //    ///     Обновленный нод.
    //    /// </param>
    //    /// <returns>
    //    ///     Смотритель за асинхронной операцией,
    //    ///     запросом.
    //    /// </returns>
    //    //public AsyncOperationWatcher QueryUpdateUnitNode(UnitNode node, bool updateReference)
    //    public AsyncOperationWatcher<Object> QueryUpdateUnitNode(UnitNode node)
    //    {
    //        AsyncOperationWatcher<Object> watcher;

    //        watcher = RemoteCallAsync<Object>(() => queryManager.BeginUpdateUnitNode(userGUID,
    //                                                                           node));

    //        watcher.AddValueRecivedHandler((object x) =>
    //            {
    //                UnitNode unode = (UnitNode)x;
    //                AddUnitToCache(unode);
    //            });
    //        return watcher;
    //    }

    //    /// <summary>
    //    /// Построить дерево структуры с учетом фильтра
    //    /// </summary>
    //    /// <param name="uparams">корневые узлы, начиная с которых строится дерево</param>
    //    /// <param name="filterTypes">Фильтр типов</param>
    //    /// <param name="privileges">Фильтр прав доступа</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginGetUnitNodeTree(UnitNode[] uparams, UnitTypeId[] filterTypes, Privileges privileges)
    //    {
    //        List<int> nodeIDs;
    //        if (uparams == null) nodeIDs = null;
    //        else
    //        {
    //            nodeIDs = new List<int>();
    //            foreach (UnitNode node in uparams)
    //                nodeIDs.Add(node.Idnum);
    //        }

    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetUnitNodeTree(userGUID,
    //            nodeIDs == null ? null : nodeIDs.ToArray(), filterTypes, privileges));
    //    }

    //    /// <summary>
    //    /// Запрос подключенных серверов сбора данных
    //    /// </summary>
    //    /// <returns></returns>
    //    public string[] GetBlockUIDs()
    //    {
    //        return RemoteCall<String[]>(() => queryManager.GetBlockUIDs(userGUID));
    //    }

    //    /// <summary>
    //    /// Список доступных модулей сбора
    //    /// </summary>
    //    /// <param name="blockNode">Сервер сбора, с которого запрашиваютя имена модулей</param>
    //    /// <returns></returns>
    //    public IEnumerable<ModuleInfo> GetModuleLibNames(int blockId)
    //    {
    //        return RemoteCall<IEnumerable<ModuleInfo>>(() => queryManager.GetModuleLibNames(userGUID, blockId));
    //    }

    //    /// <summary>
    //    /// Запросить свойства для определенного модуля сбора
    //    /// </summary>
    //    /// <param name="blockNode">Сервер сбора, с которого запрашиваютя свойства</param>
    //    /// <param name="libName">Имя модуля сбора</param>
    //    /// <returns></returns>
    //    public IEnumerable<ItemProperty> GetModuleLibraryProperties(int blockId, String libName)
    //    {
    //        return RemoteCall<IEnumerable<ItemProperty>>(() => queryManager.GetModuleLibraryProperties(userGUID, blockId, libName));
    //    }

    //    public UnitAttributeProperty[] GetUnitNodeAttributeProperties(UnitNode unitNode)
    //    {
    //        UTypeNode[] types = Types;
    //        if (types != null)
    //        {
    //            List<UnitAttributeProperty> lstRes = new List<UnitAttributeProperty>();

    //            UTypeNode type = null;
    //            foreach (var item in types)
    //                if (item.Idnum == ((int)unitNode.Typ))
    //                {
    //                    type = item;
    //                    break;
    //                }

    //            UnitAttributeProperty ptr;
    //            if (type != null)
    //            {
    //                foreach (var item in type.Props.Split(';'))
    //                {
    //                    if (item != null && !string.IsNullOrEmpty(item.Trim()))
    //                    {
    //                        ptr = new UnitAttributeProperty(item, item, "", "");
    //                        lstRes.Add(ptr);
    //                    }
    //                }
    //            }

    //            return lstRes.ToArray();
    //        }
    //        else
    //            return RemoteCall<UnitAttributeProperty[]>(() => queryManager.GetUnitNodeAttributeProperties(userGUID, unitNode));
    //    }

    //    /// <summary>
    //    /// Переместить узел внутри родительского
    //    /// </summary>
    //    /// <param name="unitNode">Узел</param>
    //    /// <param name="destNode">
    //    /// Узел, рядом к которому перетащить перетаскиваемый узел. 
    //    /// Что бы добавить в конец - null.
    //    /// </param>
    //    /// <param name="addAfter">Поместить перетаскиваемый узел перед или после указанного.</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher BeginMoveUnitNode(UnitNode unitNode, UnitNode destNode, bool addAfter)// int index)
    //    {
    //        int destID = destNode == null ? 0 : destNode.Idnum;

    //        return RemoteCallAsync(() => queryManager.BeginMoveUnitNode(userGUID, unitNode.Idnum, destID, addAfter));
    //    }

    //    /// <summary>
    //    /// Переместить узел в новый родительский узел
    //    /// </summary>
    //    /// <param name="newParent">Новый родитель</param>
    //    /// <param name="unitNode">Узел</param>
    //    /// <param name="destNode">
    //    /// Узел, рядом к которому перетащить перетаскиваемый узел. 
    //    /// Что бы добавить в конец - null.
    //    /// </param>
    //    /// <param name="addAfter">Поместить перетаскиваемый узел перед или после указанного.</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher BeginMoveUnitNode(UnitNode newParent, UnitNode unitNode, UnitNode destNode, bool addAfter)
    //    {
    //        int destID = destNode == null ? 0 : destNode.Idnum;
    //        int newParentID = newParent == null ? 0 : newParent.Idnum;

    //        return RemoteCallAsync(() => queryManager.BeginMoveUnitNode(userGUID, newParentID, unitNode.Idnum, destID, addAfter));
    //    }

    //    /// <summary>
    //    /// Копировать элемент или ветку
    //    /// </summary>
    //    /// <param name="unitIDs">Ид узлов для копирования</param>
    //    /// <param name="recursive">true, что бы скопировать всю ветку и false - только один узел</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher QueryCopyUnitNode(int[] unitIDs, bool recursive)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginCopyUnitNode(userGUID, unitIDs, recursive));
    //    }

    //    /// <summary>
    //    /// Экспортировать указанные узлы в файл
    //    /// </summary>
    //    /// <param name="nodeIds">Ид экспортируемых узлов</param>
    //    /// <param name="beginValuesTime">Начальное время для экспорта значений</param>
    //    /// <param name="endValuesTime">Конечное время для экспорта значений</param>
    //    /// <param name="exportFormat">Формат файла</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginExport(int[] nodeIds, DateTime beginValuesTime, DateTime endValuesTime, ExportFormat exportFormat)
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginExport(userGUID, nodeIds, beginValuesTime, endValuesTime, exportFormat));
    //    }

    //    /// <summary>
    //    /// Получить узлы сохраненные в файле
    //    /// </summary>
    //    /// <param name="buffer">Содержимое файла</param>
    //    /// <param name="exportFormat">Формат файла</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher<ImportDataContainer> BeginImport(byte[] buffer, ExportFormat exportFormat)
    //    {
    //        return RemoteCallAsync<ImportDataContainer>(() => queryManager.BeginImport(userGUID, buffer, exportFormat));
    //    }

    //    /// <summary>
    //    /// Добавить импортируемые узлы
    //    /// </summary>
    //    /// <param name="rootNode">
    //    /// Базовый узел, в который будут импортироваться данные. 
    //    /// Для импорта в корень - должен быть null.
    //    /// </param>
    //    /// <param name="importContainer">Оберта с импортируемым деревом</param>
    //    /// <returns>вотчер</returns>
    //    /// <remarks>
    //    /// В случае совпадения типа и названия (а для параметров ещё и кода)
    //    /// Узел перезапишется, причем пока без запроса на подтверждение
    //    /// </remarks>
    //    public AsyncOperationWatcher BeginApplyImport(UnitNode rootNode, ImportDataContainer importContainer)
    //    {
    //        int rootNodeID = rootNode == null ? 0 : rootNode.Idnum;

    //        return RemoteCallAsync(() => queryManager.BeginApplyImport(userGUID, rootNodeID, importContainer));
    //    }

    //    /// <summary>
    //    /// Изменить код в формулах
    //    /// </summary>
    //    /// <param name="oldCode">Старый код</param>
    //    /// <param name="newCode">Новый код</param>
    //    /// <param name="unitNodes">Узлы, в которых требуется произвести замену</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher BeginChangeParameterCode(RevisionInfo revision, string oldCode, string newCode, UnitNode[] unitNodes)
    //    {
    //        if (unitNodes == null)
    //            return RemoteCallAsync(() => queryManager.BeginChangeParameterCode(userGUID, revision, oldCode, newCode));


    //        List<int> unitIds = new List<int>();

    //        foreach (UnitNode node in unitNodes)
    //        {
    //            unitIds.Add(node.Idnum);
    //        }

    //        return RemoteCallAsync(() => queryManager.BeginChangeParameterCode(userGUID, oldCode, newCode, unitIds.ToArray()));
    //    }
    //    #endregion

    //    #region Расчет
    //    /// <summary>
    //    /// Запросить зависимости параметра
    //    /// </summary>
    //    /// <param name="parameterNode">Параметр, чьи зависимочти требуется найти</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginGetDependence(RevisionInfo revision, ParameterNode parameterNode)
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetDependence(userGUID, revision, parameterNode.Idnum));
    //    }

    //    /// <summary>
    //    /// Есть ли параметры использующие данный код
    //    /// </summary>
    //    /// <param name="code">Код</param>
    //    /// <returns></returns>
    //    public bool HasReference(RevisionInfo revision, String code)
    //    {
    //        return RemoteCall<bool>(() => queryManager.HasReference(userGUID, revision, code));
    //    }

    //    /// <summary>
    //    /// Запросить ссылки на узел
    //    /// </summary>
    //    /// <param name="paramCode">Код параметра</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginGetReference(RevisionInfo revision, String paramCode)
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetReference(userGUID, revision, paramCode));
    //    }

    //    /// <summary>
    //    /// Начать расчет параметров
    //    /// </summary>
    //    /// <param name="parameters">Расчитываемые параметры</param>
    //    /// <param name="beginTime">Начальное время</param>
    //    /// <param name="startTime">Конечное время</param>
    //    /// <param name="recalcAll">Пересичтать все</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginCalc(UnitNode[] parameters, DateTime beginTime, DateTime startTime, bool recalcAll)
    //    {
    //        int[] parametersIDs = new int[parameters.Length];
    //        for (int i = 0; i < parameters.Length; i++)
    //            parametersIDs[i] = parameters[i].Idnum;

    //        return RemoteCallAsync<Object>(() => queryManager.BeginCalc(userGUID, parametersIDs, beginTime, startTime, recalcAll));
    //    }

    //    /// <summary>
    //    /// Запросить список функций, используемых в расчете
    //    /// </summary>
    //    /// <param name="revision">Ревизия для которой запрашивается список функций.</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginGetCalcFunctions(RevisionInfo revision)
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetCalcFunction(userGUID, revision));
    //    }

    //    /// <summary>
    //    /// Проверить формулу на ошибки
    //    /// </summary>
    //    /// <param name="formulaText">Формула</param>
    //    /// <param name="arguments">Аргументы условного параметра или функции</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher BeginCheckFormula(RevisionInfo revision, string formulaText, KeyValuePair<int, CalcArgumentInfo[]>[] arguments)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginCheckFormula(userGUID, revision, formulaText, arguments));
    //    }

    //    /// <summary>
    //    /// Начать запрос констант, используемых в расчете
    //    /// </summary>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginGetConsts()
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetConsts(userGUID));
    //    }

    //    /// <summary>
    //    /// Сохранить изменения констант
    //    /// </summary>
    //    /// <param name="constsInfo">Изменяемые константы</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher BeginSaveConsts(IEnumerable<ConstsInfo> constsInfo)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginSaveConsts(userGUID, constsInfo));
    //    }

    //    /// <summary>
    //    /// Удалить константы из системы
    //    /// </summary>
    //    /// <param name="constsInfo">Изменяемые константы</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher BeginRemoveConsts(IEnumerable<ConstsInfo> constsInfo)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginRemoveConsts(userGUID, constsInfo));
    //    }

    //    /// <summary>
    //    /// Запросить список пользовательских функций
    //    /// </summary>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher<Object> BeginGetCustomFunctions()
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetCustomFunctins(userGUID));
    //    }

    //    /// <summary>
    //    /// Сохранить пользовательские функции
    //    /// </summary>
    //    /// <param name="functionInfo">Сохраняемые функции</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher BeginSaveCustomFunctions(IEnumerable<CustomFunctionInfo> functionInfo)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginSaveCustomFunctions(userGUID, functionInfo));
    //    }

    //    /// <summary>
    //    /// Удалить пользовательские функции
    //    /// </summary>
    //    /// <param name="functionInfo">Удаляемые функции</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher BeginRemoveCustomFunctions(IEnumerable<CustomFunctionInfo> functionInfo)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginRemoveCustomFunctions(userGUID, functionInfo));
    //    }

    //    #region Циклический расчет
    //    /// <summary>
    //    /// Проверить запущен ли циклический расчет
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool IsRoundRobinStarted()
    //    {
    //        return RemoteCall<bool>(() => queryManager.IsRoundRobinStarted(userGUID));
    //    }

    //    /// <summary>
    //    /// Запустить циклический расчет
    //    /// </summary>
    //    public void StartRoundRobinCalc()
    //    {
    //        RemoteCall(() => queryManager.StartRoundRobinCalc(userGUID));
    //    }

    //    /// <summary>
    //    /// Остановить циклический расчет
    //    /// </summary>
    //    public void StopRoundRobinCalc()
    //    {
    //        RemoteCall(() => queryManager.StopRoundRobinCalc(userGUID));
    //    }

    //    /// <summary>
    //    /// Получить количество сообщений последнего циклического расчета
    //    /// </summary>
    //    /// <returns>Количество сообщений</returns>
    //    public int GetLastRoundRobinMessagesCount()
    //    {
    //        return RemoteCall<int>(() => queryManager.GetLastRoundRobinMessagesCount(userGUID));
    //    }

    //    /// <summary>
    //    /// Получить сообщения последнего циклического расчета
    //    /// </summary>
    //    /// <param name="start">Начальное сообщение</param>
    //    /// <param name="count">Количество сообщений</param>
    //    /// <returns>вотчер</returns>
    //    public AsyncOperationWatcher BeginGetLastRoundRobinMessages(int start, int count)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginGetLastRoundRobinMessages(userGUID, start, count));
    //    }

    //    /// <summary>
    //    /// Получить настройки циклического расчета
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool GetRoundRobinAutoStart()
    //    {
    //        return RemoteCall<bool>(() => queryManager.GetRoundRobinAutoStart(userGUID));
    //    }

    //    /// <summary>
    //    /// Сохранить настройики циклического расчета
    //    /// </summary>
    //    /// <param name="autoStart"></param>
    //    public void SetRoundRobinAutoStart(bool autoStart)
    //    {
    //        RemoteCall(() => queryManager.SetRoundRobinAutoStart(userGUID, autoStart));
    //    }

    //    /// <summary>
    //    /// Получить информацию о циклическом расчете
    //    /// </summary>
    //    /// <returns></returns>
    //    public RoundRobinInfo GetRoundRobinInfo()
    //    {
    //        return RemoteCall<RoundRobinInfo>(() => queryManager.GetRoundRobinInfo(userGUID));
    //    }
    //    #endregion
    //    #endregion

    //    #region Работа с отчетами
    //    /// <summary>
    //    /// Запросить информацию о доступных источниках данных для отчётов
    //    /// </summary>
    //    /// <returns></returns>
    //    public ReportSourceInfo[] GetReportSources()
    //    {
    //        return RemoteCall<ReportSourceInfo[]>(() => queryManager.GetReportSources(userGUID));
    //    }

    //    /// <summary>
    //    /// Запросить пустые настройки для указанного источника данных отчёта
    //    /// </summary>
    //    /// <param name="reportSourceID">ИД источника данных</param>
    //    /// <returns></returns>
    //    public ReportSourceSettings GetReportSourceSettings(Guid reportSourceID)
    //    {
    //        return RemoteCall<ReportSourceSettings>(() => queryManager.GetReportSourceSettings(userGUID, reportSourceID));
    //    }

    //    /// <summary>
    //    /// Подготовить данные для отчёта
    //    /// </summary>
    //    /// <param name="settings">Настройки источников данных</param>
    //    /// <param name="reportParameters">Параметры отчёта</param>
    //    /// <returns>
    //    /// Вотчер.
    //    /// Данные приходят в формате словаря ИД источника данных -- DataSet с данными.
    //    /// </returns>
    //    public AsyncOperationWatcher<FastReportWrap> BeginGenerateReportData(ReportSourceSettings[] settings, ReportParameter[] reportParameters)
    //    {
    //        return RemoteCallAsync<FastReportWrap>(
    //            () => queryManager.BeginGenerateReportData(userGUID, settings, reportParameters));
    //    }

    //    /// <summary>
    //    /// Сформировать отчёт.
    //    /// </summary>
    //    /// <param name="reportNode">Узел отчёта</param>
    //    /// <param name="saveInSystem">Флаг, указывающий, сохранять ли готовый отчёт в системе</param>
    //    /// <param name="reportParameters">Параметры отчёта</param>
    //    /// <returns>
    //    /// Вотчер.
    //    /// Данные приходят в виде тела готового отчёта.
    //    /// </returns>
    //    public AsyncOperationWatcher<byte[]> BeginGenerateReport(ReportNode reportNode, bool saveInSystem, ReportParameter[] reportParameters)
    //    {
    //        return RemoteCallAsync<byte[]>(() => queryManager.BeginGenerateReport(userGUID, reportNode.Idnum, saveInSystem, reportParameters));
    //    }

    //    /// <summary>
    //    /// Получить список готовых отчётов, сформированных за указанный итервал времени.
    //    /// </summary>
    //    /// <param name="dateFrom">Начальное время искомого интервала</param>
    //    /// <param name="dateTo">Конечное время искомого интервала</param>
    //    /// <returns>Вотчер.</returns>
    //    public AsyncOperationWatcher<PreferedReportInfo[]> BeginGetPreferedReports(DateTime dateFrom, DateTime dateTo)
    //    {
    //        return RemoteCallAsync<PreferedReportInfo[]>(() => queryManager.BeginGetPreferedReports(userGUID, dateFrom, dateTo));
    //    }

    //    /// <summary>
    //    /// Получить готовый отчёт
    //    /// </summary>
    //    /// <param name="report">Информация о готовом отчёте</param>
    //    /// <returns>
    //    /// Вотчер.
    //    /// Тело отчёта.
    //    /// </returns>
    //    public AsyncOperationWatcher<byte[]> BeginGetPreferedReportBody(PreferedReportInfo report)
    //    {
    //        return RemoteCallAsync<byte[]>(() => queryManager.BeginGetPreferedReportBody(userGUID, report));
    //    }

    //    /// <summary>
    //    /// Удалить готовый отчёт.
    //    /// </summary>
    //    /// <param name="report">Информация о готовом отчёте</param>
    //    public void DeletePreferedReport(PreferedReportInfo report)
    //    {
    //        RemoteCall(() => queryManager.DeletePreferedReport(userGUID, report));
    //    }
    //    #endregion

    //    /// <summary>
    //    /// Выставляет Парента по ИД  ФулНэйм у параметров
    //    /// </summary>
    //    /// <param name="node"></param>
    //    private void AddUnitToCache(UnitNode node)
    //    {
    //        SetChildFullName(node);
    //        if (unitsCache.ContainsKey(node.Idnum))
    //            unitsCache[node.Idnum] = node;
    //        else
    //            unitsCache.Add(node.Idnum, node);
    //    }
    //    /// <summary>
    //    /// Выставляет ФулНэйм у параметров
    //    /// </summary>
    //    /// <param name="node"></param>
    //    private void SetChildFullName(UnitNode node)
    //    {
    //        if (node != null && node.Parameters != null)
    //        {
    //            foreach (var item in node.Parameters)
    //            {
    //                if (unitsCache.ContainsKey(item.ParameterId))
    //                    item.SetFullName(unitsCache[item.ParameterId].FullName);
    //            }
    //        }
    //    }

    //    #region Работа с расписаниями получения параметров

    //    public AsyncOperationWatcher<Object> QueryParamsSchedules()
    //    {
    //        AsyncOperationWatcher<Object> watcher =
    //            RemoteCallAsync<Object>(() => queryManager.BeginGetParamsSchedules(userGUID));

    //        watcher.AddValueRecivedHandler((object x) =>
    //            {
    //                Schedule[] schedules = x as Schedule[];

    //                foreach (var it in schedules)
    //                    if (!scheduleCache.ContainsKey(it.Id))
    //                        scheduleCache.Add(it.Id, it);
    //            });

    //        return watcher;
    //    }

    //    public Schedule TryGetParamsSchedule(int id)
    //    {
    //        return scheduleCache.ContainsKey(id) ? scheduleCache[id] :
    //                                               null;
    //    }

    //    public Schedule[] GetParamsSchedules()
    //    {
    //        return RemoteCall<Schedule[]>(() => queryManager.GetParamsSchedules(userGUID));
    //    }
    //    public Schedule GetParamsSchedule(int id)
    //    {
    //        return RemoteCall<Schedule>(() => queryManager.GetParamsSchedule(userGUID, id));
    //    }
    //    public Schedule GetParamsSchedule(string name)
    //    {
    //        return RemoteCall<Schedule>(() => queryManager.GetParamsSchedule(userGUID, name));
    //    }
    //    public AsyncOperationWatcher<Object> QueryParamsSchedule(int id)
    //    {
    //        var watcher =
    //            RemoteCallAsync<Object>(() => queryManager.BeginGetParamsSchedule(userGUID,
    //                                                                              id));

    //        watcher.AddValueRecivedHandler((object x) =>
    //            {
    //                Schedule schedule = x as Schedule;

    //                if (!scheduleCache.ContainsKey(schedule.Id))
    //                    scheduleCache.Add(schedule.Id, schedule);
    //            });

    //        return watcher;
    //    }

    //    public AsyncOperationWatcher QueryParamsSchedule(string name)
    //    {
    //        var watcher =
    //            RemoteCallAsync<Object>(() => queryManager.BeginGetParamsSchedule(userGUID,
    //                                                                              name));

    //        watcher.AddValueRecivedHandler((object x) =>
    //        {
    //            Schedule schedule = x as Schedule;

    //            if (!scheduleCache.ContainsKey(schedule.Id))
    //                scheduleCache.Add(schedule.Id, schedule);
    //        });

    //        return watcher;
    //    }

    //    public AsyncOperationWatcher QueryAddParamsSchedule(Schedule added)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginAddParamsSchedule(userGUID,
    //                                                                                added));
    //    }

    //    public AsyncOperationWatcher QueryUpdateParamsSchedule(Schedule updated)
    //    {
    //        AsyncOperationWatcher watcher =
    //            RemoteCallAsync(() => queryManager.BeginUpdateParamsSchedule(userGUID,
    //                                                                               updated));

    //        watcher.AddFinishHandler(() =>
    //            {
    //                if (scheduleCache.ContainsKey(updated.Id))
    //                    scheduleCache[updated.Id] = updated;
    //                else
    //                    scheduleCache.Add(updated.Id, updated);
    //            });

    //        return watcher;
    //    }

    //    public AsyncOperationWatcher QueryDeleteParamsSchedule(int id)
    //    {
    //        AsyncOperationWatcher watcher =
    //            RemoteCallAsync(() => queryManager.BeginDeleteParamsSchedule(userGUID,
    //                                                                               id));

    //        watcher.AddFinishHandler(() =>
    //            {
    //                if (scheduleCache.ContainsKey(id))
    //                    scheduleCache.Remove(id);
    //            });

    //        return watcher;
    //    }

    //    #endregion

    //    #region Работа с мнемосхемными обновлениями
    //    /// <summary> Регистрирует на сервере приложения клиента, 
    //    /// для обновления значений мнемосхем
    //    /// </summary>
    //    /// <param name="authority">Параметры авторизации</param>
    //    /// <param name="parameters">Массив технологических параметров, 
    //    /// требующихся клиенту</param>
    //    public int RegisterClient(ParamValueItemWithID[] parameters)
    //    {
    //        return RemoteCall<int>(() => queryManager.RegisterClient(userGUID, parameters));
    //    }
    //    /// <summary>
    //    /// Разрегистрация массива параметров
    //    /// </summary>
    //    /// <param name="taID">Номер транзакции</param>
    //    public void UnRegisterClient(int taID)
    //    {
    //        RemoteCall(() => queryManager.UnRegisterClient(userGUID, taID));
    //    }
    //    /// <summary>
    //    /// Получение значений параметров
    //    /// </summary>
    //    /// <param name="taID">Номер транзакции</param>
    //    /// <returns>Массив параметров со значениями</returns>
    //    public ParamValueItemWithID[] GetValuesFromBank(int taID)
    //    {
    //        return RemoteCall<ParamValueItemWithID[]>(() => queryManager.GetValuesFromBank(userGUID, taID));
    //    }
    //    #endregion

    //    // hack {
    //    public AsyncOperationWatcher SendSprav(int blockId)
    //    {
    //        return RemoteCallAsync(() => queryManager.SendDataToBlockServer(userGUID, blockId));
    //    }
    //    // hack }

    //    public void DeleteLoadValues(UnitNode unitNode, DateTime dateTime)
    //    {
    //        RemoteCall(() => queryManager.DeleteLoadValues(userGUID, unitNode.Idnum, dateTime));
    //    }

    //    /// <summary>
    //    /// Реализация работы с асинхронными 
    //    /// операциями. уже менее Сырые интерфейсы.
    //    /// </summary>
    //    bool IAsyncOperationManager.IsComplete(ulong id)
    //    {
    //        return RemoteCall<bool>(() => queryManager.GetOperationState(userGUID, id).IsCompleted, true);
    //    }

    //    double IAsyncOperationManager.GetProgress(ulong id)
    //    {
    //        return RemoteCall<double>(() => queryManager.GetOperationState(userGUID, id).Progress, true);
    //    }

    //    string IAsyncOperationManager.GetStatus(ulong id)
    //    {
    //        return RemoteCall<String>(() => queryManager.GetOperationState(userGUID, id).StateString, true);
    //    }

    //    bool IAsyncOperationManager.IsInterrupted(ulong id)
    //    {
    //        return RemoteCall<bool>(() => queryManager.GetOperationState(userGUID, id).IsInterrupted, true);
    //    }

    //    Object IAsyncOperationManager.GetResult(ulong id)
    //    {
    //        return RemoteCall<Object>(() => queryManager.GetOperationResult(userGUID, id), true);
    //    }

    //    public Message[] GetMessages(ulong id)
    //    {
    //        return RemoteCall<Message[]>(() => queryManager.GetOperationMessages(userGUID, id), true);
    //    }

    //    Exception IAsyncOperationManager.GetError(ulong id)
    //    {
    //        Exception exc = RemoteCall<Exception>(() => queryManager.GetOperationState(userGUID, id).Error);
    //        if (exc is UserNotConnectedException)
    //        {
    //            OnUserDisconnected();
    //            return null;
    //        }
    //        return exc;
    //    }

    //    void IAsyncOperationManager.End(ulong id)
    //    {
    //        RemoteCall(() => queryManager.EndAsyncOperation(userGUID, id));
    //    }

    //    bool IAsyncOperationManager.Abort(ulong id)
    //    {
    //        RemoteCall(() => queryManager.AbortOperation(userGUID, id));
    //        return true;
    //    }

    //    public AsyncOperationWatcher<Object> BeginGetParameters(ChannelNode canalNode)
    //    {
    //        return RemoteCallAsync<Object>(() => queryManager.BeginGetParameters(userGUID, canalNode.Idnum));
    //    }

    //    public AsyncOperationWatcher QueryLogs(DateTime from, DateTime to)
    //    {
    //        return RemoteCallAsync(() => queryManager.BeginGetLogs(from, to));
    //    }


    //    #region Работа с расширениями
    //    public bool ExternalIDSupported(ExtensionUnitNode unitNode)
    //    {
    //        return RemoteCall<bool>(() => queryManager.ExternalIDSupported(userGUID, unitNode.Idnum));
    //    }

    //    public bool ExternalCodeSupported(ExtensionUnitNode unitNode)
    //    {
    //        return RemoteCall<bool>(() => queryManager.ExternalCodeSupported(userGUID, unitNode.Idnum));
    //    }

    //    public bool ExternalIDCanAdd(ExtensionUnitNode unitNode)
    //    {
    //        return RemoteCall<bool>(() => queryManager.ExternalIDCanAdd(userGUID, unitNode.Idnum));
    //    }

    //    public EntityStruct[] ExternalIDList(ExtensionUnitNode unitNode)
    //    {
    //        return RemoteCall<EntityStruct[]>(() => queryManager.ExternalIDList(userGUID, unitNode.Idnum));
    //    }

    //    public ItemProperty[] GetExternalProperties(ExtensionUnitNode unitNode)
    //    {
    //        return RemoteCall<ItemProperty[]>(() => queryManager.GetExternalProperties(userGUID, unitNode.Idnum));
    //    }

    //    public ExtensionDataInfo[] GetExtensionTableInfo()
    //    {
    //        return RemoteCall<ExtensionDataInfo[]>(() => queryManager.GetExtensionTableInfo(userGUID));
    //    }

    //    public ExtensionDataInfo[] GetExtensionTableInfo(ExtensionUnitNode extensionUnitNode)
    //    {
    //        return RemoteCall<ExtensionDataInfo[]>(() => queryManager.GetExtensionTableInfo(userGUID, extensionUnitNode.Idnum));
    //    }

    //    public ExtensionDataInfo[] GetExtensionTableInfo(String extensionName)
    //    {
    //        return RemoteCall<ExtensionDataInfo[]>(() => queryManager.GetExtensionTableInfo(userGUID, extensionName));
    //    }

    //    public AsyncOperationWatcher<ExtensionData> BeginGetExtensionExtendedTable(ExtensionUnitNode unitNode, String tableName, DateTime dateFrom, DateTime dateTo)
    //    {
    //        return RemoteCallAsync<ExtensionData>(() => queryManager.BeginGetExtensionExtendedTable(userGUID, unitNode.Idnum, tableName, dateFrom, dateTo));
    //    }

    //    public AsyncOperationWatcher<ExtensionData> BeginGetExtensionExtendedTable(ExtensionUnitNode unitNode, ExtensionDataInfo tableInfo, DateTime dateFrom, DateTime dateTo)
    //    {
    //        return BeginGetExtensionExtendedTable(unitNode, tableInfo.Name, dateFrom, dateTo);
    //    }

    //    public AsyncOperationWatcher<ExtensionData> BeginGetExtensionExtendedTable(ExtensionUnitNode unitNode, String tableName)
    //    {
    //        return BeginGetExtensionExtendedTable(unitNode, tableName, DateTime.MinValue, DateTime.MaxValue);
    //    }

    //    public AsyncOperationWatcher<ExtensionData> BeginGetExtensionExtendedTable(String extensionName, ExtensionDataInfo tableInfo)
    //    {
    //        return RemoteCallAsync<ExtensionData>(() => queryManager.BeginGetExtensionExtendedTable(userGUID, extensionName, tableInfo.Name, DateTime.MinValue, DateTime.MaxValue));
    //    }
    //    #endregion

    //    public UnitNode GetParent(UnitNode unitNode, UnitTypeId typeID)
    //    {
    //        return RemoteCall<UnitNode>(() => queryManager.GetParentUnitNode(userGUID, unitNode.Idnum, typeID));
    //    }

    //    internal AsyncOperationWatcher BeginChangeArguments(OptimizationGateNode TopOptimizationNode, DateTime CurrentTime, KeyValuePair<ArgumentsValues, ArgumentsValues>[] keyValuePair)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    internal Dictionary<UnitTypeId, int> GetStatistic(UnitNode unitNode)
    //    {
    //        return RemoteCall<Dictionary<UnitTypeId, int>>(() => queryManager.GetStatistic(userGUID, unitNode.Idnum));
    //    }

    //    internal IEnumerable<ASC.Audit.AuditEntry> GetAudit(DateTime timeStart, DateTime timeEnd)
    //    {
    //        return RemoteCall<IEnumerable<ASC.Audit.AuditEntry>>(() => queryManager.GetAudit(userGUID, timeStart, timeEnd));
    //    }

    //    internal IEnumerable<ASC.Audit.AuditEntry> GetAudit(int indexStart, int indexEnd)
    //    {
    //        return RemoteCall<IEnumerable<ASC.Audit.AuditEntry>>(() => queryManager.GetAudit(userGUID, indexStart, indexEnd));
    //    }

    //    internal IEnumerable<IntervalDescription> GetStandardIntervals()
    //    {
    //        return RemoteCall<IntervalDescription[]>(() => queryManager.GetStandardsIntervals(userGUID));
    //    }

    //    internal void RemoveStandardIntervals(IEnumerable<IntervalDescription> intervalsToRemove)
    //    {
    //        RemoteCall(() => queryManager.RemoveStandardIntervals(userGUID, intervalsToRemove.ToArray()));
    //    }

    //    internal void SaveStandardIntervals(IEnumerable<IntervalDescription> modifiedIntervals)
    //    {
    //        RemoteCall(() => queryManager.SaveStandardIntervals(userGUID, modifiedIntervals.ToArray()));
    //    }
    //}
}

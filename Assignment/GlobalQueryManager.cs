using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ASC.Audit;
using COTES.ISTOK.Assignment.Audit;
using COTES.ISTOK.Assignment.Extension;
using COTES.ISTOK.Calc;
using COTES.ISTOK.DiagnosticsInfo;
using COTES.ISTOK.Extension;
using NLog;

namespace COTES.ISTOK.Assignment
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    internal class GlobalQueryManager : MarshalByRefObject, IGlobalQueryManager, ITransmitterExtension, IDisposable, IGlobal
    {
        Logger log = LogManager.GetCurrentClassLogger();

        public static GlobalQueryManager globSvcManager = null;

        // Объект, отвечающий за асинхронные операции
        public AsyncOperation asyncOperation;
        IUserManager userManager;
        ISecurityManager securityManager;
        ILockManager lockManager;
        IUnitTypeManager unitTypeManager;
        ScheduleManager scheduleManager;
        IUnitManager unitManager;
        BlockProxy blockProxy;
        ValueReceiver valueReceiver;
        ParameterRegistrator registrator;
        ExtensionManager extensionManager;
        CalcServerAdapter calcServer;
        AssignmentCalcSupplier calcSupplier;
        ReportUtility reportUtility;
        ExportImportManager exportImportManager;
        Diagnostics globalDiagnostics;
        RevisionManager revisionManager;
        IAuditServer auditServer;

        ServiceDataChangeRegistrator dataChangeRegistrator;

//        const bool useBlock =
//#if EMA
//            false;
//#else
// true;
//#endif

        //ParamsLoadManager paramLoadManager = null;

        /// <summary>
        /// Список потоков, отвечающих за обновление значений параметров
        /// </summary>
        List<Thread> lstParamUpdateThreads = new List<Thread>();

        //        ValueReceiver receiver = new ValueReceiver(CommonData.eventLog);
        internal GlobalQueryManager(IUserManager userManager,
                                    ISecurityManager securityManager,
                                    ILockManager lockManager,
                                    IUnitManager unitManager,
                                    IUnitTypeManager unitTypeManager,
                                    ScheduleManager scheduleManager,
                                    BlockProxy blockProxy,
                                    ValueReceiver valueReceiver,
                                    ParameterRegistrator registrator,
                                    ExtensionManager extensionManager,
                                    AssignmentCalcSupplier calcSupplier,
                                    CalcServerAdapter calcServer,
                                    ReportUtility reportUtility,
                                    ExportImportManager exportImportManager,
                                    Diagnostics globalDiagnostics,
                                    RevisionManager revisionManager,
                                    COTES.ISTOK.Assignment.Audit.IAuditServer auditServer)
        {
            asyncOperation = new AsyncOperation();
            this.userManager = userManager;
            this.securityManager = securityManager;
            this.lockManager = lockManager;
            this.unitManager = unitManager;
            this.unitTypeManager = unitTypeManager;
            this.scheduleManager = scheduleManager;
            this.blockProxy = blockProxy;
            this.valueReceiver = valueReceiver;
            this.registrator = registrator;
            this.extensionManager = extensionManager;
            this.calcServer = calcServer;
            this.calcSupplier = calcSupplier;
            this.reportUtility = reportUtility;
            this.exportImportManager = exportImportManager;
            this.globalDiagnostics = globalDiagnostics;
            this.revisionManager = revisionManager;
            //this.paramLoadManager = new ParamsLoadManager(asyncOperation,
            //                                              valueReceiver.SaveValues);

            dataChangeRegistrator = new ServiceDataChangeRegistrator();
            this.securityManager.SessionRegistered += new EventHandler<SessionIDEventArgs>((s, e) => dataChangeRegistrator.SessionStarted(e.SessionGUID));
            this.securityManager.SessionUnregistered += new EventHandler<SessionIDEventArgs>((s, e) => dataChangeRegistrator.SessionStopped(e.SessionGUID));
            this.unitManager.UnitNodeChanged += new EventHandler<UnitNodeEventArgs>((s, e) =>
            {
                dataChangeRegistrator.Enqueue(new ServiceUnitNodeChange(e.UnitNodeID));
            });
            this.unitTypeManager.UnitTypeChanged += new EventHandler<UnitTypeEventArgs>((s, e) =>
            {
                dataChangeRegistrator.Enqueue(new ServiceUnitTypeChange((int)e.UnitTypeID));
            });
            this.auditServer = auditServer;
        }

        internal void Clear()
        {
            try
            {
                log.Info("Выполняется остановка потоков обновления значений.");
                while (lstParamUpdateThreads.Count > 0)
                {
                    Thread thr = lstParamUpdateThreads[0];
                    if (thr != null /*&& thr.ThreadState == ThreadState.Running*/)
                        thr.Abort();
                    lstParamUpdateThreads.RemoveAt(0);
                }
                log.Info("Остановка потоков обновления значений выполнена.");
            }
            catch (Exception exp)
            {
                log.ErrorException("", exp);
                //throw;
            }
            try
            {
                log.Info("Выполняется очистка списка асинхронных операций.");
                asyncOperation.Dispose();
                log.Info("Очистка списка асинхронных операций выполнена.");
            }
            catch (Exception exp)
            {
                log.ErrorException("", exp);
                //throw;
            }
        }

        private OperationState GetOperationState(Guid userGuid)
        {
            securityManager.ValidateAccess(userGuid);

            return new OperationState(userGuid);
        }

        public ReturnContainer Return(Guid userGuid)
        {
            return new ReturnContainer(dataChangeRegistrator.Dequeue(userGuid));
        }

        public ReturnContainer<T> Return<T>(Guid userGuid, T value)
        {
            return new ReturnContainer<T>(value, dataChangeRegistrator.Dequeue(userGuid));
        }

        #region Работа с блочными

        public bool AttachBlock(string uid, IDictionary connAttributes)
        {
            try
            {
                return blockProxy.AttachBlockE(uid, connAttributes);
            }
            catch (Exception exp)
            {
                log.ErrorException("", exp);
                return ExceptionMethod<bool>(exp);
            }
        }

        public ReturnContainer<string[]> GetBlockUIDs(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);
                return Return(userGuid, blockProxy.GetBlockUIDs());
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<string[]>(ex));
            }
        }

        List<int> processingSchedules = new List<int>();

        public ReturnContainer<ModuleInfo[]> GetModuleLibNames(Guid userGuid, int blockID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                //BlockNode blockNode = unitManager.GetUnitNode(state, blockID) as BlockNode;
                BlockNode blockNode = unitManager.ValidateUnitNode<BlockNode>(state, blockID, Privileges.Read);

                if (blockNode == null) throw new ArgumentException("Неверный идентификатор узла");

                var modules = blockProxy.GetModuleLibNames(blockNode);

                return Return(userGuid, modules == null ? null : modules.ToArray());
            }
            catch (Exception ex)
            {
                log.ErrorException("Ошибка запроса имен модулей.", ex);
                return Return(userGuid, ExceptionMethod<ModuleInfo[]>(ex));
            }
        }

        public ReturnContainer<ItemProperty[]> GetModuleLibraryProperties(Guid userGuid, int blockId, String libName)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                BlockNode blockNode = unitManager.ValidateUnitNode<BlockNode>(state, blockId, Privileges.Read);

                //if (blockNode == null) throw new ArgumentException("Неверный идентификатор узла");

                return Return(userGuid, blockProxy.GetModuleLibraryProperties(blockNode, libName).ToArray());
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ItemProperty[]>(ex));
            }
        }

        #endregion

        #region Schema parts
        /// <summary> Регистрирует на сервере приложения клиента, 
        /// для обновления значений мнемосхем
        /// </summary>
        /// <param name="authority">Параметры авторизации</param>
        /// <param name="parameters">Массив технологических параметров, 
        /// требующихся клиенту</param>
        public ReturnContainer<int> RegisterClient(Guid userGuid, ParamValueItemWithID[] parameters)
        {
            OperationState state = GetOperationState(userGuid);
            
            int taID = 0;
            if (parameters == null || parameters.Length == 0) return Return(userGuid, taID);

            try
            {
                //GNSI.Trace("Start RegisterClient");
                taID = registrator.Register(parameters);
                List<ParamValueItemWithID> block_params = new List<ParamValueItemWithID>();
                foreach (BlockNode item in blockProxy.Blocks)
                {
                    try
                    {
                        block_params.Clear();
                        //block_params = item.FilterParameters(parameters);
                        foreach (ParamValueItemWithID param in parameters)
                        {
                            if (unitManager.Find(state, item, param.ParameterID) != null)
                                block_params.Add(param);
                        }
                        if (block_params != null && block_params.Count > 0)
                        {
                            ParametersTransaction ta;
                            int tmp_ID;

                            tmp_ID = registrator.Register(block_params.ToArray());
                            ta = registrator.GetTransaction(tmp_ID);
                            RegisterBlockParameters(taID, ta, item);
                        }
                    }
                    catch { }
                }

            }
            catch (Exception ex)
            {
                log.ErrorException("Ошибка регистрации группы параметров на обновление.", ex);
                return Return(userGuid, ExceptionMethod<int>(ex));
            }

            var user = securityManager.GetUserInfo(userGuid);
            log.Info("Пользователь id{0} - {1} зарегистрировал транзакцию с id{2} на обновление",
                        user.Idnum,
                        user.Text,
                        taID);

            return Return(userGuid, taID);
        }

        /// <summary>
        /// Словарь состояний потоков обновления значений параметров (true - работать, false - закончить)
        /// ключ - id транзакции, значение - состояние
        /// </summary>
        volatile Dictionary<int, bool> dicParamUpdateThreadState = new Dictionary<int, bool>();

        /// <summary>
        /// Регистрация параметров на блочном для обновления
        /// </summary>
        /// <param name="taID">Номер общей (на глобале) транзакции</param>
        /// <param name="ta">Транзакция с массивом параметров</param>
        /// <param name="block">Узел блочного сервера</param>
        private void RegisterBlockParameters(int taID, ParametersTransaction ta, BlockNode block)
        {
            try
            {
                ParamUpdateInfo info = new ParamUpdateInfo(taID, ta, block);
                Thread thr = new Thread(new ParameterizedThreadStart(ParamUpdateThread));

                thr.Name = taID.ToString();
                lock (dicParamUpdateThreadState)
                {
                    dicParamUpdateThreadState[taID] = true;
                }
                lstParamUpdateThreads.Add(thr);
                thr.Start(info);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
        }

        /// <summary>
        /// Разрегистрация массива параметров
        /// </summary>
        /// <param name="taID">Номер транзакции</param>
        public ReturnContainer UnRegisterClient(Guid userGuid, int taID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                List<Thread> lstRemove = new List<Thread>();
                var lstDicRemove = new List<int>();
                string name = taID.ToString();

                foreach (Thread item in lstParamUpdateThreads)
                    if (item.Name == name)
                    {
                        lock (dicParamUpdateThreadState)
                        {
                            if (dicParamUpdateThreadState.ContainsKey(taID))
                            {
                                dicParamUpdateThreadState[taID] = false;
                                lstDicRemove.Add(taID);
                            }
                        }
                        item.Interrupt();
                        //item.Abort();
                        lstRemove.Add(item);
                    }

                lock (dicParamUpdateThreadState)
                {
                    foreach (var item in lstDicRemove)
                        dicParamUpdateThreadState.Remove(item);
                }

                foreach (Thread item in lstRemove)
                {
                    if (!item.Join(1000)) item.Abort();
                    lstParamUpdateThreads.Remove(item);
                }

                registrator.Unregister(taID);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }

            var user = securityManager.GetUserInfo(userGuid);
            log.Info("Пользователь id{0} - {1} разрегистрировал транзакцию с id{2} на обновление",
                        user.Idnum,
                        user.Text,
                        taID);
            return Return(userGuid);
        }

        /// <summary>
        /// Получение значений параметров
        /// </summary>
        /// <param name="taID">Номер транзакции</param>
        /// <returns>Массив параметров со значениями</returns>
        public ReturnContainer<ParamValueItemWithID[]> GetValuesFromBank(Guid userGuid, int taID)
        {
            OperationState state = new OperationState(userGuid);

            try
            {
                return Return(userGuid, registrator.GetValues(taID));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ParamValueItemWithID[]>(ex));
            }
        }

        /// <summary>
        /// Метод потока, который периодически обновляет значения параметров
        /// </summary>
        private void ParamUpdateThread(object param)
        {
            ParamValueItemWithID[] values;
            ParamValueItemWithID[] target;
            ParamUpdateInfo info = null;
            BlockNode node;
            int taID = 0;
            int nodeId = 0;

            try
            {
                info = (ParamUpdateInfo)param;
                nodeId = info.Node.Idnum;
                target = info.Transaction.Parameters;

                bool alive = false;
                lock (dicParamUpdateThreadState)
                {
                    if (dicParamUpdateThreadState.ContainsKey(info.TaID))
                        alive = dicParamUpdateThreadState[info.TaID];
                }
                while (alive)
                {
                    try
                    {
                        node = null;
                        foreach (var item in blockProxy.Blocks)
                        {
                            if (item.Idnum == nodeId)
                            {
                                node = item;
                                break;
                            }
                        }
                        if (node != null && node/*.BlockManager*/ != null)
                        {
                            if (taID == 0)
                                taID = blockProxy.RegisterClient(node, target);

                            values = blockProxy.GetValuesFromBank(node, taID);
                            foreach (ParamValueItemWithID item in values)
                            {
                                foreach (ParamValueItemWithID t_item in target)
                                {
                                    if (item.ParameterID == t_item.ParameterID)
                                    {
                                        lock (t_item)
                                        {
                                            t_item.Quality = item.Quality;
                                            t_item.Time = item.Time;
                                            t_item.Value = item.Value;
                                            //t_item.corrval = item.corrval;
                                            //t_item.channel = item.channel;
                                            t_item.ChangeTime = item.ChangeTime;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (ThreadInterruptedException) { throw; }
                    catch (ThreadAbortException) { throw; }
                    catch (Exception)
                    {
                        taID = 0;
                    }

                    //Чтоб не сильно часто обновлять
                    if (info.UpdateInterval < 100)
                        Thread.Sleep(100);
                    else
                        Thread.Sleep(info.UpdateInterval);
                    lock (dicParamUpdateThreadState)
                    {
                        if (dicParamUpdateThreadState.ContainsKey(info.TaID))
                            alive = dicParamUpdateThreadState[info.TaID];
                        else
                            alive = false;
                    }
                }
            }
            catch (ThreadInterruptedException) { }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                log.ErrorException("", ex);
                //
            }
            finally
            {
                try
                {
                    if (info != null)
                    {
                        if (info.Transaction != null)
                            registrator.Unregister(info.Transaction.ID);
                        if (taID != 0)
                            blockProxy.UnRegisterClient(info.Node, taID);
                    }
                }
                catch { }
            }
        }
        #endregion

        /// <summary>
        /// This is to insure that when created as a Singleton, the first instance never dies,
        /// regardless of the expired time.
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            System.Runtime.Remoting.Lifetime.ILease lease =
                  base.InitializeLifetimeService() as System.Runtime.Remoting.Lifetime.ILease;

            if (lease != null)
            {
                lease.InitialLeaseTime = TimeSpan.Zero;
            }
            return lease;
            //return null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            //if (dbwork != null) dbwork.Dispose();
        }

        #endregion

        #region ITestConnection Members

        public bool Test(Object state)
        {
            return true;
        }

        public bool Test(String blockUID)
        {
            return blockProxy.Attached(blockUID);
        }

        #endregion

        /// <summary>
        /// Создаёт новую сессию для пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns>Идентификатор сессии</returns>
        public ConnectReturnContainer Connect(String userName, String password)
        {
            try
            {
                return new ConnectReturnContainer()
                {
                    UserGuid = securityManager.Register(userName, password),
                    AliveTimeout = securityManager.UserTimeout
                };
            }
            catch (Exception ex)
            {
                return ExceptionMethod<ConnectReturnContainer>(ex);
            }
        }

        /// <summary>
        /// Удаляет информацию о сессии клиента
        /// </summary>
        /// <param name="sessionGuid">Идентификатор сессии</param>
        public void Disconnect(Guid sessionGuid)
        {
            try
            {
                securityManager.Unregister(sessionGuid);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
        }

        public ReturnContainer<UserNode> GetUser(Guid userGuid)
        {
            try
            {
                return Return(userGuid, securityManager.GetUserInfo(userGuid));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UserNode>(ex));
            }
        }

        public ReturnContainer Alive(Guid userGuid)
        {
            try
            {
                securityManager.Alive(userGuid);
            }
            catch (Exception ex)
            {
                ExceptionMethod<Object>(ex);
            }
            return Return(userGuid);
        }

        #region Блокировки
        public ReturnContainer LockNode(Guid userGuid, int nodeID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                //UnitNode node = unitManager.GetUnitNode(state, nodeID);
                UnitNode node = unitManager.ValidateUnitNode<UnitNode>(state, nodeID, Privileges.Write);
                lockManager.LockNode(state, node);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        public ReturnContainer ReleaseNode(Guid userGuid, int nodeID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                UnitNode node = unitManager.ValidateUnitNode(state, nodeID, Privileges.Write);
                lockManager.ReleaseNode(state, node);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        public ReturnContainer LockValues(Guid userGuid, int nodeID, DateTime startTime, DateTime endTime)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                UnitNode node = unitManager.ValidateUnitNode(state, nodeID, Privileges.Execute);
                lockManager.LockValues(state, node, startTime, endTime);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        public ReturnContainer ReleaseValues(Guid userGuid, int nodeID, DateTime startTime, DateTime endTime)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                UnitNode node = unitManager.ValidateUnitNode(state, nodeID, Privileges.Read);
                if (node == null)
                    return Return(userGuid);

                lockManager.ReleaseValues(state, node, startTime, endTime);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        public ReturnContainer ReleaseAll(Guid userGuid, int nodeID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                UnitNode node = unitManager.ValidateUnitNode(state, nodeID, Privileges.Read);
                lockManager.ReleaseAll(state, node);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }
        #endregion

        /// <summary>
        /// Получить имя пользователя по UID
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="userID">Идентификатор пользователя</param>
        /// <returns>Имя пользователя</returns>
        public ReturnContainer<string> GetUserLogin(Guid userGuid, int userID)
        {
            OperationState state = GetOperationState(userGuid);

            try
            {
                UserNode user = userManager.GetUser(userID);
                if (user != null) return Return(userGuid, user.Text);
                return Return(userGuid, String.Empty);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<string>(ex));
            }
        }

        public ReturnContainer<double> GetLoadProgress(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                return Return(userGuid, LoadState.Progress);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<double>(ex));
            }
        }

        public ReturnContainer<string> GetLoadStatusString(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                return Return(userGuid, LoadState.StateString);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<string>(ex));
            }
        }

        #region Диагностика
        /// <summary>
        /// Возвращает диагностический объект
        /// </summary>
        /// <returns></returns>
        public ReturnContainer<COTES.ISTOK.DiagnosticsInfo.Diagnostics> GetDiagnosticsObject(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAdminAccess(userGuid);
                return Return(userGuid, globalDiagnostics);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<COTES.ISTOK.DiagnosticsInfo.Diagnostics>(ex));
            }
        }

        internal COTES.ISTOK.DiagnosticsInfo.ISummaryInfo GetAsyncOperationManager()
        {
            return asyncOperation;
        }
        #endregion

        #region Работа с типами
        /// <summary>
        /// Запросить типы оборудования
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<UTypeNode[]> GetUnitTypes(Guid userGuid)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                return Return(userGuid, unitTypeManager.GetUnitTypes(state));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UTypeNode[]>(ex));
            }
        }

        /// <summary>
        /// Изменить тип оборудования
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="unitTypeNode">Тип оборудования для сохранения</param>
        public ReturnContainer UpdateUnitType(Guid userGuid, UTypeNode unitTypeNode)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                unitTypeManager.UpdateUnitType(state, unitTypeNode);
                log.Info("Пользователь {{0}} обновил тип {1}.",
                               GetUserLogInfo(userGuid),
                               FormatObjectLogDescription(unitTypeNode.Idnum,
                                                          unitTypeNode.Text));
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        /// <summary>
        /// Добавить тип оборудования
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="unitTypeNode">Тип оборудования для добавления</param>
        public ReturnContainer AddUnitType(Guid userGuid, UTypeNode unitTypeNode)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                unitTypeManager.AddUnitType(state, unitTypeNode);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        /// <summary>
        /// Удалить тип оборудования
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="unitTypeNodesIDs">Идентификатор удаляемых типов оборудования</param>
        public ReturnContainer RemoveUnitType(Guid userGuid, int[] unitTypeNodesIDs)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                double step = AsyncOperation.MaxProgressValue / unitTypeNodesIDs.Length;
                foreach (int unitTypeNodeID in unitTypeNodesIDs)
                {
                    unitTypeManager.RemoveUnitType(state, unitTypeNodeID);
                    state.Progress += step;
                }
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }
        #endregion

        #region Работа с пользователями
        /// <summary>
        /// Запросит пользователей системы
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<UserNode[]> GetUserNodes(Guid userGuid)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                return Return(userGuid, userManager.GetUserNodes(state));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UserNode[]>(ex));
            }
        }

        /// <summary>
        /// Запросит пользователей системы
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<UserNode[]> GetUserNodesByIds(Guid userGuid, int[] ids)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                var it = userManager.GetUserNodes(state, ids);
                return Return(userGuid, it);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UserNode[]>(ex));
            }
        }

        /// <summary>
        /// Изменить пользователя системы
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="userNode">Редактируемый пользователь</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<UserNode> UpdateUserNode(Guid userGuid, UserNode userNode)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                var node = userManager.UpdateUserNode(state, userNode);
                log.Info("Пользователь {0} обновил данные пользователя {1}.",
                               GetUserLogInfo(userGuid),
                               FormatObjectLogDescription(userNode.Idnum,
                                                          userNode.Text));
                return Return(userGuid, node);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UserNode>(ex));
            }
        }

        /// <summary>
        /// Добавить нового пользователя
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="userNode">Добавляемый пользователь</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<UserNode> AddUserNode(Guid userGuid, UserNode userNode)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                var node = userManager.AddUserNode(state, userNode);
                log.Info("Пользовать {0} добавил учетную запись {1}.",
                               GetUserLogInfo(userGuid),
                               FormatObjectLogDescription(userNode.Idnum,
                                                          userNode.Text));
                return Return(userGuid, node);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UserNode>(ex));
            }
        }

        /// <summary>
        /// Удалить пользователей
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="userNodeIDs">Динтификаторы удаляемых пользователей</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer RemoveUserNode(Guid userGuid, int[] userNodeIDs)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                double step = 100 / userNodeIDs.Length;
                foreach (int userNodeID in userNodeIDs)
                {
                    userManager.RemoveUserNode(state, userNodeID);
                    log.Info("Пользователь {1} удалил учетную запись {1}.",
                                   GetUserLogInfo(userGuid),
                                   FormatObjectLogDescription(userNodeID, null));
                    state.Progress += step;
                }
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        /// <summary>
        /// Создать первого пользователя
        /// </summary>
        /// <param name="userNode">Узел добавляемого пользователя</param>
        public void NewAdmin(UserNode userNode)
        {
            try
            {
                userManager.NewAdmin(userNode);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
        }
        #endregion

        #region Работа с группами пользователей
        /// <summary>
        /// Запросить группы пользователей
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<GroupNode[]> GetGroupNodes(Guid userGuid)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                return Return(userGuid, userManager.GetGroupNodes(state));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<GroupNode[]>(ex));
            }
        }

        /// <summary>
        /// Добавить новую группу пользователей
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="groupNode">Добавляемая группа</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<GroupNode> AddGroupNode(Guid userGuid, GroupNode groupNode)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                log.Info("Пользователь {0} добавил новую группу пользователей {1}.",
                               GetUserLogInfo(userGuid),
                               FormatObjectLogDescription(groupNode.Idnum,
                                                          groupNode.Text));
                return Return(userGuid, userManager.AddGroupNode(state, groupNode));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<GroupNode>(ex));
            }
        }

        /// <summary>
        /// Изменить группу пользователей
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="groupNode">Редактируемая группа</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<GroupNode> UpdateGroupNode(Guid userGuid, GroupNode groupNode)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                log.Info("Пользователь {0} отредактировал группу пользователей {1}.",
                               GetUserLogInfo(userGuid),
                               FormatObjectLogDescription(groupNode.Idnum,
                                                          groupNode.Text));
                return Return(userGuid, userManager.UpdateGroupNode(state, groupNode));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<GroupNode>(ex));
            }
        }

        /// <summary>
        /// Удалить группы пользователей
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="groupNodeIDs">Идентификаторы удаляемых групп</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer RemoveGroupNode(Guid userGuid, int[] groupNodeIDs)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                foreach (int groupNodeID in groupNodeIDs)
                {
                    log.Info("Пользователь {0} удалил группу пользователей {1}.",
                                   GetUserLogInfo(userGuid),
                                   FormatObjectLogDescription(groupNodeID, null));
                    userManager.RemoveGroupNode(state, groupNodeID);
                }
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }
        #endregion

        #region Работа с юнит нодами

        /// <summary>
        /// Запускает асинхронную операцию
        /// получения списка идов дочерних узлов для нода.
        /// </summary>
        /// <param name="userGuid">
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
        public ReturnContainer<UnitNode[]> GetUnitNodesFiltered(Guid userGuid, int parent, int[] filterTypes)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                var it = unitManager.GetUnitNodes(state, parent, filterTypes, Privileges.Read);
                return Return(userGuid, it);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UnitNode[]>(ex));
            }
        }

        public ReturnContainer<UnitNode[]> GetUnitNodes(Guid userGuid, int[] ids)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                var it = unitManager.GetUnitNodes(state, ids);
                return Return(userGuid, it);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UnitNode[]>(ex));
            }
        }

        public ReturnContainer<UnitNode[]> GetAllUnitNodes(Guid userGuid, int parent, int[] filterTypes)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                var it = unitManager.GetAllUnitNodes(state, parent, filterTypes, Privileges.Read);
                return Return(userGuid, it);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UnitNode[]>(ex));
            }
        }

        public ReturnContainer<UnitNode[]> GetAllUnitNodesMinMax(Guid userGuid, int parent, int[] filterTypes, int minLevel, int maxLevel)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                var it = unitManager.GetAllUnitNodes(state, parent, filterTypes, minLevel, maxLevel, Privileges.Read);
                return Return(userGuid, it);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UnitNode[]>(ex));
            }
        }

        /// <summary>
        /// Запускает асинхронную операцию
        /// получения списка дочерних узлов для нода, совпадающих по фильтру.
        /// </summary>
        /// <param name="userGuid">
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
        public ReturnContainer<UnitNode[]> GetUnitNodes2(Guid userGuid, int parent, ParameterFilter filter, RevisionInfo revision)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                var it = unitManager.GetUnitNodes(state, parent, filter, revision);
                return Return(userGuid, it);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UnitNode[]>(ex));
            }
        }

        public ReturnContainer<UnitNode> GetUnitNodeFiltered(Guid userGuid, int unitId, int[] filterTypes)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                return Return(userGuid, unitManager.GetUnitNode(state, unitId, filterTypes, Privileges.Read));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UnitNode>(ex));
            }
        }

        /// <summary>
        /// Запускает асинхронную операцию для получения
        /// унит нода.
        /// </summary>
        /// <param name="userGuid">
        ///     юзер
        /// </param>
        /// <param name="node">
        ///     Ид получаемого нода.
        /// </param>
        /// <returns>
        ///     Идентификатор асинхронной операции.
        /// </returns>
        public ReturnContainer<UnitNode> GetUnitNode(Guid userGuid, int node)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                var it = unitManager.ValidateUnitNode(state, node, Privileges.Read);
                return Return(userGuid, it);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UnitNode>(ex));
            }
        }

        public ReturnContainer<TreeWrapp<UnitNode>[]> GetUnitNodeTree(Guid userGuid, int[] unitNodeIDs, int[] filterTypes, Privileges privileges)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                return Return(userGuid, unitManager.GetUnitNodeTree(state, unitNodeIDs, filterTypes, privileges));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<TreeWrapp<UnitNode>[]>(ex));
            }
        }

        /// <summary>
        /// Запускает операцию по удалению массива нодов.
        /// </summary>
        /// <param name="userGuid">
        ///     юзер.
        /// </param>
        /// <param name="node">
        ///     Айдишки нодов.
        /// </param>
        /// <returns>
        ///     Айдишка асинхронной операции.
        /// </returns>
        public ReturnContainer RemoveUnitNodes(Guid userGuid, int[] nodes)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                unitManager.RemoveUnitNode(state, nodes);
                foreach (var it in nodes)
                {
                    log.Info("Пользователь {0} удалил узел id: {1} .",
                                   GetUserLogInfo(state.UserGUID),
                                   FormatObjectLogDescription(it, null));
                }
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        public ReturnContainer<ulong> BeginRemoveUnitNodes(Guid userGuid, int[] nodes)
        {
            securityManager.ValidateAccess(userGuid);

            return Return(userGuid, asyncOperation.BeginAsyncOperation(userGuid,
                    (OperationState state, object[] arrParams) =>
                    {
                        unitManager.RemoveUnitNode(state, nodes);
                        foreach (var it in nodes)
                        {
                            log.Info("Пользователь {0} удалил узел id: {1} .",
                                           GetUserLogInfo(state.UserGUID),
                                           FormatObjectLogDescription(it, null));
                        }
                        return null;
                    }));
        }

        /// <summary>
        /// Запускает операцию на удаление нода.
        /// </summary>
        /// <param name="userGuid">
        ///     юзер.
        /// </param>
        /// <param name="node">
        ///     Айдишка нода.
        /// </param>
        /// <returns>
        ///     Айдишка асинхронной операции.
        /// </returns>
        public ReturnContainer RemoveUnitNode(Guid userGuid, int node)
        {
            RemoveUnitNodes(userGuid, new int[] { node });
            return Return(userGuid);
        }

        /// <summary>
        /// Запускает операцию на добавление
        /// нода.
        /// </summary>
        /// <param name="userGuid">
        ///     Юзер.
        /// </param>
        /// <param name="unitType">
        ///     Новый нод.
        /// </param>
        /// <param name="parent">
        ///     Айдишка родительского нода.
        /// </param>
        public ReturnContainer<int> AddUnitNode(Guid userGuid, int unitType, int parent)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                var node = unitManager.NewInstance(state, unitType);
                var result = unitManager.AddUnitNode(state, new UnitNode[] { node }, parent);

                return Return(userGuid, result.First().Idnum);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<int>(ex));
            }
        }

        public ReturnContainer AddUnitNodes(Guid userGuid, UnitNode[] nodes, int parent)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                unitManager.AddUnitNode(state, nodes, parent);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        /// <summary>
        /// Запускает операцию на обновление нода.
        /// </summary>
        /// <param name="userGuid">
        ///     Юзер.
        /// </param>
        /// <param name="node">
        ///     Обновляемый нод
        /// </param>
        /// <returns>
        ///     Айдишка операции.
        /// </returns>
        public ReturnContainer<UnitNode> UpdateUnitNode(Guid userGuid, UnitNode node)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                var result = unitManager.UpdateUnitNode(state, node);
                log.Info("Пользователь {0} отредактировал узел {1}.",
                               GetUserLogInfo(userGuid),
                               GetLogInfo(result));
                return Return(userGuid, result);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UnitNode>(ex));
            }
        }

        public ReturnContainer MoveUnitNode(Guid userGUID, int parentId, int unitNodeId, int neighborId, bool addAfter)
        {
            try
            {
                OperationState state = GetOperationState(userGUID);

                int index = -1;
                if (neighborId > 0)
                {
                    UnitNode unitNode = unitManager.ValidateUnitNode<UnitNode>(state, neighborId, Privileges.Read);
                    index = unitNode.Index;
                    if (addAfter)
                        ++index;
                }

                unitManager.MoveUnitNode(state, parentId, unitNodeId, index);
            }
            catch (Exception ex)
            {
                ExceptionMethod<Object>(ex);
            }
            return Return(userGUID);
        }

        /// <summary>
        /// Копировать элементы или ветки
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="unitIDs">Ид копируемых узлов</param>
        /// <param name="recursive">true, если копируется ветки и false - отдельные элементы</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer CopyUnitNode(Guid userGuid, int[] unitIDs, bool recursive)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                unitManager.CopyUnitNode(state, unitIDs, recursive);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        /// <summary>
        /// Экспорт узлов в файл
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="nodeIds">Экспортируемы узлы</param>
        /// <param name="beginValuesTime">Начальное время экспортируемых значений</param>
        /// <param name="endValuesTime">Конечное время экспортируемых значений</param>
        /// <param name="exportFormat">Формат файла</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<ulong> BeginExport(Guid userGuid, int[] nodeIds, DateTime beginValuesTime, DateTime endValuesTime, ExportFormat exportFormat)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);
                return Return(userGuid, asyncOperation.BeginAsyncOperation(userGuid, (OperationState state, Object[] args) =>
                {
                    exportImportManager.Export(state, nodeIds, beginValuesTime, endValuesTime, exportFormat);
                    return null;
                }));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ulong>(ex));
            }
        }

        /// <summary>
        /// Импортировать из файла узлы и передать их клиенту
        /// </summary>
        /// <param name="userdGUID">Идентификатор сессии пользователя</param>
        /// <param name="buffer">Содержимое файла</param>
        /// <param name="exportFormat">Формат файла</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<ImportDataContainer> BeginImport(Guid userGuid, byte[] buffer, ExportFormat exportFormat)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                exportImportManager.Import(state, buffer, exportFormat);
                return Return(userGuid, state.AsyncResult[0] as ImportDataContainer);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ImportDataContainer>(ex));
            }
        }

        /// <summary>
        /// Сохранить импортируемые узлы
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="nodeWrappes">Обертка импортируемых узлов</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<ulong> BeginApplyImport(Guid userGuid, int rootNodeID, ImportDataContainer importContainer)
        {
            securityManager.ValidateAccess(userGuid);
            return Return(userGuid, asyncOperation.BeginAsyncOperation(userGuid, (OperationState state, Object[] args) =>
            {
                UnitNode importRootNode;

                if (rootNodeID <= 0)
                    importRootNode = null;
                else
                    importRootNode = unitManager.ValidateUnitNode<UnitNode>(state, rootNodeID, Privileges.Write);

                exportImportManager.ApplyImport(state, userGuid, importRootNode, importContainer);

                return null;
            }));
        }

        /// <summary>
        /// Изменить код в использующих его формулах
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="oldCode">Старый код</param>
        /// <param name="newCode">Новый код</param>
        /// <param name="unitNodesIds">Ид узлов в которых необходимо изменить код</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer ChangeParameterCode(Guid userGuid, string oldCode, string newCode, ParameterNodeReference[] unitNodesIds)
        {
            OperationState state = GetOperationState(userGuid);

            List<Tuple<CalcParameterNode, RevisionInfo>> unitNodeList = new List<Tuple<CalcParameterNode, RevisionInfo>>();
            List<String> formulaList = new List<String>();
            String[] formulaArray;
            CalcParameterNode node;

            foreach (var unitNodeID in unitNodesIds)
            {
                //node = unitManager.GetUnitNode(state, unitNodeID.ParameterNode.Idnum) as CalcParameterNode;
                node = unitManager.ValidateUnitNode<CalcParameterNode>(state, unitNodeID.ParameterNode.Idnum, Privileges.Write);
                //if (node != null)
                //{
                unitNodeList.Add(System.Tuple.Create(node, unitNodeID.Revision));
                formulaList.Add(node.GetFormulaStorage().Get(unitNodeID.Revision));
                //}
            }
            formulaArray = formulaList.ToArray();
            calcServer.ChangeParameterCode(state, oldCode, newCode, formulaArray);

            state.StateString = "Сохранение";
            double step = (AsyncOperation.MaxProgressValue - state.Progress) / unitNodeList.Count;
            for (int i = 0; i < formulaArray.Length; i++)
            {
                unitNodeList[i].Item1.GetFormulaStorage().Set(unitNodeList[i].Item2, formulaArray[i]);

                unitManager.UpdateUnitNode(state, unitNodeList[i].Item1);

                state.Progress += step;
            }
            foreach (var it in unitNodesIds)
                log.Info("Пользователь {0} изменил код с {1} на {2} для узла {3}.",
                               GetUserLogInfo(userGuid),
                               oldCode,
                               newCode,
                               FormatObjectLogDescription(it, null));
            return Return(userGuid);
        }
        #endregion

        #region Работа с расписаниями

        public ReturnContainer<Schedule[]> GetParamsSchedules(Guid userGuid)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                return Return(userGuid, scheduleManager.GetParameterSchedules(state));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<Schedule[]>(ex));
            }
        }
        public ReturnContainer<Schedule> GetParamsSchedule(Guid userGuid, int id)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                return Return(userGuid, scheduleManager.GetUnloadParamsSchedule(state, id));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<Schedule>(ex));
            }
        }
        public ReturnContainer<Schedule> GetParamsScheduleByName(Guid userGuid, string name)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                return Return(userGuid, scheduleManager.GetUnloadParamsSchedule(state, name));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<Schedule>(ex));
            }
        }

        public ReturnContainer AddParamsSchedule(Guid userGuid, Schedule added)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                scheduleManager.AddUnloadParamsSchedule(state, added);
                log.Info("Пользователь {0} добавил новое расписание id: {1} .",
                               GetUserLogInfo(userGuid),
                               added.Id);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        public ReturnContainer UpdateParamsSchedule(Guid userGuid,
                                                 Schedule updated)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                scheduleManager.UpdateUnloadParamsSchedule(state, updated);
                log.Info("Пользователь {0} отредактировал расписание id: {1} .",
                               GetUserLogInfo(userGuid),
                               updated.Id);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        public ReturnContainer DeleteParamsSchedule(Guid userGuid,
                                                 int id)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                scheduleManager.RemoveUnloadParamsSchedule(state, id);
                log.Info("Пользователь {0} удалил расписание id: {1} .",
                               GetUserLogInfo(userGuid),
                               id);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        public ReturnContainer<UnitNode> GetUnitNodeByCode(Guid userGuid, string code)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                return Return(userGuid, (UnitNode)unitManager.GetParameter(state, code));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UnitNode>(ex));
            }
        }

        public ReturnContainer<UnitNode> GetParentUnitNode(Guid userGuid, int unitNodeID, int typeID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                UnitNode unitNode = unitManager.ValidateUnitNode<UnitNode>(state, unitNodeID, Privileges.Read);
                UnitNode node = unitManager.GetParentNode(state, unitNode, typeID);
                securityManager.ValidateAccess(userGuid, node, Privileges.Read);

                return Return(userGuid, node);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UnitNode>(ex));
            }
        }

        #endregion

        #region Работа с отчетами
        public ReturnContainer<ReportSourceInfo[]> GetReportSources(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                return Return(userGuid, reportUtility.GetSupportedSources());
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ReportSourceInfo[]>(ex));
            }
        }

        public ReturnContainer<ReportSourceSettings> GetReportSourceSettings(Guid userGuid, Guid reportSourceID)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);
                return Return(userGuid, reportUtility.GetReportSourceSettings(reportSourceID));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ReportSourceSettings>(ex));
            }
        }

        public ReturnContainer<FastReportWrap> GenerateReportData(Guid userGuid, ReportSourceSettings[] settings, ReportParameter[] reportParameters)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                return Return(userGuid, reportUtility.GenerateReportData(state, settings, reportParameters));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<FastReportWrap>(ex));
            }
        }

        public ReturnContainer<FastReportWrap> GenerateEmptyReportData(Guid userGuid, ReportSourceSettings[] settings)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                return Return(userGuid, reportUtility.GenerateEmptyReportData(state, settings));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<FastReportWrap>(ex));
            }
        }

        public ReturnContainer<byte[]> GenerateReport(Guid userGuid, int reportID, bool saveInSystem, ReportParameter[] reportParameters)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                ReportNode reportNode = unitManager.ValidateUnitNode<ReportNode>(state, reportID, Privileges.Read | Privileges.Execute);
                UserNode userNode = securityManager.GetUserInfo(userGuid);

                return Return(userGuid, reportUtility.GenerateReport(userNode, state, reportNode, saveInSystem, reportParameters));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<byte[]>(ex));
            }
        }

        public ReturnContainer<byte[]> GenerateExcelReport(Guid userGuid, int parameterId, DateTime dateFrom, DateTime dateTo, bool saveInSystem)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                //ExcelReportNode reportNode = unitManager.GetUnitNode(state, parameterId) as ExcelReportNode;
                ExcelReportNode reportNode = unitManager.ValidateUnitNode<ExcelReportNode>(state, parameterId, Privileges.Execute);
                UserNode userNode = securityManager.GetUserInfo(userGuid);

                return Return(userGuid, reportUtility.GenerateReport(userNode, state, reportNode, dateFrom, dateTo, saveInSystem));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<byte[]>(ex));
            }
        }

        public ReturnContainer<PreferedReportInfo[]> GetPreferedReports(Guid userGuid, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                var it = new List<PreferedReportInfo>(reportUtility.GetReportsFromSystem(dateFrom, dateTo));
                it.RemoveAll(i =>
                {
                    try
                    {
                        unitManager.ValidateUnitNode<ReportNode>(state, i.ReportId, Privileges.Read);
                        return false;
                    }
                    catch
                    {
                        return true;
                    }
                });
                return Return(userGuid, it.ToArray());
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<PreferedReportInfo[]>(ex));
            }
        }

        public ReturnContainer<byte[]> GetPreferedReportBody(Guid userGuid, PreferedReportInfo reportInfo)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                ReportNode reportNode = unitManager.ValidateUnitNode<ReportNode>(state, reportInfo.ReportId, Privileges.Read);
                var it = reportUtility.GetReportBody(reportInfo);
                return Return(userGuid, it);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<byte[]>(ex));
            }
        }

        public ReturnContainer DeletePreferedReport(Guid userGuid, PreferedReportInfo reportInfo)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                ReportNode reportNode = unitManager.ValidateUnitNode<ReportNode>(state, reportInfo.ReportId, Privileges.Execute);

                reportUtility.DeleteReport(reportInfo);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }
        #endregion

        #region Слежение за состоянием асинхронных операций

        /// <summary>
        /// Методы управления операциями,
        /// пока еще сырые, в процессе написания 
        /// еще подправятся.
        /// </summary>

        public ReturnContainer<OperationInfo> GetOperationState(Guid userGuid,
                                               ulong operation_id)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                return Return(userGuid, asyncOperation.GetAsyncOperationState(operation_id));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<OperationInfo>(ex));
            }
        }

        public ReturnContainer<UAsyncResult> GetOperationResult(Guid userGuid,
                                         ulong operation_id)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);
                var r = asyncOperation.GetAsyncOperationData(operation_id, true);
                if (r == null) 
                    return Return<UAsyncResult>(userGuid, null);

                var res = new UAsyncResult();
                var ms = new List<Message>();
                var pg = new List<Package>();
                var nb = new List<NodeBack>();

                if (r is Package)
                    pg.Add((Package)r);
                else
                    if (r is Package[])
                        pg.AddRange((Package[])r);
                    else if (r is byte[])
                    {
                        res.Bytes = r as byte[];
                    }
                    else if (r is NodeBack)
                    {
                        nb.Add((NodeBack)r);
                    }
                    else if (r is NodeBack[])
                    {
                        nb.AddRange((NodeBack[])r);
                    }
                res.Messages = ms.ToArray();
                res.Packages = pg.ToArray();
                res.CalcNodeBack = nb.ToArray();
                return Return(userGuid, res);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UAsyncResult>(ex));
            }
        }

        public ReturnContainer<UAsyncResult> GetOperationMessages(Guid userGuid, ulong operationID)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                var m = asyncOperation.GetAsyncOperationMessages(operationID, true);
                if (m == null) return Return<UAsyncResult>(userGuid, null);
                var res = new UAsyncResult();
                // hack to not change client
                var retM = new List<Message>();
                foreach (var item in m)
                {
                    if (item is MessageByException)
                        retM.Add(new Message(item.Time, item.Category, item.Text));
                    else
                        retM.Add(item);
                }
                res.Messages = retM.ToArray();//m;
                return Return(userGuid, res);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<UAsyncResult>(ex));
            }
        }

        public ReturnContainer WaitAsyncOperation(Guid userGuid, ulong operationID)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                asyncOperation.WaitEndOperation(operationID);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        public ReturnContainer EndAsyncOperation(Guid userGuid, ulong operationID)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                asyncOperation.EndAsyncOperation(operationID);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        public ReturnContainer AbortOperation(Guid userGuid, ulong operation_id)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                asyncOperation.InteruptAsyncOperation(operation_id);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        #endregion

        #region Отправка данных на блочные

        public ReturnContainer<ulong> SendDataToBlockServer(Guid userGuid, int nodeId)
        {
            securityManager.ValidateAccess(userGuid);

            return Return(userGuid, asyncOperation.BeginAsyncOperation(userGuid, (OperationState context, Object[] arg) =>
            {
                BlockNode block = null;
                var node = unitManager.ValidateUnitNode(context, nodeId, Privileges.Write);
                if (node == null) throw new ArgumentException("nodeId");
                
                if (node.Typ == (int)UnitTypeId.Block)
                {
                    block = (BlockNode)node;
                    blockProxy.SendDataToBlock(context, block.BlockUID, // temp, test, debug
                                           unitManager.SerializeDataSet(context, unitManager.GetBlockData(context, block.Idnum)));
                    log.Info("Выполнена отправка данных на блочный id: {0} .",
                                   block.BlockUID);
                }
                else
                    if (node.Typ == (int)UnitTypeId.Channel)
                    {
                        block = unitManager.GetParentNode(context, node, (int)UnitTypeId.Block) as BlockNode;
                        if (block == null)
                        {
                            log.Error("Блочный сервер не найден.");
                            return null;
                        }
                        blockProxy.SendDataToBlock(context, block.BlockUID, node.Idnum,
                            unitManager.SerializeDataSet(context, unitManager.GetBlockData(context, block.Idnum, node.Idnum)));
                        log.Info("Выполнена отправка данных на канал {0} блочного сервера: {1} .",
                            node.Text,
                            block.BlockUID);
                    }
                //var block = unitManager.ValidateUnitNode<BlockNode>(context, nodeId, Privileges.Write);
                return null;
            }, null));
        }

        public ReturnContainer DeleteLoadValues(Guid userGuid, int unitNodeID, DateTime timeFrom)
        {
            try
            {
                //securityManager.ValidateAdminAccess(userGuid);

                //var state = new OperationState(userGuid);
                var state = GetOperationState(userGuid);
                //UnitNode unitNode = unitManager.GetUnitNode(state, unitNodeID);
                UnitNode unitNode = unitManager.ValidateUnitNode(state, unitNodeID, Privileges.Write);

                if (unitNode.Typ != (int)UnitTypeId.Block && unitNode.Typ != (int)UnitTypeId.Channel)
                    throw new ArgumentException();

                ThreadPool.QueueUserWorkItem(s => blockProxy.DeleteLoadValues(state, unitNode, timeFrom));
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        #endregion

        #region Значения параметров
        //public ReturnContainer<ulong> BeginGetArgumentedValues(Guid userGuid, int[] ids, DateTime beginTime, DateTime endTime)
        //{
        //    return BeginGetValues4(userGuid, ids, beginTime, endTime, Interval.Zero, CalcAggregation.Nothing, useBlock);//, true);
        //}
        /// <summary>
        /// Запросить значения парметров
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="ids">Идентификаторы парамтров</param>
        /// <param name="beginTime">Начальное время запрашиваемого периода</param>
        /// <param name="endTime">Конечное время запрашиваемого периода</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<ulong> BeginGetValues(Guid userGuid, int[] ids, DateTime beginTime, DateTime endTime, bool useBlockValues)
        {
            return BeginGetValues4(userGuid, ids, beginTime, endTime, Interval.Zero, CalcAggregation.Nothing, useBlockValues);
        }
        /// <summary>
        /// Запросить значения параметра
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="parameterID">Идентификатор параметра</param>
        /// <param name="beginTime">Начальное время запрашиваемого периода</param>
        /// <param name="endTime">Конечное время запрашиваемого периода</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<ulong> BeginGetValues2(Guid userGuid, int parameterID, DateTime beginTime, DateTime endTime, bool useBlockValues)
        {
            return BeginGetValues3(userGuid, parameterID, beginTime, endTime, Interval.Zero, CalcAggregation.Nothing, useBlockValues);
        }
        /// <summary>
        /// Запросить значения параметра
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="parameterID">Идентификатор параметра</param>
        /// <param name="beginTime">Начальное время запрашиваемого периода</param>
        /// <param name="endTime">Конечное время запрашиваемого периода</param>
        /// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        /// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        /// <returns></returns>
        public ReturnContainer<ulong> BeginGetValues3(Guid userGuid, int parameterID, DateTime beginTime, DateTime endTime,
            Interval interval, CalcAggregation aggregation, bool useBlockValues)
        {
            return BeginGetValues4(userGuid, new int[] { parameterID }, beginTime, endTime, interval, aggregation, useBlockValues);
        }
        /// <summary>
        /// Запросить значения параметра
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="parametersID">Идентификаторы параметров</param>
        /// <param name="beginTime">Начальное время запрашиваемого периода</param>
        /// <param name="endTime">Конечное время запрашиваемого периода</param>
        /// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        /// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        /// <param name="useBlockValues">Обращаться ли к серверу сбора данных для запроса значений или же использовать локальные значения</param>
        /// <returns></returns>
        //public ulong BeginGetValues(Guid userGuid, int[] parametersID, DateTime beginTime, DateTime endTime,
        //    Interval interval, CalcAggregation aggregation, bool useBlockValues)
        //{
        //    return BeginGetValues(userGuid, parametersID, beginTime, endTime, interval, aggregation, useBlockValues, false);
        //}

        public ReturnContainer<ulong> BeginGetValues4(Guid userGuid, int[] parametersID, DateTime beginTime, DateTime endTime, Interval interval, CalcAggregation aggregation, bool useBlockValues)//, bool multiArgs)
        {
            securityManager.ValidateAccess(userGuid);
            return Return(userGuid, asyncOperation.BeginAsyncOperation(userGuid,
                (OperationState state, object[] arrParams) =>
                {
                    //List<Tuple<ParameterNode, ArgumentsValues>> lstNodes = new List<Tuple<ParameterNode, ArgumentsValues>>();
                    List<ParameterValuesRequest> requests = new List<ParameterValuesRequest>();
                    ParameterNode node;

                    foreach (var item in parametersID)
                    {
                        node = unitManager.CheckUnitNode<ParameterNode>(state, item, Privileges.Read);

                        if (node != null)
                            requests.Add(new ParameterValuesRequest()
                        {
                            Parameters = new Tuple<ParameterNode, ArgumentsValues>[] { Tuple.Create(node, (ArgumentsValues)null) },
                            StartTime = beginTime,
                            EndTime = endTime,
                            AggregationInterval = interval,
                            Aggregation = aggregation,
                        });
                    }

                    valueReceiver.AsyncGetValues(
                        state,
                        AsyncOperation.MaxProgressValue,
                        //new ParameterValuesRequest[]
                        //{
                        //    new ParameterValuesRequest()
                        //    {
                        //        Parameters = lstNodes.ToArray(),
                        //        StartTime = beginTime, 
                        //        EndTime = endTime, 
                        //        AggregationInterval = interval, 
                        //        Aggregation = aggregation, 
                        //    }
                        //},
                        requests.ToArray(),
                        true,
                        useBlockValues);//, multiArgs);
                    return null;
                }));
        }

        public ReturnContainer<ulong> BeginGetValues5(Guid userGuid,
                                    int parameterID,
                                    ArgumentsValues arguments,
                                    DateTime beginTime,
                                    DateTime endTime,
                                    Interval interval,
                                    CalcAggregation aggregation)
        {
            securityManager.ValidateAccess(userGuid);

            return Return(userGuid, asyncOperation.BeginAsyncOperation(userGuid, (OperationState state, object[] arrParams) =>
            {
                ParameterNode node = unitManager.ValidateUnitNode<ParameterNode>(state, parameterID, Privileges.Read);

                valueReceiver.AsyncGetValues(
                    state,
                    AsyncOperation.MaxProgressValue,
                    new ParameterValuesRequest[]
                    {
                        new ParameterValuesRequest()
                        {
                            Parameters = new Tuple<ParameterNode,ArgumentsValues>[]
                            {
                                Tuple.Create(node, arguments)
                            },
                            StartTime = beginTime, 
                            EndTime = endTime,
                            AggregationInterval = interval, 
                            Aggregation = aggregation,
                        }
                    },
                    true);
                return null;
            }));
        }

        public ReturnContainer<ulong> BeginGetSortedArgs(Guid userGuid, int unitNodeID, DateTime time)
        {
            securityManager.ValidateAccess(userGuid);

            return Return(userGuid, asyncOperation.BeginAsyncOperation(userGuid, (OperationState state, object[] arrParams) =>
                {
                    OptimizationGateNode optimizationNode =
                        unitManager.ValidateParentNode<OptimizationGateNode>(state, unitNodeID, Privileges.Execute);

                    return valueReceiver.GetSortedArgs(state, optimizationNode, time);
                }));
        }

        public ReturnContainer<ulong> BeginGetOptimizationArguments(Guid userGuid, int optimizationNodeID, DateTime time)
        {
            securityManager.ValidateAccess(userGuid);

            return Return(userGuid, asyncOperation.BeginAsyncOperation(userGuid, (OperationState state, object[] arrParams) =>
            {
                OptimizationGateNode optimizationNode =
                    unitManager.ValidateUnitNode<OptimizationGateNode>(state, optimizationNodeID, Privileges.Execute);

                return valueReceiver.GetOptimizationArguments(state, optimizationNode, time);
            }));
        }

        public ReturnContainer<ulong> BeginGetOptimizationArgsForReport(Guid userGuid, int optimizationNodeID, DateTime time)
        {
            securityManager.ValidateAccess(userGuid);

            return Return(userGuid, asyncOperation.BeginAsyncOperation(userGuid, (OperationState state, object[] arrParams) =>
             {
                 OptimizationGateNode optimizationNode =
                     unitManager.ValidateUnitNode<OptimizationGateNode>(state, optimizationNodeID, Privileges.Execute);

                 ArgumentsValues[] args = valueReceiver.GetSortedArgs(state, optimizationNode, time);
                 if (args == null)
                 {
                     args = valueReceiver.GetOptimizationArguments(state, optimizationNode, time);
                     List<ArgumentsValues> argsList = new List<ArgumentsValues>(args);
                     argsList.RemoveAll(a => a == null);
                     argsList.Sort();
                     args = argsList.ToArray();
                 }

                 const int maxArgsToReport = 20;
                 if (args != null && args.Length > maxArgsToReport)
                 {
                     ArgumentsValues[] source = args;
                     args = new ArgumentsValues[maxArgsToReport];
                     for (int i = 0; i < args.Length; i++)
                         args[i] = source[i];
                 }
                 return args;
             }));
        }

        /// <summary>
        /// Сохранить значения парамтров
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="packages">Сохраняемые параметры</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer SaveValues(Guid userGuid, Package[] packages)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                valueReceiver.AsyncSaveValues(state, userGuid, AsyncOperation.MaxProgressValue, packages);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        public ReturnContainer DeleteValuesOptimization(Guid userGuid, int optimizationNodeID, ArgumentsValues[] valueArguments, DateTime time)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                OptimizationGateNode optimizationNode = unitManager.ValidateUnitNode<OptimizationGateNode>(state, optimizationNodeID, Privileges.Read);

                UnitNode[] parameters = unitManager.GetAllUnitNodes(state, optimizationNodeID,
                    new int[] { (int)UnitTypeId.ManualParameter, (int)UnitTypeId.Parameter }, Privileges.Write | Privileges.Execute);

                List<Tuple<UnitNode, ArgumentsValues, DateTime>> list = new List<Tuple<UnitNode, ArgumentsValues, DateTime>>();

                foreach (var unitNode in parameters)
                {
                    foreach (var arguments in valueArguments)
                    {
                        list.Add(System.Tuple.Create(unitNode, arguments, time));
                    }
                }

                valueReceiver.DeleteValues(state, AsyncOperation.MaxProgressValue, list.ToArray());//parameters, valueArguments, new DateTime[] { time });
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        /// <summary>
        /// Удалить значения парамтеров
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="parameterID">Идентификатор параметра</param>
        /// <param name="deletingDateTimes">Времена удаляемых значений</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer DeleteValues(Guid userGuid, int parameterID, DateTime[] deletingDateTimes)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                UnitNode node;

                node = unitManager.ValidateUnitNode<ParameterNode>(state, parameterID, Privileges.Write | Privileges.Execute);

                List<Tuple<UnitNode, ArgumentsValues, DateTime>> list = new List<Tuple<UnitNode, ArgumentsValues, DateTime>>();

                foreach (var time in deletingDateTimes)
                {
                    list.Add(System.Tuple.Create(node, (ArgumentsValues)null, time));
                }

                valueReceiver.DeleteValues(state, AsyncOperation.MaxProgressValue, list);////new UnitNode[] { node }, null, deletingDateTimes);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }
        #endregion

        #region Расчет
        /// <summary>
        /// Запросить зависимости параметра
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="parameterID">Идентификатор параметра, чьи зависимочти требуется найти</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<ParameterNodeDependence[]> GetDependence(Guid userGuid, RevisionInfo revision, int parameterID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                securityManager.ValidateAccess(userGuid);
                CalcParameterNode parameterNode = unitManager.ValidateUnitNode<CalcParameterNode>(state, parameterID, Privileges.Read);

                return Return(userGuid, calcServer.GetDependence(state, revision, parameterNode));
            }
            catch (Exception ex)
            {
                log.ErrorException("", ex);
                return Return(userGuid, ExceptionMethod<ParameterNodeDependence[]>(ex));
            }
        }

        /// <summary>
        /// Используется ли в формулах данный код
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="code">Код</param>
        /// <returns></returns>
        public ReturnContainer<bool> HasReference(Guid userGuid, String code)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                return Return(userGuid, calcServer.HasReference(state, code));
            }
            catch (Exception ex)
            {
                log.ErrorException("", ex);
                return Return(userGuid, ExceptionMethod<bool>(ex));
            }
        }

        /// <summary>
        /// Запросить ссылки на узел
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="code">Код параметра</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<ParameterNodeReference[]> GetReference(Guid userGuid, String code)//int parameterID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                return Return(userGuid, calcServer.GetReference(state, code));
            }
            catch (Exception ex)
            {
                log.ErrorException("", ex);
                return Return(userGuid, ExceptionMethod<ParameterNodeReference[]>(ex));
            }
        }

        /// <summary>
        /// Начать расчет параметров
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="parametersIDs">Идентификаторы расчитываемых параметров</param>
        /// <param name="beginTime">Начальное время</param>
        /// <param name="startTime">Конечное время</param>
        /// <param name="recalcAll">Пересичтать все</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<ulong> BeginCalc(Guid userGuid, int[] parametersIDs, DateTime beginTime, DateTime startTime, bool recalcAll)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                return Return(userGuid, asyncOperation.BeginAsyncOperation(userGuid, (OperationState state, Object[] args) =>
                    {
                        var nodes = parametersIDs.Select(i => unitManager.ValidateUnitNode<UnitNode>(state, i, Privileges.Read));


                        calcServer.Calc(state, nodes, beginTime, startTime, false, recalcAll);
                        return null;
                    }));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ulong>(ex));
            }
        }

        /// <summary>
        /// Запросить список функций, используемых в расчете
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<FunctionInfo[]> GetCalcFunction(Guid userGuid, RevisionInfo revision)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                return Return(userGuid, calcServer.GetCalcFunctions(state, revision));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<FunctionInfo[]>(ex));
            }
        }

        /// <summary>
        /// Проверить формулу на ошибки
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="formulaText">Текст формулы</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<Message[]> CheckFormula(Guid userGuid, RevisionInfo revision, string formulaText, KeyValuePair<int, CalcArgumentInfo[]>[] arguments)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                calcServer.CheckFormula(state, revision, formulaText, arguments);
                return Return(userGuid, state.messages.ToArray());

            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<Message[]>(ex));
            }
        }

        /// <summary>
        /// Запросить список констант
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<ConstsInfo[]> GetConsts(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);
                return Return(userGuid, calcServer.GetCalcConsts().ToArray());
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ConstsInfo[]>(ex));
            }
        }

        /// <summary>
        /// Сохранить изменения констант
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="constsInfo">Список измененных констант</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer SaveConsts(Guid userGuid, IEnumerable<ConstsInfo> constsInfo)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                calcSupplier.SaveCalcConsts(state, constsInfo);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        /// <summary>
        /// Удалить константы, используемые в расчете
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="constsInfo">Коллекция удаляемых констант</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer RemoveConsts(Guid userGuid, IEnumerable<ConstsInfo> constsInfo)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                calcSupplier.RemoveCalcConsts(state, constsInfo);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        /// <summary>
        /// Запросить список пользовательских функций
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<CustomFunctionInfo[]> GetCustomFunctions(Guid userGuid)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                return Return(userGuid, calcSupplier.GetCustomFunction().ToArray());
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<CustomFunctionInfo[]>(ex));
            }
        }

        /// <summary>
        /// Сохранить пользовательские функции
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="functionInfo">Сохраняемые функции</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer SaveCustomFunctions(Guid userGuid, IEnumerable<CustomFunctionInfo> functionInfo)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                calcSupplier.SaveCustomFunctions(state, functionInfo);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        /// <summary>
        /// Удалить пользовательские функции
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="functionInfo">Удаляемые функции</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer RemoveCustomFunctions(Guid userGuid, IEnumerable<CustomFunctionInfo> functionInfo)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                calcSupplier.RemoveCustomFunctions(state, functionInfo);
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        #region Циклический расчет
        /// <summary>
        /// Проверить запущен ли циклический расчет
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <returns></returns>
        public ReturnContainer<bool> IsRoundRobinStarted(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                return Return(userGuid, calcServer.IsStarted);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<bool>(ex));
            }
        }

        /// <summary>
        /// Запустить циклический расчет
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        public ReturnContainer StartRoundRobinCalc(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                calcServer.StartRoundRobin();
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        /// <summary>
        /// Остановить циклический расчет
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        public ReturnContainer StopRoundRobinCalc(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                calcServer.StopRoundRobin();
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        /// <summary>
        /// Получить количество сообщений последнего циклического расчета
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <returns>Количество сообщений</returns>
        public ReturnContainer<int> GetLastRoundRobinMessagesCount(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                if (calcServer.LastRoundRobinMessages != null)
                    return Return(userGuid, calcServer.LastRoundRobinMessages.Count);
                return Return(userGuid, 0);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<int>(ex));
            }
        }

        /// <summary>
        /// Получить сообщения последнего циклического расчета
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="start">Начальное сообщение</param>
        /// <param name="count">Количество сообщений</param>
        /// <returns>Идентификатор асинхронной операции</returns>
        public ReturnContainer<Message[]> GetLastRoundRobinMessages(Guid userGuid, int start, int count)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                if (calcServer.LastRoundRobinMessages != null)
                {
                    count = Math.Min(count, calcServer.LastRoundRobinMessages.Count);
                    Message[] messages = new COTES.ISTOK.Message[count];
                    calcServer.LastRoundRobinMessages.CopyTo(start, messages, 0, count);
                    state.AddMessage(messages);
                }
                return Return(userGuid, state.messages.ToArray());
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<Message[]>(ex));
            }
        }

        /// <summary>
        /// Получить настройки циклического расчета
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <returns></returns>
        public ReturnContainer<bool> GetRoundRobinAutoStart(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);
                return Return(userGuid, GlobalSettings.Instance.AllowRoundRobinAutostart);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<bool>(ex));
            }
        }

        /// <summary>
        /// Установить настройку автозапуска
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="autoStart">true - автозапуск разрешен, false - запрещен</param>
        public ReturnContainer SetRoundRobinAutoStart(Guid userGuid, bool autoStart)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);
                GlobalSettings.Instance.AllowRoundRobinAutostart = autoStart;
                GlobalSettings.Instance.Save();
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGuid);
        }

        /// <summary>
        /// Получить информацию о циклическом расчете
        /// </summary>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <returns></returns>
        public ReturnContainer<RoundRobinInfo> GetRoundRobinInfo(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);
                return Return(userGuid, calcServer.GetRoundRobinInfo());
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<RoundRobinInfo>(ex));
            }
        }
        #endregion
        #endregion

        #region Ревизии

        public ReturnContainer<RevisionInfo[]> GetRevisions(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);
                return Return(userGuid, revisionManager.GetRevisions().ToArray());
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<RevisionInfo[]>(ex));
            }
        }

        //public ReturnContainer RemoveRevisions(Guid userGuid, int[] revisionIDs)
        //{
        //    try
        //    {
        //        securityManager.ValidateAdminAccess(userGuid);

        //        var revisionToDelete = from r in revisionManager.GetRevisions() where revisionIDs.Contains(r.ID) select r;

        //        foreach (var revision in revisionToDelete)
        //        {
        //            revisionManager.Delete(revision);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionMethod<object>(ex);
        //    }
        //    return Return(userGuid);
        //}

        //public ReturnContainer UpdateRevisions(Guid userGuid, RevisionInfo[] revisions)
        //{
        //    try
        //    {
        //        securityManager.ValidateAdminAccess(userGuid);

        //        foreach (var revision in revisions)
        //        {
        //            if (revision.ID >= 0)
        //            {
        //                revisionManager.Update(revision);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionMethod<object>(ex);
        //    }
        //    return Return(userGuid);
        //}

        public ReturnContainer UpdateRevisions(Guid userGUID, int[] revisionIDs, RevisionInfo[] revisions)
        {
            try
            {
                securityManager.ValidateAdminAccess(userGUID);

                if (revisionIDs != null)
                {
                    var revisionToDelete = from r in revisionManager.GetRevisions() where revisionIDs.Contains(r.ID) select r;

                    foreach (var revision in revisionToDelete)
                    {
                        revisionManager.Delete(revision);
                    }
                }

                if (revisions != null)
                {
                    foreach (var revision in revisions)
                    {
                        if (revision.ID >= 0)
                        {
                            revisionManager.Update(revision);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionMethod<object>(ex);
            }
            return Return(userGUID);
        }

        #endregion

        #region Работа с расширениями
        public ReturnContainer<bool> ExternalIDSupported(Guid userGuid, int unitNodeID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                ExtensionUnitNode extensionUnitNode = unitManager.ValidateUnitNode<ExtensionUnitNode>(state, unitNodeID, Privileges.Read);

                return Return(userGuid, extensionManager.ExternalIDSupported(extensionUnitNode));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<bool>(ex));
            }
        }

        public ReturnContainer<bool> ExternalCodeSupported(Guid userGuid, int unitNodeID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                ExtensionUnitNode extensionUnitNode = unitManager.ValidateUnitNode<ExtensionUnitNode>(state, unitNodeID, Privileges.Read);

                return Return(userGuid, extensionManager.ExternalCodeSupported(extensionUnitNode));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<bool>(ex));
            }
        }

        public ReturnContainer<bool> ExternalIDCanAdd(Guid userGuid, int unitNodeID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                ExtensionUnitNode extensionUnitNode = unitManager.ValidateUnitNode<ExtensionUnitNode>(state, unitNodeID, Privileges.Read);

                return Return(userGuid, extensionManager.ExternalIDCanAdd(extensionUnitNode));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<bool>(ex));
            }
        }

        public ReturnContainer<EntityStruct[]> ExternalIDList(Guid userGuid, int unitNodeID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                ExtensionUnitNode extensionUnitNode = unitManager.ValidateUnitNode<ExtensionUnitNode>(state, unitNodeID, Privileges.Read);

                return Return(userGuid, extensionManager.GetExternalIDList(extensionUnitNode));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<EntityStruct[]>(ex));
            }
        }

        public ReturnContainer<ItemProperty[]> GetExternalProperties(Guid userGuid, int unitNodeID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                ExtensionUnitNode extensionUnitNode = unitManager.ValidateUnitNode<ExtensionUnitNode>(state, unitNodeID, Privileges.Read);

                return Return(userGuid, extensionManager.GetExternalProperties(extensionUnitNode));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ItemProperty[]>(ex));
            }
        }

        public ReturnContainer<ExtensionDataInfo[]> GetExtensionTableInfo(Guid userGuid)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                return Return(userGuid, extensionManager.GetTabInfo());
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ExtensionDataInfo[]>(ex));
            }
        }

        public ReturnContainer<ExtensionDataInfo[]> GetExtensionTableInfoById(Guid userGuid, int unitNodeID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                ExtensionUnitNode extensionUnitNode = unitManager.ValidateUnitNode<ExtensionUnitNode>(state, unitNodeID, Privileges.Read);

                return Return(userGuid, extensionManager.GetTabInfo(extensionUnitNode));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ExtensionDataInfo[]>(ex));
            }
        }

        public ReturnContainer<ExtensionDataInfo[]> GetExtensionTableInfoByCaption(Guid userGuid, String extensionCaption)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                return Return(userGuid, extensionManager.GetTabInfo(extensionCaption));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ExtensionDataInfo[]>(ex));
            }
        }

        public ReturnContainer<ExtensionData> GetExtensionExtendedTable(Guid userGuid, int unitNodeID, String tabKeyword, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                ExtensionUnitNode extensionUnitNode = unitManager.ValidateUnitNode<ExtensionUnitNode>(state, unitNodeID, Privileges.Read);

                ExtensionDataInfo extensionDataInfo = extensionManager.GetTabInfo(extensionUnitNode, tabKeyword);
                if (extensionDataInfo == null)
                    return null;
                return Return(userGuid, extensionManager.GetTab(extensionUnitNode, extensionDataInfo, dateFrom, dateTo));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ExtensionData>(ex));
            }
        }

        public ReturnContainer<ExtensionData> GetExtensionExtendedTable2(Guid userGuid, String extensionCaption, String tabKeyword, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);
                ExtensionDataInfo extensionInfo = extensionManager.GetTabInfo(extensionCaption, tabKeyword);
                return Return(userGuid, extensionManager.GetTab(extensionInfo, dateFrom, dateTo));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ExtensionData>(ex));
            }
        }
        #endregion

        #region ISourceExtension Members

        COTES.ISTOK.ParameterReceiverExtension.Parameter[] ITransmitterExtension.GetExtensionParameters(DateTime start, DateTime finish)
        {
            Guid BoilerTypeGUID = new Guid("{60A7DA28-684D-435c-9911-625310B2E045}");

            List<COTES.ISTOK.ParameterReceiverExtension.Parameter> parameterList = new List<COTES.ISTOK.ParameterReceiverExtension.Parameter>();
            COTES.ISTOK.ParameterReceiverExtension.Parameter parameter;

            ParameterNode[] pars = extensionManager.GetSourceParameters();

            //Package[] packs = extension.GetExtensionParameters(start, finish);
            //Package[] packs = valueReceiver.GetValues(pars, start, finish);
            OperationState state = new OperationState();
            valueReceiver.AsyncGetValues(
                state,
                0f,
                new ParameterValuesRequest[]
                {
                    new ParameterValuesRequest()
                    {
                        Parameters = (from p in pars select Tuple.Create(p, (ArgumentsValues)null)).ToArray(),
                        StartTime = start, 
                        EndTime = finish, 
                    }
                },
                true,
                false);

            Package[] packs;

            if (state.AsyncResult != null)
                packs = (from x in state.AsyncResult where x is Package select x as Package).ToArray();
            else
                packs = new Package[0];

            UTypeNode[] unitTypes = unitTypeManager.GetUnitTypes(new OperationState(securityManager.InternalSession));

            UTypeNode boilerType = unitTypes.First(t => t.ExtensionGUID == BoilerTypeGUID);

            foreach (Package package in packs)
            {
                String code;
                int externalID = 0;
                //if (!codeDictionary.TryGetValue(package.Id, out code))
                //    codeDictionary[package.Id] = code = extension.GetParameterCode(package.Id);
                ParameterNode parameterNode = pars.First(p => p.Idnum == package.Id);
                LoadParameterNode loadParameter = parameterNode as LoadParameterNode;

                if (loadParameter != null)
                {
                    code = loadParameter.LoadCode;
                }
                else if (parameterNode != null)
                {
                    code = parameterNode.Code;
                }
                else code = null;

                if (parameterNode != null && boilerType != null)
                {
                    ExtensionUnitNode unitNode = null;

                    unitNode = unitManager.GetParentNode(new OperationState(), parameterNode, boilerType.Idnum) as ExtensionUnitNode;

                    if (unitNode != null)
                    {
                        externalID = unitNode.ExternalID;
                    }
                }

                foreach (ParamValueItem item in package.Values)
                {
                    if (!double.IsNaN(item.Value))
                    {
                        parameter = new COTES.ISTOK.ParameterReceiverExtension.Parameter();
                        parameter.Id = package.Id;
                        parameter.ExternalID = externalID;
                        parameter.Code = code;
                        parameter.Value = item.Value;
                        parameter.Time = item.Time;
                        parameter.Quality = item.Quality == Quality.Good ? 100 : 0;
                        parameterList.Add(parameter);
                    }
                }
            }

            if (parameterList.Count == 0) return null;
            return parameterList.ToArray();
        }

        string ITransmitterExtension.GetParameterCode(int parameterID)
        {
            //ParameterNode param = unitManager.GetUnitNode(new OperationState(), parameterID) as ParameterNode;
            ParameterNode param = unitManager.CheckUnitNode<ParameterNode>(new OperationState(), parameterID, Privileges.Read);
            if (param != null) return param.Code;
            return null;
        }

        #endregion

        public ReturnContainer<ParameterItem[]> BeginGetChannelParameters(Guid userGuid, int channelNodeId)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);
                //UnitNode unitNode = unitManager.GetUnitNode(state, channelNodeId);
                //ChannelNode channelNode = unitNode as ChannelNode;

                //if (unitNode == null)
                //    throw new Exception("Узел не найден");
                //if (channelNode == null)
                //    throw new Exception("Узел не является каналом");
                //securityManager.ValidateAccess(userGuid, unitNode, Privileges.Write);
                ChannelNode channelNode = unitManager.ValidateUnitNode<ChannelNode>(state, channelNodeId, Privileges.Write);

                state.AllowStartAsyncResult = true;
                blockProxy.GetParameters(state, channelNode);
                return Return(userGuid, state.GetAsyncResult<ParameterItem>().ToArray());
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<ParameterItem[]>(ex));
            }
        }
        public ReturnContainer<ParameterItem[]> EndGetChannelParameters(Guid userGuid, ulong operationId)
        {
            throw new NotImplementedException();
        }

        #region Интеграция логов
        public ReturnContainer<ulong> BeginGetLogs(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Форматирование данных перед логирорванием
        private string GetUserLogInfo(Guid userGuid)
        {
            try
            {
                var user = securityManager.GetUserInfo(userGuid);

                return FormatObjectLogDescription(user.Idnum, user.Text);
            }
            catch
            {
                return "[Пользователь не известен.]";
            }
        }

        private string GetLogInfo(UnitNode node)
        {
            try
            {
                return FormatObjectLogDescription(node.Idnum, node.FullName);
            }
            catch
            {
                return @"[Не известено.]";
            }
        }

        private string FormatObjectLogDescription(object id, object name)
        {
            const string error_message = @"Не известно.";
            const string format = @"[id: {0} ; name: {1} ;]";

            try
            {
                return String.Format(format,
                                     id == null ? error_message : id.ToString(),
                                     name == null ? error_message : id.ToString());
            }
            catch
            {
                return String.Format(@"[{0}]", error_message);
            }
        }
        #endregion

        public ReturnContainer<Interval> GetParameterInterval(Guid userGuid, int parameterID)
        {
            try
            {
                var state = GetOperationState(userGuid);

                ParameterNode parameterNode = unitManager.ValidateUnitNode<ParameterNode>(state, parameterID, Privileges.Read);

                UnitNode unitNode = parameterNode;
                ParameterGateNode gateNode;

                gateNode = unitManager.CheckParentNode<ParameterGateNode>(state, parameterNode.ParentId, Privileges.Read);

                //while (unitNode != null)
                //{
                //if ((gateNode = unitNode as ParameterGateNode) != null)
                if (gateNode != null)
                    return Return(userGuid, gateNode.Interval);

                //    unitNode = unitManager.GetUnitNode(state, unitNode.ParentId);
                //}
                return Return(userGuid, Interval.Zero);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<Interval>(ex));
            }
        }

        public ReturnContainer<bool> IsExtensionType(Guid userGuid, int nodeType)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                UTypeNode typeNode = unitTypeManager.GetUnitType(state, nodeType);

                return Return(userGuid, typeNode.ExtensionGUID != Guid.Empty);
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<bool>(ex));
            }
        }

        public ReturnContainer<Dictionary<int, int>> GetStatistic(Guid userGuid, int unitNodeID)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                UnitNode unitNode = unitManager.ValidateUnitNode<UnitNode>(state, unitNodeID, Privileges.Read);

                return Return(userGuid, unitManager.GetStatistic(state, unitNode));
            }
            catch (Exception ex)
            {
                return Return(userGuid, ExceptionMethod<Dictionary<int, int>>(ex));
            }
        }

        public OperationState LoadState { get; set; }

        public ReturnContainer<AuditEntry[]> GetAudit(Guid userGuid, AuditRequestContainer request)
        {
            try
            {
                securityManager.ValidateAccess(userGuid);

                return Return(userGuid, auditServer.ReadAuditEntries(request).ToArray());
            }
            catch (Exception exc)
            {
                return Return(userGuid, ExceptionMethod<AuditEntry[]>(exc));
            }
        }

        public ReturnContainer<IntervalDescription[]> GetStandardsIntervals(Guid userGuid)
        {
            try
            {
                OperationState state = GetOperationState(userGuid);

                return Return(userGuid, unitManager.GetStandardsIntervals(state));
            }
            catch (Exception exc)
            {
                return Return(userGuid, ExceptionMethod<IntervalDescription[]>(exc));
            }
        }

        public ReturnContainer RemoveStandardIntervals(Guid userGuid, IntervalDescription[] intervalsToRemove)
        {
            try
            {
                securityManager.ValidateAdminAccess(userGuid);

                unitManager.RemoveStandardIntervals(new OperationState(userGuid), intervalsToRemove);
                return Return(userGuid);
            }
            catch (Exception exc)
            {
                return Return(userGuid, ExceptionMethod<Object>(exc));
            }
        }

        public ReturnContainer SaveStandardIntervals(Guid userGuid, IntervalDescription[] modifiedIntervals)
        {
            try
            {
                securityManager.ValidateAdminAccess(userGuid);

                unitManager.SaveStandardIntervals(new OperationState(userGuid), modifiedIntervals);
                return Return(userGuid);
            }
            catch (Exception exc)
            {
                return Return(userGuid, ExceptionMethod<Object>(exc));
            }
        }

        protected T ExceptionMethod<T>(Exception ex)
        {
            if (ex != null)
            {
                if (ex is UserNotConnectedException)
                    throw new FaultException<UserNotConnectedException>(ex as UserNotConnectedException, ex.Message);
                else if (ex is NoOneUserException)
                    throw new FaultException<NoOneUserException>(ex as NoOneUserException, ex.Message);
                else if (ex is LockException)
                    throw new FaultException<LockExceptionFault>(new LockExceptionFault(ex as LockException), ex.Message);
                else
                {
                    log.ErrorException("Передача в клиент неопределённого исключения", ex);
                    throw new FaultException(ex.Message);
                }
            }
            return default(T);
        }
    }

    /// <summary>
    /// Класс хранения информации о потоку обновления
    /// </summary>
    class ParamUpdateInfo
    {
        private BlockNode bnode;
        //private ParamReceiveItem[] arrParameters;
        private ParametersTransaction ta;
        /// <summary>
        /// глобальный номер транзакции
        /// </summary>
        public int TaID { get; private set; }
        private int updateInterval = 5000;

        public ParamUpdateInfo(//ParamReceiveItem[] parameters,
            int taID,
            ParametersTransaction transaction,
            BlockNode node)
        {
            TaID = taID;
            bnode = node;
            //arrParameters = parameters;
            ta = transaction;
        }

        /// <summary>
        /// Узел блочного сервера
        /// </summary>
        public BlockNode Node
        {
            get { return bnode; }
        }
        /// <summary>
        /// Массив параметров
        /// </summary>
        public ParamValueItemWithID[] Parameters
        {
            get
            {
                if (ta != null)
                    return ta.Parameters;
                else
                    return null;
            }
        }
        /// <summary>
        /// Транзакция
        /// </summary>
        public ParametersTransaction Transaction
        {
            get { return ta; }
        }
        /// <summary>
        /// Интервал обновления параметров (мс)
        /// </summary>
        public int UpdateInterval
        {
            get { return updateInterval; }
            set { updateInterval = value; }
        }
    }
}

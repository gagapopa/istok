using System;
using System.Collections.Generic;
using System.Linq;
using COTES.ISTOK.ASC;
using NLog;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Проверка прав пользователей и регистрация текущих подключенных пользователей
    /// </summary>
    class LockManager : ILockManager
    {
        Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Информация о блокировки значений
        /// </summary>
        class LockInfo
        {
            /// <summary>
            /// ИД сессии пользователя
            /// </summary>
            public Guid UserGUID { get; protected set; }

            /// <summary>
            /// Начальное время блокировки
            /// </summary>
            public DateTime TimeStart { get; protected set; }

            /// <summary>
            /// Конечное время блокировки
            /// </summary>
            public DateTime TimeEnd { get; protected set; }

            public LockInfo(Guid guid, DateTime startTime, DateTime endTime)
            {
                this.UserGUID = guid;
                this.TimeStart = startTime;
                this.TimeEnd = endTime;
            }

            public LockInfo(Guid guid, DateTime lockTime)
                : this(guid, lockTime, lockTime)
            { }

            public override bool Equals(object obj)
            {
                LockInfo lockInfo = obj as LockInfo;
                if (lockInfo!=null)
                {
                    return this.UserGUID.Equals(lockInfo.UserGUID)
                        && this.TimeStart.Equals(lockInfo.TimeStart)
                        && this.TimeEnd.Equals(lockInfo.TimeEnd);
                }
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return this.UserGUID.GetHashCode() +
                    this.TimeStart.GetHashCode() +
                    this.TimeEnd.GetHashCode();
            }
        }

        public ISecurityManager SecurityManager { get; set; }

        public IUnitManager UnitManager { get; set; }

        /// <summary>
        /// Блокировки на редактирование узлов
        /// </summary>
        Dictionary<int, Guid> lockNodes = new Dictionary<int, Guid>();

        /// <summary>
        /// Блокировки на редактирование значений
        /// </summary>
        Dictionary<int, List<LockInfo>> lockValues = new Dictionary<int, List<LockInfo>>();

        Object lockSyncObject = new Object();

        public LockManager()
        {
        }

        #region Блокировки {Lock,Relese}{Node,Values}()
        /// <summary>
        /// Взять узел на редактирование.
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя</param>
        /// <param name="unitNode">Редактируемы узел</param>
        /// <returns>
        /// Если блокировка взята только что, возвращает true.
        /// Если блокировка взята ранее - false.
        /// </returns>
        /// <exception cref="COTES.ISTOK.ASC.LockException">Узел редактируется другим пользователем.</exception>
        public bool LockNode(OperationState state, UnitNode unitNode)
        {
            Guid anotherGUID;

            lock (lockSyncObject)
            {
                CheckLockValues(state, unitNode);

                if (lockNodes.TryGetValue(unitNode.Idnum, out anotherGUID))
                {
                    if (anotherGUID == state.UserGUID)
                        return false;
                    if (SecurityManager.CheckAliveUser(anotherGUID))
                        throw new LockException(
                            unitNode,
                            Privileges.Write,
                            SecurityManager.GetUserInfo(anotherGUID).Text);
                }

                lockNodes[unitNode.Idnum] = state.UserGUID;
                return true;
            }
        }

        /// <summary>
        /// Освободить редактируемый узел
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя</param>
        /// <param name="unitNode">Редактируемы узел</param>
        public void ReleaseNode(OperationState state, UnitNode unitNode)
        {
            Guid anotherGUID;

            lock (lockSyncObject)
            {
                if (lockNodes.TryGetValue(unitNode.Idnum, out anotherGUID)
                    && anotherGUID == state.UserGUID)
                    lockNodes.Remove(unitNode.Idnum);
            }
        }

        /// <summary>
        /// Проверить можно ли взять узел на редактирование.
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя</param>
        /// <param name="unitNode">Редактируемы узел</param>
        /// <exception cref="COTES.ISTOK.ASC.LockException">Узел редактируется другим пользователем.</exception>
        private void CheckLockNode(OperationState state, UnitNode unitNode)
        {
            Guid anotherGUID;

            lock (lockSyncObject)
            {
                if (lockNodes.TryGetValue(unitNode.Idnum, out anotherGUID) && state.UserGUID != anotherGUID)
                {
                    if (SecurityManager.CheckAliveUser(anotherGUID))
                        throw new LockException(
                            unitNode,
                            Privileges.Write,
                            SecurityManager.GetUserInfo(anotherGUID).Text);
                    else
                    {
                        lockNodes.Remove(unitNode.Idnum);
                    }
                }
            }
        }

        /// <summary>
        /// Взять узел на редактирования значений
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя</param>
        /// <param name="unitNode">Редактируемы узел</param>
        /// <param name="startTime">Начальное время редактируемых значений</param>
        /// <param name="endTime">Конечное время редактируемых значений</param>
        /// <exception cref="COTES.ISTOK.ASC.LockException">Узел редактируется другим пользователем.</exception>
        /// <exception cref="COTES.ISTOK.ASC.MultioLockException">Узел редактируется другим пользователем.</exception>
        public void LockValues(OperationState state, UnitNode unitNode, DateTime startTime, DateTime endTime)
        {
            List<LockInfo> lockList;

            lock (lockSyncObject)
            {
                if (unitNode.Typ == (int)UnitTypeId.ManualParameter || unitNode.Typ == (int)UnitTypeId.TEP)
                {
                    int templateType = unitNode.Typ == (int)UnitTypeId.ManualParameter ? (int)UnitTypeId.ManualGate : (int)UnitTypeId.TEPTemplate;
                    UnitNode templateNode = UnitManager.GetParentNode(state, unitNode, templateType);
                    if (templateNode != null)
                    {
                        // Проверяем не взят ли шаблон на изменение
                        CheckLockNode(state, templateNode);
                        // Проверяем не взят ли шаблон на изменение значений
                        CheckLockValues(state, templateNode, startTime, endTime);
                    }
                }
                else if (unitNode.Typ == (int)UnitTypeId.TEPTemplate || unitNode.Typ == (int)UnitTypeId.ManualGate)
                {
                    // Проверяем не взят ли шаблон на изменение
                    CheckLockNode(state, unitNode);
                }
                else throw new ArgumentException(String.Format("Узел '{0}' нельзя взять на редактирование значений", unitNode.Text));

                // Проверяем взят ли требуемый узел на значения
                CheckLockValues(state, unitNode, startTime, endTime);

                // берем узел на значения
                if (!lockValues.TryGetValue(unitNode.Idnum, out lockList))
                    lockValues[unitNode.Idnum] = lockList = new List<LockInfo>();

                LockInfo lockInfo = new LockInfo(state.UserGUID, startTime, endTime);
                if (!lockList.Contains(lockInfo))
                    lockList.Add(lockInfo);

                // Проверка параметров шаблона блокировки на значения
                if (unitNode.Typ == (int)UnitTypeId.TEPTemplate || unitNode.Typ == (int)UnitTypeId.ManualGate)
                {
                    int[] filter;
                    if (unitNode.Typ == (int)UnitTypeId.ManualGate)
                        filter = new int[] { (int)UnitTypeId.ManualParameter };
                    else
                        filter = new int[] { (int)UnitTypeId.TEP };
                    UnitNode[] parameters = UnitManager.GetAllUnitNodes(state, unitNode.Idnum, filter, Privileges.Execute);
                    
                    Dictionary<int, LockCause> lockDictionary = new Dictionary<int, LockCause>();

                    foreach (UnitNode parameterNode in parameters)
                    {
                        try
                        {
                            CheckLockValues(state, parameterNode, startTime, endTime);
                        }
                        catch (LockException exc)
                        {
                            foreach (var item in exc.Causes)
                            {
                                lockDictionary[item.Node.Idnum] = item;
                            }
                        }
                    }
                    if (lockDictionary.Count > 0)
                        throw new LockException(lockDictionary.Values.Cast<LockCause>().ToArray());
                }
            }
        }

        /// <summary>
        /// Проверить, взят ли узел на редактирования значений другим пользователем.
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя</param>
        /// <param name="unitNode">Редактируемы узел</param>
        /// <exception cref="COTES.ISTOK.ASC.LockException">Узел редактируется другим пользователем.</exception>
        private void CheckLockValues(OperationState state, UnitNode unitNode)
        {
            if (unitNode.Typ == (int)UnitTypeId.TEPTemplate || unitNode.Typ == (int)UnitTypeId.ManualGate)
                CheckLockValues(state, unitNode, DateTime.MinValue, DateTime.MaxValue);
        }

        /// <summary>
        /// Проверить, можно ли взять узел на редактирование значений за указанный интервал
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя</param>
        /// <param name="unitNode">Редактируемы узел</param>
        /// <param name="startTime">Начальное время редактируемых значений</param>
        /// <param name="endTime">Конечное время редактируемых значений</param>
        /// <exception cref="COTES.ISTOK.ASC.LockException">Узел редактируется другим пользователем.</exception>
        private void CheckLockValues(OperationState state, UnitNode unitNode, DateTime startTime, DateTime endTime)
        {
            List<LockInfo> blockList;
            List<LockInfo> lockList;

            lock (lockSyncObject)
            {
                if (lockValues.TryGetValue(unitNode.Idnum, out lockList))
                {
                    if (startTime != DateTime.MinValue)
                        blockList = lockList.FindAll(l => l.UserGUID != state.UserGUID
                            && ((l.TimeStart <= startTime && startTime < l.TimeEnd)
                            || (l.TimeStart < endTime && endTime <= l.TimeEnd)
                            || (startTime <= l.TimeStart && l.TimeEnd <= endTime)));
                    else blockList = lockList.FindAll(l => l.UserGUID != state.UserGUID);

                    if (blockList.Count > 0)
                    {
                        List<String> userNames = new List<String>();

                        foreach (LockInfo lockInfo in blockList)
                        {
                            if (SecurityManager.CheckAliveUser(lockInfo.UserGUID))
                                userNames.Add(SecurityManager.GetUserInfo(lockInfo.UserGUID).Text);
                            else
                            {
                                lockList.Remove(lockInfo);
                            }
                        }
                        if (userNames.Count > 0)
                            throw new LockException(unitNode, Privileges.Write, userNames.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Освободить значения редактируемого узла за указанный интервал.
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя</param>
        /// <param name="unitNode">Редактируемы узел</param>
        /// <param name="startTime">Начальное время редактируемых значений</param>
        /// <param name="endTime">Конечное время редактируемых значений</param>
        public void ReleaseValues(OperationState state, UnitNode unitNode, DateTime startTime, DateTime endTime)
        {
            List<LockInfo> lockList;

            lock (lockSyncObject)
            {
                if (lockValues.TryGetValue(unitNode.Idnum, out lockList))
                {
                    List<LockInfo> blockList = lockList.FindAll(l => l.TimeStart == startTime && l.TimeEnd == endTime);

                    foreach (LockInfo lockInfo in blockList)
                    {
                        // если блокировка взята в текущей сессии
                        if (state.UserGUID == lockInfo.UserGUID)
                            lockList.Remove(lockInfo);
                    }

                    if (lockList.Count == 0)
                        lockValues.Remove(unitNode.Idnum);
                }
            }
        }

        /// <summary>
        /// Освободить указанный узел от редактирования и редактирования его значений.
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя</param>
        /// <param name="unitNode">Редактируемы узел</param>
        public void ReleaseAll(OperationState state, UnitNode unitNode)
        {
            Guid anotherGUID;
            List<LockInfo> lockList;

            lock (lockSyncObject)
            {
                // сбросить редактирование узла
                // если блокировка устарела или это блокировка того же пользователя или текущий пользователь админ
                if (lockNodes.TryGetValue(unitNode.Idnum, out anotherGUID)
                    && (!SecurityManager.CheckAliveUser(anotherGUID)
                        || SecurityManager.AreSameUser(state.UserGUID, anotherGUID)
                        || SecurityManager.CheckAdminAccess(state.UserGUID)))
                {
                    lockNodes.Remove(unitNode.Idnum);
                }

                // сбросить редактирование значений
                if (lockValues.TryGetValue(unitNode.Idnum, out lockList))
                {
                    LockInfo[] lockArray = lockList.ToArray();
                    foreach (LockInfo lockInfo in lockArray)
                    {
                        if (SecurityManager.AreSameUser(state.UserGUID, lockInfo.UserGUID)
                            || SecurityManager.CheckAdminAccess(state.UserGUID))
                        {
                            lockList.Remove(lockInfo);
                        }
                    }
                    if (lockList.Count == 0)
                        lockValues.Remove(unitNode.Idnum);
                }
            }
        } 
        #endregion
    }
}

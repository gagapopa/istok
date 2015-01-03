using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.DiagnosticsInfo;
using NLog;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Проверка прав пользователей и регистрация текущих подключенных пользователей
    /// </summary>
    class SecurityManager : ISecurityManager, ISummaryInfo
    {
        Logger log = LogManager.GetCurrentClassLogger();

            /// <summary>
        /// Информация о сессии пользователя
        /// </summary>
        class UserInfo
        {
            /// <summary>
            /// Информация о пользователе
            /// </summary>
            public UserNode User { get; protected set; }

            /// <summary>
            /// Время последней активности
            /// </summary>
            public DateTime LastActive { get; set; }

            public UserInfo(UserNode user)
            {
                this.User = user;
                LastActive = DateTime.Now;
            }
        }

        /// <summary>
        /// Информация о пользователе для внутренних операций
        /// </summary>
        class InternalUserNode : UserNode
        {
            // TODO: Здесь, воможно, нужны какие то ограничения или даже настройки

            public override bool IsAdmin
            {
                get
                {
                    return true;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override bool CheckGroupPrivilegies(int groupId, Privileges priv)
            {
                return true;
            }

            public override bool CheckPrivileges(int unitType, Privileges priv)
            {
                return true;
            }
        }

        public IUnitManager UnitManager { get; set; }

        public IUserManager UserManager { get; set; }

        /// <summary>
        /// Словарь сессий пользователей
        /// </summary>
        Dictionary<Guid, UserInfo> connectedUsers = new Dictionary<Guid, UserInfo>();

        /// <summary>
        /// ИД Сессии для внутренних операций
        /// </summary>
        public Guid InternalSession { get; protected set; }

        /// <summary>
        /// Пользователь для внутренних операций
        /// </summary>
        private UserInfo InternalUser { get; set; }

        public SecurityManager()
        {
            InternalSession = Guid.NewGuid();
            InternalUser = new UserInfo(new InternalUserNode());
            connectedUsers[InternalSession] = InternalUser;

            UserTimeout = DefaultUserTimeOut;
        }

        #region Диагностика
        const string caption = "Сессии пользователей";
        public DataSet GetSummaryInfo()
        {
            DataSet ds = new DataSet();
            DataTable table = new DataTable(caption);

            table.Columns.Add("Имя пользователя");
            table.Columns.Add("Администратор", typeof(bool));
            lock (connectedUsers)
            {
                foreach (var item in connectedUsers.Values)
                    table.Rows.Add(item.User.Text, item.User.IsAdmin);
            }
            ds.Tables.Add(table);
            
            return ds;
        }
        public string GetSummaryCaption()
        {
            return caption;
        }
        #endregion

        /// <summary>
        /// Зарегистрировать сессию пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>
        /// Идентификатор сессии пользователя,
        /// Guid.Empty - если сессия не зарегистрированна
        /// </returns>
        public Guid Register(string userName, string password)
        {
            UserNode user = UserManager.GetUser(userName);
            String hashPassword = GetHashPassword(password);
            if (user != null && user.Password.Equals(hashPassword))
            {
                lock (connectedUsers)
                {
                    Guid guid = Guid.NewGuid();
                    connectedUsers[guid] = new UserInfo(user);

                    log.Info("Пользователь авторизован: id{0} - {1}.",
                                   user.Idnum,
                                   userName);

                    OnSessionRegistered(guid);
                    return guid;
                }
            }

            log.Warn("Попытка авторизации: id{0} - {1}",
                           user == null ? 0 : user.Idnum,
                           userName);

            return Guid.Empty;
        }

        //// основной рабочий коннект к базе
        //public static MyDBdata dbwork =null;
        // расчитать хэш для пароля
        public string GetHashPassword(string pass)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] ret = md5.ComputeHash(Encoding.UTF8.GetBytes(pass));
            return Encoding.ASCII.GetString(ret);
            //return pass;
        }

        /// <summary>
        /// Удалить информацию о сессии пользователя
        /// </summary>
        /// <param name="sessionGuid">Идентификатор сессии пользователя</param>
        public void Unregister(Guid sessionGuid)
        {
            lock (connectedUsers)
            {
                if (connectedUsers.ContainsKey(sessionGuid))
                {
                    String fullName = connectedUsers[sessionGuid].User.Text;
                    if (String.IsNullOrEmpty(fullName))
                        fullName = connectedUsers[sessionGuid].User.Text;

                    log.Info("Закрыта авторизация пользователя: id{0} - {1}",
                                   connectedUsers[sessionGuid].User.Idnum,
                                   fullName);
                    connectedUsers.Remove(sessionGuid);
                    OnSessionUnregistered(sessionGuid);
                }
                else
                {
                    log.Error("Попытка снятия авторизации для не существующей сесси.");
                }
            }
        }

        /// <summary>
        /// Закрыть все сессии пользователя
        /// </summary>
        /// <param name="userNodeID">Идентификатор пользователя</param>
        public void UnregisterAll(int userNodeID)
        {
            lock (connectedUsers)
            {
                List<Guid> toRemove = new List<Guid>();
                foreach (Guid userGuid in connectedUsers.Keys)
                {
                    if (connectedUsers[userGuid].User.Idnum.Equals(userNodeID))
                        toRemove.Add(userGuid);
                }
                foreach (Guid userGuid in toRemove)
                {
                    connectedUsers.Remove(userGuid);
                    OnSessionUnregistered(userGuid);
                }

                var user = UserManager.GetUser(userNodeID);
                if (user != null)
                {
                    log.Warn("Все сессии пользователя были принудительно закрыты: id{0} - {1}",
                                   user.Idnum,
                                   user.Text);
                }
            }
        }

        /// <summary>
        /// Событие, возникаемое при подключении нового пользователя
        /// </summary>
        public event EventHandler<SessionIDEventArgs> SessionRegistered;

        /// <summary>
        /// Событие при отключении пользователя
        /// </summary>
        public event EventHandler<SessionIDEventArgs> SessionUnregistered;

        /// <summary>
        /// Вызвать событие SessionRegistered
        /// </summary>
        /// <param name="sessionGUID"></param>
        private void OnSessionRegistered(Guid sessionGUID)
        {
            if (SessionRegistered != null)
                SessionRegistered(this, new SessionIDEventArgs(sessionGUID));
        }

        /// <summary>
        /// Вызвать событие SessionUnregistered
        /// </summary>
        /// <param name="sessionGUID"></param>
        private void OnSessionUnregistered(Guid sessionGUID)
        {
            if (SessionUnregistered != null)
                SessionUnregistered(this, new SessionIDEventArgs(sessionGUID));
        }

        /// <summary>
        /// Запросить информацию пользователя
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <returns></returns>
        public UserNode GetUserInfo(Guid userGUID)
        {
            UserInfo userInfo;

            lock (connectedUsers)
            {
                if (connectedUsers.TryGetValue(userGUID, out userInfo)) return userInfo.User;
            }
            return null;
        }

        #region ValidateAccess() & CheckAccess()
        /// <summary>
        /// Проверить подключен ли пользователь к системе.
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        public void ValidateAccess(Guid userGUID)
        {
            lock (connectedUsers)
            {
                if (!connectedUsers.ContainsKey(userGUID)
                    && userGUID != InternalSession)
                    ThrowUserNotConnectedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userGUID"></param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна.</exception>
        public bool CheckAdminAccess(Guid userGUID)
        {
            lock (connectedUsers)
            {
                ValidateAccess(userGUID);

                UserInfo userInfo = connectedUsers[userGUID];

                return userInfo.User.IsAdmin;
            }
        }

        /// <summary>
        /// Проверить права доступа пользователя. 
        /// Является ли пользователь администратором.
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна.</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает запрашиваемыми правами доступа.</exception>
        public void ValidateAdminAccess(Guid userGUID)
        {
            if (!CheckAdminAccess(userGUID))
                throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// Проверить права доступа пользователя. 
        /// Обладает ли пользователь правами доступа к определенному узлу.
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitNode">Проверяемый узел</param>
        /// <param name="privileges">Запрашиваемые права доступа</param>
        /// <exception cref="UserNotConnectedException">Указанная сессия не действительна.</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает запрашиваемыми правами доступа.</exception>
        public void ValidateAccess(Guid userGUID, UnitNode unitNode, Privileges privileges)
        {
            lock (connectedUsers)
            {
                ValidateAccess(userGUID);

                if (!CheckAccess(userGUID, unitNode, privileges))
                    throw new UnauthorizedAccessException(FormatUnauthorizedAccessMessage(unitNode, privileges));
            }
        }

        /// <summary>
        /// Проверить права доступа пользователя. 
        /// Обладает ли пользователь правами доступа к определенному узлу.
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitNode">Проверяемый узел</param>
        /// <param name="privileges">Запрашиваемые права доступа</param>
        /// <returns>
        /// Если текущий пользователь обладает указанными правами, true.
        /// В противном случае метод вернет false, 
        /// а также добавит соответствующие сообщение в состояние асинхронной операции.
        /// </returns>
        /// <exception cref="UserNotConnectedException">Указанная сессия не действительна.</exception>
        public bool CheckAccess(OperationState state, UnitNode unitNode, Privileges privileges)
        {
            lock (connectedUsers)
            {
                bool ret;
                ValidateAccess(state.UserGUID);

                ret = CheckAccess(state.UserGUID, unitNode, privileges);

                //if (!ret)
                //    state.AddMessage(new Message(MessageCategory.Error, FormatUnauthorizedAccessMessage(unitNode, privileges)));

                return ret;
            }
        }

        /// <summary>
        /// Сформировать текст сообщения при отказе в доступе
        /// </summary>
        /// <param name="unitNode">Узел, к которому запрашивался доступ.</param>
        /// <param name="privileges">Запрашиваемые права</param>
        /// <returns></returns>
        private String FormatUnauthorizedAccessMessage(UnitNode unitNode, Privileges privileges)
        {
            return String.Format("Не достаточно прав для {1} '{0}'", UnitManager.GetFullName(unitNode),
                (privileges == Privileges.Read ? "чтения" :
                privileges == Privileges.Write ? "записи" :
                privileges == Privileges.Execute ? "использования" : "узла"));
        }

        private void ThrowUserNotConnectedException()
        {
            var exp = new UserNotConnectedException();
            log.ErrorException("Не верное значение сессии.", exp);
            throw exp;
        }

        /// <summary>
        /// Проверить доступ пользователя
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitNode">Узел, доступ к которому запрашивается</param>
        /// <param name="privileges">Проверяемые права</param>
        /// <returns>
        /// Если текущий пользователь обладает указанными правами, true.
        /// В противном случае - false.
        /// </returns>
        /// <exception cref="UserNotConnectedException">Указанная сессия не действительна.</exception>
        public bool CheckAccess(Guid userGUID, UnitNode unitNode, Privileges privileges)
        {
            lock (connectedUsers)
            {
                ValidateAccess(userGUID);

                UserInfo userInfo = connectedUsers[userGUID];

                if (unitNode == null)
                {
                    // Корень все могут читать, но писать туда может только администратор
                    return (privileges & Privileges.Write) == Privileges.NothingDo || userInfo.User.IsAdmin;
                }

                return userInfo.User.IsAdmin
                     || (userInfo.User.CheckPrivileges(unitNode.Typ, privileges)
                    && userInfo.User.CheckGroupPrivilegies(unitNode.Owner, privileges));
            }
        } 
        #endregion

        #region Alive()
        /// <summary>
        /// Обновить последние время активности пользователя
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя.</param>
        public void Alive(Guid userGUID)
        {
            UserInfo userInfo;

            lock (connectedUsers)
            {
                if (connectedUsers.TryGetValue(userGUID, out userInfo))
                    userInfo.LastActive = DateTime.Now;
                else ThrowUserNotConnectedException();
            }
        }

        public bool CheckAliveUser(Guid userGUID)
        {
            return connectedUsers.ContainsKey(userGUID);
            //return true;
        }

        public readonly TimeSpan DefaultUserTimeOut = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Время после последней активности пользователя, перед его отключением по таймауту.
        /// </summary>
        public TimeSpan UserTimeout { get; private set; }

        /// <summary>
        /// Проверить не пора ли отключить пользователя по таймауту.
        /// </summary>
        /// <param name="userInfo">Информация о сессии пользователя</param>
        /// <returns></returns>
        private bool CheckAliveUser(UserInfo userInfo)
        {
            return userInfo == InternalUser || DateTime.Now.Subtract(userInfo.LastActive) < UserTimeout;
        } 
        #endregion

        public bool AreSameUser(Guid userGUID, Guid anotherGUID)
        {
            lock (connectedUsers)
            {
                UserInfo userInfo, anotherUserInfo;
                if (connectedUsers.TryGetValue(userGUID, out userInfo)
                    && connectedUsers.TryGetValue(anotherGUID, out anotherUserInfo)
                    && userInfo.User.Idnum == anotherUserInfo.User.Idnum)
                {
                    return true;
                }
                return false;
            }
        }
    }
}

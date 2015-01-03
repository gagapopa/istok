using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using COTES.ISTOK.ASC;
using NLog;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Управление пользователями и группами
    /// </summary>
    class UserManager : IUserManager 
    {
        Logger log = LogManager.GetCurrentClassLogger();

        ISecurityManager securityManager;

        MyDBdata dbwork;

        public UserManager(MyDBdata dbwork, ISecurityManager securityManager)
        {
            this.securityManager = securityManager;
            this.dbwork = dbwork.Clone();
        }

        List<UserNode> usersList;

        private List<UserNode> users
        {
            get
            {
                if (usersList == null)
                    usersList = LoadUsers();

                return usersList;
            }
        }

        List<GroupNode> groupList;

        private List<GroupNode> groups
        {
            get
            {
                if (groupList == null)
                    groupList = LoadGroups();
                return groupList;
            }
        }

        /// <summary>
        /// Запросить находится ли система в режиме песочницы
        /// </summary>
        /// <remarks>Режим песочницы наступает если в системе не пресутсвует ни одного пользователя</remarks>
        private bool SandboxMode { get { return users != null && users.Count == 0; } }

        #region Работа с пользователями

        /// <summary>
        /// Запросить узел пользователя по его логину
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns>
        /// Узел запрашиваемого пользователя, 
        /// null - если пользователя с таким именем не найдено
        /// </returns>
        public UserNode GetUser(string userName)
        {
            UserNode node = null;
            if (users != null)
            {
                lock (users)
                {
                    if (users.Count == 0)
                        throw new NoOneUserException();

                    node = users.Find(u => u.Text.Equals(userName));
                }

            }
            return node;
        }

        /// <summary>
        /// Получить узел пользователя по UID
        /// </summary>
        /// <param name="userID">Идентификатор пользователя</param>
        /// <returns>Узел пользователя</returns>
        public UserNode GetUser(int userID)
        {
            if (users == null)
                return null;

            lock (users)
            {
                return users.Find(u => u.Idnum.Equals(userID));
            }
        }

        /// <summary>
        /// Загрузить пользователей из БД
        /// </summary>
        /// <returns>Список загруженных пользователей</returns>
        private List<UserNode> LoadUsers()
        {
            List<UserNode> ret = new List<UserNode>();

            try
            {
                DataTable dt = null;
                if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

                dt = dbwork.ExecSQL_toTable("select * from users order by idnum", null);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DataTable groups = null;
                        UserNode newnode = new UserNode(row);
                        DB_Parameters pars = new DB_Parameters();
                        pars.Add("p_idnum", DbType.Int32, newnode.Idnum);
                        groups = dbwork.ExecSQL_toTable("select * from usergroup where userid=@p_idnum", pars);
                        foreach (DataRow groupRow in groups.Rows)
                        {
                            newnode.SetGroupPrivilegies((int)groupRow["groupid"], (Privileges)(int)groupRow["priv"]);
                        }
                        groups.Dispose();
                        ret.Add(newnode);
                    }
                    dt.Dispose();
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("", ex);
                ret = null;
                throw;
            }
            return ret;
        }

        /// <summary>
        /// Обновить пользователя из БД
        /// </summary>
        /// <param name="curnode">Узел пользователя для обновления</param>
        /// <returns>Обновленный ущел пользователя</returns>
        private UserNode ReloadUserNode(UserNode curnode)
        {
            lock (users)
            {
                int index = users.IndexOf(curnode);
                bool res = users.Remove(curnode);
                if (res)
                    return LoadUserNode(curnode.Idnum, index);
                return null;
            }
        }

        /// <summary>
        /// Загрузить и добавить в список пользователей пользователя из БД
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="index">Индекс в списке пользователя, куда следует добавить загруженного пользователя</param>
        /// <returns>Загруженный пользователь</returns>
        private UserNode LoadUserNode(int id, int index)
        {
            DataTable dt = null;
            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            dt = dbwork.ExecSQL_toTable("select * from users where idnum=@p_idnum", id);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataTable groups = null;
                    UserNode newnode = new UserNode(dt.Rows[0]);
                    DB_Parameters pars = new DB_Parameters();
                    pars.Add("p_idnum", DbType.Int32, newnode.Idnum);
                    dt.Dispose();
                    groups = dbwork.ExecSQL_toTable("select * from usergroup where userid=@p_idnum", pars);
                    foreach (DataRow groupRow in groups.Rows)
                    {
                        newnode.SetGroupPrivilegies((int)groupRow["groupid"], (Privileges)(int)groupRow["priv"]);
                    }
                    groups.Dispose();
                    users.Insert(index, newnode);
                    return newnode;
                }
                return null;
        }

        /// <summary>
        /// Запросить список пользователей, присутствующих в системе
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        public UserNode[] GetUserNodes(OperationState state)
        {
            securityManager.ValidateAdminAccess(state.UserGUID);

            List<UserNode> userList = new List<UserNode>();

            lock (users)
            {
                foreach (UserNode userNode in users)
                {
                    UserNode clonnedNode = (UserNode)userNode.Clone();
                    clonnedNode.Password = null;
                    //state.AddAsyncResult(clonnedNode);
                    userList.Add(clonnedNode);
                }
            }
            return userList.ToArray();
        }

        /// <summary>
        /// Запросить список пользователей, присутствующих в системе
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="ids">Массив ИДов</param>
        public UserNode[] GetUserNodes(OperationState state, int[] ids)
        {
            List<UserNode> lstRes = new List<UserNode>();
            securityManager.ValidateAdminAccess(state.UserGUID);

            foreach (UserNode userNode in users)
            {
                UserNode clonnedNode = (UserNode)userNode.Clone();
                clonnedNode.Password = null;
                lstRes.Add(clonnedNode);
            }

            return lstRes.ToArray();
        }

        /// <summary>
        /// Изменить пользователя 
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="updnode">Редактируемый узел пользователя</param>
        /// <returns>Обновленный узел пользователя</returns>
        public UserNode UpdateUserNode(OperationState state, UserNode updnode)
        {
            UserNode curnode = users.Find(x => x.Idnum == updnode.Idnum);
            UserNode newnode = null;

            if (curnode == null) throw new Exception("Редактируемый объект не найден в серверном списке.");
            securityManager.ValidateAdminAccess(state.UserGUID);

            if (String.IsNullOrEmpty(updnode.Text)) throw new ArgumentException("Имя пользователя не может быть пустым");

            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            int transaction = 0;
            try
            {
                if (!String.IsNullOrEmpty(updnode.Password))
                {
                    updnode.Password = securityManager.GetHashPassword(updnode.Password);
                }
                else
                {
                    updnode.Password = curnode.Password;
                }

                transaction = dbwork.StartTransaction();
                string query = "update users set name=@p_name,pass=@p_pass,fullname=@p_fullname,position=@p_position,roles=@p_roles where idnum=@p_idnum";
                DB_Parameters dbp = new DB_Parameters();
                dbp.Add("p_name", DbType.String, updnode.Text);
                dbp.Add("p_pass", DbType.Binary, Encoding.ASCII.GetBytes(updnode.Password));
                dbp.Add("p_fullname", DbType.String, updnode.UserFullName);
                dbp.Add("p_position", DbType.String, updnode.Position);
                dbp.Add("p_roles", DbType.String, updnode.Roles);
                dbp.Add("p_idnum", DbType.Int32, updnode.Idnum);
                dbwork.ExecSQL(transaction, query, dbp);
                UpdateUserGroups(transaction, (UserNode)updnode);
                dbwork.Commit(transaction);
                securityManager.UnregisterAll(curnode.Idnum);
            }
            catch (Exception ex)
            {
                dbwork.Rollback(transaction);
                log.ErrorException("Ошибка изменения пользователяя", ex);
                throw new Exception("Ошибка изменения пользователяя", ex);
            }
            finally
            {
                dbwork.CloseTransaction(transaction);
            }
            newnode = ReloadUserNode(curnode);
            return newnode;
        }

        /// <summary>
        /// Добавить нового пользователя
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="addnode">Добавляемый пользователь</param>
        /// <returns>Добавленный пользователь</returns>
        public UserNode AddUserNode(OperationState state, UserNode addnode)
        {
            if (!SandboxMode)
                securityManager.ValidateAdminAccess(state.UserGUID);

            if (String.IsNullOrEmpty(addnode.Text)) throw new ArgumentException("Имя пользователя не может быть пустым");
            if (String.IsNullOrEmpty(addnode.Password)) throw new ArgumentException("Пароль не может быть пустым");

            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            int transaction = 0;
            int newidnum = 0;
            try
            {
                transaction = dbwork.StartTransaction();
                // запись в таблицу типов объектов
                string query = "insert into users (name,pass,fullname,position,roles) values (@p_name,@p_pass,@p_fullname,@p_position,@p_roles);";
                query += dbwork.SelectIdentity;
                DB_Parameters dbp = new DB_Parameters();
                dbp.Add("p_name", DbType.String, addnode.Text);
                dbp.Add("p_pass", DbType.Binary, Encoding.ASCII.GetBytes(securityManager.GetHashPassword(addnode.Password)));
                dbp.Add("p_fullname", DbType.String, addnode.UserFullName);
                dbp.Add("p_position", DbType.String, addnode.Position);
                dbp.Add("p_roles", DbType.String, addnode.Roles);
                using (DbDataReader reader = dbwork.ExecSQL_toReader(transaction, query, dbp))
                {
                    if (reader.Read())
                    {
                        newidnum = Convert.ToInt32(reader.GetValue(0));
                        addnode.Idnum = newidnum;
                    }
                }
                UpdateUserGroups(transaction, addnode);
                dbwork.Commit(transaction);
                // добавить в список
                //users = null;
            }
            catch (Exception ex)
            {
                dbwork.Rollback(transaction);
                log.ErrorException("Ошибка добавления пользователя", ex);
                throw new Exception("Ошибка добавления пользователя", ex);
            }
            finally
            {
                dbwork.CloseTransaction(transaction);
            }
            lock (users)
            {
                return LoadUserNode(newidnum, users.Count);
            }
        }

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="userNodeID">Идентификаторы удаляемого пользователя</param>
        public void RemoveUserNode(OperationState state, int userNodeID)
        {
            UserNode curnode = users.Find(x => x.Idnum == userNodeID);
            if (curnode == null) throw new Exception("Удаляемый объект не найден в серверном списке.");

            securityManager.ValidateAdminAccess(state.UserGUID);

            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            int transaction = 0;
            try
            {
                transaction = dbwork.StartTransaction();
                // удалим запись из таблицы типов объектов
                string query = "delete from users where idnum=@p_idnum; " +
                "delete from usergroup where userid=@p_idnum";
                DB_Parameters dbp = new DB_Parameters();
                dbp.Add("p_idnum", DbType.Int32, curnode.Idnum);
                dbwork.ExecSQL(transaction, query, dbp);
                dbwork.Commit(transaction);
                // удалить это оборудование из списка
                lock (users)
                {
                    users.Remove(curnode);
                }
                securityManager.UnregisterAll(userNodeID);
            }
            catch (Exception ex)
            {
                dbwork.Rollback(transaction);
                log.ErrorException("Ошибка удаления пользователя", ex);
                throw new Exception("Ошибка удаления пользователя", ex);
            }
            finally
            {
                dbwork.CloseTransaction(transaction);
            }
        }

        /// <summary>
        /// Обновить таблицу прав пользователя в БД по отношению к группам
        /// </summary>
        /// <param name="userNode">Узел пользователя</param>
        private void UpdateUserGroups(int transaction, UserNode userNode)
        {
            Dictionary<int, Privileges> groupPrivilegies = userNode.getGroups();
            String updateQuery = "update usergroup set priv=@p_priv where userid=@p_idnum and groupid=@p_groupid",
                insertQuery = "insert into usergroup (userid, groupid, priv) values(@p_idnum, @p_groupid, @p_priv)",
                deleteQuery = "delete from usergroup where userid=@p_idnum and groupid=@p_groupid";
            DB_Parameters dbp = new DB_Parameters();

            if (groupPrivilegies != null)
                foreach (int groupId in groupPrivilegies.Keys)
                {
                    GroupNode gr = groups.Find(x => x.Idnum == groupId);
                    if (gr != null)
                    {
                        dbp.Clear();
                        dbp.Add("p_idnum", DbType.Int32, userNode.Idnum);
                        dbp.Add("p_groupid", DbType.Int32, groupId);
                        dbp.Add("p_priv", DbType.Int32, (int)groupPrivilegies[groupId]);
                        if ((groupPrivilegies[groupId] & Privileges.Read) == 0)
                            dbwork.ExecSQL(transaction, deleteQuery, dbp);
                        else
                        {
                            int rows = dbwork.ExecSQL(transaction, updateQuery, dbp);
                            if (rows == 0) dbwork.ExecSQL(transaction, insertQuery, dbp);
                        }
                    }
                }
        }

        /// <summary>
        /// Добавить первого поьзователя
        /// </summary>
        /// <param name="userNode">Узел первого пользователя</param>
        public void NewAdmin(UserNode userNode)
        {
            if (SandboxMode)
            {
                AddUserNode(new OperationState(securityManager.InternalSession), userNode);
            }
            else throw new UnauthorizedAccessException();
        }

        //public UserNode[] GetUsers()
        //{
        //    UserNode[] usersBU = users.ToArray();
        //    UserNode[] retUsers = new UserNode[usersBU.Length];

        //    for (int i = 0; i < usersBU.Length; i++)
        //    {
        //        retUsers[i] = usersBU[i].Clone() as UserNode;
        //        retUsers[i].Password = String.Empty;
        //    }

        //    return retUsers;
        //}
        #endregion

        #region Работа с группами пользователей
        /// <summary>
        /// Получить узел группы пользователей по GID
        /// </summary>
        /// <param name="GID">Идентификатор группы</param>
        /// <returns>Узел группы</returns>
        public GroupNode GetGroup(int GID)
        {
            if (groups == null)
                return null;

            lock (groups)
            {
                return groups.Find(x => x.Idnum == GID);
            }
        }

        /// <summary>
        /// Запросить узел пользователя по его логину
        /// </summary>
        /// <param name="groupName">Уникальное имя группы</param>
        /// <returns>Узел группы</returns>
        public GroupNode GetGroup(string groupName)
        {
            if (groups == null)
                return null;

            lock (groups)
            {
                return groups.Find(x => x.Text.Equals(groupName));
            }
        }

        /// <summary>
        /// Загрузить список групп пользователя
        /// </summary>
        /// <returns>Загруженный список пользователей</returns>
        private List<GroupNode> LoadGroups()
        {
            //this.Clear();
            List<GroupNode> groupNodeList = new List<GroupNode>();

            try
            {
                DataTable dt = null;
                if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");
                dt = dbwork.ExecSQL_toTable("select * from groups order by idnum", null);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        GroupNode newnode = new GroupNode(row);
                        groupNodeList.Add(newnode);
                    }
                    dt.Dispose();
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("", ex);
                return null;
            }
            return groupNodeList;
        }

        /// <summary>
        /// Обновить узел группы пользователей из БД
        /// </summary>
        /// <param name="curnode">Обновляемый узел</param>
        /// <returns>Обновленный узел</returns>
        private GroupNode ReloadItem(GroupNode curnode)
        {
            lock (groups)
            {
                int index = groups.IndexOf(curnode);
                bool res = groups.Remove(curnode);
                if (res)
                    return LoadGroupNode(curnode.Idnum, index);
                return null;
            }
        }

        /// <summary>
        /// Загрузить узел группы пользователей из БД и добавить в список групп
        /// </summary>
        /// <param name="id">Идентификатор группы</param>
        /// <param name="index">Индекс в списке групп, куда будет добавлятся группа</param>
        /// <returns>Загруженный узел группы</returns>
        private GroupNode LoadGroupNode(int id, int index)
        {
            DataTable dt = null;
            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");
            dt = dbwork.ExecSQL_toTable("select * from groups where idnum=@p_idnum", id);

            if (dt != null && dt.Rows.Count > 0)
            {
                GroupNode newnode = new GroupNode(dt.Rows[0]);
                groups.Insert(index, newnode);
                dt.Dispose();
                return newnode;
            }
            return null;
        }

        /// <summary>
        /// Запросить список групп пользователей
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        public GroupNode[] GetGroupNodes(OperationState state)
        {
            if (!SandboxMode)
                securityManager.ValidateAccess(state.UserGUID);

            List<GroupNode> groupList=new List<GroupNode>();

            lock (groups)
            {
                if (groups != null)
                    foreach (GroupNode groupNode in groups)
                    {
                        //state.AddAsyncResult(groupNode);
                        groupList.Add(groupNode);
                    }
            }
            return groupList.ToArray();
        }

        /// <summary>
        /// Добавить новую группу пользователей
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="addnode">Добавляемая группа</param>
        /// <returns>Добавленная группа</returns>
        public GroupNode AddGroupNode(OperationState state, GroupNode addnode)
        {
            securityManager.ValidateAdminAccess(state.UserGUID);

            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");
            int transaction = 0;
            int newidnum = 0;
            try
            {
                transaction = dbwork.StartTransaction();

                // запись в таблицу типов объектов
                string query = "insert into groups (name,description) values (@p_name,@p_desc); ";
                query += dbwork.SelectIdentity;
                DB_Parameters dbp = new DB_Parameters();
                dbp.Add("p_name", DbType.String, addnode.Text);
                dbp.Add("p_desc", DbType.String, ((GroupNode)addnode).Description);
                using (DbDataReader reader = dbwork.ExecSQL_toReader(transaction, query, dbp))
                {
                    reader.Read();
                    newidnum = Convert.ToInt32(reader.GetValue(0));
                }
                dbwork.Commit(transaction);
                // добавить в список
            }
            catch (Exception ex)
            {
                dbwork.Rollback(transaction);
                log.ErrorException("Ошибка добавления пользователя", ex);
                throw new Exception("Ошибка добавления пользователя", ex);
            }
            finally
            {
                dbwork.CloseTransaction(transaction);
            }
            lock (groups)
            {
                return LoadGroupNode(newidnum, groups.Count);
            }
        }

        /// <summary>
        /// Изменить группу пользователя
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="updnode">Редактируемая группа пользователей</param>
        /// <returns>Обновленная группа пользователей</returns>
        public GroupNode UpdateGroupNode(OperationState state, GroupNode updnode)
        {
            GroupNode curnode = groups.Find(x => x.Idnum == updnode.Idnum);
            GroupNode retNode = null;
            if (curnode == null) throw new Exception("Редактируемый объект не найден в серверном списке.");

            securityManager.ValidateAdminAccess(state.UserGUID);

            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");
            int transaction = 0;
            try
            {
                transaction = dbwork.StartTransaction();
                string query = "update groups set name=@p_name,description=@p_desc where idnum=@p_idnum";
                DB_Parameters dbp = new DB_Parameters();
                dbp.Add("p_name", DbType.String, updnode.Text);
                dbp.Add("p_desc", DbType.String, ((GroupNode)updnode).Description);//(updnode as UserNode).Roles);
                dbp.Add("p_idnum", DbType.Int32, updnode.Idnum);
                dbwork.ExecSQL(transaction, query, dbp);
                dbwork.Commit(transaction);
                retNode = ReloadItem(curnode);
            }
            catch (Exception ex)
            {
                dbwork.Rollback(transaction);
                log.ErrorException("Ошибка изменения пользователяя", ex);
                throw new Exception("Ошибка изменения пользователяя", ex);
            }
            finally
            {
                dbwork.CloseTransaction(transaction);
            }
            return retNode;
        }

        /// <summary>
        /// Удалить группу пользователей
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="groupNodeID">Идентификатор удаляемой группы</param>
        public void RemoveGroupNode(OperationState state, int groupNodeID)
        {
            GroupNode curnode = groups.Find(x => x.Idnum == groupNodeID);
            if (curnode == null) throw new Exception("Удаляемый объект не найден в серверном списке.");

            securityManager.ValidateAdminAccess(state.UserGUID);

            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");
            int transaction = 0;
            try
            {
                transaction = dbwork.StartTransaction();
                // удалим запись из таблицы типов объектов
                string query = "delete from groups where idnum=@p_idnum; delete from usergroup where groupid=@p_idnum";
                DB_Parameters dbp = new DB_Parameters();
                dbp.Add("p_idnum", DbType.Int32, curnode.Idnum);
                dbwork.ExecSQL(transaction, query, dbp);
                dbwork.Commit(transaction);
                // удалить это оборудование из списка
                lock (groups)
                {
                    groups.Remove(curnode);
                }
            }
            catch (Exception ex)
            {
                dbwork.Rollback(transaction);
                log.ErrorException("Ошибка удаления пользователя", ex);
                throw new Exception("Ошибка удаления пользователя", ex);
            }
            finally
            {
                dbwork.CloseTransaction(transaction);
            }
        }
        #endregion
    }
}

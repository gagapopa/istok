using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Assignment.Extension;
using NLog;

namespace COTES.ISTOK.Assignment
{
    class UnitTypeManager : IUnitTypeManager
    {
        Logger log = LogManager.GetCurrentClassLogger();

        private MyDBdata dbwork;
        public ISecurityManager SecurityManager { get; set; }

        public ExtensionManager ExtensionManager { get; set; }

        public UnitTypeManager(MyDBdata dbwork)
        {
            this.dbwork = dbwork.Clone();
        }

        List<UTypeNode> types;

        #region Загрузка структуры

        public event EventHandler<UnitTypeEventArgs> UnitTypeChanged;

        protected void OnUnitTypeChanged(UTypeNode unitType)
        {
            if (UnitTypeChanged != null)
                UnitTypeChanged(this, new UnitTypeEventArgs((UnitTypeId)unitType.Idnum));
        }

        // проверить правильность хэша у типа оборудования
        private bool CheckTypeHash(int type, object hash)
        {
            //string r_hash = GetTypeHash(type);
            //string n_hash = null;

            //if (r_hash == null) return true;

            //if (hash.GetType() != typeof(string))
            //{
            //    if (hash != null && hash != DBNull.Value) n_hash = Encoding.UTF8.GetString((byte[])hash);
            //}
            //else
            //    n_hash = (string)hash;

            //if (r_hash != n_hash) return false;
            return true;
        }

        public UTypeNode GetUnitType(OperationState state, int type)
        {
            SecurityManager.ValidateAccess(state.UserGUID);

            return types.Find(t => t.Idnum.Equals((int)type));
        }

        public void LoadTypes(OperationState state)
        {
            types = new List<UTypeNode>();
            //types.Clear();

            UTypeNode[] extensionTypes = ExtensionManager.GetExtensionUnitType();

            //try
            //{
            DataTable dt = null;
            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            dt = dbwork.ExecSQL_toTable("select * from unit_type order by idnum", null);

            if (dt != null)
            {
                int type = (int)UnitTypeId.Station;
                bool can_add;

                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        type = (int)row["idnum"];
                        can_add = false;
                    }
                    catch { can_add = true; }

                    if (!can_add)
                        can_add = CheckTypeHash(type, row["fkey"]);
                    if (can_add)
                    {
                        UTypeNode newnode = LoadUnitTypeNode(row);

                        newnode.AcceptChanges();
                        types.Add(newnode);
                    }
                }
                dt.Dispose();
            }
            foreach (UTypeNode extType in extensionTypes)
            {
                UTypeNode type = types.Find(t => t.ExtensionGUID == extType.ExtensionGUID);
                if (type == null)
                    AddUnitType(new OperationState(SecurityManager.InternalSession), extType);
            }
            //return true;
            //}
            //catch (Exception ex)
            //{
            //    log.Message(MessageLevel.Error, ex);
            //    return false;
            //}
        }

        /// <summary>
        /// Создать и заполнить объект типа оборудования
        /// </summary>
        /// <param name="row">строка с данными, загружаемого типа</param>
        /// <returns>Тип с заполненыим свойствами</returns>
        private static UTypeNode LoadUnitTypeNode(DataRow row)
        {
            UTypeNode newnode = new UTypeNode();
            newnode.Idnum = (int)row["idnum"];
            newnode.Text = row["name"].ToString();
            //newnode.Tree_visible = (int)row["tree_visible"];
            newnode.Props = row["props"].ToString();
            if (!row.IsNull("img"))
                newnode.Icon = (byte[])row["img"];
            else
                newnode.Icon = null;
            if (row.Table.Columns.Contains("ext_guid") && !row.IsNull("ext_guid"))
                newnode.ExtensionGUID = new Guid((byte[])row["ext_guid"]);
            String filter;
            if (DBNull.Value.Equals(row["child_filter"])) filter = null;
            else filter = row["child_filter"].ToString();

            newnode.Filter = filter;

            return newnode;
        }

        /// <summary>
        /// Запросить тип из БД
        /// </summary>
        /// <param name="id">Идентификатор загружаемого типа</param>
        /// <param name="index">Индес в списке типов, куда требуется добавить тип</param>
        /// <returns>Загруженый тип оборудования</returns>
        public UTypeNode LoadUnitType(int id, int index)
        {
            DataTable dt = null;
            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            dt = dbwork.ExecSQL_toTable("select * from unit_type where idnum=@p_idnum", id);

            if (dt != null && dt.Rows.Count > 0)
            {
                UTypeNode newnode = LoadUnitTypeNode(dt.Rows[0]);

                types.Insert(index, newnode);
                dt.Dispose();
                return newnode;
            }
            return null;
        }

        /// <summary>
        /// Перегрузить тип оборудования
        /// </summary>
        /// <param name="item">Старый экземпляр типа</param>
        public UTypeNode ReloadUnitType(UTypeNode item)
        {
            int index = types.IndexOf(item);
            bool res = types.Remove(item);
            if (res)
            {
                return LoadUnitType(item.Idnum, index);
            }
            return null;
        }

        /// <summary>
        /// очистить структуру
        /// </summary>
        private void Clear()
        {
            if (!Monitor.TryEnter(types, 10000))
            {
                log.Error("GNSI.Clear: Серверный список типов оборудования занят.");
                return;
            }
            try { types.Clear(); }
            finally { Monitor.Exit(types); }
        }
        #endregion

        #region Работа с типами
        /// <summary>
        /// Cинхронный запрос типов оборудования
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        public UTypeNode[] GetUnitTypes(OperationState state)
        {
            //if (!securityManager.SandboxMode)
            SecurityManager.ValidateAccess(state.UserGUID);

            var res = new List<UTypeNode>();

            UserNode unode = SecurityManager.GetUserInfo(state.UserGUID);
            foreach (UTypeNode typeNode in types)
            {
                if (unode == null
                    || unode.IsAdmin
                    || unode.CheckPrivileges(typeNode.Idnum, Privileges.Read))
                {
                    res.Add(typeNode);
                }
            }

            return res.ToArray();
        }

        /// <summary>
        /// Асинхронно сохранить изменения типа оборудования
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitTypeNode">Тип оборудования для сохранения</param>
        public UTypeNode UpdateUnitType(OperationState state, UTypeNode unitTypeNode)
        {
            SecurityManager.ValidateAdminAccess(state.UserGUID);

            UTypeNode curnode = types.Find((node) => node.Idnum == unitTypeNode.Idnum);

            if (curnode == null) throw new Exception("Редактируемый объект не найден в серверном списке.");

            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            int transaction = 0;
            try
            {
                transaction = dbwork.StartTransaction();
                string query = "update unit_type set name=@p_name,img=@p_img,props=@p_props,ext_guid=@p_ext_guid, child_filter=@p_child_filter where idnum=@p_idnum";
                DB_Parameters dbp = new DB_Parameters();
                dbp.Add("p_name", DbType.String, unitTypeNode.Text);
                //dbp.Add("p_tree_visible", DbType.Int32, unitTypeNode.Tree_visible);
                dbp.Add("p_img", DbType.Binary, unitTypeNode.Icon);
                dbp.Add("p_props", DbType.String, unitTypeNode.Props);
                dbp.Add("p_ext_guid", DbType.Binary, unitTypeNode.ExtensionGUID == Guid.Empty ? null : unitTypeNode.ExtensionGUID.ToByteArray());
                dbp.Add("p_child_filter", DbType.String, unitTypeNode.Filter);
                dbp.Add("p_idnum", DbType.Int32, unitTypeNode.Idnum);
                dbwork.ExecSQL(transaction, query, dbp);
                dbwork.Commit(transaction);
                log.Trace("Пользователь {{0}} обновил тип {1}.",
                                SecurityManager.GetUserInfo(state.UserGUID).Text,
                                unitTypeNode.Text);
            }
            catch (Exception ex)
            {
                dbwork.Rollback(transaction);
                log.ErrorException("UTypeList.Update", ex);
                throw new Exception("Ошибка изменения типа оборудования", ex);
            }
            finally
            {
                dbwork.CloseTransaction(transaction);
            }
            var retNode = ReloadUnitType(curnode);
            OnUnitTypeChanged(curnode);
            return retNode;
        }

        /// <summary>
        /// Добавить новый тип оборудования
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitTypeNode">Тип оборудования для добавления</param>
        public UTypeNode AddUnitType(OperationState state, UTypeNode unitTypeNode)
        {
            SecurityManager.ValidateAdminAccess(state.UserGUID);

            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            int transaction = 0;
            int newidnum = 0;
            try
            {
                transaction = dbwork.StartTransaction();
                // запись в таблицу типов объектов
                string query = "insert into unit_type (name,img,props, ext_guid, child_filter) values (@p_name,@p_img,@p_props,@ext_guid,@p_child_filter); ";
                query += dbwork.SelectIdentity;
                DB_Parameters dbp = new DB_Parameters();
                dbp.Add("p_name", DbType.String, unitTypeNode.Text);
                //dbp.Add("p_tree_visible", DbType.Int32, unitTypeNode.Tree_visible);
                dbp.Add("p_img", DbType.Binary, unitTypeNode.Icon);
                dbp.Add("p_props", DbType.String, unitTypeNode.Props);
                dbp.Add("ext_guid", DbType.Binary, unitTypeNode.ExtensionGUID == Guid.Empty ? null : unitTypeNode.ExtensionGUID.ToByteArray());
                dbp.Add("p_child_filter", DbType.String, unitTypeNode.Filter);
                using (DbDataReader reader = dbwork.ExecSQL_toReader(transaction, query, dbp))
                {
                    reader.Read();
                    newidnum = Convert.ToInt32(reader.GetValue(0));
                }
                dbwork.Commit(transaction);
                if (log.IsTraceEnabled)
                {
                    log.Trace("Пользователь {0} добавил тип {1}.",
                      SecurityManager.GetUserInfo(state.UserGUID).Text,
                      unitTypeNode.Text); 
                }
            }
            catch (Exception ex)
            {
                dbwork.Rollback(transaction);
                log.ErrorException("UTypeList.Append", ex);
                throw new Exception("Ошибка добавления типа оборудования", ex);
            }
            finally
            {
                dbwork.CloseTransaction(transaction);
            }
            // добавить в список
            UTypeNode newTypeNode = LoadUnitType(newidnum, types.Count);
            OnUnitTypeChanged(newTypeNode);
            return newTypeNode;
        }

        /// <summary>
        /// Удалить тип оборудования
        /// </summary>
        /// <param name="state">Состояние асинхронной операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitTypeNodeID">Идентификатор типа оборудования для удаления</param>
        public void RemoveUnitType(OperationState state, int unitTypeNodeID)
        {
            SecurityManager.ValidateAdminAccess(state.UserGUID);

            UTypeNode curnode = types.Find(node => node.Idnum == unitTypeNodeID);

            if (curnode == null) throw new Exception("Удаляемый объект не найден в серверном списке.");

            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            int transaction = 0;
            try
            {
                transaction = dbwork.StartTransaction();
                // удалим запись из таблицы типов объектов
                string query = "delete from unit_type where idnum=@p_idnum";
                DB_Parameters dbp = new DB_Parameters();
                dbp.Add("p_idnum", DbType.Int32, curnode.Idnum);
                dbwork.ExecSQL(transaction, query, dbp);
                dbwork.Commit(transaction);
                // удалить это оборудование из списка
                types.Remove(curnode);
                if (log.IsTraceEnabled)
                {
                    log.Trace("Пользователь {0} удалил тип {1}.",
                        SecurityManager.GetUserInfo(state.UserGUID).Text,
                        curnode.Text); 
                }
            }
            catch (Exception ex)
            {
                dbwork.Rollback(transaction);
                log.ErrorException("UTypeList.Delete", ex);
                throw new Exception("Ошибка удаления типа оборудования", ex);
            }
            finally
            {
                dbwork.CloseTransaction(transaction);
            }
            OnUnitTypeChanged(curnode);
        }
        #endregion

        public int GetExtensionType(Guid parentGUID)
        {
            if (parentGUID != Guid.Empty)
            {
                UTypeNode typeNode = types.Find(t => t.ExtensionGUID == parentGUID);

                if (typeNode != null)
                    return typeNode.Idnum;
            }
            return (int)UnitTypeId.Unknown;
        }
    }
}

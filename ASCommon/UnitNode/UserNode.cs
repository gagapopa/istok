using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Представление для пользователя
    /// </summary>
    [Serializable]
    [DataContract]
    public class UserNode : ServerNode
    {
        [DataMember]
        private Dictionary<int, Privileges> userPriv;
        [DataMember]
        private Dictionary<int, Privileges> groupPriv;

        //public int idnum = 0;
        //public int Idnum { get { return idnum; } set { idnum = value; } }
        //public string Text { get; set; }
        //public string FullName { get; set; }

        /// <summary>
        /// Проверить обладает ли пользователь правом на оборудование
        /// </summary>
        /// <param name="unitType">Тип оборудования</param>
        /// <param name="priv">Права, на которые производится проверка</param>
        /// <returns>true, если пользователь обладает данными правами,
        /// false, в противном случае</returns>
        public virtual bool CheckPrivileges(int unitType, Privileges priv)
        {
            Privileges savedPriv = Privileges.NothingDo;
            if (userPriv != null && userPriv.TryGetValue(unitType, out savedPriv))
                return (savedPriv & priv) != Privileges.NothingDo;//== priv;
            return false;
        }

        /// <summary>
        /// Установить права пользователя на оборудование
        /// </summary>
        /// <param name="unitType">Тип оборудования</param>
        /// <param name="priv">Устанавливаемые права</param>
        public virtual void SetPrivileges(int unitType, Privileges priv)
        {
            Privileges pr = Privileges.NothingDo;
            if (userPriv == null) userPriv = new Dictionary<int, Privileges>();
            if (userPriv.TryGetValue(unitType, out pr))
            {
                if (pr != priv)
                {
                    userPriv[unitType] = priv;
                    //modified = true;
                }
            }
            else if (priv != Privileges.NothingDo)
            {

                userPriv.Add(unitType, priv);
                //modified = true;
            }
        }

        /// <summary>
        /// Получить права пользователя относитьльно групп
        /// </summary>
        /// <returns>Матрицу прав пользователя, относитьльно групп</returns>
        public Dictionary<int, Privileges> getGroups()
        {
            return groupPriv;
        }

        /// <summary>
        /// Проверить обладает ли пользователь правом на группу
        /// </summary>
        /// <param name="groupId">ИД группы</param>
        /// <param name="priv">Права, на которые производится проверка</param>
        /// <returns>true, если пользователь обладает данными правами
        /// false, в противном случае</returns>
        public virtual bool CheckGroupPrivilegies(int groupId, Privileges priv)
        {
            if (groupId == 0) return true;
            if (groupId < 0) return (-groupId) == idnum;

            Privileges grPriv = Privileges.NothingDo;
            if (groupPriv != null && groupPriv.TryGetValue(groupId, out grPriv))
                return (grPriv & priv) != Privileges.NothingDo; //== priv;
            return false;
        }

        /// <summary>
        /// Установить права пользователя на группу
        /// (Добавление и удаление пользователя из группы)
        /// </summary>
        /// <param name="groupId">ИД группы</param>
        /// <param name="priv">Устанавливаемые права, 
        /// лишение права Read равносильно удалению пользователя из группы</param>
        public virtual void SetGroupPrivilegies(int groupId, Privileges priv)
        {
            bool isRemove = (priv & Privileges.Read) == 0;
            if (groupPriv == null) groupPriv = new Dictionary<int, Privileges>();
            Privileges grPriv = Privileges.NothingDo;
            if (groupPriv.TryGetValue(groupId, out grPriv))
            {
                if (grPriv != priv)
                {
                    groupPriv[groupId] = priv;
                    //modified = true;
                }
            }
            else if (!isRemove)
            {
                groupPriv.Add(groupId, priv);
                //modified = true;
            }
        }

        /// <summary>
        /// Пароль или хэш пароля
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// Полное имя пользователя
        /// </summary>
        [DataMember]
        public String UserFullName { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        [DataMember]
        public String Position { get; set; }

        /// <summary>
        /// Является ли пользователь администратором системы
        /// </summary>
        [DataMember]
        public virtual bool IsAdmin { get; set; }

        /// <summary>
        /// Скрыта ли в интерфейсе пользователя структура для данныго пользователя
        /// </summary>
        [DataMember]
        public bool StructureHide { get; set; }

        /// <summary>
        /// Может ли пользователь фиксировать значения
        /// </summary>
        [DataMember]
        public bool CanLockValues { get; set; }

        /// <summary>
        /// Строка в БД с привилегиями пользователя
        /// </summary>
        public string Roles
        {
            get
            {
                StringBuilder res = new StringBuilder();
                if (IsAdmin) res.Append("IsAdmin;");
                if (StructureHide) res.Append("StructureHide;");
                if (CanLockValues) res.Append("CanLockValues;");

                if (userPriv != null)
                    foreach (int key in userPriv.Keys)
                    {
                        if (userPriv[key] != Privileges.NothingDo)
                            res.AppendFormat("{0}={1};", (int)key, (int)userPriv[key]);
                    }
                return res.ToString();
            }
        }

        public UserNode() : base() { }
        public UserNode(DataRow row)
        {
            idnum = (int)row["idnum"];
            Text = row["name"].ToString();

            UserFullName = row["fullname"].ToString();
            Position = row["position"].ToString();
            Password = Encoding.ASCII.GetString((byte[])row["pass"]);
            string[] roles = row["roles"].ToString().Split(new char[] { ';' });
            foreach (string role in roles)
            {
                switch (role)
                {
                    case "IsAdmin": IsAdmin = true; break;
                    case "StructureHide": StructureHide = true; break;
                    case "CanLockValues": CanLockValues = true; break;
                    default:
                        String[] ab = role.Split(new char[] { '=' });
                        int type;
                        Privileges priv;
                        try
                        {
                            type = (int)int.Parse(ab[0]);
                            priv = (Privileges)int.Parse(ab[1]);
                            if (userPriv == null) userPriv = new Dictionary<int, Privileges>();
                            if (!userPriv.ContainsKey(type)) userPriv.Add(type, priv);
                        }
                        catch (FormatException) { }
                        break;
                }
            }
        }

        public override object Clone()
        {
            UserNode res = new UserNode();
            res.idnum = Idnum;
            res.Text = Text;
            res.UserFullName = UserFullName;
            res.Position = Position;
            res.Password = Password;
            res.IsAdmin = IsAdmin;
            res.StructureHide = StructureHide;
            res.CanLockValues = CanLockValues;
            if (userPriv != null)
                res.userPriv = new Dictionary<int, Privileges>(userPriv);
            if (groupPriv != null)
                res.groupPriv = new Dictionary<int, Privileges>(groupPriv);

            return res;
        }
    }

    [Flags]
    [DataContract]
    public enum Privileges
    {
        [EnumMember]
        NothingDo = 0,
        [EnumMember]
        Read = 1,
        [EnumMember]
        Write = 2,
        [EnumMember]
        Execute = 4
    }
}

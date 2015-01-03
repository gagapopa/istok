using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC.Audit
{
    partial class AuditUser : AuditItem
    {
        const String userLoginProperty = "Name";
        const String userGroupsProperty = "UserGroups";
        const String userFullNameProperty = "UserFullName";
        const String userPositionProperty = "UserPosition";
        const String userRoleProperty = "UserRole";
        const String userPasswordProperty = "UserPassword";

        public override IEnumerable<string> AuditProperties
        {
            get
            {
                yield return userLoginProperty;
                yield return userGroupsProperty;
                yield return userFullNameProperty;
                yield return userPositionProperty;
                yield return userRoleProperty;
                yield return userPasswordProperty;
            }
        }

        public override string GetHead(string propertyName)
        {
            switch (propertyName)
            {
                case userLoginProperty: return "Имя";
                case userGroupsProperty: return "Группы";
                case userFullNameProperty: return "Полное имя";
                case userPositionProperty: return "Должность";
                case userRoleProperty: return "Роль";
                case userPasswordProperty: return "Пароль";
                default:
                    return propertyName;
            }
        }

        public override string GetChange(string propertyName)
        {
            switch (propertyName)
            {
                case userLoginProperty:
                    return GetChange(UserLoginOld, UserLoginNew);
                case userGroupsProperty:
                    return GetChange(UserGroupsOld, UserGroupsNew);
                case userFullNameProperty:
                    return GetChange(UserFullNameOld, UserFullNameNew);
                case userPositionProperty:
                    return GetChange(UserPositionOld, UserPositionNew);
                case userRoleProperty:
                    return GetChange(UserRoleOld, UserRoleNew);
                case userPasswordProperty:
                    return (UserPasswordChanged ? "изменён" : String.Empty);
                default:
                    return null;
            }
        }

        private bool IsAdded { get { return UserLoginOld == null; } }

        private bool IsRemoved { get { return UserLoginNew == null; } }

        protected override bool IsUpdate { get { return !(IsAdded || IsRemoved); } }

        public override string ToString()
        {
            const String addedFormat = "Добавлен пользователь";
            const String removedFormat = "Удалён пользователь";
            const String updateFormat = "Изменён пользователь";

            if (IsAdded)
            {
               return addedFormat;
            }
            else if (IsRemoved)
            {
                return removedFormat;
            }
            else
            {
                return updateFormat;
            }
        }
    }
}

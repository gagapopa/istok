using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ASC.Audit;

namespace COTES.ISTOK.Assignment.Audit
{
    class UserManagerAuditDecorator : IUserManager
    {
        IUserManager userManager;
        IAuditServer audit;

        public ISecurityManager Security { get; set; }

        public UserManagerAuditDecorator(IAuditServer auditServer, IUserManager userManager)
        {
            this.userManager = userManager;
            this.audit = auditServer;
        }

        private void AuditUserChange(AuditEntry entry, UserNode oldUser, UserNode newUser, bool passwordChanged)
        {
            String oldGroups = null, newGroups = null;

            if (oldUser != null)
            {
                oldGroups = GetGroupsString(oldUser);
            }
            if (newUser != null)
            {
                newGroups = GetGroupsString(newUser);
            }

            if (oldUser == null
                || newUser == null
                || !String.Equals(oldUser.Text, newUser.Text)
                || !String.Equals(oldUser.UserFullName, newUser.UserFullName)
                || !String.Equals(oldUser.Position, newUser.Position)
                || !String.Equals(oldUser.Roles, newUser.Roles)
                || !String.Equals(oldGroups, newGroups)
                || passwordChanged)
            {
                entry.AuditUsers.Add(new AuditUser()
                {
                    UserID = (newUser ?? oldUser).Idnum,
                    UserLoginOld = (oldUser == null ? null : oldUser.Text),
                    UserLoginNew = (newUser == null ? null : newUser.Text),
                    UserFullNameOld = (oldUser == null ? null : oldUser.UserFullName),
                    UserFullNameNew = (newUser == null ? null : newUser.UserFullName),
                    UserPositionOld = (oldUser == null ? null : oldUser.Position),
                    UserPositionNew = (newUser == null ? null : newUser.Position),
                    UserRoleOld = (oldUser == null ? null : oldUser.Roles),
                    UserRoleNew = (newUser == null ? null : newUser.Roles),
                    UserGroupsOld = oldGroups,
                    UserGroupsNew = newGroups,
                    UserPasswordChanged = passwordChanged,
                });
            }
        }

        private string GetGroupsString(UserNode oldUser)
        {
            String oldGroups;
            StringBuilder groupBulder = new StringBuilder();
            var dict = oldUser.getGroups();

            if (dict != null)
            {
                foreach (var item in dict.Keys)
                {
                    if (groupBulder.Length > 0)
                        groupBulder.Append(";");
                    groupBulder.AppendFormat("{0}={1}",
                        userManager.GetGroup(item).Text,
                        (int)dict[item]);
                }
            }
            oldGroups = groupBulder.ToString();
            return oldGroups;
        }

        private void AuditGroupChange(AuditEntry entry, GroupNode oldGroup, GroupNode newGroup)
        {
            if (oldGroup == null
                || newGroup == null
                || !String.Equals(oldGroup.Text, newGroup.Text)
                || !String.Equals(oldGroup.Description, newGroup.Description))
            {
                entry.AuditGroups.Add(new AuditGroup()
                   {
                       GroupID = (newGroup ?? oldGroup).Idnum,
                       GroupNameOld = (oldGroup == null ? null : oldGroup.Text),
                       GroupNameNew = (newGroup == null ? null : newGroup.Text),
                       GroupDescriptionOld = (oldGroup == null ? null : oldGroup.Description),
                       GroupDescriptionNew = (newGroup == null ? null : newGroup.Description),
                   });
            }
        }

        #region Audited IUserManager Members

        public UserNode AddUserNode(OperationState state, UserNode addnode)
        {
            var userNode = userManager.AddUserNode(state, addnode);

            var entry = new AuditEntry(Security.GetUserInfo(state.UserGUID));

            AuditUserChange(entry, null, userNode, true);

            audit.WriteAuditEntry(entry);

            return userNode;
        }

        public UserNode UpdateUserNode(OperationState state, UserNode updnode)
        {
            var oldUser = userManager.GetUser(updnode.Idnum).Clone() as UserNode;

            bool passwordChanged = !String.IsNullOrEmpty(updnode.Password);

            var entry = new AuditEntry(Security.GetUserInfo(state.UserGUID));

            //UserNode userNode = null;

            var userNode = userManager.UpdateUserNode(state, updnode);

            AuditUserChange(entry, oldUser, userNode, passwordChanged);

            audit.WriteAuditEntry(entry);

            return userNode;
        }

        //private void AuditStuff(AuditEntry entry, UserNode oldUser, UserNode newNode, bool passwordChanged)
        //{
          
        //    AuditUserChange(entry, oldUser, newNode, passwordChanged);

        //    audit.WriteAuditEntry(entry);
        //}

        public void RemoveUserNode(OperationState state, int userNodeID)
        {
            var oldUser = userManager.GetUser(userNodeID).Clone() as UserNode;

            var entry = new AuditEntry(Security.GetUserInfo(state.UserGUID));

            //try
            //{
            userManager.RemoveUserNode(state, userNodeID);
            //}
            //catch (UserNotConnectedException)
            //{
            //    AuditStuff(entry, oldUser, null, false);
            //    throw;
            //}
            //AuditStuff(entry, oldUser, null, false);
            AuditUserChange(entry, oldUser, null, false);

            audit.WriteAuditEntry(entry);
        }

        public void NewAdmin(UserNode userNode)
        {
            userManager.NewAdmin(userNode);

            var user = userManager.GetUser(userNode.Text);

            if (user != null)
            {
                var entry = new AuditEntry(user);

                AuditUserChange(entry, null, user, true);

                audit.WriteAuditEntry(entry);
            }
        }

        public GroupNode AddGroupNode(OperationState state, GroupNode addnode)
        {
            var groupNode = userManager.AddGroupNode(state, addnode);

            var entry = new AuditEntry(Security.GetUserInfo(state.UserGUID));

            AuditGroupChange(entry, null, groupNode);

            audit.WriteAuditEntry(entry);

            return groupNode;
        }

        public void RemoveGroupNode(OperationState state, int groupNodeID)
        {
            var oldGroup = userManager.GetGroup(groupNodeID).Clone() as GroupNode;

            userManager.RemoveGroupNode(state, groupNodeID);

            var entry = new AuditEntry(Security.GetUserInfo(state.UserGUID));

            AuditGroupChange(entry, oldGroup, null);

            audit.WriteAuditEntry(entry);
        }

        public GroupNode UpdateGroupNode(OperationState state, GroupNode updnode)
        {
            var oldGroup = userManager.GetGroup(updnode.Idnum).Clone() as GroupNode;

            var groupNode = userManager.UpdateGroupNode(state, updnode);

            var entry = new AuditEntry(Security.GetUserInfo(state.UserGUID));

            AuditGroupChange(entry, oldGroup, groupNode);

            audit.WriteAuditEntry(entry);

            return groupNode;
        }
        #endregion

        #region Rest IUserManager Members

        public UserNode GetUser(int userID)
        {
            return userManager.GetUser(userID);
        }

        public UserNode GetUser(string userName)
        {
            return userManager.GetUser(userName);
        }

        public UserNode[] GetUserNodes(OperationState state)
        {
            return userManager.GetUserNodes(state);
        }

        public UserNode[] GetUserNodes(OperationState state, int[] ids)
        {
            return userManager.GetUserNodes(state, ids);
        }

        public GroupNode GetGroup(int GID)
        {
            return userManager.GetGroup(GID);
        }

        public GroupNode GetGroup(string groupName)
        {
            return userManager.GetGroup(groupName);
        }

        public GroupNode[] GetGroupNodes(OperationState state)
        {
            return userManager.GetGroupNodes(state);
        }

        #endregion
    }
}

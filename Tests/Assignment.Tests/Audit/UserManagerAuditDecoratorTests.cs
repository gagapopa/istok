using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ASC.Audit;
using COTES.ISTOK.Assignment;
using COTES.ISTOK.Assignment.Audit;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Assignment.Audit
{
    [TestFixture]
    class UserManagerAuditDecoratorTests
    {
        /// <summary>
        /// место куда сохраняется аудит во время тестов
        /// </summary>
        List<AuditEntry> entries;

        /// <summary>
        /// пользователь от чьего имени производятся все действия
        /// </summary>
        UserNode user;

        /// <summary>
        /// Список существующих групп
        /// </summary>
        List<GroupNode> groupList;

        /// <summary>
        /// сервер аудита
        /// </summary>
        Moq.Mock<IAuditServer> auditServerMock;

        /// <summary>
        /// управление безопастностью
        /// </summary>
        Moq.Mock<ISecurityManager> securityMock;

        /// <summary>
        /// декорируемый объект
        /// </summary>
        Moq.Mock<IUserManager> userManagerMock;

        /// <summary>
        /// тестируемый объект
        /// </summary>
        UserManagerAuditDecorator decorator;

        [SetUp]
        public void InitGeneralObjects()
        {
            // место куда сохраняется аудит во время тестов
            entries = new List<AuditEntry>();

            // пользователь от чьего имени производятся все действия
            user = new UserNode()
            {
                Text = "user1",
                UserFullName = "User One",
                Position = "User"
            };

            groupList = new List<GroupNode>();

            // сервер аудита
            auditServerMock = new Moq.Mock<IAuditServer>();
            auditServerMock.Setup(a =>
                a.WriteAuditEntry(Moq.It.IsAny<AuditEntry>()))
                .Callback<AuditEntry>(e => entries.Add(e));

            // управление безопастностью
            securityMock = new Moq.Mock<ISecurityManager>();
            securityMock.Setup(s =>
                s.GetUserInfo(Moq.It.IsAny<Guid>()))
                .Returns(user);

            // декорируемый объект
            userManagerMock = new Moq.Mock<IUserManager>();
            userManagerMock.Setup(m =>
                m.GetGroup(Moq.It.IsAny<int>()))
                .Returns<int>(id =>
                    (from g in groupList
                     where g.Idnum == id
                     select g).First());

            // тестируемый объект
            decorator = new UserManagerAuditDecorator(auditServerMock.Object, userManagerMock.Object);
            decorator.Security = securityMock.Object;
        }

        [Test]
        public void AddUserNode__AuditCorrectData()
        {
            // исходные данные
            var groupNode = new GroupNode()
            {
                Idnum=24,
                Text="group 1"
            };
            groupList.Add(groupNode);

            var state = new OperationState();
            var newNode = new UserNode()
            {
                Text="user 2",
                UserFullName="User Two",
                Position="Director",                
            };
            newNode.SetPrivileges((int)UnitTypeId.Folder, Privileges.Read | Privileges.Write);
            newNode.SetGroupPrivilegies(groupNode.Idnum, Privileges.Read);
            String groups = "group 1=1";

            // декорируемый метод
            userManagerMock.Setup(m => 
                m.AddUserNode(
                    state, 
                    Moq.It.IsAny<UserNode>()))
                .Returns<OperationState, UserNode>((s, u) =>
                {
                    var r = u.Clone() as UserNode;
                    r.Idnum = new Random().Next(1, 16500);
                    return r;
                });

            // вызов тестируемого метода
            var addedNode = decorator.AddUserNode(state, newNode);

            // проверка полученных результатов 
            Assert.IsNotNull(addedNode);
            Assert.AreNotSame(newNode, addedNode);
            Assert.AreNotEqual(0, addedNode.Idnum);

            // проверяем корректность аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.IsFalse(entry.IsEmpty);

            Assert.AreEqual(1, entry.AuditUsers.Count);
            var auditUser = entry.AuditUsers.First();

            Assert.AreEqual(addedNode.Idnum, auditUser.UserID);
            Assert.IsNull(auditUser.UserLoginOld);
            Assert.AreEqual(newNode.Text, auditUser.UserLoginNew);            
            Assert.IsNull(auditUser.UserGroupsOld);
            Assert.AreEqual(groups, auditUser.UserGroupsNew);            
            Assert.IsNull(auditUser.UserFullNameOld);
            Assert.AreEqual(newNode.UserFullName, auditUser.UserFullNameNew);
            Assert.IsNull(auditUser.UserPositionOld);
            Assert.AreEqual(newNode.Position, auditUser.UserPositionNew);
            Assert.IsNull(auditUser.UserRoleOld);
            Assert.AreEqual(newNode.Roles, auditUser.UserRoleNew);
            Assert.IsTrue(auditUser.UserPasswordChanged);
        }

        [Test]
        public void NewAdmin__AuditSelfAdd()
        {  
            // исходные данные
            var groupNode = new GroupNode()
            {
                Idnum = 24,
                Text = "group 1"
            };
            groupList.Add(groupNode);

            //var state = new OperationState();
            int newUserIdnum = 5;
            var newNode = new UserNode()
            {
                Text = "user 2",
                UserFullName = "User Two",
                Position = "Director",
            };
            newNode.SetPrivileges((int)UnitTypeId.Folder, Privileges.Read | Privileges.Write);
            newNode.SetGroupPrivilegies(groupNode.Idnum, Privileges.Read);
            String groups = "group 1=1";

            // декорируемый метод
            //userManagerMock.Setup(m =>
            //    m.NewAdmin(
            //        Moq.It.IsAny<UserNode>()))
            //    .Returns<UserNode>(u =>
            //    {
            //        var r = u.Clone() as UserNode;
            //        r.Idnum = new Random().Next(1, 16500);
            //        return r;
            //    });
            userManagerMock.Setup(m => m.GetUser(newNode.Text)).Returns(newNode);
            userManagerMock.Setup(m => m.NewAdmin(newNode)).Callback(() => newNode.Idnum = newUserIdnum);

            // вызов тестируемого метода
            decorator.NewAdmin(newNode);

            // проверяем корректность аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(newNode.Text, entry.UserLogin);
            Assert.AreEqual(newNode.UserFullName, entry.UserFullName);
            Assert.AreEqual(newNode.Position, entry.UserPosition);

            Assert.IsFalse(entry.IsEmpty);

            Assert.AreEqual(1, entry.AuditUsers.Count);
            var auditUser = entry.AuditUsers.First();

            Assert.AreEqual(newUserIdnum, auditUser.UserID);
            Assert.IsNull(auditUser.UserLoginOld);
            Assert.AreEqual(newNode.Text, auditUser.UserLoginNew);
            Assert.IsNull(auditUser.UserGroupsOld);
            Assert.AreEqual(groups, auditUser.UserGroupsNew);
            Assert.IsNull(auditUser.UserFullNameOld);
            Assert.AreEqual(newNode.UserFullName, auditUser.UserFullNameNew);
            Assert.IsNull(auditUser.UserPositionOld);
            Assert.AreEqual(newNode.Position, auditUser.UserPositionNew);
            Assert.IsNull(auditUser.UserRoleOld);
            Assert.AreEqual(newNode.Roles, auditUser.UserRoleNew);
            Assert.IsTrue(auditUser.UserPasswordChanged);
        }

        [Test]
        public void RemoveUserNode__AuditCorrectData()
        {
            // исходные данные
            var state = new OperationState();
            var groupNode = new GroupNode()
            {
                Idnum = 24,
                Text = "group 1"
            };
            groupList.Add(groupNode);
            
            var userNode = new UserNode()
            {
                Text="user 2",
                UserFullName="User Two",
                Position="Director",                
            };
            userNode.SetPrivileges((int)UnitTypeId.Folder, Privileges.Read | Privileges.Write);
            userNode.SetGroupPrivilegies(groupNode.Idnum, Privileges.Read);
            String groups = "group 1=1";

            // вспомогательный метод декорируемого объекта
            userManagerMock.Setup(m => m.GetUser(Moq.It.IsAny<int>())).Returns(userNode);

            // вызов тестируемого метода
            decorator.RemoveUserNode(state, userNode.Idnum);

            // проверка правильности аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.IsFalse(entry.IsEmpty);

            Assert.AreEqual(1, entry.AuditUsers.Count);
            var auditUser = entry.AuditUsers.First();

            Assert.AreEqual(userNode.Idnum, auditUser.UserID);
            Assert.AreEqual(userNode.Text, auditUser.UserLoginOld);
            Assert.IsNull(auditUser.UserLoginNew);
            Assert.AreEqual(groups, auditUser.UserGroupsOld);
            Assert.IsNull(auditUser.UserGroupsNew);
            Assert.AreEqual(userNode.UserFullName, auditUser.UserFullNameOld);
            Assert.IsNull(auditUser.UserFullNameNew);
            Assert.AreEqual(userNode.Position, auditUser.UserPositionOld);
            Assert.IsNull(auditUser.UserPositionNew);
            Assert.AreEqual(userNode.Roles, auditUser.UserRoleOld);
            Assert.IsNull(auditUser.UserRoleNew);
            Assert.IsFalse(auditUser.UserPasswordChanged);
        }

        [Test]
        public void UpdateUserNode_NoChanges_AuditNothing()
        {
            // исходные данные
            var groupNode = new GroupNode()
            {
                Idnum = 24,
                Text = "group 1"
            };
            groupList.Add(groupNode);

            var state = new OperationState();
            var oldNode = new UserNode()
            {
                Text = "user 2",
                UserFullName = "User Two",
                Position = "Director",
            };
            oldNode.SetPrivileges((int)UnitTypeId.Folder, Privileges.Read | Privileges.Write);
            oldNode.SetGroupPrivilegies(groupNode.Idnum, Privileges.Read);
            //String groups = "group 1=1";
            var newNode = oldNode.Clone() as UserNode;

            // декорируемый метод
            userManagerMock.Setup(m => 
                m.UpdateUserNode(
                    state, 
                    Moq.It.IsAny<UserNode>()))
                .Returns<OperationState, UserNode>((s, u) =>
                    u.Clone() as UserNode);

            // вспомогательный метод 
            userManagerMock.Setup(m => 
                m.GetUser(oldNode.Idnum))
                .Returns(oldNode);

            // вызов тестируемого метода
            var userNode = decorator.UpdateUserNode(state, newNode);

            // проверка полученных результатов
            Assert.IsNotNull(userNode);
            Assert.AreNotSame(oldNode, userNode);
            Assert.AreNotSame(newNode, userNode);

            // проверка правильности аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.IsTrue(entry.IsEmpty);
        }

        [Test]
        public void UpdateUserNode_SomeChanges_AuditCorrectData()
        {
            // исходные данные
            var groupNode = new GroupNode()
            {
                Idnum = 24,
                Text = "group 1"
            };
            groupList.Add(groupNode);

            var state = new OperationState();
            var oldNode = new UserNode()
            {
                Text = "user 2",
                UserFullName = "User Two",
                Position = "Director",
            };
            oldNode.SetPrivileges((int)UnitTypeId.Folder, Privileges.Read | Privileges.Write);
            oldNode.SetGroupPrivilegies(groupNode.Idnum, Privileges.Read);
            String oldGroups = "group 1=1";
            var newNode = oldNode.Clone() as UserNode;
            newNode.SetGroupPrivilegies(groupNode.Idnum, Privileges.Write);
            String newGroups = "group 1=2";

            // декорируемый метод
            userManagerMock.Setup(m =>
                m.UpdateUserNode(
                    state,
                    Moq.It.IsAny<UserNode>()))
                .Returns<OperationState, UserNode>((s, u) =>
                    u.Clone() as UserNode);

            // вспомогательный метод 
            userManagerMock.Setup(m =>
                m.GetUser(oldNode.Idnum))
                .Returns(oldNode);

            // вызов тестируемого метода
            var userNode = decorator.UpdateUserNode(state, newNode);

            // проверка полученных результатов
            Assert.IsNotNull(userNode);
            Assert.AreNotSame(oldNode, userNode);
            Assert.AreNotSame(newNode, userNode);

            // проверка правильности аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.IsFalse(entry.IsEmpty);

            Assert.AreEqual(1, entry.AuditUsers.Count);
            var auditUser = entry.AuditUsers.First();

            Assert.AreEqual(userNode.Idnum, auditUser.UserID);
            Assert.AreEqual(oldNode.Text, auditUser.UserLoginOld);
            Assert.AreEqual(newNode.Text, auditUser.UserLoginNew);
            Assert.AreEqual(oldGroups, auditUser.UserGroupsOld);
            Assert.AreEqual(newGroups, auditUser.UserGroupsNew);
            Assert.AreEqual(oldNode.UserFullName, auditUser.UserFullNameOld);
            Assert.AreEqual(newNode.UserFullName, auditUser.UserFullNameNew);
            Assert.AreEqual(oldNode.Position, auditUser.UserPositionOld);
            Assert.AreEqual(newNode.Position, auditUser.UserPositionNew);
            Assert.AreEqual(oldNode.Roles, auditUser.UserRoleOld);
            Assert.AreEqual(newNode.Roles, auditUser.UserRoleNew);
            Assert.IsFalse(auditUser.UserPasswordChanged);
        }

        [Test]
        public void UpdateUserNode_PasswordChangedOnly_AuditCorrectData()
        {
            // исходные данные
            var groupNode = new GroupNode()
            {
                Idnum = 24,
                Text = "group 1"
            };
            groupList.Add(groupNode);

            var state = new OperationState();
            var oldNode = new UserNode()
            {
                Text = "user 2",
                UserFullName = "User Two",
                Position = "Director",
            };
            oldNode.SetPrivileges((int)UnitTypeId.Folder, Privileges.Read | Privileges.Write);
            oldNode.SetGroupPrivilegies(groupNode.Idnum, Privileges.Read);
            String groups = "group 1=1";
            var newNode = oldNode.Clone() as UserNode;
            newNode.Password = "newPassword";

            // декорируемый метод
            userManagerMock.Setup(m =>
                m.UpdateUserNode(
                    state,
                    Moq.It.IsAny<UserNode>()))
                .Returns<OperationState, UserNode>((s, u) =>
                    u.Clone() as UserNode);

            // вспомогательный метод 
            userManagerMock.Setup(m =>
                m.GetUser(oldNode.Idnum))
                .Returns(oldNode);

            // вызов тестируемого метода
            var userNode = decorator.UpdateUserNode(state, newNode);

            // проверка полученных результатов
            Assert.IsNotNull(userNode);
            Assert.AreNotSame(oldNode, userNode);
            Assert.AreNotSame(newNode, userNode);

            // проверка правильности аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.IsFalse(entry.IsEmpty);

            Assert.AreEqual(1, entry.AuditUsers.Count);
            var auditUser = entry.AuditUsers.First();

            Assert.AreEqual(userNode.Idnum, auditUser.UserID);
            Assert.AreEqual(oldNode.Text, auditUser.UserLoginOld);
            Assert.AreEqual(newNode.Text, auditUser.UserLoginNew);
            Assert.AreEqual(groups, auditUser.UserGroupsOld);
            Assert.AreEqual(groups, auditUser.UserGroupsNew);
            Assert.AreEqual(oldNode.UserFullName, auditUser.UserFullNameOld);
            Assert.AreEqual(newNode.UserFullName, auditUser.UserFullNameNew);
            Assert.AreEqual(oldNode.Position, auditUser.UserPositionOld);
            Assert.AreEqual(newNode.Position, auditUser.UserPositionNew);
            Assert.AreEqual(oldNode.Roles, auditUser.UserRoleOld);
            Assert.AreEqual(newNode.Roles, auditUser.UserRoleNew);
            Assert.IsTrue(auditUser.UserPasswordChanged);
        }

        [Test]
        public void AddGroupNode__AuditCorrectData()
        {
            // исходные данные
            var state = new OperationState();
            var newNode = new GroupNode()
            {
                Text = "group 1",
                Description="Group One"
            };

            // декорируемый метод
            userManagerMock.Setup(m =>
                m.AddGroupNode(
                    state,
                    Moq.It.IsAny<GroupNode>()))
                .Returns<OperationState, GroupNode>((s, g) =>
                {
                    var r = g.Clone() as GroupNode;
                    r.Idnum = new Random().Next(1, 16500);
                    return r;
                });

            // вызов тестируемого метода
            var addedNode = decorator.AddGroupNode(state, newNode);

            // проверка полученных результатов 
            Assert.IsNotNull(addedNode);
            Assert.AreNotSame(newNode, addedNode);
            Assert.AreNotEqual(0, addedNode.Idnum);

            // проверяем корректность аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.IsFalse(entry.IsEmpty);

            Assert.AreEqual(1, entry.AuditGroups.Count);
            var auditGroup = entry.AuditGroups.First();

            Assert.AreEqual(addedNode.Idnum, auditGroup.GroupID);
            Assert.IsNull(auditGroup.GroupNameOld);
            Assert.AreEqual(newNode.Text, auditGroup.GroupNameNew);
            Assert.IsNull(auditGroup.GroupDescriptionOld);
            Assert.AreEqual(newNode.Description, auditGroup.GroupDescriptionNew);
        }

        [Test]
        public void RemoveGroupNode__AuditCorrectData()
        {
            // исходные данные
            var state = new OperationState();

            var groupNode = new GroupNode()
            {
                Idnum = 24,
                Text = "group 1",
                Description = "Group One"
            };
            groupList.Add(groupNode);

            // вызов тестируемого метода
            decorator.RemoveGroupNode(state, groupNode.Idnum);

            // проверка правильности аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.IsFalse(entry.IsEmpty);

            Assert.AreEqual(1, entry.AuditGroups.Count);
            var auditGroup = entry.AuditGroups.First();

            Assert.AreEqual(groupNode.Idnum, auditGroup.GroupID);
            Assert.AreEqual(groupNode.Text, auditGroup.GroupNameOld);
            Assert.IsNull(auditGroup.GroupNameNew);
            Assert.AreEqual(groupNode.Description, auditGroup.GroupDescriptionOld);
            Assert.IsNull(auditGroup.GroupDescriptionNew);
        }

        [Test]
        public void UpdateGroupNode_NoChanges_AuditNothing()
        {
            // исходные данные
            var state = new OperationState();

            var oldNode = new GroupNode()
            {
                Idnum = 24,
                Text = "group 1",
                Description = "Group One"
            };
            var newNode = oldNode.Clone() as GroupNode;
            groupList.Add(oldNode);

            // декорируемый метода
            userManagerMock.Setup(m => 
                m.UpdateGroupNode(
                    state,
                    Moq.It.IsAny<GroupNode>()))
                .Returns<OperationState, GroupNode>((s, g) => 
                    g.Clone() as GroupNode);

            // вызов тестируемого метода
            var groupNode = decorator.UpdateGroupNode(state, newNode);

            // проверка правильности аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.IsTrue(entry.IsEmpty);
        }

        [Test]
        public void UpdateGroupNode_SomeChanges_AuditCorrectData()
        {
            // исходные данные
            var state = new OperationState();

            var oldNode = new GroupNode()
            {
                Idnum = 24,
                Text = "group 1",
                Description = "Group One"
            };
            var newNode = oldNode.Clone() as GroupNode;
            newNode.Text = "Group 1";
            groupList.Add(oldNode);

            // декорируемый метода
            userManagerMock.Setup(m =>
                m.UpdateGroupNode(
                    state,
                    Moq.It.IsAny<GroupNode>()))
                .Returns<OperationState, GroupNode>((s, g) =>
                    g.Clone() as GroupNode);

            // вызов тестируемого метода
            var groupNode = decorator.UpdateGroupNode(state, newNode);

            // проверка правильности аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.IsFalse(entry.IsEmpty);

            Assert.AreEqual(1, entry.AuditGroups.Count);
            var auditGroup = entry.AuditGroups.First();

            Assert.AreEqual(groupNode.Idnum, auditGroup.GroupID);
            Assert.AreEqual(oldNode.Text, auditGroup.GroupNameOld);
            Assert.AreEqual(newNode.Text, auditGroup.GroupNameNew);
            Assert.AreEqual(oldNode.Description, auditGroup.GroupDescriptionOld);
            Assert.AreEqual(newNode.Description, auditGroup.GroupDescriptionNew);
        }
    }
}

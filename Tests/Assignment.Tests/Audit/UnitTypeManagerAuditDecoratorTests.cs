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
    class UnitTypeManagerAuditDecoratorTests
    {
        /// <summary>
        /// сервер аудита
        /// </summary>
        Moq.Mock<IAuditServer> auditServerMock;

        /// <summary>
        /// декорируемый объект
        /// </summary>
        Moq.Mock<IUnitTypeManager> typeManagerMock;

        /// <summary>
        /// проверка безопастности
        /// </summary>
        Moq.Mock<ISecurityManager> securityMock;

        /// <summary>
        /// пользователь от чьего имени производятся все действия
        /// </summary>
        UserNode user;

        /// <summary>
        /// суда осуществляется запись аудита во время теста
        /// </summary>
        List<AuditEntry> entries;

        /// <summary>
        /// тестируемый объект
        /// </summary>
        UnitTypeManagerAuditDecorator decorator;

        [SetUp]
        public void InitGeneralObjects()
        {
            // сервер аудита
            auditServerMock = new Moq.Mock<IAuditServer>();

            // декорируемый объект
            typeManagerMock = new Moq.Mock<IUnitTypeManager>();

            // проверка безопастности
            securityMock = new Moq.Mock<ISecurityManager>();

            // пользователь от чьего имени производятся все действия
            user = new UserNode()
            {
                Text = "user1",
                UserFullName = "User One",
                Position = "User"
            };
            securityMock.Setup(s => 
                s.GetUserInfo(Moq.It.IsAny<Guid>()))
                .Returns(user);

            // суда осуществляется запись аудита во время теста
            entries = new List<AuditEntry>();

            // запись аудита в entries
            auditServerMock.Setup(a =>
                a.WriteAuditEntry(Moq.It.IsAny<AuditEntry>()))
                .Callback<AuditEntry>(e => entries.Add(e));

            // тестируемый объект
            decorator = new UnitTypeManagerAuditDecorator(auditServerMock.Object, typeManagerMock.Object);
            decorator.Security = securityMock.Object;
        }

        [Test]
        public void AddUnitType__AuditCorrectData()
        {
            // исходные данные для тестируемого метода
            var state = new OperationState();
            var typeNode = new UTypeNode()
            {
                Text = "Тип1",
                ExtensionGUID = Guid.NewGuid(),
                Props = "interval;speed",
                Icon = new byte[] { 121, 122, 123, 134, 64, 124 },
                ChildFilter = new List<int>(new int[] { 1, 4, 6, 3 }),
            };

            // декорируемый метод
            typeManagerMock.Setup(m =>
                m.AddUnitType(
                    state,
                    Moq.It.IsAny<UTypeNode>()))
                .Returns<OperationState, UTypeNode>((s, t) =>
                {
                    var r = t.Clone() as UTypeNode;
                    r.Idnum = new Random().Next(1, 16500);
                    return r;
                });

            // вызываем тестируемый метод
            var type = decorator.AddUnitType(state, typeNode);

            // проверяем результат операции
            Assert.IsNotNull(type);
            Assert.AreNotSame(typeNode, type);

            // проверяем корректность аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.AreEqual(1, entry.AuditTypes.Count);
            var auditType = entry.AuditTypes.First();

            Assert.AreEqual(type.Idnum, auditType.TypeID);
            Assert.AreEqual(typeNode.ExtensionGUID, auditType.ExtGuid);
            Assert.IsNull(auditType.TypeNameOld);
            Assert.AreEqual(typeNode.Text, auditType.TypeNameNew);
            Assert.IsNull(auditType.TypePropsOld);
            Assert.AreEqual(typeNode.Props, auditType.TypePropsNew);
            Assert.IsNull(auditType.TypeImageOld);
            Assert.AreEqual(typeNode.Icon, auditType.TypeImageNew);
            Assert.IsNull(auditType.TypeChildFilterOld);
            Assert.AreEqual(typeNode.Filter, auditType.TypeChildFilterNew);
        }

        [Test]
        public void RemoveUnitType__AuditCorrectData()
        {
            // исходные данные для тестируемого метода
            var state = new OperationState();
            int unitTypeNodeID = 4;

            // узел перед удалением
            var typeNode = new UTypeNode()
            {
                Idnum = 4,
                Text = "Тип1",
                ExtensionGUID = Guid.NewGuid(),
                Props = "interval;speed",
                Icon = new byte[] { 121, 122, 123, 134, 64, 124 },
                ChildFilter = new List<int>(new int[] { 1, 4, 6, 3 }),
            };

            // декорируемый метод можно не перегружать

            // требуемый вспомогательный метод
            typeManagerMock.Setup(m => 
                m.GetUnitType(state, Moq.It.IsAny<int>()))
                .Returns(typeNode);

            // вызов тестируемого метода
            decorator.RemoveUnitType(state, unitTypeNodeID);

            // проверяем корректность аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.AreEqual(1, entry.AuditTypes.Count);
            var auditType = entry.AuditTypes.First();

            Assert.AreEqual(typeNode.Idnum, auditType.TypeID);
            Assert.AreEqual(typeNode.ExtensionGUID, auditType.ExtGuid);
            Assert.AreEqual(typeNode.Text, auditType.TypeNameOld);
            Assert.IsNull(auditType.TypeNameNew);
            Assert.AreEqual(typeNode.Props, auditType.TypePropsOld);
            Assert.IsNull(auditType.TypePropsNew);
            Assert.AreEqual(typeNode.Icon, auditType.TypeImageOld);
            Assert.IsNull(auditType.TypeImageNew);
            Assert.AreEqual(typeNode.Filter, auditType.TypeChildFilterOld);
            Assert.IsNull(auditType.TypeChildFilterNew);
        }

        [Test]
        public void UpdateUnitType_NoChanges_AuditNothing()
        {
            // исходные данные для тестируемого метода
            var state = new OperationState();

            var oldNode = new UTypeNode()
            {
                Idnum = 4,
                Text = "Тип1",
                ExtensionGUID = Guid.NewGuid(),
                Props = "interval;speed",
                Icon = new byte[] { 121, 122, 123, 134, 64, 124 },
                ChildFilter = new List<int>(new int[] { 1, 4, 6, 3 }),
            };
            var newNode = oldNode.Clone() as UTypeNode;

            // вспомогательный метод
            typeManagerMock.Setup(m => 
                m.GetUnitType(state, Moq.It.IsAny<int>()))
                .Returns(oldNode);

            // декорируемый метод
            typeManagerMock.Setup(m => 
                m.UpdateUnitType(
                    state, 
                    Moq.It.IsAny<UTypeNode>()))
                .Returns<OperationState, UTypeNode>((s, t) => 
                    t.Clone() as UTypeNode);

            // вызов тестируемого метода
            var type = decorator.UpdateUnitType(state, newNode);

            // проверяем результат операции
            Assert.IsNotNull(type);
            Assert.AreNotSame(oldNode, type);
            Assert.AreNotSame(newNode, type);

            // проверяем корректность аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.IsTrue(entry.IsEmpty);
        }

        [Test]
        public void UpdateUnitType_SomeChanges_AuditCorrectData()
        {
            // исходные данные для тестируемого метода
            var state = new OperationState();

            var oldNode = new UTypeNode()
            {
                Idnum = 4,
                Text = "Тип1",
                ExtensionGUID = Guid.NewGuid(),
                Props = "interval;speed",
                Icon = new byte[] { 121, 122, 123, 134, 64, 124 },
                ChildFilter = new List<int>(new int[] { 1, 4, 6, 3 }),
            };
            var newNode = oldNode.Clone() as UTypeNode;
            newNode.Props = "speed;interval";

            // вспомогательный метод
            typeManagerMock.Setup(m =>
                m.GetUnitType(state, Moq.It.IsAny<int>()))
                .Returns(oldNode);

            // декорируемый метод
            typeManagerMock.Setup(m =>
                m.UpdateUnitType(
                    state,
                    Moq.It.IsAny<UTypeNode>()))
                .Returns<OperationState, UTypeNode>((s, t) =>
                    t.Clone() as UTypeNode);

            // вызов тестируемого метода
            var type = decorator.UpdateUnitType(state, newNode);

            // проверяем результат операции
            Assert.IsNotNull(type);
            Assert.AreNotSame(oldNode, type);
            Assert.AreNotSame(newNode, type);

            // проверяем корректность аудита
            Assert.AreEqual(1, entries.Count);
            var entry = entries[0];
            Assert.IsNotNull(entry);
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.AreEqual(1, entry.AuditTypes.Count);
            var auditType = entry.AuditTypes.First();

            Assert.AreEqual(oldNode.Idnum, auditType.TypeID);
            Assert.AreEqual(oldNode.ExtensionGUID, auditType.ExtGuid);

            Assert.AreEqual(oldNode.Text, auditType.TypeNameOld);
            Assert.AreEqual(newNode.Text, auditType.TypeNameNew);

            Assert.AreEqual(oldNode.Props, auditType.TypePropsOld);
            Assert.AreEqual(newNode.Props, auditType.TypePropsNew);

            Assert.AreEqual(oldNode.Icon, auditType.TypeImageOld);
            Assert.AreEqual(newNode.Icon, auditType.TypeImageNew);

            Assert.AreEqual(oldNode.Filter, auditType.TypeChildFilterOld);
            Assert.AreEqual(newNode.Filter, auditType.TypeChildFilterNew);
        }
    }
}

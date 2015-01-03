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
    public class UnitManagerAuditDecoratorTests
    {
        Moq.Mock<IUnitManager> unitManager;
        Moq.Mock<ISecurityManager> security;
        Moq.Mock<IAuditServer> audit;
        Moq.Mock<IUnitTypeManager> types;

        /// <summary>
        /// Тестируемы объект
        /// </summary>
        UnitManagerAuditDecorator decorator;

        /// <summary>
        /// Записи аудита полученные во время теста
        /// </summary>
        List<AuditEntry> entries;

        /// <summary>
        /// Пользователь от чего имени производятся все операции во время тестов
        /// </summary>
        UserNode user;

        /// <summary>
        /// все доступные во время теста типы
        /// </summary>
        List<UTypeNode> allowUnitTypes;

        /// <summary>
        /// Все доступные во время теста узлы
        /// </summary>
        List<UnitNode> nodes;

        [SetUp]
        public void InitDependences()
        {
            unitManager = new Moq.Mock<IUnitManager>();
            security = new Moq.Mock<ISecurityManager>();
            audit = new Moq.Mock<IAuditServer>();
            types = new Moq.Mock<IUnitTypeManager>();

            decorator = new UnitManagerAuditDecorator(audit.Object, unitManager.Object);
            //decorator.Audit = audit.Object;
            decorator.Security = security.Object;
            decorator.UnitTypes = types.Object;

            // Перехват записи аудита
            entries = new List<AuditEntry>();
            audit.Setup(a =>
                a.WriteAuditEntry(
                    Moq.It.IsAny<AuditEntry>()))
                .Callback<AuditEntry>(e => entries.Add(e));

            // запрос пользователя для аудита
            user = new UserNode()
            {
                Text = "user1",
                UserFullName = "User One",
                Position = "User"
            };
            security.Setup(s => s.GetUserInfo(Moq.It.IsAny<Guid>())).Returns(user);

            // запрос типа узла
            allowUnitTypes = new List<UTypeNode>(new UTypeNode[]
             {
                 new UTypeNode()
                 {
                     Idnum = (int)UnitTypeId.Folder,
                     Text = "Folder"
                 }
             });
            types.Setup(t =>
                t.GetUnitType(
                    Moq.It.IsAny<OperationState>(),
                    Moq.It.IsAny<int>()))
                .Returns<OperationState, int>((s, id) =>
                    (from t in allowUnitTypes
                     where t.Idnum == (int)id
                     select t).First());

            // Запрос узлов по ИД из декорируемого метода
            nodes = new List<UnitNode>();
            unitManager.Setup(m =>
                m.ValidateUnitNode<UnitNode>(
                    Moq.It.IsAny<OperationState>(),
                    Moq.It.IsAny<int>(),
                    Moq.It.IsAny<Privileges>()))
                .Returns<OperationState, int, Privileges>((s, id, p) =>
                    (from n in nodes
                     where n.Idnum == id
                     select n).First());
        }

        [Test]
        public void AddUnitNode__AuditNewNodeAndNewProperties()
        {
            // настройка вызова декорируемого метода
            unitManager.Setup(m =>
                m.AddUnitNode(
                    Moq.It.IsAny<OperationState>(),
                    Moq.It.IsAny<IEnumerable<UnitNode>>(),
                    Moq.It.IsAny<int>()))
                .Returns<OperationState, IEnumerable<UnitNode>, int>((s, n, p) => n);

            // аргументы для вызова метода
            OperationState state = new OperationState();
            UnitNode node = new UnitNode()
                {
                    Idnum = 10,
                    Typ = (int)UnitTypeId.Folder,
                    Text = "Папка 1",
                    FullName = "/Станция/Папка 1"
                };
            node.DocIndex = "1.1";

            nodes.Add(node);
            int parentID = 1;

            var typeNode = allowUnitTypes.Find(t => t.Idnum == (int)UnitTypeId.Folder);

            // тестируемый метод
            var newNodes = decorator.AddUnitNode(state, nodes, parentID);

            // рещультат операции вернулся без изменений
            Assert.IsNotNull(newNodes);
            Assert.AreEqual(nodes.Count(), newNodes.Count());
            for (int i = 0; i < nodes.Count(); i++)
            {
                Assert.AreSame(nodes.ElementAt(i), newNodes.ElementAt(i));
            }

            // запись в аудит только одна
            Assert.AreEqual(1, entries.Count);

            // проверка записи пользователя
            var entry = entries[0];
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            // изменился 1 узел
            Assert.AreEqual(1, entry.AuditUnits.Count());
            var auditUnit = entry.AuditUnits.First();
            Assert.AreEqual(node.Idnum, auditUnit.UnitNodeID);
            Assert.IsNull(auditUnit.FullPathOld);
            Assert.AreEqual(node.FullName, auditUnit.FullPathNew);
            Assert.IsNull(auditUnit.TypeNameOld);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameNew);

            // изменилось 1 свойства
            Assert.AreEqual(1, entry.AuditProps.Count());
            var auditProp = entry.AuditProps.First();

            Assert.AreEqual(node.Idnum, auditProp.UnitNodeID);
            Assert.AreEqual(node.FullName, auditProp.UnitNodeFullPath);
            Assert.AreEqual("index", auditProp.PropName);
            Assert.IsNull(auditProp.RevisionID);
            Assert.IsNull(auditProp.RevisionTime);
            Assert.IsNull(auditProp.RevisionBrief);
            Assert.IsNull(auditProp.ValueOld);
            Assert.AreEqual("1.1", auditProp.ValueNew);

            // изменилось 0 lobs
            Assert.AreEqual(0, entry.AuditLobs.Count());
        }

        [Test]
        public void RemoveUnitNode__AuditRemoveNode()
        {
            // аргументы для вызова метода
            OperationState state = new OperationState();
            UnitNode node1 = new UnitNode()
            {
                Idnum = 10,
                Typ = (int)UnitTypeId.Folder,
                Text = "Папка 1",
                FullName = "/Станция/Папка 1"
            };
            node1.DocIndex = "1.1";
            UnitNode node2 = new UnitNode()
            {
                Idnum = 11,
                Typ = (int)UnitTypeId.Folder,
                Text = "Папка 2",
                FullName = "/Станция/Папка 2"
            };
            node2.DocIndex = "1.1";
            UnitNode node3 = new UnitNode()
            {
                Idnum = 12,
                Typ = (int)UnitTypeId.Folder,
                Text = "Папка 3",
                FullName = "/Станция/Папка 2/Папка 3"
            };
            node2.DocIndex = "1.3";

            nodes.Add(node1);
            nodes.Add(node2);
            nodes.Add(node3);

            var ids = new int[] { node1.Idnum, node2.Idnum };

            // декорируемый метод
            // вместо 2 переданных ид удаляем 3 нода
            unitManager.Setup(m => 
                m.RemoveUnitNode(Moq.It.IsAny<OperationState>(), ids))
                .Callback<OperationState, int[]>((s, idArray) => 
                {
                    s.AddAsyncResult(node1);
                    s.AddAsyncResult(node2);
                    s.AddAsyncResult(node3);
                });

            // тестируемый метод
            decorator.RemoveUnitNode(state, ids);

            // проверяемы вызов декорируемого метода
            unitManager.Verify(m => m.RemoveUnitNode(state, ids));

            // запись в аудит только одна
            Assert.AreEqual(1, entries.Count);

            // проверка записи пользователя
            var entry = entries[0];
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            var typeNode = allowUnitTypes.Find(t => t.Idnum == (int)UnitTypeId.Folder);

            // изменилось 3 узла
            Assert.AreEqual(3, entry.AuditUnits.Count());
            // первый узел
            var auditUnit = entry.AuditUnits.First();
            Assert.AreEqual(node1.Idnum, auditUnit.UnitNodeID);
            Assert.AreEqual(node1.FullName, auditUnit.FullPathOld);
            Assert.IsNull(auditUnit.FullPathNew);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameOld);
            Assert.IsNull(auditUnit.FullPathNew);
            // второй узел
            auditUnit = entry.AuditUnits.ElementAt(1);
            Assert.AreEqual(node2.Idnum, auditUnit.UnitNodeID);
            Assert.AreEqual(node2.FullName, auditUnit.FullPathOld);
            Assert.IsNull(auditUnit.FullPathNew);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameOld);
            Assert.IsNull(auditUnit.TypeNameNew);
            // третий узел
            auditUnit = entry.AuditUnits.ElementAt(2);
            Assert.AreEqual(node3.Idnum, auditUnit.UnitNodeID);
            Assert.AreEqual(node3.FullName, auditUnit.FullPathOld);
            Assert.IsNull(auditUnit.FullPathNew);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameOld);
            Assert.IsNull(auditUnit.TypeNameNew);

            // изменилось 0 свойства
            Assert.AreEqual(0, entry.AuditProps.Count());
            Assert.AreEqual(0, entry.AuditLobs.Count());
        }

        [Test]
        public void UpdateUnitNode_ChangedNodeNameOnly_AuditNode()
        {
            // декорируемый метод
            unitManager.Setup(m =>
                m.UpdateUnitNode(
                    Moq.It.IsAny<OperationState>(),
                    Moq.It.IsAny<UnitNode>()))
                .Returns<OperationState, UnitNode>((s, n) => n);

            // передоваемые аргументы
            var state = new OperationState();

            UnitNode originalNode = new UnitNode()
            {
                Idnum = 11,
                Typ = (int)UnitTypeId.Folder,
                Text = "Before",
                FullName = "/Station/Before",
            };
            originalNode.SetAttribute("index", "21");
            originalNode.SetBinaries("buffer", new byte[] { 21, 44, 56 });
            nodes.Add(originalNode);

            var updnode = new UnitNode()
            {
                Idnum = originalNode.Idnum,
                Typ = (int)UnitTypeId.Folder,
                Text = "After",
                FullName = "/Station/After"
            };
            updnode.SetAttribute("index", "21");
            updnode.SetBinaries("buffer", new byte[] { 21, 44, 56 });

            // тестируемый метод
            var node = decorator.UpdateUnitNode(state, updnode);

            Assert.AreSame(updnode, node);

            // запись в аудит только одна
            Assert.AreEqual(1, entries.Count);

            // проверка записи пользователя
            var entry = entries[0];
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            // тип узла
            var typeNode = allowUnitTypes.Find(t => t.Idnum == (int)UnitTypeId.Folder);

            // изменился 1 узел
            Assert.AreEqual(1, entry.AuditUnits.Count());
            var auditUnit = entry.AuditUnits.First();
            Assert.AreEqual(node.Idnum, auditUnit.UnitNodeID);
            Assert.AreEqual(originalNode.FullName, auditUnit.FullPathOld);
            Assert.AreEqual(node.FullName, auditUnit.FullPathNew);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameOld);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameNew);

            // изменилось 0 свойства
            Assert.AreEqual(0, entry.AuditProps.Count());
            Assert.AreEqual(0, entry.AuditLobs.Count());
        }

        [Test]
        public void UpdateUnitNode_ChangedPropertyValueOnly_AuditProperties()
        {
            // декорируемый метод
            unitManager.Setup(m =>
                m.UpdateUnitNode(
                    Moq.It.IsAny<OperationState>(),
                    Moq.It.IsAny<UnitNode>()))
                .Returns<OperationState, UnitNode>((s, n) => n);

            // передаваемые аргументы
            var state = new OperationState();

            UnitNode originalNode = new UnitNode()
            {
                Idnum = 11,
                Typ = (int)UnitTypeId.Folder,
                Text = "Before",
                FullName = "/Station/Before",
            };
            originalNode.SetAttribute("index", "21");
            originalNode.SetBinaries("buffer", new byte[] { 21, 44, 56 });
            nodes.Add(originalNode);

            var updnode = new UnitNode()
            {
                Idnum = originalNode.Idnum,
                Typ = (int)UnitTypeId.Folder,
                Text = "Before",
                FullName = "/Station/Before"
            };
            updnode.SetAttribute("index", "22");
            updnode.SetAttribute("formula_code", "System");
            updnode.SetBinaries("buffer", new byte[] { 21, 36, 56 });

            // тестируемый метод
            var node = decorator.UpdateUnitNode(state, updnode);

            Assert.AreSame(updnode, node);

            // запись в аудит только одна
            Assert.AreEqual(1, entries.Count);

            // проверка записи пользователя
            var entry = entries[0];
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            // тип узла
            var typeNode = allowUnitTypes.Find(t => t.Idnum == (int)UnitTypeId.Folder);

            // изменилось 0 узлов
            Assert.AreEqual(0, entry.AuditUnits.Count());

            // изменилось 2 свойства
            Assert.AreEqual(2, entry.AuditProps.Count());
            // первое свойства index
            var auditProp = entry.AuditProps.First();
            Assert.AreEqual(node.Idnum, auditProp.UnitNodeID);
            Assert.AreEqual(node.FullName, auditProp.UnitNodeFullPath);
            Assert.AreEqual("index", auditProp.PropName);
            Assert.IsNull(auditProp.RevisionID);
            Assert.IsNull(auditProp.RevisionTime);
            Assert.IsNull(auditProp.RevisionBrief);
            Assert.AreEqual("21", auditProp.ValueOld);
            Assert.AreEqual("22", auditProp.ValueNew);
            // второе свойствл formula_code
            auditProp = entry.AuditProps.ElementAt(1);
            Assert.AreEqual(node.Idnum, auditProp.UnitNodeID);
            Assert.AreEqual(node.FullName, auditProp.UnitNodeFullPath);
            Assert.AreEqual("formula_code", auditProp.PropName);
            Assert.IsNull(auditProp.RevisionID);
            Assert.IsNull(auditProp.RevisionTime);
            Assert.IsNull(auditProp.RevisionBrief);
            Assert.IsNull(auditProp.ValueOld);
            Assert.AreEqual("System", auditProp.ValueNew);

            // изменилось 1 длинное свойство свойство
            Assert.AreEqual(1, entry.AuditLobs.Count());
            var auditLob = entry.AuditLobs.First();
            Assert.AreEqual(node.Idnum, auditLob.UnitNodeID);
            Assert.AreEqual(node.FullName, auditLob.UnitNodeFullPath);
            Assert.AreEqual("buffer", auditLob.PropName);
            Assert.IsNull(auditLob.RevisionID);
            Assert.IsNull(auditLob.RevisionTime);
            Assert.IsNull(auditLob.RevisionBrief);
            Assert.AreEqual(new byte[] { 21, 44, 56 }, auditLob.ValueOld);
            Assert.AreEqual(new byte[] { 21, 36, 56 }, auditLob.ValueNew);
        }

        [Test]
        public void UpdateUnitNode_ChangedPropertyAndNodeType_AuditEveryThing()
        {
            // декорируемый метод
            unitManager.Setup(m =>
                m.UpdateUnitNode(
                    Moq.It.IsAny<OperationState>(),
                    Moq.It.IsAny<UnitNode>()))
                .Returns<OperationState, UnitNode>((s, n) => n);

            // перадаваемые аргументы
            var state = new OperationState();

            UnitNode originalNode = new UnitNode()
            {
                Idnum = 11,
                Typ = (int)UnitTypeId.Folder,
                Text = "Before",
                FullName = "/Station/Before",
            };
            originalNode.SetAttribute("index", "21");
            originalNode.SetBinaries("buffer", new byte[] { 21, 44, 56 });
            nodes.Add(originalNode);

            var updnode = new UnitNode()
            {
                Idnum = originalNode.Idnum,
                Typ = (int)UnitTypeId.Folder,
                Text = "After",
                FullName = "/Station/After"
            };
            updnode.SetAttribute("index", "22");
            updnode.SetBinaries("buffer", new byte[] { 21, 36, 56 });

            // тестируемый метод
            var node = decorator.UpdateUnitNode(state, updnode);

            Assert.AreSame(updnode, node);

            // запись в аудит только одна
            Assert.AreEqual(1, entries.Count);

            // проверка записи пользователя
            var entry = entries[0];
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            // тип узла
            var typeNode = allowUnitTypes.Find(t => t.Idnum == (int)UnitTypeId.Folder);

            // изменился 1 узел
            Assert.AreEqual(1, entry.AuditUnits.Count());
            var auditUnit = entry.AuditUnits.First();
            Assert.AreEqual(node.Idnum, auditUnit.UnitNodeID);
            Assert.AreEqual(originalNode.FullName, auditUnit.FullPathOld);
            Assert.AreEqual(node.FullName, auditUnit.FullPathNew);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameOld);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameNew);

            // изменилось 1 свойство
            Assert.AreEqual(1, entry.AuditProps.Count());
            var auditProp = entry.AuditProps.First();
            Assert.AreEqual(node.Idnum, auditProp.UnitNodeID);
            Assert.AreEqual(node.FullName, auditProp.UnitNodeFullPath);
            Assert.AreEqual("index", auditProp.PropName);
            Assert.IsNull(auditProp.RevisionID);
            Assert.IsNull(auditProp.RevisionTime);
            Assert.IsNull(auditProp.RevisionBrief);
            Assert.AreEqual("21", auditProp.ValueOld);
            Assert.AreEqual("22", auditProp.ValueNew);

            // изменилось 1 длинное свойство свойство
            Assert.AreEqual(1, entry.AuditLobs.Count());
            var auditLob = entry.AuditLobs.First();
            Assert.AreEqual(node.Idnum, auditLob.UnitNodeID);
            Assert.AreEqual(node.FullName, auditLob.UnitNodeFullPath);
            Assert.AreEqual("buffer", auditLob.PropName);
            Assert.IsNull(auditLob.RevisionID);
            Assert.IsNull(auditLob.RevisionTime);
            Assert.IsNull(auditLob.RevisionBrief);
            Assert.AreEqual(new byte[] { 21, 44, 56 }, auditLob.ValueOld);
            Assert.AreEqual(new byte[] { 21, 36, 56 }, auditLob.ValueNew);
        }

        [Test]
        public void UpdateUnitNode_ChangedNothing_AuditNothing()
        {
            // декорируемый метод
            unitManager.Setup(m =>
                m.UpdateUnitNode(
                    Moq.It.IsAny<OperationState>(),
                    Moq.It.IsAny<UnitNode>()))
                .Returns<OperationState, UnitNode>((s, n) => n);

            // передаваемые аргументы
            var state = new OperationState();

            UnitNode originalNode = new UnitNode()
            {
                Idnum = 11,
                Typ = (int)UnitTypeId.Folder,
                Text = "Before",
                FullName = "/Station/Before",
            };
            originalNode.SetAttribute("index", "21");
            originalNode.SetBinaries("buffer", new byte[] { 21, 44, 56 });
            nodes.Add(originalNode);

            var updnode = new UnitNode()
            {
                Idnum = originalNode.Idnum,
                Typ = (int)UnitTypeId.Folder,
                Text = "Before",
                FullName = "/Station/Before"
            };
            updnode.SetAttribute("index", "21");
            updnode.SetBinaries("buffer", new byte[] { 21, 44, 56 });

            // тестируемый метод
            var node = decorator.UpdateUnitNode(state, updnode);

            Assert.AreSame(updnode, node);

            // ничего не записано
            Assert.AreEqual(1, entries.Count);
            // проверка записи пользователя
            var entry = entries[0];
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            Assert.IsTrue(entry.IsEmpty);
        }

        [Test]
        public void UpdateUnitNode_ChangedRevisionProperty_CorrectRevisionAudit()
        {
            // ревизии
            var revision1 = new RevisionInfo()
            {
                ID = 1,
                Time = new DateTime(2012, 01, 01),
                Brief = "Revision 1"
            };
            var revision2 = new RevisionInfo()
            {
                ID = 2,
                Time = new DateTime(2012, 03, 01),
                Brief = "Revision 2"
            };

            // декорируемый метод
            unitManager.Setup(m =>
                m.UpdateUnitNode(
                    Moq.It.IsAny<OperationState>(),
                    Moq.It.IsAny<UnitNode>()))
                .Returns<OperationState, UnitNode>((s, n) => n);

            // передаваемые аргументы
            var state = new OperationState();

            UnitNode originalNode = new UnitNode()
            {
                Idnum = 11,
                Typ = (int)UnitTypeId.Folder,
                Text = "Before",
                FullName = "/Station/Before",
            };
            originalNode.SetAttribute("index", "21");
            originalNode.SetAttribute("index", revision1, "21.0");
            originalNode.SetBinaries("buffer", revision1, new byte[] { 21, 44, 56 });
            nodes.Add(originalNode);

            var updnode = new UnitNode()
            {
                Idnum = originalNode.Idnum,
                Typ = (int)UnitTypeId.Folder,
                Text = "Before",
                FullName = "/Station/Before"
            };
            updnode.SetAttribute("index", revision1, "22.0");
            updnode.SetAttribute("index", revision2, "22.1");
            updnode.SetBinaries("buffer", revision1, new byte[] { 21, 36, 56 });

            // тестируемый метод
            var node = decorator.UpdateUnitNode(state, updnode);

            Assert.AreSame(updnode, node);

            // запись в аудит только одна
            Assert.AreEqual(1, entries.Count);

            // проверка записи пользователя
            var entry = entries[0];
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            // тип узла
            var typeNode = allowUnitTypes.Find(t => t.Idnum == (int)UnitTypeId.Folder);

            // изменилось 0 узлов
            Assert.AreEqual(0, entry.AuditUnits.Count());

            // изменилось 2 свойства
            Assert.AreEqual(3, entry.AuditProps.Count());
            // первое свойства index, revision1
            var auditProp = (from p in entry.AuditProps
                             where p.PropName == "index" && p.RevisionID == revision1.ID
                             select p).First();
            Assert.AreEqual(node.Idnum, auditProp.UnitNodeID);
            Assert.AreEqual(node.FullName, auditProp.UnitNodeFullPath);
            Assert.AreEqual("index", auditProp.PropName);
            Assert.AreEqual(revision1.ID, auditProp.RevisionID);
            Assert.AreEqual(revision1.Time, auditProp.RevisionTime);
            Assert.AreEqual(revision1.Brief, auditProp.RevisionBrief);
            Assert.AreEqual("21.0", auditProp.ValueOld);
            Assert.AreEqual("22.0", auditProp.ValueNew);
            // второе свойствл index, revision2
            auditProp = (from p in entry.AuditProps
                         where p.PropName == "index" && p.RevisionID == revision2.ID
                         select p).First();
            Assert.AreEqual(node.Idnum, auditProp.UnitNodeID);
            Assert.AreEqual(node.FullName, auditProp.UnitNodeFullPath);
            Assert.AreEqual("index", auditProp.PropName);
            Assert.AreEqual(revision2.ID, auditProp.RevisionID);
            Assert.AreEqual(revision2.Time, auditProp.RevisionTime);
            Assert.AreEqual(revision2.Brief, auditProp.RevisionBrief);
            Assert.IsNull(auditProp.ValueOld);
            Assert.AreEqual("22.1", auditProp.ValueNew);
            // второе свойствл index, RevisionInfo.Default
            auditProp = (from p in entry.AuditProps
                         where p.PropName == "index" && p.RevisionID == null
                         select p).First();
            Assert.AreEqual(node.Idnum, auditProp.UnitNodeID);
            Assert.AreEqual(node.FullName, auditProp.UnitNodeFullPath);
            Assert.AreEqual("index", auditProp.PropName);
            Assert.IsNull(auditProp.RevisionID);
            Assert.IsNull(auditProp.RevisionTime);
            Assert.IsNull(auditProp.RevisionBrief);
            Assert.AreEqual("21", auditProp.ValueOld);
            Assert.IsNull(auditProp.ValueNew);

            // изменилось 1 длинное свойство свойство
            Assert.AreEqual(1, entry.AuditLobs.Count());
            var auditLob = entry.AuditLobs.First();
            Assert.AreEqual(node.Idnum, auditLob.UnitNodeID);
            Assert.AreEqual(node.FullName, auditLob.UnitNodeFullPath);
            Assert.AreEqual("buffer", auditLob.PropName);
            Assert.AreEqual(revision1.ID, auditLob.RevisionID);
            Assert.AreEqual(revision1.Time, auditLob.RevisionTime);
            Assert.AreEqual(revision1.Brief, auditLob.RevisionBrief);
            Assert.AreEqual(new byte[] { 21, 44, 56 }, auditLob.ValueOld);
            Assert.AreEqual(new byte[] { 21, 36, 56 }, auditLob.ValueNew);

            var taskFactory = new System.Threading.Tasks.TaskFactory();

            var task = taskFactory.StartNew<int>(() => 10);

            //task.Con
            int r = task.Result;
        }

        [Test]
        public void MoveUnitNode__AuditFullPathChangeAndSortIndexChange()
        {
            // подготавливаем входные аргументы
            var state = new OperationState();
            int newParentID = 10;
            int oldIndex = 4;
            int newIndex = 10;

            UnitNode node = new UnitNode()
            {
                Idnum = 2,
                Typ = (int)UnitTypeId.Folder,
                Text = "Node1",
                FullName = "/Folder1/Node1",
                Index = 4
            };
            nodes.Add(node);

            // определяем поведение декорируемого метода
            unitManager.Setup(m =>
                m.MoveUnitNode(
                    Moq.It.IsAny<OperationState>(),
                    Moq.It.IsAny<int>(),
                    Moq.It.IsAny<int>(),
                    Moq.It.IsAny<int>()))
                .Callback<OperationState, int, int, int>((s, p, id, i) =>
                {
                    var n = unitManager.Object.ValidateUnitNode<UnitNode>(s, id, Privileges.Read);

                    n.ParentId = p;
                    n.FullName = "/Folder2/Node1";
                    n.Index = i;
                });

            // тестируемый метод
            decorator.MoveUnitNode(state, newParentID, node.Idnum, newIndex);


            // запись в аудит только одна
            Assert.AreEqual(1, entries.Count);

            // проверка записи пользователя
            var entry = entries[0];
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            // тип узла
            var typeNode = allowUnitTypes.Find(t => t.Idnum == (int)UnitTypeId.Folder);

            // изменился 1 узел
            Assert.AreEqual(1, entry.AuditUnits.Count());
            var auditUnit = entry.AuditUnits.First();
            Assert.AreEqual(node.Idnum, auditUnit.UnitNodeID);
            Assert.AreEqual("/Folder1/Node1", auditUnit.FullPathOld);
            Assert.AreEqual("/Folder2/Node1", auditUnit.FullPathNew);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameOld);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameNew);

            // изменилось 1 свойство
            Assert.AreEqual(1, entry.AuditProps.Count());
            var auditProp = entry.AuditProps.First();
            Assert.AreEqual(node.Idnum, auditProp.UnitNodeID);
            Assert.AreEqual(node.FullName, auditProp.UnitNodeFullPath);
            Assert.AreEqual("sortindex", auditProp.PropName);
            Assert.IsNull(auditProp.RevisionID);
            Assert.IsNull(auditProp.RevisionTime);
            Assert.IsNull(auditProp.RevisionBrief);
            Assert.AreEqual(oldIndex.ToString(), auditProp.ValueOld);
            Assert.AreEqual(newIndex.ToString(), auditProp.ValueNew);

            // изменилось 0 длинных свойств
            Assert.AreEqual(0, entry.AuditLobs.Count());
        }

        [Test]
        public void CopyUnitNode__AuditAllNodesInState()
        {
            // подготавливаем входные аргументы
            var state = new OperationState();
            bool recursive = true;

            UnitNode node1 = new UnitNode()
            {
                Idnum = 2,
                Typ = (int)UnitTypeId.Folder,
                Text = "Node1",
                FullName = "/Folder1/Node1",
                Index = 4
            };
            UnitNode node2 = new UnitNode()
            {
                Idnum = 3,
                Typ = (int)UnitTypeId.Folder,
                Text = "Node2",
                FullName = "/Folder1/Node1/Node2",
                Index = 1
            };
            UnitNode node3 = new UnitNode()
            {
                Idnum = 4,
                Typ = (int)UnitTypeId.Folder,
                Text = "Node3",
                FullName = "/Folder1/Node1/Node3",
                Index = 2
            };
            UnitNode node4 = new UnitNode()
            {
                Idnum = 5,
                Typ = (int)UnitTypeId.Folder,
                Text = "Node4",
                FullName = "/Folder1/Node4",
                Index = 2
            };
            nodes.Add(node1);
            nodes.Add(node2);
            nodes.Add(node3);
            nodes.Add(node4);

            var ids = new int[] { node1.Idnum, node4.Idnum };

            // определяем поведение декорируемого метода
            unitManager.Setup(m =>
                m.CopyUnitNode(
                    Moq.It.IsAny<OperationState>(),
                    Moq.It.IsAny<int[]>(),
                    Moq.It.IsAny<bool>()))
                .Callback<OperationState, int[], bool>((s, idArray, r) =>
                {
                    foreach (var n in new UnitNode[] { node1, node2, node3, node4 })
                    {
                        var copyNode = n.Clone() as UnitNode;
                        copyNode.Idnum = n.Idnum + 54;
                        copyNode.FullName = n.FullName.Replace("/Folder1", "/Folder2");
                        s.AddAsyncResult(copyNode);
                    }
                });

            // тестируемый метод
            decorator.CopyUnitNode(state, ids, recursive);

            // проверяем вызов декорируемого метода без искажений аргументов
            unitManager.Verify(m => m.CopyUnitNode(state, ids, recursive));

            // запись в аудит только одна
            Assert.AreEqual(1, entries.Count);

            // проверка записи пользователя
            var entry = entries[0];
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            // тип узла
            var typeNode = allowUnitTypes.Find(t => t.Idnum == (int)UnitTypeId.Folder);

            var oldPathArray = new String[] {
                "/Folder1/Node1",
                "/Folder1/Node1/Node2",
                "/Folder1/Node1/Node3",
                "/Folder1/Node4",
            };
            var newPathArray = new String[] {
                "/Folder2/Node1",
                "/Folder2/Node1/Node2",
                "/Folder2/Node1/Node3",
                "/Folder2/Node4",
            };
            var oldIds = new int[]{
                node1.Idnum,
                node2.Idnum,
                node3.Idnum,
                node4.Idnum,
            };
            var indexes = new int[]{
                node1.Index,
                node2.Index,
                node3.Index,
                node4.Index,
            };

            // изменилось 4 узла
            Assert.AreEqual(4, entry.AuditUnits.Count());
            for (int i = 0; i < oldPathArray.Length; i++)
            {
                var auditUnit = (from u in entry.AuditUnits
                                 where u.FullPathNew == newPathArray[i]
                                 select u).First();
                Assert.AreNotEqual(oldIds[i], auditUnit.UnitNodeID);
                Assert.IsNull(auditUnit.FullPathOld);
                Assert.AreEqual(newPathArray[i], auditUnit.FullPathNew);
                Assert.IsNull(auditUnit.TypeNameOld);
                Assert.AreEqual(typeNode.Text, auditUnit.TypeNameNew);
            }

            // изменилось 4 свойства
            Assert.AreEqual(4, entry.AuditProps.Count());
            for (int i = 0; i < newPathArray.Length; i++)
            {
                var auditProp = (from p in entry.AuditProps
                                 where p.UnitNodeFullPath == newPathArray[i]
                                 select p).First();
                Assert.AreNotEqual(oldIds[i], auditProp.UnitNodeID);
                Assert.AreEqual(newPathArray[i], auditProp.UnitNodeFullPath);
                Assert.AreEqual("sortindex", auditProp.PropName);
                Assert.IsNull(auditProp.RevisionID);
                Assert.IsNull(auditProp.RevisionTime);
                Assert.IsNull(auditProp.RevisionBrief);
                Assert.IsNull(auditProp.ValueOld);
                Assert.AreEqual(indexes[i].ToString(), auditProp.ValueNew);
            }

            // изменилось 0 длинных свойств
            Assert.AreEqual(0, entry.AuditLobs.Count());
        }

        [Test]
        public void ApplyImport__AuditAllNodesInState()
        {
            // подготавливаем входные аргументы
            var state = new OperationState();            

            UnitNode node1 = new UnitNode()
            {
                Typ = (int)UnitTypeId.Folder,
                Text = "Node1",
                FullName = "/Folder1/Node1",
            };
            UnitNode node2 = new UnitNode()
            {
                Typ = (int)UnitTypeId.Folder,
                Text = "Node2",
                FullName = "/Folder1/Node1/Node2",
            };
            UnitNode node3 = new UnitNode()
            {
                Typ = (int)UnitTypeId.Folder,
                Text = "Node3",
                FullName = "/Folder1/Node1/Node3",
            };
            UnitNode node4 = new UnitNode()
            {
                Typ = (int)UnitTypeId.Folder,
                Text = "Node4",
                FullName = "/Folder1/Node4",
            };
            UnitNode oldNode4 = new UnitNode()
            {
                Idnum = 5,
                Typ = (int)UnitTypeId.Folder,
                Text = "OldNode4",
                FullName = "/Folder1/OldNode4",
            };
            UnitNode rootNode = new UnitNode() 
            {
                Idnum = 10,
                Typ = (int)UnitTypeId.Folder,
                Text = "Folder1",
                FullName = "/Folder1/Node4",
            };
            nodes.Add(rootNode);
            nodes.Add(oldNode4);

            var importContainer = new ImportDataContainer(
                new TreeWrapp<UnitNode>[]
                {
                    new TreeWrapp<UnitNode>(node1, 
                        new TreeWrapp<UnitNode>[]
                        {
                            new TreeWrapp<UnitNode>(node2),
                            new TreeWrapp<UnitNode>(node3)
                        }),
                    new TreeWrapp<UnitNode>(node4)
                }, null);

            // определяем поведение декорируемого метода
            unitManager.Setup(m =>
                m.ApplyImport(
                    Moq.It.IsAny<OperationState>(),
                    Moq.It.IsAny<UnitNode>(),
                    Moq.It.IsAny<ImportDataContainer>()))
                .Callback<OperationState, UnitNode, ImportDataContainer>((s, root, data) =>
                {
                    var tw2 = new TreeWrapp<UnitNode>(node2.Clone() as UnitNode);
                    var tw3 = new TreeWrapp<UnitNode>(node3.Clone() as UnitNode);
                    var tw1 = new TreeWrapp<UnitNode>(node1.Clone() as UnitNode, new TreeWrapp<UnitNode>[] { tw2, tw3 });
                    var tw4 = new TreeWrapp<UnitNode>(node4.Clone() as UnitNode) { OldItem = oldNode4 };

                    s.AddAsyncResult(tw1);
                    s.AddAsyncResult(tw2);
                    s.AddAsyncResult(tw3);
                    s.AddAsyncResult(tw4);
                });

            // тестируемый метод
            decorator.ApplyImport(state, rootNode, importContainer);

            // проверяем вызов декорируемого метода без искажений аргументов
            unitManager.Verify(m => m.ApplyImport(state, rootNode, importContainer));

            // запись в аудит только одна
            Assert.AreEqual(1, entries.Count);

            // проверка записи пользователя
            var entry = entries[0];
            Assert.AreEqual(user.Text, entry.UserLogin);
            Assert.AreEqual(user.UserFullName, entry.UserFullName);
            Assert.AreEqual(user.Position, entry.UserPosition);

            // тип узла
            var typeNode = allowUnitTypes.Find(t => t.Idnum == (int)UnitTypeId.Folder);

            //var oldPathArray = new String[] {
            //    "/Folder1/Node1",
            //    "/Folder1/Node1/Node2",
            //    "/Folder1/Node1/Node3",
            //    "/Folder1/Node4",
            //};
            //var newPathArray = new String[] {
            //    "/Folder2/Node1",
            //    "/Folder2/Node1/Node2",
            //    "/Folder2/Node1/Node3",
            //    "/Folder2/Node4",
            //};
            //var oldIds = new int[]{
            //    node1.Idnum,
            //    node2.Idnum,
            //    node3.Idnum,
            //    node4.Idnum,
            //};
            //var indexes = new int[]{
            //    node1.Index,
            //    node2.Index,
            //    node3.Index,
            //    node4.Index,
            //};

            // изменилось 4 узла
            Assert.AreEqual(4, entry.AuditUnits.Count());
            //for (int i = 0; i < oldPathArray.Length; i++)
            //{
            var auditUnit = (from u in entry.AuditUnits
                             where u.FullPathNew == node1.FullName
                             select u).First();
            //Assert.AreNotEqual(oldIds[i], auditUnit.UnitNodeID);
            Assert.IsNull(auditUnit.FullPathOld);
            Assert.AreEqual(node1.FullName, auditUnit.FullPathNew);
            Assert.IsNull(auditUnit.TypeNameOld);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameNew);
             auditUnit = (from u in entry.AuditUnits
                             where u.FullPathNew == node2.FullName
                             select u).First();
            //Assert.AreNotEqual(oldIds[i], auditUnit.UnitNodeID);
            Assert.IsNull(auditUnit.FullPathOld);
            Assert.AreEqual(node2.FullName, auditUnit.FullPathNew);
            Assert.IsNull(auditUnit.TypeNameOld);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameNew);
             auditUnit = (from u in entry.AuditUnits
                             where u.FullPathNew == node3.FullName
                             select u).First();
            //Assert.AreNotEqual(oldIds[i], auditUnit.UnitNodeID);
            Assert.IsNull(auditUnit.FullPathOld);
            Assert.AreEqual(node3.FullName, auditUnit.FullPathNew);
            Assert.IsNull(auditUnit.TypeNameOld);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameNew);
             auditUnit = (from u in entry.AuditUnits
                             where u.FullPathNew == node4.FullName
                             select u).First();
            //Assert.AreNotEqual(oldIds[i], auditUnit.UnitNodeID);
            Assert.AreEqual(oldNode4.FullName, auditUnit.FullPathOld);
            Assert.AreEqual(node4.FullName, auditUnit.FullPathNew);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameOld);
            Assert.AreEqual(typeNode.Text, auditUnit.TypeNameNew);
            //}

            // изменилось 0 свойств
            Assert.AreEqual(0, entry.AuditProps.Count());
            //for (int i = 0; i < newPathArray.Length; i++)
            //{
            //    var auditProp = (from p in entry.AuditProps
            //                     where p.UnitNodeFullPath == newPathArray[i]
            //                     select p).First();
            //    Assert.AreEqual(oldIds[i], auditProp.UnitNodeID);
            //    Assert.AreEqual(newPathArray[i], auditProp.UnitNodeFullPath);
            //    Assert.AreEqual("index", auditProp.PropName);
            //    Assert.IsNull(auditProp.RevisionID);
            //    Assert.IsNull(auditProp.RevisionTime);
            //    Assert.IsNull(auditProp.RevisionBrief);
            //    Assert.IsNull(auditProp.ValueOld);
            //    Assert.AreEqual(indexes[i], auditProp.ValueNew);
            //}

            // изменилось 0 длинных свойств
            Assert.AreEqual(0, entry.AuditLobs.Count());
        }
    }
}

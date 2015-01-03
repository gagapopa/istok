using System;
using System.Collections.Generic;
using System.Threading;
using COTES.ISTOK;
using System.Xml;
using System.IO;
using COTES.ISTOK.ASC;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace COTES.ISTOK.Assignment
{
    class ExcelExporter : Exporter
    {
        public ExcelExporter()
        {
            //
        }

        public override ImportDataContainer Deserialize(OperationState state, byte[] importBuffer)
        {
            List<UnitNode> lstRes = new List<UnitNode>();
            TreeWrapp<UnitNode>[] res;

            using (MemoryStream ms = new MemoryStream(importBuffer))
            {
                BinaryFormatter bf = new BinaryFormatter();
                res = bf.Deserialize(ms) as TreeWrapp<UnitNode>[];
            }

            //if (res != null)
            //{
            //    UnitNode node;
            //    foreach (var item in res)
            //    {
            //        node = item.UnitNode;
            //        SetUnitNodes(node, item.ChildNodes);
            //        lstRes.Add(node);
            //    }
            //}
            //values = new Package[0];
            //return res;
            return new ImportDataContainer(res, null);
        }

        private void SetUnitNodes(UnitNode parent, TreeWrapp<UnitNode>[] children)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            parent.ClearNodes();
            if (children != null && children.Length > 0)
            {
                UnitNode node;
                foreach (var item in children)
                {
                    node = item.Item;
                    SetUnitNodes(node, item.ChildNodes);
                    parent.AddNode(node);
                }
            }
        }
        public override byte[] Serialize(OperationState state, TreeWrapp<UnitNode>[] nodes, int nodeCount, ParameterNode[] parameters, DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }
    }
}

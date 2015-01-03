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
    class WordExporter : Exporter
    {
        //UnitManager umanager;

        public WordExporter(IUnitManager umanager)
        {
            //this.umanager = umanager;
        }
        private TreeWrapp<UnitNode>[] DeserializeData(OperationState state, DataSet data)
        {
            Dictionary<string, TreeWrapp<UnitNode>> dicParents;
            Dictionary<string, string> dicFormulas;
            var lstParents = new TreeWrapp<UnitNode>(null);
            TreeWrapp<UnitNode> list = new TreeWrapp<UnitNode>(null);
            TreeWrapp<UnitNode> pnode;
            TreeWrapp<UnitNode> curnode;
            UnitNode node;
            string strong_parent;
            string strPostfix;
            bool add;

            state.Progress = 0;
            try
            {
                if (data != null)
                {
                    DataTable table;
                    DataRow row;
                    //foreach (DataTable tbl in data.Tables)
                    for (int ti = 0; ti < data.Tables.Count;ti++ )
                    {
                        table = data.Tables[ti];
                        //List<UnitNode> plist = new List<UnitNode>();

                        //table.Merge(item);
                        curnode = new TreeWrapp<UnitNode>(new UnitNode());
                        curnode.Item.Typ = (int)UnitTypeId.Folder;
                        curnode.Item.Text = table.TableName;
                        curnode.Item.Index = ti;
                        list.AddChild(curnode);
                        dicParents = new Dictionary<string, TreeWrapp<UnitNode>>();

                        //foreach (DataRow item in table.Rows)
                        for (int ri = 0; ri < table.Rows.Count;ri++ )
                        {
                            row = table.Rows[ri];
                            node = null;
                            strPostfix = "";
                            dicFormulas = new Dictionary<string, string>();

                            if (table.Columns.Contains("Type"))
                            {
                                string type = row["Type"].ToString().ToLower();

                                if (type.Contains("ручной ввод"))
                                {
                                    node = new ParameterNode();
                                    node.Typ = (int)UnitTypeId.ManualParameter;
                                }
                                else
                                    if (type.Contains("книга") || type.Contains("эхо") || type.Contains("тип.эх"))
                                    {
                                        node = new NormFuncNode();
                                        node.Typ = (int)UnitTypeId.NormFunc;
                                    }
                            }
                            if (node == null)
                            {
                                node = new ParameterNode();
                                node.Typ = (int)UnitTypeId.TEP;
                            }
                            if (node != null) node.Index = ri;
                            if (table.Columns.Contains("Postfix"))
                            {
                                strPostfix = row["Postfix"].ToString();
                                strPostfix = strPostfix == null ? "" : strPostfix.Trim();
                            }
                            add = true;
                            lstParents = new TreeWrapp<UnitNode>(null);
                            strong_parent = "";
                            foreach (DataColumn column in table.Columns)
                            {
                                switch (column.ColumnName)
                                {
                                    case "Name":
                                        node.Text = row["Name"].ToString();
                                        break;
                                    case "Code":
                                        node.Code = row["Code"].ToString();
                                        break;
                                    case "Unit":
                                        string tmp = row["Unit"].ToString();

                                        if (!string.IsNullOrEmpty(tmp))
                                        {
                                            tmp = tmp.Trim();
                                            if (!tmp.Equals("-"))
                                                node.SetAttribute("unit", tmp);
                                        }
                                        break;
                                    case "Index":
                                        node.DocIndex = row["Index"].ToString();
                                        break;
                                    default:
                                        string name = column.ColumnName.ToLower();
                                        var cellcontent = row[column].ToString();
                                        const string prefix = "formula:";
                                        if (string.IsNullOrEmpty(cellcontent)) break;

                                        if (name.StartsWith("parent_"))
                                        {
                                            if (!dicParents.TryGetValue(column.ColumnName, out pnode))
                                            {
                                                pnode = new TreeWrapp<UnitNode>(new UnitNode());
                                                pnode.Item.Text = column.ColumnName.Substring("parent_".Length);
                                                pnode.Item.Typ = (int)UnitTypeId.Folder;
                                                dicParents[column.ColumnName] = pnode;
                                            }
                                            if (cellcontent.StartsWith(prefix))
                                                dicFormulas[pnode.Item.Text] = cellcontent.Substring(prefix.Length);
                                            lstParents.AddChild(pnode);
                                            add = false;
                                        }
                                        else
                                            if (name.StartsWith("parent"))
                                            //if (!string.IsNullOrEmpty(item[column].ToString()) && name.StartsWith("parent"))
                                            {
                                                if (!dicParents.TryGetValue(row[column].ToString(), out pnode))
                                                {
                                                    pnode = new TreeWrapp<UnitNode>(new UnitNode());
                                                    pnode.Item.Text = row[column].ToString();
                                                    pnode.Item.Typ = (int)UnitTypeId.Folder;
                                                    dicParents[row[column].ToString()] = pnode;
                                                }
                                                strong_parent = pnode.Item.Text;
                                                lstParents.AddChild(pnode);
                                                //pnode.Nodes.Add(node);
                                                add = false;
                                            }

                                        break;
                                }
                            }

                            if (lstParents.ChildNodes != null)
                            {
                                foreach (var par in lstParents.ChildNodes)
                                {
                                    pnode = new TreeWrapp<UnitNode>((UnitNode)node.Clone());
                                    if (string.IsNullOrEmpty(strong_parent) || par.Item.Text != strong_parent) pnode.Item.Code += "," + par.Item.Text;
                                    if (!string.IsNullOrEmpty(strPostfix) && !string.IsNullOrEmpty(pnode.Item.Code))
                                        pnode.Item.Code = pnode.Item.Code + "," + strPostfix;
                                    if (dicFormulas.ContainsKey(par.Item.Text))
                                        pnode.Item.SetAttribute("formula_text", dicFormulas[par.Item.Text]);
                                    par.AddChild(pnode);
                                }
                            }
                            if (add)
                            {
                                if (!string.IsNullOrEmpty(strPostfix) && !string.IsNullOrEmpty(node.Code))
                                    node.Code = node.Code + "," + strPostfix;
                                curnode.AddChild(node);
                            }
                        }

                        foreach (var item in dicParents.Values)
                        {
                            ChangeParentType(item);
                            curnode.AddChild(item);
                        }
                        ChangeParentType(curnode);
                    }
                }
            }
            catch (Exception) 
            {
                //
            }

            state.Progress = AsyncOperation.MaxProgressValue;
            return list.ChildNodes;
        }

        private void ChangeParentType(TreeWrapp<UnitNode> parent)
        {
            int type = 0;
            foreach (var child in parent.ChildNodes)
            {
                if (type != 0)
                {
                    if (type != child.Item.Typ)
                    {
                        type = 0;
                        break;
                    }
                }
                else
                    type = child.Item.Typ;
            }
            switch (type)
            {
                case (int)UnitTypeId.ManualParameter:
                    parent.Item.Typ = (int)UnitTypeId.ManualGate;
                    break;
                case (int)UnitTypeId.TEP:
                    parent.Item.Typ = (int)UnitTypeId.TEPTemplate;
                    break;
            }
        }

        public override byte[] Serialize(OperationState state, TreeWrapp<UnitNode>[] nodes, int nodeCount, ParameterNode[] parameters, DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public override ImportDataContainer Deserialize(OperationState state, byte[] importBuffer)
        {
            List<UnitNode> lstNodes = new List<UnitNode>();
            DataSet ds;

            using (MemoryStream ms = new MemoryStream(importBuffer))
            {
                BinaryFormatter bf = new BinaryFormatter();
                ds = bf.Deserialize(ms) as DataSet;
            }

            //values = new Package[0];
            //return DeserializeData(state, ds);
            return new ImportDataContainer(DeserializeData(state, ds), null);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;
//using COTES.ODKCaller;

namespace XMLTest
{
    /*class DGV : DataGridView
    {
        public DGV()
        {
            DoubleBuffered = true;
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }
    }*/

    public partial class Form1 : Form
    {
        private XmlDocument doc = null;
        private XmlNode m_node = null;
        private string m_cell = "";
        
        [DllImport("user32.dll")]
        public static extern ushort GetKeyState(short nVirtKey);

        public Form1()
        {
            InitializeComponent();
            
            XmlDocument doc = new XmlDocument();
        }

        private void AddInfo(XmlDocument doc)
        {
            XmlNode root = doc.DocumentElement;
            XmlElement elem2;
            XmlText txt;
            XmlElement elem = doc.CreateElement("struct");
            XmlAttribute attr = doc.CreateAttribute("name");
            attr.Value = "block1";
            elem.Attributes.Append(attr);

            elem2 = doc.CreateElement("param");
            //attr = doc.CreateAttribute("code");
            
            //attr.Value = "local::CompresssedValueArchive\\blabla.OUT";
            //XmlNode nd = (XmlNode)elem2;

            txt = doc.CreateTextNode("local::CompresssedValueArchive\\blabla.OUT");
            elem2.AppendChild(txt);
            //elem2.Attributes.Append(attr);
            attr = doc.CreateAttribute("name");
            attr.Value = "Bla bla";
            elem2.Attributes.Append(attr);

            elem.AppendChild(elem2);
            root.AppendChild(elem);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            
        }

        private void RefreshStructs()
        {
            XmlAttribute attr;
            int i;
            string txt;

            lbxStructs.Items.Clear();

            if (doc == null ||
                doc.DocumentElement == null) return;

            for (i = 0; i < doc.DocumentElement.ChildNodes.Count; i++)
            {
                txt = doc.DocumentElement.ChildNodes[i].Name;
                attr = doc.DocumentElement.ChildNodes[i].Attributes["name"];
                if (attr != null) txt += "(" + attr.Value + ")";
                
                lbxStructs.Items.Add(txt);
            }

            dgv.Rows.Clear();
            dgv.Columns.Clear();
        }

        private TreeNode FillNode(XmlNode root)
        {
            TreeNode node = new TreeNode();
            TreeNode nnode;
            int i;

            if (root == null) return null;
            
            node.Text = root.Name;
            //node.Text += "[" + root.NodeType.ToString() + "]";
            if (root.Attributes != null)
            {
                for (i = 0; i < root.Attributes.Count; i++)
                {
                    if (root.Attributes[i].Name.ToLower() == "name")
                    {
                        node.Text += " (" + root.Attributes[i].Value + ")";
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(root.Value))
                node.Text += " = " + root.Value;

            for (i = 0; i < root.ChildNodes.Count; i++)
            {
                nnode = FillNode(root.ChildNodes[i]);
                node.Nodes.Add(nnode);
            }

            return node;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (doc != null)
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        txtFilename.Text = saveFileDialog.FileName;
                        doc.Save(txtFilename.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                //XmlNameTable nt;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFilename.Text = openFileDialog.FileName;
                    doc = new XmlDocument();
                    doc.Load(txtFilename.Text);
                    RefreshStructs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateDataView()
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            //dgv.Columns
        }

        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            DataGridViewColumn column;

            column = new DataGridViewColumn();
            //column.Name = "namee";
            //column.CellTemplate
            //column.HeaderText = "";
            dgv.Columns.Add(txtColumnName.Text, txtColumnName.Text);
        }

        private void btnDelColumn_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedCells.Count > 0)
            {
                dgv.Columns.RemoveAt(dgv.SelectedCells[0].ColumnIndex);
            }
        }

        private void RefreshView(XmlNode root)
        {
            int i;

            dgv.Rows.Clear();
            dgv.Columns.Clear();

            if (root == null) return;


            for (i = 0; i < root.ChildNodes.Count; i++)
            {
                dgv.Rows.Add(GetObjects(root.ChildNodes[i]));
            }
        }

        private object[] GetObjects(XmlNode node)
        {
            DataGridViewColumn column;
            XmlAttribute attr;
            object[] objs = null;
            int i;

            if (node == null) return null;

            objs = new object[dgv.Columns.Count + node.Attributes.Count];

            //if (node.ChildNodes.Count > 0 &&
            //    node.FirstChild.GetType() == typeof(XmlText))
            //    objs[dgv.Columns["parameter"].Index] = node.FirstChild.Value;
            //else
            //    objs[dgv.Columns["parameter"].Index] = node.Name;

            for (i = 0; i < node.Attributes.Count; i++)
            {
                attr = node.Attributes[i];
                column = dgv.Columns[attr.Name];
                if (column == null)
                {
                    column = dgv.Columns[dgv.Columns.Add(attr.Name,
                        attr.Name)];
                }
                objs[column.Index] = attr.Value;
            }

            return objs;
        }

        private void lbxStructs_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            XmlNode node;

            if (lbxStructs.SelectedIndex < 0) return;

            if (doc != null &&
                doc.DocumentElement != null &&
                lbxStructs.SelectedIndex < doc.DocumentElement.ChildNodes.Count)
            {
                node = doc.DocumentElement.ChildNodes[lbxStructs.SelectedIndex];
                m_node = node;
                RefreshView(node);
            }
        }

        private void dgv_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            m_cell = e.Control.Text;
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            Commit();
        }

        private void Commit()
        {
            XmlAttribute attr;
            XmlElement elem;
            int i, j;
            object tmp;

            if (m_node == null) return;

            while (m_node.ChildNodes.Count > 0)
                m_node.RemoveChild(m_node.ChildNodes[0]);

            for (i = 0; i < dgv.Rows.Count - 1; i++)
            {
                elem = doc.CreateElement("param");

                for (j = 0; j < dgv.Columns.Count; j++)
                {
                    attr = doc.CreateAttribute(dgv.Columns[j].HeaderText);
                    tmp = dgv.Rows[i].Cells[j].Value;
                    if (tmp != null)
                    {
                        attr.Value = tmp.ToString();
                        elem.Attributes.Append(attr);
                    }
                }
                
                m_node.AppendChild(elem);
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshStructs();
        }

        private void addStructToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlElement elem;

            if (doc == null)
                doc = new XmlDocument();

            if (doc.DocumentElement == null)
            {
                elem = doc.CreateElement("body");
                doc.AppendChild(elem);
            }

            elem = doc.CreateElement("struct");
            doc.DocumentElement.AppendChild(elem);
            RefreshStructs();
        }

        private void deleteStructToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lbxStructs.SelectedIndex < 0) return;

            if (doc == null || doc.DocumentElement == null ||
                lbxStructs.SelectedIndex >= doc.DocumentElement.ChildNodes.Count) return; 

            doc.DocumentElement.RemoveChild(
                doc.DocumentElement.ChildNodes[lbxStructs.SelectedIndex]);
            RefreshStructs();
        }

        private void lbxStructs_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                popupStructs.Show((Control)sender, e.X, e.Y);
            }
        }

        private void lbxStructs_SelectedIndexChanged(object sender, EventArgs e)
        {
            XmlNode node;
            XmlAttribute attr;

            txtStructName.Text = "";

            if (lbxStructs.SelectedIndex < 0 ||
                doc == null ||
                doc.DocumentElement == null ||
                lbxStructs.SelectedIndex >= doc.DocumentElement.ChildNodes.Count)
                return;
            
            node = doc.DocumentElement.ChildNodes[lbxStructs.SelectedIndex];
            attr = node.Attributes["name"];
            if (attr != null)
                txtStructName.Text = attr.Value;
        }

        private void btnChangeName_Click(object sender, EventArgs e)
        {
            XmlNode node;
            XmlAttribute attr;

            if (lbxStructs.SelectedIndex < 0 ||
                doc == null ||
                doc.DocumentElement == null ||
                lbxStructs.SelectedIndex >= doc.DocumentElement.ChildNodes.Count)
                return;

            node = doc.DocumentElement.ChildNodes[lbxStructs.SelectedIndex];
            attr = node.Attributes["name"];
            if (attr == null)
            {
                attr = doc.CreateAttribute("name");
                node.Attributes.Append(attr);
            }
            
            attr.Value = txtStructName.Text;
            RefreshStructs();
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0) return;

            txtColumnName.Text = dgv.Columns[e.ColumnIndex].HeaderText;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedCells.Count > 0)
            {
                dgv.Columns[dgv.SelectedCells[0].ColumnIndex].Name =
                    txtColumnName.Text;
                dgv.Columns[dgv.SelectedCells[0].ColumnIndex].HeaderText =
                    txtColumnName.Text;
            }
        }

        private static bool IsKeyPressed(Keys key)
        {
            const ushort keyDownBit = 0x80;

            return ((GetKeyState((short)key) & keyDownBit) == keyDownBit);
        }

        private void dgView_KeyDown(object sender, KeyEventArgs e)
        {
            DataGridViewCell cell;
            string str;
            string[][] cols;
            string[] str_lines;
            int[] pos = null;
            char[] sep_item = new char[] { '\t' };
            char[] sep_line = new char[] { '\n', '\r' };
            int i, j, i_row, i_col;
            int n_row;
            DataSet dataset = new DataSet();
            dataset.Tables.Add(new DataTable());

            cell = dgv.CurrentCell;

            if (cell != null)
            {
                if (IsKeyPressed(Keys.ShiftKey) && IsKeyPressed(Keys.Insert))
                {
                    str = Clipboard.GetText();
                    if (string.IsNullOrEmpty(str)) return;

                    //lock_update = true;

                    i_row = cell.RowIndex;
                    i_col = cell.ColumnIndex;


                    str_lines = str.Split(sep_line);
                    str_lines = DelNulls(str_lines);

                    cols = new string[str_lines.Length][];
                    //table = dataset.Tables[0];
                    
                    for (j = 0; j < str_lines.Length; j++)
                    {
                        if (string.IsNullOrEmpty(str_lines[j])) continue;
                        cols[j] = str_lines[j].Split(sep_item);
                        if (string.IsNullOrEmpty(cols[j][0])) continue;

                        if (pos == null)
                        {
                            int inc = 0;
                            pos = new int[cols[j].Length];

                            for (int k = i_col; k < dgv.Columns.Count; k++)
                            {
                                if (inc == pos.Length) break;
                                if (dgv.Columns[k].Visible)
                                {
                                    pos[inc] = k;
                                    inc++;
                                    continue;
                                }
                            }
                        }

                        if (i_row + j > dgv.Rows.Count - 1)
                        {
                            n_row = dgv.Rows.Add();
                            //row = dataset.Tables[0].NewRow();
                        }
                        else
                        {
                            //row = dataset.Tables[0].Rows[i_row + j];
                            n_row = i_row + j;
                        }

                        for (i = i_col; i < cols[j].Length+i_col; i++)
                        {
                            if (i - i_col < pos.Length && pos[i-i_col] < dgv.Columns.Count)
                            {
                                dgv.Rows[n_row].Cells[i].Value =
                                    cols[j][i-i_col].ToString().Trim();
                                //row[pos[i]] = cols[j][i].ToString().Trim();
                            }
                            else break;
                        }

                        if (i_row + j > dgv.Rows.Count - 1)
                        {
                            //dataset.Tables[0].Rows.Add(row);
                        }
                    }

                    //lock_update = false;

                    //UpdateDB();
                    //UpdateDG();
                    dgv.CurrentCell = dgv.Rows[i_row].Cells[i_col];
                }
            }
        }
        private string[] DelNulls(string[] lstStr)
        {
            List<string> lst = new List<string>();
            string[] res;
            int i;

            for (i = 0; i < lstStr.Length; i++)
            {
                if (!string.IsNullOrEmpty(lstStr[i]) && !string.IsNullOrEmpty(lstStr[i].Trim()))
                {
                    lst.Add(lstStr[i]);
                }
            }

            res = new string[lst.Count];
            for (i = 0; i < lst.Count; i++)
            {
                res[i] = lst[i];
            }

            return res;
        }

        private string FormatTag(int row, string tag)
        {
            string[] arrStrs;
            string ntag;

            ntag = dgv.Rows[row].Cells["code"].Value.ToString();

            arrStrs = ntag.Split(new char[] { '.' });
            ntag = arrStrs[0] + tag;
            //"=([code]|*.MAX)"

            return ntag;
        }

        private void GetODKValues(int row, int column, bool allColumn)
        {
            //WinConnect wincc = new WinConnect();
            //string[] arrTags;
            //string s_tag;
            //string tag = "";
            //string strval;
            //int r = row, c = column;
            //uint t_id;
            //object[] values;
            //int i;

            //try
            //{
            //    wincc.Connect();

            //    s_tag = dgv.Rows[r].Cells[c].Value.ToString();
                
            //    if (allColumn) r = 0;

            //    for (i = r; i < dgv.Rows.Count; i++)
            //    {
            //        if (cbxNulls.Checked && allColumn)
            //        {
            //            if (dgv.Rows[i].Cells[c].Value != null &&
            //                dgv.Rows[i].Cells[c].Value.ToString() != "")
            //                continue;
            //        }
            //        tag = FormatTag(i, s_tag);
            //        arrTags = new string[] { tag };
            //        t_id = wincc.StartUpdateTransaction(arrTags);
            //        Thread.Sleep(50);
            //        values = wincc.GetTag(arrTags);
            //        wincc.StopUpdateTransaction(t_id);

            //        if (values.Length > 0)
            //            strval = values[0].ToString();
            //        else
            //            strval = "";

            //        dgv.Rows[i].Cells[c].Value = strval;

            //        if (!allColumn) break;
            //    }
            //}
            //catch (Exception)
            //{
            //    //
            //}
            //finally
            //{
            //    wincc.DisConnect();
            //}
            
        }

        private void getValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentCell == null) return;

            GetODKValues(dgv.CurrentCell.RowIndex,
                dgv.CurrentCell.ColumnIndex,
                false);
        }

        private void getValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentCell == null) return;


            if (dgv.SelectedCells.Count > 1)
            {
                foreach (DataGridViewCell item in dgv.SelectedCells)
                {
                    GetODKValues(item.RowIndex,
                    item.ColumnIndex,
                    false);
                }
            }
            else
            {
                GetODKValues(dgv.CurrentCell.RowIndex,
                    dgv.CurrentCell.ColumnIndex,
                    true);
            }
        }

        private void dgv_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                popupGrid.Show((Control)sender, e.X, e.Y);
            }
        }

        private void copyToAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewCell cell;
            object value;
            int row, col;

            if (dgv.CurrentCell == null) return;

            value = dgv.CurrentCell.Value;
            col = dgv.CurrentCell.ColumnIndex;

            if (dgv.SelectedCells.Count > 1)
            {
                foreach (DataGridViewCell item in dgv.SelectedCells)
                {
                    cell = item;

                    if (cbxNulls.Checked && cell.Value != null) continue;

                    cell.Value = value;
                }
            }
            else
            {
                for (row = 0; row < dgv.Rows.Count - 1; row++)
                {
                    cell = dgv.Rows[row].Cells[col];

                    if (cbxNulls.Checked && cell.Value != null) continue;

                    cell.Value = value;
                }
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            Color color = new Color();

            if (dgv.CurrentCell == null) return;

            if (dgv.CurrentCell.FormattedValue != "")
            {
                color = Color.FromArgb(int.Parse(dgv.CurrentCell.Value.ToString()));
                colorDialog.Color = color;
            }

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                dgv.CurrentCell.Value = colorDialog.Color.ToArgb();
            }
        }

        private void setFormulaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewCell cell;

            if (dgv.CurrentCell == null) return;

            cell = dgv.CurrentCell;

            SetFormula(cell.RowIndex, cell.ColumnIndex, txtStructName.Text);
        }

        private void setFormulaToAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewCell cell;
            int row;

            if (dgv.CurrentCell == null) return;

            cell = dgv.CurrentCell;

            if (dgv.SelectedCells.Count > 1)
            {
                foreach (DataGridViewCell item in dgv.SelectedCells)
                {
                    SetFormula(item.RowIndex, cell.ColumnIndex, txtStructName.Text);
                }
            }
            else
            {
                for (row = 0; row < dgv.Rows.Count - 1; row++)
                    SetFormula(row, cell.ColumnIndex, txtStructName.Text);
            }
        }

        private void SetFormula(int row, int col, string prefix)
        {
            string[] txt;
            string tag;

            tag = dgv.Rows[row].Cells["code"].Value.ToString();
            txt = tag.Split(new char[] { '/' });
            tag = txt[txt.Length - 1];
            txt = tag.Split(new char[] { '.' });
            tag = txt[0];

            if (txt[1] == "OUT") dgv.Rows[row].Cells[col].Value = prefix + "_" + tag;
        }

        private void btnAddFormulaCodToImport_Click(object sender, EventArgs e)
        {
            const string formulacod = "formula_cod";
            if (doc == null ||
                doc.DocumentElement == null) return;

            var nodes = doc.GetElementsByTagName("node");
            foreach (XmlNode node in nodes)
            {
                var type = node.Attributes.GetNamedItem("type");
                if (type == null || type.Value.ToLower() != "parameter") continue;
                string code = "";
                bool hasFormulaCod = false;
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.Name.ToLower() == "property")
                    {
                        var name = child.Attributes.GetNamedItem("name");
                        if (name != null)
                        {
                            if (name.Value.ToLower() == "code")
                                code = GetText(child);
                            if (name.Value.ToLower() == formulacod)
                                hasFormulaCod = true;
                        }
                    }
                }
                if (!hasFormulaCod && !string.IsNullOrEmpty(code))
                {
                    var newnode = doc.CreateNode(XmlNodeType.Element, "property", "");
                    var attr = doc.CreateNode(XmlNodeType.Attribute, "name", "") as XmlAttribute;
                    attr.Value = formulacod;
                    newnode.Attributes.Append(attr);
                    var fcode = doc.CreateNode(XmlNodeType.Text, "", "");
                    fcode.Value = code;
                    newnode.AppendChild(fcode);
                    node.AppendChild(newnode);
                }
            }
        }
        private string GetText(XmlNode node)
        {
            if (node == null) throw new ArgumentNullException("node");
            var res = new StringBuilder();
            
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Text)
                    res.Append(child.Value);
            }
            
            return res.ToString();
        }
    }
}
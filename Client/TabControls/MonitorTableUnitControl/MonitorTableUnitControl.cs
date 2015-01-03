using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    partial class MonitorTableUnitControl : BaseUnitControl
    {
        Dictionary<int, List<Cell>> dicParams = new Dictionary<int, List<Cell>>();
        Dictionary<Cell, ChildParamNode> dicChild = new Dictionary<Cell, ChildParamNode>();
        MonitorTableUnitProvider monitorTableUnitProvider = null;
        MonitorTableNode monitorNode = null;

        public delegate void ControlSelectedDelegate(ChildParamNode control);
        public event ControlSelectedDelegate SelectedControlChanged = null;

        bool updating = false;

        public MonitorTableUnitControl(MonitorTableUnitProvider unitProvider, bool editMode)
            : base(unitProvider)
        {
            InitializeComponent();
            EditMode = editMode;
            dgv.ReadOnly = !EditMode;
            dgv.AllowUserToAddRows = EditMode;
            dgv.AllowUserToDeleteRows = EditMode;
            dgv.RowHeadersVisible = EditMode;

            typeof(Control).InvokeMember("DoubleBuffered",
                  BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                  null, dgv, new object[] { true });
        }
        public MonitorTableUnitControl(MonitorTableUnitProvider unitProvider)
            : this(unitProvider, false)
        {
            //
        }

        public override void InitiateProcess()
        {
            if (UnitProvider as MonitorTableUnitProvider != null)
            {
                monitorTableUnitProvider = (MonitorTableUnitProvider)UnitProvider;
                monitorNode = (MonitorTableNode)monitorTableUnitProvider.UnitNode;
                monitorTableUnitProvider.UpdateInterval = monitorNode.UpdateInterval;
                CreateTable();
                timer1.Enabled = !EditMode;
            }
        }

        public bool EditMode { get; set; }

        private void CreateTable()
        {
            try
            {
                FreeTable();
                monitorTableUnitProvider.RegisterParameters();
                UpdateDGV();
                UpdateValues();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FreeTable()
        {
            dicParams.Clear();
            dgv.Rows.Clear();
            dgv.Columns.Clear();
        }

        private byte[] SerializeCell(Cell cell)
        {
            return cell.ToBytes();
            //BinaryFormatter bf = new BinaryFormatter();
            //MemoryStream ms = new MemoryStream();

            //if (cell == null) throw new ArgumentNullException();

            //bf.Serialize(ms, cell);
            //return ms.ToArray();
        }
        private Cell DeserializeCell(byte[] bytes)
        {
            Cell cell = new Cell();
            cell.FromBytes(bytes);
            return cell;
            //BinaryFormatter bf = new BinaryFormatter();
            //MemoryStream ms = new MemoryStream(bytes);
            //return (Cell)bf.Deserialize(ms);
        }

        public void SetCell(Cell cell)
        {
            DataTable table;

            if (!EditMode) return;
            if (cell == null) throw new ArgumentNullException("cell");
            if (monitorNode.Table == null) monitorNode.Table = new DataTable();
            table = monitorNode.Table;

            while (table.Columns.Count <= cell.ColumnIndex)
            {
                DataColumn column = table.Columns.Add("", typeof(object));
                //column.DataType = typeof(object);
                column.Caption = "None";
            }
            while (table.Rows.Count <= cell.RowIndex)
            {
                DataRow row = table.NewRow();
                table.Rows.Add(row);
            }

            if (cell.RowIndex == 0)
            {
                string caption = cell.Text;
                if (string.IsNullOrEmpty(caption)) caption = "None";
                table.Rows[cell.RowIndex][cell.ColumnIndex] = SerializeCell(cell);
                table.Columns[cell.ColumnIndex].Caption = caption;
            }
            else
                table.Rows[cell.RowIndex][cell.ColumnIndex] = SerializeCell(cell);
            monitorNode.Table = table;
        }

        /// <summary>
        /// Формирование ГридВью из таблицы нода
        /// </summary>
        private void UpdateDGV()
        {
            try
            {
                updating = true;
                if (monitorNode.Table != null)
                {
                    Cell cell;
                    object cellobj;
                    for (int row = 0; row < monitorNode.Table.Rows.Count; row++)
                    {
                        for (int col = 0; col < monitorNode.Table.Columns.Count; col++)
                        {
                            cellobj = monitorNode.Table.Rows[row][col];
                            if (cellobj is byte[])
                                cell = DeserializeCell(cellobj as byte[]);
                            else
                                cell = null;
                            //cell = cellobj as Cell;
                            if (cell == null)
                            {
                                cell = new Cell(row, col);
                                if (cellobj != null) cell.Text = cellobj.ToString();
                            }
                            if (cell.ParameterId != 0)
                            {
                                if (!dicParams.ContainsKey(cell.ParameterId))
                                    dicParams.Add(cell.ParameterId, new List<Cell>());
                                if (!dicParams[cell.ParameterId].Contains(cell))
                                    dicParams[cell.ParameterId].Add(cell);
                                if (!dicChild.ContainsKey(cell))
                                    dicChild.Add(cell, GetChildParam(cell.ParameterId));
                            }
                            while (dgv.Columns.Count < col) dgv.Columns.Add("col" + col.ToString(), "None");
                            if (dgv.Columns.Count == col)
                            {
                                DataGridViewColumn column = new DataGridViewTextBoxColumn();
                                column.Name = "col" + col.ToString();
                                if (row == 0) column.HeaderText = cell.Text;
                                dgv.Columns.Add(column);
                            }
                            else
                            {
                                if (row != 0)
                                {
                                    while (dgv.Rows.Count <= row - 1) dgv.Rows.Add();
                                    dgv.Rows[row - 1].Cells[col].Tag = cell;
                                    dgv.Rows[row - 1].Cells[col].Value = cell.Text;
                                    //dgv.Rows[row - 1].Cells[col].Style.Alignment
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                updating = false;
            }
        }

        /// <summary>
        /// Формирование таблицы нода по ГридВью
        /// </summary>
        private void UpdateTable()
        {
            Cell c;

            if (updating || !EditMode) return;
            monitorNode.Table = new DataTable();

            for (int col = 0; col < dgv.Columns.Count; col++)
            {
                c = new Cell(0, col);
                c.Text = dgv.Columns[col].HeaderText;
                SetCell(c);
            }
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Tag == null || (c = cell.Tag as Cell) == null)
                    {
                        c = new Cell(cell.RowIndex + 1, cell.ColumnIndex);
                        if (cell.Value != null) c.Text = cell.Value.ToString();
                    }
                    if (c != null) SetCell(c);
                }
            }
            //вызов Set для сериализации
            //monitorNode.Table = monitorNode.Table;
        }

        private ChildParamNode GetChildParam(int paramId)
        {
            if (monitorNode.Parameters == null) return null;
            foreach (var item in monitorNode.Parameters)
                if (item.ParameterId == paramId) return item;
            return null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateValues();
        }

        private void UpdateValues()
        {
            if (monitorTableUnitProvider.ParamValues != null)
            {
                lock (monitorTableUnitProvider.ParamValues)
                {
                    try
                    {
                        dgv.SuspendLayout();
                        foreach (ParamValueItemWithID item in monitorTableUnitProvider.ParamValues)
                        {
                            if (dicParams.ContainsKey(item.ParameterID))
                            {
                                foreach (var cell in dicParams[item.ParameterID])
                                {
                                    if (dgv.Columns.Count > cell.ColumnIndex &&
                                        dgv.Rows.Count > cell.RowIndex - 1)
                                    {
                                        object val = item.Value;
                                        if (dicChild.ContainsKey(cell))
                                        {
                                            string f = "";
                                            int? dec = ((MonitorTableParamNode)dicChild[cell]).DecNumber;
                                            if (dec != null) f = string.Format("N{0}", dec);
                                            val = ((double)val).ToString(f);
                                        }
                                        dgv[cell.ColumnIndex, cell.RowIndex - 1].Value = val;
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        dgv.ResumeLayout();
                    }
                }
            }
        }

        private void cmsTable_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = !EditMode;
            if (EditMode)
            {
                DataGridViewCell gcell;
                bool selected = dgv.CurrentCell != null;
                bool hasparam = false;
                if (selected)
                {
                    gcell = dgv.CurrentCell;
                    hasparam = (gcell.Tag != null && (gcell.Tag is Cell) && ((Cell)gcell.Tag).ParameterId != 0);
                }

                setLinkToolStripMenuItem.Enabled = selected;
                setParameterToolStripMenuItem.Enabled = selected && !hasparam;
                removeColumnToolStripMenuItem.Enabled = selected;
                clearLinkToolStripMenuItem.Enabled = selected;
                clearParameterToolStripMenuItem.Enabled = selected && hasparam;
            }
        }

        private void addColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColumnNameForm frm = new ColumnNameForm();
            
            if (frm.ShowDialog() == DialogResult.OK)
            {
                if (!dgv.Columns.Contains(frm.ColumnName))
                {
                    DataGridViewColumn column = new DataGridViewTextBoxColumn();

                    column.HeaderText = frm.ColumnName;
                    dgv.Columns.Add(column);
                    UpdateTable();
                }
                else
                    MessageBox.Show("Колонка с таким названием уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void removeColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentCell == null) return;

            if (MessageBox.Show("Удалить колонку?", "Удаление колонки", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                dgv.Columns.RemoveAt(dgv.SelectedCells[0].ColumnIndex);
                UpdateTable();
            }
        }

        private void setParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentCell == null) return;

            if (dgv.Rows[dgv.CurrentCell.RowIndex].IsNewRow)
            {
                dgv.Rows.Add();
                dgv.CurrentCell = dgv.Rows[dgv.CurrentCell.RowIndex - 1].Cells[dgv.CurrentCell.ColumnIndex];
            }

            SelectForm frm = new SelectForm(unitProvider.StructureProvider);
            frm.Filter.Add((int)UnitTypeId.Parameter);
            frm.Filter.Add((int)UnitTypeId.ManualParameter);
            frm.Filter.Add((int)UnitTypeId.TEP);
            frm.MultiSelect = false;
            frm.ShowDialog();
            if (frm.SelectedObjects != null && frm.SelectedObjects.Length > 0)
            {
                DataGridViewCell gcell = dgv.CurrentCell;
                Cell cell = null;

                if (gcell.Tag == null || !(gcell.Tag is Cell))
                {
                    cell = new Cell(gcell.RowIndex + 1, gcell.ColumnIndex);
                    gcell.Tag = cell;
                }
                else
                    cell = (Cell)gcell.Tag;

                if (cell != null)
                {
                    bool found = false;

                    cell.ParameterId = frm.SelectedObjects[0].Idnum;
                    cell.Text = frm.SelectedObjects[0].Text;
                    if (!dicParams.ContainsKey(cell.ParameterId))
                        dicParams.Add(cell.ParameterId, new List<Cell>());
                    if (!dicParams[cell.ParameterId].Contains(cell))
                        dicParams[cell.ParameterId].Add(cell);
                    foreach (var item in monitorNode.Parameters)
                    {
                        if (item.ParameterId == frm.SelectedObjects[0].Idnum)
                        {
                            dicChild[cell] = item;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        monitorNode.AddChildParam((ParameterNode)frm.SelectedObjects[0]);
                        foreach (var item in monitorNode.Parameters)
                        {
                            if (item.ParameterId == frm.SelectedObjects[0].Idnum)
                            {
                                dicChild[cell] = item;
                                break;
                            }
                        }
                    }
                }

                DataGridViewCell c = dgv.CurrentCell;
                dgv.CurrentCell = null;
                dgv.CurrentCell = c;
            }
            dgv.Refresh();
            UpdateTable();
        }

        private void clearParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentCell == null) return;

            Cell cell = GetCell();

            if (cell == null || cell.ParameterId == 0) return;

            if (MessageBox.Show("Удалить параметр?", "Удаление параметра", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                ClearParameter(cell);
            }
            dgv.Refresh();
            UpdateTable();
        }

        private void ClearParameter(Cell cell)
        {
            if (cell == null) return;
            if (dicParams.ContainsKey(cell.ParameterId))
            {
                dicParams[cell.ParameterId].Remove(cell);
                if (dicParams[cell.ParameterId].Count == 0)
                    dicParams.Remove(cell.ParameterId);
            }
            cell.ParameterId = 0;
            if (dicChild.ContainsKey(cell))
            {
                monitorNode.RemoveChildParam(dicChild[cell]);
                dicChild.Remove(cell);
            }
            dgv.Refresh();
            UpdateTable();
        }

        private void setLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentCell == null) return;

            if (dgv.Rows[dgv.CurrentCell.RowIndex].IsNewRow)
            {
                dgv.Rows.Add();
                dgv.CurrentCell = dgv.Rows[dgv.CurrentCell.RowIndex - 1].Cells[dgv.CurrentCell.ColumnIndex];
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Cell cell = GetCell();
                if (cell != null) cell.Link = openFileDialog.FileName;
                dgv.Refresh();
                UpdateTable();
            }
        }

        private Cell GetCell()
        {
            if (dgv.CurrentCell == null) return null;

            DataGridViewCell gcell = dgv.CurrentCell;
            Cell cell = null;

            if (gcell.Tag == null || !(gcell.Tag is Cell))
            {
                cell = new Cell(gcell.RowIndex + 1, gcell.ColumnIndex);
                gcell.Tag = cell;
            }
            else
                cell = (Cell)gcell.Tag;

            return cell;
        }

        private void clearLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentCell == null) return;

            Cell cell = GetCell();
            if (cell != null) cell.Link = "";
            dgv.Refresh();
            UpdateTable();
        }

        private void dgv_CurrentCellChanged(object sender, EventArgs e)
        {
            Cell cell = GetCell();

            if (SelectedControlChanged != null)
            {
                if (cell != null && dicChild.ContainsKey(cell))
                    SelectedControlChanged(dicChild[cell]);
                else
                    SelectedControlChanged(null);
            }
        }

        private void dgv_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            Cell cell;

            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {
                if (i >= dgv.Rows.Count) break;
                foreach (DataGridViewCell col in dgv.Rows[i].Cells)
                {
                    cell = GetCell();
                    if (cell != null) ClearParameter(cell);
                }
            }
            dgv.Refresh();
        }

        private void dgv_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Удалить строку?", "Удаление строки", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                e.Cancel = true;
        }

        private void dgv_Paint(object sender, PaintEventArgs e)
        {
            //
        }

        private void dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Cell cell = GetCell();

            if (cell != null)
            {
                object val = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (val != null)
                    cell.Text = val.ToString();
                else
                    cell.Text = "";
            }
            dgv.Refresh();
            UpdateTable();
        }

        private void dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridViewCell dgvCell;
            Cell cell;

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            dgvCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (dgvCell != null)
            {
                if (dgvCell.Tag != null && (cell = dgvCell.Tag as Cell) != null)
                {
                    if (EditMode)
                    {
                        if (cell.ParameterId != 0) dgvCell.Style.BackColor = Color.Yellow;
                        else dgvCell.Style.BackColor = Color.White;
                    }

                    if (!string.IsNullOrEmpty(cell.Link))
                    {
                        dgvCell.Style.Font = new Font(dgv.Font, FontStyle.Underline);
                        dgvCell.Style.ForeColor = Color.Blue;
                    }
                    else
                    {
                        dgvCell.Style.Font = new Font(dgv.Font, FontStyle.Regular);
                        dgvCell.Style.ForeColor = Color.Black;
                    }
                }
                else
                {
                    dgvCell.Style.BackColor = Color.White;
                    dgvCell.Style.Font = new Font(dgv.Font, FontStyle.Regular);
                    dgvCell.Style.ForeColor = Color.Black;
                }
            }
        }

        private void dgv_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (EditMode || e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridViewCell dgvCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Cell cell;
            if (dgvCell.Tag != null && (cell = dgvCell.Tag as Cell) != null)
            {
                if (!string.IsNullOrEmpty(cell.Link))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(cell.Link);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }
        }

        private void dgv_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void dgv_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (EditMode || e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                this.Cursor = Cursors.Default;
                return;
            }
            DataGridViewCell dgvCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Cell cell;
            if (dgvCell.Tag != null && (cell = dgvCell.Tag as Cell) != null
                && !string.IsNullOrEmpty(cell.Link))
                this.Cursor = Cursors.Hand;
            else
                this.Cursor = Cursors.Default;
        }
    }
}

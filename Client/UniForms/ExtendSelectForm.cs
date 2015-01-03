using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    public partial class ExtendSelectForm : TreeForm
    {
        public String[] ColumnList { get; set; }

        public UnitNode[] SelectedObjects
        {
            get
            {
                HashSet<UnitNode> nodes = new HashSet<UnitNode>();
                foreach (DataGridViewRow gridRow in dataGridView1.Rows)
                {
                    UnitNode node = gridRow.Tag as UnitNode;
                    nodes.Add(node);
                }
                return nodes.ToArray();
            }
            set
            {
                dataGridView1.Rows.Clear();
                foreach (var unitNode in value)
                {
                    AddUnitNode(unitNode);
                }
            }
        }

        public ExtendSelectForm(StructureProvider provider)
            : base(provider)
        {
            InitializeComponent();

            if (Program.MainForm != null)
                treeViewUnitObjects.ImageList = Program.MainForm.Icons;
        }

        private void AddUnitNode(UnitNode unitNode)
        {
            var gridRow = dataGridView1.Rows[dataGridView1.Rows.Add()];
            gridRow.Tag = unitNode;
            gridRow.Cells[docIndexColumn.Index].Value = unitNode.DocIndex;
            gridRow.Cells[nameColumn.Index].Value = unitNode.Text;
            gridRow.Cells[codeColumn.Index].Value = unitNode.Code;
        }

        private void MoveRows(int shift)
        {
            var movedRows = GetSelectedRows();
            var selectedCells = GetSelectedCells();

            dataGridView1.SuspendLayout();
            try
            {
                int minIndex = (from r in movedRows select r.Index).Min() + shift;
                int maxIndex = (from r in movedRows select r.Index).Max() + shift;

                if (0 <= minIndex && maxIndex < dataGridView1.Rows.Count)
                {
                    // sort rows before moving
                    var rows = shift > 0 ?
                        movedRows.OrderByDescending(r => r.Index) :
                        movedRows.OrderBy(r => r.Index);

                    foreach (var gridRow in rows)
                    {
                        int index = gridRow.Index;
                        int newIndex = index + shift;

                        dataGridView1.Rows.RemoveAt(index);
                        dataGridView1.Rows.Insert(newIndex, gridRow);
                    }

                    // restore selection
                    dataGridView1.ClearSelection();
                    foreach (var item in selectedCells)
                    {
                        item.Selected = true;
                    }
                }
            }
            finally
            {
                dataGridView1.ResumeLayout();
            }
        }

        private IEnumerable<DataGridViewRow> GetSelectedRows()
        {
            HashSet<DataGridViewRow> selectedRows = new HashSet<DataGridViewRow>();

            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow gridRow in dataGridView1.SelectedRows)
                {
                    selectedRows.Add(gridRow);
                }
            }
            else
            {
                foreach (DataGridViewCell gridCell in dataGridView1.SelectedCells)
                {
                    selectedRows.Add(dataGridView1.Rows[gridCell.RowIndex]);
                }
            }
            return selectedRows;
        }

        private IEnumerable<DataGridViewCell> GetSelectedCells()
        {
            HashSet<DataGridViewCell> selectedCells = new HashSet<DataGridViewCell>();

            foreach (DataGridViewCell item in dataGridView1.SelectedCells)
            {
                selectedCells.Add(item);
            }
            return selectedCells;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


        }

        private void addSelectedToolStripButton_Click(object sender, EventArgs e)
        {
            var nodes = treeViewUnitObjects.SelectedNodes;
            if (nodes != null)
            {
                foreach (var treeNode in nodes)
                {
                    var unitNode = strucProvider.GetUnitNode((int)treeNode.Tag);
                    AddUnitNode(unitNode);
                }
            }
        }

        private void removeSelectedToolStripButton_Click(object sender, EventArgs e)
        {
            HashSet<DataGridViewRow> rowToRemove = new HashSet<DataGridViewRow>();

            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow gridRow in dataGridView1.SelectedRows)
                {
                    rowToRemove.Add(gridRow);
                }
            }
            else
            {
                foreach (DataGridViewCell gridCell in dataGridView1.SelectedCells)
                {
                    rowToRemove.Add(dataGridView1.Rows[gridCell.RowIndex]);
                }
            }

            foreach (var gridRow in rowToRemove)
            {
                dataGridView1.Rows.Remove(gridRow);
            }
        }

        private void moveUpToolStripButton_Click(object sender, EventArgs e)
        {
            MoveRows(-1);
        }

        private void moveDownToolStripButton_Click(object sender, EventArgs e)
        {
            MoveRows(1);
        }

        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(UnitTreeNode)))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(UnitTreeNode)))
            {
                UnitTreeNode cont = ((UnitTreeNode)e.Data.GetData(typeof(UnitTreeNode)));
                if (cont.Node is ParameterNode || cont.Node is NormFuncNode)
                {
                    e.Effect = DragDropEffects.Copy;
                    AddUnitNode(cont.Node);
                }
                else e.Effect = DragDropEffects.None;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}

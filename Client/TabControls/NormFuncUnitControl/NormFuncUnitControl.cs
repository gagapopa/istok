using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.ClientCore.Utils;
using ZedGraph;

namespace COTES.ISTOK.Client
{
    partial class NormFuncUnitControl : BaseUnitControl
    {
        NormFuncNode funcNode = null;
        MultiDimensionalTable mdt;

        DimensionPropertyCollection copyDimensionData = null;
        DataTable currentTable = null;

        public NormFuncUnitControl(NormFuncUnitProvider unitProvider, bool editMode)
            : base(unitProvider)
        {
            InitializeComponent();
            EditMode = editMode;
            // по умолчанию выбрана текущая ревизия
            if (EditMode)
            {
                Revision = UnitProvider.GetRealRevision(RevisionInfo.Current);
            }
            else
            {
                Revision = RevisionInfo.Current;
            }
            //Init();

            //unitProvider.RemoteDataService.CurrentRevisionChanged += new EventHandler(RemoteDataService_CurrentRevisionChanged);
        }

        void unitProvider_CurrentRevisionChanged(object sender, EventArgs e)
        {
            if (funcNode != null && Revision == RevisionInfo.Current)
            {
                Init();
                if (EditMode)
                    DrawTree();
                else
                    DrawGraph();
            }
        }

        public NormFuncUnitControl(NormFuncUnitProvider unitProvider)
            : this(unitProvider, false)
        {
            //
        }

        public bool EditMode { get; set; }

        public RevisionInfo Revision { get; set; }

        private void Init()
        {
            splitContainer1.Panel1Collapsed = !EditMode;
            pgCalc.Visible = !EditMode;
            pgCalc.Dock = DockStyle.Fill;
            pgCalc.Left = pgCalc.Top = 0;
            dgv.Visible = EditMode;
            dgv.Dock = DockStyle.Fill;
            dgv.Left = dgv.Top = 0;

            if (funcNode != null)
            {
                // получить ревизию с учётом текущей и верхней ревизии
                RevisionInfo revision = UnitProvider.GetRealRevision(Revision);

                mdt = funcNode.GetMDTable(revision);

                NormFuncCalculator calc = new NormFuncCalculator(mdt);
                pgCalc.SelectedObject = calc;
            }
            else
                mdt = null;
        }

        public override void InitiateProcess()
        {
            if (UnitProvider is NormFuncUnitProvider)
            {
                NormFuncUnitProvider funcUnitProvider;
                funcUnitProvider = (NormFuncUnitProvider)UnitProvider;
                if (EditMode)
                {
                    funcNode = (NormFuncNode)funcUnitProvider.NewUnitNode;
                }
                else
                {
                    funcNode = (NormFuncNode)funcUnitProvider.UnitNode; 
                }

                Init();

                revisionToolStripComboBox.Items.Add(Revision);
                revisionToolStripComboBox.SelectedItem = Revision;
                DrawTree();
                if (tvDimensions.Nodes.Count > 0)
                {
                    tvDimensions.SelectedNode = tvDimensions.Nodes[0];
                    DrawGraph();
                }
                funcUnitProvider.CurrentRevisionChanged += unitProvider_CurrentRevisionChanged;
            }
        }

        protected override void DisposeControl()
        {
            base.DisposeControl();
            UnitProvider.CurrentRevisionChanged -= unitProvider_CurrentRevisionChanged;
        }

        public void UpdateNode()
        {
            if (mdt != null)
            {   
                // получить ревизию с учётом текущей и верхней ревизии
                RevisionInfo revision = UnitProvider.GetRealRevision(Revision);

                funcNode.SetMDTable(revision, mdt);
            }
        }

        private void DrawTree()
        {
            List<string> lstParents = new List<string>();
            TreeNode root = new TreeNode("График");
            TreeNode ptr = tvDimensions.SelectedNode;

            while (ptr != null)
            {
                lstParents.Add(ptr.Text);
                ptr = ptr.Parent;
            }

            tvDimensions.Nodes.Clear();
            DrawTreeRec(root, root);
            tvDimensions.Nodes.Add(root);
            UpdateNode(root);

            lstParents.Reverse();
            ptr = null;
            foreach (var item in lstParents)
            {
                TreeNodeCollection nodes;

                if (ptr == null)
                    nodes = tvDimensions.Nodes;
                else
                    nodes = ptr.Nodes;

                foreach (TreeNode node in nodes)
                {
                    if (node.Text == item)
                    {
                        ptr = node;
                        ptr.Expand();
                        break;
                    }
                }
            }

            if (ptr != null) tvDimensions.SelectedNode = ptr;
        }
        private void DrawTree(TreeNode node)
        {
            TreeNode parent, root;
            TreeNode ptr;

            if (node == null) return;
            if (node.Parent == null) return;

            parent = node.Parent;
            root = parent;
            while (root.Parent != null) root = root.Parent;

            ptr = node;
            parent.Nodes.Clear();
            double[] coords = GetCoordinates(parent);
            DrawTreeRec(root, parent, coords);

            foreach (TreeNode item in parent.Nodes)
                if (item.Text == ptr.Text)
                {
                    tvDimensions.SelectedNode = item;
                    break;
                }
        }
        private void DrawTreeRec(TreeNode root, TreeNode parent, params double[] coordinates)
        {
            try
            {
                TreeNode node;
                int res;

                if (mdt == null) return;

                res = mdt.DimensionInfo.Length - coordinates.Length;
                if (res < 1) return;

                double[] coords = new double[coordinates.Length + 1];
                coordinates.CopyTo(coords, 0);
                foreach (var item in mdt.GetDimension(coordinates))
                {
                    coords[coordinates.Length] = item;
                    if (res > 1)
                    {
                        DimensionInfo dInfo = mdt.DimensionInfo[res - 1];
                        DimensionValue dValue;
                        string txt;

                        txt = string.Format("{0} = {1} {2}", dInfo.Name, item.ToString(), dInfo.Measure);
                        node = new TreeNode(txt);
                        dValue = new DimensionValue(dInfo.Name, dInfo.Measure, item);
                        node.Tag = dValue;
                        DrawTreeRec(root, node, coords);
                        parent.Nodes.Add(node);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            return;
        }
        private void UpdateNode(TreeNode node)
        {
            DimensionPropertyCollection props;

            try
            {
                if (node != null)
                {
                    props = GetNodeData(node, true);
                    node.Tag = props;
                    if (props != null)
                    {
                        copyDimensionData = (DimensionPropertyCollection)props.Clone();
                        if (node.Parent == null)
                            pgProperties.SelectedObject = funcNode;
                        else
                            pgProperties.SelectedObject = props;
                        currentTable = props.Table;
                        SetDataSource(currentTable);
                        ChangeGraphAxes();
                        if (node.Nodes.Count > 0)
                            dgv.ReadOnly = true;
                        else
                            dgv.ReadOnly = false;
                    }
                    else
                        ClearSelection();
                }
                else
                    ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private void ClearSelection()
        {
            dgv.Columns.Clear();
            dgv.DataSource = null;
            copyDimensionData = null;
            pgProperties.SelectedObject = null;
            currentTable = null;
        }
        private void ChangeGraphAxes()
        {
            DimensionInfo dInfo;
            NormFuncNode node;
            double[] coords;
            string title;

            node = funcNode;
            title = node.Text;

            title = node.GetAttribute("TableName", Revision);
            if (tvDimensions.SelectedNode != null)
            {
                coords = GetCoordinates(tvDimensions.SelectedNode);
                if (coords != null && coords.Length > 0)
                {
                    if (!string.IsNullOrEmpty(title)) title += ", ";
                    title += "при";
                    for (int i = 0; i < coords.Length; i++)
                    {
                        dInfo = mdt.DimensionInfo[mdt.DimensionInfo.Length - i - 1];
                        title += "\n" + dInfo.Name + " = " + coords[i].ToString() + dInfo.Measure;
                    }
                }
            }
            graph.GraphPane.Title.Text = title;
            if (mdt.DimensionInfo.Length > 0)
            {
                string txt;

                txt = mdt.DimensionInfo[0].Name;
                if (!string.IsNullOrEmpty(mdt.DimensionInfo[0].Measure))
                    txt += ", " + mdt.DimensionInfo[0].Measure;
                graph.GraphPane.XAxis.Title.Text = txt;
            }
            graph.GraphPane.YAxis.Title.Text = node.ResultUnit;
        }
        private DimensionPropertyCollection GetNodeData(TreeNode node)
        {
            return GetNodeData(node, false);
        }
        private DimensionPropertyCollection GetNodeData(TreeNode node, bool update)
        {
            DimensionPropertyCollection props;
            DimensionInfo dInfo;
            NormFuncNode cnode;
            TreeNode ptr;

            double[] coords;
            int dim = 0;

            if (node == null) throw new ArgumentNullException("node");

            coords = GetCoordinates(node);
            props = new DimensionPropertyCollection();

            cnode = funcNode;
            if (cnode == null) throw new Exception("Wrong node type");
            props.Table = mdt.GetTable(coords);

            if (coords.Length == 0)
            {
                for (int i = mdt.DimensionInfo.Length - 1; i >= 0; i--)
                    props.AddDimensionInfo((DimensionInfo)mdt.DimensionInfo[i].Clone());
            }
            else
            {
                double value;

                ptr = node.Parent;
                while (ptr != null)
                {
                    ptr = ptr.Parent;
                    dim++;
                }

                dim = mdt.DimensionInfo.Length - dim;
                if (dim > mdt.DimensionInfo.Length - 1) throw new Exception("Измерение не найдено.");
                dInfo = mdt.DimensionInfo[dim];

                value = coords[coords.Length - 1];
                if (update)
                    node.Text = string.Format("{0} = {1} {2}", dInfo.Name, value.ToString(), dInfo.Measure);
                props.AddDimensionInfo(new DimensionValue(dInfo.Name, dInfo.Measure, value));
            }

            return props;
        }
        private double[] GetCoordinates(TreeNode node)
        {
            List<double> lstCoordinates = new List<double>();

            if (node == null) throw new ArgumentNullException("node");

            if (node.Parent != null)
                lstCoordinates.AddRange(GetCoordinates(node.Parent));

            if (node.Tag != null)
            {
                if (node.Tag is DimensionPropertyCollection)
                {
                    DimensionPropertyCollection props = (DimensionPropertyCollection)node.Tag;

                    if (props.DimensionInfo.Length == 1 && props.DimensionInfo[0] is DimensionValue)
                        lstCoordinates.Add(((DimensionValue)props.DimensionInfo[0]).Value);
                }
                else
                    if (node.Tag is DimensionValue)
                    {
                        lstCoordinates.Add(((DimensionValue)node.Tag).Value);
                    }
            }

            return lstCoordinates.ToArray();
        }

        public void AddDimension(string name, string measure, double value)
        {
            if (mdt == null) throw new ArgumentNullException("mdt");
            mdt.AddDimension(name, measure, value);
            DrawTree();
        }

        private void tvDimensions_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (e.Node != null)
                {
                    if (e.Node.Tag == null || e.Node.Tag is DimensionValue) e.Node.Tag = GetNodeData(e.Node);
                    if (e.Node.Tag is DimensionPropertyCollection)
                    {
                        DimensionPropertyCollection tmp = (DimensionPropertyCollection)e.Node.Tag;
                        if (e.Node.Parent == null)
                            pgProperties.SelectedObject = funcNode;
                        else
                            pgProperties.SelectedObject = e.Node.Tag;
                        copyDimensionData = (DimensionPropertyCollection)tmp.Clone();
                        currentTable = tmp.Table;
                        ChangeGraphAxes();
                    }
                    else
                        ClearSelection();
                    SetDataSource(currentTable);
                    if (e.Node.Nodes.Count > 0 || (mdt != null && mdt.DimensionInfo.Length == 0))
                        dgv.ReadOnly = true;
                    else
                        dgv.ReadOnly = false;
                }
                else
                {
                    ClearSelection();
                }
                DrawGraph();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
        private void DrawGraph()
        {
            NormFuncGraphParam param;
            NormFuncDrawer drawer;

            try
            {
                param = new NormFuncGraphParam();

                param.mdtable = mdt;
                if (mdt != null && mdt.DimensionInfo.Length > 1)
                    param.strAxeZ = mdt.DimensionInfo[1].Name;
                param.delta_x = 1;//double.Parse(txtStepX.Text);
                param.delta_y = 1;//double.Parse(txtStepY.Text);
                if (tvDimensions.SelectedNode != null)
                    param.coordinates = GetCoordinates(tvDimensions.SelectedNode);
                else
                    param.coordinates = null;
                drawer = new NormFuncDrawer(param);

                NormFuncDrawer.Clear(graph.MasterPane[0]);
                if (mdt != null && param.coordinates != null && param.coordinates.Length >= mdt.DimensionInfo.Length - 2)
                    drawer.Draw(graph.MasterPane[0]);
                graph.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void SetDataSource(object source)
        {
            dgv.DataSource = null;
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            dgv.DataSource = source;
            dgv.CurrentCell = null;//исправляет косяк при нажатии Tab на последней ячейке
        }

        private void tvDimensions_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = tvDimensions.SelectedNode;

            if (tvDimensions.SelectedNode != null
                && tvDimensions.SelectedNode.Tag != null
                && tvDimensions.SelectedNode.Tag is DimensionPropertyCollection)
            {
                DimensionPropertyCollection props = (DimensionPropertyCollection)tvDimensions.SelectedNode.Tag;
                DataTable table = props.Table;

                ClearSelection();
            }
            else
            {
                copyDimensionData = null;
            }
        }

        private void ApplyTableChanges(DataTable table)
        {
            List<double> lstParams = new List<double>();
            double[] arrParamsY;
            double[] arrCoords;
            double value;
            bool changed = false;
            int i, j;
            double val_x;

            if (table == null) return;

            arrParamsY = GetCoordinates(tvDimensions.SelectedNode);
            arrCoords = new double[arrParamsY.Length + 1];
            Array.Copy(arrParamsY, arrCoords, arrParamsY.Length);
            
            lstParams.Clear();
            if (dgv.Rows.Count > 0)
            {
                for (i = 0; i < dgv.Columns.Count; i++)
                {
                    DataGridViewColumn item = dgv.Columns[i];
                    try
                    {
                        value = (double)dgv.Rows[0].Cells[i].Value;
                    }
                    catch (InvalidCastException) { value = double.NaN; }

                    if (!double.IsNaN(value))
                    {
                        if (!item.Visible)
                        {
                            arrCoords[arrCoords.Length - 1] = value;
                            mdt.RemoveValue(arrCoords);
                            table.Columns.Remove(item.Name);
                            i--;
                            changed = true;
                        }
                        else
                            lstParams.Add(value);
                    }
                }
            }

            List<double[]> lstDelete = new List<double[]>();

            if (table.Rows.Count > 0)
            {
                for (j = 0; j < table.Columns.Count; j++)
                {
                    try
                    {
                        val_x = (double)table.Rows[0][j];
                    }
                    catch (InvalidCastException) { val_x = double.NaN; }

                    for (i = 1; i < table.Rows.Count; i++)
                    {
                        double[] nCoords = new double[arrCoords.Length];

                        try
                        {
                            value = (double)table.Rows[i][j];
                        }
                        catch (InvalidCastException)
                        {
                            value = double.NaN;
                        }

                        arrCoords[arrCoords.Length - 1] = val_x;
                        if (!double.IsNaN(value))
                        {
                            Array.Copy(arrCoords, nCoords, arrCoords.Length);
                            Array.Reverse(nCoords);
                            mdt.SetValue(value, nCoords);
                        }
                        else
                        {
                            lstDelete.Add(arrCoords);
                        }
                        changed = true;
                    }
                }
                foreach (var item in lstDelete)
                {
                    double[] nCoords = new double[item.Length - 1];

                    Array.Copy(item, nCoords, item.Length - 1);
                    if (mdt.GetDimension(nCoords).Length > 1)
                        mdt.RemoveValue(item);
                    else
                    {
                        nCoords = new double[item.Length];

                        Array.Copy(item, nCoords, item.Length);
                        Array.Reverse(nCoords);
                        mdt.SetValue(0f, nCoords);
                    }
                }
                mdt.FilterDimension(lstParams.ToArray(), arrParamsY);

                table.AcceptChanges();
                if (changed)
                {
                    if (tvDimensions.SelectedNode.Parent != null)
                        UpdateNode(tvDimensions.SelectedNode.Parent);
                    UpdateNode(tvDimensions.SelectedNode);
                }
            }
        }

        private double GetViewValue(DataView dataview, int row, int column, bool currentvalue)
        {
            double value;

            if (dataview == null) throw new ArgumentNullException("dataview");

            try
            {
                if (currentvalue)
                {
                    //value = (double)dataview[row].Row[column];
                    value = double.Parse(dataview[row].Row[column].ToString());
                }
                else
                {
                    //value = (double)(dataview[row][column]);
                    value = double.Parse(dataview[row][column].ToString());
                }
            }
            catch (FormatException) { value = double.NaN; }

            return value;
        }

        private void ApplyDimensionChanges()
        {
            DimensionPropertyCollection props;
            DimensionInfo[] src;
            DimensionInfo[] dest;
            DimensionInfo[] copy;
            List<double> lstCoords = new List<double>();
            bool needRedraw = false;
            int i, j;

            try
            {
                if (tvDimensions.SelectedNode == null
                    || tvDimensions.SelectedNode.Tag == null
                    || !(tvDimensions.SelectedNode.Tag is DimensionPropertyCollection)
                    || copyDimensionData == null)
                    return;

                props = tvDimensions.SelectedNode.Tag as DimensionPropertyCollection;

                dest = mdt.DimensionInfo;
                src = props.DimensionInfo;
                copy = copyDimensionData.DimensionInfo;
                
                for (i = 0; i < dest.Length; i++)
                {
                    for (j = 0; j < copy.Length; j++)
                    {
                        if (dest[i].Equals(copy[j]))
                        {
                            if (!dest[i].Equals(src[j]))
                            {
                                dest[i].Measure = src[j].Measure;
                                dest[i].Name = src[j].Name;
                                needRedraw = true;
                            }
                            if (copy[j] is DimensionValue)
                            {
                                DimensionValue sval = (DimensionValue)src[j];
                                DimensionValue cval = (DimensionValue)copy[j];

                                if (sval.Value != cval.Value)
                                {
                                    lstCoords.AddRange(GetCoordinates(tvDimensions.SelectedNode.Parent));
                                    lstCoords.Add(cval.Value);

                                    mdt.ChangeDimensionValue(sval.Value, lstCoords.ToArray());
                                }
                            }
                            break;
                        }
                    }
                }

                ChangeGraphAxes();
                if (needRedraw)
                {
                    DrawTree();
                }
                else
                {
                    if (tvDimensions.SelectedNode != null)
                        UpdateNode(tvDimensions.SelectedNode);
                }

                if (tvDimensions.SelectedNode != null)
                {
                    props = tvDimensions.SelectedNode.Tag as DimensionPropertyCollection;
                    copyDimensionData = (DimensionPropertyCollection)props.Clone();
                }
            }
            catch (Exception ex)
            {
                tvDimensions.SelectedNode.Tag = copyDimensionData;
                UpdateNode(tvDimensions.SelectedNode);
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }
        private void DiscardDimensionChanges()
        {
            if (tvDimensions.SelectedNode != null)
                tvDimensions.SelectedNode.Tag = copyDimensionData.Clone();
        }

        private void dgv_Leave(object sender, EventArgs e)
        {
            if (!dgv.ReadOnly)
            {
                ApplyTableChanges(currentTable);
                DrawGraph();
            }
        }

        private void dgv_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            try
            {
                e.Value = double.Parse(e.Value.ToString());
                e.ParsingApplied = true;
            }
            catch (FormatException)
            {
                e.ParsingApplied = false;
            }
        }

        private void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if ((e.Context & DataGridViewDataErrorContexts.Parsing) != 0)
            {
                MessageBox.Show("Некорректный формат", "Ошибка");
            }
            e.Cancel = true;
        }

        private void dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int dim = 0;

            if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value is DBNull)
            {
                foreach (DataGridViewColumn item in dgv.Columns)
                    if (item.Visible) dim++;
                if (dim == 1)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0f;
                }
            }
        }

        private void mnuRight_Opening(object sender, CancelEventArgs e)
        {
            if (!EditMode || (mdt != null && mdt.DimensionInfo.Length == 0))
            {
                e.Cancel = true;
                return;
            }
            if (dgv.DataSource == null)
            {
                deleteColumnToolStripMenuItem.Enabled = false;
                addColumnToolStripMenuItem.Enabled = false;
                deleteTableToolStripMenuItem.Enabled = false;
                graphToolStripMenuItem.Enabled = false;
            }
            else
            {
                graphToolStripMenuItem.Enabled = true;
                if (dgv.ReadOnly)
                {
                    deleteColumnToolStripMenuItem.Enabled = false;
                    addColumnToolStripMenuItem.Enabled = false;
                    deleteTableToolStripMenuItem.Enabled = false;
                }
                else
                {
                    addColumnToolStripMenuItem.Enabled = true;
                    deleteTableToolStripMenuItem.Enabled = true;

                    if (dgv.CurrentCell == null)
                        deleteColumnToolStripMenuItem.Enabled = false;
                    else
                        deleteColumnToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void deleteTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTable == null) return;

            if (MessageBox.Show("Вы уверены?", "Очистка таблицы", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                bool found = false;

                foreach (DataGridViewColumn item in dgv.Columns)
                {
                    if (item.Visible)
                    {
                        if (found)
                        {
                            item.Visible = false;
                        }
                        else
                        {
                            foreach (DataGridViewRow row in dgv.Rows)
                            {
                                row.Cells[item.Index].Value = 0f;
                            }
                            found = true;
                        }
                    }
                }
                ApplyTableChanges(currentTable);
            }
        }

        private DataRow AddRow(DataTable table)
        {
            DataRow row = null;

            if (table == null) return row;

            try
            {
                DataRow tmp_row = null;

                if (table.Rows.Count > 0) tmp_row = table.Rows[table.Rows.Count - 1];
                row = table.NewRow();
                if (tmp_row != null)
                {
                    if (tmp_row[0] is DBNull)
                        row[0] = 1f;
                    else
                    {
                        try
                        {
                            row[0] = double.Parse(tmp_row[0].ToString()) + 1f;
                        }
                        catch { row[0] = 1f; }
                    }
                }
                else
                    row[0] = DBNull.Value;
                table.Rows.Add(row);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return row;
        }
        private DataColumn AddColumn(DataTable table)
        {
            DataColumn col = null;

            if (table == null) return col;

            try
            {
                string strName = "0";

                if (table.Columns.Count != 0)
                    strName = table.Columns[table.Columns.Count - 1].ColumnName;
                while (table.Columns.Contains(strName))
                    strName = Convert.ToString(double.Parse(strName) + 1f);
                col = table.Columns.Add(strName, typeof(double));
                if (table.Rows.Count > 0)
                {
                    try
                    {
                        table.Rows[0][col] = double.Parse(col.ColumnName);
                    }
                    catch (FormatException) { table.Rows[0][col] = DBNull.Value; }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return col;
        }

        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddRow(currentTable);
        }

        private void addColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddColumn(currentTable);
        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTable == null || dgv.CurrentCell == null) return;

            DataView dv = new DataView(currentTable, "", "", DataViewRowState.CurrentRows);

            dv[dgv.CurrentRow.Index].Row.Delete();
        }

        private void deleteColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = 0;
            if (currentTable == null || dgv.CurrentCell == null) return;

            foreach (DataGridViewColumn item in dgv.Columns) if (item.Visible) count++;

            if (count > 1)
                dgv.Columns[dgv.CurrentCell.ColumnIndex].Visible = false;
            else
                MessageBox.Show("Нельзя удалять единственный столбец.", "Ошибка");
        }

        private void graphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawGraph();
        }

        private void showReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //CTableFunction func = new CTableFunction(dataObject.Idnum.ToString());
            //ShowRelations(this, func, FormulaRelation.Reference);
        }

        private void addDimensionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NormFuncAddDimensionForm frmAdd = new NormFuncAddDimensionForm();

            if (mdt == null) throw new Exception("Таблица не задана.");

            frmAdd.ShowDialog();
            if (frmAdd.DialogResult != DialogResult.OK) return;

            try
            {
                foreach (var item in mdt.DimensionInfo)
                    if (item.Name == frmAdd.DimensionName)
                    {
                        MessageBox.Show("Такое измерение уже существует.");
                        return;
                    }

                mdt.AddDimension(frmAdd.DimensionName, frmAdd.DimensionMeasure, frmAdd.DimensionValue);
                DrawTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void deleteDimensionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DimensionInfo dInfo;
            double[] coords;

            try
            {
                if (tvDimensions.SelectedNode == null) return;

                coords = GetCoordinates(tvDimensions.SelectedNode);

                if (coords.Length <= mdt.DimensionInfo.Length)
                {
                    if (coords.Length == 0 && mdt.DimensionInfo.Length > 0)
                        dInfo = mdt.DimensionInfo[mdt.DimensionInfo.Length - 1];
                    else
                        dInfo = mdt.DimensionInfo[mdt.DimensionInfo.Length - coords.Length];
                    mdt.RemoveDimension(dInfo);
                    DrawTree();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void addBranchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NormFuncAddBranchForm frmAdd = new NormFuncAddBranchForm();
            List<double> lstCoords = new List<double>();
            double[] coords;

            try
            {
                coords = GetCoordinates(tvDimensions.SelectedNode);
                if (coords == null || coords.Length == 0)
                {
                    MessageBox.Show("Нельзя добавить ветку к этому узлу.", "Ошибка");
                    return;
                }

                if (frmAdd.ShowDialog() == DialogResult.OK)
                {
                    lstCoords.Add(0f);

                    for (int i = 0; i < mdt.DimensionInfo.Length - coords.Length - 1; i++)
                        lstCoords.Add(0f);
                    Array.Reverse(coords);
                    if (coords[0] == frmAdd.Value)
                    {
                        MessageBox.Show("Такая ветка уже существует.", "Ошибка");
                        return;
                    }
                    coords[0] = frmAdd.Value;
                    lstCoords.AddRange(coords);

                    mdt.SetValue(0, lstCoords.ToArray());
                    DrawTree(tvDimensions.SelectedNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка");
            }
        }

        private void removeBranchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tvDimensions.SelectedNode != null)
            {
                try
                {
                    mdt.RemoveValue(GetCoordinates(tvDimensions.SelectedNode));
                    DrawTree();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка");
                }
            }
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrawTree();
        }

        private void pgProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            ApplyDimensionChanges();
        }

        private void pgCalc_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            pgCalc.Refresh();
        }

        private void mnuTree_Opening(object sender, CancelEventArgs e)
        {
            if (mdt != null)
            {
                deleteDimensionToolStripMenuItem.Enabled = mdt.DimensionInfo.Length != 0;
                removeBranchToolStripMenuItem.Enabled = mdt.DimensionInfo.Length != 0;
            }
        }

        private void revisionToolStripComboBox_DropDown(object sender, EventArgs e)
        {
            Object selectedObject = revisionToolStripComboBox.SelectedItem;

            revisionToolStripComboBox.Items.Clear();

            foreach (var revision in unitProvider.StructureProvider.Session.Revisions)
            {
                revisionToolStripComboBox.Items.Add(revision);
            }
            if (!EditMode)
            {
                revisionToolStripComboBox.Items.Add(RevisionInfo.Current);
            }

            revisionToolStripComboBox.SelectedItem = selectedObject;
        }

        private void revisionToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RevisionInfo revision = revisionToolStripComboBox.SelectedItem as RevisionInfo;

            if (revision != null && revision != Revision)
            {
                //if (EditMode)
                //{
                //    UpdateNode();
                //}
                Revision = revision;
                Init();
                if (EditMode)
                    DrawTree();
                else
                    DrawGraph();
            }
        }
    }    
}

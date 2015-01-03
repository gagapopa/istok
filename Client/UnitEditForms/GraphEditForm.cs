using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using System.Drawing.Drawing2D;
using COTES.ISTOK.Client;
using System.Linq;
using System.Reflection;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    partial class GraphEditForm : ParamEditForm
    {
        private bool isHistogram;

        public GraphEditForm(StructureProvider strucProvider, UnitNode unitNode)
            : base(strucProvider, unitNode)
        {
            InitializeComponent();
            //Init();
        }
        public GraphEditForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
            //Init();
        }

        private void Init()
        {
            //GraphUnitProvider gup = new GraphUnitProvider(UnitNode as GraphNode);
            //pgNode.SelectedObject = gup.CreateDynamicObject();
            isHistogram = UnitNode is HistogramNode;
            dgv.Columns["clmLine"].Visible = !isHistogram;
            pgNode.SelectedObject = UnitNode;

            typeof(Control).InvokeMember("DoubleBuffered",
                  BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                  null, dgv, new object[] { true });
            LoadParameters();
        }

        private void LoadParameters()
        {
            DataGridViewRow row;

            dgv.Rows.Clear();
            dgv.SuspendLayout();

            var lst = from elem in UnitNode.Parameters
                      orderby elem.SortIndex
                      select elem;
                      
            foreach (var item in lst)
            {
                row = dgv.Rows[dgv.Rows.Add()];
                row.Cells["clmParam"].Value = item.Text;
                if (!isHistogram)
                    row.Cells["clmLine"].Value = DrawCellImage(row.Cells["clmLine"], item as GraphParamNode);
                row.Tag = item;
            }
            dgv.ResumeLayout();
            dgv_SelectionChanged(dgv, EventArgs.Empty);
        }
        private void LoadParameters(ChildParamNode node)
        {
            foreach (DataGridViewRow item in dgv.Rows)
            {
                if (item.Tag != null && item.Tag.Equals(node))
                {
                    item.Cells["clmParam"].Value = node.Text;
                    if (!isHistogram)
                        item.Cells["clmLine"].Value = DrawCellImage(item.Cells["clmLine"], node as GraphParamNode);
                    item.Tag = node;
                    break;
                }
            }
        }

        private Image DrawCellImage(DataGridViewCell cell, GraphParamNode node)
        {
            Bitmap bmp = new Bitmap(cell.Size.Width, cell.Size.Height);
            Graphics g = Graphics.FromImage(bmp);
            Pen pen = new Pen(node.LineColor);
            pen.DashStyle = (DashStyle)node.LineStyle;
            pen.Width = (float)node.LineWidth;
            int y = bmp.Height / 2;

            g.DrawLine(pen, 3, y, bmp.Width - 3, y);
            
            GraphicsPath nodeSymbol = MakePath(node.LineSymbol, 10.0f);

            if (nodeSymbol.PointCount > 0)
            {
                GraphicsPath path;
                PointF[] points = new PointF[nodeSymbol.PointCount];
                byte[] types = new byte[nodeSymbol.PointCount];
                for (int i = 0; i < nodeSymbol.PointCount; i++)
                {
                    points[i].X = nodeSymbol.PathPoints[i].X + bmp.Width / 2;
                    points[i].Y = nodeSymbol.PathPoints[i].Y + bmp.Height / 2;
                    types[i] = nodeSymbol.PathTypes[i];
                }
                path = new GraphicsPath(points, types);
                g.FillPath(new SolidBrush(Color.White), path);
                g.DrawPath(new Pen(new SolidBrush(node.LineColor)), path);
            }

            return (Image)bmp;
        }

        private GraphicsPath MakePath(LineSymbolType linetype, float size)
        {
            ZedGraph.Symbol symbol = new ZedGraph.Symbol();
            GraphicsPath res;

            symbol.Type = (ZedGraph.SymbolType)((int)linetype);
            symbol.Size = size;
            res = symbol.MakePath(null, 1.0f);
            return res;
        }

        private void GraphEditForm_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void deleteParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Удалить параметры?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    List<DataGridViewRow> lstRows = new List<DataGridViewRow>();
                    List<ChildParamNode> lstParams = new List<ChildParamNode>(this.UnitNode.Parameters);
                    foreach (DataGridViewRow item in dgv.SelectedRows)
                    {
                        lstParams.Remove(item.Tag as ChildParamNode);
                        lstRows.Add(item);
                    }
                    foreach (var item in lstRows)
                        dgv.Rows.Remove(item);
                    this.UnitNode.Parameters = lstParams.ToArray();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            List<object> lstObjs = new List<object>();

            foreach (DataGridViewRow item in dgv.SelectedRows)
                if (item.Tag != null) lstObjs.Add(item.Tag);
            if (lstObjs.Count > 0)
                pgParameter.SelectedObjects = lstObjs.ToArray();
            else
                pgParameter.SelectedObjects = null;
        }

        private void addParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectForm uni = new SelectForm(strucProvider);
            TypeFilter filter = new TypeFilter();

            filter.Add((int)UnitTypeId.Parameter);
            filter.Add((int)UnitTypeId.ManualParameter);
            filter.Add((int)UnitTypeId.TEP);
            uni.Filter = filter;
            //uni.MdiParent = this.MdiParent;
            uni.ShowDialog();

            if (uni.SelectedObjects != null)
            {
                foreach (var item in uni.SelectedObjects)
                    if (item is ParameterNode) UnitNode.AddChildParam((ParameterNode)item);
                LoadParameters();
            }
        }

        private void pgParameter_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            LoadParameters(pgParameter.SelectedObject as ChildParamNode);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            ChangeParamSort(1);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            ChangeParamSort(-1);
        }

        private void ChangeParamSort(int direction)
        {
            List<DataGridViewRow> lst = new List<DataGridViewRow>();
            DataGridViewRow target;
            ChildParamNode param;
            int target_idx, row_idx;

            if (dgv.SelectedRows == null) return;

            try
            {
                dgv.SuspendLayout();
                foreach (DataGridViewRow row in dgv.SelectedRows) lst.Add(row);
                lst.Sort((Comparison<DataGridViewRow>)((DataGridViewRow x, DataGridViewRow y)
                    => -direction / Math.Abs(direction) * x.Index.CompareTo(y.Index)));
                
                foreach (var row in lst)
                {
                    row_idx = row.Index;
                    target_idx = row.Index + direction;
                    if (target_idx < 0 || target_idx >= dgv.Rows.Count) continue;
                    target = dgv.Rows[target_idx];
                    if (!lst.Contains(target))
                    {
                        dgv.Rows.Remove(row);
                        dgv.Rows.Insert(target_idx, row);
                        param = row.Tag as ChildParamNode;
                        if (param != null) param.SortIndex += direction;
                        dgv.Rows.Remove(target);
                        dgv.Rows.Insert(row_idx, target);
                        param = target.Tag as ChildParamNode;
                        if (param != null) param.SortIndex -= direction;
                        row.Selected = true;
                    }
                }
                foreach (DataGridViewRow item in dgv.SelectedRows)
                {
                    if (!lst.Contains(item)) item.Selected = false;
                }
            }
            finally
            {
                dgv.ResumeLayout();
            }
        }
    }
}

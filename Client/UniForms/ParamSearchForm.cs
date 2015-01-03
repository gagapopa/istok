using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using System.Text.RegularExpressions;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    public partial class ParamSearchForm : BaseAsyncWorkForm
    {
        Form frmFuncEdit = null;
        private int parent_id;
        private ParameterFilter p_filter = new ParameterFilter();

        //public event ChoiceHandler Choosed = null;

        int count = 0;

        public ParamSearchForm(StructureProvider strucProvider, int parentid)
            : base(strucProvider)
        {
            InitializeComponent();
            Init();
            //lblCount.Alignment = ToolStripItemAlignment.Right;
            //tabControl1.TabPages.Remove(tabControl1.TabPages["tpProperties"]);
            SetCount();

            parent_id = parentid;
            tvParameters.ImageList = Program.MainForm.Icons;
        }
        public ParamSearchForm(StructureProvider strucProvider, int parentid, ParameterFilter filter)
            : this(strucProvider, parentid)
        {
            Init();
            Filter = filter;
        }

        public ParameterFilter Filter
        {
            get { return p_filter; }
            set
            {
                p_filter = value;
                if (p_filter == null)
                {
                    p_filter = new ParameterFilter();
                    return;
                }

                txtCode.Text = p_filter.Code;
                txtName.Text = p_filter.Name;
                cbxReg.Checked = p_filter.UseReg;
                cbxWhole.Checked = p_filter.WholeWord;
            }
        }

        public int ParentId
        {
            get { return parent_id; }
            set { parent_id = value; }
        }

        private void Init()
        {
#if DEBUG
            chbxShaitan.Visible = true;
#else
            chbxShaitan.Visible = false;
#endif
        }

        private void LockControls()
        {
            btnSearch.Enabled = false;
        }
        private void UnlockControls()
        {
            btnSearch.Enabled = true;
            UpdateFormula();
        }

        private void AddResult(object res)
        {
            UnitNode[] nodes = res as UnitNode[];
            UnitTreeNode treenode;
            string strCode;

            lock (tvParameters)
            {
                tvParameters.BeginUpdate();

                foreach (var item in nodes)
                {
                    strCode = string.IsNullOrEmpty(item.Code) ? "" : " [" + item.Code + "]";
                    treenode = new UnitTreeNode();
                    treenode.Text = item.FullName + strCode;
                    treenode.Node = item;
                    treenode.Tag = item;
                    treenode.ImageKey = ((int)item.Typ).ToString();
                    treenode.SelectedImageKey = ((int)item.Typ).ToString();
                    tvParameters.Nodes.Add(treenode);
                }
                tvParameters.EndUpdate();
            }
            count += nodes.Length;
            SetCount();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ParameterFilter filter;
            
            try
            {
                count = 0;
                tvParameters.Nodes.Clear();
                
                filter = new ParameterFilter(txtName.Text, txtCode.Text);
                filter.UseReg = cbxReg.Checked;
                filter.WholeWord = cbxWhole.Checked;
                if (!string.IsNullOrEmpty(txtPropertyName.Text))
                {
                    filter.AddProperty(txtPropertyName.Text, txtPropertyValue.Text);
                    filter.UseMustExist = cbxMustExist.Checked;
                }

                LockControls();
                //AsyncOperationWatcher<Object> watcher = 
                var nodes = strucProvider.GetUnitNodesFiltered(parent_id, filter);
                //watcher.AddStartHandler(() => { if (this.IsHandleCreated)this.Invoke((Action)LockControls); });
                //watcher.AddFinishHandler(() => { if (this.IsHandleCreated)this.Invoke((Action)UnlockControls); });
                //watcher.AddValueRecivedHandler((object nodes) => { if (this.IsHandleCreated) this.Invoke((Action<object>)AddResult, nodes); });
                //RunWatcher(watcher);
                AddResult(nodes);
                UnlockControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void SetCount()
        {
            //lblCount.Text = "Количество: " + count.ToString();
            Text = "Поиск (найдено: " + count.ToString() + ")";
        }

        private void tvParameters_DoubleClick(object sender, EventArgs e)
        {
            if (!splitContainer1.Panel2Collapsed) //Шайтан включен
                ShowEditor();
            //if (tvParameters.SelectedNode != null && Choosed != null)
            //{
            //    ServerNode item = (ServerNode)tvParameters.SelectedNode.Tag;
            //    Choosed(this, new ChoiceEventArgs(item));
            //}
        }

        private void cbxWhole_CheckedChanged(object sender, EventArgs e)
        {
            SetFilter();
        }

        private void SetFilter()
        {
            p_filter.Name = txtName.Text;
            p_filter.Code = txtCode.Text;
            p_filter.UseReg = cbxReg.Checked;
            p_filter.WholeWord = cbxWhole.Checked;
        }

        private void cbxReg_CheckedChanged(object sender, EventArgs e)
        {
            SetFilter();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            SetFilter();
        }

        private void txtCode_TextChanged(object sender, EventArgs e)
        {
            SetFilter();
        }

        private void param_search_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (e.CloseReason == CloseReason.UserClosing)
            //{
            //    e.Cancel = true;
            //    this.Hide();
            //}
        }

        //public new void Show(IWin32Window owner)
        //{
        //    tvParameters.Nodes.Clear();
        //    if (!this.Visible)
        //        base.Show(owner);
        //}

        private void txtPropertyName_TextChanged(object sender, EventArgs e)
        {
            txtPropertyValue.Enabled = !string.IsNullOrEmpty(txtPropertyName.Text);
        }

        private void tvParameters_MouseMove(object sender, MouseEventArgs e)
        {
            //UnitNode node;

            //if (e.Button != MouseButtons.Left)
            //    return;

            //if (tvParameters.SelectedNode != null &&
            //        tvParameters.SelectedNode.Tag != null &&
            //        (tvParameters.SelectedNode.Tag is UnitNode))
            //{
            //    node = tvParameters.SelectedNode.Tag as UnitNode;
            //    if (!string.IsNullOrEmpty(node.Code))
            //    {
            //        tvParameters.DoDragDrop(node.Code,
            //            DragDropEffects.Copy | DragDropEffects.Move);
            //    }
            //}
            //if (tvParameters.SelectedNode != null &&
            //        tvParameters.SelectedNode is UnitTreeNode)
            //{
            //    tvParameters.DoDragDrop((UnitTreeNode)tvParameters.SelectedNode,
            //            DragDropEffects.Copy | DragDropEffects.Move);
            //}
        }

        private void tvParameters_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    tvParameters.SelectedNode = tvParameters.GetNodeAt(e.Location);
            //}
            //else
            //    if (e.Button == MouseButtons.Middle)
            //    {
            //        UnitNode node;

            //        if (tvParameters.SelectedNode != null &&
            //            tvParameters.SelectedNode.Tag != null &&
            //            (tvParameters.SelectedNode.Tag is UnitNode))
            //        {
            //            node = tvParameters.SelectedNode.Tag as UnitNode;
            //            BaseEditForm frmEdit = BaseEditForm.NewEditorForm(node);
            //            frmEdit.SetMode(FormEditState.Edit, node.Idnum);
            //            frmEdit.Show();
            //        }
            //    }
        }

        private void chbxShaitan_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !chbxShaitan.Checked;
        }

        private void param_search_Load(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !chbxShaitan.Visible | !chbxShaitan.Checked;
        }

        private void btnEditor_Click(object sender, EventArgs e)
        {
            ShowEditor();
        }

        private void ShowEditor()
        {
            //frm.Controls.Add

            CalcParameterNode snode;

            if (tvParameters.SelectedNode == null ||
                (snode = tvParameters.SelectedNode.Tag as CalcParameterNode) == null) return;

            //snode = tvParameters.SelectedNode.Tag as ServerNode;

            if (frmFuncEdit == null || frmFuncEdit.IsDisposed)
            {
                FormulaUnitProvider fup = new FormulaUnitProvider(strucProvider, (CalcParameterNode)snode.Clone());
                BaseUnitControl[] controls;
                frmFuncEdit = new Form();
                frmFuncEdit.StartPosition = FormStartPosition.CenterScreen;
                controls = null;// fup.CreateControls();
                throw new NotImplementedException();
                if (controls != null && controls.Length > 0)
                {
                    controls[0].Dock = DockStyle.Fill;
                    controls[0].InitiateProcess();
                    frmFuncEdit.Controls.Add(controls[0]);
                    //
                }
                //frmFuncEdit.SetMode(FormEditState.Edit, (ServerNode)snode);
                frmFuncEdit.FormClosed += new FormClosedEventHandler(frmFuncEdit_FormClosed);
                //frmFuncEdit.Updated += new UpdatedHandler(frmFuncEdit_OnUpdated);
                frmFuncEdit.Show(this);
                //txtFormula.Text = snode.Formula;//frmFuncEdit.Formula;
                //frmFuncEdit.Dispose();
            }
            else
                frmFuncEdit.Activate();
        }

        void frmFuncEdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            //UpdateFormula();
            Form frmFuncEdit = sender as Form;
            if (frmFuncEdit != null && frmFuncEdit.Controls.Count > 0 && frmFuncEdit.Controls[0] is BaseUnitControl)
            {
                BaseUnitControl buc = (BaseUnitControl)frmFuncEdit.Controls[0];
                txtFormula.Text = (buc.UnitProvider.UnitNode as CalcParameterNode).Formula;
                SubmitFormula();
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            SubmitFormula();
        }

        private void SubmitFormula()
        {
            CalcParameterNode snode;
            if (tvParameters.SelectedNode == null ||
                (snode = tvParameters.SelectedNode.Tag as CalcParameterNode) == null ||
                txtFormula.Text.Equals(snode.Formula)) return;
            SubmitFormula(snode, txtFormula.Text);
        }

        private void SubmitFormula(CalcParameterNode node, string formula)
        {
            try
            {
                if (node == null) throw new ArgumentNullException("node");
                //if (tvParameters.SelectedNode == null ||
                //    (snode = tvParameters.SelectedNode.Tag as CalcParameterNode) == null) return;

                node.Formula = txtFormula.Text;
                strucProvider.UpdateUnitNode(node);
                //удивительно, но вотчера никто не запускает. может и не надо следить за процессом
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения формулы: " + ex.Message);
            }
        }

        private void tvParameters_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateFormula();
        }

        private void UpdateFormula()
        {
            CalcParameterNode snode;

            if (tvParameters.SelectedNode == null ||
                (snode = tvParameters.SelectedNode.Tag as CalcParameterNode) == null)
            {
                txtFormula.Text = "";
                return;
            }
            
            txtFormula.Text = snode.Formula;
        }

        private void txtFormula_Leave(object sender, EventArgs e)
        {
            SubmitFormula();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void Find()
        {
            string txt = txtFormula.Text;
            string sFind = txtFind.Text;
            string sReplace = txtReplace.Text;
            int pos = txtFormula.SelectionStart + txtFormula.SelectionLength;
            int p;

            if (string.IsNullOrEmpty(txtFind.Text)) return;

            p = txt.IndexOf(sFind, pos);
            if (p != -1)
            {
                txtFormula.SelectionStart = p;
                txtFormula.SelectionLength = sFind.Length;
            }
            else
            {
                txtFormula.SelectionLength = 0;
                txtFormula.SelectionStart = 0;
            }
        }

        private void Replace()
        {
            Replace(false);
        }
        private void Replace(bool replaceAll)
        {
            if (replaceAll)
            {
                txtFormula.SelectionLength = 0;
                txtFormula.SelectionStart = 0;
                Find();
                while (txtFormula.SelectionLength != 0)
                {
                    txtFormula.SelectedText = txtReplace.Text;
                    Find();
                }
            }
            else
            {
                if (txtFormula.SelectionLength != 0)
                    txtFormula.SelectedText = txtReplace.Text;
                Find();
            }
        }
        private string ReplaceAll(string target, string oldVal, string newVal)
        {
            int p = 0;
            while (p != -1)
            {
                p = target.IndexOf(oldVal, p);
                if (p == -1) break;
                target = target.Remove(p, oldVal.Length);
                target = target.Insert(p, newVal);
                p += newVal.Length;
                if (p >= target.Length) break;
            }
            return target;
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            Replace();
            SubmitFormula();
        }

        private void tvParameters_ItemDrag(object sender, ItemDragEventArgs e)
        {
            UnitTreeNode unitTreeNode;
            if ((unitTreeNode = e.Item as UnitTreeNode) != null)
            {
                DataObject dobj = new DataObject();
                dobj.SetData(unitTreeNode);
                dobj.SetData(Program.CreateDragDropData(unitTreeNode.Node));
                tvParameters.DoDragDrop(dobj, DragDropEffects.All);
            }
        }

        private void tvParameters_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (chbxShaitan.Visible) SubmitFormula();
        }

        private void btnReplaceAll_Click(object sender, EventArgs e)
        {
            Replace(true);
            SubmitFormula();
        }

        private void btnCopyFormula_Click(object sender, EventArgs e)
        {
            CopyFormula();
        }

        private void CopyFormula()
        {
            CalcParameterNode curNode, node;
            string oldCode, newCode;
            string oldFormula;
            bool needChange = true;

            try
            {
                curNode = tvParameters.SelectedNode == null ? null : tvParameters.SelectedNode.Tag as CalcParameterNode;
                if (curNode == null) throw new Exception("Параметр не определен.");

                if (string.IsNullOrEmpty(txtCopyMask.Text))
                {
                    needChange = false;
                }
                
                Regex regex = new Regex(txtCopyMask.Text);
                Match m = regex.Match(curNode.Code);
                if (m.Success)
                    oldCode = m.Value;
                else
                    throw new Exception("No mask matches.");

                oldFormula = txtFormula.Text;

                foreach (TreeNode item in tvParameters.Nodes)
                {
                    if (item != tvParameters.SelectedNode)
                    {
                        node = item == null ? null : item.Tag as CalcParameterNode;
                        if (node != null)
                        {
                            m = regex.Match(node.Code);
                            if (m.Success)
                                newCode = m.Value;
                            else
                                newCode = oldCode;
                            //newFormula = oldFormula;
                            if (needChange && newCode != oldCode)
                            {
                                node.Formula = ReplaceAll(oldFormula, oldCode, newCode);
                            }
                            else
                                node.Formula = oldFormula;
                            strucProvider.UpdateUnitNode(node);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

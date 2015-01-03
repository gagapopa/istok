using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно запроса параметров канала с сервера сбора
    /// </summary>
    partial class GetParamsForm : BaseAsyncWorkForm
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        
        private ChannelNode canalNode = null;
        public event EventHandler OnUpdated;
        UnitProvider prov;

        protected GetParamsForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
        }
        public GetParamsForm(StructureProvider strucProvider, ChannelNode canal, UnitProvider prov)
            : this(strucProvider)
        {
            canalNode = canal;
            this.prov = prov;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (canalNode == null) return;

            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            dataGridView2.ContextMenuStrip = null;
            paramList = new List<ParameterItem>();
            //AsyncOperationWatcher<Object> watcher = prov.BeginGetParameters(canalNode);
            //watcher.AddValueRecivedHandler(ParamReceived);
            //watcher.AddFinishHandler(deliveryThread_RunWorkerCompleted);
            //RunWatcher(watcher);
            var pars = prov.StructureProvider.Session.GetChannelParameters(canalNode.Idnum);
            ParamReceived(pars);
            deliveryThread_RunWorkerCompleted();

            base.OnLoad(e);
        }

        List<ParameterItem> paramList;
        private void ParamReceived(Object p)
        {
            ParameterItem[] pars = p as ParameterItem[];

            if (pars != null)
                paramList.AddRange(pars);
        }

        #region Добавить параметры в Справочники
        private UnitNode GetUnitNode(DataGridViewRow row)
        {
            UnitNode addnode = new UnitNode();

            addnode.Typ = (int)UnitTypeId.Parameter;
            addnode.Text = row.Cells[0].Value.ToString();

            for (int i = 1; i < row.Cells.Count; i++)
            {
                try
                {
                    if (!String.IsNullOrEmpty(row.Cells[i].Value.ToString()))
                    {
                        addnode.SetAttribute(dataGridView2.Columns[row.Cells[i].ColumnIndex].Name, row.Cells[i].Value.ToString());
                    }
                }
                catch { }
            }

            return addnode;
        }
        //private bool AddParameter(DataGridViewRow row)
        //{
        //    try
        //    {
        //        if (canalNode == null) return false;

        //        UnitNode addnode = new UnitNode();

        //        addnode.Typ = UnitTypeId.Parameter;
        //        addnode.Text = row.Cells[0].Value.ToString();

        //        for (int i = 1; i < row.Cells.Count; i++)
        //        {
        //            try
        //            {
        //                if (!String.IsNullOrEmpty(row.Cells[i].Value.ToString()))
        //                {
        //                    addnode.Attributes[dataGridView2.Columns[row.Cells[i].ColumnIndex].Name] = row.Cells[i].Value.ToString();
        //                }
        //            }
        //            catch { }
        //        }

        //        AsyncOperationWatcher addWatcher = rds.QueryAddUnitNode(addnode, canalNode.Idnum);
        //        RunWatcher(addWatcher);
        //        WaitWatcher(addWatcher);
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonData.Error("getparams.AddParameter: " + ex.Message);
        //        MessageBox.Show(ex.Message);
        //        return false;
        //    }
        //    return true;
        //}

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null) return;

            //int count = 0;
            UnitNode node = GetUnitNode(dataGridView2.CurrentRow);
            if (node!=null)//AddParameter(dataGridView2.CurrentRow))
            {
                dataGridView2.Rows.Remove(dataGridView2.CurrentRow);
                //count++;
            }
            addParamsBackgroundWorker.RunWorkerAsync(new List<UnitNode>(new UnitNode[] { node }));
        }

        private void сохранитьВыделенныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count == 0) return;

            //int count = 0;
            List<UnitNode> nodeList = new List<UnitNode>();
            UnitNode node;
            for (int i = dataGridView2.SelectedRows.Count - 1; i >= 0; i--)
                if (!dataGridView2.SelectedRows[i].IsNewRow)
                {
                    if ((node = GetUnitNode(dataGridView2.SelectedRows[i])) != null)//AddParameter(dataGridView2.SelectedRows[i]))
                    {
                        nodeList.Add(node);
                        dataGridView2.Rows.Remove(dataGridView2.SelectedRows[i]);
                        //count++;
                    }
                    else break;
                }
            //addParamsBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(addParamsBackgroundWorker_DoWork);
            addParamsBackgroundWorker.RunWorkerAsync(nodeList);
        }

        private void сохранитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //int count = 0;
            List<UnitNode> nodeList=new List<UnitNode>();
            UnitNode node;
            for (int i = dataGridView2.Rows.Count - 1; i >= 0; i--)
                if (!dataGridView2.Rows[i].IsNewRow)
                {
                    if ((node=GetUnitNode(dataGridView2.Rows[i]))!=null)//AddParameter(dataGridView2.Rows[i]))
                    {
                        nodeList.Add(node);
                        dataGridView2.Rows.RemoveAt(i);
                        //count++;
                    }
                    else break;
                }
            addParamsBackgroundWorker.RunWorkerAsync(nodeList);
        }
        #endregion

        private void deliveryThread_RunWorkerCompleted()
        {
            if (InvokeRequired) Invoke((Action)deliveryThread_RunWorkerCompleted);
            else
            {
                Font fdel = new Font(dataGridView2.Font, FontStyle.Italic);
                dataGridView2.ContextMenuStrip = contextMenuStrip3;
                dataGridView2.Columns.Add(Consts.ParameterName, "Наименование параметра");
                for (int i = 0; i < paramList.Count; i++)
                {
                    ParameterItem pitem = paramList[i];
                    int rowIndex = dataGridView2.Rows.Add(new object[] { pitem.Name });
                    foreach (ItemProperty property in pitem.Properties)
                    {
                        int colIndex = -1;
                        if (dataGridView2.Columns.Contains(property.Name))
                            colIndex = dataGridView2.Columns[property.Name].Index;
                        else
                        {
                            //String name;
                            //switch (key)
                            //{
                            //    case Consts.ParameterCode:
                            //        name = "Код";
                            //        break;
                            //    case Consts.ParameterUnit:
                            //        name = "Единица измерения";
                            //        break;
                            //    default:
                            //        name = key;
                            //        break;
                            //}
                            colIndex = dataGridView2.Columns.Add(property.Name, property.DisplayName);
                        }
                        dataGridView2.Rows[rowIndex].Cells[colIndex].Value = pitem[property];
                    }
                }
            }
        }

        void addParamsBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            const string formulacode = "formula_cod";
            try
            {
                List<UnitNode> nodeList = e.Argument as List<UnitNode>;

                addParamsBackgroundWorker.ReportProgress(20, "Добавление параметров");
                if (nodeList != null)
                {
                    foreach (var newnode in nodeList)
                    {
                        if (!newnode.Attributes.ContainsKey(formulacode))
                        {
                            var fcode = new RevisedStorage<string>();
                            RevisedStorage<string> rcode;
                            if (newnode.Attributes.TryGetValue("code", out rcode))
                            {
                                fcode.Set(rcode.Get());
                                newnode.Attributes[formulacode] = fcode;
                            }
                        }
                    }
                    prov.StructureProvider.AddUnitNodes(nodeList.ToArray(), canalNode);
                }
                addParamsBackgroundWorker.ReportProgress(100, "Завершение операции");
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка добавления параметров.", exc);
                ShowError(exc);
            }
            //throw new NotImplementedException();
        }

        private void addParamsBackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            ShowStatus(e.UserState.ToString(), e.ProgressPercentage);
        }

        private void addParamsBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            ShowStatusStripAsyncView();
            if (OnUpdated != null)
                OnUpdated(this, EventArgs.Empty);
        }
    }
}

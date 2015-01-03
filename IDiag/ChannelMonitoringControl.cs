using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using COTES.ISTOK;
using COTES.ISTOK.DiagnosticsInfo;

namespace COTES.ISTOK.Monitoring
{
    /// <summary>
    /// Компонент, производящий мониторинг каналов, при помощи BaseDiagnostics
    /// </summary>
    [DockingAttribute(DockingBehavior.Ask)]
    [Description("Компонент, производящий мониторинг каналов")]
    public partial class ChannelMonitoringControl : UserControl
    {
        DataGridViewCellStyle blockedStyle;
        DataGridViewCellStyle unstoredStyle;
        DataGridViewCellStyle exceptStyle;
        DataGridViewCellStyle unloadedStyle;

        Color GridViewEnabledColor = Color.White;
        Color GridViewDisableColor = Color.DarkGray;

        private bool isEnabled;

        public ChannelMonitoringControl()
        {
            InitializeComponent();

            refreshWaitCallBack = new WaitCallback(RefreshChannelsInfo);

            blockedStyle = new DataGridViewCellStyle(monitoringDataGridView.DefaultCellStyle);
            unstoredStyle = new DataGridViewCellStyle(monitoringDataGridView.DefaultCellStyle);
            exceptStyle = new DataGridViewCellStyle(monitoringDataGridView.DefaultCellStyle);
            unloadedStyle = new DataGridViewCellStyle(monitoringDataGridView.DefaultCellStyle);

            blockedStyle.BackColor = Color.Gray;
            exceptStyle.BackColor = Color.Red;
            exceptStyle.ForeColor = Color.White;
            unstoredStyle.BackColor = Color.Aquamarine;
            unloadedStyle.BackColor = Color.Khaki;
        }

        /// <summary>
        /// Установить или получить объект диагностики каналов
        /// </summary>
        [Browsable(false)]
        public IDiagnostics ChannelDiagnostics { get; set; }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Контекстное меню, для управления столбцами")]
        public ContextMenuStrip ColumnsContextMenuStrip
        {
            get { return columnContextMenuStrip; }
        }

        /// <summary>
        /// Возвращает значение, доступен ли компонент (доступна ли связь с ChannelDiagnostics)
        /// </summary>
        [Browsable(false)]
        public bool IsEnabled
        {
            get { return isEnabled; }
            protected set
            {
                //CrossAppDomainDelegate deleg;

                isEnabled = value;
                //if (isEnabled)
                //    deleg = setBackground1;
                //else
                //    deleg = setBackground2;
                
                if (this.InvokeRequired)
                    this.Invoke((Action<bool>)setBackground2, isEnabled);
                else setBackground2(isEnabled);
            }
        }

        private void setBackground2(bool enabled)
        {
            if (enabled)
                monitoringDataGridView.BackgroundColor = GridViewEnabledColor;
            else
            {
                monitoringDataGridView.BackgroundColor = GridViewDisableColor;
                monitoringDataGridView.DataSource = null;
            }
            monitoringDataGridView.Enabled = enabled;
        }

        private void setBackground1()
        {
            monitoringDataGridView.BackgroundColor = GridViewEnabledColor;
            monitoringDataGridView.Enabled = isEnabled;
        }

        #region Monitoring Columns Manage
        private void monitoringDataGridView_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            DataGridViewColumn column = e.Column;
            column.HeaderCell.ContextMenuStrip = headerContextMenuStrip;
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Name = column.Name + "MenuItem";
            menuItem.Text = column.HeaderText;
            menuItem.Checked = column.Visible;
            menuItem.Tag = column;
            menuItem.Click += new EventHandler(showHideMenuItem_Click);
            columnContextMenuStrip.Items.Add(menuItem);
        }

        private void monitoringDataGridView_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            DataGridViewColumn column = e.Column;
            foreach (ToolStripItem menuItem in columnContextMenuStrip.Items)
            {
                if (menuItem.Tag != null && menuItem.Tag is DataGridViewColumn
                    && ((DataGridViewColumn)(menuItem.Tag)) == column)
                { columnContextMenuStrip.Items.Remove(menuItem); break; }
            }

        }

        private void showHideMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            if (menuItem.Tag != null && menuItem.Tag is DataGridViewColumn)
            {
                DataGridViewColumn column = (DataGridViewColumn)menuItem.Tag;
                menuItem.Checked = column.Visible ^= true;
                column.DataGridView.Refresh();
            }
        }

        private void showAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripItem menuItem in columnContextMenuStrip.Items)
            {
                if (menuItem is ToolStripMenuItem && menuItem.Tag != null && menuItem.Tag is DataGridViewColumn)
                {
                    DataGridViewColumn column = (DataGridViewColumn)menuItem.Tag;
                    ((ToolStripMenuItem)menuItem).Checked = column.Visible = true;
                }
            }
        }

        private void hideAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripItem menuItem in columnContextMenuStrip.Items)
            {
                if (menuItem is ToolStripMenuItem && menuItem.Tag != null && menuItem.Tag is DataGridViewColumn)
                {
                    DataGridViewColumn column = (DataGridViewColumn)menuItem.Tag;
                    ((ToolStripMenuItem)menuItem).Checked = column.Visible = false;
                }
            }
        }

        private void headerContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip menu = (ContextMenuStrip)sender;
            Point delta = this.PointToClient(new Point(menu.Left, menu.Top));
            System.Windows.Forms.DataGridView.HitTestInfo hitInfo =
                monitoringDataGridView.HitTest(delta.X, delta.Y);

            if (hitInfo.ColumnIndex >= 0)
                headerContextMenuStrip.Tag = monitoringDataGridView.Columns[hitInfo.ColumnIndex];
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContextMenuStrip menu = sender as ContextMenuStrip;
            ToolStripMenuItem menuItem;
            DataGridViewColumn column;

            if ((menuItem = sender as ToolStripMenuItem) != null && (menu = menuItem.Owner as ContextMenuStrip) != null &&
                (column = menu.Tag as DataGridViewColumn) != null)
            {
                foreach (ToolStripItem columnMenuItem in columnContextMenuStrip.Items)
                    if (columnMenuItem.Tag as DataGridViewColumn == column &&
                        (menuItem = columnMenuItem as ToolStripMenuItem) != null) menuItem.Checked = false;
                column.Visible = false;
            }
        }
        #endregion

        private void monitoringDataGridView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ContextMenuStrip menu = headerContextMenuStrip;
            System.Windows.Forms.DataGridView.HitTestInfo hitInfo = monitoringDataGridView.HitTest(menu.Left, menu.Top);
        }

        private void monitoringDataGridView_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.ContextMenuStrip != null)
                e.ContextMenuStrip.Tag = ((DataGridView)sender).Columns[e.ColumnIndex];
            hideToolStripMenuItem.Enabled = true;
        }

        #region Channel Manage Menu Item
        private void viewChannelsPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TestRemoteConnection(ChannelDiagnostics))
            {
                if (!ChannelDiagnostics.CanManageChannels()) throw new InvalidOperationException("Данный класс диагностики не поддерживает управление каналами");
                ChannelInfo idchannel;
                if (monitoringDataGridView.SelectedRows.Count > 0)
                {
                    try
                    {
                        //idchannel = int.Parse(monitoringDataGridView.SelectedRows[0].Cells["ChannelName"].Value.ToString());
                        //IDictionary props = ChannelDiagnostics.GetItemProperty(idchannel);
                        var channelIDies = ChannelDiagnostics.GetChannels();
                        int id = int.Parse(monitoringDataGridView.SelectedRows[0].Cells["Номер"].Value.ToString());
                        idchannel = (from i in channelIDies where i.Id == id select i).FirstOrDefault();

                        var props = idchannel;
                        if (props != null)
                        {
                            //IDictionary props = propertyDictionary[idchannel];
                            DataTable table1 = new DataTable();
                            table1.Columns.Add("Name");
                            table1.Columns.Add("Value");
                            if (props != null)
                            {
                                foreach (var key in props.Properties)
                                {
                                    DataRow row = table1.NewRow();
                                    row["Name"] = key.DisplayName;
                                    row["Value"] = props[key];
                                    table1.Rows.Add(row);
                                }
                            }
                            PropertyGridForm grid0 = new PropertyGridForm();
                            grid0.Properties = table1;
                            grid0.ShowDialog();
                        }
                    }
                    catch (FormatException) { }
                }
            }
        }

        private void flushWriteBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TestRemoteConnection(ChannelDiagnostics))
                if (ChannelDiagnostics.CanManageBuffer()) ChannelDiagnostics.FlushBuffer();
                else throw new InvalidOperationException("Данный класс диагностики не поддерживает управление буфером сбора");
        }

        private void viewWriteBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TestRemoteConnection(ChannelDiagnostics))
            {
                if (!ChannelDiagnostics.CanManageBuffer()) throw new InvalidOperationException("Данный класс диагностики не поддерживает управление буфером сбора");
                ChannelInfo idchannel;
                if (monitoringDataGridView.SelectedRows.Count > 0)
                {
                    List<ParamValueItemWithID> values = null;
                    DataTable valuesDataTable = new DataTable();
                    valuesDataTable.Columns.Add("ID");
                    valuesDataTable.Columns.Add("Time");
                    valuesDataTable.Columns.Add("Value");
                    valuesDataTable.Columns.Add("Quality");

                    //idchannel = int.Parse(monitoringDataGridView.SelectedRows[0].Cells["ChannelName"].Value.ToString());
                    var channelIDies = ChannelDiagnostics.GetChannels();
                    int id = int.Parse(monitoringDataGridView.SelectedRows[0].Cells["Номер"].Value.ToString());
                    idchannel = (from i in channelIDies where i.Id == id select i).FirstOrDefault();

                    if (idchannel != null)
                    {
                        values = ChannelDiagnostics.GetBufferValues(idchannel);

                        foreach (ParamValueItemWithID param in values)
                        {
                            DataRow row = valuesDataTable.NewRow();

                            row["ID"] = param.ParameterID;
                            row["Time"] = param.Time;
                            row["Value"] = param.Value;
                            row["Quality"] = param.Quality;

                            valuesDataTable.Rows.Add(row);
                        }

                        PropertyGridForm grid0 = new PropertyGridForm();
                        grid0.Properties = valuesDataTable;
                        grid0.ShowDialog();
                    }
                }
            }
        }

        private bool TestRemoteConnection(ITestConnection<Object> testConnection)
        {
            bool ret;

            ret = testConnection != null && TestConnection<Object>.Test(testConnection, null);
            if (!ret) IsEnabled = false;
            return ret;
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TestRemoteConnection(ChannelDiagnostics))
            {
                if (!ChannelDiagnostics.CanManageChannels()) throw new InvalidOperationException("Данный класс диагностики не поддерживает управление каналами");
                ChannelInfo idchannel;
                if (monitoringDataGridView.SelectedRows.Count > 0)
                {
                    //idchannel = int.Parse(monitoringDataGridView.SelectedRows[0].Cells["ChannelName"].Value.ToString());
                    var channelIDies = ChannelDiagnostics.GetChannels();
                    int id = int.Parse(monitoringDataGridView.SelectedRows[0].Cells["Номер"].Value.ToString());
                    idchannel = (from i in channelIDies where i.Id == id select i).FirstOrDefault();

                    if (idchannel != null)
                    {
                        ChannelDiagnostics.StartChannel(idchannel);
                    }
                }
                Exec();
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TestRemoteConnection(ChannelDiagnostics))
            {
                if (!ChannelDiagnostics.CanManageChannels()) throw new InvalidOperationException("Данный класс диагностики не поддерживает управление каналами");
                ChannelInfo idchannel;
                if (monitoringDataGridView.SelectedRows.Count > 0)
                {
                    //idchannel = int.Parse(monitoringDataGridView.SelectedRows[0].Cells["ChannelName"].Value.ToString());
                    var channelIDies = ChannelDiagnostics.GetChannels();
                    int id = int.Parse(monitoringDataGridView.SelectedRows[0].Cells["Номер"].Value.ToString());
                    idchannel = (from i in channelIDies where i.Id == id select i).FirstOrDefault();

                    if (idchannel != null)
                    {
                        ChannelDiagnostics.StopChannel(idchannel);
                    }
                }
                Exec();
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (TestRemoteConnection(ChannelDiagnostics))
            //{
            //    if (!ChannelDiagnostics.CanManageChannels()) throw new InvalidOperationException("Данный класс диагностики не поддерживает управление каналами");
            //    int idchannel = 0;
            //    if (monitoringDataGridView.SelectedRows.Count > 0)
            //    {
            //        idchannel = int.Parse(monitoringDataGridView.SelectedRows[0].Cells["ChannelName"].Value.ToString());
            //        ChannelDiagnostics.LoadChannel(idchannel);
            //    }
            //    Exec();
            //}
        }

        private void unloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (TestRemoteConnection(ChannelDiagnostics))
            //{
            //    if (!ChannelDiagnostics.CanManageChannels()) throw new InvalidOperationException("Данный класс диагностики не поддерживает управление каналами");
            //    int idchannel = 0;
            //    if (monitoringDataGridView.SelectedRows.Count > 0)
            //    {
            //        idchannel = int.Parse(monitoringDataGridView.SelectedRows[0].Cells["ChannelName"].Value.ToString());
            //        ChannelDiagnostics.UnloadChannel(idchannel);
            //    }
            //    Exec();
            //}
        }

        private void manageToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            try
            {
                ChannelInfo idchannel ;
                if (monitoringDataGridView.SelectedRows.Count > 0)
                {
                    //idchannel = int.Parse(monitoringDataGridView.SelectedRows[0].Cells["ChannelName"].Value.ToString());
                    var channelIDies = ChannelDiagnostics.GetChannels();
                    int id = int.Parse(monitoringDataGridView.SelectedRows[0].Cells["Номер"].Value.ToString());
                    idchannel = (from i in channelIDies where i.Id == id select i).FirstOrDefault();

                    ChannelStatus status = ChannelDiagnostics.GetChannelState(idchannel);

                    loadToolStripMenuItem.Enabled = status == ChannelStatus.Unloaded;
                    unloadToolStripMenuItem.Enabled = !loadToolStripMenuItem.Enabled;

                    startToolStripMenuItem.Enabled = status != ChannelStatus.Unloaded && (status & ChannelStatus.Started) != ChannelStatus.Started;
                    stopToolStripMenuItem.Enabled = status != ChannelStatus.Unloaded && (status & ChannelStatus.Started) == ChannelStatus.Started;
                }
            }
            catch
            {
                startToolStripMenuItem.Enabled = stopToolStripMenuItem.Enabled =
                    loadToolStripMenuItem.Enabled = unloadToolStripMenuItem.Enabled = false;
            }
        }
        #endregion

        #region Refresh Channel Information
        DataTable table = null;
        private bool executing;
        private Thread currentThread;
        private TimeSpan joinInterval = TimeSpan.FromSeconds(1);
        private WaitCallback refreshWaitCallBack = null;//new WaitCallback(RefreshChannelsInfo);

        /// <summary>
        /// Обновить информацию о канале
        /// </summary>
        public void RefreshChannelsInfo()
        {
            if (this.DesignMode) return;
            if (!executing) ThreadPool.QueueUserWorkItem(refreshWaitCallBack);
        }

        /// <summary>
        /// Очистить информацию о каналах
        /// </summary>
        public void CleanChannelsInfo()
        {
            isEnabled = false;
            if (currentThread != null) if (!currentThread.Join(joinInterval))
                    currentThread.Abort();
            monitoringDataGridView.DataSource = null;
            //executing = false;
        }

        private void RefreshChannelsInfo(object state)
        {
            //String currentRow;
            //List<String> selChannels = new List<String>();
            currentThread = Thread.CurrentThread;

            try
            {
                //foreach (DataGridViewRow row in monitoringDataGridView.SelectedRows)
                //{
                //    selChannels.Add(row.Cells["ChannelName"].Value.ToString());
                //}
                //if (monitoringDataGridView.CurrentRow != null) currentRow = monitoringDataGridView.CurrentRow.Cells["ChannelName"].Value.ToString();

                Exec();

                //foreach (DataGridViewRow row in monitoringDataGridView.Rows)
                //{
                //    String chnName = row.Cells["ChannelName"].Value.ToString();
                //    if (selChannels.Contains(chnName)) row.Selected = true;
                //}
            }
            catch { }
            finally { currentThread = null; }
            //selChannels.Clear();
        }

        private void Exec()
        {
            if (this.DesignMode) return;
            int i;
            try
            {
                DataTable oldTable = table;
                executing = true;
                if (TestRemoteConnection(ChannelDiagnostics))
                {
                    if (!ChannelDiagnostics.CanManageChannels())
                        throw new InvalidOperationException("Данный класс диагностики не поддерживает управление каналами");

                    var channelIDies = ChannelDiagnostics.GetChannels();

                    table = ChannelDiagnostics.GetChannelInfo();

                    for (i = 0; i < channelIDies.Length; i++)
                    {
                        if (table.Select("Номер='" + channelIDies[i].Id + "'").Length == 0)
                        {
                            DataRow row = table.NewRow();
                            row["Номер"] = channelIDies[i].Id;
                            table.Rows.Add(row);
                        }
                    }

                    if (monitoringDataGridView.InvokeRequired)
                        monitoringDataGridView.Invoke((CrossAppDomainDelegate)drawGrid);
                    if (oldTable != null) oldTable.Dispose();
                }
            }
            catch { IsEnabled = false; }
            finally { executing = false; }
        }

        private void drawGrid()
        {
            monitoringDataGridView.DataSource = null;
            monitoringDataGridView.Rows.Clear();

            if (table!=null)
            {
                foreach (DataColumn dataColumn in table.Columns)
                    if (!monitoringDataGridView.Columns.Contains(dataColumn.ColumnName)) 
                        monitoringDataGridView.Columns.Add(dataColumn.ColumnName, dataColumn.Caption);

                foreach (DataRow dataRow in table.Rows)
                {
                    DataGridViewRow gridRow = monitoringDataGridView.Rows[monitoringDataGridView.Rows.Add()];
                    foreach (DataColumn dataColumn in table.Columns)
                        gridRow.Cells[dataColumn.ColumnName].Value = dataRow[dataColumn].ToString();
                } 
            }
            ColorDifferentiationOfPants();
            IsEnabled = true;
        }

        private void monitoringDataGridView_Sorted(object sender, EventArgs e)
        {
            ColorDifferentiationOfPants();
        }

        private void ColorDifferentiationOfPants()
        {
            foreach (DataGridViewRow row in monitoringDataGridView.Rows)
            {
                var channelIDies = ChannelDiagnostics.GetChannels();
                int id= int.Parse(row.Cells["Номер"].Value.ToString());
                ChannelInfo info = (from i in channelIDies where i.Id == id select i).FirstOrDefault();

                ChannelStatus state;
                if (info != null)
                {
                    state = ChannelDiagnostics.GetChannelState(info);
                }
                else state = ChannelStatus.Unloaded;
                if (state == ChannelStatus.Unloaded) row.DefaultCellStyle = unloadedStyle;
                else if ((state & ChannelStatus.Blocked) == ChannelStatus.Blocked) row.DefaultCellStyle = blockedStyle;
                else if ((state & ChannelStatus.Storable) != ChannelStatus.Storable) row.DefaultCellStyle = unstoredStyle;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
        }
        #endregion

        private void dataGridContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (TestRemoteConnection(ChannelDiagnostics))
            {
                bool manageChannels = ChannelDiagnostics.CanManageChannels();
                bool manageBuffer = ChannelDiagnostics.CanManageBuffer();

                viewChannelsPropertiesToolStripMenuItem.Visible = manageToolStripMenuItem.Visible = manageChannels;
                viewWriteBufferToolStripMenuItem.Visible = flushWriteBufferToolStripMenuItem.Visible = manageBuffer;
                toolStripSeparator3.Visible = manageBuffer || manageChannels;
            }
        }
    }
}

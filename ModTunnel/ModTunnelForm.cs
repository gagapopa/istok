//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Text;
//using System.Windows.Forms;
//using System.Runtime.Remoting;
//using System.Threading;
//using COTES.ISTOK.Modules;
//using COTES.ISTOK.Modules.Tunnel.Properties;
//using System.Resources;
//using System.Globalization;
//using System.Reflection;
//using COTES.ISTOK;
//using SimpleLogger;

//namespace COTES.ISTOK.Modules.Tunnel
//{
//    public sealed partial class ModTunnelForm : Form
//    {
//        //private ModTunnel modTunnel;
//        private DataTable channelsInfoTable;
//        private ResourceManager resourceManager;
//        private CultureInfo cultureInfo;

//        //public ModTunnel Tunnel
//        //{
//        //    get { return modTunnel; }
//        //    set
//        //    {
//        //        if (modTunnel != null)
//        //        {
//        //            modTunnel.ObjectRegistered -= new EventHandler<TunnelEventArgs>(modTunnel_ObjectRegistered);
//        //            modTunnel.ObjectUnRegistered -= new EventHandler<TunnelEventArgs>(modTunnel_ObjectUnRegistered);
//        //            modTunnel.StartStateChanged -= new EventHandler(modTunnel_StartStateChanged);
//        //        }
//        //        modTunnel = value;
//        //        modTunnel.ObjectRegistered += new EventHandler<TunnelEventArgs>(modTunnel_ObjectRegistered);
//        //        modTunnel.ObjectUnRegistered += new EventHandler<TunnelEventArgs>(modTunnel_ObjectUnRegistered);
//        //        modTunnel.StartStateChanged += new EventHandler(modTunnel_StartStateChanged);

//        //        modTunnel.TimeOut = TimeSpan.FromMinutes(Settings.Default.TimeOut);
//        //    }
//        //}

//        public ModTunnelForm()
//        {
//            channelsInfoTable = new DataTable();
//            InitializeComponent();

//            unRegisterToolStripMenuItem.Enabled = false;
//            intervalChanged();
//            this.Icon = Resources.well01;

//            resourceManager = Resources.ResourceManager;
//            //cultureInfo = CultureInfo.CurrentUICulture;
//            cultureInfo = Program.CurrentCulture;
//        }

//        private void modTunnel_ObjectUnRegistered(object sender, TunnelEventArgs e)
//        {
//            String text = resourceManager.GetString("UnRegisteredText",cultureInfo);
//            BeginInvoke(new TextValueDelegate(setTextValue), label2, text/*"Unregistered: "*/ + e.URI);
//            //Program.WriteLog("Message", text + e.URI);
//            //Consts.WriteLog(MessageCategory.Message, "Tunnel - " + text + e.URI);
//            modTunnel.MessageLog.Message(MessageLevel.Info, "Tunnel - " + text + e.URI);
//            BeginInvoke(new ThreadStart(Exec));
//        }

//        private void modTunnel_ObjectRegistered(object sender, TunnelEventArgs e)
//        {
//            String text = resourceManager.GetString("RegisteredText", cultureInfo);
//            if (e.URI != null) BeginInvoke(new TextValueDelegate(setTextValue), label2, text/*"Registered: "*/ + e.URI);
//            //Program.WriteLog("Message", text + e.URI);
//            //Consts.WriteLog(MessageCategory.Message, "Tunnel - " + text + e.URI);
//            modTunnel.MessageLog.Message(MessageLevel.Info, "Tunnel - " + text + e.URI);

//            BeginInvoke(new ThreadStart(Exec));
//        }
//        private void modTunnel_StartStateChanged(object sender, EventArgs e)
//        {
//            String text;
//            if (modTunnel.IsStarted) text = resourceManager.GetString("StartedText", cultureInfo);
//            else text = resourceManager.GetString("StoppedText", cultureInfo);
//            if (sender != this)
//                //Program.WriteLog("Message", text);
//                //Consts.WriteLog(MessageCategory.Message, "Tunnel - " + text);
//                modTunnel.MessageLog.Message(MessageLevel.Info, "Tunnel - " + text);

//            BeginInvoke(new TextValueDelegate(setCaption), this, text);
//        }


//        private delegate void TextValueDelegate(Control control, String text);
//        private void setTextValue(Control control, String text)
//        {
//            control.Text = text;
//        }
//        private void setCaption(Control control, String text)
//        {
//            String caption = this.Text;
//            caption = caption.Split('-')[0];
//            this.Text = caption + "- " + text;
//        }

//        public void StartTunnel()
//        {
//            startToolStripMenuItem_Click(this, EventArgs.Empty);
//        }

//        private void startToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                if (!modTunnel.IsStarted)
//                {
//                    try
//                    {
//                        modTunnel.Start(int.Parse(Settings.Default.Port));
//                    }
//                    catch (RemotingException exc)
//                    {
//                        MessageBox.Show(exc.Message, "Error Caption", MessageBoxButtons.OK); 
//                        //Program.WriteLog("Error", exc.Message);
//                        //Consts.WriteLog(MessageCategory.Error, "Tunnel - " + exc.Message);
//                        modTunnel.MessageLog.Message(MessageLevel.Error, "Tunnel - " + exc.Message);

//                    }
//                }
//                Exec();
//            }
//            catch (Exception exc)
//            {
//                MessageBox.Show("Can't start server: " + exc.Message, "Error");
//                //Program.WriteLog("Error", "Can't start server: " + exc.Message);
//                //Consts.WriteLog(MessageCategory.Error, "Tunnel - Can't start server: " + exc.Message);
//                modTunnel.MessageLog.Message(MessageLevel.Error, "Tunnel - Can't start server: " + exc.Message);
//            }
//        }

//        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                if (modTunnel.IsStarted)
//                    modTunnel.Stop();
//                Exec();
//            }
//            catch (Exception exc)
//            {
//                MessageBox.Show("Can't start server: " + exc.Message, "Error");
//                //Program.WriteLog("Error", "Can't start server: " + exc.Message);
//                //Consts.WriteLog(MessageCategory.Error, "Tunnel - Can't start server: " + exc.Message);
//                modTunnel.MessageLog.Message(MessageLevel.Error, "Tunnel - Can't start server: " + exc.Message);
//            }
//        }

//        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
//        {
//            modTunnel.Stop();
//            Application.Exit();
//        }

//        private void unRegisterToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            String selectedURI;
//            if (monitoringDataGridView.SelectedRows.Count > 0)
//            {
//                selectedURI = (String)monitoringDataGridView.SelectedRows[0].Cells["URI"].Value;
//                modTunnel.UnRegisterObject(selectedURI);
//            }
//        }

//        private void timeOutTextBox_TextChanged(object sender, EventArgs e)
//        {
//            float minutes;
//            try
//            {
//                minutes = float.Parse(((Control)sender).Text);
//            }
//            catch (FormatException)
//            {
//                minutes = 0;
//                ((Control)sender).Text = minutes.ToString();
//            }

//            modTunnel.TimeOut = TimeSpan.FromMinutes(minutes);
//        }

//        private void Form1_Resize(object sender, EventArgs e)
//        {
//            Form senderForm = (Form)sender;
//            if (senderForm.WindowState == FormWindowState.Minimized) senderForm.Hide();

//        }

//        private void ModTunnelForm_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            if (e.CloseReason == CloseReason.UserClosing)
//            {
//                e.Cancel = true;
//                Hide();
//            }
//        }

//        private void RefreshButton_Click(object sender, EventArgs e)
//        {
//            String selChannel = null;
//            if (monitoringDataGridView.SelectedRows.Count > 0)
//            {
//                selChannel = (String)monitoringDataGridView.SelectedRows[0].Cells["ChannelName"].Value;
//            }
//            Exec();
//            if (selChannel != null)
//            {
//                for (int i = 0; i < monitoringDataGridView.Rows.Count; i++)
//                    if (monitoringDataGridView.Rows[i].Cells["ChannelName"].Value.Equals(selChannel)) { monitoringDataGridView.Rows[i].Selected = true; break; }
//            }
//        }

//        private void Exec()
//        {
//            int i;
//            List<PropertyTable> channelsInfo = new List<PropertyTable>();
//            try
//            {
//                List<MarshalByRefObject> sharedObject = modTunnel.SharedObjects;
//                List<String> sharedModules = modTunnel.SharedModules;
//                String localURI;
//                for (i = 0; i < sharedObject.Count; i++)
//                {
//                    try
//                    {
//                        IHandler handler = (IHandler)sharedObject[i];

//                        List<PropertyTable> ret = handler.GetHandlerInfo();
//                        localURI = RemotingServices.GetObjectUri(sharedObject[i]);
//                        localURI = localURI.Substring(localURI.IndexOf("/", 2) + 1);
//                        foreach (PropertyTable chInfo in ret)
//                        {
//                            chInfo.AddProperty("ModuleName", "", "", "", sharedModules[i]);
//                            chInfo.AddProperty("URI", "", "", "", localURI);
//                        }
//                        channelsInfo.AddRange(ret);
//                    }
//                    catch (NullReferenceException) { }
//                    catch (InvalidCastException) { }
//                }
//                channelsInfoTable.Clear();
//                foreach (PropertyTable chnInfo in channelsInfo)
//                {
//                    for (i = 0; i < chnInfo.Count; i++)
//                    {
//                        if (channelsInfoTable.Columns.IndexOf(chnInfo[i].name) == -1)
//                            channelsInfoTable.Columns.Add(chnInfo[i].name);
//                    }

//                    DataRow row = channelsInfoTable.Rows.Add();
//                    for (i = 0; i < channelsInfoTable.Columns.Count; i++) row[i] = "NULL";
//                    for (i = 0; i < chnInfo.Count; i++)
//                    {
//                        Property pr = chnInfo[i];
//                        row[pr.name] = pr.value;
//                    }
//                }
//                monitoringDataGridView.DataSource = channelsInfoTable;

//            }
//            catch (Exception exc)
//            {
//                MessageBox.Show(exc.Message);
//                //Program.WriteLog("Error", exc.Message);
//                //Consts.WriteLog(MessageCategory.Error, "Tunnel - " + exc.Message);
//                modTunnel.MessageLog.Message(MessageLevel.Error, "Tunnel - " + exc.Message);
//            }
//        }

//        private void autoRefreshToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            bool isChecked = ((ToolStripMenuItem)sender).Checked ^= true;
//            Settings.Default.AutoRefreshEnabled = isChecked;
//            timer1.Enabled = isChecked;
//        }

//        private void intervalChanged()
//        {
//            if (Settings.Default.AutoRefreshEnabled)
//            {
//                double tick = Settings.Default.AutoRefreshPeriod;
//               timer1.Interval = (int)(tick * 1000);
//            }
//        }

//        private void timer1_Tick(object sender, EventArgs e)
//        {
//            String selChannel = null;
//            if (monitoringDataGridView.SelectedRows.Count > 0)
//            {
//                selChannel = (String)monitoringDataGridView.SelectedRows[0].Cells["ChannelName"].Value;
//            }
//            Exec();
//            if (selChannel != null)
//            {
//                for (int i = 0; i < monitoringDataGridView.Rows.Count; i++)
//                    if (monitoringDataGridView.Rows[i].Cells["ChannelName"].Value.Equals(selChannel)) { monitoringDataGridView.Rows[i].Selected = true; break; }
//            }
//        }

//        private void dataGridView1_DoubleClick(object sender, EventArgs e)
//        {
//            int channelID;
//            PropertyGridForm grid0;
//            String handlerURI;
//            IHandler handler;
//            if (((DataGridView)(sender)).SelectedRows.Count > 0)
//            {
//                channelID = int.Parse((String)((DataGridView)(sender)).SelectedRows[0].Cells["ChannelName"].Value);
//                handlerURI = (String)((DataGridView)(sender)).SelectedRows[0].Cells["URI"].Value;

//                handler = (IHandler)modTunnel.GetObjectByURI(handlerURI);

//                PropertyTable table = handler.GetProperties(channelID.ToString());

//                grid0 = new PropertyGridForm();
//                grid0.Properties = table.Properties;
//                grid0.ShowDialog();
//            }
//        }

//        private void dataGridView1_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
//        {
//            DataGridViewColumn column = e.Column;
//            String name = resourceManager.GetString(column.Name + "ColumnHeader",cultureInfo);
//            if (name != null && name != "")
//                column.HeaderText = name;

//            column.HeaderCell.ContextMenuStrip = headerContextMenuStrip;
//            ToolStripMenuItem menuItem = new ToolStripMenuItem();
//            menuItem.Name = column.Name + "MenuItem";
//            menuItem.Text = column.HeaderText;
//            menuItem.Checked = column.Visible;
//            menuItem.Tag = column;
//            column.Tag = menuItem;
//            menuItem.Click += new EventHandler(showHideMenuItem_Click);
//            columnContextMenuStrip.Items.Add(menuItem);
//        }

//        private void monitoringDataGridView_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
//        {
//            DataGridViewColumn column = e.Column;
//            foreach (ToolStripItem menuItem in columnContextMenuStrip.Items)
//            {
//                if (menuItem.Tag != null && menuItem.Tag is DataGridViewColumn
//                    && ((DataGridViewColumn)(menuItem.Tag)) == column)
//                { columnContextMenuStrip.Items.Remove(menuItem); break; }
//            }
//        }

//        private void monitoringDataGridView_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
//        {
//            if (e.ContextMenuStrip != null)
//                e.ContextMenuStrip.Tag = ((DataGridView)sender).Columns[e.ColumnIndex];
//            hideToolStripMenuItem.Enabled = true;
//            unRegisterToolStripMenuItem.Enabled = ((DataGridView)sender).SelectedRows.Count > 0;
//        }

//        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            DataGridViewColumn column = null;
//            if (headerContextMenuStrip.Tag != null && headerContextMenuStrip.Tag is DataGridViewColumn)
//            {
//                column = (DataGridViewColumn)headerContextMenuStrip.Tag;
//                column.Visible = false;
//                foreach (ToolStripItem menuItem in columnContextMenuStrip.Items)
//                {
//                    if (menuItem.Tag != null && menuItem.Tag is DataGridViewColumn
//                        && ((DataGridViewColumn)(menuItem.Tag)) == column)
//                    { ((ToolStripMenuItem)menuItem).Checked = column.Visible; break; }
//                }
//            }
//        }

//        private void showHideMenuItem_Click(object sender, EventArgs e)
//        {
//            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
//            if (menuItem.Tag != null && menuItem.Tag is DataGridViewColumn)
//            {
//                DataGridViewColumn column = (DataGridViewColumn)menuItem.Tag;
//                menuItem.Checked = column.Visible ^= true;
//                column.DataGridView.Refresh();
//            }
//        }

//        private void showAllToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            foreach (ToolStripItem menuItem in columnContextMenuStrip.Items)
//            {
//                if (menuItem is ToolStripMenuItem && menuItem.Tag != null && menuItem.Tag is DataGridViewColumn)
//                {
//                    DataGridViewColumn column = (DataGridViewColumn)menuItem.Tag;
//                    ((ToolStripMenuItem)menuItem).Checked = column.Visible = true;
//                }
//            }
//        }

//        private void hideAllToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            foreach (ToolStripItem menuItem in columnContextMenuStrip.Items)
//            {
//                if (menuItem is ToolStripMenuItem && menuItem.Tag != null && menuItem.Tag is DataGridViewColumn)
//                {
//                    DataGridViewColumn column = (DataGridViewColumn)menuItem.Tag;
//                    ((ToolStripMenuItem)menuItem).Checked = column.Visible = false;
//                }
//            }
//        }

//        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            ConfigurationForm configureationForm = new ConfigurationForm();
//            if (configureationForm.ShowDialog() == DialogResult.OK)
//                intervalChanged();
//        }

//        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (modTunnel != null) modTunnel.Stop();
//            Application.Exit();
//        }

//        #region Languages
//        private void InitialiseLanguageMenu()
//        {
//            ToolStripItemCollection menuCollection = languageToolStripMenuItem.DropDownItems;
//            //CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures);
//            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);

//            neutralToolStripMenuItem.Tag = CultureInfo.InvariantCulture;

//            foreach (CultureInfo culture in cultures)
//            {
//                ToolStripMenuItem menuItem;
//                ResourceSet resourceSet;

//                if (culture.Equals(CultureInfo.InvariantCulture)) continue;
//                resourceSet = resourceManager.GetResourceSet(culture, true, false);
//                if (resourceSet != null)
//                {
//                    ToolStripItem[] items = menuCollection.Find(culture.EnglishName, false);
//                    if (items.Length == 0)
//                    {
//                        menuItem = new ToolStripMenuItem();
//                        menuItem.Name = culture.EnglishName;
//                        menuItem.Text = culture.NativeName;
//                        menuItem.Click += new EventHandler(menuItem_Click);
//                        menuItem.Tag = culture;
//                        menuCollection.Add(menuItem);
//                    }
//                    else menuItem = (ToolStripMenuItem)items[0];
//                }
//            }
//        }

//        private void languageToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
//        {
//            ToolStripItemCollection menuCollection = ((ToolStripMenuItem)sender).DropDownItems;
//            int i;

//            for (i = 0; i < menuCollection.Count; i++)
//            {
//                if (menuCollection[i].Tag != null && menuCollection[i].Tag is CultureInfo)
//                    ((ToolStripMenuItem)menuCollection[i]).Checked = cultureInfo.Equals((CultureInfo)menuCollection[i].Tag);
//            }
//        }
       
//        private void menuItem_Click(object sender, EventArgs e)
//        {
//            Thread.CurrentThread.CurrentUICulture = (CultureInfo)((ToolStripItem)sender).Tag;
//            cultureInfo = CultureInfo.CurrentUICulture;

//            ComponentResourceManager resources = new ComponentResourceManager(typeof(ModTunnelForm));
//            //resources.ApplyResources(this, "$this");
//            this.Text = resources.GetString("$this.Text");

//            commitControlLocale(resources, this.Controls);
//            foreach (Component component in this.components.Components)
//            {
//                if (component is ContextMenuStrip)
//                    commitMenuLocale(resources, ((ContextMenuStrip)component).Items);
//            }
//            commitColumnsLocale();
//            Properties.Settings.Default.CurrentCulture = cultureInfo.LCID;
//            Properties.Settings.Default.Save();
//            modTunnel_StartStateChanged(this, e);
//        }

//        private void commitControlLocale(ComponentResourceManager resources, Control.ControlCollection thisControls)
//        {
//            foreach (Control control in thisControls)
//            {
//                //resources.ApplyResources(control, control.Name);
//                control.Text = resources.GetString(control.Name + ".Text");
//                if (control is MenuStrip)
//                {
//                    commitMenuLocale(resources, ((MenuStrip)control).Items);
//                }
//                else commitControlLocale(resources, control.Controls);
//            }
//        }

//        private void commitMenuLocale(ComponentResourceManager resources, ToolStripItemCollection items)
//        {
//            foreach (ToolStripItem item in items)
//            {
//                resources.ApplyResources(item, item.Name);
//                if (item is ToolStripMenuItem)
//                    commitMenuLocale(resources, ((ToolStripMenuItem)item).DropDownItems);
//            }
//        }

//        private void commitColumnsLocale()
//        {
//            foreach (DataGridViewColumn column in monitoringDataGridView.Columns)
//            {
//                String name = resourceManager.GetString(column.Name + "ColumnHeader",cultureInfo);
//                if (name == null || name == "")
//                {
//                    name = resourceManager.GetString(column.Name + "ColumnHeader", CultureInfo.InvariantCulture);
//                    if (name == null || name == "")
//                        name = column.Name;
//                }
//                column.HeaderText = name;
//                if (column.Tag != null && column.Tag is ToolStripMenuItem)
//                    ((ToolStripMenuItem)column.Tag).Text = name;
//            }
//        }
//        #endregion

//        private void ModTunnelForm_Load(object sender, EventArgs e)
//        {
//            InitialiseLanguageMenu();
//            modTunnel_StartStateChanged(sender, e);
//        }
//    }
//}
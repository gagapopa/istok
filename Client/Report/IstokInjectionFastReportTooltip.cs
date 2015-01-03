using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using FastReport;
using FastReport.Design;
using FastReport.Design.ToolWindows;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окошко в FastReport'е с истоковскими настройками
    /// </summary>
    class IstokInjectionFastReportTooltip : ToolWindowBase
    {
        PropertyGrid reportParamPropertyGrid;
        TreeView reportSourceTreeView;

        public ReportNode ReportNode { get; set; }

        public StructureProvider StructureProvider { get; set; }

        public IstokInjectionFastReportTooltip(Designer designer)
            : base(designer)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var toolStrip = new ToolStrip();
            toolStrip.Dock = DockStyle.Top;
            var getReportDataToolTip = toolStrip.Items.Add("getReportData");
            getReportDataToolTip.Text = "Данные";
            getReportDataToolTip.Click += getReportDataToolTip_Click;

            this.reportParamPropertyGrid = new PropertyGrid();
            this.reportParamPropertyGrid.Dock = System.Windows.Forms.DockStyle.Top;
            //this.reportParamPropertyGrid.HelpVisible = false;
            this.reportParamPropertyGrid.Location = new System.Drawing.Point(3, 16);
            this.reportParamPropertyGrid.Name = "reportParamPropertyGrid";
            this.reportParamPropertyGrid.Size = new System.Drawing.Size(442, 321);
            reportParamPropertyGrid.PropertyValueChanged += reportParamPropertyGrid_PropertyValueChanged;

            // clear default tabs
            //List<Type> propertyTabTypes = new List<Type>();
            //foreach (var propertyTab in this.reportParamPropertyGrid.PropertyTabs)
            //{
            //    propertyTabTypes.Add(propertyTab.GetType());
            //}
            //foreach (var tabType in propertyTabTypes)
            //{
            //    this.reportParamPropertyGrid.PropertyTabs.RemoveTabType(tabType); 
            //}
            // add custom tabs
            this.reportParamPropertyGrid.PropertyTabs.AddTabType(typeof(ReportSettingPropertyTab), PropertyTabScope.Global);
            this.reportParamPropertyGrid.PropertyTabs.AddTabType(typeof(ReportParamsPropertyTab), PropertyTabScope.Global);

            reportSourceTreeView = new TreeView();
            reportSourceTreeView.CheckBoxes = true;
            reportSourceTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            reportSourceTreeView.FullRowSelect = true;
            reportSourceTreeView.Location = new System.Drawing.Point(0, 160);
            reportSourceTreeView.Name = "reportSourceTreeView";
            reportSourceTreeView.Size = new System.Drawing.Size(197, 120);
            reportSourceTreeView.TabIndex = 0;
            reportSourceTreeView.AfterCheck += reportSourceTreeView_AfterCheck;

            this.Text = "ИСТОК инжэкшен";// "Istok Injection";
            this.Size = new System.Drawing.Size(197, 400);
            this.MinimumSize = new System.Drawing.Size(197, 400);
            this.Name = GetType().ToString();
            this.Shortcut = FastReport.DevComponents.DotNetBar.eShortcut.CtrlJ;
            this.Icon = Properties.Resources.main;
            this.Image = Properties.Resources.report1;
            base.Control.Controls.Add(reportSourceTreeView);
            base.Control.Controls.Add(reportParamPropertyGrid);
            base.Control.Controls.Add(toolStrip);
        }

        void getReportDataToolTip_Click(object sender, EventArgs e)
        {
            var settingsList = new List<ReportSourceSettings>();
            foreach (var guid in ReportNode.GetReportSourcesGuids())
            {
                var settings = ReportNode.GetReportSetting(guid);
                if (settings != null && settings.Enabled)
                {
                    settingsList.Add(settings);
                }
            }

            var propertiesContainer = ReportNode.ParameterContainer;

            var wrap = StructureProvider.GenerateReportData(settingsList.ToArray(), propertiesContainer.Parameters.ToArray());

            RefreshData(wrap);
        }

        void RefreshData()
        {
            var settingsList = new List<ReportSourceSettings>();
            foreach (var guid in ReportNode.GetReportSourcesGuids())
            {
                var settings = ReportNode.GetReportSetting(guid);
                if (settings != null && settings.Enabled)
                {
                    settingsList.Add(settings);
                }
            }

            var wrap = StructureProvider.GenerateEmptyReportData(settingsList.ToArray());

            RefreshData(wrap);
        }

        private void RefreshData(FastReportWrap wrap)
        {
            Designer.Lock();
            Report frxReport = Designer.Report;

            // save dataSource bindings
            Dictionary<String, String> dataBindDictionary = new Dictionary<String, String>();
            foreach (Base obj in frxReport.AllObjects)
            {
                var objType = obj.GetType();
                var propertyInfo = objType.GetProperty("DataSource", typeof(FastReport.Data.DataSourceBase));
                if (propertyInfo != null)
                {
                    var dataSource = (FastReport.Data.DataSourceBase)propertyInfo.GetValue(obj, new Object[] { });
                    if (dataSource != null)
                    {
                        dataBindDictionary[obj.Name] = dataSource.Name;
                    }
                }
            }

            frxReport.Dictionary.ClearRegisteredData();
            frxReport.Dictionary.Clear();
            frxReport.Parameters.Clear();
            frxReport = wrap.UpdateReport(frxReport);

            if (Designer.Report == null)
                Designer.Report = frxReport;

            foreach (var item in wrap.DataSources)
            {
                FastReport.Data.DataSourceBase dataSource = frxReport.GetDataSource(item);
                if (dataSource != null)
                    dataSource.Enabled = true;
            }
            frxReport.Dictionary.UpdateRelations();

            // restore dataSource bindings
            foreach (var objName in dataBindDictionary.Keys)
            {
                var dataSource = frxReport.GetDataSource(dataBindDictionary[objName]);
                if (dataSource != null)
                {
                    var obj = (from o in frxReport.AllObjects.OfType<Base>() where o.Name == objName select o).First();
                    obj.GetType().GetProperty("DataSource", typeof(FastReport.Data.DataSourceBase)).SetValue(obj, dataSource, new Object[] { });
                }
            }

            Designer.Unlock();

            Designer.UpdatePlugins(this);
            Designer.Update();
        }

        void reportParamPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            RefreshReferences();
            RefreshData();
            reportParamPropertyGrid.Refresh();
        }

        void reportSourceTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.ByKeyboard || e.Action == TreeViewAction.ByMouse)
            {
                ReportSourceInfo info = e.Node.Tag as ReportSourceInfo;
                if (info != null)
                {
                    ReportSourceSettings settings = ReportNode.GetReportSetting(info.ReportSourceId); 
                    if (settings == null)
                    {
                        settings = StructureProvider.GetReportSourceSettings(info.ReportSourceId);
                        ReportNode.SetReportSetting(info.ReportSourceId, settings);
                    }
                    settings.Enabled = e.Node.Checked;
                    RefreshReferences();
                    RefreshData();
                }
            }
        }

        private void RefreshReferences()
        {
            HashSet<Guid> references = new HashSet<Guid>();
            int count = -1;

            while (count != references.Count)
            {
                count = references.Count;

                foreach (var guid in ReportNode.GetReportSourcesGuids())
                {
                    var settings = ReportNode.GetReportSetting(guid);

                    if (settings != null && settings.Enabled && settings.References != null)
                    {
                        foreach (var rf in settings.References)
                        {
                            references.Add(rf);
                        }
                    }
                }

                foreach (var guid in references)
                {
                    var settings = ReportNode.GetReportSetting(guid);
                    if (settings == null)
                    {
                        settings = StructureProvider.GetReportSourceSettings(guid);
                    }
                    settings.Enabled = true;
                    ReportNode.SetReportSetting(guid, settings);
                }
            }

            foreach (TreeNode treeNode in reportSourceTreeView.Nodes)
            {
                var info = treeNode.Tag as ReportSourceInfo;

                if (info!=null)
                {
                    var settings = ReportNode.GetReportSetting(info.ReportSourceId);

                    treeNode.Checked = settings != null && settings.Enabled;
                }
            }
            reportParamPropertyGrid.Refresh();
        }

        public override void UpdateContent()
        {
            base.UpdateContent();

            if (ReportNode != null && StructureProvider != null)
            {
                if (reportParamPropertyGrid.Site == null)
                {
                    reportParamPropertyGrid.Site = new UserInterfaceServiceContainer(StructureProvider.GetServiceContainer(), StructureProvider.Session);

                    reportParamPropertyGrid.SelectedObject = ReportNode;
                    RefreshData();
                }

                var sources = StructureProvider.GetReportSources();
                reportSourceTreeView.Nodes.Clear();

                foreach (var item in sources)
                {
                    if (item.Visible)
                    {
                        var settings = ReportNode.GetReportSetting(item.ReportSourceId);

                        var treeNode = reportSourceTreeView.Nodes.Add(item.ReportSourceId.ToString(), item.Caption);
                        treeNode.Tag = item;

                        if (settings != null)
                        {
                            treeNode.Checked = settings.Enabled;
                        }
                    }
                }

                RefreshReferences();
                reportParamPropertyGrid.Refresh();
            }
        }
    }
}

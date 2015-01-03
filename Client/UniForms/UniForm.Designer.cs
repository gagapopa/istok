namespace COTES.ISTOK.Client
{
    partial class UniForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmsTreeView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.schemaFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualsFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calcFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.addItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.setPropertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.getParamsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateSpravToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.referencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.statisticToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAuditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectionTimer = new System.Windows.Forms.Timer(this.components);
            this.tsFilter = new System.Windows.Forms.ToolStrip();
            this.tsbFolders = new System.Windows.Forms.ToolStripButton();
            this.tsbGraphs = new System.Windows.Forms.ToolStripButton();
            this.tsbSchemas = new System.Windows.Forms.ToolStripButton();
            this.tsbMonitors = new System.Windows.Forms.ToolStripButton();
            this.tsbManuals = new System.Windows.Forms.ToolStripButton();
            this.tsbCalcs = new System.Windows.Forms.ToolStripButton();
            this.tsbReports = new System.Windows.Forms.ToolStripButton();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbShowTree = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.structurePanel.SuspendLayout();
            this.cmsTreeView.SuspendLayout();
            this.tsFilter.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Size = new System.Drawing.Size(360, 228);
            this.splitContainer1.SplitterDistance = 181;
            // 
            // treeViewUnitObjects
            // 
            this.treeViewUnitObjects.ContextMenuStrip = this.cmsTreeView;
            this.treeViewUnitObjects.LineColor = System.Drawing.Color.Black;
            this.treeViewUnitObjects.SelectedNodes = new System.Windows.Forms.TreeNode[0];
            this.treeViewUnitObjects.Size = new System.Drawing.Size(181, 203);
            this.treeViewUnitObjects.OnDeselect += new System.EventHandler(this.treeViewUnitObjects_OnDeselect);
            this.treeViewUnitObjects.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewUnitObjects_BeforeSelect);
            this.treeViewUnitObjects.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewUnitObjects_AfterSelect);
            this.treeViewUnitObjects.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewUnitObjects_DragDrop);
            this.treeViewUnitObjects.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewUnitObjects_DragEnter);
            this.treeViewUnitObjects.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewUnitObjects_DragOver);
            this.treeViewUnitObjects.DragLeave += new System.EventHandler(this.treeViewUnitObjects_DragLeave);
            this.treeViewUnitObjects.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.treeViewUnitObjects_GiveFeedback);
            this.treeViewUnitObjects.DoubleClick += new System.EventHandler(this.treeViewUnitObjects_DoubleClick);
            // 
            // structurePanel
            // 
            this.structurePanel.Controls.Add(this.tsFilter);
            this.structurePanel.Size = new System.Drawing.Size(181, 228);
            this.structurePanel.Controls.SetChildIndex(this.tsFilter, 0);
            this.structurePanel.Controls.SetChildIndex(this.treeViewUnitObjects, 0);
            // 
            // cmsTreeView
            // 
            this.cmsTreeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateToolStripMenuItem,
            this.toolStripMenuItem4,
            this.filterToolStripMenuItem,
            this.toolStripMenuItem1,
            this.addItemToolStripMenuItem,
            this.editToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.removeItemToolStripMenuItem,
            this.toolStripMenuItem5,
            this.setPropertyToolStripMenuItem,
            this.toolStripMenuItem2,
            this.getParamsToolStripMenuItem,
            this.updateSpravToolStripMenuItem,
            this.deleteValuesToolStripMenuItem,
            this.toolStripMenuItem3,
            this.searchToolStripMenuItem,
            this.referencesToolStripMenuItem,
            this.toolStripMenuItem6,
            this.statisticToolStripMenuItem,
            this.viewAuditToolStripMenuItem});
            this.cmsTreeView.Name = "cmsTreeView";
            this.cmsTreeView.Size = new System.Drawing.Size(205, 348);
            this.cmsTreeView.Opening += new System.ComponentModel.CancelEventHandler(this.cmsTreeView_Opening);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.updateToolStripMenuItem.Text = "Обновить";
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.updateToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(201, 6);
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.folderFilterToolStripMenuItem,
            this.graphFilterToolStripMenuItem,
            this.schemaFilterToolStripMenuItem,
            this.monitorFilterToolStripMenuItem,
            this.manualsFilterToolStripMenuItem,
            this.calcFilterToolStripMenuItem,
            this.reportFilterToolStripMenuItem});
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.filterToolStripMenuItem.Text = "Фильтр";
            this.filterToolStripMenuItem.Visible = false;
            // 
            // folderFilterToolStripMenuItem
            // 
            this.folderFilterToolStripMenuItem.CheckOnClick = true;
            this.folderFilterToolStripMenuItem.Name = "folderFilterToolStripMenuItem";
            this.folderFilterToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.folderFilterToolStripMenuItem.Text = "Папки";
            this.folderFilterToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChange_Click);
            // 
            // graphFilterToolStripMenuItem
            // 
            this.graphFilterToolStripMenuItem.CheckOnClick = true;
            this.graphFilterToolStripMenuItem.DoubleClickEnabled = true;
            this.graphFilterToolStripMenuItem.Name = "graphFilterToolStripMenuItem";
            this.graphFilterToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.graphFilterToolStripMenuItem.Text = "Графики";
            this.graphFilterToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChange_Click);
            // 
            // schemaFilterToolStripMenuItem
            // 
            this.schemaFilterToolStripMenuItem.CheckOnClick = true;
            this.schemaFilterToolStripMenuItem.Name = "schemaFilterToolStripMenuItem";
            this.schemaFilterToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.schemaFilterToolStripMenuItem.Text = "Мнемосхемы";
            this.schemaFilterToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChange_Click);
            // 
            // monitorFilterToolStripMenuItem
            // 
            this.monitorFilterToolStripMenuItem.CheckOnClick = true;
            this.monitorFilterToolStripMenuItem.Name = "monitorFilterToolStripMenuItem";
            this.monitorFilterToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.monitorFilterToolStripMenuItem.Text = "Мониторинг";
            this.monitorFilterToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChange_Click);
            // 
            // manualsFilterToolStripMenuItem
            // 
            this.manualsFilterToolStripMenuItem.CheckOnClick = true;
            this.manualsFilterToolStripMenuItem.Name = "manualsFilterToolStripMenuItem";
            this.manualsFilterToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.manualsFilterToolStripMenuItem.Text = "Ручной ввод";
            this.manualsFilterToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChange_Click);
            // 
            // calcFilterToolStripMenuItem
            // 
            this.calcFilterToolStripMenuItem.CheckOnClick = true;
            this.calcFilterToolStripMenuItem.Name = "calcFilterToolStripMenuItem";
            this.calcFilterToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.calcFilterToolStripMenuItem.Text = "Расчет";
            this.calcFilterToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChange_Click);
            // 
            // reportFilterToolStripMenuItem
            // 
            this.reportFilterToolStripMenuItem.CheckOnClick = true;
            this.reportFilterToolStripMenuItem.Name = "reportFilterToolStripMenuItem";
            this.reportFilterToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.reportFilterToolStripMenuItem.Text = "Отчеты";
            this.reportFilterToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChange_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(201, 6);
            this.toolStripMenuItem1.Visible = false;
            // 
            // addItemToolStripMenuItem
            // 
            this.addItemToolStripMenuItem.Name = "addItemToolStripMenuItem";
            this.addItemToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.addItemToolStripMenuItem.Text = "Добавить";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.editToolStripMenuItem.Text = "Изменить";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyItemToolStripMenuItem,
            this.copyTreeToolStripMenuItem});
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.copyToolStripMenuItem.Text = "Копировать";
            // 
            // copyItemToolStripMenuItem
            // 
            this.copyItemToolStripMenuItem.Name = "copyItemToolStripMenuItem";
            this.copyItemToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.copyItemToolStripMenuItem.Text = "Элемент";
            this.copyItemToolStripMenuItem.Click += new System.EventHandler(this.copyItemToolStripMenuItem_Click);
            // 
            // copyTreeToolStripMenuItem
            // 
            this.copyTreeToolStripMenuItem.Name = "copyTreeToolStripMenuItem";
            this.copyTreeToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.copyTreeToolStripMenuItem.Text = "Ветку";
            this.copyTreeToolStripMenuItem.Click += new System.EventHandler(this.copyTreeToolStripMenuItem_Click);
            // 
            // removeItemToolStripMenuItem
            // 
            this.removeItemToolStripMenuItem.Name = "removeItemToolStripMenuItem";
            this.removeItemToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeItemToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.removeItemToolStripMenuItem.Text = "Удалить";
            this.removeItemToolStripMenuItem.Click += new System.EventHandler(this.removeItemToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(201, 6);
            // 
            // setPropertyToolStripMenuItem
            // 
            this.setPropertyToolStripMenuItem.Name = "setPropertyToolStripMenuItem";
            this.setPropertyToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.setPropertyToolStripMenuItem.Text = "Установить свойство";
            this.setPropertyToolStripMenuItem.Click += new System.EventHandler(this.setPropertyToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(201, 6);
            // 
            // getParamsToolStripMenuItem
            // 
            this.getParamsToolStripMenuItem.Name = "getParamsToolStripMenuItem";
            this.getParamsToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.getParamsToolStripMenuItem.Text = "Запросить параметры";
            this.getParamsToolStripMenuItem.Click += new System.EventHandler(this.getParamsToolStripMenuItem_Click);
            // 
            // updateSpravToolStripMenuItem
            // 
            this.updateSpravToolStripMenuItem.Name = "updateSpravToolStripMenuItem";
            this.updateSpravToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.updateSpravToolStripMenuItem.Text = "Обновить справочники";
            this.updateSpravToolStripMenuItem.Click += new System.EventHandler(this.updateSpravToolStripMenuItem_Click);
            // 
            // deleteValuesToolStripMenuItem
            // 
            this.deleteValuesToolStripMenuItem.Name = "deleteValuesToolStripMenuItem";
            this.deleteValuesToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.deleteValuesToolStripMenuItem.Text = "Пересобрать значения";
            this.deleteValuesToolStripMenuItem.Click += new System.EventHandler(this.deleteValuesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(201, 6);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.searchToolStripMenuItem.Text = "Поиск";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // referencesToolStripMenuItem
            // 
            this.referencesToolStripMenuItem.Name = "referencesToolStripMenuItem";
            this.referencesToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.referencesToolStripMenuItem.Text = "Поиск ссылок";
            this.referencesToolStripMenuItem.Click += new System.EventHandler(this.referencesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(201, 6);
            // 
            // statisticToolStripMenuItem
            // 
            this.statisticToolStripMenuItem.Name = "statisticToolStripMenuItem";
            this.statisticToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.statisticToolStripMenuItem.Text = "Показать статистику";
            this.statisticToolStripMenuItem.Click += new System.EventHandler(this.statisticToolStripMenuItem_Click);
            // 
            // viewAuditToolStripMenuItem
            // 
            this.viewAuditToolStripMenuItem.Name = "viewAuditToolStripMenuItem";
            this.viewAuditToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.viewAuditToolStripMenuItem.Text = "Просмотреть историю";
            this.viewAuditToolStripMenuItem.Click += new System.EventHandler(this.viewAuditToolStripMenuItem_Click);
            // 
            // selectionTimer
            // 
            this.selectionTimer.Interval = 350;
            this.selectionTimer.Tick += new System.EventHandler(this.selectionTimer_Tick);
            // 
            // tsFilter
            // 
            this.tsFilter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tsFilter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbFolders,
            this.tsbGraphs,
            this.tsbSchemas,
            this.tsbMonitors,
            this.tsbManuals,
            this.tsbCalcs,
            this.tsbReports});
            this.tsFilter.Location = new System.Drawing.Point(0, 203);
            this.tsFilter.Name = "tsFilter";
            this.tsFilter.Size = new System.Drawing.Size(181, 25);
            this.tsFilter.TabIndex = 16;
            this.tsFilter.Text = "toolStrip1";
            // 
            // tsbFolders
            // 
            this.tsbFolders.CheckOnClick = true;
            this.tsbFolders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFolders.Image = global::COTES.ISTOK.Client.Properties.Resources.folder_orange;
            this.tsbFolders.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFolders.Name = "tsbFolders";
            this.tsbFolders.Size = new System.Drawing.Size(23, 22);
            this.tsbFolders.Text = "Папки";
            this.tsbFolders.Click += new System.EventHandler(this.FilterChange_Click);
            // 
            // tsbGraphs
            // 
            this.tsbGraphs.CheckOnClick = true;
            this.tsbGraphs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGraphs.Image = global::COTES.ISTOK.Client.Properties.Resources.toolStripGraf;
            this.tsbGraphs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGraphs.Name = "tsbGraphs";
            this.tsbGraphs.Size = new System.Drawing.Size(23, 22);
            this.tsbGraphs.Text = "Графики";
            this.tsbGraphs.Click += new System.EventHandler(this.FilterChange_Click);
            // 
            // tsbSchemas
            // 
            this.tsbSchemas.CheckOnClick = true;
            this.tsbSchemas.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSchemas.Image = global::COTES.ISTOK.Client.Properties.Resources.toolStripMnem;
            this.tsbSchemas.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSchemas.Name = "tsbSchemas";
            this.tsbSchemas.Size = new System.Drawing.Size(23, 22);
            this.tsbSchemas.Text = "Мнемосхемы";
            this.tsbSchemas.Click += new System.EventHandler(this.FilterChange_Click);
            // 
            // tsbMonitors
            // 
            this.tsbMonitors.CheckOnClick = true;
            this.tsbMonitors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMonitors.Image = global::COTES.ISTOK.Client.Properties.Resources.unittype_default;
            this.tsbMonitors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMonitors.Name = "tsbMonitors";
            this.tsbMonitors.Size = new System.Drawing.Size(23, 22);
            this.tsbMonitors.Text = "Мониторинг";
            this.tsbMonitors.Click += new System.EventHandler(this.FilterChange_Click);
            // 
            // tsbManuals
            // 
            this.tsbManuals.CheckOnClick = true;
            this.tsbManuals.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbManuals.Image = global::COTES.ISTOK.Client.Properties.Resources.administrator1__edit__16x16;
            this.tsbManuals.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbManuals.Name = "tsbManuals";
            this.tsbManuals.Size = new System.Drawing.Size(23, 22);
            this.tsbManuals.Text = "Ручной ввод";
            this.tsbManuals.Click += new System.EventHandler(this.FilterChange_Click);
            // 
            // tsbCalcs
            // 
            this.tsbCalcs.CheckOnClick = true;
            this.tsbCalcs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCalcs.Image = global::COTES.ISTOK.Client.Properties.Resources.kcalc;
            this.tsbCalcs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCalcs.Name = "tsbCalcs";
            this.tsbCalcs.Size = new System.Drawing.Size(23, 22);
            this.tsbCalcs.Text = "Расчет";
            this.tsbCalcs.Click += new System.EventHandler(this.FilterChange_Click);
            // 
            // tsbReports
            // 
            this.tsbReports.CheckOnClick = true;
            this.tsbReports.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReports.Image = global::COTES.ISTOK.Client.Properties.Resources.excel;
            this.tsbReports.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbReports.Name = "tsbReports";
            this.tsbReports.Size = new System.Drawing.Size(23, 22);
            this.tsbReports.Text = "Отчеты";
            this.tsbReports.Click += new System.EventHandler(this.FilterChange_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.toolStrip1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(360, 27);
            this.flowLayoutPanel1.TabIndex = 17;
            this.flowLayoutPanel1.Visible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbShowTree});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(66, 25);
            this.toolStrip1.TabIndex = 17;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.Visible = false;
            // 
            // tsbShowTree
            // 
            this.tsbShowTree.Checked = true;
            this.tsbShowTree.CheckOnClick = true;
            this.tsbShowTree.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbShowTree.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbShowTree.Image = global::COTES.ISTOK.Client.Properties.Resources.view_tree;
            this.tsbShowTree.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbShowTree.Name = "tsbShowTree";
            this.tsbShowTree.Size = new System.Drawing.Size(23, 22);
            this.tsbShowTree.Text = "Отображать дерево";
            this.tsbShowTree.Click += new System.EventHandler(this.tsbShowTree_Click);
            // 
            // UniForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 277);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "UniForm";
            this.Text = "Структура";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UniForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.UniForm_FormClosed);
            this.Load += new System.EventHandler(this.UniForm_Load);
            this.Controls.SetChildIndex(this.flowLayoutPanel1, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.structurePanel.ResumeLayout(false);
            this.structurePanel.PerformLayout();
            this.cmsTreeView.ResumeLayout(false);
            this.tsFilter.ResumeLayout(false);
            this.tsFilter.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip cmsTreeView;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateSpravToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getParamsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyTreeToolStripMenuItem;
        private System.Windows.Forms.Timer selectionTimer;
        private System.Windows.Forms.ToolStrip tsFilter;
        private System.Windows.Forms.ToolStripButton tsbGraphs;
        private System.Windows.Forms.ToolStripButton tsbSchemas;
        private System.Windows.Forms.ToolStripButton tsbManuals;
        private System.Windows.Forms.ToolStripButton tsbCalcs;
        private System.Windows.Forms.ToolStripButton tsbMonitors;
        private System.Windows.Forms.ToolStripButton tsbFolders;
        private System.Windows.Forms.ToolStripButton tsbReports;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbShowTree;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem folderFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem graphFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem schemaFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem monitorFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualsFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calcFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem setPropertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem referencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem statisticToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewAuditToolStripMenuItem;
    }
}

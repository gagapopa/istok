namespace COTES.ISTOK.Client
{
    partial class ParamsSheduleEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParamsSheduleEditorForm));
            this.toolStripGruopEdit = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEdit = new System.Windows.Forms.ToolStripButton();
            this.cmsEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addScheduleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editScheduleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeScheduleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripGruopEdit.SuspendLayout();
            this.cmsEdit.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewSchedules
            // 
            this.listViewSchedules.ContextMenuStrip = this.cmsEdit;
            this.listViewSchedules.Margin = new System.Windows.Forms.Padding(0);
            this.listViewSchedules.Size = new System.Drawing.Size(299, 286);
            this.listViewSchedules.SelectedIndexChanged += new System.EventHandler(this.listViewSchedules_ItemSelectionChanged);
            this.listViewSchedules.DoubleClick += new System.EventHandler(this.listViewSchedules_DoubleClick);
            // 
            // toolStripGruopEdit
            // 
            this.toolStripGruopEdit.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripGruopEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAdd,
            this.toolStripButtonDelete,
            this.toolStripButtonEdit});
            this.toolStripGruopEdit.Location = new System.Drawing.Point(0, 0);
            this.toolStripGruopEdit.Name = "toolStripGruopEdit";
            this.toolStripGruopEdit.Size = new System.Drawing.Size(299, 25);
            this.toolStripGruopEdit.TabIndex = 13;
            this.toolStripGruopEdit.Text = "toolStripGroupEdit";
            this.toolStripGruopEdit.Visible = false;
            // 
            // toolStripButtonAdd
            // 
            this.toolStripButtonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAdd.Image")));
            this.toolStripButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAdd.Name = "toolStripButtonAdd";
            this.toolStripButtonAdd.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAdd.Text = "toolStripButtonAddGroup";
            this.toolStripButtonAdd.Click += new System.EventHandler(this.toolStripButtonAdd_Click);
            // 
            // toolStripButtonDelete
            // 
            this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDelete.Image")));
            this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDelete.Text = "toolStripButtonDeleteGroup";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // toolStripButtonEdit
            // 
            this.toolStripButtonEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonEdit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonEdit.Image")));
            this.toolStripButtonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEdit.Name = "toolStripButtonEdit";
            this.toolStripButtonEdit.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonEdit.Text = "toolStripButtonEditGroup";
            this.toolStripButtonEdit.Click += new System.EventHandler(this.toolStripButtonEdit_Click);
            // 
            // cmsEdit
            // 
            this.cmsEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addScheduleToolStripMenuItem,
            this.editScheduleToolStripMenuItem,
            this.removeScheduleToolStripMenuItem});
            this.cmsEdit.Name = "cmsEdit";
            this.cmsEdit.Size = new System.Drawing.Size(129, 70);
            // 
            // addScheduleToolStripMenuItem
            // 
            this.addScheduleToolStripMenuItem.Name = "addScheduleToolStripMenuItem";
            this.addScheduleToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.addScheduleToolStripMenuItem.Text = "Добавить";
            this.addScheduleToolStripMenuItem.Click += new System.EventHandler(this.addScheduleToolStripMenuItem_Click);
            // 
            // editScheduleToolStripMenuItem
            // 
            this.editScheduleToolStripMenuItem.Name = "editScheduleToolStripMenuItem";
            this.editScheduleToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.editScheduleToolStripMenuItem.Text = "Изменить";
            this.editScheduleToolStripMenuItem.Click += new System.EventHandler(this.editScheduleToolStripMenuItem_Click);
            // 
            // removeScheduleToolStripMenuItem
            // 
            this.removeScheduleToolStripMenuItem.Name = "removeScheduleToolStripMenuItem";
            this.removeScheduleToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.removeScheduleToolStripMenuItem.Text = "Удалить";
            this.removeScheduleToolStripMenuItem.Click += new System.EventHandler(this.removeScheduleToolStripMenuItem_Click);
            // 
            // ParamsSheduleEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(299, 308);
            this.Controls.Add(this.toolStripGruopEdit);
            this.MaximizeBox = false;
            this.Name = "ParamsSheduleEditorForm";
            this.Text = "Расписания";
            this.Load += new System.EventHandler(this.ParamsSheduleEditor_Load);
            this.Controls.SetChildIndex(this.listViewSchedules, 0);
            this.Controls.SetChildIndex(this.toolStripGruopEdit, 0);
            this.toolStripGruopEdit.ResumeLayout(false);
            this.toolStripGruopEdit.PerformLayout();
            this.cmsEdit.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripGruopEdit;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdd;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripButton toolStripButtonEdit;
        private System.Windows.Forms.ContextMenuStrip cmsEdit;
        private System.Windows.Forms.ToolStripMenuItem addScheduleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editScheduleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeScheduleToolStripMenuItem;
    }
}
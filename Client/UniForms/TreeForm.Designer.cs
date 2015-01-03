namespace COTES.ISTOK.Client
{
    partial class TreeForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.structurePanel = new System.Windows.Forms.Panel();
            this.treeViewUnitObjects = new COTES.ISTOK.Client.MultiSelectTreeViewControl();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.structurePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.structurePanel);
            this.splitContainer1.Size = new System.Drawing.Size(396, 302);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 14;
            // 
            // structurePanel
            // 
            this.structurePanel.Controls.Add(this.treeViewUnitObjects);
            this.structurePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.structurePanel.Location = new System.Drawing.Point(0, 0);
            this.structurePanel.Name = "structurePanel";
            this.structurePanel.Size = new System.Drawing.Size(200, 302);
            this.structurePanel.TabIndex = 3;
            // 
            // treeViewUnitObjects
            // 
            this.treeViewUnitObjects.AllowDrop = true;
            this.treeViewUnitObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewUnitObjects.HideSelection = false;
            this.treeViewUnitObjects.Location = new System.Drawing.Point(0, 0);
            this.treeViewUnitObjects.Name = "treeViewUnitObjects";
            this.treeViewUnitObjects.SelectedNodes = new System.Windows.Forms.TreeNode[0];
            this.treeViewUnitObjects.Size = new System.Drawing.Size(200, 302);
            this.treeViewUnitObjects.TabIndex = 2;
            this.treeViewUnitObjects.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewUnitObjects_BeforeExpand);
            this.treeViewUnitObjects.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeViewUnitObjects_ItemDrag);
            // 
            // TreeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(396, 324);
            this.Controls.Add(this.splitContainer1);
            this.Name = "TreeForm";
            this.Load += new System.EventHandler(this.TreeForm_Load);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.structurePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.SplitContainer splitContainer1;
        protected MultiSelectTreeViewControl treeViewUnitObjects;
        protected System.Windows.Forms.Panel structurePanel;

    }
}

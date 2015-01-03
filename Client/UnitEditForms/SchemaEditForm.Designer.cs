namespace COTES.ISTOK.Client
{
    partial class SchemaEditForm
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Size = new System.Drawing.Size(584, 353);
            this.splitContainer1.SplitterDistance = 307;
            // 
            // pgNode
            // 
            this.pgNode.Size = new System.Drawing.Size(273, 157);
            this.pgNode.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgNode_PropertyValueChanged);
            // 
            // pgParameter
            // 
            this.pgParameter.Size = new System.Drawing.Size(273, 179);
            this.pgParameter.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgParameter_PropertyValueChanged);
            // 
            // SchemaEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(584, 410);
            this.Name = "SchemaEditForm";
            this.Load += new System.EventHandler(this.SchemaEditForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion



    }
}

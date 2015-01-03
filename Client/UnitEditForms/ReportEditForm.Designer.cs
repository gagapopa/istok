namespace COTES.ISTOK.Client
{
    partial class ReportEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportEditForm));
            FastReport.Design.DesignerSettings designerSettings1 = new FastReport.Design.DesignerSettings();
            FastReport.Design.DesignerRestrictions designerRestrictions1 = new FastReport.Design.DesignerRestrictions();
            FastReport.Export.Email.EmailSettings emailSettings1 = new FastReport.Export.Email.EmailSettings();
            FastReport.PreviewSettings previewSettings1 = new FastReport.PreviewSettings();
            FastReport.ReportSettings reportSettings1 = new FastReport.ReportSettings();
            this.designerControl1 = new FastReport.Design.StandardDesigner.DesignerControl();
            this.environmentSettings1 = new FastReport.EnvironmentSettings();
            ((System.ComponentModel.ISupportInitialize)(this.designerControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // designerControl1
            // 
            this.designerControl1.AskSave = true;
            this.designerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.designerControl1.LayoutState = resources.GetString("designerControl1.LayoutState");
            this.designerControl1.Location = new System.Drawing.Point(0, 0);
            this.designerControl1.Name = "designerControl1";
            this.designerControl1.Size = new System.Drawing.Size(535, 205);
            this.designerControl1.TabIndex = 16;
            this.designerControl1.UIStyle = FastReport.Utils.UIStyle.VisualStudio2005;
            this.designerControl1.Load += new System.EventHandler(this.designerControl1_Load);
            // 
            // environmentSettings1
            // 
            designerSettings1.ApplicationConnection = null;
            designerSettings1.DefaultFont = new System.Drawing.Font("Arial", 10F);
            designerSettings1.Icon = ((System.Drawing.Icon)(resources.GetObject("designerSettings1.Icon")));
            designerSettings1.Restrictions = designerRestrictions1;
            designerSettings1.Text = "";
            this.environmentSettings1.DesignerSettings = designerSettings1;
            emailSettings1.Address = "";
            emailSettings1.Host = "";
            emailSettings1.MessageTemplate = "";
            emailSettings1.Name = "";
            emailSettings1.Password = "";
            emailSettings1.UserName = "";
            this.environmentSettings1.EmailSettings = emailSettings1;
            previewSettings1.Icon = ((System.Drawing.Icon)(resources.GetObject("previewSettings1.Icon")));
            previewSettings1.Text = "";
            this.environmentSettings1.PreviewSettings = previewSettings1;
            this.environmentSettings1.ReportSettings = reportSettings1;
            this.environmentSettings1.UIStyle = FastReport.Utils.UIStyle.VisualStudio2005;
            this.environmentSettings1.CustomSaveReport += new FastReport.Design.OpenSaveReportEventHandler(this.environmentSettings1_CustomSaveReport);
            // 
            // UltimateReportEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 262);
            this.Controls.Add(this.designerControl1);
            this.Name = "UltimateReportEditForm";
            this.Text = "UltimateReportEditForm";
            this.Controls.SetChildIndex(this.designerControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.designerControl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FastReport.Design.StandardDesigner.DesignerControl designerControl1;
        private FastReport.EnvironmentSettings environmentSettings1;
    }
}
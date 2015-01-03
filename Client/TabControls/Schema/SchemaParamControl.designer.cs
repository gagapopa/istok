namespace COTES.ISTOK.Client
{
  partial class SchemaParamControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <item name="disposing">true if managed resources should be disposed; otherwise, false.</item>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.toolTip = new System.Windows.Forms.ToolTip(this.components);
        this.SuspendLayout();
        // 
        // toolTip
        // 
        this.toolTip.AutoPopDelay = 5000;
        this.toolTip.InitialDelay = 50;
        this.toolTip.ReshowDelay = 10;
        this.toolTip.ShowAlways = true;
        this.toolTip.UseAnimation = false;
        this.toolTip.UseFading = false;
        // 
        // SchemParamControl
        // 
        this.Name = "SchemParamControl";
        this.Size = new System.Drawing.Size(54, 20);
        this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FieldControl_KeyDown);
        this.ResumeLayout(false);

    }

    #endregion

      public System.Windows.Forms.ToolTip toolTip;

  }
}

using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Контрол размещения параметров на мнемосхеме
    /// </summary>
    public partial class SchemaParamControl : UserControl
    {
        const int WM_KEYDOWN = 0x0100;
        static DoubleConverter doubleconv = new DoubleConverter();
        static DateTimeConverter dateconv = new DateTimeConverter();
        public static Color AlertColor = Color.FromArgb(255, 192, 192);
        public static Color WarningColor = Color.FromArgb(255, 255, 192);
        public static Color GoodColor = Color.FromArgb(192, 255, 192);
        private FontConverter fontConverter = new FontConverter(); 

        private string text;
        private string caption;
        private DateTime? fieldTime = null;
        private Double? fieldValue = null;
        private Double? oldFieldValue = null;

        private bool isSprDesign = false;

        private int quality = 0;
        
        private enum FormState { fsNone, fsMoving, fsResize }
        private FormState state = FormState.fsNone;
        private Point deltaLocation = new Point();
        protected bool initState = false;

        protected SchemaParamNode dataNode = null;
        public SchemaParamNode DataNode { get { return dataNode; } }

        public int ParameterId { get { try { return dataNode.ParameterId; } catch { return 0; } } }

        [Browsable(false)]
        public bool IsSprDesign { get { return isSprDesign; } }

        public event EventHandler OnPropertyModified;

        public SchemaParamControl()
        {
            InitializeComponent();

            this.TabStop = true;
            this.ResizeRedraw = true;
        }
        private SchemaParamControl( bool design)
            : this()
        {
            isSprDesign = design;
        }
        public SchemaParamControl(ChildParamNode node, Control owner, bool design)
            : this( design)
        {
            InitProperties(node as SchemaParamNode);
            AddToParent(owner);
        }
        public delegate void AddToParentDelegate(Control owner);
        void AddToParent(Control owner)
        {
            if (owner == null) return;
            if(owner.InvokeRequired) owner.Invoke(new AddToParentDelegate(AddToParent),new object[]{owner});
            else owner.Controls.Add(this);
        }
        void InitProperties(SchemaParamNode node)
        {
            dataNode = node;
            if (dataNode == null) return;
            BeginInit();


            if (dataNode.TextFont == null)
                dataNode.TextFont = Font;
            else
                this.Font = dataNode.TextFont;

            this.Left = dataNode.Left;
            this.Top = dataNode.Top;
            this.Width = dataNode.Width;
            this.Height = dataNode.Height;

            EndInit();
        }

        public void SetDefaultColors()
        {
            dataNode.NominalColor = GoodColor;
            dataNode.MinAlertColor = AlertColor;
            dataNode.MinWarningColor = WarningColor;
            dataNode.MaxAlertColor = AlertColor;
            dataNode.MaxWarningColor = WarningColor;
        }
        public void CopyProperty(SchemaParamControl fromControl)
        {
            dataNode.NominalColor = fromControl.DataNode.NominalColor;
            dataNode.MinAlertColor = fromControl.DataNode.MinAlertColor;
            dataNode.MinAlertValue = fromControl.DataNode.MinAlertValue;
            dataNode.MinWarningColor = fromControl.DataNode.MinWarningColor;
            dataNode.MinWarningValue = fromControl.DataNode.MinWarningValue;
            dataNode.MaxAlertColor = fromControl.DataNode.MaxAlertColor;
            dataNode.MaxAlertValue = fromControl.DataNode.MaxAlertValue;
            dataNode.MaxWarningColor = fromControl.DataNode.MaxWarningColor;
            dataNode.MaxWarningValue = fromControl.DataNode.MaxWarningValue;
            this.Font = fromControl.Font;
            this.Size = fromControl.Size;
            dataNode.DecNumber = fromControl.DataNode.DecNumber;
        }

        protected void BeginInit()
        {
            initState = true;
        }
        protected void EndInit()
        {
            SetBoundsCore(Left, Top, Width, Height, BoundsSpecified.All);
            SetBackgroundColorCore();
            initState = false;
        }
        protected void DoModified()
        {
            if (OnPropertyModified != null) OnPropertyModified(this, new EventArgs());
        }

        protected void SetToolTip()
        {
            string s = caption;
            if (!isSprDesign && this.fieldTime != null && this.fieldTime > DateTime.MinValue)
            {
                s = s + "\r\n" + this.fieldTime.Value.ToString("G");
            }

            this.toolTip.SetToolTip(this, s);
            if (!String.IsNullOrEmpty(s)) toolTip.ShowAlways = true;
        }

        public string Caption
        {
            get { return this.caption; }
            set
            {
                this.caption = value;
                SetToolTip();
            }
        }

        public override string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        public Double? Value
        {
            get { return this.fieldValue; }
            set
            {
                this.oldFieldValue = this.fieldValue;
                this.fieldValue = value;
                if (!isSprDesign)
                {
                    string formatStr = "";
                    if (dataNode.DecNumber != null) formatStr = String.Format("N{0}", dataNode.DecNumber);
                    this.text = value == null ? "" : ((double)value).ToString(formatStr, SchemFloatProvider.GetProvider());
                }
                SetBackgroundColorCore();
                this.Invalidate();
            }
        }
        /// <summary>
        /// Качество значения параметра
        /// </summary>
        [Browsable(false)]
        public int Quality
        {
            get { return quality; }
            set { quality = value; }
        }
        /// <summary>
        /// Интервал параметра
        /// </summary>
        [Browsable(false)]
        public int Interval { get; set; }
        /// <summary>
        /// Время актуальности мнемосхемы
        /// </summary>
        [Browsable(false)]
        public int Relevance { get; set; }
        // Провайдер 
        public class SchemFloatProvider : IFormatProvider
        {
            private static SchemFloatProvider usfloatProvider = new SchemFloatProvider();
            public static SchemFloatProvider GetProvider()
            {
                return usfloatProvider;
            }
            private System.Globalization.NumberFormatInfo provider;
            private SchemFloatProvider()
            {
                provider = new System.Globalization.NumberFormatInfo();
                provider.CurrencyGroupSeparator = "";
            }
            public object GetFormat(Type formatType)
            {
                return provider.GetFormat(formatType);
            }
        }

        [Browsable(false)]
        [Category("Свойства ячейки"), Description("Время поступления значения")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        public DateTime? Time
        {
            get { return this.fieldTime; }
            set
            {
                this.fieldTime = value;
                SetBackgroundColorCore();
                if (!isSprDesign)
                {
                    SetToolTip();
                }
            }
        }

        protected virtual void SetBackgroundColorCore()
        {
            Color backColor = base.BackColor;

            if (isSprDesign || fieldValue == null)
            { if (dataNode.NominalColor.IsEmpty) backColor = DefaultBackColor; else backColor = dataNode.NominalColor; }
            else
            {
                if (quality == 0)
                {
                    backColor = Color.Gray;
                    // return;
                }
                else
                {
                    if (Time != null && Interval > 0 && Relevance > 0
                        && DateTime.Now.Subtract((DateTime)Time).TotalSeconds > Interval + Relevance)
                    {
                        backColor = Color.LightGray;
                        //return;
                    }
                    else
                    {
                        double hyst;

                        if (dataNode.Hysteresis != null)
                            hyst = Math.Abs((double)dataNode.Hysteresis) / 100;
                        else
                            hyst = 0;

                        if (oldFieldValue == null)
                            hyst = 0f;

                        double hystPlus = 1f + hyst;
                        double hystMinus = 1f - hyst;

                        // в нижней зоне аварии
                        if (dataNode.MinAlertValue != null && fieldValue < hystMinus * dataNode.MinAlertValue)
                        {
                            if (dataNode.MinAlertColor.IsEmpty)
                                backColor = DefaultBackColor;
                            else backColor = dataNode.MinAlertColor;
                        }
                        // в нижней зоне предупреждения
                        else if (dataNode.MinWarningValue != null && dataNode.MinAlertValue != null
                            && (fieldValue < hystMinus * dataNode.MinWarningValue)
                            && (fieldValue > hystPlus * dataNode.MinAlertValue))
                        {
                            if (dataNode.MinWarningColor.IsEmpty)
                                backColor = DefaultBackColor;
                            else backColor = dataNode.MinWarningColor;
                        }

                        // в верхней зоне аварии
                        else if (dataNode.MaxAlertValue != null && fieldValue > hystPlus * dataNode.MaxAlertValue)
                        {
                            if (dataNode.MaxAlertColor.IsEmpty)
                                backColor = DefaultBackColor;
                            else backColor = dataNode.MaxAlertColor;
                        }
                        // в верхней зоне предупреждения
                        else if (dataNode.MaxWarningValue != null && dataNode.MaxAlertValue != null
                            && (fieldValue > hystPlus * dataNode.MaxWarningValue)
                            && (fieldValue < hystMinus * dataNode.MaxAlertValue))
                        {
                            if (dataNode.MaxWarningColor.IsEmpty)
                                backColor = DefaultBackColor;
                            else backColor = dataNode.MaxWarningColor;
                        }

                        // в норме
                        else/* if ((dataNode.MinAlertValue == null || fieldValue >= hystPlus * dataNode.MinAlertValue)
                          && (dataNode.MinWarningValue == null || fieldValue >= hystPlus * dataNode.MinWarningValue)
                          && (dataNode.MaxAlertValue == null || fieldValue <= hystMinus * dataNode.MaxAlertValue)
                          && (dataNode.MaxWarningValue == null || fieldValue <= hystMinus * dataNode.MaxWarningValue)
                          )*/
                        {
                            if (dataNode.NominalColor.IsEmpty)
                                backColor = DefaultBackColor;
                            else backColor = dataNode.NominalColor;
                        }
                    }
                }
            }
            if (backColor != base.BackColor)
            {
                base.BackColor = backColor;
                this.Invalidate();
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (width < 10) width = 10;
            if (height < 7) height = 7;
            if (Parent != null)
            {
                if (x < 0) x = 0;
                if (x + width > Parent.ClientSize.Width)
                {
                    if ((width > Width) && (Width > 10)) width = Width;
                    if ((x > Left) && (Left > 0)) x = Left;
                    if (x + width > Parent.ClientSize.Width) x = 0;
                    if (x + width > Parent.ClientSize.Width) width = Parent.ClientSize.Width;
                }
                if (y < 0) y = 0;
                if (y + height > Parent.ClientSize.Height)
                {
                    if ((height > Height) && (Height > 10)) height = Height;
                    if ((y > Top) && (Top > 0)) y = Top;
                    if (y + height > Parent.ClientSize.Height) y = 0;
                    if (y + height > Parent.ClientSize.Height) height = Parent.ClientSize.Height;
                }
            }
            if ((x != Left || y != Top || width != Width || height != Height))
            {
                base.SetBoundsCore(x, y, width, height, specified);
                if (!initState)
                {
                    if ((dataNode != null) && (IsSprDesign))
                    {
                        dataNode.Left = Left;
                        dataNode.Top = Top;
                        dataNode.Width = Width;
                        dataNode.Height = Height;
                    }
                    DoModified();
                }
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawString(Text, this.Font, Brushes.Black, new Point(0,0));
            if (isSprDesign)
            {
                Pen pen = new Pen(Color.Black, 1);
                if (Focused) pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                else pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                e.Graphics.DrawRectangle(pen, /*e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1*/0, 0, Width - 1, Height - 1);
                if (Focused)
                {
                    e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle.Right - 5, e.ClipRectangle.Bottom - 5, 5, 5);
                    e.Graphics.DrawRectangle(Pens.Black, e.ClipRectangle.Right - 6, e.ClipRectangle.Bottom - 6, 5, 5);
                }
            }
        }

        public override bool PreProcessMessage(ref System.Windows.Forms.Message msg)
        {
            if (msg.Msg == WM_KEYDOWN)
            {
                int keyCode = (Int32)msg.WParam;
                Keys key = (Keys)keyCode;
                if ((key == Keys.Left) || (key == Keys.Right) || (key == Keys.Up) || (key == Keys.Down))
                    return false;
                else
                    return base.PreProcessMessage(ref msg);
            }
            else
                return base.PreProcessMessage(ref msg);
        }

        private void FieldControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (isSprDesign)
            {
                switch (e.KeyCode)
                {
                    case (Keys.Down):
                        if (e.Control) this.Top++;
                        else if (e.Shift) this.Height++;
                        break;
                    case (Keys.Up):
                        if (e.Control) this.Top--;
                        else if (e.Shift) this.Height--;
                        break;
                    case (Keys.Right):
                        if (e.Control) this.Left++;
                        else if (e.Shift) this.Width++;
                        break;
                    case (Keys.Left):
                        if (e.Control) this.Left--;
                        else if (e.Shift) this.Width--;
                        break;
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (isSprDesign)
            {
                if ((e.Button == MouseButtons.Left) && (e.Clicks == 1))
                    StartDrag(e.Location);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (isSprDesign)
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isSprDesign)
            {
                if (CheckResizeLocation(e.Location))
                    Cursor = Cursors.SizeNWSE;
                else if (state == FormState.fsMoving)
                    Cursor = Cursors.SizeAll;
                else
                    Cursor = Cursors.Default;
                MoveDrag(e.Location);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (isSprDesign)
            {
                FinishDrag(e.Location);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.Invalidate();
        }

        bool CheckResizeLocation(Point mouseLocation)
        {
            if ((mouseLocation.X > (Width - 5))
              && (mouseLocation.Y > (Height - 5)))
                return true;
            else
                return false;
        }

        void StartDrag(Point mouseLocation)
        {
            deltaLocation.X = mouseLocation.X;
            deltaLocation.Y = mouseLocation.Y;
            if (CheckResizeLocation(mouseLocation))
                state = FormState.fsResize;
            else
                state = FormState.fsMoving;
        }

        void MoveDrag(Point mouseLocation)
        {
            switch (state)
            {
                case FormState.fsMoving:
                    int left = Left + (mouseLocation.X - deltaLocation.X);
                    int top = Top + (mouseLocation.Y - deltaLocation.Y);
                    SetBounds(left, top, Width, Height);
                    break;
                case FormState.fsResize:
                    int width = mouseLocation.X;
                    int height = mouseLocation.Y;
                    SetBounds(Left, Top, width, height);
                    break;
            }
        }

        void FinishDrag(Point mouseLocation)
        {
            state = FormState.fsNone;
            deltaLocation.X = 0; deltaLocation.Y = 0;
        }

        public void UpdateColor()
        {
            SetBackgroundColorCore();
            //
        }
    }
}

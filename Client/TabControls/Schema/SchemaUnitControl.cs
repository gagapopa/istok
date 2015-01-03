using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using System.Threading;
using System.IO;
using COTES.ISTOK.ClientCore.UnitProviders;
using System.Threading.Tasks;

namespace COTES.ISTOK.Client
{
    partial class SchemaUnitControl : BaseUnitControl
    {
        SchemaUnitProvider schemaUnitProvider;
        SchemaNode schemaNode = null;

        private SchemaParamControl selectedControl = null;
        private SchemaParamControl lastEditingControl = null;

        private SchemaReceiver sReceiver = null;
        //private System.Threading.Timer timer;
        //private ParamReceiveItem[] schem_values = null;

        GraphUnitProvider graphProvider = null;
        GraphNode graphNode = null;
        Form graphForm = null;

        Point mousePos;

        public delegate void ControlSelectedDelegate(SchemaParamControl control);
        public event ControlSelectedDelegate SelectedControlChanged = null;

        ///// <summary>
        ///// Номер транзакции обновления параметров
        ///// </summary>
        //int taID = 0;

        public SchemaUnitControl(SchemaUnitProvider unitProvider, bool editMode)
            : base(unitProvider)
        {
            InitializeComponent();
            EditMode = editMode;
        }
        public SchemaUnitControl(SchemaUnitProvider unitProvider)
            : this(unitProvider, false)
        {
            //
        }

        public override void InitiateProcess()
        {
            if (UnitProvider is SchemaUnitProvider)
            {
                schemaUnitProvider = (SchemaUnitProvider)UnitProvider;
                if (!schemaUnitProvider.NewUnitNode.Equals(schemaNode))
                {
                    //schemaNode = (SchemaNode)schemaUnitProvider.UnitNode;
                    schemaNode = (SchemaNode)schemaUnitProvider.NewUnitNode;
                    CreateSchema();
                    timer1.Enabled = !EditMode;
                }
            }
        }

        public bool EditMode { get; set; }

        private void CreateSchema()
        {
            try
            {
                FreeSchema();
                if (schemaNode == null) return;

                if (!schemaNode.BackColor.Equals(Color.Empty))
                    this.BackColor = schemaNode.BackColor;
                else
                    this.BackColor = SystemColors.Control;

                byte[] image = schemaNode.ImageBuffer;
                if (image != null)
                {
                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream(image))
                    {
                        pictureBox.Image = Image.FromStream(stream);
                        stream.Close();
                    }
                }

                if (!EditMode && schemaNode.Parameters != null)
                {
                    sReceiver = new SchemaReceiver(pictureBox, new MouseEventHandler(pictureBox_MouseDoubleClick));
                    sReceiver.EditMode = EditMode;
                    sReceiver.AddParameters(schemaNode, unitProvider);
                    // вывести последние значения

                    //schemaUnitProvider.UpdateInterval = schemaNode.UpdateInterval;
                    timer1.Interval = schemaUnitProvider.UpdateInterval;
                    var task = Task.Factory.StartNew(() => schemaUnitProvider.RegisterParameters());
                    task.ContinueWith((x) =>
                    {
                        timer1.Interval = schemaUnitProvider.UpdateInterval;
                        UpdateValues();
                    });
                }
                else
                    AddParameters(schemaNode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FreeSchema()
        {
            try
            {
                sReceiver = null;
                //schemaUnitProvider.UnregisterParameters();
                pictureBox.Controls.Clear();
                pictureBox.Image = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void FreeGraphForm()
        {
            if (graphForm != null && graphForm.IsHandleCreated && !graphForm.IsDisposed)
            {
                graphForm.Dispose();
                graphForm = null;
            }
        }

        protected override void DisposeControl()
        {
            FreeSchema();
            FreeGraphForm();
            if (schemaUnitProvider != null) schemaUnitProvider.UnregisterParameters();
            base.DisposeControl();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            FreeGraphForm();
            base.OnHandleDestroyed(e);
        }

        private void pictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!EditMode)
            {
                mousePos = pictureBox.PointToClient(MousePosition);
                AddParamToGraph(pictureBox.GetChildAtPoint(mousePos) as SchemaParamControl);
            }
        }

        public async void AddParamToGraph(SchemaParamControl param)
        {
            GraphUnitControl control = null;
            ParameterNode child;
            BaseUnitControl[] controls;

            if (param == null)
                return;

            DateTime beginTime, endTime;

            if (param.Time != null)
                endTime = (DateTime)param.Time;
            else  
                endTime = DateTime.Now;
            beginTime = endTime.Subtract(TimeSpan.FromMinutes(5));

            if (graphForm == null || graphForm.IsDisposed)
            {
                graphForm = new Form();
                graphForm.Width = 740;
                graphForm.Height = 480;
                graphForm.FormClosed += new FormClosedEventHandler(graphForm_FormClosed);
                graphForm.Text = "График";
            }
            if (graphNode == null)
            {
                graphNode = new GraphNode();
                graphNode.Text = UnitNode.Text;
            }
            if (graphProvider == null)
            {
                graphProvider = new GraphUnitProvider(unitProvider.StructureProvider, graphNode);
                GraphUnitProviderState state = graphProvider.GetState(null);
                state.GraphPeriod = GraphTimePeriod.User;
                state.GraphFrom = beginTime;
                state.GraphTo = endTime;
                graphProvider.DataReady = true;
            }

            bool found = false;
            if (graphNode.Parameters != null)
            {
                foreach (var item in graphNode.Parameters)
                    if (item.ParameterId == param.ParameterId)
                    {
                        found = true;
                        break;
                    }
            }
            if (!found)
            {
                child = await Task.Factory.StartNew(() => unitProvider.StructureProvider.GetUnitNode(param.ParameterId) as ParameterNode);
                graphNode.AddChildParam(child);

                graphForm.Hide();
                graphForm.SuspendLayout();
                graphForm.Controls.Clear();
                controls = UnitProviders.CreateControls(graphProvider, UniForm);
                if (controls != null && controls.Length == 1) control = controls[0] as GraphUnitControl;
                if (control != null)
                {
                    control.Dock = DockStyle.Fill;
                    graphForm.Controls.Add(control);
                }
                graphForm.ResumeLayout();
                graphProvider.ClearValues(null);
                graphProvider.QueryGraphData(null, beginTime, endTime, true);

                control.InitiateProcess();
            }

            graphForm.Show();
            graphForm.Activate();
            //if (param == null) return;

            //if (graphForm == null)
            //{
            //    graphForm = new ViewJurForm(this);
            //    graphForm.Text = "Графики";
            //}
            //graphForm.TreeCollapsed = true;
            //graphForm.TreeMenuVisible = false;
            //graphForm.TableCollapsed = true;
            //graphForm.TableMenuVisible = true;

            //if (graphForm.AddParam(param))
            //{
            //    if (!graphForm.Visible) graphForm.Show();
            //    if (graphForm.WindowState == FormWindowState.Minimized) graphForm.WindowState = FormWindowState.Normal;
            //    graphForm.Activate();
            //    graphForm.BringToFront();
            //    graphForm.QueryRun();
            //}
        }

        void graphForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (graphProvider != null)
            {
                graphProvider.Dispose();
                graphProvider = null;
            }
            if (graphNode != null)
            {
                graphNode = null;
            }
        }

        public void UpdateSchema()
        {
            CreateSchema();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateValues();
        }

        private void UpdateValues()
        {
            if (schemaUnitProvider.ParamValues != null)
            {
                lock (schemaUnitProvider.ParamValues)
                {
                    foreach (ParamValueItemWithID item in schemaUnitProvider.ParamValues)
                        sReceiver.ValueReceived(item);
                }
                sReceiver.UpdateAll();
            }
        }

        private void addParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddParameter();
        }

        /// <summary>
        /// Добавляет на схему параметры из нода
        /// </summary>
        /// <param name="node">Нод мнемосхемы</param>
        private void AddParameters(SchemaNode node)
        {
            if (node.Parameters != null)
            {
                foreach (SchemaParamNode item in node.Parameters)
                {
                    SchemaParamControl c = new SchemaParamControl(item, pictureBox, EditMode);
                    SetupControl(c, item);
                }
            }
        }

        private void SetupControl(SchemaParamControl control, SchemaParamNode param)
        {
            if (control == null) throw new ArgumentNullException("control");
            if (param == null) throw new ArgumentNullException("param");

            control.Text = "0.0";
            control.Caption = param.Text;
            control.Relevance = schemaNode.Relevance;
            control.Enter += new EventHandler(FieldControl_GotFocus);
            control.Leave += new EventHandler(FieldControl_LostFocus);
            control.OnPropertyModified += new EventHandler(spc_OnPropertyModified);
            control.ContextMenuStrip = cmsPicture;
        }

        #region Редактирование мнемосхемы
        /// <summary>
        /// Добавляет новый параметр на мнемосхему
        /// </summary>
        private void AddParameter()
        {
            SelectForm frm = new SelectForm(unitProvider.StructureProvider);
            frm.Filter.Add((int)UnitTypeId.Parameter);
            frm.Filter.Add((int)UnitTypeId.TEP);
            frm.Filter.Add((int)UnitTypeId.ManualParameter);
            mousePos = pictureBox.PointToClient(MousePosition);
            frm.ShowDialog();
            if (frm.SelectedObjects != null)
            {
                foreach (ParameterNode item in frm.SelectedObjects)
                {
                    SchemaParamNode param = new SchemaParamNode();
                    param.SetParameter(item);
                    SchemaParamControl c = new SchemaParamControl(param, pictureBox, EditMode);
                    SetupControl(c, param);
                    c.Location = mousePos;
                    if (lastEditingControl != null)
                        c.CopyProperty(lastEditingControl);
                    else
                        c.SetDefaultColors();
                    mousePos.X += 2;
                    mousePos.Y += 2;
                    schemaNode.AddChildParam(param);
                }
            }
        }
        /// <summary>
        /// Удаляет параметр с мнемосхемы
        /// </summary>
        private void RemoveParameter()
        {
            if (selectedControl == null) return;
            if (schemaNode.Parameters != null)
            {
                foreach (var item in schemaNode.Parameters)
                {
                    if (item.Equals(selectedControl.DataNode))
                    {
                        schemaNode.RemoveChildParam(item);
                        break;
                    }
                }
            }
            foreach (Control item in pictureBox.Controls)
            {
                if (item.Equals(selectedControl))
                {
                    pictureBox.Controls.Remove(item);
                    break;
                }
            }
        }
        /// <summary>
        /// Загружает фоновую картинку с локального компьютера
        /// </summary>
        private void LoadImage()
        {
            if ((openFileDialog.ShowDialog(this) == DialogResult.OK))
            {
                try
                {
                    MemoryStream stream;
                    byte[] bb = System.IO.File.ReadAllBytes(openFileDialog.FileName);
                    stream = new MemoryStream(bb);
                    pictureBox.Image = Image.FromStream(stream);
                    if (schemaNode != null) schemaNode.ImageBuffer = bb;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
        /// <summary>
        /// Сохраняет фоновую картинку мнемосхемы на локальный компьютер
        /// </summary>
        private void SaveImage()
        {
            if ((saveFileDialog.ShowDialog(this) == DialogResult.OK))
            {
                try
                {
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        //pictureBox.Image.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        if (schemaNode != null) fs.Write(schemaNode.ImageBuffer, 0, schemaNode.ImageBuffer.Length);
                        fs.Close();
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
        /// <summary>
        /// Удаляет фоновую картинку мнемосхемы
        /// </summary>
        private void ClearImage()
        {
            pictureBox.Image = null;
            if (schemaNode != null) schemaNode.ImageBuffer = null;
        }
        #endregion

        private void FieldControl_GotFocus(object sender, EventArgs e)
        {
            if (sender is SchemaParamControl)
            {
                selectedControl = (SchemaParamControl)sender;
                if (SelectedControlChanged != null)
                    SelectedControlChanged(selectedControl);
            }
        }
        private void FieldControl_LostFocus(object sender, EventArgs e)
        {
            if (sender is SchemaParamControl) selectedControl = null;
        }
        private void spc_OnPropertyModified(object sender, EventArgs e)
        {
            if (sender is SchemaParamControl) lastEditingControl = sender as SchemaParamControl;
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            selectedControl = pictureBox.GetChildAtPoint(e.Location) as SchemaParamControl;
        }

        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            //LoadImage();
        }

        private void loadPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadImage();
        }

        private void savePictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveImage();
        }

        private void clearPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearImage();
        }

        private void cmsPicture_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = !EditMode;
            deleteParameterToolStripMenuItem.Enabled = selectedControl != null;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void deleteParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedControl == null) return;
            RemoveParameter();
        }
    }

    class SchemaReceiver
    {
        private PictureBox pictureBox;
        //private SchemaParamNode[] arrParams;
        private MouseEventHandler OnDoubleClick;

        public bool EditMode { get; set; }

        private int schemaRelevance = 0;

        public SchemaReceiver()
        {
            pictureBox = null;
        }
        public SchemaReceiver(PictureBox control, MouseEventHandler OnDoubleClick)
            : this()
        {
            this.pictureBox = control;
            this.OnDoubleClick = OnDoubleClick;
        }
        public void SetControl(PictureBox control, MouseEventHandler OnDoubleClick)
        {
            this.pictureBox = control;
        }

        private delegate void ValueReceiveEventHandler(ParamValueItemWithID Param);

        public void AddParameters(SchemaParamNode[] parameters)
        {
            SchemaParamNode param;
            //arrParams = parameters;
            foreach (SchemaParamNode par in parameters)
            {
                param = (SchemaParamNode)par.Clone();
                SchemaParamControl c = new SchemaParamControl(param, pictureBox, EditMode);
                if (param.Attributes.ContainsKey(CommonData.IntervalProperty))
                {
                    int res = 0;
                    int.TryParse(param.Attributes[CommonData.IntervalProperty], out res);
                    c.Interval = res;
                }
                c.Relevance = schemaRelevance;
                c.Caption = param.Text;
                c.Text = "";
                c.MouseDoubleClick += OnDoubleClick;
                //AddParameter(param.ParameterId);
            }
        }
        /// <summary>
        /// Добавляет параметры из указанной мнемосхемы
        /// (низя вызывать более раза для мнемосхемы - испортится свойство Актуальность)
        /// </summary>
        /// <param name="currentSchema"></param>
        public async void AddParameters(SchemaNode currentSchema, UnitProvider unitProvider)
        {
            if (currentSchema.Parameters != null && currentSchema.Parameters.Length > 0)
            {
                List<SchemaParamNode> lstParams = new List<SchemaParamNode>();
                SchemaParamNode param;
                foreach (var item in currentSchema.Parameters)
                {
                    param = (SchemaParamNode)item.Clone();
                    string interval;

                    interval = await Task.Factory.StartNew(() => unitProvider.StructureProvider.GetParameterInterval(param.ParameterId).ToString());
                    param.Attributes[CommonData.IntervalProperty] = interval;
                    lstParams.Add(param);
                }

                schemaRelevance = currentSchema.Relevance;
                AddParameters(lstParams.ToArray());
            }
        }

        private SchemaParamControl[] FindControls(int paramId)
        {
            List<SchemaParamControl> lst = new List<SchemaParamControl>();
            foreach (Control c in pictureBox.Controls)
                if (c is SchemaParamControl && (c as SchemaParamControl).ParameterId == paramId)
                    lst.Add(c as SchemaParamControl);
            return lst.ToArray();
        }
        public void ValueReceived(ParamValueItemWithID Param)
        {

            if (pictureBox == null || Param == null) return;

            if (pictureBox.InvokeRequired)
            { pictureBox.Invoke(new ValueReceiveEventHandler(ValueReceived), new object[] { Param }); }
            else
            {
                SchemaParamControl[] c = FindControls(Param.ParameterID);
                if (c != null)
                {
                    foreach (var item in c)
                    {
                        if (!item.InvokeRequired)
                        {
                            item.Quality = (int)Param.Quality;
                            //item.Relevance = schemaRelevance;
                            if (item.Value != Param.Value && !item.Value.Equals(Param.Value)) item.Value = Param.Value;
                            if (item.Time != Param.Time) item.Time = Param.Time;
                        }
                        else item.BeginInvoke(new ValueReceiveEventHandler(ValueReceived), new object[] { Param });
                    }
                }
            }
        }

        public void UpdateAll()
        {
            SchemaParamControl control;

            foreach (Control c in pictureBox.Controls)
            {
                control = c as SchemaParamControl;
                if (control != null)
                {
                    control.UpdateColor();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    partial class SchemaEditForm : COTES.ISTOK.Client.ParamEditForm
    {
        SchemaUnitControl schema = null;
        SchemaUnitProvider sup = null;

        public SchemaEditForm()
            : base()
        {
            InitializeComponent();
        }
        public SchemaEditForm(StructureProvider strucProvider, UnitNode unitNode)
            : base(strucProvider, unitNode)
        {
            InitializeComponent();
        }
        public SchemaEditForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
        }

        private void Init()
        {
            sup = strucProvider.GetUnitProvider(UnitNode) as SchemaUnitProvider;
            //pgNode.SelectedObject = gup.CreateDynamicObject();
            //isHistogram = UnitNode is HistogramNode;
            //dgv.Columns["clmLine"].Visible = !isHistogram;
            //pgNode.SelectedObject = UnitNode;
            //LoadParameters();
            schema = new SchemaUnitControl(sup, true);
            schema.InitiateProcess();
            schema.SelectedControlChanged += new SchemaUnitControl.ControlSelectedDelegate(schema_SelectedControlChanged);
            schema.Dock = DockStyle.Fill;
            //schema.EditMode = true;
            splitContainer1.Panel1.Controls.Add(schema);
            pgNode.SelectedObject = UnitNode;
        }

        protected override void SaveUnit()
        {
            if (sup != null) UnitNode = sup.NewUnitNode;
            base.SaveUnit();
        }

        void schema_SelectedControlChanged(SchemaParamControl control)
        {
            pgParameter.SelectedObject = control.DataNode;
        }

        private void SchemaEditForm_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void pgParameter_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            schema.UpdateSchema();
        }

        private void pgNode_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            schema.UpdateSchema();
        }
    }
}

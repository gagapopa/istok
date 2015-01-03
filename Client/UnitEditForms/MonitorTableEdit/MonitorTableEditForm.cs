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
    partial class MonitorTableEditForm : ParamEditForm
    {
        MonitorTableUnitControl monitor = null;

        public MonitorTableEditForm(StructureProvider strucProvider, UnitNode unitNode)
            : base(strucProvider, unitNode)
        {
            InitializeComponent();
        }
        public MonitorTableEditForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
        }

        private void Init()
        {
            MonitorTableUnitProvider mtup = strucProvider.GetUnitProvider(UnitNode) as MonitorTableUnitProvider;
            monitor = new MonitorTableUnitControl(mtup, true);
            monitor.SelectedControlChanged += new MonitorTableUnitControl.ControlSelectedDelegate(monitor_SelectedControlChanged);
            monitor.Dock = DockStyle.Fill;
            splitContainer1.Panel1.Controls.Add(monitor);
            pgNode.SelectedObject = UnitNode;
        }

        void monitor_SelectedControlChanged(ChildParamNode control)
        {
            pgParameter.SelectedObject = control;
        }

        private void MonitorTableEditForm_Load(object sender, EventArgs e)
        {
            Init();
        }
    }
}

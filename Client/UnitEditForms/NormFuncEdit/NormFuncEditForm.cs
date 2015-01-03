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
    partial class NormFuncEditForm : ButtonEditForm
    {
        NormFuncUnitControl normFunc;

        public NormFuncEditForm(StructureProvider strucProvider, UnitNode unitNode)
            : base(strucProvider, unitNode)
        {
            InitializeComponent();
            Init();
        }
        public NormFuncEditForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            NormFuncUnitProvider nfup = strucProvider.GetUnitProvider(UnitNode) as NormFuncUnitProvider;
            normFunc = new NormFuncUnitControl(nfup, true);
            normFunc.InitiateProcess();
            normFunc.Dock = DockStyle.Fill;
            panel1.Controls.Add(normFunc);
        }

        protected override void SaveUnit()
        {
            normFunc.UpdateNode();
            UnitNode = (NormFuncNode)normFunc.UnitProvider.NewUnitNode;
            base.SaveUnit();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    partial class ParamEditForm : ButtonEditForm
    {
        public ParamEditForm()
            : base()
        {
            InitializeComponent();
        }
        public ParamEditForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
        }
        public ParamEditForm(StructureProvider strucProvider, UnitNode unitNode)
            : base(strucProvider, unitNode)
        {
            InitializeComponent();
            pgNode.Site = strucProvider.GetServiceContainer();
            pgParameter.Site = pgNode.Site;
        }
    }
}

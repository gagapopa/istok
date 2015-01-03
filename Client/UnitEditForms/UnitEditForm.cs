using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    partial class UnitEditForm : BaseAsyncWorkForm// Form
    {
        private UnitNode origNode = null;

        public UnitEditForm()
            : base()
        {
            InitializeComponent();
        }
        public UnitEditForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
        }
        public UnitEditForm(StructureProvider strucProvider, UnitNode unitNode)
            : this(strucProvider)
        {
            origNode = unitNode;
            UnitNode = (UnitNode)unitNode.Clone();
        }

        public delegate void SaveUnitDelegate(UnitNode unitNode);
        public virtual event SaveUnitDelegate OnSaveUnit = null;

        public UnitNode UnitNode { get; set; }

        protected virtual void SaveUnit()
        {
            //тут должно быть хитрое сравнение origNode и UnitNode, затем условие на вызов сохранения
            if (!origNode.Equals(UnitNode) && OnSaveUnit != null)
                OnSaveUnit(UnitNode);
        }

        private void UnitEditForm_Load(object sender, EventArgs e)
        {
            ClientSettings.Instance.LoadFormState(this);
        }

        private void UnitEditForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClientSettings.Instance.SaveFormState(this);
        }
    }
}

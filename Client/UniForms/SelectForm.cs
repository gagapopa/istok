using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    public partial class SelectForm : COTES.ISTOK.Client.TreeForm
    {
        public SelectForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();

            if (Program.MainForm != null)
                treeViewUnitObjects.ImageList = Program.MainForm.Icons;
            splitContainer1.Panel2Collapsed = true;
        }

        public UnitNode[] SelectedObjects { get; protected set; }

        private void SelectForm_Load(object sender, EventArgs e)
        {
            //
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private async void btnSelect_Click(object sender, EventArgs e)
        {
            List<UnitNode> lstNodes = new List<UnitNode>();
            UnitNode ptr;

            foreach (var item in treeViewUnitObjects.SelectedNodes)
            {
                if (item.Tag != null && item.Tag is int)
                {
                    if ((ptr = await Task.Factory.StartNew(() => strucProvider.GetUnitNode((int)item.Tag))) != null)
                    {
                        if (Filter != null && !Filter.IsEmpty)
                        {
                            if (Filter.Check(ptr.Typ)) lstNodes.Add(ptr);
                        }
                        else
                            lstNodes.Add(ptr);
                    }
                }
            }

            SelectedObjects = lstNodes.ToArray();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

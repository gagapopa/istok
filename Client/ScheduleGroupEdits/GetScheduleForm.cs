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
    partial class GetScheduleForm : BaseScheduleForm
    {
        public bool SelectPerfomed
        { get; set; }

        public string ScheduleName
        { get; set; }

        public int ScheduleID
        { get; set; }

        public GetScheduleForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();

            SelectPerfomed = false;
        }

        private void listViewSchedules_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.Enabled = listViewSchedules.SelectedIndices.Count != 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            var it = listViewSchedules.SelectedItems[0];
            ScheduleID = (int)it.Tag;
            ScheduleName = it.Text;
            SelectPerfomed = true;
        }

        private void GetScheduleForm_Load(object sender, EventArgs e)
        {
            LoadSchedule();
        }
    }
}

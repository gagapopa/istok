using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using FastReport;
using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.ClientCore;
using System.Threading.Tasks;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно просмотра готовых отчётов.
    /// </summary>
    partial class CreatedReportsForm : BaseAsyncWorkForm
    {
        MemoryStream reportStream = null;

        Dictionary<int, UnitNode> dicUnits = new Dictionary<int, UnitNode>();
        Dictionary<int, UserNode> dicUsers = new Dictionary<int, UserNode>();

        COTES.ISTOK.ASC.PreferedReportInfo[] arrReports = new COTES.ISTOK.ASC.PreferedReportInfo[0];

        int firstReport = 0;
        int reportCount = 0;

        private double step = 0f;

        public CreatedReportsForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();

            FillTimePeriod();
            cbxPeriod.SelectedValue = GraphTimePeriod.Days30;
            //datTo.Value = DateTime.Today;
            datFrom.Value = DateTime.Today.AddSeconds(-step / 2);
            reportPropertiesContainer = new ReportParametersContainer();
            reportPropertiesContainer.ReadOnly = true;
        }

        // TODO: hide or not hide???

        //protected virtual void CatchDieSilently()
        //{
        //    this.Close();
        //}

        private void Init()
        {
            //UpdateData();
        }

        private async void UpdateData()
        {
            if (this.InvokeRequired)
                this.Invoke((Action)UpdateData);
            else
            {
                //AsyncOperationWatcher<COTES.ISTOK.ASC.PreferedReportInfo[]> watcher =
                var rep = await Task.Factory.StartNew(() => strucProvider.GetPreferedReports(datFrom.Value, datTo.Value));
                //dgv.Rows.Clear();
                dicUnits.Clear();
                dicUsers.Clear();
                arrReports = new COTES.ISTOK.ASC.PreferedReportInfo[0];
                //watcher.AddValueRecivedHandler(AddReports);
                //this.RunWatcher(watcher);
                AddReports(rep);
            }
        }

        private void FillTimePeriod()
        {
            DataTable periods = new DataTable();

            periods.Columns.Add(new DataColumn("Text", typeof(string)));
            periods.Columns.Add(new DataColumn("Value", typeof(GraphTimePeriod)));

            periods.Rows.Add(GraphTimePeriodFormatter.Format(GraphTimePeriod.Days1),
                GraphTimePeriod.Days1);
            periods.Rows.Add(GraphTimePeriodFormatter.Format(GraphTimePeriod.Days30),
                GraphTimePeriod.Days30);
            periods.Rows.Add(GraphTimePeriodFormatter.Format(GraphTimePeriod.User),
                GraphTimePeriod.User);

            cbxPeriod.DisplayMember = "Text";
            cbxPeriod.ValueMember = "Value";
            cbxPeriod.DataSource = periods;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void AddReports(COTES.ISTOK.ASC.PreferedReportInfo[] ri)
        {
            List<int> lstNodeIds = new List<int>();
            List<int> lstUserIds = new List<int>();
            bool unitsReady = false, usersReady = false;

            if (ri != null)
            {
                foreach (var item in ri)
                {
                    if (!lstNodeIds.Contains(item.ReportId)) lstNodeIds.Add(item.ReportId);
                    if (!lstUserIds.Contains(item.UserId)) lstNodeIds.Add(item.UserId);
                }
            }
            //AsyncOperationWatcher<Object> watcher = 
            var nds = await Task.Factory.StartNew(() => strucProvider.GetUnitNodes(lstNodeIds.ToArray()));
            //watcher.AddValueRecivedHandler((object val) =>
            //{
            //    UnitNode[] nodes = val as UnitNode[];
            //    if (nodes != null)
            //        foreach (var node in nodes)
            //            dicUnits[node.Idnum] = node;

            //});
            //watcher.AddFinishHandler((COTES.ISTOK.AsyncOperationWatcher.FinishNotificationDelegate)(() => unitsReady = true));
            //RunWatcher(watcher);
            if (nds != null)
                foreach (var item in nds)
                    dicUnits[item.Idnum] = item;
            unitsReady = true;

            var usr = await Task.Factory.StartNew(() => strucProvider.Session.GetUserNodes(lstUserIds.ToArray()));
            //watcher.AddValueRecivedHandler((object val) =>
            //{
            //    UserNode[] users = val as UserNode[];
            //    if (users != null)
            //        foreach (var user in users)
            //            dicUsers[user.Idnum] = user;
            //});
            //watcher.AddFinishHandler((COTES.ISTOK.AsyncOperationWatcher.FinishNotificationDelegate)(() => usersReady = true));
            //RunWatcher(watcher);
            if (usr != null)
                foreach (var item in usr)
                    dicUsers[item.Idnum] = item;
            usersReady = true;

            while (!usersReady || !unitsReady) Thread.Sleep(30);
            DrawReports(ri);
        }

        private void DrawReports(COTES.ISTOK.ASC.PreferedReportInfo[] reports)
        {
            if (this.InvokeRequired)
                this.Invoke((Action)(() => DrawReports(reports)));
            else
            {
                dgv.Rows.Clear();
                if (reports != null)
                {
                    List<COTES.ISTOK.ASC.PreferedReportInfo> lstReports = new List<COTES.ISTOK.ASC.PreferedReportInfo>(arrReports);
                    lstReports.AddRange(reports);
                    arrReports = lstReports.ToArray();
                }
                DrawReports();
            }
        }

        private void DrawReports()
        {
            DataGridViewRow row;
            COTES.ISTOK.ASC.PreferedReportInfo item;
            int start;

            dgv.Rows.Clear();

            var lst = (from elem in arrReports
                       where (elem.CreationDate >= datFrom.Value && elem.CreationDate <= datTo.Value) 
                       select elem).ToArray();

            start = chbxPages.Checked ? 0 : firstReport;
            reportCount = lst.Length;
            for (int i = start; (i - start < udPageParams.Value || !chbxPages.Checked) && i < lst.Length; i++)
            {
                item = lst[i];
                row = dgv.Rows[dgv.Rows.Add()];
                row.Cells["clmReportName"].Value = dicUnits.ContainsKey(item.ReportId) ? dicUnits[item.ReportId].Text : "";
                row.Cells["clmUserName"].Value = dicUsers.ContainsKey(item.UserId) ? dicUsers[item.UserId].Text : "";
                row.Cells["clmDate"].Value = item.CreationDate;
                row.Tag = item;
            }
            UpdatePageInfo();
        }

        private async void dgv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            COTES.ISTOK.ASC.PreferedReportInfo ri;

            if (dgv.CurrentRow == null) return;
            ri = dgv.CurrentRow.Tag as COTES.ISTOK.ASC.PreferedReportInfo;
            if (ri != null)
            {
                //AsyncOperationWatcher<byte[]> watcher = 
                var body = await Task.Factory.StartNew(() => strucProvider.GetPreferedReportBody(ri));
                //watcher.AddValueRecivedHandler(value => reportStream = new MemoryStream(value));
                //watcher.AddSuccessFinishHandler(ShowReport);
                //RunWatcher(watcher);
                reportStream = new MemoryStream(body);
                ShowReport();
            }
        }

        private void ShowReport()
        {
            if (this.InvokeRequired)
                this.Invoke((Action)ShowReport);
            else
            {
                if (reportStream != null)
                {
                    Report frx = new Report();
                    frx.LoadPrepared(reportStream);
                    frx.ShowPrepared();
                }
            }
        }

        private void CreatedReportsForm_Load(object sender, EventArgs e)
        {
            Init();
        }

        #region Инфо о страницах
        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (firstReport + udPageParams.Value < arrReports.Length)
            {
                firstReport += (int)udPageParams.Value;
                DrawReports();
            }
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (firstReport - udPageParams.Value >= 0)
            {
                firstReport -= (int)udPageParams.Value;
                DrawReports();
            }
        }

        private void UpdatePageInfo()
        {
            lblPageInfo.Text = string.Format("{0}/{1}",
                reportCount == 0 ? 0 : firstReport / udPageParams.Value + 1,
                Math.Ceiling((decimal)reportCount / udPageParams.Value));
            btnNextPage.Enabled = chbxPages.Checked && (firstReport + udPageParams.Value < reportCount);
            btnPrevPage.Enabled = chbxPages.Checked && (firstReport - udPageParams.Value >= 0);
        }

        private void udPageParams_ValueChanged(object sender, EventArgs e)
        {
            firstReport = 0;
            DrawReports();
        }
        #endregion

        private void MoveDate(int direction)
        {
            double shift;

            if (step == 0)
                step = Math.Abs(datTo.Value.Subtract(datFrom.Value).TotalSeconds);

            shift = (double)direction * step;
            datFrom.Value = datFrom.Value.AddSeconds(shift);
        }
        private void UpdateDateTo()
        {
            if (step != 0)
                datTo.Value = datFrom.Value.AddSeconds(step);
        }

        private void btnNextDate_Click(object sender, EventArgs e)
        {
            MoveDate(1);
        }

        private void btnPrevDate_Click(object sender, EventArgs e)
        {
            MoveDate(-1);
        }

        private void cbxPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            GraphTimePeriod period;

            if (cbxPeriod.SelectedValue == null) return;

            period = (GraphTimePeriod)cbxPeriod.SelectedValue;
            step = GraphTimePeriodFormatter.FormatInterval(period);
            switch (period)
            {
                case GraphTimePeriod.Minutes30:
                    datTo.Enabled = false;
                    break;
                case GraphTimePeriod.Hours1:
                    datTo.Enabled = false;
                    break;
                case GraphTimePeriod.Hours4:
                    datTo.Enabled = false;
                    break;
                case GraphTimePeriod.Days1:
                    datTo.Enabled = false;
                    break;
                case GraphTimePeriod.Days30:
                    datTo.Enabled = false;
                    break;
                //case GraphTimePeriod.User:
                default:
                    datTo.Enabled = true;
                    break;
            }
            UpdateDateTo();
            UpdateData();
            DrawReports();
        }

        private void datFrom_ValueChanged(object sender, EventArgs e)
        {
            UpdateDateTo();
            UpdateData();
            DrawReports();
        }

        private void datTo_ValueChanged(object sender, EventArgs e)
        {
            if (datTo.Enabled)
            {
                UpdateData();
                DrawReports();
            }
        }

        private void chbxPages_CheckedChanged(object sender, EventArgs e)
        {
            DrawReports();
            if (!chbxPages.Checked)
            {
                btnPrevPage.Enabled = btnNextPage.Enabled = udPageParams.Enabled = chbxPages.Checked;
            }
            else
            {
                udPageParams.Enabled = chbxPages.Checked;
                UpdatePageInfo();
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                if (dgv.SelectedRows[0].Tag is COTES.ISTOK.ASC.PreferedReportInfo)
                {
                    if (strucProvider.Session.User.CheckPrivileges((int)UnitTypeId.Report, Privileges.Write) ||
                        strucProvider.Session.User.IsAdmin)
                    {
                        COTES.ISTOK.ASC.PreferedReportInfo report = (COTES.ISTOK.ASC.PreferedReportInfo)dgv.SelectedRows[0].Tag;
                        strucProvider.DeletePreferedReport(report);
                        //watcher.AddFinishHandler(UpdateData);
                        //this.RunWatcher(watcher);
                        UpdateData();
                    }
                    else
                        MessageBox.Show("Недостаточно прав.");
                }
            }
        }

        ReportParametersContainer reportPropertiesContainer = new ReportParametersContainer();

        private void dgv_CurrentCellChanged(object sender, EventArgs e)
        {
            COTES.ISTOK.ASC.PreferedReportInfo reportInfo = null;
            if (dgv.CurrentCell!=null)
            {
                DataGridViewRow gridRow = dgv.Rows[dgv.CurrentCell.RowIndex];
                reportInfo = gridRow.Tag as COTES.ISTOK.ASC.PreferedReportInfo; 
            }

            reportPropertiesContainer.Clear();

            if (reportInfo!=null)
            {
               reportPropertiesContainer.SetProperties(reportInfo.ReportParameters);
            }
            reportPropertyGrid.SelectedObject = reportPropertiesContainer;
            reportPropertyGrid.ExpandAllGridItems();
        }
    }
}

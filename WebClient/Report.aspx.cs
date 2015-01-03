using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using COTES.ISTOK.ASC;
using COTES.ISTOK;
using FastReport;
using FastReport.Web;
using System.IO;

namespace WebClient
{
    public partial class ReportPage : TreeContentPage
    {
        ReportNode node = null;

        DateTime dateFrom = DateTime.Today, dateTo = DateTime.Today;

        ReportParametersContainer PropertiesContainer;

        protected void Page_Load(object sender, EventArgs e)
        {
            string sid = Parameters[@"ID"];
            int id = 0;

            //if (!DateTime.TryParse(FromDate.Text, out dateFrom))
            //{
            //    dateFrom = DateTime.Today;
            //    FromDate.Text = dateFrom.ToString("dd.MM.yyyy HH:mm:ss");
            //}
            //if (!DateTime.TryParse(ToDate.Text, out dateTo))
            //{
            //    dateTo = DateTime.Today;
            //    ToDate.Text = dateTo.ToString("dd.MM.yyyy HH:mm:ss");
            //}

            if (!string.IsNullOrEmpty(sid))
            {
                int.TryParse(sid, out id);
                node = this.GetDataService().GetUnitNode(id) as ReportNode;
            }
            if (node != null && node.Typ == UnitTypeId.Report)
                lblReport.Text = node.Text;
            else
                lblReport.Text = "";

            UltimatePropertyGrid1.TypeDescriptorContext = new WebClientServiceContainer(this.GetDataService());
            if (node != null)
                ObtainProperties(node);

            //UltimatePropertyGrid1.Show(new WebClientServiceContainer(  this.GetDataService()), 

            //string q = string.Format("sf=document.getElementById('{0}').value;", FromDate.ClientID);
            //q += string.Format("st=document.getElementById('{0}').value;", ToDate.ClientID);
            //q += string.Format(@"var ex=/:|\.|\s+/g;var p='?id={0}&f='+sf.replace(ex,'')+'&t='+st.replace(ex,'');", node.Idnum);
            //q += "var w=window.open('ReportView.aspx'+p);w.focus();return false;";
            //btnMake.OnClientClick = q;
        }

        private void ObtainProperties(ReportNode reportNode)
        {
            Guid[] guids = reportNode.GetReportSourcesGuids();
            List<ReportParameter> reportParameters = new List<ReportParameter>();

            if (guids != null)
                foreach (var sourceGuid in guids)
                {
                    ReportSourceSettings settings = reportNode.GetReportSetting(sourceGuid);
                    if (settings != null && settings.Enabled)
                    {
                        ReportParameter[] properties = settings.GetReportParameters();
                        if (properties != null)
                            reportParameters.AddRange(properties);
                    }
                }
            if (PropertiesContainer == null)
                PropertiesContainer = new ReportParametersContainer();
            PropertiesContainer.SetProperties(reportParameters);
            UltimatePropertyGrid1.SelectedObject = PropertiesContainer;
            UltimatePropertyGrid1.Show();
            //UltimatePropertyGrid1.Show(new WebClientServiceContainer(this.GetDataService()), PropertiesContainer);
            //SaveInSystem = true;
        }

        protected void btnMake_Click(object sender, EventArgs e)
        {
            byte[] report = this.GetDataService().GenerateReport(node, true, PropertiesContainer.Parameters.ToArray());
            MemoryStream ms = new MemoryStream(report);

            frw.Visible = true;
            Report frx = frw.Report;
            frx.LoadPrepared(ms);
            //frw.AutoWidth
            frw.ReportDone = true;
            frx.Refresh();
            //frx.UpdateLayout(0, 0);
            frw.LastPage();
            frw.FirstPage();
            //frw.SetPage(0);
            //frw.Refresh();
        }

        public void frw_PreRender(object sender, EventArgs e)
        {
            //frw.Width = new Unit(frw.PrintWindowWidth);
            //frw.Height= new Unit(frw.PrintWindowHeight);
            //frw.PrintWindowWidth
        }

        public void frw_StartReport(object sender, EventArgs e)
        {
            //byte[] res = this.GetDataService().GenerateReport(node, dateFrom, dateTo, false);
            //MemoryStream ms = new MemoryStream(res);

            //Report frx = frw.Report;
            //frx.LoadPrepared(ms);
            //frw.ReportDone = true;
        }

        //private void GenerateReport()
        //{
        //    //try
        //    //{
        //    //    byte[] res = this.GetDataService().GenerateReport(node, dateFrom, dateTo, false);
        //    //    MemoryStream ms = new MemoryStream(res);
        //    //    //WebReport frx = new WebReport();

        //    //    //string fname = string.Format("D:\\report{0}.fpx", node.Idnum);
        //    //    //string fname = string.Format("D:\\report_a.frx");
        //    //    //using (FileStream fs = new FileStream(fname, FileMode.Create))
        //    //    //{
        //    //    //    BinaryWriter bw = new BinaryWriter(fs);
        //    //    //    if (res != null) bw.Write(res);
        //    //    //    bw.Close();
        //    //    //}
        //    //    Report frx = frw.Report;
        //    //    frx.LoadPrepared(ms);
        //    //    frw.ReportDone = true;
        //    //    //frw.ReportFile = fname;
        //    //    //frw.Prepare();
        //    //    //frw.ExportExcel2007();
        //    //    //frw.ExportText();
        //    //}
        //    //catch
        //    //{
        //    //    //
        //    //}
        //}
    }
}

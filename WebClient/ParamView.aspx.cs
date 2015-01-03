using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using COTES.ISTOK.ASC;
using COTES.ISTOK;

namespace WebClient
{
    public partial class ParamView : TreeContentPage
    {
        ParameterGateNode node = null;
        DateTime date = DateTime.Today;

        public ParamView()
        {
            //
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetSpecificTab(true);
            string sid = Parameters[@"ID"];
            string sdate = "";

            if (Parameters.Exist("date")) FromDate.Text = Parameters["date"];
            if (string.IsNullOrEmpty(FromDate.Text)) FromDate.Text = DateTime.Today.ToString("dd.MM.yyyy HH:mm:ss");
            if (!DateTime.TryParseExact(FromDate.Text, "dd.MM.yyyy HH:mm:ss",
                                   System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU"),
                                   System.Globalization.DateTimeStyles.None, out date))
            {
                date = DateTime.Today;
                FromDate.Text = "";
            }

            //if (string.IsNullOrEmpty(FromDate.Text))
            //{
            //    //date = DateTime.Today;
            //    //FromDate.Text = date.ToString("dd.MM.yyyy HH:mm:ss");
            //    FromDate.Text = "";
            //}
            //else
            //    DateTime.TryParseExact(FromDate.Text, "dd.MM.yyyy HH:mm:ss",
            //                       System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU"),
            //                       System.Globalization.DateTimeStyles.None, out date);

            GetDataButton.Click += new ImageClickEventHandler(GetDataButton_Click);
            
            int id = string.IsNullOrEmpty(sid) ? 0 : int.Parse(sid);

            UnitNode unode = DataService.GetUnitNode(id);

            if (unode == null || !(unode is ParameterGateNode)) return;

            node = (ParameterGateNode)unode;
            DrawTable();

            //FromDate.Text = date.ToString();//"dd.MM.yyyy HH:mm:ss"
            Parameters["date"] = FromDate.Text;
        }

        void GetDataButton_Click(object sender, ImageClickEventArgs e)
        {
            //Parameters["date"] = DateTime.Today.ToString("dd.MM.yyyy HH:mm:ss");
            //DrawTable();
        }

        public void LeftIB_Click(object sender, EventArgs e)
        {
            if (node == null || node.Interval == null) return;
            date = node.Interval.GetPrevTime(date);
            FromDate.Text = date.ToString("dd.MM.yyyy HH:mm:ss");
            DrawTable();
        }

        public void RightIB_Click(object sender, EventArgs e)
        {
            if (node == null || node.Interval == null) return;
            date = node.Interval.GetNextTime(date);
            FromDate.Text = date.ToString("dd.MM.yyyy HH:mm:ss");
            DrawTable();
        }

        private void DrawTable()
        {
            ParamValueItemWithID[] res = new ParamValueItemWithID[0];
            ParamValueItemWithID pri;
            DateTime dateFrom = date, dateTo;

            if (node == null) return;

            dateTo = node.Interval.GetNextTime(dateFrom);

            ParamTable.Caption = node.Text;
            ParamTable.Rows.Clear();
            ParamTable.Rows.Add(CreateTableRow(null, null, DateTime.Now));

            if (node.ManualParameters == null)
            {
                UnitTypeId[] filterTypes;
                var lst = new List<ParameterNode>();

                if (node.Typ == UnitTypeId.ManualGate) filterTypes = new UnitTypeId[] { UnitTypeId.ManualParameter };
                else if (node.Typ == UnitTypeId.TEPTemplate) filterTypes = new UnitTypeId[] { UnitTypeId.TEP };
                else filterTypes = new UnitTypeId[0];

                lst.AddRange(DataService.GetUnitNodes(node.Idnum, filterTypes).Cast<ParameterNode>());

                node.ManualParameters = lst;
            }

            res = DataService.GetValues(node.ManualParameters.ConvertAll<int>(x => x.Idnum).ToArray(),
                dateFrom, dateTo, node.Interval, CalcAggregation.First, false).ToArray();

            foreach (var item in node.ManualParameters)
            {
                pri = null;
                foreach (var ptr in res) if (ptr.ParameterID == item.Idnum) { pri = ptr; break; }

                ParamTable.Rows.Add(CreateTableRow(item, pri, DateTime.Now));
            }
        }

        private TableRow CreateTableRow(ParameterNode pnode, ParamValueItem param, DateTime time)
        {
            TableRow row = new TableRow();
            TableCell cell;
            bool e = pnode == null;

            cell = new TableCell();
            cell.CssClass = e ? "cellHeader" : "cellParam";
            cell.Text = e ? "Параметр" : string.IsNullOrEmpty(pnode.DocIndex) ? pnode.Text : string.Format("{0} {1}", pnode.DocIndex, pnode.Text);
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = e ? "cellHeader" : "cellCode";
            cell.Text = e ? "Код" : pnode.Code;
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = e ? "cellHeader" : "cellValue";
            cell.Text = e ? "Значение" : param == null ? "" : param.Value.ToString();
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.CssClass = e ? "cellHeader" : "cellDate";
            cell.Text = e ? "Время" : param == null ? "" : param.Time.ToString();
            row.Cells.Add(cell);

            return row;
        }
    }
}

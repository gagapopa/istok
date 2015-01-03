using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using COTES.ISTOK.ASC;
using System.Collections.Generic;
using COTES.ISTOK;

namespace WebClient
{
    public partial class Table : StateMonitorPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SetSpecificTab(true);
                
                int id = int.Parse(Parameters[Configuration.Get(Setting.IdObjectMarker)]);

                MonitorTableDescriptor descriptor = 
                    DataService.GetMonitorTableDescriptor(id);

                MonitorTable.Caption = descriptor.Name;

                DownloadButtonBuilder builder = 
                    new DownloadButtonBuilder();
                TableRow temp_row = new TableRow();

                var rows = descriptor.GetTable();
                foreach (IEnumerable<CellDescriptor> cell_list in rows)
                {
                    temp_row = new TableRow();

                    foreach (var cell in cell_list)
                        temp_row.Cells.Add(CreateCell(cell, builder));

                    MonitorTable.Rows.Add(temp_row);
                }

                MonitorTable.GridLines = GridLines.Both;
            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                Handler.Instance.ProcessUndefined(exp);
            }
        }

        private TableCell CreateCell(CellDescriptor cell,
                                     DownloadButtonBuilder builder)
        {
            TableCell result = new TableCell();

            if (cell == null) return result;

            if (cell.IsLink)
            {
                builder.File = cell.Content;
                builder.Text = cell.Text;

                result.Controls.Add((ImageButton)builder);
                //result.Controls.Add((HyperLink)builder);
            }
            else
                result.Text = cell.Text;

            return result;
        }

        protected void UpdateValues(object sender, EventArgs e)
        {
            try
            {
                ParametersPageManager parameters =
                    ParametersPageManager.FromQueryStrings(Request.QueryString);
                int id = int.Parse(Parameters[Configuration.Get(Setting.IdObjectMarker)]);

                WebRemoteDataService data_service = DataService; 

                MonitorTableDescriptor descriptor =
                    data_service.GetMonitorTableDescriptor(id);

                if (descriptor.UpdateTransactionID != 0)
                {
                    ParamValueItemWithID[] values =
                        data_service.GetUpdatedValues(descriptor.UpdateTransactionID);

                    foreach (var p in values)
                        foreach (var pd in descriptor[p.ParameterID])
                        {
                            if (pd.Cell != null)
                            {
                                MonitorTable.Rows[pd.Cell.Row].Cells[pd.Cell.Column].Text = p.Value.ToString();
                                MonitorTable.Rows[pd.Cell.Row].Cells[pd.Cell.Column].ToolTip =
                                    FormatTooltip(pd.Name, p.Time);
                            }
                        }
                }
            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                Handler.Instance.ProcessUndefined(exp);
            }
        }

        protected void UpdatePeriodChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateTimer.Interval = IntervalUpdateControl.Period;
            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                Handler.Instance.ProcessUndefined(exp);
            }
        }
    }
}

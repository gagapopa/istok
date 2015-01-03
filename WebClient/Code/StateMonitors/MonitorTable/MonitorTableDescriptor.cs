using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace WebClient
{
    public class MonitorTableDescriptor : 
        BaseStateMonitor<MonitorTableParameterDescriptor>
    {
        private List<CellDescriptor> cells = new List<CellDescriptor>();
        private List<String> columns = new List<string>(); 

        public MonitorTableDescriptor(int id,
                                      string name,
                                      int transaction_id)
            :base(id, name, transaction_id)
        { }

        public void AddCell(CellDescriptor cell)
        {
            cells.Add(cell);
        }

        public IEnumerable<IEnumerable<CellDescriptor>> GetTable()
        {
            List<List<CellDescriptor>> result =
                new List<List<CellDescriptor>>();
            
            foreach (var cell in cells)
            {
                while (result.Count <= cell.Row)
                    result.Add(new List<CellDescriptor>());
                while (result[cell.Row].Count <= cell.Column)
                    result[cell.Row].Add(CellDescriptor.ClearCell);

                result[cell.Row][cell.Column] = cell;
            }

            return result.Cast<IEnumerable<CellDescriptor>>();
        }
    }
}

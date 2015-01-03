using System;
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
using System.IO;

namespace WebClient
{
    public class CellDescriptor
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public string Link { get; private set; }
        public Guid Content { get; set; }
        public int ParameterID { get; private set; }
        public string Text { get; private set; }

        public readonly static CellDescriptor ClearCell =
            new CellDescriptor();

        private CellDescriptor()
        { }

        public CellDescriptor(byte[] raw_data)
        {
            using (MemoryStream stream = new MemoryStream(raw_data))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    Row = reader.ReadInt32();
                    Column = reader.ReadInt32();
                    ParameterID = reader.ReadInt32();
                    Text = reader.ReadString();
                    Link = reader.ReadString();
                }
        }

        public CellDescriptor(string data, 
                              int row, 
                              int column)
        {
            Row = row;
            Column = column;
            Text = data;
        }

        public void CorrectLink(string link)
        {
            Link = link;
        }

        public bool IsParameterCell
        {
            get
            {
                return ParameterID != 0;
            }
        }

        public bool IsLink
        {
            get
            {
                return Link != null &&
                       Link != String.Empty;
            }
        }
    }
}

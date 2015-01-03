using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;

namespace WebClient
{
    public partial class PropertiesView : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //
        }

        [CssClassProperty]
        public string CssCategory { get; set; }
        [CssClassProperty]
        public string CssProperty { get; set; }

        public WebClientServiceContainer ServiceContainer { get; private set; }

        public void Show(UnitObjectDescriptor obj_deskriptor)
        {
            try
            {
                PropertyTable.Caption = obj_deskriptor.Caption;
                
                foreach (var c in obj_deskriptor)
                {
                    if (c != string.Empty)
                        DisplayCategory(c);

                    foreach (var p in obj_deskriptor[c])
                        DisplayProperty(p);
                }
            }
            catch
            { }
        }

        private void DisplayCategory(string category)
        {
            TableRow category_row = new TableRow();
            TableCell category_cell = new TableCell();

            category_row.CssClass = CssCategory;

            category_cell.Text = category;
            category_cell.CssClass = CssCategory;

            category_row.Cells.Add(category_cell);

            PropertyTable.Rows.Add(category_row);
        }

        private void DisplayProperty(ObjectPropertyDescriptor property)
        {
            TableRow property_row = new TableRow();
            TableCell temp_cell = new TableCell();

            property_row.ToolTip = property.Tooltip;

            property_row.Cells.Add(temp_cell);

            temp_cell = new TableCell();
            temp_cell.Text = property.Name;
            temp_cell.CssClass = CssProperty;

            property_row.Cells.Add(temp_cell);

            temp_cell = new TableCell();
            temp_cell.CssClass = CssProperty;
            if (property.Value != null)
            {
                
                if (!property.ReadOnly)
                {
                    if (property.ValueType.Equals(typeof(DateTime)))
                    {

                    }
                    else if (property.TypeConverter.GetPropertiesSupported(ServiceContainer))
                    {
                        DropDownList list = new DropDownList();
                        
                        var values = property.TypeConverter.GetStandardValues(ServiceContainer);
                    }
                }
                if (property.Value.GetType().Equals(typeof(Color)))
                {
                    Color color = (Color)property.Value;
                    temp_cell.BackColor = color;
                    if (color.R == 0 && color.G == 0 && color.B == 0)
                    {
                        if (color.IsEmpty)
                            temp_cell.ForeColor = Color.Black;
                        else
                            temp_cell.ForeColor = Color.White;
                    }
                    //temp_cell.ForeColor = Color.FromArgb((int)(0xFFFFFF00 ^ color.ToArgb()));
                    temp_cell.Text = string.Format("RGB: {{{0}; {1}; {2}}}", color.R, color.G, color.B);
                }
                else
                    temp_cell.Text = property.Value.ToString();
            }

            property_row.Cells.Add(temp_cell);

            PropertyTable.Rows.Add(property_row);
        }
    }
}
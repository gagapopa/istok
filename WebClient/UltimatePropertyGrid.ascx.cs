using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Collections;

namespace WebClient
{
    public partial class UltimatePropertyGrid : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public ITypeDescriptorContext TypeDescriptorContext { get; set; }

        public Object SelectedObject { get; set; }


        Dictionary<String, DescriptorContext> propertiesDictionary = new Dictionary<string, DescriptorContext>();

        public void Show()
        {
            if (SelectedObject == null)
                return;

            if (TypeDescriptorContext == null)
                throw new ArgumentNullException("Для работы редактора свойств необходимо установить ITypeDescriptorContext");

            Type objectType = SelectedObject.GetType();

            var attribytes = objectType.GetCustomAttributes(true);

            var converterAttribute = GetAttribute<TypeConverterAttribute>(attribytes);

            Type converterType = Type.GetType(converterAttribute.ConverterTypeName);

            TypeConverter converter = Activator.CreateInstance(converterType) as TypeConverter;

            if (converter == null)
                converter = new TypeConverter();

            System.Web.UI.WebControls.Table table = new System.Web.UI.WebControls.Table();
            this.Controls.Add(table);

            var properties = converter.GetProperties(TypeDescriptorContext, SelectedObject);

            List<DescriptorContext> propertiesList;
            Dictionary<String, List<DescriptorContext>> categoryDictionary = new Dictionary<String, List<DescriptorContext>>();

            // group by category
            foreach (PropertyDescriptor item in properties)
            {
                DescriptorContext propertyContext;

                if (item.IsBrowsable)
                {
                    if (!propertiesDictionary.TryGetValue(item.Name, out propertyContext))
                    {
                        propertiesDictionary[item.Name] = propertyContext = new DescriptorContext(TypeDescriptorContext, SelectedObject, item);
                    }
                    if (!categoryDictionary.TryGetValue(item.Category,out propertiesList))
                    {
                        categoryDictionary[item.Category] = propertiesList = new List<DescriptorContext>();
                    }
                    propertiesList.Add(propertyContext);
                }
            }

            foreach (String categoryName in categoryDictionary.Keys)
            {
                DisplayCategory(table, categoryName);

                propertiesList = categoryDictionary[categoryName];

                foreach (var propertyContext in propertiesList)
                {
                    DisplayProperty(table, propertyContext);
                }
            }            
        }

        private void DisplayProperty(System.Web.UI.WebControls.Table table, DescriptorContext propertyContext)
        {
            TableRow tableRow = new TableRow();
            TableCell tableCell;

            tableRow.ToolTip = propertyContext.PropertyDescriptor.Description;

            // колонка для плюсиков
            tableRow.Cells.Add(new TableCell());

            // колонка с именем свойства
            tableCell = new TableCell();

            tableCell.Text = propertyContext.PropertyDescriptor.DisplayName;
            tableRow.Cells.Add(tableCell);

            // колонка со значением свойства
            tableCell = new TableCell();

            //var propertyConverterAttribute = GetAttribute<TypeConverterAttribute>(propertyContext.PropertyDescriptor.Attributes);

            //var propertyConverterType = Type.GetType(propertyConverterAttribute.ConverterTypeName);

            //var propertyConverter = Activator.CreateInstance(propertyConverterType) as TypeConverter;

            var propertyConverter = propertyContext.PropertyDescriptor.Converter;

            //propertyContext.PropertyConverter = propertyConverter;

            Object value = propertyContext.PropertyDescriptor.GetValue(propertyContext.Instance);

            String text = propertyConverter.ConvertToString(propertyContext, value);

            // draw icon
            // ...

            if (propertyContext.PropertyDescriptor.IsReadOnly)
            {
                // place not editable text
                tableCell.Text = text;
            }
            else
            {
                // place DropDownList
                if (propertyConverter.GetStandardValuesSupported(propertyContext))
                {
                    var collection = propertyConverter.GetStandardValues(propertyContext);
                    DropDownList dropDownList = new DropDownList();

                    foreach (var propertyValue in collection)
                    {
                        //Object value = propertyValue;
                        String propertyString = propertyConverter.ConvertToString(propertyValue);

                        dropDownList.Items.Add(propertyString);
                    }
                    dropDownList.Text = text;
                    dropDownList.ID = String.Format("{0}{1}", ID, propertyContext.PropertyDescriptor.Name);
                    dropDownList.SelectedIndexChanged += new EventHandler(dropDownList_SelectedIndexChanged);
                    tableCell.Controls.Add(dropDownList);
                }
                //// place DateTime picker
                //else if (propertyContext.PropertyDescriptor.PropertyType.Equals(typeof(DateTime)))
                //{

                //}
                //// place color picker
                //else if (propertyContext.PropertyDescriptor.PropertyType.Equals(typeof(Color)))
                //{

                //}
                // place textbox
                else
                {
                    TextBox textBox = new TextBox();
                    textBox.ID = String.Format("{0}{1}", ID, propertyContext.PropertyDescriptor.Name);
                    textBox.Text = text;
                    textBox.TextChanged += new EventHandler(textBox_TextChanged);
                    tableCell.Controls.Add(textBox);
                }
            }
            tableRow.Cells.Add(tableCell);

            table.Rows.Add(tableRow);
        }

        private static void DisplayCategory(System.Web.UI.WebControls.Table table, String categoryName)
        {
            TableRow category_row = new TableRow();
            TableCell category_cell = new TableCell();

            //category_row.CssClass = CssCategory;

            category_cell.Text = categoryName;
            //category_cell.CssClass = CssCategory;

            category_row.Cells.Add(category_cell);

            table.Rows.Add(category_row);
        }

        private String GetPropertyName(WebControl control)
        {
            String propertyName = null;
            if (control != null && control.ID.IndexOf(ID) == 0)
            {
                propertyName = control.ID.Substring(ID.Length);
            }
            return propertyName;
        }

        private DescriptorContext SetPropertyValue(String propertyName, String text)
        {
            DescriptorContext propertyContext;
            if (propertiesDictionary.TryGetValue(propertyName, out propertyContext))
            {
                Object value;
                TypeConverter propertyConverter = propertyContext.PropertyDescriptor.Converter;

                if (propertyConverter != null)
                {
                    value = propertyConverter.ConvertFromString(propertyContext, text);

                    propertyConverter.IsValid(propertyContext, value);
                }
                else value = text;

                propertyContext.PropertyDescriptor.SetValue(propertyContext.Instance, value);
            }
            return propertyContext;
        }

        void textBox_TextChanged(object sender, EventArgs e)
        {
            //String text;
            String propertyName;

            TextBox textBox = sender as TextBox;

            propertyName = GetPropertyName(textBox);

            SetPropertyValue(propertyName, textBox.Text);
        }

        void dropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            String propertyName;
            DropDownList dropDownList = sender as DropDownList;

            propertyName = GetPropertyName(dropDownList);

            SetPropertyValue(propertyName, dropDownList.SelectedValue);
        }

        private T GetAttribute<T>(IEnumerable attributes)
            where T : Attribute
        {
            return GetAttribute<T>(attributes.Cast<Object>());
        }

        private T GetAttribute<T>(IEnumerable<Object> attribytes)
            where T : Attribute
        {
            return attribytes.FirstOrDefault(x => (x as T) != null) as T;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.Client
{
    partial class SetPropertyForm : Form
    {
        StructureProvider strucProvider;
        UnitNode RootNode { get; set; }
        int[] Filter { get; set; }

        HashSet<String> propertyExclude = new HashSet<string>(new String[]
        {
#if !DEBUG
            "Typ",  
#endif
        });

        UnitNode[] allNodes;

        List<PropertyItem> anyPropertyList;

        List<PropertyItem> everyPropertyList;

        ISTOKServiceContainer serviceContainer;

        Dictionary<String, ITypeDescriptorContext> contextCash;

        Dictionary<UnitNode, HashSet<String>> nodePropertiesDictionary;

        //internal event EventHandler<UnitNodeSaveEventArgs> OnSave = null;

        //public event Action<UnitNode> OnSave = null;

        public SetPropertyForm(StructureProvider strucProvider, UnitNode unitNode, int[] filter)
        {
            InitializeComponent();

            this.strucProvider = strucProvider;
            this.RootNode = unitNode;
            this.Filter = filter;

            serviceContainer = strucProvider.Session.GetServiceContainer() as ISTOKServiceContainer;
            
            contextCash = new Dictionary<String, ITypeDescriptorContext>();

            CollectAllNodes();
            CollectProperties();
            propertyMethodRadioButton_CheckedChanged(null, EventArgs.Empty);
        }

        private void CollectAllNodes()
        {
            allNodes = strucProvider.GetAllUnitNodes(RootNode.Idnum, Filter);
        }

        private void CollectProperties()
        {
            //List<PropertyDescriptor> anyPropertyList = new List<PropertyDescriptor>();
            //List<PropertyDescriptor> everyPropertyList = new List<PropertyDescriptor>();

            Dictionary<String, PropertyDescriptor> byNameDictionary = new Dictionary<string, PropertyDescriptor>();
            Dictionary<String, int> countDictionary = new Dictionary<string, int>();

            nodePropertiesDictionary = new Dictionary<UnitNode, HashSet<String>>();
            HashSet<String> propertiesByUnitNode;

            foreach (UnitNode node in allNodes)
            {
                TypeConverter converter = GetUnitNodeConverter(node);
                var properties = converter.GetProperties(node);

                nodePropertiesDictionary[node] = propertiesByUnitNode = new HashSet<String>();

                foreach (PropertyDescriptor propertyDescriptor in properties)
                {
                    if (propertyDescriptor.IsBrowsable && !propertyDescriptor.IsReadOnly && !propertyExclude.Contains(propertyDescriptor.Name))
                    {
                        if (!byNameDictionary.ContainsKey(propertyDescriptor.Name))
                            byNameDictionary[propertyDescriptor.Name] = propertyDescriptor;

                        if (!countDictionary.ContainsKey(propertyDescriptor.Name))
                            countDictionary[propertyDescriptor.Name] = 1;
                        else
                            ++countDictionary[propertyDescriptor.Name];

                        propertiesByUnitNode.Add(propertyDescriptor.Name);
                    }
                }
            }

            //anyPropertyList = new List<PropertyItem>(byNameDictionary.Values);
            anyPropertyList = (from prop in byNameDictionary.Values select new PropertyItem(prop)).ToList();
            everyPropertyList = new List<PropertyItem>();

            foreach (var item in countDictionary.Keys)
            {
                if (countDictionary[item] == allNodes.Length)
                    everyPropertyList.Add(new PropertyItem(byNameDictionary[item]));
            }

            // сортировка свойств по алфавиту
            anyPropertyList.Sort((a, b) => String.Compare(a.Property.DisplayName, b.Property.DisplayName));
            everyPropertyList.Sort((a, b) => String.Compare(a.Property.DisplayName, b.Property.DisplayName));
        }

        private TypeConverter GetUnitNodeConverter(UnitNode node)
        {
            Type nodeType = node.GetType();

            Object[] attrs = nodeType.GetCustomAttributes(typeof(TypeConverterAttribute), true);
            TypeConverterAttribute converterAttribute = null;

            if (attrs != null && attrs.Length > 0)
                converterAttribute = attrs[0] as TypeConverterAttribute;

            if (converterAttribute != null)
            {
                Type converterType = Type.GetType(converterAttribute.ConverterTypeName);
                if (converterType != null)
                {
                    return Activator.CreateInstance(converterType) as TypeConverter;
                }
            }
            return null;
        }

        private void propertyMethodRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (everyNodeRadioButton.Checked)
                propertyNameComboBox.DataSource = everyPropertyList;
            else
                propertyNameComboBox.DataSource = anyPropertyList;
        }

        class StandardItem
        {
            TypeConverter converter;

            ITypeDescriptorContext typeContext;

            public Object Value { get; set; }

            public StandardItem(TypeConverter converter, ITypeDescriptorContext typeContext, Object value)
            {
                this.converter = converter;
                this.typeContext = typeContext;
                this.Value = value;
            }

            public override string ToString()
            {
                String ret;
                if (converter != null)
                    ret = converter.ConvertToString(typeContext, Value);
                else if (Value != null)
                    ret= Value.ToString();
                else
                    ret= "<ERROR>";

                if (String.IsNullOrEmpty(ret))
                    ret = "";
                return ret;
            }
        }

        class PropertyItem
        {
            public PropertyDescriptor Property { get; private set; }

            public PropertyItem(PropertyDescriptor property)
            {
                this.Property = property;
            }
            public override string ToString()
            {
                return Property.DisplayName;
            }
        }

        Object currentValue;

        //ITypeDescriptorContext typeContext;

        private void propertyNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PropertyItem propertyItem = propertyNameComboBox.SelectedItem as PropertyItem;
            //PropertyDescriptor property = propertyNameComboBox.SelectedItem as PropertyDescriptor;

            if (propertyItem != null)
            {
                ITypeDescriptorContext typeContext;
                PropertyDescriptor property = propertyItem.Property;
                TypeConverter propertyConverter = property.Converter;

                if (!contextCash.TryGetValue(propertyItem.Property.Name, out typeContext))
                {
                    contextCash[propertyItem.Property.Name] = typeContext = 
                        new MyTypeDescriptorContext(serviceContainer, propertyItem.Property);
                }

                propertyValueGroupBox.Controls.Clear();
                if (propertyConverter.GetStandardValuesSupported(typeContext))
                {
                    ComboBox comboBoxValue = new ComboBox();
                    comboBoxValue.Dock = DockStyle.Fill;
                    comboBoxValue.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboBoxValue.SelectedIndexChanged += new EventHandler(comboBoxValue_SelectedIndexChanged);
                    //comboBoxValue.DataSource = propertyConverter.GetStandardValues();
                    var standardValues = propertyConverter.GetStandardValues(typeContext);
                    //foreach (var value in standardValues)
                    //{
                    //    comboBoxValue.Items.Add(new StandardItem(propertyConverter, value));
                    //}
                    //comboBoxValue.DataSource = (from value in standardValues select new StandardItem(propertyConverter, value)).ToList();
                    List<StandardItem> itemList = new List<StandardItem>();
                    foreach (var value in standardValues)
                    {
                        itemList.Add(new StandardItem(propertyConverter, typeContext, value));
                    }
                    comboBoxValue.DataSource = itemList;

                    propertyValueGroupBox.Controls.Add(comboBoxValue);

                    //radioButton4.Enabled = false;
                    //radioButton5.Enabled = false;
                    groupBox2.Enabled = false;
                }
                else if (property.PropertyType.Equals(typeof(DateTime)))
                {
                    DateTimePicker picker = new DateTimePicker();
                    picker.ValueChanged += new EventHandler(picker_ValueChanged);
                    picker.Dock = DockStyle.Fill;
                    propertyValueGroupBox.Controls.Add(picker);

                    //radioButton4.Enabled = false;
                    //radioButton5.Enabled = false;
                    groupBox2.Enabled = false;
                }
                else
                {
                    TextBox textBox = new TextBox();
                    textBox.TextChanged += new EventHandler(textBox_TextChanged);
                    textBox.Multiline = true;
                    textBox.Dock = DockStyle.Fill;
                    propertyValueGroupBox.Controls.Add(textBox);

                    groupBox2.Enabled = property.PropertyType.Equals(typeof(String));
                }
            }
        }

        void textBox_TextChanged(object sender, EventArgs e)
        {
            currentValue = (sender as TextBox).Text;
        }

        void picker_ValueChanged(object sender, EventArgs e)
        {
            currentValue = (sender as DateTimePicker).Value;
        }

        void comboBoxValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentValue = ((sender as ComboBox).SelectedItem as StandardItem).Value;
        }

        private async void okButton_Click(object sender, EventArgs e)
        {
            PropertyItem propertyItem = propertyNameComboBox.SelectedItem as PropertyItem;
            //PropertyDescriptor property = propertyNameComboBox.SelectedItem as PropertyDescriptor;

            if (propertyItem != null)
            {
                //bool promt = true;
                //bool change = false;

                ITypeDescriptorContext typeContext;
                PropertyDescriptor property = propertyItem.Property;

                if (!contextCash.TryGetValue(propertyItem.Property.Name, out typeContext))
                {
                    contextCash[propertyItem.Property.Name] = typeContext =
                        new MyTypeDescriptorContext(serviceContainer, propertyItem.Property);
                }

                try
                {
                    DisableControl();
                    await SaveChanges(typeContext, property);
                    EnableControl();
                    this.Close();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
            //this.Close();
        }

        private void EnableControl()
        {
            ableControl(true);
        }

        private void DisableControl()
        {
            ableControl(false);
        }

        private void ableControl(bool enable)
        {
            groupBox1.Enabled = enable;
            groupBox2.Enabled = enable;
            propertyValueGroupBox.Enabled = enable;
            okButton.Enabled = enable;
            cancelButton.Enabled = enable;
        }

        private async Task SaveChanges(ITypeDescriptorContext typeContext, PropertyDescriptor property)
        {
            Object value = currentValue;
            List<UnitProvider> saveList = new List<UnitProvider>();

            if (value != null && !property.PropertyType.Equals(value.GetType()))
            {
                value = property.Converter.ConvertFromInvariantString(typeContext, value.ToString());
            }

            foreach (var item in allNodes)
            {
                try
                {
                    if (nodePropertiesDictionary[item].Contains(property.Name))
                    {
                        Object unitNodeValue;
                        if (property.PropertyType.Equals(typeof(String)) && !radioButton3.Checked)
                        {
                            unitNodeValue = property.GetValue(item);

                            unitNodeValue = String.Format(radioButton4.Checked ? "{0}{1}" : "{1}{0}", value, unitNodeValue);
                        }
                        else
                            unitNodeValue = value;

                        var provider = strucProvider.GetUnitProvider(item);
                        property.SetValue(item, unitNodeValue);

                        //UnitNodeSaveEventArgs args = new UnitNodeSaveEventArgs(item, promt, change);
                        //args.SuspendUpdate = true;

                        //OnSave(item);
                        provider.NewUnitNode = item;
                        saveList.Add(provider);

                        //promt = args.ChangeCodePromt;
                        //change = args.ChangeCode;
                    }
                }
                catch (InvalidOperationException) { }
                catch { }
            }

            foreach (var item in saveList)
            {
                await Program.MainForm.WorkflowSelector.Do(new LockActionArgs(() => item.Save(), true));
                //OnSave(item);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

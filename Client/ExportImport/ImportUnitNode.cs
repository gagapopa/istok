using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Client
{
    [TypeConverter(typeof(ImportUnitNodeTypeConverter))]
    class ImportUnitNode : UnitNode
    {
        [DisplayName("Код параметра"),
        Category("Импорт")]
        public String ImportCode { get; private set; }

        [Browsable(false)]
        public UnitNode Node { get; private set; }

        [DisplayName("Импортируемые значения"),
        Category("Импорт")]
        [TypeConverter(typeof(PackageValuesTypeConverter))]
        [Editor(typeof(PackageValuesUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Package[] ImportValues { get; set; }

        public override string GetNodeText()
        {
            if (Node != null)
                return Node.GetNodeText();

            return base.GetNodeText();
        }

        public ImportUnitNode(UnitNode node, Package[] values)
        {
            this.Node = node;
            this.ImportValues = values;

            if (this.Node != null)
            {
                //Text = this.Node.Text;
                //DocIndex = this.Node.DocIndex;

                Typ = this.Node.Typ;
            }
            else
                Typ = (int)UnitTypeId.ManualParameter;
        }

        public ImportUnitNode(Package[] values, String code)
            : this(null, values)
        {
            this.Text = String.Format("Параметр ${0}$", code);
            this.ImportCode = code;
        }
    }

    class PackageValuesTypeConverter:TypeConverter
    {
        public override bool CanConvertTo(
          ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }

        public override object ConvertTo(
          ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
          object value, Type destType)
        {
            Package[] packs = value as Package[];
            if (packs != null)
            {
                int count = 0;
                foreach (var item in packs)
                {
                    count += item.Count;
                }

                return String.Format("( Количество значений: {0} )", count);
            }

            return base.ConvertTo(context, culture, value, destType);
        }
    }

    class PackageValuesUIEditor : System.Drawing.Design.UITypeEditor
    {
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
            //return  base.GetEditStyle(context);
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                System.Windows.Forms.Design.IWindowsFormsEditorService svc =
                  (System.Windows.Forms.Design.IWindowsFormsEditorService)
                  provider.GetService(typeof(System.Windows.Forms.Design.IWindowsFormsEditorService));

                if (svc != null)
                {
                    Package[] pack = value as Package[];

                    DataTable table = PackageToTAble(pack);

                    DataGridView grid = new DataGridView();
                    grid.BackgroundColor = SystemColors.Window;
                    grid.RowHeadersVisible = false;
                    grid.AllowUserToAddRows = false;
                    grid.AllowUserToDeleteRows = false;
                    grid.ReadOnly = true;
                    grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    grid.DataSource = table;

                    //ForeignLangsControl flctrl =
                    //  new ForeignLangsControl((ForeignLangs)value);
                    grid.Tag = svc;

                    svc.DropDownControl(grid);

                    //value = flctrl.Foreignlangs;
                }
            }
            return base.EditValue(context, provider, value);
        }

        private DataTable PackageToTAble(Package[] pack)
        {
            CorrectedParamValueItem correctedValue;
            List<ParamValueItem> valueItem = new List<ParamValueItem>();
            DataTable retTable = new DataTable();

            bool hasOriginalColumn = false;
            DataColumn timeColumn= retTable.Columns.Add("time", typeof(DateTime));
            DataColumn originalValueColumn = retTable.Columns.Add("original value", typeof(double));
            DataColumn valueColumn = retTable.Columns.Add("value", typeof(double));
            DataColumn changeTimeColumn = retTable.Columns.Add("change time", typeof(DateTime));

            foreach (var item in pack)
            {
                valueItem.AddRange(item.Values);
            }
            foreach (var item in valueItem)
            {
                DataRow row = retTable.NewRow();

                row[timeColumn] = item.Time;
                row[valueColumn] = item.Value;
                row[changeTimeColumn] = item.ChangeTime;

                if ((correctedValue=item as CorrectedParamValueItem)!=null)
                {
                    hasOriginalColumn = true;
                    row[originalValueColumn] = correctedValue.OriginalValueItem.Value;
                }
                retTable.Rows.Add(row);
            }
            if (!hasOriginalColumn)
                retTable.Columns.Remove(originalValueColumn);
            return retTable;
        }
    }
}

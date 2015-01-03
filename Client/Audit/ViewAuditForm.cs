using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ASC.Audit;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    partial class ViewAuditForm : Form
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        public Session Session { get; set; }

        public List<AuditEntry> Entries { get; set; }

        public UnitNode UnitNode { get; set; }

        AuditCategoryTypeConverter auditCategoryConverter = new AuditCategoryTypeConverter();

        public ViewAuditForm(Session session)
        {
            Session = session;
            InitializeComponent();

            foreach (AuditCategory item in auditCategoryConverter.GetStandardValues())
            {
                if (item != AuditCategory.AllCategories)
                {
                    checkedListBox1.Items.Add(auditCategoryConverter.ConvertToString(item), false);
        }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (UnitNode!=null)
            {
                Text = String.Format("История изменений '{0}'", UnitNode.FullName);
            }
            else
            {
                Text = "Просмотр аудита";
            }

            if (Entries == null)
            {
                retrieveLast100Button_Click(this, EventArgs.Empty);
            }
            else
            {
                ShowEntries(); 
            }
        }

        private void ShowEntries()
        {
            auditEntryListView.Items.Clear();

            foreach (var entry in Entries)
            {
                var entryItem = auditEntryListView.Items.Add(entry.Time.ToString());
                //entryItem.SubItems.Add(entry.Time.ToString());
                entryItem.SubItems.Add(entry.UserLogin);

                entryItem.Tag = entry;
            }
        }

        private void ShowEntry(List<AuditEntry> entries)
        {
            groupDictionary.Clear();
            auditItemListView.Items.Clear();
            auditItemListView.Groups.Clear();
            columnDictionary.Clear();

            auditItemListView.BeginUpdate();

            try
            {
                const String nothing = "--";
                AuditEntry headEntry = null;
                
                if (entries.Count == 1)
                {
                    headEntry = entries[0];
                    //groupBox1.Text = headEntry.ToString();                    
                }
                else
                {
                    HashSet<String> set = new HashSet<string>(from e in entries select e.UserLogin);

                    if (set.Count==1)
                    {
                        headEntry = entries[0];
                    }


                    //groupBox1.Text = String.Format("{0} записей", entries.Count);

                    //auditUserLoginLabel.Text =  nothing;
                    //auditUserFullNameLabel.Text = nothing;
                    //auditUserPositionLabel.Text = nothing;
                    //auditUserRoleLabel.Text = nothing;
                }
                const String headerFormat = "{0,-15} {1}";
                auditUserLoginLabel.Text = String.Format(headerFormat, "Логин:", headEntry == null ? nothing : headEntry.UserLogin);
                auditUserFullNameLabel.Text = String.Format(headerFormat, "Полное имя:", headEntry == null ? nothing : headEntry.UserFullName);
                auditUserPositionLabel.Text = String.Format(headerFormat, "Должность:", headEntry == null ? nothing : headEntry.UserPosition);
                auditUserRoleLabel.Text = String.Format(headerFormat, "Разрешения:", headEntry == null ? nothing : headEntry.UserRole);

                auditItemListView.Columns.Clear();
                auditItemListView.Columns.Add("Действие");
                if (entries.Count>0)
                {
                    auditItemListView.Columns.Add("Пользователь");
                    auditItemListView.Columns.Add("Время");
                }

                foreach (var entry in entries)
                {
                    RefreshColumns(
                                entry.AuditTypes,
                                entry.AuditGroups,
                                entry.AuditUsers,
                                entry.AuditUnits,
                                entry.AuditProps,
                                entry.AuditLobs,
                                entry.AuditCalcStarts,
                                entry.AuditValues);

                    var subEntry = entries.Count > 0 ? entry : null;

                    if (entry.AuditTypes.Count > 0)
                    {
                        AddItems(subEntry, entry.AuditTypes, AuditCategory.TypesChange);// "AuditTypes", "Аудит типов");
                    }
                    if (entry.AuditGroups.Count > 0)
                    {
                        AddItems(subEntry, entry.AuditGroups, AuditCategory.GroupsChange);// "AuditGroups", "Аудит групп");
                    }
                    if (entry.AuditUsers.Count > 0)
                    {
                        AddItems(subEntry, entry.AuditUsers, AuditCategory.UsersChange);// "AuditUsers", "Аудит пользователей");
                    }
                    if (entry.AuditUnits.Count > 0)
                    {
                        AddItems(subEntry, entry.AuditUnits, AuditCategory.UnitsChange);// "AuditUnits", "Аудит структуры");
                    }
                    if (entry.AuditProps.Count > 0)
                    {
                        AddItems(subEntry, entry.AuditProps, AuditCategory.UnitsChange);// "AuditUnits", "Аудит структуры");
                    }
                    if (entry.AuditLobs.Count > 0)
                    {
                        AddItems(subEntry, entry.AuditLobs, AuditCategory.UnitsChange);// "AuditUnits", "Аудит структуры");
                    }
                    if (entry.AuditCalcStarts.Count > 0)
                    {
                        AddItems(subEntry, entry.AuditCalcStarts, AuditCategory.CalcStarts);// "AuditCalcStarts", "Аудит расчёта");
                    }
                    if (entry.AuditValues.Count > 0)
                    {
                        AddItems(subEntry, entry.AuditValues, AuditCategory.ValuesChange);// "AuditValues", "Аудит значений");
                    } 
                }

                foreach (ColumnHeader column in auditItemListView.Columns)
                {
                    column.Width = -1;
                }
            }
            finally
            {
                auditItemListView.EndUpdate();
            }
        }

        private void RefreshColumns(params IEnumerable[] collections)
        {
            foreach (var collection in collections)
            {
                foreach (var item in collection)
                {
                    var auditItem = item as AuditItem;
                    if (auditItem != null)
                    {
                        foreach (var property in auditItem.AuditProperties)
                        {
                            if (!columnDictionary.Contains(property))
                            {
                                columnDictionary.Add(property);
                                var column = auditItemListView.Columns.Add(property, auditItem.GetHead(property));
                                //column.Width = -2;
                            }
                        }
                    }
                }
            }
        }

        Dictionary<AuditCategory, ListViewGroup> groupDictionary = new Dictionary<AuditCategory, ListViewGroup>();
        HashSet<String> columnDictionary = new HashSet<String>();

        private void AddItems(AuditEntry entry, IEnumerable list, AuditCategory auditCategory)// string groupName, string groupHeader)
        {
            ListViewGroup listGroup;

            if (!groupDictionary.TryGetValue(auditCategory, out listGroup))
            {
                listGroup = auditItemListView.Groups.Add(auditCategory.ToString(), auditCategoryConverter.ConvertToString(auditCategory)); //groupName, groupHeader);
                groupDictionary[auditCategory] = listGroup;
            }

            foreach (var item in list)
            {
                var listItem = auditItemListView.Items.Add(item.ToString());

                if (entry!=null)
                {
                    listItem.SubItems.Add(entry.UserLogin);
                    listItem.SubItems.Add(entry.Time.ToString()); 
                }

                var auditItem = item as AuditItem;

                if (auditItem!=null)
                {
                    foreach (ColumnHeader column in auditItemListView.Columns)
                    {
                        if (!String.IsNullOrEmpty(column.Name))
                        {
                            var value = auditItem.GetChange(column.Name);
                            listItem.SubItems.Add(value);
                        }
                    }
                }

                listItem.Group = listGroup;
            }
        }

        private void auditEntryListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //var entry = e.Item.Tag as AuditEntry;
            //ShowEntry(entry);

            var list = new List<AuditEntry>();

            foreach (ListViewItem item in auditEntryListView.Items)
            {
                AuditEntry entry;
                if (item.Selected && (entry = item.Tag as AuditEntry) != null)
                {
                    list.Add(entry);
                }
            }
            ShowEntry(list);
        }

        private async void retrieveByTimeButton_Click(object sender, EventArgs e)
        {
            try
            {
                AuditRequestContainer request = GetRequest();

                request.StartTime = dateTimePickerFrom.Value;
                request.EndTime = dateTimePickerTo.Value;

                lastRequiredIndex = 0;
                retrieveNext100Button.Enabled = false;

                Entries = new List<AuditEntry>(await Task.Factory.StartNew(() => Session.GetAudit(request)));
                ShowEntries();
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка при просмотре аудита.", exc);
                Program.MainForm.ShowError(exc);
            }
        }

        const int aHundred = 100;
        int lastRequiredIndex;

        private async void retrieveLast100Button_Click(object sender, EventArgs e)
        {
            lastRequiredIndex = 0;

            await RetrieveAudit();
        }

        private async void retrieveNext100Button_Click(object sender, EventArgs e)
        {
            await RetrieveAudit();
        }

        private async System.Threading.Tasks.Task RetrieveAudit()
        {
            try
            {
                AuditRequestContainer request = GetRequest();

                request.StartIndex = lastRequiredIndex;
                request.EndIndex = (lastRequiredIndex += aHundred);

                retrieveNext100Button.Enabled = true;

                Entries = new List<AuditEntry>(await Task.Factory.StartNew(() => Session.GetAudit(request)));
                ShowEntries();
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка при просмотре аудита.", exc);
                Program.MainForm.ShowError(exc);
            }
        }

        private AuditRequestContainer GetRequest()
        {
            return new AuditRequestContainer()
            {
                FilterText = textBox1.Text,
                NodeFilter = UnitNode,
                CategoryFilter = GetCategoryFilter()
            };
        }

        private AuditCategory GetCategoryFilter()
        {
            AuditCategory category = AuditCategory.AllCategories;

            foreach (var item in checkedListBox1.CheckedItems)
            {
                category |= (AuditCategory)auditCategoryConverter.ConvertFromString(item.ToString());
            }
            return category;
        }
    }
}

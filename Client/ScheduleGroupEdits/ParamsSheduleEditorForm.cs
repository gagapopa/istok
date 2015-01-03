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

namespace COTES.ISTOK.Client
{
    partial class ParamsSheduleEditorForm : BaseScheduleForm
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        
        public ParamsSheduleEditorForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
        }

        private void EditAccepted(bool accept)
        {
            toolStripButtonDelete.Enabled =
                toolStripButtonEdit.Enabled = accept;
            removeScheduleToolStripMenuItem.Enabled =
                editScheduleToolStripMenuItem.Enabled = accept;
        }

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            AddSchedule();
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            RemoveSchedule();
        }

        private void toolStripButtonEdit_Click(object sender, EventArgs e)
        {
            EditSchedule();
        }

        private void DeleteScheduleFromListView(ListViewItem item)
        {
            listViewSchedules.BeginUpdate();
            listViewSchedules.Items.Remove(item);
            listViewSchedules.EndUpdate();
        }

        private void UpdateScheduleOnListView(ListViewItem item, Schedule schedule)
        {
            listViewSchedules.BeginUpdate();
            item.Name = item.Text = schedule.Name;
            listViewSchedules.EndUpdate();
        }

        private void listViewSchedules_ItemSelectionChanged(object sender, EventArgs e)
        {
            EditAccepted(listViewSchedules.SelectedItems.Count != 0);
        }

        private void ParamsSheduleEditor_Load(object sender, EventArgs e)
        {
            EditAccepted(false);

            LoadSchedule();
        }

        private async void AddSchedule()
        {
            try
            {
                AddEditScheduleForm add_form = new AddEditScheduleForm();
                add_form.ShowDialog();

                if (add_form.Accept)
                {
                    //var watcher =
                    strucProvider.Session.AddSchedule(add_form.Result);

                    Action<Schedule> dlg =
                        new Action<Schedule>(AddScheduleToListView);

                    //watcher.AddSuccessFinishHandler(() =>
                    //    {
                    //        Schedule schedule = current_data_service.GetParamsSchedule(add_form.Result.Name);
                    //        if (schedule != null)
                    //            this.Invoke(dlg, schedule);
                    //    });

                    //RunWatcher(watcher);
                    Schedule schedule = await Task.Factory.StartNew(() => strucProvider.Session.GetSchedule(add_form.Result.Name));
                    if (schedule != null)
                        this.Invoke(dlg, schedule);
                }
            }
            catch (Exception exp)
            {
                log.WarnException("Ошибка при редактировании элемента расписания.", exp);
                ShowError(exp);
            }
        }

        private async void EditSchedule()
        {
            try
            {
                ListViewItem selected = listViewSchedules.SelectedItems[0];
                Schedule schedule = await Task.Factory.StartNew(() => strucProvider.Session.GetSchedule((int)selected.Tag));

                if (schedule == null) throw new ISTOKException("Расписание не найдено.");
                AddEditScheduleForm frm = new AddEditScheduleForm(schedule);
                frm.ShowDialog(this);

                if (frm.Accept)
                {
                    Action<ListViewItem, Schedule> dlg =
                                new Action<ListViewItem, Schedule>(this.UpdateScheduleOnListView);

                    //hack, may contain error
                    strucProvider.Session.UpdateSchedule(schedule);
                    this.Invoke(dlg, selected, schedule);
                    //w.AddSuccessFinishHandler(() => this.Invoke(dlg, selected, schedule));
                    //this.RunWatcher(w);
                }

                //var watcher =
                //    current_data_service.QueryParamsSchedule((int)selected.Tag);

                //watcher.AddValueRecivedHandler((object x) =>
                //{
                //    try
                //    {
                //        Schedule schedule = x as Schedule;

                //        AddEditScheduleForm frm = new AddEditScheduleForm(schedule);
                //        frm.ShowDialog();

                //        if (frm.Accept)
                //        {
                //            Action<ListViewItem, Schedule> dlg =
                //                new Action<ListViewItem, Schedule>(this.UpdateScheduleOnListView);

                //            //hack, may contain error
                //            var w = this.current_data_service.QueryUpdateParamsSchedule(schedule);
                //            w.AddSuccessFinishHandler(() => this.Invoke(dlg, selected, schedule));
                //            this.RunWatcher(w);
                //        }
                //    }
                //    catch (Exception exp)
                //    {
                //        this.ShowError(exp);
                //    }
                //});

                //RunWatcher(watcher);
            }
            catch (Exception exp)
            {
                log.WarnException("Ошибка при редактировании элемента расписания.", exp);
                ShowError(exp);
            }
        }

        private void RemoveSchedule()
        {
            try
            {
                DialogResult delete = MessageBox.Show("Вы действительно желаете удалить выбранное расписание?",
                                                      "Удалить?",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                if (delete == DialogResult.Yes)
                {
                    //var watcher =
                    strucProvider.Session.DeleteSchedule((int)listViewSchedules.SelectedItems[0].Tag);

                    Action<ListViewItem> dlg = new Action<ListViewItem>(this.DeleteScheduleFromListView);

                    var deleted = listViewSchedules.SelectedItems[0];

                    //watcher.AddSuccessFinishHandler(() => this.Invoke(dlg, deleted));
                    //RunWatcher(watcher);
                    this.Invoke(dlg, deleted);
                }
            }
            catch (Exception exp)
            {
                log.WarnException("Ошибка при удалении элемента расписания.", exp);
                ShowError(exp);
            }
        }

        private void addScheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSchedule();
        }

        private void editScheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditSchedule();
        }

        private void removeScheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSchedule();
        }

        private void listViewSchedules_DoubleClick(object sender, EventArgs e)
        {
            EditSchedule();
        }
    }
}

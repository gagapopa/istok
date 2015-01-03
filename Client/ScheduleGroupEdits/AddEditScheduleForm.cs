using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Client
{
    public partial class AddEditScheduleForm : Form
    {
        private bool is_added = false;
        private bool accepted = false;
        private Schedule schedule = null;

        public AddEditScheduleForm()
        {
            InitializeComponent();
            InitializeScheduleInfo();

            schedule = new Schedule();
            is_added = true;
            UpdateSchedule();
        }

        public AddEditScheduleForm(Schedule edited)
        {
            InitializeComponent();
            InitializeScheduleInfo();

            if (edited == null)
                throw new NotImplementedException();

            schedule = edited;
            UpdateSchedule();
        }

        private void UpdateSchedule()
        {
            textBoxName.Text = schedule.Name;
            if (schedule.Rule != null)
            {
                dayOfWeekScheduleComboBox.SelectedValue = schedule.Rule.DayOfWeek;
                periodScheduleComboBox.SelectedValue = schedule.Rule.Period;
                dayScheduleNumericUpDown.Value = schedule.Rule.Day;
                scheduleTimePicker.Value = schedule.Rule.Time;

                cbxRepeatEach.Text = schedule.Rule.RepeatEachString;
                cbxRepeatFor.Text = schedule.Rule.RepeatForString;
                chbxRepeat.Checked = schedule.Rule.Repeat;
            }
        }

        public bool Accept
        {
            get { return accepted; }
        }

        public Schedule Result
        {
            get { return schedule; }
        }

        private void InitializeScheduleInfo()
        {
#if DEBUG
            groupBox1.Visible = true;
            this.Height = 347;
#else
            this.Height = 240;
#endif
            DataTable table = new DataTable();
            table.Columns.Add("value", typeof(DayOfWeek));
            table.Columns.Add("name", typeof(string));
            foreach (DayOfWeek item in Enum.GetValues(typeof(DayOfWeek)))
                table.Rows.Add(item, DayOfWeekFormatter.Format(item));
            dayOfWeekScheduleComboBox.DisplayMember = "name";
            dayOfWeekScheduleComboBox.ValueMember = "value";
            dayOfWeekScheduleComboBox.DataSource = table;
            
            table = new DataTable();
            table.Columns.Add("value", typeof(BackUpPeriod));
            table.Columns.Add("name", typeof(string));
            foreach (BackUpPeriod item in Enum.GetValues(typeof(BackUpPeriod)))
                table.Rows.Add(item, BackUpPeriodFormatter.Format(item));

            periodScheduleComboBox.DisplayMember = "name";
            periodScheduleComboBox.ValueMember = "value";
            periodScheduleComboBox.DataSource = table;

            UpdateRepeating();
            cbxRepeatEach.Items.Clear();
            cbxRepeatEach.Items.Add("30 мин.");
            cbxRepeatEach.Items.Add("1 ч.");
            cbxRepeatEach.Items.Add("8 ч.");
            cbxRepeatEach.Items.Add("12 ч.");

            cbxRepeatFor.Items.Clear();
            cbxRepeatFor.Items.Add("30 мин.");
            cbxRepeatFor.Items.Add("1 ч.");
            cbxRepeatFor.Items.Add("12 ч.");
            cbxRepeatFor.Items.Add("1 д.");
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddEditScheduleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (is_added) return;

            //TimeSpan current = new TimeSpan((int)numericUpDownHours.Value, 
            //                                (int)numericUpDownMinutes.Value,
            //                                (int)numericUpDownSeconds.Value);

            if (schedule.Name != textBoxName.Text
                /*|| schedule.Period != current*/)
            {
                DialogResult result = MessageBox.Show("Данные были изменены. Отбросить изменения и закрыть?", 
                                                      "Данные были изменены", 
                                                      MessageBoxButtons.YesNo, 
                                                      MessageBoxIcon.Warning);

                e.Cancel = result == DialogResult.No;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Control ptr = null;
            try
            {
                schedule.Name = textBoxName.Text;
                schedule.Rule = new ScheduleReg((BackUpPeriod)periodScheduleComboBox.SelectedValue,
                    (DayOfWeek)dayOfWeekScheduleComboBox.SelectedValue,
                    (int)dayScheduleNumericUpDown.Value,
                    scheduleTimePicker.Value);
                schedule.Rule.Repeat = chbxRepeat.Checked;
                ptr = cbxRepeatEach;
                schedule.Rule.RepeatEachString = cbxRepeatEach.Text;
                ptr = cbxRepeatFor;
                schedule.Rule.RepeatForString = cbxRepeatFor.Text;
                accepted = true;

                Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("Ошибка форматирования значения.");
                if (ptr != null) ptr.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void periodScheduleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BackUpPeriod period = (BackUpPeriod)periodScheduleComboBox.SelectedValue;

            dayOfWeekScheduleComboBox.Enabled = period == BackUpPeriod.Week;
            dayScheduleNumericUpDown.Enabled = period == BackUpPeriod.Month;
        }

        #region Повторение задачи
        private void UpdateRepeating()
        {
            cbxRepeatEach.Enabled = cbxRepeatFor.Enabled = chbxRepeat.Checked;
        }

        private void chbxRepeat_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRepeating();
        }

        private void cbxRepeatEach_Leave(object sender, EventArgs e)
        {
            cbxRepeatEach.Text = cbxRepeatEach.Text.ToLower();
        }

        private void cbxRepeatFor_Leave(object sender, EventArgs e)
        {
            cbxRepeatFor.Text = cbxRepeatFor.Text.ToLower();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            Control ptr = null;
            try
            {
                schedule.Name = textBoxName.Text;
                schedule.Rule = new ScheduleReg((BackUpPeriod)periodScheduleComboBox.SelectedValue,
                    (DayOfWeek)dayOfWeekScheduleComboBox.SelectedValue,
                    (int)dayScheduleNumericUpDown.Value,
                    scheduleTimePicker.Value);
                schedule.Rule.Repeat = chbxRepeat.Checked;
                ptr = cbxRepeatEach;
                schedule.Rule.RepeatEachString = cbxRepeatEach.Text;
                ptr = cbxRepeatFor;
                schedule.Rule.RepeatForString = cbxRepeatFor.Text;

                bool res = schedule.Rule.CheckTime(dtp.Value, dtpLastCall.Value);
                MessageBox.Show(res.ToString());
            }
            catch (FormatException)
            {
                MessageBox.Show("Ошибка форматирования значения.");
                if (ptr != null) ptr.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}

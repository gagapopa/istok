using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ASC.TypeConverters;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно просмотра и редактирования значений
    /// </summary>
    partial class ParameterValuesEditorForm : BaseAsyncWorkForm
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private IntervalTypeConverter intervalConverter = null;

        public IValuesController ValuesController { get; protected set; }

        public ParameterValuesEditorForm(StructureProvider strucProvider, IValuesController valuesController)
            : base(strucProvider)
        {
            InitializeComponent();

            CurrentInterval = Interval.Zero;

            typeof(Control).InvokeMember("DoubleBuffered",
                  BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                  null, valuesViewDataGridView, new object[] { true });
            
            TimeColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;
            ChangeTimeColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;
            paramQualityColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;
            BeginValueColumn.DefaultCellStyle = ValuesControlSettings.Instance.DisabledValueCellStyle;

            Image im = Program.MainForm.Icons.Images[((int)valuesController.Parameter.Typ).ToString()];
            if (im != null)
                this.Icon = System.Drawing.Icon.FromHandle(((Bitmap)im).GetHicon());

            intervalConverter = new IntervalTypeConverter()
            {
                Supplier = strucProvider.GetServiceContainer().GetService(typeof(IIntervalSupplier)) as IIntervalSupplier
            };
            this.ValuesController = valuesController;

            var collection = intervalConverter.GetStandardValues();
            if (collection != null)
            {
                foreach (Interval val in intervalConverter.GetStandardValues())
                    intervalComboBox.Items.Add(intervalConverter.ConvertToString(val));
            }
        }

        /// <summary>
        /// Текущий отображаемый интервал
        /// </summary>
        public Interval CurrentInterval { get; set; }

        /// <summary>
        /// Начальная инициализация формы
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Text = String.Format("Значения параметра: {0}", ValuesController.Title);

            if (ValuesController.VariableConsts)
            {
                CurrentInterval = Interval.Zero;
                startIntervalDateTimePicker.Enabled = false;
                prevTimeButton.Enabled = false;
                nextTimeButton.Enabled = false;
                intervalComboBox.Enabled = false;
            }
            else if (CurrentInterval == Interval.Zero)
            {
                if (ValuesController.Interval != Interval.Zero)
                {
                    foreach (Interval val in intervalConverter.GetStandardValues())
                    {
                        if (val > ValuesController.Interval)
                        {
                            CurrentInterval = val;
                            break;
                        }
                    }
                }
                else foreach (Interval val in intervalConverter.GetStandardValues())
                        CurrentInterval = val;
            }

            intervalComboBox.Text = intervalConverter.ConvertToString(CurrentInterval);

            correctToolStripMenuItem.Visible = ValuesController.Correctable;
            correctValueToolStripMenuItem.Visible = ValuesController.Correctable;
            decorrectValueToolStripMenuItem.Visible = ValuesController.Correctable;
            //lockButton.Visible = ValuesController.Lockeable && !ValuesController.LockAlways;
            //okButton.Enabled = ValuesController.Editable;

            ValuesController.ValuesRetrieved += new EventHandler(valuesController_ValuesRetrieved);
            ValuesController.LockChanged += new EventHandler(valuesController_LockChanged);
            valuesController_LockChanged(ValuesController, EventArgs.Empty);

            setInterval(ValuesController.CurrentStartTime);
            TimeChanged(ValuesController.CurrentStartTime);
        }

        /// <summary>
        /// Отобразить значения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void valuesController_ValuesRetrieved(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke((EventHandler)valuesController_ValuesRetrieved, sender, e);
            else
            {
                CorrectedParamValueItem correctedValueItem;
                DataGridViewRow gridRow;

                valuesViewDataGridView.Rows.Clear();

                foreach (ParamValueItem valueItem in ValuesController.Values)
                {
                    if (valuesViewDataGridView.Rows.Count > 100)
                        break;
                    gridRow = valuesViewDataGridView.Rows[valuesViewDataGridView.Rows.Add()];
                    gridRow.Tag = valueItem;
                    gridRow.Cells[TimeColumn.Index].Value = valueItem.Time;
                    gridRow.Cells[ValueColumn.Index].Value = valueItem.Value;
                    if ((correctedValueItem = valueItem as CorrectedParamValueItem) != null)
                        gridRow.Cells[BeginValueColumn.Index].Value = correctedValueItem.OriginalValueItem.Value;
                    else
                        gridRow.Cells[BeginValueColumn.Index].Value = double.NaN;// valueItem.Value;
                    gridRow.Cells[ChangeTimeColumn.Index].Value = valueItem.ChangeTime;
                    gridRow.Cells[paramQualityColumn.Index].Value = valueItem.Quality;
                    if (ValuesController.Corrected(valueItem))
                        gridRow.Cells[ValueColumn.Index].Style = ValuesControlSettings.Instance.ValueCorrectedCellStyle;
                }
            }
        }

        /// <summary>
        /// Отметить изменения блокировки на изменение значений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void valuesController_LockChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke((EventHandler)valuesController_LockChanged, sender, e);
            else
            {
                bool locked = !ValuesController.Lockeable || ValuesController.Locked;
                bool editable = ValuesController.Editable && locked;

                ValueColumn.ReadOnly = !editable;

                if (editable)
                    ValueColumn.DefaultCellStyle = ValuesControlSettings.Instance.EnabledValueCellStyle;
                else
                    ValueColumn.DefaultCellStyle = ValuesControlSettings.Instance.DisabledValueCellStyle;

                BeginValueColumn.Visible = ValuesController.HasOriginalValue;

                addToolStripMenuItem.Visible = ValuesController.TimeEditable;
                TimeColumn.ReadOnly = !editable || !ValuesController.TimeEditable;

                if (editable && ValuesController.TimeEditable)
                    TimeColumn.DefaultCellStyle = ValuesControlSettings.Instance.EnabledCellStyle;
                else
                    TimeColumn.DefaultCellStyle = ValuesControlSettings.Instance.DefaultCellStyle;

                lockButton.Visible = ValuesController.Lockeable && !ValuesController.LockAlways;

                lockButton.Enabled = ValuesController.Lockeable && !ValuesController.Locked;
                okButton.Enabled = locked && (ValuesController.Editable || ValuesController.Correctable);
                clearButton.Enabled = locked && (ValuesController.Editable || ValuesController.Correctable);
            }
        }
   
        /// <summary>
        /// Добавить новое значение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow gridRow; 
            ParamValueItem valueItem = new ParamValueItem();
            ValuesController.SetValue(valueItem, null);
            gridRow = valuesViewDataGridView.Rows[valuesViewDataGridView.Rows.Add()];
            gridRow.Tag = valueItem;
            valuesViewDataGridView.CurrentCell = gridRow.Cells[TimeColumn.Index];
            valuesViewDataGridView.BeginEdit(true);
        }

        /// <summary>
        /// Удалить отмеченное значение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (valuesViewDataGridView.SelectedRows.Count > 0)
            {
                ParamValueItem receiveItem;

                foreach (DataGridViewRow row in valuesViewDataGridView.SelectedRows)
                {
                    if ((receiveItem = row.Tag as ParamValueItem) != null)
                        ValuesController.DeleteValue(receiveItem);
                    valuesViewDataGridView.Rows.Remove(row);
                }
            }
        }

        /// <summary>
        /// Установить значения dateTimePicker'а в соответсвие с текущим интервалом
        /// </summary>
        /// <param name="curTime">Текущие время</param>
        private void setInterval(DateTime curTime)
        {
            if (CurrentInterval==null)
            {
                CurrentInterval = Interval.Zero;
            }

            startIntervalDateTimePicker.Enabled =
                prevTimeButton.Enabled = nextTimeButton.Enabled = CurrentInterval != Interval.Zero;
            if (curTime < startIntervalDateTimePicker.MinDate)
                curTime = new DateTime(2000, 1, 1);
            String format = "---";
            if (CurrentInterval != Interval.Zero)
            {
                format = "yyyy г.";
                //if (CurrentInterval.ToDouble() > -12) { format = "MMMM " + format; }
                //if (CurrentInterval.ToDouble() > 0) { format = "dd " + format; }
                //if (CurrentInterval.ToDouble() > 0 && CurrentInterval.ToDouble() < TimeSpan.FromDays(1).TotalSeconds) { format += " HH:mm"; }
                //if (CurrentInterval.ToDouble() > 0 && CurrentInterval.ToDouble() < TimeSpan.FromMinutes(1).TotalSeconds) { format += ":ss"; }
                if (CurrentInterval < Interval.Year)
                {
                    format = "MMMM " + format;
                    if (CurrentInterval < Interval.Month)
                    {
                        format = "dd " + format;
                        if (CurrentInterval < Interval.Day)
                        {
                            format += "HH";
                            if (CurrentInterval < Interval.Hour)
                            {
                                format += ":mm";
                                if (CurrentInterval < Interval.Minute)
                                {
                                    format += ":ss";
                                }
                            }
                        }
                    }
                }
            }

            startIntervalDateTimePicker.CustomFormat = format;
            startIntervalDateTimePicker.Value = CurrentInterval.NearestEarlierTime(/*ValuesController.StartTime,*/ curTime);
        }

        //private void intervalComboBox_Validating(object sender, CancelEventArgs e)
        //{
        //    ComboBox box = (ComboBox)sender;
        //    DoubleConverter doubleconv = new DoubleConverter();

        //    try
        //    {
        //        Interval val = (Interval)intervalConverter.ConvertFrom(box.Text);
        //        if (!intervalConverter.IsValid(val)) { e.Cancel = true; return; }
        //        box.Text = intervalConverter.ConvertToString(val);
        //    }
        //    catch (FormatException) { e.Cancel = true; }
        //}

        private void prevTimeButton_Click(object sender, EventArgs e)
        {
            DateTime startTime = new ValueAggregator().GetRange(ValuesController.CurrentStartTime, ValuesController.Interval, CurrentInterval);
            startTime = CurrentInterval.GetPrevTime(startTime);
            TimeChanged(startTime);
        }

        private void nextTimeButton_Click(object sender, EventArgs e)
        {
            DateTime startTime = new ValueAggregator().GetRange(ValuesController.CurrentStartTime, ValuesController.Interval, CurrentInterval);
            startTime = CurrentInterval.GetNextTime(startTime);
            TimeChanged(startTime); //ValuesController.CurrentEndTime);
        }

        private void startIntervalDateTimePicker_CloseUp(object sender, EventArgs e)
        {
            TimeChanged(startIntervalDateTimePicker.Value);
        }

        /// <summary>
        /// Запросить новые значения
        /// </summary>
        /// <param name="time">Время начала запрашиваемого интервала</param>
        private async void TimeChanged(DateTime time)
        {
            try
            {
                // Сохранить значения, если нужно
                if (!CheckValues())
                    return;

                DateTime start, last;

                if (ValuesController.VariableConsts)
                {
                    //start = DateTime.MinValue;
                    start = ValuesController.MinDateTimeValue;
                    last = DateTime.MaxValue;
                }
                else
                {
                    start = CurrentInterval.NearestEarlierTime(/*ValuesController.StartTime,*/ time);
                    last = CurrentInterval.GetNextTime(start);
                    startIntervalDateTimePicker.Value = start;
                    new ValueAggregator().GetSourceRange(ValuesController.Interval, CurrentInterval, ref start, ref last);
                }

                await Program.MainForm.WorkflowSelector.Do(new LockActionArgs(() => ValuesController.RetrieveValue(start, last), true));
            }
            catch (Exception exc)
            {
                log.WarnException("Ошибка запроса значений параметра.", exc);
                ShowError(exc);
            }
        }

        /// <summary>
        /// Проверить изменились ли данные, и, если требуется, запросить у пользователя совета
        /// </summary>
        /// <returns>false, при отмене. В остальных случаях true</returns>
        private bool CheckValues()
        {
            if (ValuesController.HasChanges)
            {
                DialogResult result = MessageBox.Show("Значения параметра изменились. Сохранить?", "Сохранить измененные значения", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    ValuesController.Save();
                else if (result == DialogResult.Cancel)
                    return false;
            }
            return true;
        }

        private void startIntervalDateTimePicker_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                startIntervalDateTimePicker_CloseUp(sender, EventArgs.Empty);
            }
        }

        private void intervalComboBox_DropDownClosed(object sender, EventArgs e)
        {
            Interval newInterval = Interval.Zero;
            newInterval = (Interval)intervalConverter.ConvertFromString(intervalComboBox.Text);
            if (newInterval != CurrentInterval)
            {
                CurrentInterval = newInterval;
                DateTime startTime = new ValueAggregator().GetRange(ValuesController.CurrentStartTime, ValuesController.Interval, CurrentInterval);
                setInterval(startTime);
                TimeChanged(startTime);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            ValuesController.ClearValues();
            if (ValuesController.Lockeable && !ValuesController.LockAlways)
                ValuesController.ReleaseValues();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ValuesController.Save();
            OnValueSaved();
        }

        private void lockButton_Click(object sender, EventArgs e)
        {
            if (ValuesController.Lockeable)
                ValuesController.LockValues();
        }

        private void valuesViewDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == ValueColumn.Index && double.NaN.Equals(e.Value))
                e.Value = valuesViewDataGridView.Rows[e.RowIndex].Cells[BeginValueColumn.Index].Value;

            ValuesControlSettings.Instance.FormatParameterValue(e, ValuesController.Parameter);
        }

        private void valuesViewDataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            ParamValueItem par = valuesViewDataGridView.Rows[e.RowIndex].Tag as ParamValueItem;

            if (par != null)
            {
                if (ValuesController.Modified(par))
                    valuesViewDataGridView.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(valuesViewDataGridView.DefaultCellStyle.Font, FontStyle.Bold);
                else
                {
                    valuesViewDataGridView.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(valuesViewDataGridView.DefaultCellStyle.Font, FontStyle.Regular);
                }
            }
        }

        private void valuesViewDataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex==TimeColumn.Index)
            {
                try
                {
                    Convert.ToDateTime(valuesViewDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                }
                catch
                {
                    MessageBox.Show("Неверный формат времени", "Неверный формат времени", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
            else if (e.ColumnIndex == ValueColumn.Index && valuesViewDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value!=null)
            { 
                double val;
                String doubleValue=valuesViewDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                if (!double.TryParse(doubleValue,
                       System.Globalization.NumberStyles.Float,
                       System.Globalization.NumberFormatInfo.CurrentInfo,
                       out val) // распарсить введенное значение при текущей культуре
                    && !double.TryParse(doubleValue,
                       System.Globalization.NumberStyles.Float,
                       System.Globalization.NumberFormatInfo.InvariantInfo,
                       out val)) // если не получилось то по забугорной
                {
                    MessageBox.Show("Неверный формат значения", "Неверный формат значения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }

        private void valuesViewDataGridView_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow gridRow;
            ParamValueItem valueItem;
            DateTime valueTime;
            double val;

            gridRow = valuesViewDataGridView.Rows[e.RowIndex];
            valueItem = gridRow.Tag as ParamValueItem;
            valueTime = Convert.ToDateTime(gridRow.Cells[TimeColumn.Index].Value);
            String doubleValue;// = gridRow.Cells[e.ColumnIndex].Value.ToString();

            if (gridRow.Cells[ValueColumn.Index].ReadOnly)
                return;

            if (gridRow.Cells[ValueColumn.Index].Value == null
                || (!double.TryParse(doubleValue = gridRow.Cells[ValueColumn.Index].Value.ToString(),
                   System.Globalization.NumberStyles.Float,
                   System.Globalization.NumberFormatInfo.CurrentInfo,
                   out val) // распарсить введенное значение при текущей культуре
                && !double.TryParse(doubleValue,
                   System.Globalization.NumberStyles.Float,
                   System.Globalization.NumberFormatInfo.InvariantInfo,
                   out val))) // если не получилось то по забугорной
            {
                val = double.NaN;
                gridRow.Cells[ValueColumn.Index].Value = double.NaN;
            }

            valueItem.Time = valueTime;
            valueItem.Value = val;
            ValuesController.SetValue(valueItem, valueItem);
        }

        private void valuesPrevContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            ParamValueItem valueItem = null;
            bool locked = false;
            bool corrected = false;
            bool deccorected = false;

            foreach (DataGridViewRow row in valuesViewDataGridView.SelectedRows)
            {
                if ((valueItem = row.Tag as ParamValueItem) != null)
                {
                    locked = (!ValuesController.Lockeable || (ValuesController.Locked
                        && !ValuesController.LockedTimes.Contains(valueItem.Time)));
                    corrected = locked && ValuesController.Correctable;
                    deccorected = corrected && ValuesController.Corrected(valueItem);
                    break;
                }
            }

            if (valueItem == null)
            {
                locked = !ValuesController.Lockeable || ValuesController.Locked;
            }

            addToolStripMenuItem.Enabled = ValuesController.Editable && locked;
            deleteToolStripMenuItem.Enabled = valueItem != null && (corrected || (ValuesController.Editable && locked));

            correctValueToolStripMenuItem.Enabled = corrected;
            decorrectValueToolStripMenuItem.Enabled = deccorected;
        }

        private void correctValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (valuesViewDataGridView.SelectedRows.Count > 0)
            {
                ParamValueItem receiveItem;
                DataGridViewCell gridCell = null;

                foreach (DataGridViewRow row in valuesViewDataGridView.SelectedRows)
                {
                    if ((receiveItem = row.Tag as ParamValueItem) != null)
                    {
                        gridCell = row.Cells[ValueColumn.Index];
                        gridCell.ReadOnly = false;
                        gridCell.Style = ValuesControlSettings.Instance.EnabledValueCellStyle;
                        row.Tag = ValuesController.Correct(receiveItem);
                    }
                }
                if (gridCell != null)
                {
                    valuesViewDataGridView.CurrentCell = gridCell;
                    valuesViewDataGridView.BeginEdit(true);
                }
            }
        }

        private void decorrectValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (valuesViewDataGridView.SelectedRows.Count > 0)
            {
                DataGridViewCell gridCell = null;
                ParamValueItem receiveItem;

                foreach (DataGridViewRow row in valuesViewDataGridView.SelectedRows)
                {
                    if ((receiveItem = row.Tag as ParamValueItem) != null)
                    {
                        gridCell = row.Cells[ValueColumn.Index];
                        gridCell.ReadOnly = true;
                        gridCell.Style = ValuesControlSettings.Instance.DisabledValueCellStyle;
                        receiveItem = ValuesController.Decorrect(receiveItem);
                        gridCell.Value = receiveItem.Value;
                        row.Tag = receiveItem;
                    }
                }
            }
        }

        private void ParameterValuesEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = !CheckValues();
                if (!e.Cancel)
                    ValuesController.ReleaseValues();
            }
            catch (Exception exc)
            {
                ShowError(exc);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public event EventHandler ValueSaved;

        private void OnValueSaved()
        {
            if (ValueSaved!=null)
            {
                ValueSaved(this, EventArgs.Empty);
            }
        }
    }
}

using COTES.ISTOK.ClientCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using LogsIntegration;

namespace COTES.ISTOK.Client.UniForms
{
    public partial class LogsViewer : BaseAsyncWorkForm
    {
        private readonly TimeSpan max_query_period = TimeSpan.FromDays(62.0);

        public LogsViewer(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            try
            {
                //var from = dateTimePickerFrom.Value;
                //var to = dateTimePickerTo.Value;

                //if (from >= to)
                //{
                //    ShowErrorMessage("Не корректный период...");
                //    return;
                //}
                //if (from - to > max_query_period)
                //{
                //    ShowErrorMessage("Период слишком большой. Укажите период менее 62 дней.");
                //}

                //var watcher = RemoteDataService.Instance.QueryLogs(from, to);

                //watcher.AddValueRecivedHandler(
                //    (object result) => this.ShowLogs(result as IEnumerable<LogEntry>));
            }
            catch (Exception exp)
            {
                ShowErrorMessage(exp.ToString());
            }
        }

        //private void ShowLogs(IEnumerable<LogEntry> logs)
        //{
        //    if (InvokeRequired)
        //    {
        //        this.Invoke(new Action<IEnumerable<LogEntry>>(this.ShowLogs), logs);
        //        return;
        //    }

        //    foreach (var it in logs.Select(x => new object[] { x.Level, x.Time, x.LoggerName, x.Message }))
        //        dataGridViewLogs.Rows.Add(it);
        //}

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, 
                            "Ошибка", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Error);
        }
    }
}

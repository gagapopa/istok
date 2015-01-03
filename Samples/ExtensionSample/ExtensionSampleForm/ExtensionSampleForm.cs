using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using COTES.ISTOK.ParameterReceiverExtension;

namespace ExtensionSample
{
    /// <summary>
    /// Пример реализации расширения взаимодействия ИСТОК с внешними системами
    /// Источник данных для ExtensionSample
    /// Приемник данных из ISTOKExtension
    /// </summary>
    public partial class ExtensionSampleForm : Form
    {
        /// <summary>
        /// Начальное время для запроса из ИСТОК
        /// </summary>
        DateTime lastTime;

        /// <summary>
        /// Расширение передающие данные из ИСТОК
        /// </summary>
        ISTOKExtension istokExtension;

        public ExtensionSampleForm()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = String.Empty;
            toolStripStatusLabel2.Text = String.Empty;

            // установить время с которого будет запрашиваться 
            lastTime = DateTime.Now.Subtract(TimeSpan.FromDays(21));
            // создать экземпляр расширения 
            istokExtension = new ISTOKExtension();
            // подписаться на получение данных
            istokExtension.DataReady += new EventHandler(istokExtension_DataReady);
        }
        #region Получение данных
        /// <summary>
        /// Обрабатываем событие "Данные готовы" из ИСТОК
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void istokExtension_DataReady(object sender, EventArgs e)
        {
            try
            {
                DateTime now = DateTime.Now;

                // запросить данные у расширения
                Parameter[] pars = istokExtension.GetParameters(lastTime, now);

                if (pars != null && pars.Length > 0)
                    lastTime = (from p in pars select p.Time).Max();

                if (InvokeRequired) Invoke((Action)(() =>
                {
                    toolStripStatusLabel1.Text = String.Format("Время последнего события: {0}.", now);
                    toolStripStatusLabel2.Text = String.Format("Время последнего параметра: {0}", lastTime);
                    // заполнить DataGridView полученными данными
                    FillReceiveDataGridView(pars);
                }));
                else FillReceiveDataGridView(pars);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка получения данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Заполнить receiverDataGridView полученными данными
        /// </summary>
        /// <param name="pars"></param>
        private void FillReceiveDataGridView(Parameter[] pars)
        {
            if (pars != null)
                foreach (Parameter parameter in pars)
                {
                    DataGridViewRow gridRow = receiverDataGridView.Rows[receiverDataGridView.Rows.Add()];
                    gridRow.Cells[receivedIDColumn.Index].Value = parameter.Id;
                    gridRow.Cells[receivedBoilerIDColumn.Index].Value = parameter.ExternalID;
                    gridRow.Cells[receivedCodeColumn.Index].Value = parameter.Code;
                    gridRow.Cells[receivedValueColumn.Index].Value = parameter.Value;
                    gridRow.Cells[receivedTimeColumn.Index].Value = parameter.Time;
                    gridRow.Cells[receivedQualityColumn.Index].Value = parameter.Quality;
                }
        }

        /// <summary>
        /// Очистить receiverDataGridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearButton_Click(object sender, EventArgs e)
        {
            receiverDataGridView.Rows.Clear();
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно настроек циклического расчета и просмотра информации о нем
    /// </summary>
    partial class RoundRobinCalcSettingsForm : BaseAsyncWorkForm
    {
        public RoundRobinCalcSettingsForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
        }

        protected override async void OnLoad(EventArgs e)
        {
            bool canChange = strucProvider.Session.User.IsAdmin
                || strucProvider.Session.User.CheckPrivileges((int)UnitTypeId.TEP, Privileges.Write | Privileges.Execute);

            await Task.Factory.StartNew(() =>
            {
                roundRobinAutoStart = strucProvider.Session.GetRoundRobinAutoStart();

                currentStartMessages = 0;
                allMessagesCount = strucProvider.Session.GetLastRoundRobinMessagesCount();
            });
            allowAutoStartCheckBox.Checked = roundRobinAutoStart;

            await displayMessages(currentStartMessages, Math.Min(allMessagesCount, maxShownMessage));
            allowAutoStartCheckBox.Enabled = okButton.Visible = canChange;
            base.OnLoad(e);
        }

        private DateTime lastCalcDateTime = DateTime.MinValue;
        private bool roundRobinAutoStart;

        private int allMessagesCount, maxShownMessage = 100;
        private int currentStartMessages;

        private async Task displayMessages(int start, int count)
        {
            allMessagesCount = await Task.Factory.StartNew(() => strucProvider.Session.GetLastRoundRobinMessagesCount());
            calcMessageViewControl1.ClearMessage();
            //AsyncOperationWatcher watcher = 
            var m = await Task.Factory.StartNew(() => strucProvider.Session.GetLastRoundRobinMessages(start, count));
            //watcher.AddMessageReceivedHandler(m => MessagesReceiver(start, m));
            //RunWatcher(watcher, false);
            MessagesReceiver(start, m);
        }

        private void MessagesReceiver(int start, Message[] mesg)
        {
            if (InvokeRequired) Invoke((Action<int, Message[]>)MessagesReceiver, start, mesg);
            else
            {
                if (mesg != null)
                    calcMessageViewControl1.AddMessage(mesg);
            }
        }

        public void Save()
        {
            try
            {
                strucProvider.Session.SetRoundRobinAutoStart(allowAutoStartCheckBox.Checked);
                roundRobinAutoStart = allowAutoStartCheckBox.Checked;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка сохранения настроек", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkCalcServerTimer_Tick(object sender, EventArgs e)
        {
            DateTime lastStartTime, nextStartTime, lastStopDateTime;
            TimeSpan lastCalcTimeSpan = TimeSpan.Zero;
            bool calcState;

            RoundRobinInfo info = strucProvider.Session.GetRoundRobinInfo();
            lastStartTime = info.LastStartTime;
            nextStartTime = info.NextStartTime;
            lastStopDateTime = info.LastStopTime;
            lastCalcTimeSpan = info.LastCalcTimeSpan;

            if (lastStopDateTime > lastCalcDateTime)
            {
                currentStartMessages = 0;
                allMessagesCount = strucProvider.Session.GetLastRoundRobinMessagesCount();
                displayMessages(currentStartMessages,
                    Math.Min(maxShownMessage, Math.Abs(allMessagesCount - currentStartMessages - maxShownMessage)));
                lastCalcDateTime = lastStopDateTime;
            }
            calcState = lastStopDateTime < lastStartTime;

            String timerString = (calcState ? "Идет расчет." : "Расчет остановлен.");
            if (!lastCalcTimeSpan.Equals(TimeSpan.Zero))
                timerString += String.Format(" Последний расчет длился: {0}", lastCalcTimeSpan);
            calcTimerLabel.Text = timerString;
            lastCalcStartLabel.Text = String.Format("Время последнего расчета: {0}", DateTimeToString(lastStartTime));
            nextCalcStartLabel.Text = String.Format("Время следующего расчета: {0}", DateTimeToString(nextStartTime));
        }

        private string DateTimeToString(DateTime time)
        {
            String retString;
            if (time.Equals(DateTime.MinValue)) retString = "не известно";
            else if (time.Equals(DateTime.MaxValue)) retString = "никогда";
            else retString = time.ToString("yyyy-MM-dd HH:mm:ss");
            return retString;
        }

        private void prevMessagesButton_Click(object sender, EventArgs e)
        {
            currentStartMessages -= maxShownMessage;
            if (currentStartMessages < 0) currentStartMessages = 0;

            displayMessages(currentStartMessages,
                Math.Min(maxShownMessage, allMessagesCount - currentStartMessages));
        }

        private void nextMessagesButton_Click(object sender, EventArgs e)
        {
            currentStartMessages += maxShownMessage;

            displayMessages(currentStartMessages,
                 Math.Min(maxShownMessage, allMessagesCount - currentStartMessages));
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Save();
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
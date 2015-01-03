using COTES.ISTOK.ClientCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace COTES.ISTOK.Client
{
    partial class BannerForm : Form
    {
        private Graphics grp;
        private String statusFormat = "Загрузка структуры: {0}";
        private const int stringCutInterval = 2;

        //private RemoteDataService remoteDataService;
        Session session;

        public BannerForm(Session session)
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Abort;
            this.session = session;
            //this.remoteDataService = remoteDataService;
            grp = this.CreateGraphics();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool cuted = false;
            double progress = 0.0, templateWidth;
            String curNode;

            try
            {
                progress = session.GetLoadProgress();
                if (progress == -1) throw new Exception("Ошибка получения прогресса");

                loadProgressBar.Value = (int)(progress < loadProgressBar.Maximum ? progress : loadProgressBar.Maximum);

                curNode = session.GetLoadStatusString();
                templateWidth = grp.MeasureString(statusFormat, statusLabel.Font).Width;

                while (grp.MeasureString(curNode, statusLabel.Font).Width >
                    (this.Width - templateWidth - statusLabel.Left * 2) && curNode.Length > stringCutInterval)
                {
                    curNode = curNode.Substring(0, curNode.Length - stringCutInterval);
                    cuted = true;
                }
                if (cuted) curNode += "...";
                statusLabel.Text = String.Format(statusFormat, curNode); ;

                if (progress >= loadProgressBar.Maximum)
                //throw new Exception("Перебор прогресса");
                {
                    timer1.Enabled = false;
                    this.DialogResult = DialogResult.OK;
                    //this.Hide();
                    this.Close();
                }
            }
            catch 
            { 
                timer1.Enabled = false;
                this.DialogResult = DialogResult.Abort;
                //this.Hide();
                this.Close(); 
            }
        }
    }
}

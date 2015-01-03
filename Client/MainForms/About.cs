using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace COTES.ISTOK.Client
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
#if EMA
            textBox1.Text = String.Format("АСТДК \r\n на базе «ИСТОК-СБК», v.{0}" +
                "\r\nЗакрытое акционерное общество"+
                "\r\n«Е4-СибКОТЭС»"+
                "\r\n\r\nhttp://www.cotes.ru "+
                "\r\ne-mail: sme@cotes.ru"+
                "\r\nтел. 8(383)3358358", VersionInfo.BuildVersion.VersionString/*CommonData.Version*/);
#else
            textBox1.Text = String.Format("«ИСТОК-СБК», v.{0}" +
                "\r\nЗакрытое акционерное общество"+
                "\r\n«Е4-СибКОТЭС»"+
                "\r\n\r\nhttp://www.cotes.ru "+
                "\r\ne-mail: sme@cotes.ru" +
                "\r\ne-mail: leontev@cotes.ru" +
                "\r\nтел. 8(383)3358358", VersionInfo.BuildVersion.VersionString/*CommonData.Version*/);
#endif
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
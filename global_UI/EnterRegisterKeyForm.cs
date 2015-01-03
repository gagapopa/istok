using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace COTES.ISTOK.Assignment.UI
{
    public partial class EnterRegisterKeyForm : Form
    {
        public EnterRegisterKeyForm()
        {
            InitializeComponent();
        }

        private UInt64 machineCode;
        public UInt64 MachineCode
        {
            get { return machineCode; }
            set
            {
                machineCode = value;
                machineCodeTextBox.Text = machineCode.ToString("X");
            }
        }
        public byte[] EnteredKey
        {
            get
            {
                char[] c = registerKeyTextBox.Text.Replace(" ", String.Empty).ToCharArray();
                byte[] res = new byte[(int)Math.Ceiling((float)c.Length / 2)];
                int i;
                for (i = 0; i < c.Length; i++)
                {
                    byte val = (byte)int.Parse(c[i].ToString(), System.Globalization.NumberStyles.HexNumber);
                    if (i % 2 == 0) val <<= 4;
                    res[i / 2] += val;
                }
                return res;
            }
        }
    }
}

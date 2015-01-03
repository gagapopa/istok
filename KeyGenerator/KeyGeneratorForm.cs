using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK;

namespace COTES.ISTOK.KeyGenerator
{
    public partial class KeyGeneratorForm : Form
    {
        public KeyGeneratorForm()
        {
            InitializeComponent();
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            LicenseKey key = new LicenseKey();
            key.Organization = organizatonTextBox.Text;
            key.RegisterDate = registerDateTimePicker.Value;
            key.MaxBlockCount = (int)nudMaxBlockCount.Value;

            int i;
            ulong code = 0;
            String outer = "";

            try
            {
                code = ulong.Parse(machineCodeTextBox.Text, System.Globalization.NumberStyles.HexNumber);
            }
            catch (FormatException exc) { MessageBox.Show(exc.Message); }

            byte[] generatedKey = key.GenerateKey(code);

            for (i = 0; i < generatedKey.Length; i++)
            {
                if (i > 0) outer += " ";
                outer += generatedKey[i].ToString("X2");
            }

            registerKeyTextBox.Text = outer;
        }
    }
}

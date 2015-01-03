using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.Calc;

namespace IF97Calc
{
    public partial class IF97CalcForm : Form
    {
        /// <summary>
        /// перевод из кгс/см^2 в МПа
        /// </summary>
        static double MPa2kgs = 10.1972;
        /// <summary>
        /// перевод °C в °K
        /// </summary>
        static double C2K = 273.15;
        /// <summary>
        /// перевод из ккал в кДж
        /// </summary>
        static double J2kal = 0.238846;

        /// <summary>
        /// Перевод результатов функции в указанный 
        /// (для перевода из указанного в аргумент нужно делить или отнимать)
        /// </summary>
        double[] pUnitFactor = { 1, MPa2kgs },      // "МПа", "кгс/см^2"
                 tUnitFactor = { -C2K, 0 },         // "°C", "°K"
                 tsUnitFactor = { -C2K, 0 },        // "°C", "°K"
                 psUnitFactor = { 1, MPa2kgs },     // "МПа", "кгс/см^2"
                 sUnitFactor = { 1, J2kal },        // "кДж/(кг∙°K)", "ккал/(кг∙°K)"
                 uUnitFactor = { 1, J2kal },        // "кДж/кг", "ккал/кг"
                 nuUnitFactor = { 1 },              // "м^3/кг"
                 hUnitFactor = { 1, J2kal },        // "кДж/кг", "ккал/кг"
                 wUnitFactor = { 1 },               // "м/с"
                 cnuUnitFactor = { 1, J2kal },      // "кДж/(кг∙°K)", "ккал/(кг∙°K)"
                 cpUnitFactor = { 1, J2kal };       // "кДж/(кг∙°K)", "ккал/(кг∙°K)"

        IF97 IF97lib = new IF97();

        public IF97CalcForm()
        {
            InitializeComponent();
            pUnitComboBox.SelectedIndex = 0;
            tUnitComboBox.SelectedIndex = 0;
            tsUnitComboBox.SelectedIndex = 0;
            psUnitComboBox.SelectedIndex = 0;
            sUnitComboBox.SelectedIndex = 0;
            uUnitComboBox.SelectedIndex = 0;
            nuUnitComboBox.SelectedIndex = 0;
            hUnitComboBox.SelectedIndex = 0;
            wUnitComboBox.SelectedIndex = 0;
            cnuUnitComboBox.SelectedIndex = 0;
            cpUnitComboBox.SelectedIndex = 0;
        }

        private void pTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double p, T;

                if (!double.TryParse(pTextBox.Text, out p)) p = double.NaN;
                if (!double.TryParse(tTextBox.Text, out T)) T = double.NaN;

                p /= pUnitFactor[pUnitComboBox.SelectedIndex];
                T -= tUnitFactor[tUnitComboBox.SelectedIndex];

                double ps, Ts, nu, u, s, h, cp, cnu, w;

                try
                {
                    ps = IF97lib.p_s(T) * psUnitFactor[psUnitComboBox.SelectedIndex];
                    psTextBox.Text = ps.ToString();
                }
                catch
                {
                    psTextBox.Text = "---";
                }

                try
                {
                    Ts = IF97lib.T_s(p) + tsUnitFactor[tsUnitComboBox.SelectedIndex];
                    tsTextBox.Text = Ts.ToString();
                }
                catch
                {
                    tsTextBox.Text = "---";
                }

                try
                {
                    nu = IF97lib.nu(p, T) * nuUnitFactor[nuUnitComboBox.SelectedIndex]; ;
                    nuTextBox.Text = nu.ToString();
                }
                catch
                {
                    nuTextBox.Text = "---";
                }

                try
                {
                    u = IF97lib.u(p, T) * uUnitFactor[uUnitComboBox.SelectedIndex];
                    uTextBox.Text = u.ToString();
                }
                catch
                {
                    uTextBox.Text = "---";
                }

                try
                {
                    s = IF97lib.s(p, T) * sUnitFactor[sUnitComboBox.SelectedIndex];
                    sTextBox.Text = s.ToString();
                }
                catch
                {
                    sTextBox.Text = "---";
                }

                try
                {
                    h = IF97lib.h(p, T) * hUnitFactor[hUnitComboBox.SelectedIndex];
                    hTextBox.Text = h.ToString();
                }
                catch
                {
                    hTextBox.Text = "---";
                }

                try
                {
                    cp = IF97lib.c_p(p, T) * cpUnitFactor[cpUnitComboBox.SelectedIndex];
                    cpTextBox.Text = cp.ToString();
                }
                catch
                {
                    cpTextBox.Text = "---";
                }

                try
                {
                    cnu = IF97lib.c_nu(p, T) * cnuUnitFactor[cnuUnitComboBox.SelectedIndex];
                    cnuTextBox.Text = cnu.ToString();
                }
                catch
                {
                    cnuTextBox.Text = "---";
                }

                try
                {
                    w = IF97lib.w(p, T) * wUnitFactor[wUnitComboBox.SelectedIndex];
                    wTextBox.Text = w.ToString();
                }
                catch
                {
                    wTextBox.Text = "---";
                }
            }
            catch
            {
                psTextBox.Text = "---";
                tsTextBox.Text = "---";
                uTextBox.Text = "---";
                sTextBox.Text = "---";
                hTextBox.Text = "---";
                cpTextBox.Text = "---";
                cnuTextBox.Text = "---";
                wTextBox.Text = "---";
            }
        }

        const int pUnitSIIndex = 0,
                  tUnitSIIndex = 1,
                  tsUnitSIIndex = 1,
                  psUnitSIIndex = 0,
                  sUnitSIIndex = 0,
                  uUnitSIIndex = 0,
                  nuUnitSIIndex = 0,
                  hUnitSIIndex = 0,
                  wUnitSIIndex = 0,
                  cnuUnitSIIndex = 0,
                  cpUnitSIIndex = 0;

        const int pUnitAnotherIndex = 1,
                  tUnitAnotherIndex = 0,
                  tsUnitAnotherIndex = 0,
                  psUnitAnotherIndex = 1,
                  sUnitAnotherIndex = 1,
                  uUnitAnotherIndex = 1,
                  nuUnitAnotherIndex = 0,
                  hUnitAnotherIndex = 1,
                  wUnitAnotherIndex = 0,
                  cnuUnitAnotherIndex = 1,
                  cpUnitAnotherIndex = 1;

        private void unitComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignoreUnitChange) return;

            pTextBox_TextChanged(sender, e);
            siCheckBox.Checked = (pUnitComboBox.SelectedIndex == pUnitSIIndex &&
                                  tUnitComboBox.SelectedIndex == tUnitSIIndex &&
                                  tsUnitComboBox.SelectedIndex == tsUnitSIIndex &&
                                  psUnitComboBox.SelectedIndex == psUnitSIIndex &&
                                  sUnitComboBox.SelectedIndex == sUnitSIIndex &&
                                  uUnitComboBox.SelectedIndex == uUnitSIIndex &&
                                  nuUnitComboBox.SelectedIndex == nuUnitSIIndex &&
                                  hUnitComboBox.SelectedIndex == hUnitSIIndex &&
                                  wUnitComboBox.SelectedIndex == wUnitSIIndex &&
                                  cnuUnitComboBox.SelectedIndex == cnuUnitSIIndex &&
                                  cpUnitComboBox.SelectedIndex == cpUnitSIIndex);
            anotherCheckBox.Checked = (pUnitComboBox.SelectedIndex == pUnitAnotherIndex &&
                                       tUnitComboBox.SelectedIndex == tUnitAnotherIndex &&
                                       tsUnitComboBox.SelectedIndex == tsUnitAnotherIndex &&
                                       psUnitComboBox.SelectedIndex == psUnitAnotherIndex &&
                                       sUnitComboBox.SelectedIndex == sUnitAnotherIndex &&
                                       uUnitComboBox.SelectedIndex == uUnitAnotherIndex &&
                                       nuUnitComboBox.SelectedIndex == nuUnitAnotherIndex &&
                                       hUnitComboBox.SelectedIndex == hUnitAnotherIndex &&
                                       wUnitComboBox.SelectedIndex == wUnitAnotherIndex &&
                                       cnuUnitComboBox.SelectedIndex == cnuUnitAnotherIndex &&
                                       cpUnitComboBox.SelectedIndex == cpUnitAnotherIndex);
        }

        bool ignoreUnitChange = false;
        private void siButton_Click(object sender, EventArgs e)
        {
            try
            {
                ignoreUnitChange = true;
                pUnitComboBox.SelectedIndex = pUnitSIIndex;
                tUnitComboBox.SelectedIndex = tUnitSIIndex;
                tsUnitComboBox.SelectedIndex = tsUnitSIIndex;
                psUnitComboBox.SelectedIndex = psUnitSIIndex;
                sUnitComboBox.SelectedIndex = sUnitSIIndex;
                uUnitComboBox.SelectedIndex = uUnitSIIndex;
                nuUnitComboBox.SelectedIndex = nuUnitSIIndex;
                hUnitComboBox.SelectedIndex = hUnitSIIndex;
                wUnitComboBox.SelectedIndex = wUnitSIIndex;
                cnuUnitComboBox.SelectedIndex = cnuUnitSIIndex;
                cpUnitComboBox.SelectedIndex = cpUnitSIIndex;
            }
            finally
            {
                ignoreUnitChange = false;
                unitComboBox_SelectedIndexChanged(sender, e);
            }
        }

        private void anotherButton_Click(object sender, EventArgs e)
        {
            try
            {
                ignoreUnitChange = true;
                pUnitComboBox.SelectedIndex = pUnitAnotherIndex;
                tUnitComboBox.SelectedIndex = tUnitAnotherIndex;
                tsUnitComboBox.SelectedIndex = tsUnitAnotherIndex;
                psUnitComboBox.SelectedIndex = psUnitAnotherIndex;
                sUnitComboBox.SelectedIndex = sUnitAnotherIndex;
                uUnitComboBox.SelectedIndex = uUnitAnotherIndex;
                nuUnitComboBox.SelectedIndex = nuUnitAnotherIndex;
                hUnitComboBox.SelectedIndex = hUnitAnotherIndex;
                wUnitComboBox.SelectedIndex = wUnitAnotherIndex;
                cnuUnitComboBox.SelectedIndex = cnuUnitAnotherIndex;
                cpUnitComboBox.SelectedIndex = cpUnitAnotherIndex;
            }
            finally
            {
                ignoreUnitChange = false;
                unitComboBox_SelectedIndexChanged(sender, e);
            }

        }
    }
}

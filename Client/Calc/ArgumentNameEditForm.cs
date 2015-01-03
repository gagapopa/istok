using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using COTES.ISTOK.Calc;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Окно для редактирования имени аргумента
    /// </summary>
    public partial class ArgumentNameEditForm : Form
    {
        public ArgumentNameEditForm(OptimizationArgument value)
        {
            InitializeComponent();

            OptimizationArgument = value;
            argumentNameTextBox.Text = OptimizationArgument != null ? OptimizationArgument.Name : "";
            if (OptimizationArgument != null)
                switch (OptimizationArgument.Mode)
                {
                    case OptimizationArgumentMode.Manual:
                        manualModeRadioButton.Checked = true;
                        break;
                    case OptimizationArgumentMode.Interval:
                        intervalModeRadioButton.Checked = true;
                        break;
                    case OptimizationArgumentMode.ColumnNum:
                        columnNumModeRadioButton.Checked = true;
                        break;
                    case OptimizationArgumentMode.Default:
                    case OptimizationArgumentMode.Expression:
                        expressionModeRadioButton.Checked = true;
                        break;
                    default:
                        break;
                }
            else expressionModeRadioButton.Checked = true;

            intervalFromTextBox.Text = OptimizationArgument == null || double.IsNaN(OptimizationArgument.IntervalFrom) ? ""
                : OptimizationArgument.IntervalFrom.ToString();
            intervalToTextBox.Text = OptimizationArgument == null || double.IsNaN(OptimizationArgument.IntervalTo) ? ""
                : OptimizationArgument.IntervalTo.ToString();
            intervalStepTextBox.Text = OptimizationArgument == null || double.IsNaN(OptimizationArgument.IntervalStep) ? ""
                : OptimizationArgument.IntervalStep.ToString();
        }

        /// <summary>
        /// Правило именования переменных
        /// </summary>
        Regex regex = new Regex(@"\p{L}|_\w*");

        /// <summary>
        /// Имеющиеся аргументы
        /// </summary>
        public CalcArgumentInfo[] Args { get; set; }

        //String argumentName;
        ///// <summary>
        ///// Имя аргумента
        ///// </summary>
        //public String ArgumentName
        //{
        //    get { return argumentName; }
        //    set
        //    {
        //        argumentName = value;
        //        argumentNameTextBox.Text = value;
        //    }
        //}

        //OptimizationGateNode.OptimizationArgument argument;

        public OptimizationArgument OptimizationArgument
        { get; protected set; }

        private void okButton_Click(object sender, EventArgs e)
        {
            bool ret = false;
            String name = argumentNameTextBox.Text;
            name = name.Trim();
            if (!regex.IsMatch(name))
                MessageBox.Show("Имя аргумента содержит не допустимые символы");
            else
            {
                if (OptimizationArgument == null)
                    OptimizationArgument = new OptimizationArgument();

                for (int i = 0; !ret && i < Args.Length; i++)
                    ret = String.Equals(Args[i].Name, name) && !String.Equals(Args[i].Name, OptimizationArgument.Name);
                if (ret)
                {
                    MessageBox.Show("Имя аргумента должно быть уникально");
                    return;
                }
                else
                    OptimizationArgument.Name = name;

                if (manualModeRadioButton.Checked)
                    OptimizationArgument.Mode = OptimizationArgumentMode.Manual;
                if (expressionModeRadioButton.Checked)
                    OptimizationArgument.Mode = OptimizationArgumentMode.Expression;
                if (intervalModeRadioButton.Checked)
                    OptimizationArgument.Mode = OptimizationArgumentMode.Interval;
                if (columnNumModeRadioButton.Checked)
                    OptimizationArgument.Mode = OptimizationArgumentMode.ColumnNum;

                double intervalFrom, intervalTo, intervalStep;

                if (!double.TryParse(intervalFromTextBox.Text, out intervalFrom))
                    intervalFrom = double.NaN;
                if (!double.TryParse(intervalToTextBox.Text, out intervalTo))
                    intervalTo = double.NaN;
                if (!double.TryParse(intervalStepTextBox.Text, out intervalStep))
                    intervalStep = double.NaN;

                OptimizationArgument.IntervalFrom = intervalFrom;
                OptimizationArgument.IntervalTo = intervalTo;
                OptimizationArgument.IntervalStep = intervalStep;

                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void intervalModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            intervalFromTextBox.Enabled =
                intervalToTextBox.Enabled =
                intervalStepTextBox.Enabled = intervalModeRadioButton.Checked;
        }
    }
}

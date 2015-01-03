using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace COTES.ISTOK.Assignment.UI
{
    public partial class CreateDBForm : Form
    {
        public CreateDBForm()
        {
            InitializeComponent();

#if DEBUG
            cbxHosts.Text = @"ZERG\SQLEXPRESS";
            dataBaseSuffixTextBox.Text = "ntec4";

            useInternalSecurityCheckBox.Checked = true;
            dataBaseDataPathTextBox.Text = @"c:\DATA";
            dataBaseLogFilePathTextBox.Text = @"c:\DATA";
#endif
        }

        private void createDBButton_Click(object sender, EventArgs e)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.DataSource = cbxHosts.Text;

            if (useInternalSecurityCheckBox.Checked)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = userLoginTextBox.Text;
                builder.Password = userPasswordTextBox.Text;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    //using (var trans = connection.BeginTransaction())
                    //{
                        using (SqlCommand command = connection.CreateCommand())
                        {
                        //    command.Transaction = trans;

                            // Create empty datebase
                            command.CommandText = GetScript(Properties.Resources.StationCreateDatabase);
                            command.ExecuteNonQuery();
                            //trans.Commit();

                            // Create core tables
                            command.CommandText = GetScript(Properties.Resources.StationCreateCoretables);
                            command.ExecuteNonQuery();
                            //trans.Commit();

                            // Insert standard unit types
                            command.CommandText = GetScript(Properties.Resources.StationInsertTypes);
                            command.ExecuteNonQuery();
                            //trans.Commit();

                            // Insert standard intervals
                            command.CommandText = GetScript(Properties.Resources.StationInsertIntervals);
                            command.ExecuteNonQuery();
                            //trans.Commit();
                        }
                    //}
                }
                this.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Database creation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetScript(string script)
        {
            String createScript = script;

            String dbName = String.Format("station-{0}", dataBaseSuffixTextBox.Text);

            createScript = createScript.Replace("$DBNAME", dbName);
            createScript = createScript.Replace("$DATAPATH", dataBaseDataPathTextBox.Text);
            createScript = createScript.Replace("$LOGPATH", dataBaseLogFilePathTextBox.Text);

            return createScript;
        }

        private void CreateDBForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable servTable = SqlDataSourceEnumerator.Instance.GetDataSources();

                cbxHosts.Items.Clear();
                foreach (DataRow item in servTable.Rows)
                {
                    cbxHosts.Items.Add(item[0] + "\\" + item[1]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка");
            }
        }
    }
}

using COTES.ISTOK;
using COTES.ISTOK.Block;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace COTES.ISTOK
{
    public partial class Form1 : Form
    {
        DALManager dalManager = null;
        ChannelManager chManager = null;
        ValueBuffer valBuffer = null;
        ValueReceiver valReceiver = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Init()
        {
            try
            {
                BlockSettings.Instance.Load(BlockSettings.DefaultConfigPath);
                SetupNSI();
                dalManager = SetupDAL();
                dalManager.Connect();
                
                valBuffer = new ValueBuffer();
                valReceiver = new ValueReceiver(dalManager, valBuffer);
                chManager = new ChannelManager(dalManager, valBuffer);
                LoadParameters();
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
        }

        private void ProcessError(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        private void LoadParameters()
        {
            clbParameters.Items.Clear();
            var channels = dalManager.GetChannels(chManager.GetAvailableModules());
            foreach (var channel in channels)
            {
                foreach (var param in channel.Parameters)
                    clbParameters.Items.Add(new ListBoxItem(param, string.Format("{0}\\{2}-{1}", channel.Name, param.Name, param.Idnum)));
            }
        }

        #region Setup
        private void SetupNSI()
        {
            // путь к загрузочным модулям
            const String path = "Modules";
            NSI.LoadersPath = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), path);
        }
        private DALManager SetupDAL()
        {
            DALManager dalManager;
            var props = new Dictionary<string, string>();
            string db_type = BlockSettings.Instance.DataBase.Type;
            ServerType serverType = ServerTypeFormatter.Format(db_type);

            dalManager = CreateDAL(serverType);

            props.Add("DB_host", BlockSettings.Instance.DataBase.Host);
            props.Add("DB_name", BlockSettings.Instance.DataBase.Name);
            props.Add("DB_user", BlockSettings.Instance.DataBase.User);
            props.Add("DB_pass",
            CommonData.SecureStringToString(
               CommonData.DecryptText(
                   CommonData.Base64ToString(
                       BlockSettings.Instance.DataBase.Password))));
            try { props.Add("ReplicationUser", BlockSettings.Instance.ReplicationUser); }
            catch { }
            try { props.Add("ReplicationPassword", BlockSettings.Instance.ReplicationPassword); }
            catch { }
            dalManager.ConnectionProperties = props;
            //try { dalManager.Connect(); }
            //catch (Exception exc)
            //{
            //    MessageLog.Message(MessageLevel.Error, "Ошибка подключения к БД: {0}", exc.Message);
            //}
            dalManager.CheckConnection();

            return dalManager;
        }
        private static DALManager CreateDAL(ServerType serverType)
        {
            DALManager dalManager;
            switch (serverType)
            {
                case ServerType.MSSQL:
                    dalManager = new MSSqlDAL();
                    break;
                default:
                    throw new NotSupportedException(ServerTypeFormatter.Format(serverType));
            }
            return dalManager;
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void btnLoadPackages_Click(object sender, EventArgs e)
        {
            try
            {
                lbxPackages.Items.Clear();
                
                foreach (ListBoxItem param in clbParameters.CheckedItems)
                {
                    var packages = valReceiver.LoadPackage(((ParameterItem)param.Tag).Idnum, dtpDateFrom.Value, dtpDateTo.Value, 640); //640 достаточно каждому
                    foreach (var package in packages)
                    {
                        lbxPackages.Items.Add(new ListBoxItem(package, string.Format("Package{0} ({1}) {2}-{3}", package.Id, package.Count, package.DateFrom, package.DateTo)));
                    }
                }
                
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
        }

        private void lbxPackages_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Package package = null;
                var tag = lbxPackages.SelectedItem as ListBoxItem;
                
                if (tag != null) package = tag.Tag as Package;
                if (package == null)
                {
                    dgv.DataSource = null;
                    dgv.Rows.Clear();
                    dgv.Columns.Clear();
                    return;
                }
                dgv.DataSource = package.Values;
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
        }
    }

    class ListBoxItem
    {
        public string Text { get; set; }
        public object Tag { get; private set; }

        public ListBoxItem(object tag)
        {
            Tag = tag;
        }
        public ListBoxItem(object tag, string text)
            : this(tag)
        {
            Text = text;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Text) && Tag != null)
                return Tag.ToString();
            return Text;
        }
    }
}

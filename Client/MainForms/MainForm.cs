using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Client.Extension;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    partial class MainForm : Form
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private ImageList icons = new ImageList();

        //private RemoteDataService remoteDataService;

        private Session session;
        StructureProvider strucProvider;

        private Dictionary<string, UniForm> dicForms = new Dictionary<string, UniForm>();
        private DiagnosticsForm frmDiag = null;

        public WorkflowSelector WorkflowSelector { get; set; }

        private bool connected = false;

        public MainForm()
        {
            InitializeComponent();
#if EMA
            this.Text = "АСТДК. Интерфейсный модуль";
#endif

            session = new Session();
            session.ErrorOcuired += ShowError;
            //remoteDataService = RemoteDataService.Instance;
            //remoteDataService.ServerNotAccessible += new EventHandler(remoteDataService_ServerNotAccessible);
            //remoteDataService.UserDisconnected += new EventHandler(remoteDataService_UserDisconnected);
            //remoteDataService.TypesChanged += new EventHandler<UnitTypeEventArgs>(remoteDataService_TypesChanged);

            try
            {
                RemotingConfiguration.CustomErrorsEnabled(false);
                RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
            }
            catch (Exception) { }
            //костыль для устранения мелькания формы, когда нажимаешь Отмену в окне авторизации
            this.Opacity = 0;
        }

        void remoteDataService_TypesChanged(object sender, UnitTypeEventArgs e)
        {
            if (connected)
            {
                LoadImageList();
            }
        }

        List<Form> extendForms = new List<Form>();

        /// <summary>
        /// Добавить информацию об окне, созданном не в MDI
        /// </summary>
        /// <remarks>
        /// Информирует MainForm о том, что указанное окно 
        /// необходимо будет закрыть при дисконекте или переподключении
        /// </remarks>
        /// <param name="form"></param>
        public void AddExtendForm(Form form)
        {
            form.FormClosed += new FormClosedEventHandler(form_FormClosed);
            lock (extendForms)
            {
                extendForms.Add(form);
            }
        }

        void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            lock (extendForms)
            {
                Form form = sender as Form;
                if (form != null)
                    extendForms.Remove(form);
            }
        }

        protected virtual void CloseAllChildForms()
        {
            BaseAsyncWorkForm baseForm;
            var forms = new List<Form>(MdiChildren);
            forms.AddRange(extendForms);

            foreach (Form form in forms)//MdiChildren)
            {
                try
                {
                    if ((baseForm = form as BaseAsyncWorkForm) != null)
                        baseForm.AbortAllAsyncOperations();
                    //form.Dispose();
                    form.Invoke((Action)form.Close);
                    //form.Close();
                }
                catch (ObjectDisposedException) { form.Close(); }
            }
            //nothing
        }


        bool reconnecting = false;
        private void Reconnect()
        {
            if (!reconnecting)
            {
                reconnecting = true;
                Disconnect();

                ShowConnectForm();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                ClientSettings.Instance.Load(ClientSettings.DefaultConfigPath);// ClientSettings.Instance.DefaultConfigFile);
            }
            catch
            {
                try
                {
                    ClientSettings.Instance.Load(ClientSettings.CommonDefaultConfigPath);//ClientSettings.Instance.CommonDefaultConfigFile);
                }
                catch
                {
                    // ...
                }
            }

            //ShowMenu();
            //UpdateViewChecks();
            //this.Opacity = 0;
            //ShowConnectForm();
            //ClientSettings.Instance.LoadFormState(this);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            //ShowMenu();
            UpdateViewChecks();
            //this.Opacity = 0;
            ShowConnectForm();
        }

        /// <summary>
        /// Обновить выбранные элементы в меню "Вид"
        /// </summary>
        private void UpdateViewChecks()
        {
            toolStripToolStripMenuItem.Checked = toolStrip1.Visible;
            statusStripToolStripMenuItem.Checked = statusStrip1.Visible;
        }

        /// <summary>
        /// Набор иконок для дерева
        /// </summary>
        internal ImageList Icons { get { return icons; } }

        private MessagesForm messForm;

        /// <summary>
        /// Окно вывода сообщений
        /// </summary>
        public MessagesForm MessagesForm
        {
            get
            {
                if (messForm == null || !messForm.IsHandleCreated)
                {
                    Action del = () =>
                    {
                        messForm = new MessagesForm();
                        messForm.MdiParent = this;
                    };
                    if (InvokeRequired) Invoke(del);
                    else del();
                }
                return messForm;
            }
        }

        /// <summary>
        /// Загружает из от глобала иконки.
        /// Пока загрузка синхронная, в принципе ее синхронной
        /// можно и оставить.
        /// </summary>
        private void LoadImageList()
        {
            if (InvokeRequired) BeginInvoke((Action)LoadImageList);
            else
            {
                MemoryStream stream = null;
                //UTypeNode[] types = remoteDataService.Types;
                UTypeNode[] types = session.Types;
                icons.Images.Clear();
                if (types != null)
                {
                    foreach (var type in types)
                    {
                        if (type.Icon != null)
                        {
                            stream = new MemoryStream(type.Icon.Clone() as byte[]);
                            icons.Images.Add(type.Idnum.ToString(),
                                             Image.FromStream(stream));
                        }
                    }
                }
            }
        }

        int connectAttempts = -1;
        int connectAttemptsMax = 3;
        bool showServer;

        private void ShowConnectForm(AutorizationForm prevAuthForm = null)
        {
            Hide();
            connected = false;

            AutorizationForm authForm = new AutorizationForm(prevAuthForm);
            authForm.FormClosed += authForm_FormClosed;

            if (connectAttempts < 0)
            {
                connectAttempts = 0;
                showServer = false;
            }
            else
            {
                ++connectAttempts;
            }

            if (showServer)
            {
                authForm.ShowServer();
            }

            if (connectAttempts < connectAttemptsMax)
            {
                this.Hide();
                authForm.Show();
                //authForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Количетсво попыток подключения истекло");
                Application.Exit();
            }
        }

        void authForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            AutorizationForm authForm = sender as AutorizationForm;    

            if (authForm.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    session.HostPort = authForm.ServerName;
                    session.Connect(authForm.UserName, authForm.Password);

                    SetStatusLabel();

                    //LoadUnitTypes();
                    LoadImageList();
                    strucProvider = session.GetStructureProvider();
                    connected = true;
                    authForm.SaveParams();
                }
                catch (NoOneUserException) // создание первого пользователя
                {
                    ShowNewAdminForm();
                }
                catch (Exception exc)
                {
                    ShowConnectError(exc);
                }
                if (connected)
                {
                    authForm.Hide();
                    connectAttempts = -1;
                    PostConnect();
                }
                else
                {
                    ShowConnectForm(authForm);
                }
            }
            else
            {
                connectAttempts = -1;
                Application.Exit();
            }
        }

        private void ShowConnectError(Exception exc)
        {
            Exception exception = exc;

            if (exc is UserNotConnectedException && exc.InnerException != null)
            {
                exception = exc.InnerException;
            }

            String caption = "Ощибка подключения к серверу приложения";
            String message = exception.Message;

            if (exception is System.ServiceModel.EndpointNotFoundException)
                showServer = true;
            else if (exception is UserNotConnectedException)
            {
                message = "Имя пользователя или пароль не верны";
            }
            //#if DEBUG
            //                            if (exc.InnerException != null)
            //                                message += String.Format("\nInnertException: {0}", exc.InnerException.Message);
            //                            String stackTrace;
            //                            if (exc.Data.Contains("StackTrace"))
            //                                stackTrace = exc.Data["StackTrace"].ToString();
            //                            else
            //                                stackTrace = exc.StackTrace;
            //                            message += String.Format("\nStackTrace: {0}", stackTrace);
            //#endif
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void ShowNewAdminForm()
        {
            if (MessageBox.Show("В системе не зарегистрировано ни одного пользователя",
                                "Необходимо добавить нового администратора",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                try
                {
                    UserEditForm n_user = new UserEditForm(new UserNode(),
                                                           null, //session.Types,
                                                           null, //new List<GroupNode>(session.GetGroupNodes()), 
                                                           true,
                                                           true);
                    if (n_user.ShowDialog() == DialogResult.OK)
                        session.NewAdmin(n_user.EditingNode);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Администратор добавлен кем-то другим", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PostConnect()
        {
            try // окно загрузки справочников
            {
                reconnecting = false;
                if (connected && session.GetLoadProgress() < 100)
                {
                    Hide();
                    BannerForm bannerForm = new BannerForm(session);

                    bannerForm.FormClosed += (s, e) =>
                    {
                        if (((Form)s).DialogResult == System.Windows.Forms.DialogResult.OK)
                            ShowAfterConnect();
                        else
                            ShowConnectForm();
                    };
                    bannerForm.Show();
                    return;
                }
            }
            catch { connected = false; }
            ShowAfterConnect();
        }

        private void ShowAfterConnect()
        {
            if (connected)
            {
                WorkflowSelector = new WorkflowSelector(session);
                this.Opacity = 100;
                //GetTypes();
                SetMenuImages();
                ShowMenu();
                InitRevisionComboBox();
                this.Show();
            }
            else
                Application.Exit();
        }

        private void InitRevisionComboBox()
        {
            currentRevisionToolStripComboBox.Items.Clear();
            //RemoteDataService.Instance.CurrentRevision = RemoteDataService.Instance.GetHeadRevision();
            //currentRevisionToolStripComboBox.Items.Add(RemoteDataService.Instance.CurrentRevision);
            var revisions = session.Revisions;

            foreach (var revision in revisions)
            {
                currentRevisionToolStripComboBox.Items.Add(revision);
            }
            currentRevisionToolStripComboBox.SelectedItem = strucProvider.CurrentRevision;
        }

        private void currentRevisionToolStripComboBox_DropDown(object sender, EventArgs e)
        {
            InitRevisionComboBox();
            //var revisions = RemoteDataService.Instance.GetRevisions();
            //var revisions = session.Revisions;

            //currentRevisionToolStripComboBox.Items.Clear();

            //foreach (var revision in revisions)
            //{
            //    currentRevisionToolStripComboBox.Items.Add(revision);
            //}

            //currentRevisionToolStripComboBox.SelectedItem = RemoteDataService.Instance.CurrentRevision;
        }

        private void currentRevisionToolStripComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var revision = currentRevisionToolStripComboBox.SelectedItem as RevisionInfo;

            if (revision != null)
                session.CurrentRevision = revision;
        }

        void remoteDataService_UnitTypeChanged(object sender, UnitTypeEventArgs e)
        {
            LoadImageList();
        }

        private void SetStatusLabel()
        {
            toolStripStatusLabel1.Text = "  Пользователь:    " + session.User.Text + "                " +
                Program.ExtensionManager.StatusString; ; //"Установлено соединение: " + url;            
        }

        private void Disconnect()
        {
            strucProvider = null;
            var task = Task.Factory.StartNew(() => CloseAllChildForms());
            task.ContinueWith(t => session.Disconnect());
            //remoteDataService.Disconnect();
        }

        private void SetMenuImages()
        {
            if (Icons != null)
            {
                string key;
                key = ((int)UnitTypeId.Graph).ToString();
                if (Icons.Images.ContainsKey(key))
                {
                    graphsToolStripMenuItem.Image = Icons.Images[key];
                    tsbGraphs.Image = Icons.Images[key];
                }
                else
                {
                    key = ((int)UnitTypeId.Histogram).ToString();
                    if (Icons.Images.ContainsKey(key))
                    {
                        graphsToolStripMenuItem.Image = Icons.Images[key];
                        tsbGraphs.Image = Icons.Images[key];
                    }
                }
                key = ((int)UnitTypeId.Schema).ToString();
                if (Icons.Images.ContainsKey(key))
                {
                    schemasToolStripMenuItem.Image = Icons.Images[key];
                    tsbSchemas.Image = Icons.Images[key];
                }
#if EMA
                key = ((int)UnitTypeId.Report).ToString(); 
#else
                key = ((int)UnitTypeId.ExcelReport).ToString();
#endif
                if (Icons.Images.ContainsKey(key))
                {
                    reportsToolStripMenuItem.Image = Icons.Images[key];
                    tsbReports.Image = Icons.Images[key];
                }
                key = ((int)UnitTypeId.MonitorTable).ToString();
                if (Icons.Images.ContainsKey(key))
                {
                    monitorsToolStripMenuItem.Image = Icons.Images[key];
                    tsbMonitors.Image = Icons.Images[key];
                }
                key = ((int)UnitTypeId.ManualGate).ToString();
                if (Icons.Images.ContainsKey(key))
                {
                    manualsToolStripMenuItem.Image = Icons.Images[key];
                    tsbManuals.Image = Icons.Images[key];
                }
                key = ((int)UnitTypeId.TEPTemplate).ToString();
                if (Icons.Images.ContainsKey(key))
                {
                    calcsToolStripMenuItem.Image = Icons.Images[key];
                    tsbCalcs.Image = Icons.Images[key];
                }
                key = ((int)UnitTypeId.NormFunc).ToString();
                if (Icons.Images.ContainsKey(key))
                {
                    normToolStripMenuItem.Image = Icons.Images[key];
                    tsbNorm.Image = Icons.Images[key];
                }
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!connected || MessageBox.Show("Текущее соединение будет прервано. Продолжить?", "Подключение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Reconnect();
            }
        }

        private void ShowMenu()
        {
//#if EMA
//            const bool emastyle=true;
//#else
//            const bool emastyle = false;
//#endif
            toolStrip1.Enabled = connected;

            // Редактирование пользователей и типов
            editTypesToolStripMenuItem.Visible = connected && session.User.IsAdmin;
            editUsersToolStripMenuItem.Visible = connected && session.User.IsAdmin;
            toolStripMenuItem2.Visible = connected && session.User.IsAdmin;
            intervalsToolStripMenuItem.Visible = connected && session.User.IsAdmin;
#if DEBUG
            diagnosticsToolStripMenuItem.Visible = connected && session.User.IsAdmin; 
#else
            diagnosticsToolStripMenuItem.Visible = false;
#endif
            // Экспорт/Импорт
            exportToolStripMenuItem.Visible = connected;
            importToolStripMenuItem.Visible = connected;
            toolStripMenuItem3.Visible = connected;

            // Пункты главного меню
            viewToolStripMenuItem.Visible = connected;
            calcToolStripMenuItem.Visible = connected;
            editToolStripMenuItem.Visible = connected;
            viewItemsToolStripMenuItem.Visible = connected;

            // Видимость/невидимость редактирования расписаний
#if DEBUG
            toolStripMenuItem5.Visible = /*emastyle &&*/ connected;
            scheduleToolStripMenuItem.Visible = /*emastyle &&*/ connected;
#else
            toolStripMenuItem5.Visible = false;
            scheduleToolStripMenuItem.Visible = false;
#endif
            bool showStructure = connected && !session.User.StructureHide;
            tsbStructure.Visible = toolStripSeparator1.Visible =
                structureToolStripMenuItem.Visible = /*toolStripMenuItem5.Visible =*/ showStructure;

            // Точка вхождения расширения
            ExtensionMenus();

            IEnumerable<UnitTypeId> typeIds = (IEnumerable<UnitTypeId>)(new List<UnitTypeId>());
            if (session.Types != null)
                typeIds = from t in session.Types select (UnitTypeId)t.Idnum;

            // Видимость по типам элементов
            graphsToolStripMenuItem.Visible =
                tsbGraphs.Visible = typeIds != null &&
                (typeIds.Contains(UnitTypeId.Graph) || typeIds.Contains(UnitTypeId.Histogram));
            schemasToolStripMenuItem.Visible =
                tsbSchemas.Visible = typeIds != null && typeIds.Contains(UnitTypeId.Schema);
            monitorsToolStripMenuItem.Visible =
                tsbMonitors.Visible = typeIds != null && typeIds.Contains(UnitTypeId.MonitorTable);
            manualsToolStripMenuItem.Visible =
                tsbManuals.Visible = typeIds != null && typeIds.Contains(UnitTypeId.ManualGate);
            reportsToolStripMenuItem.Visible =
                tsbReports.Visible = typeIds != null && (typeIds.Contains(UnitTypeId.ExcelReport) || typeIds.Contains(UnitTypeId.Report));
            createdReportsToolStripMenuItem.Visible =
                typeIds != null && typeIds.Contains(UnitTypeId.Report);
            calcsToolStripMenuItem.Visible =
                tsbCalcs.Visible = typeIds != null && typeIds.Contains(UnitTypeId.TEPTemplate);
            normToolStripMenuItem.Visible =
                tsbNorm.Visible = typeIds != null && typeIds.Contains(UnitTypeId.NormFunc);
        }

        private void ExtensionMenus()
        {
            Program.ExtensionManager.StatusStringChanged += new EventHandler(ExtensionManager_StatusStringChanged);
            IISTOKMenuItem[] extensionMenuItems = Program.ExtensionManager.GetMainMenuExt();

            foreach (IISTOKMenuItem item in extensionMenuItems)
            {
                ToolStripMenuItem parentMenu = null;
                ToolStripMenuItem selfMenuItem;
                foreach (ToolStripMenuItem menuItem in menuStrip1.Items)
                {
                    if (menuItem.Text.Equals(item.UberMenu))
                    {
                        parentMenu = menuItem;
                        break;
                    }
                }
                if (parentMenu == null)
                    menuStrip1.Items.Add(parentMenu = new ToolStripMenuItem(item.UberMenu));

                selfMenuItem = new ToolStripMenuItem();
                selfMenuItem.Name = item.Name;
                selfMenuItem.Text = item.Name;
                selfMenuItem.ToolTipText = item.ToolTip;
                selfMenuItem.Tag = item;
                selfMenuItem.Click += new EventHandler(selfMenuItem_Click);
                parentMenu.DropDownItems.Add(selfMenuItem);
            }

            foreach (ToolStripMenuItem item in menuStrip1.Items)
            {
                item.DropDownOpening += new EventHandler(item_DropDownOpening);
            }
        }

        async void item_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripItem, menu = sender as ToolStripMenuItem;
            foreach (var item in menu.DropDownItems)
            {
                if ((toolStripItem = item as ToolStripMenuItem) != null)
                {
                    try
                    {
                        IISTOKMenuItem extensionMenuItem = toolStripItem.Tag as IISTOKMenuItem;
                        if (extensionMenuItem != null)
                        {
                            toolStripItem.Enabled = await extensionMenuItem.GetEnabled();
                            toolStripItem.Checked = extensionMenuItem.Checked;
                        }
                    }
                    catch
                    {
                        toolStripItem.Enabled = false;
                        toolStripItem.Checked = false;
                    }
                }
            }
        }

        void selfMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ((sender as ToolStripMenuItem).Tag as IISTOKMenuItem).Click();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Сбой расширения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void ExtensionManager_StatusStringChanged(object sender, EventArgs e)
        {
            SetStatusLabel();
        }

        private void notImplementedYetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В настоящий момент идет разработка данного функционала. \nПовторите данное действие через несколько дней. Возможно, оно уже будет реализованно", "Not Implemented Yet", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //temp, test
        }

        private void editTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (session.User.IsAdmin)
            {
                UnitTypeFindForm unitTypeFindForm = new UnitTypeFindForm(strucProvider);
                unitTypeFindForm.MdiParent = this;
                unitTypeFindForm.FormClosed += new FormClosedEventHandler(unitTypeFindForm_FormClosed);
                unitTypeFindForm.Show();
            }
        }

        void unitTypeFindForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //types = null;
            //typeIds = null;
        }

        private void editUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (session.User.IsAdmin)
            {
                UserFindForm userFindForm = new UserFindForm(strucProvider);
                userFindForm.MdiParent = this;
                userFindForm.Show();
            }
        }

        #region Открытие ЮниФормы с разными фильтрами
        private void OpenUniForm(TypeFilter filter)
        {
            OpenUniForm(filter, "");
        }
        private void OpenUniForm(TypeFilter filter, string caption)
        {
            OpenUniForm(FilterParams.All, filter, caption, null);
        }
        private UniForm OpenUniForm(FilterParams filterParams, TypeFilter filter, string caption, Image img)
        {
            try
            {
                UniForm frm = null;

                if (dicForms.ContainsKey(caption))
                {
                    frm = dicForms[caption];
                    if (frm.IsDisposed || !frm.Visible)
                        frm = null;
                }
                if (frm == null)
                {
                    var provider = session.GetStructureProvider();
                    provider.FilterParams = filterParams;

                    frm = new UniForm(provider, icons);
                    frm.Filter = filter;
                    if (img != null) frm.Icon = System.Drawing.Icon.FromHandle(((Bitmap)img).GetHicon());
                    if (!string.IsNullOrEmpty(caption)) frm.Text = caption;
                    frm.MdiParent = this;
                    dicForms[caption] = frm;
                    frm.Show();
                }
                frm.Activate();
                return frm;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            return null;
        }

        /// <summary>
        /// Создать униформу по типу требуемому для отображения
        /// </summary>
        /// <param name="unitType"></param>
        /// <returns></returns>
        private UniForm OpenUniForm(FilterParams filterParams)//int unitType)
        {
            TypeFilter filter = new TypeFilter();
            String caption;
            Image icon;

            switch (filterParams)// (UnitTypeId)unitType)
            {
                case FilterParams.Schemas:
                    //case UnitTypeId.Schema:
                    //filter.Add((int)unitType);
                    filter.Add((int)UnitTypeId.Schema);

                    caption = "Мнемосхемы";
                    icon = tsbSchemas.Image;
                    break;
                //case UnitTypeId.MonitorTable:
                //case FilterParams.MonitorTables:
                //    filter.Add((int)UnitTypeId.MonitorTable);

                //    caption = "Мониторинг";
                //    icon = tsbMonitors.Image;
                //    break;
                case FilterParams.Graphs:
                    //case UnitTypeId.Graph:
                    //case UnitTypeId.Histogram:

                    filter.Add((int)UnitTypeId.Graph);
                    filter.Add((int)UnitTypeId.Histogram);

                    caption = "Графики";
                    icon = tsbGraphs.Image;
                    break;
                case FilterParams.ManualParameters:
                    //case UnitTypeId.ManualGate:
                    //case UnitTypeId.ManualParameter:
                    filter.Add((int)UnitTypeId.ManualGate);

                    caption = "Ручной ввод";
                    icon = tsbManuals.Image;
                    break;
                case FilterParams.TepParameters:
                    //case UnitTypeId.TEPTemplate:
                    //case UnitTypeId.OptimizeCalc:
                    //case UnitTypeId.TEP:
                    filter.Add((int)UnitTypeId.TEPTemplate);
                    filter.Add((int)UnitTypeId.OptimizeCalc);

                    caption = UniForm.calcFormCaption;
                    icon = tsbCalcs.Image;
                    break;
                case FilterParams.NormFunctions:
                    //case UnitTypeId.NormFunc:
                    filter.Add((int)UnitTypeId.NormFunc);

                    caption = tsbNorm.Text;
                    icon = tsbNorm.Image;
                    break;
                //case UnitTypeId.Report:
                //case UnitTypeId.ExcelReport:
                case FilterParams.Reports:
                    filter.Add((int)UnitTypeId.ExcelReport);
                    filter.Add((int)UnitTypeId.Report);

                    caption = "Отчеты";
                    icon = tsbReports.Image;
                    break;
                default:
                    caption = "";
                    icon = tsbStructure.Image;
                    break;
            }
            return OpenUniForm(filterParams, filter, caption, icon);
        }
        private void tsbGraphs_Click(object sender, EventArgs e)
        {
            //OpenUniForm((int)UnitTypeId.Graph);
            OpenUniForm(FilterParams.Graphs);
        }
        private void tsbSchemas_Click(object sender, EventArgs e)
        {
            //OpenUniForm((int)UnitTypeId.Schema);
            OpenUniForm(FilterParams.Schemas);
        }
        private void tsbMonitors_Click(object sender, EventArgs e)
        {
            //OpenUniForm((int)UnitTypeId.MonitorTable);
            //OpenUniForm(FilterParams.Monitors);
        }
        private void tsbNorm_Click(object sender, EventArgs e)
        {
            //OpenUniForm((int)UnitTypeId.NormFunc);
            OpenUniForm(FilterParams.NormFunctions);
        }
        private void tsbManuals_Click(object sender, EventArgs e)
        {
            //OpenUniForm((int)UnitTypeId.ManualGate);
            OpenUniForm(FilterParams.ManualParameters);
        }
        private void tsbCalcs_Click(object sender, EventArgs e)
        {
            //OpenUniForm((int)UnitTypeId.TEPTemplate);
            OpenUniForm(FilterParams.TepParameters);
        }
        private void tsbReports_Click(object sender, EventArgs e)
        {
            //OpenUniForm((int)UnitTypeId.Report);
            OpenUniForm(FilterParams.Reports);
        }
        #endregion

        private void OpenScheduleForm()
        {
            try
            {
                ParamsSheduleEditorForm schedule_edit_form = new ParamsSheduleEditorForm(strucProvider);
                schedule_edit_form.MdiParent = this;
                schedule_edit_form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void OpenDiagnosticsForm()
        {
            try
            {
                if (frmDiag == null || frmDiag.IsDisposed)
                {
                    frmDiag = new DiagnosticsForm(strucProvider);
                    frmDiag.Show();
                }
                else
                    frmDiag.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void tsbStructure_Click(object sender, EventArgs e)
        {
            OpenUniForm(FilterParams.All, new TypeFilter(), "", tsbStructure.Image);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.Cancel)
                return;
            if (e.CloseReason == CloseReason.UserClosing && MessageBox.Show("Вы уверены?",
                "Завершение работы", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                e.Cancel = true;
                foreach (var it in dicForms.Values)
                    it.SetFormClosingFlag(false);
                return;
            }
            try
            {
                Disconnect();
            }
            catch { }
        }

        private async void calcAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AsyncOperationWatcher<Object> watcher = 
            var nodes = await Task.Factory.StartNew(() => strucProvider.GetUnitNodesFiltered(int.MinValue));
            //List<UnitNode> nodes = new List<UnitNode>();
            //watcher.AddValueRecivedHandler(x => nodes.AddRange((UnitNode[])x));
            //watcher.AddFinishHandler(() =>
            //{
            //    CalcForm calcForm = new CalcForm(remoteDataService, icons);
            //    AddExtendForm(calcForm);
            //    calcForm.UnitNodes = nodes.ToArray();
            //    if (InvokeRequired)
            //        Invoke((Action<Form>)calcForm.Show, this);
            //});
            //watcher.Run();
            CalcForm calcForm = new CalcForm(strucProvider, icons);
            AddExtendForm(calcForm);
            calcForm.UnitNodes = nodes;
            calcForm.Show(this);
        }

        private void roundRobinCalcToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            try
            {
                bool canStartnStopRoundRobin = session.User.IsAdmin
                        || session.User.CheckPrivileges((int)UnitTypeId.TEP, Privileges.Write | Privileges.Execute);
                bool isRoundRobinStarted = false;

                if (canStartnStopRoundRobin) isRoundRobinStarted = session.IsRoundRobinStarted();
                roundRobinStartToolStripMenuItem.Enabled = canStartnStopRoundRobin &&
                    !isRoundRobinStarted;
                roundRobinStopToolStripMenuItem.Enabled = canStartnStopRoundRobin &&
                    isRoundRobinStarted;
            }
            catch (Exception exc)
            {
                log.ErrorException("Ошибка просмотра состояния автоматического расчета.", exc);
                ShowError(exc);
            }
        }

        private void roundRobinStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            session.StartRoundRobinCalc();
        }

        private void roundRobinStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            session.StopRoundRobinCalc();
        }

        private void roundRobinConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoundRobinCalcSettingsForm roundRobinForm = new RoundRobinCalcSettingsForm(strucProvider);

            roundRobinForm.MdiParent = this;
            roundRobinForm.Show();
        }

        private void toolStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip1.Visible = toolStripToolStripMenuItem.Checked = !toolStripToolStripMenuItem.Checked;
        }

        private void statusStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip1.Visible = statusStripToolStripMenuItem.Checked = !statusStripToolStripMenuItem.Checked;
        }

        private void closeAllWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportImportForm form = new ExportImportForm(ExportImportMode.Export, strucProvider);
            form.MdiParent = this;
            form.Show();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportImportForm form = new ExportImportForm(ExportImportMode.Import, strucProvider);
            form.MdiParent = this;
            form.Show();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClientSettings.Instance.SaveFormState(this);
            ClientSettings.Instance.Save();//ClientSettings.Instance.DefaultConfigFile);
        }

        private void scheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenScheduleForm();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void constsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConstantEditForm constantForm = new ConstantEditForm(strucProvider);
            constantForm.Show();
        }

        private void createdReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreatedReportsForm f = new CreatedReportsForm(strucProvider);
            f.MdiParent = this;
            f.Show();
        }

        private void tsbShowTree_Click(object sender, EventArgs e)
        {
            try
            {
                UniForm frm = this.ActiveMdiChild as UniForm;
                if (frm != null)
                    frm.IsTreeShown = !tsbShowTree.Checked;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            try
            {
                UniForm frm = this.ActiveMdiChild as UniForm;
                tsbShowTree.Enabled = frm != null;
                if (frm != null)
                    tsbShowTree.Checked = !frm.IsTreeShown;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void diagnosticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDiagnosticsForm();
        }

        private void LogsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        public void EditParam(ParameterNode node)
        {
            UniForm form = OpenUniForm(FilterParams.All, new TypeFilter(), "", tsbStructure.Image);
            if (form != null)
            {
                form.SelectUnitNode(node, false);
            }
        }

        public void ShowParam(ParameterNode node)
        {
            FilterParams filter;
            switch ((UnitTypeId)node.Typ)
            {
                case UnitTypeId.OptimizeCalc:
                case UnitTypeId.TEPTemplate:
                case UnitTypeId.TEP:
                    filter = FilterParams.TepParameters;
                    break;
                case UnitTypeId.Histogram:
                case UnitTypeId.Graph:
                    filter = FilterParams.Graphs;
                    break;
                case UnitTypeId.MonitorTable:
                case UnitTypeId.Schema:
                    filter = FilterParams.Schemas;
                    break;
                case UnitTypeId.ManualGate:
                case UnitTypeId.ManualParameter:
                    filter = FilterParams.ManualParameters;
                    break;
                case UnitTypeId.ExcelReport:
                case UnitTypeId.Report:
                    filter = FilterParams.Reports;
                    break;
                case UnitTypeId.NormFunc:
                    filter = FilterParams.NormFunctions;
                    break;
                default:
                    filter = FilterParams.All;
                    break;
            }

            UniForm form = OpenUniForm(filter);
            if (form != null)
            {
                form.SelectUnitNode(node, true);
            }
        }

        private void revisionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RevisionEditForm revisionForms = new RevisionEditForm(strucProvider);

            revisionForms.ShowDialog();
        }

        private void viewAuditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var auditForm = new ViewAuditForm(session);
            //auditForm.Entries = new List<ASC.Audit.AuditEntry>(remoteDataService.GetAllAudit());

            auditForm.MdiParent = this;
            auditForm.Show();
        }

        private void intervalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var intervalForm = new IntervalEdit(session);

            intervalForm.MdiParent = this;
            intervalForm.Show();
        }

        volatile bool showUserNotConnected;

        public void ShowError(Exception exc)
        {
            if (InvokeRequired) BeginInvoke((Action<Exception>)ShowError, exc);
            else
            {
                if (exc is UserNotConnectedException)
                {
                    if (!showUserNotConnected)
                    {
                        if (reconnecting)
                            return;

                        showUserNotConnected = true;
                        String caption = "Переподключение к серверу";
                        String message;

                        if (exc.InnerException != null)
                        {
                            message = "Нет связи с сервером.";
                        }
                        else
                        {
                            message = "Пользователь был отключен от системы.";
                        }

                        MessageBox.Show(message + "\nТребуется переподключение", caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Reconnect();
                        showUserNotConnected = false;
                    }
                }
                else
                {

                    String mess = exc.Message;
#if DEBUG
                    mess += String.Format("\n{0}", exc.StackTrace);
#endif
                    MessageBox.Show(mess,
                                       "Error",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Error);
                }
            }
        }
    }
}

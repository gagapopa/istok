//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using COTES.ISTOK.ASC;
//using COTES.ISTOK.Client;
//using COTES.ISTOK.Client.Extension;
//using COTES.ISTOK.Extension;
//using COTES.ISTOK.ClientCore;
//using System.Threading.Tasks;

//namespace COTES.ISTOK.EMA
//{
//    /// <summary>
//    /// Расширение библиотеки обмена в интерфейсном модуле.
//    /// Добавляет окно "Параметры", панель пусков котлов 
//    /// и выводит информацию в строке состояния
//    /// </summary>
//    public class ClientEMAExtension : IClientExtension
//    {
//        StructureProvider strucProvider;
//        /// <summary>
//        /// Тип котла
//        /// </summary>
//        public int BoilerTypeID
//        {
//            get
//            {
//                UTypeNode typeNode = State.GetExtensionType(BoilerTypeGUID);
//                if (typeNode != null)

//                    return typeNode.Idnum;
//                return (int)UnitTypeId.Unknown;
//            }
//        }

//        public ClientEMAExtension(StructureProvider strucProvider)
//        {
//            this.strucProvider = strucProvider;
//        }

//        #region IClientExtension Members

//        IClientState state;
//        public IClientState State
//        {
//            get { return state; }
//            set
//            {
//                if (state != null)
//                    state.ActiveUniFormChanged -= new EventHandler(state_ActiveUniFormChanged);

//                state = value;

//                if (state != null)
//                    state.ActiveUniFormChanged += new EventHandler(state_ActiveUniFormChanged);
//            }
//        }

//        IUniForm currentUniForm;
//        void state_ActiveUniFormChanged(object sender, EventArgs e)
//        {
//            if (currentUniForm != null)
//                currentUniForm.StructureTree.AfterSelect -= new TreeViewEventHandler(uniFormStructureTree_AfterSelect);

//            currentUniForm = State.ActiveUniForm;

//            if (currentUniForm != null)
//            {
//                currentUniForm.StructureTree.AfterSelect += new TreeViewEventHandler(uniFormStructureTree_AfterSelect);

//                UploadStatusString(currentUniForm.StructureTree.SelectedNode);
//            }
//            else
//                OnStatusStringChanged(String.Empty);
//        }

//        void uniFormStructureTree_AfterSelect(object sender, TreeViewEventArgs e)
//        {
//            UploadStatusString(e.Node);
//        }

//        static readonly Guid BoilerTypeGUID = new Guid("{60A7DA28-684D-435c-9911-625310B2E045}");
//        const String StartStop = "GetLifeStartStopCurrent";
//        private async void UploadStatusString(TreeNode treeNode)
//        {
//            UnitNode unitNode = State.ActiveUniForm.GetUnitNode(treeNode);
//            if (unitNode != null)
//            {
//                UTypeNode typeNode = State.GetExtensionType(BoilerTypeGUID);
//                if (typeNode != null)
//                {
//                    int typeID = typeNode.Idnum;
//                    ExtensionUnitNode boilerUnitNode = await State.GetParent(unitNode, typeID) as ExtensionUnitNode;
//                    if (boilerUnitNode != null)
//                    {
//                        //AsyncOperationWatcher<ExtensionData> watcher = 
//                        var table = State.GetExtensionExtendedTable(boilerUnitNode, StartStop);
//                        //watcher.AddValueRecivedHandler(extendedTableReceive);
//                        //watcher.Run();
//                        extendedTableReceive(table);
//                    }
//                }
//            }
//        }

//        private void extendedTableReceive(ExtensionData extendedTable)
//        {
//        }

//        protected void OnStatusStringChanged(String status)
//        {
//            statusString = status;

//            if (StatusStringChanged != null)
//                StatusStringChanged(this, EventArgs.Empty);
//        }

//        private String statusString;
//        public string StatusString
//        {
//            get { return statusString; }
//        }

//        public event EventHandler StatusStringChanged;

//        public IISTOKMenuItem[] MainMenuExt()
//        {
//            return new IISTOKMenuItem[] {
//                new BoilerPannelEMAMenuItem(this),
//                new ParametersFormEMAMenuItem(this),
//            };
//        }

//        //public IISTOKMenuItem[] ContextMenuExt(UniForm uniForm)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        #endregion

//        Dictionary<IUniForm, BoilerStartControl> boilerStartPanelDictionary = new Dictionary<IUniForm, BoilerStartControl>();
//        public BoilerStartControl GetPanel(IUniForm uniForm)
//        {
//            BoilerStartControl panel = null;

//            if (uniForm != null)
//                boilerStartPanelDictionary.TryGetValue(uniForm, out panel);
//            return panel;
//        }

//        public void SetPanel(IUniForm uniForm, BoilerStartControl boilerPanel)
//        {
//            boilerStartPanelDictionary[uniForm] = boilerPanel;
//        }

//        ParametersForm parametersForm;
//        public void ShowParametersForm()
//        {
//            if (parametersForm == null)
//            {
//                parametersForm = new ParametersForm(strucProvider, this);

//                parametersForm.FormClosed += new FormClosedEventHandler((s, e) => parametersForm = null);

//                State.ShowMdi(parametersForm);
//            }
//            else parametersForm.Activate();
//        }
//    }

//    /// <summary>
//    /// Пункт меню "Панель пуска котла".
//    /// Отображает панель в текущей Униформе
//    /// </summary>
//    public class BoilerPannelEMAMenuItem : IISTOKMenuItem
//    {
//        private ClientEMAExtension clentExtension;
//        public BoilerPannelEMAMenuItem(ClientEMAExtension clentExtension)
//        {
//            this.clentExtension = clentExtension;
//        }

//        #region ISTOKMenuItem Members

//        public string Name
//        {
//            get { return "Количество пусков"; }
//        }

//        public string ToolTip
//        {
//            get { return "Количество пусков"; }
//        }

//        public string UberMenu
//        {
//            get { return "Вид"; }
//        }

//        public async Task<bool> GetEnabled()
//        {
//            IUniForm uniForm = clentExtension.State.ActiveUniForm;
//            if (uniForm != null)
//            {
//                TreeNode treeNode = uniForm.StructureTree.SelectedNode;
//                UnitNode unitNode = uniForm.GetUnitNode(treeNode);
//                if (unitNode != null)
//                {
//                    ExtensionUnitNode boilerUnitNode = await clentExtension.State.GetParent(unitNode, clentExtension.BoilerTypeID) as ExtensionUnitNode;
//                    return boilerUnitNode != null;
//                }
//            }
//            return false;
//        }

//        public bool Checked
//        {
//            get
//            {
//                BoilerStartControl boilerPanel = clentExtension.GetPanel(clentExtension.State.ActiveUniForm);
//                if (boilerPanel != null)
//                    return boilerPanel.PanelShown();
//                return false;
//            }
//        }

//        public void Click()
//        {
//            IUniForm uniForm = clentExtension.State.ActiveUniForm;

//            BoilerStartControl boilerPanel = clentExtension.GetPanel(uniForm);
//            if (boilerPanel == null)
//            {
//                SplitContainer splitContainer = new SplitContainer();
//                splitContainer.Orientation = Orientation.Horizontal;
//                splitContainer.Dock = DockStyle.Fill;

//                Control parentControl = uniForm.StructurePanel.Parent;
//                parentControl.Controls.Remove(uniForm.StructurePanel);
//                parentControl.Controls.Add(splitContainer);

//                splitContainer.Panel1.Controls.Add(uniForm.StructurePanel);
//                splitContainer.FixedPanel = FixedPanel.Panel2;
//                splitContainer.IsSplitterFixed = true;

//                boilerPanel = new BoilerStartControl(clentExtension, uniForm);
//                //boilerPanel.BoilerSplitContainer = splitContainer;
//                splitContainer.Panel2.Controls.Add(boilerPanel);
//                boilerPanel.Dock = DockStyle.Fill;
//                splitContainer.SplitterDistance = splitContainer.Height - boilerPanel.RecomendedHeight;

//                boilerPanel.ShowPanel = () => splitContainer.Panel2Collapsed = false;
//                boilerPanel.HidePanel = () => splitContainer.Panel2Collapsed = true;
//                boilerPanel.PanelShown = () => !splitContainer.Panel2Collapsed;

//                clentExtension.SetPanel(uniForm, boilerPanel);
//            }
//            else if (boilerPanel.PanelShown())
//                boilerPanel.HidePanel();
//            else
//                boilerPanel.ShowPanel();

//            //throw new NotImplementedException();
//        }

//        #endregion
//    }

//    /// <summary>
//    /// Пункт меню "Параметры". Отображает окно "Параметры"
//    /// </summary>
//    public class ParametersFormEMAMenuItem : IISTOKMenuItem
//    {
//        private ClientEMAExtension clientExtension;
//        public ParametersFormEMAMenuItem(ClientEMAExtension clientExtension)
//        {
//            this.clientExtension = clientExtension;
//        }

//        #region ISTOKMenuItem Members

//        public string Name
//        {
//            get { return "Параметры"; }
//        }

//        public string ToolTip
//        {
//            get { return "Параметры"; }
//        }

//        public string UberMenu
//        {
//            get { return "Ресурс"; }
//        }

//        public bool Enabled
//        {
//            get { return true; }
//        }

//        public bool Checked
//        {
//            get { return false; }
//        }

//        public void Click()
//        {
//            clientExtension.ShowParametersForm();
//        }

//        #endregion
//    }
//}

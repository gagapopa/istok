using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    public partial class TreeForm : COTES.ISTOK.Client.BaseAsyncWorkForm
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        
        public TreeForm()
            : base()
        {
            Init();
        }
        public TreeForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            Init();
        }

        private void Init()
        {
            InitializeComponent();
            Filter = new TypeFilter();
        }

        /// <summary>
        /// Костыль запрещающий автоматом подгружать дочерние узлы на expand
        /// </summary>
        public bool Suspended { get; set; }

        public TypeFilter Filter { get; set; }
        public bool MultiSelect
        {
            //get { return treeViewUnitObjects.MultiSelect; }
            //set { treeViewUnitObjects.MultiSelect = value; }
            get { return false; }
            set { }
        }

        private void treeViewUnitObjects_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (Suspended)
                return;
            try
            {
                LoadUnitNodes(e.Node.Nodes, e.Node as UnitTreeNode);
            }
            catch (Exception exp)
            {
                e.Cancel = true;

                log.WarnException("Ошибка загрузки структуры элементов.", exp);
                ShowError(exp);
            }
        }

        /// <summary>
        /// Сюда запинается состояние дерева при его обновлении
        /// </summary>
        Dictionary<int, HashSet<int>> curentExpanding = new Dictionary<int, HashSet<int>>();

        /// <summary>
        /// Производит запуск асинхронной операции,
        /// создает смотрителя за ней и подключает
        /// обработчики.
        /// </summary>
        /// <param name="nodes">
        ///     Собственно куда добавляем. Коллекция дочерних
        ///     нодов узла.
        /// </param>
        /// <param name="parent">
        ///     Собственно родитель.
        /// </param>
        internal void LoadUnitNodes(TreeNodeCollection nodes, UnitTreeNode parent)
        {
            if (parent != null && parent.ChildLoaded)
                return;

            HashSet<int> expandedNodes;
            int parentID = GetUnitNodeID(parent);

            expandedNodes = PrepareTreeViewForLoad(nodes, parentID);
            //TreeNode selectedTreeNode = treeViewUnitObjects.SelectedNode;

            //var watcher =
            //    RemoteDataService.Instance.QueryUnitNodes(parent == null ?
            //                                                         int.MinValue :
            //                                                         (int)parent.Tag,
            //                                              Filter == null ?
            //                                                         new UnitTypeId[] { } :
            //                                                         Filter.GetTypes());

            strucProvider.FilteredTypes = Filter == null ? new int[] { } : Filter.GetTypes();
            UnitNode[] fnodes = null;
            try
            {
                fnodes = strucProvider.GetUnitNodesFiltered(parent == null ? int.MinValue : (int)parent.Tag);
                //fnodes = Task.Factory.StartNew(() => strucProvider.GetUnitNodesFiltered(parent == null ? int.MinValue : (int)parent.Tag)).Result;
            }
            catch (Exception) 
            {
                //TODO: надо изучить природу появления исключения здесь (при тасках оно появлялось, без них - пока не замечено)
                throw;
            }
            
            Action<TreeNodeCollection, UnitNode[]> filler =
                new Action<TreeNodeCollection, UnitNode[]>(this.FillTreeLevel);
            this.treeViewUnitObjects.Invoke(filler, nodes, fnodes);

            //watcher.AddValueRecivedHandler((object x) =>
            //{
            //    Action<TreeNodeCollection, UnitNode[]> filler =
            //        new Action<TreeNodeCollection, UnitNode[]>(this.FillTreeLevel);

            //    this.treeViewUnitObjects.Invoke(filler, nodes, x as UnitNode[]);
            //});

            //watcher.AddFinishHandler(() =>
            //{
            //    if (parent != null)
            //        parent.ChildLoaded = true;
            //    //SetTreeState(parentID, nodes, expandedNodes, selectedTreeNode);
            //    Invoke((Action<int, TreeNodeCollection, HashSet<int>>)SetTreeState, parentID, nodes, expandedNodes);
            //});

            if (parent != null) parent.ChildLoaded = true;
            Invoke((Action<int, TreeNodeCollection, HashSet<int>>)SetTreeState, parentID, nodes, expandedNodes);

            //watcher.Run();
            //return null;
        }

        private HashSet<int> PrepareTreeViewForLoad(TreeNodeCollection nodes, int parentID)
        {
            if (InvokeRequired)
            {
                return (HashSet<int>)Invoke((Func<TreeNodeCollection, int, HashSet<int>>)PrepareTreeViewForLoad, nodes, parentID);
            }
            else
            {
                HashSet<int> expandedNodes;
                if (!curentExpanding.TryGetValue(parentID, out expandedNodes))
                {
                    curentExpanding[parentID] = expandedNodes = GetTreeState(nodes);
                }

                // Если текущий выделенный объект внутри подгружаемой зоны, выделить родительский
                TreeNode treeNode = treeViewUnitObjects.SelectedNode;

                while (treeNode != null)
                {
                    if (nodes.Contains(treeNode))
                        break;
                    treeNode = treeNode.Parent;
                }
                if (treeNode != null && treeViewUnitObjects.SelectedNode != treeNode.Parent)
                    treeViewUnitObjects.SelectedNode = treeNode.Parent;

                nodes.Clear();
                AddDefaultTreeNode(nodes);

                //treeViewUnitObjects.SuspendLayout();
                return expandedNodes;
            }
        }

        private int GetUnitNodeID(UnitTreeNode parent)
        {
            return parent == null ? int.MinValue : (int)parent.Tag;
        }

        private HashSet<int> GetTreeState(TreeNodeCollection nodes)
        {
            HashSet<int> idList = new HashSet<int>();

            foreach (TreeNode treeNode in nodes)
            {
                if (treeNode.IsExpanded && treeNode.Tag != null)
                {
                    int id = (int)treeNode.Tag;

                    if (!idList.Contains(id))
                        idList.Add(id);

                    HashSet<int> tmp = GetTreeState(treeNode.Nodes);

                    idList.UnionWith(tmp);
                }
            }
            return idList;
        }

        private void SetTreeState(int parentID, TreeNodeCollection nodes, HashSet<int> expandedNodes)
        {
            //if (InvokeRequired) 
            //    Invoke((Action<int, TreeNodeCollection, HashSet<int>, TreeNode>)SetTreeState, parentID, nodes, expandedNodes, selectedTreeNode);
            //else
            //{
            try
            {
                foreach (TreeNode treeNode in nodes)
                {
                    if (treeNode.Tag != null)
                    {
                        int nodeID = (int)treeNode.Tag;
                        if (expandedNodes.Contains(nodeID))
                        {
                            curentExpanding[nodeID] = expandedNodes;
                            treeNode.Expand();
                            expandedNodes.Remove(nodeID);
                        }
                    }
                }
                //if (treeViewUnitObjects.SelectedNode != selectedTreeNode)
                //{
                //    // костыль, что бы отработал AfterSelect
                //    selectedTreeNode = treeViewUnitObjects.SelectedNode;
                //    treeViewUnitObjects.SelectedNode = null;
                //    treeViewUnitObjects.SelectedNode = selectedTreeNode;
                //}
            }
            finally
            {
                curentExpanding.Remove(parentID);
            }
            //}
        }

        /// <summary>
        /// Заполняет элемент тривиева дочерними узлами.
        /// </summary>
        /// <param name="tree_nodes">
        ///     Коллекция дочерних узлов для заполнения.
        /// </param>
        /// <param name="unit_nodes">
        ///     Коллекция идов унит нодов.
        /// </param>
        internal void FillTreeLevel(TreeNodeCollection tree_nodes,
                                   UnitNode[] units)
        {
            try
            {
                this.treeViewUnitObjects.BeginUpdate();

                tree_nodes.Clear();

                UnitTreeNode temp = null;
                foreach (UnitNode unit in units)
                {
                    temp = new UnitTreeNode()
                    {
                        Tag = unit.Idnum,
                        Node = unit,
                        Text = unit.GetNodeText(),
                        ChildLoaded = false,
                        ImageKey = ((int)unit.Typ).ToString(),
                        SelectedImageKey = ((int)unit.Typ).ToString()
                    };

                    if (unit.HasChild)
                        AddDefaultTreeNode(temp.Nodes);

                    tree_nodes.Add(temp);
                }

                if (tree_nodes == treeViewUnitObjects.Nodes && treeViewUnitObjects.Nodes.Count > 0
                    && treeViewUnitObjects.SelectedNode == null)
                    treeViewUnitObjects.SelectedNode = treeViewUnitObjects.Nodes[0];
            }
            catch (Exception exp)
            {
                log.WarnException("Ошибка загрузки структуры элементов", exp);
                ShowError(exp);
            }
            finally
            {
                this.treeViewUnitObjects.EndUpdate();
            }
        }

        private static void AddDefaultTreeNode(TreeNodeCollection nodeCollection)
        {
            const string default_node_text = "Идет загрузка...";

            nodeCollection.Add(default_node_text);
        }

        private void TreeForm_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode && !Suspended)
            {
                try
                {
                    LoadUnitNodes(treeViewUnitObjects.Nodes, null);
                }
                catch (Exception exp)
                {
                    log.WarnException("Ошибка загрузки элементов.", exp);
                    //Task.Factory.StartNew(() => ShowError(exp));
                    ShowError(exp);
                }
            }
        }

        private void treeViewUnitObjects_ItemDrag(object sender, ItemDragEventArgs e)
        {
            UnitTreeNode unitTreeNode;
            if ((unitTreeNode = e.Item as UnitTreeNode) != null)
            {
                int id = (int)unitTreeNode.Tag;
                //UnitNode node = await Task.Factory.StartNew(() => strucProvider.GetUnitNode(id));
                UnitNode node = strucProvider.GetUnitNode(id);
                if (node != null)
                {
                    unitTreeNode.Node = node;
                    DataObject dobj = new DataObject();
                    dobj.SetData(unitTreeNode);
                    dobj.SetData(Program.CreateDragDropData(unitTreeNode.Node));
                    DoDragDrop(dobj, DragDropEffects.All);
                }
            }
        }
    }

    /// <summary>
    /// Хранитель набора типов для фильтрации
    /// Вообще, задумывалось что-то более грандиознофильтрующее,
    /// но необходимость не была выявлена
    /// </summary>
    public class TypeFilter
    {
        List<int> lstTypes = new List<int>();

        public TypeFilter()
        {
            //
        }

        public bool IsEmpty { get { return lstTypes.Count == 0; } }

        /// <summary>
        /// Добавление типа в набор
        /// </summary>
        /// <param name="type"></param>
        public void Add(int type)
        {
            if (!lstTypes.Contains(type))
                lstTypes.Add(type);
        }

        /// <summary>
        /// Проверка содержания типа в наборе
        /// </summary>
        /// <param name="type"></param>
        public bool Contains(int type)
        {
            return lstTypes.Contains(type);
        }

        /// <summary>
        /// Удаление типа из набора
        /// </summary>
        /// <param name="type"></param>
        public void Remove(int type)
        {
            lstTypes.Remove(type);
        }

        /// <summary>
        /// Очистка набора типов
        /// </summary>
        public void Clear()
        {
            lstTypes.Clear();
        }

        /// <summary>
        /// Проверка удовлетворения типа фильтру
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool Check(int type)
        {
            return lstTypes.Contains(type);
        }

        /// <summary>
        /// Возвращает массив фильтруемых типов
        /// </summary>
        /// <returns></returns>
        public int[] GetTypes()
        {
            return lstTypes.ToArray();
        }
    }
}

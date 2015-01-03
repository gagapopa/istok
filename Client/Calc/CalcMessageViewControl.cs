using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.Calc;
using COTES.ISTOK.ASC;
using System.Reflection;
using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.ClientCore;
using System.Threading.Tasks;

namespace COTES.ISTOK.Client.Calc
{
    partial class CalcMessageViewControl : UserControl
    {
        [Browsable(false)]
        public StructureProvider strucProvider { get; set; }

        private const int DefaultPageCount = -1;

        private int pageCount;

        [Browsable(false)]
        public int PageCount
        {
            get { return pageCount; }
            set
            {
                pageCount = value;
                pageCountChanged();
            }
        }

        [Browsable(false)]
        public int Count { get { return displayMessage.Count; } }

        private String statusString;
        public String StatusString
        {
            get { return statusString; }
            set
            {
                statusString = value;
                SetCalcStatusLabel();
            }
        }

        [Browsable(false)]
        public Message CurrentMessage
        {
            get
            {
                DataGridViewRow gridRow;
                MessageStorage storage;

                if ((gridRow = dgvMessages.CurrentRow) != null
                    && (storage = gridRow.DataBoundItem as MessageStorage) != null)
                    return storage.Message;
                return null;
            }
        }

        public ContextMenuStrip GridContextMenu
        {
            get { return dgvMessages.ContextMenuStrip; }
            set
            {
                //if (!value.Items.Contains(GridSortMenuItem))
                //    value.Items.Add(GridSortMenuItem);
                if (InvokeRequired) Invoke((Action)(() => dgvMessages.ContextMenuStrip = value));
                else dgvMessages.ContextMenuStrip = value;
            }
        }

        int startPos;

        //List<Message> messageList;

        //List<MessageStorage> displayMessage;
        MessagesList displayMessage;

        //ToolStripMenuItem GridSortMenuItem;

        public CalcMessageViewControl()
        {
            InitializeComponent();
            typeof(Control).InvokeMember("DoubleBuffered",
                  BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                  null, dgvMessages, new object[] { true });
            PageCount = DefaultPageCount;
            dicNodes = new Dictionary<int, UnitNode>();
            //messageList = new List<Message>();
            prevMessagesButton.Visible = nextMessagesButton.Visible = PageCount > 0;
            displayMessage = new MessagesList();//new List<MessageStorage>();
            //displayMessage = new List<MessageStorage>();
            bindingSource1.DataSource = displayMessage;
            //dgvMessages.DataSource = displayMessage;
            //GridSortMenuItem = new ToolStripMenuItem("Сортировать по");
            //ToolStripMenuItem sortMenuItem;
            //foreach (DataGridViewColumn item in dgvMessages.Columns)
            //{
            //    sortMenuItem = new ToolStripMenuItem(item.HeaderText);
            //    sortMenuItem.Tag = item;
            //    sortMenuItem.Click += new EventHandler(sortMenuItem_Click);
            //    GridSortMenuItem.DropDownItems.Add(sortMenuItem);
            //}
            //GridContextMenu = new ContextMenuStrip();
        }

        void sortMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            DataGridViewColumn column = menuItem.Tag as DataGridViewColumn;

            if ((menuItem = sender as ToolStripMenuItem) != null
                && (column = menuItem.Tag as DataGridViewColumn) != null)
            {
                bindingSource1.Sort = column.DataPropertyName;
                //bindingSource1.IsSorted
                //bindingSource1.ResetBindings(true);
                //dgvMessages.Sort(column, ListSortDirection.Ascending);
            }
        }

        Object messageListLock = new Object();

        public void AddMessage(IEnumerable<Message> messages)
        {
            if (messages != null)
            {
                lock (messageListLock)
                {
                    //    messageList.AddRange(messages);
                    //}
                    foreach (var item in messages)
                    {
                        displayMessage.Add(new MessageStorage(this, item, displayMessage.Count));
                    }
                }
                SetCalcStatusLabel();
                //ShowMessage();
            }
        }

        public void AddMessage(Message message)
        {
            if (message == null)
            {
                lock (messageListLock)
                {
                    //messageList.Add(message);
                    displayMessage.Add(new MessageStorage(this, message, displayMessage.Count));
                }
                SetCalcStatusLabel();
                //ShowMessage();
            }
        }

        public void ClearMessage()
        {
            lock (messageListLock)
            {
                //messageList.Clear();
                displayMessage.Clear();
            }
            SetCalcStatusLabel();
            //ShowMessage();
        }

        private void pageCountChanged()
        {
            try
            {
                if (InvokeRequired) Invoke((Action)pageCountChanged);
                else
                {
                    prevMessagesButton.Visible = nextMessagesButton.Visible = PageCount > 0;
                }
            }
            catch (ObjectDisposedException)
            { }
        }

        private void prevMessagesButton_Click(object sender, EventArgs e)
        {
            if (PageCount > 0)
            {
                startPos -= PageCount;
                SetCalcStatusLabel();
                //ShowMessage();
            }
        }

        private void nextMessagesButton_Click(object sender, EventArgs e)
        {
            if (PageCount > 0)
            {
                startPos += PageCount;
                SetCalcStatusLabel();
                //ShowMessage();
            }
        }

        //private void ShowMessage()
        //{
        //    if (InvokeRequired) BeginInvoke((Action)ShowMessage);
        //    else
        //    {
        //        lock (messageListLock)
        //        {
        //            int start = startPos, length = messageList.Count;

        //            if (PageCount <= 0 || start < 0 || start > messageList.Count)
        //                start = startPos = 0;
        //            if (PageCount > 0 && start + PageCount < length)
        //                length = start + PageCount;
        //            displayMessage.Clear();
        //            for (int i = start; i < length; i++)
        //            {
        //                //displayMessage.FuckinAdd(new MessageStorage(this, messageList[i], i));
        //                displayMessage.Add(new MessageStorage(this, messageList[i], i));
        //            }
        //        }
        //        bindingSource1.ResetBindings(false);
        //    }
        //}

        private void SetCalcStatusLabel()
        {
            try
            {
                if (InvokeRequired) BeginInvoke((Action)SetCalcStatusLabel);
                else
                {
                    int criticalErrorCount = 0, errorCount = 0, warningCount = 0;
                    foreach (MessageStorage message in displayMessage)

                        switch (message.Category)
                        {
                            case MessageCategory.Warning:
                                ++warningCount;
                                break;
                            case MessageCategory.Error:
                                ++errorCount;
                                break;
                            case MessageCategory.CriticalError:
                                ++criticalErrorCount;
                                break;
                        }
                    StringBuilder builder = new StringBuilder();
                    if (statusString != null)
                        builder.Append(statusString);
                    if (criticalErrorCount > 0)
                        builder.AppendFormat("{0}: {1}. ", "Критических ошибок", criticalErrorCount);
                    if (errorCount > 0)
                        builder.AppendFormat("{0}: {1}. ", "Ошибок", errorCount);
                    if (warningCount > 0)
                        builder.AppendFormat("{0}: {1}. ", "Предупреждений", warningCount);

                    calcStatusLabel.Text = builder.ToString();
                    prevMessagesButton.Enabled = startPos > 0;
                    nextMessagesButton.Enabled = startPos + PageCount < displayMessage.Count;
                }
            }
            catch (ObjectDisposedException)
            { }
        }

        Dictionary<int, UnitNode> dicNodes;

        private UnitNode GetNodeNameByID(int nodeID)
        {
            if (nodeID >= 0)
            {
                UnitNode unitNode;
                if (dicNodes.TryGetValue(nodeID, out unitNode))
                    return unitNode;
                else if (strucProvider != null)
                {
                    try
                    {
                        unitNode = strucProvider.GetUnitNode(nodeID);
                        dicNodes[nodeID] = unitNode;
                        return unitNode;
                    }
                    catch { }
                }
            }
            return null;
        }

        public class MessageStorage
        {
            CalcMessageViewControl parent;

            public MessageStorage(CalcMessageViewControl parent, Message mess, int position)
            {
                this.Message = mess;
                this.parent = parent;
                this.Position = position + 1;
            }

            public int Position { get; private set; }

            public Message Message { get; private set; }

            public MessageCategory Category { get { return Message.Category; } }

            public String Text { get { return Message.Text; } }

            public String Node
            {
                get
                {
                    CalcMessage mess = Message as CalcMessage;
                    if (mess != null)
                    {
                        UnitNode unitNode = parent.GetNodeNameByID(mess.Position.NodeID);
                        String nodeText;
                        if (unitNode != null)
                            nodeText = unitNode.GetNodeText();
                        else
                            nodeText = String.Empty;
                        //    return unitNode.GetNodeText();
                        //else
                        //    return GetAddText(mess.Position);
                        return nodeText + GetAddText(mess.Position);
                    }
                    else return String.Empty;
                }
            }

            private string GetAddText(CalcPosition calcPosition)
            {
                switch (calcPosition.CurrentPart)
                {
                    case CalcPosition.NodePart.Formula:
                        break;
                    case CalcPosition.NodePart.Expression:
                        return "Критерий";
                    case CalcPosition.NodePart.DefinitionDomain:
                        return "Область определения";
                    case CalcPosition.NodePart.ArgumentExpression:
                        return String.Format("Аргумент '{0}'", calcPosition.AdditionNote); ;
                    default:
                        break;
                }
                return String.Empty;
            }

            public String Line
            {
                get
                {
                    CalcMessage mess = Message as CalcMessage;
                    if (mess != null && mess.Position.Location != null) return mess.Position.Location.sLin.ToString();
                    else return String.Empty;
                }
            }
        }

        class MessagesList : IBindingList
        {
            List<MessageStorage> hidenList = new List<MessageStorage>();

            public int FirstIndex { get; set; }

            public int ShowCount { get; set; }

            public MessagesList()
            {
                FirstIndex = 0;
                ShowCount = int.MaxValue;
            }

            #region IBindingList Members

            public void AddIndex(PropertyDescriptor property)
            {
                throw new NotImplementedException();
            }

            public object AddNew()
            {
                throw new NotImplementedException();
            }

            public bool AllowEdit
            {
                get { return false; }
            }

            public bool AllowNew
            {
                get { return false; }
            }

            public bool AllowRemove
            {
                get { return false; }
            }

            public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
            {
                sortProperty = property;
                sortDirection = direction;
                if (sortProperty != null)
                    hidenList.Sort((a, b) =>
                    {
                        int ret;
                        Object aObject = property.GetValue(a);
                        Object bObject = property.GetValue(b);
                        if (aObject is int && bObject is int)
                        {
                            int aInt, bInt;
                            aInt = (int)aObject;
                            bInt = (int)bObject;
                            ret = aInt.CompareTo(bInt);
                        }
                        else
                        {
                            String aString, bString;
                            bool aNUll = aObject == null, bNull = bObject == null;
                            if (aNUll && bNull) ret = 0;
                            else if (aNUll && !bNull) ret = -1;
                            else if (!aNUll && bNull) ret = 1;
                            else
                            {
                                aString = aObject.ToString();
                                bString = bObject.ToString();
                                ret = aString.CompareTo(bString);
                            }
                        }
                        if (direction == ListSortDirection.Descending)
                            ret *= -1;
                        return ret;
                    });
                if (ListChanged != null)
                    ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
            }

            public int Find(PropertyDescriptor property, object key)
            {
                throw new NotImplementedException();
            }

            public bool IsSorted
            {
                get { return sortProperty != null; }
            }

            public event ListChangedEventHandler ListChanged;

            public void RemoveIndex(PropertyDescriptor property)
            {
                throw new NotImplementedException();
            }

            public void RemoveSort()
            {
                ApplySort(null, ListSortDirection.Ascending);
            }

            private ListSortDirection sortDirection;
            public ListSortDirection SortDirection
            {
                get { return sortDirection; }
            }

            private PropertyDescriptor sortProperty;
            public PropertyDescriptor SortProperty
            {
                get { return sortProperty; }
            }

            public bool SupportsChangeNotification
            {
                get { return true; }
            }

            public bool SupportsSearching
            {
                get { return false; }
            }

            public bool SupportsSorting
            {
                get { return true; }
            }

            #endregion


            bool propertiesReseted;
            private void ResetProperties(MessageStorage storage)
            {
                if (ListChanged != null && storage != null)
                {
                    if (!propertiesReseted)
                    {
                        //var converter = TypeDescriptor.GetConverter(storage);

                        var converter = new ExpandableObjectConverter();

                        var properties = converter.GetProperties(storage);
                        if (properties != null)
                        {
                            foreach (PropertyDescriptor item in properties)
                            {
                                ListChanged(this, new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, item));
                            }
                            propertiesReseted = true;
                        }
                    }
                }
            }

            private void OnListChanged(ListChangedType changedType, int index)
            {
                if (ListChanged != null)
                {
                    ListChanged(this, new ListChangedEventArgs(changedType, index));
                }
            }

            #region IList Members

            public int Add(object value)
            {
                int index = -1;
                var msg = value as MessageStorage;

                if (msg!=null)
                {
                    hidenList.Add(msg);
                    index = hidenList.Count;
                    
                    ResetProperties(msg);
                    OnListChanged(ListChangedType.ItemAdded, index - 1);
                }
                return index;
            }

            public void Clear()
            {
                hidenList.Clear();
                OnListChanged(ListChangedType.Reset, 0);
            }

            public bool Contains(object value)
            {
                var msg = value as MessageStorage;

                if (msg != null)
                {
                    return hidenList.Contains(msg);
                }
                return false;
            }

            public int IndexOf(object value)
            {
                var msg = value as MessageStorage;

                if (msg != null)
                {
                    return hidenList.IndexOf(msg);
                }
                return -1;
            }

            public void Insert(int index, object value)
            {
                var msg = value as MessageStorage;

                if (msg != null)
                {
                    hidenList.Insert(index, msg);

                    OnListChanged(ListChangedType.ItemAdded, index);
                }
            }

            public bool IsFixedSize
            {
                get { return false; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public void Remove(object value)
            {
                int index = -1;
                var msg = value as MessageStorage;

                if (msg != null)
                {
                    index=IndexOf(msg);

                    if (hidenList.Remove(msg))
                    {
                        OnListChanged(ListChangedType.ItemDeleted, index);
                    }
                }
            }

            public void RemoveAt(int index)
            {
                hidenList.RemoveAt(index);

                OnListChanged(ListChangedType.ItemDeleted, index);
            }

            public object this[int index]
            {
                get
                {
                    return hidenList[index];
                }
                set
                {
                    var msg = value as MessageStorage;

                    if (msg != null)
                    {
                        hidenList[index] = value as MessageStorage;
                    }
                }
            }

            #endregion

            #region ICollection Members

            public void CopyTo(Array array, int index)
            {
                var msgArray = array as MessageStorage[];

                if (msgArray != null)
                {
                    hidenList.CopyTo(msgArray, index);
                }
            }

            public int Count
            {
                get { return hidenList.Count; }
            }

            public bool IsSynchronized
            {
                get { return false; }
            }

            public object SyncRoot
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region IEnumerable Members

            public System.Collections.IEnumerator GetEnumerator()
            {
                for (int i = FirstIndex; i < hidenList.Count && i < FirstIndex + ShowCount; i++)
                {
                    yield return hidenList[i];
                }
            }

            #endregion
        }

        //class MessagesList : List<MessageStorage>, IBindingList
        //{

        //    #region IBindingList Members

        //    public void AddIndex(PropertyDescriptor property)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public object AddNew()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public bool AllowEdit
        //    {
        //        get { return false; }
        //    }

        //    public bool AllowNew
        //    {
        //        get { return false; }
        //    }

        //    public bool AllowRemove
        //    {
        //        get { return false; }
        //    }

        //    Type messageType = typeof(MessageStorage);
        //    public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        //    {
        //        sortProperty = property;
        //        sortDirection = direction;
        //        if(sortProperty!=null)
        //        this.Sort((a, b) =>
        //        {
        //            int ret;
        //            Object aObject = property.GetValue(a);
        //            Object bObject = property.GetValue(b);
        //            if (aObject is int && bObject is int)
        //            {
        //                int aInt, bInt;
        //                aInt = (int)aObject;
        //                bInt = (int)bObject;
        //                ret= aInt.CompareTo(bInt);
        //            }
        //            else
        //            {
        //                String aString, bString;
        //                bool aNUll = aObject == null, bNull = bObject == null;
        //                if (aNUll && bNull) ret = 0;
        //                else if (aNUll && !bNull) ret = -1;
        //                else if (!aNUll && bNull) ret = 1;
        //                else
        //                {
        //                    aString = aObject.ToString();
        //                    bString = bObject.ToString();
        //                    ret = aString.CompareTo(bString); 
        //                }
        //            }
        //            if (direction == ListSortDirection.Descending)
        //                ret *= -1;
        //            return ret;
        //        });
        //        if (ListChanged != null)
        //            ListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
        //    }

        //    public int Find(PropertyDescriptor property, object key)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public bool IsSorted
        //    {
        //        get { return sortProperty != null; }
        //    }

        //    public event ListChangedEventHandler ListChanged;

        //    public void RemoveIndex(PropertyDescriptor property)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public void RemoveSort()
        //    {
        //        ApplySort(null, ListSortDirection.Ascending);
        //    }

        //    private ListSortDirection sortDirection;
        //    public ListSortDirection SortDirection
        //    {
        //        get { return sortDirection; }
        //    }

        //    private PropertyDescriptor sortProperty;
        //    public PropertyDescriptor SortProperty
        //    {
        //        get { return sortProperty; }
        //    }

        //    public bool SupportsChangeNotification
        //    {
        //        get { return false; }
        //    }

        //    public bool SupportsSearching
        //    {
        //        get { return false; }
        //    }

        //    public bool SupportsSorting
        //    {
        //        get { return true; }
        //    }

        //    #endregion

        //    internal void FuckinAdd(MessageStorage messageStorage)
        //    {
        //        base.Add(messageStorage);
        //        if (ListChanged != null)
        //            ListChanged(this, new ListChangedEventArgs(ListChangedType.ItemAdded, base.IndexOf(messageStorage)));
        //        //throw new NotImplementedException();
        //    }
        //}
    }
}

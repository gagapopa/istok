using System;
using System.Collections.Generic;
using System.Windows.Forms;
using COTES.ISTOK;

namespace COTES.ISTOK.Client
{

    public delegate void InfoMessageStateHandler(object sender, InfoMessageStateEventArgs e);
    public class InfoMessageStateEventArgs : EventArgs
    {
        private InfoMessage infoMessage = null;

        public InfoMessageStateEventArgs(InfoMessage message)
        {
            infoMessage = message;
        }

        public InfoMessage Message
        {
            get { return infoMessage; }
        }
    }
    /// <summary>
    /// Управленец сообщениями
    /// </summary>
    public class MessageManager
    {
        List<InfoMessage> lstMessages;

        public InfoMessageStateHandler OnStateChange = null;

        public MessageManager()
        {
            lstMessages = new List<InfoMessage>();
        }

        ///// <summary>
        ///// Список сообщений
        ///// </summary>
        //public List<InfoMessage> Messages
        //{
        //    get { return lstMessages; }
        //}

        /// <summary>
        /// Изменяет состояние узла. Если узел в списке отсутствует, тогда он добавляется
        /// </summary>
        /// <param name="node">Узел</param>
        /// <param name="infoMessageState">Новое состояние узла</param>
        public void ChangeNodeState(TreeNode node, InfoMessageState infoMessageState)
        {
            InfoMessage msginfo = null;
            bool needUpdate = true;

            if (node == null)
                return;

            lock (lstMessages)
            {
                foreach (InfoMessage item in lstMessages)
                {
                    if (item.Node == node)
                    {
                        msginfo = item;
                        break;
                    }
                }

                if (msginfo == null)
                {
                    msginfo = new InfoMessage();
                    msginfo.Node = node;
                    msginfo.Committed = false;
                    msginfo.State = infoMessageState;
                    //msginfo.Text = "";
                    msginfo.Time = DateTime.Now;

                    //CommonData.Error(msginfo.ToString());
                    lstMessages.Add(msginfo);
                }
                else
                {
                    if (msginfo.State == infoMessageState)
                        needUpdate = false;
                    else
                    {
                        msginfo.State = infoMessageState;
                        msginfo.Committed = false;
                        msginfo.Time = DateTime.Now;
                    }
                }
            }

            if (OnStateChange != null && needUpdate)
                OnStateChange(this, new InfoMessageStateEventArgs(msginfo));
        }
        /// <summary>
        /// Удаляет сообщения, связанные с указанным узлом
        /// </summary>
        /// <param name="node">Указанный узел</param>
        public void RemoveNode(TreeNode node)
        {
            if (node == null) return;

            lock (lstMessages)
            {
                foreach (InfoMessage item in lstMessages)
                {
                    if (item.Node == node)
                    {
                        lstMessages.Remove(item);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает текущее состояние узлов
        /// </summary>
        /// <returns>Состояние</returns>
        public DiagState GetCurrentState()
        {
            DiagState state = DiagState.Good;

            lock (lstMessages)
            {
                foreach (InfoMessage item in lstMessages)
                {
                    if (item.State != InfoMessageState.NodeOnline &&
                        item.State != InfoMessageState.NodeDisabled)
                    {
                        if (!item.Committed)
                        {
                            state = DiagState.BadNotComitted;
                            break;
                        }
                        else
                            state = DiagState.Bad;
                    }
                }
            }

            return state;
        }

        /// <summary>
        /// Квитирование всех сообщений
        /// </summary>
        public void CommitAll()
        {
            lock (lstMessages)
            {
                foreach (InfoMessage item in lstMessages)
                {
                    item.Committed = true;
                }
            }
        }

        /// <summary>
        /// Проверка на принадлежность сообщения списку
        /// </summary>
        /// <param name="message">Проверяемое сообщение</param>
        /// <returns>true, если сообщение есть в списке, иначе - false</returns>
        public bool Contains(InfoMessage message)
        {
            foreach (InfoMessage item in lstMessages)
            {
                if (item == message) return true;
            }

            return false;
        }

        /// <summary>
        /// Удаление устаревших узлов
        /// </summary>
        /// <param name="nodes"></param>
        public void ClearNodes(TreeNodeCollection nodes)
        {
            List<InfoMessage> lstDelete = new List<InfoMessage>();

            if (nodes == null)
                throw new ArgumentNullException("nodes");

            foreach (InfoMessage item in lstMessages)
            {
                if (!NodeContains(nodes, item.Node))
                    lstDelete.Add(item);
            }

            foreach (InfoMessage item in lstDelete)
            {
                lstMessages.Remove(item);
            }
        }

        private bool NodeContains(TreeNodeCollection nodes, TreeNode node)
        {
            bool res = false;

            foreach (TreeNode item in nodes)
            {
                if (item == node)
                    res = true;
                else
                    res = NodeContains(item.Nodes, node);

                if (res) break;
            }

            return res;
        }
    }
    /// <summary>
    /// Класс инфо сообщения
    /// </summary>
    public class InfoMessage
    {
        private string text;
        private string strNode;
        private TreeNode node;
        private DateTime time;
        private bool committed;
        private InfoMessageState state;

        public InfoMessage()
        {
            text = "";
            node = null;
            time = DateTime.MinValue;
            committed = false;
            state = InfoMessageState.NodeDataWarning;
        }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        /// <summary>
        /// Узел, с которым связано сообщение
        /// </summary>
        public TreeNode Node
        {
            get { return node; }
            set { node = value; SetNodeText(); }
        }
        /// <summary>
        /// Текст узла
        /// </summary>
        public string NodeText
        {
            get { return strNode; }
        }
        /// <summary>
        /// Время сообщения
        /// </summary>
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
        /// <summary>
        /// Подтвержденность сообщения
        /// </summary>
        public bool Committed
        {
            get { return committed; }
            set { committed = value; }
        }
        /// <summary>
        /// Состояние (скорее тип) сообщения
        /// </summary>
        public InfoMessageState State
        {
            get { return state; }
            set { state = value; }
        }
        /// <summary>
        /// Формирование текста состояния узла
        /// </summary>
        /// <returns>Текст состояния</returns>
        public string FormatMessageState()
        {
            switch (state)
            {
                case InfoMessageState.NodeOnline:
                    return "Узел перешел в состояние 'Онлайн'";
                case InfoMessageState.NodeOffline:
                    return "Узел перешел в состояние 'Оффлайн'";
                case InfoMessageState.NodeDataWarning:
                    return "Узел перешел в состояние 'Данные: Предупреждение'";
                case InfoMessageState.NodeDataError:
                    return "Узел перешел в состояние 'Данные: Ошибка'";
                case InfoMessageState.NodeDisabled:
                    return "Узел перешел в состояние 'Отключен'";
            }

            return "";
        }

        public override string ToString()
        {
            string strTime, strText;

            strTime = Time.ToString();
            strText = FormatMessageState();

            return strTime + " " + strNode + ": " + strText;
        }

        private void SetNodeText()
        {
            TreeNode ptr;
            string str = "", str2;

            ptr = node;
            while (ptr != null)
            {
                str2 = ptr.Text;
                if (!string.IsNullOrEmpty(str))
                    str2 += "/";
                str = str2 + str;
                ptr = ptr.Parent;
            }

            strNode = str;
        }
    }
    /// <summary>
    /// Перечисление состояний инфо сообщения
    /// </summary>
    public enum InfoMessageState
    {
        /// <summary>
        /// Узел ушел в оффлайн
        /// </summary>
        NodeOffline,
        /// <summary>
        /// Узел вернулся в онлайн
        /// </summary>
        NodeOnline,
        /// <summary>
        /// Ошибка в данных узла
        /// </summary>
        NodeDataError,
        /// <summary>
        /// Предупреждение о данных в узле
        /// </summary>
        NodeDataWarning,
        /// <summary>
        /// Узел отключен (для канала)
        /// </summary>
        NodeDisabled
    } 
}
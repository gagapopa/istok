using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Класс предающий древовидный вид элементам, казалось бы совсем не древовидным
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class TreeWrapp<T>
    {
        /// <summary>
        /// Старая версия узла, там где она нужна
        /// </summary>
        public T OldItem { get; set; }

        /// <summary>
        /// Обернутый узел
        /// </summary>
        public T Item { get; set; }

        List<TreeWrapp<T>> childNodes;

        /// <summary>
        /// Обертка для дочерних элементов
        /// </summary>
        public TreeWrapp<T>[] ChildNodes
        {
            get
            {
                if (childNodes == null)
                    return null;
                return childNodes.ToArray();
            }
        }

        /// <summary>
        /// Количество всех элементов, включая дочерних
        /// </summary>
        public int RecursiveCount
        {
            get
            {
                int count = 1;
                if (ChildNodes != null)
                    foreach (TreeWrapp<T> childWrap in ChildNodes) count += childWrap.RecursiveCount;
                return count;
            }
        }
        public TreeWrapp(T unitNode)
        {
            this.Item = unitNode;
        }

        public TreeWrapp(T unitNode, TreeWrapp<T>[] childs)
            : this(unitNode)
        {
            this.childNodes = new List<TreeWrapp<T>>(childs);
        }

        public void AddChild(TreeWrapp<T> item)
        {
            if (childNodes == null)
                childNodes = new List<TreeWrapp<T>>();

            childNodes.Add(item);
        }

        public TreeWrapp<T> AddChild(T item)
        {
            TreeWrapp<T> child;
            if (childNodes == null)
                childNodes = new List<TreeWrapp<T>>();

            childNodes.Add(child=new TreeWrapp<T>(item));
            return child;
        }

        public void AddChild(IEnumerable<T> items)
        {
            if (childNodes == null)
                childNodes = new List<TreeWrapp<T>>();

            foreach (T item in items)
                childNodes.Add(new TreeWrapp<T>(item));
        }
    }
}

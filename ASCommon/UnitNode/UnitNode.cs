using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    [TypeConverter(typeof(UnitNodeTypeConverter))]
    [KnownType(typeof(BlockNode))]
    [KnownType(typeof(CalcParameterNode))]
    [KnownType(typeof(ChannelNode))]
    [KnownType(typeof(ExcelReportNode))]
    [KnownType(typeof(GlobalNode))]
    [KnownType(typeof(GraphNode))]
    [KnownType(typeof(HistogramNode))]
    [KnownType(typeof(LoadParameterNode))]
    [KnownType(typeof(ManualParameterNode))]
    [KnownType(typeof(MonitorTableNode))]
    [KnownType(typeof(NormFuncNode))]
    [KnownType(typeof(OptimizationGateNode))]
    [KnownType(typeof(ParameterGateNode))]
    [KnownType(typeof(ParameterNode))]
    [KnownType(typeof(ParametrizedUnitNode))]
    [KnownType(typeof(ReportNode))]
    [KnownType(typeof(SchemaNode))]
    [DataContract]
    public class UnitNode : ServerNode
    {
        const String sortIndexAttributeName = "sortindex";
        const String docIndexAttributeName = "index";
        const String ownerAttributeName = "owner";

        protected static DoubleConverter doubleconv = new DoubleConverter();
        protected static DateTimeConverter dateconv = new DateTimeConverter();

        [DataMember]
#if DEBUG
        [Browsable(true)]
        [CategoryOrder(CategoryGroup.Debug)]
        [ReadOnly(true)]
#else
        [Browsable(false)]
#endif
        public string FullName { get; set; }

        [Browsable(false)]
        [DataMember]
        public bool IsCrypted { get; set; }

        [Browsable(false)]
        [DataMember]
        public virtual ChildParamNode[] Parameters
        {
            get
            {
                return null;
            }
            set
            { }
        }

        [DisplayName("Тип")]
        [CategoryOrder(CategoryGroup.General)]
        [TypeConverter(typeof(UnitTypeTypeConverter))]
        [Editor(typeof(UnitTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DataMember]
        //public UnitTypeId Typ { get; set; }
        public int Typ { get; set; }

        /// <summary>
        /// количество дочерних элементов
        /// </summary>
        [Browsable(false)]
        public bool HasChild { get { return nodesIds.Count != 0; } }

        /// <summary>
        /// код для формулы
        /// </summary>
        [DisplayName("Код"), Description("Уникальный код параметра.")]
        [Browsable(false)]
        public virtual string Code { get { return ""; } set { throw new Exception("UnitNode.set_Code() не реализован"); } }

        #region String Attributes
        [DataMember]
        private Dictionary<String, RevisedStorage<String>> attributes = new Dictionary<String, RevisedStorage<String>>(StringComparer.OrdinalIgnoreCase); // new Hashtable(StringComparer.OrdinalIgnoreCase);

        [DisplayName("Свойства")]
        [ReadOnly(true)]
        [Browsable(false)]
        public Dictionary<String, RevisedStorage<String>> Attributes
        {
            get { return attributes; }
            set { attributes = new Dictionary<String, RevisedStorage<String>>(value, StringComparer.OrdinalIgnoreCase);/* modified = true;*/ } //new Hashtable(value, StringComparer.OrdinalIgnoreCase); modified = true; }
        }

        public string GetAttribute(string attribureName)
        {
            RevisedStorage<String> versionValue;

            if (Attributes.TryGetValue(attribureName, out versionValue))
            {
                try
                {
                    return versionValue.Get();
                }
                catch (ArgumentException) { }
            }
            return String.Empty;
        }

        public string GetAttribute(string attribureName, RevisionInfo revision)
        {
            RevisedStorage<String> versionValue;

            if (Attributes.TryGetValue(attribureName, out versionValue))
            {
                try
                {
                    return versionValue.Get(revision);
                }
                catch (ArgumentException) { }
            }
            return String.Empty;
        }

        public string GetAttribute(string attribureName, DateTime time)
        {
            RevisedStorage<String> versionValue;

            if (Attributes.TryGetValue(attribureName, out versionValue))
            {
                try
                {
                    return versionValue.Get(time);
                }
                catch (ArgumentException) { }
            }
            return String.Empty;
        }

        public void SetAttribute(string attribureName, string value)
        {
            RevisedStorage<String> versionValue;

            if (!Attributes.TryGetValue(attribureName, out versionValue))
                Attributes[attribureName] = versionValue = new RevisedStorage<String>();

            versionValue.Set(value);
        }

        public void SetAttribute(string attribureName, RevisionInfo revision, string value)
        {
            RevisedStorage<String> versionValue;

            if (!Attributes.TryGetValue(attribureName, out versionValue))
                Attributes[attribureName] = versionValue = new RevisedStorage<String>();

            versionValue.Set(revision, value);
        } 
        #endregion

        #region Binary Attributes
        [DataMember]
        private Dictionary<string, RevisedStorage<byte[]>> binaries = new Dictionary<string, RevisedStorage<byte[]>>(StringComparer.OrdinalIgnoreCase);

        [Browsable(false)]
        public Dictionary<string, RevisedStorage<byte[]>> Binaries
        {
            get { return binaries; }
            set { binaries = new Dictionary<string, RevisedStorage<byte[]>>(value, StringComparer.OrdinalIgnoreCase);/* modified = true;*/ }
        }

        public byte[] GetBinaries(string attribureName)
        {
            RevisedStorage<byte[]> versionValue;

            if (Binaries.TryGetValue(attribureName, out versionValue))
            {
                try
                {
                    return versionValue.Get();
                }
                catch (ArgumentException) { }
            }
            return null;
        }

        public byte[] GetBinaries(string attribureName, RevisionInfo revision)
        {
            RevisedStorage<byte[]> versionValue;

            if (Binaries.TryGetValue(attribureName, out versionValue))
            {
                try
                {
                    return versionValue.Get(revision);
                }
                catch (ArgumentException) { }
            }
            return null;
        }

        public byte[] GetBinaries(string attribureName, DateTime time)
        {
            RevisedStorage<byte[]> versionValue;

            if (Binaries.TryGetValue(attribureName, out versionValue))
            {
                try
                {
                    return versionValue.Get(time);
                }
                catch (ArgumentException) { }
            }
            return null;
        }

        public void SetBinaries(string attribureName, byte[] value)
        {
            RevisedStorage<byte[]> versionValue;

            if (!Binaries.TryGetValue(attribureName, out versionValue))
                Binaries[attribureName] = versionValue = new RevisedStorage<byte[]>();

            versionValue.Set(value);
        }

        public void SetBinaries(string attribureName, RevisionInfo revision, byte[] value)
        {
            RevisedStorage<byte[]> versionValue;

            if (!Binaries.TryGetValue(attribureName, out versionValue))
                Binaries[attribureName] = versionValue = new RevisedStorage<byte[]>();

            versionValue.Set(revision, value);
        }
        #endregion

        /// <summary>
        /// Возвращает список всех ревизий, в которых значения свойств отличаются.
        /// </summary>
        public IEnumerable<RevisionInfo> GetRevisions()
        {
            var attrsRevisions = (from s in Attributes.Values from r in s select r).Distinct();
            var binariesRevisions = (from s in Binaries.Values from r in s select r).Distinct();

            return attrsRevisions.Union(binariesRevisions).OrderBy(r => r.Time);
        }

        /// <summary>
        /// Возвращает старшую по времени ревизию из свойства Revisions
        /// </summary>
        public RevisionInfo GetHeadRevision()
        {
            return (from r in GetRevisions() orderby r.Time descending select r).First();
        }

#if DEBUG
        [Browsable(true)]
        [DisplayName("Индекс сортировки")]
        [CategoryOrder(CategoryGroup.Debug)]
        [ReadOnly(true)]
#else
        [Browsable(false)] 
#endif
        public virtual int Index
        {
            get
            {
                int res;

                if (int.TryParse(GetAttribute(sortIndexAttributeName), out res))
                    return res;
                return 0;
            }
            set
            {
                SetAttribute(sortIndexAttributeName, value.ToString());
            }
        }

        [DisplayName("Индекс")]
        [Description("Индекс для ссылов внутренней документации")]
        [CategoryOrder(CategoryGroup.General)]
        public virtual String DocIndex
        {
            get { return GetAttribute(docIndexAttributeName); }
            set { SetAttribute(docIndexAttributeName, value); }
        }

        /// <summary>
        /// Получить название узла в сочетании с индексом (DocIndex)
        /// </summary>
        /// <returns></returns>
        public virtual String GetNodeText()
        {
            string txt;

            if (!String.IsNullOrEmpty(DocIndex))
                txt = DocIndex + " " + Text;
            else
                txt = Text;

            return txt;
        }

        /// <summary>
        /// собственник узла, используется для реализации пользовательских шаблонов
        /// </summary>
        [Browsable(true)]
        [DisplayName("Владелец")]
        [Description("собственник узла, влияющий на видимость и права других пользователей")]
        [CategoryOrder(CategoryGroup.General)]
        [TypeConverter(typeof(OwnerTypeConverter))]
        public int Owner
        {
            get
            {
                int res;

                if (int.TryParse(GetAttribute(ownerAttributeName), out res))
                    return res;

                return 0;
            }
            set
            {
                SetAttribute(ownerAttributeName, value.ToString());
            }
        }

        [Browsable(false)]
        [DataMember]
        public int ParentId { get; set; }

        [DataMember]
        protected List<int> nodesIds = new List<int>();
        [Browsable(false)]
        public int[] NodesIds
        {
            get
            {
                if (nodesIds == null) nodesIds = new List<int>();
                return nodesIds.ToArray();
            }
        }

        public UnitNode()
        {
            Text = "Новый элемент";
        }
        public UnitNode(DataRow row)
            : base(row)
        {
            if (row["parent"] != DBNull.Value)
                ParentId = (int)row["parent"];
            Typ = (int)row["type"];
        }

        public virtual ChildParamNode AddChildParam(DataRow row)
        {
            throw new NotImplementedException();
        }

        public virtual ChildParamNode AddChildParam(ParameterNode node)
        {
            throw new NotImplementedException();
        }

        public override object[] ToCells()
        {
            ServerNode _type = null;
            if (_type == null)
                return new object[] { Idnum, Text, "Неизвестный тип (" + Typ.ToString() + ")", Code };
            else
                return new object[] { Idnum, Text, _type.Text, Code };
        }

        public override string ToString()
        {
            return FullName;
        }

        public override object Clone()
        {
            UnitNode res;//= UnitNode.NewInstance(Typ);
            res = GetType().GetConstructor(new Type[] { }).Invoke(null) as UnitNode;
            if (res != null)
            {
                res.idnum = Idnum;
                res.Typ = Typ;
                res.Text = Text;
                res.FullName = FullName;
                //res.Editor = Editor;
                //res.tree_visible = Tree_visible;
                res.Attributes = new Dictionary<string, RevisedStorage<string>>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in Attributes.Keys)
                {
                    res.Attributes[item] = Attributes[item].Clone() as RevisedStorage<String>;
                }
                res.Binaries = new Dictionary<string, RevisedStorage<byte[]>>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in Binaries.Keys)
                {
                    res.Binaries[item] = Binaries[item].Clone() as RevisedStorage<byte[]>;
                }
                res.nodesIds = new List<int>(nodesIds);
                res.ParentId = ParentId;
                res.IsCrypted = IsCrypted;
            }
            return res;
        }

        #region Работа с нодами
        /// <summary>
        /// Очищает дочерние узлы.
        /// </summary>
        public void ClearNodes()
        {
            if (nodesIds != null) nodesIds.Clear();
        }

        /// <summary>
        /// Добавляет дочерний узел.
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(UnitNode node)
        {
            if (node == null) return;
            if (!NodesIds.Contains(node.Idnum))
                AddNode(node.Idnum);
        }
        public void AddNode(int nodeId)
        {
            if (!nodesIds.Contains(nodeId)) nodesIds.Add(nodeId);
        }
        public void AddNodes(int[] nodeIds)
        {
            if (nodeIds == null) throw new ArgumentNullException("nodeIds");
            foreach (var item in nodeIds) AddNode(item);
        }

        /// <summary>
        /// Удалить узел из числа детей
        /// </summary>
        /// <param name="unitNode"></param>
        public void RemoveNode(UnitNode unitNode)
        {
            if (nodesIds.Contains(unitNode.Idnum)) nodesIds.Remove(unitNode.Idnum);
        }
        #endregion

        public override bool Equals(object obj)
        {
            UnitNode node = obj as UnitNode;

            if (node == null) return false;
            if (!base.Equals(obj)) return false;

            if (Typ != node.Typ) return false;
            if (Code != node.Code) return false;
            //if (Tree_visible != node.Tree_visible) return false;
            if (IsCrypted != node.IsCrypted) return false;

            if (!CheckDictionaryEquals(Attributes, node.Attributes)) return false;
            if (!CheckDictionaryEquals(Binaries, node.Binaries)) return false;
            if (!CheckListEquals(NodesIds, node.NodesIds)) return false;
            if (!CheckListEquals(Parameters, node.Parameters)) return false;
            return true;
        }

        public override int GetHashCode()
        {
            //unchecked
            //{
            //    int result = 17;

            //    result += typ.GetHashCode() * 31;
            //    result += Code == null ? 0 : Code.GetHashCode();
            //    result += (Tree_visible ? 1 : 0) * 31;
            //    result += (IsCrypted ? 1 : 0) * 31;
            //    result += attributes.GetHashCode();
            //    result += binaries.GetHashCode();
            //    result += nodesIds.GetHashCode();
            //    result += Parameters.GetHashCode();

            return base.GetHashCode();
            //}
        }

        private bool CheckDictionaryEquals<T>(Dictionary<String, RevisedStorage<T>> a, Dictionary<String, RevisedStorage<T>> b)//IDictionary a, IDictionary b)
        {
            if (a != null && b != null)
            {
                if (a != b)
                {
                    List<String> keys = new List<String>();
                    foreach (var item in a.Keys)
                    {
                        if (item != null && !keys.Contains(item))
                            keys.Add(item);
                    }
                    foreach (var item in b.Keys)
                    {
                        if (item != null && !keys.Contains(item))
                            keys.Add(item);
                    }
                    //if (a.Count != b.Count) return false;
                    foreach (var item in keys)
                    {
                        if (a.ContainsKey(item) && b.ContainsKey(item))
                        {
                            if (!a[item].Equals(b[item],
                                (k, l) =>
                                {
                                    if (k is IEnumerable)
                                        return CheckEnumerableEquals((IEnumerable)k, l as IEnumerable);
                                    else
                                        return Object.Equals(k, l);
                                }))
                                return false;
                        }
                        else
                            //else if ((a.ContainsKey(item) && a[item] != null && a[item].ToString() != "")
                            //    || (b.ContainsKey(item) && b[item] != null && b[item].ToString() != ""))
                            return false;
                    }
                }
            }
            else if (a != b) return false;
            return true;
        }
        private bool CheckEnumerableEquals(IEnumerable a, IEnumerable b)
        {
            if (a != null && b != null)
            {
                if (a != b)
                {
                    IEnumerator enA = a.GetEnumerator();
                    IEnumerator enB = b.GetEnumerator();
                    bool rA, rB;
                    bool res;

                    while ((rA = enA.MoveNext()) & (rB = enB.MoveNext()))
                    {
                        if (enA.Current is IEnumerable)
                            res = CheckEnumerableEquals((IEnumerable)enA.Current, enB.Current as IEnumerable);
                        else
                            res = enA.Current.Equals(enB.Current);
                        if (!res) return false;
                    }
                    if (rA != rB) return false;
                }
            }
            else if (a != b) return false;
            return true;
        }
        private bool CheckListEquals(IList a, IList b)
        {
            if (a != null && b != null)
            {
                if (a != b)
                {
                    if (a.Count != b.Count) return false;
                    for (int i = 0; i < a.Count; i++)
                        if (!a[i].Equals(b[i])) return false;
                }
            }
            else if (a != b) return false;
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Настройка источника данных для запроса структуры и может быть значений
    /// </summary>
    [DataContract(IsReference = true)]
    public class StructureReportSourceSettings : ReportSourceSettings
    {
        public StructureReportSourceSettings(ReportSourceInfo info)
            : base(info)
        {

        }

        public StructureReportSourceSettings(StructureReportSourceSettings x)
            : this(x.Info)
        {
            this.ValueSourceSetting = x.ValueSourceSetting;
            //this.RootNodeID = x.RootNodeID;
            //this.RootNodeParameterRequired = x.RootNodeParameterRequired;
            //this.RootNodeTypeFilter = x.RootNodeTypeFilter;
            //this.QueryValues = x.QueryValues;
            //this.NodeTypeFilter = x.NodeTypeFilter;
            //this.Filter = x.Filter;

            if (x.Tables != null)
            {
                Tables = new StructureReportTable[x.Tables.Length];
                x.Tables.CopyTo(Tables, 0);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public int TablesCount
        {
            get
            {
                if (Tables == null)
                    return 0;
                return Tables.Length;
            }
        }

        [DataMember]
        [TypeConverter(typeof(StructureTableConverter))]
        public StructureReportTable[] Tables { get; set; }

        public StructureReportTable GetTable(string rootParameterName)
        {
            if (Tables != null)
            {
                foreach (var item in Tables)
                {
                    if (item.RootParameterName.Equals(rootParameterName))
                        return item;
                }
            }
            return null;
        }

        [System.ComponentModel.Browsable(false)]
        public bool QueryValues
        {
            get
            {
                bool queryValues = false;

                if (Tables != null)
                {
                    for (int i = 0; !queryValues && i < Tables.Length; i++)
                    {
                        queryValues = Tables[i] != null && Tables[i].QueryValues;
                    }
                }
                return queryValues;
            }
        }

        [DataMember]
        [System.ComponentModel.Browsable(false)]
        public Guid ValueSourceSetting { get; set; }

        public override Guid[] References
        {
            get
            {
                if (QueryValues && ValueSourceSetting != null)
                    return new Guid[] { ValueSourceSetting };

                return base.References;
            }
        }

        //public const String rootNodeParameterName = "structure_root";

        public override ReportParameter[] GetReportParameters()
        {
            List<ReportParameter> paramterList = new List<ReportParameter>();

            if (Tables != null)
            {
                foreach (var item in Tables)
                {
                    if (item.RootNode == StructureReportTable.RootNodeMethod.Request)
                    {
                        String displayName = "Корневой узел";
                        if (Tables.Length > 1)
                            displayName = String.Format("{0} ({1})", displayName, item.TableName);
                        ReportParameter parameter = new ReportParameter(item.RootParameterName,
                                                                        displayName,
                                                                        "",
                                                                        "Структура",
                                                                        typeof(int).FullName,
                                                                        typeof(StructureParameterTypeConverter).FullName,
                                                                        null);
                        parameter.BaseSettings = this;
                        parameter.RequiredString = true;
                        paramterList.Add(parameter);
                    }
                }
            }

            if (paramterList.Count > 0)
                return paramterList.ToArray();

            return base.GetReportParameters();
        }

        public override byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    int count;
                    int length;

                    bw.Write(Enabled);

                    count = TablesCount;

                    bw.Write(count);

                    for (int i = 0; i < count; i++)
                    {
                        byte[] bytes = Tables[i].ToBytes();

                        length = bytes.Length;
                        bw.Write(length);
                        bw.Write(bytes);
                    }
                }
                return ms.ToArray();
            }
        }

        public override void FromBytes(byte[] bytes)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        int count;
                        int length;

                        Enabled = br.ReadBoolean();


                        count = TablesCount;

                        count = br.ReadInt32();
                        if (count > 15)
                            return;

                        StructureReportTable[] tables = new StructureReportTable[count];
                        StructureReportTable table;

                        for (int i = 0; i < count; i++)
                        {
                            byte[] tableBuffer;
                            length = br.ReadInt32();
                            tableBuffer = br.ReadBytes(length);
                            table = new StructureReportTable();//this);
                            table.FromBytes(tableBuffer);
                            tables[i] = table;
                        }
                        Tables = tables;
                    }
                }
            }
            catch (EndOfStreamException) { }
        }

        public override object Clone()
        {
            return new StructureReportSourceSettings(this);
        }

        //[TypeConverter(typeof(NodeFilterTypeConverter))]
        //public enum NodeFilterType { Logical, Expression }

        //[TypeConverter(typeof(LogicalOperationTypeConverter))]
        //public enum LogicalOperation { AND, OR, NOT }

        //[Serializable]
        //[TypeConverter(typeof(NodeFilterConverter))]
        //public class NodeFilter
        //{
        //    [DisplayName("Тип фильтра")]
        //    public NodeFilterType FilterType { get; set; }

        //    [DisplayName("Логическая операция")]
        //    public LogicalOperation Operation { get; set; }

        //    [Browsable(false)]
        //    public NodeFilter[] Childs { get; set; }

        //    [DisplayName("Свойство узла"),
        //    Description("")]
        //    public String PropertyName { get; set; }

        //    [DisplayName("Проверяемое выражение"),
        //    Description()]
        //    public String Expression { get; set; }

        //    public void Write(BinaryWriter bw)
        //    {
        //        bw.Write((int)FilterType);
        //        bw.Write((int)Operation);
        //        bw.Write(PropertyName);
        //        bw.Write(Expression);

        //        int count = Childs == null ? 0 : Childs.Length;
        //        bw.Write(count);
        //        for (int i = 0; i < count; i++)
        //            Childs[i].Write(bw);
        //    }

        //    public static NodeFilter Read(BinaryReader br)
        //    {
        //        NodeFilter filter = new NodeFilter();
        //        filter.FilterType = (NodeFilterType)br.ReadInt32();
        //        filter.Operation = (LogicalOperation)br.ReadInt32();
        //        filter.PropertyName = br.ReadString();
        //        filter.Expression = br.ReadString();

        //        int count;
        //        count = br.ReadInt32();
        //        filter.Childs = new NodeFilter[count];
        //        for (int i = 0; i < count; i++)
        //            filter.Childs[i] = NodeFilter.Read(br);
        //        return filter;
        //    }

        //    public bool CheckNode(UnitNode node)
        //    {
        //        if (FilterType == NodeFilterType.Logical)
        //        {
        //            bool res;
        //            bool ret;

        //            if (Operation == LogicalOperation.AND)
        //                ret = true;
        //            else ret = false;

        //            for (int i = 0; i < Childs.Length; i++)
        //            {
        //                res = Childs[i].CheckNode(node);
        //                if (Operation == LogicalOperation.AND)
        //                {
        //                    ret &= res;
        //                    if (!ret)
        //                        break;
        //                }
        //                else if (Operation == LogicalOperation.OR)
        //                {
        //                    ret |= res;
        //                    if (ret)
        //                        break;
        //                }
        //                else if (Operation == LogicalOperation.NOT)
        //                {
        //                    ret = !res;
        //                    break;
        //                }
        //            }

        //            return ret;
        //        }
        //        else if (FilterType == NodeFilterType.Expression)
        //        {
        //            String value;

        //            if (node.Attributes.TryGetValue(PropertyName, out value))
        //            {
        //                if (value.IndexOf(Expression) >= 0)
        //                    return true;
        //            }
        //        }
        //        return false;
        //    }
        //}

        public int IndexOf(StructureReportTable table)
        {
            for (int i = 0; i < TablesCount; i++)
            {
                if (Tables[i] == table)
                    return i;
            }
            return -1;
        }
    }

    [DataContract]
    //[TypeConverter(typeof(StructureTableConverter))]
    public class StructureReportTable : ICloneable, IManualSerializable
    {
        [DataMember]
        [Browsable(false)]
        public StructureReportSourceSettings SourceSettings { get; set; }

        public StructureReportTable()//StructureReportSourceSettings sourceSettings)
        {
            RootNodeStructureLevel = new StructureLevel();
            NodeStructureLevel = new StructureLevel();
            //this.SourceSettings = sourceSettings;

            RootFromAnotherTable = -1;
            RootNodeMinLevel = -1;
            NodeMinLevel = -1;
        }

        public StructureReportTable(StructureReportTable x)
            : this()//x.SourceSettings)
        {
            //this.SourceSettings = x.SourceSettings;

            TableName = x.TableName;

            RootNode = x.RootNode;

            RootFromAnotherTable = x.RootFromAnotherTable;

            RootNodeID = x.RootNodeID;

            RootNodeMinLevel = x.RootNodeMinLevel;
            RootNodeMaxLevel = x.RootNodeMaxLevel;
            if (x.RootNodeTypeFilter != null)
            {
                RootNodeTypeFilter = new int[x.RootNodeTypeFilter.Length];
                x.RootNodeTypeFilter.CopyTo(RootNodeTypeFilter, 0);
            }

            QueryValues = x.QueryValues;

            NodeMinLevel = x.NodeMinLevel;
            NodeMaxLevel = x.NodeMaxLevel;
            if (x.NodeTypeFilter != null)
            {
                NodeTypeFilter = new int[x.NodeTypeFilter.Length];
                x.NodeTypeFilter.CopyTo(NodeTypeFilter, 0);
            }

            //Filter =x.Filter ;
        }

        /// <summary>
        /// Имя таблицы передаваемой в FastReport
        /// </summary>
        [DataMember]
        [Category("Основное"),
        DisplayName("Имя таблицы"),
        Description("Имя таблицы в источниках данных отчета.")]
        public String TableName { get; set; }

        [Browsable(false)]
        public String RootParameterName
        {
            get { return String.Format("structure_root_{0}", TableName); }
        }

        #region Настройки корневого(ых) элемента(ов)
        public enum RootNodeMethod { PreSet, Request, AnotherTable }

        [DataMember]
        [Category("Корневой элемент"), 
        DisplayName("Выбор корня"), 
        Description("Как определяется родительский элементов для данной таблицы?")]
        public RootNodeMethod RootNode { get; set; }

        /// <summary>
        /// Номер таблицы
        /// </summary>
        [DataMember]
        [Category("Корневой элемент"), 
        DisplayName("Связь с другой таблицей"),
        Description()]
        [TypeConverter(typeof(StructureReportTableConverter))]
        public int RootFromAnotherTable { get; set; }

        /// <summary>
        /// ИД корнего узла, начиная с которого требуется искать узлы.
        /// </summary>
        /// <remarks>
        /// Хранение ид узла делает экспорт/импорт не переносимой
        /// При экспорте следует менять ИД на что-то другое
        /// При импорте что-то другое следует менять на ИД
        /// </remarks>
        [DataMember]
        [Browsable(false)]
        public int RootNodeID { get; set; }

        private UnitNode rootUnitNode;
        [Category("Корневой элемент"), 
        DisplayName("Корень"),
        Description("Заданный вручную корневой элемент или родительский элемент для выбора корня во время формирования отчета.")]
        [Editor(typeof(UnitNodeTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public UnitNode RootUnitNode
        {
            get
            {
                if (rootUnitNode == null && RootNodeID > 0)
                {
                    rootUnitNode = new UnitNode() { Idnum = RootNodeID };

                } return rootUnitNode;
            }
            set
            {
                rootUnitNode = value;
                if (rootUnitNode == null)
                {
                    RootNodeID = 0;
                }
                else
                {
                    RootNodeID = rootUnitNode.Idnum;
                }
            }
        }

        ///// <summary>
        ///// Уточнять при формировании отчёта корневой узел
        ///// </summary>
        //public bool RootNodeParameterRequired { get; set; }

        /// <summary>
        /// Минимальный уровень для выбора корнегого элемента.
        /// Если &lt;0, то уровни не учитываются.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public int RootNodeMinLevel
        {
            get
            {
                return RootNodeStructureLevel != null && RootNodeStructureLevel.Enabled ? RootNodeStructureLevel.MinLevel : -1;
            }
            set
            {
                RootNodeStructureLevel = new StructureLevel
                {
                    Enabled = value >= 0,
                    MinLevel = value,
                    MaxLevel = RootNodeMaxLevel
                };
            }
        }

        /// <summary>
        /// Максимальный уровень для выбора корнегого элемента.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public int RootNodeMaxLevel
        {
            get
            {
                return RootNodeStructureLevel.Enabled ? RootNodeStructureLevel.MaxLevel : -1;
            }
            set
            {
                RootNodeStructureLevel = new StructureLevel
                {
                    Enabled = RootNodeMinLevel >= 0,
                    MinLevel = RootNodeMinLevel,
                    MaxLevel = value
                };
            }
        }

        [Category("Корневой элемент"),
        DisplayName("Ограничение корня по уровню"),
        Description("")]
        public StructureLevel RootNodeStructureLevel { get; set; }
        //{
        //    get
        //    {
        //        return new StructureLevel()
        //        {
        //            Enabled = RootNodeMinLevel >= 0,
        //            MinLevel = RootNodeMinLevel,
        //            MaxLevel = RootNodeMaxLevel
        //        };
        //    }
        //    set
        //    {
        //        RootNodeMinLevel = value.Enabled ? value.MinLevel : -1;
        //        RootNodeMaxLevel = value.Enabled ? value.MaxLevel : -1;
        //    }
        //}

        /// <summary>
        /// При уточнении корнего узла, использовать только следующие типы
        /// </summary>
        [DataMember]
        [Category("Корневой элемент"),
        DisplayName("Фильтр типов корня"),
        Description("Фильтр типов для выбора корнего элемента во время формирование отчета.")]
        [TypeConverter(typeof(UnitTypeArrayConverter)), 
        Editor(typeof(UnitTypeArrayEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int[] RootNodeTypeFilter { get; set; }
        #endregion

        /// <summary>
        /// Запрашивать значения
        /// </summary>
        [DataMember]
        [Category("Основное"),
        DisplayName("Запрашивать значения"),
        Description("Запросить при формировании отчета значений для параметров из данной таблицы")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool QueryValues { get; set; }

        /// <summary>
        /// Минимальный уровень искомых элементов.
        /// Если &lt;0, то уровни не учитываются.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public int NodeMinLevel
        {
            get
            {
                return NodeStructureLevel != null && NodeStructureLevel.Enabled ? NodeStructureLevel.MinLevel : -1;
            }
            set { NodeStructureLevel =new StructureLevel
                {
                    Enabled = value >= 0,
                    MinLevel = value,
                    MaxLevel = NodeMaxLevel
                };
            }
        }

        /// <summary>
        /// Минимальный уровень искомых элементов.
        /// </summary>
        [DataMember]
        [Browsable(false)]
        public int NodeMaxLevel
        {
            get
            {
                return NodeStructureLevel.Enabled ? NodeStructureLevel.MaxLevel : -1;
            }
            set
            {
                NodeStructureLevel = new StructureLevel
                {
                    Enabled = NodeMinLevel >= 0,
                    MinLevel = NodeMinLevel,
                    MaxLevel = value
                };
            }
        }

        [Category("Фильтр элементов"),
        DisplayName("Ограничение по уровню"),
        Description()]
        public StructureLevel NodeStructureLevel { get; set; }
        //{
        //    get
        //    {
        //        return new StructureLevel
        //        {
        //            Enabled = NodeMinLevel >= 0,
        //            MinLevel = NodeMinLevel,
        //            MaxLevel = NodeMaxLevel
        //        };
        //    }
        //    set
        //    {
        //        NodeMinLevel = value.Enabled ? value.MinLevel : -1;
        //        NodeMaxLevel = value.Enabled ? value.MaxLevel : -1;
        //    }
        //}

        /// <summary>
        /// Фильтр по типам для передаваемых узлов
        /// </summary>
        [DataMember]
        [Category("Фильтр элементов"),
        DisplayName("Фильтр типов"),
        Description("Фильтр типов элемнтов таблицы.")]
        [TypeConverter(typeof(UnitTypeArrayConverter)),
        Editor(typeof(UnitTypeArrayEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int[] NodeTypeFilter { get; set; }

        /// <summary>
        /// Более сложный фильтр
        /// </summary>
        //public NodeFilter Filter { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            return new StructureReportTable(this);
        }

        #endregion

        #region IManualSerializable Members

        public void FromBytes(byte[] bytes)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        int count;

                        TableName = br.ReadString();

                        RootNode = (RootNodeMethod)br.ReadInt32();

                        RootFromAnotherTable = br.ReadInt32();
                        RootNodeID = br.ReadInt32();
                        RootNodeMinLevel = br.ReadInt32();
                        RootNodeMaxLevel = br.ReadInt32();

                        count = br.ReadInt32();
                        RootNodeTypeFilter = new int[count];
                        for (int i = 0; i < count; i++)
                            RootNodeTypeFilter[i] = br.ReadInt32();

                        QueryValues = br.ReadBoolean();

                        NodeMinLevel = br.ReadInt32();
                        NodeMaxLevel = br.ReadInt32();

                        count = br.ReadInt32();
#if DEBUG
                        if (count > 100)
                            count = 0; 
#endif
                        NodeTypeFilter = new int[count];
                        for (int i = 0; i < count; i++)
                            NodeTypeFilter[i] = br.ReadInt32();

                        //Filter = NodeFilter.Read(br);
                    }
                }
            }
            catch (EndOfStreamException) { }
        }

        public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    int count;

                    bw.Write(TableName);

                    bw.Write((int)RootNode);

                    bw.Write(RootFromAnotherTable);
                    bw.Write(RootNodeID);
                    bw.Write(RootNodeMinLevel);
                    bw.Write(RootNodeMaxLevel);

                    count = RootNodeTypeFilter == null ? 0 : RootNodeTypeFilter.Length;
                    bw.Write(count);
                    for (int i = 0; i < count; i++)
                        bw.Write((int)RootNodeTypeFilter[i]);

                    bw.Write(QueryValues);

                    bw.Write(NodeMinLevel);
                    bw.Write(NodeMaxLevel);

                    count = NodeTypeFilter == null ? 0 : NodeTypeFilter.Length;
                    bw.Write(count);
                    for (int i = 0; i < count; i++)
                        bw.Write((int)NodeTypeFilter[i]);

                    //if (Filter != null)
                    //    Filter.Write(bw);
                }
                return ms.ToArray();
            }
        }

        #endregion

        public override string ToString()
        {
            return TableName;
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    //Editor(typeof(StructureLevelEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class StructureLevel
    {
        //[Browsable(false)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool Enabled { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }

        public override string ToString()
        {
            if (Enabled)
            {
                return String.Format("[{0}; {1}]", MinLevel, MaxLevel);
            }
            return String.Empty;
        }
    }

    //public class NodeFilterConverter : ExpandableObjectConverter
    //{
    //    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
    //    {
    //        PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
    //        List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
    //        StructureReportSourceSettings.NodeFilter filter = value as StructureReportSourceSettings.NodeFilter;

    //        foreach (PropertyDescriptor item in collection)
    //        {
    //            switch (item.Name)
    //            {
    //                case "Operation":
    //                    properties.Add(new NodeFilterPropertyDescriptor(filter, StructureReportSourceSettings.NodeFilterType.Logical, item));
    //                    break;
    //                case "PropertyName":
    //                    properties.Add(new NodeFilterPropertyDescriptor(filter, StructureReportSourceSettings.NodeFilterType.Expression, item));
    //                    break;
    //                case "Expression":
    //                    properties.Add(new NodeFilterPropertyDescriptor(filter, StructureReportSourceSettings.NodeFilterType.Expression, item));
    //                    break;
    //                default:
    //                    properties.Add(item);
    //                    break;
    //            }
    //        }
    //        return new PropertyDescriptorCollection(properties.ToArray());
    //    }

    //    class NodeFilterPropertyDescriptor : SimplePropertyDescriptor
    //    {
    //        StructureReportSourceSettings.NodeFilter filter;
    //        StructureReportSourceSettings.NodeFilterType browsableValue;
    //        PropertyDescriptor baseDescriptor;

    //        public NodeFilterPropertyDescriptor(StructureReportSourceSettings.NodeFilter nodeFilter,
    //            StructureReportSourceSettings.NodeFilterType browsableType,
    //            PropertyDescriptor baseDescriptor)
    //            : base(baseDescriptor.ComponentType, baseDescriptor.Name, baseDescriptor.PropertyType)
    //        {
    //            this.filter = nodeFilter;
    //            this.browsableValue = browsableType;
    //            this.baseDescriptor = baseDescriptor;
    //        }

    //        public override void AddValueChanged(object component, EventHandler handler)
    //        {
    //            baseDescriptor.AddValueChanged(component, handler);
    //        }

    //        public override AttributeCollection Attributes
    //        {
    //            get
    //            {
    //                return baseDescriptor.Attributes;
    //            }
    //        }

    //        public override bool CanResetValue(object component)
    //        {
    //            return baseDescriptor.CanResetValue(component);
    //        }

    //        public override string Category
    //        {
    //            get
    //            {
    //                return baseDescriptor.Category;
    //            }
    //        }

    //        public override Type ComponentType
    //        {
    //            get
    //            {
    //                return baseDescriptor.ComponentType;
    //            }
    //        }

    //        public override TypeConverter Converter
    //        {
    //            get
    //            {
    //                return baseDescriptor.Converter;
    //            }
    //        }

    //        public override string Description
    //        {
    //            get
    //            {
    //                return baseDescriptor.Description;
    //            }
    //        }

    //        public override bool DesignTimeOnly
    //        {
    //            get
    //            {
    //                return baseDescriptor.DesignTimeOnly;
    //            }
    //        }

    //        public override string DisplayName
    //        {
    //            get
    //            {
    //                return baseDescriptor.DisplayName;
    //            }
    //        }

    //        public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
    //        {
    //            return baseDescriptor.GetChildProperties(instance, filter);
    //        }

    //        public override object GetEditor(Type editorBaseType)
    //        {
    //            return baseDescriptor.GetEditor(editorBaseType);
    //        }

    //        public override object GetValue(object component)
    //        {
    //            throw new NotImplementedException();
    //        }

    //        public override bool IsBrowsable
    //        {
    //            get
    //            {
    //                return filter.FilterType == browsableValue;
    //            }
    //        }

    //        public override bool IsLocalizable
    //        {
    //            get
    //            {
    //                return baseDescriptor.IsLocalizable;
    //            }
    //        }

    //        public override bool IsReadOnly
    //        {
    //            get
    //            {
    //                return baseDescriptor.IsReadOnly;
    //            }
    //        }

    //        public override string Name
    //        {
    //            get
    //            {
    //                return baseDescriptor.Name;
    //            }
    //        }

    //        public override Type PropertyType
    //        {
    //            get
    //            {
    //                return baseDescriptor.PropertyType;
    //            }
    //        }

    //        public override void RemoveValueChanged(object component, EventHandler handler)
    //        {
    //            baseDescriptor.RemoveValueChanged(component, handler);
    //        }

    //        public override void ResetValue(object component)
    //        {
    //            baseDescriptor.ResetValue(component);
    //        }

    //        public override void SetValue(object component, object value)
    //        {
    //            throw new NotImplementedException();
    //        }

    //        public override bool ShouldSerializeValue(object component)
    //        {
    //            return baseDescriptor.ShouldSerializeValue(component);
    //        }

    //        public override bool SupportsChangeEvents
    //        {
    //            get
    //            {
    //                return baseDescriptor.SupportsChangeEvents;
    //            }
    //        }
    //    }
    //}

    //public class NodeFilterTypeConverter : EnumConverter
    //{
    //    public NodeFilterTypeConverter()
    //        : base(typeof(StructureReportSourceSettings.NodeFilterType))
    //    {

    //    }

    //    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    //    {
    //        if (destinationType == typeof(String))
    //        {
    //            switch ((StructureReportSourceSettings.NodeFilterType)value)
    //            {
    //                case StructureReportSourceSettings.NodeFilterType.Logical:
    //                    return "Операция";
    //                case StructureReportSourceSettings.NodeFilterType.Expression:
    //                    return "Выражение";
    //            }
    //        } return base.ConvertTo(context, culture, value, destinationType);
    //    }

    //    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    //    {
    //        if (value is String)
    //        {
    //            switch (value.ToString().ToLower())
    //            {
    //                case "операция":
    //                    return StructureReportSourceSettings.NodeFilterType.Logical;
    //                case "выражение":
    //                    return StructureReportSourceSettings.NodeFilterType.Expression;
    //            }
    //        } return base.ConvertFrom(context, culture, value);
    //    }
    //}

    //public class LogicalOperationTypeConverter : EnumConverter
    //{
    //    public LogicalOperationTypeConverter()
    //        : base(typeof(StructureReportSourceSettings.LogicalOperation))
    //    {

    //    }

    //    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    //    {
    //        if (destinationType == typeof(String))
    //        {
    //            switch ((StructureReportSourceSettings.LogicalOperation)value)
    //            {
    //                case StructureReportSourceSettings.LogicalOperation.AND:
    //                    return "И";
    //                case StructureReportSourceSettings.LogicalOperation.OR:
    //                    return "ИЛИ";
    //                case StructureReportSourceSettings.LogicalOperation.NOT:
    //                    return "НЕ";
    //            }
    //        }
    //        return base.ConvertTo(context, culture, value, destinationType);
    //    }

    //    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    //    {
    //        if (value is String)
    //        {
    //            switch (value.ToString().ToLower())
    //            {
    //                case "&&":
    //                case "and":
    //                case "и":
    //                    return StructureReportSourceSettings.LogicalOperation.AND;
    //                case "||":
    //                case "or":
    //                case "или":
    //                    return StructureReportSourceSettings.LogicalOperation.OR;
    //                case "!":
    //                case "not":
    //                case "не":
    //                    return StructureReportSourceSettings.LogicalOperation.NOT;
    //            }
    //        }
    //        return base.ConvertFrom(context, culture, value);
    //    }
    //}

    ///// <summary>
    ///// Контэйнер параметров отчёта
    ///// </summary>
    //public interface IReportParameterContainer
    //{
    //    /// <summary>
    //    /// Получить параметр отчёта по имени
    //    /// </summary>
    //    /// <param name="parameterName">Имя параметра отчёта</param>
    //    /// <returns></returns>
    //    ReportParameter GetParameter(String parameterName);
    //}

    public class StructureParameterTypeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            ReportNode reportNode;
            ReportParametersContainer parameterContainer = context.Instance as ReportParametersContainer;

            if (parameterContainer == null && (reportNode = context.Instance as ReportNode) != null)
            {
                parameterContainer = reportNode.ParameterContainer;
            }

            if (parameterContainer != null)
            {
                StructureReportSourceSettings settings;
                ReportParameter parameter = parameterContainer.GetParameter(context.PropertyDescriptor.Name);
                IStructureRetrieval retrieval = context.GetService(typeof(IStructureRetrieval)) as IStructureRetrieval;

                if (parameter != null && retrieval != null)
                {
                    settings = parameter.BaseSettings as StructureReportSourceSettings;
                    if (settings != null)
                    {
                        StructureReportTable reportTable = settings.GetTable(parameter.Name);

                        if (reportTable != null)
                        {
                            UnitNode rootNode = retrieval.GetUnitNode(reportTable.RootNodeID);
                            if (rootNode != null)
                            {
                                UnitNode[] unitNodes = retrieval.GetChildNodes(rootNode.Idnum, reportTable.RootNodeTypeFilter, reportTable.RootNodeMinLevel, reportTable.RootNodeMaxLevel);

                                foreach (var item in unitNodes)
                                {
                                    idDictionary[item.Idnum] = item.FullName;
                                    nameDictionary[item.FullName] = item.Idnum;
                                }

                                List<int> unitNodeIds = new List<UnitNode>(unitNodes).ConvertAll(n => n.Idnum);
                                return new StandardValuesCollection(unitNodeIds);
                            }
                        }
                    }
                }
            }

            return base.GetStandardValues(context);
        }

        Dictionary<String, int> nameDictionary = new Dictionary<String, int>();
        Dictionary<int, String> idDictionary = new Dictionary<int, String>();

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(String);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            String nodeName;
            if (value is int)
            {
                if (!idDictionary.TryGetValue((int)value, out nodeName))
                {
                    IStructureRetrieval retrieval = context.GetService(typeof(IStructureRetrieval)) as IStructureRetrieval;
                    if (retrieval != null)
                    {
                        UnitNode unitNode = retrieval.GetUnitNode((int)value);
                        idDictionary[unitNode.Idnum] = nodeName = unitNode.FullName;
                        nameDictionary[nodeName] = unitNode.Idnum;
                    }
                }
                if (!String.IsNullOrEmpty(nodeName))
                    return nodeName;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(String);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            //IStructureRetrieval retrieval = context.GetService(typeof(IStructureRetrieval)) as IStructureRetrieval;
            int nodeID;
            if (value != null && nameDictionary.TryGetValue(value.ToString(), out nodeID))
                return nodeID;

            return base.ConvertFrom(context, culture, value);
        }
    }
}

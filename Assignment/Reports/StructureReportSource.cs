using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Assignment.Extension;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Источник данных конструктивных данных отчёта
    /// </summary>
    class StructureReportSource : IReportSource
    {
        ReportSourceInfo info;
        public static readonly Guid StructureReportSourceID = new Guid("{E05CD612-75B7-4cac-9249-F792957F6EC7}");

        IUnitManager unitManager;
        IUnitTypeManager typeManager;
        ISecurityManager securityManager;
        ValueReceiver valueReceiver;
        ExtensionManager extensionManager;

        TimeReportSource valueSource;

        public StructureReportSource(IUnitManager unitManager,
                                     IUnitTypeManager typeManager,
                                     ISecurityManager smanager,
                                     ValueReceiver vreceiver,
                                     ExtensionManager extensionManager)
        {
            info = new ReportSourceInfo(StructureReportSourceID, true, "Конструктивные данные");
            this.unitManager = unitManager;
            this.typeManager = typeManager;
            this.securityManager = smanager;
            this.valueReceiver = vreceiver;
            this.extensionManager = extensionManager;
        }

        #region IReportSource Members

        public ReportSourceInfo Info
        {
            get { return info; }
        }

        public Guid[] References
        {
            get { return new Guid[] { TimeReportSource.TimeReportSourceID }; }
        }

        public void SetReference(IReportSource source)
        {
            TimeReportSource reportSource = source as TimeReportSource;

            if (reportSource != null)
                valueSource = reportSource;
        }

        public ReportSourceSettings CreateSettingsObject()
        {
            StructureReportSourceSettings settings = new StructureReportSourceSettings(info);

            if (valueSource != null)
                settings.ValueSourceSetting = valueSource.Info.ReportSourceId;

            return settings;
        }

        public void GenerateData(OperationState state, DataSet dataSet, ReportSourceSettings settings, params ReportParameter[] reportParameters)
        {
            PropertyManager propertyManager = new PropertyManager(extensionManager, unitManager);
            StructureReportSourceSettings reportSettings = settings as StructureReportSourceSettings;

            if (reportSettings != null)
            {
                Dictionary<String, HashSet<Tuple<int, UnitNode>>> nodesPerTable = new Dictionary<String, HashSet<Tuple<int, UnitNode>>>();
                Dictionary<String, Package[]> packagesPerTable = new Dictionary<String, Package[]>();

                // get nodes for each table
                foreach (var table in reportSettings.Tables)
                {
                    GetTableNodes(reportSettings, table, reportParameters, nodesPerTable);
                }

                // get values
                if (reportSettings.QueryValues && valueSource != null)
                {
                    // get time interval 
                    TimeReportSourceSetting timeReportSettings = settings.GetReference(valueSource.Info.ReportSourceId) as TimeReportSourceSetting;
                    DateTime timeFrom = DateTime.MaxValue, timeTo = DateTime.MinValue;
                    if (timeReportSettings != null)
                        timeReportSettings.GetReportInterval(reportParameters, out timeFrom, out timeTo);

                    foreach (var table in from t in reportSettings.Tables where t.QueryValues select t)
                    {
                        var nodes = nodesPerTable[table.TableName];
                        Package[] packageValues = GetParameterValues(state, timeFrom, timeTo, nodes);

                        packagesPerTable[table.TableName] = packageValues;
                    }
                }

                // create empty tables
                CreateTables(state, dataSet, propertyManager, reportSettings);

                FillTables(state, dataSet, propertyManager, reportSettings, nodesPerTable, packagesPerTable);
            }
        }

        public void GenerateEmptyData(OperationState state, DataSet dataSet, ReportSourceSettings settings)
        {
            PropertyManager propertyManager = new PropertyManager(extensionManager, unitManager);
            StructureReportSourceSettings reportSettings = settings as StructureReportSourceSettings;

            CreateTables(state, dataSet, propertyManager, reportSettings);

            FillDummyTable(from t in reportSettings.Tables
                           where t.RootNode != StructureReportTable.RootNodeMethod.AnotherTable
                           select dataSet.Tables[t.TableName]);
        }

        #endregion

        const String parentIDColumn = "parentTableID";
        const String nodeIdnumColumn = "Idnum";
        const String valueIdColumn = "parameterID";
        const String valueTimeColumn = "time";
        const String valueValueColumn = "value";

        private String GetValuesTableName(String nodeTableName)
        {
            return String.Format("{0}_values", nodeTableName);
        }

        /// <summary>
        /// Создать пустые таблицы для всех таблиц структур и данных и описать связи между ними
        /// </summary>
        /// <param name="state"></param>
        /// <param name="dataSet"></param>
        /// <param name="propertyManager"></param>
        /// <param name="reportSettings"></param>
        private void CreateTables(OperationState state, DataSet dataSet, PropertyManager propertyManager, StructureReportSourceSettings reportSettings)
        {
            // create empty tables
            foreach (var table in reportSettings.Tables)
            {
                IEnumerable<UTypeNode> types;
                if (table.NodeTypeFilter == null || table.NodeTypeFilter.Length == 0)
                {
                    types = typeManager.GetUnitTypes(state);
                }
                else
                {
                    types = (from t in table.NodeTypeFilter select typeManager.GetUnitType(state, t));
                }
                //table.NodeTypeFilter
                HashSet<ItemProperty> itemsList = new HashSet<ItemProperty>();
                foreach (var unitType in types)
                {
                    var properties = propertyManager.GetProperties(state, unitType);
                    foreach (var prop in properties)
                    {
                        itemsList.Add(prop);
                    }
                }
                var dataTable = new DataTable(table.TableName);
                dataSet.Tables.Add(dataTable);
                dataTable.Columns.Add(nodeIdnumColumn, typeof(int));
                if (table.RootNode == StructureReportTable.RootNodeMethod.AnotherTable)
                {
                    dataTable.Columns.Add(parentIDColumn, typeof(int));
                }
                foreach (var item in itemsList)
                {
                    if (!dataTable.Columns.Contains(item.Name))
                    {
                        Type type = item.ValueType;

                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            type = type.GetGenericArguments().First();
                        }

                        if (!type.Namespace.Contains("System"))
                        {
                            type = typeof(String);
                        }

                        dataTable.Columns.Add(item.Name, type);
                    }
                }
                // add empty values table
                if (table.QueryValues)
                {
                    var valueTable = new DataTable(GetValuesTableName(table.TableName));
                    valueTable.Columns.Add(valueTimeColumn, typeof(DateTime));
                    DataColumn idColumn = valueTable.Columns.Add(valueIdColumn, typeof(int));
                    DataColumn valueColumn = valueTable.Columns.Add(valueValueColumn, typeof(double));

                    dataSet.Tables.Add(valueTable);
                    dataSet.Relations.Add(dataTable.Columns[nodeIdnumColumn], idColumn);
                }
            }

            // set structure table relations
            foreach (var table in from t in reportSettings.Tables
                                  where t.RootNode == StructureReportTable.RootNodeMethod.AnotherTable
                                  select t)
            {
                var foreignColumn = dataSet.Tables[table.TableName].Columns[parentIDColumn];
                var keyColumn = dataSet.Tables[reportSettings.Tables[table.RootFromAnotherTable].TableName].Columns[nodeIdnumColumn];

                dataSet.Relations.Add(keyColumn, foreignColumn);
            }
        }

        /// <summary>
        /// Запросить элемента для указанной таблицы.
        /// Полученные элемнты добавляются в nodesPerTable
        /// </summary>
        /// <param name="reportSettings"></param>
        /// <param name="table"></param>
        /// <param name="reportParameters"></param>
        /// <param name="nodesPerTable"></param>
        /// <returns></returns>
        private HashSet<Tuple<int, UnitNode>> GetTableNodes(StructureReportSourceSettings reportSettings,
                                                StructureReportTable table,
                                                ReportParameter[] reportParameters,
                                                Dictionary<String, HashSet<Tuple<int, UnitNode>>> nodesPerTable)
        {
            HashSet<Tuple<int, UnitNode>> nodeSet;
            if (!nodesPerTable.TryGetValue(table.TableName, out nodeSet))
            {
                nodeSet = new HashSet<Tuple<int, UnitNode>>();
                nodesPerTable[table.TableName] = nodeSet;

                switch (table.RootNode)
                {
                    case StructureReportTable.RootNodeMethod.PreSet:
                        foreach (var unitNode in GetUnitNodes(table, table.RootNodeID))
                        {
                            nodeSet.Add(Tuple.Create(table.RootNodeID, unitNode));
                        }
                        break;
                    case StructureReportTable.RootNodeMethod.Request:
                        int rootNodeID;

                        ReportParameter rootNodeParameter = reportParameters.FirstOrDefault(p => p.Name == table.RootParameterName);

                        if (rootNodeParameter != null)
                        {
                            if (rootNodeParameter.GetValue() == null)
                                throw new Exception(String.Format("Не установленно значение параметра '{0}'", rootNodeParameter.DisplayName));
                            rootNodeID = (int)rootNodeParameter.GetValue();
                        }
                        else goto case StructureReportTable.RootNodeMethod.PreSet;

                        foreach (var unitNode in GetUnitNodes(table, rootNodeID))
                        {
                            nodeSet.Add(Tuple.Create(rootNodeID, unitNode));
                        }
                        break;
                    case StructureReportTable.RootNodeMethod.AnotherTable:
                        var parentNodes = GetTableNodes(reportSettings, reportSettings.Tables[table.RootFromAnotherTable], reportParameters, nodesPerTable);

                        foreach (var parentNodeTuple in parentNodes)
                        {
                            var parentNode = parentNodeTuple.Item2;
                            foreach (var unitNode in GetUnitNodes(table, parentNode.Idnum))
                            {
                                nodeSet.Add(Tuple.Create(parentNode.Idnum, unitNode));
                            }
                        }
                        break;
                }
            }
            return nodeSet;
        }

        /// <summary>
        /// Запросить значения параметров для указаных элементов
        /// </summary>
        /// <param name="state"></param>
        /// <param name="timeFrom"></param>
        /// <param name="timeTo"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private Package[] GetParameterValues(OperationState state, DateTime timeFrom, DateTime timeTo, HashSet<Tuple<int, UnitNode>> nodes)
        {
            var parameterList = (from t in nodes select t.Item2).OfType<ParameterNode>();
            OperationState subState = state.Sub();

            valueReceiver.AsyncGetValues(subState,
                                         0f,
                                         (from p in parameterList
                                          select new ParameterValuesRequest()
                                          {
                                              Parameters = new Tuple<ParameterNode, ArgumentsValues>[]
                                                               {
                                                                   Tuple.Create(p, (ArgumentsValues)null)
                                                               },
                                              StartTime = timeFrom,
                                              EndTime = timeTo
                                          }).ToArray(),
                                         true,
                                         true);

            Package[] packageValues;

            if (subState.AsyncResult != null)
                packageValues = (from x in subState.AsyncResult where x is Package select x as Package).ToArray();
            else
                packageValues = new Package[0];
            return packageValues;
        }

        /// <summary>
        /// Заполнить подготовленные таблицы в dataSet'е
        /// полученными данными
        /// </summary>
        /// <param name="state"></param>
        /// <param name="dataSet"></param>
        /// <param name="propertyManager"></param>
        /// <param name="reportSettings"></param>
        /// <param name="nodesPerTable"></param>
        /// <param name="packagesPerTable"></param>
        private void FillTables(OperationState state,
                                DataSet dataSet,
                                PropertyManager propertyManager,
                                StructureReportSourceSettings reportSettings,
                                Dictionary<String, HashSet<Tuple<int, UnitNode>>> nodesPerTable,
                                Dictionary<String, Package[]> packagesPerTable)
        {
            foreach (var tableName in nodesPerTable.Keys)
            {
                FillNodeTable(dataSet, propertyManager, nodesPerTable, tableName);
            }

            foreach (var tableName in packagesPerTable.Keys)
            {
                var dataTable = dataSet.Tables[GetValuesTableName(tableName)];
                var packages = packagesPerTable[tableName];

                FillValuesTable(dataTable, packages);
            }
        }

        /// <summary>
        /// Заполнить таблицу структуры
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="propertyManager"></param>
        /// <param name="nodesPerTable"></param>
        /// <param name="tableName"></param>
        private void FillNodeTable(DataSet dataSet, PropertyManager propertyManager, Dictionary<String, HashSet<Tuple<int, UnitNode>>> nodesPerTable, string tableName)
        {
            var dataTable = dataSet.Tables[tableName];
            var nodes = nodesPerTable[tableName];

            if (dataTable.Rows.Count > 0)
            {
                return;
            }

            bool addParentID = dataTable.ParentRelations.Count > 0;

            if (addParentID)
            {
                // explicit fill parent tables
                foreach (DataRelation parentRelations in dataTable.ParentRelations)
                {
                    FillNodeTable(dataSet, propertyManager, nodesPerTable, parentRelations.ParentTable.TableName);
                }
            }

            foreach (var tuple in nodes)
            {
                var node = tuple.Item2;
                var rootNodeID = tuple.Item1;
                DataRow row = dataTable.NewRow();
                row[nodeIdnumColumn] = node.Idnum;

                if (addParentID)
                    row[parentIDColumn] = rootNodeID;

                ItemProperty[] properties = propertyManager.GetProperties(node);

                if (properties != null)
                {
                    foreach (var property in properties)
                    {
                        try
                        {
                            Object value = property.PropertyValue;
                            Type type = property.ValueType;

                            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                type = type.GetGenericArguments().First();
                            }

                            if (!type.Namespace.Contains("System"))
                            {
                                var converter = System.ComponentModel.TypeDescriptor.GetConverter(property.ValueType);
                                //type = typeof(String);
                                value = converter.ConvertToString(property.PropertyValue);
                            }

                            if (dataTable.Columns.Contains(property.Name))
                            {
                                DataColumn propertyColumn = dataTable.Columns[property.Name];
                                //else
                                //    propertyColumn = dataTable.Columns.Add(property.Name, type);

                                if (property.PropertyValue != null)
                                {
                                    row[propertyColumn] = value;
                                }
                            }
                        }
                        catch (NotSupportedException) { }
                    }
                }

                dataTable.Rows.Add(row);
            }
        }

        /// <summary>
        /// Заполнить таблицу значений
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="packages"></param>
        private void FillValuesTable(DataTable dataTable, Package[] packages)
        {
            Dictionary<DateTime, Dictionary<int, ParamValueItem>> tableDictionary = new Dictionary<DateTime, Dictionary<int, ParamValueItem>>();
            Dictionary<int, ParamValueItem> timeDictionary;

            foreach (var package in packages)
            {
                foreach (var item in package.Values)
                {
                    if (!tableDictionary.TryGetValue(item.Time, out timeDictionary))
                        tableDictionary[item.Time] = timeDictionary = new Dictionary<int, ParamValueItem>();

                    timeDictionary[package.Id] = item;
                }
            }

            var timeList = new List<DateTime>(tableDictionary.Keys);
            timeList.Sort();

            foreach (var time in timeList)
            {
                timeDictionary = tableDictionary[time];
                foreach (var parameterID in timeDictionary.Keys)
                {
                    DataRow dataRow = dataTable.Rows.Add();
                    dataRow[valueIdColumn] = parameterID;
                    dataRow[valueTimeColumn] = time;
                    dataRow[valueValueColumn] = timeDictionary[parameterID].Value;
                }
            }
        }

        /// <summary>
        /// Заполнить таблицы и все их зависимости 
        /// </summary>
        /// <param name="dataTable"></param>
        private void FillDummyTable(IEnumerable<DataTable> dataTables)
        {
            int counter = 0;
            Random valueGenerator = new Random();

            foreach (var dataTable in dataTables)
            {
                FillDummyTable(dataTable, valueGenerator, 0, ref counter); 
            }
        }

        /// <summary>
        /// Заполнить таблицу и все ее зависимости
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="valueGenerator"></param>
        /// <param name="parentRowId"></param>
        /// <param name="counter"></param>
        private void FillDummyTable(DataTable dataTable, Random valueGenerator, int parentRowId, ref int counter)
        {
            const int nodeCount = 5;
            const int valueCount = 5;

            for (int i = 0; i < nodeCount; i++)
            {
                int rowID = ++counter;
                var dataRow = dataTable.NewRow();
                dataRow[nodeIdnumColumn] = rowID;

                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    if (dataColumn.ColumnName == parentIDColumn)
                    {
                        dataRow[dataColumn.ColumnName] = parentRowId;
                    }
                    else if (dataColumn.ColumnName != nodeIdnumColumn
                        && dataColumn.ColumnName != "Idnum")
                    {
                        dataRow[dataColumn.ColumnName] = CreateDummyPropertyValues(dataColumn.ColumnName, dataColumn.DataType, rowID);
                    }
                }
                dataTable.Rows.Add(dataRow);
                foreach (DataRelation relation in dataTable.ChildRelations)
                {
                    DataTable childTable = relation.ChildTable;
                    if (childTable.TableName.EndsWith("_values"))
                    {
                        // values table
                        Interval interval = Interval.Hour;
                        DateTime nowTime = interval.NearestEarlierTime(DateTime.Now);

                        for (int j = 0; j < valueCount; j++)
                        {
                            //int valueRowId = ++valueCounter;
                            DataRow valueRow = childTable.NewRow();
                            valueRow[valueIdColumn] = rowID;
                            valueRow[valueTimeColumn] = interval.GetTime(nowTime, j - valueCount);
                            valueRow[valueValueColumn] = valueGenerator.NextDouble();

                            childTable.Rows.Add(valueRow);
                        }
                    }
                    else
                    {
                        // sub-structure table
                        FillDummyTable(childTable, valueGenerator, rowID, ref counter);
                    }
                }
            }
        }

        /// <summary>
        /// Создать значения свойства по типу и номеру строки
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="type"></param>
        /// <param name="counter"></param>
        /// <returns></returns>
        private object CreateDummyPropertyValues(string columnName, Type type, int counter)
        {
            if (type == typeof(String))
            {
                return String.Format("{0}_{1}", columnName, counter);
            }
            else if (type == typeof(int))
            {
                return new Random(counter).Next();
            }
            else if (type == typeof(double))
            {
                return new Random(counter).NextDouble();
            }
            else if (type == typeof(bool))
            {
                return new Random(counter).Next(0, 100) > 50;
            }
            else if (type == typeof(DateTime))
            {
                return DateTime.Now;
            }
            else if (type == typeof(Interval))
            {
                return Interval.Hour.ToString();
            }
            return DBNull.Value;
        }

        private UnitNode[] GetUnitNodes(StructureReportTable table, int rootNodeID)
        {
            UnitNode[] nodes;
            nodes = unitManager.GetAllUnitNodes(new OperationState(securityManager.InternalSession),
                                                rootNodeID,
                                                table.NodeTypeFilter,
                                                table.NodeMinLevel,
                                                table.NodeMaxLevel,
                                                Privileges.Read);

            return nodes;
        }
    }

    /// <summary>
    /// Класс для обращения к свойствам узлов
    /// </summary>
    class PropertyManager
    {
        IUnitManager unitManager;

        ExtensionManager extensionManager;

        CustomTypeDescriptorContext context;

        Dictionary<Guid, System.ComponentModel.TypeConverter> typeConverterCache = new Dictionary<Guid, System.ComponentModel.TypeConverter>();

        public PropertyManager(ExtensionManager extensionManager, IUnitManager unitManager)
        {
            this.unitManager = unitManager;
            this.extensionManager = extensionManager;
            context = new CustomTypeDescriptorContext(this.extensionManager);
        }

        /// <summary>
        /// Получить свойства и их текущие значения узла
        /// </summary>
        /// <param name="unitNode">Узел, чьи свойства запрашиваются</param>
        /// <returns></returns>
        public ItemProperty[] GetProperties(UnitNode unitNode)
        {
            System.ComponentModel.TypeConverter converter;
            Type unitNodeType = unitNode.GetType();

            // Получить TypeConverter для данного узла
            if (!typeConverterCache.TryGetValue(unitNodeType.GUID, out converter))
            {
                Type typeConverterType = null;

                foreach (var item in unitNodeType.GetCustomAttributes(true))
                {
                    System.ComponentModel.TypeConverterAttribute attribute = item as System.ComponentModel.TypeConverterAttribute;
                    if (attribute != null)
                    {
                        typeConverterType = Type.GetType(attribute.ConverterTypeName);
                    }
                }

                converter = typeConverterType.GetConstructor(new Type[0]).Invoke(null) as System.ComponentModel.TypeConverter;
                typeConverterCache[unitNodeType.GUID] = converter;
            }

            if (converter != null)
            {
                var props = converter.GetProperties(context, unitNode);

                List<ItemProperty> properties = new List<ItemProperty>();
                ItemProperty property;
                foreach (System.ComponentModel.PropertyDescriptor item in props)
                {
                    if (item.IsBrowsable)
                    {
                        Object value = item.GetValue(unitNode);
                        if (DBNull.Value == value)
                            value = null;
                        property = new ItemProperty()
                        {
                            Name = item.Name,
                            DisplayName = item.DisplayName,
                            Description = item.Description,
                            Category = item.Category,
                            //DefaultValue = (value ?? String.Empty).ToString(),
                            PropertyValue = value,
                            ValueType = item.PropertyType
                        };
                        properties.Add(property);
                    }
                }
                return properties.ToArray();
            }

            return null;
        }

        public IEnumerable<ItemProperty> GetProperties(OperationState state, UTypeNode unitType)
        {
            var node = unitManager.NewInstance(state, unitType.Idnum);

            return GetProperties(node);
        }

        /// <summary>
        /// Вспомогательный класс для тайпконвертеров, реализующий всякие интерфейсы
        /// </summary>
        class CustomTypeDescriptorContext : System.ComponentModel.ITypeDescriptorContext,
            COTES.ISTOK.Extension.IExternalsSupplier
        {
            ExtensionManager extensionManager;

            public CustomTypeDescriptorContext(ExtensionManager extensionManager)
            {
                this.extensionManager = extensionManager;
            }

            #region ITypeDescriptorContext Members

            public System.ComponentModel.IContainer Container
            {
                get { return null; }
            }

            public object Instance
            {
                get { return null; }
            }

            public void OnComponentChanged()
            {
            }

            public bool OnComponentChanging()
            {
                return false;
            }

            public System.ComponentModel.PropertyDescriptor PropertyDescriptor
            {
                get { return null; }
            }

            #endregion

            #region IServiceProvider Members

            public object GetService(Type serviceType)
            {
                if (serviceType.IsInstanceOfType(this))
                {
                    return this;
                }
                return null;
            }

            #endregion

            #region IExternalsSupplier Members

            bool COTES.ISTOK.Extension.IExternalsSupplier.ExternalCodeSupported(COTES.ISTOK.Extension.ExtensionUnitNode unitNode)
            {
                return extensionManager.ExternalCodeSupported(unitNode);
            }

            bool COTES.ISTOK.Extension.IExternalsSupplier.ExternalIDSupported(COTES.ISTOK.Extension.ExtensionUnitNode unitNode)
            {
                return extensionManager.ExternalIDSupported(unitNode);
            }

            bool COTES.ISTOK.Extension.IExternalsSupplier.ExternalIDCanAdd(COTES.ISTOK.Extension.ExtensionUnitNode unitNode)
            {
                return extensionManager.ExternalIDCanAdd(unitNode);
            }

            EntityStruct[] COTES.ISTOK.Extension.IExternalsSupplier.GetExternalIDList(COTES.ISTOK.Extension.ExtensionUnitNode unitNode)
            {
                return extensionManager.GetExternalIDList(unitNode);
            }

            ItemProperty[] COTES.ISTOK.Extension.IExternalsSupplier.GetExternalProperties(COTES.ISTOK.Extension.ExtensionUnitNode unitNode)
            {
                return extensionManager.GetExternalProperties(unitNode);
            }

            #endregion
        }
    }
}

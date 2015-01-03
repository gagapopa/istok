using System;
using System.Collections.Generic;
using System.Threading;
using COTES.ISTOK;
using System.Xml;
using System.IO;
using COTES.ISTOK.ASC;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Globalization;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Отдельное сереализация/десереализация для отдельных свойств
    /// </summary>
    interface IPropertyXML
    {
        /// <summary>
        /// Сереализовать значение свойства в XML
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="service"></param>
        /// <param name="unitNode"></param>
        /// <param name="propertyName">Имя свойства</param>
        void Serialize(
            XmlWriter writer,
            IExportService service,
            UnitNode unitNode,
            String propertyName);

        /// <summary>
        /// Десереализовать значение свойства из XML
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="service"></param>
        /// <param name="revisionList">Список ревизий прочитанных из файла импорта</param>
        /// <param name="unitNode"></param>
        /// <param name="propertyName">Имя свойства</param>
        /// <exception cref="ArgumentException">Если указанной ИД ревизии не найденно в revisionList</exception>
        void Deserialize(
            XmlReader reader,
            IExportService service,
            List<RevisionInfo> revisionList,
            UnitNode unitNode,
            String propertyName);
    }

    /// <summary>
    /// Преобразование XML свойство владелец
    /// </summary>
    class OwnerPropertyXML : IPropertyXML
    {
        #region IPropertyXML Members

        public void Serialize(
            XmlWriter writer,
            IExportService service,
            UnitNode unitNode,
            String propertyName)
        {
            RevisedStorage<String> storage = unitNode.Attributes[propertyName];

            String ownerClass, ownerValue;
            int GID;// = int.Parse(value.ToString());

            foreach (var revision in storage)
            {
                GID = int.Parse(storage.Get(revision).ToString());
                service.OwnerToString(GID, out ownerClass, out ownerValue);

                writer.WriteStartElement(XmlExporter.propertyElement);
                writer.WriteAttributeString(XmlExporter.propertyElementNameAttribute, propertyName);
                writer.WriteAttributeString(XmlExporter.propertyElementTypeAttribute, "string");
                if (revision != RevisionInfo.Default)
                    writer.WriteAttributeString(XmlExporter.propertyElementRevisionAttribute, revision.ID.ToString());

                writer.WriteAttributeString(XmlExporter.propertyElementOwnerClassAttribute, ownerClass);
                if (!String.IsNullOrEmpty(ownerValue))
                    writer.WriteValue(ownerValue);
                writer.WriteEndElement(); 
            }
        }

        public void Deserialize(
            XmlReader reader,
            IExportService service,
            List<RevisionInfo> revisionList,
            UnitNode unitNode,
            String propertyName)
        {
            String name = propertyName;
            String cls = null;
            RevisionInfo revision = RevisionInfo.Default;

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    String attributeName = reader.Name.ToLower();
                    if (attributeName == XmlExporter.propertyElementNameAttribute)
                    {
                        name = reader.Value;
                    }
                    else if (attributeName == XmlExporter.propertyElementOwnerClassAttribute)
                    {
                        cls = reader.Value.ToLower();
                    }
                    else if (attributeName == XmlExporter.propertyElementRevisionAttribute)
                    {
                        int revisionID=int.Parse(reader.Value);
                        revision = revisionList.Find(r => r.ID == revisionID);
                        if (revision == null)
                            throw new ArgumentException(String.Format("Для свойства {0} не найденно требуемой ревизии {1} в свойстве RevisionList", propertyName, revisionID));
                    }
                } while (reader.MoveToNextAttribute());
            }

            reader.MoveToElement();

            if (!String.IsNullOrEmpty(cls))
            {
                String value = reader.ReadElementContentAsString();

                value = service.OwnerFromString(cls, value).ToString();

                unitNode.SetAttribute(name, revision, value);
            }
        }

        #endregion
    }

    /// <summary>
    /// Преобразование XML свойств по умолчанию
    /// </summary>
    class DefaultPropertyXML : IPropertyXML
    {
        #region IPropertyXML Members

        public void Serialize(
            XmlWriter writer,
            IExportService service,
            UnitNode unitNode,
            String propertyName)
        {
            RevisedStorage<String> stringStorage;
            RevisedStorage<byte[]> blobStorage;

            if (unitNode.Attributes.TryGetValue(propertyName, out stringStorage))
            {
                foreach (var revision in stringStorage)
                {
                    writer.WriteStartElement(XmlExporter.propertyElement);
                    writer.WriteAttributeString(XmlExporter.propertyElementNameAttribute, propertyName);
                    writer.WriteAttributeString(XmlExporter.propertyElementTypeAttribute, "string");
                    if (revision != RevisionInfo.Default)
                        writer.WriteAttributeString(XmlExporter.propertyElementRevisionAttribute, revision.ID.ToString());

                    writer.WriteValue(stringStorage.Get(revision));

                    writer.WriteEndElement();
                }
            }
            else if (unitNode.Binaries.TryGetValue(propertyName, out blobStorage))
            {
                foreach (var revision in blobStorage)
                {
                    writer.WriteStartElement(XmlExporter.propertyElement);
                    writer.WriteAttributeString(XmlExporter.propertyElementNameAttribute, propertyName);
                    writer.WriteAttributeString(XmlExporter.propertyElementTypeAttribute, "blob"); 
                    if (revision != RevisionInfo.Default)
                        writer.WriteAttributeString(XmlExporter.propertyElementRevisionAttribute, revision.ID.ToString());

                    byte[] binaryData = blobStorage.Get(revision);
                    writer.WriteBinHex(binaryData, 0, binaryData.Length);

                    writer.WriteEndElement();
                }
            }
        }

        public void Deserialize(
            XmlReader reader,
            IExportService service,
            List<RevisionInfo> revisionList,
            UnitNode unitNode,
            String propertyName)
        {
            String name = propertyName, type = "string";
            RevisionInfo revision = RevisionInfo.Default;

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    String attributeName = reader.Name.ToLower();
                    if (attributeName == XmlExporter.propertyElementNameAttribute)
                    {
                        name = reader.Value;
                    }
                    else if (attributeName == XmlExporter.propertyElementTypeAttribute)
                    {
                        type = reader.Value.ToLower();
                    }
                    else if (attributeName == XmlExporter.propertyElementRevisionAttribute)
                    {
                        int revisionID = int.Parse(reader.Value);
                        revision = revisionList.Find(r => r.ID == revisionID);
                        if (revision == null)
                            throw new ArgumentException(String.Format("Для свойства {0} не найденно требуемой ревизии {1} в свойстве RevisionList", propertyName, revisionID));
                    }
                } while (reader.MoveToNextAttribute());
            }

            reader.MoveToElement();

            if (type == "blob")
            {
                int rd;
                byte[] buf = new byte[1 << 10];
                Stream tmpbuf = new MemoryStream();
                while ((rd = reader.ReadElementContentAsBinHex(buf, 0, buf.Length)) > 0)
                {
                    tmpbuf.Write(buf, 0, rd);
                }
                buf = new byte[tmpbuf.Length];
                tmpbuf.Position = 0;
                tmpbuf.Read(buf, 0, buf.Length);

                unitNode.SetBinaries(name, revision, buf);
            }
            else
            {
                String value = reader.ReadElementContentAsString();
                unitNode.SetAttribute(name, revision, value);
            }
        }

        #endregion
    }
    
    /// <summary>
    /// Реализация экспорта/импорта для XML
    /// </summary>
    class XmlExporter : Exporter
    {
        System.ComponentModel.TypeConverter unitTypeEnumConverter = new System.ComponentModel.EnumConverter(typeof(UnitTypeId));
        System.ComponentModel.TypeConverter qualityEnumConverter = new System.ComponentModel.EnumConverter(typeof(Quality));

        IExportService service;

        bool useZip;

        UltimateZipper zipper;

        Dictionary<String, IPropertyXML> propertyXMLCollection;
        
        IPropertyXML defaultPropertyXML;

        public XmlExporter(IExportService service, bool useZip)
        {
            this.useZip = useZip;
            this.service = service;

            propertyXMLCollection = new Dictionary<String, IPropertyXML>();
            propertyXMLCollection["owner"] = new OwnerPropertyXML();
            defaultPropertyXML = new DefaultPropertyXML();

            zipper = new UltimateZipper();
            zipper.UseGZip = true;
        }

        private double TimeRangeCount(DateTime startTime, DateTime endTime)
        {
            return Math.Min(0, endTime.Subtract(startTime).TotalDays);
        }

        private double getDeserializeProgress(Stream stream)
        {
            GZipStream gzipStream;
            if ((gzipStream = stream as GZipStream) != null)
                return getDeserializeProgress(gzipStream.BaseStream);
            return (double)stream.Position / stream.Length * 100;
        }

        private IPropertyXML GetPropertyXML(String propertyName)
        {
            IPropertyXML propertyXML;

            if (!propertyXMLCollection.TryGetValue(propertyName, out propertyXML))
                propertyXML = defaultPropertyXML;

            return propertyXML;
        }

        #region String Constants
        internal const String rootElement = "InfoSysStructure";

        internal const String revisionElement = "revision";
        internal const String revisionElementIDAttribute = "id";
        internal const String revisionElementTimeAttribute = "time";
        internal const String revisionElementBriefAttribute = "brief";
        internal const String revisionElementCommentAttribute = "comment";

        internal const String nodeElement = "node";
        internal const String nodeElementNameAttribute = "name";
        internal const String nodeElementTypeAttribute = "type";

        internal const String parameterElement = "parameter";
        internal const String parameterElementCodeAttribute = "parameter_code";

        internal const String propertyElement = "property";
        internal const String propertyElementNameAttribute = "name";
        internal const String propertyElementTypeAttribute = "type";
        internal const String propertyElementRevisionAttribute = "revision";
        internal const String propertyElementOwnerClassAttribute = "owner";

        internal const String parameterValuesElement = "parameter_values";
        internal const String parameterValuesElementCodeAttribute = "code";
        internal const String parameterValuesElementIDAttribute = "id";

        internal const String packageElement = "package";
        internal const String packageElementStartTimeAttribute = "time_start";
        internal const String packageElementEndTimeAttribute = "time_end";
        internal const String packageElementIntervalAttribute = "interval";

        internal const String valueElement = "value";
        internal const String valueElementTimeAttribute = "time";
        internal const String valueElementChangeTimeAttribute = "change_time";
        internal const String valueElementQualiteAttribute = "quality";
        internal const String valueElementOriginalValueAttribute = "original_value";
        #endregion

        #region Serialize()
        public override byte[] Serialize(OperationState state, TreeWrapp<UnitNode>[] nodes, int nodeCount, ParameterNode[] parameters, DateTime startTime, DateTime endTime)
        {
            byte[] buf;

            using (MemoryStream str = new MemoryStream())
            {
                if (useZip)
                {
                    using (GZipStream gstr = new GZipStream(str, CompressionMode.Compress))
                    {
                        WriteXML(state, nodes, nodeCount, parameters, startTime, endTime, gstr);
                    }
                }
                else WriteXML(state, nodes, nodeCount, parameters, startTime, endTime, str);

                buf = str.ToArray();

                return buf;
            }
        }

        private void WriteXML(
            OperationState state,
            TreeWrapp<UnitNode>[] nodes,
            int nodeCount,
            ParameterNode[] parameters,
            DateTime startTime,
            DateTime endTime,
            Stream str)
        {
            double delta;
            //int factor = 3;
            XmlWriterSettings setts = new XmlWriterSettings();

            setts.NewLineChars = "\n";
            setts.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(str, setts))
            {
                state.Progress = 0;

                double count = nodeCount;

                if (parameters != null)
                    count += parameters.Length * TimeRangeCount(startTime, endTime);

                delta = 100.0 / count;

                writer.WriteStartDocument();
                writer.WriteStartElement(rootElement);

                List<RevisionInfo> revisionList = new List<RevisionInfo>();
                GetRevisions(nodes, revisionList);
                revisionList.Sort((a, b) => a.Time.CompareTo(b.Time));

                SerializeRevisions(state, writer, revisionList);

                SerializeNodes(state, writer, nodes, delta);

                SerializeValues(state, writer, parameters, startTime, endTime, delta);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void GetRevisions(TreeWrapp<UnitNode>[] nodes, List<RevisionInfo> revisionList)
        {
            foreach (var unitNode in nodes)
            {
                foreach (var revision in unitNode.Item.GetRevisions())
                {
                    if (revision != RevisionInfo.Default
                        && !revisionList.Contains(revision))
                    {
                        revisionList.Add(revision);
                    }
                }
                if (unitNode.ChildNodes != null)
                    GetRevisions(unitNode.ChildNodes, revisionList);
            }
        }

        private void SerializeRevisions(
            OperationState state, 
            XmlWriter writer, 
            IEnumerable<RevisionInfo> revisionList)
        {
            foreach (var revision in revisionList)
            {
                writer.WriteStartElement(revisionElement);
                writer.WriteAttributeString(revisionElementIDAttribute, revision.ID.ToString());
                writer.WriteAttributeString(revisionElementTimeAttribute, revision.Time.ToString());
                writer.WriteAttributeString(revisionElementBriefAttribute, revision.Brief);
                writer.WriteAttributeString(revisionElementCommentAttribute, revision.Comment);

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Сериализовать узлы
        /// </summary>
        /// <param name="writer">экземпляр класс XmlWriter</param>
        /// <param name="list">Список узлов</param>
        /// <param name="delta">процент 1 узла, по отношению к их общему числу</param>
        private void SerializeNodes(
            OperationState state,
            XmlWriter writer,
            TreeWrapp<UnitNode>[] list,
            double delta)
        {
            int i;
            for (i = 0; i < list.Length; i++)
            {
                TreeWrapp<UnitNode> node = list[i];
                String name = node.Item.Text;

                writer.WriteStartElement(nodeElement);
                writer.WriteAttributeString(nodeElementNameAttribute, name);
                String nodeTypeString = unitTypeEnumConverter.ConvertToInvariantString((UnitTypeId)node.Item.Typ); 
                writer.WriteAttributeString(nodeElementTypeAttribute, nodeTypeString);
                foreach (String key in node.Item.Attributes.Keys)
                {
                    IPropertyXML propertyXML = GetPropertyXML(key);

                    propertyXML.Serialize(writer, service, node.Item, key);
                }
                if (node.Item.Binaries != null)
                    foreach (String key in node.Item.Binaries.Keys)
                    {
                        IPropertyXML propertyXML = GetPropertyXML(key);

                        propertyXML.Serialize(writer, service, node.Item, key);
                   }
                if (node.Item.Parameters != null)
                {
                    for (int j = 0; j < node.Item.Parameters.Length; j++)
                    {
                        String parameterCode = service.GetParameterCode(state, node.Item.Parameters[j].ParameterId);

                        if (String.IsNullOrEmpty(parameterCode))
                        {
                            state.AddMessage(new Message(MessageCategory.Error, "Параметр '{0}' должен иметь код для экспорта (используется в '{1}')", node.Item.Parameters[j].ParameterId.ToString(), node.Item.Text));
                        }
                        else
                        {
                            writer.WriteStartElement(parameterElement);
                            writer.WriteAttributeString(parameterElementCodeAttribute, parameterCode);
                            foreach (String attr in node.Item.Parameters[j].Attributes.Keys)
                            {
                                writer.WriteStartElement(propertyElement);
                                writer.WriteAttributeString(propertyElementNameAttribute, attr);
                                writer.WriteAttributeString(propertyElementTypeAttribute, "string");
                                String val = node.Item.Parameters[j].Attributes[attr].ToString();
                                if (!String.IsNullOrEmpty(val)) writer.WriteValue(val);
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                    }
                }
                if (node.ChildNodes != null)
                    SerializeNodes(state, writer, node.ChildNodes, delta);
                writer.WriteEndElement();
                state.Progress += delta;
            }
        }

        /// <summary>
        /// Сериализовать значения параметров
        /// </summary>
        /// <param name="writer">экземпляр класс XmlWriter</param>
        /// <param name="list">Список узлов</param>
        /// <param name="delta">процент 1 узла, по отношению к их общему числу</param>
        private void SerializeValues(
            OperationState state,
            XmlWriter writer,
            ParameterNode[] unitNodes,
            DateTime startTime,
            DateTime endTime,
            double delta)
        {
            double parameterDelta = TimeRangeCount(startTime, endTime) * delta;

            foreach (ParameterNode parameterNode in unitNodes)
            {
                double curProggres = state.Progress;
                var packages = service.GetValues(state, parameterNode, startTime, endTime);

                bool writePackageAsBinary = false;

                writer.WriteStartElement(parameterValuesElement);
                if (!String.IsNullOrEmpty(parameterNode.Code))
                    writer.WriteAttributeString(parameterValuesElementCodeAttribute, parameterNode.Code);
                else writer.WriteAttributeString(parameterValuesElementIDAttribute, parameterNode.Idnum.ToString());

                foreach (Package pack in packages)
                {
                    if (writePackageAsBinary)
                    {
                        PackedPackage packedPackage = zipper.Pack(pack);
                        writer.WriteStartElement(packageElement);
                        writer.WriteAttributeString(packageElementStartTimeAttribute, packedPackage.DateFrom.ToString());
                        writer.WriteAttributeString(packageElementEndTimeAttribute, packedPackage.DateTo.ToString());
                        //writer.WriteAttributeString(packageElementIntervalAttribute, packedPackage.Interval.ToDouble().ToString(NumberFormatInfo.InvariantInfo));
                        writer.WriteAttributeString(packageElementIntervalAttribute, packedPackage.Interval.ToString());

                        writer.WriteBinHex(packedPackage.Buffer, 0, packedPackage.Buffer.Length);

                        writer.WriteEndElement();
                    }
                    else
                    {
                        foreach (ParamValueItem valueItem in pack.Values)
                        {
                            CorrectedParamValueItem correctedValueItem;
                            writer.WriteStartElement(valueElement);

                            writer.WriteAttributeString(valueElementTimeAttribute, valueItem.Time.ToString());
                            writer.WriteAttributeString(valueElementChangeTimeAttribute, valueItem.ChangeTime.ToString());
                            writer.WriteAttributeString(valueElementQualiteAttribute, valueItem.Quality.ToString());
                            if ((correctedValueItem = valueItem as CorrectedParamValueItem) != null)
                                writer.WriteAttributeString(valueElementOriginalValueAttribute, correctedValueItem.OriginalValueItem.Value.ToString(NumberFormatInfo.InvariantInfo));

                            writer.WriteValue(valueItem.Value);
                            writer.WriteEndElement();
                        }
                    }
                    state.Progress += TimeRangeCount(pack.DateFrom, pack.DateTo) * delta;
                }
                writer.WriteEndElement();
                state.Progress = curProggres + parameterDelta;
            }
        } 
        #endregion

        #region Deserialize()

        public override ImportDataContainer Deserialize(OperationState state, byte[] importBuffer)
        {
            using (Stream stream = new MemoryStream(importBuffer))
            {
                if (useZip)
                {
                    using (GZipStream gstream = new GZipStream(stream, CompressionMode.Decompress))
                        return ReadXML(state, gstream);
                }
                else return ReadXML(state, stream);
            }
        }

        private ImportDataContainer ReadXML(OperationState state, Stream stream)
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                TreeWrapp<UnitNode> root = new TreeWrapp<UnitNode>(null);
                Dictionary<String, List<Package>> valuesDictionary = new Dictionary<String, List<Package>>();
                List<RevisionInfo> revisionList = new List<RevisionInfo>();
                
                revisionList.Add(RevisionInfo.Default);

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element
                        && reader.Name.Equals(rootElement, StringComparison.InvariantCultureIgnoreCase))
                    {
                        while (true)
                        {
                            // переходим к началу элемента
                            while (reader.Read() && reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement) ;

                            // выход из цикла
                            if (reader.NodeType == XmlNodeType.EndElement || reader.NodeType == XmlNodeType.None)
                                break;
                            else if (reader.Name.Equals(revisionElement, StringComparison.InvariantCultureIgnoreCase))
                            {
                                DeserializeRevision(state, reader, stream, revisionList);
                            }
                            else if (reader.Name.Equals(nodeElement, StringComparison.InvariantCultureIgnoreCase))
                            {
                                DeserializeNode(state, reader, stream, root, revisionList);
                            }
                            else if (reader.Name.Equals(parameterValuesElement, StringComparison.InvariantCultureIgnoreCase))
                            {
                                DeserealizeValues(state, reader, stream, valuesDictionary);
                            }
                            state.Progress = getDeserializeProgress(stream);
                        }
                    }
                }

                Dictionary<String, Package[]> retValuesDictionary = new Dictionary<string, Package[]>();
                foreach (var item in valuesDictionary.Keys)
                {
                    retValuesDictionary[item] = valuesDictionary[item].ToArray();
                }

                return new ImportDataContainer(root.ChildNodes, retValuesDictionary);
            }
        }

        private void DeserealizeValues(OperationState state, XmlReader reader, Stream stream, Dictionary<String, List<Package>> valuesDictionary)
        {
            Package pack;
            String code=null;
            int id;

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    String attributeName = reader.Name.ToLower();
                    if (attributeName == parameterValuesElementCodeAttribute) code = reader.Value;
                    else if (attributeName == parameterValuesElementIDAttribute) id = int.Parse(reader.Value);
                } while (reader.MoveToNextAttribute());
            }

            reader.MoveToElement();
            if (reader.IsEmptyElement)
                return;
            
            List<Package> packageList;
            if (!String.IsNullOrEmpty(code))
            {
                if (!valuesDictionary.TryGetValue(code, out packageList))
                    valuesDictionary[code] = packageList = new List<Package>();


                pack = new Package();

                while (true)
                {
                    while (reader.Read() && reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement) ;

                    // выход из цикла
                    if (reader.NodeType == XmlNodeType.EndElement || reader.NodeType == XmlNodeType.None)
                        break;
                    else if (reader.Name.Equals(packageElement, StringComparison.InvariantCultureIgnoreCase))
                    {
                        DateTime startTime = DateTime.MinValue, endTime = DateTime.MaxValue;
                        Interval interval = Interval.Zero;
                        do
                        {
                            String attributeName = reader.Name.ToLower();
                            try
                            {
                                if (attributeName == packageElementStartTimeAttribute)
                                {
                                    startTime = Convert.ToDateTime(reader.Value);
                                }
                                else if (attributeName == packageElementEndTimeAttribute)
                                {
                                    endTime = Convert.ToDateTime(reader.Value);
                                }
                                else if (attributeName == packageElementIntervalAttribute)
                                {
                                    //interval = new Interval(double.Parse(reader.Value, NumberFormatInfo.InvariantInfo));
                                    interval = Interval.FromString(reader.Value);
                                }
                            }
                            catch (FormatException) { }
                        } while (reader.MoveToNextAttribute());

                        reader.MoveToContent();

                        Package package;
                        PackedPackage packedPackage = new PackedPackage(0, startTime, endTime, Interval.Zero);
                        using (Stream tmpbuf = new MemoryStream())
                        {
                            int rd;
                            byte[] buf = new byte[1 << 10];
                            while ((rd = reader.ReadElementContentAsBinHex(buf, 0, buf.Length)) > 0)
                            {
                                tmpbuf.Write(buf, 0, rd);
                            }
                            buf = new byte[tmpbuf.Length];
                            tmpbuf.Position = 0;
                            tmpbuf.Read(buf, 0, buf.Length);
                            packedPackage.Buffer = buf;
                        }
                        package = zipper.Unpack(packedPackage);
                        packageList.Add(package);
                    }
                    else if (reader.Name.Equals(valueElement, StringComparison.InvariantCultureIgnoreCase))
                    {
                        DateTime time = DateTime.MinValue, change_time = DateTime.MinValue;
                        Quality quality = Quality.Good;
                        double value = 0.0;
                        double originalValue = double.NaN;
                        bool isCorrected = false;

                        if (reader.MoveToFirstAttribute())
                        {
                            do
                            {
                                String attributeName = reader.Name.ToLower();
                                try
                                {
                                    if (attributeName == valueElementTimeAttribute)
                                    {
                                        time = Convert.ToDateTime(reader.Value);
                                    }
                                    else if (attributeName == valueElementChangeTimeAttribute)
                                    {
                                        change_time = Convert.ToDateTime(reader.Value);
                                    }
                                    else if (attributeName == valueElementQualiteAttribute)
                                    {
                                        quality = (Quality)qualityEnumConverter.ConvertFromString(reader.Value);
                                    }
                                    else if (attributeName == valueElementOriginalValueAttribute)
                                    {
                                        isCorrected = true;
                                        originalValue = Convert.ToDouble(reader.Value);
                                    }
                                }
                                catch (FormatException) { }
                            } while (reader.MoveToNextAttribute());
                        }

                        reader.MoveToContent();
                        value = reader.ReadElementContentAsDouble();

                        ParamValueItem receiveItem;

                        if (isCorrected)
                            receiveItem = new CorrectedParamValueItem(new ParamValueItem(time, quality, originalValue), value);
                        else
                            receiveItem = new ParamValueItem(time, quality, value);

                        receiveItem.ChangeTime = change_time;

                        pack.Add(receiveItem);
                        state.Progress = getDeserializeProgress(stream);
                    }
                }
                if (pack != null && pack.Count > 0)
                    packageList.Add(pack);
            }
            //return new Package[] { pack };
        }

        private void DeserializeNode(OperationState state, XmlReader reader, Stream stream, TreeWrapp<UnitNode> wrapNode, List<RevisionInfo> revisionList)
        {
            TreeWrapp<UnitNode> nodeWrapp;
            UnitNode node = null;
            String name = "noname";
            int type = (int)UnitTypeId.Unknown;

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    String attributeName = reader.Name.ToLower();
                    if (attributeName == nodeElementNameAttribute) name = reader.Value;
                    else if (attributeName == nodeElementTypeAttribute) try
                        {
                            type = (int)unitTypeEnumConverter.ConvertFromInvariantString(reader.Value);
                        }
                        catch (FormatException) { type = (int)UnitTypeId.Unknown; }
                } while (reader.MoveToNextAttribute());
            }
            node = service.NewInstance(state, type);
            node.Text = name;
            node.Typ = type;

            nodeWrapp = wrapNode.AddChild(node);
            reader.MoveToElement();
            if (!reader.IsEmptyElement)
            {
                DeserializeNodeChild(state, reader, stream, nodeWrapp, revisionList);
            }
        }

        private void DeserializeRevision(OperationState state, XmlReader reader, Stream stream, List<RevisionInfo> revisionList)
        {
            RevisionInfo revision = new RevisionInfo();

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    String attributeName = reader.Name.ToLower();

                    switch (attributeName)
                    {
                        case revisionElementIDAttribute:
                            revision.ID = int.Parse(reader.Value);
                            break;
                        case revisionElementTimeAttribute:
                            revision.Time = DateTime.Parse(reader.Value);
                            break;
                        case revisionElementBriefAttribute:
                            revision.Brief = reader.Value;
                            break;
                        case revisionElementCommentAttribute:
                            revision.Comment = reader.Value;
                            break;
                    }
                } while (reader.MoveToNextAttribute());
            }

            if (revision.ID > 0 && revision.Time > DateTime.MinValue)
                revisionList.Add(revision);
        }

        /// <summary>
        /// Десереализовать узлы
        /// </summary>
        /// <param name="reader">экземпляр класс XmlReader</param>
        /// <param name="rootList"></param>
        /// <param name="parentNode">узел, которому будут добавляться узлы из потока</param>
        /// <param name="stream">поток со структурой в формате xml</param>
        private void DeserializeNodeChild(OperationState state, XmlReader reader, Stream stream, TreeWrapp<UnitNode> wrapNode, List<RevisionInfo> revisionList)
        {
            while (true)
            {
                // переходим к началу элемента
                while (reader.Read() && reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement) ;

                // выход из цикла
                if (reader.NodeType == XmlNodeType.EndElement || reader.NodeType == XmlNodeType.None)
                    break;
                else if (reader.Name.Equals(nodeElement, StringComparison.InvariantCultureIgnoreCase))
                {
                    DeserializeNode(state, reader, stream, wrapNode, revisionList);
                }
                else if (reader.Name.Equals(propertyElement, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (wrapNode.Item != null)
                        DeserializeProperty(reader, wrapNode.Item, revisionList);
                }
                else if (reader.Name.Equals(parameterElement, StringComparison.InvariantCultureIgnoreCase))
                {
                    DeserializeParameter(reader, wrapNode);
                }
                state.Progress = getDeserializeProgress(stream);
            }
        }

        private void DeserializeProperty(XmlReader reader, UnitNode parentNode, List<RevisionInfo> revisionList)//, Dictionary<String, String> attributes)
        {
            String name = "name";

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    String attributeName = reader.Name.ToLower();
                    if (attributeName == propertyElementNameAttribute)
                    {
                        name = reader.Value;
                        break;
                    }
                } while (reader.MoveToNextAttribute());
            }
            IPropertyXML propertyXML = GetPropertyXML(name);

            propertyXML.Deserialize(reader, service, revisionList, parentNode, name/*, type*/);
        }

        private void DeserializeParameter(XmlReader reader, TreeWrapp<UnitNode> wrapNode)
        {
            String code = "";
            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    String attributeName = reader.Name.ToLower();
                    if (attributeName == "parameter_code") code = reader.Value;
                } while (reader.MoveToNextAttribute());
            }
            ParameterNode paramNode = new ParameterNode();
            paramNode.Code = code;
            ChildParamNode param = wrapNode.Item.AddChildParam(paramNode);

            if (paramNode is ParameterNode) param.SetParameter((ParameterNode)paramNode);
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                while (reader.Read() && reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement) ;
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("property", StringComparison.InvariantCultureIgnoreCase))
                {
                    DeserializeProperty(reader, param);
                }
            }
        }

        private void DeserializeProperty(XmlReader reader, ChildParamNode param)
        {
            String name = "name";

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    String attributeName = reader.Name.ToLower();
                    if (attributeName ==propertyElementNameAttribute) name = reader.Value;
                } while (reader.MoveToNextAttribute());
            }
            reader.MoveToElement();

            String value = reader.ReadElementContentAsString();
            param.Attributes[name] = value;
        } 
        #endregion
    }
}

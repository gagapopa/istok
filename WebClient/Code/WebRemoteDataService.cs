using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.IO;

using Drawing = System.Drawing;

using COTES.ISTOK.ASC;
using COTES.ISTOK;
using System.Drawing;

namespace WebClient
{
    public class WebRemoteDataService
    {
        private static object lock_icons = new object();

        private const string icon_extention = @".jpg";
        private readonly UnitTypeId[] filter = null;

        private IChannel connection_channel = null;
        private IGlobalQueryManager query_manager = null;
        private Guid user;

        private MnemoschemesManager mnemoschemes = new MnemoschemesManager();
        private MonitorTablesManager monitor_tables = new MonitorTablesManager();
        private static CacheObjects<int, UnitObjectDescriptor> object_deskriptor_cache =
            new CacheObjects<int, UnitObjectDescriptor>();
        private static FileResourceManager file_resources = new FileResourceManager();

        public WebRemoteDataService()
        {
            //пусть filter будет null, так он будет все типы пропускать
            //filter = new UnitTypeId[]
            //{
            //    UnitTypeId.Block,
            //    UnitTypeId.Boiler,
            //    UnitTypeId.Channel,
            //    UnitTypeId.ExcelReport,
            //    UnitTypeId.Folder,
            //    UnitTypeId.Graph,
            //    UnitTypeId.Histogram,
            //    UnitTypeId.ManualGate,
            //    UnitTypeId.ManualParameter,
            //    UnitTypeId.MonitorTable,
            //    UnitTypeId.NormFunc,
            //    UnitTypeId.Parameter,
            //    UnitTypeId.Report,
            //    UnitTypeId.Root,
            //    UnitTypeId.Schema,
            //    UnitTypeId.Station,
            //    UnitTypeId.TEP,
            //    UnitTypeId.TEPTemplate
            //};

            Connect();
        }

        public void UpdateConnectoinIfNeed()
        {
            if (query_manager == null)
                Connect();
        }

        private void Connect()
        {
            if (connection_channel == null)
            {
                BinaryClientFormatterSinkProvider client_provider = new BinaryClientFormatterSinkProvider();
                BinaryServerFormatterSinkProvider server_provider = new BinaryServerFormatterSinkProvider();
                server_provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

                IDictionary ht = new Hashtable(StringComparer.OrdinalIgnoreCase);
                ht.Add("name", string.Empty);
                ht.Add("port", 0);
                ht.Add("typeFilterLevel", System.Runtime.Serialization.Formatters.TypeFilterLevel.Full);
                //ht["machineName"] = BaseSettings.Instance["Settings/ClientInterface"].ToString();//OldClientSettings.Instance.ClientInterface;

                connection_channel = new TcpChannel(ht, client_provider, server_provider);
                ChannelServices.RegisterChannel(connection_channel, false);
            }

            bool result = false;
            WellKnownClientTypeEntry[] types = RemotingConfiguration.GetRegisteredWellKnownClientTypes();
            foreach (var typ in types)
            {
                result = typ.ObjectType.Equals(typeof(IGlobalQueryManager));
                if (result) break;
            }

            if (!result)
                RemotingConfiguration.RegisterWellKnownClientType(typeof(IGlobalQueryManager),
                                                                  Configuration.Get(Setting.GlobalServerUrl));

            query_manager =
                Activator.GetObject(typeof(IGlobalQueryManager),
                                    Configuration.Get(Setting.GlobalServerUrl)) as IGlobalQueryManager;
            if (query_manager == null) throw new Exception("Не удалось подключиться к серверу приложения.");
        }

        public bool LoginUser(string name, string password)
        {
            try
            {
                UpdateConnectoinIfNeed();

                user = query_manager.Connect(name, password);

                return !user.Equals(Guid.Empty);
            }
            catch
            {
                return false;
            }
        }

        public void LogoutUser()
        {
            try
            {
                var remove_resource = file_resources.ClearForUser(user);
                foreach (var it in remove_resource)
                {
                    try { File.Delete(HttpContext.Current.Server.MapPath(it.Link)); }
                    catch { }
                }

                foreach (var it in mnemoschemes)
                    try { query_manager.UnRegisterClient(user, it.UpdateTransactionID); }
                    catch { }

                foreach (var it in monitor_tables)
                    try { query_manager.UnRegisterClient(user, it.UpdateTransactionID); }
                    catch { }

                mnemoschemes.Clear();
                monitor_tables.Clear();

                query_manager.Disconnect(user);

                GC.Collect();
            }
            catch { }
        }

        public IEnumerable<UnitNode> GetRootStructureLayer()
        {
            const int root_id = -1;

            return GetStructureLayer(root_id);
        }

        public IEnumerable<UnitNode> GetStructureLayer(int id)
        {
            try
            {
                UpdateConnectoinIfNeed();

                ulong async_operation_id =
                    query_manager.BeginGetUnitNodes(user,
                                                    id,
                                                    filter);

                query_manager.WaitAsyncOperation(user, async_operation_id);

                var result = query_manager.GetOperationResult(user, async_operation_id);

                query_manager.EndAsyncOperation(user, async_operation_id);

                return result as IEnumerable<UnitNode>;
            }
            catch (UserNotConnectedException)
            {
                LogoutUser();
                return null;
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public void UpdateIconsForTreeView()
        {
            try
            {
                lock (lock_icons)
                {
                    UpdateConnectoinIfNeed();

                    var types = GetUnitTypes();

                    ClearOldIcons();

                    MemoryStream stream = null;
                    Drawing.Image im = null;
                    foreach (var it in types)
                    {
                        stream = new MemoryStream(it.Icon.Clone() as byte[]);
                        im = Drawing.Image.FromStream(stream);
                        im.Save(HttpContext.Current.Server.MapPath(Configuration.Get(Setting.TempIconFolderUrl)) +
                                    it.Idnum.ToString() +
                                        icon_extention);
                    }
                }
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        private void ClearOldIcons()
        {
            DirectoryInfo load_directory =
                new DirectoryInfo(HttpContext.Current.Server.MapPath(Configuration.Get(Setting.TempIconFolderUrl)));

            var old_images = load_directory.GetFiles();
            foreach (var it in old_images)
                it.Delete();
        }

        public string GetTypeIconUrl(UnitTypeId type)
        {
            try
            {
                lock (lock_icons)
                {
                    return Configuration.Get(Setting.TempIconFolderUrl) +
                           ((int)type).ToString() +
                           icon_extention;
                }
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public bool ServerIsLoad()
        {
            try
            {
                UpdateConnectoinIfNeed();

                const double loaded = 100.0;
                return query_manager.GetLoadProgress(user) == loaded;
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public double GetServerLoadProgress()
        {
            try
            {
                UpdateConnectoinIfNeed();

                return query_manager.GetLoadProgress(user);
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public string GetServerLoadStatusString()
        {
            try
            {
                UpdateConnectoinIfNeed();

                return query_manager.GetLoadStatusString(user);
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public string GetTypeProcessUrl(UnitTypeId node_type,
                                        int id)
        {
            try
            {
                const string process_page_config_prefix = @"ProcessPage";

                string page = "";

                switch (node_type)
                {
                    case UnitTypeId.Histogram:
                    case UnitTypeId.Graph:
                    case UnitTypeId.MonitorTable:
                    case UnitTypeId.Schema:
                    case UnitTypeId.NormFunc:
                    //case UnitTypeId.Boiler:
                    case UnitTypeId.ExcelReport:
                    case UnitTypeId.Report:
                    case UnitTypeId.TEPTemplate:
                    case UnitTypeId.ManualGate:
                        page = node_type.ToString();
                        break;
                    default:
                        break;
                }

                ParametersPageManager builder = new ParametersPageManager();

                builder.Add(Configuration.Get(Setting.IdObjectMarker), id.ToString());

                return WebConfigurationManager.AppSettings[process_page_config_prefix +
                                                           page] +
                       builder.ToString();
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public UnitNode GetUnitNode(int id)
        {
            try
            {
                return query_manager.GetUnitNode(user, id);
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public UnitNode[] GetUnitNodes(int parent, UnitTypeId[] filterTypes)
        {
            object result;

            try
            {
                List<UnitNode> lst = new List<UnitNode>();

                UpdateConnectoinIfNeed();
                var async_operation = query_manager.BeginGetUnitNodes(user, parent, filterTypes);
                query_manager.WaitAsyncOperation(user, async_operation);

                while ((result = query_manager.GetOperationResult(user, async_operation)) != null)
                    if (result is UnitNode[]) lst.AddRange((UnitNode[])result);

                query_manager.EndAsyncOperation(user, async_operation);
                return lst.ToArray();
                
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public IEnumerable<ParamValueItemWithID> GetValues(int[] parametersID,
                                                       DateTime beginTime,
                                                       DateTime endTime,
                                                       Interval interval,
                                                       CalcAggregation aggregation,
                                                       bool useBlock)
        {
            List<ParamValueItemWithID> lst = new List<ParamValueItemWithID>();
            object result;

            try
            {
                UpdateConnectoinIfNeed();
                var async_operation = query_manager.BeginGetValues(user,
                                                                   parametersID,
                                                                   beginTime,
                                                                   endTime,
                                                                   interval,
                                                                   aggregation,
                                                                   useBlock);

                query_manager.WaitAsyncOperation(user, async_operation);
                
                while ((result = query_manager.GetOperationResult(user, async_operation)) != null)
                    if (result is Package) lst.AddRange(((Package)result).Values.ConvertAll(i => new ParamValueItemWithID(i, ((Package)result).Id)));
                
                query_manager.EndAsyncOperation(user, async_operation);
                return lst.ToArray();
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public IEnumerable<ParamValueItem> GetValues(int paranetrs_id,
                                                           DateTime from,
                                                           DateTime to,
                                                           bool useBlock)
        {
            List<ParamValueItem> lst = new List<ParamValueItem>();
            object result;

            try
            {
                UpdateConnectoinIfNeed();

                var async_operation = query_manager.BeginGetValues(user,
                                                                   paranetrs_id,
                                                                   from,
                                                                   to,
                                                                   useBlock);

                query_manager.WaitAsyncOperation(user, async_operation);

                while ((result = query_manager.GetOperationResult(user, async_operation)) != null)
                    if (result is Package) lst.AddRange(((Package)result).Values);

                query_manager.EndAsyncOperation(user, async_operation);
                return lst.ToArray();
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public IEnumerable<ParamValueItem> GetValues(int id_parametr,
                                                       DateTime from_date,
                                                       DateTime to_date,
                                                       Interval interval,
                                                       CalcAggregation agregation,
                                                       bool useBlock)
        {
            List<ParamValueItem> lst = new List<ParamValueItem>();
            object result;

            try
            {
                UpdateConnectoinIfNeed();

                var async_operation = query_manager.BeginGetValues(user,
                                                                   id_parametr,
                                                                   from_date,
                                                                   to_date,
                                                                   interval,
                                                                   agregation,
                                                                   useBlock);

                query_manager.WaitAsyncOperation(user, async_operation);

                while ((result = query_manager.GetOperationResult(user, async_operation)) != null)
                    if (result is Package) lst.AddRange(((Package)result).Values);

                query_manager.EndAsyncOperation(user, async_operation);
                return lst.ToArray();
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public MnemoschemesDescriptor GetMnemoschemeDescriptor(int id)
        {
            try
            {
                UpdateConnectoinIfNeed();

                if (mnemoschemes.Exist(id))
                    return mnemoschemes[id];

                SchemaNode node = query_manager.GetUnitNode(user, id) as SchemaNode;

                Size size;
                Guid content;

                content = CreateMnemoschemeImage(out size,
                                                 node.ImageBuffer,
                                                 node.Idnum);

                MnemoschemesDescriptor result =
                    new MnemoschemesDescriptor(node.Idnum,
                                               node.Text,
                                               query_manager.RegisterClient(user,
                                                                            ToParamReciveItem(node.Parameters)),
                                               content,
                                               size);

                MnemoschemeParameterDescriptor param = null;
                foreach (SchemaParamNode item in node.Parameters)
                {
                    ParameterNode par = GetUnitNode(item.ParameterId) as ParameterNode;
                    if (par != null)
                    {
                        double interval = (int)GetParameterInterval(par).ToDouble();
                        item.Attributes[CommonData.IntervalProperty] = interval.ToString();
                    }

                    param = new MnemoschemeParameterDescriptor(node, item);
                    //new MnemoschemeParameterDescriptor(item.Idnum,
                    //                                   item.ParameterId,
                    //                                   item.Text,
                    //                                   item.MinAlertValue,
                    //                                   item.MaxAlertValue,
                    //                                   item.MinWarningValue,
                    //                                   item.MaxWarningValue,
                    //                                   new Rectangle(item.Left,
                    //                                                 item.Top,
                    //                                                 item.Width,
                    //                                                 item.Height));
                    result.AddParameter(param);
                }

                mnemoschemes.Add(result);

                return result;
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public Guid AddFileResource(string filename)
        {
            Guid guid = Guid.NewGuid();

            lock (file_resources)
            {
                file_resources.AddNewResource(user, new FileResource(guid)
                {
                    Link = filename,
                    IsLoad = false
                });                
            }

            return guid;
        }

        private ParamValueItemWithID[] ToParamReciveItem(ChildParamNode[] parametrs)
        {
            ParamValueItemWithID[] result = new ParamValueItemWithID[parametrs.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = new ParamValueItemWithID()
                    {
                         ParameterID = parametrs[i].ParameterId,
                    };
            return result;
        }

        private Guid CreateMnemoschemeImage(out Size size,
                                            byte[] content,
                                            int id)
        {
            const string format = @"{0}.png";

            Guid result = FileResource.GetGuid(id);

            lock (file_resources)
            {
                if (!file_resources.ExistResource(result) && content != null)
                {
                    string img_name = "";
                    using (MemoryStream stream = new MemoryStream(content))
                    {
                        Bitmap img = new Bitmap(stream);
                        size = img.Size;
                        img_name = Configuration.Get(Setting.TempImageFolderUrl) +
                                   String.Format(format, id);
                        img.Save(HttpContext.Current.Server.MapPath(img_name));
                    }

                    file_resources.AddNewResource(user, new FileResource(result)
                                                   {
                                                       Link = img_name,
                                                       IsLoad = true
                                                   });
                }
                else
                    size = new Size();

                return result;
            }
        }

        public ParamValueItemWithID[] GetUpdatedValues(int transaction_id)
        {
            try
            {
                UpdateConnectoinIfNeed();

                return query_manager.GetValuesFromBank(user,
                                                       transaction_id);
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public UnitObjectDescriptor GetObjectDescriptor(int id)
        {
            try
            {
                UpdateConnectoinIfNeed();

                if (object_deskriptor_cache.ContainsKey(id))
                    return object_deskriptor_cache[id];

                UnitObjectDescriptor result =
                    new UnitObjectDescriptor(GetUnitNode(id));//,
                                             //new WebClientServiceContainer(this));

                object_deskriptor_cache.Add(id, result);

                return result;
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public MonitorTableDescriptor GetMonitorTableDescriptor(int id)
        {
            try
            {
                UpdateConnectoinIfNeed();

                if (monitor_tables.Exist(id))
                    return monitor_tables[id];

                MonitorTableNode node = GetUnitNode(id) as MonitorTableNode;

                MonitorTableDescriptor result =
                    new MonitorTableDescriptor(node.Idnum,
                                               node.Text,
                                               query_manager.RegisterClient(user,
                                                                            ToParamReciveItem(node.Parameters)));


                IDictionary<int, ChildParamNode> parametrs =
                    node.Parameters.ToDictionary(x => x.ParameterId);

                CellDescriptor temp = null;
                if (node.Table != null)
                {
                    for (int i = 0; i < node.Table.Columns.Count; i++)
                        for (int j = 0; j < node.Table.Rows.Count; j++)
                        {
                            if (node.Table.Rows[j][i] is byte[])
                                temp = new CellDescriptor(node.Table.Rows[j][i] as byte[]);
                            else
                                temp = new CellDescriptor(node.Table.Rows[j][i].ToString(),
                                                          j,
                                                          i);

                            if (temp.IsParameterCell &&
                                parametrs.ContainsKey(temp.ParameterID))
                            {
                                result.AddParameter(
                                       new MonitorTableParameterDescriptor(parametrs[temp.ParameterID].Idnum,
                                                                           parametrs[temp.ParameterID].ParameterId,
                                                                           parametrs[temp.ParameterID].Text,
                                                                           temp)
                                                                                                                    );
                            }

                            if (temp.IsLink)
                                lock (file_resources)
                                {
                                    Guid content_id = FileResource.GetGuid(node.Idnum,
                                                                           temp.Column,
                                                                           temp.Row);

                                    if (!file_resources.ExistResource(content_id))
                                        file_resources.AddNewResource(user, new FileResource(content_id)
                                                                       {
                                                                           Link = temp.Link,
                                                                           IsLoad = false
                                                                       });

                                    temp.Content = content_id;
                                }

                            result.AddCell(temp);
                        }
                }

                monitor_tables.Add(result);

                return result;
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public FileResource GetResource(Guid guid)
        {
            try
            {
                lock (file_resources)
                {
                    FileResource file = file_resources[guid];
                    if (!file.IsLoad)
                    {
                        file.Link = LoadRemoteFile(file.Link);
                        file.IsLoad = true;
                    }

                    return file_resources[guid];
                }
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        private string LoadRemoteFile(string link)
        {
            try
            {
                string file_name = Path.GetFileName(link);
                string load_path =
                    Configuration.Get(Setting.TempFileFolderUrl) +
                    file_name;

                File.Copy(link, 
                          HttpContext.Current.Server.MapPath(load_path), 
                          true);

                return load_path;
            }
            catch
            {
                return String.Empty;
            }
        }

        #region ServiceContainerInterface

        public string[] GetBlockUIDs()
        {
            try
            {
                UpdateConnectoinIfNeed();

                return query_manager.GetBlockUIDs(user);
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public UTypeNode[] GetUnitTypes()
        {
            try
            {
                ulong async_operation = query_manager.BeginGetUnitTypes(user);

                query_manager.WaitAsyncOperation(user, async_operation);

                List<UTypeNode> types = new List<UTypeNode>();

                object temp = null;
                while ((temp = query_manager.GetOperationResult(user,
                                                                async_operation)) != null &&
                       temp is UTypeNode)
                {
                    types.Add(temp as UTypeNode);
                }

                query_manager.EndAsyncOperation(user, async_operation);

                return types.ToArray();
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public UserNode GetUser()
        {
            try
            {
                UpdateConnectoinIfNeed();

                return query_manager.GetUser(user);
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public GroupNode[] GetGroupNodes()
        {
            try
            {
                UpdateConnectoinIfNeed();

                ulong async_operation = query_manager.BeginGetGroupNodes(user);

                query_manager.WaitAsyncOperation(user, async_operation);

                object result = query_manager.GetOperationResult(user, async_operation);

                query_manager.EndAsyncOperation(user, async_operation);

                return result as GroupNode[];
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public string GetUserLogin(int owner_id)
        {
            try
            {
                UpdateConnectoinIfNeed();

                return query_manager.GetUserLogin(user, owner_id);
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public string[] GetModuleLibNames(int blockId)
        {
            try
            {
                UpdateConnectoinIfNeed();

                return query_manager.GetModuleLibNames(user,
                                                       blockId);
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public ChannelProperty[] GetModuleLibraryProperties(int blockId, 
                                                            string libName)
        {
            try
            {
                UpdateConnectoinIfNeed();

                return query_manager.GetModuleLibraryProperties(user,
                                                                blockId,
                                                                libName);
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public Schedule[] GetParmetrUnloadSchedules()
        {
            try
            {
                UpdateConnectoinIfNeed();

                ulong async_operation = query_manager.BeginGetParamsSchedules(user);

                query_manager.WaitAsyncOperation(user, async_operation);

                object result = query_manager.GetOperationResult(user, async_operation);

                query_manager.EndAsyncOperation(user, async_operation);

                return result as Schedule[];
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public Schedule GetParametrUnloadSchedule(int id)
        {
            try
            {
                UpdateConnectoinIfNeed();

                ulong async_operation = query_manager.BeginGetParamsSchedule(user, id);

                query_manager.WaitAsyncOperation(user, async_operation);

                object result = query_manager.GetOperationResult(user, async_operation);

                query_manager.EndAsyncOperation(user, async_operation);

                return result as Schedule;
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public byte[] GetFilledExcelReport(int id, DateTime dateFrom, DateTime dateTo)
        {
            //return query_manager.GetFilledExcelReport(user, id, dateFrom, dateTo);
            return null;
        }

        public byte[] GenerateReport(ReportNode report, DateTime dateFrom, DateTime dateTo, bool saveInSystem)
        {
            //return query_manager.GenerateReport(user, report, dateFrom, dateTo, saveInSystem);
            return null;
        }

        #endregion


        public void DeleteUnregisterResource()
        {
            try
            {
                lock (file_resources)
                {
                    List<string> files = new List<string>();

                    files.AddRange(
                        Directory.GetFiles(
                            HttpContext.Current.Server.MapPath(
                                Configuration.Get(Setting.TempImageFolderUrl))));

                    files.AddRange(
                        Directory.GetFiles(
                            HttpContext.Current.Server.MapPath(
                                Configuration.Get(Setting.TempFileFolderUrl))));

                    string temp = "";
                    foreach (var it in file_resources)
                        if (it.IsLoad)
                        {
                            temp = HttpContext.Current.Server.MapPath(it.Link);
                            if (files.Contains(temp))
                                files.Remove(temp);
                        }
                    

                    foreach (var it in files)
                        try { File.Delete(it); }
                        catch { }
                }
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public void ClearTempZedGraphImages()
        {
            try
            {
                string path = 
                    HttpContext.Current.Server.MapPath(Configuration.Get(Setting.TempGraphicFolder));
                
                foreach (var it in Directory.GetFiles(path))
                    try { File.Delete(it); }
                    catch { }
            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        /// <summary>
        /// Получить интервал параметра
        /// </summary>
        /// <remarks>
        /// Интервал параметра берется и первого родительского элемента в структуре, обладающего интервалом.
        /// Это Ручной ввод, Расчет и оптимизация
        /// </remarks>
        /// <param name="Parameter">Параметр</param>
        /// <returns>Полученный интервал</returns>
        public DateTime GetParameterStartTime(ParameterNode Parameter)
        {
            UnitNode unitNode = Parameter;
            ParameterGateNode gateNode;

            while (unitNode != null)
            {
                if ((gateNode = unitNode as ParameterGateNode) != null)
                    return gateNode.StartTime;

                unitNode = GetUnitNode(unitNode.ParentId);
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// Получить интервал параметра
        /// </summary>
        /// <remarks>
        /// Интервал параметра берется и первого родительского элемента в структуре, обладающего интервалом.
        /// Это Ручной ввод, Расчет и оптимизация
        /// </remarks>
        /// <param name="Parameter">Параметр</param>
        /// <returns>Полученный интервал</returns>
        public Interval GetParameterInterval(ParameterNode Parameter)
        {
            UnitNode unitNode = Parameter;
            ParameterGateNode gateNode;

            while (unitNode != null)
            {
                if ((gateNode = unitNode as ParameterGateNode) != null)
                    return gateNode.Interval;

                unitNode = GetUnitNode(unitNode.ParentId);
            }
            return Interval.Zero;
        }

        public UnitNode[] GetAllUnitNodes(UnitNode unitNode, UnitTypeId[] filterTypes, int minLevel, int maxLevel)
        {
            object result;

            try
            {
                List<UnitNode> lst = new List<UnitNode>();

                UpdateConnectoinIfNeed();
                var async_operation = query_manager.BeginGetAllUnitNodes(user, unitNode == null ? 0 : unitNode.Idnum,
                                                         filterTypes,
                                                         minLevel,
                                                         maxLevel);
                query_manager.WaitAsyncOperation(user, async_operation);

                while ((result = query_manager.GetOperationResult(user, async_operation)) != null)
                    if (result is UnitNode[]) lst.AddRange((UnitNode[])result);

                query_manager.EndAsyncOperation(user, async_operation);
                return lst.ToArray();

            }
            catch (Exception exp)
            {
                throw new ServerException(exp);
            }
        }

        public byte[] GenerateReport(ReportNode reportNode, bool saveInSystem, ReportParameter[] reportParameters)
        {
            var ret = RemoteCallSync<byte>(() => query_manager.BeginGenerateReport(user, reportNode.Idnum, saveInSystem, reportParameters));
            if (ret != null)
                return ret.ToArray();
            return null;
        }

        private IEnumerable<T> RemoteCallSync<T>(Func<ulong> call)
        {
            //try
            //{
            ulong operationID = call();
            Object obj;
            List<T> valList = new List<T>();

            query_manager.WaitAsyncOperation(user, operationID);

            while ((obj = query_manager.GetOperationResult(user, operationID)) != null)
            {
                if (obj is T)
                    valList.Add((T)obj);
                else if (obj is IEnumerable<T>)
                    valList.AddRange(obj as IEnumerable<T>);
                //else break;
            }
            query_manager.EndAsyncOperation(user, operationID);

            return valList;
            //}
            //catch (System.Net.Sockets.SocketException) { OnServerNotAccessible(); }
            //catch (RemotingException) { OnServerNotAccessible(); }
            //catch (UserNotConnectedException) { OnUserDisconnected(); }
            //return default(IEnumerable<T>);
        }

        IValuesRequest valueRequest;
        public IValuesRequest GetValueRequest()
        {
            if (valueRequest == null)
                valueRequest = new MyValuesRequest(this);
            return valueRequest;
        }
    }

    class MyValuesRequest:IValuesRequest
    {

        WebRemoteDataService remoteService;

        public MyValuesRequest(WebRemoteDataService remoteService)
        {
            this.remoteService = remoteService;
        }

        #region IValuesRequest Members

        public IEnumerable<ParamValueItem> GetParameterValues(int parameterID, bool useBlock, DateTime from_date, DateTime to_date, CalcAggregation agregation)
        {


            //WebRemoteDataService data_service = DataService;

            Interval interval = Interval.Zero;

            UnitNode parametr_node = remoteService.GetUnitNode(parameterID);
            remoteService.GetParameterInterval(parametr_node as ParameterNode);
            //if (parametr_node != null &&
            //    parametr_node is ParameterNode)
            //    interval = (parametr_node as ParameterNode).Interval;

            return remoteService.GetValues(parameterID,
                                          from_date,
                                          to_date,
                                          interval,
                                          agregation,
                                          useBlock);
        }

        public IEnumerable<ParamValueItem> GetParameterValues(int parameterID, bool useBlock, DateTime from_date, DateTime to_date)
        {


            return remoteService.GetValues(parameterID,
                                         from_date,
                                         to_date,
                                         useBlock);

        }

        #endregion
    }
}

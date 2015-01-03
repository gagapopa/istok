using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Assignment.Extension;
using COTES.ISTOK.Extension;
using NLog;

namespace COTES.ISTOK.Assignment
{
    class UnitManager : IUnitManager
    {
        Logger log = LogManager.GetCurrentClassLogger();

        private object sync_obj = new object();
        protected const Char CryptoSignChar = '\x02';
        private MyDBdata dbwork;

        public RevisionManager RevisionManager { get; set; }
        public ISecurityManager SecurityManager { get; set; }
        public ILockManager LockManager { get; set; }
        public BlockProxy BlockProxy { get; set; }
        public IUnitTypeManager UnitTypeManager { get; set; }

        public ExtensionManager ExtensionManager { get; set; }

        /// <summary>
        /// Для шифровки/расшифровки формул
        /// </summary>
        private Cryptor cryptor = new Cryptor();


        /// <summary>
        /// Синхронизация загрузки/выдачи юнитов
        /// </summary>
        private Dictionary<int, object> dicUnitSync = new Dictionary<int, object>();

        public UnitManager(MyDBdata dbwork)//,
            //RevisionManager revisionManager,
            //BlockProxy blockProxy,
            //IUnitTypeManager unitTypeManager)
        {
            this.dbwork = dbwork.Clone();
            //this.revisionManager = revisionManager;
            //this.blockProxy = blockProxy;
            //this.unitTypeManager = unitTypeManager;
        }

        //public void SetSecurity(ISecurityManager manager, ILockManager lockManager)
        //{
        //    this.lockManager = lockManager;
        //    securityManager = manager;
        //}

        Dictionary<int, UnitNode> dicUnits = new Dictionary<int, UnitNode>();
        Dictionary<String, ParameterNode> dicParameters = new Dictionary<string, ParameterNode>();

        /// <summary>
        /// Список всех параметров, даже без кода
        /// </summary>
        List<ParameterNode> parametersList = new List<ParameterNode>();

        List<UnitNode> units;
        List<NormFuncNode> normFuncNodes = new List<NormFuncNode>();

        #region Загрузка структуры
        private Dictionary<string, string> LoadChildAttributes(int idnum)
        {
            Dictionary<String, String> res =
                new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
            try
            {
                DataTable dt = null;

                if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

                DB_Parameters Param = new DB_Parameters();
                Param.Add("parentid", DbType.Int32, idnum, ParameterDirection.Input);
                string query = "select idnum,name,value from child_props where parentid=@parentid";
                dt = dbwork.ExecSQL_toTable(query, Param);

                if (dt != null)
                {
                    String name;
                    String value;

                    foreach (DataRow row in dt.Rows)
                    {
                        name = row["name"].ToString();
                        value = row["value"].ToString();

                        res[name] = value;
                    }
                    dt.Dispose();
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(String.Format("ID: {0}", idnum), ex);
                throw;
            }
            return res;
        }

        private Dictionary<String, RevisedStorage<T>> LoadAttributes<T>(DataTable dt)
        {
            Dictionary<String, RevisedStorage<T>> res =
                new Dictionary<String, RevisedStorage<T>>(StringComparer.OrdinalIgnoreCase);

            if (dt != null)
            {
                String name;
                T value;
                RevisionInfo revision;
                RevisedStorage<T> versionValue;

                foreach (DataRow row in dt.Rows)
                {
                    name = row["name"].ToString();
                    value = (T)Convert.ChangeType(row["value"], typeof(T));
                    if (DBNull.Value.Equals(row["revision"]))
                        revision = RevisionInfo.Default;
                    else
                        revision = RevisionManager.GetRevision(Convert.ToInt32(row["revision"]));

                    if (!res.TryGetValue(name, out versionValue))
                        res[name] = versionValue = new RevisedStorage<T>();

                    versionValue.Set(revision, value);
                }
            }

            return res;
        }

        private Dictionary<string, RevisedStorage<string>> LoadUnitAttributes(UnitNode node)
        {
            Dictionary<String, RevisedStorage<String>> res =
                new Dictionary<String, RevisedStorage<String>>(StringComparer.OrdinalIgnoreCase);
            try
            {
                DataTable dt = null;

                if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

                DB_Parameters Param = new DB_Parameters();
                Param.Add("parentid", DbType.Int32, node.Idnum, ParameterDirection.Input);
                string query = "select idnum,name,revision,value from props where parentid=@parentid";

                using (dt = dbwork.ExecSQL_toTable(query, Param))
                {
                    res = LoadAttributes<String>(dt);
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(String.Format("ID: {0}", node.Idnum), ex);
                throw;
            }
            String formula_field = "formula_text";
            if (res.ContainsKey(formula_field))
            {
                DecryptFormula(node, res[formula_field]);
            }

            return res;
        }

        private void DecryptFormula(UnitNode node, RevisedStorage<String> versionValue)
        {
            foreach (var time in versionValue)
            {
                String formula = versionValue.Get(time);//.ToString();
                if (!String.IsNullOrEmpty(formula) && formula[0] == CryptoSignChar)
                {
                    node.IsCrypted = true;
                    formula = formula.Substring(1);
                    formula = cryptor.DecryptingString(formula);
                    versionValue.Set(time, formula);// = formula;
                }
            }
        }

        private Dictionary<string, RevisedStorage<byte[]>> LoadUnitBinaries(UnitNode node)
        {
            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            try
            {
                DB_Parameters Param = new DB_Parameters();
                Param.Add("parentid", DbType.Int32, node.Idnum, ParameterDirection.Input);

                string query = "select name, revision, value from lobs where parentid=@parentid";
                using (DataTable dt = dbwork.ExecSQL_toTable(query, Param))
                {
                    return LoadAttributes<byte[]>(dt);
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(String.Format("ID {0}", node.Idnum), ex);
                throw;
            }
        }

        /************************************************************************************************************************/
        //Оптимизация загрузки бд.
        /************************************************************************************************************************/

        /// <summary>
        /// Загружает юниты.
        /// </summary>
        public void LoadUnits(OperationState state)
        {
            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            const string query_units = "select u.idnum,u.name,u.type," +
                " u.parent as parent from unit u;";

            const string query_properties = "select parentid, name, revision, value from props";

            const string query_binaries = "select parentid, name, revision, value from lobs";

            DataTable table_units;

            state.StateString = "Загрузка элементов";

            table_units = dbwork.ExecSQL_toTable(query_units, null);

            double percent_on_node = (100.0 - state.Progress) / (table_units.Rows.Count * 4);

            units = new List<UnitNode>(table_units.Rows.Count);

            for (int i = table_units.Rows.Count - 1; i >= 0; i--)
            {
                var inst = NewInstance(state, table_units.Rows[i]);
                table_units.Rows.RemoveAt(i);

                units.Add(inst);
                lock (dicUnits) dicUnits[inst.Idnum] = inst;

                if (inst is ParameterNode)
                {
                    parametersList.RemoveAll(x => x.Idnum == inst.Idnum);
                    parametersList.Add(inst as ParameterNode);
                }

                if (inst.Typ == (int)UnitTypeId.Block)
                    BlockProxy.AddBlock(inst as BlockNode);

                if (inst.Typ == (int)UnitTypeId.NormFunc)
                    normFuncNodes.Add(inst as NormFuncNode);

                state.Progress += percent_on_node;
            }

            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            DataTable table_properties;
            DataTable table_binaries;

            state.StateString = "Загрузка свойсвт элементов";

            table_properties = dbwork.ExecSQL_toTable(query_properties, null);
            table_binaries = dbwork.ExecSQL_toTable(query_binaries, null);

            percent_on_node = (AsyncOperation.MaxProgressValue - state.Progress - 1.0) / units.Count;

            var temp_units = new List<UnitNode>(units);

            UnitNode unitNode;
            String propertyName, propertyValue;
            RevisionInfo revision;
            byte[] propertyBody;
            const string formula_field = "formula_text";
            foreach (DataRow propertyRow in table_properties.Rows)
            {
                int nodeId = Convert.ToInt32(propertyRow["parentid"]);
                if (dicUnits.TryGetValue(nodeId, out unitNode))
                {
                    propertyName = propertyRow["name"].ToString();
                    propertyValue = propertyRow["value"].ToString();

                    if (DBNull.Value.Equals(propertyRow["revision"]))
                        revision = RevisionInfo.Default;
                    else
                        revision = RevisionManager.GetRevision(Convert.ToInt32(propertyRow["revision"]));

                    if (formula_field.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)
                        && !String.IsNullOrEmpty(propertyValue)
                        && propertyValue[0] == CryptoSignChar)
                    {
                        unitNode.IsCrypted = true;
                        propertyValue = cryptor.DecryptingString(propertyValue.Substring(1));
                    }
                    unitNode.SetAttribute(propertyName, revision, propertyValue);
                }
            }
            foreach (DataRow propertyRow in table_binaries.Rows)
            {
                int nodeId = Convert.ToInt32(propertyRow["parentid"]);
                if (dicUnits.TryGetValue(nodeId, out unitNode))
                {
                    propertyName = propertyRow["name"].ToString();
                    propertyBody = (byte[])propertyRow["value"];

                    if (DBNull.Value.Equals(propertyRow["revision"]))
                        revision = RevisionInfo.Default;
                    else
                        revision = RevisionManager.GetRevision(Convert.ToInt32(propertyRow["revision"]));

                    unitNode.SetBinaries(propertyName, revision, propertyBody);
                }
            }

            state.StateString = "Формирование структуры";
            for (int i = 0; i < temp_units.Count; i++)
            {
                LoadChildParams(temp_units[i]);

                if (temp_units[i] is ParameterNode)
                {
                    if (!String.IsNullOrEmpty(temp_units[i].Code))
                        dicParameters[temp_units[i].Code] = temp_units[i] as ParameterNode;
                }

                if (dicUnits.TryGetValue(temp_units[i].ParentId, out unitNode))
                {
                    unitNode.AddNode(temp_units[i]);
                    units.Remove(temp_units[i]);
                }

                state.Progress += percent_on_node;
            }

            // уведомить о загрузке каждого узла в структуре
            new List<UnitNode>(dicUnits.Values).ForEach(OnUnitNodeLoaded);

            //return true;
        }

        /************************************************************************************************************************/

        private UnitNode LoadUnit(OperationState state, int idnum)
        {
            DataTable dt = null;
            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            try
            {
                object sync = new object();
                lock (sync)
                {
                    lock (dicUnitSync)
                    {
                        dicUnitSync[idnum] = sync;
                    }

                    DB_Parameters Param = new DB_Parameters();
                    StringBuilder res = new StringBuilder();
                    res.Append("select u.idnum,u.name,u.type,t.fkey");
                    res.Append(",(select count(*) from unit u2 where u2.parent=u.idnum) as count_child, u.parent as parent");
                    res.Append(" from unit u, unit_type t");
                    res.Append(" where u.type=t.idnum");
                    res.Append(" and u.idnum=@p_idnum");

                    Param.Add("p_idnum", DbType.Int32, idnum, ParameterDirection.Input);
                    dt = dbwork.ExecSQL_toTable(res.ToString(), Param);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        UnitNode childNode, newnode = LoadUnit(state, dt.Rows[0]);
                        dt.Dispose();
                        int transaction = 0;
                        try
                        {
                            transaction = dbwork.StartTransaction();
                            Param = new DB_Parameters();
                            Param.Add("p_idnum", DbType.Int32, newnode.Idnum, ParameterDirection.Input);
                            using (DbDataReader reader = dbwork.ExecSQL_toReader(transaction, "select idnum from unit where parent=@p_idnum", Param))
                            {
                                List<int> lstIds = new List<int>();
                                while (reader != null && reader.Read())
                                    if (!reader.IsDBNull(0)) lstIds.Add(Convert.ToInt32(reader[0]));
                                newnode.AddNodes(lstIds.ToArray());
                            }
                        }
                        finally
                        {
                            dbwork.CloseTransaction(transaction);
                        }

                        foreach (int childID in newnode.NodesIds)
                            if (dicUnits.TryGetValue(childID, out childNode))
                                newnode.AddNode(childNode);

                        OnUnitNodeLoaded(newnode);
                        return newnode;
                    }
                }
            }
            finally
            {
                lock (dicUnitSync)
                {
                    dicUnitSync.Remove(idnum);
                }
            }
            return null;
        }

        /// <summary>
        /// Событие уведомляющие, что узел загрузился
        /// </summary>
        public event EventHandler<UnitNodeEventArgs> UnitNodeLoaded;

        public event EventHandler<UnitNodeEventArgs> UnitNodeChanged;

        /// <summary>
        /// Вызывает событие UnitNodeLoaded для указанного узла
        /// </summary>
        /// <param name="unitNode"></param>
        protected void OnUnitNodeLoaded(UnitNode unitNode)
        {
            if (UnitNodeLoaded != null)
                UnitNodeLoaded(this, new UnitNodeEventArgs(unitNode));
        }

        protected void OnUnitNodeChanged(UnitNode unitNode)
        {
            if (UnitNodeChanged != null)
                UnitNodeChanged(this, new UnitNodeEventArgs(unitNode));
        }

        /// <summary>
        /// Создается нод и заносится в списки нодов.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private UnitNode LoadUnit(OperationState state, DataRow row)
        {
            UnitNode newnode = NewInstance(state, row);

            foreach (var item in units)
            {
                if (item.Idnum == newnode.Idnum)
                {
                    units.Remove(item);
                    break;
                }
            }
            lock (dicUnits) dicUnits[newnode.Idnum] = newnode;
            UnitNode parent;
            if (newnode.ParentId == 0) units.Add(newnode);
            else if (dicUnits.TryGetValue(newnode.ParentId, out parent))
            {
                parent.AddNode(newnode);
            }

            LoadChildParams(newnode);
            newnode.Attributes = LoadUnitAttributes(newnode);
            newnode.Binaries = LoadUnitBinaries(newnode);
            if (newnode is ParameterNode)
            {
                parametersList.RemoveAll(x => x.Idnum == newnode.Idnum);
                parametersList.Add(newnode as ParameterNode);
                if (!String.IsNullOrEmpty(newnode.Code))
                    dicParameters[newnode.Code] = newnode as ParameterNode;
            }

            if (newnode.Typ == (int)UnitTypeId.Block)
            {
                BlockProxy.AddBlock(newnode as BlockNode);
            }

            if (newnode.Typ == (int)UnitTypeId.NormFunc)
                normFuncNodes.Add(newnode as NormFuncNode);

            return newnode;
        }

        private void LoadChildParams(UnitNode node)
        {
            try
            {
                List<string> lstUsers = new List<string>();

                if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

                int transaction = 0;
                try
                {
                    transaction = dbwork.StartTransaction();
                    string query = "select distinct username from child_params";
                    using (DbDataReader reader = dbwork.ExecSQL_toReader(transaction, query, null))
                    {
                        if (reader != null)
                            while (reader.Read()) lstUsers.Add(reader.GetString(0));
                    }

                    foreach (var item in lstUsers)
                    {
                        LoadChildParams(transaction, node, item);
                    }
                }
                finally
                {
                    dbwork.CloseTransaction(transaction);
                }
            }
            catch (Exception ex)
            {
                log.ErrorException(String.Format("Text: {0}", node.Text), ex);
            }
        }

        private bool LoadChildParams(int transaction, UnitNode parent, string user)
        {
            try
            {
                // Загрузка пользовательских параметров шаблона
                if (parent == null) throw new Exception("Не задан шаблон");

                DataTable dt = null;
                if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

                DB_Parameters dbp = new DB_Parameters();
                dbp.Add("username", DbType.String, user);
                dbp.Add("parentId", DbType.Int32, parent.Idnum);
                // Сначала проверим какие-нибудь сохраненные настройки по этому пользователю
                // если нет никаких, то берем общие настройки шаблона (user="")
                string query = "select count(*) from child_params where username=@username and parentId=@parentId";
                using (DbDataReader reader = dbwork.ExecSQL_toReader(transaction, query, dbp))
                {
                    if (reader == null) dbp[0].ParamValue = "";
                    else
                    {
                        if (!reader.Read()) dbp[0].ParamValue = "";
                        else
                        {
                            if (reader.GetInt32(0) == 0) dbp[0].ParamValue = "";
                        }
                    }
                }
                // затем выбираем все параметры шаблона
                query = "select tp.*, u1.name as name " +
                               " from child_params tp " +
                               "      left join unit u1 on (u1.idnum=tp.paramId) " +
                               " where tp.username=@username " +
                               " and tp.parentId=@parentId " +
                               " order by tp.sortIndex";
                dt = dbwork.ExecSQL_toTable(query, dbp);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ChildParamNode node = parent.AddChildParam(row);
                        node.Attributes = LoadChildAttributes(node.Idnum);
                        node.SetFullName(parent.FullName);
                        //эта шняга для выставления сортИндекса у ChildParamNode
                        parent.AcceptChanges();
                    }
                    dt.Dispose();
                }
                return true;
            }
            catch (Exception ex)
            {
                log.ErrorException(String.Format("Text: {0}", parent.Text), ex);
                return false;
            }
        }

        private string GetTreeQuery(ref DB_Parameters Param, int parentId)
        {
            StringBuilder res = new StringBuilder();
            res.Append("select u.idnum,u.name,u.type,t.fkey");
            res.Append(",(select count(*) from unit u2 where u2.parent=u.idnum) as count_child");
            res.Append(",cast(p.value as decimal) as sortindex, u.parent as parent");
            res.Append(" from unit u");
            res.Append("      left join unit_type t on (u.type=t.idnum)");
            res.Append("      left join props p on (p.tablename='unit' and p.name='sortindex' and p.parentid=u.idnum)");
            if (parentId > 0)
            {
                Param = new DB_Parameters();
                Param.Add("parent", DbType.Int32, parentId, ParameterDirection.Input);
                res.Append(" where u.parent=@parent");
            }
            else
            {
                res.Append(" where u.parent is null");
            }
            res.Append(" order by sortindex");
            return res.ToString();
        }

        private UnitNode NewInstance(OperationState state, DataRow row)
        {
            UnitNode res;
            int type = (int)row["type"];
            //Object fkey = row["fkey"];

            // Создание узла из расширения
            UTypeNode unitType = UnitTypeManager.GetUnitType(state, type);
            if (unitType != null && unitType.ExtensionGUID != Guid.Empty)
            {
                res = ExtensionManager.NewUnitNode(unitType, row);
                if (res != null)
                    return res;
            }

            //if (UnitTypeId.Block.Equals(type) && CheckTypeHash(UnitTypeId.Block, fkey))
            //    res = new BlockNode(row);
            //else if (UnitTypeId.Channel.Equals(type) && CheckTypeHash(UnitTypeId.Channel, fkey))
            //    res = new ChannelNode(row);
            //else if (UnitTypeId.Parameter.Equals(type) && CheckTypeHash(UnitTypeId.Parameter, fkey))
            //    res = new LoadParameterNode(row);
            //else if (UnitTypeId.TEP.Equals(type) && CheckTypeHash(UnitTypeId.TEP, fkey))
            //    res = new CalcParameterNode(row);
            //else if (UnitTypeId.Graph.Equals(type) && CheckTypeHash(UnitTypeId.Graph, fkey))
            //    res = new GraphNode(row);
            //else if (UnitTypeId.Histogram.Equals(type) && CheckTypeHash(UnitTypeId.Histogram, fkey))
            //    res = new HistogramNode(row);
            //else if (UnitTypeId.MonitorTable.Equals(type) && CheckTypeHash(UnitTypeId.MonitorTable, fkey))
            //    res = new MonitorTableNode(row);
            //else if (UnitTypeId.Schema.Equals(type) && CheckTypeHash(UnitTypeId.Schema, fkey))
            //    res = new SchemaNode(row);
            //else if (UnitTypeId.ManualGate.Equals(type) && CheckTypeHash(UnitTypeId.ManualGate, fkey))
            //    res = new ParameterGateNode(row);
            //else if (UnitTypeId.TEPTemplate.Equals(type))
            //    res = new ParameterGateNode(row);
            //else if (UnitTypeId.OptimizeCalc.Equals(type))
            //    res = new OptimizationGateNode(row);
            //else if (UnitTypeId.ManualParameter.Equals(type) && CheckTypeHash(UnitTypeId.ManualParameter, fkey))
            //    res = new ManualParameterNode(row);
            //else if (UnitTypeId.Report.Equals(type) && CheckTypeHash(UnitTypeId.Report, fkey))
            //    res = new ReportNode(row);
            //else if (UnitTypeId.NormFunc.Equals(type) && CheckTypeHash(UnitTypeId.NormFunc, fkey))
            //    res = new NormFuncNode(row);
            //else if (UnitTypeId.ExcelReport.Equals(type) && CheckTypeHash(UnitTypeId.ExcelReport, fkey))
            //    res = new ExcelReportNode(row);

            if (type == (int)UnitTypeId.Block)
                res = new BlockNode(row);
            else if (type == (int)UnitTypeId.Channel)
                res = new ChannelNode(row);
            else if (type == (int)UnitTypeId.Parameter)
                res = new LoadParameterNode(row);
            else if (type == (int)UnitTypeId.TEP)
                res = new CalcParameterNode(row);
            else if (type == (int)UnitTypeId.Graph)
                res = new GraphNode(row);
            else if (type == (int)UnitTypeId.Histogram)
                res = new HistogramNode(row);
            else if (type == (int)UnitTypeId.MonitorTable)
                res = new MonitorTableNode(row);
            else if (type == (int)UnitTypeId.Schema)
                res = new SchemaNode(row);
            else if (type == (int)UnitTypeId.ManualGate)
                res = new ParameterGateNode(row);
            else if (type == (int)UnitTypeId.TEPTemplate)
                res = new ParameterGateNode(row);
            else if (type == (int)UnitTypeId.OptimizeCalc)
                res = new OptimizationGateNode(row);
            else if (type == (int)UnitTypeId.ManualParameter)
                res = new ManualParameterNode(row);
            else if (type == (int)UnitTypeId.Report)
                res = new ReportNode(row);
            else if (type == (int)UnitTypeId.NormFunc)
                res = new NormFuncNode(row);
            else if (type == (int)UnitTypeId.ExcelReport)
                res = new ExcelReportNode(row);
            //else if (UnitTypeId.Boiler.Equals(type) && CommonData.CheckTypeHash(UnitTypeId.Boiler, fkey))
            //    res = new MonitorTableNode(row);
            else res = new UnitNode(row);
            return res;
        }

        public UnitNode NewInstance(OperationState state, int type)
        {
            UnitNode res = null;
            // Создание узла из расширения
            UTypeNode unitType = UnitTypeManager.GetUnitType(state, type);
            //if (unitType == null)
            //    throw new Exception(String.Format("Не допустимый тип {0}", type));

            if (unitType != null && unitType.ExtensionGUID != Guid.Empty)
                res = ExtensionManager.NewUnitNode(unitType);

            if (res == null)
            {
                switch (type)
                {
                    case (int)UnitTypeId.Block: res = new BlockNode(); break;
                    case (int)UnitTypeId.Channel: res = new ChannelNode(); break;
                    case (int)UnitTypeId.NormFunc: res = new NormFuncNode(); break;
                    case (int)UnitTypeId.ExcelReport: res = new ExcelReportNode(); break;
                    case (int)UnitTypeId.TEPTemplate:
                    case (int)UnitTypeId.ManualGate: res = new ParameterGateNode(); break;
                    case (int)UnitTypeId.OptimizeCalc: res = new OptimizationGateNode(); break;
                    case (int)UnitTypeId.ManualParameter: res = new ManualParameterNode(); break;
                    case (int)UnitTypeId.Parameter: res = new LoadParameterNode(); break;
                    case (int)UnitTypeId.Report: res = new ReportNode(); break;
                    case (int)UnitTypeId.Schema: res = new SchemaNode(); break;
                    case (int)UnitTypeId.Graph: res = new GraphNode(); break;
                    case (int)UnitTypeId.Histogram: res = new HistogramNode(); break;
                    case (int)UnitTypeId.MonitorTable: res = new MonitorTableNode(); break;
                    case (int)UnitTypeId.TEP: res = new CalcParameterNode(); break;
                    //case UnitTypeId.Boiler: res = new MonitorTableNode(); break;
                    default: res = new UnitNode(); break;
                }
            }
            res.Typ = type;
            //res.Text = String.Format("Новый {0}", unitType.Text);
            res.Text = unitType == null ? "Элемент" : unitType.Text;
            return res;
        }

        /// <summary>
        /// очистить структуру
        /// </summary>
        private void Clear()
        {
            if (!Monitor.TryEnter(units, 10000))
            {
                log.Error("GNSI.Clear: Серверный список оборудования занят.");
                return;
            }
            try
            {
                units.Clear();
            }
            finally { Monitor.Exit(units); }
        }
        #endregion

        /// <summary>
        /// Получить все нормативные функции
        /// </summary>
        /// <returns></returns>
        public NormFuncNode[] GetNormFuncs(OperationState state)
        {
            return normFuncNodes.ToArray();
        }

        /// <summary>
        /// Получить все параметры
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ParameterNode> GetParameters(OperationState state)
        {
            //return dicParameters.Values;
            return parametersList.ToArray();
        }

        public ParameterNode GetParameter(OperationState state, string code)
        {
            SecurityManager.ValidateAccess(state.UserGUID);

            ParameterNode ret = null;
            if (!string.IsNullOrEmpty(code))
                dicParameters.TryGetValue(code, out ret);
            if (ret == null) return null;

            SecurityManager.ValidateAccess(state.UserGUID, ret, Privileges.Read);

            SetFullName(ret);
            return ret;
        }

        #region Работа с юнит нодами

        private UnitNode GetUnitNode(int idnum)
        {
            UnitNode unitNode;

            if (dicUnits.TryGetValue(idnum, out unitNode))
            {
                SetFullName(unitNode);
            }

            return unitNode;
        }

        /// <summary>
        /// Ищет дочерние ноды.
        /// </summary>
        /// <param name="userGUID">
        ///     идентификатор сессии пользователя
        /// </param>
        /// <param name="id">
        ///     родительский ид.
        /// </param>
        /// <param name="filterTypes">
        ///     фильтр типов дочерних узлов.
        /// </param>
        /// <returns>
        ///     Массив нодов. Если указан фильтр, то вернуться могут не только
        ///     дочерние ноды, а также внуки, правнуки и т.д., пока не будут найдены
        ///     совпадения по типу.
        /// </returns>
        public UnitNode[] GetUnitNodes(OperationState state, int id, int[] filterTypes, Privileges privileges)
        {
            int[] nodes;

            var node = CheckUnitNode(state, id, privileges);
            if (node == null) nodes = units.ConvertAll<int>(x => x.Idnum).ToArray();
            else nodes = node.NodesIds;
            var lstUnits = new List<UnitNode>();
            UnitNode ptr;
            foreach (var item in nodes)
                if ((ptr = GetUnitNode(state, item, filterTypes, privileges)) != null)
                {
                    SetFullName(ptr);
                    lstUnits.Add(ptr);
                }

            lstUnits.Sort((a, b) =>
            {
                int ret = a.Index.CompareTo(b.Index);
                if (ret == 0) ret = a.Idnum.CompareTo(b.Idnum);
                return ret;
            });

            return lstUnits.ToArray();
            //BUG{
            //var bf = new BinaryFormatter();
            //byte[] buf;
            //using (var ms = new MemoryStream())
            //{
            //    bf.Serialize(ms, lstUnits);
            //    buf = ms.GetBuffer();
            //}
            //return buf;
            //}            
        }

        public UnitNode[] GetAllUnitNodes(OperationState state, int id, int[] filterTypes, Privileges privileges)
        {
            return GetAllUnitNodes(state, id, filterTypes, -1, 0, privileges);
        }

        public UnitNode[] GetAllUnitNodes(OperationState state, int id, int[] filterTypes, int minLevel, int maxLevel, Privileges privileges)
        {
            var lstUnits = new List<UnitNode>();
            UnitNode ptr;
            int[] nodes;

            //var node = GetUnitNode(state, id);
            // get root node
            var node = GetUnitNode(id);

            if (node == null)
            {
                nodes = units.ConvertAll<int>(x => x.Idnum).ToArray();
            }
            else
            {
                if (!SecurityManager.CheckAccess(state.UserGUID, node, privileges))
                    return lstUnits.ToArray();
                nodes = node.NodesIds;
            }

            // при minLevel <0 не учитывать уровни
            if (minLevel < 0)
            {
                minLevel = 0;
                maxLevel = int.MaxValue;
            }

            int nextMinLevel = minLevel - 1;
            int nextMaxLevel = maxLevel - 1;

            if (nextMinLevel < 0)
                nextMinLevel = 0;

            foreach (var item in nodes)
            {
                //ptr = ValidateUnitNode<UnitNode>(userGUID, item, privileges);
                //ptr = GetUnitNode(state, item);
                //if (ptr == null || !SecurityManager.CheckAccess(state.UserGUID, ptr, privileges))
                //    continue;
                ptr = CheckUnitNode(state, item, privileges);
                if (ptr != null)
                {
                    if (minLevel < 1
                        && (filterTypes == null || filterTypes.Length == 0 || filterTypes.Contains(ptr.Typ)))
                    {
                        SetFullName(ptr);
                        lstUnits.Add(ptr);
                    }

                    if (nextMaxLevel >= 0)
                        lstUnits.AddRange(GetAllUnitNodes(state, ptr.Idnum, filterTypes, nextMinLevel, nextMaxLevel, privileges));
                }
            }
            //lstUnits.Sort((a, b) => a.Index.CompareTo(b.Index));
            return lstUnits.ToArray();
        }

        #region ValidateUnitNode() & CheckUnitNode()

        public UnitNode ValidateUnitNode(OperationState state, int unitNodeID, Privileges privileges)
        {
            UnitNode unitNode = GetUnitNode(unitNodeID);

            if (unitNode == null)
                throw new Exception("Узел не найден");

            SecurityManager.ValidateAccess(state.UserGUID, unitNode, privileges);

            return unitNode;
        }

        public T ValidateUnitNode<T>(OperationState state, int unitNodeID, Privileges privileges)
            where T : UnitNode
        {
            UnitNode unitNode = ValidateUnitNode(state, unitNodeID, privileges);

            T specificUnitNode = unitNode as T;

            if (specificUnitNode == null)
                throw new Exception("Узел не подходящиго типа");

            return specificUnitNode;
        }

        public UnitNode CheckUnitNode(OperationState state, int unitNodeID, Privileges privileges)
        {
            UnitNode unitNode = GetUnitNode(unitNodeID);
            //T specificUnitNode = unitNode as T;

            if (unitNode == null)
            {
                //state.AddMessage(new Message(MessageCategory.Error, "Узел не найден"));
                return null;
            }

            if (!SecurityManager.CheckAccess(state, unitNode, privileges))
                return null;

            return unitNode;
            //if (specificUnitNode == null)
            //{
            //    state.AddMessage(new Message(MessageCategory.Error, "Узел не подходящиго типа"));
            //    return default(T);
            //}

            //return specificUnitNode;
        }

        public T CheckUnitNode<T>(OperationState state, int unitNodeID, Privileges privileges)
            where T : UnitNode
        {
            UnitNode unitNode = CheckUnitNode(state, unitNodeID, privileges);
            T specificUnitNode = unitNode as T;

            //if (unitNode == null)
            //{
            //    state.AddMessage(new Message(MessageCategory.Error, "Узел не найден"));
            //    return default(T);
            //}

            //if (!SecurityManager.CheckAccess(state, unitNode, privileges))
            //    return default(T);

            if (specificUnitNode == null)
            {
                //state.AddMessage(new Message(MessageCategory.Error, "Узел не подходящиго типа"));
                return default(T);
            }

            return specificUnitNode;
        }

        public T ValidateParentNode<T>(OperationState state, int unitNodeID, Privileges privileges)
            where T : UnitNode
        {
            UnitNode childNode = ValidateUnitNode<UnitNode>(state, unitNodeID, Privileges.Read);
            UnitNode unitNode = childNode;
            T parentNode = null;

            while (unitNode != null && (parentNode = unitNode as T) == null)
                unitNode = ValidateUnitNode<UnitNode>(state, unitNode.ParentId, Privileges.Read);

            if (parentNode == null)
                throw new ISTOKException(String.Format("Родительский узел с требуемым типом для '{0}', не найден", childNode.FullName));

            SecurityManager.CheckAccess(state.UserGUID, parentNode, privileges);

            return parentNode;
        }

        public T CheckParentNode<T>(OperationState state, int unitNodeID, Privileges privileges)
            where T : UnitNode
        {
            UnitNode childNode = CheckUnitNode<UnitNode>(state, unitNodeID, Privileges.Read);
            UnitNode unitNode = childNode;
            T parentNode = null;

            while (unitNode != null && (parentNode = unitNode as T) == null)
                unitNode = CheckUnitNode<UnitNode>(state, unitNode.ParentId, Privileges.Read);

            //if (parentNode == null)
            //    throw new ISTOKException(String.Format("Родительский узел с требуемым типом для '{0}', не найден", childNode.FullName));

            if (parentNode != null
                && SecurityManager.CheckAccess(state.UserGUID, parentNode, privileges))
                return parentNode;

            //return parentNode;
            return null;
        }
        #endregion

        public UnitNode GetUnitNode(OperationState state, int unitId, int[] filterTypes, Privileges privileges)
        {
            UnitNode ptr = CheckUnitNode<UnitNode>(state, unitId, privileges);

            if (ptr != null)
            {
                SetFullName(ptr);
                if (filterTypes != null && filterTypes.Length > 0)
                {
                    if (filterTypes.Contains(ptr.Typ))
                        return ptr;
                    else
                    {
                        UnitNode[] res = GetUnitNodes(state, ptr.Idnum, filterTypes, privileges);

                        if (res.Length > 0) return ptr;
                    }
                }
                else
                    return ptr;
            }
            return null;
        }

        /// <summary>
        /// Ищет дочерние ноды по фильтру.
        /// </summary>
        /// <param name="userGUID">
        ///     идентификатор сессии пользователя
        /// </param>
        /// <param name="id">
        ///     родительский ид.
        /// </param>
        /// <param name="filterTypes">
        ///     фильтр параметров.
        /// </param>
        /// <returns>
        ///     Массив нодов.
        /// </returns>
        public UnitNode[] GetUnitNodes(OperationState state, int id, ParameterFilter filter, RevisionInfo revision)
        {
            List<UnitNode> lstRes = new List<UnitNode>();
            UnitNode[] arr;

            int[] nodes = null;
            UnitNode node;

            if (id <= 0)
                nodes = units.ConvertAll<int>(x => x.Idnum).ToArray();
            else if ((node = CheckUnitNode<UnitNode>(state, id, Privileges.Read)) != null)
            {
                if (filter.Check(node, revision))
                {
                    SetFullName(node);
                    lstRes.Add(node);
                }
                nodes = node.NodesIds;
            }

            if (nodes != null)
                foreach (var item in nodes)
                    if ((arr = GetUnitNodes(state, item, filter, revision)) != null)
                        lstRes.AddRange(arr);

            return lstRes.ToArray();
        }

        public UnitNode[] GetUnitNodes(OperationState state, int[] ids)
        {
            List<UnitNode> lstRes = new List<UnitNode>();
            UnitNode ptr;

            SecurityManager.ValidateAccess(state.UserGUID);

            foreach (var item in ids)
            {
                ptr = CheckUnitNode<UnitNode>(state, item, Privileges.Read);
                if (ptr != null)
                {
                    SetFullName(ptr);
                    lstRes.Add(ptr);
                }
            }

            return lstRes.ToArray();
        }

        public TreeWrapp<UnitNode>[] GetUnitNodeTree(OperationState state, int[] unitIds,
            int[] filterTypes, Privileges privileges)
        {
            UnitNode node;
            List<TreeWrapp<UnitNode>> wrappList = new List<TreeWrapp<UnitNode>>();

            if (unitIds == null) unitIds = units.ConvertAll<int>(x => x.Idnum).ToArray();
            double step = AsyncOperation.MaxProgressValue / unitIds.Length, progress;
            foreach (int unitId in unitIds)
            {
                progress = state.Progress;
                node = GetUnitNode(state, unitId, filterTypes, privileges);
                if (node != null)
                    wrappList.Add(GetUnitNodeTree(state, step, node, filterTypes, privileges));
                state.Progress = progress + step;
            }

            return wrappList.ToArray();
        }

        private TreeWrapp<UnitNode> GetUnitNodeTree(OperationState state, double step,
            UnitNode unitNode, int[] filterTypes, Privileges privileges)
        {
            TreeWrapp<UnitNode> root = new TreeWrapp<UnitNode>(unitNode);
            UnitNode[] nodes = GetUnitNodes(state, unitNode.Idnum, filterTypes, privileges);
            double progress = state.Progress, curStep = step / nodes.Length;

            foreach (UnitNode node in nodes)
                root.AddChild(GetUnitNodeTree(state, curStep, node, filterTypes, privileges));

            state.Progress = progress + step;
            return root;
        }

        public IEnumerable<UnitNode> AddUnitNode(OperationState state, IEnumerable<UnitNode> nodes, int parent)
        {
            UnitNode pnode = null;
            List<UnitNode> resList = new List<UnitNode>();

            if (parent > 0)
            {
                pnode = ValidateUnitNode<UnitNode>(state, parent, Privileges.Write);
            }

            foreach (UnitNode node in nodes)
            {
                resList.Add(AddUnitNode(state, parent, pnode, node));
            }

            resList.ForEach(OnUnitNodeChanged);

            return resList;
        }

        /// <summary>
        /// Добавить узел к указанному родительскому элементу
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="parent">ИД родительского элемента. 0, если корень</param>
        /// <param name="pnode">Родительский элемент, null если корень</param>
        /// <param name="node">Добавляемый элемент</param>
        /// <returns>Новый элемент</returns>
        private UnitNode AddUnitNode(OperationState state, int parent, UnitNode pnode, UnitNode node)
        {
            UnitNode res;
            PreAddUnitNode(state, pnode, node);
            int index = 0;
            IEnumerable<UnitNode> childs;
            if (pnode != null)
                childs = GetUnitNodes(state, pnode.NodesIds);
            else childs = units;
            foreach (UnitNode childNode in childs)
            {
                if (childNode.Index > index) index = childNode.Index;
            }
            node.Index = ++index;
            node.ParentId = parent;
            int newId = UpdateUnitNode(node, true, 0, null);

            //if (pnode != null) pnode.AddNode(res);
            LoadUnit(state, parent);

            res = LoadUnit(state, newId);
            SetFullName(res);

            if (log.IsTraceEnabled)
            {
                log.Trace("Пользователь {0} добавил новый узел {1}.",
                       SecurityManager.GetUserInfo(state.UserGUID).Text,
                       GetFullName(res));
            }
            return res;
        }

        /// <summary>
        /// Провести проверки перед добавлением Узла
        /// </summary>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="pnode">Родительский узел</param>
        /// <param name="node">Узел</param>
        private void PreAddUnitNode(OperationState state, UnitNode pnode, UnitNode node)
        {
            UTypeNode parentType;
            if (pnode != null)
            {
                //int parentTypeId = (int)pnode.Typ, nodeTypeId = (int)node.Typ;
                SecurityManager.ValidateAccess(state.UserGUID, pnode, Privileges.Write);

                //parentType = types.Find(x => x.Idnum == parentTypeId);
                parentType = UnitTypeManager.GetUnitType(state, pnode.Typ);
                if (!parentType.ChildFilterAll && !parentType.ChildFilter.Contains((int)node.Typ))//nodeTypeId))
                {
                    //UTypeNode nodeType = types.Find(x => x.Idnum == nodeTypeId);
                    UTypeNode nodeType = UnitTypeManager.GetUnitType(state, node.Typ);
                    throw new Exception(String.Format("В элемент типа {0} нельзя добавить элемент типа {1}", parentType.Text, nodeType.Text));
                }
            }
        }

        public void RemoveUnitNode(OperationState state, int[] remnodes)
        {
            bool locked;
            List<UnitNode> releaseNodes = new List<UnitNode>();

            // состовляем плоский список всех удаляемых узлов
            var nodes = GetAllUnitNodes(state, Privileges.Write, remnodes).ToList();

            int transaction = 0;
            try
            {
                // берем все удаляемые узлы на редактирование
                foreach (var node in nodes)
                {
                    locked = LockManager.LockNode(state, node);
                    if (locked)
                        releaseNodes.Add(node);
                }

                // удаляем все узлы одной транзакцией
                transaction = dbwork.StartTransaction();

                foreach (var node in nodes)
                {
                    RemoveUnitNode(transaction, node);
                    state.AddAsyncResult(node);
                }
                dbwork.Commit(transaction);

                lock (dicUnits)
                {
                    // удалить узлы из справочника
                    foreach (var node in nodes)
                    {
                        dicUnits.Remove(node.Idnum);

                        RemoveNodeIndexes(node);

                        if (remnodes.Contains(node.Idnum))
                        {
                            //UnitNode parentNode = GetUnitNode(state, node.ParentId);
                            UnitNode parentNode = GetUnitNode(node.ParentId);

                            if (parentNode == null)
                                units.Remove(node);
                            else
                                parentNode.RemoveNode(node);
                        }

                        if (log.IsTraceEnabled)
                        {
                            log.Trace("Узел {1} удалён пользователем {0}",
                                               SecurityManager.GetUserInfo(state.UserGUID).Text,
                                               node.FullName);
                        }
                    }

                    var nodeIdes = from x in nodes select x.Idnum;

                    foreach (var unit in dicUnits.Values)
                    {
                        if (unit.Parameters != null)
                        {
                            var lst = (from elem in unit.Parameters
                                       where !nodeIdes.Contains(elem.ParameterId)
                                       select elem).ToArray();
                            if (lst.Length != unit.Parameters.Length) unit.Parameters = lst;
                        }
                    }
                }
                foreach (var item in nodes)
                {
                    OnUnitNodeChanged(item);
                }
            }
            //catch
            //{
            //    dbwork.Rollback(transaction);
            //    throw;
            //}
            finally
            {
                dbwork.CloseTransaction(transaction);
                // освобождаем залоченные узлы
                foreach (var node in releaseNodes)
                {
                    LockManager.ReleaseNode(state, node);
                }
            }
        }

        private void RemoveUnitNode(int transaction, UnitNode curnode)
        {
            string query = "delete from props where parentid=@p_parentid";
            DB_Parameters dbp = new DB_Parameters();
            //dbp.Add("p_tablename", DbType.String, "unit");
            dbp.Add("p_parentid", DbType.Int32, curnode.Idnum);
            dbwork.ExecSQL(transaction, query, dbp);
            //dbwork.Commit(transaction);
            // delete all parameter values
            String valuesDeleteQuery = "delete from value_mparam where idparam=@p_parentid";
            dbwork.ExecSQL(transaction, valuesDeleteQuery, dbp);
            //dbwork.Commit(transaction);
            // удалить Дополнительные свойства объекта
            if (curnode.Parameters != null)
            {
                foreach (var item in curnode.Parameters)
                {
                    query = "delete from child_props where parentid=@p_parentid";
                    dbp = new DB_Parameters();
                    //dbp.Add("p_tablename", DbType.String, "child_params");
                    dbp.Add("p_parentid", DbType.Int32, item.Idnum);
                    dbwork.ExecSQL(transaction, query, dbp);
                }
            }
            query = "delete from child_params where parentId=@p_idnum or paramId=@p_idnum";
            dbp.Add("p_idnum", DbType.Int32, curnode.Idnum);
            dbwork.ExecSQL(transaction, query, dbp);
            //dbwork.Commit(transaction);
            // удалим запись из таблицы оборудования
            query = "delete from unit where idnum=@p_idnum";
            dbp = new DB_Parameters();
            dbp.Add("p_idnum", DbType.Int32, curnode.Idnum);
            dbwork.ExecSQL(transaction, query, dbp);
            //dbwork.Commit(transaction);

            //lock (dicUnits) dicUnits.Remove(curnode.Idnum);
            //UnitNode parentNode = GetUnitNode(curnode.ParentId);
            //if (parentNode == null) units.Remove(curnode);
            //else
            //{
            //    parentNode.RemoveNode(curnode);
            //}

            //ParameterNode param = curnode as ParameterNode;
            //if (param != null)
            //{
            //    if (parametersList.Contains(param)) parametersList.Remove(param);
            //    if (!String.IsNullOrEmpty(param.Code) && dicParameters.ContainsKey(param.Code))
            //        dicParameters.Remove(param.Code);
            //}
            //RemoveNodeIndexes(curnode);
        }

        private IEnumerable<UnitNode> GetAllUnitNodes(OperationState state, Privileges privileges, params int[] nodeIds)
        {
            Stack<int> unitNodeIDStack = new Stack<int>();

            for (int i = nodeIds.Length - 1; i >= 0; i--)
            {
                unitNodeIDStack.Push(nodeIds[i]);

            }
            while (unitNodeIDStack.Count > 0)
            {
                int id = unitNodeIDStack.Pop();

                UnitNode node = ValidateUnitNode<UnitNode>(state, id, privileges);

                for (int i = node.NodesIds.Length - 1; i >= 0; i--)
                {
                    unitNodeIDStack.Push(node.NodesIds[i]);
                }

                yield return node;
            }
            //Stack<UnitNode> unitNodeStack = new Stack<UnitNode>();

            //for (int i = nodeIds.Length - 1; i >= 0; i--)
            //{
            //    unitNodeStack.Push(ValidateUnitNode<UnitNode>(userGuid, nodeIds[i], privileges));
            //}
            //while (unitNodeStack.Count>0)
            //{
            //    UnitNode node = unitNodeStack.Pop();

            //}
        }

        public UnitNode UpdateUnitNode(OperationState state, UnitNode updnode)
        {
            bool newItem = updnode.Idnum == 0;
            int nodeId = updnode.Idnum;
            bool locked = false;

            UnitNode curnode = null;

            if (!newItem)
            {
                curnode = ValidateUnitNode<UnitNode>(state, updnode.Idnum, Privileges.Write);

                locked = LockManager.LockNode(state, curnode);
            }

            // точка вхождения расширений для уведомления при сохранении узла
            try
            {
                ExtensionUnitNode extensionUnitNode = updnode as ExtensionUnitNode;

                if (extensionUnitNode != null)
                    ExtensionManager.NotifyChange(extensionUnitNode);
            }
            catch (Exception exc)
            {
                state.AddMessage(new MessageByException(exc));
            }

            try
            {
                nodeId = UpdateUnitNode(updnode, newItem, nodeId, curnode);
                RemoveNodeIndexes(curnode);
            }
            finally
            {
                if (locked)
                    LockManager.ReleaseNode(state, curnode);
            }

            curnode = LoadUnit(state, nodeId);
            SetFullName(curnode);

            if (log.IsInfoEnabled)
            {
                log.Info("Пользователь {0} отредактировал узел '{1}'.",
                                       SecurityManager.GetUserInfo(state.UserGUID).Text,
                                       GetFullName(curnode)); 
            }
            OnUnitNodeChanged(curnode);

            return curnode;
        }

        private void RemoveNodeIndexes(UnitNode curnode)
        {
            ParameterNode paramNode;
            NormFuncNode normFuncNode;
            if ((paramNode = curnode as ParameterNode) != null)
            {
                if (parametersList.Contains(paramNode))
                    parametersList.Remove(paramNode);
                ParameterNode anotherParamNode;
                if (dicParameters.TryGetValue(paramNode.Code, out anotherParamNode)
                    && paramNode == anotherParamNode)
                    dicParameters.Remove(curnode.Code);
            }
            if ((normFuncNode = curnode as NormFuncNode) != null)
                normFuncNodes.Remove(normFuncNode);
        }

        private int UpdateUnitNode(UnitNode updnode, bool newItem, int nodeId, UnitNode curnode)
        {
            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            int transaction = 0;
            try
            {
                updnode.AcceptChanges();
                transaction = dbwork.StartTransaction();

                // проверить дублирование кода параметра
                string scod = updnode.Code;
                if (!String.IsNullOrEmpty(scod))
                    lock (dicUnits)
                    {
                        foreach (var item in dicUnits.Keys)
                            if (item != updnode.Idnum && dicUnits[item].Code == scod)
                                throw new Exception("Код параметра '" + scod + "' дублирован.");
                    }

                string query;
                if (newItem)
                    query = "insert into unit (name,type,parent) values(@p_name,@p_type,@p_parent);select @@identity;";
                else
                    query = "update unit set name=@p_name,type=@p_type where idnum=@p_idnum";

                DB_Parameters dbp = new DB_Parameters();
                dbp.Add("p_name", DbType.String, updnode.Text);
                dbp.Add("p_type", DbType.Int32, updnode.Typ);
                dbp.Add("p_parent", DbType.Int32, updnode.ParentId > 0 ? (Object)updnode.ParentId : DBNull.Value);
                dbp.Add("p_idnum", DbType.Int32, nodeId);
                using (DbDataReader reader = dbwork.ExecSQL_toReader(transaction, query, dbp))
                {
                    if (newItem && reader.Read())
                        if (!reader.IsDBNull(0))
                            nodeId = Convert.ToInt32(reader.GetValue(0));
                }
                // Дополнительные свойства объекта
                query = "delete from child_props where " +
                    "parentid in (select idnum from child_params where parentId=@p_idnum)";
                dbp = new DB_Parameters();
                dbp.Add("p_idnum", DbType.Int32, nodeId);
                dbwork.ExecSQL(transaction, query, dbp);
                query = "delete from child_params where parentId=@p_idnum";
                dbp = new DB_Parameters();
                dbp.Add("p_idnum", DbType.Int32, nodeId);
                dbwork.ExecSQL(transaction, query, dbp);

                if (updnode.Parameters != null)
                {
                    foreach (var item in updnode.Parameters)
                    {
                        int childIdnum = 0;

                        query = "insert into child_params (username,parentId,paramId,paramCode) " +
                                            " values (@username,@parentId,@paramId,@paramCode); select @@identity;";
                        dbp = new DB_Parameters();
                        dbp.Add("username", DbType.String, "");
                        dbp.Add("parentId", DbType.Int32, nodeId);
                        dbp.Add("paramId", DbType.Int32, item.ParameterId);
                        dbp.Add("paramCode", DbType.String, "");
                        //dbp.Add("sortIndex", DbType.Int32, item.SortIndex);
                        using (DbDataReader reader = dbwork.ExecSQL_toReader(transaction, query, dbp))
                        {
                            if (reader.Read())
                                if (!reader.IsDBNull(0))
                                    childIdnum = Convert.ToInt32(reader.GetValue(0));
                        }

                        dbp = new DB_Parameters();
                        DB_Parameter parName = new DB_Parameter("p_name", DbType.String);
                        DB_Parameter parValue = new DB_Parameter("p_value", DbType.String);
                        dbp.Add("p_parentid", DbType.Int32, childIdnum);
                        dbp.Add(parName);
                        foreach (var attrib in item.Attributes.Keys)
                        {
                            parName.ParamValue = attrib;
                            parValue.ParamValue = item.Attributes[attrib];
                            dbp.Insert(0, parValue);
                            int row = dbwork.ExecSQL(transaction, "update child_props set value=@p_value where parentid=@p_parentid and name=@p_name", dbp);
                            if (row == 0) dbwork.ExecSQL(transaction, "insert into child_props (value,parentid,name) values (@p_value,@p_parentid,@p_name)", dbp);
                            dbp.Remove(parValue);
                        }
                    }
                }

                // Бинарные атрибуты
                dbp = new DB_Parameters();
                DB_Parameter pName = new DB_Parameter("p_name", DbType.String);
                DB_Parameter pValue = new DB_Parameter("p_value", DbType.Binary);
                DB_Parameter pRevision = new DB_Parameter("p_revision", DbType.Int32);
                dbp.Add("p_parentid", DbType.Int32, nodeId);
                dbp.Add(pName);
                foreach (var key in updnode.Binaries.Keys)
                {
                    var versionValue = updnode.Binaries[key];
                    StringBuilder deleteExcessBuilder = new StringBuilder();
                    pName.ParamValue = key;

                    deleteExcessBuilder.Append("delete from lobs where parentid=@p_parentid and name=@p_name");

                    foreach (var revision in versionValue)
                    {
                        pRevision.ParamValue = revision.Equals(RevisionInfo.Default) ? null : (Object)revision.ID;
                        byte[] data = updnode.GetBinaries(key, revision);

                        if (data != null && data.Length > 0)
                        {
                            pValue.ParamValue = data;// updnode.Binaries[key];
                            dbp.Insert(0, pRevision);
                            dbp.Insert(0, pValue);
                            int row = dbwork.ExecSQL(transaction, "update lobs set value=@p_value where parentid=@p_parentid and name=@p_name and (revision=@p_revision or (revision is null and @p_revision is null))", dbp);
                            if (row == 0) dbwork.ExecSQL(transaction, "insert into lobs (value,parentid,name,revision) values (@p_value,@p_parentid,@p_name,@p_revision)", dbp);
                            dbp.Remove(pRevision);
                            dbp.Remove(pValue);

                            if (revision.Equals(RevisionInfo.Default))
                                deleteExcessBuilder.Append(" and revision is not null");
                            else
                                deleteExcessBuilder.AppendFormat(" and revision<>'{0}'", revision.ID);
                        }
                    }
                    dbwork.ExecSQL(transaction, deleteExcessBuilder.ToString(), dbp);
                }

                // Атрибуты
                dbp = new DB_Parameters();
                pName = new DB_Parameter("p_name", DbType.String);
                pValue = new DB_Parameter("p_value", DbType.String);
                pRevision = new DB_Parameter("p_revision", DbType.Int32);
                dbp.Add("p_parentid", DbType.Int32, nodeId);
                dbp.Add(pName);

                foreach (string attrName in updnode.Attributes.Keys)
                {
                    UpdateUnitNodeProperty(updnode, dbp, transaction, pName, pValue, pRevision, attrName, updnode.Attributes[attrName]);
                }
                dbwork.Commit(transaction);
            }
            catch (Exception ex)
            {
                dbwork.Rollback(transaction);
                log.ErrorException("UnitList.Update", ex);
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                dbwork.CloseTransaction(transaction);
            }
            return nodeId;
        }

        /// <summary>
        /// Обновить свойство уззла в БД
        /// </summary>
        /// <param name="updnode">Узел</param>
        /// <param name="dbp">Список параметров скл запроса</param>
        /// <param name="transactionId"></param>
        /// <param name="pName">Параметр имени свойства</param>
        /// <param name="pValue">Параметр значения свойства</param>
        /// <param name="attrName">Имя свойства</param>
        /// <param name="attrValue">Значение свойства</param>
        private void UpdateUnitNodeProperty(UnitNode updnode,
            DB_Parameters dbp, int transaction, DB_Parameter pName, DB_Parameter pValue, DB_Parameter pRevision,
            string attrName, RevisedStorage<String> versionValue)//String attrValue)
        {
            StringBuilder deleteExcessBuilder = new StringBuilder();
            pName.ParamValue = attrName;

            deleteExcessBuilder.Append("delete from props where parentid=@p_parentid and name=@p_name");

            foreach (var revision in versionValue)
            {
                pRevision.ParamValue = revision.Equals(RevisionInfo.Default) ? DBNull.Value : (Object)revision.ID;
                String attrValue = updnode.GetAttribute(attrName, revision);

                if (!String.IsNullOrEmpty(attrValue))
                {
                    pValue.ParamValue = attrValue;
                    dbp.Insert(0, pRevision);
                    dbp.Insert(0, pValue);
                    int row = dbwork.ExecSQL(transaction, "update props set value=@p_value where parentid=@p_parentid and name=@p_name and (revision=@p_revision or (revision is null and @p_revision is null))", dbp);
                    if (row == 0) dbwork.ExecSQL(transaction, "insert into props (value,parentid,name,revision) values (@p_value,@p_parentid,@p_name,@p_revision)", dbp);
                    dbp.Remove(pValue);
                    dbp.Remove(pRevision);

                    if (revision.Equals(RevisionInfo.Default))
                        deleteExcessBuilder.Append(" and revision is not null");
                    else
                        deleteExcessBuilder.AppendFormat(" and revision<>'{0}'", revision.ID);
                }
                int count = dbwork.ExecSQL(transaction, deleteExcessBuilder.ToString(), dbp);
            }
        }

        /// <summary>
        /// Переместить узел в другой родительский узел
        /// </summary>
        /// <param name="state">Состяние операции</param>
        /// <param name="userGuid">Идентификатор сессии пользователя</param>
        /// <param name="newParentId">ИД нового родителя</param>
        /// <param name="unitNodeId">ИД переносимого узла</param>
        /// <param name="index">
        /// Индекс в новом родительском узле.
        /// Если index &lt; 0, добавить в конец.
        /// </param>
        public void MoveUnitNode(OperationState state, int newParentId, int unitNodeId, int index)
        {
            UnitNode unitNode, parentNode = null, oldParent = null;

            unitNode = ValidateUnitNode<UnitNode>(state, unitNodeId, Privileges.Write);

            if (unitNode.ParentId > 0)
                oldParent = ValidateUnitNode<UnitNode>(state, unitNode.ParentId, Privileges.Write);

            if (newParentId > 0)
                parentNode = ValidateUnitNode<UnitNode>(state, newParentId, Privileges.Write);

            PreAddUnitNode(state, parentNode, unitNode);

            if (index < 0)
            {
                if (unitNode.NodesIds.Length>0)
                {
                    index = (from id in unitNode.NodesIds select GetUnitNode(id).Index).Max() + 1;
                }
                else
                {
                    index = 0;
                }
            }

            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            int transaction = 0;
            bool locked = false;
            try
            {
                locked = LockManager.LockNode(state, unitNode);
                //if (oldParent != null) LockNode(userGuid, oldParent);
                //if (parentNode != null) LockNode(userGuid, parentNode);

                transaction = dbwork.StartTransaction();

                DB_Parameters dbp = new DB_Parameters();
                dbp.Add("p_parent", DbType.String, parentNode == null ? DBNull.Value : (Object)parentNode.Idnum);
                dbp.Add("p_idnum", DbType.Int32, unitNode.Idnum);

                String moveQuery = "update unit set parent=@p_parent where idnum=@p_idnum";
                dbwork.ExecSQL(transaction, moveQuery, dbp);
                //dbwork.Commit(transaction);

                if (oldParent != null) oldParent.RemoveNode(unitNode);
                else units.Remove(unitNode);

                unitNode.ParentId = newParentId;
                if (parentNode != null)
                    parentNode.AddNode(unitNode);
                else units.Add(unitNode);

                UpdateUnitNodeIndex(state, transaction, index, unitNode, parentNode, false);
                //if (parentNode != null) parentNode.SortNodes();
                dbwork.Commit(transaction);
                if (log.IsTraceEnabled)
                {
                    log.Trace("Пользователь {0} переместил узел {1} в {2} новый родительский {3}.",
                        SecurityManager.GetUserInfo(state.UserGUID).Text,
                        unitNode.Text,
                        index,
                        parentNode == null ? "ROOT" : parentNode.FullName);
                }
            }
            catch (Exception ex)
            {
                dbwork.Rollback(transaction);
                log.ErrorException("UnitManager.RemoveUnitNode", ex);
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                dbwork.CloseTransaction(transaction);
                //if (parentNode != null) ReleaseNode(userGuid, parentNode);
                //if (oldParent != null) ReleaseNode(userGuid, oldParent);
                if (locked)
                    LockManager.ReleaseNode(state, unitNode);
            }
        }

        /// <summary>
        /// Обновить индексы сортировки внутри узла
        /// </summary>
        /// <param name="transactionID"></param>
        /// <param name="index">Новый индекс узла</param>
        /// <param name="unitNode">Переносимый узел</param>
        /// <param name="parentNode">Родительский узел, null - если корень </param>
        /// <param name="sort">true, если происходит сортировка внутри узла, false - если узел переноситься от другого родителя</param>
        private void UpdateUnitNodeIndex(OperationState state, int transactionID, int index, UnitNode unitNode, UnitNode parentNode, bool sort)
        {
            List<UnitNode> indexUnitNodeList;
            if (parentNode != null)
                indexUnitNodeList = new List<UnitNode>(GetUnitNodes(state, parentNode.NodesIds));
            else
                indexUnitNodeList = new List<UnitNode>(units);

            indexUnitNodeList.Sort((a, b) =>
            {
                int ret = a.Index.CompareTo(b.Index);
                if (ret == 0) ret = a.Idnum.CompareTo(b.Idnum);
                return ret;
            });
            if (indexUnitNodeList.Contains(unitNode))
            {
                if (sort && indexUnitNodeList.IndexOf(unitNode) < index) --index;
                indexUnitNodeList.Remove(unitNode);
            }
            if (index >= 0 && index < indexUnitNodeList.Count) indexUnitNodeList.Insert(index, unitNode);
            else indexUnitNodeList.Add(unitNode);

            // Атрибуты
            DB_Parameters dbParameters = new DB_Parameters();
            DB_Parameter pName = new DB_Parameter("p_name", DbType.String);
            DB_Parameter pValue = new DB_Parameter("p_value", DbType.String);
            DB_Parameter pParent = new DB_Parameter("p_parentid", DbType.Int32);
            DB_Parameter pRevision = new DB_Parameter("p_revision", DbType.Int32);
            //dbParameters.Add("p_parentid", DbType.Int32, indexUnitNode.Idnum);
            dbParameters.Add("p_tablename", DbType.String, "unit");
            dbParameters.Add(pName);
            dbParameters.Add(pParent);
            for (int i = 0; i < indexUnitNodeList.Count; i++)
            {
                indexUnitNodeList[i].Index = i;
                pParent.ParamValue = indexUnitNodeList[i].Idnum;
                UpdateUnitNodeProperty(indexUnitNodeList[i], dbParameters, transactionID, pName, pValue, pRevision, "sortindex", indexUnitNodeList[i].Attributes["sortindex"]);
            }
        }

        /// <summary>
        /// Копировать узел или ветку узлов
        /// </summary>
        /// <param name="state">Состояние операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitIDs">Ид копируемых узлов</param>
        /// <param name="recursive">true, если копируется ветки и false - отдельные элементы</param>
        public void CopyUnitNode(OperationState state, int[] unitIDs, bool recursive)
        {
            SecurityManager.ValidateAccess(state.UserGUID);

            double step = AsyncOperation.MaxProgressValue / unitIDs.Length;
            state.StateString = "Копирование узлов";
            CopyUnitNode(state, unitIDs, null, step, recursive);
        }

        /// <summary>
        /// Вспомогательный метод копирования узлов
        /// </summary>
        /// <param name="state">Состояние операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="unitIDs">Ид копируемых узлов</param>
        /// <param name="parentNode">Родительский узел в который добавляются узлы</param>
        /// <param name="step">Шаг на котрый меняется прогресс при копировании каждого элемента</param>
        /// <param name="recursive">true, если копируется ветки и false - отдельные элементы</param>
        private void CopyUnitNode(OperationState state, int[] unitIDs, UnitNode parentNode, double step, bool recursive)
        {
            double progrss = state.Progress;
            double recStep = step / unitIDs.Length;

            foreach (int unitNodeId in unitIDs)
            {
                int suffixNum = 0;
                String name, code;
                UnitNode unitNode, newNode;
                IEnumerable<UnitNode> levelNodes;

                if (!dicUnits.TryGetValue(unitNodeId, out unitNode))
                {
                    state.AddMessage(new Message(MessageCategory.Error, "Узел {0} не найден", unitNodeId));
                    continue;//throw new Exception("Узел не найден");
                }

                if (SecurityManager.CheckAccess(state.UserGUID, unitNode, Privileges.Read))
                {
                    if (!SecurityManager.CheckAccess(state.UserGUID, unitNode, Privileges.Write))
                    {
                        state.AddMessage(new Message(MessageCategory.Error, "Нельзя скопировать элемент {0}", unitNode.Text));
                        continue;
                    }

                    newNode = unitNode.Clone() as UnitNode;
                    newNode.ClearNodes();

                    UnitNode unitParentNode;

                    if (parentNode != null)
                        unitParentNode = parentNode;
                    else
                        unitParentNode = GetUnitNode(unitNode.ParentId);

                    newNode.Idnum = 0;
                    if (unitParentNode != null)
                        newNode.ParentId = unitParentNode.Idnum;

                    // выбор нового имени, если на уровне такой уже есть
                    if (unitParentNode == null) levelNodes = units;
                    else levelNodes = GetUnitNodes(state, unitParentNode.NodesIds);
                    name = newNode.Text;
                    bool nameCont = true;
                    while (nameCont)
                    {
                        nameCont = false;
                        foreach (UnitNode levelNode in levelNodes)
                            if (levelNode.Text.Equals(name))
                            {
                                nameCont = true;
                                break;
                            }

                        if (nameCont) name = String.Format("{0}_{1}", newNode.Text, ++suffixNum);
                    }
                    newNode.Text = name;

                    // выбор нового кода, если такой уже есть
                    ParameterNode param = newNode as ParameterNode;
                    if (param != null)
                    {
                        suffixNum = 0;
                        code = param.Code;
                        while (dicParameters.ContainsKey(code)) code = String.Format("{0}_{1}", param.Code, ++suffixNum);
                        param.Code = code;
                    }

                    newNode = UpdateUnitNode(state, newNode);

                    if (newNode.ParentId == unitNode.ParentId)
                        UpdateUnitNodeIndex(state, MyDBdata.WithoutTransactionID, unitNode.Index + 1, newNode, unitParentNode, false);

                    // добавляем узел в результат
                    state.AddAsyncResult(newNode);

                    if (log.IsTraceEnabled)
                    {
                        log.Trace("Пользователь {0} скопировал узел {1} в {2}.",
                               SecurityManager.GetUserInfo(state.UserGUID).Text,
                               GetFullName(unitNode),
                               GetFullName(newNode));
                    }

                    if (recursive)
                        CopyUnitNode(state, unitNode.NodesIds, newNode, recStep, recursive);
                }
            }
            state.Progress = progrss + step;
        }

        /// <summary>
        /// Сохранение импортированных узлов
        /// </summary>
        /// <param name="state">Состояние операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="importRootNode">
        /// Базовый узел, в который будут импортироваться данные. 
        /// Для импорта в корень - должен быть null.
        /// </param>
        /// <param name="importContainer">Обертка добавляемого дерева</param>
        public void ApplyImport(OperationState state, UnitNode importRootNode, ImportDataContainer importContainer)
        {
            SecurityManager.ValidateAdminAccess(state.UserGUID);

            int count = 0;
            foreach (TreeWrapp<UnitNode> nodeWrap in importContainer.Nodes)
                count += nodeWrap.RecursiveCount;
            double delta = AsyncOperation.MaxProgressValue / count;

            state.StateString = "Импорт элементов";
            ApplyImport(state, importContainer.Nodes, importRootNode, delta);
        }

        /// <summary>
        /// Вспомогательный метод сохранения импортированных узлов
        /// </summary>
        /// <param name="state">Состояние операции</param>
        /// <param name="userGUID">Идентификатор сессии пользователя</param>
        /// <param name="nodeWrappes">Обертка добавляемого дерева</param>
        /// <param name="baseUnitNode">Родительский элемент</param>
        /// <param name="delta">На сколько меняется прогресс при добавлении каждого элемента</param>
        private void ApplyImport(OperationState state, TreeWrapp<UnitNode>[] nodeWrappes, UnitNode baseUnitNode, double delta)
        {
            IEnumerable<UnitNode> baseUnitNodes;

            if (baseUnitNode == null)
                baseUnitNodes = units;
            else
                baseUnitNodes = GetUnitNodes(state, baseUnitNode.NodesIds);

            foreach (TreeWrapp<UnitNode> nodeWrapp in nodeWrappes)
            {
                UnitNode node = nodeWrapp.Item;
                UnitNode baseNode = null;

                foreach (UnitNode unitNode in baseUnitNodes)
                {
                    // Если совпадают типы узлов и их коды,
                    // и когда коды пустые, если совпадают названия,
                    // то обновить существующий узел
                    if (unitNode.Typ == node.Typ
                        && unitNode.Code.Equals(node.Code)
                        && (!String.IsNullOrEmpty(unitNode.Code)
                        || unitNode.Text.Equals(node.Text)))
                    //&& ((parameterNode = unitNode as ParameterNode) == null 
                    //|| parameterNode.Code.Equals(((ParameterNode)node).Code)))
                    {
                        baseNode = unitNode;
                        break;
                    }
                }
                if (baseNode != null)
                {
                    node.Idnum = baseNode.Idnum;
                    node = MergeUnitNodeRevision(baseNode, node);
                    nodeWrapp.OldItem = baseNode.Clone() as UnitNode;
                }
                if (baseUnitNode != null)
                    node.ParentId = baseUnitNode.Idnum;

                nodeWrapp.Item = UpdateUnitNode(state, node);
                // передаём изменённый узел в результат асинхронной операции
                state.AddAsyncResult(nodeWrapp);

                state.Progress += delta;
                ApplyImport(state, nodeWrapp.ChildNodes, nodeWrapp.Item, delta);
            }
        }

        private UnitNode MergeUnitNodeRevision(UnitNode baseNode, UnitNode diffNode)
        {
            UnitNode unitNode = baseNode.Clone() as UnitNode;

            unitNode.Text = diffNode.Text;
            unitNode.Typ = diffNode.Typ;

            foreach (var attributeName in diffNode.Attributes.Keys)
            {
                foreach (var revision in diffNode.Attributes[attributeName])
                {
                    String attributeValue = diffNode.GetAttribute(attributeName, revision);
                    unitNode.SetAttribute(attributeName, revision, attributeValue);
                }
            }

            foreach (var attributeName in diffNode.Binaries.Keys)
            {
                foreach (var revision in diffNode.Binaries[attributeName])
                {
                    byte[] attributeValue = diffNode.GetBinaries(attributeName, revision);
                    unitNode.SetBinaries(attributeName, revision, attributeValue);
                }
            }

            return unitNode;
        }
        #endregion

        /// <summary>
        /// Возвращает массив параметров которые выгружаются по
        /// данному расписанию.
        /// </summary>
        /// <param name="schedule_id">
        ///     ид расписания.
        /// </param>
        /// <returns>
        ///     массив параметров
        /// </returns>
        public ParameterNode[] GetParametersForSchedule(OperationState state, string uid, int schedule_id)
        {
            lock (sync_obj)
            {
                BlockNode block = BlockProxy.GetBlock(uid);

                if (block == null)
                    return null;

                UnitNode[] childNodes = GetChildNodes(state, block, (int)UnitTypeId.Parameter);

                var parametrs = from it in childNodes
                                where it.Typ == (int)UnitTypeId.Parameter &&
                                      it.Attributes.ContainsKey(@"schedule") &&
                                      Convert.ToInt32(it.Attributes[@"schedule"]) == schedule_id //&&
                                //chanels.Contains(it.ParentId)
                                select it as ParameterNode;

                return parametrs.ToArray();
            }
        }

        public UnitNode[] GetChildNodes(OperationState state, UnitNode unitNode, int unitTypeId)
        {
            List<UnitNode> retNodes = new List<UnitNode>();
            Queue<UnitNode> unitNodeQueue = new Queue<UnitNode>();

            unitNodeQueue.Enqueue(unitNode);

            while (unitNodeQueue.Count > 0)
            {
                UnitNode tempUnitNode = unitNodeQueue.Dequeue();
                UnitNode[] nodes = GetUnitNodes(state, tempUnitNode.NodesIds);

                if (nodes != null)
                {
                    foreach (UnitNode item in nodes)
                    {
                        if (item.Typ == unitTypeId)
                            retNodes.Add(item);
                        unitNodeQueue.Enqueue(item);
                    }
                }
            }

            return retNodes.ToArray();
        }

        #region Сериализация данных для отправки на блочные

        /// <summary>
        /// Метод сериализует датасет для оправки на блочные.
        /// </summary>
        /// <param name="ds">
        ///     собственно сам датасет
        /// </param>
        /// <returns>
        ///     массив байт, в котором лежит сериализованный, 
        ///     сжатый датасет.
        /// </returns>
        public byte[] SerializeDataSet(OperationState state, DataSet ds)
        {
            // don't touch, it is work
            MemoryStream stream = null;
            using (stream = new MemoryStream())
            using (GZipStream zip = new GZipStream(stream, CompressionMode.Compress, true))
            {
                BinaryFormatter bf = new BinaryFormatter();
                ds.RemotingFormat = SerializationFormat.Binary;
                bf.Serialize(zip, ds);
            }

            return stream.ToArray();
        }

        /// <summary>
        /// Метод извлекает данные для отправки на форму.
        /// Извлекается вся структура для конкретного блочного.
        /// </summary>
        /// <param name="id_block">
        ///     идентификатор блочного
        /// </param>
        /// <return>
        ///     датасет с набором таблиц для отправки
        /// </return>
        public DataSet GetBlockData(OperationState state, int id_block)
        {
            DataSet result = new DataSet();

            var table = RecursiveGetUnits(UnitTypeId.Channel, id_block, @"idnum");

            var channels = from it in table.AsEnumerable()
                           select it.Field<int>(@"idnum");

            foreach (var it in channels)
                result.Merge(GetBlockData(state, id_block, it));

            return result;

        }

        /// <summary>
        /// Метод извлекает данные для отправки на блочный.
        /// Извлекаются данные только для конкретного канала,
        /// конкретного блочного.
        /// </summary>
        /// <param name="id_block">
        ///     айдишка блочного
        /// </param>
        /// <param name="id_chanell">
        ///     айдишка канала
        /// </param>
        /// <returns>
        ///     датасет с набором таблиц
        /// </returns>
        public DataSet GetBlockData(OperationState state, int id_block, int id_chanell)
        {
            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            DataSet result = new DataSet();
            DataTable table = null;
            DB_Parameters parametrs = new DB_Parameters();

            parametrs.Add(new DB_Parameter(@"@id", DbType.Int32, id_chanell));
            parametrs.Add(new DB_Parameter(@"@parent", DbType.Int32, id_block));

            table = dbwork.ExecSQL_toTable(@"SELECT idnum, name FROM unit " +
                                               @"WHERE idnum = @id and parent = @parent;",
                                           parametrs);

            parametrs.Clear();

            table.TableName = CommonData.CanalTableName;

            result.Tables.Add(table);

            table = RecursiveGetUnits(UnitTypeId.Parameter,
                                      id_chanell,
                                      @"idnum, name, parent");

            table.TableName = CommonData.ParameterTableName;

            result.Tables.Add(table);

            List<int> ids = (from it in table.AsEnumerable()
                             select it.Field<int>("idnum")).ToList();

            ids.Add(id_chanell);

            result.Merge(GetUnitsPropertiesForParentID(ids));

            //ids = (from it in result.Tables[@"properties"].AsEnumerable()
            //       where it.Field<string>(@"name") == @"schedule"
            //       select Convert.ToInt32(it.Field<string>(@"value"))).ToList();

            //UnitNode block;
            //dicUnits.TryGetValue(id_block, out block);
            //if (block is BlockNode)
            //{
            //    result.Merge(GetSchedulesForIDs(ids, ((BlockNode)block).BlockUID));
            //}

            return result;
        }

        /// <summary>
        /// Рекурсивно получает унит ноды заданного типа
        /// дл заданного родительского элемента.
        /// Просматриваются так же и все узлы сгруппированные в папки.
        /// </summary>
        /// <param name="type">
        ///     тип выбираемых элементов.
        /// </param>
        /// <param name="parent_id">
        ///     айдишка родительского элемента
        /// </param>
        /// <param name="selecting_columns">
        ///     набор полей которые должны выбираться.
        /// </param>
        /// <returns>
        ///     дататабле с выбранными записями
        /// </returns>
        private DataTable RecursiveGetUnits(UnitTypeId type,
                                            int parent_id,
                                            string selecting_columns)
        {
            const string query =
                @"SELECT {0} FROM unit WHERE parent = @parent and type = @type";

            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            DB_Parameters parametrs = new DB_Parameters();

            DB_Parameter type_param = new DB_Parameter(@"@type",
                                                       DbType.Int32,
                                                       UnitTypeId.Folder);
            DB_Parameter parent_param = new DB_Parameter(@"@parent",
                                                         DbType.Int32,
                                                         parent_id);

            parametrs.Add(type_param);
            parametrs.Add(parent_param);

            DataTable table = dbwork.ExecSQL_toTable(String.Format(query,
                                                                   @"idnum"),
                                                     parametrs);

            var folders = from it in table.AsEnumerable()
                          select it.Field<int>("idnum");

            type_param.ParamValue = type;

            table = dbwork.ExecSQL_toTable(String.Format(query,
                                                         selecting_columns),
                                           parametrs);

            foreach (var it in folders)
                table.Merge(RecursiveGetUnits(type, it, selecting_columns));

            return table;
        }

        /// <summary>
        /// Выбирает свойства унитнодов с заявленными идами.
        /// </summary>
        /// <param name="ids">
        ///     массив идов унит нодов, которым принадлежат 
        ///     выбираемые ноды.
        /// </param>
        /// <returns>
        ///     таблица с выбранными свойствами 
        /// </returns>
        private DataTable GetUnitsPropertiesForParentID(IEnumerable<int> ids)
        {
            if (dbwork == null) throw new Exception("Отсутствует подключение к серверу БД");

            DataTable result = new DataTable();
            DataTable table = null;

            DB_Parameters parametrs = new DB_Parameters();
            //parametrs.Add(new DB_Parameter(@"@tablename", DbType.String, @"unit"));
            DB_Parameter parent = new DB_Parameter(@"@parent", DbType.Int32);
            parametrs.Add(parent);

            foreach (var it in ids)
            {
                parent.ParamValue = it;

                table = dbwork.ExecSQL_toTable(@"SELECT parentid as recid, name, value " +
                                                   @"FROM props " +
                                                       @"WHERE parentid = @parent;",
                                               parametrs);

                table.TableName = @"properties";
                result.Merge(table);
            }

            result.TableName = @"properties";

            return result;
        }

        #endregion

        /// <summary>
        /// Найти ближайшего родителя узла соответсвующего типа
        /// </summary>
        /// <param name="unitNode">Узел с которого начинается поиск</param>
        /// <param name="unitTypeId">Искомый тип</param>
        /// <returns></returns>
        public UnitNode GetParentNode(OperationState state, UnitNode unitNode, int unitTypeId)
        {
            UnitNode ret = unitNode;

            while (ret != null && ret.Typ != unitTypeId)
                ret = CheckUnitNode(state, ret.ParentId, Privileges.Read);

            if (ret != null && ret.Typ != unitTypeId)
                return null;
            return ret;
        }

        /// <summary>
        /// Найти узел по ИД среди дочерних узлов
        /// </summary>
        /// <param name="parentNode">Узел в ветке которого ведётся поиск</param>
        /// <param name="unitNodeID">ИД искомого параметра</param>
        /// <returns></returns>
        public UnitNode Find(OperationState state, UnitNode parentNode, int unitNodeID)
        {
            //UnitNode retUnitNode = GetUnitNode(state, unitNodeID);
            UnitNode retUnitNode = ValidateUnitNode(state, unitNodeID, Privileges.Read);
            UnitNode unitNode = retUnitNode;

            while (unitNode != null && unitNode.Idnum != parentNode.Idnum)
                //unitNode = GetUnitNode(state, unitNode.ParentId);
                unitNode = ValidateUnitNode(state, unitNode.ParentId, Privileges.Read);

            if (unitNode != null)
                return retUnitNode;
            return null;
        }

        private void SetFullName(UnitNode unitNode)
        {
            unitNode.FullName = GetFullName(unitNode);

            if (unitNode.Parameters != null)
                foreach (var item in unitNode.Parameters)
                {
                    ParameterNode param = GetUnitNode(item.ParameterId) as ParameterNode;

                    if (param != null)
                    {
                        item.SetFullName(GetFullName(param));
                    }
                }
        }

        public String GetFullName(UnitNode unitNode)
        {
            //OperationState state = new OperationState(SecurityManager.InternalSession);

            const String fullNameSeparator = "/";
            StringBuilder fullNameBuilder = new StringBuilder();
            UnitNode node = unitNode;
            while (node != null)
            {
                fullNameBuilder.Insert(0, node.Text);
                fullNameBuilder.Insert(0, fullNameSeparator);
                node = GetUnitNode(node.ParentId);
            }
            return fullNameBuilder.ToString();
        }

        public string[] GetRequiredArguments(OperationState state, UnitNode unitNode)
        {
            String[] args = null;
            OptimizationGateNode optimizationNode = null;

            //optimizationNode = GetParentNode(unitNode, UnitTypeId.OptimizeCalc) as OptimizationGateNode;
            if (unitNode.ParentId > 0)
                //optimizationNode = GetParentNode(state, GetUnitNode(state, unitNode.ParentId), (int)UnitTypeId.OptimizeCalc) as OptimizationGateNode;
                optimizationNode = CheckUnitNode<OptimizationGateNode>(state, unitNode.ParentId, Privileges.Read);

            if (optimizationNode != null)
            {
                args = GetRequiredArguments(state, optimizationNode);
            }

            if ((optimizationNode = unitNode as OptimizationGateNode) != null)
            {
                var localArgs = from a in optimizationNode.ArgsValues select a.Name;

                List<String> list = new List<String>();
                if (args != null)
                    list.AddRange(args);
                list.AddRange(localArgs);
                args = list.ToArray();
            }
            return args;
        }

        /// <summary>
        /// Запросить количество дочерних элемнтов по типам для узла.
        /// </summary>
        /// <param name="unitNode"></param>
        /// <returns></returns>
        public Dictionary<int, int> GetStatistic(OperationState state, UnitNode unitNode)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            Queue<UnitNode> proccessQueue = new Queue<UnitNode>();

            proccessQueue.Enqueue(unitNode);

            while (proccessQueue.Count > 0)
            {
                UnitNode node = proccessQueue.Dequeue();

                foreach (var childNodeID in node.NodesIds)
                {
                    int count;
                    UnitNode childNode = GetUnitNode(childNodeID);

                    proccessQueue.Enqueue(childNode);

                    if (!result.TryGetValue(childNode.Typ, out count))
                        count = 0;

                    result[childNode.Typ] = ++count;
                }
            }

            return result;
        }


        public IntervalDescription[] GetStandardsIntervals(OperationState state)
        {
            List<IntervalDescription> retList = new List<IntervalDescription>();
            using (var table = dbwork.ExecSQL_toTable("SELECT [id], [standard], [header], [interval] FROM [intervals]", null))
            {
                foreach (DataRow dataRow in table.Rows)
                {
                    retList.Add(new IntervalDescription()
                    {
                        ID = Convert.ToInt32(dataRow[0]),
                        IsStandard = Convert.ToBoolean(dataRow[1]),
                        Header = Convert.ToString(dataRow[2]),
                        interval = Interval.FromString(Convert.ToString(dataRow[3]))
                    });
                }
            }

            return retList.ToArray();
        }


        public void RemoveStandardIntervals(OperationState state, IEnumerable<IntervalDescription> intervalsToRemove)
        {
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM [intervals] WHERE ");
            int length = deleteQuery.Length;

            foreach (var interval in intervalsToRemove)
            {
                if (deleteQuery.Length > length)
                {
                    deleteQuery.Append("OR ");
                }
                deleteQuery.AppendFormat("id = {0}", interval.ID);
            }

            dbwork.ExecSQL(deleteQuery.ToString(), null);
        }

        public void SaveStandardIntervals(OperationState state, IEnumerable<IntervalDescription> modifiedIntervals)
        {
            const String updateQuery = "UPDATE [intervals] SET [standard] = @standard, [header] = @header, [interval] = @interval WHERE [id] = @id";
            const String insertQuery = "INSERT INTO [intervals] ([standard], [header], [interval]) VALUES (@standard, @header, @interval)";

            foreach (var interval in modifiedIntervals)
            {
                DB_Parameters p = new DB_Parameters();

                p.Add("id", DbType.Int32, interval.ID);
                p.Add("standard", DbType.Boolean, interval.IsStandard);
                p.Add("header", DbType.String, interval.Header);
                p.Add("interval", DbType.String, interval.interval.ToString());

                if (interval.ID==0)
                {
                    dbwork.ExecSQL(insertQuery, p);
                }
                else
                {
                    dbwork.ExecSQL(updateQuery, p);
                }
            }
        }

    }
}

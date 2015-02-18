using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using COTES.ISTOK;
using System.Net.Sockets;
using COTES.ISTOK.DiagnosticsInfo;
using System.Runtime.Serialization;

namespace COTES.ISTOK.Assignment.Gdiag
{
    /// <summary>
    /// Класс для диагностики общестанционного сервера
    /// </summary>
    [DataContract]
    [KnownType(typeof(DiagnosticsProxy))]   
    public class GlobalDiagnostics : Diagnostics
    {
    	[DataMember]
    	GlobalDiag gnode {get;set;}
    	
        BlockProxy blockProxy;
        //CalcServer cserv;
        SecurityManager securityManager;
        ParameterRegistrator registrator;     

        internal GlobalDiagnostics(GlobalDiag gnode,
            ParameterRegistrator registrator,
            BlockProxy blockProxy,
            SecurityManager securityManager)
        {
            this.gnode = gnode;
            this.registrator = registrator;
            this.blockProxy = blockProxy;
            //this.cserv = cserv;
            this.securityManager = securityManager;
        }

        public override string Text
        {
            get
            {
                return gnode.Text;
            }
            set
            {
                gnode.Text = value;
            }
        }

        public override string GetSelfProperties()
        {
            return gnode.ToString();// base.GetSelfProperties();
        }

        #region Общая диагностика
        /// <summary>
        /// Настройка собирателей информации
        /// </summary>
        protected override List<ISummaryInfo> SetupInfoGetters()
        {
            List<ISummaryInfo> lstInfoGetters = new List<ISummaryInfo>();
            //if (lstInfoGetters == null)
            //    lstInfoGetters = new List<ISummaryInfo>();
            //else
            //    lstInfoGetters.Clear();
            if (GlobalQueryManager.globSvcManager != null)
                lstInfoGetters.Add(GlobalQueryManager.globSvcManager.GetAsyncOperationManager());
            lstInfoGetters.Add(securityManager);
            lstInfoGetters.Add(registrator);
            lstInfoGetters.Add(new BatteryDiagnostician());
            lstInfoGetters.Add(new SystemDiagnostician());

            return lstInfoGetters;
        }
        #endregion

        public override void Restart(string reason, TimeSpan delay)
        {
            //GlobalQueryManager.globSvcManager.Restart(reason, delay);
        }
        
        #region Block Manage
        public override bool CanManageBlocks()
        {
            return true;
        }

        /// <summary>
        /// Получить список ИД блочных серверов
        /// </summary>
        /// <returns></returns>
        public override int[] GetBlocks()
        {
            List<int> lst = new List<int>();

            foreach (var item in blockProxy.Blocks)
                lst.Add(item.Idnum);
            
            return lst.ToArray();
        }

        public override string GetBlockProperties(int block_id)
        {
            foreach (var item in blockProxy.Blocks)
                if (item.Idnum == block_id) return item.BlockUID;
            
            return null;
        }

        public override string GetBlockDiagnosticsURL(int block_id)
        {
            string urlFormat = "tcp://{0}:{1}/{2}";

            foreach (var item in blockProxy.Blocks)
                if (item.Idnum == block_id)
                    return string.Format(urlFormat,
                        item.Host,
                        item.Port,
                        CommonData.BlockDiagnosticsURI);
            
            return "";
        }

        public override Diagnostics[] GetBlockDiagnostics()
        {
            List<Diagnostics> lst = new List<Diagnostics>();

            foreach (var item in blockProxy.Blocks)
            {
                lst.Add(GetBlockDiagnostics(item.Idnum));
            }

            return lst.ToArray();
        }
        public override Diagnostics GetBlockDiagnostics(int block_id)
        {
            Diagnostics diag = null;

            try
            {
                diag = blockProxy.GetDiagnosticsObject(block_id);
            }
            catch (SocketException) { }
            if (diag != null) diag = new DiagnosticsProxy(diag);

            return diag;
        }
        #endregion

        #region Parameter Transaction Manage
        public override bool CanManageParameterTransaction()
        {
            return true;
        }

        public override int[] GetParametersTransactionsID()
        {
            return registrator.GetTransactionsID();
        }

        public override DataTable GetParameterTransactionInfo()
        {
            DataTable table = null;
            ParametersTransaction transaction = null;
            ParamValueItemWithID[] parameters;
            int[] ids = null;

            ids = registrator.GetTransactionsID();

            if (ids != null && ids.Length > 0)
            {
                table = new DataTable();
                table.Columns.Add("id", typeof(int));
                table.Columns.Add("count", typeof(int));
                table.Columns.Add("LastCallTime", typeof(DateTime)).DateTimeMode = DataSetDateTime.Unspecified;

                for (int i = 0; i < ids.Length; i++)
                {
                    try
                    {
                        transaction = registrator.GetTransaction(ids[i]);
                    }
                    catch { }

                    if (transaction != null)
                    {
                        DataRow row = table.NewRow();

                        row["id"] = transaction.ID;
                        row["LastCallTime"] = transaction.LastCallTime;
                        row["count"] = ((parameters = transaction.GetParameters()) != null ? parameters.Length : 0);

                        table.Rows.Add(row);
                    }
                }
            }
            return table;
        }

        public override DataTable GetParameterTransactionInfo(int transaction_id)
        {
            DataTable table = null;
            ParametersTransaction transaction = null;
            ParamValueItemWithID[] parameters;

            try
            {
                transaction = registrator.GetTransaction(transaction_id);
            }
            catch { }

            if (transaction != null)
            {
                table = new DataTable();
                table.Columns.Add("id", typeof(int));
                table.Columns.Add("count", typeof(int));
                table.Columns.Add("LastCallTime", typeof(DateTime)).DateTimeMode = DataSetDateTime.Unspecified;

                DataRow row = table.NewRow();

                row["id"] = transaction.ID;
                row["LastCallTime"] = transaction.LastCallTime;
                row["count"] = ((parameters = transaction.GetParameters()) != null ? parameters.Length : 0);

                table.Rows.Add(row);
            }
            return table;
        }

        public override ParamValueItemWithID[] GetParameterTransactionValues(int transaction_id)
        {
            ParametersTransaction transaction = null;
            ParamValueItemWithID[] parameters;

            try
            {
                transaction = registrator.GetTransaction(transaction_id);
            }
            catch { }

            parameters = transaction.GetParameters();
            return parameters;
        }
        #endregion

        #region Calc Transaction Manage
        public override bool CanManageCalcTransaction()
        {
            return true;
        }

        public override int[] GetCalcTransactions()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

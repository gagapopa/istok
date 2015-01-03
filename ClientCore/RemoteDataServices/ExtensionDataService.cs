using COTES.ISTOK.ASC;
using COTES.ISTOK.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace COTES.ISTOK.ClientCore
{
    public class ExtensionDataService : AsyncGlobalWorker
    {
        internal ExtensionDataService(Session session)
            : base(session)
        {
            //
        }

        public ExtensionDataInfo[] GetExtensionTableInfo(int unitNodeId)
        {
            string opid = "GetExtensionTableInfo" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetExtensionTableInfoById(session.Uid, unitNodeId);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ExtensionDataInfo[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public ExtensionDataInfo[] GetExtensionTableInfo(string extensionName)
        {
            string opid = "GetExtensionTableInfo" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetExtensionTableInfoByCaption(session.Uid, extensionName);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ExtensionDataInfo[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public ExtensionData GetExtensionExtendedTable(string extensionCaption, string tabKeyword, DateTime dateFrom, DateTime dateTo)
        {
            string opid = "GetExtensionExtendedTable" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetExtensionExtendedTable2(session.Uid, extensionCaption, tabKeyword, dateFrom, dateTo);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ExtensionData>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public ExtensionData GetExtensionExtendedTable(string extensionCaption, string tabKeyword)
        {
            return GetExtensionExtendedTable(extensionCaption, tabKeyword, DateTime.MinValue, DateTime.MaxValue);
        }
        public ExtensionData GetExtensionExtendedTable(int unitNodeId, string tabKeyword)
        {
            return GetExtensionExtendedTable(unitNodeId, tabKeyword, DateTime.MinValue, DateTime.MaxValue);
        }
        public ExtensionData GetExtensionExtendedTable(int unitNodeId, string tabKeyword, DateTime dateFrom, DateTime dateTo)
        {
            string opid = "GetExtensionExtendedTable" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetExtensionExtendedTable(session.Uid, unitNodeId, tabKeyword, dateFrom, dateTo);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ExtensionData>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public bool ExternalCodeSupported(int nodeId)
        {
            string opid = "ExternalCodeSupported" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).ExternalCodeSupported(session.Uid, nodeId);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<bool>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public bool ExternalIDSupported(int nodeId)
        {
            string opid = "ExternalIDSupported" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).ExternalIDSupported(session.Uid, nodeId);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<bool>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public bool ExternalIDCanAdd(int nodeId)
        {
            string opid = "ExternalIDCanAdd" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).ExternalIDCanAdd(session.Uid, nodeId);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<bool>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public EntityStruct[] ExternalIDList(int nodeId)
        {
            string opid = "ExternalIDList" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).ExternalIDList(session.Uid, nodeId);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<EntityStruct[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public ItemProperty[] GetExternalProperties(int nodeId)
        {
            string opid = "GetExternalProperties" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetExternalProperties(session.Uid, nodeId);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ItemProperty[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
    }
}

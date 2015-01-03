using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace COTES.ISTOK.ClientCore
{
    public class BlockDataService : AsyncGlobalWorker
    {
        internal BlockDataService(Session session)
            : base(session)
        {
            //
        }

        public ItemProperty[] GetModuleLibraryProperties(int blockId, string libName)
        {
            string opid = "GetModuleLibraryProperties" + Guid.NewGuid().ToString();
            try
            {
                var res= AllocQManager(opid).GetModuleLibraryProperties(session.Uid, blockId, libName);
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
        public ModuleInfo[] GetModuleLibNames(int blockId)
        {
            string opid = "GetModuleLibNames" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetModuleLibNames(session.Uid, blockId);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ModuleInfo[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public ParameterItem[] GetChannelParameters(int channelNodeId)
        {
            string opid = "GetChannelParameters" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).BeginGetChannelParameters(session.Uid, channelNodeId);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ParameterItem[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public string[] GetBlockUIDs()
        {
            string opid = "GetBlockUIDs" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetBlockUIDs(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<string[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public ulong SendSprav(int nodeId)
        {
            string opid = "SendSprav" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).SendDataToBlockServer(session.Uid, nodeId);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ulong>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
    }
}

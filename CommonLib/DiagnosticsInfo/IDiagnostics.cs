using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;

namespace COTES.ISTOK.DiagnosticsInfo
{
    [ServiceContract]
    public interface IDiagnostics : ITestConnection<Object>
    {
        [OperationContract]
        bool CanManageBlocks();
        [OperationContract]
        bool CanManageBuffer();
        [OperationContract]
        bool CanManageCalcTransaction();
        [OperationContract]
        bool CanManageChannels();
        [OperationContract]
        bool CanManageParameterTransaction();
        [OperationContract]
        void FlushBuffer();
        //[OperationContract]
        //Diagnostics[] GetBlockDiagnostics();
        [OperationContract]
        Diagnostics GetBlockDiagnostics(int block_id);
        [OperationContract]
        String GetBlockDiagnosticsURL(int block_id);
        [OperationContract]
        String GetBlockProperties(int block_id);
        [OperationContract]
        int[] GetBlocks();
        [OperationContract]
        List<ParamValueItemWithID> GetBufferValues(ChannelInfo channelInfo);
        [OperationContract]
        int[] GetCalcTransactions();
        [OperationContract]
        DataTable GetChannelInfo();
        [OperationContract]
        ChannelInfo[] GetChannels();
        [OperationContract]
        ChannelStatus GetChannelState(ChannelInfo channelInfo);
        [OperationContract]
        int[] GetParametersTransactionsID();
        [OperationContract]
        DataTable GetParameterTransactionInfo();
        //[OperationContract]
        //DataTable GetParameterTransactionInfo(int transaction_id);
        [OperationContract]
        ParamValueItemWithID[] GetParameterTransactionValues(int transaction_id);
        [OperationContract]
        DataTable GetRedundancyInfo();
        [OperationContract]
        String GetSelfProperties();
        [OperationContract]
        bool IsRedundancySupported();
        [OperationContract]
        void Restart(String reason, TimeSpan delay);
        [OperationContract]
        void StartChannel(ChannelInfo channelInfo);
        [OperationContract]
        void StopChannel(ChannelInfo channelInfo);
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace COTES.ISTOK.DiagnosticsInfo
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	[ServiceKnownType("GetFiagTypes",typeof(HelperForDiagnosticType))]	
	//[ServiceKnownType(typeof(Diagnostics))]
    public interface IDiagnostics : ITestConnection<Object>
    {
    	[OperationContract]
    	string GetText();
    	[OperationContract]
    	void SetText(string _text);
    	[OperationContract]
    	DataSet GetAllInfo();
    	[OperationContract]
		Diagnostics GetDiagnosticsObject(Guid uid);
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
        IDiagnostics GetBlockDiagnostics(int block_id);
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

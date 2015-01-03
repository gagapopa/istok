using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ServiceModel;

namespace COTES.ISTOK
{
    [ServiceContract]
    public interface IGlobal: ITestConnection<String>
    {
        [OperationContract]
        bool AttachBlock(string uid, IDictionary connAttributes);
        //void DataRequest(string uid, int schedule_id);
        //void DataSend(string uid, Package[] packages);
    }
}

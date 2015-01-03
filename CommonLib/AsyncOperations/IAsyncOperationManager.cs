using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    public interface IAsyncOperationManager
    {
        bool IsComplete(ulong id);
        bool IsInterrupted(ulong id);
        UAsyncResult GetResult(ulong id);
        UAsyncResult GetMessages(ulong id);
        double GetProgress(ulong id);
        string GetStatus(ulong id);
        void End(ulong id);
        bool Abort(ulong id);
    }
}

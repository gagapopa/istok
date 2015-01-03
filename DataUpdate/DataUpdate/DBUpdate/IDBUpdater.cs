using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOKDataUpdate
{
    interface IDBUpdater
    {
        Guid UpdaterID { get; }

        int DBVersionFrom { get; }

        int DBVersionTo { get; }

        IEnumerable<Guid> RequiredBefore { get; }

        //IEnumerable<Guid> RequiredAfter { get; }

        bool Update(CurrentService service);
    }
}

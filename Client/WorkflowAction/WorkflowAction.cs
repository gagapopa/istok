using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    abstract class WorkflowAction
    {
        public Session Session { get; private set; }

        public WorkflowAction(Session session)
        {
            this.Session = session;
        }

        public abstract Task<ActionArgs> Do(ActionArgs args);
    }
}

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
   class WorkflowSelector
    {
        Dictionary<String, WorkflowAction> actionDictionary = new Dictionary<string, WorkflowAction>();
        private Session session;

        public WorkflowSelector(Session session)
        {
            this.session = session;

            actionDictionary["SaveWorkflowAction"] = new SaveNodeWorkflowAction(session);
            actionDictionary["ChangeParameterCodeWorkflowAction"] = new ChangeParameterCodeWorkflowAction(session);
            actionDictionary["LockExceptionWorkflowAction"] = new LockExceptionWorkflowAction(session);
        }

        public WorkflowAction GetWorkflow(ActionArgs args)
        {
            WorkflowAction action;

            actionDictionary.TryGetValue(args.ActionName, out action);

            return action;
        }

        public Task<ActionArgs> Do(ActionArgs args)
        {
            var action = GetWorkflow(args);
            return action.Do(args);
        }
    }
}

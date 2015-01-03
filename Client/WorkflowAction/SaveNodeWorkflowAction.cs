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
    class SaveNodeWorkflowAction : WorkflowAction
    {
        public SaveNodeWorkflowAction(Session session)
            : base(session)
        {

        }

        public override Task<ActionArgs> Do(ActionArgs args)
        {
            return Task.Factory.StartNew<ActionArgs>(() =>
            {
                SaveActionArgs saveArgs = (SaveActionArgs)args;

                String confirmMessage = saveArgs.UnitProvider.GetConfirmMessage();

                if (!String.IsNullOrEmpty(confirmMessage))
                {
                    MessageBoxButtons buttons = saveArgs.Cancelable ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo;

                    var result = MessageBox.Show(confirmMessage, "Подтверждение", buttons, MessageBoxIcon.Question);

                    // TODO Реализовать Cancelable
                    if (result == DialogResult.Cancel)
                    {
                        saveArgs.Cancel = true;
                    }
                    if (result != DialogResult.Yes)
                    {
                        return saveArgs;
                    }
                }

                IEnumerable<ActionArgs> actions;
                try
                {
                    actions = saveArgs.UnitProvider.Save();
                }
                catch (LockException exc)
                {
                    MessageBox.Show(exc.Message);
                    actions = null;
                }
                catch (Exception exc)
                {
                    Program.MainForm.ShowError(exc);
                    saveArgs.Cancel = true;
                    return saveArgs;
                }

                if (actions != null)
                {
                    foreach (var actionArgs in actions)
                    {
                        var action = Program.MainForm.WorkflowSelector.GetWorkflow(actionArgs);

                        if (action != null)
                        {
                            action.Do(actionArgs).Wait();
                        }
                    }
                }
                return saveArgs;
            });
        }
    }
}

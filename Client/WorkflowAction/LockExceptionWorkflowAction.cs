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
    class LockExceptionWorkflowAction : WorkflowAction
    {
        public LockExceptionWorkflowAction(Session session)
            : base(session)
        {

        }

        public override Task<ActionArgs> Do(ActionArgs args)
        {
            return Task.Factory.StartNew<ActionArgs>(() =>
            {
                bool done = false;
                LockActionArgs actionArgs = (LockActionArgs)args;

                while (!done)
                {
                    try
                    {
                        actionArgs.LockableAction();
                        done = true;
                        actionArgs.Success = true;
                    }
                    catch (LockException exc)
                    {
                        if (exc.Causes.Count() > 1)
                        {

                            Program.MainForm.Invoke((Action)(() =>
                                MessageBox.Show(
                                    exc.Message,
                                    "Значения некоторых параметров редактируются другими пользователями",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Asterisk)));
                            done = true;
                        }
                        else
                        {
                            var cause = exc.Causes.First();

                            if (cause.Node == null) throw new ArgumentNullException("exc.UnitNode");

                            bool canReset = actionArgs.RepeatOnRelease
                                && (Session.User.IsAdmin
                                || cause.UserNames.Contains<String>(Session.User.Text));

                            DialogResult result = (DialogResult)Program.MainForm.Invoke(
                                (Func<DialogResult>)(() => MessageBox.Show(
                                       Program.MainForm,
                                       exc.Message +
                                       (canReset ? "\nСбросить?" : ""),
                                       "Узел редактируется другим пользователем",
                                       canReset ? MessageBoxButtons.YesNo : MessageBoxButtons.OK,
                                       canReset ? MessageBoxIcon.Question : MessageBoxIcon.Asterisk)));

                            if (result == DialogResult.Yes)
                            {
                                Session.StructureProvider.ReleaseAll(cause.Node);
                            }
                            else
                            {
                                done = true;
                            }
                        }
                    }
                    catch (UserNotConnectedException) { throw; }
                    catch (Exception exc)
                    {
                        Program.MainForm.Invoke((Action)(() => MessageBox.Show(Program.MainForm,
                                                                               exc.Message,
                                                                               "Запрет на редактирование",
                                                                               MessageBoxButtons.OK,
                                                                               MessageBoxIcon.Asterisk)));
                        done = true;
                    }
                    done = done || actionArgs.OneShoot;
                }
                return actionArgs;
            });
        }
    }
}

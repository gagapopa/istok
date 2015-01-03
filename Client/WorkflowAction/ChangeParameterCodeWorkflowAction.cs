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
    class ChangeParameterCodeWorkflowAction : WorkflowAction
    {
        public ChangeParameterCodeWorkflowAction(Session session)
            : base(session)
        {

        }

        public override async Task<ActionArgs> Do(ActionArgs args)
        {
            ChangeParameterCodeActionArgs actionArgs = (ChangeParameterCodeActionArgs)args;

            DialogResult result = MessageBox.Show("Параметр ипользовался в расчете. Изменить код в формулах на новый?",
                "Изменение кода параметра", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                TepForm tepForm = null;
                Program.MainForm.Invoke((Action)(() =>
                {
                    tepForm = new TepForm(Session.StructureProvider, Program.MainForm.Icons);
                    Program.MainForm.AddExtendForm(tepForm);
                    tepForm.Relation = FormulaRelation.Reference;
                    tepForm.Node = actionArgs.OldNode;
                    tepForm.OnSave += async (s, e) =>
                        {
                            await Task.Factory.StartNew(() => Session.StructureProvider.ChangeParameterCode(actionArgs.OldNode, actionArgs.NewNode, tepForm.NodesToSave));
                        };
                    tepForm.TopMost = true;
                    tepForm.ShowDialog();
                }));                
            }
            return actionArgs;
        }
    }
}

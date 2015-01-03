using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.ClientCore
{
    public class ChangeParameterCodeActionArgs : ActionArgs
    {
        public UnitNode OldNode { get; private set; }

        public UnitNode NewNode { get; private set; }

        public ChangeParameterCodeActionArgs(UnitNode oldNode, UnitNode newNode)
            : base("ChangeParameterCodeWorkflowAction")
        {
            this.OldNode = oldNode;
            this.NewNode = newNode;
        }
    }
}

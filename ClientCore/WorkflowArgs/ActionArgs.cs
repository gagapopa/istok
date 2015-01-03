using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.ClientCore
{
    public class ActionArgs
    {
        public String ActionName { get; private set; }

        protected ActionArgs(String actionName)
        {
            this.ActionName = actionName;
        }
    }
}

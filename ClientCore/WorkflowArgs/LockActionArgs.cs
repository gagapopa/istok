using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.ClientCore
{
    public class LockActionArgs : ActionArgs
    {
        public Action LockableAction { get; private set; }

        public bool RepeatOnRelease { get; private set; }

        public bool OneShoot { get; set; }

        public LockActionArgs(Action lockableAction, bool repeatOnRelease)
            : base("LockExceptionWorkflowAction")
        {
            this.LockableAction = lockableAction;
            this.RepeatOnRelease = repeatOnRelease;
        }

        public bool Success { get; set; }
    }
}

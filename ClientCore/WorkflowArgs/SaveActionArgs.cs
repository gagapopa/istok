using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore.UnitProviders;

namespace COTES.ISTOK.ClientCore
{
    public class SaveActionArgs : ActionArgs
    {
        public UnitProvider UnitProvider { get; private set; }

        public bool Cancelable { get; private set; }

        public bool Cancel { get; set; }

        public SaveActionArgs(UnitProvider unitProvider, bool calcelable)
            : base("SaveWorkflowAction")
        {
            this.UnitProvider = unitProvider;
            this.Cancelable = calcelable;
        }
    }
}

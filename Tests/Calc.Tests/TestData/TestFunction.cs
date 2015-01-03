using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.Tests.Calc
{
    class TestFunction : Function
    {
        private RevisionInfo revision;

        Func<double[], double> action;

        public TestFunction(String name, RevisionInfo revision, CalcArgumentInfo[] args)
            :this(name, revision, args, null)
        {
        }

        public TestFunction(String name, RevisionInfo revision, CalcArgumentInfo[] args, Func<double[], double> action)
            : base(name, args, "", "")
        {
            this.revision = revision;
            this.action = action;
        }

        public override RevisionInfo Revision
        {
            get
            {
                return revision;
            }
        }

        public override void Subroutine(ICalcContext context)
        {
            Variable retVar = context.SymbolTable.GetSymbol("@ret") as Variable;

            if (retVar != null && action != null)
            {
                Object[] objectArgs = LoadArguments(context);

                if (objectArgs != null)
                {
                    double[] args = (from o in objectArgs select (double)o).ToArray();
                    double ret = action(args);
                    retVar.Value = SymbolValue.CreateValue(ret);
                }
                else
                    retVar.Value = SymbolValue.Nothing;
            }
        }

        public object[] TestLoadArguments(ICalcContext calcContext)
        {
            return LoadArguments(calcContext);
        }
    }
}

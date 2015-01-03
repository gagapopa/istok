using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC.Audit
{
    partial class AuditCalcStart 
    {
        public override string ToString()
        {
            return String.Format("Запуск расчёта {0} параметров за период от {1} до {2}.{3}",
                AuditCalcNodes.Count,
                CalcStart, 
                CalcEnd, 
                (CalcRecalc ? " С перерасчётом зависимостей" : ""));
        }
    }
}

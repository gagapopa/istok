using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC.Audit
{
    partial class AuditEntry
    {
        public AuditEntry(COTES.ISTOK.ASC.UserNode userNode)
            : this()
        {
            Time = DateTime.Now;
            UserLogin = userNode.Text;
            UserFullName = userNode.UserFullName;
            UserPosition = userNode.Position;
            UserRole = userNode.Roles;
        }

        public override string ToString()
        {
            StringBuilder stringbuilder = new StringBuilder();

            stringbuilder.AppendFormat("{{{0} [{1}] ", Time, UserLogin);
            int length = stringbuilder.Length;

            foreach (var tuple in new Tuple<String, int>[] 
            {
                Tuple.Create("types", AuditTypes.Count),
                Tuple.Create("users", AuditUsers.Count),
                Tuple.Create("groups", AuditGroups.Count),
                Tuple.Create("units", AuditUnits.Count),
                Tuple.Create("props", AuditProps.Count),
                Tuple.Create("lobs", AuditLobs.Count),
                Tuple.Create("values", AuditValues.Count),
                Tuple.Create("calcs", AuditCalcStarts.Count),
            })
            {
                if (tuple.Item2 > 0)
                {
                    if (stringbuilder.Length > length)
                    {
                        stringbuilder.Append(", ");
                    }
                    stringbuilder.AppendFormat("{0}: {1}", tuple.Item1, tuple.Item2);
                }
            }
            stringbuilder.Append("}");

            return stringbuilder.ToString();
        }

        public bool IsEmpty
        {
            get
            {
                return AuditUnits.Count == 0
                    && AuditProps.Count == 0
                    && AuditLobs.Count == 0
                    && AuditValues.Count == 0
                    && AuditCalcStarts.Count == 0
                    && AuditTypes.Count == 0
                    && AuditUsers.Count == 0
                    && AuditGroups.Count == 0;
            }
        }
    }
}

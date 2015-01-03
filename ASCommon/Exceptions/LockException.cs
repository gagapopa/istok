using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class LockCause
    {
        public UnitNode Node { get; private set; }

        public Privileges Privileges { get; private set; }

        public String[] UserNames { get; private set; }

        public LockCause(UnitNode node, Privileges privileges, params String[] userNames)
        {
            this.Node = node;
            this.Privileges = privileges;
            this.UserNames = userNames;
        }
    }

    [Serializable]
    public class LockException : ISTOKException
    {
        private List<LockCause> causes;

        public IEnumerable<LockCause> Causes
        {
            get
            {
                return causes.AsReadOnly();
            }
        }

        public LockException(UnitNode unitNode, Privileges priv, params String[] userNames)
            : this(new LockCause(unitNode, priv, userNames))
        { }

        public LockException(params LockCause[] locks)
            : base()
        {
            this.causes = new List<LockCause>(locks);
        }

        public LockException(LockExceptionFault fault)
            : this(fault.Causes.ToArray())
        { }
        
        protected LockException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }

        public override string Message
        {
            get
            {
                StringBuilder messageBuilder = new StringBuilder();
                if (Causes.Count()>1)
                {
                    messageBuilder.Append("Следующие параметры заблокированы для редактирования значений:\n");
                    foreach (var lockCause in Causes)
                    {
                        messageBuilder.AppendFormat("\nПараметр '{0}' ", lockCause.Node.Text);
                        if (lockCause.UserNames.Length > 1)
                        {
                            messageBuilder.Append("пользователями ");
                            for (int i = 0; i < lockCause.UserNames.Length; i++)
                            {
                                if (i > 0) messageBuilder.Append(", ");
                                messageBuilder.Append(lockCause.UserNames[i]);
                            }
                        }
                    }
                }
                else
                {
                    var cause = Causes.First();
                    messageBuilder.AppendFormat("Узел '{0}' заблокирован", cause.Node.Text);

                    if (cause.UserNames.Length > 1)
                    {
                        messageBuilder.Append(" пользователями ");
                    }
                    else
                    {
                        messageBuilder.Append(" пользователем ");
                    }
                    for (int i = 0; i < cause.UserNames.Length; i++)
                    {
                        if (i > 0) messageBuilder.Append(", ");
                        messageBuilder.Append(cause.UserNames[i]);
                    }
                }

                return messageBuilder.ToString();
            }
        }
    }

    [DataContract]
    public class LockExceptionFault
    {
        [DataMember]
        private List<LockCause> causes;

        public IEnumerable<LockCause> Causes
        {
            get
            {
                return causes.AsReadOnly();
            }
        }

        public LockExceptionFault(LockException ex)
        {
            causes = new List<LockCause>(ex.Causes);
        }
    }
}
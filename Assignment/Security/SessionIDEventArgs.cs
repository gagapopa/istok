using System;

namespace COTES.ISTOK.Assignment
{
    public class SessionIDEventArgs : EventArgs
    {
        public Guid SessionGUID { get; private set; }

        public SessionIDEventArgs(Guid sessionGUID)
        {
            this.SessionGUID = sessionGUID;
        }
    }
 }

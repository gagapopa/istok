using System;
using System.Collections.Generic;
using System.Linq;

namespace COTES.ISTOK.Assignment
{
    [Serializable]
    public class UnloadParamsSchedule
    {
        public string Name { set; get; }
        public int ID { set; get; }
        public TimeSpan Period { set; get; }

        public UnloadParamsSchedule()
        { }
    }
}

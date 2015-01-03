using System;
using System.Data;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class ManualParameterNode : ParameterNode
    {
        public ManualParameterNode()
            : base() { }
        public ManualParameterNode(int id)
            : base(id) { }
        public ManualParameterNode(DataRow row)
            : base(row)
        { }
    }
}

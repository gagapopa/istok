using System;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Аргумент для событий передающих UnitNode
    /// </summary>
    [Serializable]
    public class UnitNodeEventArgs : EventArgs
    {
        public int UnitNodeID { get; private set; }

        public UnitNode UnitNode { get; protected set; }

        public UnitNodeEventArgs(int unitNodeID)
        {
            this.UnitNodeID = unitNodeID;
        }

        public UnitNodeEventArgs(UnitNode unitNode)
        {
            this.UnitNode = unitNode;
            if (unitNode != null)
                this.UnitNodeID = unitNode.Idnum;
        }
    }

    [Serializable]
    public class UnitTypeEventArgs:EventArgs
    {
        public UTypeNode UnitType { get; private set; }

        public UnitTypeId UnitTypeID { get; private set; }

        public UnitTypeEventArgs(UnitTypeId unitType)
        {
            this.UnitTypeID = unitType;
        }

        public UnitTypeEventArgs(UTypeNode typeNode)
        {
            this.UnitType = typeNode;
            if (typeNode!=null)
                this.UnitTypeID = (UnitTypeId)typeNode.Idnum;
        }
    }
}

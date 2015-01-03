using System;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public struct EntityStruct
    {
        public int ID { get; set; }

        public String Code { get; set; }

        public String Name { get; set; }

        public EntityStruct(int id, String name)
            : this()
        {
            this.ID = id;
            this.Name = name;
        }
    } 
}

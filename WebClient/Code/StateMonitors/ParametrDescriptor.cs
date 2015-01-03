using System;
using System.Drawing;

namespace WebClient
{
    public class ParameterDescriptor
    {
        public int ID { get; protected set; }
        public int ParametrID { get; protected set; }
        public string Name { get; protected set; }


        public ParameterDescriptor(int id,
                                   int parametr_id,
                                   string name)
        {
            ID = id;
            ParametrID = parametr_id;
            Name = name;
        }
    }
}

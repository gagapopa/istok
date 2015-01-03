using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    public interface IManualSerializable
    {
        void FromBytes(byte[] bytes);
        byte[] ToBytes();
    }
}

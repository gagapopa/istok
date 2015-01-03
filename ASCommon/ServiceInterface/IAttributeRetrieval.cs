using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace COTES.ISTOK.ASC
{
    public interface IAttributeRetrieval
    {
        ItemProperty[] GetProperties(UnitNode node);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using COTES.ISTOK;

namespace COTES.ISTOK.ASC
{
    public interface IChannelRetrieval
    {
        IEnumerable<ItemProperty> GetProperties(BlockNode blockNode, string libName);

        BlockNode GetBlockNode(ChannelNode channel);
    }
}

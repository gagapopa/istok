using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace WebClient
{
    public class MnemoschemesDeskriptor : 
        BaseStateMonitor<MnemoschemeParameterDescriptor>, IEnumerable<MnemoschemeParameterDescriptor>
    {
        public string ContentLocation { get; private set; }
        public Size ContentSize { get; private set; }

        public MnemoschemesDeskriptor(int id,
                                      string name,
                                      int transaction_id,
                                      string content_location,
                                      Size content_size)
            :base(id, name, transaction_id)
        {
            ContentLocation = content_location;
            ContentSize = content_size;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<MnemoschemeParameterDescriptor> GetEnumerator()
        {
            MonitorEnumerator<MnemoschemeParameterDescriptor> result =
                new MonitorEnumerator<MnemoschemeParameterDescriptor>();

            foreach (var it in parametrs.Values)
                result.AddContent(it);

            return result;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace WebClient
{
    public class MnemoschemesDescriptor : 
        BaseStateMonitor<MnemoschemeParameterDescriptor>, IEnumerable<MnemoschemeParameterDescriptor>
    {
        public Guid Content { get; private set; }
        public Size ContentSize { get; private set; }

        public MnemoschemesDescriptor(int id,
                                      string name,
                                      int transaction_id,
                                      Guid content,
                                      Size content_size)
            :base(id, name, transaction_id)
        {
            Content = content;
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
            {
                //if (it != null)
                //{
                //    interval = (int)WebRemoteDataService.GetParameterInterval(it).ToDouble();
                //    param.Attributes[CommonData.IntervalProperty] = interval.ToString();
                //}

                result.AddContent(it);
            }

            return result;
        }
    }
}

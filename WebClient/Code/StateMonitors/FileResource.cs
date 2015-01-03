using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;

namespace WebClient
{
    public class FileResource
    {
        public Guid ID { get; private set; }
        public string Link { get; set; }
        public bool IsLoad { get; set; }
        public int ClientCount { get; set; }

        public FileResource()
        {
            ID = new Guid();
        }

        public FileResource(Guid id)
        {
            ID = id;
        }

        public static Guid GetGuid(params int[] a)
        {
            //need rewrite on shift bit
            const int guid_lenght_byte = 16;
            const int guid_lenght_int = 4;

            using (MemoryStream stream = new MemoryStream(guid_lenght_byte))
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    int counter = 0;
                    foreach (var it in a)
                    {
                        writer.Write(it);
                        if (++counter >= guid_lenght_int) break;
                    }

                    while (counter++ < guid_lenght_int)
                        writer.Write(0);

                    return new Guid(stream.ToArray());
                }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using COTES.ISTOK.ASC;
using System.IO;
using System.Drawing;

namespace COTES.ISTOK.ClientCore
{
    [Serializable]
    public class Cell : IManualSerializable
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public int ParameterId { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public ContentAlignment TextAlign { get; set; }

        public Cell()
        {
            RowIndex = 0;
            ColumnIndex = 0;
            ParameterId = 0;
            Text = "";
            Link = "";
            TextAlign = ContentAlignment.MiddleLeft;
        }
        public Cell(int row, int col)
            : this()
        {
            RowIndex = row;
            ColumnIndex = col;
        }

        #region IManualSerializable Members
        
        public byte[] ToBytes()
        {
            byte[] res;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter sw = new BinaryWriter(stream))
                {
                    sw.Write(RowIndex);
                    sw.Write(ColumnIndex);
                    sw.Write(ParameterId);
                    sw.Write(Text);
                    sw.Write(Link);
                    sw.Write((int)TextAlign);
                }
                res = stream.ToArray();
            }
            return res;
        }

        public void FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    RowIndex = br.ReadInt32();
                    ColumnIndex = br.ReadInt32();
                    ParameterId = br.ReadInt32();
                    Text = br.ReadString();
                    Link = br.ReadString();
                    TextAlign = (ContentAlignment)br.ReadInt32();
                }
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ClientCore.Utils
{
    public class GraphData
    {
        List<LineInfo> lstLineInfo = new List<LineInfo>();
        List<LineData> lstLineData = new List<LineData>();

        public LineInfo[] LineInfo { get { return lstLineInfo.ToArray(); } }
        public LineData[] LineData { get { return lstLineData.ToArray(); } }

        public void AddLine(LineInfo lineInfo, LineData lineData)
        {
            if (lineInfo == null) throw new ArgumentNullException("lineInfo");
            if (lineData == null) throw new ArgumentNullException("lineData");

            lstLineInfo.Add(lineInfo);
            lstLineData.Add(lineData);
        }
    }

    public class LineInfo
    {
        public string Caption { get; set; }
        public Color Color { get; set; }
        public int Marker { get; set; }
        public bool IsVisible { get; set; }
        public int ParameterId { get; set; }
    }
    public class LineData
    {
        List<LineValuePair> lstValues;

        public IEnumerable<LineValuePair> Values
        {
            get
            {
                return lstValues;
            }
        }

        public LineData()
        {
            lstValues = new List<LineValuePair>();
        }

        public void Add(DateTime x, double y)
        {
            lstValues.Add(new LineValuePair(x, y));
        }
        public void Add(LineValuePair value)
        {
            lstValues.Add(value);
        }
    }

    public class LineValuePair
    {
        public DateTime X { get; private set; }
        public double Y { get; private set; }

        public LineValuePair()
        {
            X = DateTime.MinValue;
            Y = 0f;
        }
        public LineValuePair(DateTime x, double y)
        {
            X = x;
            Y = y;
        }
    }
}

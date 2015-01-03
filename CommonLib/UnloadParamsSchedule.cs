using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace COTES.ISTOK
{
    //[Serializable]
    //public class UnloadParamsSchedule
    //{
    //    public string Name { set; get; }
    //    public int ID { set; get; }
    //    public TimeSpan Period { set; get; }

    //    public UnloadParamsSchedule()
    //    { }

    //    public override string ToString()
    //    {
    //        return String.Format(@"[{0}] {1} - {2}",
    //                             ID,
    //                             Name,
    //                             Period);
    //    }

    //    public static UnloadParamsSchedule FromString(string source)
    //    {
    //        Regex re = new Regex(@"\[(?<ID>\d+)\] (?<Name>.+?) - (?<Period>.+)");

    //        UnloadParamsSchedule result = new UnloadParamsSchedule();

    //        Match match = re.Match(source);
    //        if (match.Success)
    //        {
    //            int temp_id = 0;
    //            int.TryParse(match.Groups["ID"].Value, out temp_id);
    //            result.ID = temp_id;
    //            result.Name = match.Groups["Name"].Value;
    //            TimeSpan temp_period = TimeSpan.FromTicks(0);
    //            TimeSpan.TryParse(match.Groups["Period"].Value, out temp_period);
    //            result.Period = temp_period;
    //        }

    //        return result;
    //    }
    //}
}

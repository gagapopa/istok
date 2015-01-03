using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ClientCore.Utils
{
    public class DoubleControlSettings
    {
        //protected static DoubleControlSettings instance;

        //public static DoubleControlSettings Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //            instance = new DoubleControlSettings();
        //        return instance;
        //    }
        //}

        /// <summary>
        /// Количество отображаемых на форме знаков после запятой
        /// </summary>
        public int RoundDigit { get; set; }

        public static String DoubleFormat(int roundDigit)
        {
            return "0." + new String('0', roundDigit);
        }

        public String DoubleFormat()
        {
            return DoubleFormat(RoundDigit);
        }

        public String DoubleFormat(UnitNode unitNode)
        {
            int roundDigit;
            ParameterNode parameterNode;

            if ((parameterNode = unitNode as ParameterNode) != null
                && parameterNode.RoundCount != null
                && parameterNode.RoundCount >= 0)
                roundDigit = (int)parameterNode.RoundCount;
            else roundDigit = RoundDigit;

            return "0." + new String('0', roundDigit);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using COTES.ISTOK;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Интерфейс для получения информации о типе узла
    /// </summary>
    public interface IUnitTypeRetrieval
    {
        int[] GetUnitTypes(int parentNodeID);
        Dictionary<int, String> GetUnitTypeLocalization(System.Globalization.CultureInfo culture);

        UTypeNode GetUnitTypeNode(int unitType);
    }
}

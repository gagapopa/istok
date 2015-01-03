using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Параметр расчёта. Содержит информацию о параметрах для разных ревизий
    /// </summary>
    public interface ICalcNode
    {
        /// <summary>
        /// ИД параметра
        /// </summary>
        int NodeID { get; }

        /// <summary>
        /// Имя параметра
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Хранилище разных ревизий параметров
        /// </summary>
        RevisedStorage<ICalcNodeInfo> Revisions { get; }
    }
}


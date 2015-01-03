using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Интерфейс для получения информации о структуре
    /// </summary>
    public interface IStructureRetrieval
    {
        /// <summary>
        /// Получить UnitNode по ИД
        /// </summary>
        /// <param name="unitNodeID">ИД узла</param>
        /// <returns></returns>
        UnitNode GetUnitNode(int unitNodeID);

        /// <summary>
        /// Запросить все узлы по фильтру типов
        /// </summary>
        /// <param name="parentNode">Родительский узел</param>
        /// <param name="typeFilter">Типы</param>
        /// <returns></returns>
        UnitNode[] GetChildNodes(int parentId, int[] typeFilter, int minLevel, int maxLevel);
    }
}

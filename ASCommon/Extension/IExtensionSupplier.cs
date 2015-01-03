using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Extension
{
    /// <summary>
    /// Обратная связь от расширения к станционному серверу
    /// </summary>
    public interface IExtensionSupplier
    {
        /// <summary>
        /// Запросить ближайший родительский узел с указанным типом
        /// </summary>
        /// <param name="unitNode">Начальный узел</param>
        /// <param name="parentTypeGUID">GUID искомого типа</param>
        /// <returns>null, если поиск завершился не удачей</returns>
        ExtensionUnitNode GetParent(ExtensionUnitNode unitNode, Guid parentTypeGUID);

        /// <summary>
        /// Запросить более подробную информацию по типу
        /// </summary>
        /// <param name="unitTypeId"></param>
        /// <returns></returns>
        UTypeNode GetTypeNode(int unitTypeId);
    }
}

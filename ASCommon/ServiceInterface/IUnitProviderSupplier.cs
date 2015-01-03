using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUnitProviderSupplier
    {
        /// <summary>
        /// Получить UnitProvider для указанного узла.
        /// </summary>
        /// <param name="unitNode"></param>
        /// <returns></returns>
        IUnitNodeProvider GetProvider(UnitNode unitNode);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IUnitNodeProvider
    {
        /// <summary>
        /// Узел находиться в режиме только для чтения
        /// </summary>
        bool ReadOnly { get; }

        /// <summary>
        /// Текущая просматриваемая/редактируемая ревизия.
        /// </summary>
        RevisionInfo CurrentRevision { get; /*set;*/ }

        /// <summary>
        /// Получить действительную ревизию.
        /// Метод должен производить замену ревизий Current и Head на конкретные соответствующие им ревизии.
        /// </summary>
        /// <param name="revision">Требуемая ревизия</param>
        /// <returns>
        /// Ревизия после подстановки стандартных ревизий.
        /// </returns>
        RevisionInfo GetRealRevision(RevisionInfo revision);

    }
}

using System;
using System.Linq;
using System.Collections.Generic;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Хранилище состяний элементов расчёта
    /// </summary>
    public class CalcStateStorage : ICalcStateStorage
    {
        /// <summary>
        /// Кэш готовых состояний элементов
        /// </summary>
        Dictionary<RevisionInfo, Dictionary<ICalcNodeInfo, ICalcState>> cachedNodes;

        public CalcStateStorage()
            : this(null)
        { }

        private CalcStateStorage(CalcStateStorage baseStorage)
        {
            this.cachedNodes = new Dictionary<RevisionInfo, Dictionary<ICalcNodeInfo, ICalcState>>();
        }

        public ICalcState GetState(ICalcNode calcNode, RevisionInfo revision)
        {
            Dictionary<ICalcNodeInfo, ICalcState> revisionCache;
            ICalcState state;
            ICalcNodeInfo nodeInfo = calcNode.Revisions.Get(revision);

            if (!cachedNodes.TryGetValue(revision, out revisionCache))
                cachedNodes[revision] = revisionCache = new Dictionary<ICalcNodeInfo, ICalcState>();

            if (!revisionCache.TryGetValue(nodeInfo, out state))
            {
                state = CreateNodeState(nodeInfo, revision);
                if (state != null)
                    revisionCache[nodeInfo] = state;
            }

            return state;
        }

        /// <summary>
        /// Создать состояние элемента расчёта
        /// </summary>
        /// <param name="nodeInfo"></param>
        /// <returns></returns>
        private CalcState CreateNodeState(ICalcNodeInfo nodeInfo, RevisionInfo revision)
        {
            IParameterInfo parameterInfo;
            IOptimizationInfo optimizationInfo;

            if ((parameterInfo = nodeInfo as IParameterInfo) != null)
            {
                return new NodeState(parameterInfo, revision);
            }
            if ((optimizationInfo = nodeInfo as IOptimizationInfo) != null)
            {
                return new OptimizationState(this, optimizationInfo, revision);
            }
            return null;
        }

        public void Clear()
        {
            cachedNodes.Clear();
        }
    }
}

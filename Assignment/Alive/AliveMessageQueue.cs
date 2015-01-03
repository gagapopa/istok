using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Класс для накопления изменений данных на сервере с последнего обращения пользователя.
    /// При добовлении изменений, пытается их агрегировать. При запросе, возвращает все имеющиеся и очищает коллекцию.
    /// </summary>
    public class ServiceDataChangeQueue
    {
        List<ServiceDataChange> changes = new List<ServiceDataChange>();

        public void Enque(ServiceDataChange dataChange)
        {
            bool merged = false;

            lock (changes)
            {
                foreach (var item in changes)
                {
                    if (item.Union(dataChange))
                    {
                        merged = true;
                        break;
                    }
                }

                if (!merged)
                {
                    changes.Add(dataChange.Clone() as ServiceDataChange);
                }
            }
        }

        public IEnumerable<ServiceDataChange> Dequeu()
        {
            IEnumerable<ServiceDataChange> retMessages;

            lock (changes)
            {
                retMessages = changes.ToArray();

                changes.Clear();
            }

            return retMessages;
        }
    }
}

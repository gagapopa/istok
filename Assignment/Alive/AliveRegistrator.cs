using System;
using System.Collections.Generic;
using System.Linq;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Класс для накопления изменений данных на сервере для всех активных сессийы
    /// </summary>
    class ServiceDataChangeRegistrator
    {
        Dictionary<Guid, ServiceDataChangeQueue> queuesDictionary;

        public ServiceDataChangeRegistrator()
        {
            queuesDictionary = new Dictionary<Guid, ServiceDataChangeQueue>();
        }

        public void SessionStarted(Guid userGuid)
        {
            lock (queuesDictionary)
            {
                queuesDictionary[userGuid] = new ServiceDataChangeQueue();
            }
        }

        public void SessionStopped(Guid userGuid)
        {
            lock (queuesDictionary)
            {
                queuesDictionary.Remove(userGuid);
            }
        }

        public void Enqueue(ServiceDataChange aliveMessage)
        {
            lock (queuesDictionary)
            {
                queuesDictionary.Values.AsParallel().ForAll(q => q.Enque(aliveMessage));
            }
        }

        public IEnumerable<ServiceDataChange> Dequeue(Guid userGuid)
        {
            lock (queuesDictionary)
            {
                ServiceDataChangeQueue queue;

                if (queuesDictionary.TryGetValue(userGuid, out queue))
                {
                    return queue.Dequeu();
                }

                return new ServiceDataChange[] { };
            }
        }
    }
}

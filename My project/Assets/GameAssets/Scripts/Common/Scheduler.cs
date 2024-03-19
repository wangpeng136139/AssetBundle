using System.Collections.Generic;
using UnityEngine;

namespace GameAsset
{
    public class Scheduler
    {
        private static float _realtimeSinceStartup;
        public static bool Autoslicing { get; set; } = true;
        public static bool IsBusy =>
           Autoslicing && Time.realtimeSinceStartup - _realtimeSinceStartup > AutoslicingTimestep;

        public static float AutoslicingTimestep { get; set; } = 1f / 60f;

        private static readonly Dictionary<string, SchedulerQueue> m_dicQueues = new Dictionary<string, SchedulerQueue>();
        private static readonly List<SchedulerQueue> m_queues = new List<SchedulerQueue>();
        private static readonly Queue<SchedulerQueue> m_append = new Queue<SchedulerQueue>();


        private static byte _updateMaxRequests = MaxRequests;
        public static byte MaxRequests { get; set; } = 10;
        public static void OnUpdateAll()
        {
            _realtimeSinceStartup = Time.realtimeSinceStartup;
            OnUpdateAllSheduler();
        }


        public static void Add(ISchedulerCharacter request,bool IsForceUpdate)
        {
            if(IsForceUpdate)
            {
                if(request != null)
                {
                    if(false == request.OnUpdate())
                    {
                        request.OnCompelte();
                        return;
                    }
                }
            }

            var key = request.GetType().Name;
            if (!m_dicQueues.TryGetValue(key, out var queue))
            {
                queue = new SchedulerQueue { Key = key, MaxLoadingCount = MaxRequests, Priority = request.Priority };
                m_dicQueues.Add(key, queue);
                m_append.Enqueue(queue);
            }

            queue.Add(request);
        }


        private static void OnUpdateAllSheduler()
        {
            if (m_append.Count > 0)
            {
                while (m_append.Count > 0)
                {
                    var item = m_append.Dequeue();
                    m_queues.Add(item);
                }

                m_queues.Sort(Comparison);
            }

            foreach (var queue in m_queues)
                if (false == queue.OnUpdate())
                    break;

            ResizeIfNeed();
        }

        private static void ResizeIfNeed()
        {
            if (_updateMaxRequests == MaxRequests) return;

            foreach (var queue in m_queues) queue.MaxLoadingCount = MaxRequests;

            _updateMaxRequests = MaxRequests;
        }

        private static int Comparison(SchedulerQueue x, SchedulerQueue y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }
}

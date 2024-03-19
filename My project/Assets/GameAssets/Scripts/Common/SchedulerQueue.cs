using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAsset
{
    public class SchedulerQueue
    {
        private  readonly List<ISchedulerCharacter> m_process = new List<ISchedulerCharacter>();

        private  readonly List<ISchedulerCharacter> m_prepear = new List<ISchedulerCharacter>();

        public int MaxLoadingCount = 10;

        public string Key { get;  set; }
        public int Priority { get; set; } = 1;
        public bool IsWorking => m_process.Count > 0 || m_prepear.Count > 0;
        public void Add(ISchedulerCharacter loading)
        {
            m_prepear.Add(loading);
        }

        public bool OnUpdate()
        {
            if (m_process.Count < MaxLoadingCount)
            {
                if(Scheduler.IsBusy)
                {
                    return false;
                }
                if (m_prepear.Count > 0)
                {
                    var temp = m_prepear[0];
                    m_prepear.RemoveAt(0);
                    m_process.Add(temp);
                }
            }

            for (int i = m_process.Count - 1; i > -1; --i)
            {
                var loadingitem = m_process[i];
                if(false == loadingitem.OnUpdate())
                {
                    m_process.RemoveAt(i);
                    loadingitem.OnCompelte();
                    --i;
                }
                if (Scheduler.IsBusy) return false;
            }
            return true;
        }
    }
}

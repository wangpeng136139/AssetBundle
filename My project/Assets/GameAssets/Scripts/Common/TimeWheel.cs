using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAsset
{
    public  class TimerWheel
    {
        private class TimerTickNode
        {
            private int m_delayTime = 0;
            private int m_repeat = 0;
            private int m_totalTime = 0;
            private bool m_release = false;
            private int m_flag = 0;
            public int DelayTime { get => m_delayTime; }
            public int Repeat { get => m_repeat; }
            public int TotalTime { get => m_totalTime; }

            public int Flag { get => m_flag; }

            public bool IsRelease()
            {
                return m_release;
            }
            public void Release()
            {
                m_release = true;
            }

            private Action m_Action;
            public void Reset(Action action, int flag, int repeat, int totalTime)
            {
                m_totalTime = totalTime;
                m_repeat = repeat;
                m_flag = flag;
                m_delayTime = totalTime;
                m_Action = null;
                m_Action += action;
                m_release = false;
            }

            public void AddDelayTime(int delayTime)
            {
                m_delayTime += delayTime;
            }

            public void ResetDelayTime()
            {
                m_delayTime = m_totalTime;
                m_release = false;
            }
            public void AddRepeate(int repeate)
            {
                m_repeat += repeate;
            }

            public void Invoke()
            {
                m_Action?.Invoke();
            }
        }
        private class TimerSlot
        {
            private int m_timeInterval = 0;
            private LinkedList<TimerTickNode> m_list = new LinkedList<TimerTickNode>();
            private  TimerWheel m_wheel = null;
            public TimerSlot(TimerWheel wheel, int slot, int timeInterval)
            {
                Reset(wheel, slot, timeInterval);
            }

            public void Reset(TimerWheel wheel, int slot, int timeInterval)
            {
                m_wheel = wheel;
                m_timeInterval = timeInterval;
            }

            public void AddTimerTick(TimerTickNode timerTick)
            {
                m_list.AddLast(timerTick);
            }

            public void Invoke()
            {
                foreach (var curNode in m_list)
                {
                    if (curNode.IsRelease() == false)
                    {
                        curNode.AddDelayTime(-m_timeInterval);
                        if (curNode.DelayTime <= 0)
                        {
                            curNode.Invoke();
                            if (curNode.Repeat > 0)
                            {
                                curNode.AddRepeate(-1);
                            }

                            if (curNode.Repeat == 0)
                            {
                                curNode.Release();
                            }
                            else
                            {
                                curNode.ResetDelayTime();
                                m_wheel.AddTimer(curNode);
                            }
                        }
                    }

                    if (curNode.IsRelease())
                    {
                        m_wheel.RemoveTickNode(curNode.Flag);
                    }
                }
                m_list.Clear();
            }
        }


        private int m_flag = 0;
        private int Flag
        {
            get
            {
                ++m_flag;
                return m_flag;
            }
        }


        private int m_tickInterval = 0;
        private int m_wheelSize = 0;
        private int m_currentSlot = 0;
        private TimerSlot[] wheel = null;
        private Dictionary<int, TimerTickNode> m_dic = new Dictionary<int, TimerTickNode>();
        public TimerWheel(int tickInterval, int wheelSize)
        {
            this.m_tickInterval = tickInterval;
            this.m_wheelSize = wheelSize;
            wheel = new TimerSlot[wheelSize];
        }

        public void Run()
        {
            while (true)
            {
                if (wheel[m_currentSlot] != null)
                {
                    wheel[m_currentSlot].Invoke();
                }
                m_currentSlot = (m_currentSlot + 1) % m_wheelSize;
            }

        }

        public void CancelTick(int flag)
        {
            if (m_dic.TryGetValue(flag, out var tick))
            {
                tick.Release();
            }
        }

        private void RemoveTickNode(int flag)
        {
            m_dic.Remove(flag);
        }

        private int AddTimer(TimerTickNode timerTick)
        {
            var tick = timerTick.DelayTime / m_tickInterval;
            int solt = (m_currentSlot + tick) % m_wheelSize;
            if (wheel[solt] == null)
            {
                wheel[solt] = new TimerSlot(this, solt, solt * m_tickInterval);
            }
            wheel[solt].AddTimerTick(timerTick);
            return Flag;
        }

        public int AddTimer(Action action, int delayTime, int repeate)
        {
            TimerTickNode timerTick = new TimerTickNode();
            timerTick.Reset(action, Flag, repeate, delayTime);
            m_dic.Add(Flag, timerTick);
            return AddTimer(timerTick);
        }
    }


}

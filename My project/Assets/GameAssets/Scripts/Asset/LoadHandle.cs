using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameAsset
{
    public struct LoadHandle
    {
        private static int s_id = 0;
        public static uint GetId()
        {
            int newValue = Interlocked.Increment(ref s_id);
            return (uint)newValue;
        }
        public uint m_id ;
        public bool m_instance;
        public Action<object> m_callback;
        public float m_fBeginTime;
        public LoadHandle(bool instance,Action<object> action)
        {
            m_fBeginTime = Time.realtimeSinceStartup;
            m_id = GetId();
            m_instance = instance;
            m_callback = action;
        }
    }
}

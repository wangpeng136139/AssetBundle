using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameAsset
{
    public class RootSingletonMono<T>:MonoBehaviour where T:MonoBehaviour
    {
        private static T m_instance = null;
        public static T GetInstance()
        {

            if(m_instance == null)
            {
                GameObject t_temp = new GameObject($"[{typeof(T).Name}]");
                m_instance = t_temp.AddComponent<T>();
                t_temp.transform.parent = Root.s_RootTrans;
            }
            return m_instance;
        }
    }
}

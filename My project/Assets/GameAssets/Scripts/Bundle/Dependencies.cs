using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameAsset
{
    public class Dependencies
    {


        public float Process
        {
            get
            {
              
                if (m_bundlesIds == null || m_bundlesIds.Length < 1)
                {
                    return 1.0f;
                }

                if (loadedState == LoadedState.Compelete)
                {
                    return 1.0f;
                }

                float fProcess = 0.0f;
                for(int i = 0; i < m_bundlesIds.Length; ++i)
                {
                    int id = m_bundlesIds[i];
                    Bundle bundle = Bundle.GetBundleById(id);
                    if(bundle != null)
                    {
                        fProcess += bundle.Process;
                    }
                }

                return fProcess / m_bundlesIds.Length;
            }
        }
        public LoadedState loadedState
        {
            get
            {
                if (m_bundlesIds == null || m_bundlesIds.Length < 1)
                {
                    return LoadedState.Compelete;
                }

                for (int i = 0; i < m_bundlesIds.Length; i++)
                {
                    int id = m_bundlesIds[i];
                    Bundle bundle = Bundle.GetBundleById(id);
                    if (null != bundle)
                    {
                        if (bundle.loadedState == LoadedState.Loading)
                        {
                            return LoadedState.Loading;
                        }
                        else if(bundle.loadedState != LoadedState.Compelete)
                        {
                            return LoadedState.Error;
                        }
                    }
                }

                return LoadedState.Compelete;
            }
        }

        public bool IsDone
        {
            get 
            {
                if (m_bundlesIds == null || m_bundlesIds.Length < 1)
                {
                    return true;
                }

                for(int i = 0; i < m_bundlesIds.Length; i++)
                {
                    int id = m_bundlesIds[i];
                    Bundle bundle = Bundle.GetBundleById(id);
                    if (null != bundle && bundle.IsDone == false)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        protected int[] m_bundlesIds = null;
       
        public Dependencies(int[] ids)
        {
            m_bundlesIds = ids;
        }


        public void StartDown(bool isForce)
        {
            if (m_bundlesIds == null || m_bundlesIds.Length < 1)
            {
                return;
            }
            for (int i = 0; i < m_bundlesIds.Length; ++i)
            {
                var mapRes = Map.GetMapResByID(m_bundlesIds[i]);
                if (mapRes != null)
                {
                    var bundle =  Bundle.Creator(mapRes);
                    if (null != bundle)
                    {
                        bundle.StartDown(isForce);
                    }
                }
              
            }
        }

        public void AsyncLoad(bool Immediate)
        {
            if (m_bundlesIds == null || m_bundlesIds.Length < 1)
            {
                return;
            }

            for (int i = 0; i < m_bundlesIds.Length; ++i)
            {
                int id = m_bundlesIds[i];
                if (Immediate)
                {
                    Bundle.Load(id);
                }
                else
                {
                    Bundle.AsyncLoad(id, null);
                }
            }
        }
        public static Dependencies AsyncLoadBundle(int[] ids, bool Immediate)
        {
            if(ids == null || ids.Length < 1)
            {
                return null;
            }

            Dependencies dependencies = new Dependencies(ids);
            dependencies.AsyncLoad(Immediate);
            return dependencies;
        }
    }
}

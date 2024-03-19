using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAsset
{
    public class EditorBundle : Bundle
    {
        protected static int m_id = 0;
        public EditorBundle(MapResInfo resInfo)
        {
            m_mapRes = resInfo;
        }

        public static void Initialization()
        {
            s_bundles = new Bundle[10000];
        }

        public override void StartDown(bool bForce)
        {
          
        }


        protected override bool IsNeedDown()
        {
            return false;
        }


        public static EditorBundle Create(MapResInfo resInfo)
        {
            var bundle =  new EditorBundle(resInfo);
            resInfo.m_id = m_id;
            s_bundles[m_id] = bundle;
            s_dicCacheBundle.Add(resInfo.m_path, bundle);
            ++m_id;
            return bundle;
        }
    }
}

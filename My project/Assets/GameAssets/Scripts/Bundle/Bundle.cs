using System.Collections.Generic;
using UnityEngine;

namespace GameAsset
{

    public class Bundle : LoadingBase
    {
        #region 公共参数以及方法
        protected static Dictionary<string, Bundle> s_dicCacheBundle = new Dictionary<string, Bundle>();

        protected static Bundle[] s_bundles = null;


        public static Bundle[] Bundles => s_bundles;
        public static System.Func<MapResInfo, Bundle> Creator { get; set; } = Create;

        public static System.Action InitializationCallBack { get; set; } = Initialization;
        public static Bundle GetBundleById(int id)
        {
            Bundle bundle = null;
            try
            {
                bundle = s_bundles[id];
            }
            catch(System.Exception e)
            {
                AssetDebugEx.LogException(e);
                bundle = null;
            }
            finally
            {
               
            }
            return bundle;
        }

        public static Bundle GetBundleByPath(string path)
        {
            if(s_dicCacheBundle.TryGetValue(path,out var bundle))
            {
                return bundle;
            }
            return null;
        }

        public static void Initialization()
        {
            s_bundles = new Bundle[Map.GetBundleCount()];
        }

        protected static Bundle Create(MapResInfo resInfo)
        {
            if (s_dicCacheBundle.TryGetValue(resInfo.m_path, out var bundle))
            {
                return bundle;
            }
            return new Bundle(resInfo);
        }




        public static Bundle Load(string path)
        {
            AsyncLoad(path, true, null, out Bundle bundle);
            return bundle;
        }


        public static Bundle Load(int id)
        {
            AsyncLoad(id, true, null, out Bundle bundle);
            return bundle;
        }

        public static void AsyncLoad(int id, System.Action<int, object> action, out Bundle bundle)
        {
            AsyncLoad(id, false, action, out bundle);
        }

        public static void AsyncLoad(int id, System.Action<int, object> action)
        {
            AsyncLoad(id, false, action, out Bundle bundle);
        }

        public static void AsyncLoad(string path, System.Action<int, object> action, out Bundle bundle)
        {
            AsyncLoad(path, false, action, out bundle);
        }

        public static void AsyncLoad(string path, System.Action<int, object> action)
        {
            AsyncLoad(path, false, action, out Bundle bundle);
        }

        protected static void AsyncLoad(MapResInfo mapRes, bool IsImmediate, System.Action<int,object> action, out Bundle bundle)
        {
            if (mapRes == null)
            {
                bundle = null;
                action?.Invoke(-1,null);
                return;
            }

            int id = mapRes.m_id;
            bundle = s_bundles[id];
            if (bundle != null)
            {
                if (false == bundle.IsDone)
                {
                    if(action != null)
                    {
                        bundle.AddCallBack(action);
                    }
                    if (IsImmediate)
                    {
                        while (false == bundle.IsDone) ;
                    }
                }
                return;
            }

            bundle = Creator(mapRes);
            s_dicCacheBundle.Add(mapRes.m_bundlePath, bundle);
            s_bundles[bundle.GetID()] = bundle;
            bundle.Load(IsImmediate);
        }

        protected static bool AsyncLoad(string path, bool Immediate, System.Action<int, object> action, out Bundle bundle)
        {
            var res = Map.GetMapResByPath(path);
            AsyncLoad(res, Immediate, action, out bundle);
            return true;
        }

        protected static bool AsyncLoad(int id, bool Immediate, System.Action<int, object> action, out Bundle bundle)
        {
            var res = Map.GetMapResByID(id);
            AsyncLoad(res, Immediate, action, out bundle);
            return true;
        }
        #endregion



        protected FileAsset m_fileAsset = null;//加载器

        protected Dependencies m_dependencies = null;

        protected DownInfo m_downInfo = null;


        protected MapResInfo m_mapRes = null;
        protected List<Asset> m_assets = new List<Asset>();

        public bool IsNative => m_mapRes.m_isNative;


        public FileAsset FileAsset => m_fileAsset;
        public float Process
        {
            get
            {
                if (loadedState == LoadedState.Compelete)
                {
                    return 1.0f;
                }

                int count = 0;
                float fProcess = 0.0f;
                if (m_downInfo != null && m_downInfo.IsDone == false)
                {
                    ++count;
                    fProcess += m_downInfo.Process;
                }

                if (m_fileAsset != null && m_fileAsset.IsDone == false)
                {
                    ++count;
                    fProcess += m_fileAsset.Progress;
                }

                if (m_dependencies != null)
                {
                    fProcess += m_dependencies.Process;
                    ++count;
                }

                fProcess = fProcess / count;
                return fProcess;
            }
        }


        public void AddAsset(Asset asset)
        {
            m_assets.Add(asset);
        }

        public void RemoveAsset(Asset asset)
        {
            m_assets.Remove(asset);
        }


        public bool IsRefrence()
        {
            for(int i = 0; i < m_assets.Count; ++i)
            {
                var item = m_assets[i];
                if(item.IsReference())
                {
                    return true;
                }
            }
            return false;
        }

        public int GetID()
        {
            return m_mapRes.m_id;
        }

        public string GetBundleName()
        {
            return m_mapRes.m_bundlePath;
        }

        protected Bundle()
        {

        }
        protected Bundle(MapResInfo resInfo)
        {
            m_mapRes = resInfo;
            if (resInfo != null && resInfo.m_dependsId != null && resInfo.m_dependsId.Length > 0)
            {
                m_dependencies = new Dependencies(resInfo.m_dependsId);
            }
        }


        private System.Action<int,object> m_onFinish;

        protected void AddCallBack(System.Action<int, object> callBack)
        {
            if(callBack != null)
            {
                m_onFinish += callBack;
            }
        }


        public virtual void StartDown(bool bForce)
        {
            if (bForce || IsNeedDown())
            {
                m_downInfo = DownInfo.StartDown(Map.GetDownPath(m_mapRes), Map.GetSavePath(m_mapRes), m_mapRes.m_startPos, m_mapRes.m_length, m_mapRes.m_crc, true);
            }
        }

        public override void Load(bool IsImmediate)
        {
            loadedState = IsImmediate ? LoadedState.ImmediateLoading : LoadedState.Loading;
            //是否需要下载
            StartDown(false);
            if (m_dependencies != null)
            {
                m_dependencies.AsyncLoad(IsImmediate);
            }
            AddQueue(loadedState == LoadedState.ImmediateLoading);
        }

        public override bool OnUpdate()
        {
            if(m_dependencies != null)
            {
                if(false == m_dependencies.IsDone)
                {
                    return true;
                }
            }

            if (m_downInfo != null && false == m_downInfo.IsDone)
            {
                return true;
            }

            if(m_downInfo != null && m_downInfo.DownloadStatus == DownloadStatus.Failed)
            {
                loadedState = LoadedState.Error;
            }

            if(loadedState != LoadedState.Error)
            {
                if (m_fileAsset == null)
                {
                    if(loadedState == LoadedState.ImmediateLoading)
                    {
                        m_fileAsset = LoadAssetBundle();
                    }
                    else
                    {
                        m_fileAsset = AsyncLoadBundle();
                    }
                    if (m_fileAsset == null)
                    {
                        loadedState = LoadedState.Error;
                    }
                }


                if (m_fileAsset != null)
                {
                    if(m_fileAsset.IsDone == false)
                    {
                        return true;
                    }
                    else
                    {
                        loadedState = LoadedState.Compelete;
                        if (m_dependencies != null)
                        {
                            if (m_dependencies.loadedState != LoadedState.Compelete)
                            {
                                loadedState = LoadedState.Error;
                            }
                        }
                    }
                }
            }



            if (IsDone)
            {
                return false;
            }
            return true;
        }

        private FileAsset LoadAssetBundle()
        {
            if(loadedState == LoadedState.Error)
            {
                return null;
            }
            if (PathUtils.Exist(m_mapRes.m_path, out string fullPath))
            {
                var assetBundle = FileAsset.Load(fullPath,IsNative,true);
                return assetBundle;
            }
            return null;
        }


        protected FileAsset AsyncLoadBundle()
        {
            if (loadedState == LoadedState.Error)
            {
                return null;
            }

            if (PathUtils.Exist(m_mapRes.m_path, out string fullPath))
            {
                var request = FileAsset.Load(fullPath, m_mapRes.m_isNative, false);
                return request;
            }
            return null;
        }

        public Object LoadAsset(string path)
        {
            if(m_fileAsset != null)
            {
                return m_fileAsset.LoadAsset(path);
            }
            return null;
        }

        private void OnFinish(object obj)
        {
            var tempCallBack = m_onFinish;
            m_onFinish = null;
            tempCallBack?.Invoke(m_mapRes.m_id, obj);
        }

        protected virtual bool  IsNeedDown()
        {
            return Map.IsNeedDown(m_mapRes.m_path, m_mapRes.m_length, m_mapRes.m_crc, out var fullPath,false);
        }

        public override bool OnCompelte()
        {
            OnFinish(null);
            return true;
        }
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace GameAsset
{
    public class Asset : LoadingBase
    {
        #region 公共区域
        protected static Dictionary<string, Asset> s_dicCacheAsset = new Dictionary<string, Asset>();
        public static Dictionary<string, Asset> DicCacheAsset => s_dicCacheAsset;

        public static Func<string, Type, Asset> Creator { get; set; } = Create;

        protected static Asset Create(string path, Type type)
        {
            var mapRes = Map.GetMapResByAssetPath(path);
            if(mapRes == null)
            {
                AssetDebugEx.Log($"无法找到{path}");
                return null;
            }

            var bundle = Bundle.Creator(mapRes);
            var asset = new Asset(path, type, bundle);
            return asset;
        }
        


        public static T Load<T>(string path,bool isInstance) where T:class
        {
            return (T)Load(path, typeof(T), isInstance) as T;
        }

        public static object Load(string path, Type type, bool isInstance)
        {
            AsyncLoad(path, true, type, null, isInstance, out Asset asset);
            if(isInstance)
            {
                return asset.GetInstanceObj();
            }
            else
            {
                return asset.GetObject();
            } 
          
        }

        public static void AsyncLoad<T>(string path, bool isInstance, Action<object> action)
        {
            AsyncLoad(path, false, typeof(T), action, isInstance, out Asset asset);
        }

        public static void AsyncLoad(string path, Type type,bool isInstance, Action<object> action)
        {
            AsyncLoad(path, false, type, action, isInstance, out Asset asset);
        }

        protected static void AsyncLoad(string path, bool Immediate, Type type, Action<object> action,bool isInstance, out Asset asset)
        {
            if (s_dicCacheAsset.TryGetValue(path, out asset))
            {
                if (false == asset.IsDone)
                {
                    asset.AddOnFinishCallBack(isInstance, action);
                    if (Immediate)
                    {
                        while (false == asset.IsDone);
                    }
                    return;
                   
                }
                else if(asset.loadedState == LoadedState.UnLoad || asset.loadedState == LoadedState.None)
                {
                    asset.Load(Immediate);
                    return;
                }
            }

       
            asset = Creator(path, type);
            if (Immediate == false)
            {
                asset.AddOnFinishCallBack(isInstance, action);
            }
            s_dicCacheAsset.Add(path, asset);
            asset.Load(Immediate);
        }
        #endregion

        private WeakReference m_objectRefrence = null;

        protected object m_object = null;

        protected WeakReference m_bundleReference = null;

        protected List<WeakReference> m_weakReferences = new List<WeakReference>();

        private AssetBundleRequest m_assetBundleRequest = null;
        /// <summary>
        /// 是否是常驻内存
        /// </summary>
        private bool m_IsResident = false;
        public override bool OnUpdate()
        {
            return OnUpdateState();
        }


        public bool IsReference()
        {
            if (true == m_IsResident)
            {
                return true;
            }

            if (m_objectRefrence != null)
            {
                if (m_objectRefrence.IsAlive)
                {
                    return true;
                }
            }

            for(int i = 0; i < m_weakReferences.Count; ++i)
            {
                var item = m_weakReferences[i];
                if(item.IsAlive)
                {
                    return true;
                }
            }


            return false;
        }


        protected void SetObject(object obj)
        {
            if(obj == null)
            {
                return;
            }
            if(m_IsResident)
            {
                m_object = obj;
            }
            else
            {
                m_objectRefrence = new WeakReference(obj);
            }
        }

        public int GetBundleID()
        {
            if(m_bundleReference != null && m_bundleReference.Target != null)
            {
                var bundle = m_bundleReference.Target as Bundle;
                if(bundle != null)
                {
                    return bundle.GetID();
                }
            }
            return -1;
        }

        public override bool OnCompelte()
        {
            OnFinish(GetBundleID(), GetObject());
            return true;
        }

       
        protected virtual bool OnUpdateState()
        {
            if (m_bundleReference == null || m_bundleReference.Target == null)
            {
                loadedState = LoadedState.Error;
            }
            else
            {
                var bundle = m_bundleReference.Target as Bundle;
                if (bundle == null)
                {
                    SetObject(null);
                }
                else
                {
                    if (bundle.IsDone)
                    {
                        if (bundle.FileAsset == null)
                        {
                            loadedState = LoadedState.Error;
                        }
                        else
                        {
                            if(bundle.FileAsset.loadedState == LoadedState.ImmediateLoading)
                            {
                                var obj = bundle.LoadAsset(m_path);
                                if (obj == null || bundle.loadedState == LoadedState.Error)
                                {
                                    loadedState = LoadedState.Error;
                                }
                                else
                                {
                                    loadedState = LoadedState.Compelete;
                                }
                                SetObject(obj);
                            }
                            else if (bundle.IsNative)
                            {
                                var obj = bundle.FileAsset.GetBytes();
                                loadedState = bundle.FileAsset.loadedState;
                                SetObject(obj);
                            }
                            else
                            {
                                if (m_assetBundleRequest == null)
                                {
                                    m_assetBundleRequest = bundle.FileAsset.LoadAssetAsync(m_path);
                                    if (m_assetBundleRequest == null)
                                    {
                                        loadedState = LoadedState.Error;
                                        SetObject(null);
                                    }
                                }
                                else
                                {
                                    if (m_assetBundleRequest.isDone)
                                    {
                                        var obj = m_assetBundleRequest.asset;
                                        loadedState = obj == null ? LoadedState.Error : LoadedState.Compelete;
                                        SetObject(obj);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if(IsDone)
            {
                return false;
            }
            return true;
        }


        protected List<LoadHandle> m_assetHandles = new List<LoadHandle>();
        protected string m_path = string.Empty;
        protected Type m_type;

        protected Asset()
        {

        }
        protected Asset(string path, Type type,Bundle bundle)
        {
            m_path = path;
            m_type = type;
            m_bundleReference = new WeakReference(bundle);
            bundle.AddAsset(this);
        }
           
        protected void AddOnFinishCallBack(bool isInstance, Action<object> action)
        {
            if(m_assetHandles != null)
            {
                m_assetHandles = new List<LoadHandle>();
            }
            m_assetHandles.Add(new LoadHandle(isInstance, action));
        }

        public string GetPath()
        {
            return m_path;
        }


        public object GetObject()
        {
            if(m_IsResident)
            {
                return m_object;
            }

            if(m_objectRefrence == null)
            {
                return null;
            }
            return m_objectRefrence.Target;
        }

        public Bundle GetBundle()
        {
            if(m_bundleReference != null && m_bundleReference.Target != null)
            {
                return m_bundleReference.Target as Bundle;
            }
            return null;
        }

        public string GetBundleName()
        {
            if(m_bundleReference != null && m_bundleReference.Target != null)
            {
                var bundle = m_bundleReference.Target as Bundle;
                if(bundle != null)
                {
                    return bundle.GetBundleName();
                }
            }
          
            return string.Empty;
        }


        public  override void Load(bool IsImmediate)
        {
            loadedState = IsImmediate? LoadedState.ImmediateLoading: LoadedState.Loading;
            var bundle = GetBundle();
            bundle?.Load(loadedState == LoadedState.ImmediateLoading);
            this.AddQueue(loadedState == LoadedState.ImmediateLoading);
        }



        private object GetInstanceObj()
        {
            var obj = GetObject();
            var t_obj = obj;
            if (typeof(GameObject) == m_type)
            {
                t_obj = GameObject.Instantiate(obj as GameObject);
            }
            else if (typeof(Material) == m_type)
            {
                t_obj = new Material(obj as Material);
            }
            else if (t_obj is UnityEngine.Object)
            {
                t_obj = UnityEngine.Object.Instantiate(obj as UnityEngine.Object);
            }
            else
            {
                AssetDebugEx.LogError("无法确认要拷贝的类型");
            }

            if(t_obj != null)
            {
                m_weakReferences.Add(new WeakReference(t_obj));
            }
            return t_obj;
        }
        private void OnFinish(int id, object obj)
        {
            var handles = m_assetHandles;
            m_assetHandles = null;           
            if(handles != null)
            {
                for (int i = 0; i < handles.Count; ++i)
                {
                    var item = handles[i];
                    object t_obj = obj;
                    if (true == item.m_instance)
                    {
                        t_obj = GetInstanceObj();
                    }
                    item.m_callback?.Invoke(t_obj);
                }
            }
        }

        public void UnLoad()
        {
            loadedState = LoadedState.UnLoad;
            m_object = null;
            m_objectRefrence = null;
            m_bundleReference = null;
        }
    }
}
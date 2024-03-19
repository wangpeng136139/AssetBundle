using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GameAsset
{
    public class EditorAsset : Asset
    {
        protected EditorAsset(string path, Type type, Bundle bundle):base(path, type, bundle)
        {
            m_bundleReference = new WeakReference(bundle);
            bundle.AddAsset(this);
        }

        public static EditorAsset Create(string path, Type type)
        {
            MapResInfo mapResInfo = new MapResInfo();
            mapResInfo.m_path = path;
            mapResInfo.m_bundlePath = path;
            var bundle = Bundle.Creator(mapResInfo);
            var asset = new EditorAsset(path, type, bundle);
            return asset;
        }

        public override void Load(bool IsImmediate)
        {
            loadedState = IsImmediate ? LoadedState.ImmediateLoading : LoadedState.Loading;
            var bundle = GetBundle();
            bundle?.Load(loadedState == LoadedState.ImmediateLoading);
            this.AddQueue(loadedState == LoadedState.ImmediateLoading);
        }

        public override bool OnUpdate()
        {
            var obj = AssetDatabase.LoadAssetAtPath(m_path, m_type);
            SetObject(obj);
            return false;
        }

    }
}

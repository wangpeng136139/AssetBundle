using GameAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
namespace GameAsset
{
    [CustomEditor(typeof(Updater))]
    public class UpdaterInspectorGUI: Editor
    {

        private bool m_isShowAsset = false;
        private bool m_isShowBundle = false;
        private bool m_isShowDown = false;
        public override void OnInspectorGUI()
        {
            m_isShowAsset = EditorGUILayout.Toggle("是否显示Asset", m_isShowAsset);
            if(m_isShowAsset)
            {
                UpdateAsset();
            }

            m_isShowBundle = EditorGUILayout.Toggle("是否显示Bundle", m_isShowBundle);
            if(m_isShowBundle)
            {
                EditorGUILayout.Space();
                UpdateBundle();
            }

            m_isShowDown = EditorGUILayout.Toggle("是否显示Down", m_isShowDown);
            if(m_isShowDown)
            {
                EditorGUILayout.Space();
                UpdateDown();
            }
        }

        private void UpdateDown()
        {
            var downs = DownInfo.S_CacheDown;
            if(downs != null)
            {
                EditorGUILayout.BeginVertical("box");
                foreach (var down in downs.Values)
                {
                    if (null != down)
                    {
                        EditorGUILayout.LabelField($"savePath:{down.SavePath}");
                        EditorGUILayout.LabelField($"path:{down.DownloadStatus}");
                        EditorGUILayout.LabelField($"status:{down.DownloadStatus}");
                        EditorGUILayout.Space();
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }


        private void UpdateAsset()
        {
            var assets = Asset.DicCacheAsset;
            if(assets != null)
            {
                EditorGUILayout.BeginVertical("box");
                foreach(var asset in assets.Values)
                {
                    if(null != asset)
                    {
                        EditorGUILayout.LabelField($"path:{asset.GetPath()}");
                        EditorGUILayout.LabelField($"bundleName:{asset.GetBundleName()}");
                        EditorGUILayout.LabelField($"status:{asset.loadedState}");
                        EditorGUILayout.Space();
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
        private void UpdateBundle()
        {
            var bundles = Bundle.Bundles;
            if(bundles != null)
            {
                EditorGUILayout.BeginVertical("box");
                for(int i = 0; i < bundles.Length; ++i)
                {
                    var bundle = bundles[i];
                    if(null != bundle)
                    {
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField($"id:{bundle.GetID()}");
                        EditorGUILayout.LabelField($"bundlename:{bundle.GetBundleName()}");
                        EditorGUILayout.LabelField($"status:{bundle.loadedState}");
                        EditorGUILayout.Space();
                        EditorGUILayout.EndVertical();
                    }    
                
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}


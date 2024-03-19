
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameAsset
{
    [CustomEditor(typeof(GameAssetsScriptableObject))]
    public class GameAssetsScriptableObjectInspector : Editor
    {

        private GameAssetsScriptableObject config;
       // public string[] options = new string[] { "不打包","单独文件AssetBundle", "整体文件夹AssetBundle", "单独文件夹AssetBundle","第一层文件夹AssetBundle" };
        void OnEnable()
        {
            config = target as GameAssetsScriptableObject;
        }
        public override void OnInspectorGUI()
        {
            OnUpdateList();
        }

        private void OnUpdateList()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical();
            List<int> deleteList = new List<int>();
            GUILayout.Label($"版本号:");
            if(config.m_version == null)
            {
                config.m_version = new GameVersion("1.0.0");
            }


            var verstion =  EditorGUILayout.TextField(config.m_version.ToString());
            if (GameVersion.TryParse(verstion, out var result))
            {
                config.m_version = result;
            }

            GUILayout.Label($"调试模式:");
            var names = Enum.GetNames(typeof(DevelopGameMode));
            config.m_simulationMode = (DevelopGameMode)EditorGUILayout.Popup((int)config.m_simulationMode,names);
            GameMode.IsUpdate = config.m_simulationMode == DevelopGameMode.DevelopGameMode_UpdateAssetBundle;
            GUILayout.Label($"导出更新包路径:");
            var rect = EditorGUILayout.GetControlRect(GUILayout.Width(500));
            config.m_outPath = EditorGUI.TextField(rect, config.m_outPath);
            if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited) && rect.Contains(Event.current.mousePosition))
            {
                //改变鼠标的外表
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                {
                    var t_path = DragAndDrop.paths[0];
                    t_path = t_path.Replace("\\", "/");
                    if (string.IsNullOrEmpty(t_path))
                    {
                        EditorUtility.DisplayDialog("提示", "不能添加空路径", "确认");
                        return;
                    }
                    config.m_outPath = t_path;
                }
            }

            GUILayout.Label($"设置资源映射文件下载地址:");
            config.m_downMapPath = EditorGUILayout.TextField(config.m_downMapPath);

            GUILayout.Label($"设置资源下载地址:");
            config.m_downPath = EditorGUILayout.TextField(config.m_downPath);

            GUILayout.Label($"设置过滤后缀名(以;分隔，例如.meta;.txt):");
            config.m_ignoreExt = EditorGUILayout.TextField(config.m_ignoreExt);


       
            /*            GUILayout.Label($"AssetBundle打包方式:");
                        config.m_buildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.Popup((int)config.m_buildAssetBundleOptions, Enum.GetNames(typeof(BuildAssetBundleOptions)));
                        EditorGUILayout.Space();*/


            config.m_isAppendHashToAssetBundleName = EditorGUILayout.Toggle("使用MD5后缀:",config.m_isAppendHashToAssetBundleName);
            EditorGUILayout.Space();

            for (int i = 0; i < config.m_gameAssets.Count; ++i)
            {
    
                GUI.color = Color.gray;
                EditorGUILayout.BeginVertical("box");
                var item = config.m_gameAssets[i];
                string str = $"更新包{i}：";
                item.m_isShowDetails = EditorGUILayout.BeginFoldoutHeaderGroup(item.m_isShowDetails,str);
                if (item.m_isShowDetails)
                {
                    if (item.m_bundles != null)
                    {
                        for (int j = 0; j < item.m_bundles.Count; ++j)
                        {
                            GUI.color = Color.white;
                            EditorGUILayout.BeginVertical("helpBox");

                            var bundle = item.m_bundles[j];
                            GUILayout.Label($"路径:");
                            rect = EditorGUILayout.GetControlRect(GUILayout.Width(500));
                            bundle.m_pathName = EditorGUI.TextField(rect,bundle.m_pathName);
                            if ((Event.current.type == EventType.DragUpdated|| Event.current.type == EventType.DragExited)&& rect.Contains(Event.current.mousePosition))
                            {
                                //改变鼠标的外表
                                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                                {
                                    var t_path = DragAndDrop.paths[0];
                                    t_path = t_path.Replace("\\", "/");
                                    if (string.IsNullOrEmpty(t_path))
                                    {
                                        EditorUtility.DisplayDialog("提示", "不能添加空路径", "确认");
                                        return;                                  
                                    }
                                    bundle.m_pathName = DragAndDrop.paths[0];
                                }
                            }

                            bundle.m_isForceUpdate = GUILayout.Toggle(bundle.m_isForceUpdate, "强制更新");
                            bundle.m_isResident = GUILayout.Toggle(bundle.m_isResident, "常驻内存");
                            bundle.m_isCopyStreamingAssets = GUILayout.Toggle(bundle.m_isCopyStreamingAssets, "拷贝到StreammingAssets");

                            bundle.m_isShowDetails = GUILayout.Toggle(bundle.m_isShowDetails, "显示AB包细节");


                            if (bundle.m_isShowDetails)
                            {
                                if(bundle.m_bundleDetails != null)
                                {
                                    EditorGUILayout.BeginVertical("box");
                                    for (int k = 0; k < bundle.m_bundleDetails.Count; ++k)
                                    {   
                                        var bundleDetail = bundle.m_bundleDetails[k];
                                        EditorGUILayout.LabelField(bundleDetail);
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                            }
               

                            EditorGUILayout.BeginHorizontal();
                            var enums =  Enum.GetNames(typeof(GameAssetsType));
                            bundle.m_bundleType = (GameAssetsType)EditorGUILayout.Popup((int)bundle.m_bundleType, enums);
                            if (GUILayout.Button("-"))
                            {
                                item.m_bundles.RemoveAt(j);
                                return;
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();
                            GUI.color = Color.white;
                        }
                        GUI.color = Color.white;
                        if (GUILayout.Button("添加AB"))
                        {
                            item.m_bundles.Add(new GameAssetsBundleInfo());
                        }
                        EditorGUILayout.Space();
                    }

                    GUI.color = Color.white;
                    if (GUILayout.Button("删除更新包"))
                    {
                        if (EditorUtility.DisplayDialog("提示", $"是否要删除{str}", "确认", "取消"))
                        {
                            deleteList.Add(i);
                        }
                    }
                }
 
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
            }
            
            for(int i = 0; i < deleteList.Count; ++i)
            {
                config.m_gameAssets.RemoveAt(deleteList[i]);
            }

            GUI.color = Color.white;
            if (GUILayout.Button("添加更新包"))
            {
                config.m_gameAssets.Add(new GameAssetsSettingInfo());
            }
            bool bCheck = EditorGUI.EndChangeCheck();
            if(bCheck)
            {
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssetIfDirty(config);
            }
            EditorGUILayout.EndVertical();
        }
    }


}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameAsset
{
    public static class GameAssetMenu
    {
        [MenuItem("GameAssets/创建资源目录信息")]
        public static void GameAssetsScriptableObject()
        {
            var scriptable =  ScriptableObject.CreateInstance<GameAssetsScriptableObject>();
            UnityEditor.AssetDatabase.CreateAsset(scriptable, EditorPathUtils.GetGameAssetsScriptablePath()); //创建好的对象保存到本地路径
            UnityEditor.AssetDatabase.SaveAssets();//保存资源
            UnityEditor.AssetDatabase.Refresh();//刷新
        }


        [MenuItem("GameAssets/选中资源目录信息")]
        public static void SelectGameAssetsScriptableObject()
        {
            var temp = AssetDatabase.LoadAssetAtPath<GameAssetsScriptableObject>(EditorPathUtils.GetGameAssetsScriptablePath());
            if(temp == null)
            {
                EditorUtility.DisplayDialog("提示", $"{EditorPathUtils.GetGameAssetsScriptablePath()}文件不存在!!!!", "确认");
                return;
            }
            Selection.activeObject = temp;
            EditorGUIUtility.PingObject(temp);
        }

        [MenuItem("GameAssets/清除所有AssetBundleName")]
        public static void CleanAssetBundle()
        {
            AssetBundleHelper.ClearAssetBundleName();
        }

        [MenuItem("GameAssets/设置所有AssetBundleName")]
        public static void SetAssetBundleName()
        {
            AssetBundleHelper.SetAllAssetBundleName();
        }

        [MenuItem("GameAssets/导出更新包")]
        public static void BuildOut()
        {
            AssetBundleHelper.BuildAsset();
        }

        [MenuItem("GameAssets/清除更新包")]
        public static void ClearUpdateData()
        {
            DirectoryHelper.DeleteAllFileByDirectory(PathUtils.Combine(Application.streamingAssetsPath, FileNameDefine.UpdateBinDirectory), true);
            UnityEditor.EditorUtility.DisplayDialog("提示", "清除更新包", "确定");
        }


        [MenuItem("GameAssets/清除缓存目录")]
        public static void ClearPersistentDataPath()
        {
            DirectoryHelper.DeleteAllFileByDirectory(Application.persistentDataPath,true);
            UnityEditor.EditorUtility.DisplayDialog("提示","清除缓存目录成功","确定");
        }



        [MenuItem("GameAssets/打开缓存目录")]
        public static void OpenPersistentDataPath()
        {
            UnityEditor.EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
        [MenuItem("GameAssets/Open Asset Bundle Editor Windows")]
        public static void OpenEditorWindows()
        {
            AssetBundleEditorWindows.ShowWindow ();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameAsset
{
    public static class GameAssetMenu
    {
        [MenuItem("GameAssets/������ԴĿ¼��Ϣ")]
        public static void GameAssetsScriptableObject()
        {
            var scriptable =  ScriptableObject.CreateInstance<GameAssetsScriptableObject>();
            UnityEditor.AssetDatabase.CreateAsset(scriptable, EditorPathUtils.GetGameAssetsScriptablePath()); //�����õĶ��󱣴浽����·��
            UnityEditor.AssetDatabase.SaveAssets();//������Դ
            UnityEditor.AssetDatabase.Refresh();//ˢ��
        }


        [MenuItem("GameAssets/ѡ����ԴĿ¼��Ϣ")]
        public static void SelectGameAssetsScriptableObject()
        {
            var temp = AssetDatabase.LoadAssetAtPath<GameAssetsScriptableObject>(EditorPathUtils.GetGameAssetsScriptablePath());
            if(temp == null)
            {
                EditorUtility.DisplayDialog("��ʾ", $"{EditorPathUtils.GetGameAssetsScriptablePath()}�ļ�������!!!!", "ȷ��");
                return;
            }
            Selection.activeObject = temp;
            EditorGUIUtility.PingObject(temp);
        }

        [MenuItem("GameAssets/�������AssetBundleName")]
        public static void CleanAssetBundle()
        {
            AssetBundleHelper.ClearAssetBundleName();
        }

        [MenuItem("GameAssets/��������AssetBundleName")]
        public static void SetAssetBundleName()
        {
            AssetBundleHelper.SetAllAssetBundleName();
        }

        [MenuItem("GameAssets/�������°�")]
        public static void BuildOut()
        {
            AssetBundleHelper.BuildAsset();
        }

        [MenuItem("GameAssets/������°�")]
        public static void ClearUpdateData()
        {
            DirectoryHelper.DeleteAllFileByDirectory(PathUtils.Combine(Application.streamingAssetsPath, FileNameDefine.UpdateBinDirectory), true);
            UnityEditor.EditorUtility.DisplayDialog("��ʾ", "������°�", "ȷ��");
        }


        [MenuItem("GameAssets/�������Ŀ¼")]
        public static void ClearPersistentDataPath()
        {
            DirectoryHelper.DeleteAllFileByDirectory(Application.persistentDataPath,true);
            UnityEditor.EditorUtility.DisplayDialog("��ʾ","�������Ŀ¼�ɹ�","ȷ��");
        }



        [MenuItem("GameAssets/�򿪻���Ŀ¼")]
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

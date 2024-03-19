using GameAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

public class AssetBundleEditorWindows : EditorWindow
{

    private static string m_curAssetBundle = string.Empty;
    private static string m_curAsset = string.Empty;
    private static string m_curDepend = string.Empty;
    private static Vector2 scroll;
    private static GUIStyle selectableLabelStyle;
   
    public static void ShowWindow()
    {
        AssetBundleEditorWindows window = GetWindow<AssetBundleEditorWindows>("Asset Bundle Editor");
    }


    private void OnGUI()
    {
        if(selectableLabelStyle == null)
        {
            selectableLabelStyle = new GUIStyle(GUI.skin.label);
            selectableLabelStyle.normal.textColor = Color.white;
            selectableLabelStyle.normal.background = MakeTex(2, 2, new Color(0, 0, 1, 0.5f));
        }
        // 绘制第一个窗口
        GUILayout.BeginArea(new Rect(0, 10, position.width / 3, position.height), GUI.skin.window);
        GUILayout.Label("AssetBundle");

        var bundlenames = AssetDatabase.GetAllAssetBundleNames();

        Rect rect = new Rect();
        rect.width = position.width / 2;
        rect.height = 20 * bundlenames.Length;
        rect.y = -20;
        rect.x = -10;

        Rect viewRect = new Rect();
        viewRect.y = 20;
        viewRect.x = 0;
        viewRect.width = position.width / 2;
        viewRect.height = position.height;
        scroll = GUI.BeginScrollView(viewRect, scroll, rect, true,true);
        if (bundlenames != null && bundlenames.Length > 0)
        {
            Vector2 vector2 = Vector2.zero;
            foreach (var bundleName in bundlenames)
            {
                // 获取按钮的矩形区域
                if(vector2.y < scroll.y)
                {
                    vector2.y += 20;
                    continue;
                }
                if(vector2.y > scroll.y + position.height)
                {
                    break;
                }
                Rect rectitem = new Rect();
                rectitem.x = vector2.x;
                rectitem.width = position.width;
                rectitem.y = vector2.y;
                rectitem.height = 20;
                vector2.y += 20;

                if(bundleName == m_curAssetBundle)
                {
                    if (GUI.Button(rectitem, bundleName, selectableLabelStyle))
                    {

                    }
                }
                else
                {
                    if (GUI.Button(rectitem, bundleName, GUI.skin.label))
                    {
                        m_curAssetBundle = bundleName;
                        m_curAsset = string.Empty;
                        m_curDepend = string.Empty;
                    }
                }
               
                   
            }
        }
        else
        {
            EditorGUILayout.LabelField("No asset bundles found.");
        }
        GUI.EndScrollView();
        // 添加其他UI元素和布局
        GUILayout.EndArea();

        // 绘制第二个窗口
        GUILayout.BeginArea(new Rect(position.width / 3, 10, position.width / 3, position.height), GUI.skin.window);
        GUILayout.Label("Asset");
        if (!string.IsNullOrEmpty(m_curAssetBundle))
        {
            var assets = AssetDatabase.GetAssetPathsFromAssetBundle(m_curAssetBundle);
            if (assets != null)
            {
                foreach (var asset in assets)
                {
                    if(m_curAsset == asset)
                    {
                        if (GUILayout.Button(asset, selectableLabelStyle))
                        {
                            m_curAsset = asset;
                            var temp = AssetDatabase.LoadAssetAtPath(asset, typeof(Object));
                            Selection.activeObject = temp;
                            EditorGUIUtility.PingObject(temp);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(asset, GUI.skin.label))
                        {
                            m_curAsset = asset;
                            var temp = AssetDatabase.LoadAssetAtPath(asset, typeof(Object));
                            Selection.activeObject = temp;
                            EditorGUIUtility.PingObject(temp);
                        }
                    }

                }
            }
        }
        // 添加其他UI元素和布局
        GUILayout.EndArea();


        GUILayout.BeginArea(new Rect(position.width *2/ 3, 10, position.width / 3, position.height), GUI.skin.window);
        GUILayout.Label("Dependencies");
        if (!string.IsNullOrEmpty(m_curAsset))
        {
            var dependencies = AssetDatabase.GetDependencies(m_curAsset);
            if (dependencies != null)   
            {
                foreach (var depend in dependencies)
                {
                    if (m_curDepend == depend)
                    {
                        if (GUILayout.Button(depend, selectableLabelStyle))
                        {
                            m_curDepend = depend;
                            var temp = AssetDatabase.LoadAssetAtPath(depend, typeof(Object));
                            Selection.activeObject = temp;
                            EditorGUIUtility.PingObject(temp);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(depend, GUI.skin.label))
                        {
                            m_curDepend = depend;
                            var temp = AssetDatabase.LoadAssetAtPath(depend, typeof(Object));
                            Selection.activeObject = temp;
                            EditorGUIUtility.PingObject(temp);
                        }
                    }

                }
            }
        }
        // 添加其他UI元素和布局
        GUILayout.EndArea();
    }

    static Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = color;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}

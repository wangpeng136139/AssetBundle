using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameAsset
{
    public enum SimulatorMode
    {
        Editor,
        Simulator,
        AssetBundle,
    }
    public static class Initializer
    {
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoad()
        {
            var setting = AssetBundleHelper.GetGameAssetsScriptableObject();
            if (setting == null)  
            {
                AssetDebugEx.LogError("GameAssetsSetting is null");
                return;
            }
            var mode = setting.m_simulationMode;
            if(mode == DevelopGameMode.DevelopGameMode_Editor)
            {
                Asset.Creator = EditorAsset.Create;
                Bundle.Creator = EditorBundle.Create;
                Bundle.InitializationCallBack = EditorBundle.Initialization;
                GameMode.IsUseAssetBundle = false;
                GameMode.IsUpdate = false;
            }
            else if(mode == DevelopGameMode.DevelopGameMode_LocalAssetBundle)
            {
                GameMode.IsUseAssetBundle = true;
                GameMode.IsUpdate = false;
            }
            else if(mode == DevelopGameMode.DevelopGameMode_UpdateAssetBundle)
            {
                GameMode.IsUseAssetBundle = true;
                GameMode.IsUpdate = true;
            }
            else
            {
                AssetDebugEx.LogError("GameAssetsSetting mode is error");
            }
        } 
    }
}
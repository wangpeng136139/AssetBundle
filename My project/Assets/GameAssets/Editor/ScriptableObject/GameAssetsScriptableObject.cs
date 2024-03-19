using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GameAsset
{
    public enum GameAssetsType
    {
        None,
        Solo,
        Together,
        Folder,
        TopFolder,
    }

    [Serializable]
    public class GameAssetsBundleInfo
    {
        public string m_pathName = string.Empty;
        public GameAssetsType m_bundleType = GameAssetsType.Solo;
        public bool m_isCopyStreamingAssets = false;
        [NonSerialized]
        public bool m_isShowDetails = false;
        public bool m_isForceUpdate = false;
        public bool m_isResident = false;
        public List<string> m_bundleDetails = new List<string>();
        public List<string> m_nomarlDetails = new List<string>();
    }

    [Serializable]
    public class GameAssetsSettingInfo
    {
        [NonSerialized]
        public bool m_isShowDetails = false;


        public List<GameAssetsBundleInfo> m_bundles = new List<GameAssetsBundleInfo>();

    }

    [Serializable]
    public class GameAssetsScriptableObject:ScriptableObject
    {
        public GameVersion m_version;
        public string m_outPath = string.Empty;
        public string m_downMapPath = string.Empty;
        public string m_downPath = string.Empty;
        public List<GameAssetsSettingInfo> m_gameAssets = new List<GameAssetsSettingInfo>();
     //   public BuildAssetBundleOptions m_buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
        public bool m_isAppendHashToAssetBundleName = false;
        public string m_ignoreExt = string.Empty;

        public DevelopGameMode m_simulationMode = DevelopGameMode.DevelopGameMode_Editor;
        public bool ContainsPath(string path,out GameAssetsSettingInfo gameAsset, out GameAssetsBundleInfo bundleInfo)
        {
            gameAsset = null;
            bundleInfo = null;
            if (m_gameAssets != null)
            {
                for(int i= 0; i < m_gameAssets.Count; ++i)
                {
                    var item = m_gameAssets[i];
                    for(int j = 0; j < item.m_bundles.Count; ++j)
                    {
                        var bundle = item.m_bundles[j];
                        if (path.Contains(bundle.m_pathName)) 
                        {
                            bundleInfo = bundle;
                            gameAsset = item;
                            return true;
                        }
                    }

                }
            }
            return false;
        }
    }
}

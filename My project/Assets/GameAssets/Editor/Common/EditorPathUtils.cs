using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameAsset
{
    public static class EditorPathUtils
    {
        public static string GetGameAssetsScriptablePath()
        {
            return "Assets/GameAssets/Settings/GameAssetsSettings.asset";
           /* string path = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Assets/GameAssets/Settings/GameAssetsSettings.asset");
            path = path.Replace("\\", "/");
            return path;*/
        }

        public static string GetGameAssetsPath()
        {
            return "Assets";
        }


        public static string SplitAssetPath(string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                return path;
            }

            path = path.Replace(@"\", @"/").Replace(Application.dataPath.Replace(@"\", @"/"), "");
            path = path.Replace("/Assets/", "Assets/");
            if (path.StartsWith("Assets/"))
            {
                return path;
            }

            if (path.StartsWith("/"))
            {
                path = $"Assets{path}";
                return path;
            }

            return PathUtils.Combine("Assets", path);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAsset
{
    public static class PathUtils
    {
        private static string s_DownPath = Combine(UnityEngine.Application.persistentDataPath, FileNameDefine.UpdateBinDirectory, FileNameDefine.ResSaveDirectory);

        public static bool Exist(string path,bool isRealPath, out string fullPath)
        {
            if(false == isRealPath)
            {
                string temp = Combine(UnityEngine.Application.persistentDataPath, FileNameDefine.UpdateBinDirectory, FileNameDefine.ResSaveDirectory, path);
                if (FileUtils.Exist(temp))
                {
                    fullPath = temp;
                    return true;
                }

                temp = Combine(UnityEngine.Application.streamingAssetsPath, FileNameDefine.UpdateBinDirectory, FileNameDefine.ResSaveDirectory, path);
                if (FileUtils.Exist(temp))
                {
                    fullPath = temp;
                    return true;
                }
                fullPath = string.Empty;
            }
            else
            {
                if (FileUtils.Exist(path))
                {
                    fullPath = path;
                    return true;
                }
                fullPath = string.Empty;
            }

            return false;
        }
        public static bool Exist(string path,out string fullPath)
        {
           return Exist(path,false, out fullPath);
        }

        public static string GetDownPath(string path)
        {
            return string.Empty;
        }

        public static string Combine(params string[] paths)
        {
            string path = Path.Combine(paths);
            path = path.Replace("\\", "/");
            return path;
        }

        public static string GetExtension(string path)
        {
            var ext = Path.GetExtension(path);
            return ext;
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            try
            {
                if(string.IsNullOrEmpty(path))
                {
                    return string.Empty;
                }
                var ext = Path.GetExtension(path);
                if(string.IsNullOrEmpty(ext))
                {
                    return path;
                }
                return path.Replace(ext, string.Empty);
            }
            catch(Exception e)
            {
                AssetDebugEx.LogException(e);
                return string.Empty;
            }
          
        }
    }
}

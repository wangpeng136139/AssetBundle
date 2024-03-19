using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameAsset
{
    public class FileUtils
    {
        public static bool Exist(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }
            return false;
        }

        public static FileStream OpenRead(string path)
        {
            if (File.Exists(path))
            {
                return File.OpenRead(path);
            }
          
            return null;
        }

        public static string ReadAllText(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);  
            }
            return string.Empty;
        }

        public static long GetFileLength(string path)
        {
            var stream = OpenRead(path);
            if (stream != null)
            {
                return stream.Length;
            }
            return 0;
        }


        public static bool WriteAllText(string path,string content)
        {
            try
            {
                var dir = Path.GetDirectoryName(path);
                DirectoryHelper.CreateDirectory(dir);
                File.WriteAllText(path, content);
                return true;
            }
            catch(Exception e)
            {
                AssetDebugEx.LogException(e);
                return false;
            }
        
        }
        public static bool FileCopy(string sourceFilePath, string targeteFilePath)
        {
            try
            {
                if (false == Exist(sourceFilePath))
                {
                    return false;
                }

                var dir = Path.GetDirectoryName(targeteFilePath);
                DirectoryHelper.CreateDirectory(dir);
                File.Copy(sourceFilePath, targeteFilePath);
                return true;
            }
            catch(Exception e)
            {
                AssetDebugEx.LogException(e);
            }
            return false;
        }
    }

}

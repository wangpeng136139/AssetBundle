using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAsset
{
    public static class DirectoryHelper
    {
        public static void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void DeleteAllFileByDirectory(string directoryPath, bool isClearDirect = false)
        {
            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath,"*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    File.Delete(file);
                }

                if (isClearDirect)
                {
                    string[] dirs = Directory.GetDirectories(directoryPath);
                    foreach (string dir in dirs)
                    {
                        Directory.Delete(dir, true);
                    }
                }
            }
        }

        public static bool CreateDirectory(string dir, bool bClean = false)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            else if (bClean)
            {
                DeleteAllFileByDirectory(dir);
            }
            return true;
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            if (!Directory.Exists(target.FullName))
            {
                Directory.CreateDirectory(target.FullName);
            }

            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(PathUtils.Combine(target.ToString(), fi.Name), true);
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}

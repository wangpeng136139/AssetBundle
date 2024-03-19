using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameAsset
{
    public class AssetBundleHelper
    {
        public static GameAssetsScriptableObject GetGameAssetsScriptableObject()
        {
            var temp = AssetDatabase.LoadAssetAtPath<GameAssetsScriptableObject>(EditorPathUtils.GetGameAssetsScriptablePath());
            return temp;
        }



        public static string GetOriginBundle(string assetBundle)
        {
            var pos = assetBundle.LastIndexOf("_", StringComparison.Ordinal) + 1;
            var hash = assetBundle.Substring(pos);
            if (!string.IsNullOrEmpty(FileNameDefine.AssetBundletExtension)) hash = hash.Replace(FileNameDefine.AssetBundletExtension, "");

            var originBundle = $"{assetBundle.Replace("_" + hash, "")}";
            return originBundle;
        }


        public static void ClearAssetBundleName()
        {
            DirectoryInfo directoryInfo = Directory.CreateDirectory(EditorPathUtils.GetGameAssetsPath());
            ClearAssetBundleName(directoryInfo);
        }

        public static void SetAllAssetBundleName()
        {
            SetAllAssetBundleName(out var scriptable);
        }

        public static void SetAllAssetBundleName(out GameAssetsScriptableObject scriptable)
        {
            scriptable = GetGameAssetsScriptableObject();
            var ignoreExt = scriptable.m_ignoreExt.Split(";");
            if (scriptable != null)
            {
                for (int i = 0; i < scriptable.m_gameAssets.Count; ++i)
                {
                    var game = scriptable.m_gameAssets[i];
                    if (game != null)
                    {
                        for (int j = 0; j < game.m_bundles.Count; ++j)
                        {
                            var bundle = game.m_bundles[j];
                            var dicBundle = AssetBundleHelper.SetAssetBundleName(bundle.m_pathName, bundle.m_bundleType,out var nomarlfile,ignoreExt);
                            bundle.m_bundleDetails.Clear();
                            bundle.m_bundleDetails.AddRange(dicBundle.Keys.ToArray());
                            bundle.m_nomarlDetails.Clear();
                            bundle.m_nomarlDetails.AddRange(nomarlfile.Keys.ToArray());
                        }
                    }
                }
            }
            EditorUtility.SetDirty(scriptable);
            AssetDatabase.SaveAssetIfDirty(scriptable);
        }


        private static  string  SetFileAssetBundleName(string path,string assetBundle,bool isScene,out bool isSupport)
        {
            isSupport = false;
            try
            {
                AssetImporter assetImporter = AssetImporter.GetAtPath(path);
                if (assetImporter == null)
                {
                    AssetDebugEx.LogError($"assetImporter 找不到 {path}");
                    isSupport = false;
                    return string.Empty;
                }
                if (false == string.IsNullOrEmpty(assetBundle))
                {
                    assetBundle = PathUtils.GetFileNameWithoutExtension(assetBundle);
                    if(isScene)
                    {
                        assetBundle = $"{assetBundle}_{FileNameDefine.AssetBundletSceneExtension}{FileNameDefine.AssetBundletExtension}";
                    }
                    else
                    {
                        assetBundle = $"{assetBundle}{FileNameDefine.AssetBundletExtension}";
                    }
                   
                    assetBundle = assetBundle.ToLower();
                }

                Debug.Log($"{path}-----{assetBundle}");
                assetImporter.assetBundleName = assetBundle;
                isSupport = true;
                return assetImporter.assetBundleName;
            }
            catch(Exception e)
            {
                AssetDebugEx.LogException(e);
            }

            return string.Empty;

        }



        private static bool IsEditorFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (type != null)
            {
                string namespaceName = type.Namespace;
                if (namespaceName == "UnityEditor")
                {
                    // 这是一个编辑器对象
                    AssetDebugEx.Log($"编辑器对象:{path}");
                    return true;
                }
            }

            return false;
        }
        private static bool IsScence(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            var ext = PathUtils.GetExtension(path);
            if (ext == ".unity")
            {
                return true;
            }
            return false;
        }

        public static Dictionary<string,bool> SetAssetBundleName(string path, GameAssetsType gameAssetsType,out Dictionary<string,bool> nomarlfile,string[] ignoreExt)
        {
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            nomarlfile = new Dictionary<string, bool>();
            if (string.IsNullOrEmpty(path))
            {
                return dic;
            }

 
            path = EditorPathUtils.SplitAssetPath(path);
            if (File.Exists(path))
            {
                var ext = PathUtils.GetExtension(path);
                if(ignoreExt != null && ignoreExt.Contains(ext))
                {
                    return dic;
                }
                var assetPath = EditorPathUtils.SplitAssetPath(path);
                if (IsEditorFile(assetPath) == true && gameAssetsType != GameAssetsType.None)
                {
                    return dic;
                }
                else if (IsScence(path))
                {
                    //是文件
                    var key = SetFileAssetBundleName(path, path, true,out bool isSupport);
                    if (isSupport)
                    {
                        dic[key] = true;
                    }
                }
                else if (gameAssetsType == GameAssetsType.None)
                {
                    SetFileAssetBundleName(path, string.Empty,false, out bool isSupport);
                    if(isSupport)
                    {
                        nomarlfile[path] = true;
                    }
                }
                else
                {
                    //是文件
                    var key = SetFileAssetBundleName(path, path,false, out bool isSupport);
                    if (isSupport)
                    {
                        dic[key] = true;
                    }
                 
                }
            }
            else if (Directory.Exists(path))
            {
               var directoryInfo =  Directory.CreateDirectory(path);
                if(directoryInfo != null && directoryInfo.Exists)
                {
                    FileInfo[] files = null;
                    if(gameAssetsType == GameAssetsType.Together || gameAssetsType == GameAssetsType.Solo)
                    {
                        files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
                    }
                    else if (gameAssetsType == GameAssetsType.Folder)
                    {
                        files = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                    }
                    else if(gameAssetsType == GameAssetsType.None)
                    {
                        files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
                    }
                    else if(gameAssetsType == GameAssetsType.TopFolder)
                    {
                        var dirs = directoryInfo.GetDirectories();
                        if(dirs != null)
                        {
                            for(int i = 0; i < dirs.Length; ++i)
                            {
                                var dir = dirs[i].FullName;
                                dir = EditorPathUtils.SplitAssetPath(dir);
                                var t_dic = SetAssetBundleName(dir, gameAssetsType, out var t_nomarlhash, ignoreExt);
                                foreach (var item in t_dic)
                                {
                                    dic[item.Key] = item.Value;
                                }

                                foreach (var item in t_nomarlhash)
                                {
                                    nomarlfile[item.Key] = item.Value;
                                }
                            }
                        }
                    }
                    else if (gameAssetsType == GameAssetsType.Folder)
                    {
                        string[] subDirectories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
                        if (subDirectories != null)
                        {
                            List<string> subDirectoriesList = new List<string>(subDirectories);
                            subDirectoriesList.Sort();
                            for (int i = 0; i < subDirectoriesList.Count; ++i)
                            {
                                var dir = subDirectories[i];
                                dir = EditorPathUtils.SplitAssetPath(dir);
                                var t_dic = SetAssetBundleName(dir, gameAssetsType, out var t_nomarlhash, ignoreExt);
                                foreach (var item in t_dic)
                                {
                                    dic[item.Key] = item.Value;
                                }

                                foreach (var item in t_nomarlhash)
                                {
                                    nomarlfile[item.Key] = item.Value;
                                }
                            }
                        }
                    }

                    if (files != null)
                    {
                        for (int  i  = 0; i < files.Length; ++i)
                        {
                            var file = files[i];
                            var assetPath = EditorPathUtils.SplitAssetPath(file.FullName);
                            if (IsEditorFile(assetPath) == true && gameAssetsType != GameAssetsType.None)
                            {
                                continue;
                            }
                            if (file.Extension.ToLower() != ".meta" && file.Extension.ToLower() != ".cs")
                            {
                                if(IsScence(assetPath) || gameAssetsType == GameAssetsType.Solo)
                                {
                                    var key = SetFileAssetBundleName(assetPath, assetPath, IsScence(assetPath),out bool isSupport);
                                    if (isSupport)
                                    {
                                        dic[key] = true;
                                    }
                                }
                                else if(gameAssetsType == GameAssetsType.Folder || gameAssetsType == GameAssetsType.Together)
                                {
                                    var key = SetFileAssetBundleName(assetPath, path,false, out bool isSupport);
                                    if(isSupport)
                                    {
                                        dic[key] = true;
                                    }
                                }
                                else if(gameAssetsType == GameAssetsType.None)
                                {
                                    var key = SetFileAssetBundleName(assetPath, string.Empty,false, out bool isSupport);
                                    if(isSupport)
                                    {
                                        nomarlfile[assetPath] = true;
                                    }
                                }
                            }
                        }
                    }

                   
                       
                }   
            }
            return dic;
        }
        /// <summary>
        /// 清除assetBundle
        /// </summary>
        /// <param name="directoryInfo"></param>
        private static void ClearAssetBundleName(DirectoryInfo directoryInfo)
        {
            if (directoryInfo.Exists)
            {
                DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                FileInfo[] fileInfos = directoryInfo.GetFiles();
                for (int i = 0; i < directoryInfos.Length; i++)
                {
                    ClearAssetBundleName(directoryInfos[i]);
                }   
                for (int i = 0; i < fileInfos.Length; i++)
                {
                    FileInfo fileInfo = fileInfos[i];
                    if (fileInfo.Extension.ToLower() != ".meta" && fileInfo.Extension.ToLower() != ".cs" && fileInfo.Extension.ToLower() != ".exr")
                    {
                        try
                        {
                            string path = fileInfo.FullName.Replace(@"\", @"/").Replace(Application.dataPath.Replace(@"\", @"/"), "");
                            path = "Assets" + path;
                            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
                            if(assetImporter != null)
                            {
                                assetImporter.assetBundleName = string.Empty;
                            }
                        }
                        catch (Exception e)
                        {   
                            AssetDebugEx.LogException(e);
                        }
        
                    }
                }
            }

            string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();
            if(oldABNames != null)
            {
                for (int i = 0; i < oldABNames.Length; i++)
                {
                    AssetDatabase.RemoveAssetBundleName(oldABNames[i], true);
                    EditorUtility.DisplayProgressBar("清除AB包名", "名字:" + oldABNames[i], i * 1.0f / oldABNames.Length);
                }
                EditorUtility.ClearProgressBar();
            }
        }



        /// <summary>
        /// 打AssetBundle
        /// </summary>
        public static void BuildAsset()
        {
            var target = GetGameAssetsScriptableObject();
            if(target == null)
            {
                EditorUtility.DisplayDialog("提示", "打包配置为空,请创建对应配置", "确认");
                return;
            }
            var buildOut = target.m_outPath;
            var bundleOptions = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle;
            if (target.m_isAppendHashToAssetBundleName)
            {
                bundleOptions = bundleOptions | BuildAssetBundleOptions.AppendHashToAssetBundleName;
            }


            if(string.IsNullOrEmpty(buildOut) || Directory.Exists(buildOut) == false)
            {
                EditorUtility.DisplayDialog("提示", "打包路径为空或不存在", "确认");
                return;
            }

            ClearAssetBundleName();
            SetAllAssetBundleName(out target);

            var ignore_ext = target.m_ignoreExt.Split(";");

            //打AB包
            string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            List<AssetBundleBuild> bundleBuilds = new List<AssetBundleBuild>();
            Dictionary<string, string> fileBuilds = new Dictionary<string, string>();
            for (int i = 0; i < assetBundleNames.Length; ++i)
            {
                var name = assetBundleNames[i];
                var assets = AssetDatabase.GetAssetPathsFromAssetBundle(name);
                if(assets != null && assets.Length > 0)
                {
                    AssetBundleBuild assetBundleBuild = new AssetBundleBuild
                    {
                        assetNames = assets,
                        assetBundleName = name,
                    };
                    bundleBuilds.Add(assetBundleBuild);
                }
            }


            DirectoryHelper.CreateDirectory(buildOut,true);

            string assetBundlePath = Path.Combine(buildOut, FileNameDefine.ResSaveDirectory).Replace("\\", "/");
            assetBundlePath = assetBundlePath.ToLower();
            DirectoryHelper.CreateDirectory(assetBundlePath, true);

            string updatePath = Path.Combine(buildOut, FileNameDefine.UpdateBinDirectory).Replace("\\", "/");
            DirectoryHelper.CreateDirectory(updatePath, true);
            //必须在BuildPipeline.BuildAssetBundles之前，否则BuildPipeline.BuildAssetBundles之后，target就会为空
            if (target != null)
            {
                for (int i = 0; i < target.m_gameAssets.Count; ++i)
                {
                    var gameasset = target.m_gameAssets[i];
                    for (int j = 0; j < gameasset.m_bundles.Count; ++j)
                    {
                        var bundle = gameasset.m_bundles[j];
                        for (int k = 0; k < bundle.m_nomarlDetails.Count; ++k)
                        {
                            string t_normarlPath = bundle.m_nomarlDetails[k];
                            string ext = PathUtils.GetExtension(t_normarlPath);
                            if(ignore_ext != null && ignore_ext.Contains(ext))
                            {
                                continue;
                            }
                            if (File.Exists(t_normarlPath) == false)
                            {
                                EditorUtility.DisplayDialog("提示", $"{t_normarlPath}文件不存在,终止打包流程!!!", "确认");
                                AssetDebugEx.LogError($"{t_normarlPath}文件不存在,终止打包流程!!!");
                                return;
                            }

                            string md5 = string.Empty;
                            if (target.m_isAppendHashToAssetBundleName)
                            {
                                using (FileStream fileStream = new FileStream(t_normarlPath, FileMode.OpenOrCreate))
                                {
                                    md5 = MD5Utils.CalculateMD5(fileStream);
                                }
                            }


                            string destPath = string.Empty;
                            string filePath = string.Empty;
                            if (target.m_isAppendHashToAssetBundleName)
                            {
                                filePath = PathUtils.GetFileNameWithoutExtension(t_normarlPath);
 
                                filePath = $"{filePath}_{md5}{ext}";
                                destPath = PathUtils.Combine(assetBundlePath, filePath);
                            }
                            else
                            {
                                filePath = t_normarlPath;
                                destPath = PathUtils.Combine(assetBundlePath, t_normarlPath);
                            }
                            filePath = filePath.ToLower();
                            destPath = destPath.ToLower();
                            fileBuilds[t_normarlPath] = filePath;
                            FileUtils.FileCopy(t_normarlPath, destPath);
                        }
                    }
                }   
            }
            try
            {
                AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(assetBundlePath, bundleBuilds.ToArray(),
                    bundleOptions,
                    EditorUserBuildSettings.activeBuildTarget);

                PackageUpdateBin(assetBundlePath, updatePath, bundleBuilds, manifest, fileBuilds);
            }
            catch(Exception e)
            {
                AssetDebugEx.LogException(e);
            }
        }
        /// <summary>
        /// 打更新包
        /// </summary>
        /// <param name="accountBundlePath"></param>
        /// <param name="updatePath"></param>
        /// <param name="bundleBuilds"></param>
        /// <param name="manifest"></param>
        private static void PackageUpdateBin(string accountBundlePath, string updatePath,List<AssetBundleBuild> bundleBuilds, AssetBundleManifest manifest,Dictionary<string,string> fileBuilds)
        {
            accountBundlePath = accountBundlePath.Replace("\\", "/");
            var dirctInfo =  Directory.CreateDirectory(accountBundlePath);
            if(dirctInfo == null || dirctInfo.Exists == false)
            {
                Debug.LogError("AssetBundle目录找不到!!!!!!");
                return;
            }

            if(manifest == null)
            {
                Debug.LogError("AssetBundleManifest 为空!!!!!!");
                return;
            }



            if (bundleBuilds == null || bundleBuilds.Count <1)
            {
                Debug.LogError("BundleList 为空!!!!!!");
                return;
            }

            var mainifestBundleList = manifest.GetAllAssetBundles();
            if(mainifestBundleList == null || mainifestBundleList.Length < 1)
            {
                Debug.LogError("AssetBundleManifest bundle 为空!!!!!!");
                return;
            }

            
            Dictionary<string, string> mainifestDic = new Dictionary<string, string>();
            for (int i = 0; i < mainifestBundleList.Length; ++i)
            {
                var mainifestPath = mainifestBundleList[i];
                string orgin = GetOriginBundle(mainifestPath);
                mainifestDic[orgin] = mainifestPath;
            }
            Dictionary<string, AssetBundleBuild> assetBundleDic = new Dictionary<string, AssetBundleBuild>();
            for(int i = 0; i < bundleBuilds.Count;++i)
            {
                var bundleBuild = bundleBuilds[i];
                var key = bundleBuild.assetBundleName.Replace(FileNameDefine.AssetBundletExtension, "");
                assetBundleDic[key] = bundleBuild;
            }


            try
            {
                DirectoryHelper.DeleteAllFileByDirectory(PathUtils.Combine(Application.streamingAssetsPath, FileNameDefine.UpdateBinDirectory),true);

                var scriptable = GetGameAssetsScriptableObject();

                if (scriptable != null)
                {
                    MapCrcVersionData mapCrcVersionData = new MapCrcVersionData();
                    mapCrcVersionData.m_versionsCrc = new MapCrcVersion();
                    MapFileData mapFileData = new MapFileData();
                    mapFileData.m_version = scriptable.m_version;
                    if(scriptable.m_downPath.EndsWith("/") == false)
                    {
                        scriptable.m_downPath = $"{scriptable.m_downPath}/";
                    }
                    mapFileData.m_downPath = scriptable.m_downPath;
                    mapCrcVersionData.m_versionsCrc.m_version = scriptable.m_version;
                    mapCrcVersionData.m_versionsCrc.m_downMapPath = scriptable.m_downMapPath;

                    string jsoncrcPath = PathUtils.Combine(updatePath, FileNameDefine.MapCRCFileName);
                    string mapjsonjsonPath = PathUtils.Combine(updatePath, FileNameDefine.MapFileName);

                    List<MapResInfo> mapResInfos = new List<MapResInfo>();
                    for (int i = 0; i < scriptable.m_gameAssets.Count; ++i)
                    {
                        long startPos = 0;
                        var asset = scriptable.m_gameAssets[i];
                        string binPath = Path.Combine(updatePath, string.Format(FileNameDefine.MapFileBinName, i));
                        using (FileStream stream = new FileStream(binPath, FileMode.OpenOrCreate))
                        {
                            BinaryWriter writer = new BinaryWriter(stream);
                            if (asset.m_bundles != null)
                            {
                                for (int j = 0; j < asset.m_bundles.Count; ++j)
                                {
                                    var bundle = asset.m_bundles[j];
                                    for(int k = 0; k < bundle.m_bundleDetails.Count; ++k)
                                    {
                                        var bundleDetail = bundle.m_bundleDetails[k];
                                        if(mainifestDic.TryGetValue(bundleDetail, out var t_realbundlePath))
                                        {
                                            
                                         
                                        }
                                        else
                                        {
                                            t_realbundlePath = bundleDetail;
                                        }

                                        string  t_bundlePath = Path.Combine(accountBundlePath, $"{t_realbundlePath}");
                                        t_bundlePath = t_bundlePath.Replace("\\", "/");
                                        t_bundlePath = t_bundlePath.ToLower();
                                        if (File.Exists(t_bundlePath) == false)
                                        {
                                            EditorUtility.DisplayDialog("提示", $"{t_bundlePath}文件不存在,终止打包流程!!!", "确认");
                                            AssetDebugEx.LogError($"{t_bundlePath}文件不存在,终止打包流程!!!");
                                            return;
                                        }

                                        if (bundle.m_isCopyStreamingAssets)
                                        {
                                            string des = PathUtils.Combine(Application.streamingAssetsPath, FileNameDefine.UpdateBinDirectory, FileNameDefine.ResSaveDirectory, t_realbundlePath);
                                            FileUtils.FileCopy(t_bundlePath, des);
                                        }

                                        MapResInfo mapResInfo = new MapResInfo();
                                        mapResInfo.m_bundlePath = bundleDetail;
                                        mapResInfo.m_isForceUpdate = bundle.m_isForceUpdate;
                                        mapResInfo.m_isResident = bundle.m_isResident;
                                        using (FileStream fileStream = new FileStream(t_bundlePath, FileMode.OpenOrCreate))
                                        {
                                            mapResInfo.m_length = fileStream.Length;
                                            mapResInfo.m_startPos = startPos;
                                            mapResInfo.m_path = t_realbundlePath;
                                            BinaryReader reader = new BinaryReader(fileStream);
                                            var bytes = reader.ReadBytes((int)fileStream.Length);
                                            writer.Write(bytes);
                                            startPos += bytes.Length;
                                            //计算CRC Position 得要重置
                                            fileStream.Position = 0;
                                            mapResInfo.m_crc = CRCUtils.ComputeCRC32(fileStream);
                                            mapResInfo.m_url = string.Format(FileNameDefine.MapFileBinName, i);
                                            if (assetBundleDic.TryGetValue(bundleDetail.Replace(FileNameDefine.AssetBundletExtension, string.Empty), out var assetBundleBuild))
                                            {
                                                mapResInfo.m_assets = assetBundleBuild.assetNames;
                                            }
                                            mapResInfos.Add(mapResInfo);
                                        }
                                    }

                                    for(int k = 0; k < bundle.m_nomarlDetails.Count; ++k)
                                    {
                                        string t_nomarlDetails = bundle.m_nomarlDetails[k];
                                        if (fileBuilds.TryGetValue(t_nomarlDetails, out var t_realbundlePath))
                                        {
                                            t_realbundlePath = t_realbundlePath.Replace("\\", "/");
                                        }
                                        else
                                        {
                                            t_realbundlePath = t_nomarlDetails;
                                        }

                                        string t_bundlePath = Path.Combine(accountBundlePath, $"{t_realbundlePath}");
                                        t_bundlePath = t_bundlePath.Replace("\\", "/");
                                        if (File.Exists(t_bundlePath) == false)
                                        {
                                            EditorUtility.DisplayDialog("提示", $"{t_bundlePath}文件不存在,终止打包流程!!!", "确认");
                                            AssetDebugEx.LogError($"{t_bundlePath}文件不存在,终止打包流程!!!");
                                            return;
                                        }
                                        if (bundle.m_isCopyStreamingAssets)
                                        {
                                            string des = PathUtils.Combine(Application.streamingAssetsPath, FileNameDefine.UpdateBinDirectory, FileNameDefine.ResSaveDirectory, t_realbundlePath);
                                            FileUtils.FileCopy(t_bundlePath, des);
                                        }

                                        MapResInfo mapResInfo = new MapResInfo();
                                        mapResInfo.m_bundlePath = t_nomarlDetails;
                                        mapResInfo.m_isNative = true;
                                        using (FileStream fileStream = new FileStream(t_realbundlePath, FileMode.OpenOrCreate))
                                        {
                                            mapResInfo.m_length = fileStream.Length;
                                            mapResInfo.m_startPos = startPos;
                                            mapResInfo.m_path = t_realbundlePath;
                                            BinaryReader reader = new BinaryReader(fileStream);
                                            var bytes = reader.ReadBytes((int)fileStream.Length);
                                            writer.Write(bytes);
                                            startPos += bytes.Length;
                                            //计算CRC Position 得要重置
                                            fileStream.Position = 0;
                                            mapResInfo.m_crc = CRCUtils.ComputeCRC32(fileStream);
                                            mapResInfo.m_url = string.Format(FileNameDefine.MapFileBinName, i);
                                            mapResInfos.Add(mapResInfo);
                                        }
                                    }
                                }
                            }
                        }        
                    }

                    //重新计算id
                    mapFileData.m_mapResInfos = mapResInfos.ToArray();
                    if (mapFileData.m_mapResInfos != null)
                    {
                        for (int i = 0; i < mapFileData.m_mapResInfos.Length; ++i)
                        {
                            mapFileData.m_mapResInfos[i].m_id = i;
                        }
                    }

                    mapFileData.Initiazalition();
                    for (int j = 0; j < mapFileData.m_mapResInfos.Length; ++j)
                    {
                        var res = mapFileData.m_mapResInfos[j];
                        var depends = manifest.GetAllDependencies(res.m_path);
                        if (depends != null && depends.Length > 0)
                        {
                            res.m_dependsId = new int[depends.Length];
                            for (int k = 0; k < depends.Length; ++k)
                            {
                                string depenstr = depends[k];
                                var mapRes = mapFileData.GetMapResByRealPath(depenstr);
                                if (mapRes == null)
                                {
                                    EditorUtility.DisplayDialog("提示", $"依赖没有找到{depenstr},终止打包流程!!!!", "确认");
                                    return;
                                }
                                int id = mapRes.m_id;
                                if (id < 0)
                                {
                                    EditorUtility.DisplayDialog("提示", $"依赖没有找到{depenstr},终止打包流程!!!!", "确认");
                                    return;
                                }
                                res.m_dependsId[k] = id;
                            }
                        }
                    }

                    string mapjson = JsonUtility.ToJson(mapFileData);
                    File.WriteAllText(mapjsonjsonPath, mapjson);

                    mapCrcVersionData.m_versionsCrc.m_crc = CRCUtils.ComputeCRC32(mapjsonjsonPath);
                    mapCrcVersionData.m_versionsCrc.m_version = mapFileData.m_version;
                    string crcjson = JsonUtility.ToJson(mapCrcVersionData);
                    File.WriteAllText(jsoncrcPath, crcjson);

                    string streammingpath = PathUtils.Combine(Application.streamingAssetsPath, FileNameDefine.UpdateBinDirectory,FileNameDefine.MapFileDirectory);
                    DirectoryHelper.CreateDirectory(streammingpath,true);

                    FileUtils.FileCopy(mapjsonjsonPath, PathUtils.Combine(streammingpath,FileNameDefine.MapFileName));
                    FileUtils.FileCopy(jsoncrcPath, PathUtils.Combine(streammingpath,FileNameDefine.MapCRCFileName));
                    
                    EditorUtility.DisplayDialog("提示", "恭喜您打包成功!!!", "确认");
                    return;
                }
            }
            catch(Exception e)
            {
                EditorUtility.DisplayDialog("提示", $"打包阻断{e}!!!", "确认");
                AssetDebugEx.LogException(e);
                return;
            }
        }

    }
}

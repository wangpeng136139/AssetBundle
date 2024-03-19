using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameAsset
{
    public class Map
    {
        private static MapVersion m_map = null;


        private static bool Initialization()
        {
            return ReLoad();
        }


        public static bool ReLoad()
        {
            try
            {
                string dataPath = PathUtils.Combine(Application.persistentDataPath, FileNameDefine.UpdateBinDirectory, FileNameDefine.MapFileDirectory);
                string streamingAssetsPath = PathUtils.Combine(Application.streamingAssetsPath, FileNameDefine.UpdateBinDirectory,FileNameDefine.MapFileDirectory);
                if (Directory.Exists(dataPath) == false)
                {
                    DirectoryHelper.CopyDirectory(streamingAssetsPath, dataPath);
                }
                m_map = MapVersion.Load(dataPath);
                if (m_map == null)
                {
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                AssetDebugEx.LogException(e);
                return false;
            }
        }

        private static System.Collections.IEnumerator StartUpdateCoro(Action<UpdateState> onStateChange, Action<float,ulong ,ulong, ulong> onProcess, Action<int, object> onFinish)
        {
            onStateChange?.Invoke(UpdateState.UpdateState_OnReady);
            if (false == Initialization())
            {
                AssetDebugEx.LogError("初始化Map文件失败");
                onStateChange?.Invoke(UpdateState.UpdateState_OnFail);
                onFinish?.Invoke(Code.FailInit_Map, null);
                yield break;
            }

            onStateChange?.Invoke(UpdateState.UpdateState_OnCheck);
            string url = StringBuilderHelper.Concat(m_map.GetDownMapUrl(), FileNameDefine.MapCRCFileName);
            var codeCoro  = StartComperaVersion(null);
            yield return codeCoro;
            if(codeCoro.Current is int)
            {
                var code = (int)codeCoro.Current;
                if(code == Code.Sucess)
                {
                    var updateRes = GetNeedDownRes(false);
                    if(updateRes != null)
                    {
                        for(int i = 0; i < updateRes.Length; i++)
                        {
                            var item = updateRes[i];
                            var bundle = Bundle.Creator(item);
                            bundle.StartDown(false);
                        }
                    }
                    onStateChange?.Invoke(UpdateState.UpdateState_OnDowning);
                    yield return WaitForSecondsDefine.WaitForSeconds100ms;
                    var process = 0.0f;
                    while(process < 1.0f)
                    {
                        onProcess?.Invoke(process, DownInfo.GetBandWidth(), DownInfo.GetDownBytes(),DownInfo.GetDownLength());
                        yield return WaitForSecondsDefine.WaitForSeconds100ms;
                        process = DownInfo.GetProcess();
                    }
                    onStateChange?.Invoke(UpdateState.UpdateState_OnSuccess);
                    onProcess?.Invoke(process, DownInfo.GetBandWidth(), DownInfo.GetDownBytes(), DownInfo.GetDownLength());
                    onFinish?.Invoke(Code.Sucess, null);
                    yield break;
                }
            }
            else
            {
                onStateChange?.Invoke(UpdateState.UpdateState_OnFail);
                onFinish?.Invoke(Code.Fail_Unknow, null);
                yield break;
            }
        }
        public static void StartUpdate(Action<UpdateState> onStateChange, Action<float, ulong, ulong, ulong> onProcess, Action<int, object> onFinish)
        {
            if (GameMode.IsUpdate)
            {
                CoroutineRunner.GetInstance().StartCoroutine(StartUpdateCoro(onStateChange, onProcess, onFinish));
            }
            else if (GameMode.IsUseAssetBundle)
            {
                if (false == Initialization())
                {
                    AssetDebugEx.LogError("初始化Map文件失败");
                    onStateChange?.Invoke(UpdateState.UpdateState_OnFail);
                    onFinish?.Invoke(Code.FailInit_Map, null);
                    return;
                }
                onStateChange?.Invoke(UpdateState.UpdateState_OnSuccess);
                onFinish?.Invoke(Code.Sucess, null);
                return;
            }
            else
            {
                onStateChange?.Invoke(UpdateState.UpdateState_OnSuccess);
                onFinish?.Invoke(Code.Sucess, null);
                return;
            }
        }

        public static MapResInfo[] GetNeedDownRes(bool isFix)
        {
            var res  =  GetUpdateRes(isFix);
            return res;
        }

        
        private static  System.Collections.IEnumerator StartComperaVersion(Action<int,object> onFinish)
        {
            // 检查请求是否成功
            #region 下载MapVersion
            string mapFilePath = PathUtils.Combine(Application.persistentDataPath, FileNameDefine.UpdateBinDirectory, FileNameDefine.MapFileDirectory, FileNameDefine.MapFileName);
            string mapFileCrcPath = PathUtils.Combine(Application.persistentDataPath, FileNameDefine.UpdateBinDirectory, FileNameDefine.MapFileDirectory, FileNameDefine.MapCRCFileName);
            string url = StringBuilderHelper.Concat(m_map.GetDownMapUrl(), FileNameDefine.MapCRCFileName);
            var requestCoroutine = HttpRequest.SendHttpRequest(url, null);
            yield return requestCoroutine; // 等待SendHttpRequest协程的完成，并接收返回值
            bool needDownMap = false;
            string responsecrc = string.Empty;
            string responsemap = string.Empty;
            MapCrcVersionData mapCrcVersionData = null;
            MapFileData mapFileData = null;
            int code = Code.Sucess;
            if (requestCoroutine.Current is string)
            {
                responsecrc = requestCoroutine.Current as string;
                if (string.IsNullOrEmpty(responsecrc))
                {
                    code = Code.FailLoad_MapResVersion;
                    AssetDebugEx.LogError("map version文件为空");
                }

                try
                {
                    if(code == Code.Sucess)
                    {
                        //对比crc文件
                        mapCrcVersionData = MapCrcVersionData.LoadByJson(responsecrc);

                        if (m_map.CompareCrc(mapCrcVersionData) > 0)
                        {
                            needDownMap = true;
                        }
                        else
                        {
                            uint crc = CRCUtils.ComputeCRC32(mapFilePath);
                            if (crc == 0 || crc != mapCrcVersionData.m_versionsCrc.m_crc)
                            {
                                needDownMap = true;
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    AssetDebugEx.LogException(e);
                    code = Code.FailLoad_MapResVersionExcepition; 
                }
           
            }
            else if (requestCoroutine.Current is Exception error)
            {
                // 处理错误
                code = Code.FailLoad_ReturnMapResVersionExcepition;
                Debug.LogException(error);
            }
            else
            {
                code = Code.Fail_Unknow;
                // 处理其他情况
                Debug.LogWarning($"Unexpected result: { requestCoroutine.Current}");
            }
            #endregion

            // 检查请求是否成功
            #region 下载Map
            if(needDownMap == true && code == Code.Sucess)
            {
                url = StringBuilderHelper.Concat(m_map.GetDownMapUrl(), FileNameDefine.MapFileName);
                requestCoroutine = HttpRequest.SendHttpRequest(url, null);
                yield return requestCoroutine; // 等待SendHttpRequest协程的完成，并接收返回值
                if (requestCoroutine.Current is string)
                {
                    responsemap = requestCoroutine.Current as string;
                    
                    if (string.IsNullOrEmpty(responsemap))
                    {
                        AssetDebugEx.LogError("map  json文件为空");
                        code = Code.FailLoad_MapResVersion;
                    }

                    try
                    {
                        if (code == Code.Sucess)
                        {
                            //申请Map文件
                            mapFileData = MapFileData.LoadByJson(responsemap);
                            bool b = FileUtils.WriteAllText(mapFilePath, responsemap);
                            if (false == b)
                            {
                                AssetDebugEx.LogError("写入Map文件失败");
                                code = Code.FailLoad_MapResWriteExcepition;
                            }

                            if(code == Code.Sucess)
                            {
                                b = FileUtils.WriteAllText(mapFileCrcPath, responsecrc);
                                if (false == b)
                                {
                                    AssetDebugEx.LogError("写入MapCrc文件失败");
                                    code = Code.FailLoad_MapResVersionWriteExcepition;
                                }
                            }

                            if (code == Code.Sucess)
                            {
                                m_map.UpdateMapFileData(mapFileData);
                                m_map.UpdateMapCrcVersionData(mapCrcVersionData);
                            }

                        }
                    }
                    catch (Exception e)   
                    {
                        code = Code.FailLoad_MapResExcepition;
                        AssetDebugEx.LogException(e);
                    }

                }
                else if (requestCoroutine.Current is Exception error)
                {
                    // 处理错误
                    AssetDebugEx.LogException(error);
                    code = Code.Fail_Unknow;
                }
                else
                {
                    // 处理其他情况
                    AssetDebugEx.LogError($"Unexpected result: {requestCoroutine.Current}");
                    code = Code.Fail_Unknow;
                }
            }

            if(onFinish != null)
            {
                onFinish?.Invoke(code, null);
                yield break;
            }

            yield return code;
            #endregion
        }


        public static string GetPathByID(int id)
        {
            if (null == m_map)
            {
                return null;
            }
            return m_map.GetPathByID(id);
        }

        public static MapResInfo GetMapResByAssetPath(string path)
        {
            if (null == m_map)
            {
                return null;
            }
            return m_map.GetMapResByAssetPath(path);
        }
        public static int GetIDByPath(string path)
        {
            if (null == m_map)
            {
                return -1;
            }
            return m_map.GetIDByPath(path);
        }

        public static MapResInfo GetMapResByPath(string path)
        {
            if(null == m_map)
            {
                return null;
            }
            return m_map.GetMapResByPath(path);
        }

        public static MapResInfo GetMapResByID(int id)
        {
            if(m_map == null)
            {
                return null;
            }
            return m_map.GetMapResByID(id);
        }


        public static string GetSavePath(MapResInfo resInfo)
        {
            if(null == m_map || null == resInfo)
            {
                return string.Empty;
            }

            return PathUtils.Combine(Application.persistentDataPath, FileNameDefine.UpdateBinDirectory,FileNameDefine.ResSaveDirectory, resInfo.m_path);
        }

        public static string GetDownPath(MapResInfo resInfo)
        {
            if(null == m_map || null == resInfo)
            {
                return string.Empty;
            }

            return StringBuilderHelper.Concat(m_map.GetDownUrl() ,resInfo.m_url);
        }

        public static string GetDownMapUrl()
        {
            if (null == m_map)
            {
                return string.Empty;
            }

            return m_map.GetDownMapUrl();
        }

        public static int GetBundleCount()
        {
            if(null == m_map)
            {
                return 0;
            }
            return m_map.GetBundleCount();
        }


        public static MapResInfo[] GetUpdateRes(bool isFix)
        {
            if(m_map == null)
            {
                return null;
            }
            List<MapResInfo> listRes = new List<MapResInfo>();
            var res = m_map.MapFileData.m_mapResInfos;
            if (res != null)
            {
                for(int i = 0; i < res.Length; ++i)
                {
                    var item = res[i];
                    if(item != null)
                    {
                        if(item.m_isForceUpdate == true)
                        {
                            if (IsNeedDown(item.m_path, item.m_length, item.m_crc, out string fullPath, isFix))
                            {
                                listRes.Add(item);
                            }
                        }
                    }
                }
            }
            return listRes.ToArray();
        }


        public static bool IsNeedDown(string path, long length ,uint crc ,out string fullPath,bool isFix)
        {
            if(string.IsNullOrEmpty(path))
            {
                fullPath = string.Empty;
                return false;
            }

            if (false == PathUtils.Exist(path, out fullPath))
            {
                return true;
            }

            FileInfo fileInfo = new FileInfo(fullPath);
            if (fileInfo.Exists)
            {
                if(fileInfo.Length != length)
                {
                    return false;
                }

                if(isFix)
                {
                   var fileCrc =  CRCUtils.ComputeCRC32(fullPath, true);
                   if(fileCrc != crc )
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public static bool Exist(string path, out string fullPath)
        {
            return PathUtils.Exist(path, out fullPath);
        }
    }
}
    
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

namespace GameAsset
{
    public class FileAsset
    {

        private static List<FileAsset> s_PrepareBundle = new List<FileAsset>();

        private static List<FileAsset> s_LoadingBundle = new List<FileAsset>();

        public static readonly int MaxLoadingCount = 10;
        public static void OnUpdateAll()
        {
            if (s_LoadingBundle.Count < MaxLoadingCount)
            {
                if (s_PrepareBundle.Count > 0)
                {
                    var temp = s_PrepareBundle[0];
                    s_PrepareBundle.RemoveAt(0);
                    s_LoadingBundle.Add(temp);
                }
            }

            for (int i = s_LoadingBundle.Count - 1; i > -1; --i)
            {
                var loadingitem = s_LoadingBundle[i];
                loadingitem.OnUpdate();
                if (loadingitem.IsDone)
                {
                    s_LoadingBundle.RemoveAt(i);
                }
            }
        }


        public static FileAsset Load(string filePath, bool isNative, bool Immediate)
        {
           
            if (false == FileUtils.Exist(filePath))
            {
                AssetDebugEx.LogError($"文件不存在{filePath}");
                return null;
            }

            FileAsset fileAsset = new FileAsset(filePath);
            if (Immediate)
            {
                if (isNative)
                {
                    fileAsset.ReadFile(filePath);
                }
                else
                {
                    fileAsset.LoadAssetBundle(filePath);
                }
            }
            else
            {
                if (isNative)
                {
                    fileAsset.ReadFileAsync(filePath);
                }
                else
                {
                    fileAsset.LoadAssetBundleAsync(filePath);
                }
            }
            return fileAsset;
        }

        public LoadedState loadedState { get; protected set; } = LoadedState.None;

        public bool IsDone { get { return loadedState == LoadedState.Error || loadedState == LoadedState.Compelete; } }

        private AssetBundle m_assetBundle = null;
        private AssetBundleCreateRequest m_assetBundleRequest = null;
        private long m_fileSize = 0;
        private long m_totalBytesRead = 0;

        private byte[] m_bytes = null;

        private string m_filePath = string.Empty;

        private FileStream m_fileStream = null;
        public FileAsset(string filePath)
        {
            m_filePath = filePath;
        }

        public float Progress
        {
            get
            {
                if(m_assetBundleRequest != null)
                {
                    return m_assetBundleRequest.progress;
                }

                if(m_bytes != null)
                {
                    return (float)m_totalBytesRead / m_fileSize;
                }

                if(m_assetBundle != null)
                {
                    return 1;
                }

                return 0;
            }
        }


        private void OnUpdate()
        {
            if(m_assetBundleRequest != null)
            {
                if(m_assetBundleRequest.isDone == false)
                {
                    return;
                }
                m_assetBundle = m_assetBundleRequest.assetBundle;
                if(m_assetBundle == null)
                {
                    loadedState = LoadedState.Error;
                }
                else
                {
                    loadedState = LoadedState.Compelete;
                }
               
                m_assetBundleRequest = null;
            }
            else if(m_bytes != null)
            {
                try
                {
                    if (m_totalBytesRead < m_fileSize)
                    {
                        int length = (int)(m_fileSize - m_totalBytesRead) > 4096 ? 4096 : (int)(m_fileSize - m_totalBytesRead);
                        int bytesRead = m_fileStream.Read(m_bytes, (int)m_totalBytesRead, length);
                        m_totalBytesRead += bytesRead;
                    }
                    else
                    {
                        m_fileStream?.Close();
                        m_fileStream?.Dispose();
                        m_fileStream = null;
                        loadedState = LoadedState.Compelete;
                    }
                }
                catch(Exception e)
                {
                    AssetDebugEx.LogException(e);
                    loadedState = LoadedState.Error;
                }
            }

        }




        private void ReadFile(string filePath)
        {
            try
            {
                loadedState = LoadedState.Loading;
                m_bytes = File.ReadAllBytes(filePath);
                loadedState = m_bytes == null ? LoadedState.Error : LoadedState.Compelete;
            }
            catch (Exception ex)
            {
                loadedState = LoadedState.Error;
                AssetDebugEx.LogError($"Failed to read file: {ex.Message}");
                return;
            }
        }


        private void ReadFileAsync(string filePath)
        {
            try
            {
                loadedState = LoadedState.Loading;
                FileInfo fileInfo = new FileInfo(filePath);
                m_fileSize = fileInfo.Length;
                m_bytes = new byte[m_fileSize];
                m_totalBytesRead = 0;
                m_fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            catch(Exception ex)
            {
                m_bytes = null;
                m_fileStream?.Close();
                m_fileStream?.Dispose();
                m_fileStream = null;
                loadedState = LoadedState.Error;
                AssetDebugEx.LogException(ex);
                return;
            }
         
        }

        private void LoadAssetBundle(string filePath)
        {
            try
            {
                m_assetBundle = AssetBundle.LoadFromFile(filePath);
            }
            catch (Exception ex)
            {
                AssetDebugEx.LogError($"Failed to load AssetBundle: {ex.Message}");
                loadedState = LoadedState.Error;
                return;
            }
        }

        private void LoadAssetBundleAsync(string filePath)
        {
            try
            {
                loadedState = LoadedState.Loading;
                m_assetBundleRequest = AssetBundle.LoadFromFileAsync(filePath);
                if(m_assetBundleRequest == null)
                {
                    loadedState = LoadedState.Error;
                    return;
                }
                s_PrepareBundle.Add(this);
            }
            catch (Exception ex)
            {
                AssetDebugEx.LogException(ex);
                loadedState = LoadedState.Error;
            }
        }

        public UnityEngine.Object LoadAsset(string assetName)
        {
            if(m_assetBundle != null)
            {
                return m_assetBundle.LoadAsset(assetName);
            }
            return null;
        }

        public byte[] GetBytes()
        {
            return m_bytes;
        }
        public void UnLoad(bool unloadAllLoadedObjects = false)
        {
            m_bytes = null;
            if(m_assetBundle != null)
            {
                m_assetBundle.Unload(unloadAllLoadedObjects);
            }
            m_assetBundleRequest = null;
        }

        public AssetBundleRequest LoadAssetAsync(string path)
        {
            if(m_assetBundle != null)
            {
                return m_assetBundle.LoadAssetAsync(path);
            }
            return null;
        }
    }
}

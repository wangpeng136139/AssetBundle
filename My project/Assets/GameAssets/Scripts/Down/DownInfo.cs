using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace GameAsset
{
    public enum DownloadStatus
    {
        None,
        Wait,
        Progressing,
        Success,
        Failed
    }

    public class DownInfo
    {
        public static int MaxDownCount = 10;
        private static List<DownInfo> s_PreparedDown = new List<DownInfo>();
        private static List<DownInfo> s_ProcessDown = new List<DownInfo>();
        private static Dictionary<string, DownInfo> s_CacheDown = new Dictionary<string, DownInfo>();
        public static List<DownInfo> S_PreparedDown => s_PreparedDown;
        public static List<DownInfo> S_ProcessDown => s_ProcessDown;

        public static Dictionary<string, DownInfo> S_CacheDown => s_CacheDown;



        /// <summary>
        /// 获取带宽byte/s
        /// </summary>
        /// <returns></returns>
        public static ulong GetBandWidth()
        {
            ulong speed = 0;
            for(int i = 0; i < s_ProcessDown.Count; i++)
            {
                speed += s_ProcessDown[i].m_bandwidth;
            }
            return speed;
        }


        public static int GetDownCount()
        {
            return (s_ProcessDown.Count + s_PreparedDown.Count);
        }

        public static ulong GetDownBytes()
        {
            ulong bytes = 0;
            for (int i = 0; i < s_ProcessDown.Count; i++)
            {
                bytes += (ulong)s_ProcessDown[i].m_downloadedBytes;
            }
            return bytes;
        }

        public static ulong GetDownLength()
        {
            ulong length = 0;
            for (int i = 0; i < s_ProcessDown.Count; i++)
            {
                length += (ulong)s_ProcessDown[i].m_length;
            }

            for(int i = 0;i < s_PreparedDown.Count; i++)
            {
                length += (ulong)s_PreparedDown[i].m_length;
            }   
            return length;
        }
        public static float GetProcess()
        {
           
            if (s_ProcessDown.Count > 0 || s_PreparedDown.Count > 0)
            {
                float process = 0;
                for (int i = 0; i < s_ProcessDown.Count; i++)
                {
                    process += s_ProcessDown[i].Process;
                }
                return process / (s_ProcessDown.Count + s_PreparedDown.Count);
            }
            return 1.0f;
        }



        public static DownInfo StartDown(string url, string savePath, long startIndex, long length, uint crc, bool bImmediate)
        {
            if (s_CacheDown.TryGetValue(savePath, out var downInfo))
            {
                return downInfo;
            }

            downInfo = new DownInfo(url, savePath, startIndex, length, crc);
            downInfo.DownloadStatus = DownloadStatus.Wait;
            if (!bImmediate)
            {
                s_ProcessDown.Add(downInfo);
                s_CacheDown.Add(downInfo.SavePath, downInfo);
                downInfo.Start();
            }
            else
            {
                s_PreparedDown.Add(downInfo);
            }

            return downInfo;
        }


        public static void OnUpdateAll()
        {
            var minCount = Math.Min(MaxDownCount - s_ProcessDown.Count, s_PreparedDown.Count);
            for (int i = minCount - 1; i > -1; --i)
            {
                var item = s_PreparedDown[i];
                s_PreparedDown.RemoveAt(i);
                s_ProcessDown.Add(item);
                item.DownloadStatus = DownloadStatus.Wait; ;
                s_CacheDown.Add(item.SavePath, item);
                item.Start();
            }


            for (int i = s_ProcessDown.Count - 1; i > -1; --i)
            {
                var download = s_ProcessDown[i];
                if (false == download.IsDone) continue;
                s_ProcessDown.RemoveAt(i);
            }
        }
        /// <summary>
        /// FTD验证ID
        /// </summary>
        public static string FtpUserID;
        public static string FtpPassword;


        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
        SslPolicyErrors spe)
        {
            return true;
        }

        /// <summary>
        /// 线程
        /// </summary>
        private Thread m_thread = null;

        /// <summary>
        /// url
        /// </summary>
        protected string m_url = string.Empty;
        /// <summary>
        /// 开始位置
        /// </summary>
        protected long m_startPos = 0;
        /// <summary>
        /// 下载长度
        /// </summary>
        protected long m_length = 0;
        /// <summary>
        /// 文件CRC
        /// </summary>
        protected uint m_crc = 0;
        /// <summary>
        /// 下载字节数
        /// </summary>
        protected long m_downloadedBytes = 0;
        /// <summary>
        /// 下载状态
        /// </summary>
        public DownloadStatus DownloadStatus = DownloadStatus.None;
        /// <summary>
        /// 保存路径
        /// </summary>
        private string m_savePath = string.Empty;
        /// <summary>
        /// 文件写入流
        /// </summary>
        protected FileStream m_writer = null;
        //写入文件
        private readonly byte[] _readBuffer = new byte[1024 * 4];
        //错误信息
        private string error = string.Empty;
        /// <summary>
        /// 最大下载数量
        /// </summary>
        private readonly int MaxRetryTimes = 3;
        /// <summary>
        /// 当前重试次数
        /// </summary>
        private int retryTimes = 0;
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsDone => DownloadStatus == DownloadStatus.Failed || DownloadStatus == DownloadStatus.Success;
        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath { get => m_savePath; }
        /// <summary>
        /// 下载速度bytes/s
        /// </summary>
        public long m_lastdownloadedBytes { get; private set; } = 0;

        public float Process { get => (float)m_downloadedBytes * 1.0f / m_length; }
        /// <summary>
        /// 采样时间
        /// </summary>
        private double m_bandwidthSampleTime = 0.0f;
        /// <summary>
        /// 带宽
        /// </summary>
        public ulong m_bandwidth { get; protected internal set; }
        public DownInfo(string url, string savePath, long startIndex, long length, uint crc)
        {
            this.m_savePath = savePath;
            this.m_url = url;
            this.m_startPos = startIndex;
            this.m_length = length;
            this.m_crc = crc;
            this.m_bandwidth = 0;
            this.m_bandwidthSampleTime = 0.0f;
        }

        public WebRequest CreateWebRequest()
        {
            WebRequest request;
            if (m_url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                request = GetHttpWebRequest();
            }
            else if (m_url.StartsWith("ftp", StringComparison.OrdinalIgnoreCase))
            {
                var ftpWebRequest = (FtpWebRequest)WebRequest.Create(m_url);
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                if (!string.IsNullOrEmpty(FtpUserID))
                    ftpWebRequest.Credentials = new NetworkCredential(FtpUserID, FtpPassword);

                if (m_downloadedBytes > 0) ftpWebRequest.ContentOffset = m_downloadedBytes;

                request = ftpWebRequest;
            }
            else
            {
                request = GetHttpWebRequest();
            }

            return request;
        }


        public WebRequest GetHttpWebRequest()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(m_url);
            httpWebRequest.ProtocolVersion = HttpVersion.Version10;
            httpWebRequest.Timeout = 30 * 1000;//超时定位30毫秒
            httpWebRequest.AddRange(m_startPos + m_downloadedBytes, m_startPos+m_length-1);
            return httpWebRequest;
        }


        private void Downloading()
        {
            var request = CreateWebRequest();
            request.Proxy = null;
            using (var response = request.GetResponse())
            {
                if (response.ContentLength > 0)
                {
                    if (m_length == 0) m_length = response.ContentLength + m_downloadedBytes;

                    using (var reader = response.GetResponseStream())
                    {
                        if (m_downloadedBytes >= m_length) return;
                        var startTime = DateTime.Now;
                        while (DownloadStatus == DownloadStatus.Progressing)
                        {
                            if (ReadToEnd(reader)) break;
                        }
                    }
                }
                else
                {
                    DownloadStatus = DownloadStatus.Success;
                }
            }
        }

        private bool ReadToEnd(Stream reader)
        {
            var len = reader.Read(_readBuffer, 0, _readBuffer.Length);
            if (len <= 0) return true;
            var dt = GetRealtimeSinceBeginSample();
            if (dt > 1000) BeginSample();
            if(dt > 0 && m_lastdownloadedBytes > 0)
            {
                m_bandwidth = (ulong)(len / dt) * 1000;
            }
           
            m_writer.Write(_readBuffer, 0, len);
            m_lastdownloadedBytes += len;
            m_downloadedBytes += len;
            return false;
        }


        private void Run()
        {
            try
            {
                Downloading();
                CloseWrite();
                if (DownloadStatus == DownloadStatus.Failed) return;

                if (m_downloadedBytes != m_length)
                {
                    error = $"下载大小不匹配 {m_downloadedBytes} and  {m_length}";
                    AssetDebugEx.LogError(error);
                    if (CanRetry()) return;
                    DownloadStatus = DownloadStatus.Failed;
                    return;
                }

                if (m_crc != 0)
                {
                    var crc = CRCUtils.ComputeCRC32(SavePath,true);
                    if (this.m_crc != crc)
                    {
                        File.Delete(SavePath);
                        error = $"CRC不匹配 {crc} and {this.m_crc}";
                        AssetDebugEx.LogError(error);
                        if (CanRetry()) return;

                        DownloadStatus = DownloadStatus.Failed;
                        return;
                    }
                }


                DownloadStatus = DownloadStatus.Success;
            }
            catch (Exception e)
            {
                AssetDebugEx.LogException(e);
                CloseWrite();
                if (CanRetry()) return;

                DownloadStatus = DownloadStatus.Failed;
            }
        }

        private bool CanRetry()
        {
            if (retryTimes < MaxRetryTimes)
            {
                Thread.Sleep(1000);
                Retry();
                retryTimes++;
                return true;
            }

            return false;
        }

        public void Retry()
        {
            DownloadStatus = DownloadStatus.Wait;
            AssetDebugEx.Log($"尝试重新下载{m_url} 保存路径:{SavePath},次数:{retryTimes}");
            Start();
        }

        private void CloseWrite()
        {
            if (m_writer != null)
            {
                m_writer.Flush();
                m_writer.Close();
                m_writer = null;
            }
        }


        private void BeginSample()
        {
            m_lastdownloadedBytes = 0;
            m_bandwidthSampleTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
        }

        private double GetRealtimeSinceBeginSample()
        {
            return DateTime.Now.TimeOfDay.TotalMilliseconds - m_bandwidthSampleTime;
        }


        public void Start()
        {
            if (DownloadStatus != DownloadStatus.Wait) return;
            AssetDebugEx.Log("开始下载 {0}", m_url);
            DownloadStatus = DownloadStatus.Progressing;
            BeginSample();
            var file = new FileInfo(SavePath);
            if (file.Exists && file.Length > 0)
            {
                if (m_length > 0 && file.Length == m_length)
                {
                    DownloadStatus = DownloadStatus.Success;
                    return;
                }

                try
                {
                    File.Delete(SavePath);
                    m_writer = File.Create(SavePath);
                    m_downloadedBytes = 0;
                    /*LogUtil.Log("开始断点续传");
                    writer = File.OpenWrite(info.savePath);
                    downloadedBytes = writer.Length - 1;
                    if (downloadedBytes > 0) writer.Seek(-1, SeekOrigin.End);*/
                }
                catch (Exception e)
                {
                    AssetDebugEx.LogException(e);
                    CloseWrite();
                    DownloadStatus = DownloadStatus.Failed;
                    return;
                }
            }
            else
            {
                var dir = System.IO.Path.GetDirectoryName(SavePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

                m_writer = File.Create(SavePath);
                m_downloadedBytes = 0;
            }

            m_thread = new Thread(Run)
            {
                IsBackground = true
            };
            m_thread.Start();
        }
    }
}

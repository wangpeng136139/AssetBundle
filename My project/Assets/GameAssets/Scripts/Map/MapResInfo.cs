using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAsset
{
    [Serializable]
    public class MapResInfo
    {
        public int m_id;
        public int[] m_dependsId;
        /// <summary>
        /// 文件名+md5
        /// </summary>
        public string m_path = string.Empty;
        public long m_startPos = 0;
        public long m_length = 0;
        public uint m_crc = 0;
        /// <summary>
        /// 资产名
        /// </summary>
        public string m_bundlePath = string.Empty;
        /// <summary>
        /// 归属更新包
        /// </summary>
        public string m_url = string.Empty;
        /// <summary>
        /// 资产Path
        /// </summary>
        public string[] m_assets = null;
        /// <summary>
        /// 是否使用原生数据
        /// </summary>
        public bool m_isNative = false;
        /// <summary>
        /// 是否强制更新
        /// </summary>
        public bool m_isForceUpdate = false;
        /// <summary>
        /// 是否常驻内存
        /// </summary>
        public bool m_isResident = false;
    }
}

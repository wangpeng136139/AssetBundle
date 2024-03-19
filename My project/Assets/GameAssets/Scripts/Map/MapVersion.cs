using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameAsset
{
    [Serializable]
    public class MapVersion
    {
        private MapFileData m_mapFileData = null;

        private MapCrcVersionData m_mapVersionCrcData = null;

        public MapFileData MapFileData
        {
            get
            {
                return m_mapFileData;
            }
        }


        public void UpdateMapFileData(MapFileData data)
        {
            m_mapFileData = data;
        }

        public void UpdateMapCrcVersionData(MapCrcVersionData data)
        {
            m_mapVersionCrcData = data;
        }
        public static MapVersion Load(string path)
        {
            try
            {
                MapVersion mapVersion = new MapVersion();
                mapVersion.m_mapVersionCrcData = MapCrcVersionData.LoadByPath(path);
                if (mapVersion.m_mapVersionCrcData == null)
                {
                    AssetDebugEx.LogError("map Crc 为空!!!");
                    return mapVersion;
                }

                mapVersion.m_mapFileData = MapFileData.LoadByPath(path);
                if (mapVersion.m_mapFileData == null)
                {
                    AssetDebugEx.LogError("map 文件是空!!!");
                    return mapVersion;
                }
                mapVersion.m_mapFileData.Initiazalition();
                return mapVersion;
            }
            catch(Exception e)
            {
                AssetDebugEx.LogException(e);
                return null;
            }
        }

        public int CompareCrc(MapCrcVersionData crc)
        {
            return m_mapVersionCrcData.CompareTo(crc);
        }


        public string GetDownMapUrl()
        {
            if (null == m_mapVersionCrcData || null == m_mapVersionCrcData.m_versionsCrc)
            {
                return string.Empty;
            }
            return m_mapVersionCrcData.m_versionsCrc.m_downMapPath;
        }

        public string GetDownUrl()
        {
            if(null == m_mapFileData)
            {
                return string.Empty;
            }
            return m_mapFileData.m_downPath;
        }

        public MapResInfo GetMapResByPath(string path)
        {
            if (m_mapFileData == null)
            {
                return null;
            }
            return m_mapFileData.GetMapResByPath(path);
        }

        public MapResInfo GetMapResByID(int id)
        {
            if (m_mapFileData == null)
            {
                return null;
            }
            return m_mapFileData.GetMapResByID(id);
        }

        public MapResInfo GetMapResByAssetPath(string path)
        {
            if (m_mapFileData == null)
            {
                return null;
            }

            return m_mapFileData.GetMapResByAssetPath(path);
        }

        public string GetPathByID(int id)
        {
            if (m_mapFileData == null)
            {
                return string.Empty;
            }

            return m_mapFileData.GetPathByID(id);
        }



        public int GetBundleCount()
        {
            if(m_mapFileData == null)
            {
                return 0;
            }

            return m_mapFileData.GetBundleCount();

        }

        public int GetIDByPath(string path)
        {
            if (m_mapFileData == null)
            {
                return -1;
            }
            return m_mapFileData.GetIDByPath(path);
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameAsset
{
    [Serializable]
    public class MapFileData
    {
        [SerializeField]
        public GameVersion m_version;
        [SerializeField]
        public string m_downPath = string.Empty;
        [SerializeField]
        public MapResInfo[] m_mapResInfos = null;
        [NonSerialized]
        public Dictionary<string, MapResInfo> m_dicByPath = new Dictionary<string, MapResInfo>();
        [NonSerialized]
        public Dictionary<string, MapResInfo> m_dicByAssetPath = new Dictionary<string, MapResInfo>();
        [NonSerialized]
        public Dictionary<string, MapResInfo> m_dicByRealPath = new Dictionary<string, MapResInfo>();

        public static MapFileData LoadByPath(string path)
        {
            string mapFilePath = PathUtils.Combine(path, FileNameDefine.MapFileName);
            string mapFileJson = FileUtils.ReadAllText(mapFilePath);
            return LoadByJson(mapFileJson);
        }

        public static MapFileData LoadByJson(string json)
        {
            return JsonUtility.FromJson<MapFileData>(json);
        }


        public static MapResInfo[] GetACompareList(MapFileData a, MapFileData b)
        {
            List<MapResInfo> list = new List<MapResInfo>();
            if(a == null && b == null)
            {
                return null;
            }

            if(a == null || a.m_mapResInfos == null)
            {
                return null;
            }

            for(int i = 0; i < a.m_mapResInfos.Length; ++i)
            {
                var item = a.m_mapResInfos[i];
                string bundlepath = item.m_bundlePath;
                var res = b.GetMapResByPath(bundlepath);
                if(res == null || res.m_crc != item.m_crc)
                {
                    list.Add(item);
                }
            }
            return list.ToArray();
        }


        public void Initiazalition()
        {
            m_dicByPath.Clear();
            if (m_mapResInfos != null)
            {
                for (int i = 0; i < m_mapResInfos.Length; ++i)
                {
                    var item = m_mapResInfos[i];
                    if (item.m_assets != null)
                    {
                        for (int j = 0; j < item.m_assets.Length; ++j)
                        {
                            string asset = item.m_assets[j];
                            m_dicByAssetPath.Add(asset, item);
                        }
                    }
                    m_dicByPath.Add(item.m_bundlePath, item);
                    m_dicByRealPath.Add(item.m_path, item);
                }
            }
        }

        public MapResInfo GetMapResByPath(string path)
        {
            m_dicByPath.TryGetValue(path, out var value);
            return value;
        }

        public MapResInfo GetMapResByRealPath(string path)
        {
            m_dicByRealPath.TryGetValue(path, out var value);
            return value;
        }

        public MapResInfo GetMapResByAssetPath(string path)
        {
            m_dicByAssetPath.TryGetValue(path, out var value);
            return value;
        }

        public MapResInfo GetMapResByID(int id)
        {
            if (m_mapResInfos == null)
            {
                return null;
            }

            if (id < 0 || id >= m_mapResInfos.Length)
            {
                return null;
            }

            return m_mapResInfos[id];
        }



        public string GetPathByID(int id)
        {
            var res = GetMapResByID(id);
            if (res != null)
            {
                return res.m_path;
            }
            return string.Empty;
        }


        public int GetIDByPath(string path)
        {
            var res = GetMapResByPath(path);
            if (res != null)
            {
                return res.m_id;
            }
            return -1;
        }


        public int GetBundleCount()
        {
            if (m_mapResInfos == null)
            {
                return 0;
            }
            return m_mapResInfos.Length;

        }
    }

}


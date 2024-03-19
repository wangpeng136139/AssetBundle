using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameAsset
{
    [Serializable]
    public class MapCrcVersionData
    {
        public MapCrcVersion m_versionsCrc = null;

        public static MapCrcVersionData LoadByPath(string path)
        {
            try
            {
                string mapCrc = PathUtils.Combine(path, FileNameDefine.MapCRCFileName);
                string mapCrcJson = FileUtils.ReadAllText(mapCrc);
                return LoadByJson(mapCrcJson);
            }
            catch(Exception e)
            {
                AssetDebugEx.LogException(e);
                return null;
            }

        }

        public static MapCrcVersionData LoadByJson(string json)
        {
            try
            {
                return JsonUtility.FromJson<MapCrcVersionData>(json);
            }
            catch (Exception e)
            {
                AssetDebugEx.LogException(e);
                return null;
            }
          
        }

        public int CompareTo(MapCrcVersionData other)
        {
            try
            {
                if (other == null || other.m_versionsCrc == null)
                {
                    return 1;
                }
                int temp = m_versionsCrc.m_version.CompareTo(other.m_versionsCrc.m_version);
                if (temp == 0)
                {
                    if (m_versionsCrc.m_crc != other.m_versionsCrc.m_crc)
                    {
                        return 1;
                    }
                }

                return temp;
            }
            catch(Exception e)
            {
                AssetDebugEx.LogException(e);
            }

            return 1;
        }
    }

}

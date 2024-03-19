using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAsset
{
    [Serializable]
    public class MapCrcVersion
    {
        public uint m_crc = 0;
        public GameVersion m_version;
        public string m_downMapPath = string.Empty;
    }
}
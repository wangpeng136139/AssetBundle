using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameAsset
{
    public static class TextureImporterHelper
    {
        public static void SetTextureSize(TextureImporter textrueImport,int size)
        {
            if(textrueImport != null)
            {
                var setting = textrueImport.GetDefaultPlatformTextureSettings();
                setting.maxTextureSize = size;
                SetPlatformTextureSettings(textrueImport, setting);
            }
        }
        public static void SetTextureImporterFormat(TextureImporter textrueImport, TextureImporterFormat format)
        {
            if (textrueImport != null)
            {
                var setting = textrueImport.GetDefaultPlatformTextureSettings();
                setting.format = format;
                SetPlatformTextureSettings(textrueImport, setting);
            }
        }

        public static void SetPlatformTextureSettings(TextureImporter textrueImport, TextureImporterPlatformSettings setting)
        {
            if (textrueImport != null)
            {
                textrueImport.SetPlatformTextureSettings(setting);
            }
        }
    }

}

using UnityEngine;
using UnityEditor;

namespace GameAsset
{
    public class TextureAssetCheck : AssetPostprocessor
    {
        public enum TextureSize
        {
            x32 = 32,
            x64 = 64,
            x128 = 128,
            x256 = 256,
            x512 = 512,
            x1024 = 1024,
            x2048 = 2048,
            x4096 = 4096
        }


        // ������֮ǰ����
        public void OnPreprocessTexture()
        {
            // �����޸�һЩ���Ի���ʹ�ô�����֤����������Ƿ�Ϻ��淶
            TextureImporter textureImporter = assetImporter as TextureImporter;
            Debug.LogError("Pre Texture:" + textureImporter.assetPath);
            if (textureImporter.maxTextureSize > (int)TextureSize.x2048)
            {
                TextureImporterHelper.SetTextureSize(textureImporter, (int)TextureSize.x2048);
            }
        }

        // ������֮�����
        public void OnPostprocessTexture(Texture2D texture)
        {
            TextureImporter textureImporter = assetImporter as TextureImporter;
            Debug.LogError("Post Texutre:" + assetPath.ToLower());
            Debug.LogError("Post Texture:" + texture.width + "    " + texture.height);
        }
    }
}



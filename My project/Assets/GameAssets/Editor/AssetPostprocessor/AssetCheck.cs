using UnityEngine;
using UnityEditor;

namespace GameAsset
{
    public class AssetCheck : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var asset in importedAssets)
            {
                if(asset.EndsWith(".meta") == false &&  asset.EndsWith(".cs"))
                {
                    continue;
                }

                if(asset.IndexOf("/GameAssets/") > -1)
                {
                    continue;
                }
            }

           /* foreach (var asset in deletedAssets)
            {
                Debug.LogError("delete:" + asset);
            }

            foreach (var asset in movedAssets)
            {
                Debug.LogError("move:" + asset);
            }

            foreach (var asset in movedFromAssetPaths)
            {
                Debug.LogError("moveFromAsset:" + asset);
            }*/
        }

        // ������Դ���붼���������
        public void OnPreprocessAsset()
        {
            //Ŀ¼��ϢҲ�ᱻ��ȡ��
            //Debug.LogError("Asset:" + assetImporter.assetPath);
        }
    }
}



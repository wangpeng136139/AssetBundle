using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAsset
{
    public class Root : MonoBehaviour
    {
        private static Root s_Root = null;
        public static GameObject s_RootObj
        {
            get
            {
                if (s_Root == null)
                {
                    var root = new GameObject("[Root]");
                    s_Root = root.AddComponent<Root>();
                    GameObject.DontDestroyOnLoad(root);
                }
                return s_Root.gameObject;
            }
        }
        public static Transform s_RootTrans
        {
            get
            {
                return s_RootObj.transform;
            }
        }

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameAsset
{
    public static class GameObjectUtility
    {
        public static void SetLayerRecursively(this GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static T FindComponent<T>(this GameObject go,string name) where T : Component
        {
            var child  = go.FindGameObject(name);
            if(child != null)
            {
                return child.GetComponent<T>();
            }
            return null;
        }
        public static T FindComponent<T>(this GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();
            if (comp != null)
            {
                return comp;
            }
            foreach (Transform child in go.transform)
            {
                var com =  child.gameObject.FindComponent<T>();
                if(com != null)
                {
                    return com;
                }
            }
            return null;
        }
        
        public static void FindComponent<T>(this GameObject go,ref List<T> components) where T : Component
        {
            var comp = go.GetComponent<T>();
            if (comp != null)
            {
                components.Add(comp);
            }
            foreach (Transform child in go.transform)
            {
                child.gameObject.FindComponent<T>(ref components);
            }
        }

        public static Component FindComponent(this GameObject go, Type type)
        {
            var comp = go.GetComponent(type);
            if (comp != null)
            {
                return comp;
            }
            foreach (Transform child in go.transform)
            {
                var obj =  child.gameObject.FindComponent(type);
                if(obj != null)
                {
                    return obj;
                }
            }
            return null;
        }

        public static Transform FindTransform(this GameObject go, string name)
        {
            var obj = go.FindGameObject(name);
            if(obj != null)
            {
                return obj.transform;
            }
            return null;
        }


        public static GameObject FindGameObject(this GameObject go, string name)
        {
            if (go.name == name)
            {
                return go;
            }
            foreach (Transform child in go.transform)
            {
               var obj =  child.gameObject.FindGameObject(name);
                if(obj != null)
                {
                    return obj;
                }
            }
            return null;
        }


        public static void FindGameObject(this GameObject go, string name, ref List<GameObject> list)
        {
            if (go.name == name)
            {
                list.Add(go);
            }
            foreach (Transform child in go.transform)
            {
                child.gameObject.FindGameObject(name, ref list);
            }
        }
    }
}

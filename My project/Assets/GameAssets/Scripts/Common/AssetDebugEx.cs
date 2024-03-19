using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameAsset
{
    public static class AssetDebugEx
    {
        public static void Log(params string[] strs)
        {
            var str = StringBuilderHelper.Concat(strs);
            Debug.Log(str);
        }

        public static void LogError(params string[] strs)
        {
            var str = StringBuilderHelper.Concat(strs);
            Debug.LogError(str);
        }

        public static void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}

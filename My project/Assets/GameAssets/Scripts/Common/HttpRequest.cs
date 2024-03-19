
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System;

namespace GameAsset
{
    public static class HttpRequest
    {
        public static void SendHttpRequestAsync(string url,Action<string> action)
        {
            CoroutineRunner.GetInstance().StartCoroutine(SendHttpRequest(url, action));
        }


        public static IEnumerator   SendHttpRequest(string url, Action<string> action)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string responseText = webRequest.downloadHandler.text;
                    action?.Invoke(responseText);
                    yield return responseText; // 返回字符串值
                }
                else
                {
                    Debug.LogError("HTTP request failed: " + webRequest.error);
                    yield return string.Empty; // 返回字符串值
                }
            }
        }

        public static string SendHttpRequestSync(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                webRequest.SendWebRequest();

                while (!webRequest.isDone)
                {
                    // Wait for the request to complete
                }

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string responseText = webRequest.downloadHandler.text;
                    // Process the response JSON here
                    return responseText;
                }
                else
                {
                    Debug.LogError("HTTP request failed: " + webRequest.error);
                    return null;
                }
            }
        }
    }
}

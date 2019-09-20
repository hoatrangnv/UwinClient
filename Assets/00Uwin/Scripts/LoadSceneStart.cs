using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadSceneStart : MonoBehaviour
{
    [SerializeField]
    private string urlGetScene;
    List<Dictionary<string, string>> startValue;
    public PopStartGame popupStart;

    void Start()
    {
        StartCoroutine(VKCommon.DownloadTextFromURL(urlGetScene, (value) =>
        {
            startValue = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(value);

            string hash = startValue[0]["hash"];
            int canStartGame = int.Parse(startValue[0]["canStartGame"]);

            if (canStartGame == 1)
            {
#if UNITY_EDITOR
                string url = startValue[0]["urlWin"];
                StartCoroutine(DownloadScene(url, hash));
#elif UNITY_ANDROID
                string url = startValue[0]["urlAndroid"];
                StartCoroutine(DownloadScene(url, hash));
#elif UNITY_IOS
                string url = startValue[0]["urlIOS"];
                StartCoroutine(DownloadScene(url, hash));
#endif
            }
            else
            {
                string message = startValue[0]["message"];
                popupStart.SetNotice(message);

                return;
            }
        }));
    }

    private IEnumerator DownloadScene(string urlSceneBundle, string hash)
    {
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(urlSceneBundle, Hash128.Parse(hash), 0);
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        ulong totalBytes = 0;
        while (!operation.isDone)
        {
            if (totalBytes == 0)
            {
                string length = request.GetResponseHeader("Content-Length");
                if (length != null)
                {
                    totalBytes = ulong.Parse(length);
                }
            }

            yield return new WaitForEndOfFrame();
        }
        AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);

        if (assetBundle.isStreamedSceneAssetBundle)
        {
            string[] scenePaths = assetBundle.GetAllScenePaths();
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);

            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            if (async == null)
                yield break;
        }
    }
}

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadSceneStart : MonoBehaviour
{
    List<Dictionary<string, string>> startValue;

    public PopStartGame popupStart;
    private string urlGetScene = "https://script.googleusercontent.com/macros/echo?user_content_key=zVne-cxdxeRmW1cD6gQrmyiBysejP1DPnO755fBcTw3WgDQ3LfD6id8uTPZ4MpVV2GO5WugoDRrruZ2pA2kaPvkNfa79uGbgm5_BxDlH2jW0nuo2oDemN9CCS2h10ox_1xSncGQajx_ryfhECjZEnP7KCTBjVJrk4qsEB6LwEGXvnBf-bsruUn7ek-0PWKje6A-XnLZ0xMfpXBRCsVGbd50mgLDwm9cO&lib=MvKeuOV5026TnJu1afN8k5Z3BOh0vXkHY";
    public Text notice;


    void Awake()
    {
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Start()
    {
        updateText = true;
        notice.text = "Đang tải game";

        GetText();
    }
    private void GetText()
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
        }, () => ErrorConnect()));
    }

    private void ErrorConnect()
    {
        updateText = false;
        notice.text = "";

        popupStart.SetPopup("Hãy kiểm tra kết nối mạng và thử lại!");
        popupStart.SetActionForceUpdate((value) =>
        {
            if (value)
            {
                updateText = true;
                notice.text = "Đang tải game";
                GetText();
            }
            else
            {
                popupStart.gameObject.SetActive(false);
            }
        });
    }

    public float timeUpdateText;
    public float curTime;

    private bool updateText;
    void Update()
    {
        if (updateText)
        {
            curTime -= Time.deltaTime;

            if (curTime <= 0)
            {
                curTime = timeUpdateText;

                string text = notice.text;
                addDot(ref text);
                notice.text = text;
            }
        }
    }

    private void addDot(ref string text)
    {
        int len = text.Length - 13;

        if (len > 3)
        {
            text = "Đang tải game";
        }
        else
        {
            text += ".";
        }
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
            updateText = false;
            notice.text = "Tải game hoàn tất";

            string[] scenePaths = assetBundle.GetAllScenePaths();
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);

            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            if (async == null)
                yield break;
        }
    }
}

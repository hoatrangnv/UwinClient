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

    public Image imgProcess;

    void Awake()
    {
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Start()
    {
        updateText = true;
        notice.text = "Đang tải game";

        StartCoroutine(loadTextOnline(urlGetScene));
    }

    void downloadBundleScene(string json)
    {
        try
        {
            startValue = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

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
        }
        catch
        {
            ErrorConnect("Đã xảy ra lỗi, bạn hãy thử lại!");
        }
    }

    IEnumerator loadTextOnline(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                ErrorConnect("Hãy kiểm tra kết nối mạng và thử lại!");
            }
            else
            {
                downloadBundleScene(webRequest.downloadHandler.text);
            }
        }
    }

    private void ErrorConnect(string message)
    {
        updateText = false;
        notice.text = "";

        popupStart.SetPopup(message);
        popupStart.SetActionForceUpdate((value) =>
        {
            if (value)
            {
                updateText = true;
                notice.text = "Đang tải game";
                StartCoroutine(loadTextOnline(urlGetScene));
            }

            popupStart.gameObject.SetActive(false);
            imgProcess.fillAmount = 0;
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
        imgProcess.fillAmount = 0.5f;
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

            imgProcess.fillAmount = (request.downloadProgress + 0.5f) / 2f;

            yield return new WaitForEndOfFrame();
        }
        AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);

        if (assetBundle.isStreamedSceneAssetBundle)
        {
            imgProcess.fillAmount = 100;
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

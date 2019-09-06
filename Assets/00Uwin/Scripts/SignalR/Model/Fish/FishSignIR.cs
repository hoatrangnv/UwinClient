using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FishSignIR : MonoBehaviour
{
    #region Sinleton
    private static FishSignIR instance;

    public static FishSignIR Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<FishSignIR>();
            }
            return instance;
        }
    }
    #endregion

    [SerializeField]
    GameObject elementParent;

    public FAccountResponse fishAccount;
    private string token;
    private string bundleID = "com.age.uwin";
    public GameObject FInfo;
    public GameObject FSendMoney;

    // Use this for initialization
    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // UwinCa not support for WebGL
            if (elementParent != null)
                elementParent.SetActive(false);
            return;
        }

        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => OpenGame());
    }


    private void OpenGame()
    {
        if (Database.Instance.islogin)
        {
            UILayerController.Instance.ShowLoading();

            StartCoroutine(ISendRequestOther("1", "https://api.uwin369.net//Account/GetTokenAuthen", null, 0, (responseData) =>
            {
                StartCoroutine(ISendRequestOther("1", "https://api.uwin369.net//Account/GetFishAccount", null, 0, (res) =>
                {
                    UILayerController.Instance.HideLoading();
                    fishAccount = JsonConvert.DeserializeObject<FAccountResponse>(res);
                    OpenApp(responseData);
                }));
            }));
        }
        else
        {
            NotifyController.Instance.Open("Bạn cần phải đăng nhập để chơi game!", NotifyController.TypeNotify.Error);
        }
    }

    public HTTPResponse _gvar;
    public static int TIME_OUT = 16;
    private DateTime lastTimeInternetError = DateTime.MinValue;
    private string mytoken = "";

    IEnumerator ISendRequestOther(string code, string url, Dictionary<string, string> datas, int method, Action<string> callback)
    {
        HTTPMethods httpMethod = HTTPMethods.Get;

        // request response
        bool isDone = false;
        string responseData = "";
        WebServiceStatus.Status responseStatus = WebServiceStatus.Status.INTERNET_ERROR;

        VKDebug.Log("Send Url: " + url, VKCommon.HEX_ORANGE);
        var request = new HTTPRequest(new Uri(url), httpMethod, (req, res) =>
        {
            switch (req.State)
            {
                case HTTPRequestStates.Finished:
                    if (res.StatusCode == 200) // 200 is ok
                    {
                        responseData = res.DataAsText;
                        responseStatus = CheckError(responseData);

                        callback.Invoke(responseData);
                    }
                    else
                    {
                        responseStatus = WebServiceStatus.Status.ERROR;
                    }
                    break;
                case HTTPRequestStates.ConnectionTimedOut:
                case HTTPRequestStates.TimedOut:
                    responseStatus = WebServiceStatus.Status.INTERNET_ERROR;
                    LPopup.OpenPopupTop("Thông báo", "Đã xảy ra lỗi vui lòng thử lại!");

                   UILayerController.Instance.HideLoading();
                    break;
                default:
                    responseStatus = WebServiceStatus.Status.ERROR;
                    break;
            }

            isDone = true;
        });

        // add data post
        request.AddHeader("Content-Type", "application/json");
        if (httpMethod == HTTPMethods.Post && datas != null && datas.Count > 0)
        {
            foreach (var item in datas)
            {
                request.AddField(item.Key, item.Value);
            }
        }

#if !BESTHTTP_DISABLE_COOKIES && (!UNITY_WEBGL || UNITY_EDITOR)
        // add header and cookie
        request.IsCookiesEnabled = true;
        if (_gvar != null)
        {
            request.Cookies = _gvar.Cookies;
        }
#endif
        request.Timeout = new TimeSpan(0, 0, TIME_OUT);
        request.Send();

        yield return new WaitUntil(() => isDone);

        Debug.Log("Request Done");
    }

    // Check bị mất token bắt đăng nhập lại
    private WebServiceStatus.Status CheckError(string response)
    {
        lastTimeInternetError = DateTime.MinValue;

        VKDebug.LogColorRed(response, "check status");

        if (string.IsNullOrEmpty(response))
            return WebServiceStatus.Status.SERVER_EXCEPTION;

        if (response.ToUpper().Contains("<!DOCTYPE"))
        {
            VKDebug.LogColorRed(response, "chua DOCTYPE");
            return WebServiceStatus.Status.SERVER_EXCEPTION;
        }


        if (response.Contains("{\"Message\":\""))
        {
            string[] arr = Regex.Split(response, "\":\"");
            if (arr.Length == 2)
            {
                if (response.Contains("\"Message\":\"Authorization has been denied for this request.\""))
                {
                    VKDebug.LogColorRed(response, "chua khong author");
                    return WebServiceStatus.Status.AUTHORIZATION_EXCEPTION;
                }

                else
                {
                    VKDebug.LogColorRed(response);
                    VKDebug.LogColorRed(response, "eo biet");
                    return WebServiceStatus.Status.SERVER_EXCEPTION;
                }

            }
        }

        return WebServiceStatus.Status.OK;
    }

    public bool CheckAppInstallation()
    {
        bool installed = false;
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = curActivity.Call<AndroidJavaObject>("getPackageManager");

        AndroidJavaObject launchIntent = null;
        try
        {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleID);
            if (launchIntent == null)
                installed = false;

            else
                installed = true;
        }

        catch (System.Exception e)
        {
            installed = false;
        }
        return installed;
    }

    public void RunFishingAndroid()
    {
        if (CheckAppInstallation())
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleID);

            launchIntent.Call<AndroidJavaObject>("putExtra", "message", this.token);
            ca.Call("startActivity", launchIntent);

            up.Dispose();
            ca.Dispose();
            packageManager.Dispose();
            launchIntent.Dispose();
        }
        else
        {
            LPopup.OpenPopupTop("Thông báo", "Hãy tải App bắn cá để chơi game!", (value) =>
            {
                if (value)
                {
                    Application.OpenURL("https://id.uwin369.net/apps/");
                }
            }, true);
        }
    }

    public void RunFishingIOS()
    {
        try
        {
            tokenFish tokenFish = JsonConvert.DeserializeObject<tokenFish>(token);
            Application.OpenURL("com.age.uwin.schemesdefault://uwinca?key=" + tokenFish.key + "&token=" + tokenFish.token);
        }
        catch (Exception ex)
        {
            LPopup.OpenPopupTop("Thông báo", "Hãy tải app cá để chơi game!", (value) =>
            {
                if (value)
                {
                    Application.OpenURL("https://id.uwin369.net/apps/");
                }
            }, true);
        }
    }

    private void OpenApp(string token)
    {
        this.token = token;
        UILayerController.Instance.ShowLayer(UILayerKey.LFInfo, DataResourceLobby.instance.listObjLayer[(int)IndexSourceGate.LFInfo]);
    }
}


public class tokenFish
{
    public string key;
    public string token;
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BestHTTP;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FishSignIR : MonoBehaviour
{
    [SerializeField]
    GameObject elementParent;

    // Use this for initialization
    void Start()
    {
        if(Application.platform == RuntimePlatform.WebGLPlayer)
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
        Debug.Log("Open Game fISHHHHHHH");
        if (Database.Instance.islogin)
        {
            StartCoroutine(ISendRequestOther("", "https://api.uwin369.net//Account/GetTokenAuthen", null, 0));
        }
        else
        {
            NotifyController.Instance.Open("Bạn cần phải đăng nhập để chơi game!", NotifyController.TypeNotify.Error);
        }


    }

    //IEnumerator GetInfoToken()
    //{
    //    yield return mytoken.Length > 0;
    //    string url = "https://api.uwin369.net/Account/AccessTokenAuthen/?token=" + mytoken;
    //    StartCoroutine(ISendRequestInfo("", url, null, 0));
    //}

    public HTTPResponse _gvar;
    public static int TIME_OUT = 16;
    private DateTime lastTimeInternetError = DateTime.MinValue;
    private string mytoken = "";

    IEnumerator ISendRequestOther(string code, string url, Dictionary<string, string> datas, int method)
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

                        OpenApp(responseData);
                    }
                    else
                    {
                        responseStatus = WebServiceStatus.Status.ERROR;
                    }
                    break;
                case HTTPRequestStates.ConnectionTimedOut:
                case HTTPRequestStates.TimedOut:
                    responseStatus = WebServiceStatus.Status.INTERNET_ERROR;
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

//    IEnumerator ISendRequestInfo(string code, string url, Dictionary<string, string> datas, int method)
//    {
//        HTTPMethods httpMethod = HTTPMethods.Get;


//        // request response
//        bool isDone = false;
//        string responseData = "";
//        WebServiceStatus.Status responseStatus = WebServiceStatus.Status.INTERNET_ERROR;

//        VKDebug.Log("Send Url: " + url, VKCommon.HEX_ORANGE);
//        var request = new HTTPRequest(new Uri(url), httpMethod, (req, res) =>
//        {
//            switch (req.State)
//            {
//                case HTTPRequestStates.Finished:
//                    if (res.StatusCode == 200) // 200 is ok
//                    {
//                        responseData = res.DataAsText;
//                        responseStatus = CheckError(responseData);

//                        Debug.Log("GetInfo: " + responseData);

//                    }
//                    else
//                    {
//                        responseStatus = WebServiceStatus.Status.ERROR;
//                    }
//                    break;
//                case HTTPRequestStates.ConnectionTimedOut:
//                case HTTPRequestStates.TimedOut:
//                    responseStatus = WebServiceStatus.Status.INTERNET_ERROR;
//                    UILayerController.Instance.HideLoading();
//                    break;
//                default:
//                    responseStatus = WebServiceStatus.Status.ERROR;
//                    break;
//            }

//            isDone = true;
//        });

//        // add data post
//        request.AddHeader("Content-Type", "application/json");
//        if (httpMethod == HTTPMethods.Post && datas != null && datas.Count > 0)
//        {
//            foreach (var item in datas)
//            {
//                request.AddField(item.Key, item.Value);
//            }
//        }

//#if !BESTHTTP_DISABLE_COOKIES && (!UNITY_WEBGL || UNITY_EDITOR)
//        // add header and cookie
//        request.IsCookiesEnabled = true;
//        if (_gvar != null)
//        {
//            request.Cookies = _gvar.Cookies;
//        }
//#endif
    //    request.Timeout = new TimeSpan(0, 0, TIME_OUT);
    //    request.Send();

    //    yield return new WaitUntil(() => isDone);

    //    Debug.Log("Request Done");
    //}

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

    private void OpenApp(string token)
    {
        Debug.LogError("token: " + token);
#if UNITY_ANDROID

        try
        {
            string bundleId = "com.age.uwin";
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);

            launchIntent.Call<AndroidJavaObject>("putExtra", "message", token);
            ca.Call("startActivity", launchIntent);

            up.Dispose();
            ca.Dispose();
            packageManager.Dispose();
            launchIntent.Dispose();
        }
        catch (System.Exception e)
        {
            Application.OpenURL("https://loc777.club/appca.apk");
        }



#elif UNITY_IPHONE
        try
        {
            Application.OpenURL("iOSUwinCa://");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.InnerException);
            Application.OpenURL("https://uwin369.net/uwinca.html");
        }

#endif

    }
}

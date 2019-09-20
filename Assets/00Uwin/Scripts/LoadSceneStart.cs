using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadSceneStart : MonoBehaviour {

    [SerializeField]
    private string urlSceneBundle;

    private void Start()
    {
     
    }

    private string GetLinkDownloadBundle(string sceneBundleName)
    {
#if UNITY_WEBGL
		return urlSceneBundle + "/WebGL/" + sceneBundleName;
#elif UNITY_ANDROID
        return urlSceneBundle + "/Android/" + sceneBundleName;
#elif UNITY_IOS
		return urlSceneBundle + "/iOS/" + sceneBundleName;
#else
		return urlSceneBundle + "/" + sceneBundleName;
#endif
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
        string[] allsceneName = assetBundle.GetAllScenePaths();

        foreach (var item in allsceneName)
        {

        }
    }
}

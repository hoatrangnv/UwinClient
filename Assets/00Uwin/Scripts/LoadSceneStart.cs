using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneStart : MonoBehaviour {

    [SerializeField]
    private string strGetLink;

    private void Start()
    {
        StartCoroutine(VKCommon.DownloadTextFromURL("https://script.google.com/macros/s/AKfycbzrpif9MibanmQPVU-5Oh6fLgnRPP5-4NyXWuuK/exec", (string strConfig) =>
        {
            Debug.Log(strConfig);
        }));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsHistoryVP : MonoBehaviour {

    [HideInInspector]
    public bool isInit = false;
    [HideInInspector]
    public GameObject mObj;

    [HideInInspector]
    public LVipPoint vipPoint;

    [HideInInspector]
    public bool isGetData = false;

    public virtual void Init(LVipPoint vipPoint)
    {
        mObj = gameObject;
        this.vipPoint = vipPoint;
    }

    public virtual void Reload()
    {
        mObj.SetActive(true);
    }

    public virtual void Close()
    {
        mObj.SetActive(false);
    }

    public virtual void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {

    }
}
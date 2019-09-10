using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LContentVP : AbsHistoryVP
{
    public ElementContentVP element;
    private VipPoint vp;
    private RewardVP rVP;

    public override void Init(LVipPoint vpController)
    {
        base.Init(vpController);
        element.gameObject.SetActive(false);
    }

    public override void Reload()
    {
        base.Reload();

        if (!isGetData)
        {
            SendRequest.GetContentVP();
        }

        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        WebServiceController.Instance.OnWebServiceResponseString += OnWebServiceResponseString;

    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
        WebServiceController.Instance.OnWebServiceResponseString -= OnWebServiceResponseString;

    }

    public override void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetVipPoint:
                if (status == WebServiceStatus.Status.OK)
                {
                    VipPoint vp = JsonConvert.DeserializeObject<VipPoint>(data);

                    if (vp.Code == 0)
                    {
                        isGetData = true;
                        ShowContent(vp);
                    }
                    else
                    {
                        LPopup.OpenPopupTop("Thông báo", "Đã xảy ra lỗi. Hãy thử lại!");
                    }
                }
                else
                {
                    LPopup.OpenPopupTop("Thông báo", "Đã xảy ra lỗi. Hãy thử lại!");
                }
                break;
        }
    }


    public void OnWebServiceResponseString(string code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case "GetRewardVP":
                if (status == WebServiceStatus.Status.OK)
                {
                    RewardVP rvp = JsonConvert.DeserializeObject<RewardVP>(data);
                    ShowReward(rvp);
                }
                else
                {
                    LPopup.OpenPopupTop("Thông báo", "Đã xảy ra lỗi. Hãy thử lại!");
                }
                break;
        }
    }

    private void ShowContent(VipPoint vPoint)
    {
        this.vp = vPoint;
        element.gameObject.SetActive(true);
        element.SetValueText(vPoint);
    }

    private void ShowReward(RewardVP rVP)
    {
        this.rVP = rVP;
        element.gameObject.SetActive(true);
        element.SetValueReward(rVP);
    }
} 

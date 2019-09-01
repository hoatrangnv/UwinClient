using BestHTTP;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LFishingGame : UILayer
{
    [Space(40)]
    public Button btClose;
    public InputField inputFieldMoney;
    public InputField inputFieldCaptcha;
    public Image imgCaptcha;
    public Button btGetCaptcha;
    public Button btSendMoney;

    private MCaptchaResponse captchaData;

    public override void StartLayer()
    {
        base.StartLayer();

        btClose.onClick.AddListener(ClickBtClose);
        btGetCaptcha.onClick.AddListener(ClickBtGetCaptcha);
        btSendMoney.onClick.AddListener(ClickBtSendMoney);
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        GetCaptcha();
    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    public void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GenCaptcha:
                if (status == WebServiceStatus.Status.OK)
                {
                    captchaData = JsonUtility.FromJson<MCaptchaResponse>(data);

                    StartCoroutine(VKCommon.LoadImageFromBase64(imgCaptcha, captchaData.Data, 0f));
                }
                else
                {
                    LPopup.OpenPopupTop("Thông báo", "Không lấy được Captcha. Hãy thử lại!");
                }
                break;
            case WebServiceCode.Code.SendFishingMoney:
                UILayerController.Instance.HideLoading();
                if (status == WebServiceStatus.Status.OK)
                {
                    FSendMoneyResponse fSend = JsonConvert.DeserializeObject<FSendMoneyResponse>(data);

                    if (fSend.code == 1)
                    {
                        //Database.Instance.UpdateUserGold(fSend.currentMoney);
                        FishSignIR.Instance.fishAccount.data.currentBalance = fSend.currentMoneyCa;
                    }
                    LPopup.OpenPopupTop("Thông báo", fSend.msg);
                }
                else
                {
                    LPopup.OpenPopupTop("Thông báo", "Lỗi kết nối. Hãy thử lại!");
                }
                break;
        }
    }

    #region listener

    private void ClickBtClose()
    {
        UILayerController.Instance.ShowLayer(UILayerKey.LFInfo,
            DataResourceLobby.instance.listObjLayer[(int)IndexSourceGate.LFInfo]);

        Close();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtGetCaptcha()
    {
        GetCaptcha();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtSendMoney()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);

        if (string.IsNullOrEmpty(inputFieldCaptcha.text) || string.IsNullOrEmpty(inputFieldMoney.text))
        {
            LPopup.OpenPopupTop("Thông báo", "Hãy nhập đủ thông tin");
            return;
        }

        UILayerController.Instance.ShowLoading();
        SendRequest.SendRequestMoneyFishing(inputFieldMoney.text, inputFieldCaptcha.text, captchaData.Token);
        //RequestSendMoney(inputFieldMoney.text);
    }

    #endregion

    private void GetCaptcha()
    {
        imgCaptcha.color = new Color(0f, 0f, 0f, 0f);
        SendRequest.SendGenCaptchaRequest();
    }
}

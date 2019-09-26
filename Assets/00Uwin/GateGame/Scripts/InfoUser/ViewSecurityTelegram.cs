using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewSecurityTelegram : AbsInfoUser
{
    [Header("UpdatePhone Fisrt")]
    public GameObject objPhoneUpdateFisrt;
    public InputField inputFieldPhoneUpdateFisrt;
    public Button btUpdatePhoneFisrt;

    [Header("Down")]
    [Space(10)]
    public Button btActiveLoginSecurity;
    public Button btDisableLoginSecurity;

    [Header("Delete Security Login")]
    public GameObject objPanelOTPSecurity;
    public Button btClosePanelOTP;
    public Button btRequestDeleleActiveSecurity;
    public Button btGetOTP;
    public InputField inputFieldOTP;

    private bool isRequestActiveSecurity = false;
    private bool isRequestsGetOTPFisrt = false;
    private bool isUpdatePhoneFisrt = false;

    public override void Init(LViewInfoUser viewInfoUser)
    {
        base.Init(viewInfoUser);

        // security
        btActiveLoginSecurity.onClick.AddListener(ClickBtActiveSecurityLogin);
        btDisableLoginSecurity.onClick.AddListener(ClickBtDisableSecurityLogin);
        btUpdatePhoneFisrt.onClick.AddListener(ClickBtRequestPhoneFisrt);
    }

    public override void Reload()
    {
        base.Reload();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;

        if (Database.Instance.Account().IsRegisterPhone())
        {
            btUpdatePhoneFisrt.gameObject.SetActive(false);

            inputFieldPhoneUpdateFisrt.contentType = InputField.ContentType.Standard;
            inputFieldPhoneUpdateFisrt.text = Database.Instance.Account().GetTel();
            inputFieldPhoneUpdateFisrt.interactable = false;
        }
        else
        {
            btUpdatePhoneFisrt.gameObject.SetActive(true);
            inputFieldPhoneUpdateFisrt.interactable = true;
        }

        isRequestActiveSecurity = Database.Instance.Account().IsOTP;

        ChangeSecuritySucceed();
        objPanelOTPSecurity.SetActive(false);
    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    private void ClickBtClosePanelOTP()
    {
        objPanelOTPSecurity.SetActive(false);

        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtRequestPhoneFisrt()
    {
        UILayerController.Instance.ShowLoading();

        if (string.IsNullOrEmpty(inputFieldPhoneUpdateFisrt.text))
        {
            LPopup.OpenPopupTop("Thông báo", "Hãy nhập đầy đủ thông tin gồm số điện thoại và mã OTP");
            return;
        }

        //SendRequest.SendUpdatePhone(inputFieldPhoneUpdateFisrt.text, inputFiedOTPUnUpdateFisrt.text);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
        isUpdatePhoneFisrt = true;
    }

    private void ClickBtActiveSecurityLogin()
    {
        if (!Database.Instance.Account().IsRegisterPhone())
        {
            LPopup.OpenPopupTop("Thông báo", "Đăng kí số điện thoại mới thực hiện được chức năng này");
            return;
        }

        if (Database.Instance.Account().IsOTP)
        {
            return;
        }

        UILayerController.Instance.ShowLoading();
        SendRequest.SendUpdateRegisterSmsPlus(false);
        isRequestActiveSecurity = true;

    }

    private void ClickBtDisableSecurityLogin()
    {
        if (!Database.Instance.Account().IsRegisterPhone())
        {
            LPopup.OpenPopupTop("Thông báo", "Đăng kí số điện thoại mới thực hiện được chức năng này");
            return;
        }

        if (!Database.Instance.Account().IsOTP)
        {
            return;
        }

        objPanelOTPSecurity.SetActive(true);
    }

    private void ChangeSecuritySucceed()
    {
        if (isRequestActiveSecurity)
        {
            Database.Instance.Account().IsOTP = true;
        }
        else
        {
            Database.Instance.Account().IsOTP = false;
        }

        // Set Again Layout
        if (Database.Instance.Account().IsOTP)
        {
            btActiveLoginSecurity.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            btDisableLoginSecurity.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            btActiveLoginSecurity.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            btDisableLoginSecurity.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }

        if (isRequestActiveSecurity == false)
        {
            ClickBtClosePanelOTP();
        }
    }

}
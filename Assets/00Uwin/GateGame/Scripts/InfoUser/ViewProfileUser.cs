using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewProfileUser : AbsInfoUser
{
    [Header("Left")]
    public Button btChangeAvatar;
    public Image imgAvartar;
    public Text txtNameUser;
    public Text txtDateJoinGame;
    public Button btChangePassword;

    [Header("Right")]
    public Button btAddGem;
    public VKTextValueChange txtQuantityGem;

    public Button btAddCoin;
    public VKTextValueChange txtQuantityCoin;

    public Text txtNameAcount;
    public Text txtIdAcount;

    public Text txtPhoneNumber;
    public Button changePhoneNumber;

    public Button btActiveSecurity;
    public Text txtBtActiveSecurity;
    public Image imgBt;
    public Sprite[] listSpriteBtSecurity;


    public Text txtActiveSecurityLogin;


    private bool isUpdatePhone = false;
    private bool isActiveSecurityLogin = false;
    private bool isBlockInvitePlay = false;

    public override void Init(LViewInfoUser viewInfoUser)
    {
        base.Init(viewInfoUser);
        btChangeAvatar.onClick.AddListener(ClickBtChangeAvatar);
        btChangePassword.onClick.AddListener(ClickBtChangePass);
        btAddCoin.onClick.AddListener(ClickBtAddCoin);
        btAddGem.onClick.AddListener(ClickBtAddGem);
        changePhoneNumber.onClick.AddListener(ClickBtUpdatePhone);

        btActiveSecurity.onClick.AddListener(ClickBtActiveSecurity);
    }

    public override void Reload()
    {
        base.Reload();
        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;

        MAccountInfo account = Database.Instance.Account();

        // Info Left
        txtNameUser.text = account.DisplayName;
        txtDateJoinGame.text = account.Time;

        // Info Right
        SetGold(account.Gold);
        SetCoin(account.Coin);

        txtNameAcount.text = account.Username.Substring(0, account.Username.Length - 4) + "xxxx";
        txtIdAcount.text = account.AccountID.ToString();

        string phoneNumber = "";
        switch (Database.Instance.Account().State)
        {
            case -1:     phoneNumber = "Đăng ký lại"; break;
            case  0:     phoneNumber = "Chưa đăng ký"; break;
            case  1:     phoneNumber = account.GetTel(); break;
            case  2:     phoneNumber = "Chờ xác nhận"; break;
        }

        txtPhoneNumber.text = phoneNumber;
        
        if (!Database.Instance.Account().IsRegisterPhone())
        {
            txtActiveSecurityLogin.text = "";
            txtBtActiveSecurity.text = "Tắt";
            imgBt.sprite = listSpriteBtSecurity[1];
        }
        else if (Database.Instance.Account().IsOTP)
        {
            txtActiveSecurityLogin.text = "Đã đăng kí";
            txtBtActiveSecurity.text = "Tắt";
            imgBt.sprite = listSpriteBtSecurity[1];
        }
        else
        {
            txtActiveSecurityLogin.text = "Chưa đăng kí";
            txtBtActiveSecurity.text = "Bật";
            imgBt.sprite = listSpriteBtSecurity[0];
        }

        SetAvatar(Database.Instance.Account().AvatarID);
    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    #region WebServiceController

    public override void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {

    }

    #endregion

    #region Listener

    private void ClickBtAddCoin()
    {
        LShop shop = (LShop)UILayerController.Instance.ShowLayer(UILayerKey.LShop, DataResourceLobby.instance.listObjLayer[0]);
        shop.listToggleMenu[(int)IndexViewShop.CHANGE_COIN].isOn = true;
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtAddGem()
    {
        UILayerController.Instance.ShowLayer(UILayerKey.LShop, DataResourceLobby.instance.listObjLayer[0]);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtChangeAvatar()
    {
        UILayerController.Instance.ShowLayer(UILayerKey.LChangeAvatar, DataResourceLobby.instance.listObjLayer[(int)IndexSourceGate.LCHANGE_AVATAR]);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtChangePass()
    {
        if (!Database.Instance.Account().IsRegisterPhone())
        {
            LPopup.OpenPopupTop("Thông báo", "Cần cập nhập điện thoại để thực hiện chức năng này");
        }
        else
        {
            UILayerController.Instance.ShowLayer(UILayerKey.LChangePass, DataResourceLobby.instance.listObjLayer[(int)IndexSourceGate.LCHANGE_PASS]);
        }
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtUpdatePhone()
    {
        viewInfoUser.listToggleMenu[(int)IndexViewInforUser.SECURIRY].isOn = true;
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtActiveSecurity()
    {
        viewInfoUser.listToggleMenu[(int)IndexViewInforUser.SECURIRY].isOn = true;
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtActiveInvitePlay()
    {
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    #endregion

    public void SetAvatar(int id)
    {
        try
        {
            imgAvartar.sprite = DataResourceLobby.instance.listSpriteAvatar[id];
        }
        catch
        {
            VKDebug.LogColorRed("Khong co");
        }
    }

    private void SetGold(double quantity, bool isRun = false)
    {
        if (isRun)
        {
            txtQuantityGem.UpdateNumber(Database.Instance.Account().Gold);
        }
        else
        {
            txtQuantityGem.SetNumber(Database.Instance.Account().Gold);
        }

    }

    private void SetCoin(double quantity, bool isRun = false)
    {
        if (isRun)
        {
            txtQuantityCoin.UpdateNumber(Database.Instance.Account().Coin);
        }
        else
        {
            txtQuantityCoin.SetNumber(Database.Instance.Account().Coin);
        }

    }
}

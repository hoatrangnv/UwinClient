using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LFInfo : UILayer
{
    [Space(40)]
    public Button btClose;
    public Button btOpenGame;
    public Button btReceiveMoney;
    public Button btSendMoney;

    public Text curMoney;
    public Text IDCa;
    FAccountResponse fish;

    public override void StartLayer()
    {
        base.StartLayer();

        btClose.onClick.AddListener(ClickBtClose);
        btSendMoney.onClick.AddListener(ClickBtSendMoney);
        btReceiveMoney.onClick.AddListener(ClickBtReceiveMoney);
        btOpenGame.onClick.AddListener(ClickBtnOpenGame);
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        FAccountResponse fish = FishSignIR.Instance.fishAccount;

        UpdateCurrentMoney(fish.data.currentBalance);
        IDCa.text = fish.data.userid.ToString();
    }

    public override void Close()
    {
        base.Close();
    }

    public void UpdateCurrentMoney(long money)
    {
        curMoney.text = VKCommon.ConvertStringMoney(money.ToString());
    }

    #region listener

    private void ClickBtClose()
    {
        Close();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    private void ClickBtSendMoney()
    {
        UILayerController.Instance.ShowLayer(UILayerKey.LFSendMoney,
            DataResourceLobby.instance.listObjLayer[(int)IndexSourceGate.LFSendMoney]);
        Close();
    }

    private void ClickBtnOpenGame()
    {
#if UNITY_ANDROID
        FishSignIR.Instance.RunFishingAndroid();
#elif UNITY_IOS
        FishSignIR.Instance.RunFishingIOS();
#endif
    }

    private void ClickBtReceiveMoney()
    {
        UILayerController.Instance.ShowLayer(UILayerKey.LFReceiveMoney,
            DataResourceLobby.instance.listObjLayer[(int)IndexSourceGate.LFReceiveMoney]);
        Close();
    }

    #endregion
}

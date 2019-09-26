using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LThongBao : UILayer
{
    public Button btClose;
    public Toggle toggleOpen;

    public override void StartLayer()
    {
        base.StartLayer();
        toggleOpen.isOn = false;
        btClose.onClick.AddListener(ClickBtClose);
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
    }

    public override void Close()
    {
        base.Close();

        if (toggleOpen.isOn)
        {
            PlayerPrefs.SetInt("Telegram", 0);
        }
    }

    private void ClickBtClose()
    {
        Close();
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }
}

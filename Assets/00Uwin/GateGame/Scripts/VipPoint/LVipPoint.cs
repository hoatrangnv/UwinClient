using BestHTTP;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LVipPoint : UILayer
{
    [Space(40)]
    [Header("ViewInfo")]
    public Button btClose;

    public Toggle[] listToggleMenu;
    public AbsHistoryVP[] listViewHistoryVP = new AbsHistoryVP[2];


    #region Implement

    public override void StartLayer()
    {
        base.StartLayer();
        AddEventTogget();
        btClose.onClick.AddListener(ClickBtClose);

        for (int i = 0; i < listViewHistoryVP.Length; i++)
        {
            listViewHistoryVP[i].Init(this);
        }
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        ReloadLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();
    }
    #endregion

    private void ClickBtClose()
    {
        Close();

        for (int i = 0; i < listViewHistoryVP.Length; i++)
        {
            listViewHistoryVP[i].Close();
        }
    }

    public override void ReloadLayer()
    {
        for (int i = 0; i < listToggleMenu.Length; i++)
        {
            if (i == 0)
            {
                listToggleMenu[i].isOn = true;
            }
            else
            {
                listToggleMenu[i].isOn = false;
            }
        }
    }

    private void AddEventTogget()
    {
        for (int i = 0; i < listToggleMenu.Length; i++)
        {
            var j = i;
            listToggleMenu[i].onValueChanged.AddListener((value) => { ClickToggle(j, value); });
        }
    }

    private void ClickToggle(int id, bool value)
    {
        OpenMenuTab(id, value);
        AudioAssistant.Instance.Shot(StringHelper.SOUND_GATE_BT);
    }

    public void OpenMenuTab(int id, bool value)
    {
        if (value == true)
        {
            for (int i = 0; i < listViewHistoryVP.Length; i++)
            {
                if (i == id)
                {
                    listViewHistoryVP[id].Reload();
                    continue;
                }

                if (listViewHistoryVP[i] != null)
                {
                    listViewHistoryVP[i].Close();
                }
            }
        }
    }
}

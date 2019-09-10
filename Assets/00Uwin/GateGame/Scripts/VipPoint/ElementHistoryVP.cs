using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementHistoryVP : MonoBehaviour {

    public Text vipPoint;
    public Text time;
    public Text reward;

    public void SetLayoutHistory(historyVP vp)
    {
        this.vipPoint.text = vp.VipPoint.ToString();
        this.time.text = vp.ReceiveDate.ToString();
        this.reward.text =   VKCommon.ConvertStringMoney(vp.GoldReward) + " GOLD";
    }
}

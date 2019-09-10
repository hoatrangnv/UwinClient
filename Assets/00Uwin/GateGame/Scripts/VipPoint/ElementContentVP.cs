using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementContentVP : MonoBehaviour
{
    public List<Text> Points;
    public List<Button> BtnPoints;

    public Text curPoint;
    public Text sumPoint;
    public Text nextPoint;
    public Image iconVip;

    public List<barPoint> barPoints;
    public List<Sprite> iconVips;

    public Slider barPoint;

    private int indexReward;

    public void SetValueReward(RewardVP reward)
    {
        if(reward.Code == 1)
        {
            LPopup.OpenPopupTop("Thông báo", reward.Msg);

            BtnPoints[indexReward].interactable = false;
            Points[indexReward].text = "<color=white>ĐÃ NHẬN QUÀ</color>";
        }
        else
        {
            LPopup.OpenPopupTop("Thông báo", "Đã xảy ra lỗi. Hãy thử lại!");
        }
    }

    public void SetValueText(VipPoint vp)
    {
        try
        {
            for (int i = 0; i < vp.ListVippoint.Count; i++)
            {
                string textValue = "";

                if (vp.ListVippoint[i].Status == 0)
                {
                    textValue = "NHẬN QUÀ";
                    int rewardID = vp.ListVippoint[i].RewardID;
                    int index = i;

                    BtnPoints[i].onClick.AddListener(() => { OnClickReward(rewardID, index); });
                    BtnPoints[i].interactable = true;
                }
                else if (vp.ListVippoint[i].Status == 1)
                {
                    textValue = "<color=white>ĐÃ NHẬN QUÀ</color>";
                }
                else
                {
                    textValue = vp.ListVippoint[i].LevelPoint.ToString();
                }

                Points[i].text = textValue;
                Points[i + 9].text = vp.ListVippoint[i].LevelPoint.ToString();
            }

            string[] lvPoints = vp.LevelPoint.Split('/');

            curPoint.text = lvPoints[0];
            nextPoint.text = lvPoints[1];
            sumPoint.text = lvPoints[2];

            int idxIcon = getIndexIcon(vp.Vippoint);
            iconVip.sprite = iconVips[idxIcon];
            iconVip.gameObject.SetActive(true);

            SetValueSlider(int.Parse(lvPoints[2]));
        }
        catch (Exception ex)
        {
            VKDebug.Log(ex.Message);
        }
    }

    private int getIndexIcon(int value)
    {
        if (value < 4)
            return 0;
        else if (value < 7)
            return 1;
        else if (value < 9)
            return 2;
        else return 3;
    }

    private void SetValueSlider(int point)
    {
        foreach (barPoint item in barPoints)
        {
            if (point > item.textMin && point <= item.textMax)
            {
                float value = ((point - item.textMin) * (item.valueMax - item.valueMin) / (item.textMax - item.textMin)) + item.valueMin;

                barPoint.value = value;
                return;
            }
        }
    }

    private void OnClickReward(int value, int indexBtn)
    {
        this.indexReward = indexBtn;
        SendRequest.GetRewardVP(value);
    }
}

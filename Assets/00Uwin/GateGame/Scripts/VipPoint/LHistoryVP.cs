using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LHistoryVP : AbsHistoryVP{

    [Header("--------------------------------------------------")]
    [Space(40)]
    public VKPageController vkPageController;

    public List<ElementHistoryVP> listElementHistoryVP;

    private List<historyVP> listData;
    private int itemInPage;

    public override void Init(LVipPoint vpController)
    {
        base.Init(vpController);

        for (int i = 0; i < listElementHistoryVP.Count; i++)
        {
            listElementHistoryVP[i].gameObject.SetActive(false);
        }
        itemInPage = listElementHistoryVP.Count;
    }
    public override void Reload()
    {
        base.Reload();

        WebServiceController.Instance.OnWebServiceResponse += OnWebServiceResponse;
        if (!isGetData)
        {
            SendRequest.GetHistoryVP();
        }
    }

    public override void Close()
    {
        base.Close();
        WebServiceController.Instance.OnWebServiceResponse -= OnWebServiceResponse;
    }

    public override void OnWebServiceResponse(WebServiceCode.Code code, WebServiceStatus.Status status, string data)
    {
        switch (code)
        {
            case WebServiceCode.Code.GetHistoryVP:
                if (status == WebServiceStatus.Status.OK)
                {
                    isGetData = true;
                    var listdata = Newtonsoft.Json.JsonConvert.DeserializeObject<List<historyVP>>(data);
                    ShowHistory(listdata);
                }
                else
                {
                    ShowHistory(null);
                }
                break;
        }
    }

    private void ShowHistory(List<historyVP> listData)
    {
        if (listData != null)
        {
            this.listData = listData;
            int maxPage = Mathf.CeilToInt(((float)listData.Count) / itemInPage);
            vkPageController.InitPage(maxPage, OnSelectPage);

            if (listData.Count > 0)
            {
                OnSelectPage(1);
            }
        }
        else
        {
            vkPageController.InitPage(0, OnSelectPage);
        }
    }

    public void OnSelectPage(int page)
    {
        var items = listData.Select(a => a).Skip((page - 1) * itemInPage).Take(itemInPage).ToList();

        int itemCount = items.Count;
        for (int i = 0; i < listElementHistoryVP.Count; i++)
        {
            if (i < itemCount)
            {
                listElementHistoryVP[i].gameObject.SetActive(true);
                listElementHistoryVP[i].SetLayoutHistory(items[i]);
            }
            else
            {
                listElementHistoryVP[i].gameObject.SetActive(false);
            }
        }
    }
}

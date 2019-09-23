using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySignalr : MonoBehaviour
{
    [Header("CONFIG")]
    public int _GAMEID;
    public string _URL;
    public string _HUBNAME;

    public XLuaBehaviour xlua;
    public LobbyController lobbyController { get; set; }

    private LobbySignalRServer _server;
    #region Sinleton
    private static LobbySignalr instance;

    public static LobbySignalr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<LobbySignalr>();
            }
            return instance;
        }
    }
    #endregion

    #region UnityMethod
    void OnApplicationQuit()
    {
        if (Database.Instance.islogin)
        {
            SignalRController.Instance.CloseServer(_GAMEID);
        }
    }
    #endregion

    public void ConnectLobby()
    {
        _server = SignalRController.Instance.CreateServer<LobbySignalRServer>(_GAMEID);
        _server.OnSRSEvent = OnSRSEvent;
        _server.OnSRSHubEvent = OnSRSHubEvent;
        _server.SRSInit(_URL, _HUBNAME);
    }


    #region Event Method
    private void OnSRSHubEvent(string command, params object[] datas)
    {
        switch (command)
        {
            case SRSConst.ENTER_LOBBY:
                HandleEnterLobby(datas);
                break;
            case SRSConst.UPDATE_MONEY_LOBBY:
                HandleUpdateMoneyLobby(datas);
                break;
            case SRSConst.KICK_USER:
                HandleKickUser(datas);
                break;
        }
    }

    private void OnSRSEvent(string command, params object[] datas)
    {
        switch (command)
        {
            case SRSConst.ON_CONNECTED:
                HandleConnected();
                break;
            case SRSConst.ON_ERROR:
                HandleConnectError(datas[0].ToString());
                break;
            case SRSConst.ON_CLOSED:
                HandleConnectClose();
                break;
        }
    }
    #endregion

    #region Handle Method
    public void HandleConnected()
    {
        _server.HubCallEnterLobby(this._GAMEID);
    }

    public void HandleConnectError(string msg)
    {
        UILayerController.Instance.HideLoading();
        if (string.IsNullOrEmpty(msg))
        {
            LPopup.OpenPopup("Lỗi", msg);
        }
    }

    public void HandleConnectClose()
    {
        UILayerController.Instance.HideLoading();

        StopAllCoroutines();
    }

    public void HandleEnterLobby(object[] data)
    {
    }

    public void HandleUpdateMoneyLobby(object[] data)
    {
        Database.Instance.UpdateUserGold(new MAccountInfoUpdateGold((double)data[0]));
        LPopup.OpenPopupTop("Thông báo", data[1].ToString());
    }

    public void HandleKickUser(object[] data)
    {
        LPopup.OpenPopupTop("Thông báo", "Tài khoản của bạn đã được đăng nhập từ một nơi khác, hãy khởi động lại game để tiếp tục!",
        (value) =>
        {
            Application.Quit();
        }, false);

        //DestroyImmediate(gameObject.transform.Find("Controller"));
        //DestroyImmediate(gameObject.transform.Find("LuaManager"));
        //SignalRController.Instance.CloseServer(4);

        //xlua.InvokeXLua("ClickBtLogout");
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Start");
    }

    #endregion
}

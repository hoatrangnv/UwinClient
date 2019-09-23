using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySignalRServer : ISignalRServer
{
    #region SignalR
    protected override void OnConnected(Connection con)
    {
        base.OnConnected(con);

        //        StartPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_CONNECTED, null);
        }
    }

    protected override void OnClosed(Connection con)
    {
        base.OnClosed(con);

        //        StopPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_CLOSED, null);
        }
    }

    protected override void OnError(Connection con, string err)
    {
        base.OnError(con, err);

        //        StopPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_ERROR, new object[] { err });
        }
    }

    protected override void OnReconnecting(Connection con)
    {
        base.OnReconnecting(con);

        //        StopPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_RECONNECTING, null);
        }
    }

    protected override void OnReconnected(Connection con)
    {
        base.OnReconnected(con);

        //        StartPingpong();

        if (OnSRSEvent != null)
        {
            OnSRSEvent.Invoke(SRSConst.ON_RECONNECTED, null);
        }
    }

    protected override void OnStateChanged(Connection con, ConnectionStates oldState, ConnectionStates newState)
    {
        base.OnStateChanged(con, oldState, newState);
    }

    protected override void OnNonHubMessage(Connection con, object data)
    {
        base.OnNonHubMessage(con, data);

        VKDebug.LogWarning("OnNonHubMessage");
    }
    #endregion

    #region Hub Response
    protected void HubEnterLobby(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.ENTER_LOBBY, msg.Arguments);
        }
    }

    protected void HubUpdateMoney(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.UPDATE_MONEY_LOBBY, msg.Arguments);
        }
    }

    protected void HubKickUser(Hub hub, MethodCallMessage msg)
    {
        if (OnSRSHubEvent != null)
        {
            OnSRSHubEvent.Invoke(SRSConst.KICK_USER, msg.Arguments);
        }
    }
    #endregion

    #region Hub Send method

    public void HubCallEnterLobby(int gameID)
    {
        _hub.Call("EnterLobby", (int)gameID);
    }

    //public void HubCallUpdateMoneyLobby()
    //{
    //    _hub.Call("UpdateMoneyLobby");
    //}

    #endregion

    #region Hub Method
    protected override void RegisterHubFunction()
    {
        base.RegisterHubFunction();
        _hub.On("EnterLobby", HubEnterLobby);
        _hub.On("UpdateMoneyLobby", HubUpdateMoney);
        _hub.On("KickUser", HubKickUser);

    }
    #endregion
}

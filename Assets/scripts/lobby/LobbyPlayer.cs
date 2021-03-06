﻿using UnityEngine;
using UnityEngine.Networking;

public class LobbyPlayer : NetworkLobbyPlayer
{
    private RectTransform playerInfo;

    public override void OnClientEnterLobby()
    {
        if (playerInfo == null)
        {
            playerInfo = LobbyUIHandler.instance.PlayerEnter();
            PlayerInfoUIHandler handler = playerInfo.GetComponent<PlayerInfoUIHandler>();
            handler.sendPlayerReady = SendReadyToBeginMessage;
            handler.sendPlayerNotReady = SendNotReadyToBeginMessage;
        }
        readyToBegin = false;
        playerInfo.GetComponent<PlayerInfoUIHandler>().SetPlayerReady(false);
        if (!isServer && isLocalPlayer)
        {
            playerInfo.GetComponent<PlayerInfoUIHandler>().ShowReadyButton();
        }
    }

    public override void OnStartAuthority()
    {
        if (isServer)
        {
            SendReadyToBeginMessage();
        }
        else if (playerInfo != null)
        {
            playerInfo.GetComponent<PlayerInfoUIHandler>().ShowReadyButton();
        }
    }

    public override void OnClientReady(bool readyState)
    {
        playerInfo.GetComponent<PlayerInfoUIHandler>().SetPlayerReady(readyState);
    }

    void OnDestroy()
    {
        if (playerInfo != null)
        {
            playerInfo.GetComponent<PlayerInfoUIHandler>().PlayerExit();
        }
    }
}

using UnityEngine;
using UnityEngine.Networking;

public class LobbyPlayer : NetworkLobbyPlayer
{
    private RectTransform playerInfo;

    public override void OnClientEnterLobby()
    {
        if (playerInfo == null)
        {
            playerInfo = LobbyUIHandler.instance.AddPlayer();
            PlayerInfoUIHandler handler = playerInfo.GetComponent<PlayerInfoUIHandler>();
            handler.sendPlayerReady = SendReadyToBeginMessage;
            handler.sendPlayerNotReady = SendNotReadyToBeginMessage;
        }
        readyToBegin = false;
    }

    public override void OnStartAuthority()
    {
        if (isServer)
        {
            playerInfo.GetComponent<PlayerInfoUIHandler>().SetPlayerReady(true);
            SendReadyToBeginMessage();
        }
        else
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

    [ClientRpc]
    public void RpcGameStart()
    {
        LobbyManager.instance.GameStart();
    }

    [ClientRpc]
    public void RpcReturnLobby()
    {
        LobbyManager.instance.ReturnLobby();
    }
}

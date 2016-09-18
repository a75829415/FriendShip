using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System;

public class LobbyPlayer : NetworkLobbyPlayer
{
    public Text playerNameText;
    public Button readyButton;

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();
        LobbyManager.instance.AddPlayer(this);
        SetupOtherPlayer();
        SendNotReadyToBeginMessage();
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        SetupLocalPlayer();
        if (LobbyManager.instance.Mode == GameMode.ClassicSingle)
        {
            SendReadyToBeginMessage();
        }
    }

    public override void OnClientReady(bool readyState)
    {
        readyButton.GetComponentInChildren<Text>().text = readyState ?
            (isLocalPlayer ? "取消准备" : "就绪") : (isLocalPlayer ? "准备" : "等待");
    }

    public override void OnClientExitLobby()
    {
        base.OnClientExitLobby();
        LobbyManager.instance.RemovePlayer(this);
    }

    private void SetupOtherPlayer()
    {
        playerNameText.text = "Remote Player";
        readyButton.GetComponentInChildren<Text>().text = "等待";
        readyButton.interactable = false;
    }

    private void SetupLocalPlayer()
    {
        playerNameText.text = "You (Local Player)";
        readyButton.GetComponentInChildren<Text>().text = "准备";
        readyButton.interactable = true;
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(ReadyButton_Click);
    }

    public void ReadyButton_Click()
    {
        Text readyText = readyButton.GetComponentInChildren<Text>();
        if (readyText.text.Equals("准备"))
        {
            SendReadyToBeginMessage();
            readyText.text = "取消准备";
        }
        else
        {
            SendNotReadyToBeginMessage();
            readyText.text = "准备";
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class LobbyManager : NetworkLobbyManager
{
    public static LobbyManager instance;

    void Awake()
    {
        instance = this;
    }

    public Text ip;
    public GameObject startPanel;
    public GameObject stopPanel;
    public GameObject lobbyPanel;
    public GameObject readyPanel;

    private NetworkClient localClient;
    private NetworkClient remoteClient;

    public void CreateRoom()
    {
        localClient = StartHost();
        startPanel.SetActive(false);
        stopPanel.SetActive(true);
        lobbyPanel.SetActive(true);
        readyPanel.SetActive(true);
    }

    public void JoinRoom()
    {
        if (!string.IsNullOrEmpty(ip.text))
        {
            remoteClient = StartClient();
            remoteClient.Connect(ip.text, networkPort);
            startPanel.SetActive(false);
            stopPanel.SetActive(true);
        }
    }

    public void SingleGame()
    {
        ShipController.isSingleGame = true;
        StartHost();
        ServerChangeScene(playScene);
    }

    public void QuitRoom()
    {
        if (localClient != null)
        {
            StopHost();
        }
        if (remoteClient != null)
        {
            remoteClient.Disconnect();
        }
        startPanel.SetActive(true);
        stopPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        readyPanel.SetActive(false);
    }
}

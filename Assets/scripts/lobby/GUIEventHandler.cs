using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUIEventHandler : MonoBehaviour
{

    public static GUIEventHandler instance;

    public RectTransform startPanel;
    public RectTransform stopPanel;
    public RectTransform lobbysPanel;
    public RectTransform gameLobbyPanel;
    public delegate void QuitRoomDelegate();
    public QuitRoomDelegate quitRoomDelegate;
    public RectTransform leftContainer;
    public RectTransform rightContainer;
    public RectTransform lobbyInfoPrefab;

    private Dictionary<string, RectTransform> servers;

    private RectTransform leftPlayer;
    private RectTransform rightPlayer;

    void Awake()
    {
        instance = this;
        servers = new Dictionary<string, RectTransform>();
    }

    public void AddLobby(string address)
    {
        RectTransform lobby = Instantiate(lobbyInfoPrefab);
        lobby.GetComponent<Image>().color = new Color(255, 255, 255, (lobbysPanel.childCount % 2) * 127);
        lobby.GetComponentInChildren<Text>().text = address;
        lobby.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            if (Time.realtimeSinceStartup - LobbyDiscovery.instance.serverAddresses[address]
                > LobbyDiscovery.instance.broadcastInterval / 500.0f)
            {
                Destroy(servers[address].gameObject);
                servers.Remove(address);
                LobbyDiscovery.instance.serverAddresses.Remove(address);
                return;
            }
            LobbyManager.instance.Mode = GameMode.ClassicDouble;
            LobbyManager.instance.StartClient().Connect(address, LobbyManager.instance.networkPort);
            startPanel.localScale = lobbysPanel.localScale = new Vector3(0, 1, 1);
            stopPanel.localScale = gameLobbyPanel.localScale = new Vector3(1, 1, 1);
            quitRoomDelegate = LobbyManager.instance.StopClient;
        });
        lobby.SetParent(lobbysPanel, false);
        servers.Add(address, lobby);
    }

    public void CreateRoom()
    {
        LobbyManager.instance.Mode = GameMode.ClassicDouble;
        LobbyManager.instance.StartHost();
        LobbyDiscovery.instance.StartBroadcasting();
        startPanel.localScale = lobbysPanel.localScale = new Vector3(0, 1, 1);
        stopPanel.localScale = gameLobbyPanel.localScale = new Vector3(1, 1, 1);
        quitRoomDelegate = LobbyManager.instance.StopHost;
    }

    public void SingleGame()
    {
        LobbyManager.instance.Mode = GameMode.ClassicSingle;
        LobbyManager.instance.StartHost();
        quitRoomDelegate = LobbyManager.instance.StopHost;
    }

    public void QuitRoom()
    {
        if (LobbyDiscovery.instance.isServer)
        {
            LobbyDiscovery.instance.StopBroadcastingOrListening();
        }
        quitRoomDelegate();
        startPanel.localScale = new Vector3(1, 1, 1);
        stopPanel.localScale = new Vector3(0, 1, 1);
    }

    public void AddPlayer(RectTransform playerInfo)
    {
        if (leftPlayer == null)
        {
            leftPlayer = playerInfo;
            playerInfo.SetParent(leftContainer);
            playerInfo.localPosition = new Vector3(0, 0, 0);
            playerInfo.localScale = new Vector3(1, 1, 1);
        }
        else if (rightPlayer == null)
        {
            rightPlayer = playerInfo;
            playerInfo.SetParent(rightContainer);
            playerInfo.localPosition = new Vector3(0, 0, 0);
            playerInfo.localScale = new Vector3(1, 1, 1);
        }
    }
}

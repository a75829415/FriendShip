using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeGUIHandler : MonoBehaviour
{
    public static WelcomeGUIHandler instance;

    public RectTransform menuPanel;
    public RectTransform viewPanel;
    public RectTransform detailPanel;
    public RectTransform lobbysPanel;
    public RectTransform lobbyInfoPrefab;
    public RectTransform[] lobbys;
    public Button readyButton;
    public Button startButton;

    private Dictionary<string, RectTransform> servers;

    void Awake()
    {
        instance = this;
        servers = new Dictionary<string, RectTransform>();
    }

    public void ShowWelcomeGUI()
    {
        menuPanel.gameObject.SetActive(true);
        viewPanel.gameObject.SetActive(true);
        detailPanel.gameObject.SetActive(true);
    }

    public void HideWelcomeGUI()
    {
        menuPanel.gameObject.SetActive(false);
        viewPanel.gameObject.SetActive(false);
        detailPanel.gameObject.SetActive(false);
    }

    public void SingleGame()
    {
        LobbyManager.instance.Mode = GameMode.ClassicSingle;
        LobbyGUIHandler.quitRoomDelegate = () =>
        {
            LobbyManager.instance.StopHost();
            LobbyGUIHandler.instance.HideLobbyGUI();
            ShowWelcomeGUI();
        };
        LobbyManager.instance.StartHost();
    }

    public void ShowLobbys()
    {
        lobbysPanel.gameObject.SetActive(true);
        HideWelcomeGUI();
    }

    public void HideLobbys()
    {
        ShowWelcomeGUI();
        lobbysPanel.gameObject.SetActive(false);
    }

    public void CreateRoom()
    {
        LobbyManager.instance.Mode = GameMode.ClassicDouble;
        LobbyGUIHandler.quitRoomDelegate = () =>
        {
            LobbyManager.instance.StopHost();
            LobbyDiscovery.instance.StopBroadcastingOrListening();
            LobbyDiscovery.instance.StartListening();
            LobbyGUIHandler.instance.HideLobbyGUI();
            ShowLobbys();
        };
        LobbyManager.instance.StartHost();
        LobbyDiscovery.instance.StartBroadcasting();
        readyButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(LobbyManager.instance.CheckClientsReady);
        LobbyGUIHandler.instance.ShowLobbyGUI();
        lobbysPanel.gameObject.SetActive(false);
    }

    public void AddLobby(string address)
    {
        RectTransform lobby = Instantiate(lobbyInfoPrefab);
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
            LobbyGUIHandler.quitRoomDelegate = () =>
            {
                LobbyManager.instance.StopClient();
                LobbyGUIHandler.instance.HideLobbyGUI();
                ShowLobbys();
            };
            LobbyManager.instance.StartClient().Connect(address, LobbyManager.instance.networkPort);
            readyButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
            LobbyGUIHandler.instance.ShowLobbyGUI();
            lobbysPanel.gameObject.SetActive(false);
        });
        for (int i = 0; i < lobbys.Length; i++)
        {
            if (lobbys[i].childCount == 0)
            {
                lobby.SetParent(lobbys[i], false);
                break;
            }
        }
        servers.Add(address, lobby);
    }
}

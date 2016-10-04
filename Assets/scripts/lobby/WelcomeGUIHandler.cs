using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeGUIHandler : MonoBehaviour
{
    public static WelcomeGUIHandler instance;
    
    public Canvas WelcomeGUI;
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
        WelcomeGUI.gameObject.SetActive(true);
    }

    public void HideWelcomeGUI()
    {
        WelcomeGUI.gameObject.SetActive(false);
    }

    public void ShowLobbys()
    {
        lobbysPanel.gameObject.SetActive(true);
    }

    public void HideLobbys()
    {
        lobbysPanel.gameObject.SetActive(false);
    }

    public void CreateRoom()
    {
        GameSettingHandler.instance.ShowSettings();
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
                ShowWelcomeGUI();
                ShowLobbys();
            };
            LobbyManager.instance.StartClient().Connect(address, LobbyManager.instance.networkPort);
            readyButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
            HideWelcomeGUI();
            LobbyGUIHandler.instance.ShowLobbyGUI();
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

    public void StartSingleGame()
    {
        LobbyGUIHandler.quitRoomDelegate = () =>
        {
            LobbyManager.instance.StopHost();
            LobbyGUIHandler.instance.HideLobbyGUI();
            ShowWelcomeGUI();
        };
        LobbyManager.instance.StartHost();
        LobbyManager.instance.CheckClientsReady();
    }

    public void StartDoubleGame()
    {
        LobbyGUIHandler.quitRoomDelegate = () =>
        {
            LobbyManager.instance.StopHost();
            LobbyDiscovery.instance.StopBroadcastingOrListening();
            LobbyDiscovery.instance.StartListening();
            LobbyGUIHandler.instance.HideLobbyGUI();
            ShowWelcomeGUI();
            ShowLobbys();
        };
        LobbyManager.instance.StartHost();
        LobbyDiscovery.instance.StartBroadcasting();
        readyButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(LobbyManager.instance.CheckClientsReady);
        HideWelcomeGUI();
        LobbyGUIHandler.instance.ShowLobbyGUI();
    }
}

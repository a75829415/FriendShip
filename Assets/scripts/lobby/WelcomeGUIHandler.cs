using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WelcomeGUIHandler : MonoBehaviour
{
    public delegate void VoidDelegate();

    public static WelcomeGUIHandler instance;
    public static VoidDelegate voidDelegate;

    public RectTransform lobbysPanel;
    public RectTransform lobbyInfoPrefab;
    public RectTransform[] lobbys;

    private Dictionary<string, RectTransform> servers;

    void Awake()
    {
        instance = this;
        servers = new Dictionary<string, RectTransform>();
        if (voidDelegate != null)
        {
            voidDelegate();
            voidDelegate = null;
        }
    }

    public void SingleGame()
    {
        LobbyGUIHandler.createRoomDelegate = () =>
        {
            LobbyManager.instance.StartHost();
        };
        LobbyGUIHandler.quitRoomDelegate = () =>
        {
            LobbyManager.instance.StopHost();
        };
        LobbyManager.instance.Mode = GameMode.ClassicSingle;
        SceneManager.LoadScene(LobbyManager.instance.lobbyScene);
    }

    public void MultipleGame()
    {
        lobbysPanel.gameObject.SetActive(true);
    }

    public void CreateRoom()
    {
        LobbyGUIHandler.createRoomDelegate = () =>
        {
            LobbyManager.instance.StartHost();
            LobbyDiscovery.instance.StartBroadcasting();
        };
        LobbyGUIHandler.quitRoomDelegate = () =>
        {
            LobbyManager.instance.StopHost();
            LobbyDiscovery.instance.StopBroadcastingOrListening();
            LobbyDiscovery.instance.StartListening();
        };
        LobbyManager.instance.Mode = GameMode.ClassicDouble;
        SceneManager.LoadScene(LobbyManager.instance.lobbyScene);
    }

    public void ReturnMenu()
    {
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
            LobbyGUIHandler.createRoomDelegate = () =>
            {
                LobbyManager.instance.StartClient().Connect(address, LobbyManager.instance.networkPort);
            };
            LobbyGUIHandler.quitRoomDelegate = () =>
            {
                LobbyManager.instance.StopClient();
            };
            LobbyManager.instance.Mode = GameMode.ClassicDouble;
            SceneManager.LoadScene(LobbyManager.instance.lobbyScene);
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

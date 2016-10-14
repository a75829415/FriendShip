using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIHandler : UIHandlerBase
{
    public delegate void VoidDelegate();
    public static VoidDelegate quitRoomDelegate;
    public static LobbyUIHandler instance;

    private static Dictionary<RectTransform, bool> players = new Dictionary<RectTransform, bool>();
    private static bool isServer;
    private static string address;

    public RectTransform playersLayout;
    public RectTransform playerInfoPrefab;
    public Text titleText;
    public Button startGameButton;

    void Awake()
    {
        instance = this;
        Initialize(isServer, address);
    }

    public void Initialize(bool isServer, string address)
    {
        List<RectTransform> buffer = new List<RectTransform>(players.Keys);
        foreach (RectTransform player in buffer)
        {
            if (player != null)
            {
                Destroy(player.gameObject);
            }
        }
        players.Clear();
        for (int i = 0; i < LobbyManager.instance.minPlayers; i++)
        {
            RectTransform playerInfo = Instantiate(playerInfoPrefab);
            playerInfo.GetComponent<PlayerInfoUIHandler>().SetPlayer(i + 1);
            playerInfo.SetParent(playersLayout, false);
            players.Add(playerInfo, false);
        }
        titleText.text = isServer ? "我的房间 (" + GetLocalIpv4() + ")" : address + "的房间";
        startGameButton.gameObject.SetActive(isServer);
        LobbyUIHandler.isServer = isServer;
        LobbyUIHandler.address = address;
    }

    private string GetLocalIpv4()
    {
        IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
        foreach (IPAddress ip in localIPs)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "IP地址不可用";
    }

    public RectTransform PlayerEnter()
    {
        List<RectTransform> buffer = new List<RectTransform>(players.Keys);
        foreach (RectTransform player in buffer)
        {
            if (!players[player])
            {
                player.GetComponent<PlayerInfoUIHandler>().PlayerEnter();
                players[player] = true;
                return player;
            }
        }
        Debug.LogWarning("Too many players.");
        return null;
    }

    public void PlayerExit(RectTransform playerInfo)
    {
        playerInfo.GetComponent<PlayerInfoUIHandler>().Initialize();
        players[playerInfo] = false;
    }

    // ---- UI event handlers ----
    public void OnReturnButtonClick()
    {
        quitRoomDelegate();
        LobbyManager.instance.StopGame();
    }

    public void OnStartGameButtonClick()
    {
        LobbyManager.instance.CheckClientsReady();
    }
}

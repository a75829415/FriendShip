using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIHandler : MonoBehaviour
{
    public static LobbyUIHandler instance;

    public delegate void VoidDelegate();
    public VoidDelegate quitRoomDelegate;
    public RectTransform lobbyPanel;
    public RectTransform playersLayout;
    public RectTransform playerInfoPrefab;
    public Text titleText;
    public Button startGameButton;

    private Dictionary<RectTransform, bool> players;

    void Awake()
    {
        instance = this;
        players = new Dictionary<RectTransform, bool>();
    }

    public void ShowGUI(bool showGUI)
    {
        lobbyPanel.gameObject.SetActive(showGUI);
    }

    public void Initialize(bool isServer, string address)
    {
        List<RectTransform> buffer = new List<RectTransform>(players.Keys);
        foreach (RectTransform player in buffer)
        {
            Destroy(player.gameObject);
        }
        players.Clear();
        for (int i = 0; i < LobbyManager.instance.minPlayers; i++)
        {
            RectTransform playerInfo = Instantiate(playerInfoPrefab);
            playerInfo.GetComponent<PlayerInfoUIHandler>().SetPlayer(i + 1);
            playerInfo.SetParent(playersLayout, false);
            players.Add(playerInfo, false);
        }
        titleText.text = isServer ? "我的房间" : address + "的房间";
        startGameButton.gameObject.SetActive(isServer);
    }

    public RectTransform AddPlayer()
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

    public void RemovePlayer(RectTransform playerInfo)
    {
        playerInfo.GetComponent<PlayerInfoUIHandler>().Initialize();
        players[playerInfo] = false;
    }

    // ---- UI event handlers ----
    public void OnReturnButtonClick()
    {
        quitRoomDelegate();
    }

    public void OnStartGameButtonClick()
    {
        LobbyManager.instance.CheckClientsReady();
    }
}

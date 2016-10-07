using System.Collections.Generic;
using UnityEngine;

public class ChooseLobbyUIHandler : MonoBehaviour
{
    public static ChooseLobbyUIHandler instance;

    public RectTransform chooseLobbyPanel;
    public RectTransform lobbysLayout;
    public RectTransform lobbyInfoPrefab;

    private Dictionary<string, RectTransform> servers;
    private float timer = 3.0f;

    void Awake()
    {
        instance = this;
        servers = new Dictionary<string, RectTransform>();
    }

    void Update()
    {
        if ((timer -= Time.deltaTime) < 0)
        {
            timer = 3.0f;
            List<string> buffer = new List<string>(servers.Keys);
            foreach (string address in buffer)
            {
                if (Time.realtimeSinceStartup - LobbyDiscovery.instance.serverAddresses[address]
                    > LobbyDiscovery.instance.broadcastInterval / 500.0f)
                {
                    Destroy(servers[address].gameObject);
                    servers.Remove(address);
                    LobbyDiscovery.instance.serverAddresses.Remove(address);
                }
            }
        }
    }

    public void ShowGUI(bool showGUI)
    {
        chooseLobbyPanel.gameObject.SetActive(showGUI);
    }

    public void AddLobby(string address, string mode, string playerNumber)
    {
        RectTransform lobbyInfo = Instantiate(lobbyInfoPrefab);
        lobbyInfo.GetComponent<LobbyInfoUIHandler>().Initialize(address, mode, playerNumber);
        lobbyInfo.SetParent(lobbysLayout, false);
        servers.Add(address, lobbyInfo);
    }

    // ---- UI event handlers ----
    public void OnReturnButtonClick()
    {
        WelcomeUIHandler.instance.ShowGUI(true);
        ShowGUI(false);
    }
}

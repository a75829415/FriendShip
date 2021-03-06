﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChooseLobbyUIHandler : UIHandlerBase
{
    public static ChooseLobbyUIHandler instance;
    
    public RectTransform lobbiesLayout;
    public RectTransform lobbyInfoPrefab;
    public InputField searchServerInputField;

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

    public void AddLobby(string address, string mode, string playerNumber)
    {
        RectTransform lobbyInfo = Instantiate(lobbyInfoPrefab);
        lobbyInfo.GetComponent<LobbyInfoUIHandler>().Initialize(address, mode, playerNumber);
        lobbyInfo.SetParent(lobbiesLayout, false);
        servers.Add(address, lobbyInfo);
    }

    // ---- UI event handlers ----
    public void OnReturnButtonClick()
    {
        WelcomeUIHandler.instance.ShowGUI(true);
        ShowGUI(false);
    }

    public void OnSearchServerButtonClick()
    {
        if (!Regex.IsMatch(searchServerInputField.text,
            @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
        {
            PopupUIHandler.instance.Popup("咦，这个IP地址的格式怎么看上去好像不太对...");
            return;
        }
        LobbyUISystemInitializer.instance.SetPanelToShow(currentPanel);
        LobbyManager.instance.JoinGame(searchServerInputField.text);
        LobbyUIHandler.instance.Initialize(false, searchServerInputField.text);
        LobbyUIHandler.instance.ShowGUI(true);
        ShowGUI(false);
        searchServerInputField.text = string.Empty;
    }
}

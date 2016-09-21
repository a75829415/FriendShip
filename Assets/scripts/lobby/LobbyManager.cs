﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;

public class LobbyManager : NetworkLobbyManager
{
    public static LobbyManager instance
    {
        get
        {
            return singleton as LobbyManager;
        }
    }

    private Dictionary<NetworkConnection, ShipControlMode> controlModeAllocation;

    private GameMode mode;
    public GameMode Mode
    {
        get
        {
            return mode;
        }
        set
        {
            if ((mode = value) == GameMode.ClassicSingle)
            {
                minPlayers = 1;
            }
            else
            {
                minPlayers = 2;
            }
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        controlModeAllocation = new Dictionary<NetworkConnection, ShipControlMode>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    //public override void OnLobbyServerPlayersReady()
    //{
    //    switch (Mode)
    //    {
    //        case GameMode.ClassicSingle:
    //            controlModeAllocation.Add(leftPlayer.connectionToClient, ShipControlMode.BothPaddles);
    //            break;
    //        case GameMode.ClassicDouble:
    //            float random = Random.value;
    //            controlModeAllocation.Add(leftPlayer.connectionToClient,
    //                random < 0.5 ? ShipControlMode.LeftPaddleOnly : ShipControlMode.RightPaddleOnly);
    //            controlModeAllocation.Add(rightPlayer.connectionToClient,
    //                random < 0.5 ? ShipControlMode.RightPaddleOnly : ShipControlMode.LeftPaddleOnly);
    //            break;
    //    }
    //    base.OnLobbyServerPlayersReady();
    //}

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        //HideLobbyGUI();
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        ReturnLobby();
        StopHost();
    }

    public override void OnLobbyClientDisconnect(NetworkConnection conn)
    {
        //ShowLobbyGUI();
        StopClient();
    }

    public ShipControlMode GetShipControlMode(NetworkConnection conn)
    {
        ShipControlMode mode;
        return controlModeAllocation.TryGetValue(conn, out mode) ? mode : ShipControlMode.BothPaddles;
    }

    public void ReturnLobby()
    {
        SendReturnToLobby();
        Destroy(Manager.instance);
        //ShowLobbyGUI();
    }
}

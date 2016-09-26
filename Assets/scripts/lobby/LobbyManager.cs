using UnityEngine;
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
        set
        {
            if (singleton == null)
            {
                singleton = value;
            }
            else if (singleton != value)
            {
                Destroy(value.gameObject);
            }
        }
    }

    public ClassicNetHub classicNetHub;
    
    private Dictionary<int, ShipControlMode> controlModeAllocation;

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

    public NetHub GameNetHub
    {
        get
        {
            switch (Mode)
            {
                case GameMode.ClassicSingle:
                    return classicNetHub;
                case GameMode.ClassicDouble:
                    return classicNetHub;
                default:
                    return null;
            }
        }
    }

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        controlModeAllocation = new Dictionary<int, ShipControlMode>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public override void OnLobbyServerPlayersReady()
    {
        switch (Mode)
        {
            case GameMode.ClassicSingle:
                controlModeAllocation.Add(lobbySlots[0].connectionToClient.connectionId, ShipControlMode.BothPaddles);
                break;
            case GameMode.ClassicDouble:
                float random = Random.value;
                controlModeAllocation.Add(lobbySlots[0].connectionToClient.connectionId,
                    random < 0.5 ? ShipControlMode.LeftPaddleOnly : ShipControlMode.RightPaddleOnly);
                controlModeAllocation.Add(lobbySlots[1].connectionToClient.connectionId,
                    random < 0.5 ? ShipControlMode.RightPaddleOnly : ShipControlMode.LeftPaddleOnly);
                break;
        }
		Instantiate(GameNetHub);
		NetworkServer.Spawn(NetHub.instance.gameObject);
        base.OnLobbyServerPlayersReady();
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        SendReturnToLobby();
        StopHost();
    }

    public override void OnLobbyClientDisconnect(NetworkConnection conn)
    {
        StopClient();
    }

    public ShipControlMode GetShipControlMode(NetworkConnection conn)
    {
        ShipControlMode mode;
        return controlModeAllocation.TryGetValue(conn.connectionId, out mode) ? mode : ShipControlMode.BothPaddles;
    }
}

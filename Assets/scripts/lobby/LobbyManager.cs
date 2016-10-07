using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkLobbyManager
{
    public const uint SINGLE_GAME_OVER = 0x00;
    public const uint DOUBLE_GAME_OVER = 0x01;
    public const uint LOSE_CONNECT_TO_CLIENT = 0x10;
    public const uint LOSE_CONNECT_TO_SERVER = 0x11;

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

    public delegate void VoidDelegate();
    public VoidDelegate stopGameDelegate;
    public ClassicNetHub classicNetHub;
    public CompetitiveNetHub competitiveNetHub;

    private Dictionary<int, ShipControlMode> controlModeAllocation;
    private NetHub gameNetHub;

    private GameMode mode;
    public GameMode Mode
    {
        get
        {
            return mode;
        }
        set
        {
            switch (mode = value)
            {
                case GameMode.Classic:
                    gameNetHub = classicNetHub;
                    break;
                case GameMode.Competitive:
                    gameNetHub = competitiveNetHub;
                    break;
                default:
                    gameNetHub = null;
                    Debug.LogWarning("Game mode not set.");
                    break;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name.Equals(lobbyScene))
            {
                Application.Quit();
            }
            else if (minPlayers == 1)
            {
                ChangeToLobbyScene(SINGLE_GAME_OVER);
            }
            else if (minPlayers == 2)
            {
                ChangeToLobbyScene(DOUBLE_GAME_OVER);
            }
        }
    }

    public override void OnLobbyServerPlayersReady()
    {
        if (minPlayers == 1)
        {
            CheckClientsReady();
        }
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name.Equals(playScene))
        {
            ChangeToLobbyScene(LOSE_CONNECT_TO_CLIENT);
        }
    }

    public override void OnLobbyClientDisconnect(NetworkConnection conn)
    {
        ChangeToLobbyScene(LOSE_CONNECT_TO_SERVER);
    }

    public void CheckClientsReady()
    {
        bool allClientsReady = true;
        int playerCount = 0;
        foreach (LobbyPlayer player in lobbySlots)
        {
            if (player != null)
            {
                allClientsReady &= player.readyToBegin || player.isLocalPlayer;
                playerCount++;
            }
        }
        if (playerCount < minPlayers)
        {
            Debug.LogWarning("Not enough players.");
        }
        else if (allClientsReady)
        {
            controlModeAllocation.Clear();
            switch (minPlayers)
            {
                case 1:
                    controlModeAllocation.Add(lobbySlots[0].connectionToClient.connectionId, ShipControlMode.BothPaddles);
                    break;
                case 2:
                    float random = Random.value;
                    controlModeAllocation.Add(lobbySlots[0].connectionToClient.connectionId,
                        random < 0.5 ? ShipControlMode.LeftPaddleOnly : ShipControlMode.RightPaddleOnly);
                    controlModeAllocation.Add(lobbySlots[1].connectionToClient.connectionId,
                        random < 0.5 ? ShipControlMode.RightPaddleOnly : ShipControlMode.LeftPaddleOnly);
                    break;
                case 4:
                    Debug.LogError("Coming soon.");
                    break;
            }
            Instantiate(gameNetHub);
            NetworkServer.Spawn(NetHub.instance.gameObject);
            ChangeToPlayScene();
        }
        else
        {
            Debug.LogWarning("Players not ready.");
        }
    }

    public ShipControlMode GetShipControlMode(NetworkConnection conn)
    {
        ShipControlMode mode;
        return controlModeAllocation.TryGetValue(conn.connectionId, out mode) ? mode : ShipControlMode.Unknown;
    }

    public void ChangeToPlayScene()
    {
        (lobbySlots[0] as LobbyPlayer).RpcGameStart();
        ServerChangeScene(playScene);
    }

    public void ChangeToLobbyScene(uint reason)
    {
        switch (reason)
        {
            case SINGLE_GAME_OVER:
            case LOSE_CONNECT_TO_SERVER:
                LobbyUIHandler.instance.quitRoomDelegate();
                break;
            case DOUBLE_GAME_OVER:
            case LOSE_CONNECT_TO_CLIENT:
                (lobbySlots[0] as LobbyPlayer).RpcReturnLobby();
                ServerChangeScene(lobbyScene);
                break;
        }
    }

    public void GameStart()
    {
        WelcomeUIHandler.instance.ShowGUI(false);
        LobbyUIHandler.instance.ShowGUI(false);
    }

    public void ReturnLobby()
    {
        LobbyUIHandler.instance.ShowGUI(true);
        if (NetHub.instance != null)
        {
            Destroy(NetHub.instance.gameObject);
        }
    }

    // ---- UI event hooks ----
    public void StartGame()
    {
        if (minPlayers == 1)
        {
            stopGameDelegate = StopHost;
            StartHost();
        }
        else
        {
            stopGameDelegate = () =>
            {
                LobbyDiscovery.instance.StopBroadcastingOrListening();
                LobbyDiscovery.instance.StartListening();
                StopHost();
            };
            StartHost();
            LobbyDiscovery.instance.StartBroadcasting((int)mode + "*" + minPlayers);
        }
    }

    public void JoinGame(string address)
    {
        stopGameDelegate = StopClient;
        StartClient().Connect(address, networkPort);
    }

    public void StopGame()
    {
        stopGameDelegate();
    }
}

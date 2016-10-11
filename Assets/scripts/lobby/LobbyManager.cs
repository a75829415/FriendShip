using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkLobbyManager
{
    public const uint SINGLE_GAME_OVER = 0x00;
    public const uint DOUBLE_GAME_OVER = 0x01;
    public const uint LOSE_CONNECTION_TO_CLIENT = 0x10;
    public const uint LOSE_CONNECTION_TO_SERVER = 0x11;

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
    public VoidDelegate loseConnectionDelegate;

    [Header("Net Hub Prefabs")]
    public ClassicNetHub classicNetHub;
    public CompetitiveNetHub competitiveNetHub;
    public BoomNetHub boomNetHub;

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
                case GameMode.Boom:
                    gameNetHub = boomNetHub;
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
            loseConnectionDelegate = () =>
            {
                PopupUIHandler.instance.Popup("哎呀，有玩家溜走了Σ(っ °Д °;)っ...");
            };
            ChangeToLobbyScene(LOSE_CONNECTION_TO_CLIENT);
        }
    }

    public override void OnLobbyClientConnect(NetworkConnection conn)
    {
        if (minPlayers == 1)
        {
            return;
        }
        PopupUIHandler.instance.OnOkButtonClick();
        LobbyUISystemInitializer.instance.SetPanelToShow(ChooseLobbyUIHandler.instance.currentPanel);
        loseConnectionDelegate = () =>
        {
            PopupUIHandler.instance.Popup("糟了，房主的Note 7好像炸了\n怎么都没法重新连接上( ╯□╰ )...");
        };
    }

    public override void OnLobbyClientDisconnect(NetworkConnection conn)
    {
        ChangeToLobbyScene(LOSE_CONNECTION_TO_SERVER);
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
            PopupUIHandler.instance.Popup("人都不够怎么玩游戏嘛←_←");
        }
        else if (allClientsReady)
        {
            controlModeAllocation.Clear();
            switch (minPlayers)
            {
                case 1:
                    controlModeAllocation.Add(lobbySlots[0].connectionToClient.connectionId,
                        mode == GameMode.Boom ? ShipControlMode.BothPaddlesAndFire : ShipControlMode.BothPaddles);
                    break;
                case 2:
                    float random = Random.value;
                    controlModeAllocation.Add(lobbySlots[0].connectionToClient.connectionId,
                        random < 0.5 ? (mode == GameMode.Boom ? ShipControlMode.BothPaddles : ShipControlMode.LeftPaddleOnly)
                        : (mode == GameMode.Boom ? ShipControlMode.FireOnly : ShipControlMode.RightPaddleOnly));
                    controlModeAllocation.Add(lobbySlots[1].connectionToClient.connectionId,
                        random < 0.5 ? (mode == GameMode.Boom ? ShipControlMode.FireOnly : ShipControlMode.RightPaddleOnly)
                        : (mode == GameMode.Boom ? ShipControlMode.BothPaddles : ShipControlMode.LeftPaddleOnly));
                    break;
                case 4:
                    Debug.LogError("Coming soon.");
                    break;
            }
            Instantiate(gameNetHub);
            NetworkServer.Spawn(NetHub.instance.gameObject);
            ServerChangeScene(playScene);
        }
        else
        {
            PopupUIHandler.instance.Popup("别着急嘛，还有人没准备好呢\n(✿◡‿◡)");
        }
    }

    public void ChangeToLobbyScene(uint reason)
    {
        switch (reason)
        {
            case SINGLE_GAME_OVER:
                stopGameDelegate();
                break;
            case DOUBLE_GAME_OVER:
                ServerChangeScene(lobbyScene);
                break;
            case LOSE_CONNECTION_TO_SERVER:
                loseConnectionDelegate();
                StopGame();
                break;
            case LOSE_CONNECTION_TO_CLIENT:
                loseConnectionDelegate();
                ServerChangeScene(lobbyScene);
                break;
        }
    }

    public ShipControlMode GetShipControlMode(NetworkConnection conn)
    {
        ShipControlMode mode;
        return controlModeAllocation.TryGetValue(conn.connectionId, out mode) ? mode : ShipControlMode.Unknown;
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
        loseConnectionDelegate = () =>
        {
            PopupUIHandler.instance.Popup("IP地址真的写对了吗,,ԾㅂԾ,,\n连不上诶...");
            ChooseLobbyUIHandler.instance.ShowGUI(true);
            LobbyUIHandler.instance.ShowGUI(false);
        };
        LobbyUIHandler.quitRoomDelegate = () =>
        {
            ChooseLobbyUIHandler.instance.ShowGUI(true);
            LobbyUIHandler.instance.ShowGUI(false);
        };
        StartClient().Connect(address, networkPort);
        PopupUIHandler.instance.Popup("全速连接中...", false);
    }

    public void StopGame()
    {
        stopGameDelegate();
    }
}

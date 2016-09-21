using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;

public class LobbyManager : NetworkLobbyManager
{
    public static LobbyManager instance;

    public Text ip;
    public RectTransform startPanel;
    public RectTransform stopPanel;
    public RectTransform lobbyPanel;
    public delegate void QuitRoomDelegate();
    public QuitRoomDelegate quitRoomDelegate;
    public RectTransform leftContainer;
    public RectTransform rightContainer;
    public Manager classicManager;

    private LobbyPlayer leftPlayer;
    private LobbyPlayer rightPlayer;

    private Dictionary<NetworkConnection, ShipControlMode> controlModeAllocation;

    private GameMode mode;
    public GameMode Mode
    {
        get
        {
            return mode;
        }
        private set
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

    public Manager GameManager
    {
        get
        {
            switch (Mode)
            {
                case GameMode.ClassicSingle:
                    return classicManager;
                case GameMode.ClassicDouble:
                    return classicManager;
                default:
                    return null;
            }
        }
    }

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        controlModeAllocation = new Dictionary<NetworkConnection, ShipControlMode>();
    }

    public override void OnLobbyServerPlayersReady()
    {
        switch (Mode)
        {
            case GameMode.ClassicSingle:
                controlModeAllocation.Add(leftPlayer.connectionToClient, ShipControlMode.BothPaddles);
                break;
            case GameMode.ClassicDouble:
                float random = Random.value;
                controlModeAllocation.Add(leftPlayer.connectionToClient,
                    random < 0.5 ? ShipControlMode.LeftPaddleOnly : ShipControlMode.RightPaddleOnly);
                controlModeAllocation.Add(rightPlayer.connectionToClient,
                    random < 0.5 ? ShipControlMode.RightPaddleOnly : ShipControlMode.LeftPaddleOnly);
                break;
        }
        DontDestroyOnLoad(Instantiate(GameManager));
        HideLobbyGUI();
        base.OnLobbyServerPlayersReady();
    }

    public override void OnLobbyClientDisconnect(NetworkConnection conn)
    {
        ShowLobbyGUI();
        StopClient();
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        ReturnLobby();
        StopHost();
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
        ShowLobbyGUI();
    }

    public void ShowLobbyGUI()
    {
        startPanel.localScale = lobbyPanel.localScale = new Vector3(1, 1, 1);
    }

    public void HideLobbyGUI()
    {
        startPanel.localScale = stopPanel.localScale = lobbyPanel.localScale = new Vector3(0, 0, 0);
    }

    public void CreateRoom()
    {
        Mode = GameMode.ClassicDouble;
        StartHost();
        startPanel.localScale = new Vector3(0, 1, 1);
        stopPanel.localScale = new Vector3(1, 1, 1);
        quitRoomDelegate = QuitHostRoom;
    }

    public void JoinRoom()
    {
        if (!string.IsNullOrEmpty(ip.text))
        {
            Mode = GameMode.ClassicDouble;
            NetworkClient client = StartClient();
            client.Connect(ip.text, networkPort);
            startPanel.localScale = new Vector3(0, 1, 1);
            stopPanel.localScale = new Vector3(1, 1, 1);
            quitRoomDelegate = QuitRemoteRoom;
        }
    }

    public void SingleGame()
    {
        Mode = GameMode.ClassicSingle;
        StartHost();
        quitRoomDelegate = QuitHostRoom;
    }

    public void QuitHostRoom()
    {
        StopHost();
        startPanel.localScale = new Vector3(1, 1, 1);
        stopPanel.localScale = new Vector3(0, 1, 1);
    }

    public void QuitRemoteRoom()
    {
        StopClient();
        startPanel.localScale = new Vector3(1, 1, 1);
        stopPanel.localScale = new Vector3(0, 1, 1);
    }

    public void QuitRoom()
    {
        quitRoomDelegate();
    }


    public void AddPlayer(LobbyPlayer player)
    {
        if (leftPlayer == null)
        {
            leftPlayer = player;
            player.transform.SetParent(leftContainer);
            player.transform.localPosition = new Vector3(0, 0, 0);
            player.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (rightPlayer == null)
        {
            rightPlayer = player;
            player.transform.SetParent(rightContainer);
            player.transform.localPosition = new Vector3(0, 0, 0);
            player.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void RemovePlayer(LobbyPlayer player)
    {
        if (leftPlayer != null && leftPlayer.Equals(player))
        {
            leftPlayer = null;
        }
        else if (rightPlayer != null && rightPlayer.Equals(player))
        {
            rightPlayer = null;
        }
    }
}

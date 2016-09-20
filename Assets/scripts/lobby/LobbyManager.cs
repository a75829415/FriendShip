using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

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
                default:
                    return null;
            }
        }
    }

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject obj = null;
        if (leftPlayer != null && leftPlayer.connectionToClient.connectionId == conn.connectionId)
        {
            obj = Instantiate(gamePlayerPrefab);
            obj.GetComponent<ShipController>().ControlMode = ShipControlMode.Left;
            Debug.Log("Left controller created.");
        }
        if (rightPlayer != null && rightPlayer.connectionToClient.connectionId == conn.connectionId)
        {
            obj = Instantiate(gamePlayerPrefab);
            obj.GetComponent<ShipController>().ControlMode = ShipControlMode.Right;
            Debug.Log("Right controller created.");
        }
        Instantiate(GameManager);
        return obj;
    }

    public override void OnLobbyClientExit()
    {
        ShowLobbyGUI();
        stopPanel.localScale = new Vector3(0, 0, 0);
    }

    public void ReturnLobby()
    {
        SendReturnToLobby();
        Destroy(Manager.instance);
    }

    public void ShowLobbyGUI()
    {
        startPanel.localScale = stopPanel.localScale = lobbyPanel.localScale = new Vector3(1, 1, 1);
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

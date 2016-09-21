using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIEventHandler : MonoBehaviour
{

    public static GUIEventHandler instance;

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

    public Manager GameManager
    {
        get
        {
            switch (LobbyManager.instance.Mode)
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
        LobbyManager.instance.Mode = GameMode.ClassicDouble;
        LobbyDiscovery.instance.StartBroadcasting();
        startPanel.localScale = new Vector3(0, 1, 1);
        stopPanel.localScale = new Vector3(1, 1, 1);
        quitRoomDelegate = QuitHostRoom;
        DontDestroyOnLoad(Instantiate(GameManager));
    }

    public void JoinRoom()
    {
        List<string> addresses = LobbyDiscovery.instance.GetServerAddresses();
        if (addresses.Count > 0)
        {
            //StartClient().Connect(addresses[0], networkPort);
            //StartListening();
        }
        else
        {
            Debug.LogError("No servers found.");
        }
        /*
        if (!string.IsNullOrEmpty(ip.text))
        {
            Mode = GameMode.ClassicDouble;
            StartClient().Connect(ip.text, networkPort);
            startPanel.localScale = new Vector3(0, 1, 1);
            stopPanel.localScale = new Vector3(1, 1, 1);
            quitRoomDelegate = QuitRemoteRoom;
            DontDestroyOnLoad(Instantiate(GameManager));
        }*/
    }

    public void SingleGame()
    {
        LobbyManager.instance.Mode = GameMode.ClassicSingle;
        LobbyManager.instance.StartHost();
        quitRoomDelegate = QuitHostRoom;
        DontDestroyOnLoad(Instantiate(GameManager));
    }

    public void QuitHostRoom()
    {
        //StopHost();
    }

    public void QuitRemoteRoom()
    {
        //StopClient();
    }

    public void QuitRoom()
    {
        quitRoomDelegate();
        startPanel.localScale = new Vector3(1, 1, 1);
        stopPanel.localScale = new Vector3(0, 1, 1);
        Destroy(Manager.instance.gameObject);
        LobbyDiscovery.instance.StopBroadcastingOrListening();
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

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

    private RectTransform leftPlayer;
    private RectTransform rightPlayer;

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
        //LobbyDiscovery.instance.StartBroadcasting();
        LobbyManager.instance.StartHost();
        startPanel.localScale = new Vector3(0, 1, 1);
        stopPanel.localScale = new Vector3(1, 1, 1);
        quitRoomDelegate = QuitHostRoom;
        DontDestroyOnLoad(Instantiate(LobbyManager.instance.GameManager));
    }

    public void JoinRoom()
    {
        //List<string> addresses = LobbyDiscovery.instance.GetServerAddresses();
        //if (addresses.Count > 0)
        //{
        //    StartClient().Connect(addresses[0], networkPort);
        //    StartListening();
        //}
        //else
        //{
        //    Debug.LogError("No servers found.");
        //}

        LobbyManager.instance.Mode = GameMode.ClassicDouble;
        LobbyManager.instance.StartClient().Connect("127.0.0.1", LobbyManager.instance.networkPort);
        startPanel.localScale = new Vector3(0, 1, 1);
        stopPanel.localScale = new Vector3(1, 1, 1);
        quitRoomDelegate = QuitRemoteRoom;
        DontDestroyOnLoad(Instantiate(LobbyManager.instance.GameManager));
    }

    public void SingleGame()
    {
        LobbyManager.instance.Mode = GameMode.ClassicSingle;
        LobbyManager.instance.StartHost();
        quitRoomDelegate = QuitHostRoom;
        DontDestroyOnLoad(Instantiate(LobbyManager.instance.GameManager));
    }

    public void QuitHostRoom()
    {
        LobbyManager.instance.StopHost();
    }

    public void QuitRemoteRoom()
    {
        LobbyManager.instance.StopClient();
    }

    public void QuitRoom()
    {
        quitRoomDelegate();
        startPanel.localScale = new Vector3(1, 1, 1);
        stopPanel.localScale = new Vector3(0, 1, 1);
        Destroy(Manager.instance.gameObject);
        //LobbyDiscovery.instance.StopBroadcastingOrListening();
    }

    public void AddPlayer(RectTransform playerInfo)
    {
        if (leftPlayer == null)
        {
            leftPlayer = playerInfo;
            playerInfo.SetParent(leftContainer);
            playerInfo.localPosition = new Vector3(0, 0, 0);
            playerInfo.localScale = new Vector3(1, 1, 1);
        }
        else if (rightPlayer == null)
        {
            rightPlayer = playerInfo;
            playerInfo.SetParent(rightContainer);
            playerInfo.localPosition = new Vector3(0, 0, 0);
            playerInfo.localScale = new Vector3(1, 1, 1);
        }
    }
}

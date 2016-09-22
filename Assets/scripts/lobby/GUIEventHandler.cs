using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUIEventHandler : MonoBehaviour
{

    public static GUIEventHandler instance;

    public RectTransform startPanel;
    public RectTransform stopPanel;
    public RectTransform lobbysPanel;
    public RectTransform gameLobbyPanel;
    public delegate void QuitRoomDelegate();
    public QuitRoomDelegate quitRoomDelegate;
    public RectTransform leftContainer;
    public RectTransform rightContainer;
    public RectTransform lobbyInfo;

    private RectTransform leftPlayer;
    private RectTransform rightPlayer;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        int i = 0;
        foreach (string address in LobbyDiscovery.instance.GetServerAddresses())
        {
            RectTransform lobby = Instantiate(lobbyInfo);
            lobbyInfo.SetParent(lobbysPanel);
            lobbyInfo.GetComponent<Image>().color = new Color(255, 255, 255, (i++ % 2) * 127);
            lobbyInfo.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                LobbyManager.instance.StartClient().Connect(
                    lobbyInfo.GetComponentInChildren<Text>().text,
                    LobbyManager.instance.networkPort);
            });
        }
    }

    public void CreateRoom()
    {
        LobbyManager.instance.Mode = GameMode.ClassicDouble;
        LobbyManager.instance.StartHost();
        LobbyDiscovery.instance.StartBroadcasting();
        startPanel.localScale = new Vector3(0, 1, 1);
        stopPanel.localScale = new Vector3(1, 1, 1);
        quitRoomDelegate = QuitHostRoom;
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
        if (LobbyDiscovery.instance.isServer)
        {
            LobbyDiscovery.instance.StopBroadcastingOrListening();
        }
        quitRoomDelegate();
        startPanel.localScale = new Vector3(1, 1, 1);
        stopPanel.localScale = new Vector3(0, 1, 1);
        Destroy(Manager.instance.gameObject);
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

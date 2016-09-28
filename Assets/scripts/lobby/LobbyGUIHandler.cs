using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyGUIHandler : MonoBehaviour
{
    public delegate void VoidDelegate();

    public static LobbyGUIHandler instance;
    public static VoidDelegate createRoomDelegate;
    public static VoidDelegate quitRoomDelegate;

    public RectTransform menuPanel;
    public RectTransform gameLobbyPanel;
    public RectTransform leftContainer;
    public RectTransform rightContainer;
    public RectTransform lobbyInfoPrefab;
    
    private RectTransform leftPlayer;
    private RectTransform rightPlayer;

    void Awake()
    {
        instance = this;
        if (createRoomDelegate != null)
        {
            createRoomDelegate();
            createRoomDelegate = null;
        }
    }

    public void QuitRoom()
    {
        quitRoomDelegate();
        SceneManager.LoadScene("welcome");
    }

    public void AddPlayer(RectTransform playerInfo)
    {
        if (leftPlayer == null)
        {
            leftPlayer = playerInfo;
            playerInfo.SetParent(leftContainer, false);
        }
        else if (rightPlayer == null)
        {
            rightPlayer = playerInfo;
            playerInfo.SetParent(rightContainer, false);
        }
    }
}

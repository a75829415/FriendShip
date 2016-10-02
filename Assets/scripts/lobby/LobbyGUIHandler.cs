using UnityEngine;

public class LobbyGUIHandler : MonoBehaviour
{
    public delegate void VoidDelegate();

    public static LobbyGUIHandler instance;
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
    }

    public void ShowLobbyGUI()
    {
        menuPanel.gameObject.SetActive(true);
        gameLobbyPanel.gameObject.SetActive(true);
    }

    public void HideLobbyGUI()
    {
        menuPanel.gameObject.SetActive(false);
        gameLobbyPanel.gameObject.SetActive(false);
    }

    public void QuitRoom()
    {
        quitRoomDelegate();
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

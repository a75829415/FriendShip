using UnityEngine;

public class LobbyGUIHandler : MonoBehaviour
{
    public delegate void VoidDelegate();

    public static LobbyGUIHandler instance;
    public static VoidDelegate quitRoomDelegate;
    
    public Canvas LobbyGUI;
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
        LobbyGUI.gameObject.SetActive(true);
    }

    public void HideLobbyGUI()
    {
        LobbyGUI.gameObject.SetActive(false);
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

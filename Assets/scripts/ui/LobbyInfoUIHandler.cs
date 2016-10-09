using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfoUIHandler : MonoBehaviour
{
    public static LobbyInfoUIHandler instance;

    public Sprite classicSprite;
    public Sprite competitveSprite;
    public Text addressText;
    public Text modeText;
    public Image lobbyInfoImage;

    private GameMode lobbyGameMode;

    void Awake()
    {
        instance = this;
    }

    public void Initialize(string address, string mode, string playerNumber)
    {
        addressText.text = address;
        switch (Convert.ToInt32(mode))
        {
            case (int)GameMode.Classic:
                modeText.text = "经典模式";
                lobbyInfoImage.sprite = classicSprite;
                lobbyGameMode = GameMode.Classic;
                break;
            case (int)GameMode.Competitive:
                modeText.text = "竞技模式";
                lobbyInfoImage.sprite = classicSprite;
                lobbyGameMode = GameMode.Competitive;
                break;
            default:
                modeText.text = "未知模式";
                lobbyInfoImage.sprite = classicSprite;
                lobbyGameMode = GameMode.Unset;
                break;
        }
        LobbyManager.instance.minPlayers = Convert.ToInt32(playerNumber);
    }

    // ---- UI event handlers ----
    public void OnLobbyInfoButtonClick()
    {
        LobbyManager.instance.Mode = lobbyGameMode;
        LobbyUISystemInitializer.instance.SetPanelToShow(LobbyUIHandler.instance.currentPanel);
        LobbyManager.instance.JoinGame(addressText.text);
        LobbyUIHandler.instance.Initialize(false, addressText.text);
        LobbyUIHandler.instance.ShowGUI(true);
        ChooseLobbyUIHandler.instance.ShowGUI(false);
    }
}

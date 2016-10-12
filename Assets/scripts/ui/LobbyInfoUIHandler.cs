using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfoUIHandler : MonoBehaviour
{
    public static LobbyInfoUIHandler instance;

    public Sprite classicSprite;
    public Sprite competitveSprite;
    public Sprite boomSprite;
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
                modeText.text = "对抗模式";
                lobbyInfoImage.sprite = competitveSprite;
                lobbyGameMode = GameMode.Competitive;
                break;
            case (int)GameMode.Boom:
                modeText.text = "爆破模式";
                lobbyInfoImage.sprite = boomSprite;
                lobbyGameMode = GameMode.Boom;
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
        LobbyUISystemInitializer.instance.SetPanelToShow(ChooseLobbyUIHandler.instance.currentPanel);
        LobbyManager.instance.JoinGame(addressText.text);
        LobbyUIHandler.instance.Initialize(false, addressText.text);
        LobbyUIHandler.instance.ShowGUI(true);
        ChooseLobbyUIHandler.instance.ShowGUI(false);
    }
}

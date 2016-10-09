using UnityEngine;
using UnityEngine.UI;

public class GameSettingUIHandler : UIHandlerBase
{
    public static GameSettingUIHandler instance;

    public Slider healthSlider;
    public Text healthValueText;
    public Slider playerNumberSlider;
    public Text playerNumberValueText;
    public Toggle classicToggle;
    public Toggle competitveToggle;

    void Awake()
    {
        instance = this;
    }

    public override void ShowGUI(bool showGUI)
    {
        base.ShowGUI(showGUI);
        if (showGUI)
        {
            classicToggle.isOn = Configuration.mode == GameMode.Classic;
            competitveToggle.isOn = Configuration.mode == GameMode.Competitive;
            healthSlider.value = Configuration.health;
            healthValueText.text = Configuration.health.ToString();
            playerNumberSlider.value = Configuration.indexOfPlayers;
            playerNumberValueText.text = Configuration.NumberOfPlayers.ToString();
        }
    }

    // ---- UI event handlers ----
    public void OnReturnButtonClick()
    {
        WelcomeUIHandler.instance.ShowGUI(true);
        ShowGUI(false);
    }

    public void OnCreateRoomButtonClick()
    {
        if (Configuration.playerNumberSet[(int)playerNumberSlider.value] == 4)
        {
            PopupUIHandler.instance.Popup("啊哦，我们的程序猿们还在日夜加班开发四人游戏模式(ง •_•)ง\n敬请期待");
            return;
        }
        Configuration.health = (uint)healthSlider.value;
        Configuration.indexOfPlayers = (int)playerNumberSlider.value;
        if (classicToggle.isOn)
        {
            LobbyManager.instance.Mode = Configuration.mode = GameMode.Classic;
        }
        else if (competitveToggle.isOn)
        {
            LobbyManager.instance.Mode = Configuration.mode = GameMode.Competitive;
        }
        LobbyManager.instance.minPlayers = Configuration.playerNumberSet[(int)playerNumberSlider.value];
        LobbyUIHandler.quitRoomDelegate = () =>
        {
            LobbyUISystemInitializer.instance.SetPanelToShow(WelcomeUIHandler.instance.currentPanel);
            WelcomeUIHandler.instance.ShowGUI(true);
            LobbyUIHandler.instance.ShowGUI(false);
        };
        if (LobbyManager.instance.minPlayers == 1)
        {
            LobbyUISystemInitializer.instance.SetPanelToShow(WelcomeUIHandler.instance.currentPanel);
        }
        else
        {
            LobbyUISystemInitializer.instance.SetPanelToShow(LobbyUIHandler.instance.currentPanel);
            LobbyUIHandler.instance.Initialize(true, string.Empty);
            LobbyUIHandler.instance.ShowGUI(true);
            ShowGUI(false);
        }
        LobbyManager.instance.StartGame();
    }

    public void OnHealthChange()
    {
        healthValueText.text = healthSlider.value.ToString();
    }

    public void OnPlayerNumberChange()
    {
        playerNumberValueText.text = Configuration.playerNumberSet[(int)playerNumberSlider.value].ToString();
    }
}

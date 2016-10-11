using UnityEngine;
using UnityEngine.UI;

public class GameSettingUIHandler : UIHandlerBase
{
    public static GameSettingUIHandler instance;

    public Slider healthSlider;
    public Text healthValueText;
    public Slider playerNumberSlider;
    public Text playerNumberValueText;
    public Slider invincibleTimeSlider;
    public Text invincibleTimeValueText;
    public Toggle enableMiniViewToggle;
    public RectTransform miniViewSubPanel;
    public Slider miniViewSizeSlider;
    public Text miniViewSizeValueText;
    public Toggle swapViewsToggle;
    public Toggle classicToggle;
    public Toggle competitveToggle;
    public Toggle boomToggle;

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
            boomToggle.isOn = Configuration.mode == GameMode.Boom;
            healthSlider.value = Configuration.health;
            playerNumberSlider.value = Configuration.indexOfPlayers;
            invincibleTimeSlider.value = Configuration.indexOfInvincibleTime;
            enableMiniViewToggle.isOn = Configuration.enableMiniView;
            miniViewSizeSlider.value = Configuration.indexOfMiniViewSize;
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
        LobbyManager.instance.Mode = Configuration.mode;
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
        Configuration.health = (uint)healthSlider.value; ;
    }

    public void OnPlayerNumberChange()
    {
        int index = (int)playerNumberSlider.value;
        Configuration.indexOfPlayers = index;
        playerNumberValueText.text = Configuration.NumberOfPlayers.ToString();
    }

    public void OnInvincibleTimeChange()
    {
        int index = (int)invincibleTimeSlider.value;
        Configuration.indexOfInvincibleTime = index;
        invincibleTimeValueText.text = Configuration.InvincibleTime + "s";
    }

    public void OnEnableMiniViewChange()
    {
        Configuration.enableMiniView = enableMiniViewToggle.isOn;
        miniViewSubPanel.gameObject.SetActive(enableMiniViewToggle.isOn);
    }

    public void OnMiniViewSizeChange()
    {
        int index = (int)miniViewSizeSlider.value;
        Configuration.indexOfMiniViewSize = index;
        miniViewSizeValueText.text = Configuration.MiniViewSizeText;
    }

    public void OnSwapViewsChange()
    {
        Configuration.swapViews = swapViewsToggle.isOn;
    }

    public void OnGameModeChange()
    {
        enableMiniViewToggle.interactable = true;
        if (classicToggle.isOn)
        {
            Configuration.mode = GameMode.Classic;
        }
        else if (competitveToggle.isOn)
        {
            Configuration.mode = GameMode.Competitive;
        }
        else if (boomToggle.isOn)
        {
            Configuration.mode = GameMode.Boom;
            enableMiniViewToggle.isOn = true;
            enableMiniViewToggle.interactable = false;
        }
    }
}

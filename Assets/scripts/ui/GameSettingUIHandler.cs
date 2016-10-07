using UnityEngine;
using UnityEngine.UI;

public class GameSettingUIHandler : MonoBehaviour
{
    public static GameSettingUIHandler instance;

    public RectTransform gameSettingPanel;
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

    public void ShowGUI(bool showGUI)
    {
        if (showGUI)
        {
            classicToggle.isOn = Configuration.mode == GameMode.Classic;
            competitveToggle.isOn = Configuration.mode == GameMode.Competitive;
            healthSlider.value = Configuration.health;
            healthValueText.text = Configuration.health.ToString();
            playerNumberSlider.value = Configuration.indexOfPlayers;
            playerNumberValueText.text = Configuration.NumberOfPlayers.ToString();
        }
        gameSettingPanel.gameObject.SetActive(showGUI);
    }

    // ---- UI event handlers ----
    public void OnReturnButtonClick()
    {
        WelcomeUIHandler.instance.ShowGUI(true);
        ShowGUI(false);
    }

    public void OnCreateRoomButtonClick()
    {
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
        LobbyUIHandler.instance.quitRoomDelegate = () =>
        {
            WelcomeUIHandler.instance.ShowGUI(true);
            LobbyUIHandler.instance.ShowGUI(false);
            LobbyManager.instance.StopGame();
        };
        LobbyManager.instance.StartGame();
        LobbyUIHandler.instance.Initialize(true, string.Empty);
        LobbyUIHandler.instance.ShowGUI(true);
        ShowGUI(false);
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

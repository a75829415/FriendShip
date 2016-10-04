using UnityEngine;
using UnityEngine.UI;

public class GameSettingHandler : MonoBehaviour
{
    public static GameSettingHandler instance;

    public RectTransform settingsPanel;
    public Slider healthSlider;
    public Text healthValueText;
    public Toggle classicToggle;
    public Toggle competitveToggle;
    public Toggle singleToggle;
    public Toggle doubleToggle;
    public Toggle quadrupleToggle;

    void Awake()
    {
        instance = this;
    }

    public void ShowSettings()
    {
        classicToggle.isOn = true;
        singleToggle.isOn = true;
        settingsPanel.gameObject.SetActive(true);
    }

    public void HideSettings()
    {
        settingsPanel.gameObject.SetActive(false);
    }

    public void OnHealthChange()
    {
        healthValueText.text = healthSlider.value.ToString();
    }

    // Remain to do: set health
    public void CreateRoom()
    {
        if (classicToggle.isOn)
        {
            if (singleToggle.isOn)
            {
                LobbyManager.instance.Mode = GameMode.ClassicSingle;
                WelcomeGUIHandler.instance.StartSingleGame();
            }
            else if (doubleToggle.isOn)
            {
                LobbyManager.instance.Mode = GameMode.ClassicDouble;
                WelcomeGUIHandler.instance.StartDoubleGame();
            }
            else if (quadrupleToggle.isOn)
            {
                // TODO: start quadruple game
            }
        }
        else if (competitveToggle.isOn)
        {
            if (singleToggle.isOn)
            {
                LobbyManager.instance.Mode = GameMode.CompetitiveSingle;
                WelcomeGUIHandler.instance.StartSingleGame();
            }
            else if (doubleToggle.isOn)
            {
                LobbyManager.instance.Mode = GameMode.CompetitiveDouble;
                WelcomeGUIHandler.instance.StartDoubleGame();
            }
            else if (quadrupleToggle.isOn)
            {
                // TODO: start quadruple game
            }
        }
        HideSettings();
    }
}

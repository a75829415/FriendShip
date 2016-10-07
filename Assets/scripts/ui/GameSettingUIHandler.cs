﻿using UnityEngine;
using UnityEngine.UI;

public class GameSettingUIHandler : MonoBehaviour
{
    public readonly int[] playerNumberSet = { 1, 2, 4 };

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
            classicToggle.isOn = true;
            competitveToggle.isOn = false;
            healthSlider.value = 3;
            healthValueText.text = "3";
            playerNumberSlider.value = 0;
            playerNumberValueText.text = "1";
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
        // set health: (int)healthSlider.value
        // ...
        if (classicToggle.isOn)
        {
            LobbyManager.instance.Mode = GameMode.Classic;
        }
        else if (competitveToggle.isOn)
        {
            LobbyManager.instance.Mode = GameMode.Competitive;
        }
        LobbyManager.instance.minPlayers = playerNumberSet[(int)playerNumberSlider.value];
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
        playerNumberValueText.text = playerNumberSet[(int)playerNumberSlider.value].ToString();
    }
}
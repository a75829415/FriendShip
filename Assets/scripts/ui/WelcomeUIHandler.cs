using UnityEngine;
using UnityEngine.UI;

public class WelcomeUIHandler : MonoBehaviour
{
    public static WelcomeUIHandler instance;
    
    public RectTransform welcomePanel;
    public Text titleText;
    public Button startGameButton;
    public Button newProductButton;
    public Button gameReferralButton;
    public Button contactUsButton;
    public RectTransform startGamePanel;
    public RectTransform newProductPanel;
    public RectTransform gameReferralPanel;
    public RectTransform contactUsPanel;
    
    void Awake()
    {
        instance = this;
    }

    public void ShowGUI(bool showGUI)
    {
        welcomePanel.gameObject.SetActive(showGUI);
    }

    // ---- UI event handlers ----
    public void OnStartGameButtonClick()
    {
        titleText.text = "开始游戏";
        startGamePanel.gameObject.SetActive(true);
        newProductPanel.gameObject.SetActive(false);
        gameReferralPanel.gameObject.SetActive(false);
        contactUsPanel.gameObject.SetActive(false);
    }

    public void OnNewProductButtonClick()
    {
        titleText.text = "敬请期待";
        startGamePanel.gameObject.SetActive(false);
        newProductPanel.gameObject.SetActive(true);
        gameReferralPanel.gameObject.SetActive(false);
        contactUsPanel.gameObject.SetActive(false);
        PopupUIHandler.instance.Popup("新功能很快就会上线哦o(*￣▽￣*)ブ");
    }

    public void OnGameReferralButtonClick()
    {
        titleText.text = "关于游戏";
        startGamePanel.gameObject.SetActive(false);
        newProductPanel.gameObject.SetActive(false);
        gameReferralPanel.gameObject.SetActive(true);
        contactUsPanel.gameObject.SetActive(false);
    }

    public void OnContactUsButtonClick()
    {
        titleText.text = "联系开发者";
        startGamePanel.gameObject.SetActive(false);
        newProductPanel.gameObject.SetActive(false);
        gameReferralPanel.gameObject.SetActive(false);
        contactUsPanel.gameObject.SetActive(true);
    }

    public void OnCreateRoomButtonClick()
    {
        GameSettingUIHandler.instance.ShowGUI(true);
        ShowGUI(false);
    }

    public void OnJoinRoomButtonClick()
    {
        ChooseLobbyUIHandler.instance.ShowGUI(true);
        ShowGUI(false);
    }
}

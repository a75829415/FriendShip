using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUIHandler : MonoBehaviour
{
    public delegate void VoidDelegate();
    public VoidDelegate playerReady;
    public VoidDelegate playerNotReady;
    public VoidDelegate sendPlayerReady;
    public VoidDelegate sendPlayerNotReady;
    public Sprite unknownPlayerSprite;
    public Sprite leftPlayerSprite;
    public Sprite rightPlayerSprite;
    public Sprite leftPlayerReadySprite;
    public Sprite rightPlayerReadySprite;
    public Text playerText;
    public Image playerImage;
    public Button playerReadyButton;
    public Button playerCancelButton;

    private int playerIndex;
    public int PlayerIndex
    {
        get
        {
            return playerIndex;
        }
        set
        {
            if ((playerIndex = value) % 2 == 1)
            {
                playerReady = () => { playerImage.sprite = leftPlayerReadySprite; };
                playerNotReady = () => { playerImage.sprite = leftPlayerSprite; };
            }
            else
            {
                playerReady = () => { playerImage.sprite = rightPlayerReadySprite; };
                playerNotReady = () => { playerImage.sprite = rightPlayerSprite; };
            }
        }
    }

    void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        playerImage.sprite = unknownPlayerSprite;
        playerReadyButton.gameObject.SetActive(false);
        playerCancelButton.gameObject.SetActive(false);
    }

    public void ShowReadyButton()
    {
        playerReadyButton.gameObject.SetActive(true);
    }

    public void SetPlayer(int index)
    {
        playerText.text = "Player " + index;
        PlayerIndex = index;
    }

    public void SetPlayerReady(bool ready)
    {
        if (ready)
        {
            playerReady();
        }
        else
        {
            playerNotReady();
        }
    }

    // ---- UI event handlers ----
    public void OnPlayerReadyButtonClick()
    {
        playerReady();
        sendPlayerReady();
        playerCancelButton.gameObject.SetActive(true);
        playerReadyButton.gameObject.SetActive(false);
    }

    public void OnPlayerCancelButtonClick()
    {
        playerNotReady();
        sendPlayerNotReady();
        playerReadyButton.gameObject.SetActive(true);
        playerCancelButton.gameObject.SetActive(false);
    }

    // ---- UI event hooks ----
    public void PlayerEnter()
    {
        playerNotReady();
    }

    public void PlayerExit()
    {
        LobbyUIHandler.instance.PlayerExit(GetComponent<RectTransform>());
    }
}

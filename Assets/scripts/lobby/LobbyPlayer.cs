using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer
{
    public RectTransform playerInfoPrefab;

    private RectTransform playerInfo;
    private Text playerNameText;
    private Button playerStateButton;

    public override void OnClientEnterLobby()
    {
        if (playerInfo == null)
        {
            playerInfo = Instantiate(playerInfoPrefab);
            playerNameText = playerInfo.GetComponentInChildren<Text>();
            playerStateButton = playerInfo.GetComponentInChildren<Button>();
            LobbyGUIHandler.instance.AddPlayer(playerInfo);
        }
        readyToBegin = false;
        if (isLocalPlayer)
        {
            SetupLocalPlayer();
        }
        else
        {
            SetupOtherPlayer();
        }
    }

    public override void OnStartAuthority()
    {
        SetupLocalPlayer();
        if (LobbyManager.instance.Mode == GameMode.ClassicSingle)
        {
            SendReadyToBeginMessage();
        }
    }

    public override void OnClientReady(bool readyState)
    {
        playerStateButton.GetComponentInChildren<Text>().text = readyState ? "就    绪" : "等    待";
    }

    private void SetupOtherPlayer()
    {
        playerNameText.text = "Remote Player";
        playerStateButton.GetComponentInChildren<Text>().text = readyToBegin ? "就    绪" : "等    待";
        playerStateButton.interactable = false;
    }

    private void SetupLocalPlayer()
    {
        playerNameText.text = "You (Local Player)";
        playerStateButton.GetComponentInChildren<Text>().text = readyToBegin ? "就    绪" : "等    待";
        playerStateButton.interactable = false;
        WelcomeGUIHandler.instance.readyButton.GetComponentInChildren<Text>().text = "准    备";
        WelcomeGUIHandler.instance.readyButton.onClick.RemoveAllListeners();
        WelcomeGUIHandler.instance.readyButton.onClick.AddListener(ChangeReadyState);
    }

    void OnDestroy()
    {
        if (playerInfo != null)
        {
            Destroy(playerInfo.gameObject);
        }
    }

    public void ChangeReadyState()
    {
        Text readyText = WelcomeGUIHandler.instance.readyButton.GetComponentInChildren<Text>();
        if (readyText.text.Equals("准    备"))
        {
            SendReadyToBeginMessage();
            readyText.text = "取消准备";
        }
        else
        {
            SendNotReadyToBeginMessage();
            readyText.text = "准    备";
        }
    }

    [ClientRpc]
    public void RpcReturnLobby()
    {
        LobbyGUIHandler.instance.ShowLobbyGUI();
        if (NetHub.instance != null)
        {
            Destroy(NetHub.instance.gameObject);
        }
    }

    [ClientRpc]
    public void RpcGameStart()
    {
        WelcomeGUIHandler.instance.HideWelcomeGUI();
        LobbyGUIHandler.instance.HideLobbyGUI();
    }
}

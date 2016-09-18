using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShipController : NetworkBehaviour
{
    private Ship ship;

    // Use this for initialization
    void Start()
    {
        ship = GameObject.FindGameObjectWithTag("Player").GetComponent<Ship>();
        LobbyManager.instance.HideLobbyGUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (LobbyManager.instance.Mode == GameMode.ClassicSingle)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CmdPaddleLeft();
            }
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                CmdPaddleRight();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isServer)
                {
                    CmdPaddleLeft();
                }
                else
                {
                    CmdPaddleRight();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void OnApplicationQuit()
    {
        LobbyManager.instance.SendReturnToLobby();
        LobbyManager.instance.ShowLobbyGUI();
        LobbyManager.instance.quitRoomDelegate();
        Application.CancelQuit();
    }

    [ClientRpc]
    public void RpcPaddleLeft()
    {
        ship.PaddleLeft();
    }

    [ClientRpc]
    public void RpcPaddleRight()
    {
        ship.PaddleRight();
    }

    [Command]
    public void CmdPaddleLeft()
    {
        RpcPaddleLeft();
    }

    [Command]
    public void CmdPaddleRight()
    {
        RpcPaddleRight();
    }
}

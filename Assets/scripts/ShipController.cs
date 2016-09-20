using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShipController : NetworkBehaviour
{
    public ShipControlMode ControlMode { get; set; }

    private Ship ship;

    // Use this for initialization
    void Start()
    {
        ship = Manager.instance.ship/*GameObject.FindGameObjectWithTag("Player").GetComponent<Ship>()*/;
        LobbyManager.instance.HideLobbyGUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (ControlMode == ShipControlMode.Whole)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Input.mousePosition.x < Screen.width / 2)
                {
                    CmdPaddleLeft();
                }
                else
                {
                    CmdPaddleRight();
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CmdPaddleLeft();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                CmdPaddleRight();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (ControlMode == ShipControlMode.Left)
                {
                    CmdPaddleLeft();
                }
                else if (ControlMode == ShipControlMode.Right)
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

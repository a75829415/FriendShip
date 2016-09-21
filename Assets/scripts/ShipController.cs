using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShipController : NetworkBehaviour
{
    private Ship ship;
    private ShipControlMode controlMode;

    // Use this for initialization
    void Start()
    {
        ship = Manager.instance.ship/*GameObject.FindGameObjectWithTag("Player").GetComponent<Ship>()*/;
        if (isLocalPlayer)
        {
            controlMode = LobbyManager.instance.GetShipControlMode(connectionToServer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (controlMode == ShipControlMode.BothPaddles)
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
                if (controlMode == ShipControlMode.LeftPaddleOnly)
                {
                    CmdPaddleLeft();
                }
                else if (controlMode == ShipControlMode.RightPaddleOnly)
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
        GUIEventHandler.instance.ShowLobbyGUI();
        GUIEventHandler.instance.quitRoomDelegate();
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

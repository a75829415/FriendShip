﻿using UnityEngine;
using UnityEngine.Networking;

public class ShipController : NetworkBehaviour
{
    private Ship ship;

    [SyncVar]
    private ShipControlMode controlMode;

	public static bool ModeIsFireable(ShipControlMode mode)
	{
		return mode == ShipControlMode.FireOnly;
	}

    // Use this for initialization
    void Start()
    {
        ship = Manager.instance.ship;
        if (isServer)
        {
            controlMode = LobbyManager.instance.GetShipControlMode(connectionToClient);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        Manager.instance.NotifyControlMode(controlMode);
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

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShipController : NetworkBehaviour
{
    public Canvas joystickGUI;
    public Slider viewChangeSlider;

    private Ship ship;

    [SyncVar(hook = "OnControlModeChange")]
    private ShipControlMode controlMode;

    public static bool ModeIsFireable(ShipControlMode mode)
    {
        return mode == ShipControlMode.FireOnly || mode == ShipControlMode.BothPaddlesAndFire;
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
        switch (controlMode)
        {
            case ShipControlMode.BothPaddles:
            case ShipControlMode.BothPaddlesAndFire:
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
                break;
            case ShipControlMode.LeftPaddleOnly:
                if (Input.GetMouseButtonDown(0))
                {
                    CmdPaddleLeft();
                }
                break;
            case ShipControlMode.RightPaddleOnly:
                if (Input.GetMouseButtonDown(0))
                {
                    CmdPaddleRight();
                }
                break;
            case ShipControlMode.FireOnly:
                if (Input.GetMouseButtonDown(0))
                {
                    // TODO: control behavior
                }
                break;
        }
    }

    // ---- sync val hook ----
    public void OnControlModeChange(ShipControlMode mode)
    {
        joystickGUI.gameObject.SetActive(ModeIsFireable(mode));
    }

    // ---- joystick event handlers ----
    public void OnViewChange()
    {
        ship.boomerElevation = viewChangeSlider.value;
    }

    public void OnFire()
    {
        ship.BoomABoomer();
    }

    // ---- commands ----
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

    // ---- client rpc calls ----
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
}

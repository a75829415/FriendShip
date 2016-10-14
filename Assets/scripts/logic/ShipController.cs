#if UNITY_ANDROID && !UNITY_EDITOR
#define ANDROID
#endif
#if UNITY_IPHONE && !UNITY_EDITOR
#define IPHONE
#endif

using UnityEngine;
using UnityEngine.EventSystems;
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
        if (ModeIsFireable(controlMode))
        {
            ship.boomerElevation = viewChangeSlider.value;
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
#if IPHONE || ANDROID
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    {
                        if (Input.GetTouch(0).rawPosition.x < Screen.width / 2)
#else
                if (Input.GetMouseButtonDown(0))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        if (Input.mousePosition.x < Screen.width / 2)
#endif
                        {
                            CmdPaddleLeft();
                        }
                        else
                        {
                            CmdPaddleRight();
                        }
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
#if IPHONE || ANDROID
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began
                    && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#else
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
#endif
                {
                    // TODO: control behavior
                }
                break;
        }
    }

    public void ShowJoystick(bool showJoystick)
    {
        joystickGUI.gameObject.SetActive(showJoystick);
    }

    // ---- sync val hook ----
    public void OnControlModeChange(ShipControlMode mode)
    {
        if (isLocalPlayer)
        {
            ShowJoystick(ModeIsFireable(mode));
        }
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

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShipController : NetworkBehaviour
{
    public bool isSingleGame = false;

    private Ship ship;

    // Use this for initialization
    void Start()
    {
        ship = GameObject.FindGameObjectWithTag("Player").GetComponent<Ship>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(isLocalPlayer);
        if (isSingleGame)
        {
            if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.LeftArrow))
            {
                CmdPaddleLeft();
            }
            if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.RightArrow))
            {
                CmdPaddleRight();
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
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

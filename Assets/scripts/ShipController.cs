using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShipController : NetworkBehaviour
{
    private Transform ship;

    // Use this for initialization
    void Start()
    {
        ship = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            CmdRotate((isServer ? 5 : -5) * Time.deltaTime);
        }
    }

    [ClientRpc]
    public void RpcRotate(float angle)
    {
        ship.Rotate(new Vector3(0, -1, 0), angle);
    }

    [Command]
    public void CmdRotate(float angle)
    {
        RpcRotate(angle);
    }
}

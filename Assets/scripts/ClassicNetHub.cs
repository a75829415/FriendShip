using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ClassicNetHub : NetHub {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[ClientRpc]
	public void RpcUpdateStatus(uint serverHealth)
	{
		((ClassicManager)(Manager.instance)).health = serverHealth;
    }

	[ClientRpc]
	public void RpcNotifyGameOver(float result)
	{
		((ClassicManager)(Manager.instance)).GameOver(result);
	}

}

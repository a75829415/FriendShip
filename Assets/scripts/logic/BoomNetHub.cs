using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BoomNetHub : NetHub {
	public BoomManager boomManagerPrototype;

	void Awake()
	{
		base.AwakeWorkaround();
		DontDestroyOnLoad(Instantiate(boomManagerPrototype));
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	[ClientRpc]
	public void RpcUpdateStatus(uint serverCurrentHealth, uint serverHealth, uint serverScore)
	{
		if (!isServer)
		{
			((BoomManager)(Manager.instance)).currentHealth = serverCurrentHealth;
			((BoomManager)(Manager.instance)).health = serverHealth;
			((BoomManager)(Manager.instance)).score = serverScore;
        }
	}

	[ClientRpc]
	public void RpcNotifyGameOver(float result, uint score)
	{
		((BoomManager)(Manager.instance)).GameOver(result, score);
	}

}

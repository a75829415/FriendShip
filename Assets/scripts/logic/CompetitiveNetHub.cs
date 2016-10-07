using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CompetitiveNetHub : NetHub {
	public CompetitiveManager competetiveManagerPrototype;

	// Use this for initialization
	void Awake()
	{
		base.AwakeWorkaround();
		DontDestroyOnLoad(Instantiate(competetiveManagerPrototype));
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
	public void RpcUpdateStatus(uint serverCurrentLeftHealth, uint serverLeftHealth, uint serverCurrentRightHealth, uint serverRightHealth)
	{
		if (!isServer)
		{
			((CompetitiveManager)(Manager.instance)).currentLeftHealth = serverCurrentLeftHealth;
			((CompetitiveManager)(Manager.instance)).leftHealth = serverLeftHealth;
			((CompetitiveManager)(Manager.instance)).currentRightHealth = serverCurrentRightHealth;
			((CompetitiveManager)(Manager.instance)).rightHealth = serverRightHealth;
		}
	}

	[ClientRpc]
	public void RpcNotifyGameOver(float time, uint leftHealth, uint rightHealth)
	{
		((CompetitiveManager)(Manager.instance)).GameOver(time, leftHealth, rightHealth);
	}

}

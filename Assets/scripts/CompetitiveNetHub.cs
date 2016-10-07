﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CompetitiveNetHub : NetHub {
	public CompetitiveNetHub competetiveManagerPrototype;

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
	public void RpcUpdateStatus(uint serverLeftHealth, uint serverRightHealth)
	{
		if (!isServer)
		{
			((CompetitiveManager)(Manager.instance)).currentLeftHealth = serverLeftHealth;
			((CompetitiveManager)(Manager.instance)).currentRightHealth = serverRightHealth;
		}
	}

	[ClientRpc]
	public void RpcNotifyGameOver(float time, uint leftHealth, uint rightHealth)
	{
		((CompetitiveManager)(Manager.instance)).GameOver(time, leftHealth, rightHealth);
	}

}

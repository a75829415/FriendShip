﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ClassicManager : Manager
{
	public delegate void GameOverHandler(ClassicManager manager, float time);

	public ShipCollider classicShipColliderProtocal;

	public ShipCollider shipCollider;

	public GameOverHandler gameOverHandler = DefaultGameOverHandler;

	public uint health;

	public uint currentHealth;

	public override bool IsObstacle(Collider collider)
	{
		return !System.Object.ReferenceEquals(shipCollider, collider);
	}

	public override bool IsGaming()
	{
		return currentHealth > 0 && ship != null;
    }

	public override GameMode GetGameMode()
	{
		return GameMode.Classic;
	}

	void Awake()
	{
		base.AwakeWorkaround();
	}

	// Use this for initialization
	void Start()
	{
		base.StartWorkaround();
		if (NetHub.instance.isServer)
		{
			health = Configuration.health;
			currentHealth = health;
		}
		UpdateClient();
    }
	
	// Update is called once per frame
	void Update()
	{
		base.UpdateWorkaround();
    }

	public override void InitializeShipCollider()
	{
		shipCollider = Instantiate(classicShipColliderProtocal);
		shipCollider.reservedTransform.SetParent(ship.reservedTransform);
	}

	public override void NotifyCrash(ShipCollider shipCollider, Collider obstacleCollider)
	{
		base.NotifyCrash(shipCollider, obstacleCollider);
		if (NetHub.instance.isServer && --currentHealth == 0)
		{
			UpdateClient();
			((ClassicNetHub)(NetHub.instance)).RpcNotifyGameOver(GameTime);
		}
	}

	public override void UpdateClient()
	{
		base.UpdateClient();
		if (NetHub.instance.isServer)
		{
			((ClassicNetHub)(NetHub.instance)).RpcUpdateStatus(currentHealth, health);
		}
	}

	public void GameOver(float time)
	{
		Time.timeScale = 0.0f;
		gameOverHandler(this, time);
	}

	public static void DefaultGameOverHandler(ClassicManager manager, float time)
	{
		manager.resultStringHandler("时间：" + TimeToString(time));
    }

}

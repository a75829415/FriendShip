using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ClassicManager : Manager
{
	public delegate void GameOverHandler(ClassicManager manager, float time);

	public Collider classicShipColliderProtocal;

	public RectTransform hud;
	public Text hudTime;
	public Text hudHealth;

	public GameOverHandler gameOverHandler = DefaultGameOverHandler;

	public uint health;

	public uint currentHealth;

	public override bool IsGaming()
	{
		return currentHealth > 0 && ship != null;
    }

	void Awake()
	{
		base.AwakeWorkaround();
		hud.gameObject.SetActive(false);
	}

	// Use this for initialization
	void Start()
	{
		base.StartWorkaround();
		currentHealth = health;
    }
	
	// Update is called once per frame
	void Update()
	{
		base.UpdateWorkaround();
		if (!hud.gameObject.activeSelf && IsGaming())
		{
			uiCanvas.worldCamera = playerCamera;
			hud.gameObject.SetActive(true);
		}
		hudTime.text = TimeToString(GameTime);
		hudHealth.text = currentHealth + "/" + health;
    }

	public override void InitializeShipCollider()
	{
		Instantiate(classicShipColliderProtocal).GetComponent<Transform>().SetParent(ship.reservedTransform);
	}

	public override void NotifyCrash(Collider shipCollider, Collider obstacleCollider)
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
		((ClassicNetHub)(NetHub.instance)).RpcUpdateStatus(currentHealth);
	}

	public void GameOver(float time)
	{
		Time.timeScale = 0.0f;
		hud.gameObject.SetActive(false);
		gameOverHandler(this, time);
	}

	public static void DefaultGameOverHandler(ClassicManager manager, float time)
	{
		manager.gameOverPanel.gameObject.SetActive(true);
		manager.resultText.text = "时间: " + TimeToString(time);
    }

}

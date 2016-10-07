using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class CompetitiveManager : Manager {
	public delegate void GameOverHandler(CompetitiveManager manager, float time, uint leftHealth, uint rightHealth);

	public GameOverHandler gameOverHandler = DefaultGameOverHandler;

	public uint leftHealth;
	public uint rightHealth;

	public ShipCollider competetiveShipColliderProtocal;

	public ShipCollider leftCollider;
	public ShipCollider rightCollider;

	public RectTransform hud;
	public Text hudLeftHealthValue;
	public Text hudRightHealthValue;

	public uint currentLeftHealth;
	public uint currentRightHealth;

	public override bool IsGaming()
	{
		return currentLeftHealth > 0 && currentRightHealth > 0 && ship != null;
    }

	void Awake()
	{
		base.AwakeWorkaround();
		hud.gameObject.SetActive(false);
	}

	// Use this for initialization
	void Start () {
		base.StartWorkaround();
		if (NetHub.instance.isServer)
		{
			leftHealth = Configuration.health;
			rightHealth = Configuration.health;
			currentLeftHealth = leftHealth;
			currentRightHealth = rightHealth;
		}
		UpdateClient();
	}
	
	// Update is called once per frame
	void Update () {
		base.UpdateWorkaround();
		if (!hud.gameObject.activeSelf && IsGaming())
		{
			uiCanvas.worldCamera = playerCamera;
			hud.gameObject.SetActive(true);
		}
		hudLeftHealthValue.text = currentLeftHealth + "/" + leftHealth;
		hudRightHealthValue.text = currentRightHealth + "/" + rightHealth;
	}

	public override void InitializeShipCollider()
	{
		leftCollider = Instantiate(competetiveShipColliderProtocal);
		leftCollider.reservedTransform.position = new Vector3(-0.5f, 0, 0);
        leftCollider.reservedTransform.SetParent(ship.reservedTransform);
		rightCollider = Instantiate(competetiveShipColliderProtocal);
		rightCollider.reservedTransform.position = new Vector3(0.5f, 0, 0);
		rightCollider.reservedTransform.SetParent(ship.reservedTransform);
	}

	public override void NotifyCrash(ShipCollider shipCollider, Collider obstacleCollider)
	{
		crashHandler(shipCollider, obstacleCollider);
		if (NetHub.instance.isServer)
		{
			if (System.Object.ReferenceEquals(shipCollider, leftCollider))
			{
				--currentLeftHealth;
			}
			else if (System.Object.ReferenceEquals(shipCollider, rightCollider))
			{
				--currentRightHealth;
			}

		}
		if (currentLeftHealth == 0 || currentRightHealth == 0)
		{
			((CompetitiveNetHub)(NetHub.instance)).RpcNotifyGameOver(GameTime, currentLeftHealth, currentRightHealth);
		}
	}

	public override void UpdateClient()
	{
		base.UpdateClient();
		if (NetHub.instance.isServer)
		{
			((CompetitiveNetHub)(NetHub.instance)).RpcUpdateStatus(currentLeftHealth, leftHealth, currentRightHealth, rightHealth);
		}
    }

	public void GameOver(float time, uint lHealth, uint rHealth)
	{
		Time.timeScale = 0.0f;
		hud.gameObject.SetActive(false);
		gameOverHandler(this, time, lHealth, rHealth);
    }

	public static void DefaultGameOverHandler(CompetitiveManager manager, float time, uint lHealth, uint rHealth)
	{
		manager.gameOverPanel.gameObject.SetActive(true);
		string result = "获胜方: ";
		if (lHealth > 0)
		{
			result += "左边的小伙伴";
		}
		else if (rHealth > 0)
		{
			result += "右边的小伙伴";
		}
		else
		{
			result = "平局";
		}
        manager.resultText.text = result;
	}

}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class CompetitiveManager : Manager {
	public delegate void GameOverHandler(CompetitiveManager manager, float time, uint leftHealth, uint rightHealth);

	public GameOverHandler gameOverHandler = DefaultGameOverHandler;

	public uint leftHealth;
	public uint rightHealth;

	public Collider competetiveShipColliderProtocal;

	public Collider leftCollider;
	public Collider rightCollider;

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
		currentLeftHealth = leftHealth;
		currentRightHealth = rightHealth;
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
		leftCollider.GetComponent<Transform>().position = new Vector3(-0.5f, 0, 0);
        leftCollider.GetComponent<Transform>().SetParent(ship.reservedTransform);
		rightCollider = Instantiate(competetiveShipColliderProtocal);
		rightCollider.GetComponent<Transform>().position = new Vector3(0.5f, 0, 0);
		rightCollider.GetComponent<Transform>().SetParent(ship.reservedTransform);
	}

	public override void NotifyCrash(Collider shipCollider, Collider obstacleCollider)
	{
		crashHandler(shipCollider, obstacleCollider);
		if (NetHub.instance.isServer)
		{
			if (UnityEngine.Object.ReferenceEquals(shipCollider, leftCollider))
			{
				--currentLeftHealth;
			}
			else
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
		((CompetitiveNetHub)(NetHub.instance)).RpcUpdateStatus(currentLeftHealth, currentRightHealth);
    }

	public void GameOver(float time, uint lHealth, uint rHealth)
	{
		Time.timeScale = 0.0f;
		hud.gameObject.SetActive(false);
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

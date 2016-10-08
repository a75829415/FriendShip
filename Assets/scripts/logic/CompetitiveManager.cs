﻿using UnityEngine;
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
	public Text hudLeftHealth;
	public Text hudLeftHealthValue;
	public Text hudRightHealth;
	public Text hudRightHealthValue;

	public uint currentLeftHealth;
	public uint currentRightHealth;

	public enum Winner
	{
		left,
		right,
		draw
	}

	public override bool IsGaming()
	{
		return currentLeftHealth > 0 && currentRightHealth > 0 && ship != null;
    }

	public bool IsSinglePlayerMode()
	{
		return localControlMode == ShipControlMode.BothPaddles;
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

	public override void NotifyControlMode(ShipControlMode controlMode)
	{
		base.NotifyControlMode(controlMode);
		if (IsPaddlingLeft())
		{
			hudLeftHealth.color = Color.red;
			hudRightHealth.color = Color.black;
		}
		else if (IsPaddlingRight())
		{
			hudLeftHealth.color = Color.black;
			hudRightHealth.color = Color.red;
		}
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
			if (System.Object.ReferenceEquals(shipCollider, rightCollider))
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

	public static Winner DeterminesWinner(uint lHealth, uint rHealth)
	{
		if (rHealth < lHealth)
		{
			return Winner.left;
		}
		else if (lHealth < rHealth)
		{
			return Winner.right;
		}
		return Winner.draw;
	}

	public bool IsWinner(Winner winner)
	{
		return winner == Winner.left && IsPaddlingLeft();
	}

	public bool IsLoser(Winner winner)
	{
		return winner == Winner.right && IsPaddlingRight();
	}

	public static void DefaultGameOverHandler(CompetitiveManager manager, float time, uint lHealth, uint rHealth)
	{
		Debug.Log(lHealth + ", " + rHealth);
		CompetitiveManager competitiveManager = (CompetitiveManager)(manager);
        manager.gameOverPanel.gameObject.SetActive(true);
		string result;
		Winner winner = DeterminesWinner(lHealth, rHealth);
		if (competitiveManager.IsWinner(winner))
		{
			result = "胜利";
		}
		else if (competitiveManager.IsLoser(winner))
		{
			result = "失败";
		}
		else
		{
			if (winner == Winner.draw)
			{
				result = "平局";
			}
			else if (competitiveManager.IsSinglePlayerMode())
			{
				result = "获胜方：";
				if (winner == Winner.left)
				{
					result += "左侧的小伙伴";
				}
				else if (winner == Winner.right)
				{
					result += "右侧的小伙伴";
                }
			}
			else
			{
				throw new System.Exception("Conflict result");
			}
		}
        manager.resultText.text = result;
	}

}
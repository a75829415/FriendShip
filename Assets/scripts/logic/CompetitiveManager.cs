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

	public uint currentLeftHealth;
	public uint currentRightHealth;

	public enum Winner
	{
		left,
		right,
		draw
	}

	public override bool IsObstacle(Collider collider)
	{
		return !System.Object.ReferenceEquals(leftCollider, collider) && !System.Object.ReferenceEquals(rightCollider, collider);
	}

	public override bool IsGaming()
	{
		return currentLeftHealth > 0 && currentRightHealth > 0 && ship != null;
    }

	public override GameMode GetGameMode()
	{
		return GameMode.Competitive;
	}

	public bool IsSinglePlayerMode()
	{
		return GetControlMode() == ShipControlMode.BothPaddles;
	}

	void Awake()
	{
		base.AwakeWorkaround();
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
	}

	public override void InitializeShipCollider()
	{
		leftCollider = Instantiate(competetiveShipColliderProtocal);
		leftCollider.reservedTransform.position = new Vector3(-0.25f, 0, 0);
        leftCollider.reservedTransform.SetParent(ship.reservedTransform);
		rightCollider = Instantiate(competetiveShipColliderProtocal);
		rightCollider.reservedTransform.position = new Vector3(0.25f, 0, 0);
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

	public bool WinAsLeft(Winner winner)
	{
		return winner == Winner.left && IsPaddlingLeft();
    }

	public bool WinAsRight(Winner winner)
	{
		return winner == Winner.right && IsPaddlingRight();
	}

	public bool IsWinner(Winner winner)
	{
		return WinAsLeft(winner) || WinAsRight(winner);
	}

	public bool LoseAsLeft(Winner winner)
	{
		return winner == Winner.right && IsPaddlingLeft();
	}

	public bool LoseAsRight(Winner winner)
	{
		return winner == Winner.left && IsPaddlingRight();
	}

	public bool IsLoser(Winner winner)
	{
		return LoseAsLeft(winner) || LoseAsRight(winner);
	}

	public static void DefaultGameOverHandler(CompetitiveManager manager, float time, uint lHealth, uint rHealth)
	{
		Debug.Log(lHealth + ", " + rHealth);
		CompetitiveManager competitiveManager = (CompetitiveManager)(manager);
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
		manager.resultStringHandler(result);
	}

}

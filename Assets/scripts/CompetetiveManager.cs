using UnityEngine;
using System.Collections;
using System;

public class Competetive : Manager {
	public delegate void GameOverHandler(uint leftHealth, uint rightHealth);

	public GameOverHandler gameOverHandler = DefaultGameOverHandler;

	public uint leftHealth;
	public uint rightHealth;

	public Collider competetiveShipColliderProtocal;

	public Collider leftCollider;
	public Collider rightCollider;

	private uint currentLeftHealth;
	private uint currentRightHealth;

	// Use this for initialization
	void Start () {
		base.StartWorkaround();
	}
	
	// Update is called once per frame
	void Update () {
    }

	public override void InitializeShipCollider()
	{
		leftCollider = Instantiate(competetiveShipColliderProtocal);
		leftCollider.GetComponent<Transform>().SetParent(ship.reservedTransform);
		
	}

	public override void NotifyCrash(Collider shipCollider, Collider obstacleCollider)
	{
		crashHandler(shipCollider, obstacleCollider);
		if (UnityEngine.Object.ReferenceEquals(shipCollider, leftCollider))
		{
			--currentLeftHealth;
		}
		else
		{
			--currentRightHealth;
		}
		if (currentLeftHealth == 0 || currentRightHealth == 0)
		{
			gameOverHandler(currentLeftHealth, currentRightHealth);
		}
	}

	public static void DefaultGameOverHandler(uint leftHealth, uint rightHealth)
	{
	}

}

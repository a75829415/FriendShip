using UnityEngine;
using System.Collections;

public class ClassicManager : Manager
{
	public delegate void GameOverHandler(float time);

	public GameOverHandler gameOverHandler = DefaultGameOverHandler;

	public uint health;

	private uint currentHealth;

	void Awake()
	{
		base.AwakeWorkaround();
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
	}

	public override void NotifyCrash(Collider shipCollider, Collider obstacleCollider)
	{
		crashHandler(shipCollider, obstacleCollider);
		if (--currentHealth == 0)
		{
			gameOverHandler(GameTime);
		}
	}

	public static void DefaultGameOverHandler(float time)
	{
	}

}

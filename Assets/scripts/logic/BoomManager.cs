using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoomManager : Manager {
	public delegate void GameOverHandler(BoomManager manager, float time, uint score);

	public ShipCollider classicShipColliderProtocal;

	public ShipCollider shipCollider;

	public RectTransform hud;
	public Text hudScore;
	public Text hudHealth;

	public GameOverHandler gameOverHandler = DefaultGameOverHandler;

	public uint health;

	public uint currentHealth;

	public uint score;

	public override bool IsObstacle(Collider collider)
	{
		return !System.Object.ReferenceEquals(shipCollider, collider);
	}

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
		if (NetHub.instance.isServer)
		{
			health = Configuration.health;
			currentHealth = health;
			score = 0;
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

	public void Scores()
	{
		++score;
	}

	public void GameOver(float time, uint score)
	{
		Time.timeScale = 0.0f;
		hud.gameObject.SetActive(false);
		gameOverHandler(this, time, score);
	}

	public static void DefaultGameOverHandler(BoomManager manager, float time, uint score)
	{
		manager.resultStringHandler("得分：" + score.ToString());
	}

}

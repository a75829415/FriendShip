using UnityEngine;
using System.Collections;

public class ShipCollider : MoveableObject
{
	public Collider reservedCollider;

	public delegate void CrashNotificationHandler(ShipCollider shipCollider, Collider obstacleCollider);

	public CrashNotificationHandler crashNotificationHandler = DefaultCrashNotificationHandler;

	void Awake()
	{
		base.AwakeWorkaround();
	}

	void Start()
	{
		base.StartWorkaround();
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnTriggerEnter(Collider other)
	{
		if (!Manager.instance.ship.IsInvincible())
		{
			crashNotificationHandler(this, other);
        }
	}

	public static void DefaultCrashNotificationHandler(ShipCollider shipCollider, Collider obstacleCollider)
	{
		Debug.Log("Crash");
		Manager.instance.ship.ResetInvincibleStatus();
		Manager.instance.ship.ResetForce();
		Manager.instance.NotifyCrash(shipCollider, obstacleCollider);
	}

}

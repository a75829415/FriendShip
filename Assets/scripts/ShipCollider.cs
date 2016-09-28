using UnityEngine;
using System.Collections;

public class ShipCollider : MonoBehaviour
{
	public Collider reservedCollider;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnTriggerEnter(Collider other)
	{
		if (!Manager.instance.ship.IsInvincible())
		{
			Debug.Log("Crash");
			Manager.instance.ship.ResetInvincibleStatus();
			Manager.instance.ship.ResetForce();
			Manager.instance.NotifyCrash(reservedCollider, other);
		}
	}

}

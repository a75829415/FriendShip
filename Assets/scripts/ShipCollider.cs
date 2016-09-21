using UnityEngine;
using System.Collections;

public class ShipCollider : MonoBehaviour
{
	public Ship ship;
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
		Debug.Log("Crash...");
		if (!ship.IsInvincible())
		{
			Debug.Log("Crash");
			ship.ResetInvincibleStatus();
			ship.ResetForce();
			Manager.instance.NotifyCrash(reservedCollider, other);
		}
	}

}

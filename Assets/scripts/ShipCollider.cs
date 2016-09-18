using UnityEngine;
using System.Collections;

public class ShipCollider : MonoBehaviour
{
	public Ship ship;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnTriggerEnter()
	{
		if (!ship.IsInvincible())
		{
			Debug.Log("Crash");
			ship.ResetInvincibleStatus();
		}
	}

}

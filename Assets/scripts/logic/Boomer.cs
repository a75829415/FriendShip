using UnityEngine;
using System.Collections;

public class Boomer : MoveableObject
{
	public Collider reservedCollider;

	void Awake()
	{
		base.AwakeWorkaround();
		reservedCollider = GetComponent<Collider>();
    }

	// Use this for initialization
	void Start () {
		base.StartWorkaround();
	}
	
	// Update is called once per frame
	void Update () {
		if (reservedTransform.position.y < Background.instance.reservedTransform.position.y)
		{
			Destroy(reservedTransform.gameObject);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (Manager.instance.IsObstacle(other))
		{
			other.gameObject.SetActive(false);
			((BoomManager)(Manager.instance)).Scores();
		}
	}

}

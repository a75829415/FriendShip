using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boomer : MoveableObject
{
	public Rigidbody reservedRigidBody;
	public Collider reservedCollider;

	private static List<Boomer> activePool = new List<Boomer>();
	private static Stack<Boomer> inactivePool = new Stack<Boomer>();

	public struct Status
	{
		public Status(Vector3 iPosition, Vector3 iVelocity)
		{
			position = iPosition;
			velocity = iVelocity;
		}
		public Vector3 position;
		public Vector3 velocity;
	}

	public Status CurrentStatus
	{
		get { return new Status(reservedTransform.position, reservedRigidBody.velocity); }
		set { reservedTransform.position = value.position; reservedRigidBody.velocity = value.velocity; }
	}

	public static Status[] GetStatuses()
	{
		Status[] result = new Status[activePool.Count];
		for (int i = 0; i < result.Length; ++i)
		{
			result[i] = activePool[i].CurrentStatus;
		}
        return result;
    }

	public static void ApplyStatuses(Status[] data)
	{
		while (activePool.Count < data.Length)
		{
			activateABoomer();
		}
		while (activePool.Count > data.Length)
		{
			deactivate(activePool[activePool.Count - 1]);
		}
		for (int i = 0; i < data.Length; ++i)
		{
			activePool[i].CurrentStatus = data[i];
		}
	}

	public static Boomer activateABoomer()
	{
		Boomer result;
		if (inactivePool.Count == 0)
		{
			result = Instantiate(Manager.instance.boomerPrototype);
        }
		else
		{
			result = inactivePool.Pop();
		}
		activePool.Add(result);
		Manager.instance.RegisterToMove(result);
		result.gameObject.SetActive(true);
		return result;
	}

	public static void deactivate(Boomer boomer)
	{
		activePool.Remove(boomer);
		inactivePool.Push(boomer);
		Manager.instance.UnregisterToMove(boomer);
		boomer.gameObject.SetActive(false);
	}

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
			deactivate(this);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Obstacle")
		{
			other.gameObject.SetActive(false);
			((BoomManager)(Manager.instance)).Scores();
		}
	}

}

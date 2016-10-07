using UnityEngine;
using System.Collections;

public class MoveableObject : MonoBehaviour
{
	public Transform reservedTransform;

	void Awake()
	{
		AwakeWorkaround();
	}

	public void AwakeWorkaround()
	{
		reservedTransform = GetComponent<Transform>();
	}

	public void StartWorkaround()
	{
	}

	public void MoveVertically(float x, float z)
	{
		reservedTransform.Translate(x, 0, z, Space.World);
	}

	public void MoveToVertically(float x, float z)
	{
		reservedTransform.position = new Vector3(x, reservedTransform.position.y, z);
	}

	public Vector3 Position { get { return reservedTransform.position; } }

}

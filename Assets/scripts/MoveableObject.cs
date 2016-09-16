using UnityEngine;
using System.Collections;

public class MoveableObject : MonoBehaviour
{
	public Transform reservedTransform;

	// Use this for initialization
	void Start()
	{
		StartWorkaround();
	}

	public void StartWorkaround()
	{
		reservedTransform = GetComponent<Transform>();
	}

	public void Move(float x, float z)
	{
		reservedTransform.Translate(x, 0, z, Space.World);
	}

}

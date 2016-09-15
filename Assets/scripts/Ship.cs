using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour
{
	private Transform reservedTransform;
	

	// Use this for initialization
	void Start()
	{
		reservedTransform = GetComponent<Transform>();
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	void Move(float x, float z)
	{
		reservedTransform.Translate(x, 0, z);
    }

}

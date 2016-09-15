using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour
{
	private Transform reservedTransform;
	private Rigidbody reservedRigidbody;
	private ConstantForce reservedBackgroundForce;

	public float accelerationBase = 0.5f;


	// Use this for initialization
	void Start()
	{
		reservedTransform = GetComponent<Transform>();
		reservedRigidbody = GetComponent<Rigidbody>();
		reservedBackgroundForce = GetComponent<ConstantForce>();
    }

	// Update is called once per frame
	void Update()
	{
		
	}

	private float ToActualForce(float acceleration)
	{
		return acceleration * reservedRigidbody.mass;
    }

	void Move(float x, float z)
	{
		reservedTransform.Translate(x, 0, z);
    }

}

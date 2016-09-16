using UnityEngine;
using System.Collections;

public class Ship : MoveableObject
{
	private Rigidbody reservedRigidbody;
	private ConstantForce reservedBackgroundForce;

	public float accelerationBase;
	public float accelerationGrowth;

	// Use this for initialization
	void Start()
	{
        base.StartWorkaround();
		reservedRigidbody = GetComponent<Rigidbody>();
		reservedBackgroundForce = GetComponent<ConstantForce>();
		reservedBackgroundForce.relativeForce = Vector3.forward * ForceValueBase();
    }

	// Update is called once per frame
	void Update()
	{
		UpdateForce();
	}

	private void UpdateForce()
	{
		reservedBackgroundForce.relativeForce += Vector3.forward * DeltaForceValue();
    }

	private float ForceValueBase()
	{
		return ToActualForce(accelerationBase);
    }

	private float DeltaForceValue()
	{
		return Time.deltaTime * accelerationGrowth * ForceValueBase();
    }

	private float ToActualForce(float acceleration)
	{
		return acceleration * reservedRigidbody.mass;
    }

}

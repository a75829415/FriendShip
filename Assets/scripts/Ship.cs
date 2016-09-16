using UnityEngine;
using System.Collections;

public class Ship : MoveableObject
{
	private Rigidbody reservedRigidbody;
	private ConstantForce reservedBackgroundForce;

	public float accelerationBase;
	public float sideAccelerationBase;
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

	private float SideAccelerationValueBase()
	{
		return sideAccelerationBase;
	}

	private float LeftAccelerationValueBase()
	{
		return SideAccelerationValueBase();
	}

	private float RightAccelerationValueBase()
	{
		return SideAccelerationValueBase();
	}

	private float PaddleOffset()
	{
		return 1.0f;
	}

	private Vector3 LeftPosition()
	{
		return new Vector3(-PaddleOffset(), 0, 0);
	}

	private Vector3 RightPosition()
	{
		return new Vector3(PaddleOffset(), 0, 0);
	}

	public void PaddleLeft()
	{
		reservedRigidbody.AddForceAtPosition(Vector3.forward * LeftAccelerationValueBase(), LeftPosition(), ForceMode.VelocityChange);
	}

	public void PaddleRight()
	{
		reservedRigidbody.AddForceAtPosition(Vector3.forward * RightAccelerationValueBase(), RightPosition(), ForceMode.VelocityChange);
	}

}

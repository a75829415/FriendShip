using UnityEngine;
using System.Collections;

public class Ship : MoveableObject
{
	public Collider mainCollider;

	private Rigidbody reservedRigidbody;
	private ConstantForce reservedBackgroundForce;

	public float accelerationBase;
	public float sideAccelerationBase;
	public float accelerationGrowth;
	public float invincibleTimeBase;

	private float invincibleTime;


	// Use this for initialization
	void Start()
	{
        base.StartWorkaround();
		reservedRigidbody = GetComponent<Rigidbody>();
		reservedBackgroundForce = GetComponent<ConstantForce>();
		invincibleTime = invincibleTimeBase;
        ResetForce();
    }

	// Update is called once per frame
	void Update()
	{
		UpdateInvincibilityStatus();
		UpdateForce();
	}

	public bool isInvincible()
	{
		return invincibleTime > 0;
	}

	public void ResetInvincibleStatus()
	{
		invincibleTime = invincibleTimeBase;
		UpdateEngagingStatus();
	}

	public void UpdateEngagingStatus()
	{
		if (isInvincible())
		{
			Disengage();
		}
		else
		{
			Engage();
		}
	}

	private void UpdateInvincibilityStatus()
	{
		invincibleTime -= Time.deltaTime;
		UpdateEngagingStatus();
    }

	private void Engage()
	{
		mainCollider.isTrigger = false;
	}

	private void Disengage()
	{
		mainCollider.isTrigger = true;
	}

	private void UpdateForce()
	{
		reservedBackgroundForce.relativeForce += ValueToForce(DeltaForceValue());
	}

	private void ResetForce()
	{
		reservedBackgroundForce.relativeForce = ValueToForce(ForceValueBase());
	}

	private float ForceValueBase()
	{
		return ToActualForceValue(accelerationBase);
    }

	private float DeltaForceValue()
	{
		return Time.deltaTime * accelerationGrowth * ForceValueBase();
    }

	private float ToActualForceValue(float acceleration)
	{
		return acceleration * reservedRigidbody.mass;
    }

	private Vector3 ValueToForce(float force)
	{
		return Vector3.forward * force;
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
		return new Vector3(PaddleOffset(), 0, 0);
	}

	private Vector3 RightPosition()
	{
		return new Vector3(-PaddleOffset(), 0, 0);
	}

	public void PaddleLeft()
	{
		reservedRigidbody.AddTorque(Vector3.Cross((Vector3.forward * LeftAccelerationValueBase()), LeftPosition()), ForceMode.VelocityChange);
	}

	public void PaddleRight()
	{
		reservedRigidbody.AddTorque(Vector3.Cross((Vector3.forward * RightAccelerationValueBase()), RightPosition()), ForceMode.VelocityChange);
	}


	void OnColliderEnter(Collider other)
	{
		ResetForce();
	}

	void OnColliderExit(Collider other)
	{
		ResetInvincibleStatus();
	}

}

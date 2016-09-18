using UnityEngine;
using System.Collections;

public class Ship : MoveableObject
{
	public Collider mainCollider;

	public Rigidbody reservedRigidbody;
	public ConstantForce reservedBackgroundForce;

	public float accelerationBase;
	public float sideAccelerationBase;
	public float accelerationGrowth;
	public float invincibleTimeBase;

	private float invincibleTime;

	private bool unprepared;

	public float GetSpeed()
	{
		return reservedRigidbody.velocity.magnitude;
	}

	public float GetVertical()
	{
		return reservedRigidbody.angularVelocity.y;
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

	private void UpdateInvincibilityStatus()
	{
		invincibleTime -= Time.deltaTime;
		UpdateEngagingStatus();
	}


	// Use this for initialization
	void Start()
	{
		unprepared = true;
        base.StartWorkaround();
		ResetInvincibleStatus();
    }

	// Update is called once per frame
	void Update()
	{
		if (Manager.instance.IsOperating())
		{
			if (unprepared)
			{
				ResetForce();
				unprepared = false;
            }
			UpdateInvincibilityStatus();
			UpdateForce();
		}
	}

	private void UpdateEngagingStatus()
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
		if (Manager.instance.IsOperating())
		{
			reservedRigidbody.AddTorque(
			Vector3.Cross((Vector3.forward * LeftAccelerationValueBase()) * accelerationBase, LeftPosition()), ForceMode.VelocityChange);
		}
	}

	public void PaddleRight()
	{
		if (Manager.instance.IsOperating())
		{
			reservedRigidbody.AddTorque(
				Vector3.Cross((Vector3.forward * RightAccelerationValueBase()) * accelerationBase, RightPosition()), ForceMode.VelocityChange);
		}
	}


	void OnCollisionEnter(Collision collision)
	{
		if (!isInvincible())
		{
			Debug.Log("Crash!");
			ResetForce();
			ResetInvincibleStatus();
		}
	}

}

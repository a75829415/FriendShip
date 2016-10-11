using UnityEngine;
using System.Collections;

public class Ship : MoveableObject
{
	public Rigidbody reservedRigidBody;
	public ConstantForce reservedBackgroundForce;

	public struct Status
	{
		public Status(Vector3 iPosition, Vector3 iRotation, Vector3 iVelocity, Vector3 iAngularVelocity, Vector3 iRelativeForce, float iBoomerElevation)
		{
			position = iPosition;
			rotation = iRotation;
			velocity = iVelocity;
			angularVelocity = iAngularVelocity;
			relativeForce = iRelativeForce;
			boomerElevation = iBoomerElevation;
        }
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 velocity;
		public Vector3 angularVelocity;
		public Vector3 relativeForce;
		public float boomerElevation;
    }

	public Status CurrentStatus
	{
		get {
			return new Status(reservedTransform.position, reservedTransform.eulerAngles, reservedRigidBody.velocity,
				reservedRigidBody.angularVelocity, reservedBackgroundForce.relativeForce, boomerElevation);
		}
		set {
			reservedTransform.position = value.position;
			reservedTransform.eulerAngles = value.rotation;
			reservedRigidBody.velocity = value.velocity;
			reservedRigidBody.angularVelocity = value.angularVelocity;
			reservedBackgroundForce.relativeForce = value.relativeForce;
			boomerElevation = value.boomerElevation;
		}
	}

	public float accelerationBase;
	public float sideAccelerationBase;
	public float accelerationGrowth;
	public float invincibleTimeBase;
	public float boomerInitialSpeed;

	public float boomerElevation = 0.0f;

	private float invincibleTime;

	public float InvincibleTime { get { return invincibleTime; } set { invincibleTime = value; } }

	private bool unprepared;

	public float GetSpeed()
	{
		return reservedRigidBody.velocity.magnitude;
	}

	public float GetVertical()
	{
		return reservedRigidBody.angularVelocity.y;
	}

	public bool IsInvincible()
	{
		return invincibleTime > 0;
	}

	public void ResetInvincibleStatus()
	{
		invincibleTime = invincibleTimeBase;
	}

	private void UpdateInvincibilityStatus()
	{
		invincibleTime -= Time.deltaTime;
	}


	// Use this for initialization
	void Start()
	{
		base.StartWorkaround();
		unprepared = true;
		invincibleTimeBase = Configuration.InvincibleTime;
		ResetInvincibleStatus();
		Manager.instance.RegisterShip(this);
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


	private void UpdateForce()
	{
		reservedBackgroundForce.relativeForce += ValueToForce(DeltaForceValue());
	}

	public void ResetForce()
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
		return acceleration * reservedRigidBody.mass;
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
			reservedRigidBody.AddTorque(
			Vector3.Cross((Vector3.forward * LeftAccelerationValueBase()) * accelerationBase, LeftPosition()), ForceMode.VelocityChange);
		}
	}

	public void PaddleRight()
	{
		if (Manager.instance.IsOperating())
		{
			reservedRigidBody.AddTorque(
				Vector3.Cross((Vector3.forward * RightAccelerationValueBase()) * accelerationBase, RightPosition()),
				ForceMode.VelocityChange);
		}
	}

	public void BoomABoomer()
	{
		NetHub.instance.CmdBoomABoomer();
    }

	public void ServerBoom()
	{
		if (Manager.instance.IsOperating() && !IsInvincible())
		{
			Boomer boomer = Boomer.activateABoomer();
			boomer.reservedTransform.position = reservedTransform.position;
			float vertical = boomerInitialSpeed * Mathf.Sin(boomerElevation);
			float horizontal = boomerInitialSpeed * Mathf.Cos(boomerElevation);
			boomer.reservedRigidBody.velocity = horizontal * (-reservedTransform.forward) + new Vector3(0, vertical, 0) + reservedRigidBody.velocity;
			reservedRigidBody.velocity = (reservedRigidBody.velocity + horizontal * (reservedTransform.forward));
		}
	}

}

using UnityEngine;
using System.Collections;

public class Ship : MoveableObject
{
	public Collider mainCollider;
	public Camera thirdPersonCamera;
    public Transform thirdPersonCameraTransform;

	private Rigidbody reservedRigidbody;
	private ConstantForce reservedBackgroundForce;

	public float accelerationBase;
	public float sideAccelerationBase;
	public float accelerationGrowth;
	public float invincibleTimeBase;

	private float invincibleTime;

	private bool unprepared;


	private float ChasingCameraProgress(float y)
	{
		float starting = 50.0f;
		float result = GetSpeed() + starting;
		float ratio = 2.0f * Manager.instance.WaitTime / Manager.instance.waitTimeBase - 1.0f;
		if (ratio > 0) {
			result -= starting * ratio;
		}
		result /= 50.0f;
		result += 1.0f;
		Debug.Log(Mathf.Pow(result, y));
        return Mathf.Pow(result, y);
	}

	public float GetSpeed()
	{
		return reservedRigidbody.velocity.magnitude;
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
		reservedRigidbody = GetComponent<Rigidbody>();
		reservedBackgroundForce = GetComponent<ConstantForce>();
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
		UpdateCamera();
	}

	private void UpdateCamera()
	{
		UpdateCameraPosition();
		UpdateCameraRotation();
		UpdateCameraFOV();
    }

	private void UpdateCameraPosition()
	{
		float x = 0;
		float y = 3 + 13 / ChasingCameraProgress(2.2f);
		float z = -3f + 16f / ChasingCameraProgress(2.5f);
		thirdPersonCameraTransform.localPosition = new Vector3(x, y, z);
    }

	private void UpdateCameraRotation()
	{
		float x = 15 + 75 /  ChasingCameraProgress(1.0f);
		float y = 0;
		float z = 0;
		thirdPersonCameraTransform.localRotation = Quaternion.Euler(x, y, z);
    }

	private void UpdateCameraFOV()
	{
		thirdPersonCamera.fieldOfView = 90 + 30 / ChasingCameraProgress(1.0f);
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

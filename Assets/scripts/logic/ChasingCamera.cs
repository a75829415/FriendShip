using UnityEngine;
using System.Collections;

public class ChasingCamera : MoveableObject {
	public Ship ship;
	public Camera reservedCamera;

	public Transform rearViewCameraTransfrom;
	public Camera rearViewCamera;

	public float maxRadiansDeltaPerSecond;
	public float maxMagnitudeDeltaPerSecond;

	private Vector3 previousPosition = new Vector3(0, 16, 13);
	private Vector3 previousRotation = new Vector3(90, 0, 0);
	private float previousAngle = 0.0f;

	private bool IsPreparing()
	{
		return Manager.instance.WaitTime > 3.0f;
	}

	private bool RearViewOnPosition()
	{
		return Manager.instance.WaitTime <= 1.5f;
    }

	private float GetMaxRadiansDelta()
	{
		return maxRadiansDeltaPerSecond * Time.deltaTime;
    }

	private float GetMaxMagnitudeDelta()
	{
		return maxMagnitudeDeltaPerSecond * Time.deltaTime;
	}

	private float ChasingCameraProgress(float y)
	{
		float starting = 50.0f;
		float result = ship.GetSpeed() + starting;
		float ratio = 2.0f * Manager.instance.WaitTime / Manager.instance.waitTimeBase - 1.0f;
		if (ratio > 0)
		{
			result -= starting * ratio;
		}
		result /= 50.0f;
		result += 1.0f;
		return Mathf.Pow(result, y);
	}

	// Use this for initialization
	void Start () {
		base.StartWorkaround();
		gameObject.SetActive(true);
		CheckControlMode();
    }
	
	// Update is called once per frame
	void Update () {
		UpdateCameraPosition();
		UpdateCameraRotation();
		UpdateCameraFOV();
		UpdateRearViewCamera();
		UpdateCameraVerticalRotation();
	}

	private void UpdateCameraPosition()
	{
		float x = 0;
		float y = 3 + 13 / ChasingCameraProgress(2.2f);
		float z = -3f + 16f / ChasingCameraProgress(2.5f);
		Vector3 expected = new Vector3(x, y, z);
        reservedTransform.localPosition = Vector3.RotateTowards(
			previousPosition, expected, GetMaxRadiansDelta(), GetMaxMagnitudeDelta());
		previousPosition = reservedTransform.localPosition;
    }

	private void UpdateCameraRotation()
	{
		float x = 15 + 75 / ChasingCameraProgress(1.0f);
		float y = 0;
		float z = 0;
		Vector3 expected = new Vector3(x, y, z);
		reservedTransform.localEulerAngles = Vector3.RotateTowards(
			previousRotation, expected, GetMaxRadiansDelta(), GetMaxMagnitudeDelta());
		previousRotation = reservedTransform.localEulerAngles;
    }

	private void UpdateCameraFOV()
	{
		Vector3 expected = new Vector3(90 + 30 / ChasingCameraProgress(1.0f), 0, 0);
		reservedCamera.fieldOfView = Vector3.RotateTowards(
			new Vector3(reservedCamera.fieldOfView, 0, 0), expected, GetMaxRadiansDelta(), GetMaxMagnitudeDelta()).x;
	}

	private void UpdateCameraVerticalRotation()
	{
		float angle = ship.reservedRigidBody.angularVelocity.y;
		bool toward_left;
		if (angle < 0)
		{
			toward_left = true;
		}
		else
		{
			toward_left = false;
		}
		angle = Mathf.Abs(angle) + 1.0f;
		angle = 15 - 15 / angle;
		if (toward_left)
		{
			angle = -angle;
		}
		Vector3 fakeExpected = new Vector3(angle + 15, 0, 0);
		float actualAngle = Vector3.RotateTowards(
			new Vector3(previousAngle + 15, 0, 0), fakeExpected, GetMaxRadiansDelta(), GetMaxMagnitudeDelta()).x - 15;
        reservedTransform.RotateAround(
			new Vector3(ship.Position.x, Position.y, ship.Position.z),
			Vector3.up,
			actualAngle);
		previousAngle = actualAngle;
	}

	public void UpdateRearViewCamera()
	{
		Vector3 mirrorPosition = new Vector3(reservedTransform.localPosition.x,
			reservedTransform.localPosition.y, -reservedTransform.localPosition.z);
		Vector3 mirrorRotation = new Vector3(reservedTransform.localEulerAngles.x, 180.0f, reservedTransform.localEulerAngles.z);
		Vector3 finalPosition = Vector3.zero;
		Vector3 finalRotation = new Vector3(-ship.boomerElevation, 180.0f, 0.0f);
        if (IsPreparing())
		{
			rearViewCameraTransfrom.localPosition = mirrorPosition;
			rearViewCameraTransfrom.localEulerAngles = mirrorRotation;
            rearViewCamera.fieldOfView = reservedCamera.fieldOfView;
		}
		else
		{
			if (RearViewOnPosition())
			{
				rearViewCameraTransfrom.localPosition = finalPosition;
				rearViewCameraTransfrom.localEulerAngles = finalRotation;
            }
			else
			{
				float process = (3.0f - Manager.instance.WaitTime) / 1.5f;
				rearViewCameraTransfrom.localPosition = (finalPosition - mirrorPosition) * process + mirrorPosition;
                rearViewCameraTransfrom.localEulerAngles = (finalRotation - mirrorRotation) * process + mirrorRotation;
			}
		}
		if (Vector3.Distance(rearViewCameraTransfrom.localPosition, finalPosition) < 1.0f)
		{
			rearViewCamera.cullingMask = rearViewCamera.cullingMask & ~(1 << LayerMask.NameToLayer("Ignore in FP"));
		}
		else
		{
			rearViewCamera.cullingMask = rearViewCamera.cullingMask | (1 << LayerMask.NameToLayer("Ignore in FP"));
		}
	}

	public void CheckControlMode()
	{
		bool lookAhead;
		lookAhead = (Manager.instance.GetControlMode() != ShipControlMode.FireOnly);
		if (Configuration.swapViews)
		{
			lookAhead = !lookAhead;
		}
		if (lookAhead)
		{
			SetAsMainView(reservedCamera);
			SetAsMiniView(rearViewCamera);
		}
		else
		{
			SetAsMainView(rearViewCamera);
			SetAsMiniView(reservedCamera);
		}
	}

	public static void SetAsMainView(Camera camera)
	{
		camera.depth = -1;
		camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
	}

	public static void SetAsMiniView(Camera camera)
	{
		if (Configuration.enableMiniView)
		{
			camera.depth = 0;
		}
		else
		{
			camera.depth = -2;
		}
		camera.rect = new Rect(0.02f, 0.02f, Configuration.MiniViewSize, Configuration.MiniViewSize);
	}

}

using UnityEngine;
using System.Collections;

public class ChasingCamera : MoveableObject {
	public Ship ship;
	public Camera reservedCamera;

	public float maxRadiansDeltaPerSecond;
	public float maxMagnitudeDeltaPerSecond;

	private Vector3 previousPosition = new Vector3(0, 16, 13);
	private Vector3 previousRotation = new Vector3(90, 0, 0);
	private float previousAngle = 0.0f;

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
    }
	
	// Update is called once per frame
	void Update () {
		UpdateCameraPosition();
		UpdateCameraRotation();
		UpdateCameraFOV();
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
		float angle = ship.reservedRigidbody.angularVelocity.y;
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

}

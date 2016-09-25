using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetHub : NetworkBehaviour {
	public static NetHub instance;

	void Awake()
	{
		instance = this;
	}

	[ClientRpc]
	public void RpcUpdateRandomSeed(int seed)
	{
		Random.InitState(seed);
	}

	[ClientRpc]
	public void RpcUpdateWaitTime(float serverWaitTime)
	{
		Manager.instance.WaitTime = serverWaitTime;
	}

	[ClientRpc]
	public void RpcUpdateInvincibleTime(float serverInvincibleTime)
	{
		Manager.instance.ship.InvincibleTime = serverInvincibleTime;
	}

	[ClientRpc]
	public void RpcUpdateGameTime(float serverGameTime)
	{
		Manager.instance.GameTime = serverGameTime;
	}

	[ClientRpc]
	public void RpcUpdateShip(Vector3 position, Vector3 rotation, Vector3 velocity, Vector3 angularVelocity, Vector3 relativeForce)
	{
		Manager.instance.ship.reservedTransform.position = position;
		Manager.instance.ship.reservedTransform.eulerAngles = rotation;
		Manager.instance.ship.reservedRigidbody.velocity = velocity;
		Manager.instance.ship.reservedRigidbody.angularVelocity = angularVelocity;
		Manager.instance.ship.reservedBackgroundForce.relativeForce = relativeForce;
	}

	[ClientRpc]
	public void RpcMoveTowardEast()
	{
		Manager.instance.MoveTowardEast();
	}

	[ClientRpc]
	public void RpcMoveTowardSouth()
	{
		Manager.instance.MoveTowardSouth();
	}

	[ClientRpc]
	public void RpcMoveTowardWest()
	{
		Manager.instance.MoveTowardWest();
	}

	[ClientRpc]
	public void RpcMoveTowardNorth()
	{
		Manager.instance.MoveTowardNorth();
	}

}

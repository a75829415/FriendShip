using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetHub : NetworkBehaviour {
	public static NetHub instance;
	[SyncVar]
	private int seed;

	void Awake()
	{
		AwakeWorkaround();
    }

	public void AwakeWorkaround()
	{
		instance = this;
		DontDestroyOnLoad(instance);
		if (isServer)
		{
			seed = System.DateTime.Now.Millisecond;
		}
        Random.InitState(seed);
	}

	/*
	public override void OnStartAuthority()
	{
		base.OnStartAuthority();
		CmdRequestForSeed();
	}

	[Command]
	public void CmdRequestForSeed()
	{
		RpcUpdateRandomSeed(seed);
	}

	[ClientRpc]
	public void RpcUpdateRandomSeed(int serverSeed)
	{
		if (!isServer)
		{
			seed = serverSeed;
			Random.InitState(seed);
		}
	}
	*/

	[ClientRpc]
	public void RpcUpdateWaitTime(float serverWaitTime)
	{
		if (!isServer)
		{
			Manager.instance.WaitTime = serverWaitTime;
		}
	}

	[ClientRpc]
	public void RpcUpdateInvincibleTime(float serverInvincibleTime)
	{
		if (!isServer && Manager.instance.ship != null)
		{
			Manager.instance.ship.InvincibleTime = serverInvincibleTime;
		}
	}

	[ClientRpc]
	public void RpcUpdateGameTime(float serverGameTime)
	{
		if (!isServer)
		{
			Manager.instance.GameTime = serverGameTime;
		}
	}

	[ClientRpc]
	public void RpcUpdateShip(Vector3 position, Vector3 rotation, Vector3 velocity, Vector3 angularVelocity, Vector3 relativeForce)
	{
		if (!isServer && Manager.instance.ship != null)
		{
			Manager.instance.ship.reservedTransform.position = position;
			Manager.instance.ship.reservedTransform.eulerAngles = rotation;
			Manager.instance.ship.reservedRigidbody.velocity = velocity;
			Manager.instance.ship.reservedRigidbody.angularVelocity = angularVelocity;
			Manager.instance.ship.reservedBackgroundForce.relativeForce = relativeForce;
		}
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

	void OnDestroy()
	{
		if (Manager.instance != null)
		{
			Destroy(Manager.instance.gameObject);
		}
	}

}

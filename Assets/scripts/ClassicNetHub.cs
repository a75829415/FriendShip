using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ClassicNetHub : NetHub {
	public ClassicManager classicManagerPrototype;

	void Awake()
	{
		base.AwakeWorkaround();
		DontDestroyOnLoad(Instantiate(classicManagerPrototype));
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	[ClientRpc]
	public void RpcUpdateStatus(uint serverHealth)
	{
		if (!isServer)
		{
			((ClassicManager)(Manager.instance)).currentHealth = serverHealth;
		}
    }

	[ClientRpc]
	public void RpcNotifyGameOver(float result)
	{
		((ClassicManager)(Manager.instance)).GameOver(result);
	}

	void OnDestroy()
	{
		Destroy(Manager.instance.gameObject);
	}

}

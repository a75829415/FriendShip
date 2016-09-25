using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class LobbyDiscovery : NetworkDiscovery
{
    private static LobbyDiscovery singleton;
    public static LobbyDiscovery instance
    {
        get
        {
            return singleton;
        }
        set
        {
            if (singleton == null)
            {
                singleton = value;
            }
            else if (singleton != value)
            {
                Destroy(value.gameObject);
            }
        }
    }

    public Dictionary<string, float> serverAddresses { get; private set; }

    void Awake()
    {
        instance = this;
        serverAddresses = new Dictionary<string, float>();
    }

    void Start()
    {
        StartListening();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        float receiveTime;
        if (serverAddresses.TryGetValue(fromAddress, out receiveTime))
        {
            serverAddresses[fromAddress] = Time.realtimeSinceStartup;
        }
        else
        {
            serverAddresses.Add(fromAddress, Time.realtimeSinceStartup);
            GUIEventHandler.instance.AddLobby(fromAddress);
        }
    }

    public void StartBroadcasting()
    {
        if (running)
        {
            StopBroadcastingOrListening();
        }
        Initialize();
        StartAsServer();
    }

    public void StartListening()
    {
        if (running)
        {
            StopBroadcastingOrListening();
        }
        Initialize();
        StartAsClient();
    }

    public void StopBroadcastingOrListening()
    {
        if (isServer)
        {
            StopBroadcast();
            StartListening();
        }
        else if (isClient)
        {
            StopBroadcast();
        }
        serverAddresses.Clear();
    }
}

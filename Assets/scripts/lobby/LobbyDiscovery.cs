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

    private Dictionary<string, float> serverAddresses;

    void Awake()
    {
        Initialize();
        instance = this;
        serverAddresses = new Dictionary<string, float>();
    }

    void Start()
    {
        StartListening();
    }

    void Update()
    {
        if (isClient && running)
        {
            foreach (string address in serverAddresses.Keys)
            {
                if (Time.realtimeSinceStartup - serverAddresses[address] > broadcastInterval * 3.0f)
                {
                    serverAddresses.Remove(address);
                }
            }
        }
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
        }
    }

    public void StartBroadcasting()
    {
        if (running)
        {
            StopBroadcastingOrListening();
        }
        StartAsServer();
    }

    public void StartListening()
    {
        if (running)
        {
            StopBroadcastingOrListening();
        }
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

    public List<string> GetServerAddresses()
    {
        List<string> addresses = new List<string>();
        foreach (string address in serverAddresses.Keys)
        {
            addresses.Add(address);
        }
        return addresses;
    }
}

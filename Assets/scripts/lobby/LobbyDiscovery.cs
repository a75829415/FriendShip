using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class LobbyDiscovery : NetworkDiscovery
{
    public static LobbyDiscovery instance;

    private float startTime;
    private Dictionary<string, float> serverAddresses;

    void Awake()
    {
        Initialize();
        instance = this;
        serverAddresses = new Dictionary<string, float>();
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
        startTime = Time.realtimeSinceStartup;
        StartAsServer();
    }

    public void StartListening()
    {
        if (running)
        {
            StopBroadcastingOrListening();
        }
        startTime = Time.realtimeSinceStartup;
        StartAsClient();
    }

    public void StopBroadcastingOrListening()
    {
        StopBroadcast();
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

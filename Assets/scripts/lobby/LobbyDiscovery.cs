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
        DontDestroyOnLoad(gameObject);
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
            string[] buffer = data.Split('*');
            ChooseLobbyUIHandler.instance.AddLobby(fromAddress, buffer[0], buffer[1]);
        }
    }

    public void StartBroadcasting(string data)
    {
        if (running)
        {
            StopBroadcastingOrListening();
        }
        broadcastData = data;
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
        StopBroadcast();
        broadcastData = string.Empty;
    }
}

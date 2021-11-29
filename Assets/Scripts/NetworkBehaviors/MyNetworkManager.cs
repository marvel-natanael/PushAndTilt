using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameManager manager;
    [SerializeField] private string hostName;

    public string HostName { get => hostName; set => hostName = value; }

    public override void OnStartHost()
    {
        base.OnStartHost();
        var manager = FindObjectOfType<GameManager>();

        GetComponent<MyNetworkDiscovery>().AdvertiseServer();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        if (NetworkServer.connections.Count > 5)
        {
            conn.Disconnect();
        }
        else
        {
            Debug.Log("A player has joined");
            manager.SetPlayerConnected(0, NetworkServer.connections.Count);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        manager.SetPlayerConnected(0, NetworkServer.connections.Count);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("You're connected!");
        //todo: Send player name
    }

    public override void Awake()
    {
        base.Awake();
        if (!manager)
        {
            manager = FindObjectOfType<GameManager>();
            if (!manager)
            {
                Debug.LogError("MyNetworkManager: Manager not found");
            }
        }
    }
}
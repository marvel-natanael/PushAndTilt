using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameManager manager;
    [SerializeField] private string hostName;
    [SerializeField] private string playerName;

    /// <summary>
    /// Get and set server hostName
    /// </summary>
    public string HostName { get => hostName; set => hostName = value; }

    public string PlayerName { get => playerName; set => playerName = value; }

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
            Debug.Log("MyNetworkManager.cs/OnServerConnect(): a player has been kicked (full server)");
            conn.Disconnect();
        }
        else
        {
            Debug.Log("MyNetworkManager.cs/OnServerConnect(): A player has joined");
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
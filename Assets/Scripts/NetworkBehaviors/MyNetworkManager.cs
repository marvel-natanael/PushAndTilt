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
        Debug.Log("MyNetworkManager.cs/OnServerConnect(): A player has joined");
        manager.SetPlayerConnected(0, NetworkServer.connections.Count);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
        manager.AddPlayerObject(conn.connectionId, player.GetComponent<NetworkIdentity>());
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        manager.SetPlayerConnected(0, NetworkServer.connections.Count);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        manager.SetPlayerConnected(manager.PlayerCount, manager.PlayerCount);
        manager.RegisterClientName(conn.connectionId, playerName);
        Debug.Log("You're connected!");
    }

    public override void Awake()
    {
        base.Awake();
        if (!(manager = FindObjectOfType<GameManager>()))
        {
            Debug.LogError("MyNetworkManager.cs/Awake(): manager not found");
        }
    }
}
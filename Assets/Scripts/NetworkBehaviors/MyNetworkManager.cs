using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class MyNetworkManager : NetworkManager
{
    private GameManager manager;
    private string hostName;
    private string localPlayerName;

    /// <summary>
    /// Get and set server hostName
    /// </summary>
    public string HostName { get => hostName; set => hostName = value; }

    public string LocalPlayerName { get => localPlayerName; set => localPlayerName = value; }

    public override void OnStartHost()
    {
        base.OnStartHost();
        GetComponent<MyNetworkDiscovery>().AdvertiseServer();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        if (!(manager = FindObjectOfType<GameManager>()))
        {
            Debug.LogError($"{ToString()}: manager not found");
        }
        manager.ServerSetPlayerCount(NetworkServer.connections.Count);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        manager.ServerSetPlayerCount(NetworkServer.connections.Count);
        base.OnServerDisconnect(conn);
        Debug.Log($"{ToString()}: {conn.address} has disconnected");
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
        player.GetComponent<NetPlayerScript>().ServerSetPlayerName(manager.NewPlayerName);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnApplicationQuit()
    {
        if (NetworkServer.active)
        {
            ServerDisconnect();
        }
        if (NetworkClient.active)
        {
            ClientDisconnect();
        }
        base.OnApplicationQuit();
    }

    [Server]
    public void ServerDisconnect()
    {
    }

    [Client]
    public void ClientDisconnect()
    {
        var player = NetworkClient.localPlayer.GetComponent<NetPlayerScript>();
        if (player.IsReady)
        {
            FindObjectOfType<LobbyManager>().CmdSetPlayerReadyState(false);
        }
        if (player.isAlive)
        {
            player.CmdDie(null);
        }
    }
}
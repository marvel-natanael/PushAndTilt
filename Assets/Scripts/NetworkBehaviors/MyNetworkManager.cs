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

    public override void OnStartServer()
    {
        GetComponent<MyNetworkDiscovery>().AdvertiseServer();
        base.OnStartServer();
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

    public override void OnStopClient()
    {
        NetPlayerScript player;
        if (player = NetworkClient.localPlayer.GetComponent<NetPlayerScript>())
        {
            if (player.IsReady)
            {
                FindObjectOfType<LobbyManager>().CmdSetPlayerReadyState(false);
            }
            if (player.isAlive)
            {
                player.CmdDie(null);
            }
        }
        else
        {
            Debug.LogError($"{ToString()}: player object not found when disconnecting");
        }
        base.OnStopClient();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
    }

    public void Disconnect()
    {
        if (NetworkClient.isConnected)
        {
            StopClient();
        }
        if (NetworkServer.active)
        {
            StopServer();
        }
    }

#if UNITY_EDITOR

    [SerializeField] private bool drawGUI;

    private void OnGUI()
    {
        if (!drawGUI) return;
        GUILayout.BeginArea(new Rect(0, 200, 215, 9999));
        GUILayout.Label($"NetworkServer.active = {NetworkServer.active}");
        GUILayout.Label($"NetworkClient.isConnected = {NetworkClient.isConnected}");
        GUILayout.EndArea();
    }

#endif
}
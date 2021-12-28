using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
        if (string.IsNullOrEmpty(hostName))
        {
            hostName = networkAddress;
        }
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
        manager.ServerSetPlayerCount(NetworkServer.connections.Count, conn);
        base.OnServerDisconnect(conn);
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

    public void ResetGame()
    {
        ServerChangeScene(onlineScene);
    }

    private IEnumerator RestartGame()
    {
        StopServer();
        Debug.Log($"Server Stopped");
        yield return new WaitForSeconds(5);
        StartServer();
        Debug.Log($"Server Started");
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
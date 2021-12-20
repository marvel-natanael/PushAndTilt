using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    #region Fields

    [SerializeField] private MyNetworkManager netManager;
    [SerializeField] private bool running;

    //Network-managed Variables
    [SerializeField, SyncVar] private int playersConnected;

    [SerializeField, SyncVar] private int playerAlive;
    [SerializeField, SyncVar(hook = nameof(HookNewPlayerName))] private string newPlayerName;

    [Header("GUI settings")]
    [SerializeField] private bool showGUI;

    [SerializeField] private Vector2 guiOffset;
    [SerializeField] private List<NetPlayerScript> players;

    #endregion Fields

    #region Properties

    public int PlayerCount { get => playerAlive; }
    public bool Running { get => running; }
    public int PlayersConnected => playersConnected;

    public string NewPlayerName => newPlayerName;

    #endregion Properties

    #region Server_Functions

    [Server]
    public void ServerStartGame()
    {
        running = true;
        FindObjectOfType<NetworkObstacle>().ServerStartGame();
        foreach (var conn in NetworkServer.connections)
        {
            conn.Value.identity.GetComponent<NetPlayerScript>().ServerSetPlayerAliveState(true);
        }
    }

    /// <summary>
    /// Server-side function to set server's <c>running</c> state
    /// </summary>
    /// <param name="state">New state</param>
    [Server]
    public void ServerSetRunningState(bool state)
    {
        running = state;
    }

    /// <summary>
    /// Server-side function to set server's <c>playerConnected</c> count
    /// </summary>
    /// <param name="count">New count</param>
    [Server]
    public void ServerSetPlayerCount(int count)
    {
        playersConnected = count;
    }

    /// <summary>
    /// Server-side function to set server's <c>playerAlive</c> count
    /// </summary>
    /// <param name="count">New count</param>
    [Server]
    public void ServerSetAlivePlayerCount(int count)
    {
        playerAlive = count;
    }

    /// <summary>
    /// Server-side function to set server's <c>newPlayerName</c> name
    /// </summary>
    /// <param name="name">New name</param>
    [Server]
    public void ServerSetNewPlayerName(string name)
    {
        newPlayerName = name;
    }

    /// <summary>
    /// Server-side function to manually set run state
    /// </summary>
    /// <remarks>Only used for testing and developing</remarks>
    [Server]
    private void Running_Switch()
    {
        running = !running;
    }

    #endregion Server_Functions

    #region Commands

    /// <summary>
    /// Command function to set a new player name
    /// </summary>
    /// <remarks>Only run this by client</remarks>
    /// <param name="name">New name</param>
    /// <param name="conn">Leave this empty to automatically get this object's connection</param>
    [Command(requiresAuthority = false)]
    public void CmdSetNewPlayerName(string name, NetworkConnectionToClient conn = null)
    {
        ServerSetNewPlayerName(name);
        conn.identity.GetComponent<NetPlayerScript>().ServerSetPlayerName(name);
        RpcSetNewPlayerName(conn, name);
        Debug.Log($"{ToString()}: Command ran, sent name = {name}");
    }

    #endregion Commands

    #region ClientRPCs

    /// <summary>
    /// Rpc function to tell the client that server has received the name
    /// </summary>
    /// <param name="conn">this connection </param>
    /// <param name="name">New name</param>
    [TargetRpc]
    private void RpcSetNewPlayerName(NetworkConnection conn, string name)
    {
        Debug.Log($"{ToString()}: Rpc recieved, newPlayerName = {newPlayerName}");
    }

    #endregion ClientRPCs

    #region Hooks

    /// <summary>
    /// Hook function for <c>newPlayerName</c>
    /// </summary>
    /// <param name="old">Old value</param>
    /// <param name="new">New value</param>
    [Client]
    private void HookNewPlayerName(string old, string @new)
    {
        newPlayerName = @new;
        FindObjectOfType<LobbyUIScript>().UI_ShowJoined(newPlayerName);
    }

    #endregion Hooks

    public override void OnStartServer()
    {
        playersConnected = NetworkServer.connections.Count;
        running = false;
        playerAlive = 0;
        newPlayerName = "";
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        CmdSetNewPlayerName(netManager.LocalPlayerName);
        base.OnStartClient();
    }

    private void Awake()
    {
        if (!(netManager = FindObjectOfType<MyNetworkManager>()))
        {
            Debug.LogError($"{ToString()}: netManager not found");
        }
        if (GameObject.FindGameObjectsWithTag("Manager").Length > 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnGUI()
    {
        if (!showGUI) return;
        GUILayout.BeginArea(new Rect(10 + guiOffset.x, 40 + guiOffset.y, 215, 9999));
        if (GUILayout.Button(running ? "stop" : "run")) Running_Switch();
        GUILayout.EndArea();
    }
}
using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    #region Fields

    [SerializeField] private MyNetworkManager netManager;

    //Network-managed Variables
    [SerializeField, SyncVar] private bool running;

    [SerializeField, SyncVar] private int playersConnected;

    [SerializeField, SyncVar] private int playerAlive;
    [SerializeField, SyncVar(hook = nameof(HookNewPlayerName))] private string newPlayerName;

    [SerializeField] private Vector2 guiOffset;
    private readonly SyncList<string> players = new SyncList<string>();

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
        ServerSetAlivePlayerCount(playersConnected);
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
    public void ServerSetPlayerCount(int count, NetworkConnection conn = null)
    {
        if (playersConnected > count)
        {
            var player = conn.identity.GetComponent<NetPlayerScript>();
            //  Detect if there's any player alive
            if (playerAlive > 0)
            {
                //  Game has run, but we don't know if the game is still running or not
                if (running)
                {
                    //  Game is still running
                    ServerDecreaseAlivePlayer(player.PlayerName);
                }
            }
            else
            {
                //  Game hasn't run yet (still in lobby)
                //  Call a LobbyManager function to do verification
                if (players.Contains(player.PlayerName))
                    players.Remove(player.PlayerName);
                FindObjectOfType<LobbyManager>().ServerOnClientDisconnect();
            }
        }
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
    /// Server-side function to decrease the player alive count
    /// </summary>
    /// <param name="count">optional: input bigger number to decrease more than one</param>
    [Server]
    public void ServerDecreaseAlivePlayer(string name)
    {
        if (players.Contains(name))
        {
            if (playerAlive > 2)
            {
                playerAlive--;
                players.Remove(name);
            }
            else if (playerAlive == 2)
            {
                playerAlive--;
                players.Remove(name);
                if (players.Count == 1)
                {
                    ServerSetRunningState(false);
                    RpcShowWinner(players[0]);
                }
            }
            else
            {
                Debug.LogWarning($"{ToString()}: tried to decrease playerAlive count when count is 0");
            }
        }
        else Debug.LogWarning($"player {name} doesn't exist in player name list");
    }

    /// <summary>
    /// Server-side function to set server's <c>newPlayerName</c> name
    /// </summary>
    /// <param name="name">New name</param>
    [Server]
    public void ServerSetNewPlayerName(string name)
    {
        if (!players.Contains(name))
        {
            newPlayerName = name;
            players.Add(name);
        }
        else
        {
            newPlayerName = $"{name}'s clone";
            players.Add(newPlayerName);
        }
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

    #region Client_Functions

    [Client]
    private void ClientShowWinner(string name)
    {
        FindObjectOfType<EndGameUIScript>().ShowWin(name);
    }

    #endregion Client_Functions

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
        conn.identity.GetComponent<NetPlayerScript>().ServerSetPlayerName(newPlayerName);
    }

    #endregion Commands

    #region ClientRPCs

    [ClientRpc]
    private void RpcShowWinner(string name)
    {
        ClientShowWinner(name);
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
        Start();
        playersConnected = NetworkServer.connections.Count;
        running = false;
        playerAlive = 0;
        newPlayerName = "";
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        Start();
        CmdSetNewPlayerName(netManager.LocalPlayerName);
        base.OnStartClient();
    }

    private void Start()
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

#if UNITY_EDITOR

    [Header("GUI settings")]
    [SerializeField] private bool showGUI;

    private void OnGUI()
    {
        if (!showGUI) return;
        GUILayout.BeginArea(new Rect(10 + guiOffset.x, 40 + guiOffset.y, 215, 9999));
        GUILayout.Label($"Registered player names:");
        foreach (var name in players)
        {
            GUILayout.Label(name);
        }
        GUILayout.EndArea();
    }

#endif
}
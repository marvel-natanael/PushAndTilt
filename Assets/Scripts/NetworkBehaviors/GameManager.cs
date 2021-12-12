using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    #region Fields

    [SerializeField, SyncVar(hook = nameof(SetPlayerConnected))] private int playersConnected;
    [SerializeField, SyncVar(hook = nameof(SetRunState))] private bool running;
    [SerializeField, SyncVar(hook = nameof(SetPlayerCount))] private int playerAlive;
    [SerializeField, SyncVar(hook = nameof(SetNewPlayerName))] private string newPlayerName;

    [Header("GUI settings")]
    private string runStatus;

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

    /// <summary>
    /// Hook function for <c>running</c>.
    /// </summary>
    /// <param name="old">Old value</param>
    /// <param name="_new">New value</param>
    [Server]
    public void SetRunState(bool _old, bool _new)
    {
        running = _new;
    }

    /// <summary>
    /// Hook function for <c>playerCount</c>.
    /// </summary>
    /// <param name="old">Old value</param>
    /// <param name="_new">New value</param>
    [Server]
    public void SetPlayerCount(int _old, int _new)
    {
        playerAlive = _new;
    }

    /// <summary>
    /// Hook function for <c>playersConnected</c>
    /// </summary>
    /// <param name="old">Old value</param>
    /// <param name="_new">New value</param>
    [Server]
    public void SetPlayerConnected(int _old, int _new)
    {
        playersConnected = _new;
    }

    #endregion Server_Functions

    #region Client_Functions

    private void SetNewPlayerName(string old, string @new)
    {
        newPlayerName = @new;
    }

    #endregion Client_Functions

    #region Commands

    [Command(requiresAuthority = false)]
    public void CmdSetNewPlayerName(string name)
    {
        RpcSetNewPlayerName(name);
        Debug.Log($"GameManager.cs/CmdSetNewPlayerName: Command ran");
    }

    #endregion Commands

    #region ClientRPCs

    [ClientRpc]
    private void RpcSetNewPlayerName(string name)
    {
        Debug.Log($"GameManager.cs/RpcSetNewPlayerName: Rpc recieved, newPlayerName = {newPlayerName}");
    }

    #endregion ClientRPCs

    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("Manager").Length > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnGUI()
    {
        if (!showGUI) return;
        GUILayout.BeginArea(new Rect(10 + guiOffset.x, 40 + guiOffset.y, 215, 9999));
        if (GUILayout.Button(runStatus)) Running_Switch();
        GUILayout.EndArea();
    }

    public void Running_Switch()
    {
        SetRunState(running, !running);
    }
}
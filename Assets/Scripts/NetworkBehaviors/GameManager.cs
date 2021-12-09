using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    [SerializeField, SyncVar(hook = nameof(SetPlayerConnected))] private int playersConnected;
    [SerializeField, SyncVar(hook = nameof(SetRunState))] private bool running;
    [SerializeField, SyncVar(hook = nameof(SetPlayerCount))] private int playerAlive;

    private readonly SyncDictionary<int, string> playerNames = new SyncDictionary<int, string>();
    private readonly SyncDictionary<int, NetworkIdentity> playerObjs = new SyncDictionary<int, NetworkIdentity>();

    [Header("GUI settings")]
    private string runStatus;

    [SerializeField] private bool showGUI;
    [SerializeField] private Vector2 guiOffset;

    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("Manager").Length > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    #region Properties

    public int PlayerCount { get => playerAlive; }
    public bool Running { get => running; }
    public int PlayersConnected => playersConnected;

    public SyncDictionary<int, string> PlayerNames => playerNames;

    public SyncDictionary<int, NetworkIdentity> PlayerObjs => playerObjs;

    #endregion Properties

    [Server]
    public void AddPlayerObject(int id, NetworkIdentity playerObj)
    {
        PlayerObjs_Callback(SyncIDictionary<int, NetworkIdentity>.Operation.OP_ADD, id, playerObj);
        Debug.Log("GameManager.cs/AddPlayerObject(): playerObj addded...");
        RpcSetPlayerNames();
    }

    [Client]
    public void RegisterClientName(int id, string name)
    {
        PlayerNames_Callback(SyncIDictionary<int, string>.Operation.OP_ADD, id, name);
        Debug.Log("GameManager.cs/RegisterClientName(): Client name registered...");
    }

    [ClientRpc]
    private void RpcSetPlayerNames()
    {
        foreach (var player in playerObjs)
        {
            player.Value.gameObject.GetComponent<NetPlayerScript>().SetPlayerName(playerNames[player.Key]);
        }
    }

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

    public override void OnStartClient()
    {
        base.OnStartClient();
        playerNames.Callback += PlayerNames_Callback;
        playerObjs.Callback += PlayerObjs_Callback;
    }

    private void PlayerObjs_Callback(SyncIDictionary<int, NetworkIdentity>.Operation op, int key, NetworkIdentity item)
    {
        switch (op)
        {
            case SyncIDictionary<int, NetworkIdentity>.Operation.OP_ADD:
                break;

            case SyncIDictionary<int, NetworkIdentity>.Operation.OP_SET:
                break;

            case SyncIDictionary<int, NetworkIdentity>.Operation.OP_REMOVE:
                break;

            case SyncIDictionary<int, NetworkIdentity>.Operation.OP_CLEAR:
                break;
        }
    }

    private void PlayerNames_Callback(SyncIDictionary<int, string>.Operation op, int key, string item)
    {
        switch (op)
        {
            case SyncIDictionary<int, string>.Operation.OP_ADD:
                break;

            case SyncIDictionary<int, string>.Operation.OP_SET:
                break;

            case SyncIDictionary<int, string>.Operation.OP_REMOVE:
                break;

            case SyncIDictionary<int, string>.Operation.OP_CLEAR:
                break;
        }
    }

    public void Running_Switch()
    {
        SetRunState(running, !running);
    }

    private void OnGUI()
    {
        if (!showGUI) return;
        GUILayout.BeginArea(new Rect(10 + guiOffset.x, 40 + guiOffset.y, 215, 9999));
        if (GUILayout.Button(runStatus)) Running_Switch();
        GUILayout.EndArea();
    }
}
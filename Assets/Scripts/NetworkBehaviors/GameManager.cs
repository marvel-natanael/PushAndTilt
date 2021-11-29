using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField, SyncVar(hook = nameof(SetPlayerConnected))] private int playersConnected;
    [SerializeField, SyncVar(hook = nameof(SetRunState))] private bool running;
    [SerializeField, SyncVar(hook = nameof(SetPlayerCount))] private int playerAlive;

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

    #endregion Properties

    #region Server_Functions

    /// <summary>
    /// Hook function for <c>running</c>.
    /// </summary>
    /// <param name="old">Old value</param>
    /// <param name="_new">New value</param>
    [Server]
    public void SetRunState(bool old, bool _new)
    {
        running = _new;
    }

    /// <summary>
    /// Hook function for <c>playerCount</c>.
    /// </summary>
    /// <param name="old">Old value</param>
    /// <param name="_new">New value</param>
    [Server]
    public void SetPlayerCount(int old, int _new)
    {
        playerAlive = _new;
    }

    /// <summary>
    /// Hook function for <c>playersConnected</c>
    /// </summary>
    /// <param name="old">Old value</param>
    /// <param name="_new">New value</param>
    [Server]
    public void SetPlayerConnected(int old, int _new)
    {
        playersConnected = _new;
    }

    [Server]
    private short CountPlayerNumber()
    {
        short playerCount = 5;
        //todo: Implement proper player counter
        return playerCount;
    }

    #endregion Server_Functions

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
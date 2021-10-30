using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField, SyncVar(hook = nameof(SetRunState))] private bool running;
    [SerializeField, SyncVar(hook = nameof(SetPlayerCount))] private int playerCount;

    [Header("GUI settings")]
    private string runStatus;

    [SerializeField] private bool showGUI;
    [SerializeField] private Vector2 guiOffset;

    public int PlayerCount { get => playerCount; }
    public bool Running { get => running; }

    /// <summary>
    /// Hook function for <c>running</c>.
    /// </summary>
    /// <param name="old">Old value</param>
    /// <param name="_new">New Value</param>
    private void SetRunState(bool old, bool _new)
    {
        running = _new;
    }

    /// <summary>
    /// Hook function for <c>playerCount</c>.
    /// </summary>
    /// <param name="old">Old value</param>
    /// <param name="_new">New value</param>
    private void SetPlayerCount(int old, int _new)
    {
        playerCount = _new;
    }

    public override void OnStartServer()
    {
        //todo remove this code if ready to implement player counter
        SetPlayerCount(playerCount, 5);
        base.OnStartServer();
    }

    private void Update()
    {
        runStatus = running ? "Pause" : "Resume";
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
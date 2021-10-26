using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField, SyncVar] private bool running;
    [SerializeField, SyncVar] private int playerCount;
    [SerializeField, Range(0.1f, 2.0f)] private float timeScale;

    [Header("GUI settings")]
    private string runStatus;

    [SerializeField] private bool showGUI;
    [SerializeField] private Vector2 guiOffset;

    public int PlayerCount { get => playerCount; }
    public bool Running { get => running; }

    /// <summary>
    /// Switches the runnning status, only server is able to change the status
    /// </summary>
    public void Running_Switch()
    {
        running = !running;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        running = false;
        playerCount = 5;
        timeScale = 1f;
    }

    private void Update()
    {
        Time.timeScale = timeScale;
        runStatus = running ? "Pause" : "Resume";
    }

    private void OnGUI()
    {
        if (!showGUI) return;
        GUILayout.BeginArea(new Rect(10 + guiOffset.x, 40 + guiOffset.y, 215, 9999));
        if (GUILayout.Button(runStatus)) Running_Switch();
        GUILayout.EndArea();
    }
}
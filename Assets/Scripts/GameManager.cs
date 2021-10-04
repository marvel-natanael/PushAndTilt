using Mirror;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField, Range(0.1f, 2.0f)] private float timeScale;
    private static int playerCount;
    private static bool running;

    [Header("GUI settings")]
    private string runStatus;
    [SerializeField] private bool showGUI;
    [SerializeField] private Vector2 guiOffset;

    public static int PlayerCount { get => playerCount; }
    public static bool Running { get => running; }

    public void Running_Switch()
    {
        running = !running;
    }

    // Start is called before the first frame update
    private void Start()
    {
        running = false;
        timeScale = 1f;

        //Debug only
        playerCount = 5;
    }

    // Update is called once per frame
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
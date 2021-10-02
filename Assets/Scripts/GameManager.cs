using Mirror;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField, Range(0.1f, 2.0f)] private float timeScale;
    private static int playerCount;
    private static bool running;

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
    }

    // Update is called once per frame
    private void Update()
    {
        Time.timeScale = timeScale;
    }
}
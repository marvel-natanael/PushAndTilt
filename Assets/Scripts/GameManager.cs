using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField, Range(0.1f, 2.0f)] private float timeScale;

    private static int playerCount;
    private static Vector2 screenPointZero;
    private static Vector2 screenPointOne;

    public static int PlayerCount { get => playerCount; }
    public static Vector2 ScreenPointZero { get => screenPointZero; }
    public static Vector2 ScreenPointOne { get => screenPointOne; }

    // Start is called before the first frame update
    private void Start()
    {
        //Normalize timescale
        Time.timeScale = 1;
        //Screen Calculation
        {
            {
                var temp = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z));
                screenPointZero.x = temp.x;
                screenPointZero.y = temp.y;
                //Debug.Log("ScreenPointZero.x = " + screenPointZero.x);
                //Debug.Log("ScreenPointZero.y = " + screenPointZero.y);
            }
            {
                var temp = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
                screenPointOne.x = temp.x;
                screenPointOne.y = temp.y;
                //Debug.Log("ScreenPointOne.x = " + screenPointOne.x);
                //Debug.Log("ScreenPointOne.y = " + screenPointOne.y);
            }
        }
        //Debug only
        playerCount = 5;
        //Debug.Log("PlayerCount = " + playerCount);
    }

    // Update is called once per frame
    private void Update()
    {
        Time.timeScale = timeScale;
    }
}
using Mirror;
using UnityEngine;

public static class GameScreen
{
    [SyncVar] private static Vector2 corner_BottomLeft;
    [SyncVar] private static Vector2 corner_TopRight;

    public static Vector2 Corner_TopRight { get => corner_TopRight; }
    public static Vector2 Corner_BottomLeft { get => corner_BottomLeft; }

    [Server]
    public static void CalculateScreen()
    {
        //Top Right Corner
        {
            var temp = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            corner_TopRight.x = temp.x;
            corner_TopRight.y = temp.y;
            Debug.Log("corner_TopRight.x = " + corner_TopRight.x);
            Debug.Log("corner_TopRight.y = " + corner_TopRight.y);
        }
        //Bottom Left Corner
        {
            var temp = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z));
            corner_BottomLeft.x = temp.x;
            corner_BottomLeft.y = temp.y;
            Debug.Log("corner_BottomLeft.x = " + corner_BottomLeft.x);
            Debug.Log("corner_BottomLeft.y = " + corner_BottomLeft.y);
        }
    }
}

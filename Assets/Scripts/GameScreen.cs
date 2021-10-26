using Mirror;
using UnityEngine;

public class GameScreen : NetworkBehaviour
{
    [SerializeField, SyncVar] private Vector2 corner_BottomLeft;
    [SerializeField, SyncVar] private Vector2 corner_TopRight;

    public Vector2 Corner_TopRight { get => corner_TopRight; }
    public Vector2 Corner_BottomLeft { get => corner_BottomLeft; }
    public float ScreenWidth_inWorldUnits { get => corner_TopRight.x - corner_BottomLeft.x; }
    public float ScreenHeight_inWorldUnits { get => corner_TopRight.y - corner_BottomLeft.y; }

    /// <summary>
    /// Calculate screen to world point references. Very important for the server to run this once as many object management are
    /// dependent on the data stored in <c>GameScreen</c>.
    /// </summary>
    [Server]
    public void CalculateScreen()
    {
        //Top Right Corner
        {
            var temp = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            corner_TopRight.x = temp.x;
            corner_TopRight.y = temp.y;
        }
        //Bottom Left Corner
        {
            var temp = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z));
            corner_BottomLeft.x = temp.x;
            corner_BottomLeft.y = temp.y;
        }
    }

    /// <summary>
    /// Show positions as logs on unity console.
    /// </summary>
    public void ShowData()
    {
        Debug.Log("corner_TopRight.x = " + corner_TopRight.x);
        Debug.Log("corner_TopRight.y = " + corner_TopRight.y);
        Debug.Log("corner_BottomLeft.x = " + corner_BottomLeft.x);
        Debug.Log("corner_BottomLeft.y = " + corner_BottomLeft.y);
    }
}
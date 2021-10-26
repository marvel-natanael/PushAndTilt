using Mirror;
using System;
using UnityEngine;

public class GameScreen : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetBottomLeft))] private Vector2 corner_BottomLeft;
    [SyncVar(hook = nameof(SetTopRight))] private Vector2 corner_TopRight;
    [SerializeField] private bool showGUI;
    [SerializeField] private Vector2 guiOffset;

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
            SetTopRight(corner_TopRight, new Vector2(temp.x, temp.y));
        }
        //Bottom Left Corner
        {
            var temp = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z));
            SetBottomLeft(corner_BottomLeft, new Vector2(temp.x, temp.y));
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

    /// <summary>
    /// Hook function for <c>corner_BottomLeft</c>.
    /// </summary>
    /// <param name="old">Old value</param>
    /// <param name="_new">New value</param>
    private void SetBottomLeft(Vector2 old, Vector2 _new)
    {
        corner_BottomLeft = _new;
    }

    /// <summary>
    /// Hook function for <c>corner_TopRight</c>.
    /// </summary>
    /// <param name="old">Old value</param>
    /// <param name="_new">New value</param>
    private void SetTopRight(Vector2 old, Vector2 _new)
    {
        corner_TopRight = _new;
    }

    public override void OnStartServer()
    {
        CalculateScreen();
        base.OnStartServer();
    }

    private void OnGUI()
    {
        if (!showGUI) return;
        GUILayout.BeginArea(new Rect(10 + guiOffset.x, 40 + guiOffset.y, 215, 9999));
        if (GUILayout.Button("Show data")) ShowData();
        GUILayout.EndArea();
    }
}
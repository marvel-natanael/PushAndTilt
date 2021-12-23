using Mirror;
using UnityEngine;

public class ObstacleScript : NetworkBehaviour
{
    private NetworkObstacle netObs;
    private GameScreen screen;
    private Vector2 speed;

    private Direction direction;

    public Vector2 Speed { get => speed; }

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (!(netObs = FindObjectOfType<NetworkObstacle>()))
            Debug.LogError($"{ToString()}: netObs not found");
        if (!(screen = FindObjectOfType<GameScreen>()))
            Debug.LogError($"{ToString()}: screen not found");
    }

    [Server]
    public void SetVelocity(Vector2 vel)
    {
        GetComponent<Rigidbody2D>().velocity = vel;
    }

    [Server]
    private void ServerDetectPosition()
    {
        if ((transform.position.y > screen.Corner_TopRight.y + 2.0f))
        {
            NetworkServer.Destroy(gameObject);
        }

        if (transform.position.x < screen.Corner_BottomLeft.x - 2.0f)
        {
            NetworkServer.Destroy(gameObject);
        }

        if (transform.position.x > screen.Corner_TopRight.x + 2.0f)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isServer)
        {
            ServerDetectPosition();
        }
    }
}
using Mirror;
using UnityEngine;

public class ObstacleScript : NetworkBehaviour
{
    private GameScreen screen;
    private Vector2 speed;

    public Vector2 Speed { get => speed; }

    public override void OnStartServer()
    {
        if (!(screen = FindObjectOfType<GameScreen>()))
            Debug.LogError($"{ToString()}: screen not found");
        transform.SetParent(GameObject.FindGameObjectWithTag("obstacleManager").transform);
        base.OnStartServer();
    }

    [Server]
    public void SetVelocity(Vector2 vel)
    {
        GetComponent<Rigidbody2D>().velocity = vel;
    }

    [Server]
    private void ServerDetectPosition()
    {
        if (transform.position.y > screen.Corner_TopRight.y + 2.0f)
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
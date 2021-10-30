using Mirror;
using UnityEngine;

public class ObstacleScript : NetworkBehaviour
{
    private GameScreen screen;
    private Vector2 speed;

    public Vector2 Speed { get => speed; }

    private void Start()
    {
        screen = GameObject.FindGameObjectWithTag("screen").GetComponent<GameScreen>();
        transform.SetParent(GameObject.FindGameObjectWithTag("obstacleManager").transform);
    }

    public void SetVelocity(Vector2 vel)
    {
        GetComponent<Rigidbody2D>().velocity = vel;
    }

    private void Update()
    {
        if (isServer)
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
    }
}
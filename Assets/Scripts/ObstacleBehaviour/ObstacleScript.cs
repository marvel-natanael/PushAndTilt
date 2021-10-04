using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    private Vector2 speed;
    private float bottom;
    private float rightWall;
    private float leftWall;

    public Vector2 Speed { get => speed; set => speed = value; }

    private void Start()
    {
        bottom = GameScreen.Corner_BottomLeft.y;
        leftWall= GameScreen.Corner_BottomLeft.x;
        rightWall= GameScreen.Corner_TopRight.x;
    }

    public void SetVelocity(Vector2 vel)
    {
        GetComponent<Rigidbody2D>().velocity = vel;
    }

    private void Update()
    {
        if (transform.position.y < bottom - 2.0f)
        {
            Destroy(gameObject);
        }
        if (transform.position.x < leftWall - 2.0f)
        {
            Destroy(gameObject);
        }
        if (transform.position.x > rightWall + 2.0f)
        {
            Destroy(gameObject);
        }
    }
}
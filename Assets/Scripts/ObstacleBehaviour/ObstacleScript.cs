using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    private Vector2 speed;
    private float bottom;
    private float rightWall;
    private float leftWall;
    private Rigidbody2D rb;

    private void Start()
    {
        bottom = GameManager.ScreenPointZero.y;
        leftWall= GameManager.ScreenPointZero.x;
        rightWall= GameManager.ScreenPointOne.x;

        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(speed.x, speed.y);
    }

    private void Update()
    {
        if (transform.position.y < bottom - 1.0f)
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

    public Vector2 Speed { set => speed = value; }
}
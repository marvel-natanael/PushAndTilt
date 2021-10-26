using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    GameScreen screen;
    private Vector2 speed;
    private float top;
    private float rightWall;
    private float leftWall;

    public Vector2 Speed { get => speed; set => speed = value; }

    private void Start()
    {
        screen = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameScreen>();
        top = screen.Corner_TopRight.y;
        leftWall= screen.Corner_BottomLeft.x;
        rightWall= screen.Corner_TopRight.x;
    }

    public void SetVelocity(Vector2 vel)
    {
        GetComponent<Rigidbody2D>().velocity = vel;
    }

    private void Update()   
    {
        if (transform.position.y > top + 2.0f)
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
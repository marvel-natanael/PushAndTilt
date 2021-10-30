using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    private Vector2 speed;
    private float bottom;
    private float rightWall;
    private float leftWall;

    private void Start()
    {
        bottom = GameScreen.Corner_BottomLeft.y;
        leftWall= GameScreen.Corner_BottomLeft.x;
        rightWall= GameScreen.Corner_TopRight.x;
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
}
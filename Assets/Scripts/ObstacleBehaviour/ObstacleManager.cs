using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private GameObject verticalSweeper;
    [SerializeField] private GameObject horizontalSweeper;
    [SerializeField] private GameObject hole;
    [SerializeField, Range(1.0f, 2.0f), ContextMenuItem("Randomize", "RandomTolerance")] private float holeTolerance;
    [SerializeField] private float speed;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float maxSpeed;

    private Vector2 holeSize;

    private void Start()
    {
        holeSize = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>().sprite.bounds.size * holeTolerance / 2;
    }

    private void Update()
    {
        //Detect if there is still an obstacle
        if (transform.childCount == 0)
        {
            var num = Random.Range(1.0f, 10.0f);
            if (num <= 5.0f)
            {
                //vertical spawn
                SpawnObstacle(0);
            }
            else if (num <= 7.5f)
            {
                //horizontal left-right spawn
                SpawnObstacle(1);
            }
            else if (num <= 10.0f)
            {
                //horizontal right-left spawn
                SpawnObstacle(2);
            }
        }

        //Speed modifier
        if (speed < maxSpeed)
        {
            speed += (speedMultiplier / 100) * Time.deltaTime;
        }
    }

    private void RandomTolerance()
    {
        holeTolerance = Random.Range(1.0f, 2.0f);
    }

    private void SpawnObstacle(short mode)
    {
        short count;
        if (GameManager.PlayerCount > 1)
        {
            count = (short)Random.Range(GameManager.PlayerCount - 1, GameManager.PlayerCount);
        }
        else
        {
            count = 1;
        }
        switch (mode)
        {
            //Vertical Spawn
            case 0:
                {
                    var leftWall = GameScreen.Corner_TopRight.x;
                    var rightWall = GameScreen.Corner_BottomLeft.x;
                    var spawnPoint = GameScreen.Corner_BottomLeft.y + 1.0f;
                    {
                        var temp = Instantiate(verticalSweeper, transform);
                        temp.transform.position = new Vector3(0, spawnPoint, 0);
                        temp.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -speed / 1.5f);
                    }
                    //Create holes
                    for (int i = 0; i < count; i++)
                    {
                        var temp = Instantiate(hole, transform.GetChild(0).transform);
                        temp.transform.position = new Vector3(Random.Range(leftWall + holeSize.x, rightWall - holeSize.x), spawnPoint, 0);
                        var sprite = temp.GetComponent<SpriteMask>().sprite.bounds.size;
                        temp.transform.localScale = new Vector3(holeSize.x / sprite.x, holeSize.y / sprite.y, 1);
                    }
                }
                break;
            //Left-Right Spawn
            case 1:
                {
                    var ground = GameScreen.Corner_TopRight.y;
                    var ceiling = GameScreen.Corner_BottomLeft.y;
                    var spawnPoint = GameScreen.Corner_TopRight.x - 1.0f;
                    {
                        var temp = Instantiate(horizontalSweeper, transform);
                        temp.transform.position = new Vector3(spawnPoint, 0, 0);
                        temp.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
                    }
                    //Create holes
                    for (int i = 0; i < count; i++)
                    {
                        var temp = Instantiate(hole, transform.GetChild(0).transform);
                        temp.transform.position = new Vector3(spawnPoint, Random.Range(ground + holeSize.y, ceiling - holeSize.y), 0);
                        var sprite = temp.GetComponent<SpriteMask>().sprite.bounds.size;
                        temp.transform.localScale = new Vector3(holeSize.x / sprite.x, holeSize.y / sprite.y, 1);
                    }
                }
                break;
            //Right-Left Spawn
            case 2:
                {
                    var ground = GameScreen.Corner_TopRight.y;
                    var ceiling = GameScreen.Corner_BottomLeft.y;
                    var spawnPoint = GameScreen.Corner_BottomLeft.x + 1.0f;
                    {
                        var temp = Instantiate(horizontalSweeper, transform);
                        temp.transform.position = new Vector3(spawnPoint, 0, 0);
                        temp.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0);
                    }
                    //Create holes
                    for (int i = 0; i < count; i++)
                    {
                        var temp = Instantiate(hole, transform.GetChild(0).transform);
                        temp.transform.position = new Vector3(spawnPoint, Random.Range(ground + holeSize.y, ceiling - holeSize.y), 0);
                        var sprite = temp.GetComponent<SpriteMask>().sprite.bounds.size;
                        temp.transform.localScale = new Vector3(holeSize.x / sprite.x, holeSize.y / sprite.y, 1);
                    }
                }
                break;

            default:
                break;
        }
    }
}
using UnityEngine;

public class VerticalSpawner: MonoBehaviour
{
    [SerializeField] private GameObject obstacle;
    [SerializeField] private GameObject hole;
    private float charSize;
    private bool isActive = false;

    public float CharSize { get => charSize; set => charSize = value; }
    public bool IsActive { get => isActive; }

    public void SpawnVerticalObstacle()
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
        //Spawning the sweeper
        {
            var temp = Instantiate(obstacle, transform);
            {
                var leftWall = GameManager.ScreenPointZero.x;
                var rightWall = GameManager.ScreenPointOne.x;
                var spawnPoint = GameManager.ScreenPointOne.y * 1.2f;
                temp.transform.position = new Vector3(rightWall - leftWall, spawnPoint, 0);
            }
            for (int i = 0; i < count; i++)
            {
                var temp2 = Instantiate(hole, transform.GetChild(0));
                var leftWall = GameManager.ScreenPointZero.x;
                var rightWall = GameManager.ScreenPointOne.x;
                var spawnPoint = GameManager.ScreenPointOne.y * 1.2f;
                temp.transform.position = new Vector3(Random.Range(leftWall + charSize / 2, rightWall - charSize / 2), spawnPoint, 0);
            }
        }
    }
}
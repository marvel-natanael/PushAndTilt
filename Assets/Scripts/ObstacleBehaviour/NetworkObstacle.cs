using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObstacle : NetworkBehaviour
{
    [Header("Sweeper Prefab")]
    [SerializeField] private GameObject material;

    [Header("Generator Settings")]
    [SerializeField] private short reduceHoleBy;

    [SerializeField, Range(1.0f, 2.0f), ContextMenuItem("Randomize", "RandomTolerance")] private float holeTolerance;

    [Header("Runtime Variables")]
    [SerializeField] private float speed;

    [SerializeField] private float speedMultiplier;
    [SerializeField] private float maxSpeed;

    [Header("Synchronized Variables")]
    [SerializeField, SyncVar] private Direction direction;

#if UNITY_EDITOR

    [Header("GUI Options")]
    [SerializeField] private bool showGUI;

    [SerializeField] private Vector2 guiOffset;
#endif
    private SyncList<float> holes = new SyncList<float>();

    public enum Direction
    {
        Down, Left, Right
    }

    [Server]
    private bool GenerateObstacle()
    {
        holes.Clear();
        //RNG: determine where obstacle will originate
        {
            var num = Random.Range(0f, 1f);
            var holeCount = Mathf.Clamp(Random.Range(GameManager.PlayerCount - reduceHoleBy, GameManager.PlayerCount), 1, GameManager.PlayerCount);
            var num2 = (GameScreen.Corner_TopRight.y - GameScreen.Corner_BottomLeft.y) / holeCount;
            //Chance 25%
            if (num < 0.25f)
            {
                direction = Direction.Right;
            }
            //Chance 25%
            else if (num < 0.5f)
            {
                direction = Direction.Left;
            }
            //Chance 50%
            else
            {
                num2 = (GameScreen.Corner_TopRight.x - GameScreen.Corner_BottomLeft.x) / holeCount;
                direction = Direction.Down;
            }
            //Generate a list of holes position
            {
                var temp = new List<float>();
                for (short i = 0; i < holeCount; i++)
                {
                    temp.Add(Random.Range(i * num2, (i + 1) * num2) - ((GameScreen.Corner_TopRight.y - GameScreen.Corner_BottomLeft.y) / 2));
                }
                temp.Sort();
                for (short i = 0; i < holeCount; i++)
                {
                    holes.Add(temp[i]);
                }
            }
        }
        return true;
    }

    [Client]
    private bool CreateObstacle()
    {
        Vector2 vel = Vector2.zero;
        //First obstacle
        {
            var instance = Instantiate(material);
            instance.transform.localScale = new Vector3(1, (GameScreen.Corner_TopRight.y - (holes[holes.Count - 1] + holeTolerance)) / 2, 1);
            switch (direction)
            {
                case Direction.Left:
                    instance.transform.localPosition = new Vector3(GameScreen.Corner_TopRight.x + 1f, holes[holes.Count - 1] + holeTolerance);
                    vel = new Vector2(-speed * speedMultiplier, 0);
                    break;

                case Direction.Right:
                    instance.transform.localPosition = new Vector3(GameScreen.Corner_BottomLeft.x - 1f, holes[holes.Count - 1] + holeTolerance);
                    vel = new Vector2(speed * speedMultiplier, 0);
                    break;

                case Direction.Down:
                    instance.transform.localPosition = new Vector3(holes[holes.Count - 1] + holeTolerance, GameScreen.Corner_TopRight.y + 1f);
                    instance.transform.localScale = new Vector3((GameScreen.Corner_TopRight.x - holes[holes.Count - 1] + holeTolerance) / 2, 1, 1);
                    vel = new Vector2(0, -speed * speedMultiplier);
                    break;
            }
            instance.GetComponent<ObstacleScript>().SetVelocity(vel);
            instance.transform.parent = transform;
        }

        //first+1 -> holeCount-1 obstacle
        if (holes.Count > 1)
        {
            for (int i = holes.Count - 2; i > -1; i--)
            {
                var instance = Instantiate(material);
                instance.transform.localScale = new Vector3(1, holes[i + 1] - holeTolerance - (holes[i] + holeTolerance), 1);
                switch (direction)
                {
                    case Direction.Left:
                        instance.transform.localPosition = new Vector3(GameScreen.Corner_TopRight.x + 1f, holes[i] + holeTolerance);
                        vel = new Vector2(-speed * speedMultiplier, 0);
                        break;

                    case Direction.Right:
                        instance.transform.localPosition = new Vector3(GameScreen.Corner_BottomLeft.x - 1f, holes[i] + holeTolerance);
                        vel = new Vector2(speed * speedMultiplier, 0);
                        break;

                    case Direction.Down: //Basically, down
                        instance.transform.localPosition = new Vector3(holes[i] + holeTolerance, GameScreen.Corner_TopRight.y + 1f);
                        instance.transform.localScale = new Vector3(holes[i + 1] - holeTolerance - (holes[i] + holeTolerance), 1, 1);
                        vel = new Vector2(0, -speed * speedMultiplier);
                        break;
                }
                instance.GetComponent<ObstacleScript>().SetVelocity(vel);
                instance.transform.parent = transform;
            }
        }

        //Last obstacle
        {
            var instance = Instantiate(material);
            instance.transform.localScale = new Vector3(1, holes[1] - holeTolerance - GameScreen.Corner_BottomLeft.y, 1);
            switch (direction)
            {
                case Direction.Left:
                    instance.transform.localPosition = new Vector3(GameScreen.Corner_TopRight.x + 1f, GameScreen.Corner_BottomLeft.y);
                    vel = new Vector2(-speed * speedMultiplier, 0);
                    break;

                case Direction.Right:
                    instance.transform.localPosition = new Vector3(GameScreen.Corner_BottomLeft.x - 1f, GameScreen.Corner_BottomLeft.y);
                    vel = new Vector2(speed * speedMultiplier, 0);
                    break;

                case Direction.Down: //Basically, down
                    instance.transform.localPosition = new Vector3(GameScreen.Corner_BottomLeft.x, GameScreen.Corner_TopRight.y + 1f);
                    instance.transform.localScale = new Vector3(holes[1] - holeTolerance - GameScreen.Corner_BottomLeft.x, 1, 1);
                    vel = new Vector2(0, -speed * speedMultiplier);
                    break;
            }
            instance.GetComponent<ObstacleScript>().SetVelocity(vel);
            instance.transform.parent = transform;
        }
        return true;
    }

    private void Start()
    {
        if (isServer) GenerateObstacle();
    }

    private void Update()
    {
        if (GameManager.Running)
        {
            if (transform.childCount < 1)
            {
                if (isServer)
                {
                    GenerateObstacle();
                }
                if (isClient)
                {
                    CreateObstacle();
                }
            }
            if (speed < maxSpeed)
            {
                speed += speedMultiplier / 100 * Time.deltaTime;
            }
        }
    }

    private void OnGUI()
    {
        if (!showGUI) return;
        GUILayout.BeginArea(new Rect(10 + guiOffset.x, 40 + guiOffset.y, 215, 9999));
        if (GUILayout.Button("Generate Obstacle"))
        {
            Debug.Log("Obstacle Generated? = " + GenerateObstacle().ToString());
        }
        if (GUILayout.Button("Create Obstacle"))
        {
            Debug.Log("Obstacle Created? = " + CreateObstacle().ToString());
        }
        GUILayout.Label("Number of holes = " + holes.Count);
        GUILayout.EndArea();
    }

    private void OnDrawGizmos()
    {
        foreach (float item in holes)
        {
            if (direction != Direction.Down)
            {
                Gizmos.DrawSphere(new Vector3(0, item), 0.1f);
            }
            else
            {
                Gizmos.DrawSphere(new Vector3(item, 0), 0.1f);
            }
        }
    }
}
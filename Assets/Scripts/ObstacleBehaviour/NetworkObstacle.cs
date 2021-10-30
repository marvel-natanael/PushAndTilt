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

    private SyncList<float> holes = new SyncList<float>();

    public enum Direction
    {
        Down, Left, Right
    }

    [Client]
    private bool CreateObstacle()
    {
        Vector2 vel;
        //First obstacle
        {
            var instance = Instantiate(material);
            instance.transform.localScale = new Vector3(1, (GameScreen.Corner_TopRight.y - holes[holes.Count - 1] + holeTolerance) / 2, 1);
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

                default: //Basically, down
                    instance.transform.localPosition = new Vector3(holes[holes.Count - 1] + holeTolerance, GameScreen.Corner_TopRight.y + 1f);
                    instance.transform.localScale = new Vector3((GameScreen.Corner_TopRight.x - holes[holes.Count - 1] + holeTolerance) / 2, 1, 1);
                    vel = new Vector2(0, -speed * speedMultiplier);
                    break;
            }
            instance.GetComponent<Rigidbody2D>().velocity = vel;
            instance.transform.parent = transform;
        }
        //first+1 -> holeCount-1 obstacle
        for (int i = 0; i < holes.Count; i++)
        {
        }
        //Last obstacle
        {
        }
        return true;
    }

    [Server]
    private bool GenerateObstacle()
    {
        //RNG: determine where obstacle will originate
        {
            var num = Random.Range(0f, 1f);
            var holeCount = Mathf.Clamp(Random.Range(GameManager.PlayerCount - reduceHoleBy, GameManager.PlayerCount), 1, GameManager.PlayerCount);
            var num2 = (GameScreen.Corner_TopRight.y - GameScreen.Corner_BottomLeft.y) / holeCount;
            //Chance 25%
            if (num < 0.75f)
            {
                direction = Direction.Right;
            }
            //Chance 25%
            else if (num > 0.5f)
            {
                direction = Direction.Left;
            }
            //Chance 50%
            else
            {
                num2 = (GameScreen.Corner_TopRight.x - GameScreen.Corner_BottomLeft.x) / holeCount;
                direction = Direction.Down;
            }
            //Generate a list of
            {
                var temp = new List<float>();
                for (short i = 0; i < holeCount; i++)
                {
                    temp.Add(Random.Range(i * num2, (i + 1) * num2));
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

    private void Update()
    {
        if (GameManager.Running)
        {
            if (isClient)
            {
                if (transform.childCount < 1)
                {
                    CreateObstacle();
                }
            }
            else
            {
                GenerateObstacle();
                //Speed modifier
                if (speed < maxSpeed)
                {
                    speed += (speedMultiplier / 100) * Time.deltaTime;
                }
            }
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate Obstacle"))
        {
            Debug.Log("Obstacle Generated? = " + GenerateObstacle().ToString());
        }
        if (GUILayout.Button("Create Obstacle"))
        {
            Debug.Log("Obstacle Created? = " + CreateObstacle().ToString());
        }
    }
}
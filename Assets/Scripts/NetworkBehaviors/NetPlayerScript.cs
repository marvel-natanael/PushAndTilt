using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class NetPlayerScript : NetworkBehaviour
{
    #region References

    private GameManager manager;
    private MyNetworkManager netManager;
    private GameScreen screen;
    private TextMesh nameLabel;
    private Rigidbody2D rb;
    private float charRadius;
    [SerializeField] private TextMesh playerReadyLabel;
    [SerializeField] private TextMesh playerNameLabel;

    #endregion References

    #region Jumping Fields

    [SerializeField]
    private GameObject jumpHeightIndicator;

    [SerializeField] private float chargeHeight;
    [SerializeField] private float chargeLimit;
    [SerializeField] private float chargeMultiplier;
    [SerializeField] private float charge;
    [SerializeField] private bool isCharging;

    #endregion Jumping Fields

    #region Movement Fields

    [Header("Player speed parameters")]
    [SerializeField]
    private float speed;

    [SerializeField]
    private float acceleration;

    [SerializeField]
    private float speedLimit;

    [SerializeField, Range(0.01f, 1f)]
    private float restitution;

#if !UNITY_EDITOR
    private float dirX, dirY;
#endif

    #endregion Movement Fields

    #region Network Managed Fields

    [Header("Network Managed Fields")]
    [SerializeField, SyncVar(hook = nameof(HookSetPlayerName))] private string playerName;

    [SerializeField, SyncVar(hook = nameof(HookSetPlayerReady))] private bool isReady;
    [SerializeField, SyncVar(hook = nameof(HookSetPlayerActive))] private bool active;

    #endregion Network Managed Fields

    #region Properties

    public bool isAlive => active;
    public string PlayerName => playerName;
    public bool IsReady => isReady;

    #endregion Properties

    #region Movement_Handlers

    /// <summary>
    /// Calculates the maximum height of player's jump.
    /// </summary>
    /// <returns>float value containing the max jump speed</returns>
    private float CalculateMaxVelocity()
    {
        var temp = Mathf.Sqrt(2 * (Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale) * (screen.Corner_TopRight.y - transform.position.y));
        Debug.Log($"MaxHeight={temp}");
        return temp;
    }

    /// <summary>
    /// Handles the player movement
    /// </summary>
    private void HandleMovements()
    {
        //Horizontal Movements
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.D) ^ Input.GetKey(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.D))
            {
                if (speed < speedLimit) speed += Time.deltaTime * acceleration;
            }
            if (Input.GetKey(KeyCode.A))
            {
                if (speed > -speedLimit) speed -= Time.deltaTime * acceleration;
            }
        }
        else
        {
            speed = 0;
        }
        rb.velocity = new Vector2(rb.velocity.x + speed, rb.velocity.y);
        //X pos clamping
        {
            if (transform.position.x + charRadius > screen.Corner_TopRight.x)
            {
                var deep = transform.position.x + charRadius - screen.Corner_TopRight.x;
                rb.velocity = new Vector2(-(Mathf.Abs(rb.velocity.x) + deep) * restitution, rb.velocity.y);
            }
            if (transform.position.x - charRadius < screen.Corner_BottomLeft.x)
            {
                var deep = transform.position.x - charRadius + screen.Corner_TopRight.x;
                rb.velocity = new Vector2(Mathf.Abs(rb.velocity.x + deep) * restitution, rb.velocity.y);
            }
        }
#else
            dirX = Input.acceleration.x * speed;
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, -8.5f, 8.5f), transform.position.y);
            rb.velocity = new Vector2(dirX, rb.velocity.y);
#endif
        //Vertical Movements
        if (rb.velocity.y == 0)
        {
            if (isCharging == true)
            {
                if (charge <= chargeLimit)
                {
                    charge += Time.fixedDeltaTime * chargeMultiplier * 10;
                    if (transform.localScale.y > 0.2f)
                    {
                        transform.localScale = new Vector3(transform.localScale.x + Time.fixedDeltaTime, transform.localScale.y - Time.fixedDeltaTime, transform.localScale.z);
                    }
                }
                //CalculateChargeVisualization();
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + charge);
                transform.localScale = Vector3.one;
                charge = 0;
            }
        }
    }

    /// <summary>
    /// Handles the player's screen touch.
    /// </summary>
    private void TouchHandler()
    {
        isCharging = Input.GetMouseButton(0);
    }

    #endregion Movement_Handlers

    #region Server_Functions

    /// <summary>
    /// Server-side function to set this connection's <c>playerName</c> on the server
    /// </summary>
    /// <param name="name">New name</param>
    [Server]
    public void ServerSetPlayerName(string name)
    {
        playerName = name;
    }

    /// <summary>
    /// Server-side function to set this connection's <c>isReady</c> on the server
    /// </summary>
    /// <param name="state">New value</param>
    [Server]
    public void ServerSetPlayerReadyState(bool state)
    {
        isReady = state;
    }

    [Server]
    public void ServerSetPlayerAliveState(bool state)
    {
        active = state;
        if (isReady) isReady = false;
    }

    #endregion Server_Functions

    #region Client_Functions

    /// <summary>
    /// Client-side function that changes player's ready status
    /// </summary>
    /// <param name="state">The ready state of this player</param>
    [Client]
    private void ClientSetPlayerReadyState(bool state)
    {
        if (isReady) playerReadyLabel.text = $"Ready";
        else playerReadyLabel.text = $"";
    }

    #endregion Client_Functions

    #region Commands

    [Command]
    public void CmdDie(NetworkConnectionToClient conn = null)
    {
        ServerSetPlayerAliveState(false);
    }

    #endregion Commands

    #region Hook_Functions

    /// <summary>
    /// Hook function for <c>playerName</c>
    /// </summary>
    /// <param name="old">Old value</param>
    /// <param name="new">New value</param>
    [Client]
    private void HookSetPlayerName(string old, string @new)
    {
        playerName = @new;
        playerNameLabel.text = playerName;
    }

    /// <summary>
    /// Hook function for <c>isReady</c>
    /// </summary>
    /// <param name="old">Old value</param>
    /// <param name="new">New Value</param>
    [Client]
    private void HookSetPlayerReady(bool old, bool @new)
    {
        isReady = @new;
        ClientSetPlayerReadyState(isReady);
    }

    [Client]
    public void HookSetPlayerActive(bool old, bool @new)
    {
        active = @new;
        chargeLimit = CalculateMaxVelocity();
    }

    #endregion Hook_Functions

    private void Awake()
    {
        if (!(netManager = FindObjectOfType<MyNetworkManager>()))
        {
            Debug.LogError("NetPlayerScript.cs/Awake(): netManager is missing!");
        }
        if (!(screen = GameObject.FindGameObjectWithTag("screen").GetComponent<GameScreen>()))
        {
            Debug.LogError("NetPlayerScript.cs/Awake(): screen is missing!");
        }
        if (!(jumpHeightIndicator = transform.GetChild(0).gameObject))
        {
            Debug.LogError("NetPlayerScript.cs/Awake(): jumpHeightIndicator is missing!");
        }
        if (!(rb = GetComponent<Rigidbody2D>()))
        {
            Debug.LogError("NetPlayerScript.cs/Awake(): rb is missing!");
        }
        if (!(playerReadyLabel = transform.GetChild(0).GetComponent<TextMesh>()))
        {
            Debug.LogError("NetPlayerScript.cs/Awake(): readySprite is missing!");
        }
        if (!(nameLabel = transform.GetChild(1).GetComponent<TextMesh>()))
        {
            Debug.LogError("NetPlayerScript.cs/Awake(): nameLabel is missing!");
        }
        if (!(manager = FindObjectOfType<GameManager>()))
        {
            Debug.LogError("NetPlayerScript.cs/Awake(): manager is missing!");
        }
    }

    private void Start()
    {
        //move
        speed = 0f;
        acceleration = 2.5f;
        speedLimit = 10f;

        //jump
        isCharging = false;
        charge = 0f;
        chargeHeight = 0f;
        chargeMultiplier = 2.5f;
        restitution = 0.01f;
        charRadius = GetComponent<SpriteRenderer>().bounds.size.x / 2f;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (active)
            {
                Debug.Log("Movement is activated");
                TouchHandler();
                HandleMovements();
            }
        }
    }
}
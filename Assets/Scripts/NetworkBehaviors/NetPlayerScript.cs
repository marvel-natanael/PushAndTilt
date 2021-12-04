using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class NetPlayerScript : NetworkBehaviour
{
    private GameScreen screen;
    private TextMesh nameLabel;
    private SpriteRenderer readySprite;
    private Rigidbody2D rb;
    private float charRadius;
    [SerializeField] private List<Sprite> readySprites;

    #region Jumping Fields

    [Header("Player jump charge parameters")]
    [SerializeField]
    private GameObject jumpHeightIndicator;

    [SerializeField]
    private float chargeHeight;

    [SerializeField]
    private float chargeLimit;

    [SerializeField]
    private float chargeMultiplier;

    [SerializeField]
    private float charge;

    [SerializeField]
    private bool isCharging;

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

    #endregion Movement Fields

    #region Network Managed Fields

    [Header("Network Managed Fields")]
    [SerializeField, SyncVar(hook = nameof(SetPlayerAliveStatus))] private bool isActive;

    [SerializeField, SyncVar(hook = nameof(SetReadyState))] private bool isReady;
    [SerializeField, SyncVar(hook = nameof(SetPlayerName))] private string playerName;

    #endregion Network Managed Fields

    #region Properties

    public bool isAlive => isActive;

    #endregion Properties

#if !UNITY_EDITOR
    private float dirX, dirY;
#endif

    private void Awake()
    {
        if (!(screen = GameObject.FindGameObjectWithTag("screen").GetComponent<GameScreen>()))
        {
            Debug.LogError("NetPlayerScript.cs/Awake(): screen is missing!");
        }
        if (!(jumpHeightIndicator = transform.GetChild(0).gameObject))
        {
            Debug.LogError("NetPlayerScript.cs/Awake(): jumpHeightIndicator is missing!");
        }
        charRadius = GetComponent<SpriteRenderer>().bounds.size.x / 2;
        if (!(rb = GetComponent<Rigidbody2D>()))
        {
            Debug.LogError("NetPlayerScript.cs/Awake(): rb is missing!");
        }
        if (!(nameLabel = transform.GetComponentInChildren<TextMesh>()))
        {
            Debug.LogError("NetPlayerScript.cs/Awake(): nameLabel is missing!");
        }
        if (!(readySprite = transform.GetComponentInChildren<SpriteRenderer>()))
        {
            Debug.LogError("NetPlayerScript.cs/Awake(): readySprite is missing!");
        }
    }

    private void Start()
    {
        chargeLimit = CalculateMaxVelocity();
        isCharging = false;
        SetPlayerAliveStatus(false, false);
        SetReadyState(false, false);
        if (isLocalPlayer)
        {
            SetPlayerName(string.Empty, FindObjectOfType<MyNetworkManager>().PlayerName);
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            TouchHandler();
            HandleMovements();
        }
    }

    /// <summary>
    /// Calculates the maximum height of player's jump.
    /// </summary>
    /// <returns>float value containing the max jump speed</returns>
    private float CalculateMaxVelocity()
    {
        return Mathf.Sqrt(2 * (Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale) * (screen.Corner_TopRight.y - transform.position.y));
    }

    /// <summary>
    /// Handles the player movement
    /// </summary>
    private void HandleMovements()
    {
        if (isLocalPlayer)
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
    }

    /// <summary>
    /// Handles the player's screen touch.
    /// </summary>
    private void TouchHandler()
    {
        isCharging = Input.GetMouseButton(0);
    }

    /// <summary>
    /// [Button Function] Toggles player's ready state
    /// </summary>
    public void ToggleReady()
    {
        if (isLocalPlayer)
        {
            SetReadyState(isReady, !isReady);
            if (isReady)
            {
                readySprite.sprite = readySprites[1];
            }
            else readySprite.sprite = readySprites[0];
        }
    }

    private void SetReadyVisibility(bool state)
    {
        readySprite.enabled = state;
    }

    [ClientRpc]
    public void Arise()
    {
        SetPlayerAliveStatus(false, true);
        SetReadyVisibility(false);
    }

    #region HookFunction

    /// <summary>
    /// Hook funtion to set a player's active state
    /// </summary>
    /// <remarks>This function is called to set the player's ability to move.</remarks>
    /// <param name="_old">[unused]</param>
    /// <param name="_new">new state</param>
    public void SetPlayerAliveStatus(bool _old, bool _new)
    {
        isActive = _new;
    }

    /// <summary>
    /// Hook function to set player's ready state
    /// </summary>
    /// <remarks>
    /// This function is used when a player is connected, but the game hasn't started yet.
    /// </remarks>
    /// <param name="_old">[unused]</param>
    /// <param name="_new">new state</param>
    private void SetReadyState(bool _old, bool _new)
    {
        isReady = _new;
    }

    private void SetPlayerName(string _old, string _new)
    {
        playerName = _new;
    }

    #endregion HookFunction

    private void OnValidate()
    {
        if (restitution < 0.01f) restitution = 0.01f;
        if (restitution > 1f) restitution = 1f;
        if (chargeMultiplier < 1) chargeMultiplier = 1;
    }
}
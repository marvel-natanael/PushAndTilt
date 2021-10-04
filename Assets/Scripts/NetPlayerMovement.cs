using Mirror;
using UnityEngine;

public class NetPlayerMovement : NetworkBehaviour
{
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

    [Header("Player speed parameters")]
    [SerializeField]
    private float speed;

    [SerializeField]
    private float speedMultiplier;

    [SerializeField]
    private float speedLimit;

    private float charRadius;

    private Rigidbody2D rb;
#if !UNITY_EDITOR
    private float dirX, dirY;
#endif

    private float CalculateMaxVelocity()
    {
        return rb.gravityScale * ((GameScreen.Corner_TopRight.y - GameScreen.Corner_BottomLeft.y) / 2);
    }

    private void Start()
    {
        charRadius = GetComponent<SpriteRenderer>().bounds.size.x / 2;
        jumpHeightIndicator = transform.GetChild(0).gameObject;
        rb = GetComponent<Rigidbody2D>();
        chargeLimit = CalculateMaxVelocity();
        isCharging = false;
    }

    private void CalculateChargeVisualization()
    {
        if (rb.gravityScale != 0)
        {
            chargeHeight = (charge * charge) / (2 * rb.gravityScale);
        }
        jumpHeightIndicator.transform.localScale = new Vector3(0.5f, chargeHeight, 1.0f);
    }

    private void FixedUpdate()
    {
        TouchHandler();
        HandleMovements();
    }

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
                    if (speed < speedLimit)
                    {
                        Debug.Log("D pressed!");
                        speed += Time.deltaTime * speedMultiplier;
                    }
                }
                if (Input.GetKey(KeyCode.A))
                {
                    if (speed > -speedLimit)
                    {
                        Debug.Log("A pressed!");
                        speed -= Time.deltaTime * speedMultiplier;
                    }
                }
            }
            else
            {
                speed = 0;
            }
            rb.velocity = new Vector2(rb.velocity.x + speed, rb.velocity.y);
            //X pos clamping
            if (transform.position.x + charRadius > GameScreen.Corner_TopRight.x)
            {
                rb.velocity = new Vector2(-Mathf.Abs(rb.velocity.x), rb.velocity.y);
            }
            if (transform.position.x - charRadius < GameScreen.Corner_BottomLeft.x)
            {
                rb.velocity = new Vector2(Mathf.Abs(rb.velocity.x), rb.velocity.y);
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
                    Debug.Log(isCharging);
                    if (charge <= chargeLimit)
                    {
                        charge += Time.fixedDeltaTime * chargeMultiplier * 10;
                        if (transform.localScale.y > 0.2f)
                        {
                            Debug.Log("Change");
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

    private void TouchHandler()
    {
        isCharging = Input.GetMouseButton(0);
    }

    private void OnValidate()
    {
        if (chargeMultiplier < 1)
        {
            chargeMultiplier = 1;
        }
    }
}
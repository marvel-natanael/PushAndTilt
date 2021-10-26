using Mirror;
using UnityEngine;

public class NetPlayerMovement : NetworkBehaviour
{
    private GameScreen screen;

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
    private float acceleration;

    [SerializeField]
    private float speedLimit;

    [SerializeField, Range(0.01f, 1f)]
    private float restitution;

    private float charRadius;

    private Rigidbody2D rb;
#if !UNITY_EDITOR
    private float dirX, dirY;
#endif

    private float CalculateMaxVelocity()
    {
        return rb.gravityScale * ((screen.Corner_TopRight.y - screen.Corner_BottomLeft.y) / 2);
    }

    private void Start()
    {
        screen = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameScreen>();
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

    private void TouchHandler()
    {
        isCharging = Input.GetMouseButton(0);
    }

    private void OnValidate()
    {
        if (restitution < 0.01f) restitution = 0.01f;
        if (restitution > 1f) restitution = 1f;
        if (chargeMultiplier < 1) chargeMultiplier = 1;
    }
}
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

    private Rigidbody2D rb;
#if !UNITY_EDITOR
    private float dirX, dirY;
#endif

    private float CalculateMaxVelocity()
    {
        return Mathf.Sqrt(2 * rb.gravityScale * (GameManager.ScreenPointOne.y - transform.position.y));
    }

    private void Start()
    {
        jumpHeightIndicator = transform.GetChild(0).gameObject;
        rb = GetComponent<Rigidbody2D>();
        //chargeLimit = CalculateMaxVelocity();
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
            if (Input.GetKey(KeyCode.D))
            {
                Debug.Log("D pressed!");
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
            if (Input.GetKey(KeyCode.A))
            {
                Debug.Log("A pressed!");
                rb.velocity = new Vector2(-speed, rb.velocity.y);
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
                        charge += Time.fixedDeltaTime* chargeMultiplier * 10;
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
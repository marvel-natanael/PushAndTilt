using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player jump charge parameters")]
    [SerializeField]
    private GameObject heightIndicator;

    [SerializeField]
    private float chargeLimit;

    [SerializeField]
    private float chargeMultiplier;

    [SerializeField]
    private float charge;

    [SerializeField]
    private bool isCharging;

    [SerializeField]
    private bool isGrounded;

    [Header("Player speed parameters")]
    [SerializeField]
    private float speed;

    private Rigidbody2D rb;
#if !UNITY_EDITOR
    private float dirX, dirY;
#endif

    //GLBB : endVelocity ^ 2 = startVelocity ^ 2 - 2 * gravity * (sceen height - player's pos)
    //0 = (maxSpeed)^2 - 2 * gravity * (screen height - player's pos)
    //2gh = max^2
    //sqrt(2gh) = max
    private float CalculateMaxVelocity()
    {
        return Mathf.Sqrt(2 * rb.gravityScale * (GameManager.ScreenPointOne.y - transform.position.y));
    }

    //private float CalculateMaxHeight()
    //{
    //    if (rb.gravityScale != 0)
    //    {
    //        return (charge * charge) / (2 * rb.gravityScale);
    //    }
    //    return 0f;
    //}   

    private void Start()
    {
        chargeLimit = CalculateMaxVelocity();
        rb = GetComponent<Rigidbody2D>();
        isCharging = false;
    }

    public void MovePlayerVertical(bool b)
    {
        isCharging = b;
    }

    private void FixedUpdate()
    {
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
        if (isGrounded)
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
                    //transform.Translate(Vector3.up * jump, Space.World);
                }
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + charge);
                transform.localScale = Vector3.one;
                charge = 0;
            }
        }
    }

    private void Update()
    {
        {
            var spriteHeight = GetComponent<SpriteRenderer>().bounds.extents.y;
            if (transform.position.y + spriteHeight > GameManager.ScreenPointOne.y)
            {
                transform.position = new Vector3(transform.position.x, GameManager.ScreenPointOne.y - spriteHeight, transform.position.z);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            Debug.Log("Hole");
        }
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Obst");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            Debug.Log("Hole through");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnValidate()
    {
        if (chargeMultiplier < 1)
        {
            chargeMultiplier = 1;
        }
    }
}
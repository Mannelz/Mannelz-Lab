using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField]
    private float speed;
    private float speedRun;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float dashForce;

    [Header("Controllers")]
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private float groundRay;

    //     

    private float horizontal;
    private float vertical;
    private float gravity;
    
    private float jumpTime;
    private float jumpTimer;
    private float dashTime;
    private float dashTimer;
    private float dashCooldown;
    private float dashingDuration;

    private bool canClimb;
    private bool canDash;
    private bool isJumping;
    private bool isClimbing;
    private bool isDashing;
    private bool doubleTapDash;

    private Rigidbody2D rb;

    void Awake()
    {
        speed = 5;
        speedRun = speed * 1.95f;

        jumpForce = 6.5f;
        jumpTime = 0.3f;

        dashForce = 25f;
        dashTime = 0.5f;
        dashCooldown = 1f;
        dashingDuration = 0.2f;

        isRight = true;
        canDash = true;

        groundRay = 1.01f;

        rb = GetComponent<Rigidbody2D>();

        gravity = rb.gravityScale;
    }

    void Update()
    {
        BrainController();

        Jump();
    }

    void FixedUpdate()
    {
        Mover();
        Climb();
        Dash();
    }

    void Mover()
    {
        horizontal = Input.GetAxis("Horizontal");
        
        if(!isDashing)
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                // Correr

                rb.velocity = new Vector2(horizontal * speedRun, rb.velocity.y);
            }
            else
            {
                // Andar

                rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            }
        }
    }

    void Jump()
    {
        if(jumpBufferTimer >= 0f && coyoteTimer > 0f)
        {
            jumpBufferTimer = 0f;
            jumpTimer = jumpTime;
            isJumping = true;
        
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if(Input.GetKey(KeyCode.Space) && isJumping)
        {
            if(jumpTimer > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                jumpTimer -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }

    void Climb()
    {
        vertical = Input.GetAxis("Vertical");

        if(!isClimbing)
        {
            rb.gravityScale = gravity;
        }

        if(canClimb && (vertical != 0f || !isGrounded))
        {
            isClimbing = true;
            rb.gravityScale = 0f;

            rb.velocity = new Vector2(rb.velocity.x, vertical * speed);
        }
        else
        {
            isClimbing = false;
        }
    }

    void Dash()
    {
        if(doubleTapDash)
        {
            dashTimer -= Time.deltaTime;

            if(dashTimer < 0f)
            {
                doubleTapDash = false;
                dashTimer = dashTime;
            }
        }

        if(Input.GetKeyDown(KeyCode.Q) && !doubleTapDash)
        {
            doubleTapDash = true;
        }
        
        if(Input.GetKeyDown(KeyCode.Q) && doubleTapDash && canDash)
        {
            StartCoroutine(Dashing());
        }
    }

    IEnumerator Dashing()
    {
        isDashing = true;
        canDash = false;

        if(isRight)
        {
            rb.AddForce(Vector2.right * dashForce, ForceMode2D.Impulse);
        }
        else if(isLeft)
        {
            rb.AddForce(Vector2.left * dashForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(dashingDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    #region Controllers

    private float coyoteTime = 0.2f;
    private float coyoteTimer = 0.2f;
    private float jumpBufferTime = 0.5f;
    private float jumpBufferTimer = 0.5f;

    private bool isGrounded;
    private bool isRight;
    private bool isLeft;

    void BrainController()
    {
        Checkers();
        Facing();
        CoyoteTime();
        JumpBuffer();
    }

    void Checkers()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundRay, groundLayer);
    }

    void Facing()
    {
        // Direita
        if(horizontal > 0f)
        {
            transform.eulerAngles = Vector2.zero;

            isRight = true;
            isLeft = false;
        }

        // Esquerda
        if(horizontal < 0f)
        {
            transform.eulerAngles = new Vector2(0f, 180f);

            isRight = false;
            isLeft = true;
        }
    }

    void CoyoteTime()
    {
        if(isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    void JumpBuffer()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Scalable"))
        {
            canClimb = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Scalable"))
        {
            canClimb = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * groundRay);
    }

    #endregion
}
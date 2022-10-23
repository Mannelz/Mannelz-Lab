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
    private Vector2 velocidadeAtual;
    
    private float timerDash;
    private float dashCooldown;
    private float dashingDuration;

    private bool canClimb;
    private bool canDash;
    private bool isClimbing;
    private bool isDashing;
    private bool doubleTapDash;

    private Rigidbody2D rb;

    void Awake()
    {
        speed = 5;
        speedRun = speed * 1.95f;

        jumpForce = 6.5f;

        dashForce = 25f;
        timerDash = 0.5f;
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

        #region Mover com velocity
        
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

        #endregion

        #region Mover com force

        /*
        velocidadeAtual = rb.velocity;

        // Desacelerar
        if(horizontal == 0f)
        {
            rb.velocity = new Vector2(Mathf.Lerp(velocidadeAtual.x, 0f, 1f), rb.velocity.y);
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            // Correr

            if(Mathf.Abs(velocidadeAtual.x) <= speedRun)
            {
                rb.AddForce(new Vector2(horizontal * speedRun, rb.velocity.y), ForceMode2D.Force);
            }
            else
            {
                rb.velocity = new Vector2(horizontal * speedRun, rb.velocity.y);
            }
        }
        else
        {
            // Andar

            if(Mathf.Abs(velocidadeAtual.x) <= speed)
            {
                rb.AddForce(new Vector2(horizontal * speed, rb.velocity.y), ForceMode2D.Force);
            }
            else
            {
                rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            }
        }
        */

        #endregion
    }

    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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
            timerDash -= Time.deltaTime;

            if(timerDash <= 0f)
            {
                doubleTapDash = false;
                timerDash = 0.5f;
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

    private bool isGrounded;
    private bool isRight;
    private bool isLeft;

    void BrainController()
    {
        Checkers();
        Facing();
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

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Ledders"))
        {
            canClimb = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Ledders"))
        {
            canClimb = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector2.down * groundRay);
    }

    #endregion
}
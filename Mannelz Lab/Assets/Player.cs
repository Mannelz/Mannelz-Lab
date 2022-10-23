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

    [Header("Controllers")]
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private float groundRay;

    //     

    private float horizontal;
    private Vector2 velocidadeAtual;
    
    private bool isGrounded;

    private Rigidbody2D rb;

    void Awake()
    {
        speed = 5;
        speedRun = speed * 1.95f;

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Checkers();
        
        Jump();
    }

    void FixedUpdate()
    {
        Mover();
    }

    void Mover()
    {
        horizontal = Input.GetAxis("Horizontal");

        #region Mover com velocity
        
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

    #region Controllers

    void Checkers()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundRay, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector2.down * groundRay);
    }

    #endregion
}
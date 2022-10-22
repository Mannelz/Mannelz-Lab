using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private float speedRun;
    private float horizontal;

    private Rigidbody2D rb;

    void Start()
    {
        speed = 5;
        speedRun = speed * 1.95f;

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Mover();
    }

    void Mover()
    {
        horizontal = Input.GetAxis("Horizontal");

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
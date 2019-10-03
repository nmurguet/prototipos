using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed;
    public float jumpForce;
    private float moveInput;

    private bool facingRight = true;


    public Animator animator;

    public Joystick stick;

    public Rigidbody2D rb;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    private int extraJumps;
    public int extraJumpsValue;
    public bool jumpPressed;

    public bool isJump;

    public bool keyboard = false;

    public Manager levelmanager; 



    // Start is called before the first frame update
    void Start()
    {
        extraJumps = extraJumpsValue;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isJump = false;

    }


    // Update is called once per frame
    void Update()
    {
        if (keyboard)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            if (stick.Horizontal >= .4f)
            {
                moveInput = 1;

            }
            else if (stick.Horizontal <= -.4f)
            {

                moveInput = -1;
            }
            else
            {

                moveInput = 0f;
            }
        }

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("isJumping", !isGrounded);



        if (Input.GetKeyDown(KeyCode.W))
        {
            buttonJumpDown();




        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            buttonJumpUp();





        }





        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
            
            

        }

    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        rb.velocity = new Vector2(moveInput * speed * Time.deltaTime, rb.velocity.y);
        if (!facingRight && moveInput > 0)
        {
            Flip();

        }
        else if (facingRight && moveInput < 0)
        {

            Flip();
        }

        if(isJump)
        {
            rb.velocity = Vector2.up * jumpForce * Time.deltaTime;

        }



    }


    public void buttonJumpDown()
    {
        jumpPressed = true;
        if (extraJumps > 0)
        {
            rb.velocity = Vector2.up * jumpForce * Time.deltaTime;
            extraJumps--;
        }
        else if (extraJumps == 0 && isGrounded)
        {

            rb.velocity = Vector2.up * jumpForce * Time.deltaTime;
        }

    }
    public void buttonJumpUp()
    {
        jumpPressed = false;

    }

    void Flip()
    {

        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Respawn")
        {
            
         levelmanager.Respawn();
        }
    }

    public void Respawn()
    {
        rb.velocity = new Vector2(0, 0);
        animator.SetTrigger("respawn");
        

    }



}

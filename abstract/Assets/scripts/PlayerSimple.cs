using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSimple : MonoBehaviour {

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

    public bool keyboard = false;

// DASH
    public float dashSpeed;
    private float dashTime;
    public float startDashTime;
    public bool canDash; 


    // Use this for initialization
    void Start () {
        extraJumps = extraJumpsValue;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        dashTime = startDashTime;
        canDash = true; 

		
	}

	public void buttonJumpDown()
	{
		jumpPressed = true; 
		if (extraJumps > 0) {
			rb.velocity = Vector2.up * jumpForce;
			extraJumps--;
		} else if (extraJumps == 0 && isGrounded) {

			rb.velocity = Vector2.up * jumpForce;
		}

	}
	public void buttonJumpUp()
	{
		jumpPressed = false; 

	}
	
	// Update is called once per frame
	void Update () {

        if (keyboard) { 
        moveInput = Input.GetAxisRaw("Horizontal");
        }else { 
        if (stick.Horizontal >= .4f) {
			moveInput = 1;

		} else if (stick.Horizontal <= -.4f) {

			moveInput = -1;
		} else {

			moveInput = 0f;
		}
        }



        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("isJumping", !isGrounded);

		if (Input.GetKeyDown(KeyCode.W)&& extraJumps > 0)
        {
            rb.velocity = Vector2.up * jumpForce;
            extraJumps--;
           

        }else if(Input.GetKeyDown(KeyCode.W) && extraJumps==0 && isGrounded)
        {
            rb.velocity = Vector2.up * jumpForce;


        }

        if(isGrounded)
        {
            extraJumps = extraJumpsValue;


        }
        

        //Codigo del dash
        if (canDash && Input.GetKeyDown(KeyCode.E))
        {
            
            canDash = false;
            
                

        }
        if(!canDash)
        {
            if (dashTime <= 0) // tiene que terminar el dash
            {
                canDash = true;
                dashTime = startDashTime;
                rb.velocity = Vector2.zero;
            }
            else
            {
                dashTime -= Time.deltaTime;
                if (facingRight)
                {
                    rb.velocity = new Vector2(1f * dashSpeed, 0f);

                }
                else
                {
                    rb.velocity = Vector2.left * dashSpeed;
                }
            }

            // fin del codigo del dash



        }


    }


   void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);





        if (canDash) { 
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }
        if (!facingRight && moveInput > 0)
        {
            Flip();

        }
        else if (facingRight && moveInput < 0)
        {

            Flip();
        }

    }


    void Flip()
    {

        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);

    }
}

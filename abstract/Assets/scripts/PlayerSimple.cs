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


    // Use this for initialization
    void Start () {
        extraJumps = extraJumpsValue;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();


		
	}
	
	// Update is called once per frame
	void Update () {
        moveInput = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("isJumping", !isGrounded);

        if (Input.GetKeyDown(KeyCode.W) && extraJumps > 0)
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

    }


   void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

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

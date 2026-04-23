using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour
{

    //seteo de keys
    public KeyCode left;
    public KeyCode right;
    public KeyCode up;
    public KeyCode down;
    public KeyCode action;
    public KeyCode jump;



    //movement bool
    private bool pressleft = false;
    private bool pressright = false;
    private bool pressup = false;
    private bool pressdown = false;
    private bool pressjump = false;

    public float speed;
    public float jumpForce;

    private bool facingRight = true;

    public Animator animator;


    public Rigidbody2D rb;


    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;


    private int extraJumps;
    public int extraJumpsValue;
    public bool jumpPressed;

    public Transform wallCheck;
    public bool isWall;
    public bool isJump;

    private float moveInput;

    public float max_Speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isJump = false;
    }

    // Update is called once per frame
    void Update()
    {

        animator.SetFloat("xSpeed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("isJumping", !isGrounded);
        animator.SetFloat("ySpeed", Mathf.Abs(rb.velocity.y));

        moveInput = Input.GetAxisRaw("Horizontal");
        GetMovement();
        if (!facingRight && moveInput > 0
            )
        {
            Flip();

        }
        else if (facingRight && moveInput < 0)
        {

            Flip();
        }
        
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        isWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, whatIsGround);

        Movement();
        
    }


    void Flip()
    {

        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);

    }

    void GetMovement()
    {
        if (Input.GetKeyDown(left))
        {
            pressleft = true;
        }
        if (Input.GetKeyUp(left))
        {
            pressleft = false;
        }
        if (Input.GetKeyDown(right))
        {
            pressright = true;
        }

        if (Input.GetKeyUp(right))
        {
            pressright = false;
        }

        if (Input.GetKeyDown(up))
        {
            pressup = true;
        }

        if (Input.GetKeyUp(up))
        {
            pressup = false;
        }
        if (Input.GetKeyDown(down))
        {
            pressdown = true;
        }

        if (Input.GetKeyUp(down))
        {
            pressdown = false;
        }

        if (Input.GetKeyDown(jump))
        {
            pressjump = true;
        }

        if (Input.GetKeyUp(jump))
        {
            pressjump = false;
        }

 
    }

    void Movement()
    {
        //rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        
        //rb.velocity = Vector2.ClampMagnitude(rb.velocity, max_Speed);
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        /*
        if (pressleft)
        {
             moveInput = -1;
            rb.AddRelativeForce(Vector2.right * -speed);
        }
        if(pressright)
        {
            moveInput = 1;
            rb.AddRelativeForce(Vector2.right * speed);
        }
        */

        if(pressjump && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x,  jumpForce);
        }
    }

}

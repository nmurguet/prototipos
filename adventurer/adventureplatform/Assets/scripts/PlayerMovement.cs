using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	//Variables
	public float speed;
	public float jumpForce;
	private float moveInput;

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
	private bool doublejump;


	// DASH
	public float dashSpeed;
	private float dashTime;
	public float startDashTime;
	public bool canDash; 

    // Start is called before the first frame update
    void Start()
    {
		extraJumps = extraJumpsValue;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		dashTime = startDashTime;
		canDash = true; 
		doublejump = false;
        
        
    }

    // Update is called once per frame
    void Update()
    {
		moveInput = Input.GetAxisRaw("Horizontal");
        

        if(Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetTrigger("attack2");


        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            animator.ResetTrigger("attack2");


        }


        if (Input.GetKeyDown(KeyCode.W)&& extraJumps > 0)
		{

			rb.velocity = new Vector2 (rb.velocity.x, jumpForce);
			extraJumps--;
			doublejump = true;


		}else if(Input.GetKeyDown(KeyCode.W) && extraJumps==0 && isGrounded)
		{
			rb.velocity = new Vector2 (rb.velocity.x, jumpForce);


		}

		if(isGrounded)
		{
			extraJumps = extraJumpsValue;
			doublejump=false;

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
		//manejo de la animacion

	

		animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
		animator.SetBool("isJumping", !isGrounded);
		animator.SetBool("doublejump",doublejump);

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

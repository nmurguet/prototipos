using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {

    public Rigidbody2D rb;
    public float speed;
    public float jumpForce;


    //ground check stuff
    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    private Animator animator;


    private int extraJumps;
    public int extraJumpsValue;
    public bool jumpPressed;
    private bool doublejump;





    // Use this for initialization
    void Start () {
        extraJumps = extraJumpsValue;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        doublejump = false;
    }
	
	// Update is called once per frame
	void Update () {
        rb.velocity = new Vector2(1f * speed, rb.velocity.y);


        if (Input.GetKeyDown(KeyCode.Space) && extraJumps > 0)
        {

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            extraJumps--;
            doublejump = true;


        }
        else if (Input.GetKeyDown(KeyCode.W) && extraJumps == 0 && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);


        }

        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
            doublejump = false;

        }






    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        animator.SetBool("isJumping", isGrounded);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
         //gameObject.transform.position = new Vector3(-4, 1, 0);
        

    }
}

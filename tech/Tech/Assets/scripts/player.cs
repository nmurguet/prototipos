using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public Rigidbody2D rb;

    public float jumpingspeed;
    public float movespeed;

    public bool jumping;
    public bool moveleft;
    public bool moveright;

    public bool facingRight; 

    public bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    public Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 

        jumping = false; 

    }

    // Update is called once per frame
    void Update()
    {

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        if (Input.GetKeyDown(KeyCode.W))
        {
            jumping = true;
            

        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            jumping = false;


        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            moveright = true;


        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            moveright = false;


        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            moveleft = true;


        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            moveleft = false;


        }


    }

    private void FixedUpdate()
    {
        if (jumping && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpingspeed * 1000 * Time.deltaTime);
            
        }

        if(moveleft)
        {
            rb.AddForce(Vector2.left * movespeed * Time.deltaTime);


        }

        if (moveright)
        {
            rb.AddForce(Vector2.left * -movespeed * Time.deltaTime);


        }


        if (!facingRight && rb.velocity.x < 0)
        {
            Flip();

        }
        else if (facingRight && rb.velocity.x > 0)
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

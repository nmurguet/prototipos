using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Rigidbody2D rb;



    public KeyCode left;
    public KeyCode right;
    public KeyCode up;
    public KeyCode slowUp;
    public KeyCode action;
    public KeyCode land;
    public KeyCode strafeLeft;
    public KeyCode strafeRight; 

    public float speed;

    public GameObject dome;
    public float rotateDome;
    public Transform groundCheckPoint;
    public float groundCheckRadius;
    public LayerMask whatIsGround; 


    public float torque;
    public float thrust;
    public float strafeThrust;

    public float slowDampener;

    //movement bool
    private bool pressleft = false;
    private bool pressright = false;
    private bool pressup = false;
    private bool pressslowUp = false;

    private bool pressstrafeleft = false;
    private bool pressstraferight = false;

    private bool isGrounded;

    


    public bool exitShip; 
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        


    }

    // Update is called once per frame
    void Update()
    {
        GetMovement();
          
   

        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);
        if(isGrounded)
        {
           // Dome(); 
        }
    }

    private void FixedUpdate()
    {
        Movement(); 
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
        if (Input.GetKeyDown(slowUp))
        {
            pressslowUp = true;
        }

        if (Input.GetKeyUp(slowUp))
        {
            pressslowUp = false;
        }

        if (Input.GetKeyDown(strafeLeft))
        {
            pressstrafeleft = true;
        }

        if (Input.GetKeyUp(strafeLeft))
        {
            pressstrafeleft = false;
        }

        if (Input.GetKeyDown(strafeRight))
        {
            pressstraferight = true;
        }

        if (Input.GetKeyUp(strafeRight))
        {
            pressstraferight = false;
        }
    }


    void Movement()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, 18f);

        if (pressleft)
        {
            rb.AddTorque(0.3f * torque);
        }
        if (pressright)
        {
            rb.AddTorque(-0.3f * torque);
        }

        if (pressstrafeleft)
        {
            rb.AddRelativeForce(Vector2.right * -strafeThrust);
        }
        if (pressstraferight)
        {
            rb.AddRelativeForce(Vector2.right * strafeThrust);
        }

        if (pressup)
        {
            rb.AddRelativeForce(Vector2.up * thrust);
            
        }
        if (pressslowUp)
        {
            rb.AddRelativeForce((Vector2.up * thrust / slowDampener));
        }
    }

    void Dome()
    {

        if (Input.GetKeyDown(KeyCode.C))
        {
            exitShip = true;
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            exitShip = false;
        }


        if (exitShip)
        {
            dome.transform.rotation = Quaternion.Slerp(dome.transform.rotation, Quaternion.Euler(0f, 0f, 70f), rotateDome * Time.deltaTime);

        }
        else
        {
            dome.transform.rotation = Quaternion.Slerp(dome.transform.rotation, Quaternion.Euler(0f, 0f, 0f), rotateDome * Time.deltaTime);
        }


    }

}



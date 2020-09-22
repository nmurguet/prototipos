using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class car : MonoBehaviour
{
    public Rigidbody2D rb;

    public float velocidad;


    public Vector2 pos; 


    public WheelJoint2D front;
    public WheelJoint2D back;

    public float max_speed; 


    public Transform groundCheckPoint;
    public bool isGrounded;
    public float groundCheckRadius;
    public LayerMask whatIsGround;


    public ParticleSystem dust;


    float _speed; 

    public bool accel; 
    // Start is called before the first frame update
    void Start()
    {
        accel = false;
        front.useMotor = false;
        back.useMotor = false; 
    }

    // Update is called once per frame
    void Update()
    {
        pos.x = transform.position.x;
        pos.y = transform.position.y; 



    }


    private void FixedUpdate()
    {
        Movement();

    }

    void CreateDust()
    {
        dust.Play(); 


    }


    public float Speed
    {
        get
        {
            return _speed; 

        }
    }


    public void Movement()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, max_speed);
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);
        _speed = rb.velocity.magnitude / 1000;

        if (isGrounded)
        {
            if (accel)
            {
                rb.AddRelativeForce(Vector2.left * velocidad * Time.deltaTime * -1);
                CreateDust();

            }


        }


    }


    public void PressGas()
    {
        accel = true;

    }

    public void LiftGas()
    {
        accel = false; 

    }


    public void Reset()
    {
        transform.position = new Vector3(pos.x, pos.y + 3f + transform.position.z);

        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
        rb.velocity = Vector3.zero;

    }
}

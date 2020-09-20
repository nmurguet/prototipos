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


        if (Input.GetKeyUp(KeyCode.R))
        {
            Scene scene = SceneManager.GetActiveScene();

            SceneManager.LoadScene(scene.name);

        }



        if (Input.GetKeyUp(KeyCode.Q))
        {
            transform.position = new Vector3(pos.x, pos.y + 5f + transform.position.z);

            Vector3 eulerRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);

        }



        if (Input.GetKeyDown(KeyCode.A))
        {
            accel = true;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            accel = false;
        }
    }


    private void FixedUpdate()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, max_speed);
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);

        if (isGrounded) { 
            if (accel)
                {
                rb.AddRelativeForce(Vector2.left * velocidad * Time.deltaTime * -1);
           // front.useMotor = true;
           // back.useMotor = true;
                }

            else
                {
                    front.useMotor = false;
                    back.useMotor = false;

                }
                    }

    }
}

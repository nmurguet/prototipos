using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ufo : MonoBehaviour
{
    public GameController controller;


    public KeyCode left;
    public KeyCode right;
    public KeyCode up;
    public KeyCode slowUp;
    public KeyCode action;
    public KeyCode land;

    public float speed;


    public bool exitShip;
    public GameObject dome;
    public float rotateDome;

    public float getSpeed;


    public Rigidbody2D rb;

    public float torque;
    public float thrust;
    public float strafeThrust;

    public Transform landingGear;
    public Vector3 startPos;
    public Vector3 endPos;
    public Transform start;
    public Transform end;

    public bool retracted;
    public bool canRetract;

    public Vector3 landing;

    public Transform groundCheckPoint;
    public bool isGrounded;
    public float groundCheckRadius;
    public LayerMask whatIsGround;

    public float slowDampener; 
    public bool canFly;

    public ParticleSystem mainParticle;
    public ParticleSystem aftermainParticle;
    public ParticleSystem slowParticle;
    public ParticleSystem afterslowParticle;
    public ParticleSystem leftParticle;
    public ParticleSystem rightParticle;



    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<GameController>();
        rb = GetComponent<Rigidbody2D>();
        startPos = start.transform.position;
        endPos = end.transform.position;
        retracted = true;
        landingGear.position = endPos;
        canFly = true;
        getSpeed = 0;



    }

    // Update is called once per frame
    void Update()
    {
        getSpeed = rb.velocity.magnitude;
        ParticleManager();

        if (canFly)
        {
            Movement();
        }


        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);
        if (isGrounded && rb.velocity.magnitude < 0.1f)
        {
            Dome();
            if (Input.GetKeyDown(action))
            {
                exitShip = true;
                StartCoroutine(waitandExit(1f));
            }
        }
        landing = landingGear.position;

    }

    void Movement()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, 18f);

        if (Input.GetKey(left))
        {
            rb.AddTorque(0.3f * torque);
        }
        if (Input.GetKey(right))
        {
            rb.AddTorque(-0.3f * torque);
        }

        if (Input.GetKey(up))
        {
            rb.AddRelativeForce(Vector2.up * thrust);
        }
        if (Input.GetKey(slowUp))
        {
            rb.AddRelativeForce(Vector2.up * thrust / slowDampener);
        }

        if (Input.GetKeyDown(land))
        {
            canRetract = true;
        }

        LandingGear();
    }

    void LandingGear()
    {
        if (canRetract)
        {

            float step = speed * Time.deltaTime;
            startPos = start.transform.position;
            endPos = end.transform.position;
            if (!retracted)
            {
                landingGear.position = Vector3.MoveTowards(landingGear.position, endPos, step);
                if (landing == endPos)
                {
                    canRetract = false;
                    retracted = true;

                }
            }
            else
            {
                landingGear.position = Vector3.MoveTowards(landingGear.position, startPos, step);
                if (landing == startPos)
                {
                    canRetract = false;
                    retracted = false;
                }


            }
        }

    }



    IEnumerator waitandExit(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        exitShip = false;
        canFly = false;
        //controller.ExitShip();


    }


    void Dome()
    {
        if (exitShip)
        {
            dome.transform.rotation = Quaternion.Slerp(dome.transform.rotation, Quaternion.Euler(0f, 0f, 70f), rotateDome * Time.deltaTime);

        }
        else
        {
            dome.transform.rotation = Quaternion.Slerp(dome.transform.rotation, Quaternion.Euler(0f, 0f, 0f), rotateDome * Time.deltaTime);
        }


    }


    void ParticleManager()
    {
        if (Input.GetKey(up))
        {
            mainParticle.Play();
            aftermainParticle.Play();
        }
        else
        {
            mainParticle.Stop();
            aftermainParticle.Stop();
        }


        if (Input.GetKey(slowUp))
        {
            slowParticle.Play();
            afterslowParticle.Play();
        }
        else
        {
            slowParticle.Stop();
            afterslowParticle.Stop();
        }

        if (Input.GetKey(left))
        {
            leftParticle.Play();
        }
        else
        {
            leftParticle.Stop();
        }

        if (Input.GetKey(right))
        {
            rightParticle.Play();
        }
        else
        {
            rightParticle.Stop();
        }



    }
}

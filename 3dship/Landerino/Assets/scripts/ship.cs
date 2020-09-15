using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ship : MonoBehaviour
{

    public Rigidbody rb;

    public Vector3 direction;

    public Vector3 forwardThrustDirection;

    public float push;


    


    public Transform t1, t2;

    public Transform f1, f2; 

    public float thrustForce;
    public float torqueforce;


    public string level;


    public bool goUp, turnLeft, turnRight, rotLeft, rotRight, pitchForward, pitchBackwards;

    public bool engineOn;
    public float thrust;
    public float iThrust;

    // Start is called before the first frame update
    void Start()
    {
        goUp = false;
        turnLeft = false;
        turnRight = false;
        rotLeft = false;
        rotRight = false;
        pitchBackwards = false;
        pitchForward = false;
        engineOn = false; 
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();


        direction = t1.position - t2.position;
        forwardThrustDirection = f1.position - f2.position;

        push = rb.velocity.x + iThrust; 


    }

    private void FixedUpdate()
    {
        Movement();
    }


    void Movement()
    {
        if (rotRight)
        {
            rb.AddRelativeTorque(Vector3.forward * torqueforce * Time.deltaTime);
            
        }
        if (rotLeft)
        {
            rb.AddRelativeTorque(Vector3.forward * -torqueforce * Time.deltaTime);

        }
        if (pitchForward)
        {
            rb.AddRelativeTorque(Vector3.left * torqueforce * Time.deltaTime);
        }
        if (pitchBackwards)
        {
            rb.AddRelativeTorque(Vector3.left * -torqueforce * Time.deltaTime);

        }
        if(turnLeft)
        {
            rb.AddRelativeTorque(Vector3.up * torqueforce * Time.deltaTime);
        }
        if(turnRight)
        {
            rb.AddRelativeTorque(Vector3.up * -torqueforce * Time.deltaTime);
        }
        if(goUp)
        {
            rb.AddForce(direction * thrustForce * Time.deltaTime);

        }

        if(engineOn)
        {
            iThrust = thrust;
      
        }
        if(!engineOn)
        {
            iThrust = 0;
        }


        rb.AddForce(forwardThrustDirection * push * Time.deltaTime);



    }

    void GetInputs()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            level = "level";
            SceneManager.LoadScene(level);

        }



        if(Input.GetKeyDown(KeyCode.Z))
        {
            engineOn = true; 
        }
        if(Input.GetKeyUp(KeyCode.Z))
        {
            engineOn = false; 
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            goUp = true;

        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            goUp = false;

        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            rotLeft = true;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            rotLeft = false;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            rotRight = true;

        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            rotRight = false;

        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            pitchForward = true;

        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            pitchForward = false;

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            pitchBackwards = true;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            pitchBackwards = false;
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            turnLeft = true;

        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            turnLeft = false;

        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            turnRight = true;

        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            turnRight = false;

        }

    }
}

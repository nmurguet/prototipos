using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{


    public GameObject left_rocket;

    public GameObject right_rocket;

    private Rigidbody2D rb;

    public float thrust;

    //groundeck stuff

    public Transform groundCheckPoint;
    public bool isGrounded;
    public float groundCheckRadius;
    public LayerMask whatIsGround;



    //emitters
    public ParticleSystem left_emitter;
    public ParticleSystem right_emitter;
    public GameObject leftThrust;
    public GameObject rightThrust;




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        leftThrust.SetActive(false);
        rightThrust.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            Scene scene = SceneManager.GetActiveScene();

            SceneManager.LoadScene(scene.name);

        }

        Movement();



    }


    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);

    }


    void Movement()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForceAtPosition(left_rocket.transform.up * thrust, left_rocket.transform.position);
            left_emitter.Play();
            leftThrust.SetActive(true);


        }

        if (Input.GetKey(KeyCode.L))
        {
            rb.AddForceAtPosition(right_rocket.transform.up * thrust, right_rocket.transform.position);
            right_emitter.Play();
            rightThrust.SetActive(true);
        }


        if (Input.GetKeyUp(KeyCode.A))
        {
            left_emitter.Stop();
            leftThrust.SetActive(false);
            


        }

        if (Input.GetKeyUp(KeyCode.L))
        {

            right_emitter.Stop();
            rightThrust.SetActive(false);

        }
    }
    }

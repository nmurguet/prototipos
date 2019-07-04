using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;

    public float torque;
    public float thrust;

    public ParticleSystem emitter;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();


    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        
    }

    void Movement()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, 18f);

        if (Input.GetKey(KeyCode.A))
        {
            rb.AddTorque(0.3f * torque*Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddTorque(-0.3f * torque * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(Vector2.up * thrust*Time.deltaTime);
            emitter.Play();
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeForce(Vector2.up * (thrust / 1.85f)*Time.deltaTime);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            emitter.Stop();
        }
        

  
    }
}

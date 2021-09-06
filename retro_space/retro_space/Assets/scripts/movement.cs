using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{

    Rigidbody2D rb;

    public float torque;
    public float thrust;
    public float max_speed; 
    public bool gas;
    public bool left_gas;
    public bool right_gas;

    public GameObject light; 




    // Start is called before the first frame update
    void Start()
    {
        gas = false;
        left_gas = false;
        right_gas = false;
        rb = GetComponent<Rigidbody2D>();
        light.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInputs();
        light.SetActive(gas); 
    }
    private void FixedUpdate()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, max_speed);
        PlayerMovement();
    }


    void PlayerInputs()
    {
       

        if (Input.GetKey(KeyCode.A))
        {
            left_gas = true;
            
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            left_gas = false;
            
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            right_gas = true;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            right_gas = false;
        }


        if (Input.GetKeyDown(KeyCode.W))
        {
            gas = true;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            gas = false;
        }
                     
    }

    void PlayerMovement()
    {

        if (left_gas)
        {

            rb.AddTorque(0.3f * torque * Time.deltaTime);
        }
        if(right_gas)

        {
            rb.AddTorque(-0.3f * torque * Time.deltaTime);
        }



        if(gas)
        {
            rb.AddRelativeForce(Vector2.up * thrust * Time.deltaTime);
            

        }
    }
}

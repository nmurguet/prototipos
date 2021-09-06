using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    bool left_gas;
    bool right_gas;

    bool gas;
    bool back_gas;

    Rigidbody2D rb;

    public float thrust;


    public float width;
    public float height;

    public BulletManager bm; 


    //Rotation
    public float xVelocity;

    Quaternion startingRotation = Quaternion.Euler(0, 0, 0);
    

    public float tiltSmooth = 2;
    Quaternion forwardRotation = Quaternion.Euler(0, 0, 25);
    Quaternion rightRotation = Quaternion.Euler(0, 0, -25);

    //End Rotation


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bm = GetComponent<BulletManager>();
        left_gas = false;
        right_gas = false;
        gas = false;
        back_gas = false;
         
        width = (Mathf.Abs(Camera.main.transform.position.x - Camera.main.orthographicSize * Screen.width / Screen.height))-2;
        height = (Mathf.Abs(Camera.main.transform.position.y - Camera.main.orthographicSize * Screen.height / Screen.width))/ 2;


        

    }

    void FixedUpdate()
    {
        PlayerMovement();

        xVelocity = rb.velocity.x;

    }


    // Update is called once per frame
    void Update()
    {
        Boundaries();
        PlayerInputs();
        
        
    }


    void PlayerMovement()
    {
        if (gas)
        {
            rb.AddRelativeForce(Vector2.up * thrust * Time.deltaTime);
        }

        if (back_gas)
        {
            rb.AddRelativeForce(Vector2.up * -thrust * Time.deltaTime);


        }

        if (left_gas)
        {
            rb.AddRelativeForce(Vector2.left * thrust * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, forwardRotation, tiltSmooth * Time.deltaTime);
        }

        if (right_gas)
        {
            rb.AddRelativeForce(Vector2.left * -thrust * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, rightRotation, tiltSmooth * Time.deltaTime);

        }
        transform.rotation = Quaternion.Lerp(transform.rotation, startingRotation, tiltSmooth * Time.deltaTime);

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

        if (Input.GetKeyDown(KeyCode.S))
        {
            back_gas = true;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            back_gas = false;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            bm.Shoot();
        }

    }

    void Boundaries()
    {
        float _xMovementclamp = Mathf.Clamp(transform.position.x, width*-1, width);
        float _yMovementclamp = Mathf.Clamp(transform.position.y, height * -1, height);

        transform.position = new Vector3(_xMovementclamp, _yMovementclamp, transform.position.z);

        


    }
}

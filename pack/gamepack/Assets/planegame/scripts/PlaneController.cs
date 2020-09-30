using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public Rigidbody2D rb;

    public bool press;
    public bool canPlay;

    private float gravity;

    public float thrust;
    public float max_Speed;

    public Vector3 wrld;
    Quaternion downRotation;
    Quaternion forwardRotation;
    public float tiltSmooth = 5;

    public float minRotation = -145;
    public float maxRotation = 145;


    // Start is called before the first frame update
    void Start()
    {
        rb.GetComponent<Rigidbody2D>();
        canPlay = false;
        gravity = rb.gravityScale;
        rb.gravityScale = 0;
        wrld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0.0f));
        downRotation = Quaternion.Euler(0, 0, -100);
        forwardRotation = Quaternion.Euler(0, 0, 40);


    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y > wrld.y)
        {
            transform.position = new Vector3(transform.position.x, wrld.y, transform.position.z);

            rb.velocity = new Vector2(0f, 0f);

        }
        if (transform.position.y < -wrld.y)
        {
            transform.position = new Vector3(transform.position.x, -wrld.y, transform.position.z);

            rb.velocity = new Vector2(0f, 0f);
        }





        if((rb.velocity.y > 1f) && ((transform.rotation.eulerAngles.z < 50)||transform.rotation.eulerAngles.z >305))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, forwardRotation, tiltSmooth * Time.deltaTime);
        }
        if ((rb.velocity.y < -0.4f) && ((transform.rotation.eulerAngles.z < 55) || transform.rotation.eulerAngles.z > 310))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
        }


    }
    

    private void FixedUpdate()
    {
        Movement();
    }


    public void PressButton()
    {
        if (canPlay)
        {
            press = true;
                       
        }
    }

    public void ReleaseButton()
    {
        if (canPlay)
        {
            press = false;
            
        }

    }

    public void StartGame()
    {
        canPlay = true;
        rb.gravityScale = gravity;

    }

    void Movement()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, max_Speed);
        if (press && canPlay)
        {
            rb.AddForce(Vector2.up * thrust * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, forwardRotation, tiltSmooth * Time.deltaTime);

        }
    }


    public void StopGame()
    {
        canPlay = false;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(0, 0);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLander : MonoBehaviour
{

    public bool press;
    private Rigidbody2D rb;

    public float thrust;
    public float max_Speed;

    public Camera cam;

    public float t = 1.0f;

    public float howmuch;

    public float zValue;
    public float NextzValue;


    private GameObject fire;

    public ParticleSystem smoke; 


    private float gravity = 0.2f;

    public bool crashed;
    private float velocity;

    // Start is called before the first frame update
    void Start()
    {
        press = false;
        rb = GetComponent<Rigidbody2D>();
        fire = GameObject.FindGameObjectWithTag("fire"); 
        zValue = cam.orthographicSize;
        fire.SetActive(false);
        rb.gravityScale = 0;
        crashed = false; 
           
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y);
        zValue = cam.orthographicSize;

        NextzValue = transform.position.y * howmuch; 
        cam.orthographicSize = Mathf.Lerp(zValue,NextzValue, t);

        if(cam.orthographicSize < 5)
        {
            cam.orthographicSize = 5f; 

        }

        if (cam.orthographicSize > 30)
        {
            cam.orthographicSize = 30f;

        }
    }


    private void FixedUpdate()
    {
        Movement(); 
    }


    void Movement()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, max_Speed);
        if (press)
        {
            rb.AddRelativeForce(Vector2.up * thrust * Time.deltaTime);
        }
    }


    public void PressButton()
    {
        press = true;
        fire.SetActive(true);
        smoke.Play(); 
    }

    public void ReleaseButton()
    {
        press = false;
        fire.SetActive(false);
        smoke.Stop(); 


    }


    public void StartGame()
    {
        rb.gravityScale = gravity; 

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (velocity > 4f)
        {

            crashed = true;
        }
        
    }

    public void ResetCrashed()
    {
        crashed = false; 
    }
}

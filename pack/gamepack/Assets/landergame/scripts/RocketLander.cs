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

    public float maxSpeedDamage; 

    public AudioSource source;
    

    private GameObject fire;

    public ParticleSystem smoke; 


    private float gravity = 0.2f;

    public bool crashed;
    private float velocity;
    private bool _touched;

    public float fuel = 100f; 

    private bool canPlay;

    public FuelBar fuelBar; 

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
        _touched = false;
        canPlay = false;
        fuelBar.SetMaxFuel(fuel);
        


}

    // Update is called once per frame
    void Update()
    {
        if(fuel<0)
        {
            canPlay = false;
            smoke.Stop();
            source.Stop(); fire.SetActive(false);
        }
        velocity = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y);
        zValue = cam.orthographicSize;

        NextzValue = transform.position.y * howmuch; 
        cam.orthographicSize = Mathf.Lerp(zValue,NextzValue, t);

        if(cam.orthographicSize < 5)
        {
            cam.orthographicSize = 5f; 

        }

        if (cam.orthographicSize > 23)
        {
            cam.orthographicSize = 23f;

        }
    }


    private void FixedUpdate()
    {
        Movement(); 
    }


    void Movement()
    {
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, max_Speed);
        if (press&&canPlay)
        {
            rb.AddRelativeForce(Vector2.up * thrust * Time.deltaTime);
            fuel -= 0.5f;
            fuelBar.SetFuel(fuel); 
        }
    }


    public void PressButton()
    {
        if (canPlay) { 
        press = true;
        fire.SetActive(true);
        smoke.Play();
        source.Play();
            
        }
    }

    public void ReleaseButton()
    {
        if (canPlay) { 
        press = false;
        fire.SetActive(false);
        smoke.Stop();
        source.Stop();
        }

    }


    public void StartGame()
    {
        canPlay = true;
        rb.gravityScale = gravity; 

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (velocity > maxSpeedDamage)
        {

            crashed = true;
        }
        _touched = true;
        canPlay = false;
        smoke.Stop();
        source.Stop();

    }

    public void ResetCrashed()
    {
        crashed = false; 
    }

    public bool Touch
    {
        get
        {
            return _touched;
        }
        

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public float speed = 0.1f;
    private Rigidbody2D rb;


    public float iTimer = 3f;
    public float timer = 0f; 

    public GameObject exploEffect; 

    private Vector2 screenBounds; 
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        //rb.velocity = new Vector2(speed, 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        timer = timer + Time.deltaTime;


        if (timer > iTimer)
            {
            Destroy(gameObject);

        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {   
        if(collision.gameObject.tag != "bullet")
        { 
            Destroy(gameObject);
            GameObject b = Instantiate(exploEffect) as GameObject;
            b.transform.position = this.transform.position;
            b.transform.rotation = this.transform.rotation;
            Destroy(b, 0.3f);
        }

    }
}

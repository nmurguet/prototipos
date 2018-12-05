using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {

    public Rigidbody2D rb;
    public float speed;
    public float jumpForce; 

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        rb.velocity = new Vector2(1f * speed, rb.velocity.y);


        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, 1f * jumpForce);


        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.transform.position = new Vector3(-4, 1, 0);
    }
}

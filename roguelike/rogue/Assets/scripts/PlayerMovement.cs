using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Vector2 speedX = new Vector2(1, 0);
    private Vector2 speedY = new Vector2(0, 1);

    public Rigidbody2D rb; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("left"))
        {
            rb.MovePosition(rb.position - speedX);

        }
        if (Input.GetKeyDown("right"))
        {
            rb.MovePosition(rb.position + speedX);

        }
        if (Input.GetKeyDown("up"))
        {
            rb.MovePosition(rb.position + speedY);

        }
        if (Input.GetKeyDown("down"))
        {
            rb.MovePosition(rb.position - speedY);

        }

    }
}

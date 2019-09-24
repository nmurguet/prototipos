using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGravity : MonoBehaviour
{

    public Rigidbody2D rbtargetPlayer;

    public Transform target; 

    public float pullForce;

    public bool pulling; 

    // Start is called before the first frame update
    void Start()
    {
        pulling = false; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pulling) { 
        PullShip();
        }
    }


    void PullShip()

    {
        Vector2 dir = transform.position - target.position;

        rbtargetPlayer.AddForce(dir.normalized * pullForce * Time.deltaTime);
        

    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Player")
        {
            rbtargetPlayer = other.GetComponent<Rigidbody2D>();
            target = other.transform;
            
            pulling = true;
        }

    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {

            rbtargetPlayer = null;
            target = null;
            
            pulling = false;
        }
    }


}

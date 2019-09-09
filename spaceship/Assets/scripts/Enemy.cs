using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private Rigidbody2D erb;

    private Transform target;

    private bool onTarget;

    public float turnForce;
    public float thrustForce;
    public float strafeForce;

    public float maxSpeed;

    public float randomNumber;


    // Start is called before the first frame update
    void Start()
    {
        erb = GetComponent<Rigidbody2D>();
        onTarget = false; 
    }

    // Update is called once per frame
    void Update()
    {
        //maxima velocidad
        erb.velocity = Vector2.ClampMagnitude(erb.velocity, maxSpeed);

        if (!onTarget)
        {
            EnemyPatrol();

        }

        if (onTarget)
        {
           

        }

    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            target = collision.transform;
            onTarget = true;

        }

    }


    void EnemyPatrol()
    {

        erb.AddRelativeForce(Vector2.up * -thrustForce * Time.deltaTime);

        randomNumber = Random.Range(0.5f, 100f);

        if(randomNumber> 50f)
        {

            //erb.AddTorque(-0.3f * turnForce * Time.deltaTime);
            erb.AddTorque(Random.Range(-0.3f,0.3f) * turnForce * Time.deltaTime);
        }




    }
}

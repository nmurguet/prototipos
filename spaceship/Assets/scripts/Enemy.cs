using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private Rigidbody2D erb;

    private Transform target;

    public bool onTarget;

    public float turnForce;
    public float thrustForce;
    public float strafeForce;
   

    public float maxSpeed;

    public float randomNumber;


    public float smooth;
    private float zVelocity = 0f;



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
            ChaseEnemy();

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

       // Accelerate();

        randomNumber = Random.Range(0.5f, 100f);

        if(randomNumber> 50f)
        {
            erb.AddTorque(Random.Range(-0.3f,0.3f) * turnForce * Time.deltaTime);
        }




    }

    void Accelerate()
    {
        erb.AddRelativeForce(Vector2.up * -thrustForce * Time.deltaTime);
    }
    void Brake()
    {
        erb.AddRelativeForce(Vector2.up * thrustForce * Time.deltaTime);
    }


    void ChaseEnemy()
    {

        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float z1 = Mathf.SmoothDampAngle(transform.eulerAngles.z, angle + 90f, ref zVelocity, smooth);
        transform.rotation = Quaternion.Euler(0f, 0f, z1);

   

        if (Vector3.Distance(transform.position, target.position) > 0.5f)
        {
            Accelerate();

        }
        


    }
}

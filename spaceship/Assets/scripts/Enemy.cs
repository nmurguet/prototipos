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
    public float p_setSpeed;

    private float p_maxSpeed; 



    public float randomNumber;


    public float smooth;
    private float zVelocity = 0f;

    public float s_distance; 



    // Start is called before the first frame update
    void Start()
    {
        erb = GetComponent<Rigidbody2D>();
        onTarget = false;
        p_maxSpeed = maxSpeed; 
    }

    // Update is called once per frame
    void Update()
    {

        if (target)
        {

            s_distance = Vector3.Distance(transform.position, target.position);

        }
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
        Accelerate();

        if (Vector3.Distance(transform.position, target.position) >= 10f)
        {
            maxSpeed = maxSpeed + 2.5f * Time.deltaTime;
            if (maxSpeed > p_setSpeed)
            {
                maxSpeed = p_setSpeed;
            }


        }


        if (Vector3.Distance(transform.position, target.position) < 10f)
        {
            
            maxSpeed = maxSpeed - 2.5f * Time.deltaTime;
            if (maxSpeed < p_maxSpeed)
            {
                maxSpeed = p_maxSpeed;
            }



        }



    }
}

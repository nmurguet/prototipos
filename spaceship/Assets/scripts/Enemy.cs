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


    public int hitPoints = 2;


    public GameObject bulletPrefab;
    public GameObject barrel;
    public float bulletSpeed = 5.0f;

    // shooting variables
    public float timeBetweenShots = 0.3333f;  // Allow 3 shots per second
    private float timestamp;


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

        if (hitPoints == 0)
        {

            Destroy(gameObject);
        }

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

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("toco");

        if (collision.gameObject.tag == "bullet")
        {

            hitPoints = hitPoints - 1;

        }
    }


    void EnemyPatrol()
    {

        // Accelerate();

        randomNumber = Random.Range(0.5f, 100f);

        if (randomNumber > 50f)
        {
            erb.AddTorque(Random.Range(-0.3f, 0.3f) * turnForce * Time.deltaTime);
        }




    }


    void Shoot()
    {

        GameObject b = Instantiate(bulletPrefab) as GameObject;
        b.transform.position = barrel.transform.position;
        b.transform.rotation = barrel.transform.rotation;
        Vector2 shootPos = new Vector2(this.transform.position.x - barrel.transform.position.x, this.transform.position.y - barrel.transform.position.y);
        b.GetComponent<Rigidbody2D>().AddForce(shootPos * -bulletSpeed);

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

        if (Time.time >= timestamp)
        {

            Shoot();
            timestamp = Time.time + timeBetweenShots;

        }
    }

}
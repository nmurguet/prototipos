﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    private Rigidbody2D rb;

    public float turnForce;
    public float thrustForce;
    public float strafeForce;

    public GameObject leftThruster;
    public GameObject rightThruster;


    public bool rforceApplied;
    public bool forceApplied;
    public bool leftturnApplied;
    public bool rightturnApplied;
    public bool leftStrafeApplied;
    public bool rightStrafeApplied;

    public GameObject bulletPrefab;
    public GameObject barrel;

    public float bulletSpeed = 5.0f;



    // shooting variables
    public float timeBetweenShots = 0.3333f;  // Allow 3 shots per second
    private float timestamp;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        leftturnApplied = false;
        rightturnApplied = false;
        rforceApplied = false;
        forceApplied = false;
        leftStrafeApplied = false;
        rightStrafeApplied = false;
        leftThruster.SetActive(false);
        rightThruster.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        leftThruster.SetActive(false);
        rightThruster.SetActive(false);

        leftturnApplied = false;
        rightturnApplied = false;
        rforceApplied = false;
        forceApplied = false;
        leftStrafeApplied = false;
        rightStrafeApplied = false;
        if (Input.GetKey(KeyCode.A))
        {
            leftturnApplied = true;


        }
        if (Input.GetKey(KeyCode.D))
        {
            rightturnApplied = true;
        }


        if (Input.GetKey(KeyCode.S))
        {

            rforceApplied = true;


        }
        if (Input.GetKey(KeyCode.W))
        {

            forceApplied = true;
            leftThruster.SetActive(true);
            rightThruster.SetActive(true);


        }

        if (Input.GetKey(KeyCode.E))
        {

            leftStrafeApplied = true; 


        }
        if (Input.GetKey(KeyCode.Q))
        {

            rightStrafeApplied = true; 


        }

        if (Input.GetKey(KeyCode.Space)&& Time.time >= timestamp)
        {

            shootBullet();
            timestamp = Time.time + timeBetweenShots; 


        }
    }

    void FixedUpdate()
    {

        Move();
    }


    void Move()
    {
        if (leftturnApplied)
        {
            rb.AddTorque(0.3f * turnForce * Time.deltaTime);


        }
        if (rightturnApplied)
        {
            rb.AddTorque(-0.3f * turnForce * Time.deltaTime);
        }


        if (rforceApplied)
        {

            rb.AddRelativeForce(Vector2.up * thrustForce * Time.deltaTime);


        }
        if (forceApplied)
        {

            rb.AddRelativeForce(Vector2.up * -thrustForce * Time.deltaTime);


        }

        if (leftStrafeApplied)
        {

            rb.AddRelativeForce(Vector2.right * -strafeForce * Time.deltaTime);


        }
        if (rightStrafeApplied)
        {

            rb.AddRelativeForce(Vector2.right * strafeForce * Time.deltaTime);


        }





    }

    public void shootBullet()
    {
        GameObject b = Instantiate(bulletPrefab) as GameObject;
        b.transform.position = barrel.transform.position;
        b.transform.rotation = barrel.transform.rotation;
        Vector2 shootPos = new Vector2(this.transform.position.x - barrel.transform.position.x, this.transform.position.y - barrel.transform.position.y);
        b.GetComponent<Rigidbody2D>().AddForce(shootPos* -bulletSpeed);




    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ship : MonoBehaviour
{

    public Rigidbody rb;

    public Vector3 direction;

    public Transform t1, t2;

    public float thrustForce;
    public float torqueforce;


    public string level;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if(Input.GetKeyDown(KeyCode.R))
        {
            level = "level";
            SceneManager.LoadScene(level);

        }

        direction = t1.position - t2.position; 


        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(direction * thrustForce * Time.deltaTime);

        }


        if (Input.GetKey(KeyCode.A))
        {
            rb.AddRelativeTorque(Vector3.forward * -torqueforce * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddRelativeTorque(Vector3.forward * torqueforce * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeTorque(Vector3.left * torqueforce * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeTorque(Vector3.left * - torqueforce * Time.deltaTime);
        }


        if (Input.GetKey(KeyCode.E))
        {
            rb.AddRelativeTorque(Vector3.up * torqueforce * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rb.AddRelativeTorque(Vector3.up * -torqueforce * Time.deltaTime);
        }


    }
}

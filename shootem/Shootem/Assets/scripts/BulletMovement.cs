using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{

    public float speed;

    public float timer;
    private float timerCounter;

    

    public Quaternion startingRotation = Quaternion.Euler(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        timerCounter = timer;
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 pos = transform.position;

        Vector3 velocity = new Vector3(0, -speed * Time.deltaTime, 0);
        pos -= startingRotation * velocity;
        transform.position = pos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            Vector2 dir = collision.transform.position - transform.position;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(dir * 200f);
            collision.gameObject.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-200f, 200f));
            Destroy(gameObject);
            collision.gameObject.GetComponent<Enemy>().SetShrink(true);
            collision.gameObject.GetComponent<Collider2D>().enabled = false; 
     


            //timerCounter -= Time.deltaTime;

            //if (timerCounter < 0f)
            //{
            //    Destroy(collision.gameObject);

            //}

        }
    }
}

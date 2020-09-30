using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonTargeting : MonoBehaviour
{


    public Transform target;
    float angle;
    public Vector3 wrld;

    public float direction;

    public Vector2 edge;


    public GameObject bullet;

    private bool action = false;

    public Transform cannon;

    public float speedBullet;


    public GameObject muzzle; 

    


    public float speed = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        wrld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0.0f));
        direction = -1;
        edge = new Vector2(wrld.x, wrld.y);
        muzzle.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        angle = Mathf.Atan2(target.position.x, -target.position.y) * Mathf.Rad2Deg; 
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Movement(); 

        if(Input.GetKeyDown(KeyCode.A))
        {
             StartCoroutine(Shoot());
        }



    }


    public void PressButton()
    {

        StartCoroutine(Shoot());
    }


    private void FixedUpdate()
    {
        
    }

    IEnumerator Shoot()
    {
        muzzle.gameObject.SetActive(true); 
        GameObject firedBullet = Instantiate(bullet, cannon.position, cannon.rotation);
        firedBullet.GetComponent<Rigidbody2D>().velocity = cannon.up * -speedBullet;
        yield return new WaitForSeconds(0.1f);
        muzzle.gameObject.SetActive(false);

        
    }


    void Movement()
    {
        float step = speed * Time.deltaTime;
        target.position = Vector2.MoveTowards(target.position, edge, step);

        if (target.position.y + 1f > wrld.y)
        {
            
            edge = new Vector2(edge.x, edge.y * direction); 
            

        }
        if (target.position.y - 0.2f < -wrld.y)
        {
            edge = new Vector2(edge.x, edge.y * direction);

        }

    }


}

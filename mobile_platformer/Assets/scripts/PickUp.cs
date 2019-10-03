using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{

    public GameObject effect; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="Player")
        {

            Destroy(gameObject);
            GameObject b = Instantiate(effect) as GameObject;
            b.transform.position = this.transform.position;
            b.transform.rotation = this.transform.rotation;
            Destroy(b, 0.3f);

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public GameObject firstPlatform;
    public GameObject secondPlatform;
    public GameObject thirdPlatform;

    public GameObject currentPlatform;
    public GameObject startingPlatform;


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
        
        if (collision.tag == "Platform")
        {
            
            currentPlatform = collision.gameObject.transform.parent.gameObject;
            if(currentPlatform.gameObject.name == firstPlatform.gameObject.name)
            {
                thirdPlatform.gameObject.transform.position = new Vector3(secondPlatform.gameObject.transform.position.x + 75.861f, 0, 0);

            }

            if (currentPlatform.gameObject.name == thirdPlatform.gameObject.name)
            {
                
                firstPlatform.gameObject.transform.position = new Vector3(currentPlatform.gameObject.transform.position.x + 75.861f,0,0);
                secondPlatform.gameObject.transform.position = new Vector3(firstPlatform.gameObject.transform.position.x + 75.861f, 0, 0);
                
            }
            

        }

    }
}

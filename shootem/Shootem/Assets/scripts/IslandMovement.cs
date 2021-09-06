using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandMovement : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 pos = transform.position;

        Vector3 velocity = new Vector3(0, -speed * Time.deltaTime, 0);
        pos += transform.rotation * velocity;
        transform.position = pos;

    }
}

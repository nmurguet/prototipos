using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    public Transform theCamera;
    public float ParallaxFactor = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    

        Vector3 newPos = theCamera.position * ParallaxFactor;                // Calculate the position of the object
        newPos.z = 0;                       // Force Z-axis to zero, since we're in 2D
        transform.position =  newPos;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour
{

    public Transform[] backgrounds; //array of all the back and foreground to be parallax

    private float[] parallaxScales; //proportion of the cameras movement, to move the backgrounds by

    public float smoothing = 1f;        //how smooth the parallax its going to be, make sure to set this above 0

    private Transform cam;          //reference to the main camera transform

    private Vector3 previousCamPos; //store the position of the camera in the previous frame

    //called before start
    void Awake()
    {
        //set up the references of the camera
        cam = Camera.main.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        //the previous frame had the current frame camera position
        previousCamPos = cam.position;

        parallaxScales = new float[backgrounds.Length];

        //asigning corresponding parallax scales
        for(int i = 0;i<backgrounds.Length;i++)
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;

        }


    }

    // Update is called once per frame
    void Update()
    {
        //for each background
        for(int i = 0; i < backgrounds.Length;i++)
        {
            //the parallax sis the opposite of the camera movement 
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];
            //set a target x position wich is the current position plus the parallax
            float backgroundTargetPosX = backgrounds[i].position.x + parallax;

            //create a target position whicj is the backgrounds current position with its target x pos
            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

            //fade between current position and the target pos using lerp

            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);



        }

        //set the previous cam pos to the cameras position at the end of the frame
        previousCamPos = cam.position;

        
    }
}

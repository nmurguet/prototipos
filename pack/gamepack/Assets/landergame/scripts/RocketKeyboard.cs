using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketKeyboard : MonoBehaviour
{

    public RocketManager rm;
    // Start is called before the first frame update
    void Start()
    {
        rm = FindObjectOfType<RocketManager>(); 
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            rm.PressButton(); 
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            rm.ReleaseButton(); 
        }


    }
}
